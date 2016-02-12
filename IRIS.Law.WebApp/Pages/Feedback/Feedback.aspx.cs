using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using System.Net.Mail;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using System.Net;

namespace IRIS.Law.WebApp.Pages.Feedback
{
    public partial class IrisFeedback : BasePage
    {
        LogonReturnValue _logonSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

            Title = string.Format("Feedback to {0}", Solicitors.Branding.Strings.DivisionName);
            this.feedbackTitle.InnerText = string.Format("Feedback to {0}", Solicitors.Branding.Strings.DivisionName);
        }

        protected void _btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                //create the mail message
                var mail = new MailMessage();

                // Why are these the same?
                mail.From = new MailAddress(Solicitors.Branding.Strings.HelpdeskEmail);
                mail.To.Add(Solicitors.Branding.Strings.HelpdeskEmail);

                //set the content
                mail.Subject = "Fee Earner Desktop (FED) Feedback";

                string emailBody = "<p style='font-family:arial;font-size:8pt'><b>The following has been submitted from the Fee Earner Desktop.</b></p><br/>";

                string clientType = "";

                switch (_logonSettings.UserType)
                {
                    case 1:
                        clientType = "Staff";
                        break;
                    case 3:
                        clientType = "Third Party";
                        break;
                    case 2:
                        clientType = "Client";
                        break;
                    default:
                        clientType = "Not Found!";
                        break;
                 }


                emailBody += "<table cellpadding=3 style='font-family:arial;font-size:8pt'>";
                emailBody += "<tr><td><b>User Type: </b></td><td>" + clientType + "</td></tr>";
                emailBody += "<tr><td><b>How would you rate the overall design of the site? </b></td><td>" + _rdbtnListRateOverallSite.SelectedItem.Value + "</td></tr>";
                emailBody += "<tr><td><b>How easy was it to find the information you were looking for? </b></td><td>" + _rdbtnListFindInformation.SelectedItem.Value + "</td></tr>";
                emailBody += "<tr><td><b>How often do you visit the site? </b></td><td>" + _rdbtnListVisitSite.SelectedItem.Value + "</td></tr>";
                emailBody += "<tr><td><b>Do you have any suggestions on how this website can be improved? </b></td><td>" + _txtSuggestions.Text + "</td></tr>";

                emailBody += "</table>";

                mail.Body = emailBody;
                mail.IsBodyHtml = true;



                //send the message
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationSettings.AppSettings["SMTPHost"].ToString();
                smtp.Port = int.Parse(ConfigurationSettings.AppSettings["SMTPPort"].ToString());

                if (ConfigurationSettings.AppSettings["SMTPUserName"].ToString() != string.Empty && ConfigurationSettings.AppSettings["SMTPPassword"].ToString() != string.Empty)
                {
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["SMTPUserName"].ToString(), ConfigurationSettings.AppSettings["SMTPPassword"].ToString());
                    smtp.Credentials = credentials;
                    smtp.UseDefaultCredentials = false;
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }


                if (ConfigurationSettings.AppSettings["IISVersion"].ToString() == "7")
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                else
                    smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

                smtp.Send(mail);


                _lblError.Text = "Feedback Sent!";
                _lblError.CssClass = "successMessage";

                _txtSuggestions.Text = "";
                _rdbtnListFindInformation.SelectedValue = "5";
                _rdbtnListRateOverallSite.SelectedValue = "5";
                _rdbtnListVisitSite.SelectedValue = "5";

            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }

            
        }
    }
}

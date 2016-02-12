using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.UserControls;
using IRIS.Law.WebApp.App_Code;
using System.Configuration;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.MasterPages;
using System.IO;
using System.Threading;

namespace IRIS.Law.WebApp
{
    public partial class Home : BasePage
    {
        LogonReturnValue _logonSettings = null;

        
        
        protected void Page_Load(object sender, EventArgs e)
        {
			_logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
           
            if (_logonSettings.UserType == 2)
            {
                ((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();
            }

            if (!Page.IsPostBack)
            {

                
                try
                {
                    _lblHomePageText.Text = GetText();

                }
                catch (Exception ex)
                {
                    _lblError.Text = ex.Message;
                    _lblError.CssClass = "errorMessage";
                }
            }
        }

        private string GetText()
        {

            string text = "";

            if (!this.Page.IsPostBack)
            {
                try
                {
                    text = File.ReadAllText(Server.MapPath("~/PageTextFiles/Home.txt"));
                    text = string.Format(text, Solicitors.Branding.Strings.ProductName);

                }
                catch (Exception ex)
                {
                    _lblError.Text = ex.Message;
                    _lblError.CssClass = "errorMessage";
                }

            }

            return text;
        }

        
    }
}

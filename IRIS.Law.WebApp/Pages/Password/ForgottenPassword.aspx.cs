using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.Pages.Password
{
    public partial class ForgottenPassword : System.Web.UI.Page
    {
        private LogonServiceClient _logonService = null;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void _btnSend_Click(object sender, EventArgs e)
        {
            if (_txtCaptcha.Text == Session["strRandom"].ToString())
            {
                _lblCaptchaError.Text = "";
                
                _logonService = new LogonServiceClient();
                ReturnValue returnValue;

                returnValue = _logonService.RequestPassword(_txtEmail.Text);

                if (!returnValue.Success)
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = returnValue.Message;
                    return;
                }
                else
                {
                    _lblError.CssClass = "successMessage";
                    _lblError.Text = "The new password will be sent to you by e-mail";
                }

                _txtEmail.Text = "";
                _txtEmailConfirm.Text = "";
                _txtCaptcha.Text = "";
            }
            else
            {
                _lblCaptchaError.Text = "entry is not correct";
            }

        }

        protected void _btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Login.aspx");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.Password
{
    public partial class ChangePassword : BasePage
    {
        private LogonServiceClient _logonService = null;
        LogonReturnValue _logonSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];


            if (!IsPostBack)
            {
                _txtCurrentPassword.Text = "";
                _txtConfirmNewPassword.Text = "";
                _txtNewPassword.Text = "";
            }
        }

        protected void _btnSubmit_Click(object sender, EventArgs e)
        {
            _logonService = new LogonServiceClient();
            ReturnValue returnValue;

            returnValue = _logonService.ChangePassword(_logonSettings.LogonId, Convert.ToString(Session[SessionName.LogonName]), _txtCurrentPassword.Text.Trim(), _txtNewPassword.Text.Trim());

            if (!returnValue.Success)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = returnValue.Message;
                return;
            }
            else
            {
                _lblError.CssClass = "successMessage";
                _lblError.Text = "Password Changed";
            }

           
        }
    }
}

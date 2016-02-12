using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp
{
	public partial class LoginMobile : System.Web.UI.Page
	{
        protected override void OnPreInit(EventArgs e)
        {
            Page.Title = string.Format("Login - {0}", Solicitors.Branding.Strings.ProductName);
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			_lblError.Text = string.Empty;

			if (!IsPostBack)
			{
				if (Request.QueryString["SessionExpired"] != null)
				{
					if (Request.QueryString["SessionExpired"].ToString() == "1")
					{
						_lblError.Text = "Session has expired. Please login again.";
					}
				}
				if (Request.QueryString["AccessDenied"] != null)
				{
					if (Request.QueryString["AccessDenied"].ToString() == "1")
					{
						_lblError.Text = "Access denied.";
					}
				}
				_txtUsername.Focus();
			}
		}

		protected void _btnLogin_Click(object sender, EventArgs e)
		{
			LogonServiceClient _logonService = null;
			try
			{
				_logonService = new LogonServiceClient();
				LogonReturnValue returnValue = _logonService.Logon(_txtUsername.Text.Trim(), _txtPassword.Text.Trim());

				if (!returnValue.Success)
				{
					_lblError.Text = returnValue.Message;
					return;
				}

                if (returnValue.UserType != 1)
                {
                    _lblError.Text = "Access Denied";
                    return;
                }


				Session[SessionName.LogonId] = returnValue.LogonId;
                Session[SessionName.MemberId] = returnValue.MemberId;
                Session[SessionName.DefaultFeeEarner] = returnValue.UserDefaultFeeMemberId;

                Response.Redirect("~/Pages/Time/AddTimeEntryMobile.aspx");
			}
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _logonService = null;
                return;
            }
			catch (Exception ex)
			{
				_lblError.Text = ex.Message;
				_logonService = null;
				return;
			}
			finally
			{
				if (_logonService != null)
				{
                    if (_logonService.State != System.ServiceModel.CommunicationState.Faulted)
					    _logonService.Close();
				}
			}
		}

		protected void _btnReset_Click(object sender, EventArgs e)
		{
			_txtUsername.Text = string.Empty;
			_txtPassword.Text = string.Empty;
		}
	}
}

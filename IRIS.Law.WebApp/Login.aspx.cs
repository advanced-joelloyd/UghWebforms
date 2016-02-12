using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using IRIS.Law.WebApp.Security;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Client;
using NLog;

namespace IRIS.Law.WebApp
{
    public partial class Login : System.Web.UI.Page
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private LogonServiceClient _logonService = null;
        private ClientServiceClient _clientService = null;

        protected override void OnPreInit(EventArgs e)
        {
            Page.Title = string.Format("Login - {0}", Solicitors.Branding.Strings.ProductName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this._btnLogin);

            welcomeMessage.InnerText = string.Format("Welcome to {0}", Solicitors.Branding.Strings.ProductName);

            _lblError.Text = string.Empty;
            if (!IsPostBack)
            {
                if (AutoLogin())
                    return;

                if (Session[SessionName.LogonSettings] != null)
                {
                    Response.Redirect("~/Home.aspx", true);
                    return;
                }

                if (Page.Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-30);
                }
                Session.Abandon();


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
                _btnReset.Attributes.Add("onclick", "javascript:return fnReset();");
                _txtUsername.Focus();


            }
        }

        private bool AutoLogin()
        {
            var username = Request.QueryString["username"];
            var token = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
            {
                var sharedSecret = ConfigurationManager.AppSettings["ILBFEDSharedSecret"];
                if (string.IsNullOrEmpty(sharedSecret))
                {
                    _lblError.Text = "Configuration invalid for automatic login. Please login manually.";
                    return true;
                }

                token = HttpUtility.UrlDecode(token).Replace(' ', '+');
                var tokenParts = token.Split('|');
                var salt = tokenParts[0];
                var ciphertext = tokenParts[1];

                var plaintext = new AesDecryptor(sharedSecret).Decrypt(ciphertext, salt);
                var plaintextParts = plaintext.Split('|');
                var timestamp = DateTime.Parse(plaintextParts[0]);
                var password = plaintextParts[1];

                if (DateTime.Now > timestamp.AddSeconds(30))
                {
                    _lblError.Text = "Automatic login token has timed out. Please login manually.";
                    return true;
                }

                DoLogin(username, password);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates user type specific control accessibility
        /// </summary>
        /// <param name="usertype"></param>
        private void PopulateControlPermissions(int usertype)
        {
            Dictionary<string, bool> objPermissions = new Dictionary<string, bool>();

            objPermissions.Add(SessionName.AddressVisible, true);
            objPermissions.Add(SessionName.AddressReadOnly, false);
            objPermissions.Add(SessionName.ContactVisible, true);
            objPermissions.Add(SessionName.ContactReadOnly, false);
            objPermissions.Add(SessionName.GeneralVisible, true);
            objPermissions.Add(SessionName.GeneralReadOnly, false);
            objPermissions.Add(SessionName.IndividualVisible, true);
            objPermissions.Add(SessionName.IndividualReadOnly, false);
            objPermissions.Add(SessionName.OrgVisible, true);
            objPermissions.Add(SessionName.OrgReadOnly, false);
            objPermissions.Add(SessionName.ClientMatterVisible, true);
            objPermissions.Add(SessionName.ClientMatterReadOnly, false);
            objPermissions.Add(SessionName.MatterDetailsVisible, true);
            objPermissions.Add(SessionName.MatterDetailsReadOnly, false);
            objPermissions.Add(SessionName.AddInfoVisible, true);
            objPermissions.Add(SessionName.AddInfoReadOnly, false);
            objPermissions.Add(SessionName.MatterContactVisible, true);
            objPermissions.Add(SessionName.MatterContactReadOnly, false);
            objPermissions.Add(SessionName.MatterPublicFundingVisible, true);
            objPermissions.Add(SessionName.MatterPublicFundingReadOnly, false);
            objPermissions.Add(SessionName.UploadDocVisible, true);
            objPermissions.Add(SessionName.UploadDocReadOnly, false);
            objPermissions.Add(SessionName.ReUploadDocVisible, true);
            objPermissions.Add(SessionName.ReUploadDocReadOnly, false);
            objPermissions.Add(SessionName.EditDocVisible, true);
            objPermissions.Add(SessionName.EditDocReadOnly, false);
            objPermissions.Add(SessionName.SearchClientVisible, true);
            objPermissions.Add(SessionName.UserDocColIsPublic, true);
            switch (usertype)
            {
                case 2:
                    objPermissions[SessionName.ClientMatterReadOnly] = true;
                    objPermissions[SessionName.GeneralVisible] = false;
                    objPermissions[SessionName.MatterDetailsReadOnly] = true;
                    objPermissions[SessionName.AddInfoVisible] = false;
                    objPermissions[SessionName.AddInfoReadOnly] = true;
                    objPermissions[SessionName.MatterContactVisible] = false;
                    objPermissions[SessionName.MatterContactReadOnly] = true;
                    objPermissions[SessionName.MatterPublicFundingVisible] = false;
                    objPermissions[SessionName.MatterPublicFundingReadOnly] = true;
                    objPermissions[SessionName.EditDocVisible] = false;
                    objPermissions[SessionName.SearchClientVisible] = false;
                    objPermissions[SessionName.UserDocColIsPublic] = false;

                    break;
                case 3:
                    objPermissions[SessionName.AddressReadOnly] = true;
                    objPermissions[SessionName.ClientMatterVisible] = false;
                    objPermissions[SessionName.ClientMatterReadOnly] = true;
                    objPermissions[SessionName.ContactReadOnly] = true;
                    objPermissions[SessionName.GeneralVisible] = false;
                    objPermissions[SessionName.GeneralReadOnly] = true;
                    objPermissions[SessionName.IndividualReadOnly] = true;
                    objPermissions[SessionName.OrgReadOnly] = true;
                    objPermissions[SessionName.MatterDetailsReadOnly] = true;
                    objPermissions[SessionName.AddInfoVisible] = false;
                    objPermissions[SessionName.AddInfoReadOnly] = true;
                    objPermissions[SessionName.MatterContactVisible] = false;
                    objPermissions[SessionName.MatterContactReadOnly] = true;
                    objPermissions[SessionName.MatterPublicFundingVisible] = false;
                    objPermissions[SessionName.MatterPublicFundingReadOnly] = true;
                    objPermissions[SessionName.ReUploadDocVisible] = false;
                    objPermissions[SessionName.EditDocVisible] = false;
                    objPermissions[SessionName.UserDocColIsPublic] = false;
                    break;
            }

            Session[SessionName.ControlSettings] = objPermissions;
        }

        protected void _btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                DoLogin(_txtUsername.Text.Trim(), _txtPassword.Text.Trim());
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                _logger.ErrorException("Endpoint not found", ex);
                _lblError.Text = IRIS.Law.WebApp.App_Code.DataConstants.WSEndPointErrorMessage;
                _logonService = null;
                return;
            }
            catch (TimeoutException ex)
            {
                _logger.ErrorException("Login timeout", ex);
                _lblError.Text = ex.Message;
                _logonService = null;
                return;
            }
            catch (ThreadAbortException) //ignore as thrown by Response.Redirect
            { }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not login", ex);
                _lblError.Text = ex.Message;
                _logonService = null;
                return;
            }
        }

        private void DoLogin(string username, string password)
        {
            // Block MSH from loggin into FED because it is too well known
            // Block rekoop integration user as well as it should only be using web services
            if (username.Trim().ToLower() == "msh" || username.Trim().ToLower() == "rekoop")
            {
                _lblError.Text = "Invalid logon details";
                return;
            }

            _logonService = new LogonServiceClient();
            LogonReturnValue returnValue;

            returnValue = _logonService.Logon(username, password);

            if (!returnValue.Success)
            {
                _lblError.Text = returnValue.Message;
                return;
            }

            Session[SessionName.LogonName] = username;

            Session["LogonID"] = returnValue.LogonId;

            if (null == Session[SessionName.ControlSettings])
                PopulateControlPermissions(returnValue.UserType);

            Session[SessionName.LogonSettings] = returnValue;

            if (!string.IsNullOrEmpty(returnValue.WebStyleSheet))
            {
                // This session is used for all kind of operation being done on CSS files
                Session[SessionName.StyleSheet] = returnValue.WebStyleSheet;

                // This session is used for security purpose
                // for instance, if user change the CSS contents, then on click of preview button of ChangeStyle.aspx screen
                // the new temperory CSS file is created and this CSS is set to Session[SessionName.StyleSheet]
                // and if user wants to cancel the operation, the Session[SessionName.UserStyleSheet] will set to Session[SessionName.StyleSheet]
                Session[SessionName.UserStyleSheet] = returnValue.WebStyleSheet;
            }

            if (returnValue.UserType == 2)
            {
                _clientService = new ClientServiceClient();
                bool isMember = true;
                Guid memOrOrgId = Guid.Empty;

                if (returnValue.MemberId == IRIS.Law.WebApp.App_Code.DataConstants.DummyGuid)
                {
                    memOrOrgId = returnValue.OrganisationId;
                    isMember = false;
                }
                else
                {
                    memOrOrgId = returnValue.MemberId;
                }

                ClientDetailReturnValue _clientReturnValue = _clientService.GetClientDetail(returnValue.LogonId, memOrOrgId,
                                                                                            isMember);

                Session[SessionName.MemberId] = returnValue.MemberId;
                Session[SessionName.OrganisationId] = returnValue.OrganisationId;
                Session[SessionName.ClientRef] = _clientReturnValue.ClientReference;
                Session[SessionName.ClientName] = _clientReturnValue.Name;
            }

            if (returnValue.IsFirstTimeLoggedIn && returnValue.UserType != 1)
            {
                Response.Redirect("~/Pages/Password/ForceChangePassword.aspx", true);
            }

            if (Session["CurrentPage"] != null)
            {
                Response.Redirect(Session["CurrentPage"].ToString());
            }
            else
            {
                Response.Redirect("Home.aspx", true);
            }
        }

        private Guid ParseGuidFromQueryStringOrUseDummyGuid(string name)
        {
            var input = Request.QueryString[name];
            if (string.IsNullOrEmpty(input))
                return IRIS.Law.WebApp.App_Code.DataConstants.DummyGuid;
            Guid guid;
            if (!Guid.TryParse(input, out guid))
                throw new FormatException(name + " is not a valid Guid");
            return guid;
        }
    }
}

using System;
using IRIS.Law.PmsCommonData;
using IRIS.Law.Services.Pms.Security;
using IRISLegal;

namespace IRIS.Law.WebServices
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            if (!SrvSecurityCommon.Logon(IlbCommon.ILBWebServiceSecretLogonUserName, IlbCommon.ILBWebServiceSecretLogonPassword))
                throw new Exception(string.Format("Invalid {0} web service secret logon details", Solicitors.Branding.Strings.ProductName));

            Host.AddSpecialLoggedOnUser(IlbCommon.IlbWebServiceSecretLogonId);

            // Tell the ApplicationSettings we are running as a web service so that ApplicationSettings.Instance works for multiple sessions
            ApplicationSettings.IsWebServices = true;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
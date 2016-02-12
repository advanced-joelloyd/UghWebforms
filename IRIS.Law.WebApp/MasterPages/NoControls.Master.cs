using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.MasterPages
{
    public partial class NoControls : System.Web.UI.MasterPage
    {
        private LogonReturnValue _logonSettings;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            }

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }
    }
}

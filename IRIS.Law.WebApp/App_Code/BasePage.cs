using System;
using System.Web;
using System.Web.UI;
using System.IO;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.App_Code
{
    public partial class BasePage : System.Web.UI.Page
    {
        public BasePage()
        {

        }

        protected override void OnPreInit(EventArgs e)
        {
            if (Session[SessionName.LogonSettings] == null)
            {
                Session.Clear();
                //Session.Abandon();

                Session["CurrentPage"] = Request.Url;

                Response.Redirect("~/Login.aspx?SessionExpired=1", true);

                return;
            }
            else
            {
                if (System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "changestyle.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "viewstyle.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "contactusadministration.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "homepageadministration.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "privacyadministration.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "supportadministration.aspx" ||
                    System.IO.Path.GetFileName(HttpContext.Current.Request.FilePath).ToLower() == "tandcadministration.aspx"


                    )
                {
                    LogonReturnValue _logonSettings;
                    _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                    if (_logonSettings.UserType != (int)DataConstants.UserType.Staff)
                    {
                        Session.Clear();
                        //Session.Abandon();

                        Session["CurrentPage"] = null;

                        Response.Redirect("~/Login.aspx?SessionExpired=1", true);

                        return;
                    }
                }
            }

            base.OnPreInit(e);
        }

        protected override object LoadPageStateFromPersistenceMedium()
        {
            var viewState = Request.Form["__COMPRESSEDVIEWSTATE"];
            if (string.IsNullOrEmpty(viewState))
            {
                return null;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(viewState);
                bytes = ViewStateCompressor.Decompress(bytes);
                var formatter = new LosFormatter();

                return formatter.Deserialize(Convert.ToBase64String(bytes));
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void SavePageStateToPersistenceMedium(object viewState)
        {
            LosFormatter formatter = new LosFormatter();
            StringWriter writer = new StringWriter();
            formatter.Serialize(writer, viewState);
            string viewStateString = writer.ToString();
            byte[] bytes = Convert.FromBase64String(viewStateString);
            bytes = ViewStateCompressor.Compress(bytes);
            ScriptManager.RegisterHiddenField(this, "__COMPRESSEDVIEWSTATE", Convert.ToBase64String(bytes));
        }

    }
}

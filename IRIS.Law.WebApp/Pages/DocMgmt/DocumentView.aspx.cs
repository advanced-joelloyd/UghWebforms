using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.Pages.DocMgmt
{
    public partial class DocumentView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }

            string file = HttpUtility.UrlDecode(Request.QueryString["file"]);
            string path = HttpUtility.UrlDecode(Request.QueryString["path"]);
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(file))
                {
                    // Start Downloading Bytes
                    FileStream fs;
                    string strContentType = string.Empty;
                    fs = File.Open(path, FileMode.Open);
                    byte[] bytBytes = new byte[fs.Length];
                    fs.Read(bytBytes, 0, bytBytes.Length);
                    fs.Close();
                    Response.AddHeader("Content-disposition", "attachment; filename=" + file);
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(bytBytes);
                    Response.End();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using IRIS.Law.WebApp.App_Code;
using System.IO;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.SiteConfig
{
    public partial class Archived : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!((LogonReturnValue)Session[SessionName.LogonSettings]).WebMaster)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);

                return;
            }

            try
            {
                if (!IsPostBack)
                {
                    _lblCustomCSSFolderPath.Value = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/Archived";

                    //Set the page size for the grids
                    _grdViewStyle.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }

        protected void _btnBack_Click(object sender, EventArgs e)
        {

            Response.Redirect("ViewStyle.aspx", true);

        }

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grd_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "unarchive")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string strSourceCSSFile;
                    string strArchiveCSSFile;

                    strSourceCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;
                    strArchiveCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/Archived/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;

                    File.Copy(strArchiveCSSFile, strSourceCSSFile, true);

                    // Delete CSS file
                    File.Delete(strArchiveCSSFile);

                    Response.Redirect("Archived.aspx");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Row Databound
        protected void _grdViewStyle_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                     LinkButton linkArchive = (LinkButton)e.Row.FindControl("_linkArchive");
                     if (linkArchive != null)
                     {
                        {
                            linkArchive.Attributes.Add("onclick", "javascript:return confirm('Are you sure you wish to un-archive this style?')");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion
    }
}

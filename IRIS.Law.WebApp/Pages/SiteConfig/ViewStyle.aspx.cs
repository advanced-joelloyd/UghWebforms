using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using IRIS.Law.WebApp.App_Code;
using System.IO;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace Iris.ILB.Web.Pages.SiteConfig
{
    public partial class ViewStyle : BasePage
    {
        #region Page Load
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
                    _lblCustomCSSFolderPath.Value = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];

                    //Set the page size for the grids
                    _grdViewStyle.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }

        #endregion


        


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
                if (e.CommandName == "edit")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string strCSSFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];
                    if (row.Cells[0].FindControl("_lblCSS") != null)
                    {
                        Session[SessionName.StyleSheet] = strCSSFilePath + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;
                        Session[SessionName.StyleSheetToBeEdited] = Session[SessionName.StyleSheet]; 
                    }
                    Response.Redirect("ChangeStyle.aspx?WizardActiveIndex=0&Edit=1");
                }
                else if (e.CommandName == "preview")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string strCSSFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];
                    if (row.Cells[0].FindControl("_lblCSS") != null)
                    {
                        Session[SessionName.StyleSheet] = strCSSFilePath + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;
                        Session[SessionName.UsedSavedPreview] = strCSSFilePath + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text; 
                    }

                    Response.Redirect("ViewStyle.aspx", true);
                }
                else if (e.CommandName == "selectdefault")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string strSourceCSSFile;
                    string strDefaultCSSFile;

                    if (row.Cells[0].FindControl("_lblCSS") != null)
                    {
                        strSourceCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;
                        strDefaultCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"] + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;

                        File.Copy(strSourceCSSFile, strDefaultCSSFile, true);

                        // Delete other CSS files, as there can be only one default CSS
                        string strDefaultCSSFolderPath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"];
                        strDefaultCSSFolderPath = strDefaultCSSFolderPath.Replace(@"\", "/").Replace(@"\\", "/");
                        string[] files = System.IO.Directory.GetFiles(strDefaultCSSFolderPath, "*.css");
                        for (int i = 0; i < files.Length; i++)
                        {
                            files[i] = files[i].Replace(@"\", "/").Replace(@"\\", "/");
                            if (strSourceCSSFile.Substring(strSourceCSSFile.LastIndexOf("/") + 1).ToLower() != files[i].Substring(files[i].LastIndexOf("/") + 1).ToLower())
                            {
                                if (File.Exists(files[i]) && !files[i].ToUpper().Contains("NIFTYCORNERS.CSS") && !files[i].ToUpper().Contains("MASTER.CSS"))
                                {
                                    File.Delete(files[i]);
                                }
                            }
                        }
                    }
                    Response.Redirect("ViewStyle.aspx");
                }
                else if (e.CommandName == "archive")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    string strSourceCSSFile;
                    string strArchiveCSSFile;
                    
                    strSourceCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;
                    strArchiveCSSFile = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/Archived/" + ((Label)row.Cells[0].FindControl("_lblCSS")).Text;

                    File.Copy(strSourceCSSFile, strArchiveCSSFile, true);

                    // Delete CSS file
                    File.Delete(strSourceCSSFile);
                    if (Session[SessionName.StyleSheet] != null)
                        if (Session[SessionName.StyleSheet].ToString() == strSourceCSSFile)
                            Session[SessionName.StyleSheet] = null;

                    Response.Redirect("ViewStyle.aspx");
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
                    Label lnkFile = (Label)e.Row.FindControl("_lblCSS");
                    Label lblDefault = (Label)e.Row.FindControl("_lblDefault");

                    // Change the color of Row which indicate the current default style
                    string defaultCSSFile = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                    defaultCSSFile = defaultCSSFile.Replace(@"\", "/");
                    defaultCSSFile = defaultCSSFile.Substring(defaultCSSFile.LastIndexOf("/") + 1);
                    if (Convert.ToString(lnkFile.Text) == defaultCSSFile)
                    {
                        e.Row.BackColor = ColorExtender.ColorPicker.StringToColor("Yellow");
                        lblDefault.Visible = true;
                    }
                    else
                    {
                        lblDefault.Visible = false;
                    }

                    LinkButton lnkDefault = (LinkButton)e.Row.FindControl("_lnkDefault");
                    if (lnkDefault != null)
                    {
                        lnkDefault.Attributes.Add("onclick", "javascript:return confirm('Are you sure you wish to make this stylesheet the default style?')");
                    }

                    LinkButton linkCopy = (LinkButton)e.Row.FindControl("_linkCopy");
                    if (linkCopy != null) 
                    {
                        linkCopy.Attributes.Add("onclick", "return showSaveAsModalPopupViaClient('" + lnkFile.Text + "');");
                    }

                    LinkButton linkEdit = (LinkButton)e.Row.FindControl("_linkEdit"); 
                    if (lnkFile.Text.ToUpper() == "IRISLEGAL.CSS")
                    {
                        lblDefault.Text = string.Format(" ({0} Default)",Solicitors.Branding.Strings.DivisionName);
                        lblDefault.Visible = true;
                        if (linkEdit != null)
                        {
                            linkEdit.Visible = false;
                        }
                    }

                    LinkButton linkArchive = (LinkButton)e.Row.FindControl("_linkArchive");
                    if (lnkFile.Text.ToUpper() == "IRISLEGAL.CSS" || Convert.ToString(lnkFile.Text) == defaultCSSFile)
                    {
                        if (linkArchive != null)
                        {
                            linkArchive.Visible = false;
                        }
                    }
                    else
                    {
                        if (linkArchive != null)
                        {
                            linkArchive.Attributes.Add("onclick", "javascript:return confirm('Are you sure you wish to archive this style? Any users attached to this style will revert to the default one.')");
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

        #region CopyFile
        private void CopyFile(String srcFile, String destFile)
        {
            try
            {
                File.Copy(srcFile, destFile, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region OK button click event
        /// <summary>
        /// Save the CSS on OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_txtCSSName.Text))
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Please specify file name where style '" + _hdnCSSFile .Value+ "' will be copied.";
                    _mpeSaveCSS.Show();
                    return;
                }

                string destFilename = _txtCSSName.Text + ".css";
                string srcFilename = _hdnCSSFile.Value;
                string strCSSFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];
                string srcFilePath = strCSSFilePath + "/" + srcFilename;
                strCSSFilePath = strCSSFilePath + "/" + destFilename;

                //Check if already the filename exists
                if (File.Exists(strCSSFilePath))
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "A file with the name you specified already exists. Specify a different file name.";
                    _mpeSaveCSS.Show();
                    return;
                }

                CopyFile(srcFilePath, strCSSFilePath);

                _lblError.Text = "Stylesheet copied successfully.";
                _lblError.CssClass = "successMessage";
                _grdViewStyle.DataSourceID = _odsStyleSheet.ID;
                _mpeSaveCSS.Hide();
            }

            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                _mpeSaveCSS.Show();
            }
        }
        #endregion

        protected void _btnArchived_Click(object sender, EventArgs e)
        {
            
                Response.Redirect("Archived.aspx", true);
           
        }
    }
}

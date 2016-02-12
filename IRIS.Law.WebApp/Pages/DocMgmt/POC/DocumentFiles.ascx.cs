using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebApp.Common;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.IO;
using IRIS.Law.WebApp.DocumentManagement;
using System.Data.SqlClient;
using System.Data; 

namespace IRIS.Law.WebApp.DocumentManagement.POC
{
    public partial class DocumentFiles : System.Web.UI.UserControl
    {
        //List<string> FileUsingVersioning = new List<string>();

        public delegate void MatterItemSelected(object sender, EventArgs e);

        public event MatterItemSelected ItemSelected;
        protected virtual void OnItemSelected(EventArgs e)
        {
            if (ItemSelected != null)
            {
                ItemSelected(this, e);
            }
        }

        public delegate void MatterItemReUpload(object sender, EventArgs e);

        public event MatterItemReUpload ItemReUpload;
        protected virtual void OnItemReUpload(EventArgs e)
        {
            if (ItemReUpload != null)
            {
                ItemReUpload(this, e);
            }
        } 

  
        private bool _readonly;
        public bool ReadOnly
        {
            get { return _readonly; }
            set { _readonly = value; }
        }

        public void Rebind()
        {
            try
            {
                _lblError.Text = "";
                _grdDocFiles.DataSource = null;
                _grdDocFiles.DataBind();

                if (Session[SessionName.ProjectId] == null)
                {

                }
                else
                {
                    //LoadUseVersioningList();
                    _grdDocFiles.DataSourceID = odsMatterDocument.ID;
                }

            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }

        //private void LoadUseVersioningList()
        //{
        //    string Folder = "";
        //    if (Session["SelectedFolder"] != null) { Folder = Session["SelectedFolder"].ToString(); }

        //    WSDocManagement.DocSvc docSvc = new WSDocManagement.DocSvc();

        //    foreach (DataRow _Dr in docSvc.GetFileByMatter(Session[SessionName.ProjectId].ToString(), Folder).Tables[0].Rows)
        //    {
        //        if ((bool)_Dr["DocUseVersioning"]) { FileUsingVersioning.Add(_Dr["DocFileName"].ToString().Trim()); }
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //LoadUseVersioningList();
                _lblError.Text = "";
                _grdDocFiles.DataSourceID = odsMatterDocument.ID;
            }
        }

        private void SetControlAccessibility(bool isReadyOnly)
        {
            if (isReadyOnly)
            {
                _grdDocFiles.Columns[0].Visible = false;
                _grdDocFiles.Columns[1].Visible = false;
            }
            else
            {
                Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];

                _grdDocFiles.Columns[0].Visible = objPerm[SessionName.EditDocVisible];
                _grdDocFiles.Columns[1].Visible = objPerm[SessionName.ReUploadDocVisible];
            }
        }
  

        #region DataSource Selection Event

        protected void odsMatterDocument_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while retrieving matters for the client
            if (e.Exception != null)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = e.Exception.InnerException.Message;
                e.ExceptionHandled = true;
            }
        }

        #endregion
 
        #region Gridview RowDatabound
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdDocFiles_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            { 
                if (e.Row.RowType == DataControlRowType.DataRow)
                { 
                    LinkButton _LnkReload = (LinkButton)e.Row.FindControl("_linkReupload");
                    Label lblDocName = (Label)e.Row.FindControl("_lblDocName");
                    LinkButton lnkFileDesc = (LinkButton)e.Row.FindControl("_linkFileDownload");
                    Label lblDocNotes = (Label)e.Row.FindControl("_lblDocNotes");
 
                    //Truncate doc name descriptions
                    if (lblDocName != null)
                    {
                        if (_LnkReload != null)
                        {
                            //_LnkReload.Visible = (FileUsingVersioning.Contains(lblDocName.Text.Trim()));
                        }
                        if (lblDocName.Text.Length > 30)
                        {
                            lblDocName.Text = lblDocName.Text.Substring(0, 30) + "...";
                        }
                    }

                    //Truncate doc name descriptions
                    if (lnkFileDesc != null)
                    {
                        if (lnkFileDesc.Text.Length > 40)
                        {
                            lnkFileDesc.Text = lnkFileDesc.Text.Substring(0, 40) + "...";
                        }
                    } 

                    //Truncate doc notes descriptions
                    if (lblDocNotes != null)
                    {
                        if (lblDocNotes.Text.Length > 20)
                        {
                            lblDocNotes.Text = lblDocNotes.Text.Substring(0, 20) + "...";
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

        #region GetValueOnIndexFromArray
        /// <summary>
        /// If strAnyValue is DocRecordHiddenValues
        /// index = 0 -> FileExtension
        /// index = 1 -> EmailHasAttachment
        /// 
        /// If strAnyValue is Charge Rate
        /// index = 0 -> ChargeDescriptionID
        /// index = 1 -> CourtId
        /// </summary>
        /// <param name="strAnyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strAnyValue, int index)
        {
            try
            {
                string[] arrayValue = strAnyValue.Split('$');
                return arrayValue[index].Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdDocFiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            { 
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkEdit") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int docId = Convert.ToInt32(_grdDocFiles.DataKeys[rowId].Values["Id"].ToString());

                    Session[SessionName.DocumentId] = docId;
                    //Response.Redirect("~/DocumentManagement/ImportDocument.aspx", true);

                    if (ItemSelected != null)
                    {
                        OnItemSelected(e);
                    }
                }
            }
            else if (e.CommandName == "reupload")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkReupload") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int docId = Convert.ToInt32(_grdDocFiles.DataKeys[rowId].Values["Id"].ToString());

                    Session[SessionName.DocumentId] = docId;

                    if (ItemReUpload != null) { OnItemReUpload(e); }
                    //Response.Redirect("~/DocumentManagement/ReuploadDocument.aspx", true);
                }
            }
            else if (e.CommandName == "fileDownload")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkFileDownload") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int docId = Convert.ToInt32(_grdDocFiles.DataKeys[rowId].Values["Id"].ToString());
                    
                    DocumentServiceClient _documentServiceClient = new DocumentServiceClient();
                    try
                    {
                        Guid ProjectId = DataConstants.DummyGuid;
                        StartDocumentDownloadReturnValue Header;
                        if (!string.IsNullOrEmpty(Convert.ToString(Session[SessionName.ProjectId])))
                        {
                            ProjectId = new Guid(Convert.ToString(Session[SessionName.ProjectId]));
                        }

                        #region Start Download
                        try
                        {
                            Header = _documentServiceClient.StartDocumentDownloadForMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, ProjectId, docId);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        #endregion

                        if (Header != null)
                        {
                            if (!Header.Success)
                            {
                                _lblError.CssClass = "errorMessage";
                                _lblError.Text = Header.Message;
                                return;
                            }

                            if (!Directory.Exists(Path.Combine(Server.MapPath("."), "Download")))
                            {
                                Directory.CreateDirectory(Path.Combine(Server.MapPath("."), "Download"));
                            }

                            long BytesLeft = Header.Size;
                            string downloadFolderPath = Path.Combine(Server.MapPath("."), "Download");
                            string FileName = Path.Combine(downloadFolderPath, Header.FileName);

                            try
                            {
                                #region Download Chunk by Chunk
                                // Open the file
                                using (FileStream FileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                                {
                                    while (BytesLeft > 0)
                                    {
                                        // Download a chunk
                                        DownloadChunkReturnValue Chunk = _documentServiceClient.DocumentDownloadChunk(
                                            ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, Header.TransferId, FileStream.Position);

                                        if (!Chunk.Success)
                                            throw new Exception("File download failed: " + Chunk.Message);

                                        // Write the chunk to the file
                                        FileStream.Write(Chunk.Bytes, 0, Chunk.ChunkSize);

                                        BytesLeft -= Chunk.ChunkSize;
                                    }
                                }
                                #endregion

                                #region Download Complete

                                // Tell the service we have finished downloading
                                _documentServiceClient.DocumentDownloadComplete(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, Header.TransferId);
                                #endregion

                                // Check the file size
                                if (new FileInfo(FileName).Length != Header.Size)
                                    throw new Exception("File download failed, file size is wrong");

                                // Make a hash of the file
                                byte[] LocalHash = FileTransferHash.CreateFileMD5Hash(FileName);

                                // Check the hash matches that from the server
                                if (!FileTransferHash.CheckHash(LocalHash, Header.Hash))
                                {
                                    throw new Exception("File download failed, document checksum does not match");
                                }

                                // Set the file modified date
                                File.SetLastWriteTime(FileName, Header.ModifiedDate);

                                try
                                {
                                    //string jscript = "HideProgress();window.open('DocumentView.aspx?file={0}&path={1}','FileDownload','height=200,width=400,status=no,toolbar=no,menubar=no,left=0,top=0');";
                                    string jscript = "HideProgress(); GetDocument('"+Functions.GetASBaseUrl()+"/DocumentManagement/DocumentView.aspx?file={0}&path={1}');";
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                                                    Guid.NewGuid().ToString(),
                                                    string.Format(jscript, HttpUtility.UrlEncode(new FileInfo(FileName).Name), HttpUtility.UrlEncode(FileName)),
                                                    true);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            catch
                            {
                                // Download failed so delete file being downloaded
                                File.Delete(FileName);
                                throw;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = ex.Message;
                    }
                    finally
                    {
                        if (_documentServiceClient != null)
                        {
                            _documentServiceClient.Close();
                        }
                    }
                
                }
            }
        }
        #endregion

        protected void _grdDocFiles_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected void _grdDocFiles_DataBound(object sender, EventArgs e)
        {
            _LblRowCount.Text = _grdDocFiles.Rows.Count.ToString();
        }
    
    }
}
using System;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;
using System.Web.UI;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class SearchDocumentFiles : System.Web.UI.UserControl
    {

        #region Private Members
        int _matterDocumentRowCount;
        #endregion

        #region SearchForDocumentButtonClientID
        public string SearchForDocumentButtonClientID
        {
            get { return _btnSearchDocuments.ClientID; }
        }
        #endregion

        #region EnableSearchDocument
        public bool EnableSearchDocument
        {
            set
            {
                _btnSearchDocuments.Enabled = value; 
            }
        }
        #endregion

        #region SearchFromFileArrayList
        private string[] _fileArrayList;
        public string[] SearchFromFileArrayList
        {
            set
            {
                _fileArrayList = value;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            //if logonId not present redirect to Login page
            if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }

            //Set the page size for the grids
            _grdDocSearch.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

            if (!IsPostBack)
            {
                _pnlDocSearch.Style["display"] = "none";
                BindDocumentTypes();
            }
            _lblError.CssClass = "errorMessage";
            _lblError.Text = string.Empty;
        }
        #endregion

        #region Bind Document Types

        /// <summary>
        /// Gets the matters for the client.
        /// </summary>
        private void BindDocumentTypes()
        {
            DocumentServiceClient documentTypeService = new DocumentServiceClient();
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                DocumentTypeSearchCriteria docTypeCriteria = new DocumentTypeSearchCriteria();
                docTypeCriteria.DocTypeIDs = "1, 3";
                FileTypeReturnValue fileTypeReturnValue = documentTypeService.GetFileTypes(((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId);

                if (fileTypeReturnValue.Success)
                {
                    if (fileTypeReturnValue.FileType != null)
                    {
                        _ddlFileType.DataSource = fileTypeReturnValue.FileType;
                        _ddlFileType.DataTextField = "FileDescription";
                        _ddlFileType.DataValueField = "FileDescription";
                        _ddlFileType.DataBind();
                    }
                }
                else
                {
                    throw new Exception(fileTypeReturnValue.Message);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
            finally
            {
                if (documentTypeService.State != System.ServiceModel.CommunicationState.Faulted)
                    documentTypeService.Close();
            }

            _ddlFileType.SelectedIndex = -1;
            _ddlFileType.SelectedIndex = _ddlFileType.Items.Count - 1;
        }

        #endregion

        #region Search button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSearchFiles_Click(object sender, EventArgs e)
        {
            try
            {
                _grdDocSearch.DataSourceID = odsSearchForMatterDocument.ID;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Search Documents button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSearchDocuments_Click(object sender, EventArgs e)
        {
            try
            {
                _ddlFileType.SelectedIndex = _ddlFileType.Items.Count;
                _grdDocSearch.DataSourceID = null;
                _grdDocSearch.DataBind();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Gridview Events

        #region RowDataBound
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdDocSearch_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Truncate doc name descriptions
                    LinkButton lnkFileDesc = (LinkButton)e.Row.FindControl("_linkFileDownload");
                    if (lnkFileDesc != null)
                    {
                        if (lnkFileDesc.Text.Length > 40)
                        {
                            lnkFileDesc.Text = lnkFileDesc.Text.Substring(0, 40) + "...";
                        }
                    }

                    //Truncate doc name descriptions
                    //Label lblDocName = (Label)e.Row.FindControl("_lblDocName");
                    //if (lblDocName.Text.Length > 60)
                    //{
                    //    lblDocName.Text = lblDocName.Text.Substring(0, 60) + "...";
                    //}

                    //Truncate doc notes descriptions
                    //Label lblDocPath = (Label)e.Row.FindControl("_lblDocPath");
                    //if (lblDocPath.Text.Length > 60)
                    //{
                    //    lblDocPath.Text = lblDocPath.Text.Substring(0, 60) + "...";
                    //}
                }
            }
            catch(Exception ex)
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
        protected void _grdDocSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "fileDownload")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkFileDownload") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int docId = Convert.ToInt32(_grdDocSearch.DataKeys[rowId].Values["Id"].ToString());
                    
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
                                    string jscript = "HideProgress1(); GetDocument1('../DocMgmt/DocumentView.aspx?file={0}&path={1}');";
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
                    catch (System.ServiceModel.EndpointNotFoundException)
                    {
                        _lblError.Text = DataConstants.WSEndPointErrorMessage;
                        _lblError.CssClass = "errorMessage";
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
                            if (_documentServiceClient.State != System.ServiceModel.CommunicationState.Faulted)
                                _documentServiceClient.Close();
                        }
                    }
                
                }
            }
            _modalpopupDocSearch.Show();
        }
        #endregion

        #endregion

        #region DataSource Events for Gridview

        #region GetSearchForMatterDocuments
        /// <summary>
        /// Gets the documents for the matter.
        /// </summary>
        public DocumentSearchItem[] GetSearchForMatterDocuments(int startRow, int pageSize, string searchText, 
                Int32 fileType, Boolean deepSearch, Boolean matchCase, Boolean searchSubFolders)
        {
            DocumentServiceClient documentService = new DocumentServiceClient();
            DocumentSearchItem[] matterDocs = null;
            
            try
            {
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                DocumentSearchCriteria criteria = new DocumentSearchCriteria();
                criteria.DocumentType = fileType;
                criteria.IsDeepSearch = deepSearch;
                criteria.IsMatchCase = matchCase;
                criteria.IsSubFolderSearch = searchSubFolders;
                criteria.SearchString = searchText;

                if (HttpContext.Current.Session[SessionName.ProjectId] != null)
                {
                    DocumentSearchReturnValue returnValue = documentService.GetMatterDocumentForDeepSearch(logonId, (Guid)HttpContext.Current.Session[SessionName.ProjectId], criteria);
                    if (returnValue != null)
                    {
                        if (returnValue.Success)
                        {
                            if (returnValue.Document != null)
                            {
                                matterDocs = returnValue.Document.Rows;
                                _matterDocumentRowCount = returnValue.Document.Rows.Length;
                            }
                        }
                        else
                        {
                            throw new Exception(returnValue.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("No Project Id found.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (documentService != null)
                {
                    if (documentService.State != System.ServiceModel.CommunicationState.Faulted)
                        documentService.Close();
                }
            }
            
            return matterDocs;
        }
        #endregion

        #region GetMatterDocumentsRowsCount
        public int GetMatterDocumentsRowsCount(string searchText, Int32 fileType, Boolean deepSearch, Boolean matchCase, Boolean searchSubFolders)
        {
            return _matterDocumentRowCount;
        }
        #endregion

        #region DataSource Selection Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsSearchForMatterDocument_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while retrieving matters for the client
            if (e.Exception != null)
            {
                _lblError.CssClass = "errorMessage";
                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblError.Text = DataConstants.WSEndPointErrorMessage;
                else
                    _lblError.Text = e.Exception.InnerException.Message;
                
                e.ExceptionHandled = true;
            }
        }

        #endregion

        #endregion

    }
}
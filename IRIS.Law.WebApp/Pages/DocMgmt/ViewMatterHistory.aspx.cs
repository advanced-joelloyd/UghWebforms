using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Web;

namespace IRIS.Law.WebApp.Pages.DocMgmt
{
    public partial class ViewMatterHistory : BasePage
    {
        #region Private Members
        int _matterDocumentRowCount;
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {

            //if logonId not present redirect to Login page
            if (Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }

            LogonReturnValue _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

            _lblError.CssClass = "errorMessage";
            _lblError.Text = string.Empty;

            //ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(_grdDocFiles);

            if (!IsPostBack)
            {
                if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                {
                    if ((Guid)Session[SessionName.MemberId] != DataConstants.DummyGuid || (Guid)Session[SessionName.OrganisationId] != DataConstants.DummyGuid)
                    {
                        try
                        {
                            if (_logonSettings.CanUploadDocument)
                                _btnImportDocument.Enabled = true;
                            else
                                _btnImportDocument.Enabled = false; 
                      

                            _searchDocFiles.EnableSearchDocument = true;
                            _cliMatDetails.IsClientMember = true;

                            // If Member is Organisation, then set IsClientMember to false
                            if (new Guid(Session[SessionName.OrganisationId].ToString()) != DataConstants.DummyGuid)
                            {
                                _cliMatDetails.IsClientMember = false;
                            }

                            SetControlAccessibility();

                            _grdDocFiles.DataSourceID = odsMatterDocument.ID;
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
                }
            }

            // Assigns CSS for disabled button in Mozilla and  Safari browser
            if (_btnImportDocument.Enabled)
            {                
                _btnImportDocument.CssClass = "button";
            }
            else
            {
                _searchDocFiles.EnableSearchDocument = false;
                _btnImportDocument.CssClass = "buttonDisabled";
            }
        }
        #endregion

        /// <summary>
        /// Sets the visibility according to user type and permissions
        /// </summary>
        private void SetControlAccessibility()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];
            //4
            _grdDocFiles.Columns[0].Visible = objPerm[SessionName.EditDocVisible];
            _grdDocFiles.Columns[1].Visible = objPerm[SessionName.ReUploadDocVisible];
            _grdDocFiles.Columns[4].Visible = objPerm[SessionName.UserDocColIsPublic];
            
        }

        #region DataSource Events for Gridview

        #region GetMatterDocuments
        /// <summary>
        /// Gets the documents for the matter.
        /// </summary>
        public DocumentSearchItem[] GetMatterDocuments(int startRow, int pageSize, string sortBy)
        {
            DocumentServiceClient documentService = new DocumentServiceClient();
            DocumentSearchItem[] matterDocs = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;
                collectionRequest.ForceRefresh = true;

                if (Session[SessionName.ProjectId] != null)
                {
                    DocumentSearchReturnValue returnValue = documentService.MatterDocumentSearch(logonId, (Guid)Session[SessionName.ProjectId], sortBy);
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
                return matterDocs;
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
        }
        #endregion

        #region GetMatterDocumentsRowsCount
        public int GetMatterDocumentsRowsCount()
        {
            return _matterDocumentRowCount;
        }
        #endregion

        #region DataSource Selection Event

        protected void odsMatterDocument_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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

        #region Button Events

        #region ImportDocument Button Load
        protected void _btnImportDocument_Click(object sender, EventArgs e)
        {
            try
            {
                Session[SessionName.DocumentId] = 0;
                Response.Redirect("ImportDocument.aspx", true);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #endregion

        #region Client/Matter Changed Event
        /// <summary>
        /// This will fire after selection of Client from Client Search popup.
        /// This will fire after changing the matter from Matter dropdownlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _cliMatDetails_MatterChanged(object sender, EventArgs e)
        {
            try
            {
                LogonReturnValue _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

               

                _grdDocFiles.DataSource = null;
                _grdDocFiles.DataBind();

                if (Session[SessionName.ProjectId] == null)
                {
                    this._btnImportDocument.Enabled = false;

                    // Assigns CSS for disabled button in Mozilla and Safari browser
                    this._btnImportDocument.CssClass = "buttonDisabled";

                    _searchDocFiles.EnableSearchDocument = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "test1", "$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').attr('disabled', true);$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').addClass('buttonDisabled');", true);

                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblError.Text = _cliMatDetails.Message;
                            return;
                        }
                    }
                }
                else
                {
                    _searchDocFiles.EnableSearchDocument = true;
                    
                    if (_logonSettings.CanUploadDocument)
                        _btnImportDocument.Enabled = true;
                    else
                        _btnImportDocument.Enabled = false;

                    // Assigns CSS for disabled button in Mozilla and  Safari browser
                    if (_btnImportDocument.Enabled)
                    {
                        _btnImportDocument.CssClass = "button";
                    }
                    else
                    {
                        _btnImportDocument.CssClass = "buttonDisabled";
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "test1", "$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').attr('disabled', false);$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').removeClass('buttonDisabled');", true);

                    _grdDocFiles.DataSourceID = odsMatterDocument.ID;
                }

                SetControlAccessibility();
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
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls())
                        {
                            // search for the header link
                            LinkButton lnk = (LinkButton)tc.Controls[0];
                            if (lnk != null)
                            {
                                // inizialize a new image
                                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                // setting the dynamically URL of the image
                                img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdDocFiles.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                // checking if the header link is the user's choice
                                if (_grdDocFiles.SortExpression == lnk.CommandArgument)
                                {
                                    // adding a space and the image to the header link
                                    tc.Controls.Add(new LiteralControl(" "));
                                    tc.Controls.Add(img);
                                }
                            }
                        }
                    }
                }


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
                    Label lblDocName = (Label)e.Row.FindControl("_lblDocName");
                    if (lblDocName != null)
                    {
                        if (lblDocName.Text.Length > 30)
                        {
                            lblDocName.Text = lblDocName.Text.Substring(0, 30) + "...";
                        }
                    }

                    //Truncate doc notes descriptions
                    Label lblDocNotes = (Label)e.Row.FindControl("_lblDocNotes");
                    if (lblDocNotes != null)
                    {
                        if (lblDocNotes.Text.Length > 20)
                        {
                            lblDocNotes.Text = lblDocNotes.Text.Substring(0, 20) + "...";
                        }
                    }
                }
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
                    Response.Redirect("~/Pages/DocMgmt/ImportDocument.aspx", true);
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
                    Response.Redirect("~/Pages/DocMgmt/ReuploadDocument.aspx", true);
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
                                    string jscript = "HideProgress(); GetDocument('DocumentView.aspx?file={0}&path={1}');";
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
        }
        #endregion

        #region Methods
        protected string GetImagePath(object fileName)
        {
            const string DEFAULT_IMAGE = "PNGs/logo_16x16.png";
            string imagePath = DEFAULT_IMAGE;

            try
            {
                if (fileName != null)
                {
                    string imageFileName = fileName.ToString();

                    if (!string.IsNullOrEmpty(imageFileName) && Path.HasExtension(imageFileName))
                    {
                        imagePath = string.Format("{0}s/{1}", Path.GetExtension(imageFileName).Substring(1), imageFileName);
                    }
                }
            }
            catch
            {
                imagePath = DEFAULT_IMAGE;
            }

            return imagePath;
        }
        #endregion Methods
    }
}

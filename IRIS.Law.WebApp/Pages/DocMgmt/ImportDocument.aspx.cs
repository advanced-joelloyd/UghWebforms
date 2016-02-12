using System;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.DocMgmt
{
    public partial class ImportDocument : BasePage
    {
        LogonReturnValue _logonSettings;

        #region Members
        private bool _editMode;
        private string uploadFileErrorMessage = string.Empty;
        private string uploadFileTypes = string.Empty;
        #endregion

        #region UploadFileTypes
        public string UploadFileTypes
        {
            get
            {
                return uploadFileTypes;
            }
        }
        #endregion

        #region UploadFileTypesErrorMessage
        public string UploadFileTypesErrorMessage
        {
            get
            {
                return uploadFileErrorMessage;
            }
        }
        #endregion

        #region GetFileTypesForUpload
        private void GetFileTypesForUpload()
        {
            try
            {
                uploadFileTypes = Convert.ToString(ConfigurationManager.AppSettings["UploadFileTypes"]);

                if (!string.IsNullOrEmpty(uploadFileTypes))
                {
                    string[] arrayFileTypes = uploadFileTypes.Split('|');
                    for (int i = 0; i < arrayFileTypes.Length; i++)
                    {
                        uploadFileErrorMessage += arrayFileTypes[i].Trim() + ", ";
                    }
                    uploadFileErrorMessage = "Only " + uploadFileErrorMessage.Trim().Substring(0,uploadFileErrorMessage.Trim().Length - 1)  + " files are allowed!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            
            _lblError.CssClass = "errorMessage";
            _lblError.Text = string.Empty;
            _editMode = false;

            if (Session[SessionName.DocumentId] != null)
            {
                if (Convert.ToInt32(Session[SessionName.DocumentId]) > 0)
                {
                    _editMode = true;
                }
            }
            GetFileTypesForUpload();

            if (!IsPostBack)
            {
                try
                {
                    //BindDocumentTypes();
                    BindFeeEarner();

                    //_ddlDocType.Attributes.Add("readonly", "readonly");

                    if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                        _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        _chkPublic.Checked = true;
                        _chkPublic.Enabled = false;
                    }

                    if (_editMode == true)
                    {
                        DisplayDocumentDetails();
                        return;
                    }

                    //_txtFileName.Visible = false;
                    _ccCreatedDate.Enabled = false;
                    _fileName.Visible = true;
                    _trFileUpload.Style["display"] = "";
                    //_ddlDocType.Enabled = true;
                    // By Default Document Type should be "Document"
                    //_ddlDocType.Items.FindByValue("3").Selected = true;
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
        #endregion

        #region BindFeeEarner
        private void BindFeeEarner()
        {
            EarnerServiceClient earnerService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                EarnerSearchCriteria criteria = new EarnerSearchCriteria();
                criteria.IncludeArchived = false;
                criteria.MultiOnly = false;

                earnerService = new EarnerServiceClient();
                EarnerSearchReturnValue returnValue = earnerService.EarnerSearch(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    _ddlFeeEarner.Items.Clear();
                    foreach (EarnerSearchItem feeEarner in returnValue.Earners.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(feeEarner.Title, feeEarner.Name, feeEarner.SurName);
                        item.Value = feeEarner.Reference + "$" + feeEarner.Id.ToString();

                        if (!_editMode)
                        {
                            if (((LogonReturnValue)Session[SessionName.LogonSettings]).MemberId == feeEarner.Id)
                            {
                                item.Selected = true;
                            }
                        }

                        _ddlFeeEarner.Items.Add(item);
                    }
                    AddDefaultToDropDownList(_ddlFeeEarner);
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }


        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region DisplayDocumentDetails
        private void DisplayDocumentDetails()
        {
            try
            {
                if (_editMode == true)
                {
                    _fileName.Visible = false;
                    _trFileUpload.Style["display"] = "none";
                    _ccCreatedDate.Enabled = true;

                    // Check User Security settings for LockUnlockDocuments
                    // Which will enable/disable Locked checkbox
                    _chkLockDocument.Enabled = ((LogonReturnValue)Session[SessionName.LogonSettings]).CanUserLockDocument;

                    DocumentServiceClient docService = new DocumentServiceClient();
                    try
                    {
                        DocumentReturnValue docReturnValue = docService.GetDocumentDetails(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, (Guid)Session[SessionName.ProjectId], Convert.ToInt32(Session[SessionName.DocumentId]));

                        if (docReturnValue.Success)
                        {
                            if (docReturnValue.Document != null)
                            {
                                _lblError.Text = string.Empty;

                                _txtDocument.Text = docReturnValue.Document.FileDescription;
                                _ccCreatedDate.DateText = docReturnValue.Document.CreatedDate.ToString("dd/MM/yyyy");
                                _txtNotes.Text = docReturnValue.Document.Notes;

                                _ddlFeeEarner.SelectedIndex = -1;
                                for (int i = 0; i < _ddlFeeEarner.Items.Count; i++)
                                {
                                    if (GetValueOnIndexFromArray(_ddlFeeEarner.Items[i].Value, 1) == docReturnValue.Document.FeeEarnerId.ToString())
                                    {
                                        _ddlFeeEarner.Items[i].Selected = true;
                                    }
                                }

                                _chkEncryptFile.Checked = Convert.ToBoolean(docReturnValue.Document.IsEncrypted);
                                _chkLockDocument.Checked = Convert.ToBoolean(docReturnValue.Document.IsLocked);
                                _chkUseVersioning.Checked = Convert.ToBoolean(docReturnValue.Document.UseVersioning);
                                _chkPublic.Checked = Convert.ToBoolean(docReturnValue.Document.IsPublic);

                                if (docReturnValue.Document.IsEncrypted)
                                {
                                    _chkEncryptFile.Enabled = false;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(docReturnValue.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (docService != null)
                        {
                            if (docService.State != System.ServiceModel.CommunicationState.Faulted)
                                docService.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _btnReset.Enabled = false;
                _btnSave.Enabled = false;
                throw ex;
            }
        }
        #endregion
        
        #region GetValueOnIndexFromArray
        /// <summary>
        /// If strAnyValue is FeeEarnerDetails
        /// index = 0 -> FeeEarnerRef
        /// index = 1 -> FeeEarnerId
        /// </summary>
        /// <param name="strAnyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strAnyValue, int index)
        {
            try
            {
                if (!string.IsNullOrEmpty(strAnyValue))
                {
                    string[] arrayValue = strAnyValue.Split('$');
                    return arrayValue[index].Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CheckFileTypeExtension
        private bool CheckFileTypeExtension()
        {
            try
            {
                string[] fileTypeArray = UploadFileTypes.Split('|');
                bool validFile = false;
                for (var i = 0; i < fileTypeArray.Length; i++)
                {
                    if (_fileName.PostedFile.FileName.ToLower().LastIndexOf(fileTypeArray[i].Trim().ToLower()) > 0)
                    {
                        validFile = true;
                    }
                }

                return validFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Import button click event
        /// <summary>
        /// Import Button Click Event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event arguments</param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            DocumentServiceClient _documentServiceClient = new DocumentServiceClient();
            try
            {
                string fileName = string.Empty;
                if (_editMode == false)
                {
                    #region Upload File to Application Server
                    if (_fileName.PostedFile != null)
                    {
                        if (!string.IsNullOrEmpty(_fileName.PostedFile.FileName))
                        {
                            if (!CheckFileTypeExtension())
                            {
                                _lblError.Text = UploadFileTypesErrorMessage;
                                return;
                            }

                            if (!Directory.Exists(Path.Combine(Server.MapPath("."), "Upload")))
                            {
                                Directory.CreateDirectory(Path.Combine(Server.MapPath("."), "Upload"));
                            }

                            _fileName.PostedFile.SaveAs(Server.MapPath(".") + "/Upload/" + _fileName.PostedFile.FileName.Substring(_fileName.PostedFile.FileName.LastIndexOf("\\") + 1));

                            fileName = Server.MapPath(".") + "/Upload/" + _fileName.PostedFile.FileName.Substring(_fileName.PostedFile.FileName.LastIndexOf("\\") + 1);

                        }
                        else
                        {
                            _lblError.Text = "Please select document.";
                            return;
                        }
                    }
                    #endregion
                }

                #region Load Details
                DocumentSearchItem docDetails = new DocumentSearchItem();
                DocumentReturnValue docReturnValue = null;
                StartDocumentUploadReturnValue Header;

                docDetails.ProjectId = (Guid)Session[SessionName.ProjectId];
                // Document Type is General
                docDetails.TypeId = 1;
                docDetails.FileDescription = _txtDocument.Text;
                docDetails.Notes = _txtNotes.Text;
                if (!string.IsNullOrEmpty(_ddlFeeEarner.SelectedValue))
                {
                    docDetails.FeeEarnerId = new Guid(GetValueOnIndexFromArray(_ddlFeeEarner.SelectedValue, 1));
                }
                else
                {
                    docDetails.FeeEarnerId = DataConstants.DummyGuid;
                }
                docDetails.IsPublic = _chkPublic.Checked;
                docDetails.UseVersioning = _chkUseVersioning.Checked;
                docDetails.IsEncrypted = _chkEncryptFile.Checked;
                docDetails.IsLocked = _chkLockDocument.Checked;
                #endregion

                if (_editMode == false)
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        _lblError.Text = "Please select document.";
                        return;
                    }

                    docDetails.FileName = fileName;
                    docDetails.CreatedDate = DateTime.Now;
                    docDetails.ModifiedDate = docDetails.CreatedDate;
                    docDetails.Id = 0;

                    FileInfo FInfo = new FileInfo(fileName);
                    // Create a hash of the file
                    byte[] LocalHash = FileTransferHash.CreateFileMD5Hash(fileName);

                    #region Start Upload
                    try
                    {
                        Header = _documentServiceClient.StartNewDocumentUploadForMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, 
                            // Strip off the path as this is irrelavent
                        FInfo.LastWriteTime, FInfo.Length, LocalHash, docDetails);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (Header != null)
                    {
                        if (!Header.Success)
                        {
                            _lblError.CssClass = "errorMessage";
                            _lblError.Text = Header.Message;
                            return;
                        }
                    }
                    #endregion

                    #region Upload Chunk by Chunk
                    long BytesLeft = FInfo.Length;

                    // Open the file
                    using (FileStream FileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        while (BytesLeft > 0)
                        {
                            byte[] Bytes = new byte[Header.MaxChunkSize];

                            long Position = FileStream.Position;

                            // Read at most MaxChunkSize bytes
                            int ChunkSize = FileStream.Read(Bytes, 0, (int)Math.Min(BytesLeft, Header.MaxChunkSize));

                            // Upload the chunk
                            ReturnValue Result = _documentServiceClient.DocumentUploadChunk(
                                ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, Header.TransferId, Position, ChunkSize, Bytes);

                            if (!Result.Success)
                                throw new Exception("File upload failed: " + Result.Message);

                            BytesLeft -= ChunkSize;
                        }
                    }
                    #endregion

                    #region Upload Complete
                    // Tell the service we have finished the upload
                    docReturnValue = _documentServiceClient.DocumentUploadComplete(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, Header.TransferId);
                    #endregion

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
                else
                {
                    docDetails.Id = Convert.ToInt32(Session[SessionName.DocumentId]);
                    if (_ccCreatedDate.DateText.Length > 0)
                    {
                        docDetails.CreatedDate = Convert.ToDateTime(_ccCreatedDate.DateText);
                    }

                    try
                    {
                        docReturnValue = _documentServiceClient.EditMatterDocumentDetails(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, docDetails);                            
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

                if (docReturnValue != null)
                {
                    if (docReturnValue.Success)
                    {
                        if (docReturnValue.Document != null)
                        {
                            if (_editMode == false)
                            {
                                Session[SessionName.DocumentId] = docReturnValue.Document.Id;
                                _editMode = true;
                            }

                            DisplayDocumentDetails();

                            _lblError.Text = "Document information saved successfully.";
                            _lblError.CssClass = "successMessage";

                            _btnSave.Enabled = false;
                            _btnImport.Enabled = true;
                        }
                    }
                    else
                    {
                        _lblError.Text = "Document information not saved.<br />" + docReturnValue.Message;
                        _lblError.CssClass = "errorMessage";
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
            finally
            {
                if (_documentServiceClient != null)
                {
                    if (_documentServiceClient.State != System.ServiceModel.CommunicationState.Faulted)
                        _documentServiceClient.Close();
                }
            } 
 
            
        }

        protected void _btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Session[SessionName.DocumentId] = 0;
                _btnSave.Enabled = true;
                Response.Redirect("ImportDocument.aspx", true);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }

        #endregion

        #region Back Button Load
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Session[SessionName.DocumentId] = 0;
                Response.Redirect("ViewMatterHistory.aspx", true);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        
    }
}

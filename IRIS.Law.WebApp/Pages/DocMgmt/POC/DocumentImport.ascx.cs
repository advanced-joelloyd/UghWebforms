using System;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Web.UI.WebControls;
using IRIS.Law.PmsCommonData;
using IRIS.Law.WebApp.Common;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.DocumentManagement;
using System.Data;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class DocumentImport : System.Web.UI.UserControl
    {
        public delegate void MatterItemChanged(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the save button is clicked
        /// </summary>
        public event MatterItemChanged ItemChanged;
        protected virtual void OnItemChanged(EventArgs e)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, e);
            }
        }

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
                    uploadFileErrorMessage = "Only " + uploadFileErrorMessage.Trim().Substring(0, uploadFileErrorMessage.Trim().Length - 1) + " files are allowed!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private void LoadMatterFolders()
        {
            try
            {
                WSDocManagement.DocSvc _docSvc = new WSDocManagement.DocSvc();

                _DdlFolder.Items.Clear();

                _DdlFolder.Items.Add("General");
                ListItem _Li = new ListItem();

                foreach (DataRow _Dr in _docSvc.GetFoldersByMatter(Session[SessionName.ProjectId].ToString().Trim()).Tables[0].Rows)
                {
                    _DdlFolder.Items.Add(_Dr["DocAttributeValue"].ToString().Trim());
                    _DdlFolder.Items[_DdlFolder.Items.Count - 1].Value = _Dr["DocId"].ToString().Trim();
                    if (Session["SelectedFolder"] != null && _Dr["DocAttributeValue"].ToString().Trim() == Session["SelectedFolder"].ToString().Trim())
                    {
                        _Li = _DdlFolder.Items[_DdlFolder.Items.Count - 1];
                    }
                }

                if (_DdlFolder.Items.Count == 1) { _DdlFolder.Enabled = false; }

                if (Session["SelectedFolder"] != null)
                {
                    //_DdlFolder.SelectedItem.Text = Session["SelectedFolder"].ToString().Trim();
                    _DdlFolder.Items[_DdlFolder.Items.IndexOf(_Li)].Selected = true;
                }
            }
            catch (Exception ex) { _lblError.Text = ex.Message; }
        }

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
                if (Request.QueryString["EditDoc"] != null) { _PnlDocumentImportingPopUp.Visible = true; }
                else
                {
                    _txtDocument.Text = string.Empty;
                    _txtNotes.Text = string.Empty;
                    _chkEncryptFile.Checked = false;
                    _chkLockDocument.Checked = false;
                    _chkPublic.Checked = false;
                    _chkUseVersioning.Checked = false;
                    if (_ddlFeeEarner.Items.Count > 0) { _ddlFeeEarner.Items[0].Selected = true; } 
                }

                LoadDetails();
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            DocumentServiceClient _documentServiceClient = new DocumentServiceClient();
            try
            {
                if (Session[SessionName.DocumentId] != null)
                {
                    if (Convert.ToInt32(Session[SessionName.DocumentId]) > 0)
                    {
                        _editMode = true;
                    }
                }

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
                    docDetails.CreatedDate = DateTime.Today;
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

                            if (_DdlFolder.SelectedItem.Text.Trim() != "General")
                            {
                                //docDetails.IsFolder = true;  
                                WSDocManagement.DocSvc _docSvc = new WSDocManagement.DocSvc();

                                if (_editMode)
                                {
                                    _docSvc.SaveToDocAttributes(docDetails.Id, _DdlFolder.SelectedItem.Text, 20);
                                }
                                else
                                {
                                    _docSvc.SaveToDocAttributes(_docSvc.GetNewestDocId(), _DdlFolder.SelectedItem.Text, 20);
                                }
                            }
                        }
                    }
                    else
                    {
                        _lblError.Text = "Document information not saved.<br />" + docReturnValue.Message;
                        _lblError.CssClass = "errorMessage";
                    }
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
            finally
            {
                if (_documentServiceClient != null)
                {
                    _documentServiceClient.Close();
                }
                if (Session[SessionName.DocumentId] != null)
                {
                    Session[SessionName.DocumentId] = null;
                }
                if (ItemChanged != null) { OnItemChanged(e); }
            }
        }
        #endregion

        public void ShowPanel(bool IsVisible, bool RefreshDetails)
        {
            //_PnlDocumentImportingPopUp.Visible = IsVisible;
            //_editMode = true;

            //if (RefreshDetails) { LoadDetails(); }
            //_UpdPnlDocumentImporting.Update();
        }

        private void LoadDetails()
        {
            LoadMatterFolders();

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
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
            
        }

        public void SetButtonEnabled(bool IsEnabled)
        {
            _btnImportDocument.Enabled = IsEnabled;
        }

        protected void _btnImportDocument_Click(object sender, EventArgs e)
        {
            _txtDocument.Text = string.Empty;
            _ccCreatedDate.DateText = string.Empty;
            _txtNotes.Text = string.Empty;

            _PnlDocumentImportingPopUp.Visible = true;
            LoadDetails();
        }

        protected void _btnBack_Click(object sender, EventArgs e)
        {
            Session[SessionName.DocumentId] = null;
            _editMode = false; 
            _PnlDocumentImportingPopUp.Visible = false;

            //if (ItemChanged != null) { OnItemChanged(e); }
        }

        protected void _btnDocumentImport_Click(object sender, EventArgs e)
        {
            Session[SessionName.DocumentId] = 0;
        }
    }
}
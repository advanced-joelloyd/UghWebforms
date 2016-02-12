using System;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.Pages.DocMgmt
{
    public partial class ReuploadDocument : BasePage
    {
        #region Members
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

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            

            _lblErrorReupload.Text = string.Empty;
            _lblErrorReupload.CssClass = "errorMessage";
            bool isDocIdExists = false;

            if (Session[SessionName.DocumentId] != null)
            {
                if (Convert.ToInt32(Session[SessionName.DocumentId]) > 0)
                {
                    isDocIdExists = true;
                }
            }
            GetFileTypesForUpload();

            if (!isDocIdExists)
            {
                _lblErrorReupload.Text = "Please select Reupload option of document from View Matter History.";
                _btnResetReupload.Enabled = false;
                _btnSaveReupload.Enabled = false;
                _fileName.Enabled = false;
                _txtNotes.Enabled = false;
                return;
            }

            if (!IsPostBack)
            {
                try
                {
                    DisplayDetails();
                }
                catch (Exception ex)
                {
                    _lblErrorReupload.Text = ex.Message;
                }
            }
        }
        #endregion

        #region DisplayDetails
        private void DisplayDetails()
        {
            try
            {
                DocumentServiceClient docService = new DocumentServiceClient();
                try
                {
                    DocumentReturnValue docReturnValue = docService.GetDocumentDetails(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, (Guid)Session[SessionName.ProjectId], Convert.ToInt32(Session[SessionName.DocumentId]));

                    if (docReturnValue.Success)
                    {
                        if (docReturnValue.Document != null)
                        {
                            _lblErrorReupload.Text = string.Empty;                            
                            string fileName = docReturnValue.Document.FileName.Substring(docReturnValue.Document.FileName.LastIndexOf(@"\") + 1);
                            _lblFileNameNote.Text = "Note: File should be uploaded with the file name '" + fileName + "'";
                            _hdnDocFileName.Value = fileName;
                            _txtNotes.Text = docReturnValue.Document.Notes;
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
            catch (Exception ex)
            {
                _btnResetReupload.Enabled = false;
                _btnSaveReupload.Enabled = false;
                throw ex ;
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

        #region Re-upload Save button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSaveReupload_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            DocumentServiceClient _documentServiceClient = new DocumentServiceClient();
            try
            {
                DocumentSearchItem docDetails = new DocumentSearchItem();

                #region Upload File to Application Server
                if (_fileName.PostedFile != null)
                {
                    if (!string.IsNullOrEmpty(_fileName.PostedFile.FileName))
                    {
                        if (!CheckFileTypeExtension())
                        {
                            _lblErrorReupload.Text = UploadFileTypesErrorMessage;
                            return;
                        }
                        
                        if (string.IsNullOrEmpty(_hdnDocFileName.Value))
                        {
                            _lblErrorReupload.Text = "Existing file name to compare does not exists.";
                            return;
                        }

                        if (Convert.ToString(_hdnDocFileName.Value) != _fileName.PostedFile.FileName.Substring(_fileName.PostedFile.FileName.LastIndexOf("\\") + 1))
                        {
                            _lblErrorReupload.Text = "File should be uploaded with the file name '" + _hdnDocFileName.Value + "'";
                            return;
                        }

                        if (!Directory.Exists(Path.Combine(Server.MapPath("."), "Upload")))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath("."), "Upload"));
                        }

                        _fileName.PostedFile.SaveAs(Server.MapPath(".") + "/Upload/" + _fileName.PostedFile.FileName.Substring(_fileName.PostedFile.FileName.LastIndexOf("\\") + 1));

                        docDetails.FileName = Server.MapPath(".") + "/Upload/" + _fileName.PostedFile.FileName.Substring(_fileName.PostedFile.FileName.LastIndexOf("\\") + 1);
                    }
                    else
                    {
                        _lblErrorReupload.Text = "Please select document.";
                        return;
                    }
                }
                #endregion

                DocumentReturnValue docReturnValue = null;
                StartDocumentUploadReturnValue Header;

                #region Load Data
                docDetails.ProjectId = (Guid)Session[SessionName.ProjectId];
                // Document Type is General
                docDetails.TypeId = 1;
                docDetails.Id = Convert.ToInt32(Session[SessionName.DocumentId]);
                docDetails.Notes = _txtNotes.Text;
                fileName = docDetails.FileName;
                #endregion

                if (string.IsNullOrEmpty(fileName))
                {
                    _lblErrorReupload.Text = "Please select document.";
                    return;
                }

                FileInfo FInfo = new FileInfo(fileName);
                // Create a hash of the file
                byte[] LocalHash = FileTransferHash.CreateFileMD5Hash(fileName);

                #region Start Upload File
                try
                {
                    Header = _documentServiceClient.StartExistingDocumentUploadForMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, 
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
                        _lblErrorReupload.CssClass = "errorMessage";
                        _lblErrorReupload.Text = Header.Message;
                        return;
                    }
                }
                #endregion

                #region Upload Chunk
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
                // Tell the service we have finished the upload & Save Document Details
                docReturnValue = _documentServiceClient.DocumentUploadComplete(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, Header.TransferId);
                #endregion

                if (docReturnValue != null)
                {
                    if (docReturnValue.Success)
                    {
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }

                        if (docReturnValue.Success)
                        {
                            _lblErrorReupload.Text = "Document information saved successfully.";
                            _lblErrorReupload.CssClass = "successMessage";
                        }
                        else
                        {
                            _lblErrorReupload.Text = docReturnValue.Message;
                            _lblErrorReupload.CssClass = "errorMessage";
                        }
                    }
                    else
                    {
                        _lblErrorReupload.Text = "Document information not saved.<br />" + docReturnValue.Message;
                        _lblErrorReupload.CssClass = "errorMessage";
                    }
                }
            }
            catch (Exception ex)
            {
                _lblErrorReupload.Text = ex.Message;
                _lblErrorReupload.CssClass = "errorMessage";
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
                _lblErrorReupload.Text = ex.Message;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.PmsCommonData;
using IRIS.Law.Services.Dm.Document;
using IRIS.Law.WebServiceInterfaces;
using System.IO;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.Services.Dm.Volume;
using IRIS.Law.DmCommonData;
using System.Data;
using IRIS.Law.Services.Pms.Earner;
using System.Configuration;
using IRIS.Law.PmsCommonServices.CommonServices;
using System.Data.SqlClient;
using IRISLegal;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "DocumentService" here, you must also update the reference to "DocumentService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DocumentService : IDocumentService
    {
        #region IDocumentService

        #region StartDocumentDownloadForMatter
        /// <summary>
        /// Start a document download
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public StartDocumentDownloadReturnValue StartDocumentDownloadForMatter(Guid logonId, Guid projectId,
            int DocumentId)
        {
            StartDocumentDownloadReturnValue ReturnValue = new StartDocumentDownloadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Check permission
                            if (!UserSecuritySettings.GetUserSecuitySettings(9))
                                throw new Exception("You do not have sufficient permissions to carry out this request");
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");

                            // Must check that document belongs to the matter 
                            // (otherwise any docId could be passed and the matter security check above is meaningless)
                            this.CheckDocumentBelongsToMatter(projectId, DocumentId);

                            // if user is client or third party and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(DocumentId);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    string SourceFileName = string.Empty;
                    string downloadedFilePath = string.Empty;
                    string originalFilePath = string.Empty;
                    Common dmCommon = new Common();

                    try
                    {
                        originalFilePath = dmCommon.GetDownloadDocPath(DocumentId);
                        if (!string.IsNullOrEmpty(originalFilePath))
                        {
                            downloadedFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(originalFilePath));
                            File.Copy(originalFilePath, downloadedFilePath, true);

                            // If originalFilePath is of Temp folder then that means the original file which was in encrypted format
                            // is now decrypted and stored in Temp folder.
                            // After copying the original file(decrypted) into the temp folder delete that file(decrypted).
                            string decryptedFilePath = Path.Combine(Path.GetTempPath(), new FileInfo(originalFilePath).Name);
                            if (File.Exists(decryptedFilePath))
                            {
                                File.Delete(decryptedFilePath);
                            }
                        }
                        else
                        {
                            throw new Exception("Download failed while contacting server for file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    SourceFileName = downloadedFilePath;

                    FileDownloadInfo FileInfoData = FileTransfer.StartFileDownload(logonId, SourceFileName, true);

                    ReturnValue.TransferId = FileInfoData.TransferId;
                    //ReturnValue.FileName = FileInfoData.FileName;
                    ReturnValue.FileName = new FileInfo(originalFilePath).Name;
                    ReturnValue.Size = FileInfoData.Size;
                    ReturnValue.ModifiedDate = FileInfoData.ModifiedDate;
                    ReturnValue.Hash = FileInfoData.Hash;

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartDocumentDownloadForMember
        /// <summary>
        /// Start a document download
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public StartDocumentDownloadReturnValue StartDocumentDownloadForMember(Guid logonId, Guid memberId,
            int DocumentId)
        {
            StartDocumentDownloadReturnValue ReturnValue = new StartDocumentDownloadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:

                            if (!ApplicationSettings.Instance.IsUser(memberId, DataConstants.DummyGuid))
                                throw new Exception("Access denied");

                            this.CheckDocumentBelongsToMember(memberId, DocumentId);

                            // if user is client or third party and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(DocumentId);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");

                            break;
                    }

                    string SourceFileName = string.Empty;
                    string downloadedFilePath = string.Empty;
                    string originalFilePath = string.Empty;
                    Common dmCommon = new Common();

                    try
                    {
                        originalFilePath = dmCommon.GetDownloadDocPath(DocumentId);
                        if (!string.IsNullOrEmpty(originalFilePath))
                        {
                            downloadedFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(originalFilePath));
                            if (File.Exists(originalFilePath))
                            {
                                File.Copy(originalFilePath, downloadedFilePath, true);
                            }

                            // If originalFilePath is of Temp folder then that means the original file which was in encrypted format
                            // is now decrypted and stored in Temp folder.
                            // After copying the original file(decrypted) into the temp folder delete that file(decrypted).
                            string decryptedFilePath = Path.Combine(Path.GetTempPath(), new FileInfo(originalFilePath).Name);
                            if (File.Exists(decryptedFilePath))
                            {
                                File.Delete(decryptedFilePath);
                            }
                        }
                        else
                        {
                            throw new Exception("Download failed while contacting server for file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    SourceFileName = downloadedFilePath;

                    FileDownloadInfo FileInfoData = FileTransfer.StartFileDownload(logonId, SourceFileName, true);

                    ReturnValue.TransferId = FileInfoData.TransferId;
                    //ReturnValue.FileName = FileInfoData.FileName;
                    ReturnValue.FileName = new FileInfo(originalFilePath).Name;
                    ReturnValue.Size = FileInfoData.Size;
                    ReturnValue.ModifiedDate = FileInfoData.ModifiedDate;
                    ReturnValue.Hash = FileInfoData.Hash;

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartDocumentDownloadForOrganisation
        /// <summary>
        /// Start a document download
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public StartDocumentDownloadReturnValue StartDocumentDownloadForOrganisation(Guid logonId, Guid organisationId,
            int DocumentId)
        {
            StartDocumentDownloadReturnValue ReturnValue = new StartDocumentDownloadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:

                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisationId))
                                throw new Exception("Access denied");

                            this.CheckDocumentBelongsToOrganisation(organisationId, DocumentId);

                            // if user is client or third party and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(DocumentId);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");

                            break;
                    }

                    string SourceFileName = string.Empty;
                    string downloadedFilePath = string.Empty;
                    string originalFilePath = string.Empty;
                    Common dmCommon = new Common();

                    try
                    {
                        originalFilePath = dmCommon.GetDownloadDocPath(DocumentId);
                        if (!string.IsNullOrEmpty(originalFilePath))
                        {
                            downloadedFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(originalFilePath));
                            if (File.Exists(originalFilePath))
                            {
                                File.Copy(originalFilePath, downloadedFilePath, true);
                            }

                            // If originalFilePath is of Temp folder then that means the original file which was in encrypted format
                            // is now decrypted and stored in Temp folder.
                            // After copying the original file(decrypted) into the temp folder delete that file(decrypted).
                            string decryptedFilePath = Path.Combine(Path.GetTempPath(), new FileInfo(originalFilePath).Name);
                            if (File.Exists(decryptedFilePath))
                            {
                                File.Delete(decryptedFilePath);
                            }
                        }
                        else
                        {
                            throw new Exception("Download failed while contacting server for file.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    SourceFileName = downloadedFilePath;

                    FileDownloadInfo FileInfoData = FileTransfer.StartFileDownload(logonId, SourceFileName, true);

                    ReturnValue.TransferId = FileInfoData.TransferId;
                    //ReturnValue.FileName = FileInfoData.FileName;
                    ReturnValue.FileName = new FileInfo(originalFilePath).Name;
                    ReturnValue.Size = FileInfoData.Size;
                    ReturnValue.ModifiedDate = FileInfoData.ModifiedDate;
                    ReturnValue.Hash = FileInfoData.Hash;

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region DocumentDownloadChunk
        /// <summary>
        /// Download a document chunk
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId">Transfer id obtained when starting the download</param>
        /// <param name="FilePosition">Position in the file to start downloading the chunk</param>
        /// <returns></returns>
        public DownloadChunkReturnValue DocumentDownloadChunk(Guid logonId, Guid TransferId, long FilePosition)
        {
            DownloadChunkReturnValue ReturnValue = new DownloadChunkReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    FileChunkData FileChunkData = FileTransfer.DownloadChunk(logonId, TransferId, FilePosition);

                    ReturnValue.ChunkSize = FileChunkData.ChunkSize;
                    ReturnValue.Bytes = FileChunkData.Bytes;
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region DocumentDownloadComplete
        /// <summary>
        /// Call this to say the download is finished
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId">Transfer id obtained when starting the download</param>
        /// <returns></returns>
        public ReturnValue DocumentDownloadComplete(Guid logonId, Guid TransferId)
        {
            ReturnValue ReturnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    FileTransfer.DownloadComplete(logonId, TransferId);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForMatter
        /// <summary>
        /// Start New Document Upload for the specific matter
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForMatter(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.MemberId = Guid.Empty;
            docSearchItem.OrganisationId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(docSearchItem.ProjectId) || 
                                !UserSecuritySettings.GetUserSecuitySettings(274))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    #region Load Associate Data to DocumentStorageData

                    documentStorageData.ExitingDocument = false;
                    documentStorageData.DocDetails = docSearchItem;

                    SrvDocument srvDoc = new SrvDocument();
                    // If New document is being uploaded, Get the volume location which will help to upload document on documentuploadcomplete
                    srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                    srvDoc.FileName = docSearchItem.FileName;
                    srvDoc.DocumentModuleId = docSearchItem.ProjectId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Project;

                    bool isVoumeCreated = false;
                    Common commonFunction = new Common();
                    isVoumeCreated = commonFunction.GetVolumeLocation(ref srvDoc);

                    if (!isVoumeCreated)
                    {
                        throw new Exception("Upload document failed on the server.");
                    }
                    else
                    {
                        documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                        documentStorageData.VolumeId = srvDoc.VolumeId;
                        documentStorageData.FileName = srvDoc.FileName;
                    }

                    #endregion

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForMember
        /// <summary>
        /// Start New Document Upload for the specific member
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForMember(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.ProjectId = Guid.Empty;
            docSearchItem.OrganisationId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!ApplicationSettings.Instance.IsUser(docSearchItem.MemberId, DataConstants.DummyGuid) || !UserSecuritySettings.GetUserSecuitySettings(274))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    #region Load Associate Data to DocumentStorageData

                    documentStorageData.ExitingDocument = false;
                    documentStorageData.DocDetails = docSearchItem;

                    SrvDocument srvDoc = new SrvDocument();
                    // If New document is being uploaded, Get the volume location which will help to upload document on documentuploadcomplete
                    srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                    srvDoc.FileName = docSearchItem.FileName;
                    srvDoc.DocumentModuleId = docSearchItem.MemberId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Member;

                    bool isVoumeCreated = false;
                    Common commonFunction = new Common();
                    isVoumeCreated = commonFunction.GetVolumeLocation(ref srvDoc);

                    if (!isVoumeCreated)
                    {
                        throw new Exception("Upload document failed on the server.");
                    }
                    else
                    {
                        documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                        documentStorageData.VolumeId = srvDoc.VolumeId;
                        documentStorageData.FileName = srvDoc.FileName;
                    }

                    #endregion

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForOrganisation
        /// <summary>
        /// Start New Document Upload for the specific organisation
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForOrganisation(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.ProjectId = Guid.Empty;
            docSearchItem.MemberId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, docSearchItem.OrganisationId) || !UserSecuritySettings.GetUserSecuitySettings(274))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    #region Load Associate Data to DocumentStorageData

                    documentStorageData.ExitingDocument = false;
                    documentStorageData.DocDetails = docSearchItem;

                    SrvDocument srvDoc = new SrvDocument();
                    // If New document is being uploaded, Get the volume location which will help to upload document on documentuploadcomplete
                    srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                    srvDoc.FileName = docSearchItem.FileName;
                    srvDoc.DocumentModuleId = docSearchItem.OrganisationId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Org;

                    bool isVoumeCreated = false;
                    Common commonFunction = new Common();
                    isVoumeCreated = commonFunction.GetVolumeLocation(ref srvDoc);

                    if (!isVoumeCreated)
                    {
                        throw new Exception("Upload document failed on the server.");
                    }
                    else
                    {
                        documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                        documentStorageData.VolumeId = srvDoc.VolumeId;
                        documentStorageData.FileName = srvDoc.FileName;
                    }

                    #endregion

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForMatter
        /// <summary>
        /// Start existing Document Upload for the specific matter
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForMatter(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.MemberId = Guid.Empty;
            docSearchItem.OrganisationId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            // Can upload/re-upload documents for own matters
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(docSearchItem.ProjectId))
                                throw new Exception("Access denied");

                            // Must check that document belongs to the matter 
                            // (otherwise any docId could be passed and the matter security check above is meaningless)
                            this.CheckDocumentBelongsToMatter(docSearchItem.ProjectId, docSearchItem.Id);

                            // if user is client and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(docSearchItem.Id);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");

                            break;
                        case DataConstants.UserType.ThirdParty:
                            // Not allowed to upload existing documents
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    documentStorageData.DocDetails = docSearchItem;

                    documentStorageData.ExitingDocument = true;

                    // Saving this data is not required here, hence commented out
                    // This information will not be requiring in UploadDocumentComplete
                    // as we are calling Reupload method & this method is handling all type of file handling
                    // like, versioning, encryption etc
                    // Reupload method gets this information using DocumentId
                    // SrvDocument srvDoc = new SrvDocument();
                    //srvDoc.Load(docSearchItem.Id);
                    //documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                    //documentStorageData.VolumeId = 0;
                    //documentStorageData.FileName = srvDoc.FileName;

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForMember
        /// <summary>
        /// Start existing Document Upload for the specific member
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForMember(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.ProjectId = Guid.Empty;
            docSearchItem.OrganisationId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            if (!ApplicationSettings.Instance.IsUser(docSearchItem.MemberId, DataConstants.DummyGuid))
                                throw new Exception("Access denied");

                            this.CheckDocumentBelongsToMember(docSearchItem.MemberId, docSearchItem.Id);

                            // if user is client and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(docSearchItem.Id);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");
                            
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    documentStorageData.DocDetails = docSearchItem;

                    documentStorageData.ExitingDocument = true;

                    // Saving this data is not required here, hence commented out
                    // This information will not be requiring in UploadDocumentComplete
                    // as we are calling Reupload method & this method is handling all type of file handling
                    // like, versioning, encryption etc
                    // Reupload method gets this information using DocumentId
                    // SrvDocument srvDoc = new SrvDocument();
                    //srvDoc.Load(docSearchItem.Id);
                    //documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                    //documentStorageData.VolumeId = 0;
                    //documentStorageData.FileName = srvDoc.FileName;

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForOrganisation
        /// <summary>
        /// Start existing Document Upload for the specific organisation
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForOrganisation(Guid logonId,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            docSearchItem.ProjectId = Guid.Empty;
            docSearchItem.MemberId = Guid.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, docSearchItem.OrganisationId))
                                throw new Exception("Access denied");

                            this.CheckDocumentBelongsToOrganisation(docSearchItem.OrganisationId, docSearchItem.Id);

                            // if user is client and the document is not public, throw an access denied exception
                            SrvDocument srvDoc = new SrvDocument();
                            srvDoc.Load(docSearchItem.Id);

                            if (!srvDoc.IsPublic)
                                throw new Exception("Access denied");
                            
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Check File Type before upload
                    this.CheckFileTypeBeforeUpload(Path.GetFileName(docSearchItem.FileName));

                    DocumentStorageData documentStorageData = Host.GetDocumentStorageData(logonId);

                    documentStorageData.DocDetails = docSearchItem;

                    documentStorageData.ExitingDocument = true;

                    // Saving this data is not required here, hence commented out
                    // This information will not be requiring in UploadDocumentComplete
                    // as we are calling Reupload method & this method is handling all type of file handling
                    // like, versioning, encryption etc
                    // Reupload method gets this information using DocumentId
                    // SrvDocument srvDoc = new SrvDocument();
                    //srvDoc.Load(docSearchItem.Id);
                    //documentStorageData.VolumeLocation = srvDoc.VolumeLocation;
                    //documentStorageData.VolumeId = 0;
                    //documentStorageData.FileName = srvDoc.FileName;

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, Path.GetFileName(docSearchItem.FileName), ModifiedDate, Size, Hash, documentStorageData);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region DocumentUploadChunk
        /// <summary>
        /// Document Upload Chunk
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId"></param>
        /// <param name="FilePosition"></param>
        /// <param name="ChunkSize"></param>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public ReturnValue DocumentUploadChunk(Guid logonId, Guid TransferId,
            long FilePosition, int ChunkSize, byte[] Bytes)
        {
            ReturnValue ReturnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    FileTransfer.UploadChunk(logonId, TransferId, FilePosition, ChunkSize, Bytes);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region DocumentUploadComplete
        /// <summary>
        /// Document Upload Complete
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="TransferId"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public DocumentReturnValue DocumentUploadComplete(Guid logonId, Guid TransferId)
        {
            DocumentReturnValue ReturnValue = new DocumentReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    FileUploadInfo FileUploadInfo = FileTransfer.UploadComplete(logonId, TransferId);
                    DocumentStorageData DocumentStorageData = Host.GetDocumentStorageData(logonId);


                    SrvDocument srvDoc = new SrvDocument();

                    DocumentSearchItem docSearchItem = (DocumentSearchItem)DocumentStorageData.DocDetails;

                    if (!DocumentStorageData.ExitingDocument)
                    {
                        docSearchItem.FileName = FileUploadInfo.TempFileName;

                        string errorMessage = string.Empty;
                        string tempUploadFilePath = string.Empty;
                        string tempFileName = string.Empty;


                        #region File Encryption & Saving Document to its original location
                        try
                        {
                            if (!string.IsNullOrEmpty(DocumentStorageData.VolumeLocation))
                            {
                                //If it wants to be encrypted, encrypt and copy to a temp directory
                                if (docSearchItem.IsEncrypted)
                                {
                                    tempFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(docSearchItem.FileName));

                                    docSearchItem.FileName = tempFileName;
                                }

                                tempUploadFilePath = docSearchItem.FileName;

                                FileInfo file = new FileInfo(docSearchItem.FileName);
                                if (File.Exists(DocumentStorageData.VolumeLocation) == false)
                                {
                                    // Attempt to copy the file.
                                    file.CopyTo(DocumentStorageData.VolumeLocation, true);
                                }

                                // Get New File Name
                                docSearchItem.FileName = DocumentStorageData.FileName;

                                // Encrypt the file and save to the document server
                                if (docSearchItem.IsEncrypted)
                                {
                                    string decryptFileName = DocumentStorageData.VolumeLocation;

                                    DmEncryptionWeb dmEncryption = new DmEncryptionWeb();
                                    dmEncryption.CopyAndEncryptFile(decryptFileName, tempFileName, false);

                                    // Move encrypted File to the original Documenet Server Path
                                    File.Copy(tempFileName, DocumentStorageData.VolumeLocation, true);
                                    File.Delete(tempFileName);
                                }
                            }
                            else
                            {
                                throw new Exception("Upload document failed on the server.");
                            }
                        }
                        finally
                        {
                            if (!string.IsNullOrEmpty(tempUploadFilePath))
                            {
                                // Delete the temp file which was created when uploaded from client
                                if (File.Exists(tempUploadFilePath))
                                {
                                    File.Delete(tempUploadFilePath);
                                }
                            }
                        }
                        #endregion

                        #region Save Document Details
                        if (docSearchItem.ProjectId != Guid.Empty)
                        {
                            srvDoc.DocumentModuleId = docSearchItem.ProjectId;
                            srvDoc.Type = DmEnums.DmPMSDocType.Project;
                        }
                        else if (docSearchItem.MemberId != Guid.Empty)
                        {
                            srvDoc.DocumentModuleId = docSearchItem.MemberId;
                            srvDoc.Type = DmEnums.DmPMSDocType.Member;
                        }
                        else if (docSearchItem.OrganisationId != Guid.Empty)
                        {
                            srvDoc.DocumentModuleId = docSearchItem.OrganisationId;
                            srvDoc.Type = DmEnums.DmPMSDocType.Org;
                        }

                        byte[] bytesToUpdate = new byte[0];
                        srvDoc.BytesToUpdate = bytesToUpdate;

                        // Hardcoded
                        srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                        if (string.IsNullOrEmpty(docSearchItem.FolderName))
                        {
                            srvDoc.FolderName = "Documents";
                        }
                        else
                        {
                            srvDoc.FolderName = docSearchItem.FolderName;
                        }

                        srvDoc.TypeId = docSearchItem.TypeId;
                        srvDoc.Description = docSearchItem.FileDescription;
                        srvDoc.Notes = docSearchItem.Notes;

                        srvDoc.IsUsedVersioning = docSearchItem.UseVersioning;
                        srvDoc.CreationDate = docSearchItem.CreatedDate;
                        srvDoc.IsLocked = docSearchItem.IsLocked;
                        srvDoc.FeeEarnerId = docSearchItem.FeeEarnerId;
                        
                        srvDoc.IsEncrypted = docSearchItem.IsEncrypted;
                        srvDoc.FileName = docSearchItem.FileName;
                        srvDoc.VolumeId = DocumentStorageData.VolumeId;
                        srvDoc.VolumeLocation = DocumentStorageData.VolumeLocation;

                        //Document must become public for Clients and Third Party users
                        if (UserInformation.Instance.UserType == DataConstants.UserType.Client || UserInformation.Instance.UserType == DataConstants.UserType.ThirdParty)
                            srvDoc.IsPublic = true;
                        else
                            srvDoc.IsPublic = docSearchItem.IsPublic;
                        
                        ReturnValue.Success = srvDoc.Save(out errorMessage);

                        if (ReturnValue.Success)
                        {
                            docSearchItem.Id = srvDoc.Id;
                            docSearchItem.FileName = srvDoc.FileName;

                            srvDoc.AddDocumentStatus(srvDoc.Id, DataConstants.DocStatus.ReadyforReview, string.Empty);
                        }
                        #endregion

                        // Ensure the volume location of the saved file is returned
                        docSearchItem.VolumeLocation = DocumentStorageData.VolumeLocation;

                        ReturnValue.Document = docSearchItem;
                    }
                    else
                    {
                        docSearchItem.FileName = FileUploadInfo.TempFileName;
                        ReturnValue = this.DocumentReuploaded(docSearchItem);
                    }

                    FileTransfer.UploadReset(logonId, TransferId);

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }
        #endregion

        #region EditMatterDocumentDetails
        /// <summary>
        /// Save Document to document server and also save details to the database
        /// This method requires to work with actual files, the reason is
        /// If the existing document is not encrypted, and if user updates the document details by setting
        /// Encryption flag to true, then the actual document is to be encrypted.
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public DocumentReturnValue EditMatterDocumentDetails(Guid logonId, DocumentSearchItem docSearchItem)
        {
            DocumentReturnValue returnValue = new DocumentReturnValue();
            string errorMessage = string.Empty;

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Check permission
                            if (!UserSecuritySettings.GetUserSecuitySettings(9))
                                throw new Exception("You do not have sufficient permissions to carry out this request");
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    returnValue.Success = false;

                    SrvDocument srvDoc = new SrvDocument();
                    srvDoc.Id = docSearchItem.Id;
                    // If document details is edited then Load default values in SrvDocument
                    srvDoc.Load(docSearchItem.Id);

                    srvDoc.DocumentModuleId = docSearchItem.ProjectId;
                    byte[] bytesToUpdate = new byte[0];
                    srvDoc.BytesToUpdate = bytesToUpdate;

                    // Hardcoded
                    srvDoc.Type = DmEnums.DmPMSDocType.Project;
                    srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                    srvDoc.FolderName = "Documents";

                    srvDoc.TypeId = docSearchItem.TypeId;
                    srvDoc.Description = docSearchItem.FileDescription;
                    srvDoc.Notes = docSearchItem.Notes;
                    
                    srvDoc.IsPublic=docSearchItem.IsPublic;
                    srvDoc.IsUsedVersioning = docSearchItem.UseVersioning;
                    srvDoc.CreationDate = docSearchItem.CreatedDate;
                    srvDoc.IsLocked = docSearchItem.IsLocked;
                    srvDoc.FeeEarnerId = docSearchItem.FeeEarnerId;

                    // Don't set it while Editing Document
                    srvDoc.VolumeId = 0;

                    #region File Encryption
                    if (!srvDoc.IsEncrypted && docSearchItem.IsEncrypted)
                    {
                        // Encrypt the file and save to the document server
                        if (!string.IsNullOrEmpty(srvDoc.VolumeLocation))
                        {
                            DmEncryptionWeb dmEncryption = new DmEncryptionWeb();
                            string tempDocPath = string.Empty;

                            string savedDocPath = Path.Combine(srvDoc.VolumeLocation, srvDoc.FileName);
                            tempDocPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(srvDoc.FileName));

                            dmEncryption.CopyAndEncryptFile(savedDocPath, tempDocPath, false);

                            File.Copy(tempDocPath, savedDocPath, true);
                            File.Delete(tempDocPath);
                        }
                        else
                        {
                            throw new Exception("Upload document failed on the server.");
                        }
                        srvDoc.IsEncrypted = docSearchItem.IsEncrypted;
                    }
                    #endregion

                    returnValue.Success = srvDoc.Save(out errorMessage);

                    docSearchItem.Id = srvDoc.Id;
                    docSearchItem.FileName = srvDoc.FileName;

                    returnValue.Document = docSearchItem;
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region DocumentReuploaded
        /// <summary>
        /// Reupload document, upload updated document to the original document server location
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        private DocumentReturnValue DocumentReuploaded(DocumentSearchItem docSearchItem)
        {
            DocumentReturnValue returnValue = new DocumentReturnValue();
            string errorMessage = string.Empty;

            try
            {

                SrvDocument srvDoc = new SrvDocument();
                srvDoc.Id = docSearchItem.Id;

                if (docSearchItem.ProjectId != Guid.Empty)
                {
                    srvDoc.DocumentModuleId = docSearchItem.ProjectId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Project;
                }
                else if (docSearchItem.MemberId != Guid.Empty)
                {
                    srvDoc.DocumentModuleId = docSearchItem.MemberId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Member;
                }
                else if (docSearchItem.OrganisationId != Guid.Empty)
                {
                    srvDoc.DocumentModuleId = docSearchItem.OrganisationId;
                    srvDoc.Type = DmEnums.DmPMSDocType.Org;
                }

                // If document details is edited then Load default values in SrvDocument
                srvDoc.Load(docSearchItem.Id);

                // Don't set it while Editing Document
                srvDoc.VolumeId = 0;

                // Hardcoded
                srvDoc.ApplicationId = (int)DataConstants.Application.PMS;
                srvDoc.FolderName = "Documents";

                if (!srvDoc.IsUsedVersioning)
                    throw new Exception("Unable to reupload document as versioning is disabled. You may rename your document and retry.");

                srvDoc.Notes = docSearchItem.Notes;

                if (returnValue.Success)
                {
                    //FileStreamCreator fileStreamCreator = new FileStreamCreator();
                    if (!File.Exists(docSearchItem.FileName))
                    {
                        throw new Exception("Document is not uploaded.");
                    }
                    Common dmCommon = new Common();

                    try
                    {
                        string collection = dmCommon.GetOriginalDocFilePath(docSearchItem.Id);
                        string originalFilePath = GetValueOnIndexFromArray(collection, 0);
                        bool isEncrypted = Convert.ToBoolean(GetValueOnIndexFromArray(collection, 1));
                        bool local = Convert.ToBoolean(GetValueOnIndexFromArray(collection, 2));

                        if (!string.IsNullOrEmpty(originalFilePath))
                        {
                            // Reupload Document
                            // If existing document is in encrypted form, then the upadted doc will also be saved in encrypted format
                            if (dmCommon.ReuploadDoc(docSearchItem.Id, docSearchItem.FileName, isEncrypted, local))
                            {
                                FileInfo file = new FileInfo(docSearchItem.FileName);
                                if (File.Exists(originalFilePath))
                                {
                                    // Attempt to copy the file.
                                    file.CopyTo(originalFilePath, true);
                                }
                                // Save Document Details
                                returnValue.Success = srvDoc.Save(out errorMessage);
                            }
                            else
                            {
                                returnValue.Success = false;
                                returnValue.Message = "Reupload failed.";
                            }
                        }
                        else
                        {
                            returnValue.Success = false;
                            returnValue.Message = "System cannot able to find original file path location to reupload.";
                        }
                    }
                    catch (Exception ex)
                    {
                        returnValue.Success = false;
                        returnValue.Message = ex.Message;
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(docSearchItem.FileName))
                {
                    // Delete the temp file which was created when uploaded from client
                    if (File.Exists(docSearchItem.FileName))
                    {
                        File.Delete(docSearchItem.FileName);
                    }
                }
            }

            return returnValue;
        }
        #endregion

        #region GetValueOnIndexFromArray
        /// <summary>
        /// If strAnyValue is to get collection of values for reupload
        /// index = 0 -> original file path
        /// index = 1 -> isEncrypted
        /// index = 2 -? local
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

        #region GetDocumentDetails
        public DocumentReturnValue GetDocumentDetails(Guid logonId, Guid projectId, int docId)
        {
            DocumentReturnValue returnValue = new DocumentReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");

                            // Must check that document belongs to the matter 
                            // (otherwise any docId could be passed and the matter security check above is meaningless)
                            this.CheckDocumentBelongsToMatter(projectId, docId);

                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DocumentSearchItem docDetails = new DocumentSearchItem();

                    SrvDocument srvDoc = new SrvDocument();
                    srvDoc.Load(docId);

                    docDetails.Id = srvDoc.Id;
                    docDetails.CreatedDate = srvDoc.CreationDate;
                    docDetails.FeeEarnerId = srvDoc.FeeEarnerId;
                    docDetails.FileName = Path.Combine(srvDoc.VolumeLocation, srvDoc.FileName);
                    docDetails.TypeId = srvDoc.TypeId;
                    docDetails.FileDescription = srvDoc.Description;
                    docDetails.Notes = srvDoc.Notes;
                    docDetails.IsPublic = srvDoc.IsPublic;
                    docDetails.UseVersioning = srvDoc.IsUsedVersioning;
                    docDetails.IsEncrypted = srvDoc.IsEncrypted;
                    docDetails.IsLocked = srvDoc.IsLocked;

                    returnValue.Document = docDetails;
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        /// <summary>
        /// Throw an access denied exception if the document does not belong to the matter
        /// </summary>
        /// <returns></returns>
        private void CheckDocumentBelongsToMatter(Guid projectId, int docId)
        {
            DsProjectDocs dsProjDoc = SrvDocumentLookup.GetDocumentProjects(docId);
            if (dsProjDoc.Tables[0].Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dsProjDoc.Tables[0].Rows[0]["ProjectId"].ToString()))
                {
                    throw new Exception("Access denied. No Matter found for this document.");
                }
                else if (new Guid(dsProjDoc.Tables[0].Rows[0]["ProjectId"].ToString()) == DataConstants.DummyGuid)
                {
                    throw new Exception("Access denied. No Matter found for this document.");
                }
                if (new Guid(dsProjDoc.Tables[0].Rows[0]["ProjectId"].ToString()) != projectId)
                {
                    throw new Exception("Access denied.");
                }
            }
        }

        /// <summary>
        /// Throw an access denied exception if the document does not belong to the member
        /// </summary>
        /// <returns></returns>
        private void CheckDocumentBelongsToMember(Guid memberId, int docId)
        {
            DsMemberDocs memberDocs = SrvDocumentLookup.GetMemberDocuments(memberId);

            if (memberDocs.Mem_Map_Docs.Count > 0)
            {
                bool documentFound = false;

                foreach (DsMemberDocs.Mem_Map_DocsRow row in memberDocs.Mem_Map_Docs)
                {
                    if (row.DocID == docId)
                    {
                        documentFound = true;
                        break;
                    }
                }

                if (!documentFound)
                {
                    throw new Exception("Access denied. Document does not belong to member");
                }
            }
            else
            {
                throw new Exception("Access denied. No documents founds for this member");
            }
        }

        /// <summary>
        /// Throw an access denied exception if the document does not belong to the organisation
        /// </summary>
        /// <returns></returns>
        private void CheckDocumentBelongsToOrganisation(Guid organisationId, int docId)
        {
            DsOrgDocs organisationDocs = SrvDocumentLookup.GetOrganisationDocuments(organisationId);

            if (organisationDocs.Org_Map_Docs.Count > 0)
            {
                bool documentFound = false;

                foreach (DsOrgDocs.Org_Map_DocsRow row in organisationDocs.Org_Map_Docs)
                {
                    if (row.DocID == docId)
                    {
                        documentFound = true;
                        break;
                    }
                }

                if (!documentFound)
                {
                    throw new Exception("Access denied. Document does not belong to the organisation");
                }
            }
            else
            {
                throw new Exception("Access denied. No documents founds for this organisation");
            }
        }

        #region GetDocumentTypes
        /// <summary>
        /// Get Document Types for Import Document
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="criteria"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public DocumentTypeReturnValue GetDocumentTypes(Guid logonId, DocumentTypeSearchCriteria criteria, CollectionRequest collectionRequest)
        {
            DocumentTypeReturnValue returnValue = new DocumentTypeReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<DocumentTypeSearchItem> dataListCreator = new DataListCreator<DocumentTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvDocumentLookup.GetDocumentTypes(criteria.DocTypeIDs);
                    };

                    // Create the data list
                    returnValue.DocumentType = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "DocumentTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "DocTypeID"),
                            new ImportMapping("Description", "DocTypeDescription")
                            }
                        );
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region GetFileTypes
        /// <summary>
        /// Get File Types
        /// </summary>
        /// <param name="logonId"></param>
        /// <returns></returns>
        public FileTypeReturnValue GetFileTypes(Guid logonId)
        {
            FileTypeReturnValue returnValue = new FileTypeReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvFileSearcher srvFileSearcher = new SrvFileSearcher();
                    List<FileTypeSearchItem> listFileTypeSearchItem = new List<FileTypeSearchItem>();
                    foreach (FileSearcherFileType fileType in srvFileSearcher.AvilableFileTypes)
                    {
                        FileTypeSearchItem fileTypeSearchItem = new FileTypeSearchItem();
                        fileTypeSearchItem.FileExtension = fileType.FileExtension;
                        fileTypeSearchItem.FileDescription = fileType.FileTypeDescription;
                        listFileTypeSearchItem.Add(fileTypeSearchItem);
                    }
                    returnValue.FileType = listFileTypeSearchItem;
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region GetMatterDocumentForDeepSearch
        /// <summary>
        /// Get documents for deep search
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="fileListToSearch"></param>
        /// <returns></returns>
        public DocumentSearchReturnValue GetMatterDocumentForDeepSearch(Guid logonId, Guid projectId, DocumentSearchCriteria criteria)
        {
            DocumentSearchReturnValue returnValue = new DocumentSearchReturnValue();
            SrvFileSearcher srvFileSearcher = new SrvFileSearcher();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            //Can search for documents
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");

                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataList<DocumentSearchItem> docs = this.MatterDocumentSearchInternal(projectId);

                    string[] listOfDocuments = null;
                    string fileInfo;
                    string searchFilePath;

                    listOfDocuments = new string[docs.Rows.Count];
                    for (int i = 0; i < docs.Rows.Count; i++)
                    {
                        fileInfo = string.Empty;
                        fileInfo = docs.Rows[i].FilePath;
                        if (fileInfo != string.Empty)
                        {
                            fileInfo += "^" + docs.Rows[i].FileDescription + "^" + 
                                docs.Rows[i].Id + "^" + 
                                Convert.ToString(docs.Rows[i].CreatedDate.ToString("dd/MM/yyyy")) + 
                                "^" + Convert.ToString(docs.Rows[i].FeeEarnerRef);
                            listOfDocuments[i] = fileInfo;
                        }
                    }

                    srvFileSearcher.IgnoreCase = !criteria.IsMatchCase;
                    srvFileSearcher.SearchSubFolders = criteria.IsSubFolderSearch;
                    srvFileSearcher.SearchInFiles = criteria.IsDeepSearch;
                    srvFileSearcher.SearchFilesOfType = (SrvFileSearcher.AvailableFileTypes)criteria.DocumentType;

                    if (string.IsNullOrEmpty(criteria.SearchString))
                    {
                        criteria.SearchString = string.Empty;
                    }

                    srvFileSearcher.SearchSetPaths(listOfDocuments, criteria.SearchString);

                    List<FileInfo> filesFound = srvFileSearcher.FilesFound;
                    DataList<DocumentSearchItem> listDocuments = new DataList<DocumentSearchItem>();
                    foreach (FileInfo info in filesFound)
                    {
                        searchFilePath = string.Empty;

                        DocumentSearchItem item = new DocumentSearchItem();
                        item.FileName = info.Name;

                        string fileDescription = SrvDocumentCommon.GetDocumentDescription(info.Name);

                        item.FileDescription = fileDescription;
                        item.FilePath = info.FullName;

                        foreach (string element in listOfDocuments)
                        {
                            searchFilePath = string.Empty;
                            if (element.Contains("^"))
                            {
                                searchFilePath = element.Split('^')[0];
                                if (info.FullName == searchFilePath)
                                {
                                    item.Id = Convert.ToInt32(element.Split('^')[2]);
                                    item.CreatedDate = Convert.ToDateTime(Convert.ToString(element.Split('^')[3]));
                                    item.FeeEarnerRef = Convert.ToString(Convert.ToString(element.Split('^')[4]));
                                }
                            }
                        }

                        listDocuments.Rows.Add(item);
                    }
                    returnValue.Document = listDocuments;
                    returnValue.Message = srvFileSearcher.ErrorMessage;
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = srvFileSearcher.ErrorMessage + "<br />" + ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region MatterDocumentSearch
        /// <summary>
        /// Get List of Documents for Matter
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DocumentSearchReturnValue MatterDocumentSearch(Guid logonId, Guid projectId, string OrderBy)
        {
            DocumentSearchReturnValue returnValue = new DocumentSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");

                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataList<DocumentSearchItem> docSearch = this.MatterDocumentSearchInternal(projectId);

                    if (OrderBy == null)
                        OrderBy = string.Empty;

                    string sortOrder = "ASC";

                    if (OrderBy.Contains("DESC"))
                        sortOrder = "DESC";

                    returnValue.Document = this.SortDocumentSearchResults(docSearch, OrderBy.Replace("DESC", "").Replace("ASC", "").Trim(), sortOrder);

                    //returnValue.Document = MatterDocumentSearchInternal(projectId);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }

        private DataList<DocumentSearchItem> SortDocumentSearchResults(DataList<DocumentSearchItem> data, string column, string Order)
        {

            IEnumerable<DocumentSearchItem> sortAscending;


            switch (column)
            {
                case "FileDescription":
                    if (Order == "DESC")
                        sortAscending = data.Rows.OrderByDescending(p => p.FileDescription).ToList();
                    else
                        sortAscending = data.Rows.OrderBy(p => p.FileDescription).ToList();
                    break;
                case "FeeEarnerRef":
                    if (Order == "DESC")
                        sortAscending = data.Rows.OrderByDescending(p => p.FeeEarnerRef).ToList();
                    else
                        sortAscending = data.Rows.OrderBy(p => p.FeeEarnerRef).ToList();
                    break;
                case "CreatedDateTime":
                    if (Order == "DESC")
                        sortAscending = data.Rows.OrderByDescending(p => p.CreatedDate).ToList();
                    else
                        sortAscending = data.Rows.OrderBy(p => p.CreatedDate).ToList();
                    break;
                case "FileName":
                    if (Order == "DESC")
                        sortAscending = data.Rows.OrderByDescending(p => p.FileName).ToList();
                    else
                        sortAscending = data.Rows.OrderBy(p => p.FileName).ToList();
                    break;
                case "Notes":
                    if (Order == "DESC")
                        sortAscending = data.Rows.OrderByDescending(p => p.Notes).ToList();
                    else
                        sortAscending = data.Rows.OrderBy(p => p.Notes).ToList();
                    break;
                default:
                    sortAscending = data.Rows.OrderBy(p => p.FileDescription).ToList();
                    break;
            }

            DataList<DocumentSearchItem> docSearchOrdered = new DataList<DocumentSearchItem>();
            foreach (DocumentSearchItem item in sortAscending)
            {
                docSearchOrdered.Rows.Add(item);
            }

            return docSearchOrdered;
        }


        private DataList<DocumentSearchItem> MatterDocumentSearchInternal(Guid projectId)
        {
            bool? GetPublicDocs = null;

            switch (UserInformation.Instance.UserType)
            {
                case DataConstants.UserType.Staff:
                    // Can do everything
                    break;
                case DataConstants.UserType.Client:
                case DataConstants.UserType.ThirdParty:
                    GetPublicDocs = true;

                    break;
                default:
                    throw new Exception("Access denied");
            }

            // Get list of Project Documents
            DataList<DocumentSearchItem> docs = new DataList<DocumentSearchItem>();

            DsProjectDocs dsProjectDocs = SrvDocumentLookup.GetProjectDocuments(projectId, GetPublicDocs);

            DataView dvProjectDocs = new DataView(dsProjectDocs.Tables[0]);
            string filter = string.Empty;
            filter = " (DocDescription <> 'PLACEHOLDER_FOLDER')";

            dvProjectDocs.RowFilter = filter;

            dvProjectDocs.Sort = "DocCreated desc";

            #region fill in additional docattributes we need
            //while (idx < dvProjectDocs.Tables[0].Rows.Count)
            foreach (DataRowView drv in dvProjectDocs)
            {
                // Don't display module documents attached to matter
                if (drv.Row["DocDescription"].ToString().Trim() != "PLACEHOLDER_FOLDER")
                {
                    string emailOwner = string.Empty;
	                bool isFound = false;
                    string imgName = string.Empty;
                    //
                    DocumentSearchItem docItem = new DocumentSearchItem();
                    docItem.Id = Convert.ToInt32(drv.Row["DocID"]);
                    docItem.ProjectId = (Guid)drv.Row["ProjectId"];
                    docItem.FileName = drv.Row["DocFileName"].ToString();
                    docItem.FileDescription = drv.Row["DocDescription"].ToString();
                    docItem.Notes = drv.Row["DocNotes"].ToString();
                    docItem.UseVersioning = Convert.ToBoolean(drv.Row["DocUseVersioning"]);
                    docItem.CreatedDate = Convert.ToDateTime(drv.Row["DocCreated"]);
                    docItem.LastModified = Convert.ToDateTime(drv.Row["DocLastModified"]);
                    docItem.IsEncrypted = Convert.ToBoolean(drv.Row["DocIsEncrypted"]);
                    docItem.IsLocked = Convert.ToBoolean(drv.Row["DocIsLocked"]);
                    docItem.CreatedBy = drv.Row["DocCreatedByUserName"].ToString();
                    docItem.IsPublic = Convert.ToBoolean(drv.Row["DocIsPublic"]);

                    #region Get Attributes for each Doc

                    DsDocAttributes dsDocAttr = SrvDocumentLookup.GetDocumentAttributes(docItem.Id);

                    for (int i = 0; i < dsDocAttr.DocAttributes.Count; i++)
                    {
                        if (dsDocAttr.DocAttributes[i].DocAttributeTypeID == 6)
                        {
                            IRIS.Law.PmsCommonData.DsEarnerDetails dsEarnerDetails = SrvEarnerLookup.GetEarnerDetails(new Guid(dsDocAttr.DocAttributes[i].DocAttributeValue));
                            if (dsEarnerDetails.Earners.Count > 0)
                                docItem.FeeEarnerRef = dsEarnerDetails.Earners[0].feeRef;
                        }

                        if (dsDocAttr.DocAttributes[i].DocAttributeTypeID == 22)
                        {
                            emailOwner = dsDocAttr.DocAttributes[i].DocAttributeValue.ToString().Trim();
                        }
                    }
                    #endregion

                    #region Get Document Path for each doc

                    docItem.FilePath = SrvDocumentCommon.GetDocumentPath(docItem.Id);

                    #endregion

                    bool hasVersions = false;

                    if (Convert.ToBoolean(docItem.UseVersioning))
                    {
                        hasVersions = SrvDocumentCommon.CheckIfHasVersions(docItem.Id);
                    }

                    #region Setup the tooltip
                    string toolTip = string.Empty;
                    if (docItem.IsEncrypted)
                    {
                        toolTip += Environment.NewLine + "Document is Encrypted.";
                    }
                    if (docItem.IsLocked)
                    {
                        toolTip += Environment.NewLine + "Document is Locked.";
                    }
                    if (hasVersions)
                    {
                        toolTip += Environment.NewLine + "Document is Versioned.";
                    }
                    if (!string.IsNullOrEmpty(toolTip))
                    {
                        docItem.ToolTip = toolTip + Environment.NewLine;
                    }
                    #endregion

                    // Get Email Attachment
                    #region Get Email Attachment
                    string emailHasAttachment = string.Empty;
                    if (!string.IsNullOrEmpty(emailOwner))
                    {
                        SrvDocument srvDocument = new SrvDocument();
                        srvDocument.Load(docItem.Id);

                        if (srvDocument.EmailHasAttachement)
                        {
                            emailHasAttachment = "Yes";
                        }
                        else
                        {
                            emailHasAttachment = string.Empty;
                        }
                    }
                    #endregion

                    // Set ImageName on document attributes
                    #region Set ImageName on doc Attributes
                    if (hasVersions)
                    {
                        imgName = "documents.png";
                        isFound = true;
                    }

                    if (docItem.IsEncrypted)
                    {
                        imgName = "shield1.png";
                        isFound = true;
                    }

                    if (docItem.IsLocked)
                    {
                        imgName = "lock.png";
                        isFound = true;
                    }
                    #endregion

                    // Get Image file according to the File Extension.
                    #region Get Image File

                    if (isFound == false)
                    {
                        // Default ImageName to PMS.gif
                        imgName = "pms.png";

                        // Get File Extension
                        #region Get File Extension
                        string fileExtension = string.Empty;
                        try
                        {
                            fileExtension = Path.GetExtension(docItem.FileName).ToUpper();
                        }
                        catch
                        {
                        }

                        switch (fileExtension)
                        {
                            case ".DOC":
                            case ".DOCX":
                            case ".RTF":
                                imgName = "word.gif";
                                break;
                            case ".PDF":
                                imgName = "pdf.gif";
                                break;
                            case ".XLS":
                            case ".XLSX":
                            case ".XLSM":
                                imgName = "excel.gif";
                                break;
                            case ".TXT":
                                imgName = "text.gif";
                                break;
                            case ".PPT":
                            case ".PPS":
                                imgName = "powerPoint.gif";
                                break;
                            case ".MSG":
                                if (emailHasAttachment == "Yes")
                                {
                                    imgName = "paperclip.png";
                                }
                                else
                                {
                                    imgName = "mail.png";
                                }
                                break;
                            case ".GIF":
                            case ".JPG":
                            case ".CSV":
                            case ".JIF":
                            case ".PEG":
                            case ".PCT":
                            case ".PSP":
                            case ".PNG":
                            case ".TIF":
                            case ".IFF":
                            case ".BMP":
                            case ".PCX":
                            case ".WMF":
                            case ".TIFF":
                            case ".LFM":
                            case ".XFD":
                            case ".LPW":
                                 imgName = "pms.png";
                                break;
                            default:
                                imgName = "pms.png";
                                break;
                        }
                        #endregion
                    }
                    #endregion

                    docItem.ImgfileName = imgName;
                    docs.Rows.Add(docItem);
                }
            }
            #endregion

            return docs;
        }
        #endregion

        #region CheckFileTypeBeforeUpload
        /// <summary>
        /// Check File Type before Upload
        /// </summary>
        /// <param name="fileName"></param>
        private void CheckFileTypeBeforeUpload(string fileName)
        {
            try
            {
                fileName = fileName.ToLower();

                string uploadFileTypes = Convert.ToString(ConfigurationManager.AppSettings["UploadFileTypes"]);
                string uploadFileErrorMessage = string.Empty;

                if (!string.IsNullOrEmpty(uploadFileTypes))
                {
                    string[] arrayFileTypes = uploadFileTypes.Split('|');
                    for (int i = 0; i < arrayFileTypes.Length; i++)
                    {
                        uploadFileErrorMessage += arrayFileTypes[i].Trim() + ", ";
                    }
                    uploadFileErrorMessage = "Only " + uploadFileErrorMessage.Trim().Substring(0, uploadFileErrorMessage.Trim().Length - 1) + " files are allowed!";
                }

                string[] fileTypeArray = uploadFileTypes.Split('|');
                bool validFile = false;
                for (var i = 0; i < fileTypeArray.Length; i++)
                {
                    if (fileName.LastIndexOf(fileTypeArray[i].Trim()) > 0)
                    {
                        validFile = true;
                    }
                }

                if (!validFile)
                {
                    throw new Exception(uploadFileErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetDocumentFolderList

        public DocumentFolderListReturnValue GetDocumentFolderList(Guid logonId, Guid projectId)
        {
            DataTable dtDocumentFolders = new DataTable();
            DocumentFolderListReturnValue returnValue = new DocumentFolderListReturnValue();
            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    using (SqlConnection cn = new SqlConnection(ApplicationSettings.Instance.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "usp_GetDocumentFolders";
                        cmd.Parameters.Add(new SqlParameter("@ProjectId", projectId));

                        cn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(null,cn))
		                {
                            da.SelectCommand = cmd;
                            da.Fill(dtDocumentFolders);
		                }
	                }
                    returnValue.Document = new List<string>();
                    // Add the Default Folder that's always there but not shown in the ILB database see PmsSolution\Pms\IRIS.Law.PmsCommon\CommonForms\FrmDocDetails.cs
                    returnValue.Document.Add("Documents");
                    foreach (DataRow dr in dtDocumentFolders.Rows)
                    {
                        returnValue.Document.Add(dr[0].ToString());
                    }
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }
            return returnValue;
        }
        #endregion

        #endregion
    }
}

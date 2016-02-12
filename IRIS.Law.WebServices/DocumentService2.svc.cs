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

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "DocumentService" here, you must also update the reference to "DocumentService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DocumentService2 : IDocumentService2
    {
        #region IDocumentService
        /// <summary>
        /// Start a document download
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public StartDocumentDownloadReturnValue StartDocumentDownloadForMatter(Guid logonId, Guid projectId,
            int DocumentId /* TODO Replace this or add extra parameters needed to identify the document required */)
        {
            StartDocumentDownloadReturnValue ReturnValue = new StartDocumentDownloadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (ApplicationSettings.Instance.UserType)
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
                            throw new Exception("Unknown UserType");
                    }

                    // TODO this is just a test file name, you must put the ILB document extraction code here
                    // and set SourceFileName appropriately
                    string SourceFileName = @"c:\work\SetupWebLinkDemo.exe";

                    FileDownloadInfo FileInfoData = FileTransfer.StartFileDownload(logonId, SourceFileName, false);

                    ReturnValue.TransferId = FileInfoData.TransferId;
                    ReturnValue.FileName = FileInfoData.FileName;
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
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

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
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

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
                    FileTransfer.DownloadComplete(logonId, TransferId);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

        public StartDocumentUploadReturnValue StartNewDocumentUploadForMatter(Guid logonId, Guid projectId,
            string FileName, DateTime ModifiedDate, long Size, byte[] Hash
            /* TODO Insert parameters here that ILB requires to store the document */)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (ApplicationSettings.Instance.UserType)
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
                            throw new Exception("Unknown UserType");
                    }

                    DocumentStorageData DocumentStorageData = Host.GetDocumentStorageData(logonId);

                    DocumentStorageData.ExitingDocument = false;
                    DocumentStorageData.ProjectId = projectId;
                    // TODO set other properties on DocumentStorageData so we know everything needed to store the document
                    // when it has finished uploading
                    //DocumentStorageData.othervalue = .....

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, FileName, ModifiedDate, Size, Hash);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

        public StartDocumentUploadReturnValue StartExistingDocumentUploadForMatter(Guid logonId, Guid projectId,
            string FileName, DateTime ModifiedDate, long Size, byte[] Hash
            /* TODO Insert parameters here that ILB requires to store the document */)
        {
            StartDocumentUploadReturnValue ReturnValue = new StartDocumentUploadReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (ApplicationSettings.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Not allowed to upload existing documents
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Unknown UserType");
                    }

                    DocumentStorageData DocumentStorageData = Host.GetDocumentStorageData(logonId);

                    DocumentStorageData.ExitingDocument = true;
                    DocumentStorageData.ProjectId = projectId;
                    // TODO set other properties on DocumentStorageData so we know everything needed to store the document
                    // when it has finished uploading
                    //DocumentStorageData.othervalue = .....

                    ReturnValue.MaxChunkSize = Host.FileTransferChunkSize;

                    ReturnValue.TransferId = FileTransfer.StartFileUpload(logonId, FileName, ModifiedDate, Size, Hash);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

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
                    FileTransfer.UploadChunk(logonId, TransferId, FilePosition, ChunkSize, Bytes);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

        public ReturnValue DocumentUploadComplete(Guid logonId, Guid TransferId)
        {
            ReturnValue ReturnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    FileUploadInfo FileUploadInfo = FileTransfer.UploadComplete(logonId, TransferId);

                    DocumentStorageData DocumentStorageData = Host.GetDocumentStorageData(logonId);

                    // DocumentStorageData holds all the information needed to know how to store the document

                    // TODO this is just test code.  You should replace this with code to store the file
                    // in the ILB document management system.
                    // FileUploadInfo.TempFileName holds the full path to the actual file that has been uploaded
                    // FileUploadInfo.FileName holds the original file name (excluding path) of the file that has been uploaded
                    // The file will need moving to the correct location in the document management system.
                    File.Move(FileUploadInfo.TempFileName, @"c:\Work\Uploaded\" + FileUploadInfo.FileName);

                    FileTransfer.UploadReset(logonId, TransferId);
                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception ex)
            {
                ReturnValue.Success = false;
                ReturnValue.Message = ex.Message;
            }

            return ReturnValue;
        }

        #endregion
    }
}

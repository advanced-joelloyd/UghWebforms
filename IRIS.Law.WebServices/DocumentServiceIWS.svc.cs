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
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Document;
//using IRIS.Law.Services;
using IRIS.Law.Services.Pms.Dictation;
using IRIS.Law.WebServices;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "DocumentService" here, you must also update the reference to "DocumentService" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.DocumentServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_DocumentServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DocumentServiceIWS : IDocumentServiceIWS
    {
        #region IDocumentService
        DocumentService oDocumentService;
        #region StartDocumentDownloadForMatter
        /// <summary>
        /// Start a document download
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public StartDocumentDownloadReturnValue StartDocumentDownloadForMatter(HostSecurityToken oHostSecurityToken, Guid projectId,
            int DocumentId)
        {
            StartDocumentDownloadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartDocumentDownloadForMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId, DocumentId);
            }
            else
            {
                ReturnValue = new StartDocumentDownloadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region DocumentDownloadChunk
        /// <summary>
        /// Download a document chunk
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="TransferId">Transfer id obtained when starting the download</param>
        /// <param name="FilePosition">Position in the file to start downloading the chunk</param>
        /// <returns></returns>
        public DownloadChunkReturnValue DocumentDownloadChunk(HostSecurityToken oHostSecurityToken, Guid TransferId, long FilePosition)
        {
            DownloadChunkReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.DocumentDownloadChunk(Functions.GetLogonIdFromToken(oHostSecurityToken), TransferId, FilePosition);
            }
            else
            {
                ReturnValue = new DownloadChunkReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region DocumentDownloadComplete
        /// <summary>
        /// Call this to say the download is finished
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="TransferId">Transfer id obtained when starting the download</param>
        /// <returns></returns>
        public ReturnValue DocumentDownloadComplete(HostSecurityToken oHostSecurityToken, Guid TransferId)
        {
            ReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.DocumentDownloadComplete(Functions.GetLogonIdFromToken(oHostSecurityToken), TransferId);
            }
            else
            {
                ReturnValue = new ReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForMatter
        /// <summary>
        /// Start New Document Upload for the specific matter
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForMatter(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartNewDocumentUploadForMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForMember
        /// <summary>
        /// Start New Document Upload for the specific Member
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForMember(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartNewDocumentUploadForMember(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartNewDocumentUploadForOrganisation
        /// <summary>
        /// Start New Document Upload for the specific Organisation
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartNewDocumentUploadForOrganisation(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartNewDocumentUploadForOrganisation(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForMatter
        /// <summary>
        /// Start existing Document Upload for the specific matter
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForMatter(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartExistingDocumentUploadForMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForMember
        /// <summary>
        /// Start existing Document Upload for the specific Member
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForMember(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartExistingDocumentUploadForMember(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region StartExistingDocumentUploadForOrganisation
        /// <summary>
        /// Start existing Document Upload for the specific Organisation
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <param name="FileName"></param>
        /// <param name="ModifiedDate"></param>
        /// <param name="Size"></param>
        /// <param name="Hash"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public StartDocumentUploadReturnValue StartExistingDocumentUploadForOrganisation(HostSecurityToken oHostSecurityToken,
            DateTime ModifiedDate, long Size, byte[] Hash, DocumentSearchItem docSearchItem)
        {
            StartDocumentUploadReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.StartExistingDocumentUploadForOrganisation(Functions.GetLogonIdFromToken(oHostSecurityToken), ModifiedDate, Size, Hash, docSearchItem);
            }
            else
            {
                ReturnValue = new StartDocumentUploadReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region DocumentUploadChunk
        /// <summary>
        /// Document Upload Chunk
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="TransferId"></param>
        /// <param name="FilePosition"></param>
        /// <param name="ChunkSize"></param>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public ReturnValue DocumentUploadChunk(HostSecurityToken oHostSecurityToken, Guid TransferId,
            long FilePosition, int ChunkSize, byte[] Bytes)
        {
            ReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.DocumentUploadChunk(Functions.GetLogonIdFromToken(oHostSecurityToken), TransferId, FilePosition, ChunkSize, Bytes);
            }
            else
            {
                ReturnValue = new ReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region DocumentUploadComplete
        /// <summary>
        /// Document Upload Complete
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="TransferId"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public DocumentReturnValue DocumentUploadComplete(HostSecurityToken oHostSecurityToken, Guid TransferId)
        {
            DocumentReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.DocumentUploadComplete(Functions.GetLogonIdFromToken(oHostSecurityToken), TransferId);
            }
            else
            {
                ReturnValue = new DocumentReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
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
        /// <param name="oHostSecurityToken"></param>
        /// <param name="docSearchItem"></param>
        /// <returns></returns>
        public DocumentReturnValue EditMatterDocumentDetails(HostSecurityToken oHostSecurityToken, DocumentSearchItem docSearchItem)
        {
            DocumentReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.EditMatterDocumentDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), docSearchItem);
            }
            else
            {
                ReturnValue = new DocumentReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region GetDocumentDetails
        public DocumentReturnValue GetDocumentDetails(HostSecurityToken oHostSecurityToken, Guid projectId, int docId)
        {
            DocumentReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.GetDocumentDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId, docId);
            }
            else
            {
                ReturnValue = new DocumentReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region GetDocumentTypes
        /// <summary>
        /// Get Document Types for Import Document
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="criteria"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public DocumentTypeReturnValue GetDocumentTypes(HostSecurityToken oHostSecurityToken, DocumentTypeSearchCriteria criteria, CollectionRequest collectionRequest)
        {
            DocumentTypeReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.GetDocumentTypes(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria, collectionRequest);
            }
            else
            {
                ReturnValue = new DocumentTypeReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region GetFileTypes
        /// <summary>
        /// Get File Types
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <returns></returns>
        public FileTypeReturnValue GetFileTypes(HostSecurityToken oHostSecurityToken)
        {
            FileTypeReturnValue ReturnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.GetFileTypes(Functions.GetLogonIdFromToken(oHostSecurityToken));
            }
            else
            {
                ReturnValue = new FileTypeReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region GetMatterDocumentForDeepSearch
        /// <summary>
        /// Get documents for deep search
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="fileListToSearch"></param>
        /// <returns></returns>
        public DocumentSearchReturnValue GetMatterDocumentForDeepSearch(HostSecurityToken oHostSecurityToken, Guid projectId, DocumentSearchCriteria criteria)
        {
            DocumentSearchReturnValue ReturnValue = new DocumentSearchReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.GetMatterDocumentForDeepSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId, criteria);
            }
            else
            {
                ReturnValue = new DocumentSearchReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #region MatterDocumentSearch
        /// <summary>
        /// Get List of Documents for Matter
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DocumentSearchReturnValue MatterDocumentSearch(HostSecurityToken oHostSecurityToken, Guid projectId, string OrderBy)
        {
            DocumentSearchReturnValue ReturnValue = new DocumentSearchReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.MatterDocumentSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId, OrderBy);
            }
            else
            {
                ReturnValue = new DocumentSearchReturnValue();
                ReturnValue.Success = false;
                ReturnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }

        #endregion

        #region GetDocumentFolderList

        public DocumentFolderListReturnValue GetDocumentFolderList(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            DocumentFolderListReturnValue ReturnValue = new DocumentFolderListReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDocumentService = new DocumentService();
                ReturnValue = oDocumentService.GetDocumentFolderList(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                ReturnValue returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return ReturnValue;
        }
        #endregion

        #endregion
    }
}

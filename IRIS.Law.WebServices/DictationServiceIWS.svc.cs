using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Dictation;
using IRIS.Law.PmsCommonData;
using IRIS.Law.Services.Pms.Dictation;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebServices.IWSProvider
{

    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.DictationServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_DictationServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DictationServiceIWS : IDictationServiceIWS
    {
        #region Dictation

        ////public bool BeginDictation(Guid logonId, string dictationId, Guid id, string reference, int documentId, int applicationId, Guid selectedContact, int selectedProjectMapId)
        public void BeginDictation(string dictationId)
        {
            Guid _logonId = GetUserLogonID("cybage", "cybage@123");//Hard coded temporarily

            Host.LoadLoggedOnUser(_logonId);
            try
            {
                UserState userState = Host.GetUserState(_logonId);
                SrvDictationJob srvDictationJob = new SrvDictationJob();
                srvDictationJob.DictationJobId = dictationId;
                srvDictationJob.UID = UserInformation.Instance.DbUid;
                if (UserInformation.Instance.UserMemberId != DataConstants.DummyGuid)
                {
                    srvDictationJob.EntityId = UserInformation.Instance.UserMemberId;
                    srvDictationJob.SelectedContactId = UserInformation.Instance.UserMemberId;
                }
                else
                {
                    srvDictationJob.EntityId = UserInformation.Instance.UserOrgId;
                    srvDictationJob.SelectedContactId = UserInformation.Instance.UserOrgId;
                }

                srvDictationJob.DocId = 8086;
                srvDictationJob.ApplicationId = 1;
                srvDictationJob.SelectedProjectMapId = 0;
                srvDictationJob.DictationJobStarted = DateTime.Now;
                srvDictationJob.DictationJobTranscribed = DataConstants.BlankDate;

                try
                {
                    string errorMessage = string.Empty;
                    if (!srvDictationJob.Save(out errorMessage))
                    {
                        throw new Exception(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            finally
            {
                // Remove the logged on user's ApplicationSettings from the 
                // list of concurrent sessions
                Host.UnloadLoggedOnUser();
            }
        }

        private Guid GetUserLogonID(string userName, string password)
        {
            Guid logonId;
            LogonService oLogonService = new LogonService();
            LogonReturnValue returnValue = new LogonReturnValue();
            returnValue = oLogonService.Logon(userName, password);
            logonId = returnValue.LogonId;
            return logonId;
        }

        #endregion
    
    }
}

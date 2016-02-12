using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.PmsCommonData;
using IRIS.Law.DiaryServices.Diary.BookingEntry;
using IRIS.Law.DiaryServices.Diary.Data;
using System.Data;
using System.Collections.ObjectModel;
using IRIS.Law.DiaryData.DataSets;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.Services.Pms.Service;
using IRIS.Law.DiaryServices.Diary.DiaryParameter;
using IRIS.Law.DiaryData.StateData;
using IRIS.Law.DiaryServices.Diary.Occurrence;
using IRIS.Law.Services.Pms.DiaryProvider;
using IRIS.Law.PmsCommonData.Diary;
using IRIS.Law.Services.Pms.Occurence;
using IRIS.Law.PmsCommonData.CommonServices;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Diary;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServices;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "DiaryServiceIWS" here, you must also update the reference to "DiaryServiceIWS" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.DiaryServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_DiaryServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DiaryServiceIWS : IDiaryServiceIWS
    {
        DiaryService oDiaryService;

        #region GetDiaryMembers
        public DiaryMemberSearchReturnValue GetDiaryMembers(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            DiaryMemberSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetDiaryMembers(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new DiaryMemberSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region AppointmentSearch
        public AppointmentSearchReturnValue AppointmentSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                AppointmentSearchCriteria criteria)
        {
            AppointmentSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.AppointmentSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new AppointmentSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetAppointmentDetails
        public AppointmentReturnValue GetAppointmentDetails(HostSecurityToken oHostSecurityToken, Int32 appointmentId)
        {
            AppointmentReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetAppointmentDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), appointmentId);
            }
            else
            {
                returnValue = new AppointmentReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetMemberTaskDetails
        public TaskReturnValue GetMemberTaskDetails(HostSecurityToken oHostSecurityToken, Int32 taskId)
        {
            TaskReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetMemberTaskDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), taskId);
            }
            else
            {
                returnValue = new TaskReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetMatterTaskDetails
        public TaskReturnValue GetMatterTaskDetails(HostSecurityToken oHostSecurityToken, Guid projectId, Int32 taskId)
        {
            TaskReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetMatterTaskDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId, taskId);
            }
            else
            {
                returnValue = new TaskReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Save Appointment
        public AppointmentReturnValue SaveAppointment(HostSecurityToken oHostSecurityToken, Appointment appointmentDetails)
        {
            AppointmentReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.SaveAppointment(Functions.GetLogonIdFromToken(oHostSecurityToken), appointmentDetails);
            }
            else
            {
                returnValue = new AppointmentReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Save Task
        public TaskReturnValue SaveTask(HostSecurityToken oHostSecurityToken, Task taskDetails)
        {
            TaskReturnValue returnValue = new TaskReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.SaveTask(Functions.GetLogonIdFromToken(oHostSecurityToken), taskDetails);
            }
            else
            {
                returnValue = new TaskReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetBookingCancelledReasons
        public CancellationCodeSearchReturnValue GetBookingCancelledReasons(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            CancellationCodeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetBookingCancelledReasons(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new CancellationCodeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetBookingCancelledCategories
        public CancellationCodeSearchReturnValue GetBookingCancelledCategories(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            CancellationCodeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetBookingCancelledReasons(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new CancellationCodeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetTaskTypes
        public DiaryParameterReturnValue GetTaskTypes(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            DiaryParameterReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.GetTaskTypes(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new DiaryParameterReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region DeleteBooking
        public ReturnValue DeleteBooking(HostSecurityToken oHostSecurityToken, DeleteData deleteData)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.DeleteBooking(Functions.GetLogonIdFromToken(oHostSecurityToken), deleteData);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region MemberTaskSearch
        public TaskSearchReturnValue MemberTaskSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                TaskSearchCriteria criteria)
        {
            TaskSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.MemberTaskSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new TaskSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region MatterTaskSearch
        public TaskSearchReturnValue MatterTaskSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, TaskSearchCriteria criteria)
        {
            TaskSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oDiaryService = new DiaryService();
                returnValue = oDiaryService.MatterTaskSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new TaskSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }

            return returnValue;
        }
        #endregion
    }
}

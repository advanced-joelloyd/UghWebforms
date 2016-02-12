using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Time;
using IRIS.Law.Services.Pms.Charge;
using IRIS.Law.Services.Pms.CourtType;
using IRIS.Law.Services.Pms.UnpostedTime;
using IRIS.Law.Services.Pms.TimeType;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.PmsCommonData;
using IRIS.Law.Services.Pms.Time;
using IRIS.Law.Services.Accounts.Period;
using IRIS.Law.Services.Pms.AttendanceIndividual;
using IRIS.Law.Services.Pms.AdvocacyTypes;
using IRIS.Law.Services.Pms.AssociationRole;
using IRIS.Law.Services.Pms.Service;
using System.Data;
using IRIS.Law.PmsCommonData.Accounts;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Time;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "TimeServiceIWS" here, you must also update the reference to "TimeServiceIWS" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.TimeServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_TimeServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class TimeServiceIWS : ITimeServiceIWS
    {
        #region ITimeService
        TimeService oTimeService;
        #region ChargeRateService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public ChargeRateSearchReturnValue ChargeRateSearch(HostSecurityToken oHostSecurityToken,
                                                          CollectionRequest collectionRequest)
        {
            ChargeRateSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ChargeRateSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new ChargeRateSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ChargeRateOnPublicFundingSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ChargeRateSearchReturnValue ChargeRateOnPublicFundingSearch(HostSecurityToken oHostSecurityToken,
                                                          CollectionRequest collectionRequest,
                                                          ChargeRateSearchCriteria criteria)
        {
            ChargeRateSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ChargeRateOnPublicFundingSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new ChargeRateSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region CourtType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public CourtTypeReturnValue CourtTypeSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                    CourtTypeSearchCriteria criteria)
        {
            CourtTypeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.CourtTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new CourtTypeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Time Type Search

        public TimeTypeSearchReturnValue TimeTypeSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                        TimeTypeSearchCriteria criteria)
        {
            TimeTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.TimeTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new TimeTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Add Time

        public TimeReturnValue AddTime(HostSecurityToken oHostSecurityToken, Time time, TimeAdditionalDetail additionalTime, bool postToAccounts)
        {
            TimeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.AddTime(Functions.GetLogonIdFromToken(oHostSecurityToken), time, additionalTime, postToAccounts);
            }
            else
            {
                returnValue = new TimeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region UnPostedTimeSheet
        /// <summary>
        /// Get all the unposted time sheet for the current date for the logged in fee earner
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public UnPostedTimeSearchReturnValue UnPostedTimeSheetSearch(HostSecurityToken oHostSecurityToken,
                                                            CollectionRequest collectionRequest, UnPostedTimeSearchCriteria criteria)
        {
            UnPostedTimeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.UnPostedTimeSheetSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new UnPostedTimeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Update Time

        public TimeReturnValue UpdateTime(HostSecurityToken oHostSecurityToken, Time time, TimeAdditionalDetail additionalTime)
        {
            TimeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.UpdateTime(Functions.GetLogonIdFromToken(oHostSecurityToken), time, additionalTime);
            }
            else
            {
                returnValue = new TimeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Get Time

        public TimeReturnValue GetTime(HostSecurityToken oHostSecurityToken, int timeId)
        {
            TimeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.GetTime(Functions.GetLogonIdFromToken(oHostSecurityToken), timeId);
            }
            else
            {
                returnValue = new TimeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region ValidatePeriod

        /// <summary>
        /// Validates the period.
        /// </summary>
        /// <param name="oHostSecurityToken">User token</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public PeriodDetailsReturnValue ValidatePeriod(HostSecurityToken oHostSecurityToken, PeriodCriteria criteria)
        {
            PeriodDetailsReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ValidatePeriod(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria);
            }
            else
            {
                returnValue = new PeriodDetailsReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ValidateTimeTypeForPosting

        /// <summary>
        /// Validates the time type.
        /// </summary>
        /// <param name="oHostSecurityToken">User token</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public ReturnValue ValidateTimeTypeForPosting(HostSecurityToken oHostSecurityToken, TimeTypeCriteria criteria)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ValidateTimeTypeForPosting(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria);
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

        #region PostTime
        /// <summary>
        /// Post todays time sheets for accounting 
        /// </summary>
        /// <param name="oHostSecurityToken">User token</param>
        /// <param name="timeSheet">Fee Earner Time Sheet</param>
        /// <returns></returns>
        public ReturnValue PostTime(HostSecurityToken oHostSecurityToken, TimeSheet timeSheet)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.PostTime(Functions.GetLogonIdFromToken(oHostSecurityToken), timeSheet);
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

        #region AdvocacyTypeService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public AdvocacyTypeSearchReturnValue AdvocacyTypeSearch(HostSecurityToken oHostSecurityToken,
                                                          CollectionRequest collectionRequest)
        {
            AdvocacyTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.AdvocacyTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new AdvocacyTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region AttendanceIndividualSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public AttendanceIndividualSearchReturnValue AttendanceIndividualSearch(HostSecurityToken oHostSecurityToken,
                                                          CollectionRequest collectionRequest)
        {
            AttendanceIndividualSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.AttendanceIndividualSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new AttendanceIndividualSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ServiceSearchOnIndustry
        /// <summary>
        /// Get service search on industry
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ServiceSearchReturnValue ServiceSearchOnIndustry(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, ServiceSearchCriteria criteria)
        {
            ServiceSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ServiceSearchOnIndustry(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new ServiceSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ServiceSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public ServiceSearchReturnValue ServiceSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, int associationRoleId)
        {
            ServiceSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ServiceSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, associationRoleId);
            }
            else
            {
                returnValue = new ServiceSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ServiceContactSearch
        /// <summary>
        /// Get Contact service search on industry
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ServiceContactSearchReturnValue ServiceContactSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid assocParentOrgID, string OrderBy)
        {
            ServiceContactSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.ServiceContactSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, assocParentOrgID, OrderBy);
            }
            else
            {
                returnValue = new ServiceContactSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetAddtionalDetailTime

        public TimeReturnValue GetAddtionalDetailTime(HostSecurityToken oHostSecurityToken, Time time)
        {
            TimeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oTimeService = new TimeService();
                returnValue = oTimeService.GetAddtionalDetailTime(Functions.GetLogonIdFromToken(oHostSecurityToken), time);
            }
            else
            {
                returnValue = new TimeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #endregion
    }
}

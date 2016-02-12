using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.Services.Pms.CashColl;
using IRIS.Law.Services.Pms.WorkType;
using IRIS.Law.PmsCommonData;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Client;
using System.Collections.Specialized;
using IRIS.Law.Services.Pms.Client;
using IRIS.Law.WebServiceInterfaces.Earner;
using System.Data;
using IRIS.Law.Services.Pms.Association;
using IRIS.Law.Services.Pms.Address;
using IRIS.Law.WebServiceInterfaces.Contact;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Matter;
using IRIS.Law.WebServiceInterfaces.Utilities;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Utilities;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "MatterServiceIWS" here, you must also update the reference to "MatterServiceIWS" in Web.config and in the associated .svc file.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.MatterServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_MatterServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class UtilitiesServiceIWS : IUtilitiesServiceIWS
    {
        UtilitiesService oUtilitiesService;

        /// <summary>
        /// Search for a user
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public UserSearchReturnValue UserSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            UserSearchCriteria criteria)
        {
            UserSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oUtilitiesService = new UtilitiesService();
                returnValue = oUtilitiesService.UserSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new UserSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Get details about multiple Users.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="UserIds">Array of user ids</param>
        /// <returns></returns>
        public UserSearchReturnValue GetMultipleUserDetails(HostSecurityToken oHostSecurityToken, int[] UserIds)
        {
            UserSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oUtilitiesService = new UtilitiesService();
                returnValue = oUtilitiesService.GetMultipleUserDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), UserIds);
            }
            else
            {
                returnValue = new UserSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Get details about User.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="UserIds">Array of user ids</param>
        /// <returns></returns>
        public UserReturnValue GetUser(HostSecurityToken oHostSecurityToken, Guid userMemberId)
        {
            UserReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oUtilitiesService = new UtilitiesService();
                returnValue = oUtilitiesService.GetUser(Functions.GetLogonIdFromToken(oHostSecurityToken), userMemberId);
            }
            else
            {
                returnValue = new UserReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Get details about User.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="UserIds">Array of user ids</param>
        /// <returns></returns>
        public UserReturnValue GetUserByUid(HostSecurityToken oHostSecurityToken, int uid)
        {
            UserReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oUtilitiesService = new UtilitiesService();
                returnValue = oUtilitiesService.GetUserByUid(Functions.GetLogonIdFromToken(oHostSecurityToken), uid);
            }
            else
            {
                returnValue = new UserReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Get details about User.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="UserIds">Array of user ids</param>
        /// <returns></returns>
        public WorkTypeReturnValue GetWorkType(HostSecurityToken oHostSecurityToken, Guid workTypeId)
        {
            WorkTypeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oUtilitiesService = new UtilitiesService();
                returnValue = oUtilitiesService.GetWorkType(Functions.GetLogonIdFromToken(oHostSecurityToken), workTypeId);
            }
            else
            {
                returnValue = new WorkTypeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
    }
}

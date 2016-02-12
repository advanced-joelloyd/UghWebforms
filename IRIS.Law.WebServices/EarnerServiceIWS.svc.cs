using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.Services.Pms.Earner;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.PmsCommonData;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.Services.Pms.Address;
using System.Data;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Earner;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "EarnerServiceIWS" here, you must also update the reference to "EarnerServiceIWS" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.EarnerServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_EarnerServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class EarnerServiceIWS : IEarnerServiceIWS
    {
        #region IEarnerService Members
        EarnerService oEarnerService;
        #region Earner
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public EarnerSearchReturnValue EarnerSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            EarnerSearchCriteria criteria)
        {
            EarnerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oEarnerService = new EarnerService();
                returnValue = oEarnerService.EarnerSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new EarnerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Partner
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public PartnerSearchReturnValue PartnerSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            PartnerSearchCriteria criteria)
        {
            PartnerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oEarnerService = new EarnerService();
                returnValue = oEarnerService.PartnerSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new PartnerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region PersonDealingSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public PartnerSearchReturnValue PersonDealingSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            PartnerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oEarnerService = new EarnerService();
                returnValue = oEarnerService.PersonDealingSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new PartnerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeEarnerMemberId"></param>
        /// <returns></returns>
        public EarnerReturnValue GetFeeEarnerReference(HostSecurityToken oHostSecurityToken, Guid feeEarnerMemberId)
        {
            EarnerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oEarnerService = new EarnerService();
                returnValue = oEarnerService.GetFeeEarnerReference(Functions.GetLogonIdFromToken(oHostSecurityToken), feeEarnerMemberId);
            }
            else
            {
                returnValue = new EarnerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion
    }
}

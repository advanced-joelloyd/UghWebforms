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
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "MatterServiceIWS" here, you must also update the reference to "MatterServiceIWS" in Web.config and in the associated .svc file.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.MatterServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_MatterServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class MatterServiceIWS : IMatterServiceIWS
    {
        #region IMatterService Members
        MatterService oMatterService;

        #region GetMatter
        /// <summary>
        /// Get one matter
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">Matter project id</param>
        /// <returns></returns>
        public MatterReturnValue GetMatter(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            MatterReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.GetMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new MatterReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region AddMatter
        /// <summary>
        /// Add a new matter
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="matter">Matter details</param>
        /// <returns></returns>
        public MatterReturnValue AddMatter(HostSecurityToken oHostSecurityToken, Matter matter)
        {
            MatterReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.AddMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), matter);
            }
            else
            {
                returnValue = new MatterReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region UpdateMatter
        /// <summary>
        /// Update an existing matter
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="matter">Matter details</param>
        /// <returns></returns>
        public ReturnValue UpdateMatter(HostSecurityToken oHostSecurityToken, Matter matter)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.UpdateMatter(Functions.GetLogonIdFromToken(oHostSecurityToken), matter);
            }
            else
            {
                returnValue = new MatterReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region MatterSearch
        /// <summary>
        /// Get a list of matters
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested</param>
        /// <param name="criteria">Search criteria as enterred on the web page</param>
        /// <returns></returns>
        public MatterSearchReturnValue MatterSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                MatterSearchCriteria criteria)
        {
            MatterSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.MatterSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest,criteria);
            }
            else
            {
                returnValue = new MatterSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region MatterTypeSearch
        /// <summary>
        /// Get a list of matter types that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">The collection request.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public MatterTypeSearchReturnValue MatterTypeSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                            MatterTypeSearchCriteria criteria)
        {
            MatterTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.MatterTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new MatterTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region CashCollection

        public CashCollectionSearchReturnValue CashCollectionSearch(HostSecurityToken oHostSecurityToken, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, CashCollectionSearchCriteria criteria)
        {
            CashCollectionSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.CashCollectionSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new CashCollectionSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region WorkType
        /// <summary>
        /// Worktype search 
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue WorkTypeSearch(HostSecurityToken oHostSecurityToken,
                                                 CollectionRequest collectionRequest,
                                                 WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.WorkTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new WorkTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }


        #region GetValuesOnWorkTypeSelected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue GetValuesOnWorkTypeSelected(HostSecurityToken oHostSecurityToken, WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.GetValuesOnWorkTypeSelected(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria);
            }
            else
            {
                returnValue = new WorkTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion



        /// <summary>
        /// This method get the work type for a specified department
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Collection request</param>
        /// <param name="criteria">Search criteria for WorkType</param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue GetWorkTypesForDepartment(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
           WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.GetWorkTypesForDepartment(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new WorkTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

    
        #endregion

        #region UFNValidation
        /// <summary>
        /// Validates the UFN number against the specified earner ID
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Collection request</param>
        /// <param name="criteria">Criteria for </param>
        /// <returns></returns>
        public UFNReturnValue UFNValidation(HostSecurityToken oHostSecurityToken, Guid earnerId, DateTime UFNDate, string UFNNumber)
        {
            UFNReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.UFNValidation(Functions.GetLogonIdFromToken(oHostSecurityToken), earnerId, UFNDate,UFNNumber);
            }
            else
            {
                returnValue = new UFNReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region GetMatterTypeId

        /// <summary>
        /// Gets the matter type id for the project.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public MatterTypeReturnValue GetMatterTypeId(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            MatterTypeReturnValue returnValue = new MatterTypeReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.GetMatterTypeId(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new MatterTypeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        /// <summary>
        /// Get details about multiple matters.
        /// NOTE: the MatterSearchItem's returned only contain the ID, Reference and Description.  The other search fields
        /// are not returned but these could be added later.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="ProjectIds">Array of project ids</param>
        /// <returns></returns>
        public MatterSearchReturnValue GetMultipleMatterDetails(HostSecurityToken oHostSecurityToken, Guid[] ProjectIds)
        {
            MatterSearchReturnValue returnValue = new MatterSearchReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.GetMultipleMatterDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), ProjectIds);
            }
            else
            {
                returnValue = new MatterSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #region IMatterServiceIWS Members

        /// <summary>
        /// Gets the associations for the matter.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">The collection request.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public MatterAssociationReturnValue MatterAssociationSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                                    MatterAssociationSearchCriteria criteria)
        {
            MatterAssociationReturnValue returnValue = new MatterAssociationReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oMatterService = new MatterService();
                returnValue = oMatterService.MatterAssociationSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new MatterAssociationReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #endregion
    }
}

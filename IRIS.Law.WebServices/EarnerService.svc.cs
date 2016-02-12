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

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "EarnerService" here, you must also update the reference to "EarnerService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class EarnerService : IEarnerService
    {
        #region IEarnerService Members

        #region Earner
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public EarnerSearchReturnValue EarnerSearch(Guid logonId, CollectionRequest collectionRequest,
            EarnerSearchCriteria criteria)
        {
            EarnerSearchReturnValue returnValue = new EarnerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                   
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

                    // Create a data list creator for a list of matters
                    DataListCreator<EarnerSearchItem> dataListCreator = new DataListCreator<EarnerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        // TODO: Does not use criteria: FeeReference, EarnerId, UFNDate, UFNNumber
                        e.DataSet = SrvEarnerLookup.GetEarners(criteria.IncludeArchived, criteria.MultiOnly);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "PersonSurname");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Earners = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "EarnerSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "MemberId"),
                            new ImportMapping("Reference", "feeRef"),
                            new ImportMapping("Title", "PersonTitle"),
                            new ImportMapping("Name","PersonName"),
                            new ImportMapping("SurName","PersonSurname")
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

        #region Partner
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public PartnerSearchReturnValue PartnerSearch(Guid logonId, CollectionRequest collectionRequest,
            PartnerSearchCriteria criteria)
        {
            PartnerSearchReturnValue returnValue = new PartnerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {                    
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

                    // Create a data list creator for a list of matters
                    DataListCreator<PartnerSearchItem> dataListCreator = new DataListCreator<PartnerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // TODO: Does not use criteria: Name
                        e.DataSet = SrvEarnerLookup.GetPartnerLookup(criteria.IncludeArchived);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "PersonSurname");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    DataList<PartnerSearchItem> partnerList = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                         "PartnerSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                         criteria.ToString(),
                         collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                         new ImportMapping[] {
                            new ImportMapping("PartnerId", "MemberId"),
                            new ImportMapping("Name", "PersonName"),
                            new ImportMapping("PersonTitle", "PersonTitle"),
                            new ImportMapping("Surname", "PersonSurname")
                            }
                         );

                    DataSet dsPartnerList;

                    partnerList.Rows.ForEach(delegate(PartnerSearchItem item)
                    {
                        dsPartnerList = SrvAddressLookup.GetMemberAddresses(item.PartnerId);
                        SrvAddress srvAddress;
                        int intCtr;

                        foreach (DataRow dr in dsPartnerList.Tables[0].Rows)
                        {

                            srvAddress = new SrvAddress();
                            srvAddress.MemberId = item.PartnerId;
                            srvAddress.Load(int.Parse(dr["addressId"].ToString()));

                            for (intCtr = 0; intCtr < srvAddress.AdditionalInfoElements.Count; intCtr++)
                            {
                                AdditionalAddressElement additionalAddressElement = new AdditionalAddressElement();

                                if (srvAddress.AdditionalInfoElements[intCtr].AddressElTypeId == 8)
                                {
                                    item.Email = srvAddress.AdditionalInfoElements[intCtr].AddressElementText;
                                }

                            }
                        }
                    });


                    returnValue.Partners = partnerList;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region PersonDealingSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public PartnerSearchReturnValue PersonDealingSearch(Guid logonId, CollectionRequest collectionRequest)
        {
            PartnerSearchReturnValue returnValue = new PartnerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
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

                    // Create a data list creator for a list of matters
                    DataListCreator<PartnerSearchItem> dataListCreator = new DataListCreator<PartnerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // TODO: Does not use the criteria
                        e.DataSet = SrvMatterLookup.GetPersonDealingLookup();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "PersonSurname");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Partners = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "PersonDealingSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                        new ImportMapping("UId", "uid"),
                        new ImportMapping("Name", "PersonName"),
                        new ImportMapping("PersonTitle", "PersonTitle"),
                        new ImportMapping("Surname", "PersonSurname"),
                        new ImportMapping("PartnerId", "MemberId")
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeEarnerMemberId"></param>
        /// <returns></returns>
        public EarnerReturnValue GetFeeEarnerReference(Guid logonId, Guid feeEarnerMemberId)
        {
            EarnerReturnValue returnValue = new EarnerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
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

                    returnValue.EarnerRef = SrvEarnerCommon.GetFeeEarnerReference(feeEarnerMemberId);
                    returnValue.Success = true;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }
        #endregion
    }
}

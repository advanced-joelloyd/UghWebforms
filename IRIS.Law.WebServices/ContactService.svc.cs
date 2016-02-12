namespace IRIS.Law.WebServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using System.Text;
    using IRIS.Law.PmsCommonData;
    using IRIS.Law.PmsLogManager;
    using IRIS.Law.Services.Pms.Address;
    using IRIS.Law.Services.Pms.Association;
    using IRIS.Law.Services.Pms.AssociationRole;
    using IRIS.Law.Services.Pms.BusinessSource;
    using IRIS.Law.Services.Pms.Campaign;
    using IRIS.Law.Services.Pms.Contact;
    using IRIS.Law.Services.Pms.Disability;
    using IRIS.Law.Services.Pms.Ethnicity;
    using IRIS.Law.Services.Pms.Industry;
    using IRIS.Law.Services.Pms.MaritalStatus;
    using IRIS.Law.Services.Pms.Organisation;
    using IRIS.Law.Services.Pms.Person;
    using IRIS.Law.Services.Pms.PostcodeLookup;
    using IRIS.Law.Services.Pms.RoleExtendedInfo;
    using IRIS.Law.Services.Pms.Service;
    using IRIS.Law.Services.Pms.Sex;
    using IRIS.Law.Services.Pms.Title;
    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Contact;
    using CapscanAddress = IRIS.Law.PmsCommonServices.CommonServices.Capscan.CapscanAddress;

    // NOTE: If you change the class name "ContactService" here, you must also update the reference to "ContactService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class ContactService : IContactService
    {
        #region IContactService Members

        #region Address
        /// <summary>
        /// Gets the address types.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Address Type search criteria.</param>
        /// <returns></returns>
        public AddressTypeReturnValue GetAddressTypes(Guid logonId, CollectionRequest collectionRequest,
                AddressTypeSearchCriteria criteria)
        {
            AddressTypeReturnValue returnValue = new AddressTypeReturnValue();

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

                    // Create a data list creator for a list of address types
                    DataListCreator<AddressType> dataListCreator = new DataListCreator<AddressType>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {

                        if (criteria.MemberId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetAddressTypes(1, 2);
                        }
                        else if (criteria.OrganisationId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetAddressTypes(1, 3);
                        }

                        foreach (DataRow r in e.DataSet.Tables[0].Rows)
                        {
                            if (Boolean.Parse(r["AddressTypeArchived"].ToString()))
                            {
                                r.Delete();
                            }
                        }
                        e.DataSet.Tables[0].AcceptChanges();

                    };

                    // Create the data list
                    returnValue.AddressTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AddressTypes",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Description", "AddressTypeDescription"),
                            new ImportMapping("Id", "AddressTypeID"),
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

        /// <summary>
        /// Saves the address for the contact.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public AddressReturnValue SaveAddress(Guid logonId, Address address)
        {
            AddressReturnValue returnValue = new AddressReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(address.MemberId, address.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvAddress srvAddress = new SrvAddress();
                    if (address.Id > 0)
                    {
                        srvAddress.Load(address.Id);
                    }

                    srvAddress.MemberId = address.MemberId;
                    srvAddress.OrganisationId = address.OrganisationId;
                    srvAddress.AddressTypeId = address.TypeId;
                    srvAddress.AddressId = address.Id;
                    srvAddress.AddressStreetNumber = address.StreetNumber;
                    srvAddress.AddressPostCode = address.PostCode;
                    srvAddress.AddressHouseName = address.HouseName;
                    srvAddress.AddressLine1 = address.Line1;
                    srvAddress.AddressLine2 = address.Line2;
                    srvAddress.AddressLine3 = address.Line3;
                    srvAddress.AddressTown = address.Town;
                    srvAddress.AddressCounty = address.County;
                    srvAddress.AddressCountry = address.Country;
                    srvAddress.AddressDXTown = address.DXTown;
                    srvAddress.AddressDXNumber = address.DXNumber;
                    srvAddress.IsMailingAddress = address.IsMailingAddress;
                    srvAddress.IsBillingAddress = address.IsBillingAddress;
                    srvAddress.AddressOrgName = address.OrganisationName;
                    srvAddress.AddressComment = address.Comment;
                    srvAddress.AddressDepartment = address.Department;
                    srvAddress.AddressPostBox = address.PostBox;
                    srvAddress.AddressSubBuilding = address.SubBuilding;
                    srvAddress.AddressDependantLocality = address.DependantLocality;
                    srvAddress.AddressLastVerified = address.LastVerified;

                    string errorMessage;

                    returnValue.Success = srvAddress.Save(out errorMessage);
                    returnValue.Message = errorMessage;

                    address.Id = srvAddress.AddressId;
                    returnValue.Address = address;
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

        /// <summary>
        /// Saves the additional address element.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="additionalElement">The additional element.</param>
        /// <returns></returns>
        public ReturnValue SaveAdditionalAddressElement(Guid logonId,
                                        AdditionalAddressElement additionalElement)
        {
            ReturnValue returnValue = new ReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(additionalElement.MemberId, additionalElement.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvAddressAdditionalInfoElement srvAddressAdditionalInfoElement = new SrvAddressAdditionalInfoElement();

                    srvAddressAdditionalInfoElement.Load(additionalElement.MemberId, additionalElement.OrganisationId, additionalElement.TypeId);

                    srvAddressAdditionalInfoElement.AddressId = additionalElement.AddressId;
                    srvAddressAdditionalInfoElement.AddressElementText = additionalElement.ElementText;
                    srvAddressAdditionalInfoElement.AddressElComment = additionalElement.ElementComment;

                    string errorMessage;

                    returnValue.Success = srvAddressAdditionalInfoElement.Save(out errorMessage);
                    returnValue.Message = errorMessage;
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

        #region AssociationRole

        /// <summary>
        /// Get a list of partners that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Association Roles search criteria</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleSearch(Guid logonId, CollectionRequest collectionRequest,
            AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = new AssociationRoleSearchReturnValue();

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

                    // Create a data list creator for a list of association roles
                    DataListCreator<AssociationRoleSearchItem> dataListCreator = new DataListCreator<AssociationRoleSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvAssociationRoleLookup.GetGeneralContactAssociationRoles(criteria.IncludeArchived);
                    };

                    // Create the data list
                    returnValue.AssociationRole = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AssociationRoleSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("AssociationRoleID", "AssociationRolesID"),
                            new ImportMapping("AssociationRoleDescription", "AssociationRoleDescription")
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

        #region BusinessSource

        /// <summary>
        /// Businesses the source search.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Business Source search criteria</param>
        /// <returns></returns>
        public BusinessSourceReturnValue BusinessSourceSearch(Guid logonId, CollectionRequest collectionRequest,
                                BusinessSourceSearchCriteria criteria)
        {
            BusinessSourceReturnValue returnValue = new BusinessSourceReturnValue();

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

                    // Create a data list creator for a list of business sources
                    DataListCreator<BusinessSourceSearchItem> dataListCreator = new DataListCreator<BusinessSourceSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvBusinessSourceLookup.GetBusinessSourceLookup();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "SourceDesc");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.BusinessSources = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "BusinessSources",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "SourceId"),
                            new ImportMapping("Description", "SourceDesc"),
                            new ImportMapping("IsArchived", "SourceArchived")
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

        #region Campaign

        /// <summary>
        /// Campaigns the search.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Campaign search criteria.</param>
        /// <returns></returns>
        public CampaignSearchReturnValue CampaignSearch(Guid logonId, CollectionRequest collectionRequest, CampaignSearchCriteria criteria)
        {
            CampaignSearchReturnValue returnValue = new CampaignSearchReturnValue();

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

                    // Create a data list creator for a list of campaigns
                    DataListCreator<CampaignSearchItem> dataListCreator = new DataListCreator<CampaignSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        //Add the table to the dataset as the lookup is returning a datatable
                        e.DataSet = new System.Data.DataSet();
                        e.DataSet.Tables.Add(SrvCampaignLookup.GetCampaigns());

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "CampaignDesc");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Campaigns = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "CampaignSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("CampaignId", "CampaignId"),
                            new ImportMapping("Description", "CampaignDesc"),
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

        #region Disability

        /// <summary>
        /// Get a list of disability values that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Disability search criteria</param>
        /// <returns></returns>
        public DisabilitySearchReturnValue DisabilitySearch(Guid logonId, CollectionRequest collectionRequest,
                                                            DisabilitySearchCriteria criteria)
        {
            DisabilitySearchReturnValue returnValue = new DisabilitySearchReturnValue();

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

                    // Create a data list creator for a list of disabilities
                    DataListCreator<DisabilitySearchItem> dataListCreator = new DataListCreator<DisabilitySearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvDisabilityLookup.GetDisabilities(criteria.IncludeArchived);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "DisabilityDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Disabilities = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "DisabilitySearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "DisabilityID"),
                            new ImportMapping("Description", "DisabilityDescription"),
                            new ImportMapping("IsArchived", "DisabilityArchived")
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

        #region Ethnicity

        /// <summary>
        /// Get a list of ethnicity values that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Ethnicity search criteria</param>
        /// <returns></returns>
        public EthnicitySearchReturnValue EthnicitySearch(Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, EthnicitySearchCriteria criteria)
        {
            EthnicitySearchReturnValue returnValue = new EthnicitySearchReturnValue();

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

                    // Create a data list creator for a list of ethnicity values
                    DataListCreator<EthnicitySearchItem> dataListCreator = new DataListCreator<EthnicitySearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvEthnicityLookup.GetEthnicity();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "EthnicDesc");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Ethnicity = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "EthnicitySearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "EthnicID"),
                            new ImportMapping("Description", "EthnicDesc"),
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

        #region Industry

        /// <summary>
        /// Get a list of industries that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Industry search criteria</param>
        /// <returns></returns>
        public IndustrySearchReturnValue IndustrySearch(Guid logonId, CollectionRequest collectionRequest,
                                                        IndustrySearchCriteria criteria)
        {
            IndustrySearchReturnValue returnValue = new IndustrySearchReturnValue();

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

                    // Create a data list creator for a list of industries
                    DataListCreator<IndustrySearchItem> dataListCreator = new DataListCreator<IndustrySearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvIndustryLookup.GetIndustry();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "IndustryName");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Industries = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "IndustrySearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "IndustryID"),
                            new ImportMapping("ParentId", "IndustryParentID"),
                            new ImportMapping("Name", "IndustryName")
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

        /// <summary>
        /// Gets the industry for association role.
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public IndustryForAssociationRoleReturnValue GetIndustryForAssociationRole(Guid logonId, IndustrySearchCriteria criteria)
        {
            IndustryForAssociationRoleReturnValue returnValue = new IndustryForAssociationRoleReturnValue();

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

                    DsAssociationRolesForIndustry data =
                        SrvAssociationRoleLookup.GetAssociationRolesForIndustry(criteria.AssociationRoleId);

                    if (data.uvw_AssociationRolesForIndustry.Rows.Count != 0)
                    {
                        returnValue.IndustryId = data.uvw_AssociationRolesForIndustry[0].IndustryID;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region MaritalStatus

        /// <summary>
        /// Get a list of marital status that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Marital Status search criteria</param>
        /// <returns></returns>
        public MaritalStatusSearchReturnValue MaritalStatusSearch(Guid logonId, CollectionRequest collectionRequest,
                                                MaritalStatusSearchCriteria criteria)
        {
            MaritalStatusSearchReturnValue returnValue = new MaritalStatusSearchReturnValue();

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

                    // Create a data list creator for a list of marital status
                    DataListCreator<MaritalStatusSearchItem> dataListCreator = new DataListCreator<MaritalStatusSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvMaritalStatusLookup.GetMaritalStatusLookup();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "MaritalDesc");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.MaritalStatus = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "MaritalStatus",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "MaritalId"),
                            new ImportMapping("Description", "MaritalDesc")
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

        #region Organisation

        /// <summary>
        /// Get a list of organisation sub types that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Organisation Sub Type search criteria</param>
        /// <returns></returns>
        public OrganisationSubTypeSearchReturnValue OrganisationSubTypeSearch(Guid logonId,
                        CollectionRequest collectionRequest, OrganisationSubTypeSearchCriteria criteria)
        {
            OrganisationSubTypeSearchReturnValue returnValue = new OrganisationSubTypeSearchReturnValue();

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

                    // Create a data list creator for a list of organisation sub types
                    DataListCreator<OrganisationSubTypeSearchItem> dataListCreator = new DataListCreator<OrganisationSubTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvOrganisationlookup.GetOrganisationSubTypes();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "OrgSubTypesDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.SubTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "OrganisationSubTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "OrgSubTypesID"),
                            new ImportMapping("Description", "OrgSubTypesDescription"),
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

        #region PostcodeLookup

        /// <summary>
        /// Get a list of postcodes that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="criteria">Postcode Lookup search criteria</param>
        /// <returns></returns>
        public PostcodeLookupReturnValue PostcodeLookupSearch(Guid logonId, PostcodeLookupSearchCriteria criteria)
        {
            IrisLogManager _logError = new IrisLogManager(LogType.ERROR);

            PostcodeLookupReturnValue returnValue = new PostcodeLookupReturnValue();
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

                    StringBuilder sbQuery = new StringBuilder();

                    if (criteria.SearchStatus == string.Empty)
                    {


                        if (criteria.Address.HouseName != string.Empty)
                            sbQuery.Append(criteria.Address.HouseName + ", ");

                        if (criteria.Address.StreetNumber != string.Empty)
                            sbQuery.Append(criteria.Address.StreetNumber + ", ");

                        if (criteria.Address.Line1 != string.Empty)
                            sbQuery.Append(criteria.Address.Line1 + ", ");

                        if (criteria.Address.Line2 != string.Empty)
                            sbQuery.Append(criteria.Address.Line2 + ", ");

                        if (criteria.Address.DependantLocality != string.Empty)
                            sbQuery.Append(criteria.Address.DependantLocality + ", ");

                        if (criteria.Address.Line3 != string.Empty)
                            sbQuery.Append(criteria.Address.Line3 + ", ");

                        if (criteria.Address.Town != string.Empty)
                            sbQuery.Append(criteria.Address.Town + ", ");

                        if (criteria.Address.County != string.Empty)
                            sbQuery.Append(criteria.Address.County + ", ");

                        if (criteria.Address.PostCode != string.Empty)
                            sbQuery.Append(criteria.Address.PostCode + ", ");

                        while (sbQuery.Length > 0 && (sbQuery[sbQuery.Length - 1] == ' ' || sbQuery[sbQuery.Length - 1] == ','))
                        {
                            sbQuery.Length--;
                        }
                    }
                    else
                    {
                        sbQuery.Append(criteria.SearchStatus);
                    }

                    _logError.TraceMessage(" Query = " + sbQuery.ToString());

                    SrvPostcodeLookup postcodeLookup = new SrvPostcodeLookup();
                    List<CapscanAddress> results = postcodeLookup.CapscanQuery(sbQuery.ToString(), criteria.AmbiguityId);

                    _logError.TraceMessage(" CapscanQuery Results Count = " + results.Count);

                    List<PostcodeLookupSearchItem> postcodeItemList = new List<PostcodeLookupSearchItem>();

                    if (results.Count == 0)
                    {
                        returnValue = null;
                    }
                    else
                    {
                        //Add each address
                        foreach (CapscanAddress cAddress in results)
                        {
                            PostcodeLookupSearchItem postcodeItem = new PostcodeLookupSearchItem();
                            postcodeItem.AdminCounty = cAddress.admincounty;
                            postcodeItem.AmbiguityId = cAddress.ambiguityid;
                            postcodeItem.AmbiguityText = cAddress.ambiguitytext;
                            postcodeItem.Buildingname = cAddress.buildingname;
                            postcodeItem.BuildingNumber = cAddress.buildingnumber;
                            postcodeItem.Country = cAddress.country;
                            postcodeItem.County = cAddress.county;
                            postcodeItem.DepLocality = cAddress.deplocality;
                            postcodeItem.DepStreet = cAddress.depstreet;
                            postcodeItem.Locality = cAddress.locality;
                            postcodeItem.Organisation = cAddress.organisation;
                            postcodeItem.Postcode = cAddress.postcode;
                            postcodeItem.PostTown = cAddress.posttown;
                            postcodeItem.Resolved = cAddress.resolved;
                            postcodeItem.SearchStatus = cAddress.searchstatus;
                            postcodeItem.Street = cAddress.street;
                            postcodeItem.SubBuilding = cAddress.subbuilding;

                            postcodeItemList.Add(postcodeItem);
                        }

                        if (postcodeItemList != null)
                        {
                            returnValue.PostcodeLookup = postcodeItemList;
                        }
                    }

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();
                }
            }
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = "Could not load postcode lookup for the current user. <br>Exception: " + Ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region Sex

        /// <summary>
        /// Get a list of sex that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Sex search criteria</param>
        /// <returns></returns>
        public SexSearchReturnValue SexSearch(Guid logonId, CollectionRequest collectionRequest,
                                                SexSearchCriteria criteria)
        {
            SexSearchReturnValue returnValue = new SexSearchReturnValue();

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

                    // Create a data list creator for a list of sex
                    DataListCreator<SexSearchItem> dataListCreator = new DataListCreator<SexSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvSexLookup.GetSex(criteria.IncludeArchived);
                    };

                    // Create the data list
                    returnValue.Sex = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "SexSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "SexId"),
                            new ImportMapping("Description", "SexDesc"),
                            new ImportMapping("IsArchived", "SexArchived")
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

        #region Title

        /// <summary>
        /// Get a list of titles that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Title search criteria</param>
        /// <returns></returns>
        public TitleSearchReturnValue TitleSearch(Guid logonId, CollectionRequest collectionRequest,
            TitleSearchCriteria criteria)
        {
            TitleSearchReturnValue returnValue = new TitleSearchReturnValue();

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

                    // Create a data list creator for a list of titles
                    DataListCreator<TitleSearchItem> dataListCreator = new DataListCreator<TitleSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvTitleLookup.GetTitles(criteria.IncludeArchived);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "Title");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Title = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "TitleSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("TitleId", "Title")
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

        #region Contacts

        /// <summary>
        /// This method adds the Person, Address and Additional Address information
        /// using contact service to create a general contact.
        /// </summary>
        /// <param name="logonId">User logon ID</param>
        /// <param name="contactAddress">Address information for contact</param>
        /// <param name="person">Person information</param>
        /// <param name="additionalElement">Additional Element</param>
        /// <param name="contactType">Individual / Organisation</param>
        /// <param name="organisation">Organisation Information</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns>Return Value</returns>
        public ReturnValue SaveGeneralContact(Guid logonId,
                                              Address contactAddress,
                                              Person person,
                                              AdditionalAddressElement[] additionalElement,
                                              IRISLegal.IlbCommon.ContactType contactType,
                                              Organisation organisation,
                                              string conflictNoteSummary,
                                              string conflictNoteContent)
        {
            ReturnValue returnValue = new ReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(additionalElement[0].MemberId, additionalElement[0].OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvContact contactService = new SrvContact();
                    SrvAddress srvAddress = new SrvAddress();

                    srvAddress.AddressTypeId = Convert.ToInt32(DataConstants.SystemAddressTypes.Main);
                    srvAddress.AddressStreetNumber = contactAddress.StreetNumber;
                    srvAddress.AddressPostCode = contactAddress.PostCode;
                    srvAddress.AddressHouseName = contactAddress.HouseName;
                    srvAddress.AddressLine1 = contactAddress.Line1;
                    srvAddress.AddressLine2 = contactAddress.Line2;
                    srvAddress.AddressLine3 = contactAddress.Line3;
                    srvAddress.AddressTown = contactAddress.Town;
                    srvAddress.AddressCounty = contactAddress.County;
                    srvAddress.AddressCountry = contactAddress.Country;
                    srvAddress.AddressDXTown = contactAddress.DXTown;
                    srvAddress.AddressDXNumber = contactAddress.DXNumber;
                    srvAddress.IsMailingAddress = contactAddress.IsMailingAddress;
                    srvAddress.IsBillingAddress = contactAddress.IsBillingAddress;

                    srvAddress.AddressOrgName = contactAddress.OrganisationName;
                    srvAddress.AddressComment = contactAddress.Comment;
                    srvAddress.AddressDepartment = contactAddress.Department;
                    srvAddress.AddressPostBox = contactAddress.PostBox;
                    srvAddress.AddressSubBuilding = contactAddress.SubBuilding;
                    srvAddress.AddressStreetNumber = contactAddress.StreetNumber;
                    srvAddress.AddressDependantLocality = contactAddress.DependantLocality;
                    srvAddress.AddressLastVerified = contactAddress.LastVerified;

                    // Save Additional Address Info to Address Object
                    if (additionalElement != null)
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            srvAddress.AdditionalInfoElements[i].AddressElementText = additionalElement[i].ElementText;
                        }
                    }

                    contactService.Addresses.Add(srvAddress);
                    contactService.ContactType = contactType;

                    if (contactService.ContactType == IRISLegal.IlbCommon.ContactType.Individual)
                    {
                        //Person Information for Individual contact
                        contactService.Person.Surname = person.Surname;
                        contactService.Person.ForeName = person.ForeName;
                        contactService.Person.Title = person.Title;
                    }
                    else
                    {
                        contactService.Organisation.Name = organisation.Name;
                    }

                    contactService.ConflictNoteSummary = conflictNoteSummary;
                    contactService.ConflictNoteContent = conflictNoteContent;

                    string errorMessage;

                    returnValue.Success = contactService.Save(out errorMessage);
                    returnValue.Message = errorMessage;
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



        /// <summary>
        /// Adds a new service.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="serviceAddress">The service address.</param>
        /// <param name="serviceAdditionalElement">The service additional element.</param>
        /// <param name="serviceInfo">The service info.</param>
        /// <param name="serviceContactInfo">The service contact info.</param>
        /// <param name="serviceContactAddress">The service contact address.</param>
        /// <param name="ServiceContactAdditionalElement">The service contact additional element.</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns></returns>
        public ReturnValue SaveService(Guid logonId, Address serviceAddress,
                                       AdditionalAddressElement[] serviceAdditionalElement,
                                       ServiceInfo serviceInfo,
                                       ServiceContact serviceContactInfo,
                                       Address serviceContactAddress,
                                       AdditionalAddressElement[] ServiceContactAdditionalElement,
                                       string conflictNoteSummary,
                                       string conflictNoteContent)
        {
            ReturnValue returnValue = new ReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(serviceAdditionalElement[0].MemberId, serviceAdditionalElement[0].OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvService service = new SrvService();
                    SrvAddress srvAddress = new SrvAddress();

                    this.SetAddressValue(srvAddress, serviceAddress);

                    // Save Additional Address Info to Address Object
                    if (serviceAdditionalElement != null)
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            srvAddress.AdditionalInfoElements[i].AddressElementText = serviceAdditionalElement[i].ElementText;
                        }
                    }
                    service.Addresses.Add(srvAddress);
                    service.Name = serviceInfo.ServiceName;
                    service.NetId = SrvServiceCommon.GenerateNumericPassword(6);
                    service.NetPassword = SrvServiceCommon.GenerateStringPassword(10);
                    service.Organisation.IndustryId = serviceInfo.IndustryId;

                    service.Addresses[0].AddressTypeId = Convert.ToInt32(DataConstants.SystemAddressTypes.Main);
                    service.Addresses[0].MemberId = DataConstants.DummyGuid;
                    service.Addresses[0].OrganisationId = DataConstants.DummyGuid;

                    service.ServiceContact[0].Description = serviceContactInfo.Description;
                    service.ServiceContact[0].Position = serviceContactInfo.Position;
                    service.ServiceContact[0].Person.ForeName = serviceContactInfo.ForeName;
                    service.ServiceContact[0].Person.Surname = serviceContactInfo.SurName;
                    service.ServiceContact[0].Person.Title = serviceContactInfo.Title;
                    service.ServiceContact[0].Person.Sex = serviceContactInfo.Sex;
                    service.ServiceContact[0].ConflictNoteContent = conflictNoteContent;
                    service.ServiceContact[0].ConflictNoteSummary = conflictNoteSummary;

                    SrvAddress contactAddress = new SrvAddress();
                    this.SetAddressValue(contactAddress, serviceContactAddress);

                    // Save Additional Address Info to Address Object
                    if (ServiceContactAdditionalElement != null)
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            contactAddress.AdditionalInfoElements[i].AddressElementText = ServiceContactAdditionalElement[i].ElementText;
                        }
                    }

                    service.ServiceContact[0].Addresses.Add(contactAddress);
                    service.ServiceContact[0].Addresses[0].MemberId = DataConstants.DummyGuid;
                    service.ServiceContact[0].Addresses[0].OrganisationId = DataConstants.DummyGuid;

                    string errorMessage;

                    returnValue.Success = service.Save(out errorMessage);
                    returnValue.Message = errorMessage;
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

        /// <summary>
        /// Saves the service contact.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="contactAddress">The contact address.</param>
        /// <param name="additionalElement">The additional element.</param>
        /// <param name="serviceContact">The service contact.</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns></returns>
        public ReturnValue SaveServiceContact(Guid logonId, Address contactAddress,
                                       AdditionalAddressElement[] additionalElement,
                                       ServiceContact serviceContact,
                                       string conflictNoteSummary,
                                       string conflictNoteContent)
        {
            ReturnValue returnValue = new ReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(additionalElement[0].MemberId, additionalElement[0].OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvServiceContact serviceContactSrv = new SrvServiceContact();
                    SrvAddress srvAddress = new SrvAddress();

                    this.SetAddressValue(srvAddress, contactAddress);

                    // Save Additional Address Info to Address Object
                    if (additionalElement != null)
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            srvAddress.AdditionalInfoElements[i].AddressElementText = additionalElement[i].ElementText;
                        }
                    }
                    serviceContactSrv.Addresses.Add(srvAddress);

                    serviceContactSrv.ServiceId = serviceContact.ServiceId;
                    serviceContactSrv.Person.ForeName = serviceContact.ForeName;
                    serviceContactSrv.Person.Surname = serviceContact.SurName;
                    serviceContactSrv.Person.Title = serviceContact.Title;
                    serviceContactSrv.Position = serviceContact.Position;
                    serviceContactSrv.Person.Sex = serviceContact.Sex;

                    //These two notes fields are mandatory and have defined
                    //default values
                    serviceContactSrv.ConflictNoteSummary = conflictNoteSummary;
                    serviceContactSrv.ConflictNoteContent = conflictNoteContent;

                    string errorMessage;

                    returnValue.Success = serviceContactSrv.Save(out errorMessage);
                    returnValue.Message = errorMessage;
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
        #endregion Contacts

        #region AssociationRolesForApplicationSearch

        /// <summary>
        /// Search for association roles based on the application
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">AssociationRole search criteria.</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleForApplicationSearch(Guid logonId,
                    CollectionRequest collectionRequest, AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = new AssociationRoleSearchReturnValue();

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

                    // Create a data list creator for a list of association roles
                    DataListCreator<AssociationRoleSearchItem> dataListCreator = new DataListCreator<AssociationRoleSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvAssociationLookup.GetAssociationRolesForApplication(criteria.ApplicationId);
                    };

                    // Create the data list
                    returnValue.AssociationRole = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AssociationRoleForApplicationSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("AssociationRoleID", "AssociationRolesID"),
                            new ImportMapping("AssociationRoleDescription", "AssociationRoleDescription")
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

        #endregion AssociationRolesForApplicationSearch

        #region AssociationRoleForRoleIdSearch

        /// <summary>
        /// Search for association roles
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">AssociationRole search criteria.</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleForRoleIdSearch(Guid logonId,
                    CollectionRequest collectionRequest, AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = new AssociationRoleSearchReturnValue();

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

                    // Create a data list creator for a list of association roles
                    DataListCreator<AssociationRoleSearchItem> dataListCreator = new DataListCreator<AssociationRoleSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvAssociationLookup.GetAssociationRoles(criteria.RoleId);
                    };

                    // Create the data list
                    returnValue.AssociationRole = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AssociationRoleForRoleIdSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("AssociationRoleID", "AssociationRolesID"),
                            new ImportMapping("AssociationRoleDescription", "AssociationRoleDescription"),
                            new ImportMapping("AssociationRoleSearchClient", "AssociationRoleSearchClient"),
                            new ImportMapping("AssociationRoleSearchMatter", "AssociationRoleSearchMatter"),
                            new ImportMapping("AssociationRoleSearchGeneral", "AssociationRoleSearchGen"),
                            new ImportMapping("AssociationRoleSearchService", "AssociationRoleSearchServ"),
                            new ImportMapping("AssociationRoleSearchFeeEarner", "AssociationRoleSearchFeeEarner"),
                            new ImportMapping("AssociationRoleSpecialisedSearch", "AssociationRoleSpecialisedSearch")
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

        #endregion AssociationRoleForRoleIdSearch

        #region RoleExtendedInfoSearch

        /// <summary>
        /// Get extended info for the Roles.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="criteria">RoleExtendedInfo search criteria</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <returns></returns>
        public RoleExtendedInfoReturnValue RoleExtendedInfoSearch(Guid logonId, RoleExtendedInfoSearchCriteria criteria,
                                                CollectionRequest collectionRequest)
        {
            RoleExtendedInfoReturnValue returnValue = new RoleExtendedInfoReturnValue();

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

                    // Create a data list creator for a list of extended role info
                    DataListCreator<RoleExtendedInfoSearchItem> dataListCreator = new DataListCreator<RoleExtendedInfoSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvRoleExtendedInfoLookup.GetExtendedInformationByRoleID(criteria.AssociationRoleId);
                    };

                    // Create the data list
                    returnValue.RoleExtendedInfo = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "RoleExtendedInfoSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("TypeId", "RolExtInfoTypeId"),
                            new ImportMapping("TypeName", "RolExtInfoTypeName"),
                            new ImportMapping("UserCanEdit", "RolExtInfoUserCanEdit"),
                            new ImportMapping("SourceText", "RolExtInfoSourceText"),
                            new ImportMapping("DataType", "RolExtInfoDataType")
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

        #endregion RoleExtendedInfoSearch

        #region AddAssociationForMatter

        /// <summary>
        /// Adds the association for matter.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="association">The association.</param>
        /// <returns></returns>
        public ReturnValue AddAssociationForMatter(Guid logonId, AssociationForMatter association)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
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

                SrvAssociationForMatter _srvAssociationForMatter = new SrvAssociationForMatter();
                _srvAssociationForMatter.ApplicationId = association.ApplicationId;
                _srvAssociationForMatter.ProjectId = association.ProjectId;
                _srvAssociationForMatter.RoleId = association.RoleId;
                _srvAssociationForMatter.OrganisationId = association.OrganisationId;
                _srvAssociationForMatter.MemberId = association.MemberId;
                _srvAssociationForMatter.Description = association.Description;
                _srvAssociationForMatter.DateFrom = association.DateFrom;
                _srvAssociationForMatter.DateTo = association.DateTo;
                _srvAssociationForMatter.Reference = association.Reference;
                _srvAssociationForMatter.LetterHead = association.LetterHead;
                _srvAssociationForMatter.Comment = association.Comment;
                _srvAssociationForMatter.LinkedProjectId = association.LinkedProjectId;

                //Code from MsRoleExtendedInfo.cs
                // fill out some default values for personal reps
                if (association.RoleId == 61)
                {
                    //Static method call to the service layer..
                    DsMatterAssociations dmatAssoc = SrvAssociationLookup.GetMatterAssociations(association.ProjectId, "Personal Representative");

                    SrvRoleExtendedInfo roleExtendedInfo = new SrvRoleExtendedInfo();
                    roleExtendedInfo.RoleExtendedInfoTypeId = 128;
                    roleExtendedInfo.RoleExtendedInfoNumber = decimal.Zero;
                    roleExtendedInfo.RoleExtendedInfoDate = DataConstants.BlankDate;
                    roleExtendedInfo.RoleExtendedInfoComment = string.Empty;
                    if (dmatAssoc == null || dmatAssoc.uvw_ProjectAssociations.Count == 0)
                    {
                        roleExtendedInfo.RoleExtendedInfoText = "1";
                    }
                    else
                    {
                        roleExtendedInfo.RoleExtendedInfoText = "0";
                    }
                    _srvAssociationForMatter.RoleExtendedInfoDetails.Add(roleExtendedInfo);
                }

                if (association.RoleExtendedInfoDetails != null)
                {
                    //Get role info
                    foreach (RoleExtendedInfo info in association.RoleExtendedInfoDetails)
                    {
                        SrvRoleExtendedInfo roleExtendedInfo = new SrvRoleExtendedInfo();
                        roleExtendedInfo.RoleExtendedInfoTypeId = info.TypeId;
                        roleExtendedInfo.RoleExtendedInfoText = info.Text;
                        roleExtendedInfo.RoleExtendedInfoNumber = info.Number;
                        roleExtendedInfo.RoleExtendedInfoDate = info.Date;
                        roleExtendedInfo.RoleExtendedInfoComment = info.Comment;
                        _srvAssociationForMatter.RoleExtendedInfoDetails.Add(roleExtendedInfo);
                    }
                }

                string errorMessage;

                returnValue.Success = _srvAssociationForMatter.Save(out errorMessage);
                returnValue.Message = errorMessage;
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
            finally
            {
                // Remove the logged on user's ApplicationSettings from the 
                // list of concurrent sessions
                Host.UnloadLoggedOnUser();
            }

            return returnValue;
        }

        #endregion AddAssociationForMatter

        #region ContactSearch

        /// <summary>
        /// Search for contacts.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Contact search criteria.</param>
        /// <returns></returns>
        public ContactSearchReturnValue ContactSearch(Guid logonId, CollectionRequest collectionRequest, ContactSearchCriteria criteria)
        {
            ContactSearchReturnValue returnValue = new ContactSearchReturnValue();

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
                    // Create a data list creator for a list of contacts
                    DataListCreator<ContactSearchItem> dataListCreator = new DataListCreator<ContactSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the dataset
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvContactLookup.WebContactSearch(criteria.ContactName,
                                                                      criteria.Organisation,
                                                                      criteria.HouseNumber,
                                                                      criteria.POBox,
                                                                      criteria.PostCode,
                                                                      criteria.Town);
                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("Table");
                            e.DataSet.Tables.Add(dt);
                        }
                    };

                    // Create the data list
                    returnValue.Contacts = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ContactSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("OrganisationId", "orgId"),
                            new ImportMapping("MemberId", "memId"),
                            new ImportMapping("Name", "Name"),
                            new ImportMapping("Address", "Address"),
                            new ImportMapping("Town", "Town"),
                            new ImportMapping("PostCode", "PostCode")
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

        #region GetPerson

        /// <summary>
        /// Gets the person.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public PersonReturnValue GetPerson(Guid logonId, Guid memberId)
        {
            PersonReturnValue returnValue = new PersonReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(memberId, DataConstants.DummyGuid))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            if (!ApplicationSettings.Instance.IsUser(memberId, DataConstants.DummyGuid))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DsPersons dsPersons = SrvPersonLookup.GetPerson(memberId);

                    if (dsPersons.Persons.Rows.Count > 0)
                    {
                        returnValue.Person = new Person();
                        returnValue.Person.MemberId = dsPersons.Persons[0].MemberID;
                        returnValue.Person.Title = dsPersons.Persons[0].PersonTitle.Trim();
                        returnValue.Person.ForeName = dsPersons.Persons[0].PersonName.Trim();
                        returnValue.Person.Surname = dsPersons.Persons[0].PersonSurname.Trim();
                        returnValue.Person.MaritalStatusId = dsPersons.Persons[0].MaritalID;
                        returnValue.Person.PreviousName = dsPersons.Persons[0].personPrevName.Trim();
                        returnValue.Person.Occupation = dsPersons.Persons[0].personOccupation.Trim();
                        returnValue.Person.SalutationLettterFormal = dsPersons.Persons[0].personSalletForm.Trim();
                        returnValue.Person.SalutationLettterInformal = dsPersons.Persons[0].personSallet.Trim();
                        returnValue.Person.SalutationLettterFriendly = dsPersons.Persons[0].personSalletfriend.Trim();
                        returnValue.Person.SalutationEnvelope = dsPersons.Persons[0].personSalEnv.Trim();
                        returnValue.Person.Sex = dsPersons.Persons[0].personSex;
                        returnValue.Person.DOB = dsPersons.Persons[0].PersonDOB;
                        returnValue.Person.DOD = dsPersons.Persons[0].personDOD;
                        returnValue.Person.PlaceOfBirth = dsPersons.Persons[0].personPlaceOfBirth.Trim();
                        returnValue.Person.BirthName = dsPersons.Persons[0].personBirthName.Trim();
                        returnValue.Person.EthnicityId = dsPersons.Persons[0].personEthnic;
                        returnValue.Person.DisabilityId = dsPersons.Persons[0].personDisabled;
                        returnValue.Person.IsInArmedForces = dsPersons.Persons[0].personInArmedForces;
                        returnValue.Person.ArmedForcesNo = dsPersons.Persons[0].personArmedForcesNo;
                        returnValue.Person.NINo = dsPersons.Persons[0].personNHINo;
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

        /// <summary>
        /// Get details about multiple persons.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="MemberIds">Array of member ids</param>
        /// <returns></returns>
        public PersonSearchReturnValue GetMultiplePersonDetails(Guid logonId, Guid[] MemberIds)
        {
            PersonSearchReturnValue returnValue = new PersonSearchReturnValue();

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
                            // Can only access themself
                            if (MemberIds.Length != 1)
                                throw new Exception("Access denied");

                            if (!ApplicationSettings.Instance.IsUser(MemberIds[0], DataConstants.DummyGuid))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            // Can only access themself
                            if (MemberIds.Length != 1)
                                throw new Exception("Access denied");

                            if (!ApplicationSettings.Instance.IsUser(MemberIds[0], DataConstants.DummyGuid))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataListCreator<PersonSearchItem> dataListCreator = new DataListCreator<PersonSearchItem>();

                    returnValue.Persons = new DataList<PersonSearchItem>();

                    returnValue.Persons.FirstRowNumber = 0;

                    foreach (IRIS.Law.PmsCommonData.DsPersons.PersonsRow Row in
                        SrvPersonLookup.GetMultiplePersons(MemberIds).Tables[0].Rows)
                    {
                        PersonSearchItem Item = new PersonSearchItem();

                        Item.MemberId = Row.MemberID;
                        Item.Title = Row.PersonTitle;
                        Item.Forename = Row.PersonName;
                        Item.Surname = Row.PersonSurname;

                        returnValue.Persons.TotalRowCount++;
                        returnValue.Persons.Rows.Add(Item);
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

        #region GetOrganisation

        /// <summary>
        /// Gets the organisation.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        public OrganisationReturnValue GetOrganisation(Guid logonId, Guid organisationId)
        {
            OrganisationReturnValue returnValue = new OrganisationReturnValue();

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
                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DsOrganisations dsOrganisations = SrvOrganisationlookup.GetOrganisation(organisationId);

                    if (dsOrganisations.Organisations.Rows.Count > 0)
                    {
                        returnValue.Organisation = new Organisation();
                        returnValue.Organisation.OrganisationId = dsOrganisations.Organisations[0].OrgID;
                        returnValue.Organisation.Name = dsOrganisations.Organisations[0].OrgName.Trim();
                        returnValue.Organisation.RegisteredName = dsOrganisations.Organisations[0].OrgRegisteredName.Trim();
                        returnValue.Organisation.RegisteredNo = dsOrganisations.Organisations[0].OrgRegisteredNumber.Trim();
                        returnValue.Organisation.VATNo = dsOrganisations.Organisations[0].OrgVatNumber;
                        returnValue.Organisation.SubTypeId = dsOrganisations.Organisations[0].OrgSubTypesID;
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

        /// <summary>
        /// Get details about multiple organisations.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public OrganisationSearchReturnValue GetMultipleOrganisationDetails(Guid logonId, Guid[] OrganisationIds)
        {
            OrganisationSearchReturnValue returnValue = new OrganisationSearchReturnValue();

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
                            // Can only access themself
                            if (OrganisationIds.Length != 1)
                                throw new Exception("Access denied");

                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, OrganisationIds[0]))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            // Can only access themself
                            if (OrganisationIds.Length != 1)
                                throw new Exception("Access denied");

                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, OrganisationIds[0]))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataListCreator<OrganisationSearchItem> dataListCreator = new DataListCreator<OrganisationSearchItem>();

                    returnValue.Organisations = new DataList<OrganisationSearchItem>();

                    returnValue.Organisations.FirstRowNumber = 0;

                    foreach (IRIS.Law.PmsCommonData.DsOrganisations.OrganisationsRow Row in
                        SrvOrganisationlookup.GetMultipleOrganisations(OrganisationIds).Tables[0].Rows)
                    {
                        OrganisationSearchItem Item = new OrganisationSearchItem();

                        Item.OrganisationId = Row.OrgID;
                        Item.Name = Row.OrgName;

                        returnValue.Organisations.TotalRowCount++;
                        returnValue.Organisations.Rows.Add(Item);
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

        #region Set Address

        /// <summary>
        /// Sets the address value.
        /// </summary>
        /// <param name="serviceAddress">The service address.</param>
        /// <param name="controlAddress">The control address.</param>
        private void SetAddressValue(SrvAddress serviceAddress, Address controlAddress)
        {
            if (controlAddress != null)
            {
                serviceAddress.AddressTypeId = (int)(DataConstants.SystemAddressTypes.Main);
                serviceAddress.AddressStreetNumber = controlAddress.StreetNumber;
                serviceAddress.AddressPostCode = controlAddress.PostCode;
                serviceAddress.AddressHouseName = controlAddress.HouseName;
                serviceAddress.AddressLine1 = controlAddress.Line1;
                serviceAddress.AddressLine2 = controlAddress.Line2;
                serviceAddress.AddressLine3 = controlAddress.Line3;
                serviceAddress.AddressTown = controlAddress.Town;
                serviceAddress.AddressCounty = controlAddress.County;
                serviceAddress.AddressCountry = controlAddress.Country;
                serviceAddress.AddressDXTown = controlAddress.DXTown;
                serviceAddress.AddressDXNumber = controlAddress.DXNumber;
                serviceAddress.IsMailingAddress = controlAddress.IsMailingAddress;
                serviceAddress.IsBillingAddress = controlAddress.IsBillingAddress;

                serviceAddress.AddressOrgName = controlAddress.OrganisationName;
                serviceAddress.AddressComment = controlAddress.Comment;
                serviceAddress.AddressDepartment = controlAddress.Department;
                serviceAddress.AddressPostBox = controlAddress.PostBox;
                serviceAddress.AddressSubBuilding = controlAddress.SubBuilding;
                serviceAddress.AddressStreetNumber = controlAddress.StreetNumber;
                serviceAddress.AddressDependantLocality = controlAddress.DependantLocality;
                serviceAddress.AddressLastVerified = controlAddress.LastVerified;
            }
        }

        #endregion Set Address

        #region GetServiceContact

        public ServiceContactReturnValue GetServiceContact(Guid logonId, Guid contactId)
        {
            ServiceContactReturnValue returnValue = new ServiceContactReturnValue();

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

                    SrvServiceContact srvServiceContact = new SrvServiceContact();
                    srvServiceContact.Id = contactId;
                    srvServiceContact.Load();

                    if (srvServiceContact.IsLoaded)
                    {
                        //General Info
                        returnValue.ServiceContact = new Person();
                        returnValue.ServiceContact.Title = srvServiceContact.Person.Title;
                        returnValue.ServiceContact.ForeName = srvServiceContact.Person.ForeName;
                        returnValue.ServiceContact.Surname = srvServiceContact.Person.Surname;
                        returnValue.ServiceContact.MaritalStatusId = srvServiceContact.Person.MaritalId;
                        returnValue.ServiceContact.PreviousName = srvServiceContact.Person.PersonPreviousName;
                        returnValue.ServiceContact.Occupation = srvServiceContact.Person.PersonOccupation;
                        returnValue.ServiceContact.Sex = srvServiceContact.Person.Sex;
                        returnValue.ServiceContact.DOB = srvServiceContact.Person.PersonDOB;
                        returnValue.ServiceContact.DOD = srvServiceContact.Person.PersonDOD;
                        returnValue.ServiceContact.PlaceOfBirth = srvServiceContact.Person.PersonPlaceOfBirth;
                        returnValue.ServiceContact.BirthName = srvServiceContact.Person.PersonBirthName;
                        returnValue.CampaignId = srvServiceContact.CampaignId;

                        //Additional Info
                        returnValue.ServiceContact.SalutationLettterFormal = srvServiceContact.Person.PersonSalletForm;
                        returnValue.ServiceContact.SalutationLettterInformal = srvServiceContact.Person.PersonSalutationlettterInformal;
                        returnValue.ServiceContact.SalutationLettterFriendly = srvServiceContact.Person.PersonSalLet;
                        returnValue.ServiceContact.SalutationEnvelope = srvServiceContact.Person.PersonSalEnv;
                        returnValue.ServiceContact.EthnicityId = srvServiceContact.Person.PersonEthnicityId;
                        returnValue.ServiceContact.IsInArmedForces = srvServiceContact.Person.PersonInArmedForces;
                        returnValue.ServiceContact.ArmedForcesNo = srvServiceContact.Person.PersonArmedForcesNo;
                        returnValue.ServiceContact.NINo = srvServiceContact.Person.PersonNINo;
                        returnValue.IsReceivingMarketingStatus = srvServiceContact.IsReceivingMarketingStatus;
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

        #region GetGeneralContact

        public GeneralContactReturnValue GetGeneralContact(Guid logonId, Guid memberId, Guid organisationId)
        {
            GeneralContactReturnValue returnValue = new GeneralContactReturnValue();

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
                    SrvContact srvContact = new SrvContact();
                    if (memberId != DataConstants.DummyGuid)
                    {
                        srvContact.ContactType = IRISLegal.IlbCommon.ContactType.Individual;
                        srvContact.Id = memberId;
                    }
                    else
                    {
                        srvContact.ContactType = IRISLegal.IlbCommon.ContactType.Organisation;
                        srvContact.Id = organisationId;
                    }
                    srvContact.Load(srvContact.Id);

                    if (memberId != DataConstants.DummyGuid)
                    {
                        //General Info
                        returnValue.Person = new Person();
                        returnValue.Person.Title = srvContact.Person.Title;
                        returnValue.Person.ForeName = srvContact.Person.ForeName;
                        returnValue.Person.Surname = srvContact.Person.Surname;
                        returnValue.Person.MaritalStatusId = srvContact.Person.MaritalId;
                        returnValue.Person.PreviousName = srvContact.Person.PersonPreviousName;
                        returnValue.Person.Occupation = srvContact.Person.PersonOccupation;
                        returnValue.Person.Sex = srvContact.Person.Sex;
                        returnValue.Person.DOB = srvContact.Person.PersonDOB;
                        returnValue.Person.DOD = srvContact.Person.PersonDOD;
                        returnValue.Person.PlaceOfBirth = srvContact.Person.PersonPlaceOfBirth;
                        returnValue.Person.BirthName = srvContact.Person.PersonBirthName;
                        returnValue.CampaignId = srvContact.CampaignId;

                        //Additional Info
                        returnValue.Person.SalutationLettterFormal = srvContact.Person.PersonSalletForm;
                        returnValue.Person.SalutationLettterInformal = srvContact.Person.PersonSalutationlettterInformal;
                        returnValue.Person.SalutationLettterFriendly = srvContact.Person.PersonSalLet;
                        returnValue.Person.SalutationEnvelope = srvContact.Person.PersonSalEnv;
                        returnValue.Person.EthnicityId = srvContact.Person.PersonEthnicityId;
                        returnValue.Person.IsInArmedForces = srvContact.Person.PersonInArmedForces;
                        returnValue.Person.ArmedForcesNo = srvContact.Person.PersonArmedForcesNo;
                        returnValue.Person.NINo = srvContact.Person.PersonNINo;
                        returnValue.IsReceivingMarketingStatus = srvContact.IsReceivingMarketing;
                    }
                    else
                    {
                        returnValue.Organisation = new Organisation();
                        returnValue.Organisation.OrganisationId = srvContact.Organisation.OrganisationId;
                        returnValue.Organisation.Name = srvContact.Organisation.Name;
                        returnValue.Organisation.RegisteredName = srvContact.Organisation.RegisteredName;
                        returnValue.Organisation.RegisteredNo = srvContact.Organisation.RegisteredNumber;
                        returnValue.Organisation.VATNo = srvContact.Organisation.VATNumber;
                        returnValue.Organisation.SubTypeId = srvContact.Organisation.SubTypesId;
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

        #endregion GetGeneralContact

        #region GetContactAddress

        /// <summary>
        /// Get the address for the specified contact  
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public AddressSearchReturnValue GetContactAddresses(Guid logonId, CollectionRequest collectionRequest,
                                        AddressSearchCriteria criteria)
        {
            AddressSearchReturnValue returnValue = new AddressSearchReturnValue();
            SrvAddress srvAddress;

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
                            if (!ApplicationSettings.Instance.IsUser(criteria.MemberId, criteria.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataListCreator<Address> dataListCreator = new DataListCreator<Address>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        if (criteria.MemberId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetMemberAddresses(criteria.MemberId);
                        }
                        else if (criteria.OrganisationId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetOrganisationAddresses(criteria.OrganisationId);
                        }
                    };

                    // Create the data list
                    returnValue.Addresses = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ClientAddresses",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "AddressId"),
                            new ImportMapping("TypeId", "AddressTypeID"),
                            new ImportMapping("Line1", "AddressLine1"),
                            new ImportMapping("Line2", "AddressLine2"),
                            new ImportMapping("Line3", "AddressLine3"),
                            new ImportMapping("Town", "AddressTown"),
                            new ImportMapping("County", "AddressCounty"),
                            new ImportMapping("PostCode", "AddressPostCode"),
                            new ImportMapping("DXTown", "AddressDXTown"),
                            new ImportMapping("DXNumber", "AddressDX"),
                            new ImportMapping("Country", "AddressCountry"),
                            new ImportMapping("IsMailingAddress", "AddressMailingAddress"),
                            new ImportMapping("IsBillingAddress", "AddressBillingAddress"),
                            new ImportMapping("Comment", "AddressComment"),
                            new ImportMapping("OrganisationName", "AddressOrgName"),
                            new ImportMapping("Department", "AddressDept"),
                            new ImportMapping("PostBox", "AddressPoBox"),
                            new ImportMapping("SubBuilding", "AddressSubBldg"),
                            new ImportMapping("StreetNumber", "AddressStreetNo"),
                            new ImportMapping("HouseName", "AddressHouseName"),
                            new ImportMapping("DependantLocality", "AddressDepLocality"),
                            new ImportMapping("LastVerified", "AddressLastVerified"),
                            }
                        );

                    int intCtr;
                    foreach (Address address in returnValue.Addresses.Rows)
                    {
                        srvAddress = new SrvAddress();
                        srvAddress.MemberId = criteria.MemberId;
                        srvAddress.OrganisationId = criteria.OrganisationId;
                        srvAddress.Load(address.Id);
                        address.AdditionalAddressElements = new List<AdditionalAddressElement>();
                        for (intCtr = 0; intCtr < srvAddress.AdditionalInfoElements.Count; intCtr++)
                        {
                            AdditionalAddressElement additionalAddressElement = new AdditionalAddressElement();
                            additionalAddressElement.ElementComment = srvAddress.AdditionalInfoElements[intCtr].AddressElComment;
                            additionalAddressElement.ElementText = srvAddress.AdditionalInfoElements[intCtr].AddressElementText;
                            additionalAddressElement.TypeId = srvAddress.AdditionalInfoElements[intCtr].AddressElTypeId;
                            address.AdditionalAddressElements.Add(additionalAddressElement);
                        }
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        /// <summary>
        /// This method updates the Person / Organisation information
        /// using contact service .
        /// </summary>
        /// <param name="logonId">User logon ID</param>
        /// <param name="person">Person information</param>
        /// <param name="contactType">Individual / Organisation</param>
        /// <param name="organisation">Organisation Information</param>
        /// <returns>Return Value</returns>
        public ReturnValue UpdateGeneralContact(Guid logonId, Person person, IRISLegal.IlbCommon.ContactType contactType, Organisation organisation)
        {
            ReturnValue returnValue = new ReturnValue();

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
                        case DataConstants.UserType.ThirdParty:
                            switch (contactType)
                            {
                                case IRISLegal.IlbCommon.ContactType.Individual:
                                    if (!ApplicationSettings.Instance.IsUser(person.MemberId, DataConstants.DummyGuid))
                                        throw new Exception("Access denied");
                                    break;
                                case IRISLegal.IlbCommon.ContactType.Organisation:
                                    if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisation.OrganisationId))
                                        throw new Exception("Access denied");
                                    break;
                                default:
                                    throw new Exception("Invalid contact type");
                            }

                            break;
                        case DataConstants.UserType.Client:
                            switch (contactType)
                            {
                                case IRISLegal.IlbCommon.ContactType.Individual:
                                    if (!ApplicationSettings.Instance.IsUser(person.MemberId, DataConstants.DummyGuid))
                                        throw new Exception("Access denied");
                                    break;
                                case IRISLegal.IlbCommon.ContactType.Organisation:
                                    if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisation.OrganisationId))
                                        throw new Exception("Access denied");
                                    break;
                                default:
                                    throw new Exception("Invalid contact type");
                            }
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvServiceContact serviceContactService = new SrvServiceContact();
                    SrvContact contactService = new SrvContact();

                    string errorMessage;
                    string errorMessageWarning;

                    contactService.ContactType = contactType;
                    contactService.Load(person.MemberId);

                    if (!contactService.ValidateId(out errorMessage, out errorMessageWarning))
                    {
                        serviceContactService.Id = person.MemberId;
                        serviceContactService.Load();

                        serviceContactService.Person.ForeName = person.ForeName;
                        serviceContactService.Person.MaritalId = person.MaritalStatusId;
                        serviceContactService.Person.PersonArmedForcesNo = person.ArmedForcesNo;
                        serviceContactService.Person.PersonBirthName = person.BirthName;
                        serviceContactService.Person.PersonDisability = person.DisabilityId;
                        serviceContactService.Person.PersonDOB = person.DOB;
                        serviceContactService.Person.PersonDOD = person.DOD;
                        serviceContactService.Person.PersonEthnicityId = person.EthnicityId;
                        serviceContactService.Person.PersonNINo = person.NINo;
                        serviceContactService.Person.PersonOccupation = person.Occupation;
                        serviceContactService.Person.PersonPlaceOfBirth = person.PlaceOfBirth;
                        serviceContactService.Person.PersonPreviousName = person.PreviousName;
                        serviceContactService.Person.PersonSalEnv = person.SalutationEnvelope;
                        serviceContactService.Person.PersonSalLet = person.SalutationLettterFriendly;
                        serviceContactService.Person.PersonSalletForm = person.SalutationLettterFormal;
                        serviceContactService.Person.PersonSalutationlettterInformal = person.SalutationLettterInformal;
                        serviceContactService.Person.Sex = person.Sex;
                        serviceContactService.Person.Surname = person.Surname;
                        serviceContactService.Person.Title = person.Title;

                        returnValue.Success = serviceContactService.Save(out errorMessage);
                        returnValue.Message = errorMessage;
                    }
                    else
                    {

                        if (contactService.ContactType == IRISLegal.IlbCommon.ContactType.Individual)
                        {

                            //Person Information for Individual contact

                            contactService.Person.ForeName = person.ForeName;
                            contactService.Person.MaritalId = person.MaritalStatusId;
                            contactService.Person.PersonArmedForcesNo = person.ArmedForcesNo;
                            contactService.Person.PersonBirthName = person.BirthName;
                            contactService.Person.PersonDisability = person.DisabilityId;
                            contactService.Person.PersonDOB = person.DOB;
                            contactService.Person.PersonDOD = person.DOD;
                            contactService.Person.PersonEthnicityId = person.EthnicityId;
                            contactService.Person.PersonNINo = person.NINo;
                            contactService.Person.PersonOccupation = person.Occupation;
                            contactService.Person.PersonPlaceOfBirth = person.PlaceOfBirth;
                            contactService.Person.PersonPreviousName = person.PreviousName;
                            contactService.Person.PersonSalEnv = person.SalutationEnvelope;
                            contactService.Person.PersonSalLet = person.SalutationLettterFriendly;
                            contactService.Person.PersonSalletForm = person.SalutationLettterFormal;
                            contactService.Person.PersonSalutationlettterInformal = person.SalutationLettterInformal;
                            contactService.Person.Sex = person.Sex;
                            contactService.Person.Surname = person.Surname;
                            contactService.Person.Title = person.Title;
                        }
                        else
                        {
                            contactService.Load(organisation.OrganisationId);
                            contactService.Organisation.IndustryId = organisation.IndustryId;
                            contactService.Organisation.Name = organisation.Name;
                            contactService.Organisation.RegisteredName = organisation.RegisteredName;
                            contactService.Organisation.RegisteredNumber = organisation.RegisteredNo;
                            contactService.Organisation.SubTypesId = organisation.SubTypeId;
                            contactService.Organisation.VATNumber = organisation.VATNo;
                        }


                        returnValue.Success = contactService.Save(out errorMessage);
                        returnValue.Message = errorMessage;
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


    }
}

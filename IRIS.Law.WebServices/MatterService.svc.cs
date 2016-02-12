namespace IRIS.Law.WebServices
{
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.ServiceModel;

    using IRIS.Law.PmsCommonData;
    using IRIS.Law.PmsCommonServices.CommonServices;
    using IRIS.Law.Services.Pms.Address;
    using IRIS.Law.Services.Pms.Association;
    using IRIS.Law.Services.Pms.CashColl;
    using IRIS.Law.Services.Pms.Client;
    using IRIS.Law.Services.Pms.Matter;
    using IRIS.Law.Services.Pms.WorkType;
    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Client;
    using IRIS.Law.WebServiceInterfaces.Contact;
    using IRIS.Law.WebServiceInterfaces.Matter;

    // NOTE: If you change the class name "MatterService" here, you must also update the reference to "MatterService" in Web.config and in the associated .svc file.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class MatterService : IMatterService
    {
        #region IMatterService Members

        #region GetMatter
        /// <summary>
        /// Get one matter
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">Matter project id</param>
        /// <returns></returns>
        public MatterReturnValue GetMatter(Guid logonId, Guid projectId)
        {
            MatterReturnValue returnValue = new MatterReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
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
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvMatter srvMatter = new SrvMatter();
                    int campaignId = SrvMatterCommon.GetCampaignForProject(projectId);

                    srvMatter.Load(projectId);

                    Matter matter = new Matter();
                    matter.Id = srvMatter.ProjectId;

                    // Load Matter Details
                    matter.Description = srvMatter.MatterDescription;
                    matter.KeyDescription = srvMatter.MatterKeyDescription;
                    matter.FeeEarnerMemberId = srvMatter.FeeEarnerMemberId;
                    matter.PartnerMemberId = srvMatter.MatterPartnerMemberId;
                    matter.WorkTypeId = srvMatter.CurrentWorkTypeId;
                    matter.ClientBankId = srvMatter.ClientBankId;
                    matter.OfficeBankId = srvMatter.OfficeBankId;
                    matter.DepositBankId = srvMatter.MatterDepositBankId;
                    matter.BranchReference = srvMatter.BranchId;
                    matter.DepartmentId = srvMatter.DepartmentId;
                    matter.ChargeDescriptionId = srvMatter.ChargeDescriptionId;
                    matter.CourtId = srvMatter.CourtId;
                    matter.OpenDate = srvMatter.MatterOpenDate;
                    matter.NextReviewDate = srvMatter.MatterNextReviewDate;
                    matter.CostReviewDate = srvMatter.MatterCostReviewDate;
                    matter.LastSavedDate = srvMatter.MatterLastSaved;
                    matter.ClosedDate = srvMatter.MatterClosedDate;
                    matter.DestructDate = srvMatter.MatterDestructDate;
                    matter.FileNo = srvMatter.MatterFileNo;
                    matter.CompletedDate = srvMatter.MatterCompleted;
                    matter.SpanType1Ref = srvMatter.SpanType1;
                    matter.SpanType2Ref = srvMatter.SpanType2;

                    // Load Matter Additional Info
                    matter.Quote = srvMatter.MatterQuote;
                    matter.DisbsLimit = srvMatter.MatterDisbsLimit;
                    matter.TimeLimit = srvMatter.MatterTimeLimit;
                    matter.WIPLimit = srvMatter.MatterWIPLimit;
                    matter.OverallLimit = srvMatter.MatterOverallLimit;
                    matter.Status = srvMatter.MatterStatus;
                    matter.Indicators = srvMatter.MatterIndicators;
                    matter.BankReference = srvMatter.MatterBankReference;
                    matter.CashCollectionId = srvMatter.MatCashCollID;
                    matter.TotalLockup = srvMatter.MatTotalLockup;
                    matter.OurReference = srvMatter.OurReference;
                    matter.PreviousReference = srvMatter.PreviousReference;
                    matter.BusinessSourceId = srvMatter.SourceID;
                    matter.SourceCampaignId = campaignId;
                    matter.PersonDealingId = srvMatter.UserId;
                    matter.SalutationEnvelope = srvMatter.MatterSalutationEnvelope;
                    matter.SalutationLetter = srvMatter.MatterSalutationLetter;
                    matter.LetterHead = srvMatter.MatterLetterHead;

                    // Load Public Funding
                    matter.MatterLegalAided = srvMatter.MatterLegalAided;
                    matter.IsPublicFunding = srvMatter.IsPublicFunding;
                    matter.Franchised = srvMatter.MatterFranchised;
                    matter.isLondonRate = srvMatter.MatterLondonRate;
                    matter.UFNDate = srvMatter.UFNDate;
                    matter.UFN = srvMatter.UFNNumber;
                    matter.PFCertificateNo = srvMatter.MatterPFCertificateNo;
                    matter.PFCertificateNoLimits = srvMatter.MatterPFCertificateLimits;
                    matter.MatterTypeId = srvMatter.MatterTypeId;

                    // Load Client Details
                    Client client = new Client();
                    client.IsMember = srvMatter.IsMember;

                    //Set MemberID for Individual
                    if (client.IsMember)
                    {
                        client.MemberId = srvMatter.ClientId;
                        client.OrganisationId = DataConstants.DummyGuid;
                    }
                    //Set OrganisationId for Organisation
                    else
                    {
                        client.MemberId = DataConstants.DummyGuid;
                        client.OrganisationId = srvMatter.ClientId;
                    }

                    client.Reference = srvMatter.ClientReference;
                    client.FullName = srvMatter.ClientName;
                    client.IsArchived = srvMatter.IsClientArchived;

                    // Get Client Type ID
                    client.TypeId = this.GetClientType(srvMatter.ClientId, srvMatter.ProjectId, srvMatter.IsMember);

                    returnValue.Matter = matter;
                    returnValue.ClientDetails = client;
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

        #region AddMatter
        /// <summary>
        /// Add a new matter
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="matter">Matter details</param>
        /// <returns></returns>
        public MatterReturnValue AddMatter(Guid logonId, Matter matter)
        {
            MatterReturnValue returnValue = new MatterReturnValue();

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

                    // Ensure we have permission
                    if (!UserSecuritySettings.GetUserSecuitySettings(182))
                        throw new Exception("You do not have sufficient permissions to carry out this request");

                    // Verify Annual Licence
                    if (!IRIS.Law.PmsBusiness.LicenseDetails.AnnualLicenseIsValid())
                    {
                        throw new Exception("Unable to add Matter. Your Annual Licence has expired or is invalid.");
                    }

                    SrvMatter srvMatter = new SrvMatter();

                    srvMatter.ClientId = matter.ClientId;
                    srvMatter.MatterDescription = matter.Description;
                    srvMatter.MatterPartnerMemberId = matter.PartnerMemberId;
                    srvMatter.FeeEarnerMemberId = matter.FeeEarnerMemberId;
                    srvMatter.WorkCategoryFranchised = matter.Franchised;
                    //Call SetDefaultsOnFeeEarnerMemberId in order to set the userId
                    srvMatter.SetDefaultsOnFeeEarnerMemberId();
                    srvMatter.UFNNumber = matter.UFN;
                    srvMatter.UFNDate = matter.UFNDate;
                    srvMatter.IsPublicFunding = matter.IsPublicFunding;
                    srvMatter.ClientBankId = matter.ClientBankId;
                    srvMatter.OfficeBankId = matter.OfficeBankId;
                    srvMatter.ChargeDescriptionId = matter.ChargeDescriptionId;
                    srvMatter.CurrentWorkTypeId = matter.WorkTypeId;
                    srvMatter.DepartmentId = matter.DepartmentId;
                    srvMatter.BranchId = matter.BranchReference;
                    srvMatter.CourtId = matter.CourtId;
                    srvMatter.MatterTypeId = matter.MatterTypeId;
                    srvMatter.ClientHOUCN = matter.HOUCN;
                    srvMatter.EarnerReference = matter.FeeEarnerReference;
                    srvMatter.ConOrgMemIdCollection = new NameValueCollection();
                    foreach (JointClientCandidateSearchItem client in matter.JointClientCandidates.Rows)
                    {
                        srvMatter.ConOrgMemIdCollection.Add(client.OrganisationId, client.MemberId);
                    }

                    string errorMessage;

                    returnValue.Success = srvMatter.Save(out errorMessage);
                    returnValue.Message = errorMessage;

                    matter.Id = srvMatter.ProjectId;
                    returnValue.Matter = matter;
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

        #region UpdateMatter
        /// <summary>
        /// Update an existing matter
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="matter">Matter details</param>
        /// <returns></returns>
        public ReturnValue UpdateMatter(Guid logonId, Matter matter)
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
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvMatter srvMatter = new SrvMatter();
                    srvMatter.Load(matter.Id);

                    // Save Matter Details
                    srvMatter.ProjectId = matter.Id;
                    srvMatter.MatterDescription = matter.Description;
                    srvMatter.MatterKeyDescription = matter.KeyDescription;
                    srvMatter.FeeEarnerMemberId = matter.FeeEarnerMemberId;
                    srvMatter.MatterPartnerMemberId = matter.PartnerMemberId;
                    srvMatter.CurrentWorkTypeId = matter.WorkTypeId;
                    srvMatter.ClientBankId = matter.ClientBankId;
                    srvMatter.OfficeBankId = matter.OfficeBankId;
                    srvMatter.MatterDepositBankId = matter.DepositBankId;
                    srvMatter.BranchId = matter.BranchReference;
                    srvMatter.DepartmentId = matter.DepartmentId;
                    srvMatter.ChargeDescriptionId = matter.ChargeDescriptionId;
                    srvMatter.CourtId = matter.CourtId;
                    srvMatter.MatterOpenDate = matter.OpenDate;
                    srvMatter.MatterNextReviewDate = matter.NextReviewDate;
                    srvMatter.MatterCostReviewDate = matter.CostReviewDate;
                    srvMatter.MatterClosedDate = matter.ClosedDate;
                    srvMatter.MatterDestructDate = matter.DestructDate;
                    srvMatter.MatterFileNo = matter.FileNo;
                    srvMatter.MatterCompleted = matter.CompletedDate;

                    // Save Matter Additional Info
                    srvMatter.MatterQuote = matter.Quote;
                    srvMatter.MatterDisbsLimit = matter.DisbsLimit;
                    srvMatter.MatterTimeLimit = matter.TimeLimit;
                    srvMatter.MatterWIPLimit = matter.WIPLimit;
                    srvMatter.MatterOverallLimit = matter.OverallLimit;
                    srvMatter.MatterStatus = matter.Status;
                    srvMatter.MatterIndicators = matter.Indicators;
                    srvMatter.MatterBankReference = matter.BankReference;
                    srvMatter.MatCashCollID = matter.CashCollectionId;
                    srvMatter.MatTotalLockup = matter.TotalLockup;
                    srvMatter.OurReference = matter.OurReference;
                    srvMatter.PreviousReference = matter.PreviousReference;
                    srvMatter.SourceID = matter.BusinessSourceId;
                    // TODO: Save Campaign Id
                    // = campaignId = matter.SourceCampaignId;
                    srvMatter.UserId = matter.PersonDealingId;
                    srvMatter.MatterSalutationEnvelope = matter.SalutationEnvelope;
                    srvMatter.MatterSalutationLetter = matter.SalutationLetter;
                    srvMatter.MatterLetterHead = matter.LetterHead;

                    //Save Matter Public Funding
                    srvMatter.MatterLegalAided = matter.IsPublicFunding;
                    srvMatter.MatterFranchised = matter.Franchised;
                    srvMatter.MatterLondonRate = matter.isLondonRate;
                    srvMatter.UFNDate = matter.UFNDate;
                    srvMatter.UFNNumber = matter.UFN;
                    srvMatter.MatterPFCertificateNo = matter.PFCertificateNo;
                    srvMatter.MatterPFCertificateLimits = matter.PFCertificateNoLimits;
                    srvMatter.MatterTypeId = matter.MatterTypeId;

                    srvMatter.ClientId = matter.ClientId;

                    string errorMessage;

                    returnValue.Success = srvMatter.Save(out errorMessage);
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

        #region MatterSearch
        /// <summary>
        /// Get a list of matters
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Information about the collection being requested</param>
        /// <param name="criteria">Search criteria as enterred on the web page</param>
        /// <returns></returns>
        public MatterSearchReturnValue MatterSearch(Guid logonId, CollectionRequest collectionRequest,
                                MatterSearchCriteria criteria)
        {
            MatterSearchReturnValue returnValue = new MatterSearchReturnValue();

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
                            // Matter search will filter data that they are allowed to see
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<MatterSearchItem> dataListCreator = new DataListCreator<MatterSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvMatterCommon.WebMatterSearch(criteria.MemberId,
                                                                    criteria.OrganisationId,
                                                                    criteria.MatterId,
                                                                    criteria.MatterDescription,
                                                                    criteria.KeyDescription,
                                                                    criteria.DepartmentCode,
                                                                    criteria.BranchCode,
                                                                    criteria.FeeEarner,
                                                                    criteria.WorkTypeCode,
                                                                    criteria.OpenedDateFrom,
                                                                    criteria.OpenedDateTo,
                                                                    criteria.ClosedDateFrom,
                                                                    criteria.ClosedDateTo,
                                                                    criteria.MatterReference,
                                                                    criteria.MatterPreviousReference,
                                                                    criteria.UFN);

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("Table");
                            e.DataSet.Tables.Add(dt);
                        }
                    };

                    // Create the data list
                    returnValue.Matters = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "MatterSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "ProjectId"),
                            new ImportMapping("Reference", "MatterReference"),
                            new ImportMapping("Description", "MatterDescription"),
                            new ImportMapping("KeyDescription","MatterKeyDescription"),
                            new ImportMapping("DepartmentCode","DepartmentCode"),
                            new ImportMapping("DepartmentName","Department"),
                            new ImportMapping("BranchCode","BranchCode"),
                            new ImportMapping("BranchName","BranchName"),
                            new ImportMapping("FeeEarnerName","FeeEarner"),
                            new ImportMapping("WorkTypeCode","WorkTypeCode"),
                            new ImportMapping("WorkType","WorkType"),
                            new ImportMapping("OpenedDate","OpenedDate"),
                            new ImportMapping("ClosedDate","ClosedDate")
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

        #region MatterTypeSearch
        /// <summary>
        /// Get a list of matter types that match the search criteria
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="collectionRequest">The collection request.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public MatterTypeSearchReturnValue MatterTypeSearch(Guid logonId, CollectionRequest collectionRequest,
                                            MatterTypeSearchCriteria criteria)
        {
            MatterTypeSearchReturnValue returnValue = new MatterTypeSearchReturnValue();

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

                    // Create a data list creator for a list of matters
                    DataListCreator<MatterTypeSearchItem> dataListCreator = new DataListCreator<MatterTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvMatterLookup.GetMatterTypes(criteria.ClientTypeId);
                    };

                    // Create the data list
                    returnValue.MatterTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "MatterTypes",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "MatterTypeID"),
                            new ImportMapping("Description", "MatterTypeDesc"),
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

        #region CashCollection

        public CashCollectionSearchReturnValue CashCollectionSearch(Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, CashCollectionSearchCriteria criteria)
        {
            CashCollectionSearchReturnValue returnValue = new CashCollectionSearchReturnValue();

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

                    // Create a data list creator for a list of cash collection procedures
                    DataListCreator<CashCollectionSearchItem> dataListCreator = new DataListCreator<CashCollectionSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvCashCollLookup.GetCashColl(criteria.IncludeArchived);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "CashCollDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.CashCollection = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "CashCollectionSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "CashCollId"),
                            new ImportMapping("Description", "CashCollDescription"),
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

        #region WorkType
        /// <summary>
        /// Worktype search 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue WorkTypeSearch(Guid logonId,
                                                 CollectionRequest collectionRequest,
                                                 WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = new WorkTypeSearchReturnValue();

            try
            {
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

                    // Create a data list creator for a list of matters
                    DataListCreator<WorkTypeSearchItem> dataListCreator = new DataListCreator<WorkTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        //If this flag is true, fetch all the worktypes  
                        if (criteria.AllWorkTypes)
                        {
                            //Create the dataset  
                            e.DataSet = SrvWorkTypeLookup.GetWorkTypes();
                        }
                        else
                        {
                            // Create the dataset
                            if (criteria.MatterTypeId != 0)
                            {
                                e.DataSet = SrvWorkTypeLookup.GetWorkTypesLookup(criteria.IsPrivateClient, criteria.DepartmentId, criteria.OrganisationID, criteria.MatterTypeId);
                            }
                            else
                            {
                                e.DataSet = SrvWorkTypeLookup.GetWorkTypesLookup(criteria.IsPrivateClient, criteria.DepartmentId, criteria.OrganisationID);
                            }
                        }

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "WorkTypeDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);

                        foreach (DataRow r in e.DataSet.Tables[0].Rows)
                        {
                            r["WorkTypeDescription"] = r["WorkTypeCode"].ToString().Trim() + " - " + r["WorkTypeDescription"];
                        }
                    };

                    // Create the data list
                    returnValue.WorkTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "WorkTypeSearchOnMatterType",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "WorkTypeID"),
                            new ImportMapping("Description", "WorkTypeDescription"),
                            new ImportMapping("Code", "WorkTypeCode"),
                            new ImportMapping("IsArchived", "WorkTypeArchived")
                            }
                        );
                }
                finally
                {
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


        #region GetValuesOnWorkTypeSelected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue GetValuesOnWorkTypeSelected(Guid logonId, WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = new WorkTypeSearchReturnValue();
            string errorMessage = string.Empty;
            string warningMessage = string.Empty;
            bool isValid = false;

            try
            {
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

                    // Create the dataset
                    SrvMatter serviceMatter = new SrvMatter();
                    //Set the client id and call the  client validation method which will set the client ref
                    //Cli ref is required to get the ucn and houcn values which are set in SetDefaultsOnWorkType
                    serviceMatter.ClientId = criteria.ClientId;
                    serviceMatter.ValidateClientId(out errorMessage, out warningMessage);
                    serviceMatter.CurrentWorkTypeId = criteria.Id;
                    isValid = serviceMatter.ValidateCurrentWorkTypeId(out errorMessage, out warningMessage);

                    // TODO: Does not use the criteria: FilterString, OrganisationID, DepartmentId, DepartmentNo, IsPrivateClient, MatterTypeId

                    if (isValid)
                    {
                        serviceMatter.SetDefaultsOnWorkType();
                        returnValue.IsPublicFunded = serviceMatter.IsPublicFunding;
                        returnValue.DisbLimit = serviceMatter.WorkTypeDisbLimit;
                        returnValue.OverallLimit = serviceMatter.WorkTypeOverallLimit;
                        returnValue.Quote = serviceMatter.WorkTypeQuote;
                        returnValue.TimeLimit = serviceMatter.WorkTypeTimeLimit;
                        returnValue.WipLimit = serviceMatter.WorkTypeWipLimit;
                        returnValue.ChargeRateDescriptionId = serviceMatter.ChargeDescriptionId;
                        returnValue.Franchised = serviceMatter.WorkCategoryFranchised;
                        returnValue.WorkCategoryUFN = serviceMatter.WorkCategoryUFN;
                        returnValue.ClientHOUCN = serviceMatter.ClientHOUCN;
                        returnValue.ClientUCN = serviceMatter.ClientUCN;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Message = errorMessage;
                    }

                }
                finally
                {
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
        /// This method get the work type for a specified department
        /// </summary>
        /// <param name="logonId">Login id</param>
        /// <param name="collectionRequest">Collection request</param>
        /// <param name="criteria">Search criteria for WorkType</param>
        /// <returns></returns>
        public WorkTypeSearchReturnValue GetWorkTypesForDepartment(Guid logonId, CollectionRequest collectionRequest,
           WorkTypeSearchCriteria criteria)
        {
            WorkTypeSearchReturnValue returnValue = new WorkTypeSearchReturnValue();

            try
            {
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

                    // Create a data list creator for a list of matters
                    DataListCreator<WorkTypeSearchItem> dataListCreator = new DataListCreator<WorkTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        // TODO: Does not use the criteria: Id, FilterString, OrganisationID, DepartmentId, IsPrivateClient, MatterTypeId, ClientId
                        e.DataSet = SrvWorkTypeLookup.GetWorkTypesByDepartment(criteria.DepartmentNo);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "WorkTypeDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.WorkTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "WorkTypeSearchForDepartment",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "WorkTypeID"),
                            new ImportMapping("Description", "WorkTypeDescription"),
                            new ImportMapping("IsArchived", "WorkTypeArchived"),
                            new ImportMapping("Code","WorkTypeCode")
                            }
                        );
                }
                finally
                {
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

        #region GetClientType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgOrMemID"></param>
        /// <param name="projectId"></param>
        /// <param name="isMember"></param>
        /// <returns></returns>
        private int GetClientType(Guid orgOrMemID, Guid projectId, bool isMember)
        {
            try
            {
                if (orgOrMemID == DataConstants.DummyGuid || projectId == DataConstants.DummyGuid)
                {
                    return 0;
                }
                return SrvClientCommon.GetClientTypeId(orgOrMemID, isMember);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// Validates the UFN number against the specified earner ID
        /// </summary>
        /// <param name="logonId">Login ID</param>
        /// <param name="collectionRequest">Collection request</param>
        /// <param name="criteria">Criteria for </param>
        /// <returns></returns>
        public UFNReturnValue UFNValidation(Guid logonId, Guid earnerId, DateTime UFNDate, string UFNNumber)
        {
            UFNReturnValue returnValue = new UFNReturnValue();

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

                    SrvMatter srvMatter = new SrvMatter();
                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;

                    srvMatter.FeeEarnerMemberId = earnerId;
                    srvMatter.SetDefaultsOnFeeEarnerMemberId();

                    srvMatter.UFNValue = UFNDate.Date.Day.ToString().PadLeft(2, '0') + UFNDate.Date.Month.ToString().PadLeft(2, '0') +
                       UFNDate.Date.Year.ToString().Substring(2, 2) + "/" + UFNNumber;

                    srvMatter.UFNDate = UFNDate;
                    bool isValid = false;

                    if (UFNNumber == null)
                    {
                        isValid = srvMatter.setUniqueFileNumberValuesByDate(out errorMessage, out warningMessage);
                    }
                    else
                    {
                        srvMatter.UFNNumber = UFNNumber;
                        isValid = srvMatter.setUniqueFileNumberValuesByNumber(out errorMessage, out warningMessage);
                    }

                    if (!isValid)
                    {
                        returnValue.Success = false;
                        returnValue.Message = "The system has detected that the UFN reference entered is not valid in this instance. If the user is attempting to edit an existing UFN reference, the system will only permit the next available reference outside of the defined Fee Earner ranges.";
                    }
                    else
                    {
                        returnValue.Id = earnerId;
                        returnValue.Date = srvMatter.UFNDate;
                        returnValue.Number = srvMatter.UFNNumber;
                        returnValue.Value = srvMatter.UFNValue;
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

        #region GetMatterTypeId

        /// <summary>
        /// Gets the matter type id for the project.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public MatterTypeReturnValue GetMatterTypeId(Guid logonId, Guid projectId)
        {
            MatterTypeReturnValue returnValue = new MatterTypeReturnValue();

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
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    returnValue.MatterTypeId = SrvMatterCommon.GetMatterTypeId(projectId);
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
        /// Get details about multiple matters.
        /// NOTE: the MatterSearchItem's returned only contain the ID, Reference and Description.  The other search fields
        /// are not returned but these could be added later.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="ProjectIds">Array of project ids</param>
        /// <returns></returns>
        public MatterSearchReturnValue GetMultipleMatterDetails(Guid logonId, Guid[] ProjectIds)
        {
            MatterSearchReturnValue returnValue = new MatterSearchReturnValue();

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
                            // This is inefficient but this most likely will not be used and if it is they won't be looking for many records
                            foreach (Guid ProjectId in ProjectIds)
                            {
                                if (!SrvMatterCommon.WebAllowedToAccessMatter(ProjectId))
                                    throw new Exception("Access denied");
                            }

                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataListCreator<MatterSearchItem> dataListCreator = new DataListCreator<MatterSearchItem>();

                    returnValue.Matters = new DataList<MatterSearchItem>();

                    returnValue.Matters.FirstRowNumber = 0;

                    foreach (IRIS.Law.PmsCommonData.DsMatter.MatterRow Row in SrvMatterCommon.GetMultipleMatters(ProjectIds).Tables[0].Rows)
                    {
                        MatterSearchItem Item = new MatterSearchItem();

                        Item.Id = Row.ProjectId;
                        Item.Reference = Row.matRef;
                        Item.Description = Row.matDescription;

                        returnValue.Matters.TotalRowCount++;
                        returnValue.Matters.Rows.Add(Item);
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

        /// <summary>
        /// Gets the associations for the matter.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="collectionRequest">The collection request.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public MatterAssociationReturnValue MatterAssociationSearch(Guid logonId, CollectionRequest collectionRequest,
                                                                    MatterAssociationSearchCriteria criteria)
        {
            MatterAssociationReturnValue returnValue = new MatterAssociationReturnValue();

            try
            {
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

                    // Create a data list creator for a list of associations
                    DataListCreator<MatterAssociationSearchItem> dataListCreator = new DataListCreator<MatterAssociationSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        DataSet dataSet = SrvAssociationLookup.GetMatterAssociations(criteria.ProjectId, criteria.ApplicationId);
                        // Perform Sorting on Role
                        DataView dvContacts = new DataView(dataSet.Tables[0]);
                        dvContacts.Sort = "AssociationRoleDescription asc";

                        DataSet dsSorted = new DataSet();
                        dsSorted.Tables.Add(dvContacts.ToTable());
                        e.DataSet = dsSorted;

                        e.DataSet.Tables[0].Columns.Add("Email");
                        e.DataSet.Tables[0].Columns.Add("WorkTel");
                    };

                    // Create the data list
                    DataList<MatterAssociationSearchItem> matterAssociations = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "MatterAssociationSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Role", "AssociationRoleDescription"),
                            new ImportMapping("Title", "PersonTitle"),
                            new ImportMapping("Name", "PersonName"),
                            new ImportMapping("Surname", "PersonSurname"),
                            new ImportMapping("Description", "ProjectMapDescription"),
                            new ImportMapping("MemberId", "MemberId"),
                            new ImportMapping("OrgId", "OrgId")
                            }
                        );

                    DataSet dsMatterAssociations;

                    matterAssociations.Rows.ForEach(delegate(MatterAssociationSearchItem item)
                    {
                        if (item.MemberId == DataConstants.DummyGuid)
                        {
                            dsMatterAssociations = SrvAddressLookup.GetOrganisationAddresses(item.OrgId);
                        }
                        else
                        {
                            dsMatterAssociations = SrvAddressLookup.GetMemberAddresses(item.MemberId);
                        }
                        SrvAddress srvAddress;
                        int intCtr;

                        foreach (DataRow dr in dsMatterAssociations.Tables[0].Rows)
                        {

                            srvAddress = new SrvAddress();
                            if (item.MemberId == DataConstants.DummyGuid)
                            {
                                srvAddress.OrganisationId = item.OrgId;
                            }
                            else
                            {
                                srvAddress.MemberId = item.MemberId;
                            }
                            srvAddress.Load(int.Parse(dr["addressId"].ToString()));

                            for (intCtr = 0; intCtr < srvAddress.AdditionalInfoElements.Count; intCtr++)
                            {
                                AdditionalAddressElement additionalAddressElement = new AdditionalAddressElement();

                                if (srvAddress.AdditionalInfoElements[intCtr].AddressElTypeId == 8)
                                {
                                    item.WorkEmail = srvAddress.AdditionalInfoElements[intCtr].AddressElementText;
                                }

                                if (srvAddress.AdditionalInfoElements[intCtr].AddressElTypeId == 2)
                                {
                                    item.WorkTelephone = srvAddress.AdditionalInfoElements[intCtr].AddressElementText;
                                }


                            }
                        }
                    });


                    returnValue.Associations = matterAssociations;
                }
                finally
                {
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

        #endregion
    }
}

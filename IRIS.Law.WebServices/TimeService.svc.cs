using IRIS.Law.Services;

namespace IRIS.Law.WebServices
{
    using System;
    using System.Data;
    using System.ServiceModel;

    using IRIS.Law.ClientInterfaces.Classes.Accounts;
    using IRIS.Law.PmsCommonData;
    using IRIS.Law.PmsCommonData.Accounts;
    using IRIS.Law.Services.Accounts.Period;
    using IRIS.Law.Services.Pms.AdvocacyTypes;
    using IRIS.Law.Services.Pms.AssociationRole;
    using IRIS.Law.Services.Pms.AttendanceIndividual;
    using IRIS.Law.Services.Pms.Charge;
    using IRIS.Law.Services.Pms.CourtType;
    using IRIS.Law.Services.Pms.Matter;
    using IRIS.Law.Services.Pms.Service;
    using IRIS.Law.Services.Pms.Time;
    using IRIS.Law.Services.Pms.TimeType;
    using IRIS.Law.Services.Pms.UnpostedTime;
    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Time;

    // NOTE: If you change the class name "TimeService" here, you must also update the reference to "TimeService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class TimeService : ITimeService
    {
        #region ITimeService

        #region ChargeRateService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public ChargeRateSearchReturnValue ChargeRateSearch(Guid logonId,
                                                          CollectionRequest collectionRequest)
        {
            ChargeRateSearchReturnValue returnValue = new ChargeRateSearchReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<ChargeRateSearchItem> dataListCreator = new DataListCreator<ChargeRateSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvChargeLookup.GetChargeDescriptions();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "ChargeDescRef");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);

                    };

                    // Create the data list
                    returnValue.ChargeRates = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ChargeRateSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("DescriptionId", "ChargeDescID"),
                            new ImportMapping("DescriptionReference", "ChargeDescRef"),
                            new ImportMapping("Description", "ChargeDesc")
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

        #region ChargeRateOnPublicFundingSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ChargeRateSearchReturnValue ChargeRateOnPublicFundingSearch(Guid logonId,
                                                          CollectionRequest collectionRequest,
                                                          ChargeRateSearchCriteria criteria)
        {
            ChargeRateSearchReturnValue returnValue = new ChargeRateSearchReturnValue();

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
                    DataListCreator<ChargeRateSearchItem> dataListCreator = new DataListCreator<ChargeRateSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        // TODO: Does not use the criteria: IsLegalAid
                        e.DataSet = SrvChargeLookup.GetChargeDescriptions(criteria.IsArchived, criteria.IsPublicFunded);

                    };

                    // Create the data list
                    returnValue.ChargeRates = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ChargeRateOnPublicFundingSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("DescriptionId", "ChargeDescID"),
                            new ImportMapping("DescriptionReference", "ChargeDescRef"),
                            new ImportMapping("Description", "ChargeDesc"),
                            new ImportMapping("CourtId", "ChargeDescCourt")							
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

        #region CourtType
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public CourtTypeReturnValue CourtTypeSearch(Guid logonId, CollectionRequest collectionRequest,
                                                    CourtTypeSearchCriteria criteria)
        {
            CourtTypeReturnValue returnValue = new CourtTypeReturnValue();

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
                    DataListCreator<CourtTypeSearchItem> dataListCreator = new DataListCreator<CourtTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        // TODO: Does not use the criteria: Description
                        e.DataSet = SrvCourtTypelookup.GetCourtTypes(criteria.IncludeArchived);
                    };

                    // Create the data list
                    returnValue.CourtTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "CourtTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "CourtID"),
                            new ImportMapping("Description", "CourtDescription")
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

        #region Time Type Search

        public TimeTypeSearchReturnValue TimeTypeSearch(Guid logonId, CollectionRequest collectionRequest,
                                                        TimeTypeSearchCriteria criteria)
        {
            TimeTypeSearchReturnValue returnValue = new TimeTypeSearchReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of time types
                    DataListCreator<TimeTypeSearchItem> dataListCreator = new DataListCreator<TimeTypeSearchItem>();

                    //Gets the matter type based on the project id and then sets isChargeable
                    //which is required to get the time types. 
                    int matterType = SrvMatterCommon.GetMatterTypeId(criteria.ProjectId);
                    bool isChargeable = true;
                    //Firm
                    if (matterType == 6)
                    {
                        isChargeable = false;
                    }

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvTimeTypeLookup.GetTimeTypes(criteria.IncludeArchived, isChargeable);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "TimeTypeDescription");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);

                        foreach (DataRow r in e.DataSet.Tables[0].Rows)
                        {
                            r["TimeTypeDescription"] = r["TimeTypeCode"].ToString().Trim() + " - " + r["TimeTypeDescription"];
                        }
                    };

                    // Create the data list
                    returnValue.TimeTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "TimeTypesSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "TimeTypeID"),
                            new ImportMapping("Description", "TimeTypeDescription"),
                            new ImportMapping("Code", "TimeTypeCode"),
                            new ImportMapping("CatId", "CatID")					
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

        #region Add Time

        public TimeReturnValue AddTime(Guid logonId, Time time, TimeAdditionalDetail additionalTime, bool postToAccounts)
        {
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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    var workflowpostTime = new RecordTime();
                    return workflowpostTime.AddTime(time, additionalTime, postToAccounts);
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
                return new TimeReturnValue { Success = false, Message = Functions.SQLErrorMessage };
            }
            catch (Exception ex)
            {
                return new TimeReturnValue  {Success = false, Message = ex.Message};
            }
        }
        #endregion

        #region SetTimeAdditionalDetails
        private void SetTimeAdditionalDetails(ref SrvTime srvTime, TimeAdditionalDetail timeAdditionalDetail)
        {
            try
            {
                if (timeAdditionalDetail != null)
                {
                    if (timeAdditionalDetail.CurrentAdditionalDetails != AccountsCommon.AdditionalDetails.None)
                    {
                        switch (timeAdditionalDetail.CurrentAdditionalDetails)
                        {
                            case AccountsCommon.AdditionalDetails.Attendance:
                                srvTime.AdditionalDetailsLocation = timeAdditionalDetail.AdditionalDetailsLocation;
                                srvTime.LAId = timeAdditionalDetail.LAId;

                                if (timeAdditionalDetail.AdditionalDetailsLocation == AccountsCommon.DetailLocation.NotApplicable)
                                {
                                    srvTime.LAId = DataConstants.DummyGuid;
                                }

                                srvTime.AttendanceIndividuals.Clear();
                                for (int i = 0; i < timeAdditionalDetail.AttendanceIndividuals.Count; i++)
                                {
                                    int attendanceIndID = Convert.ToInt32(timeAdditionalDetail.AttendanceIndividuals[i]);
                                    srvTime.AttendanceIndividuals.Add(attendanceIndID);
                                }
                                srvTime.HaveAskedLAQuestions = true;
                                break;

                            case AccountsCommon.AdditionalDetails.Advocacy:
                                srvTime.AdditionalDetailsLocation = timeAdditionalDetail.AdditionalDetailsLocation;
                                srvTime.LAId = timeAdditionalDetail.LAId;

                                if (timeAdditionalDetail.AdditionalDetailsLocation == AccountsCommon.DetailLocation.NotApplicable)
                                {
                                    srvTime.LAId = DataConstants.DummyGuid;
                                }

                                srvTime.AdvocacyTypes.Clear();
                                for (int i = 0; i < timeAdditionalDetail.AdvocacyTypes.Count; i++)
                                {
                                    int advocacyTypeID = Convert.ToInt32(timeAdditionalDetail.AdvocacyTypes[i]);
                                    srvTime.AdvocacyTypes.Add(advocacyTypeID);
                                }
                                srvTime.HaveAskedLAQuestions = true;
                                break;

                            case AccountsCommon.AdditionalDetails.Travel:
                                srvTime.Miles = timeAdditionalDetail.Miles;
                                srvTime.FaresDescription = timeAdditionalDetail.FaresDescription;
                                srvTime.FaresAmount = timeAdditionalDetail.FaresAmount;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.PoliceStationCalls:
                                srvTime.AdditionalDetailsLocation = timeAdditionalDetail.AdditionalDetailsLocation;
                                srvTime.LAId = timeAdditionalDetail.LAId;

                                if (timeAdditionalDetail.AdditionalDetailsLocation == AccountsCommon.DetailLocation.NotApplicable)
                                {
                                    srvTime.LAId = DataConstants.DummyGuid;
                                }
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.RunningTime:
                                srvTime.RunningHours = timeAdditionalDetail.RunningHours;
                                srvTime.RunningMinutes = timeAdditionalDetail.RunningMinutes;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.CivilImmigrationAsylumSubstantiveHearing:
                                srvTime.IsLASubstantiveHearingAttend = timeAdditionalDetail.IsLASubstantiveHearingAttend;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.CivilImmigrationAsylumTravelWaiting:
                                srvTime.LAId = timeAdditionalDetail.LAId;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.CivilImmigrationAsylumJRFormFilling:
                                srvTime.LAIsJRFormFilling = timeAdditionalDetail.LAIsJRFormFilling;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.CivilImmigrationAsylumMentalHearing:
                                srvTime.IsLAHearingAdjourned = timeAdditionalDetail.IsLAHearingAdjourned;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.CourtDutyAttendance:
                                srvTime.IsLASuspect = timeAdditionalDetail.LASuspect;
                                srvTime.IsLAYouth = timeAdditionalDetail.IsLAYouth;
                                srvTime.LAId = timeAdditionalDetail.LAId;
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.PoliceStationAttendance:
                                srvTime.AdditionalDetailsLocation = timeAdditionalDetail.AdditionalDetailsLocation;
                                srvTime.LAId = timeAdditionalDetail.LAId;

                                if (timeAdditionalDetail.AdditionalDetailsLocation == AccountsCommon.DetailLocation.NotApplicable)
                                {
                                    srvTime.LAId = DataConstants.DummyGuid;
                                }
                                srvTime.HaveAskedLAQuestions = true;
                                break;
                            case AccountsCommon.AdditionalDetails.FileReviews:
                                srvTime.FileReviewsProjectId = timeAdditionalDetail.FileReviewsProjectId;
                                srvTime.IsFaceToFace = timeAdditionalDetail.IsFaceToFace;
                                if (srvTime.FileReviewsProjectId != DataConstants.DummyGuid)
                                {
                                    srvTime.HaveAskedLAQuestions = true;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UnPostedTimeSheet
        /// <summary>
        /// Get all the unposted time sheet for the current date for the logged in fee earner
        /// </summary>
        /// <param name="loginId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public UnPostedTimeSearchReturnValue UnPostedTimeSheetSearch(Guid logonId,
                                                            CollectionRequest collectionRequest, UnPostedTimeSearchCriteria criteria)
        {
            UnPostedTimeSearchReturnValue returnValue = new UnPostedTimeSearchReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<UnPostedTimeSearchItem> dataListCreator = new DataListCreator<UnPostedTimeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        //Get all the Unposted entries for the Logged in user
                        e.DataSet = SrvUnpostedTimeLookup.GetUnposedTimeForUserDefaultEarner();

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("uvw_UnpostedTime");
                            e.DataSet.Tables.Add(dt);
                        }
                    };


                    // Create the data list
                    returnValue.UnPostedTimeSheet = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "UnPostedTimeSheetSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("TimeId", "TimeId"),
                            new ImportMapping("ProjectId", "ProjectID"),
                            new ImportMapping("MemberId", "MemberID"),
                            new ImportMapping("OrganisationId", "OrgID"),
                            new ImportMapping("TimeDate", "TimeDate"),
                            new ImportMapping("TimeTypeId", "TimeTypeID"),
                            new ImportMapping("TimeTypeCode", "TimeTypeCode"),
                            new ImportMapping("TimeTypeDescription", "TimeTypeDescription"),
                            new ImportMapping("TimeComments", "TimeComment"),
                            new ImportMapping("TimeElapsed", "TimeElapsed"),
                            new ImportMapping("TimeCost", "TimeCost"),
                            new ImportMapping("TimeCharge", "TimeCharge"),
                            new ImportMapping("DepartmentId", "DeptID"),
                            new ImportMapping("CurrencyId", "TimeWorkingCurrencyID"),
                            new ImportMapping("MatterReference", "matRef"),
                            new ImportMapping("MatterDescription", "matDescription"),
                            new ImportMapping("BillingTypeActive", "BillingTypeActive"),
                            new ImportMapping("TimeLAAsked", "TimeLAAsked"),
                            new ImportMapping("BillingTypeArchived", "BillingTypeArchived")

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

        #region Update Time

        public TimeReturnValue UpdateTime(Guid logonId, Time time, TimeAdditionalDetail additionalTime)
        {
            TimeReturnValue returnValue = new TimeReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvTime srvTime = new SrvTime();
                    srvTime.TimeRecordNumber = time.Id;
                    srvTime.Load(srvTime.TimeRecordNumber);

                    string errorMessage;
                    string warningMessage;

                    srvTime.ProjectId = time.ProjectId;
                    srvTime.ValidateProjectId(out errorMessage, out warningMessage);
                    srvTime.SetProjectDefaults();
                    srvTime.EarnerReferenceId = time.FeeEarnerId;
                    srvTime.SetDefaultsForFeeEarnerReferenceId();
                    srvTime.SetDefaultsForChargeRateDescriptionId();
                    srvTime.Units = time.Units;
                    srvTime.Notes = time.Notes;
                    srvTime.CanProceed = time.DontCheckLimits;
                    srvTime.TimeTypeId = time.TimeTypeId;
                    srvTime.ValidateTimeTypeId(out errorMessage, out warningMessage);
                    srvTime.UserId = UserInformation.Instance.DbUid;

                    this.SetTimeAdditionalDetails(ref srvTime, additionalTime);

                    returnValue.Success = srvTime.Save(out errorMessage);
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

        #region Get Time

        public TimeReturnValue GetTime(Guid logonId, int timeId)
        {
            TimeReturnValue returnValue = new TimeReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
                Host.LoadLoggedOnUser(logonId);

                try
                {
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

                    SrvTime srvTime = new SrvTime();
                    srvTime.TimeRecordNumber = timeId;
                    srvTime.Load(srvTime.TimeRecordNumber);

                    Time time = new Time();
                    time.Id = srvTime.TimeRecordNumber;
                    time.ProjectId = srvTime.ProjectId;
                    time.TimeTypeId = srvTime.TimeTypeId;
                    time.Units = srvTime.Units;
                    time.Notes = srvTime.Notes;

                    returnValue.Time = time;
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

        #region ValidatePeriod

        /// <summary>
        /// Validates the period.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public PeriodDetailsReturnValue ValidatePeriod(Guid logonId, PeriodCriteria criteria)
        {
            PeriodDetailsReturnValue returnValue = new PeriodDetailsReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    Guid matterBranchId = DataConstants.DummyGuid;

                    // Gets the branch id by project id
                    if (criteria.ProjectId != DataConstants.DummyGuid || criteria.ProjectId != Guid.Empty)
                    {
                        matterBranchId = SrvMatterCommon.GetMatterBranchGuid(criteria.ProjectId);
                    }

                    PeriodDetails periodDetails = SrvPeriodLookup.ValidatePeriod(criteria.Date,
                                                                                criteria.IsTime,
                                                                                criteria.IsPostingVATable,
                                                                                matterBranchId,
                                                                                criteria.IsAllowedPostBack2ClosedYear);

                    switch (periodDetails.PeriodValidStatus)
                    {
                        case PeriodDetails.PeriodValidStatusValue.Valid:
                            returnValue.Message = periodDetails.PostingPeriodMessage;
                            returnValue.PeriodStatus = 1;
                            break;

                        case PeriodDetails.PeriodValidStatusValue.InvalidFinancialYear:
                        case PeriodDetails.PeriodValidStatusValue.InvalidPeriodClosedPeriod:
                            returnValue.ErrorMessage = periodDetails.PostingPeriodMessage;
                            returnValue.PeriodStatus = 2;
                            break;

                        case PeriodDetails.PeriodValidStatusValue.ValidWithWarningForClosedPeriodTime:
                            returnValue.WarningMessage = periodDetails.PostingPeriodMessage;
                            returnValue.PeriodStatus = 3;
                            break;

                        case PeriodDetails.PeriodValidStatusValue.ValidWithWarningForOpenPeriod:
                            if (PmsCommonData.ApplicationSettings.Instance.AdditionalCompletedPeriodWarning)
                            {
                                returnValue.WarningMessage = "Warning : The selected date falls in a currently completed period.";
                            }
                            returnValue.PeriodStatus = 3;
                            break;

                        case PeriodDetails.PeriodValidStatusValue.InvalidVatPeriod:
                            if (criteria.IsPostingVATable)
                            {
                                returnValue.ErrorMessage = periodDetails.PostingPeriodMessage;
                            }
                            else
                            {
                                returnValue.Message = periodDetails.PostingPeriodMessage;
                            }
                            break;

                        default:
                            returnValue.ErrorMessage = periodDetails.PostingPeriodMessage;
                            returnValue.PeriodStatus = 2;
                            break;
                    }

                    returnValue.PeriodId = periodDetails.PeriodID;
                    returnValue.PostingPeriodNumber = periodDetails.PeriodRef;
                    returnValue.IsValidFinancialPostingPeriod = periodDetails.IsValidFinancialPostingPeriod;
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

        #region ValidateTimeType

        /// <summary>
        /// Validates the time type.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="projectId">projectId</param>
        /// <param name="timeTypeId">timeTypeId</param>
        /// <returns>ReturnValue</returns>
        public ReturnValue ValidateTimeTypeForPosting(Guid logonId, TimeTypeCriteria criteria)
        {
            ReturnValue returnValue = new ReturnValue();
            returnValue.Success = false;
            returnValue.Message = "Unexpected error Validating Time Type";

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;

                    SrvTime timeService = new SrvTime();
                    bool rv = timeService.ValidateTimeType(criteria.ProjectId, criteria.TimeTypeId, criteria.FeeEarnerId, out errorMessage, out warningMessage);

                    returnValue.Success = rv;
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

        #region PostTime
        /// <summary>
        /// Post todays time sheets for accounting 
        /// </summary>
        /// <param name="logonId">User Logon Id</param>
        /// <param name="timeSheet">Fee Earner Time Sheet</param>
        /// <returns></returns>
        public ReturnValue PostTime(Guid logonId, TimeSheet timeSheet)
        {
            ReturnValue returnValue = new ReturnValue();

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
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;
                    var timeAuditDate = new DateTime(1900, 1, 1);

                    SrvTimeCommon.PostTime(timeSheet.TimeId, timeSheet.PeriodId, timeSheet.MemberId,
                                           timeSheet.CurrencyId, timeSheet.PeriodMinutes, timeSheet.MasterPostedCost,
                                           timeSheet.MasterPostedCharge, timeSheet.WorkingPostedCost, timeSheet.WorkingPostedCharge,
                                           timeSheet.OrganisationId, timeSheet.DepartmentId, timeSheet.ProjectId, timeSheet.TimeTypeId,timeSheet.TimeDate,
                                           timeAuditDate);

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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region AdvocacyTypeService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public AdvocacyTypeSearchReturnValue AdvocacyTypeSearch(Guid logonId,
                                                          CollectionRequest collectionRequest)
        {
            AdvocacyTypeSearchReturnValue returnValue = new AdvocacyTypeSearchReturnValue();

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
                    DataListCreator<AdvocacyTypeSearchItem> dataListCreator = new DataListCreator<AdvocacyTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvAdvocacyTypesLookup.GetAdvocacyTypes(false);

                    };

                    // Create the data list
                    returnValue.AdvocacyType = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AdvocacyTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Code", "AdvocacyTypeCode"),
                            new ImportMapping("Desciption", "AdvocacyTypeDesc"),
                            new ImportMapping("Id", "AdvocacyTypeID")
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

        #region AttendanceIndividualSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public AttendanceIndividualSearchReturnValue AttendanceIndividualSearch(Guid logonId,
                                                          CollectionRequest collectionRequest)
        {
            AttendanceIndividualSearchReturnValue returnValue = new AttendanceIndividualSearchReturnValue();

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
                    DataListCreator<AttendanceIndividualSearchItem> dataListCreator = new DataListCreator<AttendanceIndividualSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvAttendanceIndividualLookup.GetAttendanceIndividuals();

                    };

                    // Create the data list
                    returnValue.AttendanceIndividual = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "AttendanceIndividualSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Code", "AttendanceIndCode"),
                            new ImportMapping("Desciption", "AttendanceIndDesc"),
                            new ImportMapping("Id", "AttendanceIndID")
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

        #region ServiceSearchOnIndustry
        /// <summary>
        /// Get service search on industry
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ServiceSearchReturnValue ServiceSearchOnIndustry(Guid logonId, CollectionRequest collectionRequest, ServiceSearchCriteria criteria)
        {
            ServiceSearchReturnValue returnValue = new ServiceSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    // Create a data list creator for a list of matters
                    DataListCreator<ServiceSearchItem> dataListCreator = new DataListCreator<ServiceSearchItem>();

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

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        e.DataSet = SrvServiceLookup.ServiceSearch(criteria.IndustryId, criteria.LSCIDOnly, DataConstants.BlankDate, DataConstants.BlankDate);

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                            e.DataSet.Tables.Add(dt);
                        }

                    };

                    // Create the data list
                    returnValue.Service = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ServiceSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Name", "OrgName"),
                            new ImportMapping("Id", "orgId"),
                            new ImportMapping("AddressHouseName", "AddressHouseName"),
                            new ImportMapping("AddressStreetNo", "AddressStreetNo"),
                            new ImportMapping("AddressLine1", "AddressLine1"),
                            new ImportMapping("AddressTown", "AddressTown"),
                            new ImportMapping("AddressPostcode", "AddressPostCode")
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

        #region ServiceSearch
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <returns></returns>
        public ServiceSearchReturnValue ServiceSearch(Guid logonId, CollectionRequest collectionRequest, int associationRoleId)
        {
            ServiceSearchReturnValue returnValue = new ServiceSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    // Create a data list creator for a list of matters
                    DataListCreator<ServiceSearchItem> dataListCreator = new DataListCreator<ServiceSearchItem>();

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

                    if (associationRoleId != 0)
                    {
                        DsAssociationRolesForIndustry data = new DsAssociationRolesForIndustry();

                        //getting association roles for industry from SrvAssociationRoleLookup
                        data = SrvAssociationRoleLookup.GetAssociationRolesForIndustry(associationRoleId);

                        if (data.uvw_AssociationRolesForIndustry.Count != 0)
                        {
                            int industryId = data.uvw_AssociationRolesForIndustry[0].IndustryID;

                            // Declare an inline event (annonymous delegate) to read the 
                            // dataset if it is required
                            dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                            {
                                // Create the dataset
                                e.DataSet = SrvServiceLookup.ServiceSearch(industryId, true, DataConstants.BlankDate, DataConstants.BlankDate);

                            };

                            // Create the data list
                            returnValue.Service = dataListCreator.Create(logonId,
                                // Give the query a name so it can be cached
                                "ServiceSearch",
                                // Tell it the query criteria used so if the cache is accessed 
                                // again it knows if it is the same query
                                null,
                                collectionRequest,
                                // Import mappings to map the dataset row fields to the data 
                                // list entity properties
                                new ImportMapping[] {
                            new ImportMapping("Name", "OrgName"),
                            new ImportMapping("Id", "orgId")
                            }
                                );
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

        #region ServiceContactSearch
        /// <summary>
        /// Get Contact service search on industry
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public ServiceContactSearchReturnValue ServiceContactSearch(Guid logonId, CollectionRequest collectionRequest, Guid assocParentOrgID, string OrderBy)
        {
            ServiceContactSearchReturnValue returnValue = new ServiceContactSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    // Create a data list creator for a list of matters
                    DataListCreator<ServiceContactSearchItem> dataListCreator = new DataListCreator<ServiceContactSearchItem>();

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

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvServiceLookup.GetServiceContacts(assocParentOrgID);

                        if (OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], OrderBy);
                            e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                            e.DataSet.Tables.Add(dt);
                        }

                    };

                    // Create the data list
                    returnValue.ServiceContact = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ServiceContactSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("PersonTitle", "PersonTitle"),
                            new ImportMapping("PersonName", "PersonName"),
                            new ImportMapping("PersonSurname", "PersonSurname"),
                            new ImportMapping("Position", "AssocDescription"),
                            new ImportMapping("MemberId", "MemberID")
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

        #region GetAddtionalDetailTime

        public TimeReturnValue GetAddtionalDetailTime(Guid logonId, Time time)
        {
            TimeReturnValue returnValue = new TimeReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
                Host.LoadLoggedOnUser(logonId);

                try
                {
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

                    string errorMessage;
                    string warningMessage;

                    SrvTime srvTime = new SrvTime();
                    TimeAdditionalDetail additionalDetail = new TimeAdditionalDetail();
                    additionalDetail.CurrentAdditionalDetails = AccountsCommon.AdditionalDetails.None;

                    if (time.Id != 0)
                    {
                        srvTime.TimeRecordNumber = time.Id;
                        srvTime.Load(srvTime.TimeRecordNumber);
                    }

                    srvTime.ProjectId = time.ProjectId;
                    srvTime.SetProjectDefaults();
                    srvTime.EarnerReferenceId = time.FeeEarnerId;
                    srvTime.SetDefaultsForFeeEarnerReferenceId();
                    srvTime.SetDefaultsForChargeRateDescriptionId();
                    srvTime.TimeTypeId = time.TimeTypeId;
                    srvTime.ValidateTimeTypeId(out errorMessage, out warningMessage);
                    srvTime.UserId = UserInformation.Instance.DbUid;

                    additionalDetail.CurrentAdditionalDetails = srvTime.CurrentAdditionalDetails;

                    // Attendance, Advocacy, Police Station Calls Contacted & Police Station Attendance
                    additionalDetail.AdditionalDetailsLocation = srvTime.AdditionalDetailsLocation;
                    additionalDetail.AttendanceIndividuals = srvTime.AttendanceIndividuals;
                    additionalDetail.AdvocacyTypes = srvTime.AdvocacyTypes;
                    additionalDetail.LAId = srvTime.LAId;

                    // Travel
                    additionalDetail.Miles = srvTime.Miles;
                    additionalDetail.FaresAmount = Decimal.Round(srvTime.FaresAmount, 2);
                    additionalDetail.FaresDescription = srvTime.FaresDescription;

                    // Running Time
                    additionalDetail.RunningHours = srvTime.RunningHours;
                    additionalDetail.RunningMinutes = srvTime.RunningMinutes;

                    // ImmigrationAsylum
                    additionalDetail.LAIsJRFormFilling = srvTime.LAIsJRFormFilling;

                    // AttendanceImmigrationAsylum
                    additionalDetail.IsLASubstantiveHearingAttend = srvTime.IsLASubstantiveHearingAttend;

                    // HearingImmigrationAsylumMental
                    additionalDetail.IsLAHearingAdjourned = srvTime.IsLAHearingAdjourned;

                    // TravelWaitingImmigrationAsylum
                    additionalDetail.IsTravelWaitingDetCentre = srvTime.IsTravelWaitingDetCentre;

                    // CivilImmigrationAsylumJRFormFilling
                    additionalDetail.LAIsJRFormFilling = srvTime.LAIsJRFormFilling;

                    // CivilImmigrationAsylumMentalHearing
                    additionalDetail.IsLAHearingAdjourned = srvTime.IsLAHearingAdjourned;

                    // CivilImmigrationAsylumSubstantiveHearing
                    additionalDetail.IsLASubstantiveHearingAttend = srvTime.IsLASubstantiveHearingAttend;

                    // CourtDutyAttendance
                    additionalDetail.LASuspect = srvTime.IsLASuspect;
                    additionalDetail.IsLAYouth = srvTime.IsLAYouth;

                    // FileReviews
                    additionalDetail.IsFaceToFace = srvTime.IsFaceToFace;
                    additionalDetail.FileReviewsProjectId = srvTime.FileReviewsProjectId;

                    returnValue.Time = time;
                    returnValue.AdditionalDetail = additionalDetail;
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

        #endregion
    }
}

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
using System.Collections.Specialized;
using IRIS.Law.Services.Pms.Client;
using System.Data;
using IRIS.Law.Services.Pms.Association;
using IRIS.Law.Services.Pms.Address;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Utilities;
using IRIS.Law.Services.Pms.User;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.Services.Pms.Branch;
using IRIS.Law.Services.Pms.Organisation;
using IRIS.Law.Services.Pms.Department;
using IRIS.Law.Services.Pms.Earner;
using IRIS.Law.Services.Pms.Person;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "UtilitiesService" here, you must also update the reference to "UtilitiesService" in Web.config and in the associated .svc file.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class UtilitiesService : IUtilitiesService
    {
        #region IUtilitiesService Members

        /// <summary>
        /// Search for a user
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public UserSearchReturnValue UserSearch(Guid logonId, CollectionRequest collectionRequest,
            IRIS.Law.WebServiceInterfaces.Utilities.UserSearchCriteria criteria)
        {
            IRIS.Law.WebServiceInterfaces.Utilities.UserSearchReturnValue returnValue = new IRIS.Law.WebServiceInterfaces.Utilities.UserSearchReturnValue();

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


                    // Create a data list creator for a list of Users
                    DataListCreator<IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem> dataListCreator = new DataListCreator<IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvUserLookup.UserSearch(criteria.UserType, criteria.Name);
                    };

                    // Create the data list
                    returnValue.Users = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "UserSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("UserId", "uid"),
                            new ImportMapping("UserName", "name"),
                            new ImportMapping("Forename", "PersonName"),
                            new ImportMapping("Surname", "PersonSurname")
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
        /// Get details about multiple Users.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service.</param>
        /// <param name="UserIds">Array of user ids</param>
        /// <returns></returns>
        public UserSearchReturnValue GetMultipleUserDetails(Guid logonId, int[] UserIds)
        {
            IRIS.Law.WebServiceInterfaces.Utilities.UserSearchReturnValue returnValue = new IRIS.Law.WebServiceInterfaces.Utilities.UserSearchReturnValue();

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
                            if (UserIds.Length != 1)
                                throw new Exception("Access denied");

                            if (UserInformation.Instance.DbUid != UserIds[0])
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            // Can only access themself
                            if (UserIds.Length != 1)
                                throw new Exception("Access denied");

                            if (UserInformation.Instance.DbUid != UserIds[0])
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    DataListCreator<IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem> dataListCreator = new DataListCreator<IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem>();

                    returnValue.Users = new DataList<IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem>();

                    returnValue.Users.FirstRowNumber = 0;

                    foreach (IRIS.Law.PmsCommonData.DsUsersLookup.uvw_UsersRow Row in
                        SrvUserLookup.GetMultipleUsers(UserIds).Tables[0].Rows)
                    {
                        IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem Item = new IRIS.Law.WebServiceInterfaces.Utilities.UserSearchItem();

                        Item.UserId = Row.uid;
                        Item.UserName = Row.name;
                        Item.Forename = Row.PersonName;
                        Item.Surname = Row.PersonSurname;

                        returnValue.Users.TotalRowCount++;
                        returnValue.Users.Rows.Add(Item);
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

        #region GetUser

        public UserReturnValue GetUser(Guid logonId, Guid memberId)
        {
            //UserReturnValue userReturnValue = new UserReturnValue();
            DsSystemUsersAll systemUserReturnValue = new DsSystemUsersAll();
            UserReturnValue userReturnValue = new UserReturnValue();
            userReturnValue.User = new User();
            //DsBranches branchReturnValue = new DsBranches();
            //OrganisationReturnValue organisationReturnValue = new OrganisationReturnValue();
            //dsDepartments departmentReturnValue = new dsDepartments();
            //DsWorkTypes workTypeReturnValue = new DsWorkTypes();
            //EarnerReturnValue earnerReturnValue = new EarnerReturnValue();
            //DsPersons personReturnValue = new DsPersons();

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

                    systemUserReturnValue = SrvUserCommon.GetSystemUser();

                    userReturnValue.User.Forename = systemUserReturnValue.Tables[0].Rows[0]["Forename"].ToString();
                    userReturnValue.User.Lastname = systemUserReturnValue.Tables[0].Rows[0]["Lastname"].ToString();
                    userReturnValue.User.MemOrOrgId = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["MemOrOrgId"].ToString());
                    userReturnValue.User.Name = systemUserReturnValue.Tables[0].Rows[0]["Name"].ToString();
                    userReturnValue.User.Title = systemUserReturnValue.Tables[0].Rows[0]["Title"].ToString();
                    userReturnValue.User.Uid = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["Uid"].ToString());
                    userReturnValue.User.UserDefaBranch = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaBranch"].ToString());
                    userReturnValue.User.UserDefaDepartment = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaDepartment"].ToString());
                    userReturnValue.User.UserDefaEarner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaEarner"].ToString());
                    userReturnValue.User.UserDefaPartner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaPartner"].ToString());
                    userReturnValue.User.UserDefaWorkType = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaWorkType"].ToString());
                    userReturnValue.User.UserType = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserType"].ToString());

                    
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
                userReturnValue.Success = false;
                userReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                userReturnValue.Success = false;
                userReturnValue.Message = ex.Message;
            }

            return userReturnValue;
        }

        public UserReturnValue GetUserByUid(Guid logonId, int uid)
        {
            //UserReturnValue userReturnValue = new UserReturnValue();
            DsSystemUsers systemUserReturnValue = new DsSystemUsers();
            UserReturnValue userReturnValue = new UserReturnValue();
            userReturnValue.User = new User();
            //DsBranches branchReturnValue = new DsBranches();
            //OrganisationReturnValue organisationReturnValue = new OrganisationReturnValue();
            //dsDepartments departmentReturnValue = new dsDepartments();
            //DsWorkTypes workTypeReturnValue = new DsWorkTypes();
            //EarnerReturnValue earnerReturnValue = new EarnerReturnValue();
            //DsPersons personReturnValue = new DsPersons();

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

                    systemUserReturnValue = SrvUserLookup.GetUser(uid);//SrvUserCommon.GetSystemUser();

                    userReturnValue.User.Forename = systemUserReturnValue.Tables[0].Rows[0]["PersonName"].ToString();
                    userReturnValue.User.Lastname = systemUserReturnValue.Tables[0].Rows[0]["PersonSurname"].ToString();
                    userReturnValue.User.MemOrOrgId = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["MemberID"].ToString());
                    userReturnValue.User.Name = systemUserReturnValue.Tables[0].Rows[0]["name"].ToString();
                    userReturnValue.User.Title = systemUserReturnValue.Tables[0].Rows[0]["PersonTitle"].ToString();
                    userReturnValue.User.Uid = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["uid"].ToString());
                    userReturnValue.User.UserDefaBranch = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaBranch"].ToString());
                    userReturnValue.User.UserDefaDepartment = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaDepartment"].ToString());
                    userReturnValue.User.UserDefaEarner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaEarner"].ToString());
                    userReturnValue.User.UserDefaPartner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaPartner"].ToString());
                    userReturnValue.User.UserDefaWorkType = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaWorkType"].ToString());
                    userReturnValue.User.UserType = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserType"].ToString());

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
                userReturnValue.Success = false;
                userReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                userReturnValue.Success = false;
                userReturnValue.Message = ex.Message;
            }

            return userReturnValue;
        }

        #endregion GetUser

        #region GetWorkType

        public WorkTypeReturnValue GetWorkType(Guid logonId, Guid workTypeId)
        {
            //UserReturnValue userReturnValue = new UserReturnValue();
            DsWorkTypes dsWorkTypeReturnValue = new DsWorkTypes();
            WorkTypeReturnValue workTypeReturnValue = new WorkTypeReturnValue();
            workTypeReturnValue.WorkType = new WorkType();
            //DsBranches branchReturnValue = new DsBranches();
            //OrganisationReturnValue organisationReturnValue = new OrganisationReturnValue();
            //dsDepartments departmentReturnValue = new dsDepartments();
            //DsWorkTypes workTypeReturnValue = new DsWorkTypes();
            //EarnerReturnValue earnerReturnValue = new EarnerReturnValue();
            //DsPersons personReturnValue = new DsPersons();

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

                    dsWorkTypeReturnValue = SrvWorkTypeLookup.GetWorkTypesOnWorkTypeId(workTypeId);

                    workTypeReturnValue.WorkType.ApplicationID = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["ApplicationID"].ToString());
                    workTypeReturnValue.WorkType.BillingCodeCategoryID = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["BillingCodeCategoryID"].ToString());
                    workTypeReturnValue.WorkType.ChargeDescID = Guid.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["ChargeDescID"].ToString());
                    workTypeReturnValue.WorkType.CLHeaderID = Guid.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["CLHeaderID"].ToString());
                    workTypeReturnValue.WorkType.DeptNo = dsWorkTypeReturnValue.Tables[0].Rows[0]["DeptNo"].ToString();
                    workTypeReturnValue.WorkType.ExtendedInfoTypeDefaultCatID = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["ExtendedInfoTypeDefaultCatID"].ToString());
                    workTypeReturnValue.WorkType.ExtInfoTypeDefaultCatName = dsWorkTypeReturnValue.Tables[0].Rows[0]["ExtInfoTypeDefaultCatName"].ToString();
                    workTypeReturnValue.WorkType.NominalID = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["NominalID"].ToString());
                    workTypeReturnValue.WorkType.WorkCatID = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkCatID"].ToString());
                    workTypeReturnValue.WorkType.WorkTypeArchived = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeArchived"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeCode = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeCode"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeDept = int.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeDept"].ToString());
                    workTypeReturnValue.WorkType.WorkTypeDescription = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeDescription"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeDisbLimit = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeDisbLimit"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeID = Guid.Parse(dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeID"].ToString());
                    workTypeReturnValue.WorkType.WorkTypeIsLA = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeIsLA"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeOverallLimit = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeOverallLimit"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeQuote = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeQuote"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeTimeLimit = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeTimeLimit"].ToString();
                    workTypeReturnValue.WorkType.WorkTypeWipLimit = dsWorkTypeReturnValue.Tables[0].Rows[0]["WorkTypeWipLimit"].ToString();
                    
                    //userReturnValue.User.Lastname = systemUserReturnValue.Tables[0].Rows[0]["Lastname"].ToString();
                    //userReturnValue.User.MemOrOrgId = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["MemOrOrgId"].ToString());
                    //userReturnValue.User.Name = systemUserReturnValue.Tables[0].Rows[0]["Name"].ToString();
                    //userReturnValue.User.Title = systemUserReturnValue.Tables[0].Rows[0]["Title"].ToString();
                    //userReturnValue.User.Uid = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["Uid"].ToString());
                    //userReturnValue.User.UserDefaBranch = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaBranch"].ToString());
                    //userReturnValue.User.UserDefaDepartment = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaDepartment"].ToString());
                    //userReturnValue.User.UserDefaEarner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaEarner"].ToString());
                    //userReturnValue.User.UserDefaPartner = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaPartner"].ToString());
                    //userReturnValue.User.UserDefaWorkType = Guid.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserDefaWorkType"].ToString());
                    //userReturnValue.User.UserType = int.Parse(systemUserReturnValue.Tables[0].Rows[0]["UserType"].ToString());


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
                workTypeReturnValue.Success = false;
                workTypeReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                workTypeReturnValue.Success = false;
                workTypeReturnValue.Message = ex.Message;
            }

            return workTypeReturnValue;
        }

        #endregion GetWorkType

        #region GetDBVersion
        public DBVersionReturnValue GetDBVersion(Guid logonId)
        {
            DBVersionReturnValue dbVersion = new DBVersionReturnValue();

            try
            {
                Host.LoadLoggedOnUser(logonId);
                dbVersion.DBVersion = ApplicationSettings.Instance.DbVersion;
            }
            catch (Exception ex)
            {
                dbVersion.Success = false;
                dbVersion.Message = ex.Message;
            }
            finally
            {
                Host.UnloadLoggedOnUser();
            }

            return dbVersion;
        }
        #endregion GetDBVersion

        #endregion
    }
}

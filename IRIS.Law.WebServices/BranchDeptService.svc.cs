using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.Services.Pms.Branch;
using IRIS.Law.Services.Pms.Department;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.PmsCommonData;
using IRIS.Law.WebServiceInterfaces.Matter;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Branch;
using IRIS.Law.WebServiceInterfaces.Department;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "BranchDeptService" here, you must also update the reference to "BranchDeptService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class BranchDeptService : IBranchDeptService
    {
        #region IBranchDeptService Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public BranchSearchReturnValue BranchSearch(Guid logonId, CollectionRequest collectionRequest)
        {
            BranchSearchReturnValue returnValue = new BranchSearchReturnValue();

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
                    DataListCreator<BranchSearchItem> dataListCreator = new DataListCreator<BranchSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Create the dataset
                        //e.DataSet = service layer routine to do the query
                        e.DataSet = SrvBranchLookup.GetBranchLookup();

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "OrgName");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);

                        foreach (DataRow r in e.DataSet.Tables[0].Rows)
                        {
                            r["branchRef"] = r["branchRef"].ToString().Trim();
                            r["OrgName"] = r["branchRef"].ToString().Trim() + " - " + r["OrgName"];
                        }
                    };

                    // Create the data list
                    returnValue.Branches = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "BranchSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        null,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("OrganisationId", "OrgID"),
                            new ImportMapping("Reference", "branchRef"),
                            new ImportMapping("Name", "OrgName"),
                            new ImportMapping("IsLondonRate", "branchLondonRate")
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
        /// Search departments based on the organisation ID
        /// </summary>
        /// <param name="logonId">The logon id</param>
        /// <param name="collectionRequest">Collection request value</param>
        /// <param name="criteria">Department search criteria</param>
        /// <returns>Collection of department search item</returns>
        public DepartmentSearchReturnValue DepartmentSearch(Guid logonId, CollectionRequest collectionRequest,
            DepartmentSearchCriteria criteria)
        {
            DepartmentSearchReturnValue returnValue = new DepartmentSearchReturnValue();

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
                    DataListCreator<DepartmentSearchItem> dataListCreator = new DataListCreator<DepartmentSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        if (criteria.AllDepartment)
                        {
                            // Create the dataset for department search to get all departments
                            e.DataSet = SrvDepartmentLookup.GetDepartments();
                        }
                        else
                        {
                            // Create the dataset
                            e.DataSet = SrvDepartmentLookup.GetDepartments(criteria.OrganisationId, criteria.IncludeArchived);
                        }

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "deptName");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Departments = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "DepartmentSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "deptID"),
                            new ImportMapping("No", "deptNo"),
                            new ImportMapping("Name", "deptName"),
                            new ImportMapping("IsArchived", "deptArchived")
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
        /// Gets the client bank Id, office bank Id and partner member Id based on the branch and dept id
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="branchOrganisationId">The branch organisation id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns></returns>
        public MatterReturnValue GetBranchDepartmentDefaults(Guid logonId, Guid branchOrganisationId,
                                                int departmentId)
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

                    returnValue.Matter = new Matter();
                    DsBranchDept dsBranchDept = SrvBranchLookup.GetBranchDepts(branchOrganisationId, departmentId);
                    if (dsBranchDept.BranchDept.Rows.Count != 0)
                    {
                        returnValue.Matter.ClientBankId = dsBranchDept.BranchDept[0].bankIdClient;
                        returnValue.Matter.OfficeBankId = dsBranchDept.BranchDept[0].bankIdOffice;
                        returnValue.Matter.PartnerMemberId = dsBranchDept.BranchDept[0].MemberId;
                    }
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

        #region GetBranch

        public BranchReturnValue GetBranch(Guid logonId, Guid orgId)
        {
            DsBranches dsBranchReturnValue = new DsBranches();
            BranchReturnValue branchReturnValue = new BranchReturnValue();
            branchReturnValue.Branch = new Branch();
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

                    dsBranchReturnValue = SrvBranchLookup.GetBranch(orgId);

                    branchReturnValue.Branch.BranchArchived = dsBranchReturnValue.Tables[0].Rows[0]["BranchArchived"].ToString();
                    branchReturnValue.Branch.BranchCdsReg = int.Parse(dsBranchReturnValue.Tables[0].Rows[0]["BranchCdsReg"].ToString());
                    branchReturnValue.Branch.BranchCdsVersion = dsBranchReturnValue.Tables[0].Rows[0]["BranchCdsVersion"].ToString();
                    branchReturnValue.Branch.BranchCivilContractNo = dsBranchReturnValue.Tables[0].Rows[0]["BranchCivilContractNo"].ToString();
                    branchReturnValue.Branch.BranchCompanyName = dsBranchReturnValue.Tables[0].Rows[0]["BranchCompanyName"].ToString();
                    branchReturnValue.Branch.BranchCreditAccountKey = dsBranchReturnValue.Tables[0].Rows[0]["BranchCreditAccountKey"].ToString();
                    branchReturnValue.Branch.BranchLondonRate = dsBranchReturnValue.Tables[0].Rows[0]["BranchLondonRate"].ToString();
                    branchReturnValue.Branch.BranchLSCYear = dsBranchReturnValue.Tables[0].Rows[0]["BranchLSCYear"].ToString();
                    branchReturnValue.Branch.BranchNetId = dsBranchReturnValue.Tables[0].Rows[0]["BranchNetId"].ToString();
                    branchReturnValue.Branch.BranchNoVAT = dsBranchReturnValue.Tables[0].Rows[0]["BranchNoVAT"].ToString();
                    branchReturnValue.Branch.BranchPsiId = dsBranchReturnValue.Tables[0].Rows[0]["BranchPsiId"].ToString();
                    branchReturnValue.Branch.BranchRef = dsBranchReturnValue.Tables[0].Rows[0]["BranchRef"].ToString();
                    branchReturnValue.Branch.BranchStarsId = dsBranchReturnValue.Tables[0].Rows[0]["BranchStarsId"].ToString();
                    branchReturnValue.Branch.BranchSupNum1 = dsBranchReturnValue.Tables[0].Rows[0]["BranchSupNum1"].ToString();
                    branchReturnValue.Branch.BranchVatPeriodEnd = dsBranchReturnValue.Tables[0].Rows[0]["BranchVatPeriodEnd"].ToString();
                    branchReturnValue.Branch.BranchCivilUnifiedContract = dsBranchReturnValue.Tables[0].Rows[0]["BranchCivilUnifiedContract"].ToString();

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
                branchReturnValue.Success = false;
                branchReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                branchReturnValue.Success = false;
                branchReturnValue.Message = ex.Message;
            }

            return branchReturnValue;
        }

        #endregion GetBranch

        #region GetDepartment

        public DepartmentReturnValue GetDepartment(Guid logonId, int deptId)
        {
            dsDepartments dsDepartmentReturnValue = new dsDepartments();
            DepartmentReturnValue departmentReturnValue = new DepartmentReturnValue();
            departmentReturnValue.Department = new Department();
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

                    dsDepartmentReturnValue = SrvDepartmentLookup.GetDepartMentOnDepartmentId(deptId);

                    departmentReturnValue.Department.DeptArchived = dsDepartmentReturnValue.Tables[0].Rows[0]["DeptArchived"].ToString();
                    departmentReturnValue.Department.DeptId = int.Parse(dsDepartmentReturnValue.Tables[0].Rows[0]["DeptId"].ToString());
                    departmentReturnValue.Department.DeptName = dsDepartmentReturnValue.Tables[0].Rows[0]["DeptName"].ToString();
                    departmentReturnValue.Department.DeptNo = dsDepartmentReturnValue.Tables[0].Rows[0]["DeptNo"].ToString();

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
                departmentReturnValue.Success = false;
                departmentReturnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                departmentReturnValue.Success = false;
                departmentReturnValue.Message = ex.Message;
            }

            return departmentReturnValue;
        }

        #endregion GetDepartment
        #endregion
    }
}

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
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.BranchDept;
using IRIS.Law.WebServiceInterfaces.Branch;
using IRIS.Law.WebServiceInterfaces.Department;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "BranchDeptService" here, you must also update the reference to "BranchDeptService" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.BranchDeptServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_BranchDeptServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class BranchDeptServiceIWS : IBranchDeptServiceIWS
    {
        #region IBranchDeptService Members
        BranchDeptService oBranchDeptService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public BranchSearchReturnValue BranchSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            BranchSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBranchDeptService = new BranchDeptService();
                returnValue = oBranchDeptService.BranchSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new BranchSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Search departments based on the organisation ID
        /// </summary>
        /// <param name="oHostSecurityToken">The Host Security Token from IWS</param>
        /// <param name="collectionRequest">Collection request value</param>
        /// <param name="criteria">Department search criteria</param>
        /// <returns>Collection of department search item</returns>
        public DepartmentSearchReturnValue DepartmentSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            DepartmentSearchCriteria criteria)
        {
            DepartmentSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBranchDeptService = new BranchDeptService();
                returnValue = oBranchDeptService.DepartmentSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new DepartmentSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the client bank Id, office bank Id and partner member Id based on the branch and dept id
        /// </summary>
        /// <param name="oHostSecurityToken">The Host Security Token from IWS</param>
        /// <param name="branchOrganisationId">The branch organisation id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns></returns>
        public MatterReturnValue GetBranchDepartmentDefaults(HostSecurityToken oHostSecurityToken, Guid branchOrganisationId,
                                                int departmentId)
        {
            MatterReturnValue returnValue = new MatterReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBranchDeptService = new BranchDeptService();
                returnValue = oBranchDeptService.GetBranchDepartmentDefaults(Functions.GetLogonIdFromToken(oHostSecurityToken), branchOrganisationId, departmentId);
            }
            else
            {
                returnValue = new MatterReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the client bank Id, office bank Id and partner member Id based on the branch and dept id
        /// </summary>
        /// <param name="oHostSecurityToken">The Host Security Token from IWS</param>
        /// <param name="branchOrganisationId">The branch organisation id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns></returns>
        public BranchReturnValue GetBranch(HostSecurityToken oHostSecurityToken, Guid orgId)
        {
            BranchReturnValue branchReturnValue = new BranchReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBranchDeptService = new BranchDeptService();
                branchReturnValue = oBranchDeptService.GetBranch(Functions.GetLogonIdFromToken(oHostSecurityToken), orgId);
            }
            else
            {
                branchReturnValue = new BranchReturnValue();
                branchReturnValue.Success = false;
                branchReturnValue.Message = "Invalid Token";
            }
            return branchReturnValue;
        }

        /// <summary>
        /// Gets the client bank Id, office bank Id and partner member Id based on the branch and dept id
        /// </summary>
        /// <param name="oHostSecurityToken">The Host Security Token from IWS</param>
        /// <param name="branchOrganisationId">The branch organisation id.</param>
        /// <param name="departmentId">The department id.</param>
        /// <returns></returns>
        public DepartmentReturnValue GetDepartment(HostSecurityToken oHostSecurityToken, int deptId)
        {
            DepartmentReturnValue departmentReturnValue = new DepartmentReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBranchDeptService = new BranchDeptService();
                departmentReturnValue = oBranchDeptService.GetDepartment(Functions.GetLogonIdFromToken(oHostSecurityToken), deptId);
            }
            else
            {
                departmentReturnValue = new DepartmentReturnValue();
                departmentReturnValue.Success = false;
                departmentReturnValue.Message = "Invalid Token";
            }
            return departmentReturnValue;
        }

        
        #endregion
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IRIS.Law.WebServiceInterfaces
{
    using System.Runtime.Serialization;
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DataListOfBranchSearchItemtSdlgeiO", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces")]
    public partial class DataListOfBranchSearchItemtSdlgeiO : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int FirstRowNumberField;
        
        private IRIS.Law.WebServiceInterfaces.BranchDept.BranchSearchItem[] RowsField;
        
        private int TotalRowCountField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FirstRowNumber
        {
            get
            {
                return this.FirstRowNumberField;
            }
            set
            {
                this.FirstRowNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.BranchDept.BranchSearchItem[] Rows
        {
            get
            {
                return this.RowsField;
            }
            set
            {
                this.RowsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalRowCount
        {
            get
            {
                return this.TotalRowCountField;
            }
            set
            {
                this.TotalRowCountField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DataListOfDepartmentSearchItemtSdlgeiO", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces")]
    public partial class DataListOfDepartmentSearchItemtSdlgeiO : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int FirstRowNumberField;
        
        private IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchItem[] RowsField;
        
        private int TotalRowCountField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int FirstRowNumber
        {
            get
            {
                return this.FirstRowNumberField;
            }
            set
            {
                this.FirstRowNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchItem[] Rows
        {
            get
            {
                return this.RowsField;
            }
            set
            {
                this.RowsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TotalRowCount
        {
            get
            {
                return this.TotalRowCountField;
            }
            set
            {
                this.TotalRowCountField = value;
            }
        }
    }
}
namespace IRIS.Law.WebServiceInterfaces.Department
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DepartmentReturnValue", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Department")]
    public partial class DepartmentReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {
        
        private IRIS.Law.WebServiceInterfaces.Department.Department DepartmentField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.Department.Department Department
        {
            get
            {
                return this.DepartmentField;
            }
            set
            {
                this.DepartmentField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Department", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Department")]
    public partial class Department : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string DeptArchivedField;
        
        private int DeptIdField;
        
        private string DeptNameField;
        
        private string DeptNoField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeptArchived
        {
            get
            {
                return this.DeptArchivedField;
            }
            set
            {
                this.DeptArchivedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DeptId
        {
            get
            {
                return this.DeptIdField;
            }
            set
            {
                this.DeptIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeptName
        {
            get
            {
                return this.DeptNameField;
            }
            set
            {
                this.DeptNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeptNo
        {
            get
            {
                return this.DeptNoField;
            }
            set
            {
                this.DeptNoField = value;
            }
        }
    }
}
namespace IRIS.Law.WebServiceInterfaces.Branch
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BranchReturnValue", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Branch")]
    public partial class BranchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {
        
        private IRIS.Law.WebServiceInterfaces.Branch.Branch BranchField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.Branch.Branch Branch
        {
            get
            {
                return this.BranchField;
            }
            set
            {
                this.BranchField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Branch", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Branch")]
    public partial class Branch : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string BranchArchivedField;
        
        private int BranchCdsRegField;
        
        private string BranchCdsVersionField;
        
        private string BranchCivilContractNoField;
        
        private string BranchCompanyNameField;
        
        private string BranchCreditAccountKeyField;
        
        private string BranchLSCYearField;
        
        private string BranchLondonRateField;
        
        private string BranchNetIdField;
        
        private string BranchNoVATField;
        
        private string BranchPsiIdField;
        
        private string BranchRefField;
        
        private string BranchStarsIdField;
        
        private string BranchSupNum1Field;
        
        private string BranchVatPeriodEndField;
        
        private System.Guid OrgIdField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchArchived
        {
            get
            {
                return this.BranchArchivedField;
            }
            set
            {
                this.BranchArchivedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BranchCdsReg
        {
            get
            {
                return this.BranchCdsRegField;
            }
            set
            {
                this.BranchCdsRegField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchCdsVersion
        {
            get
            {
                return this.BranchCdsVersionField;
            }
            set
            {
                this.BranchCdsVersionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchCivilContractNo
        {
            get
            {
                return this.BranchCivilContractNoField;
            }
            set
            {
                this.BranchCivilContractNoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchCompanyName
        {
            get
            {
                return this.BranchCompanyNameField;
            }
            set
            {
                this.BranchCompanyNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchCreditAccountKey
        {
            get
            {
                return this.BranchCreditAccountKeyField;
            }
            set
            {
                this.BranchCreditAccountKeyField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchLSCYear
        {
            get
            {
                return this.BranchLSCYearField;
            }
            set
            {
                this.BranchLSCYearField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchLondonRate
        {
            get
            {
                return this.BranchLondonRateField;
            }
            set
            {
                this.BranchLondonRateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchNetId
        {
            get
            {
                return this.BranchNetIdField;
            }
            set
            {
                this.BranchNetIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchNoVAT
        {
            get
            {
                return this.BranchNoVATField;
            }
            set
            {
                this.BranchNoVATField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchPsiId
        {
            get
            {
                return this.BranchPsiIdField;
            }
            set
            {
                this.BranchPsiIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchRef
        {
            get
            {
                return this.BranchRefField;
            }
            set
            {
                this.BranchRefField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchStarsId
        {
            get
            {
                return this.BranchStarsIdField;
            }
            set
            {
                this.BranchStarsIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchSupNum1
        {
            get
            {
                return this.BranchSupNum1Field;
            }
            set
            {
                this.BranchSupNum1Field = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchVatPeriodEnd
        {
            get
            {
                return this.BranchVatPeriodEndField;
            }
            set
            {
                this.BranchVatPeriodEndField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid OrgId
        {
            get
            {
                return this.OrgIdField;
            }
            set
            {
                this.OrgIdField = value;
            }
        }
    }
}
namespace IRIS.Law.WebServiceInterfaces.BranchDept
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DepartmentSearchReturnValue", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.BranchDept")]
    public partial class DepartmentSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {
        
        private IRIS.Law.WebServiceInterfaces.DataListOfDepartmentSearchItemtSdlgeiO DepartmentsField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfDepartmentSearchItemtSdlgeiO Departments
        {
            get
            {
                return this.DepartmentsField;
            }
            set
            {
                this.DepartmentsField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BranchSearchReturnValue", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.BranchDept")]
    public partial class BranchSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {
        
        private IRIS.Law.WebServiceInterfaces.DataListOfBranchSearchItemtSdlgeiO BranchesField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfBranchSearchItemtSdlgeiO Branches
        {
            get
            {
                return this.BranchesField;
            }
            set
            {
                this.BranchesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DepartmentSearchItem", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.BranchDept")]
    public partial class DepartmentSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int IdField;
        
        private bool IsArchivedField;
        
        private string NameField;
        
        private string NoField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id
        {
            get
            {
                return this.IdField;
            }
            set
            {
                this.IdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsArchived
        {
            get
            {
                return this.IsArchivedField;
            }
            set
            {
                this.IsArchivedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string No
        {
            get
            {
                return this.NoField;
            }
            set
            {
                this.NoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BranchSearchItem", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.BranchDept")]
    public partial class BranchSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private bool IsLondonRateField;
        
        private string NameField;
        
        private System.Guid OrganisationIdField;
        
        private string ReferenceField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsLondonRate
        {
            get
            {
                return this.IsLondonRateField;
            }
            set
            {
                this.IsLondonRateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid OrganisationId
        {
            get
            {
                return this.OrganisationIdField;
            }
            set
            {
                this.OrganisationIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Reference
        {
            get
            {
                return this.ReferenceField;
            }
            set
            {
                this.ReferenceField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DepartmentSearchCriteria", Namespace="http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.BranchDept")]
    public partial class DepartmentSearchCriteria : IRIS.Law.WebServiceInterfaces.SearchCriteria
    {
        
        private bool AllDepartmentField;
        
        private bool IncludeArchivedField;
        
        private System.Guid OrganisationIdField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllDepartment
        {
            get
            {
                return this.AllDepartmentField;
            }
            set
            {
                this.AllDepartmentField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IncludeArchived
        {
            get
            {
                return this.IncludeArchivedField;
            }
            set
            {
                this.IncludeArchivedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid OrganisationId
        {
            get
            {
                return this.OrganisationIdField;
            }
            set
            {
                this.OrganisationIdField = value;
            }
        }
    }
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://www.iris.co.uk/ILB/BranchDeptService", ConfigurationName="IBranchDeptService", SessionMode=System.ServiceModel.SessionMode.Required)]
public interface IBranchDeptService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/BranchSearch", ReplyAction="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/BranchSearchRespon" +
        "se")]
    IRIS.Law.WebServiceInterfaces.BranchDept.BranchSearchReturnValue BranchSearch(System.Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/DepartmentSearch", ReplyAction="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/DepartmentSearchRe" +
        "sponse")]
    IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchReturnValue DepartmentSearch(System.Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchCriteria criteria);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetBranchDepartmen" +
        "tDefaults", ReplyAction="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetBranchDepartmen" +
        "tDefaultsResponse")]
    IRIS.Law.WebServiceInterfaces.Matter.MatterReturnValue GetBranchDepartmentDefaults(System.Guid logonId, System.Guid branchOrganisationId, int departmentId);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetBranch", ReplyAction="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetBranchResponse")]
    IRIS.Law.WebServiceInterfaces.Branch.BranchReturnValue GetBranch(System.Guid logonId, System.Guid orgId);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetDepartment", ReplyAction="http://www.iris.co.uk/ILB/BranchDeptService/IBranchDeptService/GetDepartmentRespo" +
        "nse")]
    IRIS.Law.WebServiceInterfaces.Department.DepartmentReturnValue GetDepartment(System.Guid logonId, int deptId);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface IBranchDeptServiceChannel : IBranchDeptService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class BranchDeptServiceClient : System.ServiceModel.ClientBase<IBranchDeptService>, IBranchDeptService
{
    
    public BranchDeptServiceClient()
    {
    }
    
    public BranchDeptServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public BranchDeptServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public BranchDeptServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public BranchDeptServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public IRIS.Law.WebServiceInterfaces.BranchDept.BranchSearchReturnValue BranchSearch(System.Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest)
    {
        return base.Channel.BranchSearch(logonId, collectionRequest);
    }
    
    public IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchReturnValue DepartmentSearch(System.Guid logonId, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, IRIS.Law.WebServiceInterfaces.BranchDept.DepartmentSearchCriteria criteria)
    {
        return base.Channel.DepartmentSearch(logonId, collectionRequest, criteria);
    }
    
    public IRIS.Law.WebServiceInterfaces.Matter.MatterReturnValue GetBranchDepartmentDefaults(System.Guid logonId, System.Guid branchOrganisationId, int departmentId)
    {
        return base.Channel.GetBranchDepartmentDefaults(logonId, branchOrganisationId, departmentId);
    }
    
    public IRIS.Law.WebServiceInterfaces.Branch.BranchReturnValue GetBranch(System.Guid logonId, System.Guid orgId)
    {
        return base.Channel.GetBranch(logonId, orgId);
    }
    
    public IRIS.Law.WebServiceInterfaces.Department.DepartmentReturnValue GetDepartment(System.Guid logonId, int deptId)
    {
        return base.Channel.GetDepartment(logonId, deptId);
    }
}

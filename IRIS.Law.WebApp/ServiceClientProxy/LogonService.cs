﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace IRIS.Law.WebServiceInterfaces.Logon
{
    using System.Runtime.Serialization;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LogonReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Logon")]
    public partial class LogonReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private bool CanUserEditArchivedMatterField;

        private bool CanUserLockDocumentField;

        private int DbUidField;

        private bool IsEditArchivedMattersField;

        private bool IsMemberField;

        private bool IsPostCodeLookupEnabledField;

        private System.Guid LogonIdField;

        private System.Guid MemberIdField;

        private System.Guid OrganisationIdField;

        private int TimeUnitsField;

        private System.Guid UserDefaultBranchField;

        private int UserDefaultDepartmentField;

        private System.Guid UserDefaultFeeMemberIdField;

        private System.Guid UserDefaultPartnerField;

        private System.Guid UserDefaultWorkTypeField;

        private int UserTypeField;

        private bool WebMasterField;

        private string WebStyleSheetField;

        private bool IsUsingILBDiaryField;

        private bool IsFirstTimeLoggedInField;

        private int DatabaseRoleField;

        private bool AutomaticVersioningField;

        private bool ConflictCheckRolesField;


        private bool _canAddTask;
        // Check to see if the user has logged in before
      //  [DataMember]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CanAddTask
        {
            get
            {
                return _canAddTask;
            }
            set
            {
                _canAddTask = value;
            }
        }

        private bool _canUploadDocument;
    //    [DataMember]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CanUploadDocument
        {
            get
            {
                return this._canUploadDocument;
            }
            set
            {
                this._canUploadDocument = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CanUserEditArchivedMatter
        {
            get
            {
                return this.CanUserEditArchivedMatterField;
            }
            set
            {
                this.CanUserEditArchivedMatterField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool CanUserLockDocument
        {
            get
            {
                return this.CanUserLockDocumentField;
            }
            set
            {
                this.CanUserLockDocumentField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DbUid
        {
            get
            {
                return this.DbUidField;
            }
            set
            {
                this.DbUidField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsEditArchivedMatters
        {
            get
            {
                return this.IsEditArchivedMattersField;
            }
            set
            {
                this.IsEditArchivedMattersField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsMember
        {
            get
            {
                return this.IsMemberField;
            }
            set
            {
                this.IsMemberField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPostCodeLookupEnabled
        {
            get
            {
                return this.IsPostCodeLookupEnabledField;
            }
            set
            {
                this.IsPostCodeLookupEnabledField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid LogonId
        {
            get
            {
                return this.LogonIdField;
            }
            set
            {
                this.LogonIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid MemberId
        {
            get
            {
                return this.MemberIdField;
            }
            set
            {
                this.MemberIdField = value;
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
        public int TimeUnits
        {
            get
            {
                return this.TimeUnitsField;
            }
            set
            {
                this.TimeUnitsField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserDefaultBranch
        {
            get
            {
                return this.UserDefaultBranchField;
            }
            set
            {
                this.UserDefaultBranchField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int UserDefaultDepartment
        {
            get
            {
                return this.UserDefaultDepartmentField;
            }
            set
            {
                this.UserDefaultDepartmentField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserDefaultFeeMemberId
        {
            get
            {
                return this.UserDefaultFeeMemberIdField;
            }
            set
            {
                this.UserDefaultFeeMemberIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserDefaultPartner
        {
            get
            {
                return this.UserDefaultPartnerField;
            }
            set
            {
                this.UserDefaultPartnerField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserDefaultWorkType
        {
            get
            {
                return this.UserDefaultWorkTypeField;
            }
            set
            {
                this.UserDefaultWorkTypeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int UserType
        {
            get
            {
                return this.UserTypeField;
            }
            set
            {
                this.UserTypeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool WebMaster
        {
            get
            {
                return this.WebMasterField;
            }
            set
            {
                this.WebMasterField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WebStyleSheet
        {
            get
            {
                return this.WebStyleSheetField;
            }
            set
            {
                this.WebStyleSheetField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsUsingILBDiary
        {
            get
            {
                return this.IsUsingILBDiaryField;
            }
            set
            {
                this.IsUsingILBDiaryField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsFirstTimeLoggedIn
        {
            get
            {
                return this.IsFirstTimeLoggedInField;
            }
            set
            {
                this.IsFirstTimeLoggedInField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DatabaseRole
        {
            get
            {
                return this.DatabaseRoleField;
            }
            set
            {
                this.DatabaseRoleField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AutomaticVersioning
        {
            get
            {
                return this.AutomaticVersioningField;
            }
            set
            {
                this.AutomaticVersioningField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool ConflictCheckRoles
        {
            get
            {
                return this.ConflictCheckRolesField;
            }
            set
            {
                this.ConflictCheckRolesField = value;
            }
        }

    }
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace = "http://www.iris.co.uk/ILB/LogonService", ConfigurationName = "ILogonService", SessionMode = System.ServiceModel.SessionMode.Required)]
public interface ILogonService
{

    [System.ServiceModel.OperationContractAttribute(Action = "http://www.iris.co.uk/ILB/LogonService/ILogonService/Logon", ReplyAction = "http://www.iris.co.uk/ILB/LogonService/ILogonService/LogonResponse")]
    IRIS.Law.WebServiceInterfaces.Logon.LogonReturnValue Logon(string userName, string password);

    [System.ServiceModel.OperationContractAttribute(Action = "http://www.iris.co.uk/ILB/LogonService/ILogonService/Logoff", ReplyAction = "http://www.iris.co.uk/ILB/LogonService/ILogonService/LogoffResponse")]
    IRIS.Law.WebServiceInterfaces.ReturnValue Logoff(System.Guid logonId);

    [System.ServiceModel.OperationContractAttribute(Action = "http://www.iris.co.uk/ILB/LogonService/ILogonService/ChangePassword", ReplyAction = "http://www.iris.co.uk/ILB/LogonService/ILogonService/ChangePasswordResponse")]
    IRIS.Law.WebServiceInterfaces.ReturnValue ChangePassword(System.Guid logonId, string userName, string password, string newPassword);

    [System.ServiceModel.OperationContractAttribute(Action = "http://www.iris.co.uk/ILB/LogonService/ILogonService/RequestPassword", ReplyAction = "http://www.iris.co.uk/ILB/LogonService/ILogonService/RequestPasswordResponse")]
    IRIS.Law.WebServiceInterfaces.ReturnValue RequestPassword(string userName);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface ILogonServiceChannel : ILogonService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class LogonServiceClient : System.ServiceModel.ClientBase<ILogonService>, ILogonService
{

    public LogonServiceClient()
    {
    }

    public LogonServiceClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public LogonServiceClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public LogonServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public LogonServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    public IRIS.Law.WebServiceInterfaces.Logon.LogonReturnValue Logon(string userName, string password)
    {
        return base.Channel.Logon(userName, password);
    }

    public IRIS.Law.WebServiceInterfaces.ReturnValue Logoff(System.Guid logonId)
    {
        return base.Channel.Logoff(logonId);
    }

    public IRIS.Law.WebServiceInterfaces.ReturnValue ChangePassword(System.Guid logonId, string userName, string password, string newPassword)
    {
        return base.Channel.ChangePassword(logonId, userName, password, newPassword);
    }

    public IRIS.Law.WebServiceInterfaces.ReturnValue RequestPassword(string userName)
    {
        return base.Channel.RequestPassword(userName);
    }
 
}
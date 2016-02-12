
namespace IRIS.Law.WebServiceInterfaces.Matter
{
    using System.Runtime.Serialization;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterTypeSearchReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterTypeSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private IRIS.Law.WebServiceInterfaces.DataListOfMatterTypeSearchItem3nZuvbP0 MatterTypesField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfMatterTypeSearchItem3nZuvbP0 MatterTypes
        {
            get
            {
                return this.MatterTypesField;
            }
            set
            {
                this.MatterTypesField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterSearchReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private IRIS.Law.WebServiceInterfaces.DataListOfMatterSearchItem3nZuvbP0 MattersField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfMatterSearchItem3nZuvbP0 Matters
        {
            get
            {
                return this.MattersField;
            }
            set
            {
                this.MattersField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CashCollectionSearchReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class CashCollectionSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private IRIS.Law.WebServiceInterfaces.DataListOfCashCollectionSearchItem3nZuvbP0 CashCollectionField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfCashCollectionSearchItem3nZuvbP0 CashCollection
        {
            get
            {
                return this.CashCollectionField;
            }
            set
            {
                this.CashCollectionField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "WorkTypeSearchReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class WorkTypeSearchReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private System.Guid ChargeRateDescriptionIdField;

        private string ClientHOUCNField;

        private string ClientUCNField;

        private decimal DisbLimitField;

        private bool FranchisedField;

        private bool IsPublicFundedField;

        private decimal OverallLimitField;

        private decimal QuoteField;

        private decimal TimeLimitField;

        private decimal WipLimitField;

        private int WorkCategoryUFNField;

        private IRIS.Law.WebServiceInterfaces.DataListOfWorkTypeSearchItem3nZuvbP0 WorkTypesField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ChargeRateDescriptionId
        {
            get
            {
                return this.ChargeRateDescriptionIdField;
            }
            set
            {
                this.ChargeRateDescriptionIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientHOUCN
        {
            get
            {
                return this.ClientHOUCNField;
            }
            set
            {
                this.ClientHOUCNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ClientUCN
        {
            get
            {
                return this.ClientUCNField;
            }
            set
            {
                this.ClientUCNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal DisbLimit
        {
            get
            {
                return this.DisbLimitField;
            }
            set
            {
                this.DisbLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Franchised
        {
            get
            {
                return this.FranchisedField;
            }
            set
            {
                this.FranchisedField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPublicFunded
        {
            get
            {
                return this.IsPublicFundedField;
            }
            set
            {
                this.IsPublicFundedField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal OverallLimit
        {
            get
            {
                return this.OverallLimitField;
            }
            set
            {
                this.OverallLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Quote
        {
            get
            {
                return this.QuoteField;
            }
            set
            {
                this.QuoteField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal TimeLimit
        {
            get
            {
                return this.TimeLimitField;
            }
            set
            {
                this.TimeLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal WipLimit
        {
            get
            {
                return this.WipLimitField;
            }
            set
            {
                this.WipLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int WorkCategoryUFN
        {
            get
            {
                return this.WorkCategoryUFNField;
            }
            set
            {
                this.WorkCategoryUFNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfWorkTypeSearchItem3nZuvbP0 WorkTypes
        {
            get
            {
                return this.WorkTypesField;
            }
            set
            {
                this.WorkTypesField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "UFNReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class UFNReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private System.DateTime DateField;

        private System.Guid IdField;

        private string NumberField;

        private string ValueField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Date
        {
            get
            {
                return this.DateField;
            }
            set
            {
                this.DateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id
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
        public string Number
        {
            get
            {
                return this.NumberField;
            }
            set
            {
                this.NumberField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Value
        {
            get
            {
                return this.ValueField;
            }
            set
            {
                this.ValueField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterTypeReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterTypeReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private int MatterTypeIdField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MatterTypeId
        {
            get
            {
                return this.MatterTypeIdField;
            }
            set
            {
                this.MatterTypeIdField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterAssociationSearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterAssociationSearchCriteria : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private int ApplicationIdField;

        private System.Guid ProjectIdField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ApplicationId
        {
            get
            {
                return this.ApplicationIdField;
            }
            set
            {
                this.ApplicationIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ProjectId
        {
            get
            {
                return this.ProjectIdField;
            }
            set
            {
                this.ProjectIdField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterAssociationReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterAssociationReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private IRIS.Law.WebServiceInterfaces.DataListOfMatterAssociationSearchItem3nZuvbP0 AssociationsField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfMatterAssociationSearchItem3nZuvbP0 Associations
        {
            get
            {
                return this.AssociationsField;
            }
            set
            {
                this.AssociationsField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterReturnValue : IRIS.Law.WebServiceInterfaces.ReturnValue
    {

        private IRIS.Law.WebServiceInterfaces.Client.Client ClientDetailsField;

        private IRIS.Law.WebServiceInterfaces.Matter.Matter MatterField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.Client.Client ClientDetails
        {
            get
            {
                return this.ClientDetailsField;
            }
            set
            {
                this.ClientDetailsField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.Matter.Matter Matter
        {
            get
            {
                return this.MatterField;
            }
            set
            {
                this.MatterField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Matter", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class Matter : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string BankReferenceField;

        private System.Guid BranchIdField;

        private string BranchNameField;

        private string BranchReferenceField;

        private int BusinessSourceIdField;

        private int CashCollectionIdField;

        private System.Guid ChargeDescriptionIdField;

        private int ClientBankIdField;

        private System.Guid ClientIdField;

        private System.DateTime ClosedDateField;

        private System.DateTime CompletedDateField;

        private System.DateTime CostReviewDateField;

        private int CourtIdField;

        private int DepartmentIdField;

        private string DepartmentNameField;

        private string DepartmentReferenceField;

        private int DepositBankIdField;

        private string DescriptionField;

        private System.DateTime DestructDateField;

        private decimal DisbsLimitField;

        private System.Guid FeeEarnerMemberIdField;

        private string FeeEarnerNameField;

        private string FeeEarnerReferenceField;

        private string FileNoField;

        private bool FranchisedField;

        private string HOUCNField;

        private System.Guid IdField;

        private string IndicatorsField;

        private bool IsArchivedField;

        private bool IsMemberField;

        private bool IsPublicFundingField;

        private IRIS.Law.WebServiceInterfaces.DataListOfJointClientCandidateSearchItemkPb1ZSG8 JointClientCandidatesField;

        private string KeyDescriptionField;

        private System.DateTime LastSavedDateField;

        private string LetterHeadField;

        private bool MatterLegalAidedField;

        private int MatterTypeIdField;

        private System.DateTime NextReviewDateField;

        private int OfficeBankIdField;

        private System.DateTime OpenDateField;

        private string OurReferenceField;

        private decimal OverallLimitField;

        private string PFCertificateNoField;

        private string PFCertificateNoLimitsField;

        private System.Guid PartnerMemberIdField;

        private int PersonDealingIdField;

        private string PreviousReferenceField;

        private decimal QuoteField;

        private string ReferenceField;

        private string SalutationEnvelopeField;

        private string SalutationLetterField;

        private int SourceCampaignIdField;

        private string SpanType1RefField;

        private string SpanType2RefField;

        private string StatusField;

        private decimal TimeLimitField;

        private decimal TotalLockupField;

        private string UCNField;

        private string UFNField;

        private System.DateTime UFNDateField;

        private decimal WIPLimitField;

        private string WorkTypeCodeField;

        private string WorkTypeDescriptionField;

        private System.Guid WorkTypeIdField;

        private bool isLondonRateField;

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
        public string BankReference
        {
            get
            {
                return this.BankReferenceField;
            }
            set
            {
                this.BankReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid BranchId
        {
            get
            {
                return this.BranchIdField;
            }
            set
            {
                this.BranchIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchName
        {
            get
            {
                return this.BranchNameField;
            }
            set
            {
                this.BranchNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchReference
        {
            get
            {
                return this.BranchReferenceField;
            }
            set
            {
                this.BranchReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BusinessSourceId
        {
            get
            {
                return this.BusinessSourceIdField;
            }
            set
            {
                this.BusinessSourceIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CashCollectionId
        {
            get
            {
                return this.CashCollectionIdField;
            }
            set
            {
                this.CashCollectionIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ChargeDescriptionId
        {
            get
            {
                return this.ChargeDescriptionIdField;
            }
            set
            {
                this.ChargeDescriptionIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ClientBankId
        {
            get
            {
                return this.ClientBankIdField;
            }
            set
            {
                this.ClientBankIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ClientId
        {
            get
            {
                return this.ClientIdField;
            }
            set
            {
                this.ClientIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime ClosedDate
        {
            get
            {
                return this.ClosedDateField;
            }
            set
            {
                this.ClosedDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CompletedDate
        {
            get
            {
                return this.CompletedDateField;
            }
            set
            {
                this.CompletedDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime CostReviewDate
        {
            get
            {
                return this.CostReviewDateField;
            }
            set
            {
                this.CostReviewDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CourtId
        {
            get
            {
                return this.CourtIdField;
            }
            set
            {
                this.CourtIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DepartmentId
        {
            get
            {
                return this.DepartmentIdField;
            }
            set
            {
                this.DepartmentIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentName
        {
            get
            {
                return this.DepartmentNameField;
            }
            set
            {
                this.DepartmentNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentReference
        {
            get
            {
                return this.DepartmentReferenceField;
            }
            set
            {
                this.DepartmentReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DepositBankId
        {
            get
            {
                return this.DepositBankIdField;
            }
            set
            {
                this.DepositBankIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DestructDate
        {
            get
            {
                return this.DestructDateField;
            }
            set
            {
                this.DestructDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal DisbsLimit
        {
            get
            {
                return this.DisbsLimitField;
            }
            set
            {
                this.DisbsLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid FeeEarnerMemberId
        {
            get
            {
                return this.FeeEarnerMemberIdField;
            }
            set
            {
                this.FeeEarnerMemberIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FeeEarnerName
        {
            get
            {
                return this.FeeEarnerNameField;
            }
            set
            {
                this.FeeEarnerNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FeeEarnerReference
        {
            get
            {
                return this.FeeEarnerReferenceField;
            }
            set
            {
                this.FeeEarnerReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FileNo
        {
            get
            {
                return this.FileNoField;
            }
            set
            {
                this.FileNoField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Franchised
        {
            get
            {
                return this.FranchisedField;
            }
            set
            {
                this.FranchisedField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HOUCN
        {
            get
            {
                return this.HOUCNField;
            }
            set
            {
                this.HOUCNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id
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
        public string Indicators
        {
            get
            {
                return this.IndicatorsField;
            }
            set
            {
                this.IndicatorsField = value;
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
        public bool IsPublicFunding
        {
            get
            {
                return this.IsPublicFundingField;
            }
            set
            {
                this.IsPublicFundingField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public IRIS.Law.WebServiceInterfaces.DataListOfJointClientCandidateSearchItemkPb1ZSG8 JointClientCandidates
        {
            get
            {
                return this.JointClientCandidatesField;
            }
            set
            {
                this.JointClientCandidatesField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string KeyDescription
        {
            get
            {
                return this.KeyDescriptionField;
            }
            set
            {
                this.KeyDescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastSavedDate
        {
            get
            {
                return this.LastSavedDateField;
            }
            set
            {
                this.LastSavedDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LetterHead
        {
            get
            {
                return this.LetterHeadField;
            }
            set
            {
                this.LetterHeadField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool MatterLegalAided
        {
            get
            {
                return this.MatterLegalAidedField;
            }
            set
            {
                this.MatterLegalAidedField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MatterTypeId
        {
            get
            {
                return this.MatterTypeIdField;
            }
            set
            {
                this.MatterTypeIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime NextReviewDate
        {
            get
            {
                return this.NextReviewDateField;
            }
            set
            {
                this.NextReviewDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int OfficeBankId
        {
            get
            {
                return this.OfficeBankIdField;
            }
            set
            {
                this.OfficeBankIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime OpenDate
        {
            get
            {
                return this.OpenDateField;
            }
            set
            {
                this.OpenDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OurReference
        {
            get
            {
                return this.OurReferenceField;
            }
            set
            {
                this.OurReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal OverallLimit
        {
            get
            {
                return this.OverallLimitField;
            }
            set
            {
                this.OverallLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PFCertificateNo
        {
            get
            {
                return this.PFCertificateNoField;
            }
            set
            {
                this.PFCertificateNoField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PFCertificateNoLimits
        {
            get
            {
                return this.PFCertificateNoLimitsField;
            }
            set
            {
                this.PFCertificateNoLimitsField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid PartnerMemberId
        {
            get
            {
                return this.PartnerMemberIdField;
            }
            set
            {
                this.PartnerMemberIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int PersonDealingId
        {
            get
            {
                return this.PersonDealingIdField;
            }
            set
            {
                this.PersonDealingIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PreviousReference
        {
            get
            {
                return this.PreviousReferenceField;
            }
            set
            {
                this.PreviousReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Quote
        {
            get
            {
                return this.QuoteField;
            }
            set
            {
                this.QuoteField = value;
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

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SalutationEnvelope
        {
            get
            {
                return this.SalutationEnvelopeField;
            }
            set
            {
                this.SalutationEnvelopeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SalutationLetter
        {
            get
            {
                return this.SalutationLetterField;
            }
            set
            {
                this.SalutationLetterField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SourceCampaignId
        {
            get
            {
                return this.SourceCampaignIdField;
            }
            set
            {
                this.SourceCampaignIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SpanType1Ref
        {
            get
            {
                return this.SpanType1RefField;
            }
            set
            {
                this.SpanType1RefField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SpanType2Ref
        {
            get
            {
                return this.SpanType2RefField;
            }
            set
            {
                this.SpanType2RefField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal TimeLimit
        {
            get
            {
                return this.TimeLimitField;
            }
            set
            {
                this.TimeLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal TotalLockup
        {
            get
            {
                return this.TotalLockupField;
            }
            set
            {
                this.TotalLockupField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UCN
        {
            get
            {
                return this.UCNField;
            }
            set
            {
                this.UCNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UFN
        {
            get
            {
                return this.UFNField;
            }
            set
            {
                this.UFNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime UFNDate
        {
            get
            {
                return this.UFNDateField;
            }
            set
            {
                this.UFNDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal WIPLimit
        {
            get
            {
                return this.WIPLimitField;
            }
            set
            {
                this.WIPLimitField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkTypeCode
        {
            get
            {
                return this.WorkTypeCodeField;
            }
            set
            {
                this.WorkTypeCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkTypeDescription
        {
            get
            {
                return this.WorkTypeDescriptionField;
            }
            set
            {
                this.WorkTypeDescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid WorkTypeId
        {
            get
            {
                return this.WorkTypeIdField;
            }
            set
            {
                this.WorkTypeIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool isLondonRate
        {
            get
            {
                return this.isLondonRateField;
            }
            set
            {
                this.isLondonRateField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterTypeSearchItem", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterTypeSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string DescriptionField;

        private int IdField;

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
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
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
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterSearchItem", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string BranchCodeField;

        private string BranchNameField;

        private System.Nullable<System.DateTime> ClosedDateField;

        private string DepartmentCodeField;

        private string DepartmentNameField;

        private string DescriptionField;

        private string FeeEarnerNameField;

        private System.Guid IdField;

        private string KeyDescriptionField;

        private System.DateTime OpenedDateField;

        private string ReferenceField;

        private string WorkTypeField;

        private string WorkTypeCodeField;

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
        public string BranchCode
        {
            get
            {
                return this.BranchCodeField;
            }
            set
            {
                this.BranchCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchName
        {
            get
            {
                return this.BranchNameField;
            }
            set
            {
                this.BranchNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> ClosedDate
        {
            get
            {
                return this.ClosedDateField;
            }
            set
            {
                this.ClosedDateField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentCode
        {
            get
            {
                return this.DepartmentCodeField;
            }
            set
            {
                this.DepartmentCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentName
        {
            get
            {
                return this.DepartmentNameField;
            }
            set
            {
                this.DepartmentNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FeeEarnerName
        {
            get
            {
                return this.FeeEarnerNameField;
            }
            set
            {
                this.FeeEarnerNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id
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
        public string KeyDescription
        {
            get
            {
                return this.KeyDescriptionField;
            }
            set
            {
                this.KeyDescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime OpenedDate
        {
            get
            {
                return this.OpenedDateField;
            }
            set
            {
                this.OpenedDateField = value;
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

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkType
        {
            get
            {
                return this.WorkTypeField;
            }
            set
            {
                this.WorkTypeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkTypeCode
        {
            get
            {
                return this.WorkTypeCodeField;
            }
            set
            {
                this.WorkTypeCodeField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CashCollectionSearchItem", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class CashCollectionSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string DescriptionField;

        private int IdField;

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
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
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
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "WorkTypeSearchItem", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class WorkTypeSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string CodeField;

        private string DescriptionField;

        private System.Guid IdField;

        private bool IsArchivedField;

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
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id
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
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterAssociationSearchItem", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterAssociationSearchItem : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string DescriptionField;

        private string NameField;

        private string RoleField;

        private string SurnameField;

        private string TitleField;

        private System.Guid MemberIdField;

        private string WorkEmailField;

        private string WorkTelephoneField;

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
        public string Description
        {
            get
            {
                return this.DescriptionField;
            }
            set
            {
                this.DescriptionField = value;
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
        public string Role
        {
            get
            {
                return this.RoleField;
            }
            set
            {
                this.RoleField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Surname
        {
            get
            {
                return this.SurnameField;
            }
            set
            {
                this.SurnameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Title
        {
            get
            {
                return this.TitleField;
            }
            set
            {
                this.TitleField = value;
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
        public string WorkEmail
        {
            get
            {
                return this.WorkEmailField;
            }
            set
            {
                this.WorkEmailField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkTelephone
        {
            get
            {
                return this.WorkTelephoneField;
            }
            set
            {
                this.WorkTelephoneField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterSearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterSearchCriteria : IRIS.Law.WebServiceInterfaces.SearchCriteria
    {

        private string BranchCodeField;

        private System.Nullable<System.DateTime> ClosedDateFromField;

        private System.Nullable<System.DateTime> ClosedDateToField;

        private string DepartmentCodeField;

        private System.Guid FeeEarnerField;

        private string KeyDescriptionField;

        private string MatterDescriptionField;

        private System.Guid MatterIdField;

        private string MatterPreviousReferenceField;

        private string MatterReferenceField;

        private System.Guid MemberIdField;

        private System.Nullable<System.DateTime> OpenedDateFromField;

        private System.Nullable<System.DateTime> OpenedDateToField;

        private System.Guid OrganisationIdField;

        private string UFNField;

        private string WorkTypeCodeField;

        private string OrderByField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BranchCode
        {
            get
            {
                return this.BranchCodeField;
            }
            set
            {
                this.BranchCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> ClosedDateFrom
        {
            get
            {
                return this.ClosedDateFromField;
            }
            set
            {
                this.ClosedDateFromField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> ClosedDateTo
        {
            get
            {
                return this.ClosedDateToField;
            }
            set
            {
                this.ClosedDateToField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentCode
        {
            get
            {
                return this.DepartmentCodeField;
            }
            set
            {
                this.DepartmentCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid FeeEarner
        {
            get
            {
                return this.FeeEarnerField;
            }
            set
            {
                this.FeeEarnerField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string KeyDescription
        {
            get
            {
                return this.KeyDescriptionField;
            }
            set
            {
                this.KeyDescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MatterDescription
        {
            get
            {
                return this.MatterDescriptionField;
            }
            set
            {
                this.MatterDescriptionField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid MatterId
        {
            get
            {
                return this.MatterIdField;
            }
            set
            {
                this.MatterIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MatterPreviousReference
        {
            get
            {
                return this.MatterPreviousReferenceField;
            }
            set
            {
                this.MatterPreviousReferenceField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MatterReference
        {
            get
            {
                return this.MatterReferenceField;
            }
            set
            {
                this.MatterReferenceField = value;
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
        public System.Nullable<System.DateTime> OpenedDateFrom
        {
            get
            {
                return this.OpenedDateFromField;
            }
            set
            {
                this.OpenedDateFromField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> OpenedDateTo
        {
            get
            {
                return this.OpenedDateToField;
            }
            set
            {
                this.OpenedDateToField = value;
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
        public string UFN
        {
            get
            {
                return this.UFNField;
            }
            set
            {
                this.UFNField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkTypeCode
        {
            get
            {
                return this.WorkTypeCodeField;
            }
            set
            {
                this.WorkTypeCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrderBy
        {
            get
            {
                return this.OrderByField;
            }
            set
            {
                this.OrderByField = value;
            }
        }

        
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "CashCollectionSearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class CashCollectionSearchCriteria : IRIS.Law.WebServiceInterfaces.SearchCriteria
    {

        private bool IncludeArchivedField;

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
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "WorkTypeSearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class WorkTypeSearchCriteria : IRIS.Law.WebServiceInterfaces.SearchCriteria
    {

        private bool AllWorkTypesField;

        private System.Guid ClientIdField;

        private int DepartmentIdField;

        private string DepartmentNoField;

        private System.Guid IdField;

        private bool IsPrivateClientField;

        private int MatterTypeIdField;

        private System.Guid OrganisationIDField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool AllWorkTypes
        {
            get
            {
                return this.AllWorkTypesField;
            }
            set
            {
                this.AllWorkTypesField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ClientId
        {
            get
            {
                return this.ClientIdField;
            }
            set
            {
                this.ClientIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DepartmentId
        {
            get
            {
                return this.DepartmentIdField;
            }
            set
            {
                this.DepartmentIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DepartmentNo
        {
            get
            {
                return this.DepartmentNoField;
            }
            set
            {
                this.DepartmentNoField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id
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
        public bool IsPrivateClient
        {
            get
            {
                return this.IsPrivateClientField;
            }
            set
            {
                this.IsPrivateClientField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MatterTypeId
        {
            get
            {
                return this.MatterTypeIdField;
            }
            set
            {
                this.MatterTypeIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid OrganisationID
        {
            get
            {
                return this.OrganisationIDField;
            }
            set
            {
                this.OrganisationIDField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "MatterTypeSearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Matter")]
    public partial class MatterTypeSearchCriteria : IRIS.Law.WebServiceInterfaces.SearchCriteria
    {

        private int ClientTypeIdField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ClientTypeId
        {
            get
            {
                return this.ClientTypeIdField;
            }
            set
            {
                this.ClientTypeIdField = value;
            }
        }
    }
}
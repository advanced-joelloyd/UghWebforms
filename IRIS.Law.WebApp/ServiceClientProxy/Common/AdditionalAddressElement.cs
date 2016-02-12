namespace IRIS.Law.WebServiceInterfaces.Contact
{
    using System.Runtime.Serialization;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AdditionalAddressElement", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Contact")]
    public partial class AdditionalAddressElement : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private int AddressIdField;

        private string ElementCommentField;

        private string ElementTextField;

        private int IdField;

        private System.Guid MemberIdField;

        private System.Guid OrganisationIdField;

        private int TypeIdField;

        private string TypeTextField;

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
        public int AddressId
        {
            get
            {
                return this.AddressIdField;
            }
            set
            {
                this.AddressIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ElementComment
        {
            get
            {
                return this.ElementCommentField;
            }
            set
            {
                this.ElementCommentField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ElementText
        {
            get
            {
                return this.ElementTextField;
            }
            set
            {
                this.ElementTextField = value;
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
        public int TypeId
        {
            get
            {
                return this.TypeIdField;
            }
            set
            {
                this.TypeIdField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TypeText
        {
            get
            {
                return this.TypeTextField;
            }
            set
            {
                this.TypeTextField = value;
            }
        }
    }
}
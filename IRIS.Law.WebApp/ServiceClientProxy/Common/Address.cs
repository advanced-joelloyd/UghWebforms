
namespace IRIS.Law.WebServiceInterfaces.Contact
{
    using System.Runtime.Serialization;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Address", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces.Contact")]
    public partial class Address : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] AdditionalAddressElementsField;

        private string CommentField;

        private string CountryField;

        private string CountyField;

        private string DXNumberField;

        private string DXTownField;

        private string DepartmentField;

        private string DependantLocalityField;

        private string HouseNameField;

        private int IdField;

        private bool IsBillingAddressField;

        private bool IsMailingAddressField;

        private System.DateTime LastVerifiedField;

        private string Line1Field;

        private string Line2Field;

        private string Line3Field;

        private System.Guid MemberIdField;

        private System.Guid OrganisationIdField;

        private string OrganisationNameField;

        private string PostBoxField;

        private string PostCodeField;

        private string StreetNumberField;

        private string SubBuildingField;

        private string TownField;

        private int TypeIdField;

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
        public IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] AdditionalAddressElements
        {
            get
            {
                return this.AdditionalAddressElementsField;
            }
            set
            {
                this.AdditionalAddressElementsField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Comment
        {
            get
            {
                return this.CommentField;
            }
            set
            {
                this.CommentField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Country
        {
            get
            {
                return this.CountryField;
            }
            set
            {
                this.CountryField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string County
        {
            get
            {
                return this.CountyField;
            }
            set
            {
                this.CountyField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DXNumber
        {
            get
            {
                return this.DXNumberField;
            }
            set
            {
                this.DXNumberField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DXTown
        {
            get
            {
                return this.DXTownField;
            }
            set
            {
                this.DXTownField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Department
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

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DependantLocality
        {
            get
            {
                return this.DependantLocalityField;
            }
            set
            {
                this.DependantLocalityField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string HouseName
        {
            get
            {
                return this.HouseNameField;
            }
            set
            {
                this.HouseNameField = value;
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
        public bool IsBillingAddress
        {
            get
            {
                return this.IsBillingAddressField;
            }
            set
            {
                this.IsBillingAddressField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsMailingAddress
        {
            get
            {
                return this.IsMailingAddressField;
            }
            set
            {
                this.IsMailingAddressField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastVerified
        {
            get
            {
                return this.LastVerifiedField;
            }
            set
            {
                this.LastVerifiedField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Line1
        {
            get
            {
                return this.Line1Field;
            }
            set
            {
                this.Line1Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Line2
        {
            get
            {
                return this.Line2Field;
            }
            set
            {
                this.Line2Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Line3
        {
            get
            {
                return this.Line3Field;
            }
            set
            {
                this.Line3Field = value;
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
        public string OrganisationName
        {
            get
            {
                return this.OrganisationNameField;
            }
            set
            {
                this.OrganisationNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PostBox
        {
            get
            {
                return this.PostBoxField;
            }
            set
            {
                this.PostBoxField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PostCode
        {
            get
            {
                return this.PostCodeField;
            }
            set
            {
                this.PostCodeField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StreetNumber
        {
            get
            {
                return this.StreetNumberField;
            }
            set
            {
                this.StreetNumberField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SubBuilding
        {
            get
            {
                return this.SubBuildingField;
            }
            set
            {
                this.SubBuildingField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Town
        {
            get
            {
                return this.TownField;
            }
            set
            {
                this.TownField = value;
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
    }
    
}
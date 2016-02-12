namespace IRIS.Law.WebServiceInterfaces
{
	using System.Runtime.Serialization;

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute(Name = "SearchCriteria", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces")]
	[System.Runtime.Serialization.KnownTypeAttribute(typeof(IRIS.Law.WebServiceInterfaces.Client.RatingSearchCriteria))]
	[System.Runtime.Serialization.KnownTypeAttribute(typeof(IRIS.Law.WebServiceInterfaces.Client.ClientSearchCriteria))]
	public partial class SearchCriteria : object, System.Runtime.Serialization.IExtensibleDataObject
	{

		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

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
	}
}

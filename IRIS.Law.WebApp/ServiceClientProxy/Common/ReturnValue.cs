using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRIS.Law.WebServiceInterfaces
{
	using System.Runtime.Serialization;

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute(Name = "ReturnValue", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces")]
	//[System.Runtime.Serialization.KnownTypeAttribute(typeof(IRIS.Law.WebServiceInterfaces.Address.AddressTypeReturnValue))]
	public partial class ReturnValue : object, System.Runtime.Serialization.IExtensibleDataObject
	{

		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

		private string MessageField;

		private bool SuccessField;

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
		public string Message
		{
			get
			{
				return this.MessageField;
			}
			set
			{
				this.MessageField = value;
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public bool Success
		{
			get
			{
				return this.SuccessField;
			}
			set
			{
				this.SuccessField = value;
			}
		}
	}
}

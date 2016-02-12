using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRIS.Law.WebServiceInterfaces
{
	using System.Runtime.Serialization;
	
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
	[System.Runtime.Serialization.DataContractAttribute(Name = "CollectionRequest", Namespace = "http://schemas.datacontract.org/2004/07/IRIS.Law.WebServiceInterfaces")]
	public partial class CollectionRequest : object, System.Runtime.Serialization.IExtensibleDataObject
	{

		private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

		private bool ForceRefreshField;

		private int RowCountField;

		private int StartRowField;

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
		public bool ForceRefresh
		{
			get
			{
				return this.ForceRefreshField;
			}
			set
			{
				this.ForceRefreshField = value;
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public int RowCount
		{
			get
			{
				return this.RowCountField;
			}
			set
			{
				this.RowCountField = value;
			}
		}

		[System.Runtime.Serialization.DataMemberAttribute()]
		public int StartRow
		{
			get
			{
				return this.StartRowField;
			}
			set
			{
				this.StartRowField = value;
			}
		}
	}
}

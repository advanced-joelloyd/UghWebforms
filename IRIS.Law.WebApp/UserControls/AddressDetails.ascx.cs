using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.UserControls
{
	public partial class AddressDetails : System.Web.UI.UserControl
	{
		#region Public Properties

		private Guid _logonId;
        private bool _mapHide;
		private Address _address;
        LogonReturnValue logonUser;
		/// <summary>
		/// Gets or sets the address.
		/// </summary>
		/// <value>The address.</value>
		public Address Address
		{
			get
			{
				_address = GetAddress();
				return _address;
			}
			set
			{
				_address = value;
			}
		}

        public bool HideMapControl
        {
            get
            {
                
                return _mapHide;
            }
            set
            {
                _mapHide = value;
            }
        }

		private bool _isMailingAddress = true;
		/// <summary>
		/// Gets or sets a value indicating whether this address is the mailing address.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this address is the mailing address; otherwise, <c>false</c>.
		/// </value>
		public bool IsMailingAddress
		{
			get
			{
				return _chkMailAddress.Checked;
			}
			set
			{
				_isMailingAddress = value;
			}
		}

		private bool _isBillingAddress = true;
		/// <summary>
		/// Gets or sets a value indicating whether this address is the billing address.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this address is the billing address; otherwise, <c>false</c>.
		/// </value>
		public bool IsBillingAddress
		{
			get
			{
				return _chkBillAddress.Checked;
			}
			set
			{
				_isBillingAddress = value;
			}
		}

		private bool _isMailingAddressEnabled = true;
		/// <summary>
		/// Gets or sets a value indicating whether the mailing address checkbox enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the mailing address checkbox is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsMailingAddressEnabled
		{
			get
			{
				return _chkMailAddress.Enabled;
			}
			set
			{
				_isMailingAddressEnabled = value;
			}
		}

		private bool _isBillingAddressEnabled = true;
		/// <summary>
		/// Gets or sets a value indicating whether the billing address checkbox enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the billing address checkbox is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsBillingAddressEnabled
		{
			get
			{
				return _chkBillAddress.Enabled;
			}
			set
			{
				_isBillingAddressEnabled = value;
			}
		}

        private bool _isLastVerifiedEnabled = true;

        public bool IsLastVerifiedEnabled
        {
            get
            {
                return _ccLastVerifiedDate.Enabled;
            }
            set
            {
                _isLastVerifiedEnabled = value;
            }
        }

		private string _asyncPostbackTriggers;
		/// <summary>
		/// Comma seperated control ids that trigger the asynchronous postback of the address.
		/// </summary>
		/// <value>The async postback triggers.</value>
		public string AsyncPostbackTriggers
		{
			get
			{
				return _asyncPostbackTriggers;
			}
			set
			{
				_asyncPostbackTriggers = value;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether the data has changed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the data has changed; otherwise, <c>false</c>.
		/// </value>
		public bool IsDataChanged
		{
			get
			{
				if (_hfIsDataChanged != null)
				{
					return Convert.ToBoolean(_hfIsDataChanged.Value);
				}
				return false;
			}
			set
			{
				if (_hfIsDataChanged != null)
				{
					_hfIsDataChanged.Value = value.ToString();
				}
			}
		}

		private bool _isDataChangedCheckRequired;
		/// <summary>
		/// Gets or sets a value indicating the data changed check is required.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if is data changed check required; otherwise, <c>false</c>.
		/// </value>
		public bool IsDataChangedCheckRequired
		{
			get
			{
				return _isDataChangedCheckRequired;
			}
			set
			{
				_isDataChangedCheckRequired = value;
			}
		}

		#endregion

		protected void Page_Init(object sender, EventArgs e)
		{
			if (_asyncPostbackTriggers != null && _asyncPostbackTriggers.Length > 0)
			{
				string[] controls = _asyncPostbackTriggers.Split(',');
				foreach (string controlId in controls)
				{
					AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
					trigger.ControlID = controlId;
					_updPnlAdditionalInfo.Triggers.Add(trigger);
					_updPnlAddressDetails.Triggers.Add(trigger);
				}
			}

			if (!IsPostBack)
			{
				_chkBillAddress.Enabled = _isBillingAddressEnabled;
				_chkBillAddress.Checked = _isBillingAddress;
				_chkMailAddress.Enabled = _isMailingAddressEnabled;
				_chkMailAddress.Checked = _isMailingAddress;
                _ccLastVerifiedDate.Enabled = _isLastVerifiedEnabled;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session[SessionName.LogonSettings] == null)
			{
				Response.Redirect("~/Login.aspx?SessionExpired=1", true);
				return;
			}

            if (_mapHide)
            {
                _ucMap.Visible = false;
            }

			logonUser = (LogonReturnValue)Session[SessionName.LogonSettings];
			_logonId = logonUser.LogonId;

			//Javascript error is thrown if we use more than one address control on a pg which checks for data changed 
			//Created a property to disable datachanged check when not required
			if (_isDataChangedCheckRequired)
			{
				ScriptManager.RegisterStartupScript(this, typeof(UserControl), "DataChanged", "function SetDataChanged(){document.getElementById('" + _hfIsDataChanged.ClientID + "').value = true;}", true);
			}
			else
			{
				ScriptManager.RegisterStartupScript(this, typeof(UserControl), "DataChanged", "function SetDataChanged(){}", true);
			}

            _mpePostcodeLookup.OnOkScript = string.Format("javascript:if(OkClickOnOnlineAddressVerification('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'))  SetDataChanged(); ",
                                                          _listAddress.ClientID, _txtHouseNumber.ClientID, _txtPostcode.ClientID, _txtHouseName.ClientID
                                                          , _txtAddress1.ClientID, _txtTown.ClientID, _txtCounty.ClientID, _txtCountry.ClientID, _txtDeptLoc.ClientID, _txtAddress2.ClientID, _txtAddress3.ClientID, _ccLastVerifiedDate.DateTextBoxClientID);

            
            _btnAddressLookup.Visible = logonUser.IsPostCodeLookupEnabled;
            _trLastVerified.Visible = logonUser.IsPostCodeLookupEnabled;
            
            

            if (Request["__EVENTARGUMENT"] != null && Request["__EVENTARGUMENT"] == "DBmove")
            {
                int idx = _listAddress.SelectedIndex;
                string _listValue = _listAddress.SelectedValue;

                if (_listValue != string.Empty)
                {
                    string[] listSplit = _listValue.Split(new char[] { '~' });

                    if (listSplit[1] != string.Empty)
                    {
                        PostcodeLookup(listSplit[1], listSplit[0]);
                    }
                }
                
            }
            _listAddress.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(_listAddress, "DBmove"));
            
            
		}
		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		public override void DataBind()
		{
            logonUser = (LogonReturnValue)Session[SessionName.LogonSettings];

			ClearFields();
			if (_address != null)
			{
				_hfAddressId.Value = _address.Id.ToString();
				_hfAddressTypeId.Value = _address.TypeId.ToString();
				_txtHouseNumber.Text = _address.StreetNumber;
				_txtPostcode.Text = _address.PostCode;
				_txtHouseName.Text = _address.HouseName;
				_txtAddress1.Text = _address.Line1;
				_txtAddress2.Text = _address.Line2;
				_txtAddress3.Text = _address.Line3;
				_txtTown.Text = _address.Town;
				_txtCounty.Text = _address.County;
				_txtCountry.Text = _address.Country;
				_txtDXAddress1.Text = _address.DXNumber;
				_txtDXAddress2.Text = _address.DXTown;

			    if (_address.Id == (int)Session[SessionName.ClientBillingAddressId])
                {
                    _chkBillAddress.Checked = true;
                }
                else
                {
                    _chkBillAddress.Checked = false;
                }

                if (_address.Id == (int)Session[SessionName.ClientMailingAddressId])
                {
                    _chkMailAddress.Checked = true;
                }
                else
                {
                    _chkMailAddress.Checked = false;
                }

				_txtOrganisation.Text = _address.OrganisationName;
				_txtDepartment.Text = _address.Department;
				_txtPOBox.Text = _address.PostBox;
				_txtSubBuildingName.Text = _address.SubBuilding;
				_txtDeptLoc.Text = _address.DependantLocality;
				_txtComment.Text = _address.Comment;

				if (_address.LastVerified != DataConstants.BlankDate)
				{
					_ccLastVerifiedDate.DateText = _address.LastVerified.ToString("dd/MM/yyyy");
				}
				else
				{
					_ccLastVerifiedDate.DateText = "";
				}
			}

            if (Request.QueryString["mydetails"] == "true" || logonUser.UserType == (int)DataConstants.UserType.Client)
            {
                _ucMap.Visible = false;
            }
            else
            {
                if (_txtPostcode.Text != "")
                {
                    _ucMap.Address = _txtPostcode.Text;
                }
                else
                {
                    _ucMap.Address = _address.PostCode;
                }

                _ucMap.Rebind();
            }
		}

		/// <summary>
		/// Clears the fields.
		/// </summary>
		public void ClearFields()
		{
			_txtHouseNumber.Text = string.Empty;
			_txtPostcode.Text = string.Empty;
			_txtHouseName.Text = string.Empty;
			_txtAddress1.Text = string.Empty;
			_txtAddress2.Text = string.Empty;
			_txtAddress3.Text = string.Empty;
			_txtTown.Text = string.Empty;
			_txtCounty.Text = string.Empty;
			_txtCountry.Text = string.Empty;
			_txtDXAddress1.Text = string.Empty;
			_txtDXAddress2.Text = string.Empty;
			_chkMailAddress.Checked = false;
			_chkBillAddress.Checked = false;
			_txtOrganisation.Text = string.Empty;
			_txtDepartment.Text = string.Empty;
			_txtPOBox.Text = string.Empty;
			_txtSubBuildingName.Text = string.Empty;
			_txtDeptLoc.Text = string.Empty;
			_txtComment.Text = string.Empty;
			_ccLastVerifiedDate.DateText = string.Empty;
		}

        /// <summary>
        /// Disable the fields.
        /// </summary>
        public void DisableFields()
        {
            _txtHouseNumber.Enabled  = false;
            _txtPostcode.Enabled = false;
            _txtHouseName.Enabled = false;
            _txtAddress1.Enabled = false;
            _txtAddress2.Enabled = false;
            _txtAddress3.Enabled = false;
            _txtTown.Enabled = false;
            _txtCounty.Enabled = false;
            _txtCountry.Enabled = false;
            _txtDXAddress1.Enabled = false;
            _txtDXAddress2.Enabled = false;
            _chkMailAddress.Enabled = false;
            _chkBillAddress.Enabled = false;
            _txtOrganisation.Enabled = false;
            _txtDepartment.Enabled = false;
            _txtPOBox.Enabled = false;
            _txtSubBuildingName.Enabled = false;
            _txtDeptLoc.Enabled = false;
            _txtComment.Enabled = false;
            _ccLastVerifiedDate.Enabled = false;
            _btnAddressLookup.Enabled = false;
        }

		/// <summary>
		/// Gets the address.
		/// </summary>
		/// <returns></returns>
		private Address GetAddress()
		{
			Address address = new Address();
			if (_hfAddressId.Value != string.Empty)
			{
				address.Id = Convert.ToInt32(_hfAddressId.Value);
			}
			if (_hfAddressTypeId.Value != string.Empty)
			{
				address.TypeId = Convert.ToInt32(_hfAddressTypeId.Value);
			}
			address.StreetNumber = _txtHouseNumber.Text.Trim();
			address.PostCode = _txtPostcode.Text.Trim();
			address.HouseName = _txtHouseName.Text.Trim();
			address.Line1 = _txtAddress1.Text.Trim();
			address.Line2 = _txtAddress2.Text.Trim();
			address.Line3 = _txtAddress3.Text.Trim();
			address.Town = _txtTown.Text.Trim();
			address.County = _txtCounty.Text.Trim();
			address.Country = _txtCountry.Text.Trim();
			address.DXNumber = _txtDXAddress1.Text.Trim();
			address.DXTown = _txtDXAddress2.Text.Trim();
			address.IsMailingAddress = _chkMailAddress.Checked;
			address.IsBillingAddress = _chkBillAddress.Checked;
			address.OrganisationName = _txtOrganisation.Text.Trim();
			address.Department = _txtDepartment.Text.Trim();
			address.PostBox = _txtPOBox.Text.Trim();
			address.SubBuilding = _txtSubBuildingName.Text.Trim();
			address.DependantLocality = _txtDeptLoc.Text.Trim();
			address.Comment = _txtComment.Text.Trim();
			if (_ccLastVerifiedDate.DateText != "")
			{
				address.LastVerified = Convert.ToDateTime(_ccLastVerifiedDate.DateText.Trim());
			}
			else
			{
				address.LastVerified = DataConstants.BlankDate;
			}

			return address;
		}

		

		#region CreateListViewItem

        

		private ListItem CreateListViewItem(PostcodeLookupSearchItem cAddress)
		{

			StringBuilder sbAddress = new StringBuilder();
			StringBuilder sbAddressText = new StringBuilder();


            sbAddress.Append(cAddress.SearchStatus + "~" + cAddress.AmbiguityId + "~");

			if (cAddress.AmbiguityText != string.Empty)
			{
                sbAddress.Append(cAddress.AmbiguityText + "~");
				sbAddressText.Append(cAddress.AmbiguityText + ", ");
			}

            if (cAddress.BuildingNumber != string.Empty && cAddress.AmbiguityText == string.Empty)
            {
                sbAddress.Append(cAddress.BuildingNumber + "~");
                sbAddressText.Append(cAddress.BuildingNumber + ", ");
            }

            if (cAddress.AmbiguityText == string.Empty && cAddress.BuildingNumber == string.Empty)
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }
            
			if (cAddress.Buildingname != string.Empty)
			{
                sbAddress.Append(cAddress.Buildingname + "~");
				sbAddressText.Append(cAddress.Buildingname + ", ");
			}
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }
            
            if (cAddress.Street != string.Empty)
			{
				sbAddress.Append(cAddress.Street + "~");
				sbAddressText.Append(cAddress.Street + ", ");
			}

            if (cAddress.DepStreet != string.Empty && cAddress.Street == string.Empty)
			{
				sbAddress.Append(cAddress.DepStreet + "~");
				sbAddressText.Append(cAddress.DepStreet + ", ");
			}

            if (cAddress.DepStreet == string.Empty && cAddress.Street == string.Empty)
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

            if (cAddress.DepLocality != string.Empty)
			{
				sbAddress.Append(cAddress.DepLocality + "~");
				sbAddressText.Append(cAddress.DepLocality + ", ");
			}
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

            if (cAddress.Locality != string.Empty && cAddress.DepLocality == string.Empty)
			{
				sbAddress.Append(cAddress.Locality + "~");
				sbAddressText.Append(cAddress.Locality + ", ");
			}
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }
             

			if (cAddress.PostTown != string.Empty)
			{
				sbAddress.Append(cAddress.PostTown + "~");
				sbAddressText.Append(cAddress.PostTown + ", ");
			}
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

			if (cAddress.County != string.Empty)
			{
				sbAddress.Append(cAddress.County + "~");
				sbAddressText.Append(cAddress.County + ", ");
			}
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

            if (cAddress.Postcode != string.Empty)
            {
                sbAddress.Append(cAddress.Postcode + "~");
                sbAddressText.Append(cAddress.Postcode + ", ");
            }
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

            if (cAddress.Country != string.Empty)
            {
                sbAddress.Append(cAddress.Country + "~");
                sbAddressText.Append(cAddress.Country + ", ");
            }
            else
            {
                sbAddress.Append("~");
                //sbAddressText.Append(", ");
            }

			while (sbAddress.Length > 0 && sbAddress[sbAddress.Length - 1] == '~')
			{
				sbAddress.Length--;
			}

			while (sbAddressText.Length > 0 && (sbAddressText[sbAddressText.Length - 1] == ' ' || sbAddressText[sbAddressText.Length - 1] == ','))
			{
				sbAddressText.Length--;
			}

            ListItem lvItem = new ListItem(sbAddressText.ToString(), sbAddress.ToString());
            
			return lvItem;
		}
		#endregion

        protected void _btnAddressLookup_Click(object sender, ImageClickEventArgs e)
        {
            PostcodeLookup(string.Empty, string.Empty);
        }

        protected void PostcodeLookup(string AmbiguityId, string SearchStatus)
        {
            try
            {
                ContactServiceClient contactService = new ContactServiceClient();
                PostcodeLookupReturnValue returnValue = new PostcodeLookupReturnValue();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                PostcodeLookupSearchCriteria criteria = new PostcodeLookupSearchCriteria();

                criteria.Address = new Address();
                criteria.Address.OrganisationName = _txtOrganisation.Text;
                criteria.Address.PostBox = _txtPOBox.Text;
                criteria.Address.SubBuilding = _txtSubBuildingName.Text;
                criteria.Address.HouseName = _txtHouseName.Text;
                criteria.Address.StreetNumber = _txtHouseNumber.Text;
                criteria.Address.Line1 = _txtAddress1.Text;
                criteria.Address.Line2 = _txtAddress2.Text;
                criteria.Address.Line3 = _txtAddress3.Text;
                criteria.Address.DependantLocality = _txtDeptLoc.Text;
                criteria.Address.Town = _txtTown.Text;
                criteria.Address.County = _txtCounty.Text;
                criteria.Address.PostCode = _txtPostcode.Text;
                criteria.Address.Country = _txtCountry.Text;
                criteria.Address.DXNumber = _txtDXAddress1.Text;
                criteria.Address.DXTown = _txtDXAddress2.Text;
                criteria.AmbiguityId = AmbiguityId;
                criteria.SearchStatus = SearchStatus;
                returnValue = contactService.PostcodeLookupSearch(_logonId, criteria);

                if (returnValue != null)
                {
                    //Add each address
                    if (returnValue.PostcodeLookup != null)
                    {
                        _trServiceError.Style["display"] = "none";
                        _trServiceErrorSpace.Style["display"] = "none";
                        _listAddress.Style["display"] = "";

                        _listAddress.Items.Clear();

                        foreach (PostcodeLookupSearchItem cAddress in returnValue.PostcodeLookup)
                        {
                            this._listAddress.Items.Add(CreateListViewItem(cAddress));

                        }
                    }
                    else
                    {
                        string msg = string.Empty;

                        if (string.IsNullOrEmpty(returnValue.Message))
                            msg = "No matching or ambiguous addresses could be found for these details.";
                        else
                            msg = returnValue.Message;

                        _trServiceError.Style["display"] = "";
                        _trServiceErrorSpace.Style["display"] = "";
                        _listAddress.Style["display"] = "none";
                        _lblServiceError.Text = msg;
                    }
                }
                else
                {
                    _trServiceError.Style["display"] = "";
                    _trServiceErrorSpace.Style["display"] = "";
                    _listAddress.Style["display"] = "none";
                    _lblServiceError.Text = "No matching or ambiguous addresses could be found for these details.";
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                _trServiceError.Style["display"] = "";
                _trServiceErrorSpace.Style["display"] = "";
                _listAddress.Style["display"] = "none";
                _lblServiceError.Text = ex.Message;
            }
            _mpePostcodeLookup.Show();
        }

        
	}
}
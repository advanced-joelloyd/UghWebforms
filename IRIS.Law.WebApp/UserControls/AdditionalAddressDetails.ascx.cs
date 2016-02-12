using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Contact;


namespace IRIS.Law.WebApp.UserControls
{
    public partial class AdditionalAddressDetails : System.Web.UI.UserControl
    {
        #region Public Property
        /// <summary>
        /// variable to hold all the Additional adress details information
        /// </summary>
        private AdditionalAddressElement[] _additionalAddressDetails;

        /// <summary>
        /// Property to get\set the value of _additionalAddressDetails
        /// </summary>
        public AdditionalAddressElement[] AdditionalDetails
        {
            get
            {
                _additionalAddressDetails = GetAdditionalDetails();
                return _additionalAddressDetails;
            }
            set
            {
               _additionalAddressDetails = value;
            }
        }
        #endregion 

        protected void Page_Load(object sender, EventArgs e)
        {

        }

		/// <summary>
		/// Clears the fields.
		/// </summary>
        public void ClearFields()
        {
            _txtHomeTelephone.Text = string.Empty;
            _txtWorkTel1.Text = string.Empty;
            _txtWorkTel2.Text = string.Empty;
            _txtDDI.Text = string.Empty;
            _txtMob1.Text = string.Empty;
            _txtMob2.Text = string.Empty;
            _txtFax.Text = string.Empty;
            _txtHomeEmail.Text = string.Empty;
            _txtWorkEmail.Text = string.Empty;
            _txtURL.Text = string.Empty;
        }

        /// <summary>
        /// Set the values for additional details
        /// </summary>
        /// <returns></returns>
        public AdditionalAddressElement[] GetAdditionalDetails()
        {
            AdditionalAddressElement[] addressDetails = new AdditionalAddressElement[10];

            #region Set Second Person's Additional Address Information
            for (int i = 0; i <= 9; i++)
            {
                addressDetails[i] = new AdditionalAddressElement();
                switch (i)
                {
                    case 0:
                        addressDetails[0].ElementText = _txtHomeTelephone.Text;
                        break;
                    case 1:
                        addressDetails[1].ElementText = _txtWorkTel1.Text;
                        break;
                    case 2:
                        addressDetails[2].ElementText = _txtWorkTel2.Text;
                        break;
                    case 3:
                        addressDetails[3].ElementText = _txtDDI.Text;
                        break;
                    case 4:
                        addressDetails[4].ElementText = _txtMob1.Text;
                        break;
                    case 5:
                        addressDetails[5].ElementText = _txtMob2.Text;
                        break;
                    case 6:
                        addressDetails[6].ElementText = _txtFax.Text;
                        break;
                    case 7:
                        addressDetails[7].ElementText = _txtHomeEmail.Text;
                        break;
                    case 8:
                        addressDetails[8].ElementText = _txtWorkEmail.Text;
                        break;
                    case 9:
                        addressDetails[9].ElementText = _txtURL.Text;
                        break;
                }
            }
            #endregion
            
            return addressDetails;
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            #region Set Second Person's Additional Address Information

            for (int j = 0; j <= _additionalAddressDetails.Length -1; j++)
            {

                if (_additionalAddressDetails.Length != 0)
                {
                    if (_additionalAddressDetails[j].TypeId == 1)
                    {
                        _txtHomeTelephone.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 2)
                    {
                        _txtWorkTel1.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 3)
                    {
                        _txtWorkTel2.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 4)
                    {
                        _txtMob1.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 5)
                    {
                        _txtMob2.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 6)
                    {
                        _txtFax.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 7)
                    {
                        _txtHomeEmail.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 8)
                    {
                        _txtWorkEmail.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 9)
                    {
                        _txtURL.Text = _additionalAddressDetails[j].ElementText;
                    }
                    if (_additionalAddressDetails[j].TypeId == 10)
                    {
                        _txtDDI.Text = _additionalAddressDetails[j].ElementText;
                    }
                }
               
                }
            }
          
            #endregion

        /// <summary>
        /// Disable the fields.
        /// </summary>
        public void DisableFields()
        {
            _txtHomeTelephone.Enabled = false;
            _txtWorkTel1.Enabled = false;
            _txtWorkTel2.Enabled = false;
            _txtDDI.Enabled = false;
            _txtMob1.Enabled = false;
            _txtMob2.Enabled = false;
            _txtFax.Enabled = false;
            _txtHomeEmail.Enabled = false;
            _txtWorkEmail.Enabled = false;
            _txtURL.Enabled = false;
        }

        }
    }

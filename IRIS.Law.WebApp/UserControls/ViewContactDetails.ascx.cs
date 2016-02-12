using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ViewContactDetails : System.Web.UI.UserControl
    {
        #region Private Members
        AddressSearchReturnValue _addressSearchReturnValue;
        private Guid _logonId;

        #endregion


        #region Public Properties

        private Guid _memId = DataConstants.DummyGuid;

        public Guid MemberId
        {
            get
            {
                return _memId;
            }
            set
            {
                _memId = value;
            }
        }

        private Guid _orgId = DataConstants.DummyGuid;

        public Guid OrganisationId
        {
            get
            {
                return _orgId;
            }
            set
            {
                _orgId = value;
            }
        }

        private bool _isServiceContact;

        public bool IsServiceContact
        {
            get
            {
                return _isServiceContact;
            }
            set
            {
                _isServiceContact = value;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            _logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
            if (!IsPostBack)
            {
                ViewState["IndividualsDropdownsPopulated"] = false;
                ViewState["OrganisationsDropdownsPopulated"] = false;
 
            }
            _ucAddress.DisableFields();
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays a popup with the contact details
        /// </summary>
        public void DisplayPopup()
        {
            try
            {
                if (_isServiceContact)
                {
                    GetServiceContactDetails();
                }
                else
                {
                    GetGeneralContactDetails();
                }
                if (_memId != DataConstants.DummyGuid || _orgId != DataConstants.DummyGuid)
                {
                    LoadAddressDetails();
                }
                _mpeViewContactDetails.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        private void GetGeneralContactDetails()
        {
            if (_memId == DataConstants.DummyGuid && _orgId == DataConstants.DummyGuid)
            {
                throw new Exception("Member/Organisation Id not set");
            }
            else
            {
                ContactServiceClient contactService = null;
                try
                {
                    contactService = new ContactServiceClient();
                    GeneralContactReturnValue returnValue = contactService.GetGeneralContact(_logonId, _memId, _orgId);
                    if (returnValue.Success)
                    {
                        if (_orgId == DataConstants.DummyGuid)
                        {
                            DisplayIndividualDetails(returnValue.Person);
                            SelectDropDownListValue(_ddlSourceCampaign, returnValue.CampaignId.ToString());
                            _chkReceivesMarketing.Checked = returnValue.IsReceivingMarketingStatus;
                        }
                        else
                        {
                            DisplayOrganisationDetails(returnValue.Organisation);
                            SelectDropDownListValue(_ddlSourceCampaignOrg, returnValue.CampaignId.ToString());
                            _chkReceivesMarketingOrg.Checked = returnValue.IsReceivingMarketingStatus;
                        }
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (contactService != null)
                    {
                        if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                            contactService.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Selects a value from a dropDownList if it exits in the items collection
        /// </summary>
        /// <param name="dropDownList"></param>
        /// <param name="value"></param>
        private void SelectDropDownListValue(DropDownList dropDownList, string value)
        {
            if (dropDownList.Items.FindByValue(value.Trim()) != null)
            {
                dropDownList.SelectedValue = value.Trim();
            }
            else
            {
                dropDownList.SelectedIndex = 0;
            }
        }

        private void GetServiceContactDetails()
        {
            if (_memId == DataConstants.DummyGuid)
            {
                throw new Exception("Member Id not set");
            }
            else
            {
                ContactServiceClient contactService = null;
                try
                {
                    contactService = new ContactServiceClient();
                    ServiceContactReturnValue returnValue = contactService.GetServiceContact(_logonId, _memId);
                    if (returnValue.Success)
                    {
                        DisplayIndividualDetails(returnValue.ServiceContact);
                        SelectDropDownListValue(_ddlSourceCampaign, returnValue.CampaignId.ToString());
                        _chkReceivesMarketing.Checked = returnValue.IsReceivingMarketingStatus;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (contactService != null)
                    {
                        if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                            contactService.Close();
                    }
                }
            }
        }

        private void DisplayOrganisationDetails(Organisation organisation)
        {
            _pnlContactDetails.Style.Add("width", "800px");
            _pnlContactDetails.Style.Add("height", "500px");
            _pnlOrganisation.Visible = true;
            _pnlIndividual.Visible = false;

            //Get the data for the dropdown lists only if we havent fetched it earlier
            if (!(bool)ViewState["OrganisationsDropdownsPopulated"])
            {
                BindOrganisationsDropdownLists();
                ViewState["OrganisationsDropdownsPopulated"] = true;
            }

            _txtOrganisationName.Text = organisation.Name;
            _txtRegisteredName.Text = organisation.RegisteredName;
            _txtRegisteredNo.Text = organisation.RegisteredNo;
            _txtVATNo.Text = organisation.VATNo;
            _ddlIndustry.SelectedValue = organisation.IndustryId.ToString();
            _ddlSubType.SelectedValue = organisation.SubTypeId.ToString();
        }

        private void DisplayIndividualDetails(Person person)
        {
            _pnlContactDetails.Style.Add("width", "800px");
            _pnlContactDetails.Style.Add("height", "500px");
            _pnlOrganisation.Visible = false;
            _pnlIndividual.Visible = true;

            //Get the data for the dropdown lists only if we havent fetched it earlier
            if (!(bool)ViewState["IndividualsDropdownsPopulated"])
            {
                BindIndividualsDropdownLists();
                ViewState["IndividualsDropdownsPopulated"] = true;
            }
            if (person.Title != string.Empty)
                _ddlTitle.SelectedValue = person.Title;

            _txtForenames.Text = person.ForeName;
            _txtSurname.Text = person.Surname;
            _ddlMaritalStatus.SelectedValue = person.MaritalStatusId.ToString();
            _txtPreviousName.Text = person.PreviousName;
            _txtOccupation.Text = person.Occupation;
            _ddlSex.SelectedValue = person.Sex.ToString();
            if (person.DOB != DataConstants.BlankDate)
            {
                _ccDOBDate.DateText = person.DOB.ToString("dd/MM/yyyy");
                //Contact blank date in the database was being stored as 01/01/1900 which is not the system blankdate
                //Therefore added a test to make sure that the date of death is later than the date of birth.
                //If it isn't, restore date of death to system blankdate as it is invalid.
                if ((person.DOD != DataConstants.BlankDate) && (person.DOD < person.DOB))
                {
                    _ccDODDate.DateText = DataConstants.BlankDate.ToString("dd/MM/yyyy");
                }
            }

            _txtAge.Text = AppFunctions.CalculateAge(person.DOB, person.DOD);
            if (person.DOD != DataConstants.BlankDate)
            {
                _ccDODDate.DateText = person.DOD.ToString("dd/MM/yyyy");
            }
            _txtPlaceOfBirth.Text = person.PlaceOfBirth;
            _txtBirthName.Text = person.BirthName;
            _txtFormalSalutation.Text = person.SalutationLettterFormal;
            _txtInformalSalutation.Text = person.SalutationLettterInformal;
            _txtFriendlySalutation.Text = person.SalutationLettterFriendly;
            _txtEnvelopeSalutation.Text = person.SalutationEnvelope;
            _ddlEthnicity.SelectedValue = person.EthnicityId.ToString();
            _ddlDisability.SelectedValue = person.DisabilityId.ToString();
            _chkArmedForces.Checked = person.IsInArmedForces;
            _txtArmedForcesNo.Text = person.ArmedForcesNo;
            _txtNINo.Text = person.NINo;
        }

        private void GetCampaigns(DropDownList dropDownList)
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                CampaignSearchCriteria searchCriteria = new CampaignSearchCriteria();
                CampaignSearchReturnValue returnValue = contactService.CampaignSearch(_logonId, collectionRequest,
                                                                                       searchCriteria);

                if (returnValue.Success)
                {
                    dropDownList.DataSource = returnValue.Campaigns.Rows;
                    dropDownList.DataTextField = "Description";
                    dropDownList.DataValueField = "CampaignId";
                    dropDownList.DataBind();
                    dropDownList.Items.Insert(0, "");
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        #region Individuals Dropdowns

        private void BindIndividualsDropdownLists()
        {
            try
            {
                GetTitles();
                GetMaritalStatus();
                GetSex();
                GetEthnicity();
                GetDisability();
                GetCampaigns(_ddlSourceCampaign);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the disability values for the drop down list.
        /// </summary>
        private void GetDisability()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                DisabilitySearchCriteria searchCriteria = new DisabilitySearchCriteria();
                searchCriteria.IncludeArchived = false;
                DisabilitySearchReturnValue returnValue = contactService.DisabilitySearch(_logonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlDisability.DataSource = returnValue.Disabilities.Rows;
                    _ddlDisability.DataTextField = "Description";
                    _ddlDisability.DataValueField = "Id";
                    _ddlDisability.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the ethnicity values for the drop down list.
        /// </summary>
        private void GetEthnicity()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                EthnicitySearchCriteria searchCriteria = new EthnicitySearchCriteria();
                EthnicitySearchReturnValue returnValue = contactService.EthnicitySearch(_logonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlEthnicity.DataSource = returnValue.Ethnicity.Rows;
                    _ddlEthnicity.DataTextField = "Description";
                    _ddlEthnicity.DataValueField = "Id";
                    _ddlEthnicity.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the sex values for the drop down list.
        /// </summary>
        private void GetSex()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                SexSearchCriteria searchCriteria = new SexSearchCriteria();
                searchCriteria.IncludeArchived = false;
                SexSearchReturnValue returnValue = contactService.SexSearch(_logonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlSex.DataSource = returnValue.Sex.Rows;
                    _ddlSex.DataTextField = "Description";
                    _ddlSex.DataValueField = "Id";
                    _ddlSex.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the marital status values for the drop down list.
        /// </summary>
        private void GetMaritalStatus()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                MaritalStatusSearchCriteria searchCriteria = new MaritalStatusSearchCriteria();
                MaritalStatusSearchReturnValue returnValue = contactService.MaritalStatusSearch(_logonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlMaritalStatus.DataSource = returnValue.MaritalStatus.Rows;
                    _ddlMaritalStatus.DataTextField = "Description";
                    _ddlMaritalStatus.DataValueField = "Id";
                    _ddlMaritalStatus.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the title values for the drop down list..
        /// </summary>
        private void GetTitles()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                TitleSearchCriteria searchCriteria = new TitleSearchCriteria();
                TitleSearchReturnValue returnValue = contactService.TitleSearch(_logonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _ddlTitle.DataSource = returnValue.Title.Rows;
                    _ddlTitle.DataTextField = "TitleId";
                    _ddlTitle.DataValueField = "TitleId";
                    _ddlTitle.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        #endregion

        #region Organisations Dropdowns

        private void BindOrganisationsDropdownLists()
        {
            try
            {
                GetIndustries();
                GetSubTypes();
                GetCampaigns(_ddlSourceCampaignOrg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the sub types.
        /// </summary>
        private void GetSubTypes()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                OrganisationSubTypeSearchCriteria searchCriteria = new OrganisationSubTypeSearchCriteria();
                OrganisationSubTypeSearchReturnValue returnValue = contactService.OrganisationSubTypeSearch(
                                                            _logonId, collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlSubType.DataSource = returnValue.SubTypes.Rows;
                    _ddlSubType.DataTextField = "Description";
                    _ddlSubType.DataValueField = "Id";
                    _ddlSubType.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the industries.
        /// </summary>
        private void GetIndustries()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                IndustrySearchCriteria searchCriteria = new IndustrySearchCriteria();
                IndustrySearchReturnValue returnValue = contactService.IndustrySearch(_logonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    //Add a blank item
                    _ddlIndustry.Items.Add(new ListItem("", "0"));

                    //Generate the items to be displayed in the Industry drop down list
                    //Get the main items
                    string industryText = string.Empty;
                    foreach (IndustrySearchItem industry in returnValue.Industries.Rows)
                    {
                        if (industry.ParentId == 0)
                        {
                            industryText = industry.Name;
                            _ddlIndustry.Items.Add(new ListItem(industryText, industry.Id.ToString()));

                            // Call method to get the sub items
                            GetIndustrySubItems(returnValue.Industries.Rows, industryText, industry.Id);
                        }
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        private void GetIndustrySubItems(IndustrySearchItem[] industrySearchItems, string industryText, int industryId)
        {
            foreach (IndustrySearchItem industry in industrySearchItems)
            {
                // If the IndustryParentID is not 0 and the IndustryParentID equals the industry id of the item
                // you specified then add a sub item.
                if (industry.ParentId != 0 && industry.ParentId == industryId)
                {
                    string itemText = industryText + "->" + industry.Name;
                    _ddlIndustry.Items.Add(new ListItem(itemText, industry.Id.ToString()));

                    //Call recursively to get the sub items
                    GetIndustrySubItems(industrySearchItems, itemText, industry.Id);
                }
            }
        }


        #endregion



       



        #region Address Details Tab

         /// <summary>
        /// Loads the client details.
        /// </summary>
        private void LoadAddressDetails()
        {
            ClientServiceClient serviceClient = null;
            try
            {
                serviceClient = new ClientServiceClient();
                _addressSearchReturnValue = new AddressSearchReturnValue();
                CollectionRequest collectionRequest = new CollectionRequest();
                AddressSearchCriteria searchCriteria = new AddressSearchCriteria();
                searchCriteria.MemberId = _memId;
                searchCriteria.OrganisationId = _orgId;
                _addressSearchReturnValue = serviceClient.GetClientAddresses(_logonId, collectionRequest, searchCriteria);
                
                if (_addressSearchReturnValue.Success)
                {
                    GetAddressTypes();
                    DisplayAddressDetails();
                }
                else
                {
                    throw new Exception(_addressSearchReturnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (serviceClient != null)
                {
                    if (serviceClient.State != System.ServiceModel.CommunicationState.Faulted)
                        serviceClient.Close();
                }
            }
        }
 
        /// <summary>
        /// Displays the address details for the selected address type.
        /// </summary>
        private void DisplayAddressDetails()
        {
            try
            {
                List<Address> clientAddresses = null;
                Address currentAddress = null;
                if (_addressSearchReturnValue != null)
                {
                    //Store the addresses in a list so that we can add items if necessary
                    clientAddresses = new List<Address>();
                    foreach (Address address in _addressSearchReturnValue.Addresses.Rows)
                    {
                        clientAddresses.Add(address);
                    }
                    Session[SessionName.ClientAddresses] = clientAddresses;
                }
                else
                {
                    clientAddresses = (List<Address>)Session[SessionName.ClientAddresses];
                }

                //Get the current selected address type and display the address
                foreach (Address address in clientAddresses)
                {
                    if (address.TypeId == Convert.ToInt32(_ddlAddressType.SelectedValue))
                    {
                        currentAddress = address;
                        break;
                    }
                }
                //If an address exists then display it..
                if (currentAddress != null)
                {
                    _ucAddress.Address = currentAddress;
                    _ucAddress.DataBind();
                    if (currentAddress.AdditionalAddressElements != null)
                    {
                        _aadContactInfo.AdditionalDetails = currentAddress.AdditionalAddressElements;
                        _aadContactInfo.ClearFields();
                        _aadContactInfo.DataBind();
                        _aadContactInfo.DisableFields();
                    }
                }
                else
                {
                    //No address exists for the current type, so create a new one
                    currentAddress = new Address();
                    currentAddress.LastVerified = DataConstants.BlankDate;
                    currentAddress.TypeId = Convert.ToInt32(_ddlAddressType.SelectedValue);
                    clientAddresses.Add(currentAddress);
                    _ucAddress.Address = currentAddress;
                    _ucAddress.IsDataChanged = false;
                    _ucAddress.DataBind();
                    if (currentAddress.AdditionalAddressElements != null)
                    {
                        _aadContactInfo.AdditionalDetails = currentAddress.AdditionalAddressElements;
                        _aadContactInfo.ClearFields();
                        _aadContactInfo.DataBind();
                        _aadContactInfo.DisableFields();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to display the available address types.
        /// </summary>
        private void GetAddressTypes()
        {
            ContactServiceClient contactService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                AddressTypeSearchCriteria searchCriteria = new AddressTypeSearchCriteria();
                searchCriteria.MemberId = _memId;
                searchCriteria.OrganisationId = _orgId;

                contactService = new ContactServiceClient();
                AddressTypeReturnValue returnValue = contactService.GetAddressTypes(_logonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _ddlAddressType.DataSource = returnValue.AddressTypes.Rows;
                    _ddlAddressType.DataTextField = "Description";
                    _ddlAddressType.DataValueField = "Id";
                    _ddlAddressType.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        protected void _ddlAddressType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DisplayAddressDetails();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }


        #endregion

        #endregion
    }
}
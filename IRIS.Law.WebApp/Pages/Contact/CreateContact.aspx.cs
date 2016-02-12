using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.Services.Pms.Client;
using IRIS.Law.Services.Pms.Contact;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.Contact
{
    public partial class CreateContact : BasePage
    {
        #region Private Variable
        private LogonReturnValue _logonSettings;
        #endregion

        #region Protected Methods

        #region Form Events

       
        protected override void OnInit(EventArgs e)
        {

            

            base.OnInit(e);
        }

        #region Page_Load

        /// <summary>
        /// Loads page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
                // Resets message label if "SuccessMesage" session is null.
                _lblError.Text = string.Empty;

                // Success message if contact saved successfully.
                if (Session["SuccessMesage"] != null)
                {
                    _lblError.CssClass = "successMessage";
                    _lblError.Text = Convert.ToString(Session["SuccessMesage"]);
                    Session["SuccessMesage"] = null;
                }

                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                _ddlContactType.Attributes.Add("onchange", "javascript:HideUnhideControlsOnContactType('" + _ddlContactType.ClientID + "');");
                _ddlServiceType.Attributes.Add("onchange", "javascript:return HideUnhideContactTypeonServiceType('" + _ddlServiceType.ClientID + "');");

                HideUnhideContactTypeonServiceType();             

                if (!Page.IsPostBack)
                {
                    _tblConflictCheck.Style["display"] = "none";
                    BindTitleDropDown();
                    BindIndustries();

                    // Sets display property for table defined for conflict check.
                    _tblConflictCheck.Style["display"] = "none";
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }

        

        #endregion

        #region Cancel Button Click Event on Conflict Check

        /// <summary>
        /// Resets the controls on cancelling the contact creation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnConflictCancel_Click(object sender, EventArgs e)
        {
            try
            {
                // Resets controls on page
                ResetControls();
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }

        #endregion

        #region ServiceSelected Event

        protected void _ssServiceSearch_ServiceSelected(object sender, EventArgs e)
        {
            try
            {
                ViewState["ServiceId"] = Convert.ToString(_ssService.ServiceId);
                ViewState["ServiceName"] = Convert.ToString(_ssService.ServiceText);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Checkbox UseSameAddress

        /// <summary>
        /// If Use Same Address checkbox is checked, 
        /// all address details will get copied to the Second Person's Address details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _chkUseSameAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chkUseSameAddress.Checked)
                {
                    _addressServiceDetails.Address = _addressDetails.Address;
                    _addressServiceDetails.DataBind();
                }
                else
                {
                    _addressServiceDetails.ClearFields();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #endregion

        #region Wizard Navigation Events

        #region Start Next Button

        /// <summary>
        /// Next button click on wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StartNextButton_Click(object sender, EventArgs e)
        {
            try
            {
                // After click "OK" on conflict check resets the success message 
                if (!string.IsNullOrEmpty(_lblError.Text))
                {
                    _lblError.Text = string.Empty;
                }

                if (_ddlServiceType.SelectedItem.Value == "General Contact")
                {
                    _wizardContact.WizardSteps.Remove(_wizardStepContactDetails);
                    _lblContactHeader.Text = "Add New General Contact";
                }
                else if (_ddlServiceType.SelectedItem.Value == "Service")
                {
                    _lblContactHeader.Text = "Add New Service";
                    _lblAddressDetailsHeader.Text = "Service Address Details";
                    _lblContactInfoHeader.Text = "Service Additional Address Details";
                    //_wizardContact.WizardSteps.Remove(_wizardStepContactDetails);
                }
                else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                {
                    _lblContactHeader.Text = "Add New Service Contact";
                    _lblContactDetailsHeader.Text = "Service Contact Details";
                    _lblAddressDetailsHeader.Text = "Service Contact Address Details";
                    _lblContactInfoHeader.Text = "Service Contact Additional Address Details";

                    if (_wizardContact.ActiveStepIndex == 0)
                    {
                        _cdContactDetails.BindTitle();
                        _cdContactDetails.BindSex();
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Step Cancel Button

        /// <summary>
        /// Wizard navigation cancellation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnWizardStartNavCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ResetControls();
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Step Next Button

        /// <summary>
        /// Next button click event on wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepNextButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardContact.ActiveStepIndex > 0)
                {
                    if (_ddlServiceType.SelectedItem.Value == "General Contact")
                    {
                        _lblContactHeader.Text = "Add New General Contact";
                        if (_wizardContact.ActiveStepIndex == 1)
                        {
                            _wizardContact.ActiveStepIndex = _wizardContact.ActiveStepIndex + 1;
                        }

                        //Remove steps that are not required while adding a general contact
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceContactDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressInfo);
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service")
                    {
                        _lblContactHeader.Text = "Add New Service";
                        _lblAddressDetailsHeader.Text = "Service Address Details";
                        _lblContactInfoHeader.Text = "Service Additional Address Details";

                        if (_wizardContact.ActiveStepIndex == 3)
                        {
                            _cdServiceContactDetails.BindTitle();
                            _cdServiceContactDetails.BindSex();
                        }
                        else if (_wizardContact.ActiveStepIndex == 1)
                        {
                            _cdServiceContactDetails.BindTitle();
                            _cdServiceContactDetails.BindSex();
                            //_wizardContact.WizardSteps.Remove(_wizardStepServiceContactDetails);
                            _wizardContact.ActiveStepIndex = _wizardContact.ActiveStepIndex + 1;
                        }
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                    {
                        _lblContactHeader.Text = "Add New Service Contact";
                        _lblAddressDetailsHeader.Text = "Service Contact Address Details";
                        _lblContactInfoHeader.Text = "Service Contact Additional Address Details";

                        //Remove steps that are not required while adding a service contact
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceContactDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressInfo);
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Step Previous Button

        /// <summary>
        /// Previous button click event on wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepPreviousButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardContact.ActiveStepIndex > 0)
                {
                    if (_ddlServiceType.SelectedItem.Value == "General Contact")
                    {
                        _lblContactHeader.Text = "Add New General Contact";
                        _wizardContact.WizardSteps.Remove(_wizardStepContactDetails);
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service")
                    {
                        _lblContactHeader.Text = "Add New Service";
                        _lblAddressDetailsHeader.Text = "Service Address Details";
                        _lblContactInfoHeader.Text = "Service Additional Address Details";
                        if (_wizardContact.ActiveStepIndex < 4)
                        {
                            //_wizardContact.WizardSteps.Remove(_wizardStepContactDetails);
                        }
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                    {
                        _lblContactHeader.Text = "Add New Service Contact";
                        _lblContactDetailsHeader.Text = "Service Contact Details";
                        _lblAddressDetailsHeader.Text = "Service Contact Address Details";
                        _lblContactInfoHeader.Text = "Service Contact Additional Address Details";
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Step Finish Previous Button

        /// <summary>
        /// Finish Previous button click event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepFinishPreviousButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardContact.ActiveStepIndex > 0)
                {
                    if (_ddlServiceType.SelectedItem.Value == "General Contact")
                    {
                        _lblContactHeader.Text = "Add New General Contact";
                        _wizardContact.WizardSteps.Remove(_wizardStepContactDetails);
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service")
                    {
                        _lblContactHeader.Text = "Add New Service";
                        _lblAddressDetailsHeader.Text = "Service Address Details";
                        _lblContactInfoHeader.Text = "Service Additional Address Details";
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                    {
                        _lblContactHeader.Text = "Add New Service Contact";
                        _lblAddressDetailsHeader.Text = "Service Contact Address Details";
                        _lblContactInfoHeader.Text = "Service Contact Additional Address Details";
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #region Step Finish Button

        /// <summary>
        /// Finish button click event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepFinishButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardContact.ActiveStepIndex > 0)
                {
                    if (_ddlServiceType.SelectedItem.Value == "General Contact")
                    {
                        _lblContactHeader.Text = "Add New General Contact";

                        if (_wizardContact.ActiveStepIndex == 1)
                        {
                            _wizardContact.ActiveStepIndex = _wizardContact.ActiveStepIndex + 1;
                        }
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceContactDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressInfo);
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service")
                    {
                        _lblContactHeader.Text = "Add New Service";
                    }
                    else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                    {
                        _lblContactHeader.Text = "Add New Service Contact";
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceContactDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressDetails);
                        _wizardContact.WizardSteps.Remove(_wizardStepServiceAddressInfo);
                    }
                }

                // If "canAddNewContact" flag is false, then conflict has been 
                // occurred and new contact cannot be added.
                bool canAddNewContact = ConflictCheck();

                if (!canAddNewContact)
                {
                    _wizardContact.Visible = false;

                    if (_conflictCheck.ReturnConflictCheck.Summary.Length > 0)
                    {
                        ViewState["ContactConflictNoteSummary"] = _conflictCheck.ReturnConflictCheck.Summary.ToString();
                    }

                    ViewState["ContactConflictNoteContent"] = _conflictCheck.ContactConflictNoteContent;
                }
                else
                {
                    _wizardContact.Visible = true;

                    string conflictNoteContent = "No matches found";
                    string conflictNoteSummary = _conflictCheck.ReturnConflictCheck.Summary.ToString();

                    _tblConflictCheck.Style["display"] = "none";

                    if (_ddlServiceType.Text == "General Contact")
                    {
                        CreateGeneralContact(conflictNoteSummary, conflictNoteContent);
                    }
                    else if (_ddlServiceType.Text == "Service")
                    {
                        CreateService(conflictNoteSummary, conflictNoteContent);
                    }
                    else
                    {
                        CreateServiceContact(conflictNoteSummary, conflictNoteContent);
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #endregion

        #region Conflict OK button click event

        /// <summary>
        /// Button click event on getting conflicts while adding contact.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnConflickOK_Click(object sender, EventArgs e)
        {
            string conflictNoteContent = string.Empty;
            string conflictNoteSummary = string.Empty;

            try
            {
                if (ViewState["ContactConflictNoteSummary"] != null &&
                    ViewState["ContactConflictNoteContent"] != null)
                {
                    conflictNoteSummary = Convert.ToString(ViewState["ContactConflictNoteSummary"]);
                    conflictNoteContent = Convert.ToString(ViewState["ContactConflictNoteContent"]);
                }

                if (_ddlServiceType.Text == "General Contact")
                {
                    CreateGeneralContact(conflictNoteSummary, conflictNoteContent);
                }
                else if (_ddlServiceType.Text == "Service")
                {
                    CreateService(conflictNoteSummary, conflictNoteContent);
                }
                else
                {
                    CreateServiceContact(conflictNoteSummary, conflictNoteContent);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region BindTitleDropDown

        /// <summary>
        /// Binds title dropdownlist control
        /// </summary>
        private void BindTitleDropDown()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                TitleSearchCriteria titleCriteria = new TitleSearchCriteria();
                TitleSearchReturnValue titleReturnValue = contactService.TitleSearch(_logonSettings.LogonId, collectionRequest, titleCriteria);
                if (titleReturnValue.Title != null)
                {
                    _ddlTitle.DataSource = titleReturnValue.Title.Rows;
                    _ddlTitle.DataTextField = "TitleId";
                    _ddlTitle.DataValueField = "TitleId";
                    _ddlTitle.DataBind();
                }

                // Adds "Select" at zero index of dropdownlist control.
                AddDefaultToDropDownList(_ddlTitle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                    contactService.Close();
            }
        }

        #endregion

        #region AddDefaultToDropDownList

        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }

        #endregion

        #region Create Contacts

        /// <summary>
        /// Create General contact for Individual and Organisation Contact Type 
        /// </summary>
        private void CreateGeneralContact(string conflictNoteSummary, string conflictNoteContent)
        {
            ContactServiceClient contactService = null;
            try
            {
                Person person = null;
                Organisation organisation = null;
                ContactType contactType = ContactType.Individual;

                if (_ddlContactType.Text == "Individual")
                {
                    person = new Person();
                    person.ForeName = _txtForename.Text;
                    person.Surname = _txtSurname.Text;
                    person.Title = _ddlTitle.Text;
                    contactType = ContactType.Individual;
                }

                if (_ddlContactType.Text == "Organisation")
                {
                    organisation = new Organisation();
                    organisation.Name = _txtOrgName.Text;
                    contactType = ContactType.Organisation;
                }

                // Gets the address details
                Address address = new Address();
                address = _addressDetails.Address;
                address.TypeId = 1;

                contactService = new ContactServiceClient();
                ReturnValue returnValue = contactService.SaveGeneralContact(_logonSettings.LogonId, address,
                                                            person, _aadContactInfo.AdditionalDetails,
                                                            contactType, organisation, conflictNoteSummary,
                                                            conflictNoteContent);

                if (returnValue.Success)
                {
                    _wizardContact.Visible = true;
                    Session["SuccessMesage"] = "General Contact saved successfully";

                    ResetControls();
                }
                else
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = returnValue.Message;
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
        /// Create Service 
        /// </summary>
        private void CreateService(string conflictNoteSummary, string conflictNoteContent)
        {
            ContactServiceClient contactService = null;
            try
            {
                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.ServiceName = _txtServiceName.Text;
                serviceInfo.IndustryId = Convert.ToInt32(_ddlIndustry.SelectedValue);

                //Get the address details
                Address serviceAddress = _addressDetails.Address;

                // Stores Service contact details entered by the user
                ServiceContact serviceContactInfo = new ServiceContact();
                serviceContactInfo.ServiceName = _ssService.ServiceText;
                serviceContactInfo.SurName = _cdServiceContactDetails.SurName;
                serviceContactInfo.ForeName = _cdServiceContactDetails.ForeName;
                serviceContactInfo.Title = _cdServiceContactDetails.Title;
                serviceContactInfo.Sex = Convert.ToInt32(false);
                serviceContactInfo.Position = _cdServiceContactDetails.Position;
                serviceContactInfo.Description = _cdServiceContactDetails.Description;

                //Get the address details
                Address contactAddress = _addressServiceDetails.Address;

                contactService = new ContactServiceClient();
                ReturnValue returnValue = contactService.SaveService(_logonSettings.LogonId, serviceAddress,
                                                                    _aadServiceAddressInfo.AdditionalDetails, serviceInfo,
                                                                    serviceContactInfo, contactAddress,
                                                                    _aadContactInfo.AdditionalDetails, conflictNoteSummary,
                                                                    conflictNoteContent);

                if (returnValue.Success)
                {
                    _wizardContact.Visible = true;
                    Session["SuccessMesage"] = "Service saved successfully";

                    ResetControls();
                }
                else
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = returnValue.Message;
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
        /// Create Service contract
        /// </summary>
        private void CreateServiceContact(string conflictNoteSummary, string conflictNoteContent)
        {
            ContactServiceClient contactService = null;
            try
            {
                ServiceContact serviceContactInfo = new ServiceContact();

                serviceContactInfo.ServiceId = new Guid(Convert.ToString(ViewState["ServiceId"]));
                serviceContactInfo.ServiceName = Convert.ToString(ViewState["ServiceName"]);
                serviceContactInfo.SurName = _cdContactDetails.SurName;
                serviceContactInfo.ForeName = _cdContactDetails.ForeName;
                serviceContactInfo.Title = _cdContactDetails.Title;
                if (_cdContactDetails.Sex!=string.Empty)
                    serviceContactInfo.Sex = int.Parse(_cdContactDetails.Sex);
                serviceContactInfo.Position = _cdContactDetails.Position;

                //Get the address details
                Address address = new Address();
                address = _addressDetails.Address;

                contactService = new ContactServiceClient();

                ReturnValue returnValue = contactService.SaveServiceContact(_logonSettings.LogonId, address, _aadContactInfo.AdditionalDetails, serviceContactInfo, conflictNoteSummary, conflictNoteContent);

                if (returnValue.Success)
                {
                    _wizardContact.Visible = true;
                    Session["SuccessMesage"] = "Service Contact saved successfully";

                    ResetControls();
                }
                else
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = returnValue.Message;
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

        #region ResetControls

        /// <summary>
        /// Resets all the controls for creating new contact
        /// </summary>
        private void ResetControls()
        {
            try
            {
                Response.Redirect("~/Pages/Contact/CreateContact.aspx", true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region HideUnhideControlsOnClientType

        /// <summary>
        /// Hide Unhide the controls when Client type is changed
        /// </summary>
        private void HideUnhideControlsOnClientType()
        {
            try
            {
                string clientType = string.Empty;
                clientType = _ddlContactType.SelectedItem.Value;

                _trSurname.Style["display"] = "none";
                _trForename.Style["display"] = "none";
                _trTitle.Style["display"] = "none";
                _trOrgName.Style["display"] = "none";

                _rfvSurname.Enabled = false;
                _rfvOrgName.Enabled = false;

                switch (clientType)
                {
                    case "Individual":
                        _trSurname.Style["display"] = "";
                        _trForename.Style["display"] = "";
                        _trTitle.Style["display"] = "";
                        _rfvSurname.Enabled = true;
                        break;
                    case "Organisation":
                        _trOrgName.Style["display"] = "";
                        _rfvOrgName.Enabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region HideUnhideContactTypeonServiceType

        /// <summary>
        /// Hides and unhides contact type on change of service type
        /// </summary>
        private void HideUnhideContactTypeonServiceType()
        {
            try
            {
                _tblGeneralContact.Style["display"] = "none";
                _tblServiceContact.Style["display"] = "none";
                _tblService.Style["display"] = "none";

                if (_ddlServiceType.SelectedValue == "General Contact")
                {
                    _tblGeneralContact.Style["display"] = "";
                    HideUnhideControlsOnClientType();
                }
                else if (_ddlServiceType.SelectedValue == "Service")
                {
                    _tblService.Style["display"] = "";
                }
                else if (_ddlServiceType.SelectedValue == "Service Contact")
                {
                    _tblServiceContact.Style["display"] = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BindIndustries

        /// <summary>
        /// Gets the industries.
        /// </summary>
        private void BindIndustries()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                IndustrySearchCriteria searchCriteria = new IndustrySearchCriteria();
                IndustrySearchReturnValue returnValue = contactService.IndustrySearch(_logonSettings.LogonId,
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

        /// <summary>
        /// Gets sub items for industry.
        /// </summary>
        /// <param name="industrySearchItems"></param>
        /// <param name="industryText"></param>
        /// <param name="industryId"></param>
        private void GetIndustrySubItems(IndustrySearchItem[] industrySearchItems, string industryText, int industryId)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ConflictCheck

        /// <summary>
        /// Performs conflict check on fields for adding contact.
        /// </summary>
        /// <returns></returns>
        private bool ConflictCheck()
        {
            bool canSave = false;
            Person person = null;
            Address address = null;
            Organisation organisation = null;

            try
            {
                if (_ddlServiceType.SelectedItem.Value == "General Contact")
                {
                    if (_ddlContactType.SelectedValue == Convert.ToString(ContactType.Individual))
                    {
                        person = new Person();
                        person.ForeName = _txtForename.Text;
                        person.Surname = _txtSurname.Text;
                        person.Title = _ddlTitle.Text;
                        _conflictCheck.ClientType = ClientType.Individual;
                    }
                    else if (_ddlContactType.SelectedValue == Convert.ToString(ContactType.Organisation))
                    {
                        organisation = new Organisation();
                        organisation.Name = _txtOrgName.Text;
                        _conflictCheck.ClientType = ClientType.Organisation;
                    }
                }
                else if (_ddlServiceType.SelectedItem.Value == "Service")
                {
                    person = new Person();
                    person.ForeName = _cdServiceContactDetails.ForeName;
                    person.Surname = _cdServiceContactDetails.SurName;
                    person.Title = _cdServiceContactDetails.Title;

                    // In case of "Service" client type as "Individual"
                    _conflictCheck.ClientType = ClientType.Individual;
                }
                else if (_ddlServiceType.SelectedItem.Value == "Service Contact")
                {
                    person = new Person();
                    person.ForeName = _cdContactDetails.ForeName;
                    person.Surname = _cdContactDetails.SurName;
                    person.Title = _cdContactDetails.Title;
                    _conflictCheck.ClientType = ClientType.Individual;
                }

                address = _addressDetails.Address;

                _conflictCheck.Person = person;
                _conflictCheck.Organisation = organisation;
                _conflictCheck.Address = address;
                _conflictCheck.AdditionalDetails = _aadContactInfo.AdditionalDetails;

                ConflictCheckStandardReturnValue returnValue = _conflictCheck.PerformConflictCheck();

                if (returnValue.Success)
                {
                    if (returnValue.IsConflict)
                    {
                        _tblConflictCheck.Style["display"] = "";
                        _conflictCheck.ReturnConflictCheck = returnValue;
                        _conflictCheck.BindConflictCheckGridView();
                    }

                    canSave = !returnValue.IsConflict;
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }

                // Sets the return value to get the conflict summary and note content while save.
                _conflictCheck.ReturnConflictCheck = returnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return canSave;
        }

        #endregion

        #region BindContactTitle
        public void BindContactTitle(DropDownList _ddlContactTitles)
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                TitleSearchCriteria titleCriteria = new TitleSearchCriteria();
                TitleSearchReturnValue titleReturnValue = contactService.TitleSearch(_logonSettings.LogonId, collectionRequest, titleCriteria);
                if (titleReturnValue.Title != null)
                {
                    _ddlContactTitles.DataSource = titleReturnValue.Title.Rows;
                    _ddlContactTitles.DataTextField = "TitleId";
                    _ddlContactTitles.DataValueField = "TitleId";
                    _ddlContactTitles.DataBind();
                }
                AddDefaultToDropDownList(_ddlContactTitles);
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

        #endregion


       
    }
}

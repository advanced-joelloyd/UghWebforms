using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebApp.UserControls;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Matter;

namespace IRIS.Law.WebApp.Pages.Contact
{
    public partial class AddAssociationForMatter : BasePage
    {
        LogonReturnValue _logonSettings = null;

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                _lblMessage.Text = string.Empty;

                if (!IsPostBack)
                {
                    BindFeeEarner();
                    BindAssociationRoles();
                    ShowHideAssociationSearch();
                }
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

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                RegisterClientScript();
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        protected void _btnWizardStartNextButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Get entended info only if the role has changed
                if (_hdnRefreshRoleExtInfo.Value == "true")
                {
                    GetRoleExtendedInfo();
                }
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

        protected void _btnWizardStepFinishButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Hide the  last step if there are no rows 
                if (_grdAdditionalAssociationInfo.Rows.Count == 0)
                {
                    _wizardAddAssociationsForMatter.WizardSteps.Remove(_wizardStepAdditionalAssociationInfo2);
                }
                //Perform conflict check
                bool save = ConflictCheck();
                //Save the contact if theres no conflict
                if (save)
                {
                    SaveContact();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                if (ex.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                    _lblMessage.Text = "Date is invalid";
                else
                    _lblMessage.Text = ex.Message;

                _lblMessage.CssClass = "errorMessage";
            }
        }

        protected void _ddlFeeEarner_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Display the selected fee earner if the default item is not selected
            if (_ddlFeeEarner.SelectedIndex > 0)
            {
                _hdnMemberId.Value = _ddlFeeEarner.SelectedValue;
                _hdnOrganisationId.Value = DataConstants.DummyGuid.ToString();
                _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
                _txtName.Text = _ddlFeeEarner.SelectedItem.Text;
            }
            else
            {
                //default item selected - clear selection
                _hdnMemberId.Value = DataConstants.DummyGuid.ToString();
                _hdnOrganisationId.Value = DataConstants.DummyGuid.ToString();
                _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
                _txtName.Text = string.Empty;
            }
        }

        protected void _contactSearch_ContactSelected(object sender, EventArgs e)
        {
            //display selected contact
            _ddlFeeEarner.SelectedIndex = -1;
            _hdnMemberId.Value = _contactSearch.MemberId.ToString();
            _hdnOrganisationId.Value = _contactSearch.OrganisationId.ToString();
            _txtName.Text = _contactSearch.ContactName;
            _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
        }

        protected void _serviceSearch_ServiceSelected(object sender, EventArgs e)
        {
            //display selected service
            _ddlFeeEarner.SelectedIndex = -1;
            _hdnMemberId.Value = DataConstants.DummyGuid.ToString();
            _hdnOrganisationId.Value = _serviceSearch.ServiceId.ToString();
            _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
            _txtName.Text = _serviceSearch.ServiceText;
        }

        protected void _serviceSearch_ServiceContactSelected(object sender, EventArgs e)
        {
            //display selected service contact
            _ddlFeeEarner.SelectedIndex = -1;
            _hdnMemberId.Value = _serviceSearch.ServiceContactId.ToString();
            _hdnOrganisationId.Value = DataConstants.DummyGuid.ToString();
            _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
            _txtName.Text = _serviceSearch.ServiceText;
        }

        protected void _clientSearch_ClientSelected(object sender, EventArgs e)
        {
            //If the user is supposed to select a matter then dont accept selected client
            if (_lblClientSearch.Text != "Matter")
            {
                if (_clientSearch.IsMember)
                {
                    _hdnMemberId.Value = _clientSearch.ClientId.ToString();
                    _hdnOrganisationId.Value = DataConstants.DummyGuid.ToString();
                }
                else
                {
                    _hdnMemberId.Value = DataConstants.DummyGuid.ToString();
                    _hdnOrganisationId.Value = _clientSearch.ClientId.ToString();
                }
                _ddlFeeEarner.SelectedIndex = -1;
                _hdnLinkedProjectId.Value = DataConstants.DummyGuid.ToString();
                _txtName.Text = _clientSearch.ClientName;
            }
        }

        protected void _clientSearch_MatterSelected(object sender, EventArgs e)
        {
            if (Convert.ToInt32(_ddlRole.SelectedValue) != 58)
            {
                if (_clientSearch.IsMember)
                {
                    _hdnMemberId.Value = _clientSearch.ClientId.ToString();
                    _hdnOrganisationId.Value = DataConstants.DummyGuid.ToString();
                }
                else
                {
                    _hdnMemberId.Value = DataConstants.DummyGuid.ToString();
                    _hdnOrganisationId.Value = _clientSearch.ClientId.ToString();
                }
            }
            _ddlFeeEarner.SelectedIndex = -1;
            _hdnLinkedProjectId.Value = _clientSearch.ProjectId.ToString();
            _txtName.Text = _clientSearch.ClientName + string.Format(" ({0}) ", _clientSearch.MatterReference.ToString());
        }

        protected void _grdAdditionalAssociationInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Create the controls based on the data type
                    RoleExtendedInfoSearchItem extendedInfo = (RoleExtendedInfoSearchItem)e.Row.DataItem;
                    string dataType = extendedInfo.DataType.Trim();

                    CalendarControl calendarControl = (CalendarControl)e.Row.Cells[1].FindControl("_cc");
                    TextBox textBox = (TextBox)e.Row.Cells[1].FindControl("_txt");
                    CheckBox checkBox = (CheckBox)e.Row.Cells[1].FindControl("_chk");
                    DropDownList dropDownList = (DropDownList)e.Row.Cells[1].FindControl("_ddl");

                    // Determine which control to display, based on data type and initialise data.
                    if (dataType == "Date")
                    {
                        textBox.Visible = false;
                        checkBox.Visible = false;
                        dropDownList.Visible = false;

                        if (!extendedInfo.UserCanEdit)
                        {
                            calendarControl.Enabled = false;
                        }
                    }
                    else if (dataType == "Text")
                    {
                        calendarControl.Visible = false;
                        checkBox.Visible = false;
                        dropDownList.Visible = false;

                        if (!extendedInfo.UserCanEdit)
                        {
                            textBox.ReadOnly = true;
                        }
                    }
                    else if (dataType == "Bool")
                    {
                        calendarControl.Visible = false;
                        textBox.Visible = false;
                        dropDownList.Visible = false;

                        if (!extendedInfo.UserCanEdit)
                        {
                            checkBox.Enabled = false;
                        }
                    }
                    else if (dataType == "Number")
                    {
                        calendarControl.Visible = false;
                        checkBox.Visible = false;
                        dropDownList.Visible = false;
                        if (extendedInfo.TypeName == "Percentage Residue")
                        {
                            textBox.Text = "0.00000";
                            ScriptManager.RegisterStartupScript(this, Page.GetType(), extendedInfo.DataType + e.Row.RowIndex,
                                                string.Format(" $(\"#{0}\").numeric(null,5);", textBox.ClientID), true);
                        }
                        else
                        {
                            textBox.Text = "0.00";
                            ScriptManager.RegisterStartupScript(this, Page.GetType(), extendedInfo.DataType + e.Row.RowIndex,
                                                string.Format(" $(\"#{0}\").numeric(null,2);", textBox.ClientID), true);
                        }

                        if (!extendedInfo.UserCanEdit && extendedInfo.TypeName != "Percentage Residue")
                        {
                            textBox.Enabled = false;
                        }
                        else
                        {
                            textBox.Enabled = true;
                        }
                    }
                    else if (dataType == "List")
                    {
                        calendarControl.Visible = false;
                        checkBox.Visible = false;
                        textBox.Visible = false;

                        int start = 0;

                        string sourceText = extendedInfo.SourceText.Trim();
                        //Add blank item
                        dropDownList.Items.Add(string.Empty);

                        //Get items from comma sep sourceText and add to drop down list
                        for (int i = 0; i < sourceText.Length; i++)
                        {
                            if (sourceText.Substring(i, 1) == ",")
                            {
                                dropDownList.Items.Add(sourceText.Substring(start, (i - start)));
                                start = i + 1;
                                if (i == sourceText.LastIndexOf(","))
                                {
                                    dropDownList.Items.Add(sourceText.Substring((start), (sourceText.Length - start)));
                                }
                            }
                            else
                            {
                                if (i == (sourceText.Length - 1) && sourceText.LastIndexOf(",") == -1)
                                {
                                    dropDownList.Items.Add(sourceText);
                                }
                            }
                        }

                        if (!extendedInfo.UserCanEdit)
                        {
                            dropDownList.Enabled = false;
                        }
                    }
                }
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

        protected void _btnConflickOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveContact();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                if (ex.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                    _lblMessage.Text = "Date is invalid";
                else
                    _lblMessage.Text = ex.Message;

                _lblMessage.CssClass = "errorMessage";
            }
        }

        protected void _btnWizardNavCancel_Click(object sender, EventArgs e)
        {
            // Resets the controls                        
            _txtName.Text = string.Empty;
            _ddlFeeEarner.SelectedIndex = 0;
            _ddlRole.SelectedIndex = 0;
            _hdnRefreshRoleExtInfo.Value = "true";
            ShowHideAssociationSearch();
            _wizardAddAssociationsForMatter.ActiveStepIndex = 0;
            _wizardAddAssociationsForMatter.Visible = true;
            _tblConflictCheck.Visible = false;
            _txtDescription.Text = string.Empty;
            _ccDateFrom.DateText = string.Empty;
            _ccDateTo.DateText = string.Empty;
            _txtReference.Text = string.Empty;
            _txtLetterHeading.Text = string.Empty;
            _txtCommenting.Text = string.Empty;
        }

        protected void _ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _ddlFeeEarner.SelectedIndex = -1;
                _txtName.Text = string.Empty;
                ShowHideAssociationSearch();
                _hdnRefreshRoleExtInfo.Value = "true";
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        protected void _cliMatDetails_MatterChanged(object sender, EventArgs e)
        {
            try
            {
                if (Session[SessionName.ProjectId] == null)
                {
                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblMessage.CssClass = "errorMessage";
                            _lblMessage.Text = _cliMatDetails.Message;
                            return;
                        }
                    }
                }
                else
                {
                    CheckForClientOnSpecialMatters();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessafe";
                _lblMessage.Text = ex.Message;
            }
        }

        protected void _btnServiceSearch_SearchButtonClick(object sender, EventArgs e)
        {
            try
            {
                int industryId = GetIndustryForAssociationRole(Convert.ToInt32(_ddlRole.SelectedValue));
                _serviceSearch.IndustryId = industryId;
                if (industryId <= 0)
                {
                    _serviceSearch.Reset();
                    _serviceSearch.IsIndustryDropdownEnabled = true;
                }
                else
                {
                    _serviceSearch.IsIndustryDropdownEnabled = false;
                }
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

        protected void Search_Error(object sender, ErrorEventArgs e)
        {
            _lblMessage.CssClass = "errorMessage";
            _lblMessage.Text = e.Message;
        }

        #endregion

        #region Private Methods

        private void SaveContact()
        {
            ContactServiceClient contactService = null;
            try
            {
                AssociationForMatter association = GetControlData();
                contactService = new ContactServiceClient();
                ReturnValue returnValue = contactService.AddAssociationForMatter(_logonSettings.LogonId, association);
                if (returnValue.Success)
                {
                    Response.Redirect("~/Pages/Matter/EditMatter.aspx");
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
                if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                    contactService.Close();
            }
        }

        /// <summary>
        /// Gets the control data.
        /// </summary>
        /// <returns></returns>
        private AssociationForMatter GetControlData()
        {
            try
            {
                AssociationForMatter association = new AssociationForMatter();
                association.ApplicationId = 1;//PMS
                association.ProjectId = (Guid)Session[SessionName.ProjectId];
                association.RoleId = Convert.ToInt32(_ddlRole.SelectedValue);
                association.MemberId = new Guid(_hdnMemberId.Value);
                association.OrganisationId = new Guid(_hdnOrganisationId.Value);
                association.Description = _txtDescription.Text.Trim();
                if (_ccDateFrom.DateText.Length > 0)
                {
                    association.DateFrom = Convert.ToDateTime(_ccDateFrom.DateText);
                }
                else
                {
                    association.DateFrom = DataConstants.BlankDate;
                }

                if (_ccDateTo.DateText.Length > 0)
                {
                    association.DateTo = Convert.ToDateTime(_ccDateTo.DateText);
                }
                else
                {
                    association.DateTo = DataConstants.BlankDate;
                }
                association.Reference = _txtReference.Text.Trim();
                association.LetterHead = _txtLetterHeading.Text.Trim();
                association.Comment = _txtCommenting.Text.Trim();
                association.LinkedProjectId = new Guid(_hdnLinkedProjectId.Value);

                //Get extended info from the grid
                List<RoleExtendedInfo> extendedInfo = new List<RoleExtendedInfo>();
                foreach (GridViewRow row in _grdAdditionalAssociationInfo.Rows)
                {
                    string text = string.Empty;
                    decimal number = decimal.Zero;
                    DateTime date = DataConstants.BlankDate;
                    bool hasValue = false;

                    string dataType = ((HiddenField)row.FindControl("_hdnDataType")).Value.ToString().Trim();
                    int typeId = Convert.ToInt32(((HiddenField)row.FindControl("_hdnTypeId")).Value);

                    TextBox notes = (TextBox)row.Cells[2].FindControl("_txtNotes");

                    switch (dataType)
                    {
                        case "Date":
                            CalendarControl calendarControl = (CalendarControl)row.Cells[1].FindControl("_cc");
                            string dateText = calendarControl.DateText.Trim();
                            if (dateText != string.Empty)
                            {
                                date = Convert.ToDateTime(dateText);
                                hasValue = true;
                            }
                            break;

                        case "Number":
                            TextBox numericTextBox = (TextBox)row.FindControl("_txt");
                            number = Convert.ToDecimal(numericTextBox.Text);
                            hasValue = (number != decimal.Zero);
                            break;

                        case "Text":
                            TextBox textBox = (TextBox)row.FindControl("_txt");
                            text = textBox.Text.Trim();
                            hasValue = (text != string.Empty);
                            break;

                        case "Bool":
                            CheckBox checkBox = (CheckBox)row.FindControl("_chk");
                            text = Convert.ToInt32(checkBox.Checked).ToString();
                            hasValue = (text != "0");
                            break;

                        case "List":
                            DropDownList dropDownList = (DropDownList)row.FindControl("_ddl");
                            text = dropDownList.SelectedValue.Trim();
                            hasValue = (text != string.Empty);
                            break;
                    }

                    //Add extended info item if a value was entered
                    if (notes.Text.Trim().Length > 0 || hasValue)
                    {
                        RoleExtendedInfo info = new RoleExtendedInfo();
                        info.TypeId = typeId;
                        info.Text = text;
                        info.Date = date;
                        info.Number = number;
                        info.Comment = notes.Text.Trim();
                        extendedInfo.Add(info);
                    }
                }

                if (extendedInfo.Count > 0)
                {
                    association.RoleExtendedInfoDetails = extendedInfo.ToArray();
                }

                return association;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the supplimentary details for the role
        /// </summary>
        private void GetRoleExtendedInfo()
        {
            ContactServiceClient contactService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                RoleExtendedInfoSearchCriteria criteria = new RoleExtendedInfoSearchCriteria();
                criteria.AssociationRoleId = Convert.ToInt32(_ddlRole.SelectedValue);

                contactService = new ContactServiceClient();
                RoleExtendedInfoReturnValue returnValue = contactService.RoleExtendedInfoSearch(_logonSettings.LogonId,
                                            criteria, collectionRequest);

                if (returnValue.Success)
                {
                    //Set to false so that the info is not fetched again from the service if the user
                    //navigates back and does not change the role
                    _hdnRefreshRoleExtInfo.Value = "false";

                    if (returnValue.RoleExtendedInfo.Rows.Length > 0)
                    {
                        _grdAdditionalAssociationInfo.DataSource = returnValue.RoleExtendedInfo.Rows;
                        _grdAdditionalAssociationInfo.DataBind();
                    }
                    else
                    {
                        _grdAdditionalAssociationInfo.DataSource = null;
                        _grdAdditionalAssociationInfo.DataBind();

                        _wizardAddAssociationsForMatter.WizardSteps.Remove(_wizardStepAdditionalAssociationInfo2);
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
        /// Conflicts the check.
        /// </summary>
        /// <returns></returns>
        private bool ConflictCheck()
        {
            bool canSave = false;
            try
            {
                if (_ddlRole.SelectedItem.Text.Trim() == "Otherside")
                {
                    Guid memId = new Guid(_hdnMemberId.Value);
                    Guid orgId = new Guid(_hdnOrganisationId.Value);

                    string conflictName = string.Empty;
                    Organisation organisation = null;
                    Person person = new Person();

                    if (memId != DataConstants.DummyGuid)
                    {
                        conflictName = GetPersonName(memId);
                    }
                    else
                    {
                        conflictName = GetOrganisationName(orgId);

                    }

                    person.ForeName = "";
                    person.Title = "";

                    _conflictCheck.Person = person;

                    organisation = new Organisation();
                    organisation.Name = conflictName;
                    _conflictCheck.ClientType = IRIS.Law.Services.Pms.Client.ClientType.Organisation;

                    Address address = new Address();
                    address.Line1 = GetAddress(memId, orgId);
                    address.Line3 = "";
                    address.PostCode = "";
                    _conflictCheck.Organisation = organisation;
                    _conflictCheck.Address = address;
                    ConflictCheckStandardReturnValue returnValue = _conflictCheck.PerformConflictCheck();
                    if (returnValue.Success)
                    {
                        //Display conflict check screen if conflict is found
                        if (returnValue.IsConflict)
                        {
                            _wizardAddAssociationsForMatter.Visible = false;
                            _tblConflictCheck.Visible = true;
                            _conflictCheck.ReturnConflictCheck = returnValue;
                            _conflictCheck.BindConflictCheckGridView();
                        }
                        canSave = !returnValue.IsConflict;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
                }
                else
                {
                    canSave = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return canSave;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="memId">The mem id.</param>
        /// <param name="orgId">The org id.</param>
        /// <returns></returns>
        private string GetAddress(Guid memId, Guid orgId)
        {
            string addressLine1 = string.Empty;
            ClientServiceClient clientService = null;

            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                AddressSearchCriteria criteria = new AddressSearchCriteria();
                criteria.MemberId = memId;
                criteria.OrganisationId = orgId;

                clientService = new ClientServiceClient();
                AddressSearchReturnValue returnValue = clientService.GetClientAddresses(_logonSettings.LogonId,
                                                                                       collectionRequest,
                                                                                       criteria);

                if (returnValue.Success)
                {
                    if (returnValue.Addresses.Rows.Length > 0)
                    {
                        addressLine1 = returnValue.Addresses.Rows[0].Line1;
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
                if (clientService != null)
                {
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
                        clientService.Close();
                }
            }

            return addressLine1;
        }

        /// <summary>
        /// Gets the name of the organisation to perform conflict check.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        private string GetOrganisationName(Guid clientId)
        {
            string name = string.Empty;
            ContactServiceClient contactService = null;

            try
            {
                contactService = new ContactServiceClient();
                OrganisationReturnValue returnValue = contactService.GetOrganisation(_logonSettings.LogonId, clientId);

                if (returnValue.Success)
                {
                    if (returnValue.Organisation != null)
                    {
                        name = returnValue.Organisation.Name;
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

            return name;
        }

        /// <summary>
        /// Gets the name of the person to perform the conflict check.
        /// </summary>
        /// <returns></returns>
        private string GetPersonName(Guid memberId)
        {
            string name = string.Empty;
            ContactServiceClient contactService = null;

            try
            {
                contactService = new ContactServiceClient();
                PersonReturnValue returnValue = contactService.GetPerson(_logonSettings.LogonId, memberId);

                if (returnValue.Success)
                {
                    if (returnValue.Person != null)
                    {
                        name = returnValue.Person.Surname;
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

            return name;
        }

        /// <summary>
        /// Dont allow to add associations for special matters.
        /// </summary>
        private void CheckForClientOnSpecialMatters()
        {
            if (Session[SessionName.ProjectId] != null && _ddlRole.SelectedIndex != -1)
            {
                MatterServiceClient matterService = new MatterServiceClient();

                try
                {
                    MatterTypeReturnValue returnValue =
                                                matterService.GetMatterTypeId(_logonSettings.LogonId, (Guid)Session[SessionName.ProjectId]);

                    if (returnValue.Success)
                    {
                        int roleId = Convert.ToInt32(_ddlRole.SelectedValue);
                        Button nextButton = (Button)_wizardAddAssociationsForMatter.FindControl("StartNavigationTemplateContainerID").FindControl("_btnWizardStartNextButton");
                        bool disabled = false;
                        if ((returnValue.MatterTypeId != 1) && (returnValue.MatterTypeId != 6) && (roleId <= 2))
                        {
                            disabled = true;
                        }

                        ScriptManager.RegisterStartupScript(this, Page.GetType(), "SpecialMatters",
                                        string.Format("$(\"#{0}\").attr(\"disabled\", {1});", nextButton.ClientID, disabled.ToString().ToLower()), true);
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
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                        matterService.Close();
                }
            }
        }

        /// <summary>
        /// Show/hide the search that can be performed based on the selected association role id
        /// </summary>
        private void ShowHideAssociationSearch()
        {
            if (_ddlRole.Items.Count > 0)
            {
                CheckForClientOnSpecialMatters();

                //Create a json object that will be used to show/hide the available searches.
                string jsonStr = "{";

                ContactServiceClient contactService = null;

                try
                {
                    CollectionRequest collectionRequest = new CollectionRequest();
                    AssociationRoleSearchCriteria associationRoleCriteria = new AssociationRoleSearchCriteria();
                    associationRoleCriteria.RoleId = Convert.ToInt32(_ddlRole.SelectedValue);
                    contactService = new ContactServiceClient();
                    AssociationRoleSearchReturnValue associationRoleReturnValue =
                                                contactService.AssociationRoleForRoleIdSearch(_logonSettings.LogonId,
                                                                                     collectionRequest,
                                                                                     associationRoleCriteria);
                    if (associationRoleReturnValue.Success)
                    {
                        if (associationRoleReturnValue.AssociationRole != null)
                        {
                            // Decide which rows to Show/Hide based on the selected role.
                            // Build a json object which will be used to show/hide the options using js
                            if (associationRoleReturnValue.AssociationRole.Rows.Length > 0)
                            {
                                // Client & Matter.
                                if ((associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchClient) &&
                                    (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchMatter))
                                {
                                    _lblClientSearch.Text = "Client/Matter";
                                    jsonStr += "\"ClientSearch\":\"true\"";
                                    _clientSearch.DisplayMattersForClientGridview = true;
                                }
                                // Client.
                                else if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchClient)
                                {
                                    _lblClientSearch.Text = "Client";
                                    jsonStr += "\"ClientSearch\":\"true\"";
                                    _clientSearch.DisplayMattersForClientGridview = false;
                                }
                                // Matter.
                                else if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchMatter)
                                {
                                    _lblClientSearch.Text = "Matter";
                                    jsonStr += "\"ClientSearch\":\"true\"";
                                    _clientSearch.DisplayMattersForClientGridview = true;
                                }
                                else
                                {
                                    jsonStr += "\"ClientSearch\":\"false\"";
                                }

                                // General Contact.
                                if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchGeneral)
                                {
                                    jsonStr += ",\"ContactSearch\":\"true\"";
                                }
                                else
                                {
                                    jsonStr += ",\"ContactSearch\":\"false\"";
                                }

                                // Service Contact.
                                if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchService)
                                {
                                    jsonStr += ",\"ServiceSearch\":\"true\"";
                                }
                                else
                                {
                                    jsonStr += ",\"ServiceSearch\":\"false\"";
                                }

                                // Search for Fee Earner
                                if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSearchFeeEarner)
                                {
                                    jsonStr += ",\"FeeEarnerSearch\":\"true\"";
                                }
                                else
                                {
                                    jsonStr += ",\"FeeEarnerSearch\":\"false\"";
                                }

                                jsonStr += "}";

                                _hdnSearchDisplay.Value = jsonStr;

                                // Determine whether the selected Association Role requires a specialised search.
                                if (associationRoleReturnValue.AssociationRole.Rows[0].AssociationRoleSpecialisedSearch)
                                {
                                    _hdnIsSpecialisedSearch.Value = "true";
                                }
                                else
                                {
                                    _hdnIsSpecialisedSearch.Value = "false";
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(associationRoleReturnValue.Message);
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
        /// Registers the client script that show/hides the searches based on the selected role.
        /// </summary>
        private void RegisterClientScript()
        {
            if (_hdnSearchDisplay.Value.Length > 0)
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "ShowHideSearch",
                                "ShowHideSearch(" + _hdnSearchDisplay.Value + ");", true);
            }
        }

        /// <summary>
        /// Binds fee earners
        /// </summary>
        private void BindFeeEarner()
        {
            EarnerServiceClient earnerService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                EarnerSearchCriteria criteria = new EarnerSearchCriteria();
                criteria.IncludeArchived = false;
                criteria.MultiOnly = false;

                earnerService = new EarnerServiceClient();
                EarnerSearchReturnValue returnValue = earnerService.EarnerSearch(_logonSettings.LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    _ddlFeeEarner.Items.Clear();
                    foreach (EarnerSearchItem feeEarner in returnValue.Earners.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(feeEarner.Title, feeEarner.Name, feeEarner.SurName);
                        item.Value = feeEarner.Id.ToString();
                        _ddlFeeEarner.Items.Add(item);
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
                AppFunctions.AddDefaultToDropDownList(_ddlFeeEarner);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }

        /// <summary>
        /// Binds association roles
        /// </summary>
        private void BindAssociationRoles()
        {
            ContactServiceClient contactService = new ContactServiceClient();

            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                AssociationRoleSearchCriteria associationRoleCriteria = new AssociationRoleSearchCriteria();
                //PMS
                associationRoleCriteria.ApplicationId = 1;

                AssociationRoleSearchReturnValue associationRoleReturnValue =
                                            contactService.AssociationRoleForApplicationSearch(_logonSettings.LogonId,
                                                                                 collectionRequest,
                                                                                 associationRoleCriteria);
                if (associationRoleReturnValue.Success)
                {
                    if (associationRoleReturnValue.AssociationRole != null)
                    {
                        //Sort based on AssociationRoleDescription
                        IEnumerable<AssociationRoleSearchItem> assocRolesSorted =
                            associationRoleReturnValue.AssociationRole.Rows.OrderBy(role => role.AssociationRoleDescription);

                        _ddlRole.DataSource = assocRolesSorted;
                        _ddlRole.DataTextField = "AssociationRoleDescription";
                        _ddlRole.DataValueField = "AssociationRoleID";
                        _ddlRole.DataBind();
                    }
                }
                else
                {
                    throw new Exception(associationRoleReturnValue.Message);
                }
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

        /// <summary>
        /// Gets the industry for the association role
        /// </summary>
        /// <param name="associationRoleId">The association role id.</param>
        private int GetIndustryForAssociationRole(int associationRoleId)
        {
            int industryId = -1;
            if (_hdnIsSpecialisedSearch.Value == "true")
            {
                ContactServiceClient contactService = null;
                try
                {
                    contactService = new ContactServiceClient();
                    IndustrySearchCriteria searchCriteria = new IndustrySearchCriteria();
                    searchCriteria.AssociationRoleId = associationRoleId;
                    IndustryForAssociationRoleReturnValue returnValue = contactService.GetIndustryForAssociationRole(_logonSettings.LogonId,
                                                                     searchCriteria);
                    if (returnValue.Success)
                    {
                        industryId = returnValue.IndustryId;
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
            return industryId;
        }

        #endregion
    }
}

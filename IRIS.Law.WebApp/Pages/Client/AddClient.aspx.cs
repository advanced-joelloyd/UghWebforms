using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.Services.Pms.Client;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.UserControls;
using System.Collections.Generic;

namespace IRIS.Law.WebApp.Pages.Client
{
    public partial class AddClient : BasePage
    {

        #region Properties

        private bool isMember;
        //private string firstClientConflictNoteContent = "";
        //private string secondClientConflictNoteContent = "";
        private Guid clientGuid = DataConstants.DummyGuid;
        private LogonReturnValue _logonSettings;

        public bool IsMember
        {
            get
            {
                return isMember;
            }
            set
            {
                isMember = value;
            }
        }

        public Guid ClientGuid
        {
            get
            {
                return clientGuid;
            }
            set
            {
                clientGuid = value;
            }
        }

        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                HideUnhideControlsOnClientType();
                if (!Page.IsPostBack)
                {
                    BindDropDownLists();
                    _ddlClientType.Attributes.Add("onchange", "javascript:HideUnhideControlsOnClientType('" + _ddlClientType.ClientID + "');");
                    LoadDefaultSettings();
                    _tblConflictCheck.Style["display"] = "none";
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Functions

        #region LoadDefaultSettings
        private void LoadDefaultSettings()
        {
            try
            {
                if (Session[SessionName.LogonSettings] != null)
                {
                    LogonReturnValue userSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                    _ddlPartner.SelectedIndex = -1;
                    for (int i = 0; i <= _ddlPartner.Items.Count - 1; i++)
                    {
                        if (_ddlPartner.Items[i].Value != string.Empty)
                        {
                            if (new Guid(_ddlPartner.Items[i].Value) == userSettings.UserDefaultPartner)
                            {
                                _ddlPartner.Items[i].Selected = true;
                            }
                        }
                    }

                    _ddlBranch.SelectedIndex = -1;
                    for (int i = 0; i <= _ddlBranch.Items.Count - 1; i++)
                    {
                        if (_ddlBranch.Items[i].Value != string.Empty)
                        {
                            if (new Guid(GetValueOnIndexFromArray(_ddlBranch.Items[i].Value.Trim(), 1)) == userSettings.UserDefaultBranch)
                            {
                                _ddlBranch.Items[i].Selected = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        #region HideUnhideControlsOnClientType
        /// <summary>
        /// Hide Unhide the controls when Client type is changed
        /// </summary>
        private void HideUnhideControlsOnClientType()
        {
            try
            {
                string clientType = string.Empty;
                clientType = _ddlClientType.SelectedItem.Value;

                _trSurname.Style["display"] = "none";
                _trForename.Style["display"] = "none";
                _trTitle.Style["display"] = "none";
                _trOrgName.Style["display"] = "none";
                _trMatter.Style["display"] = "none";

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
                    case "Multiple":
                        _trSurname.Style["display"] = "";
                        _trForename.Style["display"] = "";
                        _trTitle.Style["display"] = "";
                        _trMatter.Style["display"] = "";
                        _rfvSurname.Enabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ConflictCheck
        private ConflictCheckStandardReturnValue ConflictCheck(bool isSecondClient)
        {
            ConflictCheckStandardReturnValue dsConflictCheckFields;
            ClientServiceClient clientService = new ClientServiceClient();
            try
            {

                Person person = new Person();
                Address addresses = new Address();
                Organisation organisation = null;
                ClientType clientType;
                IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] addressElementsFirst;
                addressElementsFirst = new AdditionalAddressElement[10];

                if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Individual))
                {
                    clientType = ClientType.Individual;
                }
                else if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Organisation))
                {
                    clientType = ClientType.Organisation;
                    organisation = new Organisation();
                    organisation.Name = _txtOrgName.Text;
                }
                else
                {
                    clientType = ClientType.Individual;
                }

                if (!isSecondClient)
                {
                    person.Title = _ddlTitle.SelectedValue;
                    person.Surname = _txtSurname.Text;
                    person.ForeName = _txtForename.Text;

                    addresses = _addressDetails.Address;

                    #region Set First Person's Additional Address Information
                    for (int i = 0; i <= 9; i++)
                    {
                        addressElementsFirst[i] = new AdditionalAddressElement();
                        switch (i)
                        {
                            case 0:
                                addressElementsFirst[0].ElementText = _txtHomeTelephone.Text;
                                break;
                            case 1:
                                addressElementsFirst[1].ElementText = _txtWorkTel1.Text;
                                break;
                            case 2:
                                addressElementsFirst[2].ElementText = _txtWorkTel2.Text;
                                break;
                            case 3:
                                addressElementsFirst[3].ElementText = _txtMob1.Text;
                                break;
                            case 4:
                                addressElementsFirst[4].ElementText = _txtMob2.Text;
                                break;
                            case 5:
                                addressElementsFirst[5].ElementText = _txtFax.Text;
                                break;
                            case 6:
                                addressElementsFirst[6].ElementText = _txtHomeEmail.Text;
                                break;
                            case 7:
                                addressElementsFirst[7].ElementText = _txtWorkEmail.Text;
                                break;
                            case 8:
                                addressElementsFirst[8].ElementText = _txtURL.Text;
                                break;
                            case 9:
                                addressElementsFirst[9].ElementText = _txtDDI.Text;
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    person.Title = _ddlSecondTitle.SelectedValue;
                    person.Surname = _txtSecondSurname.Text;
                    person.ForeName = _txtSecondForename.Text;

                    addresses = _addressDetailsSecond.Address;

                    #region Set Second Person's Additional Address Information
                    for (int i = 0; i <= 9; i++)
                    {
                        addressElementsFirst[i] = new AdditionalAddressElement();
                        switch (i)
                        {
                            case 0:
                                addressElementsFirst[0].ElementText = _txtSecondHomeTel.Text;
                                break;
                            case 1:
                                addressElementsFirst[1].ElementText = _txtSecondWorkTel1.Text;
                                break;
                            case 2:
                                addressElementsFirst[2].ElementText = _txtSecondWorkTel2.Text;
                                break;
                            case 3:
                                addressElementsFirst[3].ElementText = _txtSecondMob1.Text;
                                break;
                            case 4:
                                addressElementsFirst[4].ElementText = _txtSecondMob2.Text;
                                break;
                            case 5:
                                addressElementsFirst[5].ElementText = _txtSecondFax.Text;
                                break;
                            case 6:
                                addressElementsFirst[6].ElementText = _txtSecondHomeEmail.Text;
                                break;
                            case 7:
                                addressElementsFirst[7].ElementText = _txtSecondWorkEmail.Text;
                                break;
                            case 8:
                                addressElementsFirst[8].ElementText = _txtSecondURL.Text;
                                break;
                            case 9:
                                addressElementsFirst[9].ElementText = _txtSecondDDI.Text;
                                break;
                        }
                    }
                    #endregion
                }

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                dsConflictCheckFields = clientService.ConflictCheck(_logonSettings.LogonId,
                    collectionRequest,
                    clientType,
                    person,
                    organisation,
                    addresses,
                    addressElementsFirst,
                    _logonSettings.ConflictCheckRoles
                    );

                return dsConflictCheckFields;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
                    clientService.Close();
            }
        }
        #endregion

        #region SaveClient
        /// <summary>
        /// Saves the client.
        /// </summary>
        private void SaveClient()
        {
            string errorMessage = string.Empty;
            ClientServiceClient clientService = null;
            ClientServiceClient secondClientService = null;
            ClientReturnValue clientFirstReturnValue = null;
            ClientReturnValue clientSecondReturnValue = null;
            IRIS.Law.WebServiceInterfaces.Client.Client clientFirst;
            IRIS.Law.WebServiceInterfaces.Contact.Organisation organisationFirst;
            IRIS.Law.WebServiceInterfaces.Contact.Address addressFirst;
            IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] addressElementsFirst = null;
            IRIS.Law.WebServiceInterfaces.Contact.Address[] addressesSecond = null;
            IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] addressElementsSecond = null;
            IRIS.Law.WebServiceInterfaces.Client.Client clientSecond = null;
            IRIS.Law.WebServiceInterfaces.Contact.Address[] addressesFirst = new Address[1];

            bool isSecondClient;
            Person personFirst;
            Person personSecond = null;
            isSecondClient = false;

            personFirst = new Person();
            addressFirst = new Address();
            organisationFirst = new Organisation();
            addressElementsFirst = new AdditionalAddressElement[10];
            clientFirst = new IRIS.Law.WebServiceInterfaces.Client.Client();

            if (ViewState["FirstClientConflictNoteContent"] != null || ViewState["FirstClientConflictNoteSummary"] != null)
            {
                clientFirst.ConflictNoteSummary = ViewState["FirstClientConflictNoteSummary"].ToString();
                clientFirst.ConflictNoteContent = ViewState["FirstClientConflictNoteContent"].ToString();
            }
            clientFirst.Branch = GetValueOnIndexFromArray(_ddlBranch.SelectedValue.Trim(), 0);
            clientFirst.PartnerId = new Guid(_ddlPartner.SelectedValue);

            personFirst.Title = _ddlTitle.SelectedValue;
            personFirst.Surname = _txtSurname.Text;
            personFirst.ForeName = _txtForename.Text;

            addressesFirst[0] = _addressDetails.Address;

            //Set Address as Main
            addressesFirst[0].TypeId = 1;

            #region Set First Person's Additional Address Information
            for (int i = 0; i <= 9; i++)
            {
                addressElementsFirst[i] = new AdditionalAddressElement();
                switch (i)
                {
                    case 0:
                        addressElementsFirst[0].ElementText = _txtHomeTelephone.Text;
                        break;
                    case 1:
                        addressElementsFirst[1].ElementText = _txtWorkTel1.Text;
                        break;
                    case 2:
                        addressElementsFirst[2].ElementText = _txtWorkTel2.Text;
                        break;
                    case 3:
                        addressElementsFirst[3].ElementText = _txtMob1.Text;
                        break;
                    case 4:
                        addressElementsFirst[4].ElementText = _txtMob2.Text;
                        break;
                    case 5:
                        addressElementsFirst[5].ElementText = _txtFax.Text;
                        break;
                    case 6:
                        addressElementsFirst[6].ElementText = _txtHomeEmail.Text;
                        break;
                    case 7:
                        addressElementsFirst[7].ElementText = _txtWorkEmail.Text;
                        break;
                    case 8:
                        addressElementsFirst[8].ElementText = _txtURL.Text;
                        break;
                    case 9:
                        addressElementsFirst[9].ElementText = _txtDDI.Text;
                        break;
                }
            }
            #endregion

            // If Client Type selected is Individual/Organisation
            if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Individual))
            {
                clientFirst.Type = ClientType.Individual;
                clientSecond = null;
                isMember = true;
                clientFirst.AssociationId = DataConstants.DummyGuid;
            }
            else if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Organisation))
            {
                clientFirst.Type = ClientType.Organisation;
                organisationFirst.Name = _txtOrgName.Text;
                clientSecond = null;
                isMember = false;
            }
            else
            {
                clientFirst.Type = ClientType.Individual;
                isMember = true;
                clientFirst.AssociationId = DataConstants.DummyGuid;

                clientSecond = new IRIS.Law.WebServiceInterfaces.Client.Client();
                clientSecond.Type = ClientType.Individual;
                personSecond = new Person();
                personSecond.Title = _ddlSecondTitle.SelectedValue;
                personSecond.Surname = _txtSecondSurname.Text;
                personSecond.ForeName = _txtSecondForename.Text;

                clientSecond.AssociationRoleId = Convert.ToInt32(_ddlSecondRelationship.SelectedValue);

                addressesSecond = new Address[1];
                addressesSecond[0] = _addressDetailsSecond.Address;

                //Set Address as Main
                addressesSecond[0].TypeId = 1;

                addressElementsSecond = new AdditionalAddressElement[10];

                #region Set Second Person's Additional Address Information
                for (int i = 0; i <= 9; i++)
                {
                    addressElementsSecond[i] = new AdditionalAddressElement();
                    switch (i)
                    {
                        case 0:
                            addressElementsSecond[0].ElementText = _txtSecondHomeTel.Text;
                            break;
                        case 1:
                            addressElementsSecond[1].ElementText = _txtSecondWorkTel1.Text;
                            break;
                        case 2:
                            addressElementsSecond[2].ElementText = _txtSecondWorkTel2.Text;
                            break;
                        case 3:
                            addressElementsSecond[3].ElementText = _txtSecondMob1.Text;
                            break;
                        case 4:
                            addressElementsSecond[4].ElementText = _txtSecondMob2.Text;
                            break;
                        case 5:
                            addressElementsSecond[5].ElementText = _txtSecondFax.Text;
                            break;
                        case 6:
                            addressElementsSecond[6].ElementText = _txtSecondHomeEmail.Text;
                            break;
                        case 7:
                            addressElementsSecond[7].ElementText = _txtSecondWorkEmail.Text;
                            break;
                        case 8:
                            addressElementsSecond[8].ElementText = _txtSecondURL.Text;
                            break;
                        case 9:
                            addressElementsSecond[9].ElementText = _txtSecondDDI.Text;
                            break;
                    }
                }
                #endregion
            }

            // If Client Type selected is Multiple
            if (_ddlClientType.SelectedValue == "Multiple")
            {
                isSecondClient = true;
            }

            if (clientFirst != null)
            {
                clientFirst.NetPassword = AppFunctions.GeneratePassword(10);

                try
                {
                    clientService = new ClientServiceClient();
                    clientFirstReturnValue = clientService.AddClient(_logonSettings.LogonId, clientFirst, personFirst, organisationFirst, addressesFirst, addressElementsFirst);

                    Session["isNewClient"] = true;
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
            }

            if (clientFirstReturnValue.Success)
            {
                if (clientFirstReturnValue == null)
                {
                    _lblError.Text = "Add new client aborted. <br>No valid client string";
                    return;
                }
                if (clientFirstReturnValue.Client == null)
                {
                    _lblError.Text = "Add new client aborted. <br>No valid client string";
                    return;
                }

                if (_ddlClientType.SelectedValue != Convert.ToString(ClientType.Organisation))
                {
                    if (!clientFirstReturnValue.Client.IsMember)
                    {
                        _lblError.Text = "Add new client aborted. <br>No valid client string";
                        return;
                    }
                }

                Session[SessionName.MemberId] = clientFirstReturnValue.Client.MemberId;
                Session[SessionName.OrganisationId] = clientFirstReturnValue.Client.OrganisationId;
                Session[SessionName.ProjectId] = null;
                Session[SessionName.MatterDesc] = null;
                Session[SessionName.ClientRef] = null;

                // If ClientType selected is Multiple, Add Second Client
                if (clientSecond != null && isSecondClient == true)
                {
                    clientSecond.NetPassword = AppFunctions.GeneratePassword(10);

                    // Never Org ID 
                    clientSecond.AssociationId = clientFirstReturnValue.Client.MemberId;

                    // Its the second client the branch and partner is same as first, 
                    // so copy the branch and partner from first
                    clientSecond.Branch = clientFirst.Branch;
                    clientSecond.PartnerId = clientFirst.PartnerId;

                    if (ViewState["SecondClientConflictNoteContent"] != null || ViewState["SecondClientConflictNoteSummary"] != null)
                    {
                        clientSecond.ConflictNoteSummary = ViewState["SecondClientConflictNoteSummary"].ToString();
                        clientSecond.ConflictNoteContent = ViewState["SecondClientConflictNoteContent"].ToString();
                    }

                    try
                    {
                        secondClientService = new ClientServiceClient();

                        clientSecondReturnValue = secondClientService.AddClient(_logonSettings.LogonId, clientSecond, personSecond, organisationFirst, addressesSecond, addressElementsSecond);

                        if (!clientSecondReturnValue.Success)
                        {
                            _lblError.Text = clientSecondReturnValue.Message;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (secondClientService != null)
                        {
                            if (secondClientService.State != System.ServiceModel.CommunicationState.Faulted)
                                secondClientService.Close();
                        }
                    }
                }
            }
            else
            {
                _lblError.Text = clientFirstReturnValue.Message;
                return;
            }

            if (clientFirstReturnValue.Client != null)
            {
                Response.Redirect("~/Pages/Client/EditClient.aspx", true);
            }

            _lblError.Text = "Problem Occured while saving Client.";
        }
        #endregion

        #region GetValueOnIndexFromArray
        /// <summary>
        /// If string is Branch Value
        /// index = 0 -> BranchRef
        /// index = 1 -> OrgId
        /// </summary>
        /// <param name="branchValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strValue, int index)
        {
            try
            {
                string[] arrayBranch = strValue.Split('$');
                return arrayBranch[index].Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        #region BindDropDownLists

        private void BindSecondPersonDropDownList()
        {
            ContactServiceClient contactService = new ContactServiceClient();

            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                TitleSearchCriteria titleCriteria = new TitleSearchCriteria();
                TitleSearchReturnValue titleReturnValue = contactService.TitleSearch(_logonSettings.LogonId, collectionRequest, titleCriteria);

                if (titleReturnValue.Title != null)
                {
                    _ddlSecondTitle.DataSource = titleReturnValue.Title.Rows;
                    _ddlSecondTitle.DataTextField = "TitleId";
                    _ddlSecondTitle.DataValueField = "TitleId";
                    _ddlSecondTitle.DataBind();
                }
                AddDefaultToDropDownList(_ddlSecondTitle);


                AssociationRoleSearchCriteria associationRoleCriteria = new AssociationRoleSearchCriteria();
                associationRoleCriteria.IncludeArchived = true;
                AssociationRoleSearchReturnValue associationRoleReturnValue = contactService.AssociationRoleSearch(_logonSettings.LogonId, collectionRequest, associationRoleCriteria);
                if (associationRoleReturnValue.AssociationRole != null)
                {
                    _ddlSecondRelationship.DataSource = associationRoleReturnValue.AssociationRole.Rows;
                    _ddlSecondRelationship.DataTextField = "AssociationRoleDescription";
                    _ddlSecondRelationship.DataValueField = "AssociationRoleID";
                    _ddlSecondRelationship.DataBind();
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
        /// Fill all dropdownlists in wizard
        /// </summary>
        private void BindDropDownLists()
        {
            ContactServiceClient contactService = new ContactServiceClient();

            EarnerServiceClient earnerService = new EarnerServiceClient();
            BranchDeptServiceClient branchClient = new BranchDeptServiceClient();
            try
            {
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
                AddDefaultToDropDownList(_ddlTitle);

                PartnerSearchCriteria partnerCriteria = new PartnerSearchCriteria();
                PartnerSearchReturnValue partnerReturnValue = earnerService.PartnerSearch(_logonSettings.LogonId, collectionRequest, partnerCriteria);

                if (partnerReturnValue.Partners != null)
                {
                    for (int i = 0; i < partnerReturnValue.Partners.Rows.Length - 1; i++)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(partnerReturnValue.Partners.Rows[i].PersonTitle, partnerReturnValue.Partners.Rows[i].Name, partnerReturnValue.Partners.Rows[i].Surname);
                        item.Value = partnerReturnValue.Partners.Rows[i].PartnerId.ToString();
                        _ddlPartner.Items.Add(item);
                    }
                }
                AddDefaultToDropDownList(_ddlPartner);

                BranchSearchReturnValue branchReturnValue = branchClient.BranchSearch(_logonSettings.LogonId, collectionRequest);
                if (branchReturnValue.Branches != null)
                {
                    for (int i = 0; i < branchReturnValue.Branches.Rows.Length; i++)
                    {
                        ListItem item = new ListItem();
                        item.Text = branchReturnValue.Branches.Rows[i].Name.ToString();
                        item.Value = branchReturnValue.Branches.Rows[i].Reference.ToString() + "$" + branchReturnValue.Branches.Rows[i].OrganisationId.ToString();
                        _ddlBranch.Items.Add(item);
                    }
                }
                AddDefaultToDropDownList(_ddlBranch);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    contactService.Close();
                    earnerService.Close();
                    branchClient.Close();
                }
            }
        }

        #endregion

        #region Checkbox Matter
        protected void _chkMatter_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chkMatter.Checked)
                {
                    _chkSecondMatter.Checked = false;
                }

                if (!_chkMatter.Checked && !_chkSecondMatter.Checked)
                {
                    _chkMatter.Checked = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void _chkSecondMatter_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chkSecondMatter.Checked)
                {
                    _chkMatter.Checked = false;
                }

                if (!_chkMatter.Checked && !_chkSecondMatter.Checked)
                {
                    _chkMatter.Checked = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            if (_chkUseSameAddress.Checked)
            {
                _addressDetailsSecond.Address = _addressDetails.Address;
                _addressDetailsSecond.DataBind();
            }
            else
            {
                _addressDetailsSecond.ClearFields();
            }
        }
        #endregion

        #region Use Same Contact Info
        /// <summary>
        /// If Same as first Contact Info checkbox is checked, 
        /// all additional contact info will get copied to the Second Person's additional contact info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _chkSameContactInfo_Click(object sender, EventArgs e)
        {
            if (_chkSameContactInfo.Checked)
            {
                //Copy Additional Contact Details
                _txtSecondHomeTel.Text = _txtHomeTelephone.Text;
                _txtSecondWorkTel1.Text = _txtWorkTel1.Text;
                _txtSecondWorkTel2.Text = _txtWorkTel2.Text;
                _txtSecondDDI.Text = _txtDDI.Text;
                _txtSecondMob1.Text = _txtMob1.Text;
                _txtSecondMob2.Text = _txtMob2.Text;
                _txtSecondFax.Text = _txtFax.Text;
                _txtSecondHomeEmail.Text = _txtHomeEmail.Text;
                _txtSecondWorkEmail.Text = _txtWorkEmail.Text;
                _txtSecondURL.Text = _txtURL.Text;
            }
            else
            {
                //Blank Additional Contact Details
                _txtSecondHomeTel.Text = "";
                _txtSecondWorkTel1.Text = "";
                _txtSecondWorkTel2.Text = "";
                _txtSecondDDI.Text = "";
                _txtSecondMob1.Text = "";
                _txtSecondMob2.Text = "";
                _txtSecondFax.Text = "";
                _txtSecondHomeEmail.Text = "";
                _txtSecondWorkEmail.Text = "";
                _txtSecondURL.Text = "";
            }
        }
        #endregion

        #region Wizard Events

        /// <summary>
        /// Cancel button event in Wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnWizardStartNavCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _ddlClientType.SelectedItem.Selected = false;
                HideUnhideControlsOnClientType();
                _wizardAddClient.ActiveStepIndex = 0;
                _txtOrgName.Text = "";
                _txtSurname.Text = "";
                _txtForename.Text = "";
                _ddlTitle.Text = "";
                _ddlPartner.Text = "";
                _ddlBranch.Text = "";

                _addressDetails.ClearFields();
                _addressDetailsSecond.ClearFields();

                _txtHomeTelephone.Text = "";
                _txtWorkTel1.Text = "";
                _txtWorkTel2.Text = "";
                _txtDDI.Text = "";
                _txtMob1.Text = "";
                _txtMob2.Text = "";
                _txtFax.Text = "";
                _txtHomeEmail.Text = "";
                _txtWorkEmail.Text = "";
                _txtURL.Text = "";

                _txtSecondHomeTel.Text = "";
                _txtSecondWorkTel1.Text = "";
                _txtSecondWorkTel2.Text = "";
                _txtSecondDDI.Text = "";
                _txtSecondMob1.Text = "";
                _txtSecondMob2.Text = "";
                _txtSecondFax.Text = "";
                _txtSecondHomeEmail.Text = "";
                _txtSecondWorkEmail.Text = "";
                _txtSecondURL.Text = "";
            }
            catch (Exception ex)
            {
                _lblError.Text = "Error Occured: " + ex.Message;
            }
        }        

        /// <summary>
        /// Next button event in wizard
        /// If Client type selected is Multiple, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepNextButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ddlClientType.SelectedItem.Value == Convert.ToString(ClientType.Organisation))
                {
                    // Hide few Additional Address Details
                    _trHomeTel.Style["display"] = "none";
                    _trHomeEmail.Style["display"] = "none";
                    _trDDI.Style["display"] = "none";
                }
                else
                {
                    // Hide few Additional Address Details
                    _trHomeTel.Style["display"] = "";
                    _trHomeEmail.Style["display"] = "";
                    _trDDI.Style["display"] = "";

                    if (_wizardAddClient.ActiveStepIndex == 2)
                    {
                        if (_ddlClientType.SelectedItem.Value != Convert.ToString(ClientType.Individual))
                        {
                            BindSecondPersonDropDownList();
                        }
                    }
                }

                if (_ddlClientType.SelectedItem.Value != "Multiple")
                {
                    if (_wizardAddClient.ActiveStepIndex == 1)
                    {
                        _wizardAddClient.WizardSteps.Remove(_wizardStepAddSecondPerson);
                        _wizardAddClient.WizardSteps.Remove(_wizardSecondAddressDetails);
                        _wizardAddClient.WizardSteps.Remove(_wizardStepSecondAdditionalContactInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = "Error Occured: " + ex.Message;
            }
        }

        /// <summary>
        /// Finish button event in wizard
        /// Conflict Check screen appears
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepFinishButton_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["FirstClientConflictNoteContent"] = null;
                ViewState["FirstClientConflictNoteSummary"] = null;
                ViewState["SecondClientConflictNoteContent"] = null;
                ViewState["SecondClientConflictNoteSummary"] = null;
                ViewState["IsSecondClient"] = null;
                if (_ddlClientType.SelectedItem.Value != "Multiple")
                {
                    _wizardAddClient.WizardSteps.Remove(_wizardStepAddSecondPerson);
                    _wizardAddClient.WizardSteps.Remove(_wizardSecondAddressDetails);
                    _wizardAddClient.WizardSteps.Remove(_wizardStepSecondAdditionalContactInfo);
                }

                _conflictCheck.IsSecondClient = false;
                SetConflictCheckProperties();
                ConflictCheckStandardReturnValue returnValue = _conflictCheck.PerformConflictCheck();

                if (!returnValue.IsConflict)
                {
                    _wizardAddClient.Style["display"] = "";
                    ViewState["FirstClientConflictNoteContent"] = "No matches found";
                    ViewState["FirstClientConflictNoteSummary"] = returnValue.Summary.ToString();

                    // Check for Second Client Conflict Check
                    if (_ddlClientType.SelectedValue == "Multiple")
                    {
                        if (!returnValue.IsConflict)
                        {
                            returnValue = ConflictCheck(true);
                            if (!returnValue.IsConflict)
                            {
                                _wizardAddClient.Style["display"] = "";
                                ViewState["SecondClientConflictNoteContent"] = "No matches found";
                                ViewState["SecondClientConflictNoteSummary"] = returnValue.Summary.ToString();
                            }
                            else
                            {
                                _tblConflictCheck.Style["display"] = "";
                                _wizardAddClient.Style["display"] = "none";
                                if (returnValue.Summary.Length > 0)
                                {
                                    ViewState["SecondClientConflictNoteSummary"] = returnValue.Summary.ToString();
                                }
                                // Create table for Second Client Conflict Check
                                _conflictCheck.IsSecondClient = true;
                                _conflictCheck.ReturnConflictCheck = returnValue;

                                // Binds grid view for conflicts
                                _conflictCheck.BindConflictCheckGridView();                               
                                ViewState["SecondClientConflictNoteContent"] = _conflictCheck.SecondClientConflictNoteContent;
                                return;
                            }
                        }
                    }

                    SaveClient();
                }
                else
                {
                    _tblConflictCheck.Style["display"] = "";
                    _wizardAddClient.Style["display"] = "none";
                    if (returnValue.Summary.Length > 0)
                    {                        
                        ViewState["FirstClientConflictNoteSummary"] = returnValue.Summary.ToString();
                    }
                    // Create table for First Client Conflict Check
                    _conflictCheck.IsSecondClient = false;
                    _conflictCheck.ReturnConflictCheck = returnValue;
                    _conflictCheck.BindConflictCheckGridView();
                    ViewState["FirstClientConflictNoteContent"] = _conflictCheck.FirstClientConflictNoteContent;

                    if (_ddlClientType.SelectedValue == "Multiple")
                    {
                        ViewState["IsSecondClient"] = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = "Error Occured: " + ex.Message;
            }
        }
        #endregion

        #region OK Button - Conflict Screen
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnConflickOK_Click(object sender, EventArgs e)
        {
            try
            {
                // If no conflict check for Second Client then Save Client, else do Conflict check for Second Client
                if (ViewState["IsSecondClient"] == null)
                {
                    SaveClient();
                }
                else
                {
                    _conflictCheck.IsSecondClient = true;
                    SetConflictCheckProperties();
                    ConflictCheckStandardReturnValue returnValue = _conflictCheck.PerformConflictCheck();

                    if (!returnValue.IsConflict)
                    {
                        _wizardAddClient.Style["display"] = "";
                        ViewState["SecondClientConflictNoteContent"] = "No matches found";
                        ViewState["SecondClientConflictNoteSummary"] = returnValue.Summary.ToString();

                        SaveClient();
                    }
                    else
                    {
                        _tblConflictCheck.Style["display"] = "";
                        _wizardAddClient.Style["display"] = "none";
                        if (returnValue.Summary.Length > 0)
                        {
                            ViewState["SecondClientConflictNoteSummary"] = returnValue.Summary.ToString();
                        }

                        _conflictCheck.ReturnConflictCheck = returnValue;
                        _conflictCheck.BindConflictCheckGridView();
                        ViewState["SecondClientConflictNoteContent"] = _conflictCheck.SecondClientConflictNoteContent;
                        ViewState["IsSecondClient"] = null;
                    }

                }
            }
            catch (Exception ex)
            {
                _lblError.Text = "Error Occured: " + ex.Message;
            }
        }
        #endregion

        #region Cancel Button - Conflict Screen
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnConflictCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _wizardAddClient.Style["display"] = "";
                _tblConflictCheck.Style["display"] = "none";
                _wizardAddClient.ActiveStepIndex = 0;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets properties on conflict check user control
        /// </summary>
        private void SetConflictCheckProperties()
        {
            Person person = null;
            Address address = null;
            Organisation organisation = null;

            if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Individual))
            {
                _conflictCheck.ClientType = ClientType.Individual;
            }
            else if (_ddlClientType.SelectedValue == Convert.ToString(ClientType.Organisation))
            {
                _conflictCheck.ClientType = ClientType.Organisation;
                organisation = new Organisation();
                organisation.Name = _txtOrgName.Text;
            }
            else
            {
                _conflictCheck.ClientType = ClientType.Individual;
            }

            if (!_conflictCheck.IsSecondClient)
            {
                person = new Person();
                person.Title = _ddlTitle.SelectedValue;
                person.Surname = _txtSurname.Text;
                person.ForeName = _txtForename.Text;

                address = _addressDetails.Address;
            }
            else
            {
                person = new Person();
                person.Title = _ddlSecondTitle.SelectedValue;
                person.Surname = _txtSecondSurname.Text;
                person.ForeName = _txtSecondForename.Text;

                address = _addressDetailsSecond.Address;
            }

            // Sets additional address details            
            AdditionalAddressElement[] additionalAddressDetails = new AdditionalAddressElement[10];
            if (!_conflictCheck.IsSecondClient)
            {
                #region Set First Person's Additional Address Information
                for (int i = 0; i <= 9; i++)
                {
                    additionalAddressDetails[i] = new AdditionalAddressElement();

                    switch (i)
                    {
                        case 0:
                            additionalAddressDetails[0].ElementText = _txtHomeTelephone.Text;
                            break;
                        case 1:
                            additionalAddressDetails[1].ElementText = _txtWorkTel1.Text;
                            break;
                        case 2:
                            additionalAddressDetails[2].ElementText = _txtWorkTel2.Text;
                            break;
                        case 3:
                            additionalAddressDetails[3].ElementText = _txtMob1.Text;
                            break;
                        case 4:
                            additionalAddressDetails[4].ElementText = _txtMob2.Text;
                            break;
                        case 5:
                            additionalAddressDetails[5].ElementText = _txtFax.Text;
                            break;
                        case 6:
                            additionalAddressDetails[6].ElementText = _txtHomeEmail.Text;
                            break;
                        case 7:
                            additionalAddressDetails[7].ElementText = _txtWorkEmail.Text;
                            break;
                        case 8:
                            additionalAddressDetails[8].ElementText = _txtURL.Text;
                            break;
                        case 9:
                            additionalAddressDetails[9].ElementText = _txtDDI.Text;
                            break;
                    }                    
                }
                #endregion
            }
            else
            {
                person.Title = _ddlSecondTitle.SelectedValue;
                person.Surname = _txtSecondSurname.Text;
                person.ForeName = _txtSecondForename.Text;

                address = _addressDetailsSecond.Address;

                #region Set Second Person's Additional Address Information
                for (int i = 0; i <= 9; i++)
                {
                    additionalAddressDetails[i] = new AdditionalAddressElement();

                    switch (i)
                    {
                        case 0:
                            additionalAddressDetails[0].ElementText = _txtSecondHomeTel.Text;
                            break;
                        case 1:
                            additionalAddressDetails[1].ElementText = _txtSecondWorkTel1.Text;
                            break;
                        case 2:
                            additionalAddressDetails[2].ElementText = _txtSecondWorkTel2.Text;
                            break;
                        case 3:
                            additionalAddressDetails[3].ElementText = _txtSecondMob1.Text;
                            break;
                        case 4:
                            additionalAddressDetails[4].ElementText = _txtSecondMob2.Text;
                            break;
                        case 5:
                            additionalAddressDetails[5].ElementText = _txtSecondFax.Text;
                            break;
                        case 6:
                            additionalAddressDetails[6].ElementText = _txtSecondHomeEmail.Text;
                            break;
                        case 7:
                            additionalAddressDetails[7].ElementText = _txtSecondWorkEmail.Text;
                            break;
                        case 8:
                            additionalAddressDetails[8].ElementText = _txtSecondURL.Text;
                            break;
                        case 9:
                            additionalAddressDetails[9].ElementText = _txtSecondDDI.Text;
                            break;
                    }
                }
                #endregion
            }

            _conflictCheck.Person = person;
            _conflictCheck.Organisation = organisation;
            _conflictCheck.Address = address;
            _conflictCheck.AdditionalDetails = additionalAddressDetails;
        }

        #endregion
    }
}   
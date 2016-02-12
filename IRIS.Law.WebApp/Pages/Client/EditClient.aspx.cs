using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.Services.Pms.Contact;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebApp.MasterPages;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Matter;
using System.Drawing;
using IRIS.Law.WebApp.UserControls;
using NLog;

namespace IRIS.Law.WebApp.Pages.Client
{
    public partial class EditClient : BasePage
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        ClientReturnValue _clientReturnValue;
        GeneralContactReturnValue _contactReturnValue;
        PersonReturnValue _personReturnValue;
        OrganisationReturnValue _organisationReturnValue;

        Guid _memberId;
        Guid _organisationId;
        LogonReturnValue _logonSettings;
        int _clientMatterRowCount;
        string _disabledTabList;
        #endregion

        #region Constants

        private const string UpdatedAddresses = "UpdatedAddresses";

        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString["PrintPage"] != null)
            {
                this.MasterPageFile = "~/MasterPages/Print.Master";

            }

        }


        protected void redirect(object sender, EventArgs e)
        {

        }


        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _btnPrint.Attributes.Add("onclick", "javascript:window.open('editclient.aspx?PrintPage=true&view=Client&mydetails=" + Request.QueryString["mydetails"] + "', 'Report', 'height=500,width=700,scrollbars=1,status=no,toolbar=no,menubar=no,location=no');return false;");

                //if logonId not present redirect to Login page
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                //User type Client not allowed to search for clients
                _hdnUserType.Value = _logonSettings.UserType.ToString();

                //Assign the logon settings value to Session in case user clicks MyDetails page
                if (Request.QueryString["mydetails"] == "true")
                {
                    if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        _memberId = _logonSettings.MemberId;
                        _organisationId = _logonSettings.OrganisationId;
                    }
                    else
                    {
                        Session[SessionName.MemberId] = _logonSettings.MemberId;
                        Session[SessionName.OrganisationId] = _logonSettings.OrganisationId;
                    }
                }

                if (Request.QueryString["mydetails"] == "true" && _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                {
                    // Do nothing
                }
                else
                {
                    if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                    {
                        _memberId = (Guid)Session[SessionName.MemberId];
                        _organisationId = (Guid)Session[SessionName.OrganisationId];
                        _pnlMatters.Visible = true;
                    }
                    else
                    {
                        //Client id is not present for editing the client
                        //redirect to search client page
                        Response.Redirect("SearchClient.aspx", true);
                    }
                }

                //Clear any prev messages
                _lblMessage.Text = string.Empty;

                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the client side changes to the value if we add this attribute in the markup
                _txtPassword.Attributes.Add("readonly", "readonly");
                _txtArmedForcesNo.Attributes.Add("readonly", "readonly");
                _txtUCN.Attributes.Add("readonly", "readonly");


                if (_logonSettings.UserType != (int)DataConstants.UserType.ThirdParty)
                {
                    //Set the page size for the grids
                    _grdClientMatters.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
                    //Populate the grid
                }

                if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                {
                    //Do Nothing
                }
                else
                {
                    _grdClientMatters.DataSourceID = _odsClientMatters.ID;
                }

                if (_logonSettings.UserType != (int)DataConstants.UserType.Staff)
                {
                    _divAddMatterButton.Visible = false;
                }

                if (!IsPostBack)
                {
                    //Used to keep track of the addresses that are updated
                    ViewState[UpdatedAddresses] = new List<string>();

                    SetControlAccessibility();
                    LoadClient();
                    //The client details should be read only for Clients and Third Party users
                    //unless viewing their own data
                    if (_logonSettings.UserType == (int)DataConstants.UserType.Client)
                    {
                        if (!(_logonSettings.MemberId == new Guid(Session[SessionName.MemberId].ToString()) &&
                        _logonSettings.OrganisationId == new Guid(Session[SessionName.OrganisationId].ToString())))
                        {
                            _btnSave.Enabled = false;
                            _divAddMatterButton.Visible = false;
                            GenerateReadOnlyTabList();
                            RegisterClientScript();
                        }

                    }

                    if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        if (!(Request.QueryString["mydetails"] == "true"))
                        {
                            _btnSave.Enabled = false;
                            _divAddMatterButton.Visible = false;
                            GenerateReadOnlyTabList();
                            RegisterClientScript();
                        }
                    }




                    //if we are coming from the search pg then display a back button
                    if (Request.UrlReferrer != null)
                    {
                        string pageName = Request.UrlReferrer.AbsolutePath;
                        pageName = AppFunctions.GetPageNameByUrl(pageName);
                        if (pageName == "SearchClient.aspx")
                        {
                            _divBack.Visible = true;
                        }
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                _logger.ErrorException("Endpoint not found", ex);
                _btnSave.Enabled = false;
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not open Edit Client page", ex);
                _btnSave.Enabled = false;
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        /// <summary>
        /// Registers the client script that disables tab controls based user type.
        /// </summary>
        private void RegisterClientScript()
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "DisableTabs",
                                "DisableTabs('" + _disabledTabList + "');", true);
        }

        #region ConstructUCN

        /// <summary>
        /// Constructs the UCN.
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="forename">The forename.</param>
        /// <param name="surname">The surname.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static string ConstructUCN(DateTime dateOfBirth, string forename, string surname)
        {
            ClientServiceClient serviceClient = null;
            string UCN = string.Empty;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    serviceClient = new ClientServiceClient();
                    ReturnValue returnValue = serviceClient.ConstructUCN(logonId, dateOfBirth, forename, surname);

                    if (returnValue.Success)
                    {
                        UCN = returnValue.Message;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
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
            return UCN;
        }

        #endregion

        #region Load Client

        /// <summary>
        /// Sets the visibility according to user type and permissions
        /// </summary>
        private void SetControlAccessibility()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];

            _pnlAddressDetails.Visible = objPerm[SessionName.AddressVisible];
            _pnlContactDetails.Visible = objPerm[SessionName.ContactVisible];
            _pnlGeneralDetails.Visible = objPerm[SessionName.GeneralVisible];
            _pnlIndividualDetails.Visible = objPerm[SessionName.IndividualVisible];
            _pnlOrganisationDetails.Visible = objPerm[SessionName.OrgVisible];
            _pnlMatters.Visible = objPerm[SessionName.ClientMatterVisible];
            //_btnSave.Enabled = !(objPerm[SessionName.AddressReadOnly] && objPerm[SessionName.ContactReadOnly] && objPerm[SessionName.GeneralReadOnly] && objPerm[SessionName.OrgReadOnly] && objPerm[SessionName.ClientMatterReadOnly]);
        }

        /// <summary>
        /// Generates a comma separated list of read only tabs
        /// </summary>
        private void GenerateReadOnlyTabList()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];

            if (objPerm[SessionName.AddressReadOnly] && _pnlAddressDetails.Visible)
                _disabledTabList += _pnlAddressDetails.ClientID + ",";

            if (objPerm[SessionName.ContactReadOnly] && _pnlContactDetails.Visible)
                _disabledTabList += _pnlContactDetails.ClientID + ",";

            if (objPerm[SessionName.GeneralReadOnly] && _pnlGeneralDetails.Visible)
                _disabledTabList += _pnlGeneralDetails.ClientID + ",";

            if (objPerm[SessionName.IndividualReadOnly] && _pnlIndividualDetails.Visible)
                _disabledTabList += _pnlIndividualDetails.ClientID + ",";

            if (objPerm[SessionName.OrgReadOnly] && _pnlOrganisationDetails.Visible)
                _disabledTabList += _pnlOrganisationDetails.ClientID + ",";

            if (objPerm[SessionName.ClientMatterReadOnly] && _pnlMatters.Visible)
                _disabledTabList += _pnlMatters.ClientID + ",";

            _disabledTabList = _disabledTabList.Substring(0, _disabledTabList.Length - 1);
        }

        /// <summary>
        /// Loads the client details.
        /// </summary>
        private void LoadClient()
        {
            ClientServiceClient serviceClient = null;
            try
            {

                if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                {
                    LoadContact();
                }
                else
                {
                    serviceClient = new ClientServiceClient();
                    _clientReturnValue = serviceClient.GetClient(_logonSettings.LogonId, _memberId, _organisationId);

                    if (_clientReturnValue.Success)
                    {
                        DisplayData();
                    }
                    else
                    {
                        throw new Exception(_clientReturnValue.Message);
                    }
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
        /// Loads the contact details.
        /// </summary>
        private void LoadContact()
        {
            ContactServiceClient serviceClient = null;

            try
            {
                serviceClient = new ContactServiceClient();
                //_contactReturnValue = serviceClient.GetGeneralContact(_logonSettings.LogonId, _memberId, _organisationId);
                if (_memberId != DataConstants.DummyGuid)
                {
                    _personReturnValue = serviceClient.GetPerson(_logonSettings.LogonId, _memberId);
                    if (_personReturnValue.Success)
                    {
                        DisplayContactData();
                    }
                    else
                    {
                        throw new Exception(_contactReturnValue.Message);
                    }
                }
                else if (_organisationId != DataConstants.DummyGuid)
                {
                    _organisationReturnValue = serviceClient.GetOrganisation(_logonSettings.LogonId, _organisationId);
                    if (_organisationReturnValue.Success)
                    {
                        DisplayContactData();
                    }
                    else
                    {
                        throw new Exception(_contactReturnValue.Message);
                    }
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
        /// Displays the client data.
        /// </summary>
        private void DisplayData()
        {
            try
            {

                if (_clientReturnValue.Client.MemberId != DataConstants.DummyGuid)
                {
                    _lblClientName.Text = CommonFunctions.MakeFullName(_clientReturnValue.Person.Title, _clientReturnValue.Person.ForeName, _clientReturnValue.Person.Surname);
                }
                else
                {
                    _lblClientName.Text = _clientReturnValue.Organisation.Name;
                }
                _imgClientArchieved.Visible = _clientReturnValue.Client.IsArchived;
                _txtClientReference.Text = _clientReturnValue.Client.Reference;

                if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                {
                    _txtClientReference.Text = string.Empty;
                }

                if (_logonSettings.UserType != (int)DataConstants.UserType.ThirdParty)
                {

                    Session[SessionName.ClientRef] = _clientReturnValue.Client.Reference;

                    IRIS.Law.WebApp.App_Code.AppFunctions.SetClientMatterDetailsInSession(_memberId, _organisationId, _lblClientName.Text, (Guid?)Session[SessionName.ProjectId], Convert.ToString(Session[SessionName.MatterDesc]));

                }

                if (Request.QueryString["PrintPage"] == null)
                {
                    ((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();
                }

                if (_clientReturnValue.Client.MemberId != DataConstants.DummyGuid)
                {
                    _pnlOrganisationDetails.Visible = false;
                    _txtHOUCN.Visible = true;
                    _lblHOUCN.Visible = true;
                    _lblUCN.Visible = true;
                    _txtUCN.Visible = true;

                    DisplayIndividualDetails();
                    _pnlIndividualDetails.Visible = true;
                    _pnlOrganisationDetails.Visible = false;
                }
                else
                {
                    _pnlIndividualDetails.Visible = false;
                    _txtHOUCN.Visible = false;
                    _lblHOUCN.Visible = false;
                    _lblUCN.Visible = false;
                    _txtUCN.Visible = false;

                    DisplayOrganisationDetails();
                    _pnlIndividualDetails.Visible = false;
                    _pnlOrganisationDetails.Visible = true;
                }
                _pnlMatters.Visible = true;

                if (_pnlAddressDetails.Visible)
                    DisplayAddressDetails();

                Session[SessionName.ContactDetails] = _clientReturnValue.AdditionalAddressElements;

                //if(_pnlGeneralDetails.Visible)
                DisplayGeneralDetails();
            }
            catch
            {
                throw new Exception("Error loading client details");
            }
        }

        /// <summary>
        /// Displays the contact data.
        /// </summary>
        private void DisplayContactData()
        {
            try
            {
                if (_memberId != DataConstants.DummyGuid)
                {
                    _lblClientName.Text = CommonFunctions.MakeFullName(_personReturnValue.Person.Title, _personReturnValue.Person.ForeName, _personReturnValue.Person.Surname);
                    _imgClientArchieved.Visible = false;
                    _pnlOrganisationDetails.Visible = false;
                    _txtHOUCN.Visible = true;
                    _lblHOUCN.Visible = true;
                    _lblUCN.Visible = true;
                    _txtUCN.Visible = true;

                    DisplayContactIndividualDetails();

                }
                else if (_organisationId != DataConstants.DummyGuid)
                {
                    _lblClientName.Text = _organisationReturnValue.Organisation.Name;
                    _imgClientArchieved.Visible = false;
                    _pnlIndividualDetails.Visible = false;
                    _txtHOUCN.Visible = false;
                    _lblHOUCN.Visible = false;
                    _lblUCN.Visible = false;
                    _txtUCN.Visible = false;

                    DisplayContactOrganisationDetails();
                }


                if (_pnlAddressDetails.Visible)
                    DisplayContactAddressDetails();
                _pnlMatters.Visible = false; // making Matter tab unvisible in My Details section
            }
            catch
            {
                throw new Exception("Error loading contact details");
            }
        }

        #endregion

        #region Address Details Tab

        /// <summary>
        /// Displays the address details for the selected address type.
        /// </summary>
        private void DisplayAddressDetails()
        {
            try
            {
                List<Address> clientAddresses = null;
                int clientMailingAddressId = 0;
                int clientBillingAddressId = 0;

                Address currentAddress = null;
                if (!IsPostBack)
                {
                    GetAddressTypes();
                    //Store the addresses in a list so that we can add items if necessary
                    clientAddresses = new List<Address>();
                    foreach (Address address in _clientReturnValue.Addresses)
                    {
                        clientAddresses.Add(address);
                        if (address.IsBillingAddress)
                        {
                            clientBillingAddressId = address.Id;
                        }
                        if (address.IsMailingAddress)
                        {
                            clientMailingAddressId = address.Id;
                        }
                    }
                    Session[SessionName.ClientAddresses] = clientAddresses;
                    Session[SessionName.ClientBillingAddressId] = clientBillingAddressId;
                    Session[SessionName.ClientMailingAddressId] = clientMailingAddressId;
       
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
                    List<string> updatedAddresses = (List<string>)ViewState[UpdatedAddresses];
                    _ucAddress.Address = currentAddress;
                    _ucAddress.IsDataChanged = updatedAddresses.Contains(currentAddress.TypeId.ToString());
                    _ucAddress.DataBind();
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
                searchCriteria.MemberId = _clientReturnValue.Client.MemberId;
                searchCriteria.OrganisationId = _clientReturnValue.Client.OrganisationId;

                contactService = new ContactServiceClient();
                AddressTypeReturnValue returnValue = contactService.GetAddressTypes(_logonSettings.LogonId, collectionRequest, searchCriteria);


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

        /// <summary>
        /// Displays the address details for the selected address type.
        /// </summary>
        private void DisplayContactAddressDetails()
        {
            try
            {
                AddressSearchReturnValue addressSearchReturnValue = new AddressSearchReturnValue();
                CollectionRequest collectionRequest = new CollectionRequest();
                AddressSearchCriteria searchCriteria = new AddressSearchCriteria();
                searchCriteria.MemberId = _memberId;
                searchCriteria.OrganisationId = _organisationId;

                ContactServiceClient serviceClient = new ContactServiceClient();
                addressSearchReturnValue = serviceClient.GetContactAddresses(_logonSettings.LogonId, collectionRequest, searchCriteria);

                List<Address> contactAddresses = null;
                Address currentAddress = null;
                if (!IsPostBack)
                {
                    GetContactAddressTypes();
                    //Store the addresses in a list so that we can add items if necessary
                    contactAddresses = new List<Address>();
                    foreach (Address address in addressSearchReturnValue.Addresses.Rows)
                    {
                        contactAddresses.Add(address);
                    }
                    Session[SessionName.ClientAddresses] = contactAddresses;
                }
                else
                {
                    contactAddresses = (List<Address>)Session[SessionName.ClientAddresses];
                }

                //Get the current selected address type and display the address
                foreach (Address address in contactAddresses)
                {
                    if (address.TypeId == 1)
                    {
                        Session[SessionName.ContactDetails] = address.AdditionalAddressElements;
                    }

                    if (address.TypeId == Convert.ToInt32(_ddlAddressType.SelectedValue))
                    {
                        currentAddress = address;
                        break;
                    }
                }
                //If an address exists then display it..
                if (currentAddress != null)
                {
                    List<string> updatedAddresses = (List<string>)ViewState[UpdatedAddresses];
                    _ucAddress.Address = currentAddress;
                    _ucAddress.IsDataChanged = updatedAddresses.Contains(currentAddress.TypeId.ToString());
                    _ucAddress.DataBind();
                }
                else
                {
                    //No address exists for the current type, so create a new one
                    currentAddress = new Address();
                    currentAddress.LastVerified = DataConstants.BlankDate;
                    currentAddress.TypeId = Convert.ToInt32(_ddlAddressType.SelectedValue);
                    contactAddresses.Add(currentAddress);
                    _ucAddress.Address = currentAddress;
                    _ucAddress.IsDataChanged = false;
                    _ucAddress.DataBind();
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
        private void GetContactAddressTypes()
        {
            ContactServiceClient contactService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                AddressTypeSearchCriteria searchCriteria = new AddressTypeSearchCriteria();
                searchCriteria.MemberId = _memberId;
                searchCriteria.OrganisationId = _organisationId;

                contactService = new ContactServiceClient();
                AddressTypeReturnValue returnValue = contactService.GetAddressTypes(_logonSettings.LogonId, collectionRequest, searchCriteria);


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
                CheckForModifiedAddress();
                if (Request.QueryString["mydetails"] == "true" && _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                {
                    DisplayContactAddressDetails();
                }
                else
                {
                    DisplayAddressDetails();
                }

                if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                       _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                {


                    if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                    {
                        //do nothing
                    }
                    else
                    {
                        if (!(_logonSettings.MemberId == new Guid(Session[SessionName.MemberId].ToString()) &&
                        _logonSettings.OrganisationId == new Guid(Session[SessionName.OrganisationId].ToString())))
                        {
                            _btnSave.Enabled = false;
                            _divAddMatterButton.Visible = false;
                            GenerateReadOnlyTabList();
                            RegisterClientScript();
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

        /// <summary>
        /// Checks if the address has been modified and updates the address in the session.
        /// </summary>
        private void CheckForModifiedAddress()
        {
            if (_ucAddress.IsDataChanged)
            {
                //Store the current address in the session before loading the new address
                List<Address> clientAddresses = (List<Address>)Session[SessionName.ClientAddresses];
                Address currentAddress = _ucAddress.Address;

                Address oldBillingAddress = new Address();
                Address oldMailingAddress = new Address();

                int oldBillingAddressId = (int)Session[SessionName.ClientBillingAddressId];
                int oldMailingAddressId = (int)Session[SessionName.ClientMailingAddressId];

                for (int i = 0; i < clientAddresses.Count; i++)
                {
                    if (clientAddresses[i].Id == oldBillingAddressId)
                    {
                        oldBillingAddress = clientAddresses[i];
                    }

                    if (clientAddresses[i].Id == oldMailingAddressId)
                    {
                        oldMailingAddress = clientAddresses[i];
                    }
                                       
                    if (clientAddresses[i].TypeId == currentAddress.TypeId)
                    {
                        int addressId = clientAddresses[i].Id;
                        clientAddresses[i] = currentAddress;
                        clientAddresses[i].Id = addressId;

                        if (clientAddresses[i].IsBillingAddress == true)
                        {
                            Session[SessionName.ClientBillingAddressId] = addressId;
                        }

                        if (clientAddresses[i].IsMailingAddress == true)
                        {
                            Session[SessionName.ClientMailingAddressId] = addressId;
                        }
           
                        //Add the address type id to the list of addresses that have been modified
                        //Only those addresses will be updated at the time of save
                        List<string> updatedAddresses = (List<string>)ViewState[UpdatedAddresses];
                        if (!updatedAddresses.Contains(currentAddress.TypeId.ToString()))
                        {
                            updatedAddresses.Add(currentAddress.TypeId.ToString());
                        }

                        // Check if we need to update the old Billing or Mailing Address too
                        if (oldBillingAddressId != (int)Session[SessionName.ClientBillingAddressId] && oldBillingAddressId != 0)
                        {
                            updatedAddresses.Add(oldBillingAddress.TypeId.ToString());
                        }

                        if (oldMailingAddressId != (int)Session[SessionName.ClientMailingAddressId] && oldMailingAddressId != 0)
                        {
                            updatedAddresses.Add(oldMailingAddress.TypeId.ToString());
                        }

                        break;
                    }
                }
                _ucAddress.IsDataChanged = false;
            }
        }

        #endregion

        #region General Details Tab

        /// <summary>
        /// Displays the client's general details.
        /// </summary>
        private void DisplayGeneralDetails()
        {
            GetPartners();
            GetBranches();
            GetBusinessSources();
            GetRatings();
            GetCampaigns();
            _ddlPartner.SelectedValue = _clientReturnValue.Client.PartnerId.ToString();
            _ddlBranch.SelectedValue = _clientReturnValue.Client.Branch;
            _ccOpenDate.DateText = _clientReturnValue.Client.OpenDate.ToString("dd/MM/yyyy");
            _txtPreviousReference.Text = _clientReturnValue.Client.PreviousReference;
            _ddlBusinessSource.SelectedValue = _clientReturnValue.Client.BusinessSourceId.ToString();
            _ddlSourceCampaign.SelectedValue = _clientReturnValue.Client.CampaignId.ToString();
            _ddlRating.SelectedValue = _clientReturnValue.Client.RatingId.ToString();
            _chkWebCaseTracking.Checked = _clientReturnValue.Client.IsWebCaseTracking;
            _txtPassword.Text = _clientReturnValue.Client.NetPassword;
            _txtGroup.Text = _clientReturnValue.Client.Group;
            _txtHOUCN.Text = _clientReturnValue.Client.HOUCN;
            _txtUCN.Text = _clientReturnValue.Client.UCN;
            _chkReceivesMarketing.Checked = _clientReturnValue.Client.IsReceivingMarketing;
            _chkArchiveClient.Checked = _clientReturnValue.Client.IsArchived;

            //Credit Control Information
            GetCashCollectionProcedures();
            _ddlCashCollection.SelectedValue = _clientReturnValue.Client.CashCollectionId.ToString();
            _txtCreditLimit.Text = _clientReturnValue.Client.TotalLockup.ToString("0.00");
        }

        /// <summary>
        /// Gets the campaigns.
        /// </summary>
        /// 
        #region AddDefaultToDropDownList
        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Not Set", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        private void GetCampaigns()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                CampaignSearchCriteria searchCriteria = new CampaignSearchCriteria();
                CampaignSearchReturnValue returnValue = contactService.CampaignSearch(_logonSettings.LogonId, collectionRequest,
                                                                                       searchCriteria);

                if (returnValue.Success)
                {
                    _ddlSourceCampaign.DataSource = returnValue.Campaigns.Rows;
                    _ddlSourceCampaign.DataTextField = "Description";
                    _ddlSourceCampaign.DataValueField = "CampaignId";
                    _ddlSourceCampaign.DataBind();
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

            AddDefaultToDropDownList(_ddlSourceCampaign);
        }

        /// <summary>
        /// Gets the cash collection procedures.
        /// </summary>
        private void GetCashCollectionProcedures()
        {
            MatterServiceClient matterService = null;
            try
            {
                matterService = new MatterServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                CashCollectionSearchCriteria searchCriteria = new CashCollectionSearchCriteria();
                searchCriteria.IncludeArchived = false;
                CashCollectionSearchReturnValue returnValue = matterService.CashCollectionSearch(_logonSettings.LogonId, collectionRequest,
                                                                    searchCriteria);

                if (returnValue.Success)
                {
                    _ddlCashCollection.DataSource = returnValue.CashCollection.Rows;
                    _ddlCashCollection.DataTextField = "Description";
                    _ddlCashCollection.DataValueField = "Id";
                    _ddlCashCollection.DataBind();
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
                if (matterService != null)
                {
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                        matterService.Close();
                }
            }

            AddDefaultToDropDownList(_ddlCashCollection);
        }

        /// <summary>
        /// Gets the ratings.
        /// </summary>
        private void GetRatings()
        {
            ClientServiceClient clientService = null;
            try
            {
                clientService = new ClientServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                RatingSearchCriteria searchCriteria = new RatingSearchCriteria();
                searchCriteria.IncludeArchived = false;
                RatingSearchReturnValue returnValue = clientService.RatingSearch(_logonSettings.LogonId, collectionRequest,
                                                                    searchCriteria);

                if (returnValue.Success)
                {
                    _ddlRating.DataSource = returnValue.Ratings.Rows;
                    _ddlRating.DataTextField = "Description";
                    _ddlRating.DataValueField = "Id";
                    _ddlRating.DataBind();
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
        }

        /// <summary>
        /// Gets the business sources.
        /// </summary>
        private void GetBusinessSources()
        {
            ContactServiceClient contactService = null;

            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                BusinessSourceSearchCriteria searchCriteria = new BusinessSourceSearchCriteria();
                BusinessSourceReturnValue returnValue = contactService.BusinessSourceSearch(_logonSettings.LogonId,
                                                        collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlBusinessSource.DataSource = returnValue.BusinessSources.Rows;
                    _ddlBusinessSource.DataTextField = "Description";
                    _ddlBusinessSource.DataValueField = "Id";
                    _ddlBusinessSource.DataBind();
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
        /// Gets the branches.
        /// </summary>
        private void GetBranches()
        {
            BranchDeptServiceClient branchDeptService = null;
            try
            {
                branchDeptService = new BranchDeptServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                BranchSearchReturnValue returnValue = branchDeptService.BranchSearch(_logonSettings.LogonId, collectionRequest);
                if (returnValue.Success)
                {
                    _ddlBranch.DataSource = returnValue.Branches.Rows;
                    _ddlBranch.DataTextField = "Name";
                    _ddlBranch.DataValueField = "Reference";
                    _ddlBranch.DataBind();
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
                if (branchDeptService != null)
                {
                    if (branchDeptService.State != System.ServiceModel.CommunicationState.Faulted)
                        branchDeptService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the partners.
        /// </summary>
        private void GetPartners()
        {
            EarnerServiceClient earnerService = null;
            try
            {
                earnerService = new EarnerServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                PartnerSearchCriteria searchCriteria = new PartnerSearchCriteria();
                PartnerSearchReturnValue returnValue = earnerService.PartnerSearch(_logonSettings.LogonId, collectionRequest
                                                                                    , searchCriteria);

                if (returnValue.Success)
                {
                    foreach (PartnerSearchItem partner in returnValue.Partners.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(partner.PersonTitle, partner.Name, partner.Surname);
                        item.Value = partner.PartnerId.ToString();
                        _ddlPartner.Items.Add(item);
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
                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the client details.
        /// </summary>
        /// <returns></returns>
        private IRIS.Law.WebServiceInterfaces.Client.Client GetClientDetails()
        {
            IRIS.Law.WebServiceInterfaces.Client.Client client = new IRIS.Law.WebServiceInterfaces.Client.Client();
            client.MemberId = _memberId;
            client.OrganisationId = _organisationId;
            if (_ddlPartner.SelectedValue != string.Empty)
            {
                client.PartnerId = new Guid(_ddlPartner.SelectedValue);
            }
            client.Branch = _ddlBranch.SelectedValue.Trim();
            if (_ccOpenDate.DateText != string.Empty)
            {
                client.OpenDate = Convert.ToDateTime(_ccOpenDate.DateText.Trim());
            }
            else
            {
                client.OpenDate = DataConstants.BlankDate;
            }
            client.PreviousReference = _txtPreviousReference.Text.Trim();
            if (_ddlBusinessSource.SelectedValue != string.Empty)
            {
                client.BusinessSourceId = Convert.ToInt32(_ddlBusinessSource.SelectedValue);
            }
            if (_ddlRating.SelectedValue != string.Empty)
            {
                client.RatingId = Convert.ToInt32(_ddlRating.SelectedValue);
            }
            client.IsWebCaseTracking = _chkWebCaseTracking.Checked;
            client.NetPassword = _txtPassword.Text;
            client.Group = _txtGroup.Text.Trim();
            client.HOUCN = _txtHOUCN.Text.Trim();
            client.UCN = _txtUCN.Text.Trim();
            client.IsReceivingMarketing = _chkReceivesMarketing.Checked;
            client.IsArchived = _chkArchiveClient.Checked;
            if (_ddlCashCollection.SelectedValue != string.Empty)
            {
                client.CashCollectionId = Convert.ToInt32(_ddlCashCollection.SelectedValue);
            }
            if (_txtCreditLimit.Text.Trim() != String.Empty)
            {
                client.TotalLockup = Convert.ToDecimal(_txtCreditLimit.Text);
            }
            if (_ddlSourceCampaign.SelectedValue.Trim() != string.Empty)
            {
                client.CampaignId = Convert.ToInt32(_ddlSourceCampaign.SelectedValue);
            }
            else
            {
                //Not set
                client.CampaignId = 0;
            }

            return client;
        }

        #endregion

        #region Organisation Details Tab

        /// <summary>
        /// Displays the organisation details.
        /// </summary>
        private void DisplayOrganisationDetails()
        {
            GetIndustries();
            GetSubTypes();
            _txtOrganisationName.Text = _clientReturnValue.Organisation.Name;
            _txtRegisteredName.Text = _clientReturnValue.Organisation.RegisteredName;
            _txtRegisteredNo.Text = _clientReturnValue.Organisation.RegisteredNo;
            _txtVATNo.Text = _clientReturnValue.Organisation.VATNo;
            _ddlIndustry.SelectedValue = _clientReturnValue.Organisation.IndustryId.ToString();
            _ddlSubType.SelectedValue = _clientReturnValue.Organisation.SubTypeId.ToString();
        }

        /// <summary>
        /// Displays the contact's organisation details.
        /// </summary>
        private void DisplayContactOrganisationDetails()
        {
            GetIndustries();
            GetSubTypes();
            _txtOrganisationName.Text = _organisationReturnValue.Organisation.Name;
            _txtRegisteredName.Text = _organisationReturnValue.Organisation.RegisteredName;
            _txtRegisteredNo.Text = _organisationReturnValue.Organisation.RegisteredNo;
            _txtVATNo.Text = _organisationReturnValue.Organisation.VATNo;
            _ddlIndustry.SelectedValue = _organisationReturnValue.Organisation.IndustryId.ToString();
            _ddlSubType.SelectedValue = _organisationReturnValue.Organisation.SubTypeId.ToString();
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
                                                            _logonSettings.LogonId, collectionRequest, searchCriteria);
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

        /// <summary>
        /// Gets the organisation details.
        /// </summary>
        /// <returns></returns>
        private Organisation GetOrganisationDetails()
        {
            Organisation organisation = new Organisation();
            organisation.OrganisationId = _organisationId;
            organisation.Name = _txtOrganisationName.Text.Trim();
            organisation.RegisteredName = _txtRegisteredName.Text.Trim();
            organisation.RegisteredNo = _txtRegisteredNo.Text.Trim();
            organisation.VATNo = _txtVATNo.Text.Trim();
            if (_ddlIndustry.SelectedValue != string.Empty)
            {
                organisation.IndustryId = Convert.ToInt32(_ddlIndustry.SelectedValue);
            }
            if (_ddlSubType.SelectedValue != string.Empty)
            {
                organisation.SubTypeId = Convert.ToInt32(_ddlSubType.SelectedValue);
            }
            return organisation;
        }
        #endregion

        #region Individual Details Tab

        /// <summary>
        /// Displays the individual details.
        /// </summary>
        private void DisplayIndividualDetails()
        {
            GetTitles();
            GetMaritalStatus();
            GetSex();
            GetEthnicity();
            GetDisability();
            _ddlTitle.SelectedValue = _clientReturnValue.Person.Title;
            _txtForenames.Text = _clientReturnValue.Person.ForeName;
            _txtSurname.Text = _clientReturnValue.Person.Surname;
            _ddlMaritalStatus.SelectedValue = _clientReturnValue.Person.MaritalStatusId.ToString();
            _txtPreviousName.Text = _clientReturnValue.Person.PreviousName;
            _txtOccupation.Text = _clientReturnValue.Person.Occupation;
            _ddlSex.SelectedValue = _clientReturnValue.Person.Sex.ToString();
            if (_clientReturnValue.Person.DOB != DataConstants.BlankDate)
            {
                _ccDOBDate.DateText = _clientReturnValue.Person.DOB.ToString("dd/MM/yyyy");
                //Contact blank date in the database was being stored as 01/01/1900 which is not the system blankdate
                //Therefore added a test to make sure that the date of death is later than the date of birth.
                //If it isn't, restore date of death to system blankdate as it is invalid.
                if ((_clientReturnValue.Person.DOD != DataConstants.BlankDate) && (_clientReturnValue.Person.DOD < _clientReturnValue.Person.DOB))
                {
                    _ccDODDate.DateText = DataConstants.BlankDate.ToString("dd/MM/yyyy");
                }
            }

            _txtAge.Text = CalculateAge(_clientReturnValue.Person.DOB, _clientReturnValue.Person.DOD);
            if (_clientReturnValue.Person.DOD != DataConstants.BlankDate)
            {
                _ccDODDate.DateText = _clientReturnValue.Person.DOD.ToString("dd/MM/yyyy");
            }
            _txtPlaceOfBirth.Text = _clientReturnValue.Person.PlaceOfBirth;
            _txtBirthName.Text = _clientReturnValue.Person.BirthName;
            _txtFormalSalutation.Text = _clientReturnValue.Person.SalutationLettterFormal;
            _txtInformalSalutation.Text = _clientReturnValue.Person.SalutationLettterInformal;
            _txtFriendlySalutation.Text = _clientReturnValue.Person.SalutationLettterFriendly;
            _txtEnvelopeSalutation.Text = _clientReturnValue.Person.SalutationEnvelope;

            if (Session["isNewClient"] != null)
            {
                if (_clientReturnValue.Person.SalutationLettterFormal == string.Empty)
                {
                    _txtFormalSalutation.Text = _clientReturnValue.Person.Title + " " + _clientReturnValue.Person.Surname;
                }

                if (_clientReturnValue.Person.SalutationLettterInformal == string.Empty)
                {
                    _txtInformalSalutation.Text = _clientReturnValue.Person.ForeName;
                }

                if (_clientReturnValue.Person.SalutationEnvelope == string.Empty)
                {
                    string personForename = "";

                    if (_clientReturnValue.Person.ForeName != string.Empty)
                    {
                        personForename = _clientReturnValue.Person.ForeName.Substring(0, 1);
                    }

                    _txtEnvelopeSalutation.Text = _clientReturnValue.Person.Title + " " + personForename + " " + _clientReturnValue.Person.Surname;
                }

                Session["isNewClient"] = null;
            }


            _ddlEthnicity.SelectedValue = _clientReturnValue.Person.EthnicityId.ToString();
            _ddlDisability.SelectedValue = _clientReturnValue.Person.DisabilityId.ToString();
            _chkArmedForces.Checked = _clientReturnValue.Person.IsInArmedForces;
            _txtArmedForcesNo.Text = _clientReturnValue.Person.ArmedForcesNo;
            _txtNINo.Text = _clientReturnValue.Person.NINo;
        }

        /// Displays the contact's individual details.
        /// </summary>
        private void DisplayContactIndividualDetails()
        {
            GetTitles();
            GetMaritalStatus();
            GetSex();
            GetEthnicity();
            GetDisability();
            _ddlTitle.SelectedValue = _personReturnValue.Person.Title;
            _txtForenames.Text = _personReturnValue.Person.ForeName;
            _txtSurname.Text = _personReturnValue.Person.Surname;
            _ddlMaritalStatus.SelectedValue = _personReturnValue.Person.MaritalStatusId.ToString();
            _txtPreviousName.Text = _personReturnValue.Person.PreviousName;
            _txtOccupation.Text = _personReturnValue.Person.Occupation;
            _ddlSex.SelectedValue = _personReturnValue.Person.Sex.ToString();
            if (_personReturnValue.Person.DOB != DataConstants.BlankDate)
            {
                _ccDOBDate.DateText = _personReturnValue.Person.DOB.ToString("dd/MM/yyyy");
                //Contact blank date in the database was being stored as 01/01/1900 which is not the system blankdate
                //Therefore added a test to make sure that the date of death is later than the date of birth.
                //If it isn't, restore date of death to system blankdate as it is invalid.
                if ((_personReturnValue.Person.DOD != DataConstants.BlankDate) && (_personReturnValue.Person.DOD < _personReturnValue.Person.DOB))
                {
                    _ccDODDate.DateText = DataConstants.BlankDate.ToString("dd/MM/yyyy");
                }
            }

            _txtAge.Text = CalculateAge(_personReturnValue.Person.DOB, _personReturnValue.Person.DOD);
            if (_personReturnValue.Person.DOD != DataConstants.BlankDate)
            {
                _ccDODDate.DateText = _personReturnValue.Person.DOD.ToString("dd/MM/yyyy");
            }
            _txtPlaceOfBirth.Text = _personReturnValue.Person.PlaceOfBirth;
            _txtBirthName.Text = _personReturnValue.Person.BirthName;
            _txtFormalSalutation.Text = _personReturnValue.Person.SalutationLettterFormal;
            _txtInformalSalutation.Text = _personReturnValue.Person.SalutationLettterInformal;
            _txtFriendlySalutation.Text = _personReturnValue.Person.SalutationLettterFriendly;
            _txtEnvelopeSalutation.Text = _personReturnValue.Person.SalutationEnvelope;
            _ddlEthnicity.SelectedValue = _personReturnValue.Person.EthnicityId.ToString();
            _ddlDisability.SelectedValue = _personReturnValue.Person.DisabilityId.ToString();
            _chkArmedForces.Checked = _personReturnValue.Person.IsInArmedForces;
            _txtArmedForcesNo.Text = _personReturnValue.Person.ArmedForcesNo;
            _txtNINo.Text = _personReturnValue.Person.NINo;
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
                DisabilitySearchReturnValue returnValue = contactService.DisabilitySearch(_logonSettings.LogonId,
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
                EthnicitySearchReturnValue returnValue = contactService.EthnicitySearch(_logonSettings.LogonId,
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
                SexSearchReturnValue returnValue = contactService.SexSearch(_logonSettings.LogonId,
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
                MaritalStatusSearchReturnValue returnValue = contactService.MaritalStatusSearch(_logonSettings.LogonId,
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
                TitleSearchReturnValue returnValue = contactService.TitleSearch(_logonSettings.LogonId, collectionRequest, searchCriteria);

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

        #region CalculateAge

        [WebMethod]
        public static string CalculateAge(DateTime dob, DateTime dod)
        {
            string clientAge = string.Empty;
            //Calculate the age from the dob
            if (dob != DataConstants.BlankDate)
            {
                if (dod != DataConstants.BlankDate)
                {
                    if (dod >= dob)
                    {
                        TimeSpan age = dod.Subtract(dob);
                        clientAge = Convert.ToString(Math.Floor(age.TotalDays / 365.25));
                    }
                }
                else
                {
                    TimeSpan age = DateTime.Today.Subtract(dob);
                    clientAge = Convert.ToString(Math.Floor(age.TotalDays / 365.25));
                }
            }

            return clientAge;
        }

        #endregion

        [WebMethod]
        public static string ReGeneratePassword()
        {
            return AppFunctions.GeneratePassword(10);
        }

        /// <summary>
        /// Gets the person details.
        /// </summary>
        /// <returns></returns>
        private Person GetPersonDetails()
        {
            Person person = new Person();
            person.MemberId = _memberId;
            person.Title = _ddlTitle.SelectedValue;
            person.ForeName = _txtForenames.Text.Trim();
            person.Surname = _txtSurname.Text.Trim();
            if (_ddlMaritalStatus.SelectedValue != string.Empty)
            {
                person.MaritalStatusId = Convert.ToInt32(_ddlMaritalStatus.SelectedValue);
            }
            person.PreviousName = _txtPreviousName.Text.Trim();
            person.Occupation = _txtOccupation.Text.Trim();
            if (_ddlSex.SelectedValue != string.Empty)
            {
                person.Sex = Convert.ToInt32(_ddlSex.SelectedValue);
            }

            if (_ccDOBDate.DateText != string.Empty)
            {
                person.DOB = Convert.ToDateTime(_ccDOBDate.DateText.Trim());
            }
            else
            {
                person.DOB = DataConstants.BlankDate;
            }

            if (_ccDODDate.DateText != string.Empty)
            {
                person.DOD = Convert.ToDateTime(_ccDODDate.DateText.Trim());
            }
            else
            {
                person.DOD = DataConstants.BlankDate;
            }
            person.PlaceOfBirth = _txtPlaceOfBirth.Text.Trim();
            person.BirthName = _txtBirthName.Text.Trim();
            person.SalutationLettterFormal = _txtFormalSalutation.Text.Trim();
            person.SalutationLettterInformal = _txtInformalSalutation.Text.Trim();
            person.SalutationLettterFriendly = _txtFriendlySalutation.Text.Trim();
            person.SalutationEnvelope = _txtEnvelopeSalutation.Text.Trim();

            if (_ddlEthnicity.SelectedValue != string.Empty)
            {
                person.EthnicityId = Convert.ToInt32(_ddlEthnicity.SelectedValue);
            }
            if (_ddlDisability.SelectedValue != string.Empty)
            {
                person.DisabilityId = Convert.ToInt32(_ddlDisability.SelectedValue);
            }
            person.IsInArmedForces = _chkArmedForces.Checked;
            if (person.IsInArmedForces)
            {
                person.ArmedForcesNo = _txtArmedForcesNo.Text.Trim();
            }
            else
            {
                person.ArmedForcesNo = string.Empty;
            }
            if (_txtNINo.Text.Replace(_meeNINo.PromptCharacter, "").Trim().Length > 0)
            {
                person.NINo = _txtNINo.Text.Trim().ToUpper();
            }
            else
            {
                person.NINo = string.Empty;
            }
            return person;
        }

        #endregion

        #region Contact Details Tab

        #region Handling Error occured from user control Contact Gridview
        /// <summary>
        /// This will fire after selection of Client from Client Search popup.
        /// This will fire after changing the matter from Matter dropdownlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _contactGridView_ErrorOccured(object sender, EventArgs e)
        {
            try
            {
                _lblMessage.CssClass = _contactGridView.MessageCssClass;
                _lblMessage.Text = _contactGridView.Message;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #endregion

        #region Client Matters Tab

        /// <summary>
        /// Displays the client matters.
        /// </summary>
        public MatterSearchItem[] GetClientMatters(int startRow, int pageSize, bool forceRefresh)
        {
            MatterServiceClient matterService = null;
            MatterSearchItem[] clientMatters = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                _organisationId = (Guid)Session[SessionName.OrganisationId];
                _memberId = (Guid)Session[SessionName.MemberId];
                matterService = new MatterServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;
                collectionRequest.ForceRefresh = forceRefresh;
                MatterSearchCriteria searchCriteria = new MatterSearchCriteria();

                if (_memberId != DataConstants.DummyGuid)
                {
                    searchCriteria.MemberId = _memberId;
                }
                else
                {
                    searchCriteria.OrganisationId = _organisationId;
                }

                MatterSearchReturnValue returnValue = matterService.MatterSearch(logonId,
                                                collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _clientMatterRowCount = returnValue.Matters.TotalRowCount;
                    clientMatters = returnValue.Matters.Rows;
                }
                return clientMatters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (matterService != null)
                {
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                        matterService.Close();
                }
            }
        }

        public int GetClientMattersRowsCount(bool forceRefresh)
        {
            return _clientMatterRowCount;
        }

        protected void _grdClientMatters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Dont display blank dates
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                MatterSearchItem searchItem = (MatterSearchItem)e.Row.DataItem;
                //Truncate large descriptions
                if (searchItem.Description.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblDescription");
                    lbldescription.Text = searchItem.Description.Substring(0, 20) + "...";
                }

                //Truncate large descriptions
                if (searchItem.KeyDescription.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblKeyDescription");
                    lbldescription.Text = searchItem.KeyDescription.Substring(0, 20) + "...";
                    //lbldescription.ToolTip = searchItem.KeyDescription;
                }

                //Hide blank dates
                if (searchItem.OpenedDate == DataConstants.BlankDate)
                {
                    Label lblOpenedDate = (Label)e.Row.FindControl("_lblOpened");
                    lblOpenedDate.Text = string.Empty;
                }

                //Hide blank dates
                if (searchItem.ClosedDate == DataConstants.BlankDate)
                {
                    Label lblClosedDate = (Label)e.Row.FindControl("_lblClosed");
                    lblClosedDate.Text = string.Empty;
                }

                //Add '-' after the client ref
                LinkButton matterLink = ((LinkButton)e.Row.FindControl("_lnkMatterReference"));

                if (matterLink.Text.Length > 6)
                {
                    matterLink.Text = matterLink.Text.Insert(6, "-");
                }
            }
        }

        protected void _grdClientMatters_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_lnkMatterReference") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    Guid projectId = new Guid(_grdClientMatters.DataKeys[rowId].Values["Id"].ToString());
                    Session[SessionName.ProjectId] = projectId;
                    Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
                }
            }
        }

        protected void _odsClientMatters_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while retrieving matters for the client
            if (e.Exception != null)
            {
                _lblMessage.CssClass = "errorMessage";
                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                else
                    _lblMessage.Text = e.Exception.InnerException.Message;
                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefresh.Value = "false";
        }

        #endregion

        #region Back
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchClient.aspx", true);
        }
        #endregion

        #region Save

        protected void _btnSave_Click(object sender, EventArgs e)
        {
            // Call the SaveContact method for Third Party users
            if (Request.QueryString["mydetails"] == "true" && _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                SaveContact();
            else
                SaveClient();

            if (_lblMessage.Text == "Edit successful")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), Guid.NewGuid().ToString(), "messageHide('" + _lblMessage.ClientID + "')", true);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), Guid.NewGuid().ToString(), "CheckUserType()", true);
            }
        }

        /// <summary>
        /// Saves the addresses.
        /// </summary>
        private void SaveAddresses()
        {
            ContactServiceClient contactService = null;
            ClientServiceClient clientService = null;
            try
            {
                CheckForModifiedAddress();
                List<string> updatedAddresses = (List<string>)ViewState[UpdatedAddresses];
                //Check if any of the addresses have been modified
                if (updatedAddresses.Count > 0)
                {
                    contactService = new ContactServiceClient();
                    List<Address> clientAddresses = (List<Address>)Session[SessionName.ClientAddresses];
                    foreach (string addressType in updatedAddresses)
                    {
                        foreach (Address address in clientAddresses)
                        {
                            if (address.TypeId.ToString() == addressType)
                            {

                                if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                                {
                                    address.MemberId = _logonSettings.MemberId;
                                    address.OrganisationId = _logonSettings.OrganisationId;
                                }
                                else
                                {
                                    address.MemberId = (Guid)Session[SessionName.MemberId];
                                    address.OrganisationId = (Guid)Session[SessionName.OrganisationId];
                                }

                                AddressReturnValue returnValue = contactService.SaveAddress(_logonSettings.LogonId, address);
                                if (!returnValue.Success)
                                {
                                    _lblMessage.CssClass = "errorMessage";
                                    _lblMessage.Text = returnValue.Message;
                                }
                                break;
                            }
                        }
                    }
                    updatedAddresses.Clear();
                    //Reload the addresses
                    clientService = new ClientServiceClient();
                    AddressSearchCriteria searchCriteria = new AddressSearchCriteria();
                    searchCriteria.MemberId = _memberId;
                    searchCriteria.OrganisationId = _organisationId;
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = true;

                    if (Request.QueryString["mydetails"] == "true" && _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        AddressSearchReturnValue addressSearchReturnValue = new AddressSearchReturnValue();
                        ContactServiceClient serviceClient = new ContactServiceClient();
                        addressSearchReturnValue = serviceClient.GetContactAddresses(_logonSettings.LogonId, collectionRequest, searchCriteria);
                        clientAddresses.Clear();
                        //Store the addresses in a list so that we can add items if necessary
                        foreach (Address address in addressSearchReturnValue.Addresses.Rows)
                        {
                            clientAddresses.Add(address);
                        }

                    }
                    else
                    {
                        AddressSearchReturnValue addresses = clientService.GetClientAddresses(_logonSettings.LogonId,
                                                                                            collectionRequest,
                                                                                            searchCriteria);
                        clientAddresses.Clear();
                        //Store the addresses in a list so that we can add items if necessary
                        foreach (Address address in addresses.Addresses.Rows)
                        {
                            clientAddresses.Add(address);
                        }
                    }


                    if (Request.QueryString["mydetails"] == "true" && _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        DisplayContactAddressDetails();
                    }
                    else
                    {
                        DisplayAddressDetails();
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
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }

                if (clientService != null)
                {
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
                        clientService.Close();
                }
            }
        }

        /// <summary>
        /// Saves the client details(user types 1 and 2).
        /// </summary>
        private void SaveClient()
        {
            //Perform HO UCN field length validation as it is not checked in the service layer
            if (_txtHOUCN.Text.Length > 0 && _txtHOUCN.Text.Length < 8)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = "All characters have not been entered for the HO UCN";
                return;
            }

            ClientServiceClient clientService = null;
            try
            {
                SaveAddresses();

                clientService = new ClientServiceClient();
                IRIS.Law.WebServiceInterfaces.Client.Client client = GetClientDetails();
                Person person = GetPersonDetails();
                Organisation organisation = GetOrganisationDetails();
                ReturnValue returnValue = clientService.UpdateClient(_logonSettings.LogonId, client, person, organisation);
                if (returnValue.Success)
                {
                    _lblMessage.CssClass = "successMessage";
                    _lblMessage.Text = "Edit successful";
                    //Generate client name label as the details may have changed
                    if (_memberId != DataConstants.DummyGuid)
                    {
                        _lblClientName.Text = CommonFunctions.MakeFullName(_ddlTitle.SelectedItem.Text.Trim(),
                                    _txtForenames.Text.Trim(), _txtSurname.Text.Trim());
                    }
                    else
                    {
                        _lblClientName.Text = _txtOrganisationName.Text.Trim();
                    }

                    _imgClientArchieved.Visible = _chkArchiveClient.Checked;
                }
                else
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = returnValue.Message;
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
            finally
            {
                if (clientService != null)
                {
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
                        clientService.Close();
                }
            }
        }

        /// <summary>
        /// Saves the contact details(user type 3).
        /// </summary>
        private void SaveContact()
        {
            ////Perform HO UCN field length validation as it is not checked in the service layer
            //if (_txtHOUCN.Text.Length > 0 && _txtHOUCN.Text.Length < 8)
            //{
            //    _lblMessage.CssClass = "errorMessage";
            //    _lblMessage.Text = "All characters have not been entered for the HO UCN";
            //    return;
            //}

            //ClientServiceClient clientService = null;
            ContactServiceClient contactService = null;
            try
            {
                ContactType contactType = ContactType.Individual;

                SaveAddresses();
                contactService = new ContactServiceClient();
                //clientService = new ClientServiceClient();
                //IRIS.Law.WebServiceInterfaces.Client.Client client = GetClientDetails();
                Person person = GetPersonDetails();
                Organisation organisation = GetOrganisationDetails();

                if (_memberId == DataConstants.DummyGuid)
                    contactType = ContactType.Organisation;

                //ReturnValue returnValue = clientService.UpdateClient(_logonSettings.LogonId, client, person, organisation);
                ReturnValue returnValue = contactService.UpdateGeneralContact(_logonSettings.LogonId, person, contactType, organisation);

                if (returnValue.Success)
                {
                    _lblMessage.CssClass = "successMessage";
                    _lblMessage.Text = "Edit successful";
                    //Generate client name label as the details may have changed
                    if (_memberId != DataConstants.DummyGuid)
                    {
                        _lblClientName.Text = CommonFunctions.MakeFullName(_ddlTitle.SelectedItem.Text.Trim(),
                                    _txtForenames.Text.Trim(), _txtSurname.Text.Trim());
                    }
                    else
                    {
                        _lblClientName.Text = _txtOrganisationName.Text.Trim();
                    }

                    // _imgClientArchieved.Visible = _chkArchiveClient.Checked;
                }
                else
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = returnValue.Message;
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

    }
}

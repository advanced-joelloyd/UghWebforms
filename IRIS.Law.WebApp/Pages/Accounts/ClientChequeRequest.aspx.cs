using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ClientChequeRequest : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;

        #endregion

        #region Protected Methods

        #region Form Events

        /// <summary>
        /// Loads page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
               
                _lblMessage.Text = string.Empty;

                if (Request.QueryString["edit"] != "true")
                {
                    _divBtnAddNew.Visible = false;
                }
                else
                {
                    _divReset.Visible = false;
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
            // if (!IsPostBack)
            LoadPageDetails();
        }

        #endregion

        #endregion

        #region Control Events

        protected void _btnAddNew_Click(object sender, EventArgs e)
        {
            Session[SessionName.ChequeRequestId] = null;
            Response.Redirect("~/Pages/Accounts/ClientChequeRequest.aspx", true);
        }

        

        /// <summary>
        /// Saves client cheque request details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Sets error message if posting date is blank
                if (string.IsNullOrEmpty(_ccPostDate.DateText))
                {
                    _lblPostingPeriod.Text = "Invalid";
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Posting Date is not within the current financial year";
                    return;
                }

                bool isPostingPeriodValid = SetPostingPeriod();

                if (isPostingPeriodValid)
                {
                    AccountsServiceClient accountsService = null;

                    try
                    {
                        accountsService = new AccountsServiceClient();
                        IRIS.Law.WebServiceInterfaces.Accounts.ChequeRequest clientChequeRequest = GetControlData();

                        // This flag is used to set the 'ProceedIfOverDrawn' flag on service class 
                        // after getting warning for insufficient funds
                        if (ViewState["ProceedIfOverDrawn"] != null)
                        {
                            if (Convert.ToBoolean(ViewState["ProceedIfOverDrawn"]))
                            {
                                clientChequeRequest.ProceedIfOverDrawn = true;
                            }
                            else
                            {
                                clientChequeRequest.ProceedIfOverDrawn = false;
                            }
                        }

                        ChequeRequestReturnValue returnValue = accountsService.SaveClientChequeRequest(_logonSettings.LogonId, clientChequeRequest);

                        if (returnValue.Success)
                        {
                            ViewState["ProceedIfOverDrawn"] = false;

                            // If print checkbox is checked then prints cheque request details
                            // else shows success message on the same page.
                            if (_chkBxPrintChequeRequest.Checked)
                            {
                                // Newly added cheque request id to populate details for printable format.
                                Session[SessionName.ChequeRequestId] = returnValue.ChequeRequest.ChequeRequestId;

                                // To redirect to printable format of cheque request details.
                                Response.Redirect("~/Pages/Accounts/PrintableClientChequeRequest.aspx", true);
                            }
                            else
                            {
                                ResetControls(true);

                                if (returnValue.Message != string.Empty)
                                {
                                    _lblMessage.CssClass = "errorMessage";
                                    _lblMessage.Text = "Client Cheque Request Saved Successfully. " + returnValue.Message;
                                }
                                else
                                {
                                    _lblMessage.CssClass = "successMessage";
                                    _lblMessage.Text = "Client Cheque Request Saved Successfully.";
                                }
                            }
                        }
                        else
                        {
                            _lblMessage.CssClass = "errorMessage";

                            if (!string.IsNullOrEmpty(returnValue.Message))
                            {
                                _lblMessage.Text = returnValue.Message;
                            }
                            else if (!string.IsNullOrEmpty(returnValue.WarningMessage))
                            {
                                ViewState["ProceedIfOverDrawn"] = true;
                                _lblMessage.Text = returnValue.WarningMessage.Replace("Client account will be overdrawn, are you sure you wish to proceed?", "Client account will be overdrawn, press Save if you are sure you wish to proceed?");
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
                        if (accountsService != null)
                        {
                            if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                                accountsService.Close();
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Matter on matter change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _cliMatDetails_MatterChanged(object sender, EventArgs e)
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
                    SetClientBank((Guid)Session[SessionName.ProjectId]);
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

        #endregion

        #region Private Methods

        #region SetPostingPeriod

        /// <summary>
        /// Validates the posting period
        /// </summary>
        private bool SetPostingPeriod()
        {
            try
            {
                PeriodDetailsReturnValue returnValue = ValidatePostingPeriod(DataConstants.DummyGuid);//, IRIS.Law.PmsCommonData.DataConstantsBankSearchTypes.Client);

                if (returnValue.Success)
                {
                    // for warning message modal popup with "OK" button will be shown.
                    if (!string.IsNullOrEmpty(returnValue.WarningMessage))
                    {
                        _lblMessage.CssClass = "successMessage";
                        _lblMessage.Text = returnValue.WarningMessage;
                    }

                    // Sets valid posting period else "Invalid"
                    _lblPostingPeriod.Text = returnValue.PostingPeriodNumber;
                }
                else
                {
                    // Sets posting error message only if posting period is invalid
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = returnValue.ErrorMessage;
                }

                return returnValue.Success;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
                return false;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Validates posting period on page load and gets client banks
        /// </summary>
        /// <param name="bankType"></param>
        /// Previous - private PeriodDetailsReturnValue ValidatePostingPeriod(Guid projectId, IRIS.Law.PmsCommonData.DataConstantsBankSearchTypes bankType)
        private PeriodDetailsReturnValue ValidatePostingPeriod(Guid projectId)
        {
            TimeServiceClient timeService = null;
            PeriodDetailsReturnValue returnValue = null;

            try
            {
                PeriodCriteria criteria = new PeriodCriteria();
                criteria.Date = Convert.ToDateTime(_ccPostDate.DateText);
                criteria.IsPostingVATable = false;
                criteria.IsTime = false;
                criteria.ProjectId = projectId;
                // This parameter has been passed as "false" in accounts
                criteria.IsAllowedPostBack2ClosedYear = false;

                timeService = new TimeServiceClient();
                returnValue = timeService.ValidatePeriod(_logonSettings.LogonId, criteria);
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
                if (timeService != null)
                {
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                        timeService.Close();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the client banks.
        /// </summary>
        private void LoadPageDetails()
        {
            if (!IsPostBack)
            {
                // Sets defaults on page load
                SetDefaults();

                // Binds client banks for client cheque request
                GetClientBanks();

                //Populate Clearance Types
                if (_ddlClientDebitCredit.SelectedValue == "Credit")
                {
                    GetClientClearanceTypes();
                    SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), true);
                }
                else
                {
                    GetClientClearanceTypes();
                    SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), false);
                }

                // If project id is in session then set the clinet bank id by project id
                if (Session[SessionName.ProjectId] != null)
                {
                    // Sets client bank for project id in context
                    SetClientBank((Guid)Session[SessionName.ProjectId]);
                }

                if (Session[SessionName.ChequeRequestId] != null)
                {
                    _hdnClientChequeRequestId.Value = Convert.ToString(Session[SessionName.ChequeRequestId]);
                    LoadControls();
                    _cliMatDetails.DisableClientMatter = true;
                    Session[SessionName.ChequeRequestId] = null;
                }
                else
                {
                    _hdnClientChequeRequestId.Value = "0";
                    ViewState["ChequeRequestProjectId"] = DataConstants.DummyGuid.ToString();
                    ViewState["MemberId"] = DataConstants.DummyGuid.ToString();
                }

                // Validates the posting period on page load
                SetPostingPeriod();
            }
        }

        /// <summary>
        /// Gets the client banks.
        /// </summary>
        private void GetClientBanks()
        {
            try
            {
                BankSearchItem[] banks = GetBanks(DataConstants.BankSearchTypes.Client);
                _ddlClientBank.DataSource = banks;
                _ddlClientBank.DataTextField = "Description";
                _ddlClientBank.DataValueField = "BankId";
                _ddlClientBank.DataBind();
                AddDefaultToDropDownList(_ddlClientBank);
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
        /// Gets the client clearance type.
        /// </summary>
        private void GetClientClearanceTypes()
        {
            try
            {
                bool isCredit = false;

                if (_ddlClientDebitCredit.SelectedValue == "Credit")
                    isCredit = true;
                
                ClearanceTypeSearchItem[] clearanceTypes = GetClearanceType(isCredit);
                _dllClearanceType.DataSource = clearanceTypes;
                _dllClearanceType.DataTextField = "ClearanceTypeDesc";
                _dllClearanceType.DataValueField = "ClearanceTypeId";
                _dllClearanceType.DataBind();
               
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
        /// Gets list of client banks
        /// </summary>
        /// <param name="bankType">Bank type should be of client type</param>
        /// <returns>Returns banks of client type on page load.</returns>
        private BankSearchItem[] GetBanks(DataConstants.BankSearchTypes bankType)
        {
            BankServiceClient bankService = null;
            BankSearchItem[] banks = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                BankSearchCriteria criteria = new BankSearchCriteria();
                criteria.IncludeArchived = false;
                criteria.BankSearchTypes = (int)bankType;
                             

                bankService = new BankServiceClient();
                BankSearchReturnValue returnValue = bankService.BankSearch(_logonSettings.LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    banks = returnValue.Banks.Rows;
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
                return banks;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
                return banks;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                return banks;
            }
            finally
            {
                if (bankService != null)
                {
                    if (bankService.State != System.ServiceModel.CommunicationState.Faulted)
                        bankService.Close();
                }
            }
        }

        /// <summary>
        /// Gets list of clearance type
        /// </summary>
        /// <returns>Returns clearance types.</returns>
        private ClearanceTypeSearchItem[] GetClearanceType(bool isCredit)
        {
            AccountsServiceClient accountService = null;
            ClearanceTypeSearchItem[] clearanceType = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                ClearanceTypeSearchCriteria criteria = new ClearanceTypeSearchCriteria();
                criteria.IsCredit = isCredit;
                criteria.IncludeArchived = false;
                
                accountService = new AccountsServiceClient();
                ClearanceTypeSearchReturnValue returnValue = accountService.ClearanceTypes(_logonSettings.LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    clearanceType = returnValue.ClearanceTypes.Rows;
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
                return clearanceType;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
                return clearanceType;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                return clearanceType;
            }
            finally
            {
                if (accountService != null)
                {
                    if (accountService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountService.Close();
                }
            }
        }

        /// <summary>
        /// Populate Clearance Days
        /// </summary>
        private void SetClearanceDays(int clearanceTypeId, bool isCredit)
        {
            AccountsServiceClient accountService = null;
 
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                ClearanceTypeSearchCriteria criteria = new ClearanceTypeSearchCriteria();
                criteria.IsCredit = isCredit;
                criteria.IncludeArchived = false;

                accountService = new AccountsServiceClient();
                ClearanceTypeSearchReturnValue returnValue = accountService.ClearanceTypes(_logonSettings.LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    foreach (ClearanceTypeSearchItem type in returnValue.ClearanceTypes.Rows)
                    {
                        if (clearanceTypeId == type.ClearanceTypeId)
                        {
                            _txtClearanceDaysChq.Text = type.ClearanceTypeChqDays.ToString();
                            _txtClearanceDaysElec.Text = type.ClearanceTypeElecDays.ToString();
                        }
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
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
                if (accountService != null)
                {
                    if (accountService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountService.Close();
                }
            }
        }

        /// <summary>
        /// Sets default values for the controls on page load
        /// </summary>
        private void SetDefaults()
        {
            try
            {
                // By default date should be today's date
                _ccPostDate.DateText = Convert.ToString(DateTime.Today);
                _txtAmount.Text = Decimal.Zero.ToString("0.00");
            }
            catch (Exception ex)
            {

                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        /// <summary>
        /// Gets the client banks.
        /// </summary>
        private void BindClientBanks(BankSearchItem[] banks)
        {
            try
            {
                _ddlClientBank.DataSource = banks;
                _ddlClientBank.DataTextField = "Description";
                _ddlClientBank.DataValueField = "BankId";
                _ddlClientBank.DataBind();
                AddDefaultToDropDownList(_ddlClientBank);
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
        /// Adds the default value to dropdown list.
        /// </summary>
        /// <param name="ddlList">The dropdown list control.</param>
        private void AddDefaultToDropDownList(DropDownList dropDownList)
        {
            try
            {
                ListItem listSelect = new ListItem("Select", "");
                dropDownList.Items.Insert(0, listSelect);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        #region SetClientBank

        /// <summary>
        /// Sets client bank by project id in session
        /// </summary>
        /// <param name="projectId">Project id in session sets the client bank.</param>
        private void SetClientBank(Guid projectId)
        {
            AccountsServiceClient accountsService = null;
            ClientBankIdReturnValue returnValue = null;

            try
            {
                returnValue = new ClientBankIdReturnValue();

                //If a value is selected then save it, this prevents the default value from 
                //overriding the selection
                if (_ddlClientBank.Items.Count > 0)
                {
                    accountsService = new AccountsServiceClient();
                    returnValue = accountsService.GetClientBankIdByProjectId(_logonSettings.LogonId, projectId);
                }

                if (returnValue.ClientBankId != 0)
                {
                    if (_ddlClientBank.Items.FindByValue(returnValue.ClientBankId.ToString()) != null)
                    {
                        _ddlClientBank.SelectedValue = returnValue.ClientBankId.ToString();
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
                if (accountsService != null)
                {
                    if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountsService.Close();
                }
            }
        }

        #endregion

        #region GetControlData

        /// <summary>
        /// Gets data from the controls and sets properties for client cheque request class.
        /// </summary>
        /// <returns>Returns object to client cheque request class</returns>
        private IRIS.Law.WebServiceInterfaces.Accounts.ChequeRequest GetControlData()
        {
            ChequeRequest clientChequeRequest = null;

            try
            {
                clientChequeRequest = new ChequeRequest();

                if (Session[SessionName.ProjectId] != null)
                {
                    clientChequeRequest.ProjectId = new Guid(Convert.ToString(Session[SessionName.ProjectId]));
                }

                if (!string.IsNullOrEmpty(_ddlClientBank.SelectedValue))
                {
                    clientChequeRequest.BankId = Convert.ToInt32(_ddlClientBank.SelectedValue);
                }

                clientChequeRequest.ChequeRequestId = Convert.ToInt32(_hdnClientChequeRequestId.Value);
                clientChequeRequest.ChequeRequestDate = Convert.ToDateTime(_ccPostDate.DateText);
                clientChequeRequest.ChequeRequestAmount = Convert.ToDecimal(_txtAmount.Text.Trim());
                clientChequeRequest.ChequeRequestDescription = _txtDescription.Text.Trim();

                if (_txtPayee.Text != string.Empty)
                {
                    clientChequeRequest.ChequeRequestPayee = _txtPayee.Text.Trim();
                }
                else
                {
                    clientChequeRequest.ChequeRequestPayee = _txtDescription.Text.Trim();
                }

                clientChequeRequest.IsChequeRequestAuthorised = _chkBxAuthorise.Checked;
                clientChequeRequest.MemberId = new Guid(ViewState["MemberId"].ToString());
                // As suggested by client: Set reference to "CHQ"
                clientChequeRequest.ChequeRequestReference = _txtReference.Text;

                if (_ddlClientDebitCredit.SelectedValue == "Credit")
                {
                    clientChequeRequest.ClientChequeRequestsIsCredit = true;
                }
                else
                {
                    clientChequeRequest.ClientChequeRequestsIsCredit = false;
                }

                clientChequeRequest.ClearanceTypeId = Convert.ToInt32(_dllClearanceType.SelectedValue);
                clientChequeRequest.ClientChequeRequestsClearanceDaysChq = Convert.ToInt32(_txtClearanceDaysChq.Text);
                clientChequeRequest.ClientChequeRequestsClearanceDaysElec = Convert.ToInt32(_txtClearanceDaysElec.Text);

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

            return clientChequeRequest;
        }

        #endregion

        #region ResetControls

        /// <summary>
        /// Resets controls on page
        /// </summary>
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            _lblMessage.Text = "";
            ResetControls(false);
        }

        private void ResetControls(bool IsSave)
        {
            try
            {
                _txtDescription.Text = string.Empty;
                _txtPayee.Text = string.Empty;
                _txtAmount.Text = "0.00";
                _updPanelAmount.Update();
                _txtReference.Text = "";
                
                if (!IsSave)
                    _ccPostDate.DateText = Convert.ToString(DateTime.Today);
                
                SetPostingPeriod();

                if (IsSave)
                    SetClientBank((Guid)Session[SessionName.ProjectId]);
                else
                    _ddlClientBank.SelectedIndex = 0;
                
                _updPanelError.Update();
                _ddlClientDebitCredit.SelectedIndex = 0;
                _dllClearanceType.SelectedIndex = 0;
                _chkBxAuthorise.Checked = false;
                _chkBxPrintChequeRequest.Checked = false;
                GetClientClearanceTypes();
                SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), true);

                _spanPayee.Visible = false;
                _txtPayee.Visible = false;
                _lblPayee.Visible = false;
                _txtPayee.Text = string.Empty;
                _rfvPayee.Enabled = false;


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

        /// <summary>
        /// Loads client cheque request details by client cheque request id
        /// </summary>
        /// <param name="chequeRequestId">Id reuired to populate client cheque request details.</param>
        private void LoadControls()
        {
            Guid projectId = DataConstants.DummyGuid;
            Guid memberId = DataConstants.DummyGuid;
            AccountsServiceClient accountsService = null;
            ChequeRequestReturnValue returnValue = null;

            if (_hdnClientChequeRequestId.Value.Trim().Length > 0)
            {
                accountsService = new AccountsServiceClient();

                try
                {
                    returnValue = new ChequeRequestReturnValue();
                    returnValue = accountsService.LoadClientChequeRequestDetails(_logonSettings.LogonId, Convert.ToInt32(_hdnClientChequeRequestId.Value));

                    if (returnValue.Success)
                    {
                        if (returnValue.ChequeRequest != null)
                        {
                            memberId = returnValue.ChequeRequest.MemberId;
                            projectId = returnValue.ChequeRequest.ProjectId;
                            _ccPostDate.DateText = returnValue.ChequeRequest.ChequeRequestDate.ToString("dd/MM/yyyy");
                            _txtDescription.Text = returnValue.ChequeRequest.ChequeRequestDescription;
                            _txtPayee.Text = returnValue.ChequeRequest.ChequeRequestPayee;
                            _ddlClientBank.SelectedValue = Convert.ToString(returnValue.ChequeRequest.BankId);
                            _txtAmount.Text = returnValue.ChequeRequest.ChequeRequestAmount.ToString();
                            _chkBxAuthorise.Checked = returnValue.ChequeRequest.IsChequeRequestAuthorised;
                            _txtReference.Text = returnValue.ChequeRequest.ChequeRequestReference;

                            if (_txtReference.Text.Trim().ToLower() == "chq")
                            {
                                _spanPayee.Visible = true;
                                _txtPayee.Visible = true;
                                _lblPayee.Visible = true;
                                _txtPayee.Text = _txtDescription.Text;
                                _rfvPayee.Enabled = true;
                            }

                            _txtClearanceDaysChq.Text = returnValue.ChequeRequest.ClientChequeRequestsClearanceDaysChq.ToString();
                            _txtClearanceDaysElec.Text = returnValue.ChequeRequest.ClientChequeRequestsClearanceDaysElec.ToString();

                            if (returnValue.ChequeRequest.ClientChequeRequestsIsCredit)
                            {
                                _ddlClientDebitCredit.SelectedValue = "Credit";
                                GetClientClearanceTypes();
                                //SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), true);
                            }
                            else
                            {
                                _ddlClientDebitCredit.SelectedValue = "Debit";
                                GetClientClearanceTypes();
                                //SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), false);
                            }
                          
                            
                            //Populate DropDownList
                            _dllClearanceType.SelectedValue = returnValue.ChequeRequest.ClearanceTypeId.ToString();
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
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
                    if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountsService.Close();
                }

                try
                {
                    if (memberId != DataConstants.DummyGuid)
                    {
                        ViewState["MemberId"] = memberId;
                    }
                    else
                    {
                        ViewState["MemberId"] = DataConstants.DummyGuid.ToString();
                    }

                    if (projectId != DataConstants.DummyGuid)
                    {
                        ViewState["ChequeRequestProjectId"] = projectId;
                        HttpContext.Current.Session[SessionName.ProjectId] = projectId;

                        // Loads client matter details.
                        LoadClientMatterDetails(projectId);
                    }
                    else
                    {
                        _cliMatDetails.LoadData = false;
                        ViewState["ChequeRequestProjectId"] = DataConstants.DummyGuid.ToString();
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
        }

        #endregion

        #region LoadClientMatterDetails

        /// <summary>
        /// Loads client matter details on load controls in edit mode
        /// </summary>
        /// <param name="projectId"></param>
        private void LoadClientMatterDetails(Guid projectId)
        {
            MatterServiceClient matterService = null;
            MatterReturnValue matterReturnValue = null;

            try
            {
                matterService = new MatterServiceClient();

                try
                {
                    matterReturnValue = new MatterReturnValue();
                    matterReturnValue = matterService.GetMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, projectId);

                    if (matterReturnValue.Success)
                    {
                        if (matterReturnValue != null)
                        {
                            #region Load Client Details
                            // Stores ClientID
                            if (matterReturnValue.ClientDetails.IsMember)
                            {
                                Session[SessionName.MemberId] = matterReturnValue.ClientDetails.MemberId;
                                Session[SessionName.OrganisationId] = DataConstants.DummyGuid;
                            }
                            else
                            {
                                Session[SessionName.MemberId] = DataConstants.DummyGuid;
                                // Shouldn't be looking for  MemberId for Organisation Client 
                               // Session[SessionName.OrganisationId] = matterReturnValue.ClientDetails.MemberId;
                                Session[SessionName.OrganisationId] = matterReturnValue.ClientDetails.OrganisationId;
                                
                            }

                            _cliMatDetails.IsClientMember = matterReturnValue.ClientDetails.IsMember;
                            Session[SessionName.ClientRef] = matterReturnValue.ClientDetails.Reference;
                            Session[SessionName.ClientName] = matterReturnValue.ClientDetails.FullName;

                            if (_cliMatDetails.Message != null)
                            {
                                if (_cliMatDetails.Message.Trim().Length > 0)
                                {
                                    throw new Exception("Loading failed for Client Matter Details.<br>Exception occured is: " + _cliMatDetails.Message);
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(matterReturnValue.Message);
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
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                        matterService.Close();
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

        #endregion

        protected void _ddlClientDebitCredit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ddlClientDebitCredit.SelectedValue == "Credit")
            {
                GetClientClearanceTypes();
                SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), true);
            }
            else
            {
                GetClientClearanceTypes();
                SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), false);
            }
        }

        protected void _dllClearanceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ddlClientDebitCredit.SelectedValue == "Credit")
            {
                SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), true);
            }
            else
            {
                SetClearanceDays(int.Parse(_dllClearanceType.SelectedValue), false);
            }
        }

        protected void _txtReference_TextChanged(object sender, EventArgs e)
        {
            if (_txtReference.Text.ToLower() == "chq")
            {
                _spanPayee.Visible = true;
                _txtPayee.Visible = true;
                _lblPayee.Visible = true;
                _txtPayee.Text = _txtDescription.Text;
                _rfvPayee.Enabled = true;
            }
            else
            {
                _spanPayee.Visible = false;
                _txtPayee.Visible = false;
                _lblPayee.Visible = false;
                _txtPayee.Text = string.Empty ;
                _rfvPayee.Enabled = false;
            }
        }

        protected void _ccPostDate_DateChanged(object sender, EventArgs e)
        {
            if(_ccPostDate.DateText=="")
            {
                _lblPostingPeriod.Text = "Invalid";
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = "Posting Date is not within the current financial year";

                return;
            }

            SetPostingPeriod();
        }

        #region TextChangedEvent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _txtAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_txtAmount.Text.Contains("."))
                {
                    _txtAmount.Text = _txtAmount.Text + ".00";
                }
 
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion
    }
}

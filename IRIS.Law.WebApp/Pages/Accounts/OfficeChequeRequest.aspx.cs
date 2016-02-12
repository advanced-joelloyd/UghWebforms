using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class OfficeChequeRequest : BasePage
    {
        #region Private Variables

        private LogonReturnValue _logonSettings;
        private bool _editMode;

        #endregion

        #region Protected Methods

        #region Page Load

        /// <summary>
        /// Loads page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

            if (Request.QueryString["edit"] != "true")
            {
                _divBtnAddNew.Visible = false;
            }
            else
            {
                _divReset.Visible = false;
            }

            if (!IsPostBack)
            {
                // Sets controls with default values
                SetDefaults();

                if (Session[SessionName.ProjectId] != null)
                {
                    ViewState["ChequeRequestProjectId"] = new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString());
                }

                if (Session[SessionName.ChequeRequestId] != null)
                {
                    _hdnOfficeChequeRequestId.Value = Convert.ToString(Session[SessionName.ChequeRequestId]);
                    LoadOfficeChequeRequestDetails();
                    _cliMatDetails.DisableClientMatter = true;
                    _editMode = true;
                    Session[SessionName.ChequeRequestId] = null;
                }
                else
                {
                    _editMode = false;
                    _hdnOfficeChequeRequestId.Value = "0";
                    ViewState["ChequeRequestProjectId"] = DataConstants.DummyGuid.ToString();
                    ViewState["MemberId"] = DataConstants.DummyGuid.ToString();
                }
                HideUnhideVATDetails();

                // Sets posting period and error messages for posting date                
                SetPostingPeriod();
            }

            if (HttpContext.Current.Session["ClientRef"] != null)
            {
                //_ddlBank.SelectedIndex = 1;
            }
        }
        #endregion
                
        #region ValidatePostingPeriod

        /// <summary>
        /// Validates posting period for creating office cheque requests
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="bankType"></param>
        /// <returns></returns>
        /// Previous private PeriodDetailsReturnValue ValidatePostingPeriod(Guid projectId, IRIS.Law.PmsCommonData.DataConstantsBankSearchTypes bankType)
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
        #endregion

        #region SetPostingPeriod

        /// <summary>
        /// Sets posting period after validating posting period.
        /// </summary>
        /// <returns>Returns true if posting period is valid, then details can bee saved.</returns>
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

        #region LoadOfficeChequeRequestDetails

        /// <summary>
        /// Loads office cheque request details in edit mode.
        /// </summary>
        private void LoadOfficeChequeRequestDetails()
        {
            Guid projectId = DataConstants.DummyGuid;
            Guid memberId = DataConstants.DummyGuid;
            if (_hdnOfficeChequeRequestId.Value.Trim().Length > 0)
            {
                AccountsServiceClient accountsService = new AccountsServiceClient();
                try
                {
                    ChequeRequestReturnValue accountsReturnValue = new ChequeRequestReturnValue();
                    accountsReturnValue = accountsService.LoadOfficeChequeRequestDetails(_logonSettings.LogonId, Convert.ToInt32(_hdnOfficeChequeRequestId.Value));

                    if (accountsReturnValue.Success)
                    {
                        if (accountsReturnValue.ChequeRequest != null)
                        {
                            projectId = accountsReturnValue.ChequeRequest.ProjectId;
                            _ccPostDate.DateText = accountsReturnValue.ChequeRequest.ChequeRequestDate.ToString("dd/MM/yyyy");
                            _ddlDisbursementType.SelectedIndex = -1;
                            if (_ddlDisbursementType.Items.FindByValue(accountsReturnValue.ChequeRequest.DisbursementTypeId.ToString()) != null)
                            {
                                _ddlDisbursementType.Items.FindByValue(accountsReturnValue.ChequeRequest.DisbursementTypeId.ToString()).Selected = true;
                            }
                            _txtDescription.Text = accountsReturnValue.ChequeRequest.ChequeRequestDescription;
                            _txtReference.Text = "CHQ";
                            _txtPayee.Text = accountsReturnValue.ChequeRequest.ChequeRequestPayee;
                            _ddlBank.SelectedIndex = -1;
                            if (_ddlBank.Items.FindByValue(accountsReturnValue.ChequeRequest.BankId.ToString()) != null)
                            {
                                _ddlBank.Items.FindByValue(accountsReturnValue.ChequeRequest.BankId.ToString()).Selected = true;
                            }

                            _txtAmount.Text = accountsReturnValue.ChequeRequest.ChequeRequestAmount.ToString("0.00");
                            _ddlVAT.SelectedIndex = -1;
                            if (accountsReturnValue.ChequeRequest.OfficeVATTable == IRIS.Law.PmsCommonData.Accounts.AccountsDataConstantsYesNo.Yes)
                            {
                                _ddlVAT.Items.FindByValue("Yes").Selected = true;
                            }
                            else
                            {
                                _ddlVAT.Items.FindByValue("No").Selected = true;
                            }

                            _ddlVATRate.SelectedIndex = -1;
                            for (int i = 0; i < _ddlVATRate.Items.Count; i++)
                            {
                                string vatRateId = GetValueOnIndexFromArray(_ddlVATRate.Items[i].Value, 0);
                                if (vatRateId == accountsReturnValue.ChequeRequest.VATRateId.ToString())
                                {
                                    _ddlVATRate.Items[i].Selected = true;
                                }
                            }

                            _txtVATAmount.Text = accountsReturnValue.ChequeRequest.VATAmount.ToString("0.00");

                            _chkBxAnticipated.Checked = accountsReturnValue.ChequeRequest.IsChequeRequestAnticipated;
                            _chkBxAuthorise.Checked = accountsReturnValue.ChequeRequest.IsChequeRequestAuthorised;
                            memberId = accountsReturnValue.ChequeRequest.MemberId;
                            HideUnhideVATDetails();
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(accountsReturnValue.Message);
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

        #region HideUnhideVATDetails
        private void HideUnhideVATDetails()
        {
            try
            {
                if (_ddlVAT.SelectedValue == "Yes")
                {
                    _tdLblVATRate.Style["display"] = "";
                    _tdDdlVATRate.Style["display"] = "";
                    _tdLblVATAmount.Style["display"] = "";
                    _tdTxtVATAmount.Style["display"] = "";
                    GetVATAmountOnVatRate();
                }
                else
                {
                    _tdLblVATRate.Style["display"] = "none";
                    _tdDdlVATRate.Style["display"] = "none";
                    _tdLblVATAmount.Style["display"] = "none";
                    _tdTxtVATAmount.Style["display"] = "none";
                }
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region LoadClientMatterDetails
        private void LoadClientMatterDetails(Guid projectId)
        {
            try
            {
                MatterServiceClient matterService = new MatterServiceClient();
                try
                {
                    MatterReturnValue matterReturnValue = new MatterReturnValue();
                    matterReturnValue = matterService.GetMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, projectId);

                    if (matterReturnValue.Success)
                    {
                        if (matterReturnValue != null)
                        {
                            #region Load Client Details
                            // Store ClientID
                            if (matterReturnValue.ClientDetails.IsMember)
                            {
                                Session[SessionName.MemberId] = matterReturnValue.ClientDetails.MemberId;
                                Session[SessionName.OrganisationId] = DataConstants.DummyGuid;
                            }
                            else
                            {
                                Session[SessionName.MemberId] = DataConstants.DummyGuid;
                                // Shouldn't be looking at memberID for organisation client. 
                              //  Session[SessionName.OrganisationId] = matterReturnValue.ClientDetails.MemberId;
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

        #region GetControlData

        /// <summary>
        /// Gets data from controls for save
        /// </summary>
        /// <returns></returns>
        private ChequeRequest GetControlData()
        {
            ChequeRequest chequeReq = new ChequeRequest();

            try
            {
                if (string.IsNullOrEmpty(_txtAmount.Text))
                {
                    throw new Exception("Amount is mandatory.");
                }

                if (Convert.ToDecimal(_txtAmount.Text.Trim()) == 0)
                {
                    throw new Exception("Amount is mandatory.");
                }

                if (ViewState["ChequeRequestProjectId"] != null)
                {
                    if (new Guid(Convert.ToString(ViewState["ChequeRequestProjectId"])) != DataConstants.DummyGuid)
                    {
                        chequeReq.ProjectId = new Guid(Convert.ToString(ViewState["ChequeRequestProjectId"]));
                    }
                    else
                    {
                        if (Session[SessionName.ProjectId] != null)
                        {
                            chequeReq.ProjectId = new Guid(Convert.ToString(Session[SessionName.ProjectId]));
                        }
                    }
                }

                chequeReq.ChequeRequestId = Convert.ToInt32(_hdnOfficeChequeRequestId.Value);
                chequeReq.ChequeRequestDate = Convert.ToDateTime(_ccPostDate.DateText);
                chequeReq.DisbursementTypeId = Convert.ToInt32(_ddlDisbursementType.SelectedValue);
                chequeReq.ChequeRequestDescription = _txtDescription.Text;
                chequeReq.ChequeRequestPayee = _txtPayee.Text;
                chequeReq.BankId = Convert.ToInt32(_ddlBank.SelectedValue);
                chequeReq.ChequeRequestAmount = Convert.ToDecimal(_txtAmount.Text);

                if (_ddlVAT.SelectedValue == "Yes")
                {
                    chequeReq.OfficeVATTable = IRIS.Law.PmsCommonData.Accounts.AccountsDataConstantsYesNo.Yes;
                }
                else
                {
                    chequeReq.OfficeVATTable = IRIS.Law.PmsCommonData.Accounts.AccountsDataConstantsYesNo.No;
                }

                chequeReq.VATRateId = Convert.ToInt32(GetValueOnIndexFromArray(_ddlVATRate.SelectedValue, 0));
                chequeReq.VATAmount = Convert.ToDecimal(_txtVATAmount.Text);
                chequeReq.IsChequeRequestAnticipated = _chkBxAnticipated.Checked;
                chequeReq.IsChequeRequestAuthorised = _chkBxAuthorise.Checked;
                chequeReq.MemberId = new Guid(ViewState["MemberId"].ToString());

                return chequeReq;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
                return chequeReq;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                return chequeReq;
            }
        }

        #endregion

        #region Save
        /// <summary>
        /// Saves office cheque request details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    // Sets error message if posting date is blank
                    if (string.IsNullOrEmpty(_ccPostDate.DateText))
                    {
                        _lblPostingPeriod.Text = "Invalid";
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = "Posting Date is not within the current financial year";
                        return;
                    }

                    ChequeRequest chequeRequest = GetControlData();
                    if (chequeRequest != null)
                    {
                        AccountsServiceClient accountsService = null;
                        try
                        {
                            bool isPostingPeriodValid = SetPostingPeriod();

                            if (isPostingPeriodValid)
                            {
                                accountsService = new AccountsServiceClient();
                                ChequeRequestReturnValue returnValue = accountsService.SaveOfficeChequeRequest(_logonSettings.LogonId, chequeRequest);

                                if (returnValue.Success)
                                {
                                    // If print checkbox is checked then prints cheque request details
                                    // else shows success message on the same page.
                                    if (_chkBxPrintChequeRequest.Checked)
                                    {
                                        // Newly added cheque request id to populate details for printable format.
                                        Session[SessionName.ChequeRequestId] = returnValue.ChequeRequest.ChequeRequestId;

                                        // To redirect to printable format of cheque request details.
                                        Response.Redirect("~/Pages/Accounts/PrintableOfficeChequeRequest.aspx", true);
                                    }
                                    else
                                    {
                                        ResetControls(true);
                                        _lblMessage.CssClass = "successMessage";
                                        _lblMessage.Text = "Office Cheque Request Saved Successfully.";
                                    }
                                }
                                else
                                {
                                    _lblMessage.CssClass = "errorMessage";
                                    _lblMessage.Text = returnValue.Message;
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

        #region AddNew

        protected void _btnAddNew_Click(object sender, EventArgs e)
        {
            Session[SessionName.ChequeRequestId] = null;
            Response.Redirect("~/Pages/Accounts/OfficeChequeRequest.aspx", true);
        }

        #endregion

        

        #region ResetControls

        /// <summary>
        /// Resets controls on page
        /// </summary>
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            _lblMessage.Text="";
            ResetControls(false);
        }

        #endregion

        #region GetVATAmountOnVatRate

        /// <summary>
        /// Gets VAT amount on VAT rate
        /// </summary>
        public void GetVATAmountOnVatRate()
        {
            try
            {
                if (string.IsNullOrEmpty(_txtAmount.Text))
                {
                    _txtAmount.Text = "0";
                }
                string selectedVATRatePerc = GetValueOnIndexFromArray(_ddlVATRate.SelectedItem.Value, 1);
                Decimal vatRatePerc = Convert.ToDecimal(selectedVATRatePerc.ToString());

                if (_ddlVAT.SelectedValue == "Yes")
                {
                    _txtVATAmount.Text = Convert.ToString(Math.Round((Convert.ToDecimal(_txtAmount.Text) / (vatRatePerc + 100)) * vatRatePerc, 2));
                }
                else
                {
                    _txtVATAmount.Text = Decimal.Zero.ToString();
                }
            }
            catch
            {
                _txtAmount.Text = "0.00";
            }
        }
        #endregion

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
                // Gets VAT Amount on VAT rate
                GetVATAmountOnVatRate();
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

        #region VATRate Selected Index Changed Event

        /// <summary>
        /// Sets vat amount for selected VAT Rate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlVATRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetVATAmountOnVatRate();
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

        #region Disbursement Selected Index Changed Event
        /// <summary>
        /// Sets payee and description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlDisbursementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDescriptionAndPayee();
        }
        #endregion

        #endregion

        #region Public Methods

        #region Client Matter Changed Event

        /// <summary>
        /// On matter changed event sets client bank id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    ViewState["ChequeRequestProjectId"] = new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString());
                    AccountsServiceClient accountsService = new AccountsServiceClient();
                    try
                    {
                        ChequeRequestReturnValue returnValue = accountsService.GetDefaultChequeRequestDetails(_logonSettings.LogonId, new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString()));

                        if (returnValue.Success)
                        {
                            _ddlBank.SelectedIndex = -1;
                            if (_ddlBank.Items.FindByValue(returnValue.ChequeRequest.BankOfficeId.ToString()) != null)
                            {
                                _ddlBank.Items.FindByValue(returnValue.ChequeRequest.BankOfficeId.ToString()).Selected = true;
                            }

                            _ddlVAT.SelectedIndex = -1;
                            if (returnValue.ChequeRequest.OfficeVATTable == IRIS.Law.PmsCommonData.Accounts.AccountsDataConstantsYesNo.Yes)
                            {
                                _ddlVAT.Items.FindByValue("Yes").Selected = true;
                            }
                            else
                            {
                                _ddlVAT.Items.FindByValue("No").Selected = true;
                            }

                            HideUnhideVATDetails();
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
                        if (accountsService != null)
                        {
                            if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                                accountsService.Close();
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

        #endregion

        #endregion

        #region Private Methods

        #region SetDefaults
        /// <summary>
        /// Sets default values for the controls on page load
        /// </summary>
        private void SetDefaults()
        {
            try
            {
                if (string.IsNullOrEmpty(_txtAmount.Text.ToString()))
                {
                    // Sets default VAT amount
                    _txtAmount.Text = "0.00";
                }

                if (string.IsNullOrEmpty(_txtVATAmount.Text.ToString()))
                {
                    // Sets default amount
                    _txtVATAmount.Text = "0.00";
                }

                // By default date should be today's date
                _ccPostDate.DateText = Convert.ToString(DateTime.Today);

                // Binds office banks
                BindOfficeBank();

                // Binds VAT rates.
                BindVATRates();

                // Binds disbursement types 
                BindDisbursementTypes();

                // Sets description and Payee
                SetDescriptionAndPayee();
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

        #region ResetControls

        /// <summary>
        /// Resets controls on page after adding client cheque request.
        /// </summary>
        private void ResetControls(bool IsSave)
        {
            try
            {
                _txtDescription.Text = string.Empty;
                _txtPayee.Text = string.Empty;
                _txtAmount.Text = "0.00";
                _updPanelAmount.Update();
                _txtReference.Text = "CHQ";

                if (!IsSave)
                   _ccPostDate.DateText = Convert.ToString(DateTime.Today);

                _ddlDisbursementType.SelectedIndex = 0;

                _ddlBank.SelectedIndex = 0;

                _chkBxAnticipated.Checked = false;
                _chkBxAuthorise.Checked = false;
                _chkBxPrintChequeRequest.Checked = false;
                
                HideUnhideVATDetails();

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

        #region BindOfficeBank

        /// <summary>
        /// Binds office banks on page load
        /// </summary>
        private void BindOfficeBank()
        {
            try
            {
                BankSearchItem[] banks = GetBanks(DataConstants.BankSearchTypes.Office);
                _ddlBank.DataSource = banks;
                _ddlBank.DataTextField = "Description";
                _ddlBank.DataValueField = "BankId";
                _ddlBank.DataBind();
                AddDefaultToDropDownList(_ddlBank);

                if (Session[SessionName.ProjectId] != null)
                {

                    AccountsServiceClient accountsService = new AccountsServiceClient();

                    ChequeRequestReturnValue returnValue = accountsService.GetDefaultChequeRequestDetails(_logonSettings.LogonId, new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString()));

                    if (returnValue.Success)
                    {
                        _ddlBank.SelectedIndex = -1;
                        if (_ddlBank.Items.FindByValue(returnValue.ChequeRequest.BankOfficeId.ToString()) != null)
                        {
                            _ddlBank.Items.FindByValue(returnValue.ChequeRequest.BankOfficeId.ToString()).Selected = true;
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

        #endregion

        #region BindDisbursementTypes

        /// <summary>
        /// Binds disbursement types
        /// </summary>
        private void BindDisbursementTypes()
        {
            _ddlDisbursementType.Items.Clear();
            AccountsServiceClient accountsClient = new AccountsServiceClient();
            try
            {
                DisbursmentTypeSearchCriteria criteria = new DisbursmentTypeSearchCriteria();
                criteria.IsExternal = true;
                criteria.IsIncludeArchived = false;

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                DisbursmentTypeReturnValue bankReturnValue = accountsClient.DisbursmentTypeSearch(_logonSettings.LogonId, collectionRequest, criteria);

                if (bankReturnValue.Success)
                {
                    if (bankReturnValue.DisbursementType != null)
                    {
                        _ddlDisbursementType.DataSource = bankReturnValue.DisbursementType.Rows;
                        _ddlDisbursementType.DataTextField = "Description";
                        _ddlDisbursementType.DataValueField = "Id";
                        _ddlDisbursementType.DataBind();
                    }
                }
                else
                {
                    _lblMessage.Text = bankReturnValue.Message;
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
                if (accountsClient.State != System.ServiceModel.CommunicationState.Faulted)
                    accountsClient.Close();
            }
        }

        #endregion

        #region BindVATRates

        /// <summary>
        /// Binds VAT rates on page load
        /// </summary>
        private void BindVATRates()
        {
            _ddlVATRate.Items.Clear();
            AccountsServiceClient vatRateClient = new AccountsServiceClient();
            try
            {
                VatRateSearchCriteria criteria = new VatRateSearchCriteria();
                criteria.IncludeNonVatable = false;
                criteria.IncludeArchived = false;

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                VatRateSearchReturnValue vatReturnValue = vatRateClient.VatRateSearch(_logonSettings.LogonId, collectionRequest, criteria);

                if (vatReturnValue.Success)
                {
                    if (vatReturnValue.VatRates != null)
                    {
                        for (int i = 0; i < vatReturnValue.VatRates.Rows.Length; i++)
                        {
                            ListItem item = new ListItem(vatReturnValue.VatRates.Rows[i].Description, vatReturnValue.VatRates.Rows[i].Id.ToString() + "$" + vatReturnValue.VatRates.Rows[i].Percentage.ToString());

                            if (vatReturnValue.VatRates.Rows[i].IsDefault)
                            {
                                _ddlVATRate.SelectedIndex = -1;
                                item.Selected = true;
                            }
                            _ddlVATRate.Items.Add(item);
                        }
                    }
                }
                else
                {
                    _lblMessage.Text = vatReturnValue.Message;
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
                if (vatRateClient.State != System.ServiceModel.CommunicationState.Faulted)
                    vatRateClient.Close();
            }
        }

        #endregion

        #region SetDescriptionAndPayee

        /// <summary>
        /// Sets description on selected index change and on page load
        /// </summary>
        private void SetDescriptionAndPayee()
        {
            try
            {
                if (_ddlDisbursementType.SelectedItem != null)
                {
                    _txtDescription.Text = _ddlDisbursementType.SelectedItem.Text;

                    Int32 idx = _txtDescription.Text.IndexOf(':');

                    if (idx != -1)
                    {
                        _txtPayee.Text = _txtDescription.Text.Substring(0, idx).Trim();
                    }
                    else
                    {
                        _txtPayee.Text = _txtDescription.Text.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        #endregion

        #region GetValueOnIndexFromArray

        /// <summary>
        /// If string is VAT Rate
        /// index = 0 -> VatId
        /// index = 1 -> VATRatePerc
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
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
                return "";
            }
        }

        #endregion

        protected void _ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
                       
            // Gets VAT Amount on VAT rate
            GetVATAmountOnVatRate();
        }

        protected void _ccPostDate_DateChanged(object sender, EventArgs e)
        {
            if (_ccPostDate.DateText == "")
            {
                _lblPostingPeriod.Text = "Invalid";
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = "Posting Date is not within the current financial year";

                return;
            }
           

            SetPostingPeriod();
        }

        

        #endregion
    }
}

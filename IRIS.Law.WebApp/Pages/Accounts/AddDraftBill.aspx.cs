using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Accounts;
using System.Configuration;
using System.Runtime.Serialization;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class AddDraftBill : BasePage
    {
        #region Private Data Members

        LogonReturnValue _logonSettings = null;
        decimal _totalCost;
        decimal _totalDisbs;
        decimal _totalDisbsVat;
        Dictionary<int, decimal> _vat = new Dictionary<int, decimal>();

        #endregion Private Data Members

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

                // Clear previous messages
                _lblMessage.Text = string.Empty;

                if (!IsPostBack)
                {
                    SetDefaults();

                    // Binds grids if project id is in session
                    if (Session[SessionName.ProjectId] != null)
                    {
                        Guid projectId = (Guid)Session[SessionName.ProjectId];
                        SetVat(projectId);
                        BindGrids();
                    }

                    // Validates the posting period on page load
                    SetPostingPeriod();
                }

                _txtTotalCost.Attributes.Add("readonly", "readonly");
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
                return;
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

        #endregion

        #region Grid View Events

        /// <summary>
        /// Row data bound event for grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdUnbilledPaidNonVatable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AddDraftBillJavascriptFunctions(e.Row);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Gets description from the grid view row.
                    Label lblUnBilledPaidNonVATableDescription = (Label)e.Row.FindControl("_lblDescription");

                    // Truncate large descriptions
                    if (lblUnBilledPaidNonVATableDescription.Text.Length > 20)
                    {
                        lblUnBilledPaidNonVATableDescription.Text = lblUnBilledPaidNonVATableDescription.Text.Substring(0, 20) + "...";
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
        /// Row data bound event for grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdUnbilledPaidVatable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AddDraftBillJavascriptFunctions(e.Row);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Gets description from the grid view row.
                    Label lblUnbilledPaidVATableDescription = (Label)e.Row.FindControl("_lblDescription");

                    // Truncate large descriptions
                    if (lblUnbilledPaidVATableDescription.Text.Length > 20)
                    {
                        lblUnbilledPaidVATableDescription.Text = lblUnbilledPaidVATableDescription.Text.Substring(0, 20) + "...";
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
        /// Row data bound event for grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdAnticipatedNonVatable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AddDraftBillJavascriptFunctions(e.Row);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Gets description from the grid view row.
                    Label lblAnticipatedNonVATableDescription = (Label)e.Row.FindControl("_lblDescription");

                    // Truncate large descriptions
                    if (lblAnticipatedNonVATableDescription.Text.Length > 20)
                    {
                        lblAnticipatedNonVATableDescription.Text = lblAnticipatedNonVATableDescription.Text.Substring(0, 20) + "...";
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
        /// Row data bound event for grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdAnticipatedVatable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                AddDraftBillJavascriptFunctions(e.Row);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Gets description from the grid view row.
                    Label lblAnticipatedVATableDescription = (Label)e.Row.FindControl("_lblDescription");

                    // Truncate large descriptions
                    if (lblAnticipatedVATableDescription.Text.Length > 20)
                    {
                        lblAnticipatedVATableDescription.Text = lblAnticipatedVATableDescription.Text.Substring(0, 20) + "...";
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
        /// Row data bound for grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdUnBilledTime_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    TimeTransaction timeTransaction = (TimeTransaction)e.Row.DataItem;
                    Control time = e.Row.FindControl("_lblTime");
                    if (time != null)
                    {
                        Label lblTime = (Label)time;
                        lblTime.Text = AppFunctions.ConvertUnits(timeTransaction.Time);
                    }

                    // Gets description from the grid view row.
                    Label lblUnBilledTimedescription = (Label)e.Row.FindControl("_lblDescription");

                    // Truncate large descriptions
                    if (lblUnBilledTimedescription.Text.Length > 20)
                    {
                        lblUnBilledTimedescription.Text = lblUnBilledTimedescription.Text.Substring(0, 20) + "...";
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

        #region Wizard Navigation Events

        /// <summary>
        /// Finish button event on wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnWizardStepFinishButton_Click(object sender, EventArgs e)
        {
            AccountsServiceClient accountsService = null;

            // Saves the draft bill
            if (Page.IsValid)
            {
                try
                {
                    DraftBill bill = GetControlData();

                    accountsService = new AccountsServiceClient();
                    DraftBillReturnValue returnValue = accountsService.AddDraftBill(_logonSettings.LogonId, bill);

                    if (returnValue.Success)
                    {
                        // if the page is valid it redirects to view page
                        Response.Redirect("ViewDraftBills.aspx", true);
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
                    return;
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

        /// <summary>
        /// Step next event on wizard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StepNextButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardAddDraftBill.ActiveStepIndex == 2)
                {
                    if (_ddlVATRate.Items.Count == 0)
                    {
                        BindVatRates();
                    }
                    CreateDraftBillPreview();

                    string script = "$(\"#{0}\").numeric(null,2);";
                    script += "$(\"#{1}\").numeric(null,2);";

                    ScriptManager.RegisterStartupScript(this, Page.GetType(), "numericTextboxes",
                                    string.Format(script, _txtCosts.ClientID, _txtVAT.ClientID), true);

                }
                else
                {
                    CalculateUnbilledAmount(_grdUnBilledPaidNonVatable);
                    CalculateUnbilledAmount(_grdUnbilledPaidVatable);
                    CalculateUnbilledAmount(_grdAnticipatedNonVatable);
                    CalculateUnbilledAmount(_grdAnticipatedVatable);
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
        /// Wizard's start button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnWizardStartNavCancel_Click(object sender, EventArgs e)
        {
            try
            {
                // Resets the controls                        
                _txtDraftBillDescription.Text = string.Empty;
                _wizardAddDraftBill.ActiveStepIndex = 0;
                _txtVatableNotes.Text = string.Empty;
                _txtNonVatableNotes.Text = string.Empty;
                _txtAnticipatedNonVatableNotes.Text = string.Empty;
                _txtAnticipatedVatableNotes.Text = string.Empty;
                _ccUnbilledTimeUpto.DateText = DateTime.Today.ToString("dd/MM/yyyy");
                _hdnBindDraftBillGrids.Value = "true";
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

        protected void StartNextButton_Click(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                // Sets error message if posting date is blank
                if (string.IsNullOrEmpty(_ccDraftBillDate.DateText))
                {
                    _lblPostingPeriod.Text = "Invalid";
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Posting Date is not within the current financial year";
                    e.Cancel = true;
                    return;
                }

                bool isPostingPeriodValid;
                if (ViewState[PERIOD_VALID] != null)
                {
                    isPostingPeriodValid = (bool)ViewState[PERIOD_VALID];
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                if (Page.IsValid && isPostingPeriodValid)
                {
                    if (_hdnBindDraftBillGrids.Value == "true")
                    {
                        BindGrids();
                        //Set to false so the grids wont be bound again if the user is 
                        //moves back and forth without changing the matter
                        _hdnBindDraftBillGrids.Value = "false";
                    }
                    RegisterNumericTextboxJavascript();
                }
                else
                {
                    e.Cancel = true;
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

        protected void StepPreviousButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_wizardAddDraftBill.ActiveStepIndex == 2)
                {
                    RegisterNumericTextboxJavascript();
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

        #region DropDownList Event

        /// <summary>
        /// Selected index changed event for VAT rate dropdownlist control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlVATRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetPaidDisbursements();
                GetUnpaidDisbursements();

                decimal disbVat = (decimal)ViewState["DisbVatTotal"];
                decimal disbsAmt = (decimal)ViewState["DisbTotal"];
                decimal vatRatePercent = Convert.ToDecimal(GetValueOnIndex(_ddlVATRate.SelectedValue, 1));
                decimal totalCost = Convert.ToDecimal(_txtCosts.Text);
                decimal vatAmount = Decimal.Round(totalCost * (vatRatePercent / 100), 2);

                _txtVAT.Text = vatAmount.ToString("F2");
                _txtTotalVAT.Text = (vatAmount + disbVat).ToString("F2");

                //grand total
                _txtTotal.Text = (totalCost + disbsAmt + disbVat + vatAmount).ToString("0.00");

                string script = "$(\"#{0}\").change(GetVATAmountOnVatRate);";
                script += "$(\"#{1}\").change(ReCalculateVat);";
                script += "$(\"#{0}\").numeric(null,2);";
                script += "$(\"#{1}\").numeric(null,2);";

                ScriptManager.RegisterStartupScript(this, Page.GetType(), "VatChange",
                                                    string.Format(script, _txtCosts.ClientID, _txtVAT.ClientID), true);
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

        #region Client Matter Changed Event

        /// <summary>
        /// Client matter user control matter changed event.
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
                    SetVat((Guid)Session[SessionName.ProjectId]);
                    _hdnBindDraftBillGrids.Value = "true";
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

        /// <summary>
        /// Event handler for the DateChanged event on the _ccUnbilledTimeUpto control
        /// </summary>
        /// <param name="sender">
        /// The _ccUnbilledTimeUpto instance
        /// </param>
        /// <param name="e">
        /// Event arguments
        /// </param>
        protected void CcUnbilledTimeUptoDateChanged(object sender, EventArgs e)
        {
            this.BindTimeTransactions();
        }
        #endregion Protected Methods

        #region CONSTANTS

        private const string PERIOD_VALID = "PostingPeriodValidity";

        #endregion

        #region Private Methods

        #region GetVatRate

        /// <summary>
        /// Gets the vat rate percentage for the given vat rate id.
        /// </summary>
        /// <param name="vatRateId">The vat rate id.</param>
        /// <returns></returns>
        private decimal GetVatRate(int vatRateId)
        {
            decimal vatRate = 0;
            try
            {
                foreach (ListItem item in _ddlVATRate.Items)
                {
                    if (vatRateId.ToString() == GetValueOnIndex(item.Value, 0))
                    {
                        vatRate = decimal.Parse(GetValueOnIndex(item.Value, 1));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return vatRate;
        }

        #endregion

        #region GetValueOnIndex

        /// <summary>
        /// index = 0 VatRateId
        /// index = 1 VatPercentage
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private string GetValueOnIndex(string text, int index)
        {
            try
            {
                string[] arrayBranch = text.Split('$');
                return arrayBranch[index];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SetPostingPeriod

        /// <summary>
        /// Validates posting period on page load and gets client banks
        /// </summary>
        /// <param name="bankType"></param>
        /// Previous private PeriodDetailsReturnValue ValidatePostingPeriod(Guid projectId, IRIS.Law.PmsCommonData.DataConstantsBankSearchTypes bankType)
        private PeriodDetailsReturnValue ValidatePostingPeriod(Guid projectId)
        {
            TimeServiceClient timeService = null;
            PeriodDetailsReturnValue returnValue = null;

            try
            {
                PeriodCriteria criteria = new PeriodCriteria();
                criteria.Date = Convert.ToDateTime(_ccDraftBillDate.DateText);
                criteria.IsPostingVATable = false;
                criteria.IsTime = false;
                criteria.ProjectId = projectId;
                // This parameter has been passed as "false" in accounts
                criteria.IsAllowedPostBack2ClosedYear = false;

                timeService = new TimeServiceClient();
                returnValue = timeService.ValidatePeriod(_logonSettings.LogonId, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
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
        /// Validates the posting period onpage load
        /// </summary>
        private void SetPostingPeriod()
        {
            try
            {
                PeriodDetailsReturnValue returnValue = ValidatePostingPeriod(DataConstants.DummyGuid);

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

                //Store the period status in the viewstate so that it can be checked before saving
                ViewState[PERIOD_VALID] = returnValue.Success;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BindGrids

        /// <summary>
        /// Binds grids on page load
        /// </summary>
        private void BindGrids()
        {
            try
            {
                // Binds all the grids
                BindNonVatableTransactions();
                BindVatableTransactions();
                BindAnticipatedNonVatableTransactions();
                BindAnticipatedVatableTransactions();
                BindTimeTransactions();
                SetActiveTabIndex();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AddDisbursementToTable

        /// <summary>
        /// Adds disbursement details 
        /// </summary>
        /// <param name="disbursements"></param>
        /// <param name="table"></param>
        private void AddDisbursementToTable(List<DisbursementLedgerTransaction> disbursements, Table table)
        {
            try
            {
                foreach (DisbursementLedgerTransaction disbursement in disbursements)
                {
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.CssClass = "boldTxt";
                    cell.Style.Add("width", "30%");
                    cell.Text = disbursement.Description;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Style.Add("width", "15%");
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.CssClass = "numericTextBox";
                    cell.Style.Add("width", "15%");
                    cell.Text = disbursement.Billed.ToString("F2");
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.CssClass = "numericTextBox";
                    cell.Style.Add("width", "15%");
                    cell.Text = _vat.ContainsKey(disbursement.PostingId) ? _vat[disbursement.PostingId].ToString("F2") : string.Empty;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Style.Add("width", "35%");
                    row.Cells.Add(cell);

                    table.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AddAnticipatedDisbursementToTable

        /// <summary>
        /// Adds anticipated disbursement details 
        /// </summary>
        /// <param name="disbursements"></param>
        /// <param name="table"></param>
        private void AddAnticipatedDisbursementToTable(List<AnticipatedDisbursementLedgerTransaction> disbursements, Table table)
        {
            try
            {
                foreach (AnticipatedDisbursementLedgerTransaction disbursement in disbursements)
                {
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.CssClass = "boldTxt";
                    cell.Style.Add("width", "30%");
                    cell.Text = "****" + disbursement.Description;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Style.Add("width", "15%");
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.CssClass = "numericTextBox";
                    cell.Style.Add("width", "15%");
                    cell.Text = disbursement.Billed.ToString("F2");
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.CssClass = "numericTextBox";
                    cell.Style.Add("width", "15%");
                    cell.Text = _vat.ContainsKey(disbursement.PostingId) ? _vat[disbursement.PostingId].ToString("F2") : string.Empty;
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.Style.Add("width", "35%");
                    row.Cells.Add(cell);

                    table.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CreateDraftBillPreview

        /// <summary>
        /// Creates the draft bill preview.
        /// </summary>
        private void CreateDraftBillPreview()
        {
            try
            {
                GetPaidDisbursements();
                GetUnpaidDisbursements();
                _totalCost = GetBilledTime();

                //Display To our professional chgs
                _txtCosts.Text = Decimal.Round(_totalCost, 2).ToString("0.00");
                decimal vatRatePercent = Convert.ToDecimal(GetValueOnIndex(_ddlVATRate.SelectedValue, 1));
                decimal vatAmount = Decimal.Round(_totalCost * vatRatePercent / 100, 2);
                _txtVAT.Text = Decimal.Round(vatAmount, 2).ToString("0.00");

                //Sub Totals
                _txtTotalCost.Text = Decimal.Round(_totalCost, 2).ToString("0.00");
                _txtTotalDisbursements.Text = Decimal.Round(_totalDisbs, 2).ToString("0.00");
                _txtTotalVAT.Text = Decimal.Round(_totalDisbsVat + vatAmount, 2).ToString("0.00");

                //Grand total
                _txtTotal.Text = (_totalCost + _totalDisbs + _totalDisbsVat + vatAmount).ToString("0.00");

                ViewState["DisbTotal"] = _totalDisbs;
                ViewState["DisbVatTotal"] = _totalDisbsVat;
                _hdnDisbTotal.Value = _totalDisbs.ToString();
                _hdnDisbVatTotal.Value = _totalDisbsVat.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CalculateUnbilledAmount

        /// <summary>
        /// Calculates the unbilled amount.
        /// </summary>
        /// <param name="grid">The grid.</param>
        private void CalculateUnbilledAmount(GridView grid)
        {
            try
            {
                foreach (GridViewRow row in grid.Rows)
                {
                    Control amountLabel = row.FindControl("_lblAmount");
                    Control billed = row.FindControl("_txtBilled");
                    Control unbilled = row.FindControl("_lblUnBilled");

                    if (amountLabel != null && billed != null && unbilled != null)
                    {
                        Label lblAmount = (Label)amountLabel;
                        TextBox txtBilled = (TextBox)billed;
                        Label lblUnbilled = (Label)unbilled;

                        decimal amt = Convert.ToDecimal(lblAmount.Text);
                        decimal billedAmt = Convert.ToDecimal(txtBilled.Text);
                        decimal unbilledAmt = amt - billedAmt;

                        lblUnbilled.Text = unbilledAmt.ToString("F2");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GetBilledTime

        /// <summary>
        /// Gets the total cost for selected time transactions.
        /// </summary>
        /// <returns></returns>
        private decimal GetBilledTime()
        {
            decimal profitCosts = 0;
            try
            {
                foreach (GridViewRow row in _grdUnBilledTime.Rows)
                {
                    Control chkbox = row.FindControl("_chkBxSelect");
                    if (chkbox != null)
                    {
                        if (((CheckBox)chkbox).Checked)
                        {
                            Control lblCharge = row.FindControl("_lblCharge");
                            if (lblCharge != null)
                            {
                                profitCosts += Convert.ToDecimal(((Label)lblCharge).Text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return profitCosts;
        }

        #endregion

        #region GetUnpaidDisbursements

        /// <summary>
        /// Gets the unpaid disbursements.
        /// </summary>
        private void GetUnpaidDisbursements()
        {
            try
            {
                List<AnticipatedDisbursementLedgerTransaction> upPaidDisbursements = new List<AnticipatedDisbursementLedgerTransaction>();
                GetUnpaidDisbursementsFromGrid(upPaidDisbursements, _grdAnticipatedNonVatable, false);
                GetUnpaidDisbursementsFromGrid(upPaidDisbursements, _grdAnticipatedVatable, true);

                AddAnticipatedDisbursementToTable(upPaidDisbursements, _tblUnPaidDisbursements);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GetPaidDisbursements

        /// <summary>
        /// Gets the paid disbursements.
        /// </summary>
        private void GetPaidDisbursements()
        {
            try
            {
                List<DisbursementLedgerTransaction> paidDisbursements = new List<DisbursementLedgerTransaction>();
                GetPaidDisbursementsFromGrid(paidDisbursements, _grdUnBilledPaidNonVatable, false);
                GetPaidDisbursementsFromGrid(paidDisbursements, _grdUnbilledPaidVatable, true);

                AddDisbursementToTable(paidDisbursements, _tblPaidDisbursements);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GetControlData

        /// <summary>
        /// Gets the control data.
        /// </summary>
        /// <returns></returns>
        private DraftBill GetControlData()
        {
            DraftBill bill = new DraftBill();

            try
            {
                if (_txtTotal.Text == "0.00")
                {
                    throw new Exception("Unable to create a zero draft bill with no disbs or time selected.");
                }

                bill.DraftBillDate = Convert.ToDateTime(_ccDraftBillDate.DateText);
                bill.ProfitCosts = Convert.ToDecimal(_txtCosts.Text.Trim());
                bill.VATAmount = Convert.ToDecimal(_txtTotalVAT.Text.Trim());
                bill.VATRateId = Convert.ToInt32(GetValueOnIndex(_ddlVATRate.SelectedValue, 0));

                if (string.IsNullOrEmpty(_ccUnbilledTimeUpto.DateText))
                {
                    bill.BilledTimeUpto = DataConstants.BlankDate;
                }
                else
                {
                    bill.BilledTimeUpto = Convert.ToDateTime(_ccUnbilledTimeUpto.DateText);
                }

                bill.ProjectId = (Guid)Session[SessionName.ProjectId];
                bill.PostingDescription = _txtDraftBillDescription.Text.Trim();
                bill.UnbilledPaidNonVATableNotes = _txtNonVatableNotes.Text.Trim();
                bill.UnbilledPaidVATableNotes = _txtVatableNotes.Text.Trim();
                bill.AntiNonVATableNotes = _txtAnticipatedNonVatableNotes.Text.Trim();
                bill.AntiVATableNotes = _txtAnticipatedVatableNotes.Text.Trim();
                bill.TimeTransactions = GetTimeTransactions().ToArray();

                List<DisbursementLedgerTransaction> transactions = new List<DisbursementLedgerTransaction>();
                GetPaidDisbursementsFromGrid(transactions, _grdUnBilledPaidNonVatable, false);
                bill.UnBilledPaidNonVatableList = transactions.ToArray();
                transactions.Clear();

                GetPaidDisbursementsFromGrid(transactions, _grdUnbilledPaidVatable, true);
                bill.UnBilledPaidVatableList = transactions.ToArray();
                transactions.Clear();

                List<AnticipatedDisbursementLedgerTransaction> antiTransactions = new List<AnticipatedDisbursementLedgerTransaction>();
                GetUnpaidDisbursementsFromGrid(antiTransactions, _grdAnticipatedNonVatable, false);
                bill.AnticipatedDisbursementNonVatableList = antiTransactions.ToArray();
                antiTransactions.Clear();

                GetUnpaidDisbursementsFromGrid(antiTransactions, _grdAnticipatedVatable, true);
                bill.AnticipatedDisbursementVatableList = antiTransactions.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bill;
        }

        #endregion

        #region GetTimeTransactions

        /// <summary>
        /// Gets the time transactions.
        /// </summary>
        /// <returns></returns>
        private List<TimeTransaction> GetTimeTransactions()
        {
            List<TimeTransaction> time = new List<TimeTransaction>();
            try
            {
                foreach (GridViewRow row in _grdUnBilledTime.Rows)
                {
                    Control timeId = row.FindControl("_hdnTimeId");
                    Control chkbox = row.FindControl("_chkBxSelect");
                    if (timeId != null && chkbox != null)
                    {
                        HiddenField hdnTimeId = (HiddenField)timeId;
                        TimeTransaction trans = new TimeTransaction();
                        trans.TimeId = Convert.ToInt32(hdnTimeId.Value);
                        trans.Bill = ((CheckBox)chkbox).Checked;
                        time.Add(trans);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return time;
        }

        #endregion

        #region SetDefaults

        /// <summary>
        /// Sets controls on page load with default values.
        /// </summary>
        private void SetDefaults()
        {
            try
            {
                // By default date should be today's date
                _ccDraftBillDate.DateText = Convert.ToString(DateTime.Today);
                _ccUnbilledTimeUpto.DateText = Convert.ToString(DateTime.Today);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BindVATRates

        /// <summary>
        /// Binds VAT rates
        /// </summary>
        private void BindVatRates()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();

                VatRateSearchCriteria criteria = new VatRateSearchCriteria();
                criteria.IncludeArchived = false;
                criteria.IncludeNonVatable = false;

                accountsService = new AccountsServiceClient();

                VatRateSearchReturnValue vatReturnValue = accountsService.VatRateSearch(_logonSettings.LogonId,
                                                            collectionRequest, criteria);

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
                    throw new Exception(vatReturnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        #region GetPaidDisbursementsFromGrid

        /// <summary>
        /// Gets the paid items from grid.
        /// </summary>
        /// <param name="disbursements">The disbursement list into which the items are added.</param>
        /// <param name="gridview">The gridview from which to fetch the billed amounts.</param>
        /// <param name="isVatable">if set to <c>true</c> is vatable.</param>
        private void GetPaidDisbursementsFromGrid(List<DisbursementLedgerTransaction> disbursements, GridView gridview, bool isVatable)
        {
            try
            {
                foreach (GridViewRow row in gridview.Rows)
                {
                    Control description = row.FindControl("_lblDescription");
                    Control billedAmt = row.FindControl("_txtBilled");
                    Control postingId = row.FindControl("_hdnPostingId");
                    Control billDisbursementAllocationAmt = row.FindControl("_hdnBillDisbursementAllocationAmount");
                    HiddenField billDisbursement = (HiddenField)billDisbursementAllocationAmt;

                    if (description != null && billedAmt != null && postingId != null
                        && billDisbursementAllocationAmt != null)
                    {
                        Label lblDescription = (Label)description;
                        TextBox txtBilledAmt = (TextBox)billedAmt;
                        HiddenField hdnPostingID = (HiddenField)postingId;

                        decimal amount = 0;
                        decimal disbAllocAmt = decimal.Parse(billDisbursement.Value);

                        if (decimal.TryParse(txtBilledAmt.Text, out amount))
                        {
                            //Disb allocation amount is checked for PaidDisbursements but not required for anticipated disbs
                            if (amount != disbAllocAmt)
                            {
                                amount = Decimal.Round(amount - disbAllocAmt, 2);
                                DisbursementLedgerTransaction disbursement = new DisbursementLedgerTransaction();
                                disbursement.Description = lblDescription.Text;
                                disbursement.Billed = amount;
                                disbursement.PostingId = Convert.ToInt32(hdnPostingID.Value);

                                _totalDisbs += amount;
                                if (isVatable)
                                {
                                    HiddenField vatRate = (HiddenField)row.FindControl("_hdnVatRateId");
                                    int vatRateId = Convert.ToInt32(vatRate.Value);
                                    decimal vatRatePercent = GetVatRate(vatRateId);
                                    decimal vatAmount = Decimal.Round(amount * vatRatePercent / 100, 2);
                                    _vat.Add(disbursement.PostingId, vatAmount);
                                    _totalDisbsVat += vatAmount;
                                }
                                disbursements.Add(disbursement);
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

        #region GetUnpaidDisbursementsFromGrid

        /// <summary>
        /// Gets the paid items from grid.
        /// </summary>
        /// <param name="disbursements">The disbursement list into which the items are added.</param>
        /// <param name="gridview">The gridview from which to fetch the billed amounts.</param>
        /// <param name="isVatable">if set to <c>true</c> is vatable.</param>
        private void GetUnpaidDisbursementsFromGrid(List<AnticipatedDisbursementLedgerTransaction> disbursements, GridView gridview, bool isVatable)
        {
            try
            {
                foreach (GridViewRow row in gridview.Rows)
                {
                    Control description = row.FindControl("_lblDescription");
                    Control billedAmt = row.FindControl("_txtBilled");
                    Control postingId = row.FindControl("_hdnPostingId");

                    if (description != null && billedAmt != null && postingId != null)
                    {
                        Label lblDescription = (Label)description;
                        TextBox txtBilledAmt = (TextBox)billedAmt;
                        HiddenField hdnPostingID = (HiddenField)postingId;
                        decimal amount = 0;

                        if (decimal.TryParse(txtBilledAmt.Text, out amount))
                        {
                            //Disb allocation amount is checked for PaidDisbursements but not required for anticipated disbs
                            if (amount > 0)
                            {
                                AnticipatedDisbursementLedgerTransaction disbursement = new AnticipatedDisbursementLedgerTransaction();
                                disbursement.Description = lblDescription.Text;
                                disbursement.Billed = amount;
                                disbursement.PostingId = Convert.ToInt32(hdnPostingID.Value);

                                _totalDisbs += amount;
                                if (isVatable)
                                {
                                    HiddenField vatRate = (HiddenField)row.FindControl("_hdnVatRateId");
                                    int vatRateId = Convert.ToInt32(vatRate.Value);
                                    decimal vatRatePercent = GetVatRate(vatRateId);
                                    decimal vatAmount = Decimal.Round(amount * vatRatePercent / 100, 2);
                                    _vat.Add(disbursement.PostingId, vatAmount);
                                    _totalDisbsVat += vatAmount;
                                }
                                disbursements.Add(disbursement);
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

        #region SetVat

        /// <summary>
        /// Sets the vat based on the branch for the project.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        private void SetVat(Guid projectId)
        {
            AccountsServiceClient accountsService = null;
            try
            {
                accountsService = new AccountsServiceClient();
                BranchVatReturnValue returnValue = accountsService.GetBranchVatForProject(_logonSettings.LogonId, projectId);

                if (returnValue.Success)
                {
                    if (returnValue.BranchNoVat)
                    {
                        _ddlVATRate.Visible = false;
                        _txtVAT.Visible = false;
                        _txtVAT.Text = "0.00";
                    }
                    else
                    {
                        _ddlVATRate.Visible = true;
                        _txtVAT.Visible = true;
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
                if (accountsService != null)
                {
                    if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountsService.Close();
                }
            }
        }

        #endregion

        #region RegisterNumericTextboxJavascript

        /// <summary>
        /// Registers the numeric textbox javascript.
        /// </summary>
        private void RegisterNumericTextboxJavascript()
        {
            try
            {
                //Add numeric textbox behaviour for currency fields
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "NumericTextbox",
                                                    "$(\".numericTextBox\").numeric(null, 2, false);", true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AddDraftBillJavascriptFunctions

        /// <summary>
        /// Adds the draft bill javascript functions to calculate the billed and unbilled amounts
        /// when the billed amount changes or the bill link is clicked.
        /// </summary>
        /// <param name="gridRow">The grid row.</param>
        private void AddDraftBillJavascriptFunctions(GridViewRow gridRow)
        {
            try
            {
                Control txtBilled = gridRow.FindControl("_txtBilled");
                Control lblUnbilled = gridRow.FindControl("_lblUnBilled");
                Control lblAmount = gridRow.FindControl("_lblAmount");
                Control lnkBtnBill = gridRow.FindControl("_lnkBtnBill");

                if (lblAmount != null && txtBilled != null && lblUnbilled != null && lnkBtnBill != null)
                {
                    TextBox billedAmount = (TextBox)txtBilled;
                    billedAmount.Attributes.Add("onchange", string.Format("CalcUnbilled('{0}','{1}','{2}');",
                                                                lblAmount.ClientID,
                                                                txtBilled.ClientID,
                                                                lblUnbilled.ClientID));

                    LinkButton lnkBill = (LinkButton)lnkBtnBill;
                    lnkBill.Attributes.Add("onclick", string.Format("return Billed('{0}','{1}','{2}');",
                                                                lblAmount.ClientID,
                                                                txtBilled.ClientID,
                                                                lblUnbilled.ClientID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SetActiveTabIndex

        /// <summary>
        /// Selects the tab where the grid has data as the active tab
        /// </summary>
        private void SetActiveTabIndex()
        {
            try
            {
                if (_grdUnBilledPaidNonVatable.Rows.Count > 0)
                {
                    _tcDraftBillDisbursements.ActiveTabIndex = 0;
                }
                else if (_grdUnbilledPaidVatable.Rows.Count > 0)
                {
                    _tcDraftBillDisbursements.ActiveTabIndex = 1;
                }
                else if (_grdAnticipatedNonVatable.Rows.Count > 0)
                {
                    _tcDraftBillDisbursements.ActiveTabIndex = 2;
                }
                else if (_grdAnticipatedVatable.Rows.Count > 0)
                {
                    _tcDraftBillDisbursements.ActiveTabIndex = 3;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Grid Data

        private void BindVatableTransactions()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                accountsService = new AccountsServiceClient();

                Guid projectId = (Guid)Session[SessionName.ProjectId];
                DisbursementLedgerReturnValue returnValue = accountsService.GetDisbursementLedgerVatableTransaction(logonId, projectId);

                if (returnValue.Success)
                {
                    _grdUnbilledPaidVatable.DataSource = returnValue.Transactions.Rows;
                    _grdUnbilledPaidVatable.DataBind();
                }
                else
                {
                    _grdUnbilledPaidVatable.DataSource = null;
                    _grdUnbilledPaidVatable.DataBind();
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void BindNonVatableTransactions()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                accountsService = new AccountsServiceClient();

                Guid projectId = (Guid)Session[SessionName.ProjectId];
                DisbursementLedgerReturnValue returnValue = accountsService.GetDisbursementLedgerNonVatableTransaction(logonId, projectId);

                if (returnValue.Success)
                {
                    _grdUnBilledPaidNonVatable.DataSource = returnValue.Transactions.Rows;
                    _grdUnBilledPaidNonVatable.DataBind();
                }
                else
                {
                    _grdUnBilledPaidNonVatable.DataSource = null;
                    _grdUnBilledPaidNonVatable.DataBind();
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void BindAnticipatedVatableTransactions()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                accountsService = new AccountsServiceClient();

                Guid projectId = (Guid)Session[SessionName.ProjectId];
                AnticipatedDisbursementLedgerReturnValue returnValue = accountsService.GetAnticipatedDisbursementLedgerVatableTransaction(logonId, projectId);

                if (returnValue.Success)
                {
                    _grdAnticipatedVatable.DataSource = returnValue.Transactions.Rows;
                    _grdAnticipatedVatable.DataBind();
                }
                else
                {
                    _grdAnticipatedVatable.DataSource = null;
                    _grdAnticipatedVatable.DataBind();
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        private void BindAnticipatedNonVatableTransactions()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                accountsService = new AccountsServiceClient();

                Guid projectId = (Guid)Session[SessionName.ProjectId];
                AnticipatedDisbursementLedgerReturnValue returnValue = accountsService.GetAnticipatedDisbursementLedgerNonVatableTransaction(logonId, projectId);

                if (returnValue.Success)
                {
                    _grdAnticipatedNonVatable.DataSource = returnValue.Transactions.Rows;
                    _grdAnticipatedNonVatable.DataBind();
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
                if (accountsService != null)
                {
                    if (accountsService.State != System.ServiceModel.CommunicationState.Faulted)
                        accountsService.Close();
                }
            }
        }

        private void BindTimeTransactions()
        {
            AccountsServiceClient accountsService = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                accountsService = new AccountsServiceClient();

                Guid projectId = (Guid)Session[SessionName.ProjectId];
                DateTime dateTo;

                if (!DateTime.TryParse(this._ccUnbilledTimeUpto.DateText, out dateTo))
                {
                    throw new Exception("Unable to convert unbilled time up to value to a date");
                }

                TimeTransactionReturnValue returnValue = accountsService.GetTimeTransaction(logonId, projectId);

                if (returnValue.Success)
                {
                    this._grdUnBilledTime.DataSource = returnValue.Transactions.Rows.Where(transaction => transaction.Date <= dateTo);
                    _grdUnBilledTime.DataBind();
                }
                else
                {
                    _grdUnBilledTime.DataSource = null;
                    _grdUnBilledTime.DataBind();
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        #region TextChangedEvent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _txtCosts_TextChanged(object sender, EventArgs e)
        {
            try
            {
                GetPaidDisbursements();
                GetUnpaidDisbursements();

                decimal disbVat = (decimal)ViewState["DisbVatTotal"];
                decimal disbsAmt = (decimal)ViewState["DisbTotal"];
                decimal vatRatePercent = Convert.ToDecimal(GetValueOnIndex(_ddlVATRate.SelectedValue, 1));
                decimal totalCost = Convert.ToDecimal(_txtCosts.Text);
                decimal vatAmount = Decimal.Round(totalCost * (vatRatePercent / 100), 2);

                _txtVAT.Text = vatAmount.ToString("F2");
                _txtTotalVAT.Text = (vatAmount + disbVat).ToString("F2");

                //grand total
                _txtTotal.Text = (totalCost + disbsAmt + disbVat + vatAmount).ToString("0.00");

                string script = "$(\"#{0}\").change(GetVATAmountOnVatRate);";
                script += "$(\"#{1}\").change(ReCalculateVat);";
                script += "$(\"#{0}\").numeric(null,2);";
                script += "$(\"#{1}\").numeric(null,2);";

                ScriptManager.RegisterStartupScript(this, Page.GetType(), "VatChange",
                                                    string.Format(script, _txtCosts.ClientID, _txtVAT.ClientID), true);
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

        protected void _ccDraftBillDate_DateChanged(object sender, EventArgs e)
        {
            if (_ccDraftBillDate.DateText == "")
            {
                _lblPostingPeriod.Text = "Invalid";
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = "Posting Date is not within the current financial year";

                return;
            }

            SetPostingPeriod();
        }
    }
}

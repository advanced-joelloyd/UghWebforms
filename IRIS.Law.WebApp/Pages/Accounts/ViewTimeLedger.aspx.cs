using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;
using System.Configuration;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewTimeLedger : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _allTimeTransactionsRowCount;
        int _writeOffTimeTransactionsRowCount;
        int _writeOffReversalTimeTransactionsRowCount;

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

                if (!IsPostBack)
                {
                    // Binds time status
                    BindTimeStatus();

                    // If project id is in session then binds deposit details by project id on page load
                    if (Session[SessionName.ProjectId] != null)
                    {
                        // Sets financial information about client, office and deposit balances.
                        SetFinancialInfo();
                        BindTimeTransactions();
                        SetTotals();
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

        #region Grid View Events

        /// <summary>
        /// Row data bound event to truncate the description and reference after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdWriteOffTimeTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TimeLedgerSearchItem writeOffReversalTimeTransaction = (TimeLedgerSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (writeOffReversalTimeTransaction.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = writeOffReversalTimeTransaction.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large description
                if (writeOffReversalTimeTransaction.PostingDescription.Length > 20)
                {
                    Label lblDescription = (Label)e.Row.FindControl("_lblDescription");
                    lblDescription.Text = writeOffReversalTimeTransaction.PostingDescription.Substring(0, 20) + "...";
                }
            }
        }

        /// <summary>
        /// Row data bound event to truncate the description and reference after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdWriteOffReversalTimeTransactions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TimeLedgerSearchItem writeOffReversalTimeTransaction = (TimeLedgerSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (writeOffReversalTimeTransaction.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = writeOffReversalTimeTransaction.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large description
                if (writeOffReversalTimeTransaction.PostingDescription.Length > 20)
                {
                    Label lblDescription = (Label)e.Row.FindControl("_lblDescription");
                    lblDescription.Text = writeOffReversalTimeTransaction.PostingDescription.Substring(0, 20) + "...";
                }
            }
        }

        #endregion

        #region Time Status Filter Dropdownlist Events

        /// <summary>
        /// Changes the time status text on screen for selected time status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlTimeStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session[SessionName.ProjectId] != null)
            {
                // Sets financial information about client, office and deposit balances.
                SetFinancialInfo();

                _hdnTimeTransactionsRefresh.Value = "true";

                _grdTimeTransactions.PageIndex = 0;
                _grdTimeTransactions.DataSourceID = _odsTimeTransactions.ID;

                _lblTimeStatus.Text = _ddlTimeStatus.SelectedItem.Text;
                SetTotals();
            }
        }

        #endregion

        #region Object Data Source Events

        /// <summary>
        /// Record selection event of time transactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsTimeTransaction_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding time transaction grid
            if (e.Exception != null)
            {
                _lblMessage.CssClass = "errorMessage";
                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                else
                    _lblMessage.Text = e.Exception.InnerException.Message;
                e.ExceptionHandled = true;
            }

            SetTotals();

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnTimeTransactionsRefresh.Value = "false";
        }

        /// <summary>
        /// Record selection event of time transactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsWriteOffTimeTransaction_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding writeoff time transaction grid
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
            _hdnWriteOffTimeTransactionsRefresh.Value = "false";
        }

        /// <summary>
        /// Record selection event of time transactions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsWriteOffReversalTimeTransaction_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding writeoff reversal time transaction grid
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
            _hdnWriteOffReversalTimeTransactionsRefresh.Value = "false";
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets time transactions rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get time transactions from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of time transactions</returns>
        public int GetAllTimeTransactionsRowsCount(bool forceRefresh, string timeFilter)
        {
            return _allTimeTransactionsRowCount;
        }

        /// <summary>
        /// Gets write off time transactions rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get write off time transactions from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of write off time transactions.</returns>
        public int GetWriteOffTimeTransactionsRowsCount(bool forceRefresh)
        {
            return _writeOffTimeTransactionsRowCount;
        }

        /// <summary>
        /// Gets write off reversal time transactions rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get write off reversal time transactions from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of write off reversal time transactions.</returns>
        public int GetWriteOffReversalTimeTransactionsRowsCount(bool forceRefresh)
        {
            return _writeOffReversalTimeTransactionsRowCount;
        }

        /// <summary>
        /// Binds time transactions ledger by changed project id
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
                    // On matter changed, time status dropdownlist should be set 
                    // for "All" to filter out all the time transactions
                    _ddlTimeStatus.SelectedIndex = -1;

                    // On matter changed, totals for time,cost and charge needs 
                    // to be set with default values
                    SetDefaultTotals();
                    SetDefaultValues();

                    // On matter changed, sessions containing totals for cost,time and charge
                    // needs to be set to "null" for getting another matter values
                    ResetSessions();

                    // Binds time transactions to grid view
                    BindTimeTransactions();

                    // Sets financial information for office,client and deposit balances
                    SetFinancialInfo();

                    // Sets label on selected item on dropdownlist control
                    _lblTimeStatus.Text = _ddlTimeStatus.SelectedItem.Text;
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
        /// Loads time transactions.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns time ledger.</returns>
        public TimeLedgerSearchItem[] LoadTimeLedger(int startRow, int pageSize, bool forceRefresh, string timeFilter)
        {
            AccountsServiceClient accountsService = null;
            TimeLedgerSearchItem[] allTimeTransactions = null;

            try
            {
                if (Session[SessionName.ProjectId] != null)
                {
                    accountsService = new AccountsServiceClient();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    Guid projectId = (Guid)Session[SessionName.ProjectId];
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    TimeLedgerSearchReturnValue returnValue = accountsService.GetTimeLedger(logonId, collectionRequest, timeFilter, projectId);

                    if (returnValue.Success)
                    {
                        _allTimeTransactionsRowCount = returnValue.TimeTransactions.TotalRowCount;
                        allTimeTransactions = returnValue.TimeTransactions.Rows;

                        if (returnValue.TotalTimeTransactions != null)
                        {
                            // If filter is "All", only then totals for time,cost and charge will be set in session
                            if (timeFilter == "All")
                            {
                                Session[SessionName.TotalTime] = returnValue.TotalTimeTransactions.TotalTimeElapsed;
                                Session[SessionName.TotalCost] = returnValue.TotalTimeTransactions.TotalCost.ToString("0.00");
                                Session[SessionName.TotalCharge] = returnValue.TotalTimeTransactions.TotalCharge.ToString("0.00");
                            }

                            Session["FilterCost"] = returnValue.TotalTimeTransactions.FilterCost.ToString();
                            Session["FilterCharge"] = returnValue.TotalTimeTransactions.FilterCharge.ToString("0.00");
                            Session["FilterTime"] = Convert.ToString(returnValue.TotalTimeTransactions.FilterTime);
                        }

                        // If the time filter is not "All", and also there are no rows to bind on grid view, 
                        // the set sessions to "null"
                        if (_allTimeTransactionsRowCount == 0 &&
                            timeFilter != "All")
                        {
                            Session["FilterCost"] = null;
                            Session["FilterCharge"] = null;
                            Session["FilterTime"] = null;
                        }
                    }
                    else
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = returnValue.Message;
                    }
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

            return allTimeTransactions;
        }

        /// <summary>
        /// Loads write off time transactions.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns write off time transactions.</returns>
        public TimeLedgerSearchItem[] LoadWriteOffTimeTransactions(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            TimeLedgerSearchItem[] writeOffTimeTransactions = null;

            try
            {
                if (Session[SessionName.ProjectId] != null)
                {
                    accountsService = new AccountsServiceClient();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    Guid projectId = (Guid)Session[SessionName.ProjectId];
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    TimeLedgerSearchReturnValue returnValue = accountsService.GetWriteOffTimeTransactions(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _writeOffTimeTransactionsRowCount = returnValue.TimeTransactions.TotalRowCount;
                        writeOffTimeTransactions = returnValue.TimeTransactions.Rows;
                    }
                    else
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = returnValue.Message;
                    }
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

            return writeOffTimeTransactions;
        }

        /// <summary>
        /// Loads write off reversal time transactions.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns write off reversal time transactions.</returns>
        public TimeLedgerSearchItem[] LoadWriteOffReversalTimeTransactions(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            TimeLedgerSearchItem[] writeOffReversalTimeTransactions = null;

            try
            {
                if (Session[SessionName.ProjectId] != null)
                {
                    accountsService = new AccountsServiceClient();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    Guid projectId = (Guid)Session[SessionName.ProjectId];
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    TimeLedgerSearchReturnValue returnValue = accountsService.GetWriteOffReversalTimeTransactions(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _writeOffReversalTimeTransactionsRowCount = returnValue.TimeTransactions.TotalRowCount;
                        writeOffReversalTimeTransactions = returnValue.TimeTransactions.Rows;
                    }
                    else
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = returnValue.Message;
                    }
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

            return writeOffReversalTimeTransactions;
        }

        #endregion

        #region Private Methods

        #region BindTimeStatus

        /// <summary>
        /// Binds time status.
        /// <remarks>Time status is required only to filter out time transactions.</remarks>
        /// </summary>
        private void BindTimeStatus()
        {
            try
            {
                DataTable dtTimeStatus = DataTables.GetTimeStatus();

                _ddlTimeStatus.DataSource = dtTimeStatus;
                _ddlTimeStatus.DataTextField = "TimeStatus";
                _ddlTimeStatus.DataValueField = "TimeStatus";
                _ddlTimeStatus.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region BindTimeTransactions

        /// <summary>
        /// Binds time transactions to grid view control
        /// </summary>
        private void BindTimeTransactions()
        {
            try
            {
                _hdnTimeTransactionsRefresh.Value = "true";
                _hdnWriteOffTimeTransactionsRefresh.Value = "true";
                _hdnWriteOffReversalTimeTransactionsRefresh.Value = "true";

                _grdTimeTransactions.PageIndex = 0;
                _grdTimeTransactions.DataSourceID = _odsTimeTransactions.ID;

                _grdWriteOffTimeTransactions.PageIndex = 0;
                _grdWriteOffTimeTransactions.DataSourceID = _odsWriteOffTimeTransactions.ID;

                _grdWriteOffReversalTimeTransactions.PageIndex = 0;
                _grdWriteOffReversalTimeTransactions.DataSourceID = _odsWriteOffReversalTimeTransactions.ID;

                int rowCount = _grdTimeTransactions.Rows.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SetFinancialInfo

        /// <summary>
        /// Loads financial information about office, client and deposit 
        /// balances by project id
        /// </summary>        
        private void SetFinancialInfo()
        {
            AccountsServiceClient accountsService = null;

            try
            {
                accountsService = new AccountsServiceClient();
                Guid projectId = (Guid)Session[SessionName.ProjectId];
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                FinancialInfoReturnValue returnValue = accountsService.GetFinancialInfoByProjectId(logonId, projectId);

                if (returnValue.Success)
                {
                    _txtOffice.Text = returnValue.OfficeBalance;
                    _txtClient.Text = returnValue.ClientBalance;
                    _txtDeposit.Text = returnValue.DepositBalance;

                    // Sets warning message if paid disbursements amount less than zero
                    if (!string.IsNullOrEmpty(returnValue.WarningMessage))
                    {
                        _lblMessage.Text = returnValue.WarningMessage;
                    }
                }
                else
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = returnValue.Message;
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

        #region SetTotals

        /// <summary>
        /// Sets calculated fields on time ledger for time transactions.
        /// </summary>
        private void SetTotals()
        {
            try
            {
                // Total time,cost and charge should be kept in session till matter is not changed.
                if (Session[SessionName.TotalTime] != null ||
                        Session[SessionName.TotalCost] != null ||
                        Session[SessionName.TotalCharge] != null)
                {
                    _txtTotalCost.Text = Convert.ToString(Session[SessionName.TotalCost]);
                    _txtTotalTime.Text = Convert.ToString(Session[SessionName.TotalTime]);
                    _txtTotalBalance.Text = Convert.ToString(Session[SessionName.TotalCharge]);
                }

                if (Session["FilterCost"] != null ||
                    Session["FilterCharge"] != null ||
                    Session["FilterTime"] != null)
                {
                    _txtTimeStatusCost.Text = Convert.ToString(Session["FilterCost"]);
                    _txtTimeStatusBalance.Text = Convert.ToString(Session["FilterCharge"]);
                    _txtTimeStatusTime.Text = Convert.ToString(Session["FilterTime"]);
                }
                else
                {
                    // if sessions for time,cost and charge are null, 
                    // then resets values to default values
                    SetDefaultValues();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Set defaults values for calculated fields

        /// <summary>
        /// Sets total cost,time and balance to "0.00"
        /// </summary>
        private void SetDefaultTotals()
        {
            _txtTotalCost.Text = "0.00";
            _txtTotalTime.Text = "0.00";
            _txtTotalBalance.Text = "0.00";
        }

        /// <summary>
        /// Sets default values for time,cost and balance if no matter is selected.
        /// </summary>
        private void SetDefaultValues()
        {
            _txtTimeStatusCost.Text = "0.00";
            _txtTimeStatusBalance.Text = "0.00";
            _txtTimeStatusTime.Text = "0.00";
        }

        /// <summary>
        /// Resets all the sessions for getting new total amounts in 
        /// time transactions for new matter.
        /// </summary>
        private void ResetSessions()
        {
            Session[SessionName.TotalTime] = null;
            Session[SessionName.TotalCost] = null;
            Session[SessionName.TotalCharge] = null;
            Session["FilterCost"] = null;
            Session["FilterCharge"] = null;
            Session["FilterTime"] = null;
        }

        #endregion

        #endregion
    }
}

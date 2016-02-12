using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewBillsLedger : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _allBillsRowCount;
        int _unclearedBillsRowCount;
        int _writeOffBillsRowCount;

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

                switch (_logonSettings.UserType)
                {
                    //staff
                    case 1:
                        _pnlAllBills.HeaderText = "All Bills";
                        _pnlUnclearedBills.HeaderText = "Uncleared Bills";
                        _pnlWriteOffs.HeaderText = "Write Offs";
                        break;
                    //client
                    case 2:
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["HideFinancialInfo"]))
                        {
                            _updPanelFinancialInfo.Visible = false;
                        }
                        else
                        {
                            _pnlAllBills.HeaderText = "Bills";
                            _pnlUnclearedBills.HeaderText = "Unpaid Bills";
                            _pnlWriteOffs.HeaderText = "Written Off Bills";
                        }

                        break;
                    //third party.
                    case 3:
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["HideFinancialInfo"]))
                        {
                            _updPanelFinancialInfo.Visible = false;
                        }
                        else
                        {
                            _pnlAllBills.HeaderText = "All Bills";
                            _pnlUnclearedBills.HeaderText = "Uncleared Bills";
                            _pnlWriteOffs.HeaderText = "Write Offs";
                        }
                        break;
                }

                if (!IsPostBack)
                {
                    // If project id is in session then binds deposit details by project id on page load
                    if (Session[SessionName.ProjectId] != null)
                    {
                        // Sets financial information about client, office and deposit balances.
                        GetFinancialInfo();
                        BindBillsLedger();
                    }
                    else
                    {
                        if (Session[SessionName.ProjectId] != null)
                        {
                            if (new Guid(Session[SessionName.MemberId].ToString()) != DataConstants.DummyGuid)
                            {
                                _cliMatDetails.IsClientMember = true;
                            }


                            _cliMatDetails.LoadClientMatterDetails();

                            // Sets financial information about client, office and deposit balances.
                            GetFinancialInfo();
                            BindBillsLedger();
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

        #region Grid View Events

        /// <summary>
        /// Row data bound event to truncate reference after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdAllBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BillsLedgerSearchItem billsLedger = (BillsLedgerSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (billsLedger.BillReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = billsLedger.BillReference.Substring(0, 20) + "...";
                }
            }
        }

        /// <summary>
        /// Row data bound event to truncate reference after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdUnclearedBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BillsLedgerSearchItem billsLedger = (BillsLedgerSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (billsLedger.BillReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = billsLedger.BillReference.Substring(0, 20) + "...";
                }
            }
        }

        /// <summary>
        /// Row data bound event to truncate reference after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdWriteOffs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BillsLedgerSearchItem billsLedger = (BillsLedgerSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (billsLedger.BillReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = billsLedger.BillReference.Substring(0, 20) + "...";
                }
            }
        }

        #endregion

        #region Object Data Source Events

        /// <summary>
        /// Record selection event of bills ledger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsAllBills_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
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
            _hdnAllBillsRefresh.Value = "false";
        }

        /// <summary>
        /// Record selection event of uncleared bills 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsUnclearedBills_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
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
            _hdnUnclearedBillsRefresh.Value = "false";
        }

        /// <summary>
        /// Record selection event of write off bills ledger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsWriteOffBills_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
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
            _hdnWriteOffBillsRefresh.Value = "false";
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds bills ledger by project id
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
                    // Binds bills ledger on matter changed
                    BindBillsLedger();
                    GetFinancialInfo();
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
        /// Gets the bills rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get bills from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of bills</returns>
        public int GetAllBillsRowsCount(bool forceRefresh)
        {
            return _allBillsRowCount;
        }

        /// <summary>
        /// Gets the bills rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get bills from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of bills</returns>
        public int GetUnclearedBillsRowsCount(bool forceRefresh)
        {
            return _unclearedBillsRowCount;
        }

        /// <summary>
        /// Gets the bills rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get bills from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of bills</returns>
        public int GetWriteOffBillsRowsCount(bool forceRefresh)
        {
            return _writeOffBillsRowCount;
        }

        /// <summary>
        /// Loads all the bills only while page index changed.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public BillsLedgerSearchItem[] LoadAllBills(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BillsLedgerSearchItem[] allBills = null;

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
                    BillsLedgerReturnValue returnValue = accountsService.LoadAllBills(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _allBillsRowCount = returnValue.Bills.TotalRowCount;
                        allBills = returnValue.Bills.Rows;
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

            return allBills;
        }

        /// <summary>
        /// Loads uncleared bills only while page index changed.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public BillsLedgerSearchItem[] LoadUnclearedBills(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BillsLedgerSearchItem[] unclearedBills = null;

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
                    BillsLedgerReturnValue returnValue = accountsService.LoadUnclearedBills(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _unclearedBillsRowCount = returnValue.Bills.TotalRowCount;
                        unclearedBills = returnValue.Bills.Rows;
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

            return unclearedBills;
        }

        /// <summary>
        /// Loads write off bills only while page index changed.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public BillsLedgerSearchItem[] LoadWriteOffBills(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BillsLedgerSearchItem[] writeOffBills = null;

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
                    BillsLedgerReturnValue returnValue = accountsService.LoadWriteOffBills(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _writeOffBillsRowCount = returnValue.Bills.TotalRowCount;
                        writeOffBills = returnValue.Bills.Rows;
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

            return writeOffBills;
        }

        #endregion

        #region Private Methods

        #region GetFinancialInfo

        /// <summary>
        /// Loads financial information about office, client and deposit 
        /// total balance by project id
        /// </summary>        
        private void GetFinancialInfo()
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

        #region BindBillsLedger

        /// <summary>
        /// Binds the bills ledger
        /// </summary>
        private void BindBillsLedger()
        {
            try
            {
                _hdnAllBillsRefresh.Value = "true";
                _hdnWriteOffBillsRefresh.Value = "true";
                _hdnUnclearedBillsRefresh.Value = "true";

                _grdAllBills.PageIndex = 0;
                _grdAllBills.DataSourceID = _odsAllBills.ID;

                _grdUnclearedBills.PageIndex = 0;
                _grdUnclearedBills.DataSourceID = _odsUnclearedBills.ID;

                _grdWriteOffs.PageIndex = 0;
                _grdWriteOffs.DataSourceID = _odsWriteOffBills.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;
using System.Configuration;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewClientBills : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _clientRowCount;

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
                    // If project id is in session then binds deposit details by project id on page load
                    if (Session[SessionName.ProjectId] != null)
                    {
                        // Sets financial information about client, office and deposit balances.
                        SetFinancialInfo();
                        BindClientBills();
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
        protected void _grdClientBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BalancesSearchItem clientBalances = (BalancesSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (clientBalances.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = clientBalances.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large description
                if (clientBalances.PostingDescription.Length > 20)
                {
                    Label lblDescription = (Label)e.Row.FindControl("_lblDescription");
                    lblDescription.Text = clientBalances.PostingDescription.Substring(0, 20) + "...";
                }

                //if (clientBalances.PostingBank.Length > 20)
                //{
                //    Label lblBank = (Label)e.Row.FindControl("_lblBank");
                //    lblBank.Text = clientBalances.PostingBank.Substring(0, 20) + "...";
                //}
            }
        }

        #endregion

        #region Object Data Source Events

        /// <summary>
        /// Record selection event of client balances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsClientBalances_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
            _hdnRefresh.Value = "false";
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds client balances details by project id
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
                    _hdnRefresh.Value = "true";

                    // Binds client bills on matter changed.
                    BindClientBills();
                    SetFinancialInfo();
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
        /// Gets the client balances details rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get client balances details from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of client balances details</returns>
        public int GetClientBalancesRowsCount(bool forceRefresh)
        {
            return _clientRowCount;
        }

        /// <summary>
        /// Loads client balances details by project id
        /// </summary>
        /// <param name="startRow">starting row for grid view</param>
        /// <param name="pageSize">Sets the page size for grid view.</param>
        /// <param name="forceRefresh">Gets records if forcefresh is true else gets records from cacahe.</param>
        /// <returns>Retrieves client balances details by project id</returns>
        public BalancesSearchItem[] LoadClientBalancesDetails(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BalancesSearchItem[] clientBalances = null;

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
                    BalancesSearchReturnValue returnValue = accountsService.GetClientBalancesDetails(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _clientRowCount = returnValue.Balances.TotalRowCount;
                        clientBalances = returnValue.Balances.Rows;
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

            return clientBalances;
        }

        #endregion

        #region Private Methods

        #region SetFinancialInfo

        /// <summary>
        /// Loads financial information about office, client and deposit 
        /// total balance by project id
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

        #region BindClientBills

        /// <summary>
        /// Binds client bills on page load and on matter changed
        /// </summary>
        private void BindClientBills()
        {
            try
            {
                _grdClientBills.PageIndex = 0;
                _grdClientBills.DataSourceID = _odsClientBalances.ID;
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
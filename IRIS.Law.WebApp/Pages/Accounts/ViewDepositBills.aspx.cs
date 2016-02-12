using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using System.Configuration;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewDepositBills : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _depositRowCount;

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
                        BindDepositsLedger();
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
        protected void _grdDepositBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BalancesSearchItem depositBalances = (BalancesSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (depositBalances.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = depositBalances.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large description
                if (depositBalances.PostingDescription.Length > 20)
                {
                    Label lblDescription = (Label)e.Row.FindControl("_lblDescription");
                    lblDescription.Text = depositBalances.PostingDescription.Substring(0, 20) + "...";
                }

                if (depositBalances.PostingBank.Length > 20)
                {
                    Label lblBank = (Label)e.Row.FindControl("_lblBank");
                    lblBank.Text = depositBalances.PostingBank.Substring(0, 20) + "...";
                }
            }
        }

        #endregion

        #region Object Data Source Events

        /// <summary>
        /// Selected event for object data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsDepositBalances_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
        /// Binds deposit details by project id
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

                    // Binds deposits ledger on matter changed
                    BindDepositsLedger();
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
        /// Gets the deposit balances details rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get deposit balances details from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of deposit balances details</returns>
        public int GetDepositBalancesRowsCount(bool forceRefresh)
        {
            return _depositRowCount;
        }

        /// <summary>
        /// Loads deposit balances details by project id
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Retrieves deposit balances details by project id</returns>
        public BalancesSearchItem[] LoadDepositBalancesDetails(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BalancesSearchItem[] depositBalances = null;

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
                    BalancesSearchReturnValue returnValue = accountsService.GetDepositBalancesDetails(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _depositRowCount = returnValue.Balances.TotalRowCount;
                        depositBalances = returnValue.Balances.Rows;
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

            return depositBalances;
        }

        #endregion

        #region Private Methods

        #region SetFinancialInfo

        /// <summary>
        /// Loads financial information about office, client and total deposit balances
        /// by project id
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

        #region BindDepositsLedger

        /// <summary>
        /// Binds deposits ledger on page load as well as matter changed
        /// </summary>
        private void BindDepositsLedger()
        {
            try
            {
                _grdDepositBills.PageIndex = 0;
                _grdDepositBills.DataSourceID = _odsDepositBalances.ID;
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

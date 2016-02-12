using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewOfficeBills : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _officeRowCount;

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
                    // If project id is in session then binds office details by project id on page load
                    if (Session[SessionName.ProjectId] != null)
                    {
                        // Sets financial information about client, office and deposit balances.
                        SetFinancialInfo();

                        // Binds office bills by project id
                        BindOfficeBills();
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

        #region Grid Events

        /// <summary>
        /// Row data bound event to truncate the description and bank names after 20 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdOfficeBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BalancesSearchItem officeBalances = (BalancesSearchItem)e.Row.DataItem;

                // Truncate large reference
                if (officeBalances.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = officeBalances.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large descriptions
                if (officeBalances.PostingDescription.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblDescription");
                    lbldescription.Text = officeBalances.PostingDescription.Substring(0, 20) + "...";
                }

                // Truncate large bank names
                //if (officeBalances.PostingBank.Length > 20)
                //{
                //    Label lblBankName = (Label)e.Row.FindControl("_lblBank");
                //    lblBankName.Text = officeBalances.PostingBank.Substring(0, 20) + "...";
                //}
            }
        }

        #endregion

        #region Object Data Source Events

        /// <summary>
        /// Selection event for object data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsOfficeBalances_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
        /// Binds office bills details by project id
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
                    // Binds office bills on matter changed 
                    BindOfficeBills();

                    // Sets financial information for selected project id
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
        /// Gets the office bills balances details rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get office bills details from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of office bills details</returns>
        public int GetOfficeBalancesRowsCount(bool forceRefresh)
        {
            return _officeRowCount;
        }

        /// <summary>
        /// Loads office bills details by project id
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Retrieves office bills details by project id</returns>
        public BalancesSearchItem[] LoadOfficeBalancesDetails(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            BalancesSearchItem[] officeBalances = null;

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
                    BalancesSearchReturnValue returnValue = accountsService.GetOfficeBalancesDetails(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _officeRowCount = returnValue.Balances.TotalRowCount;
                        officeBalances = returnValue.Balances.Rows;
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

            return officeBalances;
        }

        #endregion

        #region Private Methods

        #region BindOfficeBills

        /// <summary>
        /// Binds office bills
        /// </summary>
        private void BindOfficeBills()
        {
            try
            {
                _hdnRefresh.Value = "true";
                _grdOfficeBills.PageIndex = 0;
                _grdOfficeBills.DataSourceID = _odsOfficeBalances.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SetFinancialInfo

        /// <summary>
        /// Sets financial information about office, client and deposit balances
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

                    // Sets warning message if paid office balances amount less than zero
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

        #endregion
    }
}

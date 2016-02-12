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
    public partial class ViewDisbursementsLedger : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _disbursementsRowCount;

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
                    // If project id is in session then sets disbursements by project id
                    if (Session[SessionName.ProjectId] != null)
                    {
                        // Sets financial information about client, office and deposit balances.
                        SetFinancialInfo();

                        // Binds disbursements details on grid view
                        BindDisbursements();
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
        protected void _grdDisbursementsLedger_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DisbursementSearchItem disbursements = (DisbursementSearchItem)e.Row.DataItem;

                // Truncate large descriptions
                if (disbursements.PostingReference.Length > 20)
                {
                    Label lblReference = (Label)e.Row.FindControl("_lblReference");
                    lblReference.Text = disbursements.PostingReference.Substring(0, 20) + "...";
                }

                // Truncate large descriptions
                if (disbursements.PostingDescription.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblDescription");
                    lbldescription.Text = disbursements.PostingDescription.Substring(0, 20) + "...";
                }

                // Truncate disbursement types
                if (disbursements.PostingType.Length > 20)
                {
                    Label lblPostingType = (Label)e.Row.FindControl("_lblDisbursementType");
                    lblPostingType.Text = disbursements.PostingType.Substring(0, 20) + "...";
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
        protected void _odsLoadDisbursements_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
        /// Binds disbursement ledger
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
                    // Binds disbursements details on matter changed
                    BindDisbursements();

                    // Sets financial information on matter changed.
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
        /// Loads disbursements by project id
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public DisbursementSearchItem[] LoadDisbursements(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            DisbursementSearchItem[] disbursements = null;

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
                    DisbursementsSearchReturnValue returnValue = accountsService.GetDisbursementsDetails(logonId, collectionRequest, projectId);

                    if (returnValue.Success)
                    {
                        _disbursementsRowCount = returnValue.Disbursements.TotalRowCount;
                        disbursements = returnValue.Disbursements.Rows;
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

            return disbursements;
        }

        /// <summary>
        /// Gets the disbursements balances rows count
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public int GetDisbursementsRowsCount(bool forceRefresh)
        {
            return _disbursementsRowCount;
        }

        #endregion

        #region Private Methods

        #region BindDisbursements

        /// <summary>
        /// Binds disbursement details on grid view.
        /// </summary>
        private void BindDisbursements()
        {
            try
            {
                _hdnRefresh.Value = "true";
                _grdDisbursementsLedger.PageIndex = 0;
                _grdDisbursementsLedger.DataSourceID = _odsDisbursements.ID;
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
        /// total balance by project id
        /// </summary>
        /// <param name="returnValue"></param>
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

        #endregion
    }
}

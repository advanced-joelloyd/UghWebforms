using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebApp.UserControls;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewFinancialBalances : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads page with default values
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

                        // Sets financial balances by project id
                        SetFinancialBalances();
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

        #region Public Methods

        /// <summary>
        /// Binds financial balances by project id on matter changed
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
                    // Sets financial information about client, office and deposit balances.
                    SetFinancialInfo();

                    // Sets financial balances by project id
                    SetFinancialBalances();
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

        #region SetFinancialBalances

        /// <summary>
        /// Sets financial balances info by project id.
        /// </summary>
        private void SetFinancialBalances()
        {
            AccountsServiceClient accountsService = null;

            try
            {
                accountsService = new AccountsServiceClient();
                Guid projectId = (Guid)Session[SessionName.ProjectId];
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                FinancialBalancesReturnValue returnValue = accountsService.GetFinancialBalances(logonId, projectId);

                if (returnValue.Success)
                {
                    _txtAnticipatedBills.Text = returnValue.AnticipatedBills;
                    _txtCostBills.Text = returnValue.CostBills;
                    _txtDisbursements.Text = returnValue.AnticipatedDisbursements;
                    _txtPFClaims.Text = returnValue.AnticipatedPFClaims;
                    _txtTime.Text = returnValue.Time;
                    _txtTimeChargeout.Text = returnValue.TimeChargeOut;
                    _txtTimeCost.Text = returnValue.TimeCost;
                    _txtUnbilledDisbursements.Text = returnValue.CostUnbilledDisbursements;
                    _txtUnpaidbilledDisbursements.Text = returnValue.CostUnpaidBilledDisbursements;
                    _txtWIPChargeout.Text = returnValue.WIPChargeOut;
                    _txtWIPCost.Text = returnValue.WIPCost;
                    _txtWIPTime.Text = returnValue.WIPTime;

                    // Sets movement details
                    if (returnValue.LastBill != DataConstants.BlankDate)
                    {
                        _ccLastBill.DateText = Convert.ToString(returnValue.LastBill);
                    }
                    if (returnValue.LastFinancial != DataConstants.BlankDate)
                    {
                        _ccLastFinancial.DateText = Convert.ToString(returnValue.LastFinancial);
                    }

                    if (returnValue.LastTime != DataConstants.BlankDate)
                    {
                        _ccLastTime.DateText = Convert.ToString(returnValue.LastTime);
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

        #region SetFinancialInfo

        /// <summary>
        /// Loads financial information about office, client and deposit balances
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
                    _txtClientFinancialInfo.Text = returnValue.ClientBalance;
                    _txtOfficeFinancialInfo.Text = returnValue.OfficeBalance;
                    _txtDepositFinancialInfo.Text = returnValue.DepositBalance;

                    // Sets warning message, if paid disbursements amount less than zero
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

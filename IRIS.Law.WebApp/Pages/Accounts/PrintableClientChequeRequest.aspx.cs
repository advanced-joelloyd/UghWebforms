using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Accounts;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class PrintableClientChequeRequest : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;

        #endregion

        #region Protected Methods
        
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
                    if (Session[SessionName.ChequeRequestId] == null)
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = "Please provide an Client Cheque RequestId";
                        return;
                    }
                    else
                    {
                        _hdnClientChequeRequestId.Value = Convert.ToString(Session[SessionName.ChequeRequestId]);
                        LoadControls(Convert.ToInt32(_hdnClientChequeRequestId.Value));

                        // After printing cheque request should be zero for adding new cheque request.
                        Session[SessionName.ChequeRequestId] = null;
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
        /// Returns to the add screen on cancel button click for adding new request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Accounts/ClientChequeRequest.aspx", true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads client cheque request details by client cheque request id 
        /// for printing.
        /// </summary>
        /// <param name="chequeRequestId">Client cheque request id to populate client cheque request details</param>   
        private void LoadControls(int chequeRequestId)
        {
            AccountsServiceClient accountsService = null;
            ChequeRequestReturnValue returnValue = null;

            try
            {
                accountsService = new AccountsServiceClient();
                returnValue = new ChequeRequestReturnValue();
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                returnValue = accountsService.LoadClientChequeRequestDetailsForPrinting(logonId, chequeRequestId);

                if (returnValue.Success)
                {
                    _lblClientName.Text = returnValue.ChequeRequest.PersonName;
                    _lblAddressLine1.Text = returnValue.ChequeRequest.AddressLine1;
                    _lblAddressLine2.Text = returnValue.ChequeRequest.AddressLine2;
                    _lblAddressLine3.Text = returnValue.ChequeRequest.AddressLine3;
                    _lblAddressTown.Text = returnValue.ChequeRequest.AddressTown;
                    _lblAddressCounty.Text = returnValue.ChequeRequest.AddressCounty;
                    _lblAddressPostcode.Text = returnValue.ChequeRequest.AddressPostcode;
                    _lblMatterDescription.Text = returnValue.ChequeRequest.MatterDescription;
                    _lblMatterReference.Text = returnValue.ChequeRequest.MatterReference;
                    _lblFeeEarner.Text = returnValue.ChequeRequest.FeeEarnerReference;
                    _lblPartner.Text = returnValue.ChequeRequest.PartnerName;
                    _lblUserName.Text = returnValue.ChequeRequest.UserName;
                    _lblClientChequeRequestDate.Text = returnValue.ChequeRequest.ChequeRequestDate.ToShortDateString();
                    _lblClientChequeRequestDescription.Text = returnValue.ChequeRequest.ChequeRequestDescription;
                    _lblClientChequeRequestPayee.Text = returnValue.ChequeRequest.ChequeRequestPayee;
                    _lblClientChequeRequestBank.Text = returnValue.ChequeRequest.BankName;
                    _lblClientChequeRequestAmount.Text = returnValue.ChequeRequest.ChequeRequestAmount.ToString("0.00");
                    _chkBxClientChequeRequestAuthorised.Checked = returnValue.ChequeRequest.IsChequeRequestAuthorised;
                    _lblClearanceType.Text = returnValue.ChequeRequest.ClearanceTypeDesc;
                    _lblBranch.Text = returnValue.ChequeRequest.MatBranchRef;
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
    }
}

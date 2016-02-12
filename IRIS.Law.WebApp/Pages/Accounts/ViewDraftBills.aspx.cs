using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebApp.App_Code;
using System.Collections;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class ViewDraftBills : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _draftBillsRowCount;

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
            if (!IsPostBack)
            {
                BindDraftBills();
            }
        }

        #endregion

        /// <summary>
        /// Row data bound for client cheque request gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdDraftBills_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkBxSelect = (CheckBox)e.Row.FindControl("_chkBxSelect");
                chkBxSelect.Attributes.Add("onclick", "EnableDisableButtons('" + _grdDraftBills.ClientID + "')");
            }
        }

        /// <summary>
        /// Submits unposted draft bills to accounts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSubmit_Click(object sender, EventArgs e)
        {
            AccountsServiceClient accountsService = null;

            try
            {
                accountsService = new AccountsServiceClient();

                CheckBox checkboxSelect = null;
                List<int> arrListSelectedDraftBillIds = new List<int>();               

                foreach (GridViewRow gridViewRow in _grdDraftBills.Rows)
                {
                    checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                    if (checkboxSelect.Checked == true)
                    {                        
                        Label _lblDraftBillId = ((Label)gridViewRow.FindControl("_lblDraftBillId"));
                        arrListSelectedDraftBillIds.Add(Convert.ToInt32(_lblDraftBillId.Text));
                    }
                }

                int[] arrSelectedIds = arrListSelectedDraftBillIds.ToArray();

                // If there are any selected draft bill ids to be submitted, then call submit method.
                if (arrListSelectedDraftBillIds.Count > 0)
                {
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    DraftBillReturnValue returnValue = accountsService.SubmitDraftBill(logonId, arrSelectedIds);

                    if (returnValue.Success)
                    {
                        _hdnRefresh.Value = "true";
                        // After submission again binds unposted draft bills.
                        BindDraftBills();
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

        #region Object Data Source Events

        /// <summary>
        /// Record selection event of client balances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsDraftBills_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
        /// Gets the draft bills rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get draft bills from cache if its true, on page refresh</param>
        /// <returns>Returns rows count of draft bills</returns>
        public int GetDraftBillsRowsCount(bool forceRefresh)
        {
            return _draftBillsRowCount;
        }

        /// <summary>
        /// Loads unposted draft bills on page load.
        /// </summary>
        /// <param name="startRow">Starting row for draft bills.</param>
        /// <param name="pageSize">Page size on grid view for unposted draft bills.</param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns all unposted draft bills.</returns>
        public DraftBillSearchItem[] LoadDraftBills(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            DraftBillSearchItem[] draftBills = null;

            try
            {                
                accountsService = new AccountsServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = forceRefresh;
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;
                
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                DraftBillSearchReturnValue returnValue = accountsService.GetUnpostedDraftBills(logonId, collectionRequest);

                if (returnValue.Success)
                {
                    _draftBillsRowCount = returnValue.UnpostedDraftBills.TotalRowCount;
                    draftBills = returnValue.UnpostedDraftBills.Rows;                    
                }
                else
                {
                    throw new Exception(returnValue.Message);
                    //_lblMessage.CssClass = "errorMessage";
                    //_lblMessage.Text = returnValue.Message;
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

            return draftBills;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Binds unposted draft bills
        /// </summary>
        private void BindDraftBills()
        {
            try
            {
                _grdDraftBills.PageIndex = 0;
                _grdDraftBills.DataSourceID = _odsDraftBills.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

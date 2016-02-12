using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Accounts;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using System.Collections;

namespace IRIS.Law.WebApp.Pages.Accounts
{
    public partial class AuthoriseChequeRequests : BasePage
    {
        #region Private Variables

        LogonReturnValue _logonSettings;
        int _clientChequeRequestsCreditRowCount;
        int _clientChequeRequestsDebitRowCount;
        int _officeChequeRequestsRowCount;
        
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
            
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            
            _lblMessage.Text = string.Empty;

            if (!IsPostBack)
            {                
                BindUnauthorisedChequeRequests();

                _btnAuthorise.Attributes.Add("onclick", "return confirm('Are you sure you wish to authorise?');");
                _btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you wish to delete?');");
            }
        }

        #endregion

        #region Ajax Tab Container Events

        /// <summary>
        /// Loads unauthorised cheque requests for client and office on tab changed
        /// because on tab changed, checkboxes on gridview needs to be reset. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _tcAuthoriseChequeRequests_ActiveTabChanged(object sender, EventArgs e)
        {
            try
            {
                // If checkboxes are checked, 
                // then on tab index changed grid views need to be bound again,
                // to unselect checkboxes and disable buttons on page.
                BindUnauthorisedChequeRequests();
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
        /// Deletes unauthorised cheque request for client or office
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnDelete_Click(object sender, EventArgs e)
        {
            AccountsServiceClient accountsService = null;

            try
            {
                accountsService = new AccountsServiceClient();  

                List<int> listSelectedChequeRequestIds = new List<int>();

                CheckBox checkboxSelect = null;

                bool isClientChequeRequest = false;

                foreach (GridViewRow gridViewRow in _grdClientChequeRequestsDebit.Rows)
                {
                    checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                    if (checkboxSelect.Checked == true)
                    {
                        isClientChequeRequest = true;
                        Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                        listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                    }
                }

                foreach (GridViewRow gridViewRow in _grdClientChequeRequestsCredit.Rows)
                {
                    checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                    if (checkboxSelect.Checked == true)
                    {
                        isClientChequeRequest = true;
                        Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                        listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                    }
                }

                if (!isClientChequeRequest)
                {
                    foreach (GridViewRow gridViewRow in _grdOfficeChequeRequests.Rows)
                    {
                        checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                        if (checkboxSelect.Checked == true)
                        {
                            isClientChequeRequest = false;
                            Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                            listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                        }
                    }
                }

                // If there are any selected cheque request ids to be deleted, then call delete method.
                if (listSelectedChequeRequestIds.Count > 0)
                {
                    int[] arrSelectedChequeRequestIds =listSelectedChequeRequestIds.ToArray();
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    ChequeRequestReturnValue returnValue = accountsService.DeleteChequeRequests(logonId, arrSelectedChequeRequestIds, isClientChequeRequest);

                    if (returnValue.Success)
                    {
                        // Disables the buttons after deletion,similar to page load.
                        _btnDelete.Enabled = false;
                        _btnAuthorise.Enabled = false;

                        _hdnRefreshClientChequeRequestCredit.Value = "true";
                        _hdnRefreshClientChequeRequestDebit.Value = "true";
                        _hdnRefreshOfficeChequeRequest.Value = "true";

                        if (isClientChequeRequest)
                        {
                            _grdClientChequeRequestsCredit.PageIndex = 0;
                            _grdClientChequeRequestsCredit.DataSourceID = _odsClientChequeRequestsCredit.ID;

                            _grdClientChequeRequestsDebit.PageIndex = 0;
                            _grdClientChequeRequestsDebit.DataSourceID = _odsClientChequeRequestsDebit.ID;  
                        }
                        else
                        {
                            _grdOfficeChequeRequests.PageIndex = 0;
                            _grdOfficeChequeRequests.DataSourceID = _odsOfficeChequeRequests.ID;
                        }
                        _lblMessage.CssClass = "successMessage";
                        _lblMessage.Text = "Cheque Request Deleted";
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

        /// <summary>
        /// Authorises unauthorised cheque requests for client as well as office.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnAuthorise_Click(object sender, EventArgs e)
        {            
            AccountsServiceClient accountsService = null;

            try
            {
                accountsService = new AccountsServiceClient();

                List<int> listSelectedChequeRequestIds = new List<int>();

                CheckBox checkboxSelect = null;

                bool isClientChequeRequest = false;

                foreach (GridViewRow gridViewRow in _grdClientChequeRequestsCredit.Rows)
                {
                    checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                    if (checkboxSelect.Checked == true)
                    {
                        isClientChequeRequest = true;
                        Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                        listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                    }
                }

                foreach (GridViewRow gridViewRow in _grdClientChequeRequestsDebit.Rows)
                {
                    checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                    if (checkboxSelect.Checked == true)
                    {
                        isClientChequeRequest = true;
                        Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                        listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                    }
                }

                if (!isClientChequeRequest)
                {
                    foreach (GridViewRow gridViewRow in _grdOfficeChequeRequests.Rows)
                    {
                        checkboxSelect = (CheckBox)gridViewRow.FindControl("_chkBxSelect");
                        if (checkboxSelect.Checked == true)
                        {
                            isClientChequeRequest = false;
                            Label _lblCheckRequestId = ((Label)gridViewRow.FindControl("_lblChequeRequestId"));
                            listSelectedChequeRequestIds.Add(Convert.ToInt32(_lblCheckRequestId.Text));
                        }
                    }
                }

                // If there are any selected cheque request ids to be deleted, then call delete method.
                if (listSelectedChequeRequestIds.Count > 0)
                {
                    int[] arrSelectedChequeRequestIds = listSelectedChequeRequestIds.ToArray();
                    Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    ReturnValue returnValue = accountsService.AuthoriseChequeRequests(logonId, arrSelectedChequeRequestIds, isClientChequeRequest);

                    if (returnValue.Success)
                    {
                        // Disables the buttons after deletion,similar to page load.
                        _btnDelete.Enabled = false;
                        _btnAuthorise.Enabled = false;

                        _hdnRefreshClientChequeRequestCredit.Value = "true";
                        _hdnRefreshClientChequeRequestDebit.Value = "true";
                        _hdnRefreshOfficeChequeRequest.Value = "true";

                        if (isClientChequeRequest)
                        {
                            _grdClientChequeRequestsCredit.PageIndex = 0;
                            _grdClientChequeRequestsCredit.DataSourceID = _odsClientChequeRequestsCredit.ID;

                            _grdClientChequeRequestsDebit.PageIndex = 0;
                            _grdClientChequeRequestsDebit.DataSourceID = _odsClientChequeRequestsDebit.ID;
                        }
                        else
                        {
                            _grdOfficeChequeRequests.PageIndex = 0;
                            _grdOfficeChequeRequests.DataSourceID = _odsOfficeChequeRequests.ID;
                        }

                        _lblMessage.CssClass = "successMessage";
                        _lblMessage.Text = "Cheque Request Authorised";
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
                _lblMessage.CssClass = "successMessage";
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

        /// <summary>
        /// Row command event for editting office cheque request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdOfficeChequeRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_lblChequeRequestId") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    Label _lblChequeRequestId = (Label)(_grdOfficeChequeRequests.Rows[rowId].FindControl("_lblChequeRequestId"));
                    
                    // Gets client cheque request id
                    Session[SessionName.ChequeRequestId] = _lblChequeRequestId.Text;

                    Response.Redirect("~/Pages/Accounts/OfficeChequeRequest.aspx?edit=true", true);
                }          
            }           
        }

        /// <summary>
        /// Row command event for editting client cheque request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdClientChequeRequestsDebit_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_lblChequeRequestId") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    Label _lblChequeRequestId = (Label)(_grdClientChequeRequestsDebit.Rows[rowId].FindControl("_lblChequeRequestId"));

                    // Gets office cheque request id
                    Session[SessionName.ChequeRequestId] = _lblChequeRequestId.Text;

                    Response.Redirect("~/Pages/Accounts/ClientChequeRequest.aspx?edit=true", true);
                }
            }            
        }

        protected void _grdClientChequeRequestsCredit_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_lblChequeRequestId") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    Label _lblChequeRequestId = (Label)(_grdClientChequeRequestsCredit.Rows[rowId].FindControl("_lblChequeRequestId"));

                    // Gets office cheque request id
                    Session[SessionName.ChequeRequestId] = _lblChequeRequestId.Text;

                    Response.Redirect("~/Pages/Accounts/ClientChequeRequest.aspx?edit=true", true);
                }
            }
        }

        /// <summary>
        /// Row data bound for office cheque request gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdOfficeChequeRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Label lblAmount = (Label)e.Row.FindControl("_lblAmount");
                CheckBox chkBxAnticipated = (CheckBox)e.Row.FindControl("_chkBxAnticipated");
                Label lblIsAnticipated = (Label)e.Row.FindControl("_lblAnticipated");

                if (lblIsAnticipated.Text == "True")
                {
                    chkBxAnticipated.Checked = true;
                }               

                CheckBox chkBxSelect = (CheckBox)e.Row.FindControl("_chkBxSelect");
                chkBxSelect.Attributes.Add("onclick", "EnableDisableButtons('" + _grdOfficeChequeRequests.ClientID + "')");
            }
        }

        /// <summary>
        /// Row data bound for client cheque request gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdClientChequeRequestsDebit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkBxSelect = (CheckBox)e.Row.FindControl("_chkBxSelect");
                chkBxSelect.Attributes.Add("onclick", "EnableDisableButtons('" + _grdClientChequeRequestsDebit.ClientID + "')");
            }
        }

        protected void _grdClientChequeRequestsCredit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkBxSelect = (CheckBox)e.Row.FindControl("_chkBxSelect");
                chkBxSelect.Attributes.Add("onclick", "EnableDisableButtons('" + _grdClientChequeRequestsCredit.ClientID + "')");
            }
        }

        #region Object Data Source Events

        /// <summary>
        /// Record selection event of unauthorised cheque requests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsUnauthorisedClientChequeRequestsCredit_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
            _hdnRefreshClientChequeRequestCredit.Value = "false";            
        }

        protected void _odsUnauthorisedClientChequeRequestsDebit_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
            _hdnRefreshClientChequeRequestDebit.Value = "false";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _odsUnauthorisedOfficeChequeRequests_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
            _hdnRefreshOfficeChequeRequest.Value = "false";
        }

        #endregion
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets unauthorised client cheque requests rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get time transactions from cache if its true, on page refresh</param>
        /// <returns>Returns rows count for unauthorised client cheque requests</returns>
        public int GetUnauthorisedClientChequeRequestsCreditRowsCount(bool forceRefresh)
        {
            return _clientChequeRequestsCreditRowCount;
        }

        public int GetUnauthorisedClientChequeRequestsDebitRowsCount(bool forceRefresh)
        {
            return _clientChequeRequestsDebitRowCount;
        }

        /// <summary>
        /// Gets unauthorised office cheque requests rows count
        /// </summary>
        /// <param name="forceRefresh">Used to get write off time transactions from cache if its true, on page refresh</param>
        /// <returns>Returns rows count for unauthorised office cheque requests</returns>
        public int GetUnauthorisedOfficeChequeRequestsRowsCount(bool forceRefresh)
        {
            return _officeChequeRequestsRowCount;
        }

        /// <summary>
        /// Loads unauthorised client cheque requests only while page index changed.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns unauthorised client cheque requests only while page index changed.</returns>
        public ChequeAuthorisationSearchItem[] LoadUnauthorisedClientChequeRequestsDebit(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            ChequeAuthorisationSearchItem[] clientChequeRequests = null;

            try
            {
                accountsService = new AccountsServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = forceRefresh;
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;

                ChequeAuthorisationSearchCriteria searchCriteria = new ChequeAuthorisationSearchCriteria();
                searchCriteria.IsAuthorised = false;
                searchCriteria.IsPosted = false;

                // Suggestd by client after introducing new properties in service layer
                searchCriteria.IncludeDebit = true;
                searchCriteria.IncludeCredit = false;

                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                ChequeAuthorisationReturnValue returnValue = accountsService.GetUnauthorisedClientChequeRequests(logonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {                    
                    _clientChequeRequestsDebitRowCount = returnValue.ChequeRequests.TotalRowCount;
                    clientChequeRequests = returnValue.ChequeRequests.Rows;
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

            return clientChequeRequests;
        }

        public ChequeAuthorisationSearchItem[] LoadUnauthorisedClientChequeRequestsCredit(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            ChequeAuthorisationSearchItem[] clientChequeRequests = null;

            try
            {
                accountsService = new AccountsServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = forceRefresh;
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;

                ChequeAuthorisationSearchCriteria searchCriteria = new ChequeAuthorisationSearchCriteria();
                searchCriteria.IsAuthorised = false;
                searchCriteria.IsPosted = false;

                // Suggestd by client after introducing new properties in service layer
                searchCriteria.IncludeDebit = false;
                searchCriteria.IncludeCredit = true;

                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                ChequeAuthorisationReturnValue returnValue = accountsService.GetUnauthorisedClientChequeRequests(logonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _clientChequeRequestsCreditRowCount = returnValue.ChequeRequests.TotalRowCount;
                    clientChequeRequests = returnValue.ChequeRequests.Rows;
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

            return clientChequeRequests;
        }

        /// <summary>
        /// Loads unauthorised office cheque requests only while page index changed.
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <param name="forceRefresh"></param>
        /// <returns>Returns unauthorised office cheque requests only while page index changed.</returns>
        public ChequeAuthorisationSearchItem[] LoadUnauthorisedOfficeChequeRequests(int startRow, int pageSize, bool forceRefresh)
        {
            AccountsServiceClient accountsService = null;
            ChequeAuthorisationSearchItem[] officeChequeRequests = null;

            try
            {
                accountsService = new AccountsServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = forceRefresh;
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;

                ChequeAuthorisationSearchCriteria searchCriteria = new ChequeAuthorisationSearchCriteria();
                searchCriteria.IsAuthorised = false;
                searchCriteria.IsPosted = false;
                
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                ChequeAuthorisationReturnValue returnValue = accountsService.GetUnauthorisedOfficeChequeRequests(logonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {                    
                    _officeChequeRequestsRowCount = returnValue.ChequeRequests.TotalRowCount;
                    officeChequeRequests = returnValue.ChequeRequests.Rows;
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

            return officeChequeRequests;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Binds time transactions to grid view control
        /// </summary>
        private void BindUnauthorisedChequeRequests()
        {
            try
            {
                _hdnRefreshClientChequeRequestCredit.Value = "true";
                _hdnRefreshClientChequeRequestDebit.Value = "true";
                _hdnRefreshOfficeChequeRequest.Value = "true";

                _grdClientChequeRequestsCredit.PageIndex = 0;
                _grdClientChequeRequestsCredit.DataSourceID = _odsClientChequeRequestsCredit.ID;

                _grdClientChequeRequestsDebit.PageIndex = 0;
                _grdClientChequeRequestsDebit.DataSourceID = _odsClientChequeRequestsDebit.ID;

                _grdOfficeChequeRequests.PageIndex = 0;
                _grdOfficeChequeRequests.DataSourceID = _odsOfficeChequeRequests.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }    

        #endregion
    }
}

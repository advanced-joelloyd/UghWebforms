using System;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Web.UI;

namespace IRIS.Law.WebApp.Pages.Appointment
{
    public partial class ViewAppointment : BasePage
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        private int _appointmentRowCount;
        LogonReturnValue _logonSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                

                _lblError.Text = string.Empty;
                _lblError.CssClass = "errorMessage";
                //Set the page size for the grids
                _grdSearchAppointmentList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

                if (!IsPostBack)
                {
                    BindUsers();
                    GetDefaultFeeEarnerDetails();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }

        }

        #region BindUsers
        private void BindUsers()
        {
            DiaryServiceClient diaryService = null;
            try
            {
                diaryService = new DiaryServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                DiaryMemberSearchReturnValue returnValue = new DiaryMemberSearchReturnValue();
                returnValue = diaryService.GetDiaryMembers(_logonSettings.LogonId, collectionRequest);

                if (returnValue.Success)
                {
                    _ddlUsers.DataSource = returnValue.DiaryMembers.Rows;
                    _ddlUsers.DataTextField = "MemberDisplayName";
                    _ddlUsers.DataValueField = "MemberID";
                    _ddlUsers.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (diaryService != null)
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }
            }
        }
        #endregion

        #region Search button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //Set force refresh to true so that data is not retrieved from the cache
                _hdnRefresh.Value = "true";
                if (string.IsNullOrEmpty(_ccDate.DateText.Trim()))
                {
                    _ccDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                }
                _hdnUser.Value = _ddlUsers.SelectedValue;

                BindAppointmentList();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region BindAppointmentList
        private void BindAppointmentList()
        {
            try
            {
                _grdSearchAppointmentList.PageIndex = 0;
                _grdSearchAppointmentList.DataSourceID = _odsSearchAppointment.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Gridview RowDatabound
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdSearchAppointmentList_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell tc in e.Row.Cells)
                    {
                        if (tc.HasControls())
                        {
                            // search for the header link
                            LinkButton lnk = (LinkButton)tc.Controls[0];
                            if (lnk != null)
                            {
                                // inizialize a new image
                                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                // setting the dynamically URL of the image
                                img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchAppointmentList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                // checking if the header link is the user's choice
                                if (_grdSearchAppointmentList.SortExpression == lnk.CommandArgument)
                                {
                                    // adding a space and the image to the header link
                                    tc.Controls.Add(new LiteralControl(" "));
                                    tc.Controls.Add(img);
                                }
                            }
                        }
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label lblDate = (Label)e.Row.FindControl("_lblDate");
                    if (lblDate != null)
                    {
                        lblDate.Text = Convert.ToDateTime(lblDate.Text).ToString("dd/MM/yyyy");                        
                    }

                    //Truncate doc name descriptions
                    LinkButton linkSubject = (LinkButton)e.Row.FindControl("_linkSubject");
                    if (linkSubject != null)
                    {
                        if (linkSubject.Text.Length > 40)
                        {
                            linkSubject.Text = linkSubject.Text.Substring(0, 40) + "...";
                        }
                    }

                    //Truncate doc name descriptions
                    Label lblNotes = (Label)e.Row.FindControl("_lblNotes");
                    if (lblNotes != null)
                    {
                        if (lblNotes.Text.Length > 40)
                        {
                            lblNotes.Text = lblNotes.Text.Substring(0, 40) + "...";
                        }
                    }

                    LinkButton linkDelete = (LinkButton)e.Row.FindControl("_linkDelete");
                    if (linkDelete != null)
                    {
                        Label lblId = (Label)e.Row.FindControl("_lblId");
                        if (lblId != null)
                        {
                            linkDelete.Attributes.Add("onclick", "return showDiaryCancellationModalPopupViaClient('" + lblId.Text + "');");
                        }
                    }

                    //Disable Edit/Delete button if user is not having rights
                    Label lblEdit = (Label)e.Row.FindControl("_lblEdit");

                    if (lblEdit != null)
                    {
                        if (lblEdit.Text.Trim().Length > 0)
                        {
                            linkSubject.Enabled = Convert.ToBoolean(lblEdit.Text);
                            linkDelete.Visible = Convert.ToBoolean(lblEdit.Text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Cancellation Finished Event
        /// <summary>
        /// This will fire after cancellation details has entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _dcAppointmentDelete_CancellationFinishedChanged(object sender, EventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(_dcAppointmentDelete.TaskAppointmentId.Trim()))
                {
                    // Delete Appointment
                    DeleteData deleteData = new DeleteData();
                    deleteData.CancellationText = _dcAppointmentDelete.CancellationText;
                    deleteData.ReasonCode = _dcAppointmentDelete.ReasonCode;
                    deleteData.CategoryCode = _dcAppointmentDelete.CategoryCode;
                    deleteData.OccurenceId = Convert.ToInt32(_dcAppointmentDelete.TaskAppointmentId);
                    deleteData.MemberId = _hdnUser.Value;
                    deleteData.IsBookingATask = false;

                    DiaryServiceClient diaryService = null;
                    try
                    {
                        if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                        {
                            Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                            

                            diaryService = new DiaryServiceClient();
                            ReturnValue returnValue = diaryService.DeleteBooking(_logonId, deleteData);

                            if (returnValue.Success)
                            {
                                BindAppointmentList();

                                _lblError.CssClass = "successMessage";
                                _lblError.Text = "Appointment deleted successfully.";
                            }
                            else
                            {
                                throw new Exception(returnValue.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (diaryService != null)
                        {
                            if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                                diaryService.Close();
                        }
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdSearchAppointmentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkSubject") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int appointmentId = Convert.ToInt32(_grdSearchAppointmentList.DataKeys[rowId].Values["Id"].ToString());

                    Session[SessionName.AppointmentId] = appointmentId;
                    Response.Redirect("~/Pages/Appointment/AppointmentDetails.aspx", true);
                }
            }
        }
        #endregion

        #region BindGridView Service

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetAppointmentRowsCount(string user, string date, bool forceRefresh)
        {
            //GetAppointmentRowsCount is directly called by the objectdatasource which expects a method 
            //with the same parameters as the method used to retrieve the data i.e SearchAppointment

            //Appointment row count is saved when we retrieve the appointments based on the search criteria
            return _appointmentRowCount;
        }

        /// <summary>
        /// Searches for appointments that match the search criteria.
        /// </summary>
        public IRIS.Law.WebServiceInterfaces.Diary.Appointment[] SearchAppointment(int startRow, int pageSize, string sortBy, string user, string date, bool forceRefresh)
        {
            DiaryServiceClient diaryService = null;
            IRIS.Law.WebServiceInterfaces.Diary.Appointment[] appointments = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    AppointmentSearchCriteria criteria = new AppointmentSearchCriteria();
                    criteria.MemberID = user;
                    criteria.OrderBy = sortBy;
                    if (!string.IsNullOrEmpty(date))
                    {
                        criteria.Date = Convert.ToDateTime(date);
                    }
                    else
                    {
                        criteria.Date = DataConstants.BlankDate;
                    }

                    diaryService = new DiaryServiceClient();
                    AppointmentSearchReturnValue returnValue = diaryService.AppointmentSearch(_logonId,
                                                collectionRequest, criteria);

                    if (returnValue.Success)
                    {
                        _appointmentRowCount = returnValue.Appointments.TotalRowCount;
                        appointments = returnValue.Appointments.Rows;
                    }
                    else
                    {
                        if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                            throw new Exception("Date is invalid");
                        else
                            throw new Exception(returnValue.Message);
                    }
                }
                return appointments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (diaryService != null)
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }
            }
        }

        protected void _odsSearchAppointment_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
            if (e.Exception != null)
            {
                _lblError.CssClass = "errorMessage";
                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblError.Text = DataConstants.WSEndPointErrorMessage;
                else
                    _lblError.Text = e.Exception.InnerException.Message;
               
                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefresh.Value = "false";
        }
        #endregion

        #region GetDefaultFeeEarnerDetails
        private void GetDefaultFeeEarnerDetails()
        {
            DiaryServiceClient diaryService = null;
            try
            {
                diaryService = new DiaryServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                DiaryMemberSearchReturnValue returnValue = new DiaryMemberSearchReturnValue();
                returnValue = diaryService.GetDiaryMembers(_logonSettings.LogonId, collectionRequest);

                if (returnValue.Success)
                {
                    for (int i = 0; i < returnValue.DiaryMembers.Rows.Length; i++)
                    {
                        if (_logonSettings.UserDefaultFeeMemberId.ToString() == returnValue.DiaryMembers.Rows[i].MemberID)
                        {
                            _ddlUsers.SelectedIndex = -1;
                            if (_ddlUsers.Items.FindByValue(_logonSettings.UserDefaultFeeMemberId.ToString()) != null)
                            {
                                _ddlUsers.Items.FindByValue(_logonSettings.UserDefaultFeeMemberId.ToString()).Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (diaryService != null)
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }
            }
        }
        #endregion
    }
}

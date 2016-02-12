using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using System.Configuration;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.Task
{
    public partial class ViewMatterTasks : BasePage
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        private int _taskRowCount;
        LogonReturnValue _logonSettings;

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                
                _lblError.CssClass = "errorMessage";
                _lblError.Text = string.Empty;
                //Set the page size for the grids
                _grdSearchTaskList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

                if (!IsPostBack)
                {
                    _ccDateTo.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                    BindUsers();
                    BindTaskStatus();
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
        #endregion

        #region _cliMatDetails_MatterChanged
        /// <summary>
        /// This will fire after selection of Client from Client Search popup.
        /// This will fire after changing the matter from Matter dropdownlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _cliMatDetails_MatterChanged(object sender, EventArgs e)
        {
            try
            {
                if (_cliMatDetails.ProjectId == DataConstants.DummyGuid)
                {
                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblError.CssClass = "errorMessage";
                            _lblError.Text = _cliMatDetails.Message;
                            return;
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
            AddDefaultToDropDownList(_ddlUsers, "Select");
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
                //set the Session for Project Id for Matter 
                Session[SessionName.TaskProjectId] = Session[SessionName.ProjectId];
                _hdnUser.Value = _ddlUsers.SelectedValue;

                BindTaskList();
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

        #region BindTaskList
        private void BindTaskList()
        {
            try
            {
                _grdSearchTaskList.PageIndex = 0;
                _grdSearchTaskList.DataSourceID = _odsSearchTask.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BindTaskStatus
        private void BindTaskStatus()
        {
            try
            {
                _ddlStatus.DataSource = DataTables.GetTaskStatus();
                _ddlStatus.DataTextField = "statusText";
                _ddlStatus.DataValueField = "statusValue";
                _ddlStatus.DataBind();
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
        protected void _grdSearchTaskList_RowDataBound(Object sender, GridViewRowEventArgs e)
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
                                img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchTaskList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                // checking if the header link is the user's choice
                                if (_grdSearchTaskList.SortExpression == lnk.CommandArgument)
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
                    //Truncate doc name descriptions
                    LinkButton linkSubject = (LinkButton)e.Row.FindControl("_linkSubject");
                    if (linkSubject != null)
                    {
                        if (linkSubject.Text.Length > 30)
                        {
                            linkSubject.Text = linkSubject.Text.Substring(0, 30) + "...";
                        }
                    }

                    //Truncate doc name descriptions
                    Label lblNotes = (Label)e.Row.FindControl("_lblNotes");
                    if (lblNotes != null)
                    {
                        if (lblNotes.Text.Length > 20)
                        {
                            lblNotes.Text = lblNotes.Text.Substring(0, 20) + "...";
                        }
                    }

                    Label lblDueDate = (Label)e.Row.FindControl("_lblDueDate");
                    if (lblDueDate != null)
                    {
                        if (lblDueDate.Text == DataConstants.BlankDate.ToString("dd/MM/yyyy"))
                        {
                            lblDueDate.Text = "(none)";
                        }
                    }

                    LinkButton linkDelete = (LinkButton)e.Row.FindControl("_linkDelete");
                    if (linkDelete != null)
                    {
                        Label lblId = (Label)e.Row.FindControl("_lblId");
                        Label lblIsLimitationTask = (Label)e.Row.FindControl("_lblIsLimitationTask");
                        bool isLimitationTask = false;
                        if (lblIsLimitationTask != null)
                        {
                            isLimitationTask = Convert.ToBoolean(lblIsLimitationTask.Text);
                        }
                        if (lblId != null)
                        {
                            linkDelete.Attributes.Add("onclick", "return showDiaryCancellationModalPopupViaClient('" + lblId.Text + "', '" + isLimitationTask + "');");
                        }
                    }

                    //Diable Edit/Delete button if user is not having rights
                    Label lblEdit = (Label)e.Row.FindControl("_lblEdit");
                    if (lblEdit != null)
                    {
                        if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                            _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                        {
                            lblEdit.Text = linkSubject.Text;
                            lblEdit.Visible = true;
                        }

                        linkSubject.Visible = !(lblEdit.Visible);
                        linkDelete.Visible = !(lblEdit.Visible);

                    }
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
        #endregion

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdSearchTaskList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                if (row.Cells[0].FindControl("_linkSubject") != null)
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    int taskId = Convert.ToInt32(_grdSearchTaskList.DataKeys[rowId].Values["Id"].ToString());

                    Session[SessionName.TaskId] = taskId;
                    Response.Redirect("~/Pages/Task/TaskDetails.aspx?PreviousPage=ViewMatterTasks.aspx", true);
                }
            }
        }
        #endregion

        #region Cancellation Finished Event
        /// <summary>
        /// This will fire after cancellation details has entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _dcTaskDelete_CancellationFinishedChanged(object sender, EventArgs e)
        {
            try
            {
            
                if (!string.IsNullOrEmpty(_dcTaskDelete.TaskAppointmentId.Trim()))
                {
                    // Delete Appointment
                    DeleteData deleteData = new DeleteData();
                    deleteData.CancellationText = _dcTaskDelete.CancellationText;
                    deleteData.ReasonCode = _dcTaskDelete.ReasonCode;
                    deleteData.CategoryCode = _dcTaskDelete.CategoryCode;
                    deleteData.OccurenceId = Convert.ToInt32(_dcTaskDelete.TaskAppointmentId);
                    if (_hdnUser.Value.Trim().Length > 0)
                    {
                        deleteData.MemberId = _hdnUser.Value;
                    }
                    else
                    {
                        deleteData.MemberId = Convert.ToString(_logonSettings.MemberId);
                    }
                    deleteData.IsBookingATask = true;

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
                                BindTaskList();

                                _lblError.CssClass = "successMessage";
                                _lblError.Text = "Task deleted successfully.";
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


        #region AddDefaultToDropDownList
        private void AddDefaultToDropDownList(DropDownList ddlList, string ddlValue)
        {
            ListItem listSelect = new ListItem(ddlValue, "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region BindGridView Service

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetTaskRowsCount(string taskStatus, string user, string fromDate, string toDate, bool forceRefresh)
        {
            //GetTaskRowsCount is directly called by the objectdatasource which expects a method 
            //with the same parameters as the method used to retrieve the data i.e SearchTasks

            //Task row count is saved when we retrieve the tasks based on the search criteria
            return _taskRowCount;
        }

        /// <summary>
        /// Searches for tasks that match the search criteria.
        /// </summary>
        public IRIS.Law.WebServiceInterfaces.Diary.Task[] SearchTask(int startRow, int pageSize, string sortBy, string taskStatus, string user, string fromDate, string toDate, bool forceRefresh)
        {
            DiaryServiceClient diaryService = null;
            IRIS.Law.WebServiceInterfaces.Diary.Task[] tasks = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    TaskSearchCriteria criteria = new TaskSearchCriteria();
                    criteria.MemberID = user;
                    criteria.OrderBy = sortBy;
                    if (Session[SessionName.ProjectId] != null)
                    {
                        criteria.ProjectID = new Guid(Session[SessionName.ProjectId].ToString());
                    }

                    if (!string.IsNullOrEmpty(fromDate))
                    {
                        criteria.StartDate = Convert.ToDateTime(fromDate);
                    }
                    else
                    {
                        criteria.StartDate = DataConstants.BlankDate;
                    }

                    if (!string.IsNullOrEmpty(toDate))
                    {
                        criteria.ToDate = Convert.ToDateTime(toDate);
                    }
                    else
                    {
                        criteria.ToDate = DataConstants.BlankDate;
                    }

                    if (!string.IsNullOrEmpty(taskStatus))
                    {
                        criteria.Status = taskStatus;
                    }

                    diaryService = new DiaryServiceClient();
                    TaskSearchReturnValue returnValue = diaryService.MatterTaskSearch(_logonId,
                                                collectionRequest, criteria);

                    if (returnValue.Success)
                    {
                        _taskRowCount = returnValue.Tasks.TotalRowCount;
                        tasks = returnValue.Tasks.Rows;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
                }
                return tasks;
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

        protected void _odsSearchTask_Selected(object sender, ObjectDataSourceStatusEventArgs e)
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
    }
}

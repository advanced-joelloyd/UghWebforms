using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using IRIS.Law.WebApp.App_Code;
using System.Data;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Web.UI.HtmlControls;

namespace IRIS.Law.WebApp.Pages.Task
{
    public partial class ViewAllTasks : BasePage
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

                    //Truncate doc name descriptions
                    Label lblProgress = (Label)e.Row.FindControl("_lblPercentComplete");
                    if (lblProgress != null)
                    {
                        lblProgress.Text = lblProgress.Text + " %";
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
                    //LinkButton linkEdit = (LinkButton)e.Row.FindControl("_linkEdit");
                    if (lblEdit != null)
                    {
                        if (lblEdit.Text.Trim().Length > 0)
                        {
                            linkSubject.Visible = Convert.ToBoolean(lblEdit.Text);
                            linkDelete.Visible = Convert.ToBoolean(lblEdit.Text);
                        }
                    }

                    LinkButton lnkMatterRef = (LinkButton)e.Row.FindControl("_lnkMatterRef");
                    if (lnkMatterRef.Text.Trim().Length > 0)
                    {
                        //Check if there are multiple matters or a single matter
                        string[] allMatters = lnkMatterRef.Text.Trim().Split('~');
                        if (allMatters.Length > 1)
                        {
                            //Multiple matters, display View matters link
                            lnkMatterRef.Text = "View Matters";
                        }
                        else
                        {
                            //single matter
                            string[] matter = allMatters[0].Split('$');
                            if (matter[1].Length > 20)
                            {
                                lnkMatterRef.Text = matter[1].Substring(0, 20) + "...";
                                lnkMatterRef.ToolTip = matter[1];
                            }
                            else
                            {
                                lnkMatterRef.Text = matter[1];
                            }
                        }
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
            try
            {
                if (e.CommandName == "select")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    if (row.Cells[0].FindControl("_linkSubject") != null)
                    {
                        int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                        int taskId = Convert.ToInt32(_grdSearchTaskList.DataKeys[rowId].Values["Id"].ToString());
                        //Store the Project Id of the first Matter from the list if exists
                        //If Matter does not exists set the session varibale to null
                        string taskProjectId = ((HiddenField)_grdSearchTaskList.Rows[rowId].Cells[0].FindControl("_hdnMatter")).Value;
                        if (taskProjectId == "")
                            taskProjectId = null;
                        else
                        {
                            int index = taskProjectId.IndexOf('$');
                            taskProjectId = taskProjectId.Substring(0,index);
                        }
                        Session[SessionName.TaskProjectId] = taskProjectId;
                        Session[SessionName.TaskId] = taskId;
                        Response.Redirect("~/Pages/Task/TaskDetails.aspx?PreviousPage=ViewAllTasks.aspx", true);
                    }
                }
                else if (e.CommandName == "ViewMatters")
                {
                    //Create a data table of the matters
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("MatterDesc");
                    dataTable.Columns.Add("ProjectId");

                    //Add a row for each matter
                    string[] allMatters = e.CommandArgument.ToString().Trim().Split('~');
                    string[] matter = null;
                    for (int i = 0; i < allMatters.Length; i++)
                    {
                        matter = allMatters[i].Split('$');
                        DataRow row = dataTable.NewRow();
                        row["MatterDesc"] = matter[1];
                        row["ProjectId"] = matter[0];
                        dataTable.Rows.Add(row);
                    }
                    //For more than 1 matter bind the datatable to the repeater to display as popup
                    //else on clicking the link redirect to the Edit Matter page 
                    if (allMatters.Length > 1)
                    {
                        //Bind the repeater
                        _rptMatters.DataSource = dataTable;
                        _rptMatters.DataBind();

                        //Display the popup
                        _mpeViewMatters.Show();
                    }
                    else
                    {
                        //Store the project id in the session
                        Guid projectId = new Guid(matter[0]);
                        Session[SessionName.ProjectId] = projectId;

                        //Redirect to the Edit Matter page
                        Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
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

        protected void _rptMatters_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            try
            {
                //Store the project id in the session
                Guid projectId = new Guid(e.CommandArgument.ToString());
                Session[SessionName.ProjectId] = projectId;

                //Redirect to the edit matter page
                Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
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
                    deleteData.MemberId = _hdnUser.Value;
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

            if (_ddlUsers.SelectedValue == "")
            {
                _ddlUsers.SelectedIndex = 0;
            }
        }
        #endregion

        #region BindGridView Service

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetTaskRowsCount(string taskStatus, string user, string fromDate, string toDate, bool forceRefresh)
        {
            //GetTaskRowsCount is directly called by the objectdatasource which expects a method 
            //with the same parameters as the method used to retrieve the data i.e SearchTask

            //Task row count is saved when we retrieve the tasks based on the search criteria
            return _taskRowCount;
        }

        /// <summary>
        /// Searches for task that match the search criteria.
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
                    TaskSearchReturnValue returnValue = diaryService.MemberTaskSearch(_logonId,
                                                collectionRequest, criteria);

                    if (returnValue.Success)
                    {
                        _taskRowCount = returnValue.Tasks.TotalRowCount;
                        tasks = returnValue.Tasks.Rows;
                    }
                    else
                    {
                        if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                            throw new Exception("Date is invalid");
                        else
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

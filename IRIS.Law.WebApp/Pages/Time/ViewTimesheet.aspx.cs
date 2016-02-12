using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Time;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Earner;
using System.Configuration;
using System.Collections;

namespace IRIS.Law.WebApp.Pages.Time
{
    public partial class ViewTimesheet : BasePage
    {
        #region Private variable

        /// <summary>
        /// Variable to hold the loggin settings information
        /// </summary>
        private LogonReturnValue _logonSettings;

        /// <summary>
        /// Keeps a running count of the charge rate
        /// </summary>
        private decimal _charge;

        /// <summary>
        /// Keeps track of the number of entries in the timesheet
        /// </summary>
        private int _rowCount;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];
            
            _lblMessage.Text = string.Empty;

            //Set the page size for the grids
            _grdTodaysTimesheet.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
        }

        #region Control Events

        protected void _grdTodaysTimesheet_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "EditMatter")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    Guid projectId = new Guid(_grdTodaysTimesheet.DataKeys[row.RowIndex].Values["ProjectId"].ToString());
                    Session[SessionName.ProjectId] = projectId;
                    Response.Redirect("~/Pages/Matter/EditMatter.aspx", true);
                }
                else if (e.CommandName == "select")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    int timeId = Convert.ToInt32(_grdTodaysTimesheet.DataKeys[row.RowIndex].Values["TimeId"].ToString());
                    Session[SessionName.TimeId] = timeId;
                    Session[SessionName.MatterReference] = ((LinkButton)_grdTodaysTimesheet.Rows[row.RowIndex].FindControl("_lnkBtnEditMatter")).Text;
                    Response.Redirect("~/Pages/Time/AddTimeEntry.aspx?edit=true", true);
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

        protected void _grdTodaysTimesheet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                   

                    foreach (TableCell tc in e.Row.Cells)
                    {

                        if (tc.Controls.Count==1)
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
                                    img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdTodaysTimesheet.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                    // checking if the header link is the user's choice
                                    if (_grdTodaysTimesheet.SortExpression == lnk.CommandArgument)
                                    {
                                        // adding a space and the image to the header link
                                        tc.Controls.Add(new LiteralControl(" "));
                                        tc.Controls.Add(img);
                                    }
                                }
                            }
                        }
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    UnPostedTimeSearchItem timeEntry = (UnPostedTimeSearchItem)e.Row.DataItem;

                    LinkButton lnkMatterReference = (LinkButton)e.Row.FindControl("_lnkBtnEditMatter");
                    lnkMatterReference.Text = lnkMatterReference.Text.Insert(6, "-");

                    //Truncate large descriptions
                    if (timeEntry.MatterDescription.Length > 20)
                    {
                        Label lbldescription = (Label)e.Row.FindControl("_lblMatterDescription");
                        lbldescription.Text = timeEntry.MatterDescription.Substring(0, 20) + "...";
                    }

                    //Truncate large notes
                    if (timeEntry.TimeComments.Length > 20)
                    {
                        Label lblNotes = (Label)e.Row.FindControl("_lblNotes");
                        lblNotes.Text = timeEntry.TimeComments.Substring(0, 20) + "...";
                    }

                    //Add to the total cost (displayed in the footer)
                    _charge += timeEntry.TimeCharge;

                    Label lblUnits = (Label)e.Row.FindControl("_lblUnits");
                    lblUnits.Text = (timeEntry.TimeElapsed / _logonSettings.TimeUnits).ToString();

                    Label lblTime = (Label)e.Row.FindControl("_lblTime");
                    lblTime.Text = AppFunctions.ConvertUnits(timeEntry.TimeElapsed);
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    ((Label)e.Row.FindControl("_lblTotal")).Text = "&pound;" + _charge.ToString("0.00");
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

        #region Bind Grid

        /// <summary>
        /// The list of time entries for today
        /// </summary>
        public UnPostedTimeSearchItem[] BindTodaysTimesheet(int startRow, int pageSize, string sortBy, bool forceRefresh)
        {
            UnPostedTimeSearchItem[] timesheet = null;

            TimeServiceClient timeService = null;
            EarnerServiceClient earnerService = null;
            try
            {

                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    LogonReturnValue logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                    earnerService = new EarnerServiceClient();

                    EarnerReturnValue earnerReturnValue = earnerService.GetFeeEarnerReference(logonSettings.LogonId, logonSettings.MemberId);

                    if (earnerReturnValue.Success)
                    {
                        timeService = new TimeServiceClient();
                        CollectionRequest collectionRequest = new CollectionRequest();
                        collectionRequest.ForceRefresh = forceRefresh;
                        collectionRequest.StartRow = startRow;
                        collectionRequest.RowCount = pageSize;

                        UnPostedTimeSearchCriteria searchCriteria = new UnPostedTimeSearchCriteria();
                        searchCriteria.FeeEarnerRef = earnerReturnValue.EarnerRef;
                        searchCriteria.UserId = logonSettings.DbUid;
                        searchCriteria.TimeDate = DateTime.Now.Date;
                        searchCriteria.OrderBy = sortBy;

                        UnPostedTimeSearchReturnValue returnValue = timeService.UnPostedTimeSheetSearch(logonSettings.LogonId,
                                                        collectionRequest, searchCriteria);

                        if (returnValue.Success)
                        {
                            _rowCount = returnValue.UnPostedTimeSheet.TotalRowCount;
                            timesheet = returnValue.UnPostedTimeSheet.Rows;
                        }
                        else
                        {
                            throw new Exception(returnValue.Message);
                        }
                    }
                    else
                    {
                        //Error retrieveing earner ref. wont be able to get the time sheet
                        throw new Exception("Error retrieving timesheet");
                    }
                }
                return timesheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (timeService != null)
                {
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                        timeService.Close();
                }

                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetTimesheetRowsCount(bool forceRefresh)
        {
            //row count is saved when we retrieve the timesheet
            return _rowCount;
        }

        protected void _odsTimesheet_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding timesheet
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

        protected void _btnPostSelected_Click(object sender, EventArgs e)
        {
            try
            {
                PostTime(true);

                //Refresh the timesheet
                _hdnRefresh.Value = "true";
                _grdTodaysTimesheet.PageIndex = 0;
                _grdTodaysTimesheet.DataSourceID = "_odsTimesheet";
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

            _btnPostSelected.Enabled = false;
        }

        protected void _btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                PostTime(false);
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

        #region Post Time
        /// <summary>
        /// This method will post the selected timesheet entries for accounting
        /// </summary>
        /// <param name="displayWarnings">if set to <c>true</c> display warnings.</param>
        private void PostTime(bool displayWarnings)
        {
            TimeServiceClient timeService = null;
            EarnerServiceClient earnerService = null;
            try
            {
                UnPostedTimeSearchCriteria searchCriteria = new UnPostedTimeSearchCriteria();
                searchCriteria.UserId = _logonSettings.DbUid;
                searchCriteria.TimeDate = DateTime.Now.Date;

                earnerService = new EarnerServiceClient();
                EarnerReturnValue earnerReturnVal = earnerService.GetFeeEarnerReference(_logonSettings.LogonId,
                                                                                        _logonSettings.MemberId);
                if (earnerReturnVal.Success)
                {
                    searchCriteria.FeeEarnerRef = earnerReturnVal.EarnerRef;
                }
                else
                {
                    throw new Exception(earnerReturnVal.Message);
                }


                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = _grdTodaysTimesheet.PageIndex * _grdTodaysTimesheet.PageSize;
                collectionRequest.RowCount = _grdTodaysTimesheet.PageSize;

                //Get unposted time entries
                timeService = new TimeServiceClient();
                UnPostedTimeSearchReturnValue returnValue = timeService.UnPostedTimeSheetSearch(_logonSettings.LogonId,
                                                                                            collectionRequest,
                                                                                            searchCriteria);

                if (returnValue.UnPostedTimeSheet.Rows != null && returnValue.UnPostedTimeSheet.Rows.Length > 0)
                {
                    foreach (GridViewRow row in _grdTodaysTimesheet.Rows)
                    {
                        CheckBox timeSheetSelected = (CheckBox)row.FindControl("_chkSelect");
                        int timeId = (int)_grdTodaysTimesheet.DataKeys[row.RowIndex].Values["TimeId"];
                        
                        //Find the item in the collection 
                        UnPostedTimeSearchItem timeSheetItem = returnValue.UnPostedTimeSheet.Rows.First(time => time.TimeId == timeId);
                                               

                        //Check if the item is selected
                        if (timeSheetSelected.Checked)
                        {

                            //Validate the posting period
                            PeriodCriteria criteria = new PeriodCriteria();
                            criteria.Date = timeSheetItem.TimeDate;//returnValue.UnPostedTimeSheet.Rows[0].TimeDate;
                            criteria.IsTime = true;
                            criteria.IsPostingVATable = false;
                            criteria.IsAllowedPostBack2ClosedYear = false;

                            PeriodDetailsReturnValue periodDetailsReturnValue = new PeriodDetailsReturnValue();
                            periodDetailsReturnValue = timeService.ValidatePeriod(_logonSettings.LogonId, criteria);

                            if (periodDetailsReturnValue.Success)
                            {
                                //Display warning mesg(if any)
                                if (periodDetailsReturnValue.PeriodStatus == 3 && displayWarnings)
                                {
                                    _mdlPopUpCofirmationBox.Show();
                                    return;
                                }

                                if (periodDetailsReturnValue.PeriodStatus == 2)
                                {
                                    throw new Exception(periodDetailsReturnValue.ErrorMessage);
                                }
                                                                
                                if (timeSheetItem != null)
                                {
                                    bool canBePosted = true;

                                    if (timeSheetItem.BillingTypeActive &&
                                        timeSheetItem.BillingTypeArchived == false &&
                                        timeSheetItem.TimeLAAsked == false)
                                    {
                                        canBePosted = false;
                                    }

                                    if (canBePosted)
                                    {
                                        TimeSheet timeSheet = new TimeSheet();
                                        timeSheet.TimeId = timeSheetItem.TimeId;
                                        timeSheet.PeriodId = periodDetailsReturnValue.PeriodId;
                                        timeSheet.MemberId = timeSheetItem.MemberId.ToString();
                                        timeSheet.CurrencyId = timeSheetItem.CurrencyId;
                                        timeSheet.PeriodMinutes = timeSheetItem.TimeElapsed;
                                        timeSheet.MasterPostedCost = timeSheetItem.TimeCost;
                                        timeSheet.MasterPostedCharge = timeSheetItem.TimeCharge;
                                        timeSheet.WorkingPostedCost = timeSheetItem.TimeCost;
                                        timeSheet.WorkingPostedCharge = timeSheetItem.TimeCharge;
                                        timeSheet.OrganisationId = timeSheetItem.OrganisationId;
                                        timeSheet.DepartmentId = timeSheetItem.DepartmentId;
                                        timeSheet.ProjectId = timeSheetItem.ProjectId;
                                        timeSheet.TimeTypeId = timeSheetItem.TimeTypeId;
                                        timeSheet.TimeDate = timeSheetItem.TimeDate;

                                        ReturnValue returnVal = timeService.PostTime(_logonSettings.LogonId, timeSheet);
                                        if (!returnVal.Success)
                                        {
                                            throw new Exception(returnVal.Message);
                                        }
                                    }
                                }


                            }
                            else
                            {
                                throw new Exception(periodDetailsReturnValue.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (timeService != null)
                {
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                        timeService.Close();
                }
                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }

        #endregion
    }
}
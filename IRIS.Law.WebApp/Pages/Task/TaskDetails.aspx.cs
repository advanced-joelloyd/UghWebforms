using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Data;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces.Earner;

namespace IRIS.Law.WebApp.Pages.Task
{
    public partial class TaskDetails : BasePage
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        LogonReturnValue _logonSettings;

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                
                _lblError.CssClass = "errorMessage";
                _lblError.Text = string.Empty;

                if (!IsPostBack)
                {

                    _txtAttendees.ReadOnly = true;
                    BindTaskTypes();
                    ResetControls();

                    if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["PreviousPage"])))
                    {
                        ViewState["PreviousPage"] = Request.QueryString["PreviousPage"];
                    }

                    if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                        _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        _chkExposeToThirdParties.Checked = true;
                        _chkExposeToThirdParties.Enabled = false;
                        
                    }

                    if (Session[SessionName.TaskId] != null)
                    {
                        _divBackButton.Visible = true;
                        _divNew.Visible = true;
                        _hdnTaskId.Value = Convert.ToString(Session[SessionName.TaskId]);
                        LoadTaskDetails();
                        Session[SessionName.TaskId] = null;
                        Session[SessionName.TaskProjectId] = null;
                    }
                    else
                    {
                        _divNew.Visible = false;
                        _divBackButton.Visible = false;
                        _hdnTaskId.Value = "0";
                        _ccDueDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");

                        GetDefaultFeeEarnerDetails();
                        _TaskOU.CurrentUsers = _txtAttendees.Text;
                        _TaskOU.CurrentUsersID = _hdnAttendeesMemberId.Value;

                        _cliMatDetails.SetSession = true;

                        
                    }

                    if (_logonSettings.UserType == (int)DataConstants.UserType.Client || _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        _cliMatDetails.LoadClientMatterDetails();
                        _TaskOU.Visible = false;

                        if (Session[SessionName.ProjectId] != null)
                        {

                            LoadMatterFeeEarner(new Guid(Convert.ToString(Session[SessionName.ProjectId])));
                        }
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(this.Session[SessionName.ProjectId])))
                    {
                        this._hdnProjectId.Value = Convert.ToString(this.Session[SessionName.ProjectId]);
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
            _txtAttendees.Text = string.Empty;
            _hdnAttendeesMemberId.Value = string.Empty;
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
                            _txtAttendees.Text = _txtAttendees.Text + returnValue.DiaryMembers.Rows[i].MemberDisplayName + "; ";
                            _hdnAttendeesMemberId.Value += _hdnAttendeesMemberId.Value + returnValue.DiaryMembers.Rows[i].MemberID + "; ";
                        }
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
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

        #region LoadTaskDetails
        private void LoadTaskDetails()
        {
            Guid projectId = DataConstants.DummyGuid;
            if (_hdnTaskId.Value.Trim().Length > 0)
            {
                DiaryServiceClient diaryService = new DiaryServiceClient();
                try
                {
                    TaskReturnValue  taskReturnValue = new TaskReturnValue();
                    //If Project Id for matter is null call method GetMemberTaskDetails, else call GetMatterTaskDetails
                    if (Session[SessionName.TaskProjectId] == null)
                    {
                        taskReturnValue = diaryService.GetMemberTaskDetails(_logonSettings.LogonId, Convert.ToInt32(_hdnTaskId.Value));
                    }
                    else
                    {
                        taskReturnValue = diaryService.GetMatterTaskDetails(_logonSettings.LogonId, new Guid(Convert.ToString(Session[SessionName.TaskProjectId])), Convert.ToInt32(_hdnTaskId.Value));
                    }
                    if (taskReturnValue.Success)
                    {
                        if (taskReturnValue != null)
                        {
                            _txtAttendees.Text = taskReturnValue.Task.AttendeesName;
                            _hdnAttendeesMemberId.Value = taskReturnValue.Task.Attendees;
                            _TaskOU.CurrentUsers = taskReturnValue.Task.AttendeesName;
                            _TaskOU.CurrentUsersID = taskReturnValue.Task.Attendees;
                            _txtSubject.Text = taskReturnValue.Task.Subject;
                            if (taskReturnValue.Task.DueDate != DataConstants.BlankDate)
                            {
                                _ccDueDate.DateText = Convert.ToString(taskReturnValue.Task.DueDate);
                            }
                            else
                            {
                                _ccDueDate.DateText = string.Empty;
                            }
                            _chkCompleted.Checked = taskReturnValue.Task.IsCompleted;
                            _chkExposeToThirdParties.Checked = taskReturnValue.Task.IsPublic;

                            if (_ddlType.Items.FindByValue(taskReturnValue.Task.TypeId.ToString()) != null)
                            {
                                _ddlType.SelectedIndex = -1;
                                _ddlType.Items.FindByValue(taskReturnValue.Task.TypeId.ToString()).Selected = true;
                            }

                            _txtNotes.Text = taskReturnValue.Task.Notes;

                            projectId = taskReturnValue.Task.ProjectId;
                            _hdnProjectId.Value = Convert.ToString(projectId);
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(taskReturnValue.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }

                try
                {
                    if (projectId != DataConstants.DummyGuid)
                    {
                        //ViewState["TaskProjectId"] = projectId;
                        _cliMatDetails.ProjectId = projectId;
                        LoadClientMatterDetails(projectId);
                    }
                    else
                    {
                        _cliMatDetails.LoadData = false;
                        //ViewState["TaskProjectId"] = DataConstants.DummyGuid.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region LoadClientMatterDetails
        private void LoadClientMatterDetails(Guid projectId)
        {
            try
            {
                MatterServiceClient matterService = new MatterServiceClient();
                try
                {
                    MatterReturnValue matterReturnValue = new MatterReturnValue();
                    matterReturnValue = matterService.GetMatter(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, projectId);

                    if (matterReturnValue.Success)
                    {
                        if (matterReturnValue != null)
                        {
                            #region Load Client Details
                            // Store ClientID
                            if (matterReturnValue.ClientDetails.IsMember)
                            {
                                _cliMatDetails.MemberId = matterReturnValue.ClientDetails.MemberId;
                                _cliMatDetails.OrganisationId = DataConstants.DummyGuid;
                            }
                            else
                            {
                                _cliMatDetails.MemberId = DataConstants.DummyGuid;
                                _cliMatDetails.OrganisationId = matterReturnValue.ClientDetails.MemberId;
                            }

                            _cliMatDetails.IsClientMember = matterReturnValue.ClientDetails.IsMember;
                            _cliMatDetails.ClientRef = matterReturnValue.ClientDetails.Reference;
                            _cliMatDetails.ClientName = matterReturnValue.ClientDetails.FullName;

                            if (_cliMatDetails.Message != null)
                            {
                                if (_cliMatDetails.Message.Trim().Length > 0)
                                {
                                    throw new Exception("Loading failed for Client Matter Details.<br>Exception occured is: " + _cliMatDetails.Message);
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(matterReturnValue.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                        matterService.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ResetControls
        private void ResetControls()
        {
            try
            {
                GetDefaultFeeEarnerDetails();
                _TaskOU.CurrentUsers = _txtAttendees.Text;
                _TaskOU.CurrentUsersID = _hdnAttendeesMemberId.Value;

                _txtSubject.Text = string.Empty;
                _ccDueDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                _ddlType.SelectedIndex = 0;
                _chkCompleted.Checked = false;
                _chkExposeToThirdParties.Checked = false;
                _txtNotes.Text = string.Empty;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }
        #endregion

        #region BindTaskTypes
        private void BindTaskTypes()
        {
            DiaryServiceClient diaryService = null;
            try
            {
                diaryService = new DiaryServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                DiaryParameterReturnValue returnValue = new DiaryParameterReturnValue();
                returnValue = diaryService.GetTaskTypes(_logonSettings.LogonId, collectionRequest);

                if (returnValue.Success)
                {
                    _ddlType.DataSource = returnValue.DiaryParamters.Rows;
                    _ddlType.DataTextField = "Description";
                    _ddlType.DataValueField = "Id";
                    _ddlType.DataBind();

                    if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                        _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                    {
                        for (int i = 0; i < _ddlType.Items.Count - 1; i++)
                        {
                            if (_ddlType.Items[i].Text != "Standard Task")
                            {
                                _ddlType.Items.RemoveAt(i);
                            }
                        }

                        _ddlType.Enabled = false;
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

        #region New Button

        protected void _btnNew_Click(object sender, EventArgs e)
        {
            Session[SessionName.TaskId] = null;
            Response.Redirect("TaskDetails.aspx", true);
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
                    _hdnProjectId.Value = Convert.ToString(DataConstants.DummyGuid);
                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblError.CssClass = "errorMessage";
                            _lblError.Text = _cliMatDetails.Message;
                            this.taskdetails.Visible = false;
                            return;
                        }
                    }
                }
                else
                {
                    _hdnProjectId.Value = Convert.ToString(_cliMatDetails.ProjectId);
                }

                if (_logonSettings.UserType == (int)DataConstants.UserType.Client || _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                {
                    LoadMatterFeeEarner(new Guid(Convert.ToString(_cliMatDetails.ProjectId)));
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

        #region _TaskOU_UsersAdded
        protected void _TaskOU_UsersAdded(object sender, EventArgs e)
        {
            try
            {
                _txtAttendees.Text = _TaskOU.CurrentUsers;
                _hdnAttendeesMemberId.Value = _TaskOU.CurrentUsersID;
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region Back Button
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(ViewState["PreviousPage"])))
            {
                Response.Redirect(Convert.ToString(ViewState["PreviousPage"]), true);
            }
            else
            {
                Response.Redirect("ViewAllTasks.aspx", true);
            }
        }
        #endregion

        #region GetControlData
        private IRIS.Law.WebServiceInterfaces.Diary.Task GetControlData()
        {
            IRIS.Law.WebServiceInterfaces.Diary.Task taskDetails = new IRIS.Law.WebServiceInterfaces.Diary.Task();
            try
            {
                if (_hdnTaskId.Value.Trim().Length > 0)
                {
                    taskDetails.Id = Convert.ToInt32(_hdnTaskId.Value);
                }
                else
                {
                    taskDetails.Id = 0;
                }

                taskDetails.ProjectId = DataConstants.DummyGuid;

                if (_hdnProjectId.Value.Length > 0)
                {
                    taskDetails.ProjectId = new Guid(_hdnProjectId.Value);
                }

                if (_txtAttendees.Text.Length > 0)
                {
                    taskDetails.Attendees = _hdnAttendeesMemberId.Value;
                }

                try
                {
                    if (!string.IsNullOrEmpty(_ccDueDate.DateText))
                    {
                        taskDetails.DueDate = Convert.ToDateTime(_ccDueDate.DateText);
                    }
                    else
                    {
                        taskDetails.DueDate = DataConstants.BlankDate;
                    }
                }
                catch
                {
                    throw new Exception("Invalid Due Date");
                }

                taskDetails.Notes = _txtNotes.Text;
                taskDetails.Subject = _txtSubject.Text;
                taskDetails.IsCompleted = _chkCompleted.Checked;
                taskDetails.IsPublic = _chkExposeToThirdParties.Checked;
                taskDetails.TypeId = Convert.ToInt32(_ddlType.SelectedValue);

                taskDetails.IsReminderSet = false;
                taskDetails.IsPrivate = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return taskDetails;
        }
        #endregion

        #region CheckMandatoryFields
        private string CheckMandatoryFields()
        {
            string errorMessage = string.Empty;
            try
            {
                if (_txtAttendees.Text.Trim().Length == 0)
                {
                    errorMessage += "Attendees is mandatory <br/>";
                }
                if (_txtSubject.Text.Trim().Length == 0)
                {
                    errorMessage += "Subject is mandatory <br/>";
                }

                DateTime chkDate;
    
                if (!DateTime.TryParse(_ccDueDate.DateText, out chkDate))
                {
                    errorMessage += "Due Date is invalid <br/>";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return errorMessage;
        }
        #endregion

        #region Save
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            string errorMessage = CheckMandatoryFields();

            if (string.IsNullOrEmpty(errorMessage))
            {
                DiaryServiceClient diaryService = null;
                try
                {
                    IRIS.Law.WebServiceInterfaces.Diary.Task taskDetails = GetControlData();

                    diaryService = new DiaryServiceClient();
                    TaskReturnValue returnValue = diaryService.SaveTask(_logonSettings.LogonId, taskDetails);

                    if (returnValue.Success)
                    {
                        _hdnTaskId.Value = Convert.ToString(returnValue.Task.Id);

                        _lblError.CssClass = "successMessage";
                        _lblError.Text = "Task Saved Successfully.";
                    }
                    else
                    {
                        _lblError.CssClass = "errorMessage";

                        if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                            _lblError.Text = "Due Date is invalid";
                        else
                            _lblError.Text = returnValue.Message;
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
                finally
                {
                    if (diaryService != null)
                    {
                        if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                            diaryService.Close();
                    }
                }
            }
            else
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = errorMessage;
            }
        }
        #endregion

        #region Reset Button
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                //ResetControls();
                Response.Redirect("~/Pages/Task/TaskDetails.aspx?PreviousPage=" + Convert.ToString(ViewState["PreviousPage"]), true);
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region LoadMatterFeeEarner
        private void LoadMatterFeeEarner(Guid ProjectID)
        {
              
            MatterServiceClient matterService = new MatterServiceClient();
            try
            {
                MatterReturnValue matterReturnValue = new MatterReturnValue();
                matterReturnValue = matterService.GetMatter(_logonSettings.LogonId, ProjectID);

                if (matterReturnValue.Success)
                {
                    if (matterReturnValue != null)
                    {
                        if (matterReturnValue.Matter.FeeEarnerMemberId != null)
                        {
                            EarnerServiceClient partnerClient = new EarnerServiceClient();
                            try
                            {
                                PartnerSearchCriteria partnerCriteria = new PartnerSearchCriteria();
                                CollectionRequest collectionRequest = new CollectionRequest();
                                collectionRequest.StartRow = 0;

                                PartnerSearchReturnValue partnerReturnValue = partnerClient.PartnerSearch(_logonSettings.LogonId, collectionRequest, partnerCriteria);

                                if (partnerReturnValue.Success)
                                {
                                    if (partnerReturnValue.Partners != null)
                                    {
                                        for (int i = 0; i < partnerReturnValue.Partners.Rows.Length; i++)
                                        {
                                            if (partnerReturnValue.Partners.Rows[i].PartnerId.ToString() == matterReturnValue.Matter.FeeEarnerMemberId.ToString())
                                            {
                                                _txtAttendees.Text = CommonFunctions.MakeFullName(partnerReturnValue.Partners.Rows[i].PersonTitle, partnerReturnValue.Partners.Rows[i].Name, partnerReturnValue.Partners.Rows[i].Surname);
                                                _hdnAttendeesMemberId.Value = partnerReturnValue.Partners.Rows[i].PartnerId.ToString() + ";";
                                            }
                                           
                                            
                                        }
                                    }
                                }
                                else
                                {
                                    _lblError.Text = partnerReturnValue.Message;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                if (partnerClient.State != System.ServiceModel.CommunicationState.Faulted)
                                    partnerClient.Close();
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
                if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                    matterService.Close();
            }


        }
        #endregion
    }
}

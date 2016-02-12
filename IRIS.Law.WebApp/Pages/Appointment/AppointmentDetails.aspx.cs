using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;

namespace IRIS.Law.WebApp.Pages.Appointment
{
    public partial class AppointmentDetails : BasePage
    {
        #region EditMode
        LogonReturnValue _logonSettings;
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            

            _lblError.CssClass = "errorMessage";
            _lblError.Text = string.Empty;

            if (!IsPostBack)
            {
                _txtAttendees.ReadOnly = true;
                BindReminderState();
                BindReminderBeforeTime();
                ResetControls();

                if (Session[SessionName.AppointmentId] != null)
                {
                    _divNew.Visible = true;
                    _divBackButton.Visible = true;
                    _hdnAppointmentId.Value = Convert.ToString(Session[SessionName.AppointmentId]);
                    LoadAppointmentDetails();
                    Session[SessionName.AppointmentId] = null;
                    _divAppointments.Visible = false;
                }
                else
                {
                    _divAppointments.Visible = true;
                    _divNew.Visible = false;
                    _hdnAppointmentId.Value = "0";
                    _divBackButton.Visible = false;
                    _ccDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");

                    
                    GetDefaultFeeEarnerDetails();
                    _AppointmentOU.CurrentUsers = _txtAttendees.Text;
                    _AppointmentOU.CurrentUsersID = _hdnAttendeesMemberId.Value;

                    _cliMatDetails.SetSession = true;
                    if (!string.IsNullOrEmpty(Convert.ToString(Session[SessionName.ProjectId])))
                    {
                        _hdnProjectId.Value = Convert.ToString(Session[SessionName.ProjectId]);
                    }
                }                
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
                        if (_logonSettings.MemberId.ToString() == returnValue.DiaryMembers.Rows[i].MemberID)
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

        #region LoadAppointmentDetails
        private void LoadAppointmentDetails()
        {
            Guid projectId = DataConstants.DummyGuid;
            _hdnProjectId.Value = Convert.ToString(DataConstants.DummyGuid);
            if (_hdnAppointmentId.Value.Trim().Length > 0)
            {
                DiaryServiceClient diaryService = new DiaryServiceClient();
                try
                {
                    AppointmentReturnValue appointmentReturnValue = new AppointmentReturnValue();
                    appointmentReturnValue = diaryService.GetAppointmentDetails(_logonSettings.LogonId, Convert.ToInt32(_hdnAppointmentId.Value));

                    if (appointmentReturnValue.Success)
                    {
                        if (appointmentReturnValue != null)
                        {
                            _txtAttendees.Text = appointmentReturnValue.Appointment.AttendeesName;
                            _hdnAttendeesMemberId.Value = appointmentReturnValue.Appointment.Attendees;
                            _AppointmentOU.CurrentUsers = appointmentReturnValue.Appointment.AttendeesName;
                            _AppointmentOU.CurrentUsersID = appointmentReturnValue.Appointment.Attendees;
                            _txtSubject.Text = appointmentReturnValue.Appointment.Subject;
                            _ssAppointmentVenue.ServiceText = appointmentReturnValue.Appointment.VenueText;
                            _hdnVenueId.Value = Convert.ToString(appointmentReturnValue.Appointment.VenueId);
                            _ccDate.DateText = Convert.ToString(appointmentReturnValue.Appointment.StartDate);
                            _tcStartTime.TimeText = appointmentReturnValue.Appointment.StartTime;
                            _tcEndTime.TimeText = appointmentReturnValue.Appointment.EndTime;

                            _chkReminder.Checked = appointmentReturnValue.Appointment.IsReminderSet;

                            if (appointmentReturnValue.Appointment.IsReminderSet)
                            {
                                _ddlReminder.SelectedIndex = -1;
                                if (_ddlReminder.Items.FindByValue(appointmentReturnValue.Appointment.ReminderType) != null)
                                {
                                    _ddlReminder.Items.FindByValue(appointmentReturnValue.Appointment.ReminderType).Selected = true;
                                }

                                _ddlReminderBeforeTime.SelectedIndex = -1;
                                if (_ddlReminderBeforeTime.Items.FindByValue(appointmentReturnValue.Appointment.ReminderBeforeTime) != null)
                                {
                                    _ddlReminderBeforeTime.Items.FindByValue(appointmentReturnValue.Appointment.ReminderBeforeTime).Selected = true;
                                }

                                if (appointmentReturnValue.Appointment.ReminderType == "On")
                                {
                                    _tdReminderOn.Style["display"] = "";
                                    _tdReminderBefore.Style["display"] = "none";
                                }
                                else
                                {
                                    _tdReminderOn.Style["display"] = "none";
                                    _tdReminderBefore.Style["display"] = "";
                                }

                                _ccReminderDate.DateText = Convert.ToString(appointmentReturnValue.Appointment.ReminderDate);
                                _tcReminderTime.TimeText = appointmentReturnValue.Appointment.ReminderTime;
                            }
                            else
                            {
                                _ddlReminder.SelectedIndex = -1;
                                _ddlReminderBeforeTime.SelectedIndex = -1;
                                _ccReminderDate.DateText = string.Empty;
                                _tcReminderTime.TimeText = string.Empty;
                            }

                            _txtNotes.Text = appointmentReturnValue.Appointment.Notes;

                            projectId = appointmentReturnValue.Appointment.ProjectId;
                            _hdnProjectId.Value = Convert.ToString(projectId);

                            HideUnhideReminderControls();
                        }
                        else
                        {
                            throw new Exception("Load failed.");
                        }
                    }
                    else
                    {
                        throw new Exception(appointmentReturnValue.Message);
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
                        _cliMatDetails.ProjectId = projectId;
                        LoadClientMatterDetails(projectId);
                    }
                    else
                    {
                        _cliMatDetails.LoadData = false;
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

        #region BindReminderState
        private void BindReminderState()
        {
            try
            {
                ListItem itemOn = new ListItem("On");
                ListItem itemBefore = new ListItem("Before");
                _ddlReminder.Items.Add(itemOn);
                _ddlReminder.Items.Add(itemBefore);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BindReminderBeforeTime
        private void BindReminderBeforeTime()
        {
            try
            {
                ListItem item15Mins = new ListItem("15 minutes", Convert.ToString(new decimal(new int[] {15, 0, 0, 0})));
                ListItem item30Mins = new ListItem("30 minutes", Convert.ToString(new decimal(new int[] { 30, 0, 0, 0 })));
                ListItem item1Hour = new ListItem("1 Hour", Convert.ToString(new decimal(new int[] { 60, 0, 0, 0 })));
                ListItem item2Hour = new ListItem("2 Hours", Convert.ToString(new decimal(new int[] { 120, 0, 0, 0 })));
                ListItem item3Hour = new ListItem("3 Hours", Convert.ToString(new decimal(new int[] { 180, 0, 0, 0 })));
                ListItem item4Hour = new ListItem("4 Hours", Convert.ToString(new decimal(new int[] { 240, 0, 0, 0 })));
                ListItem item1Day = new ListItem("1 Day", Convert.ToString(new decimal(new int[] { 1440, 0, 0, 0 })));
                ListItem item2Day = new ListItem("2 Days", Convert.ToString(new decimal(new int[] { 2880, 0, 0, 0 })));
                ListItem item1Week = new ListItem("1 Week", Convert.ToString(new decimal(new int[] { 10080, 0, 0, 0 })));
                ListItem item2Week = new ListItem("2 Weeks", Convert.ToString(new decimal(new int[] { 20160, 0, 0, 0 })));
                _ddlReminderBeforeTime.Items.Add(item15Mins);
                _ddlReminderBeforeTime.Items.Add(item30Mins);
                _ddlReminderBeforeTime.Items.Add(item1Hour);
                _ddlReminderBeforeTime.Items.Add(item2Hour);
                _ddlReminderBeforeTime.Items.Add(item3Hour);
                _ddlReminderBeforeTime.Items.Add(item4Hour);
                _ddlReminderBeforeTime.Items.Add(item1Day);
                _ddlReminderBeforeTime.Items.Add(item2Day);
                _ddlReminderBeforeTime.Items.Add(item1Week);
                _ddlReminderBeforeTime.Items.Add(item2Week);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetControlData
        private IRIS.Law.WebServiceInterfaces.Diary.Appointment GetControlData()
        {
            IRIS.Law.WebServiceInterfaces.Diary.Appointment appointmentDetails = new IRIS.Law.WebServiceInterfaces.Diary.Appointment();
            try
            {
                if (_hdnAppointmentId.Value.Trim().Length > 0)
                {
                    appointmentDetails.Id = Convert.ToInt32(_hdnAppointmentId.Value);
                }
                else
                {
                    appointmentDetails.Id = 0;
                }

                appointmentDetails.ProjectId = DataConstants.DummyGuid;

                if (_hdnProjectId.Value.Length > 0)
                {
                    appointmentDetails.ProjectId = new Guid(_hdnProjectId.Value);
                }

                if (_txtAttendees.Text.Length > 0)
                {
                    appointmentDetails.Attendees = _hdnAttendeesMemberId.Value;
                }

                if (string.IsNullOrEmpty(_tcStartTime.TimeText))
                {
                    appointmentDetails.StartTime = ":";
                }
                else
                {
                    appointmentDetails.StartTime = _tcStartTime.TimeText;
                }

                if (string.IsNullOrEmpty(_tcEndTime.TimeText))
                {
                    appointmentDetails.EndTime = ":";
                }
                else
                {
                    appointmentDetails.EndTime = _tcEndTime.TimeText;
                }
                try
                {
                    appointmentDetails.StartDate = Convert.ToDateTime(_ccDate.DateText);
                   
                }
                catch
                {
                    throw new Exception("Invalid Date");
                }
                appointmentDetails.Notes = _txtNotes.Text;
                appointmentDetails.Subject = _txtSubject.Text;
                if(_hdnVenueId.Value.Trim().Length > 0)
                {
                    appointmentDetails.VenueId = new Guid(_hdnVenueId.Value);
                }

                if (_chkReminder.Checked)
                {
                    appointmentDetails.IsReminderSet = _chkReminder.Checked;
                    appointmentDetails.ReminderDate = Convert.ToDateTime(_ccReminderDate.DateText);
                    appointmentDetails.ReminderTime = _tcReminderTime.TimeText;
                    appointmentDetails.ReminderType = _ddlReminder.SelectedValue;

                    if (_ddlReminder.SelectedValue == "Before")
                    {
                        appointmentDetails.ReminderBeforeTime = _ddlReminderBeforeTime.SelectedValue;
                    }
                    else
                    {
                        appointmentDetails.ReminderBeforeTime = "0";
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return appointmentDetails;
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
                if (_ccDate.DateText.Trim().Length == 0)
                {
                    errorMessage += "Date is mandatory <br/>";
                }

                if (_chkReminder.Checked)
                {
                    if (_ccReminderDate.DateText.Trim().Length == 0)
                    {
                        errorMessage += "Reminder Date is mandatory <br/>";
                    }
                    //if (_tcReminderTime.TimeText.Trim().Length == 0)
                    //{
                    //    errorMessage += "Reminder Time is mandatory <br/>";
                    //}

                    if (_ddlReminder.SelectedValue == "Before" && _tcStartTime.TimeText == string.Empty)
                    {
                        errorMessage += "Start Time is mandatory <br/>";
                    }

                    if (_ddlReminder.SelectedValue == "On" && _tcReminderTime.TimeText == string.Empty)
                    {
                        errorMessage += "Reminder Time is mandatory <br/>";
                    }

                }

                HideUnhideReminderControls();
                
         
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
                    IRIS.Law.WebServiceInterfaces.Diary.Appointment appointmentDetails = GetControlData();

                    diaryService = new DiaryServiceClient();
                    AppointmentReturnValue returnValue = diaryService.SaveAppointment(_logonSettings.LogonId, appointmentDetails);

                    if (returnValue.Success)
                    {
                        _hdnAppointmentId.Value = Convert.ToString(returnValue.Appointment.Id);

                        _lblError.CssClass = "successMessage";
                        _lblError.Text = "Appointment Saved Successfully.";
                    }
                    else
                    {
                        _lblError.CssClass = "errorMessage";

                        if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                            _lblError.Text = "Date is invalid";
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

        #region Back Button
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewAppointment.aspx", true);
        }
        #endregion

        #region New Button

        protected void _btnNew_Click(object sender, EventArgs e)
        {
            Session[SessionName.AppointmentId] = null;
            Response.Redirect("AppointmentDetails.aspx", true);
        }

        #endregion

        #region Reset Button
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Appointment/AppointmentDetails.aspx", true);
        }
        #endregion

        #region Appointment Button
        protected void _btnAppointment_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Appointment/ViewAppointment.aspx", true);
        }
        #endregion

        #region AppointmentOU_UsersAdded
        protected void _AppointmentOU_UsersAdded(object sender, EventArgs e)
        {
            try
            {
                _txtAttendees.Text = _AppointmentOU.CurrentUsers;
                _hdnAttendeesMemberId.Value = _AppointmentOU.CurrentUsersID;
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region HideUnhideReminderControls
        private void HideUnhideReminderControls()
        {
            try
            {
                if (_chkReminder.Checked)
                {
                    _tdReminderBefore.Style["display"] = "";
                    _tdReminderType.Style["display"] = "";
                    _tdReminderOn.Style["display"] = "";

                    _ddlReminder.Enabled = true;
                    _ccReminderDate.Enabled = true;
                    _tcReminderTime.Enabled = true;
                    _ddlReminderBeforeTime.Enabled = true;

                    if (_ddlReminder.SelectedValue == "Before")
                    {
                        _tdReminderOn.Style["display"] = "none";
                        _tdReminderBefore.Style["display"] = "";
                    }
                    else
                    {
                        _tdReminderOn.Style["display"] = "";
                        _tdReminderBefore.Style["display"] = "none";
                    }
                }
                else
                {
                    _tdReminderBefore.Style["display"] = "none";
                    _tdReminderType.Style["display"] = "none";
                    _tdReminderOn.Style["display"] = "none";

                    _ddlReminder.Enabled = false;
                    _ccReminderDate.Enabled = false;
                    _tcReminderTime.Enabled = false;
                    _ddlReminderBeforeTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Checkbox Checked
        protected void _chkReminder_Checked(object sender, EventArgs e)
        {
            try
            {
                HideUnhideReminderControls();
                if (_chkReminder.Checked)
                {
                    CalculateReminderTime();
                }
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region CalculateReminderTime
        private void CalculateReminderTime()
        {
            try
            {
                //_ccReminderDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                //_tcReminderTime.TimeText = "00:00";

                //if (string.IsNullOrEmpty(_tcStartTime.TimeText))
                //{
                //    _tcStartTime.TimeText = "00:00";
                //}
                //else
                //{
                //    // Check the Start entered is correct or not
                //    try
                //    {
                //        string start = DateTime.Now.ToString("dd/MM/yyyy") + " " + _tcStartTime.TimeText + ":00";
                //        DateTime startDate = Convert.ToDateTime(start);
                //    }
                //    catch
                //    {
                //        _tcStartTime.TimeText = "00:00";
                //    }
                //}

                if (!string.IsNullOrEmpty(_ccDate.DateText))
                {
                    try
                    {
                        _ccDate.DateText = Convert.ToDateTime(_ccDate.DateText).ToString("dd/MM/yyyy");
                        _ccReminderDate.DateText = Convert.ToDateTime(_ccDate.DateText).ToString("dd/MM/yyyy");
                    }
                    catch
                    {
                        throw new Exception("Invalid Date");
                    }
                    //DateTime startDate = Convert.ToDateTime(_ccDate.DateText + " " + _tcStartTime.TimeText);
                    //DateTime reminderDateTime = startDate.AddMinutes(-10);
                    //_ccReminderDate.DateText = reminderDateTime.ToString("dd/MM/yyyy");
                    //_tcReminderTime.TimeText = reminderDateTime.ToString("HH:mm");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Client Matter Changed 
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
                            return;
                        }
                    }
                }
                else
                {
                    _hdnProjectId.Value = Convert.ToString(_cliMatDetails.ProjectId);
                }
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region ResetControls
        private void ResetControls()
        {
            try
            {
                GetDefaultFeeEarnerDetails();
                _AppointmentOU.CurrentUsers = _txtAttendees.Text;
                _AppointmentOU.CurrentUsersID = _hdnAttendeesMemberId.Value;

                _txtSubject.Text = string.Empty;
                _ccDate.DateText = DateTime.Now.ToString("dd/MM/yyyy");
                _tcStartTime.TimeText = string.Empty;
                _tcEndTime.TimeText = string.Empty;
                _txtNotes.Text = string.Empty;
                _hdnVenueId.Value = string.Empty;
                _ssAppointmentVenue.ServiceText = string.Empty;

                _chkReminder.Checked = false;
                HideUnhideReminderControls();
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
        }
        #endregion

        #region ServiceSelected Event

        protected void _ssAppointmentVenue_ServiceSelected(object sender, EventArgs e)
        {
            try
            {
                _hdnVenueId.Value = Convert.ToString(_ssAppointmentVenue.ServiceId);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
        }

        #endregion
    }
}

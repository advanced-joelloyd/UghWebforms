using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Time;
using IRIS.Law.Services.Pms.Time;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Matter;
using System.Collections.Generic;
using IRIS.Law.WebApp.UserControls;

namespace IRIS.Law.WebApp.Pages.Time
{
    public partial class AdditionalTimeDetailsMobile : System.Web.UI.Page
    {
        #region Private variables
        Guid _logonId;
        Guid _memberId;
        TimeDetails timeDetails = null;
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
      
            // If Session expires, redirect the user to the login screen
            if (Session[SessionName.LogonId] == null)
            {
                Response.Redirect("~/LoginMobile.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonId = (Guid)Session[SessionName.LogonId];
                _memberId = (Guid)Session[SessionName.MemberId];
            }

            _lblMessage.Text = string.Empty;
            timeDetails = (TimeDetails)Session[SessionName.TimeDetails];

            if (!IsPostBack)
            {
                try
                {
                    if (timeDetails == null)
                    {
                        _lblMessage.CssClass = "errorMessage";
                        _lblMessage.Text = "Session is timeout or time details are not saved in the previous screen. Please go back and enter details again.";
                        return;
                    }
                    ViewState["CurrentAdditionalDetail"] = null;
                    ViewState["FileReviewProjectId"] = DataConstants.DummyGuid.ToString();
                    LoadAdditionalDetailTimeEntry();
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
        }
        #endregion

        #region LoadAdditionalDetailTimeEntry
        /// <summary>
        /// Loads the time entry.
        /// </summary>
        private void LoadAdditionalDetailTimeEntry()
        {
            TimeServiceClient timeService = null;
            TimeAdditionalDetail additionalDetail = null;
            try
            {
                timeService = new TimeServiceClient();
                IRIS.Law.WebServiceInterfaces.Time.Time time = new IRIS.Law.WebServiceInterfaces.Time.Time();
                time.Id = timeDetails.Id;
                time.ProjectId = timeDetails.ProjectId;
                time.FeeEarnerId = timeDetails.FeeEarnerId;
                time.TimeTypeId = timeDetails.TimeTypeId;

                TimeReturnValue returnValue = timeService.GetAddtionalDetailTime(_logonId, time);

                if (returnValue.Success)
                {
                    if (returnValue.Time != null)
                    {
                        if (returnValue.AdditionalDetail != null)
                        {
                            if (returnValue.AdditionalDetail.CurrentAdditionalDetails != AdditionalDetails.None)
                            {
                                additionalDetail = returnValue.AdditionalDetail;
                            }
                            else
                            {
                                SaveTimeRecording();
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
                if (timeService != null)
                {
                    if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                        timeService.Close();
                }
            }

            try
            {
                SetupAdditionalDetails(additionalDetail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupAdditionalDetails
        /// <summary>
        /// Setup the additional details tab.
        /// </summary>
        private void SetupAdditionalDetails(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                HideAllPanels();
                ViewState["CurrentAdditionalDetail"] = additionalDetails.CurrentAdditionalDetails;
                switch (additionalDetails.CurrentAdditionalDetails)
                {
                    case AdditionalDetails.Advocacy:
                        SetupAdvocacyExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.Attendance:
                        SetupAttendanceExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.CivilImmigrationAsylumJRFormFilling:
                        SetupPreparationImmigrationAsylumExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.CivilImmigrationAsylumMentalHearing:
                        SetupHearingImmigrationAsylumMentalExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.CivilImmigrationAsylumSubstantiveHearing:
                        SetupAttendanceImmigrationAsylumExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.CivilImmigrationAsylumTravelWaiting:
                        SetupTravelWaitingImmigrationAsylumExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.CourtDutyAttendance:
                        SetupCourtDutyAttendanceExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.FileReviews:
                        SetupFileReviewsExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.PoliceStationCalls:
                        SetupPoliceStationTelCallsExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.PoliceStationAttendance:
                        SetupPoliceStationStandbyExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.RunningTime:
                        SetupRunningTimeExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.Travel:
                        SetupTravelExtraQuestions(additionalDetails);
                        break;

                    case AdditionalDetails.None:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region HideAllPanels
        private void HideAllPanels()
        {
            try
            {
                _pnlAdvocacy.Visible = false;
                _pnlAttendance.Visible = false;
                _pnlCivilImmAsylumJRFormFilling.Visible = false;
                _pnlCivilImmAsylumMentalHearingAdjourned.Visible = false;
                _pnlCivilImmAsylumSubstantiveHearing.Visible = false;
                _pnlCivilImmAsylumTravelWaitingDetCentre.Visible = false;
                _pnlCourtDutyAttendance.Visible = false;
                _pnlFileReviews.Visible = false;
                _pnlPoliceStationAttendance.Visible = false;
                _pnlPoliceStationCalls.Visible = false;
                _pnlRunningTime.Visible = false;
                _pnlTravel.Visible = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupAttendanceExtraQuestions
        private void SetupAttendanceExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlAttendance.Visible = true;
                GetAttendanceLocation();
                _ddlAttendanceLocation.SelectedIndex = (int)additionalDetails.AdditionalDetailsLocation;

                Location_IndexChanged(_ddlAttendanceLocation, _ddlServiceAttendanceLocation);

                if (_ddlServiceAttendanceLocation.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlServiceAttendanceLocation.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlServiceAttendanceLocation.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }

                TimeServiceClient timeService = null;
                try
                {
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = true;

                    timeService = new TimeServiceClient();
                    AttendanceIndividualSearchReturnValue attendanceIndreturnValue = timeService.AttendanceIndividualSearch(_logonId, collectionRequest);

                    if (attendanceIndreturnValue.Success)
                    {
                        if (attendanceIndreturnValue.AttendanceIndividual != null)
                        {
                            foreach (AttendanceIndividualSearchItem attendanceIndividual in attendanceIndreturnValue.AttendanceIndividual.Rows)
                            {
                                ListItem item = new ListItem();
                                item.Text = attendanceIndividual.Code + " - " + attendanceIndividual.Desciption;
                                item.Value = attendanceIndividual.Id.ToString();
                                _chklstAttendanceIndividuals.Items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(attendanceIndreturnValue.Message);
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
                }

                // Uncheck all the previously selected items.
                for (int i = 0; i < _chklstAttendanceIndividuals.Items.Count; i++)
                {
                    _chklstAttendanceIndividuals.Items[i].Selected = false;
                }


                // Check the Attendance Individuals.
                foreach (int individualId in additionalDetails.AttendanceIndividuals)
                {
                    for (int k = 0; k < this._chklstAttendanceIndividuals.Items.Count; k++)
                    {
                        if (Convert.ToInt32(this._chklstAttendanceIndividuals.Items[k].Value) == individualId)
                        {
                            _chklstAttendanceIndividuals.Items[k].Selected = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupAdvocacyExtraQuestions
        private void SetupAdvocacyExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlAdvocacy.Visible = true;
                GetAdvocacyLocation();
                _ddlAdvocacyLocation.SelectedIndex = (int)additionalDetails.AdditionalDetailsLocation;

                Location_IndexChanged(_ddlAdvocacyLocation, _ddlServiceAdvocacyLocation);

                if (_ddlServiceAdvocacyLocation.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlServiceAdvocacyLocation.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlServiceAdvocacyLocation.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }

                TimeServiceClient timeService = null;
                try
                {
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = true;

                    timeService = new TimeServiceClient();
                    AdvocacyTypeSearchReturnValue advocacyTypeReturnValue = timeService.AdvocacyTypeSearch(_logonId, collectionRequest);

                    if (advocacyTypeReturnValue.Success)
                    {
                        if (advocacyTypeReturnValue.AdvocacyType != null)
                        {
                            foreach (AdvocacyTypeSearchItem advocacyType in advocacyTypeReturnValue.AdvocacyType.Rows)
                            {
                                ListItem item = new ListItem();
                                item.Text = advocacyType.Code + " - " + advocacyType.Desciption;
                                item.Value = advocacyType.Id.ToString();
                                _chklstHearingType.Items.Add(item);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(advocacyTypeReturnValue.Message);
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
                }

                // Uncheck all the previously selected items.
                for (int i = 0; i < _chklstHearingType.Items.Count; i++)
                {
                    _chklstHearingType.Items[i].Selected = false;
                }


                // Check the Attendance Individuals.
                foreach (int advocacyTypeId in additionalDetails.AdvocacyTypes)
                {
                    for (int k = 0; k < this._chklstHearingType.Items.Count; k++)
                    {
                        if (Convert.ToInt32(this._chklstHearingType.Items[k].Value) == advocacyTypeId)
                        {
                            _chklstHearingType.Items[k].Selected = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupPoliceStationTelCallsExtraQuestions
        private void SetupPoliceStationTelCallsExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlPoliceStationCalls.Visible = true;
                GetPoliceStationCallsContacted();

                if (additionalDetails.AdditionalDetailsLocation == DetailLocation.PoliceStation)
                {
                    if (_ddlPoliceStationCalls.Items.FindByValue("Police Station") != null)
                    {
                        _ddlPoliceStationCalls.Items.FindByValue("Police Station").Selected = true;
                    }
                }

                Location_IndexChanged(_ddlPoliceStationCalls, _ddlServicePoliceStationCalls);

                if (_ddlServicePoliceStationCalls.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlServicePoliceStationCalls.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlServicePoliceStationCalls.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupTravelExtraQuestions
        private void SetupTravelExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlTravel.Visible = true;

                _txtMiles.Text = Convert.ToString(additionalDetails.Miles);
                _txtFaresDescription.Text = Convert.ToString(additionalDetails.FaresDescription);
                _txtFares.Text = Convert.ToString(additionalDetails.FaresAmount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupRunningTimeExtraQuestions
        private void SetupRunningTimeExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlRunningTime.Visible = true;

                _txtHour.Text = Convert.ToString(additionalDetails.RunningHours);
                _txtMinutes.Text = Convert.ToString(additionalDetails.RunningMinutes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupTravelWaitingImmigrationAsylumExtraQuestions
        private void SetupTravelWaitingImmigrationAsylumExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlCivilImmAsylumTravelWaitingDetCentre.Visible = true;

                if (timeDetails != null)
                {
                    switch (timeDetails.TimeTypeCatId)
                    {
                        case 2: // Travel.
                            _lblCivilImmAsylumTravelWaitingDetCentre.Text = "Is this travel to a detention centre?";
                            break;

                        case 3: // Waiting.
                            _lblCivilImmAsylumTravelWaitingDetCentre.Text = "Is this waiting at a detention centre?";
                            break;
                    }
                }

                _chkCivilImmAsylumTravelWaitingDetCentre.Checked = additionalDetails.IsTravelWaitingDetCentre;
                _ddlServiceCivilImmAsylumTravelWaitingDetCentre.Enabled = _chkCivilImmAsylumTravelWaitingDetCentre.Checked;

                // 96 -> Detention Center Lookup
                // Association Role Id is used to get Industry Id 
                // and using this IndustryId, Detention Center Lookup is populated
                ServiceSearchItem[] services = GetServicesLookup(96);
                _ddlServiceCivilImmAsylumTravelWaitingDetCentre.DataSource = services;
                _ddlServiceCivilImmAsylumTravelWaitingDetCentre.DataTextField = "Name";
                _ddlServiceCivilImmAsylumTravelWaitingDetCentre.DataValueField = "Id";
                _ddlServiceCivilImmAsylumTravelWaitingDetCentre.DataBind();
                AddDefaultToDropDownList(_ddlServiceCivilImmAsylumTravelWaitingDetCentre);

                if (_ddlServiceCivilImmAsylumTravelWaitingDetCentre.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlServiceCivilImmAsylumTravelWaitingDetCentre.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlServiceCivilImmAsylumTravelWaitingDetCentre.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupPreparationImmigrationAsylumExtraQuestions
        private void SetupPreparationImmigrationAsylumExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlCivilImmAsylumJRFormFilling.Visible = true;

                _chkCivilImmAsylumJRFormFilling.Checked = additionalDetails.LAIsJRFormFilling;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupHearingImmigrationAsylumMentalExtraQuestions
        private void SetupHearingImmigrationAsylumMentalExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlCivilImmAsylumMentalHearingAdjourned.Visible = true;

                _chkCivilImmAsylumMentalHearingAdjourned.Checked = additionalDetails.IsLAHearingAdjourned;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupAttendanceImmigrationAsylumExtraQuestions
        private void SetupAttendanceImmigrationAsylumExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlCivilImmAsylumSubstantiveHearing.Visible = true;

                _chkCivilImmAsylumSubstantiveHearing.Checked = additionalDetails.IsLASubstantiveHearingAttend;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupCourtDutyAttendanceExtraQuestions
        private void SetupCourtDutyAttendanceExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlCourtDutyAttendance.Visible = true;

                _txtCourtDutyAttNoOfDef.Text = Convert.ToString(additionalDetails.LASuspect);
                _chkCourtDutyAtYouth.Checked = additionalDetails.IsLAYouth;

                // 25 -> Court Type
                // Association Role Id is used to get Industry Id 
                // and using this IndustryId, Court lookup is populated
                ServiceSearchItem[] services = GetServicesLookup(25);
                _ddlCourtDutyAttCourt.DataSource = services;
                _ddlCourtDutyAttCourt.DataTextField = "Name";
                _ddlCourtDutyAttCourt.DataValueField = "Id";
                _ddlCourtDutyAttCourt.DataBind();
                AddDefaultToDropDownList(_ddlCourtDutyAttCourt);

                if (_ddlCourtDutyAttCourt.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlCourtDutyAttCourt.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlCourtDutyAttCourt.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupPoliceStationStandbyExtraQuestions
        private void SetupPoliceStationStandbyExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlPoliceStationAttendance.Visible = true;
                GetPoliceStationAttendance();
                _ddlPoliceStationAttendance.SelectedIndex = (int)additionalDetails.AdditionalDetailsLocation;

                // Get Services
                Location_IndexChanged(_ddlPoliceStationAttendance, _ddlServicePoliceStationAttendance);

                if (_ddlServicePoliceStationAttendance.Items.Count > 1)
                {
                    if (additionalDetails.LAId != null)
                    {
                        if (_ddlServicePoliceStationAttendance.Items.FindByValue(Convert.ToString(additionalDetails.LAId)) != null)
                        {
                            _ddlServicePoliceStationAttendance.Items.FindByValue(Convert.ToString(additionalDetails.LAId)).Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SetupFileReviewsExtraQuestions
        private void SetupFileReviewsExtraQuestions(TimeAdditionalDetail additionalDetails)
        {
            try
            {
                _pnlFileReviews.Visible = true;

                if (additionalDetails.IsFaceToFace)
                {
                    _rdoBtnFaceToFace.Checked = true;
                    _rdoBtnPaper.Checked = false;
                }
                else
                {
                    _rdoBtnFaceToFace.Checked = false;
                    _rdoBtnPaper.Checked = true;
                }

                Guid projectId = additionalDetails.FileReviewsProjectId;
                if (additionalDetails.FileReviewsProjectId != DataConstants.DummyGuid)
                {
                    ViewState["FileReviewProjectId"] = additionalDetails.FileReviewsProjectId.ToString();
                    _msFileReviews.ProjectId = additionalDetails.FileReviewsProjectId;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SaveTimeRecording
        private void SaveTimeRecording()
        {
            try
            {
                if (Session[SessionName.TimeDetails] == null)
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Session is timeout or the time details are not saved in the previous screen. Please go back and enter details again.";
                    return;
                }

                TimeServiceClient timeService = null;
                TimeAdditionalDetail timeAdditionalDetail = null;
                try
                {
                    timeService = new TimeServiceClient();
                    IRIS.Law.WebServiceInterfaces.Time.Time time = new IRIS.Law.WebServiceInterfaces.Time.Time();

                    TimeDetails timeDetails = (TimeDetails)Session[SessionName.TimeDetails];

                    time.ProjectId = timeDetails.ProjectId;
                    time.FeeEarnerId = timeDetails.FeeEarnerId; 
                    time.TimeTypeId = timeDetails.TimeTypeId;
                    time.Units = timeDetails.Units;
                    time.Notes = timeDetails.Notes;
                    time.Date = DateTime.Now.Date;
                    //No validation for limits from the web
                    time.DontCheckLimits = true;
                    time.BillingCodeID = 1; // Default to "Default - None Selected"

                    if (ViewState["CurrentAdditionalDetail"] != null)
                    {
                        timeAdditionalDetail = new TimeAdditionalDetail();

                        timeAdditionalDetail.CurrentAdditionalDetails = (AdditionalDetails)ViewState["CurrentAdditionalDetail"];

                        //Get the data from listview controls if that additional details is selected
                        //and set HaveAskedLaQuestions 
                        switch (timeAdditionalDetail.CurrentAdditionalDetails)
                        {
                            case AdditionalDetails.Attendance:
                                #region Attendance

                                timeAdditionalDetail.AdditionalDetailsLocation = (DetailLocation)Enum.Parse(typeof(DetailLocation), _ddlAttendanceLocation.SelectedIndex.ToString());
                                if (_ddlServiceAttendanceLocation.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlServiceAttendanceLocation.SelectedValue);
                                }

                                //Get the selected joint client candidates
                                List<ListItem> selectedItems = (from item in _chklstAttendanceIndividuals.Items.Cast<ListItem>()
                                                                where item.Selected
                                                                select item).ToList();

                                int[] attendanceInd = new int[selectedItems.Count];
                                for (int i = 0; i < selectedItems.Count; i++)
                                {
                                    int attendanceIndID = Convert.ToInt32(selectedItems[i].Value);
                                    attendanceInd[i] = attendanceIndID;
                                }
                                timeAdditionalDetail.AttendanceIndividuals = attendanceInd;
                                #endregion
                                break;

                            case AdditionalDetails.Advocacy:
                                #region Advocacy

                                timeAdditionalDetail.AdditionalDetailsLocation = (DetailLocation)Enum.Parse(typeof(DetailLocation), _ddlAdvocacyLocation.SelectedIndex.ToString());
                                if (_ddlServiceAdvocacyLocation.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlServiceAdvocacyLocation.SelectedValue);
                                }

                                //Get the selected joint client candidates
                                List<ListItem> selectedItemsHearing = (from item in _chklstHearingType.Items.Cast<ListItem>()
                                                                       where item.Selected
                                                                       select item).ToList();

                                int[] advocacyTypeId = new int[selectedItemsHearing.Count];
                                for (int i = 0; i < selectedItemsHearing.Count; i++)
                                {
                                    int tempAdvocacyTypeId = Convert.ToInt32(selectedItemsHearing[i].Value);
                                    advocacyTypeId[i] = tempAdvocacyTypeId;
                                }
                                timeAdditionalDetail.AdvocacyTypes = advocacyTypeId;
                                #endregion
                                break;

                            case AdditionalDetails.Travel:
                                #region Travel
                                if (_txtMiles.Text.Length > 0)
                                {
                                    timeAdditionalDetail.Miles = Convert.ToInt32(_txtMiles.Text);
                                }
                                timeAdditionalDetail.FaresDescription = _txtFaresDescription.Text;
                                if (_txtFares.Text.Length > 0)
                                {
                                    timeAdditionalDetail.FaresAmount = Convert.ToDecimal(_txtFares.Text);
                                }
                                #endregion
                                break;

                            case AdditionalDetails.PoliceStationCalls:
                                #region PoliceStationCalls
                                if (_ddlPoliceStationCalls.SelectedValue == "Police Station")
                                {
                                    timeAdditionalDetail.AdditionalDetailsLocation = DetailLocation.PoliceStation;
                                }
                                else
                                {
                                    timeAdditionalDetail.AdditionalDetailsLocation = DetailLocation.NotApplicable;
                                }

                                if (_ddlServicePoliceStationCalls.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlServicePoliceStationCalls.SelectedValue);
                                }
                                #endregion
                                break;

                            case AdditionalDetails.RunningTime:
                                #region RunningTime
                                if (_txtHour.Text.Length > 0)
                                {
                                    timeAdditionalDetail.RunningHours = Convert.ToInt32(_txtHour.Text);
                                }
                                if (_txtMinutes.Text.Length > 0)
                                {
                                    timeAdditionalDetail.RunningMinutes = Convert.ToInt32(_txtMinutes.Text);
                                }
                                #endregion
                                break;

                            case AdditionalDetails.CivilImmigrationAsylumSubstantiveHearing:
                                #region CivilImmigrationAsylumSubstantiveHearing
                                timeAdditionalDetail.IsLASubstantiveHearingAttend = _chkCivilImmAsylumSubstantiveHearing.Checked;
                                #endregion
                                break;

                            case AdditionalDetails.CivilImmigrationAsylumTravelWaiting:
                                #region CivilImmigrationAsylumTravelWaiting
                                timeAdditionalDetail.IsTravelWaitingDetCentre = _chkCivilImmAsylumTravelWaitingDetCentre.Checked;
                                if (_ddlServiceCivilImmAsylumTravelWaitingDetCentre.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlServiceCivilImmAsylumTravelWaitingDetCentre.SelectedValue);
                                }
                                #endregion
                                break;

                            case AdditionalDetails.CivilImmigrationAsylumJRFormFilling:
                                #region CivilImmigrationAsylumJRFormFilling
                                timeAdditionalDetail.LAIsJRFormFilling = _chkCivilImmAsylumJRFormFilling.Checked;
                                #endregion
                                break;

                            case AdditionalDetails.CivilImmigrationAsylumMentalHearing:
                                #region CivilImmigrationAsylumMentalHearing
                                timeAdditionalDetail.IsLAHearingAdjourned = _chkCivilImmAsylumMentalHearingAdjourned.Checked;
                                #endregion
                                break;

                            case AdditionalDetails.CourtDutyAttendance:
                                #region CourtDutyAttendance
                                if (_txtCourtDutyAttNoOfDef.Text.Length > 0)
                                {
                                    timeAdditionalDetail.LASuspect = Convert.ToInt32(_txtCourtDutyAttNoOfDef.Text);
                                }
                                timeAdditionalDetail.IsLAYouth = _chkCourtDutyAtYouth.Checked;
                                if (_ddlCourtDutyAttCourt.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlCourtDutyAttCourt.SelectedValue);
                                }
                                else
                                {
                                    timeAdditionalDetail.LAId = DataConstants.DummyGuid;
                                }
                                #endregion
                                break;

                            case AdditionalDetails.PoliceStationAttendance:
                                #region PoliceStationAttendance
                                timeAdditionalDetail.AdditionalDetailsLocation = (DetailLocation)Enum.Parse(typeof(DetailLocation), _ddlPoliceStationAttendance.SelectedIndex.ToString());
                                if (_ddlServicePoliceStationAttendance.SelectedValue.Length > 0)
                                {
                                    timeAdditionalDetail.LAId = new Guid(_ddlServicePoliceStationAttendance.SelectedValue);
                                }
                                else
                                {
                                    timeAdditionalDetail.LAId = DataConstants.DummyGuid;
                                }
                                #endregion
                                break;

                            case AdditionalDetails.FileReviews:
                                #region FileReviews
                                if (ViewState["FileReviewProjectId"] == null)
                                {
                                    timeAdditionalDetail.FileReviewsProjectId = DataConstants.DummyGuid;
                                }
                                else
                                {
                                    timeAdditionalDetail.FileReviewsProjectId = new Guid(ViewState["FileReviewProjectId"].ToString());
                                }
                                timeAdditionalDetail.IsFaceToFace = _rdoBtnFaceToFace.Checked;
                                #endregion
                                break;
                        }
                    }

                    TimeReturnValue returnValue = null;

                    if (((TimeDetails)Session[SessionName.TimeDetails]).Id == 0)
                    {
                        //No timeId present. Add a new time entry
                        returnValue = timeService.AddTime(_logonId, time, timeAdditionalDetail, false);
                    }
                    else
                    {
                        //If a timeId is present then we are updating an existing time entry
                        time.Id = ((TimeDetails)Session[SessionName.TimeDetails]).Id;
                        returnValue = timeService.UpdateTime(_logonId, time, timeAdditionalDetail);
                    }

                    if (returnValue.Success)
                    {
                        Session[SessionName.ProjectId] = timeDetails.ProjectId;
                        Session[SessionName.TimeDetails] = null;
                        //Redirect the user to the timesheet where entry would be listed
                        Response.Redirect("AddTimeEntryMobile.aspx?Success=1", false);
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
                    if (timeService != null)
                    {
                        if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                            timeService.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region Functions

        #region AddDefaultToDropDownList
        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region GetAdvocacyLocation
        private void GetAdvocacyLocation()
        {
            try
            {
                _ddlAdvocacyLocation.Items.Clear();
                ListItem itemNA = new ListItem("Not Applicable");
                _ddlAdvocacyLocation.Items.Add(itemNA);
                ListItem itemCourt = new ListItem("Court");
                _ddlAdvocacyLocation.Items.Add(itemCourt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetPoliceStationCallsContacted
        private void GetPoliceStationCallsContacted()
        {
            try
            {
                _ddlPoliceStationCalls.Items.Clear();
                ListItem itemNA = new ListItem("Not Applicable");
                _ddlPoliceStationCalls.Items.Add(itemNA);
                ListItem itemPoliceStn = new ListItem("Police Station");
                _ddlPoliceStationCalls.Items.Add(itemPoliceStn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetAttendanceLocation
        private void GetAttendanceLocation()
        {
            try
            {
                _ddlAttendanceLocation.Items.Clear();
                ListItem itemNA = new ListItem("Not Applicable");
                _ddlAttendanceLocation.Items.Add(itemNA);
                ListItem itemCourt = new ListItem("Court");
                _ddlAttendanceLocation.Items.Add(itemCourt);
                ListItem itemPoliceStn = new ListItem("Police Station");
                _ddlAttendanceLocation.Items.Add(itemPoliceStn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetPoliceStationCalls
        private void GetPoliceStationCalls()
        {
            try
            {
                _ddlPoliceStationCalls.Items.Clear();
                ListItem itemNA = new ListItem("Not Applicable");
                _ddlPoliceStationCalls.Items.Add(itemNA);
                ListItem itemPoliceStn = new ListItem("Police Station");
                _ddlPoliceStationCalls.Items.Add(itemPoliceStn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetPoliceStationAttendance
        private void GetPoliceStationAttendance()
        {
            try
            {
                _ddlPoliceStationAttendance.Items.Clear();
                ListItem itemNA = new ListItem("Not Applicable");
                _ddlPoliceStationAttendance.Items.Add(itemNA);
                ListItem itemPoliceStn = new ListItem("Police Station");
                _ddlPoliceStationAttendance.Items.Add(itemPoliceStn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Location_IndexChanged
        private void Location_IndexChanged(DropDownList ddlLocation, DropDownList ddlService)
        {
            try
            {
                if (ddlLocation.Items.Count > 0)
                {
                    ddlService.Items.Clear();

                    switch (ddlLocation.Text.Trim())
                    {
                        case "Not Applicable": // Not Applicable.
                            ddlService.Enabled = false;
                            ddlService.SelectedIndex = -1;
                            break;

                        case "Court": // Court.
                            ddlService.Enabled = true;

                            // 25 -> Court Type
                            // Association Role Id is used to get Industry Id 
                            // and using this IndustryId, Court lookup is populated
                            ServiceSearchItem[] services = GetServicesLookup(25);
                            ddlService.DataSource = services;
                            ddlService.DataTextField = "Name";
                            ddlService.DataValueField = "Id";
                            ddlService.DataBind();
                            break;

                        case "Police Station": // Police Station.
                            ddlService.Enabled = true;

                            // 53 -> Police Station Lookup
                            // Association Role Id is used to get Industry Id 
                            // and using this IndustryId, Police Station Lookup is populated
                            ServiceSearchItem[] servicePoliceStnLookup = GetServicesLookup(53);
                            ddlService.DataSource = servicePoliceStnLookup;
                            ddlService.DataTextField = "Name";
                            ddlService.DataValueField = "Id";
                            ddlService.DataBind();
                            break;
                    }
                    AddDefaultToDropDownList(ddlService);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetServicesLookup
        private ServiceSearchItem[] GetServicesLookup(int associationRoleId)
        {
            TimeServiceClient timeService = null;
            ServiceSearchItem[] services = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;

                timeService = new TimeServiceClient();
                ServiceSearchReturnValue serviceReturnValue = timeService.ServiceSearch(_logonId, collectionRequest, associationRoleId);

                if (serviceReturnValue.Success)
                {
                    if (serviceReturnValue.Service != null)
                    {
                        services = serviceReturnValue.Service.Rows;
                    }
                }
                else
                {
                    throw new Exception(serviceReturnValue.Message);
                }
                return services;
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
            }
        }
        #endregion

        #endregion

        #region Button Events

        #region Back Button
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Pages/Time/AddTimeEntryMobile.aspx", true);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Cancel
        protected void _btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Session[SessionName.TimeDetails] = null;
                Response.Redirect("~/Pages/Time/AddTimeEntryMobile.aspx", true);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Save
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveTimeRecording();
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

        protected void _btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/LoginMobile.aspx");
        }

        #endregion

        #region Dropdown Events

        #region Advocacy Location
        protected void _ddlAdvocacyLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlAdvocacyLocation, _ddlServiceAdvocacyLocation);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Attendance Location
        protected void _ddlAttendanceLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlAttendanceLocation, _ddlServiceAttendanceLocation);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Police Station Calls
        protected void _ddlPoliceStationCalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlPoliceStationCalls, _ddlServicePoliceStationCalls);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Police Station Calls
        protected void _ddlPoliceStationAttendance_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlPoliceStationAttendance, _ddlServicePoliceStationAttendance);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #endregion

        #region FileReview Matter

        protected void _msFileReviews_MatterChanged(object sender, EventArgs e)
        {
            ViewState["FileReviewProjectId"] = _msFileReviews.ProjectId.ToString();
        }

        protected void _msFileReviews_Error(object sender, ErrorEventArgs e)
        {
            _lblMessage.CssClass = "errorMessage";
            _lblMessage.Text = e.Message;
        }

        protected void _msFileReviews_SearchSuccessful(object sender, SuccessEventArgs e)
        {
            _lblMessage.CssClass = "successMessage";
            _lblMessage.Text = e.Message;
        }
        #endregion


        protected void _imgBtnAdvocacyLocation_Click(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlAdvocacyLocation, _ddlServiceAdvocacyLocation);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }

        }

        protected void _imgBtnAttendanceLocation_Click(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlAttendanceLocation, _ddlServiceAttendanceLocation);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }

        }

        protected void _imgBtnPoliceStationContacted_Click(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlPoliceStationCalls, _ddlServicePoliceStationCalls);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }

        }

        protected void _imgBtnPoliceStationAttended_Click(object sender, EventArgs e)
        {
            try
            {
                Location_IndexChanged(_ddlPoliceStationAttendance, _ddlServicePoliceStationAttendance);
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }

        }
    }
}

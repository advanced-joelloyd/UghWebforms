using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.Services.Pms.Time;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebApp.UserControls;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Time
{
    public partial class AddTimeEntryMobile : System.Web.UI.Page
    {
        #region Private Variable
        private Guid _logonId;
        private bool _isLoadingLastSavedTimeEntry = false;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            

            _lblMessage.Text = string.Empty;
            // If Session expires, redirect the user to the login screen
            if (Session[SessionName.LogonId] == null)
            {
                Response.Redirect("~/LoginMobile.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonId = (Guid)Session[SessionName.LogonId];
            }

            if (!IsPostBack)
            {
                //Select the fee earner associated with the logged-in user
                if (Session[SessionName.DefaultFeeEarner] != null)
                {
                    _msAddTimeEntry.DefaultFeeEarnerId = (Guid)Session[SessionName.DefaultFeeEarner];
                }

                if (!string.IsNullOrEmpty(Request.QueryString["Success"]))
                {
                    if (Request.QueryString["Success"] == "1")
                    {
                        _lblMessage.CssClass = "successMessage";
                        _lblMessage.Text = "Time entry saved successfully";
                    }

                    if (Session[SessionName.ProjectId] != null)
                    {
                        _msAddTimeEntry.ProjectId = (Guid)Session[SessionName.ProjectId];
                    }
                }
                else
                {
                    //if we are coming from the Additional Detail pg then display the entered values
                    if (Request.UrlReferrer != null)
                    {
                        string pageName = Request.UrlReferrer.AbsolutePath;
                        pageName = AppFunctions.GetPageNameByUrl(pageName);
                        if (pageName == "AdditionalTimeDetailsMobile.aspx")
                        {
                            // Load Last saved time entry
                            LoadLastSavedTimeEntry();
                            return;
                        }
                    }
                }
                Session[SessionName.TimeDetails] = null;
            }
        }

        #region LoadLastSavedTimeEntry
        private void LoadLastSavedTimeEntry()
        {
            _isLoadingLastSavedTimeEntry = true;
            try
            {
                if (Session[SessionName.TimeDetails] != null)
                {
                    TimeDetails timeDetails = (TimeDetails)Session[SessionName.TimeDetails];
                    _msAddTimeEntry.ProjectId = timeDetails.ProjectId;
                    _txtUnits.Text = ((TimeDetails)Session[SessionName.TimeDetails]).Units.ToString();
                    _txtNotes.Text = ((TimeDetails)Session[SessionName.TimeDetails]).Notes;
                }
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        protected void _btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                TimeDetails timeDetails = new TimeDetails();

                if ((Guid)ViewState["AddTimeProjectId"] != _msAddTimeEntry.ProjectIdConfirm)
                {
                    ViewState["AddTimeProjectId"] = _msAddTimeEntry.ProjectIdConfirm;
                }

                Guid _feeEarnerId = (Guid)Session[SessionName.DefaultFeeEarner];

                if (_msAddTimeEntry.FeeEarnerId != DataConstants.DummyGuid)
                {
                    _feeEarnerId = _msAddTimeEntry.FeeEarnerId;
                }

                timeDetails.ProjectId = (Guid)ViewState["AddTimeProjectId"];
                timeDetails.FeeEarnerId = _feeEarnerId;//(Guid)Session[SessionName.DefaultFeeEarner];
                timeDetails.TimeTypeId = new Guid(GetValueOnIndexFromArray(_ddlTimeType.SelectedValue.Trim(), 0));
                timeDetails.TimeTypeCatId = Convert.ToInt32(GetValueOnIndexFromArray(_ddlTimeType.SelectedValue.Trim(), 1));
                timeDetails.Units = Convert.ToInt32(_txtUnits.Text.Trim());
                timeDetails.Notes = _txtNotes.Text.Trim();
                timeDetails.Date = DateTime.Now.Date;
                // No validation for limits from the web
                timeDetails.CanProceed = true;
                timeDetails.Id = 0;

                if (IsAdditionalDetailTimeExists(timeDetails))
                {
                    Session[SessionName.TimeDetails] = timeDetails;
                    Response.Redirect("~/Pages/Time/AdditionalTimeDetailsMobile.aspx", true);
                }
                else
                {
                    TimeServiceClient timeService = null;
                    TimeReturnValue returnValue = null;
                    try
                    {
                        timeService = new TimeServiceClient();
                        IRIS.Law.WebServiceInterfaces.Time.Time time = new IRIS.Law.WebServiceInterfaces.Time.Time();

                        time.ProjectId = timeDetails.ProjectId;
                        time.FeeEarnerId = timeDetails.FeeEarnerId;
                        time.TimeTypeId = timeDetails.TimeTypeId;
                        time.Units = timeDetails.Units;
                        time.Notes = timeDetails.Notes;
                        time.Date = DateTime.Now.Date;
                        //No validation for limits from the web
                        time.DontCheckLimits = true;
                        time.BillingCodeID = 1; // Default to "Default - None Selected"

                        //No timeId present. Add a new time entry
                        returnValue = timeService.AddTime(_logonId, time, null, false);

                        if (returnValue.Success)
                        {
                            Session[SessionName.TimeDetails] = null;

                            _lblMessage.CssClass = "successMessage";
                            _lblMessage.Text = "Time entry saved successfully";

                            //Reset time entry fields
                            ResetFields();
                        }
                        else
                        {
                            throw new Exception(returnValue.Message);
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
                        if (timeService != null)
                        {
                            if (timeService.State != System.ServiceModel.CommunicationState.Faulted)
                                timeService.Close();
                        }
                    }
                }
            }
        }

        #region IsAdditionalDetailTimeExists
        /// <summary>
        /// Loads the time entry.
        /// </summary>
        private bool IsAdditionalDetailTimeExists(TimeDetails timeDetails)
        {
            TimeServiceClient timeService = null;
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
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
                return false;
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

        #region GetValueOnIndexFromArray
        /// <summary>
        /// If string is Branch Value
        /// index = 0 -> MemberId
        /// index = 1 -> OrganisationId
        /// 
        /// If string is TimeType value
        /// index = 0 -> TimeTypeId
        /// index = 2 -> CatId
        /// </summary>
        /// <param name="branchValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strValue, int index)
        {
            try
            {
                string[] arrayBranch = strValue.Split('$');
                return arrayBranch[index].Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Gets the valid time types for the matter.
        /// </summary>
        private void GetTimeTypes()
        {
            _ddlTimeType.Items.Clear();
            if (_msAddTimeEntry.ProjectId != DataConstants.DummyGuid)
            {
                TimeServiceClient timeService = null;
                try
                {
                    timeService = new TimeServiceClient();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    TimeTypeSearchCriteria criteria = new TimeTypeSearchCriteria();
                    criteria.IncludeArchived = false;
                    criteria.ProjectId = _msAddTimeEntry.ProjectId;
                    TimeTypeSearchReturnValue returnValue = timeService.TimeTypeSearch(_logonId,
                                                                                collectionRequest, criteria);

                    if (returnValue.Success)
                    {
                        for (int i = 0; i < returnValue.TimeTypes.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = returnValue.TimeTypes.Rows[i].Description.ToString();
                            item.Value = returnValue.TimeTypes.Rows[i].Id.ToString().Trim() + "$" + Convert.ToString(returnValue.TimeTypes.Rows[i].CatId);
                            _ddlTimeType.Items.Add(item);
                        }

                        //Dont set default time type if we have come back from additional details pg
                        if (!_isLoadingLastSavedTimeEntry)
                        {
                            _ddlTimeType.SelectedIndex = -1;
                            for (int i = 0; i < _ddlTimeType.Items.Count; i++)
                            {
                                if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == "3ef2937c-c31b-430c-82ed-5701a84f258e")
                                {
                                    _ddlTimeType.Items[i].Selected = true;
                                }
                            }
                        }
                        else
                        {
                            TimeDetails timeDetails = (TimeDetails)Session[SessionName.TimeDetails];
                            _ddlTimeType.SelectedIndex = -1;
                            for (int i = 0; i < _ddlTimeType.Items.Count; i++)
                            {
                                if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == timeDetails.TimeTypeId.ToString())
                                {
                                    _ddlTimeType.Items[i].Selected = true;
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
            }
        }

        protected void _msAddTimeEntry_MatterChanged(object sender, EventArgs e)
        {
            ViewState["AddTimeProjectId"] = _msAddTimeEntry.ProjectId;
            GetTimeTypes();
        }


        protected void _btnReset_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        protected void _btnSubtractUnits_Click(object sender, EventArgs e)
        {
            string units = _txtUnits.Text;
            
            if (units == string.Empty)
            {
                _txtUnits.Text = "1";
            }
            else 
            {
                if (int.Parse(_txtUnits.Text)>1)
                {
                    int newNum = int.Parse(_txtUnits.Text) - 1;
                    _txtUnits.Text = newNum.ToString();
                }
            }
        }


        protected void _btnAddUnits_Click(object sender, EventArgs e)
        {
            string units = _txtUnits.Text;

            if (units == string.Empty)
            {
                _txtUnits.Text = "1";
            }
            else
            {
                int newNum = int.Parse(_txtUnits.Text) + 1;
                _txtUnits.Text = newNum.ToString();
            }
        }

        protected void _btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/LoginMobile.aspx");
        }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields()
        {
            //_ddlTimeType.SelectedIndex = -1;
            //for (int i = 0; i < _ddlTimeType.Items.Count; i++)
            //{
            //    if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == "3ef2937c-c31b-430c-82ed-5701a84f258e")
            //    {
            //        _ddlTimeType.Items[i].Selected = true;
            //    }
            //}

            _ddlTimeType.Items.Clear();
            _msAddTimeEntry.Reset();

            //Set Attending client as the default option if its available
            //if (_ddlTimeType.Items.FindByValue("3ef2937c-c31b-430c-82ed-5701a84f258e") != null)
            //{
            //    _ddlTimeType.SelectedValue = "3ef2937c-c31b-430c-82ed-5701a84f258e";
            //}
            _txtUnits.Text = "1";
            _txtNotes.Text = string.Empty;
        }

        #region Matter Search

        protected void _msAddTimeEntry_Error(object sender, ErrorEventArgs e)
        {
            _lblMessage.CssClass = "errorMessage";
            _lblMessage.Text = e.Message;

            _ddlTimeType.Items.Clear();
        }

        protected void _msAddTimeEntry_SearchSuccessful(object sender, SuccessEventArgs e)
        {
            _lblMessage.CssClass = "successMessage";
            _lblMessage.Text = e.Message;

            if (_msAddTimeEntry.ClientRowCount == 0)
            {
                
                _ddlTimeType.Items.Clear();
            }
        }

        #endregion
    }
}
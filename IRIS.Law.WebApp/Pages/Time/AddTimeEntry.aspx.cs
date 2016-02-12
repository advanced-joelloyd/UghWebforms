using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.Services.Pms.Time;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Time
{
    public partial class AddTimeEntry : BasePage
    {
        #region Private variables
        LogonReturnValue _logonSettings;
        #endregion

        #region Form Events
        
        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            
            _lblMessage.Text = string.Empty;

            if (Request.QueryString["edit"] != "true")
            {
                _divBtnAddNew.Visible = false;
            }

            if (!IsPostBack)
            {
                try
                {
                    //Edit time entry
                    if (Session[SessionName.TimeId] != null && Session[SessionName.MatterReference] != null)
                    {
                        int timeId = Convert.ToInt32(Session[SessionName.TimeId]);
                        string matterReference = Session[SessionName.MatterReference].ToString();
                        //Store the timeId as its required for updating
                        ViewState["TimeId"] = timeId;
                        LoadTimeEntry(timeId, matterReference);
                    }
                    else if (Session[SessionName.ProjectId] != null)
                    {
                        //if a project id is in context then get the time types
                        GetTimeTypes((Guid)Session[SessionName.ProjectId]);
                        SetDefaultTimeType();
                    }

                    //if we are coming from the Additional Detail pg then display the entered values
                    if (Request.UrlReferrer != null)
                    {
                        string pageName = Request.UrlReferrer.AbsolutePath;
                        pageName = AppFunctions.GetPageNameByUrl(pageName);
                        if (pageName == "AdditionalTimeDetails.aspx")
                        {
                            // Load Last saved time entry
                            LoadLastSavedTimeEntry();
                            return;
                        }
                    }
                    Session[SessionName.TimeDetails] = null;
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
                    //Clear session values after loading the time entry so that the time entry page
                    //does not open in edit mode the next time the user visits the time recording pg.
                    Session[SessionName.TimeId] = null;
                    Session[SessionName.MatterReference] = null;
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            //No matter was in context only a client was selected. A default matter would get selected
            //after the clientMatter usercontrol loads. Get time types for the matter
            if (_ddlTimeType.Items.Count == 0 && Session[SessionName.ProjectId] != null)
            {
                GetTimeTypes((Guid)Session[SessionName.ProjectId]);
                SetDefaultTimeType();
            }
        }
        
        #endregion

        #region LoadLastSavedTimeEntry
        private void LoadLastSavedTimeEntry()
        {
            try
            {
                if (Session[SessionName.TimeDetails] != null)
                {
                    _ddlTimeType.SelectedIndex = -1;
                    for (int i = 0; i < _ddlTimeType.Items.Count; i++)
                    {
                        if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == ((TimeDetails)Session[SessionName.TimeDetails]).TimeTypeId.ToString())
                        {
                            _ddlTimeType.Items[i].Selected = true;
                        }
                    }
                      
                    _txtUnits.Text = ((TimeDetails)Session[SessionName.TimeDetails]).Units.ToString();
                    _txtNotes.Text = ((TimeDetails)Session[SessionName.TimeDetails]).Notes;

                    ViewState["TimeId"] = ((TimeDetails)Session[SessionName.TimeDetails]).Id;
                }
            }
            catch (Exception ex)
            {
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #region Control Events

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
                if (Session[SessionName.ProjectId] == null)
                {
                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblMessage.CssClass = "errorMessage";
                            _lblMessage.Text = _cliMatDetails.Message;
                            return;
                        }
                    }
                }
                else
                {
                    string prevValue = null;
                    //If a value is selected then save it, this prevents the default value from 
                    //overriding the selection
                    if (_ddlTimeType.Items.Count > 0)
                    {
                        prevValue = GetValueOnIndexFromArray(_ddlTimeType.SelectedValue, 0);
                    }

                    GetTimeTypes((Guid)Session[SessionName.ProjectId]);

                    if (prevValue != null)
                    {
                        _ddlTimeType.SelectedIndex = -1;
                        for (int i = 0; i < _ddlTimeType.Items.Count; i++)
                        {
                            if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == prevValue)
                            {
                                _ddlTimeType.Items[i].Selected = true;
                                break;
                            }
                        }
                          
                    }
                    else
                    {
                        //No value was previously selected. set the default time type
                        SetDefaultTimeType();
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
        }

        protected void _btnAddNew_Click(object sender, EventArgs e)
        {
            Session[SessionName.TimeId] = null;
            Response.Redirect("~/Pages/Time/AddTimeEntry.aspx", true);
        }

        protected void _btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (_txtUnits.Text == "0")
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Units cannot be zero";
                    return;
                }

                TimeDetails timeDetails = new TimeDetails();
                timeDetails.ProjectId = (Guid)Session[SessionName.ProjectId];
                timeDetails.FeeEarnerId = _logonSettings.UserDefaultFeeMemberId;
                timeDetails.TimeTypeId = new Guid(GetValueOnIndexFromArray(_ddlTimeType.SelectedValue.Trim(), 0));
                timeDetails.TimeTypeCatId = Convert.ToInt32(GetValueOnIndexFromArray(_ddlTimeType.SelectedValue.Trim(), 1));
                timeDetails.Units = Convert.ToInt32(_txtUnits.Text.Trim());
                timeDetails.Notes = _txtNotes.Text.Trim();
                timeDetails.Date = DateTime.Now.Date;
                // No validation for limits from the web
                timeDetails.CanProceed = true;
                if (ViewState["TimeId"] == null)
                {
                    timeDetails.Id = 0;
                }
                else
                {
                    timeDetails.Id = Convert.ToInt32(ViewState["TimeId"]);
                }

                if (IsAdditionalDetailTimeExists(timeDetails))
                {
                    Session[SessionName.TimeDetails] = timeDetails;
                    Response.Redirect("~/Pages/Time/AdditionalTimeDetails.aspx", true);
                }
                else
                {
                    TimeServiceClient timeService = null;
                    TimeReturnValue returnValue = null;
                    try
                    {
                        timeService = new TimeServiceClient();
                        IRIS.Law.WebServiceInterfaces.Time.Time time = new IRIS.Law.WebServiceInterfaces.Time.Time();

                        time.ProjectId = (Guid)Session[SessionName.ProjectId];
                        time.FeeEarnerId = _logonSettings.UserDefaultFeeMemberId;
                        time.TimeTypeId = timeDetails.TimeTypeId;
                        time.Units = timeDetails.Units;
                        time.Notes = timeDetails.Notes;
                        time.Date = DateTime.Now.Date;
                        //No validation for limits from the web
                        time.DontCheckLimits = true;
                        time.BillingCodeID = 1; // Default to "Default - None Selected"

                        if (timeDetails.Id == 0)
                        {
                            //No timeId present. Add a new time entry
                            returnValue = timeService.AddTime(_logonSettings.LogonId, time, null, false);
                        }
                        else
                        {
                            //If a timeId is present then we are updating an existing time entry
                            time.Id = timeDetails.Id;
                            returnValue = timeService.UpdateTime(_logonSettings.LogonId, time, null);
                        }

                        if (returnValue.Success)
                        {
                            Session[SessionName.TimeDetails] = null;
                            //Redirect the user to the timesheet where entry would be listed
                            Response.Redirect("ViewTimesheet.aspx", true);
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

        #endregion

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

                TimeReturnValue returnValue = timeService.GetAddtionalDetailTime(_logonSettings.LogonId, time);

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

        #region Private Methods

        /// <summary>
        /// Loads the time entry.
        /// </summary>
        private void LoadTimeEntry(int timeId, string matterReference)
        {
            TimeServiceClient timeService = null;
            try
            {
                timeService = new TimeServiceClient();
                TimeReturnValue returnValue = timeService.GetTime(_logonSettings.LogonId, timeId);

                if (returnValue.Success)
                {
                    if (returnValue.Time != null)
                    {
                        Session[SessionName.ProjectId] = returnValue.Time.ProjectId;
                        GetClientDetails(matterReference);
                        GetTimeTypes(returnValue.Time.ProjectId);

                        _ddlTimeType.SelectedIndex = -1;
                        for (int i = 0; i < _ddlTimeType.Items.Count; i++)
                        {
                            if (GetValueOnIndexFromArray(_ddlTimeType.Items[i].Value, 0) == returnValue.Time.TimeTypeId.ToString())
                            {
                                _ddlTimeType.Items[i].Selected = true;
                                break;
                            }
                        }

                        _txtUnits.Text = returnValue.Time.Units.ToString();
                        _txtNotes.Text = returnValue.Time.Notes;
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

        /// <summary>
        /// Gets the client details.
        /// </summary>
        private void GetClientDetails(string matterReference)
        {
            ClientServiceClient clientService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;

                ClientSearchCriteria criteria = new ClientSearchCriteria();
                criteria.ClientReference = matterReference.Substring(0, 6);

                clientService = new ClientServiceClient();
                ClientSearchReturnValue returnValue = clientService.ClientSearch(_logonSettings.LogonId,
                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    if (returnValue.Clients != null)
                    {
                        Session[SessionName.MemberId] = returnValue.Clients.Rows[0].MemberId;
                        Session[SessionName.OrganisationId] = returnValue.Clients.Rows[0].OrganisationId;
                        Session[SessionName.ClientRef] = returnValue.Clients.Rows[0].ClientReference;
                        Session[SessionName.ClientName] = returnValue.Clients.Rows[0].Name;
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
                if (clientService != null)
                {
                    if (clientService.State != System.ServiceModel.CommunicationState.Faulted)
                        clientService.Close();
                }
            }
        }

        /// <summary>
        /// Gets the valid time types for the matter.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        private void GetTimeTypes(Guid projectId)
        {
            TimeServiceClient timeService = null;
            try
            {
                timeService = new TimeServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                TimeTypeSearchCriteria criteria = new TimeTypeSearchCriteria();
                criteria.IncludeArchived = false;
                criteria.ProjectId = projectId;
                TimeTypeSearchReturnValue returnValue = timeService.TimeTypeSearch(_logonSettings.LogonId,
                                                                            collectionRequest, criteria);

                if (returnValue.Success)
                {
                    if (returnValue.TimeTypes != null)
                    {
                        for (int i = 0; i < returnValue.TimeTypes.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = returnValue.TimeTypes.Rows[i].Description.ToString();
                            item.Value = returnValue.TimeTypes.Rows[i].Id.ToString().Trim() + "$" + Convert.ToString(returnValue.TimeTypes.Rows[i].CatId);
                            _ddlTimeType.Items.Add(item);
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

        /// <summary>
        /// Sets the default value for time type.
        /// </summary>
        private void SetDefaultTimeType()
        {
            ListItem defaultItem = _ddlTimeType.Items.FindByText("001 - Preparation");
            if (defaultItem != null)
            {
                _ddlTimeType.SelectedIndex = -1;
                _ddlTimeType.SelectedValue = defaultItem.Value;
            }
        }

        #endregion

        #region GetValueOnIndexFromArray
        /// <summary>
        /// If strAnyValue is Time Type
        /// index = 0 -> TimeTypeId
        /// index = 1 -> CatId
        /// </summary>
        /// <param name="strAnyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strAnyValue, int index)
        {
            try
            {
                string[] arrayString = strAnyValue.Split('$');
                return arrayString[index].Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

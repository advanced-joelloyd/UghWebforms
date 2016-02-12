using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using System.Data;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ServiceSearch : System.Web.UI.UserControl
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        private int _serviceRowCount;
        private int _serviceContactRowCount;
        private LogonReturnValue _logonSettings;

        #region Constants

        private const string NO_SERVICES_FOUND = "No service(s) found.";
        private const string NO_SERVICE_CONTACTS_FOUND = "No service contact(s) found.";

        #endregion

        #region Public Properties

        #region DisplayPopup
        /// <summary>
        /// If this property is set to "True", Search textbox & Search Image will be visible & the gridview height is fixed
        /// If this is "False", Search textbox & Search Image will not be visible & the gridview height is not fixed
        /// </summary>
        private bool _displayPopup = true;
        public Boolean DisplayPopup
        {
            get { return _displayPopup; }
            set
            {
                _txtService.Visible = value;
                _imgBtnSearch.Visible = value;
                _displayPopup = value;
                if (!value)
                {
                    _grdviewDivHeight.Style["height"] = "100%";
                    _modalpopupServiceSearch.Hide();
                }
                else
                {
                    _grdviewDivHeight.Style["height"] = "350px";
                }
            }
        }
        #endregion DisplayPopup

        #region ServiceText
        private string _serviceText;
        public string ServiceText
        {
            get
            {
                return _serviceText;
            }
            set
            {
                _txtService.Text = value;
            }
        }
        #endregion

        #region IndustryText
        private string _industryText;
        public string IndustryText
        {
            get
            {
                return _industryText;
            }
        }
        #endregion

        #region ServiceId
        private Guid _serviceOrgId;
        public Guid ServiceId
        {
            get
            {
                return _serviceOrgId;
            }
        }
        #endregion

        #region ServiceContactId
        private Guid _serviceContactId;
        public Guid ServiceContactId
        {
            get
            {
                return _serviceContactId;
            }
        }
        #endregion

        #region ValidatorClientID
        public string ValidatorClientID
        {
            get { return _rfvServiceReference.ClientID; }
        }
        #endregion

        #region EnableValidation
        /// <summary>
        /// Gets or sets a value indicating whether the client field is mandatory
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        public bool EnableValidation { get; set; }
        #endregion

        #region DisplayServiceContactTextbox

        private bool _displayServiceContactTextbox = true;

        /// <summary>
        /// Gets or sets a value indicating whether to display service contact textbox.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if service contact textbox is displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayServiceContactTextbox
        {
            get
            {
                return _displayServiceContactTextbox;
            }
            set
            {
                _displayServiceContactTextbox = value;
            }
        }

        #endregion

        #region set DisplayServiceContactGridview
        private bool _displayServiceContactGridview;
        /// <summary>
        /// Sets a value indicating whether to display service contact gridview. 
        /// The contacts will be displayed in a seperate grid when the user selects a service
        /// </summary>
        /// <value>
        /// 	<c>true</c> display service contact gridview; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayServiceContactGridview
        {
            set
            {
                _displayServiceContactGridview = value;

                if (value)
                {
                    //Reduce the height of the service grid to make room for the service contact grid
                    _grdviewDivHeight.Style["height"] = "150px";
                    _tableServiceContact.Style["display"] = "";
                }
                else
                {
                    //Service contact grid not required, so increase the size of the service grid
                    _grdviewDivHeight.Style["height"] = "350px";
                    _tableServiceContact.Style["display"] = "none";
                }
            }
        }
        #endregion

        #region Set Industry

        private int _industryId;

        /// <summary>
        /// Gets or sets the industry id.
        /// </summary>
        /// <value>The industry id.</value>
        public int IndustryId
        {
            get
            {
                return _industryId;
            }
            set
            {
                _industryId = value;
                SetIndustry();
            }
        }

        #endregion Set Industry

        #region Enable/Disable Industry selection

        public bool IsIndustryDropdownEnabled
        {
            get
            {
                return _ddlIndustry.Enabled;
            }
            set
            {
                _ddlIndustry.Enabled = value;
            }
        }

        #endregion

        #endregion Public Properties

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            //User is not logged in redirect to the login page
            if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }

            _logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];

            //Set the page size for the grids
            _grdSearchServiceList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
            _grdSearchServiceContactList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

            if (!IsPostBack)
            {
                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the client side changes to the value if we add this attribute in the markup
                _txtService.Attributes.Add("readonly", "readonly");
                BindIndustries();

                SetIndustry();

                //Show/Hide the textbox which displays the currently selected service/service contact
                _txtService.Visible = _displayServiceContactTextbox;

                //Check if service selection is mandatory or not
                if (EnableValidation)
                {
                    //selected service textbox must always be display if the service is a mandatory field
                    //as the error message is displayed as a tooltip on the textbox
                    _txtService.Visible = true;
                }
                else
                {
                    //Disable validation if service is not mandatory
                    _pnlValidation.Visible = false;
                    _spnMandatory.Visible = false;
                }

                //Display the search in a modal popup if required
                if (_displayPopup == false)
                {
                    _divCancelButton.Visible = false;
                    _trCloseLink.Style["display"] = "none";
                }
                else
                {
                    _modalpopupServiceSearch.CancelControlID = _btnCancel.ClientID;
                    _pnlServiceSearch.Style.Add("display", "none");
                    _trCloseLink.Style["display"] = "";
                }

                //If contacts for the service need to be displayed then add a new column which will allow
                //the user to select a service to view the contacts
                if (_displayServiceContactGridview)
                {
                    _grdSearchServiceList.Columns[0].Visible = true;
                }
                else
                {
                    _grdSearchServiceList.Columns[0].Visible = false;
                }
            }
        }
        #endregion

        #region Public Events

        #region On Error

        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

        /// <summary>
        /// Occurs when an error is encountered in the user control.
        /// </summary>
        public new event ErrorEventHandler Error;

        protected virtual void OnError(ErrorEventArgs e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        #endregion

        #region ServiceSelected

        public delegate void ServiceSelectedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the user clicks on the service name link.
        /// </summary>
        public event ServiceSelectedEventHandler ServiceSelected;
        protected virtual void OnServiceSelected(EventArgs e)
        {
            if (ServiceSelected != null)
            {
                ServiceSelected(this, e);
            }
        }

        #endregion ServiceSelected

        #region ServiceContactSelected

        public delegate void ServiceContactSelectedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the user clicks on the service contac name link.
        /// </summary>
        public event ServiceContactSelectedEventHandler ServiceContactSelected;
        protected virtual void OnServiceContactSelected(EventArgs e)
        {
            if (ServiceContactSelected != null)
            {
                ServiceContactSelected(this, e);
            }
        }

        #endregion ServiceContactSelected

        #region SearchButtonClick

        public event ImageClickEventHandler SearchButtonClick
        {
            add
            {
                _imgBtnSearch.Click += value;
            }
            remove
            {
                _imgBtnSearch.Click -= value;
            }
        }

        #endregion SearchButtonClick

        #endregion Public Events

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

                if (_ddlIndustry.SelectedIndex > 0)
                {
                    _hdnServiceName.Value = GetIndustryValue(_ddlIndustry.SelectedItem.Text);
                }

                _grdSearchServiceList.EmptyDataText = NO_SERVICES_FOUND;
                _grdSearchServiceList.PageIndex = 0;
                _grdSearchServiceList.DataSourceID = _odsSearchService.ID;

                //Clear the contacts grid
                _grdSearchServiceContactList.EmptyDataText = string.Empty;
                _grdSearchServiceContactList.DataSourceID = "";

                if (_displayPopup == false)
                {
                    _pnlServiceSearch.Style.Add("display", "");
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs errorEventArgs = new ErrorEventArgs();
                errorEventArgs.Message = ex.Message;
                OnError(errorEventArgs);
            }
            _modalpopupServiceSearch.Show();
        }
        #endregion

        #region GetIndustryValue
        private string GetIndustryValue(string industryValue)
        {
            try
            {
                string value = string.Empty;
                if (industryValue.LastIndexOf("->") > 0)
                {
                    value = industryValue.Substring(industryValue.LastIndexOf("->") + 2);
                }
                else
                {
                    value = industryValue;
                }
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Reset button event
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            _modalpopupServiceSearch.Show();
        }
        #endregion

        #region Service RowDatabound
        protected void _grdSearchServiceList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                                img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchServiceList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                // checking if the header link is the user's choice
                                if (_grdSearchServiceList.SortExpression == lnk.CommandArgument)
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
                    Label lblAddress = (Label)e.Row.FindControl("_lblAddress");
                    Label lblAddressStreetNo = (Label)e.Row.FindControl("_lblAddressStreetNo");
                    Label lblAddressLine1 = (Label)e.Row.FindControl("_lblAddressLine1");
                    ServiceSearchItem service = (ServiceSearchItem)e.Row.DataItem;

                    if (service.AddressHouseName != "System.DBNull" && service.AddressStreetNo != "System.DBNull" && service.AddressLine1 != "System.DBNull")
                    {
                        service.AddressHouseName = service.AddressHouseName + " " + service.AddressStreetNo;
                        service.AddressHouseName = service.AddressHouseName.Trim() + " " + service.AddressLine1;
                    }

                    lblAddress.Text = service.AddressHouseName;

                    // Truncate fields having length more than 20
                    // Truncate large service name
                    if (service.Name.Length > 20)
                    {
                        LinkButton lnkBtnServiceName = (LinkButton)e.Row.FindControl("_lnkServiceName");
                        lnkBtnServiceName.Text = service.Name;//.Substring(0, 20) + "...";
                    }

                    // Truncate large house name
                    if (service.AddressHouseName.Length > 20)
                    {
                        Label lblAddressHouseName = (Label)e.Row.FindControl("_lblAddress");
                        lblAddressHouseName.Text = service.AddressHouseName.Substring(0, 20) + "...";
                    }

                    // Truncate large house name
                    if (service.AddressLine1.Length > 20)
                    {
                        lblAddressLine1.Text = service.AddressLine1.Substring(0, 20) + "...";
                    }

                     // Truncate large town name
                    if (service.AddressTown.Length > 20)
                    {
                        Label lblAddressTown = (Label)e.Row.FindControl("_lblTown");
                        lblAddressTown.Text = service.AddressTown.Substring(0, 20) + "...";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }
        #endregion

        #region ServiceContact RowDatabound
        protected void _grdSearchServiceContactList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                                img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchServiceContactList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                                // checking if the header link is the user's choice
                                if (_grdSearchServiceContactList.SortExpression == lnk.CommandArgument)
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
                    LinkButton lnkContactName = (LinkButton)e.Row.FindControl("_lnkContactName");
                    Label _lblPersonTitle = (Label)e.Row.FindControl("_lblPersonTitle");
                    Label _lblPersonSurname = (Label)e.Row.FindControl("_lblPersonSurname");
                    ServiceContactSearchItem serviceContact = (ServiceContactSearchItem)e.Row.DataItem;
                    string displayName = "";

                    if (_lblPersonSurname.Text == "To Whom It May Concern" || _lblPersonSurname.Text == "Default Contact")
                    {
                        displayName = "Default Contact";
                    }
                    else
                    {
                        if (_lblPersonSurname.Text == "To Whom It May Concern" || _lblPersonSurname.Text == "Default Contact")
                        {
                            displayName = "Default Contact";
                        }
                        else
                        {
                            displayName = _lblPersonTitle.Text.Trim() + " " + lnkContactName.Text.Trim() + " " + _lblPersonSurname.Text.Trim();
                        }
                    }

                    lnkContactName.Text = displayName;

                    // Truncates fields having length more than 20
                    // Truncate large person name
                    if (serviceContact.PersonName.Length > 20)
                    {
                        LinkButton lnkBtnPersonName= (LinkButton)e.Row.FindControl("_lnkContactName");
                        lnkBtnPersonName.Text = serviceContact.PersonName;//.Substring(0, 20) + "...";
                    }

                    // Truncate large person surname
                    if (serviceContact.PersonSurname.Length > 20)
                    {
                        Label lblPersonSurname = (Label)e.Row.FindControl("_lblPersonSurname");
                        lblPersonSurname.Text = serviceContact.PersonSurname.Substring(0, 20) + "...";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }
        #endregion

        #region Gridview RowCommand

        protected void _grdSearchServiceList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //Get the contacts for the service
                if (e.CommandName == "contact")
                {
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    _hdnServiceOrgId.Value = _grdSearchServiceList.DataKeys[rowId].Values["Id"].ToString();
                    if (!string.IsNullOrEmpty(_hdnServiceOrgId.Value))
                    {
                        _hdnRefreshServiceContact.Value = "true";
                        _grdSearchServiceContactList.EmptyDataText = NO_SERVICE_CONTACTS_FOUND;
                        _grdSearchServiceContactList.PageIndex = 0;
                        _grdSearchServiceContactList.DataSourceID = _odsServiceContactSearch.ID;

                        if (_displayPopup == false)
                        {
                            _pnlServiceSearch.Style.Add("display", "");
                        }
                        else
                        {
                            _modalpopupServiceSearch.Show();
                        }
                    }
                }
                else if (e.CommandName == "select")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    _hdnServiceOrgId.Value = _grdSearchServiceList.DataKeys[rowId].Values["Id"].ToString();
                    _serviceOrgId = new Guid(_grdSearchServiceList.DataKeys[rowId].Values["Id"].ToString());

                    if (row.Cells[0].FindControl("_lnkServiceName") != null)
                    {
                        _serviceText = ((LinkButton)row.Cells[0].FindControl("_lnkServiceName")).Text;
                        _industryText = _hdnServiceName.Value;
                    }

                    if (_displayPopup == true)
                    {
                        _txtService.Text = _serviceText + " - " + _industryText;
                        OnServiceSelected(EventArgs.Empty);
                        _modalpopupServiceSearch.Hide();
                    }
                }
                else
                {
                    _modalpopupServiceSearch.Show();
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }
        #endregion

        #region Gridview RowCommand
        protected void _grdSearchServiceContactList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "select")
                {
                    GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                    _serviceContactId = new Guid(_grdSearchServiceContactList.DataKeys[rowId].Values["MemberId"].ToString());
                    if (row.Cells[0].FindControl("_lnkContactName") != null)
                    {
                        _serviceText = ((LinkButton)row.Cells[0].FindControl("_lnkContactName")).Text;
                        _modalpopupServiceSearch.Hide();
                    }
                    OnServiceContactSelected(EventArgs.Empty);
                }
                else
                {
                    _modalpopupServiceSearch.Show();
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }
        #endregion

        #region BindIndustries
        /// <summary>
        /// Gets the industries.
        /// </summary>
        private void BindIndustries()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                IndustrySearchCriteria searchCriteria = new IndustrySearchCriteria();
                IndustrySearchReturnValue returnValue = contactService.IndustrySearch(_logonSettings.LogonId,
                                                                collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    //Add a blank item
                    AppFunctions.AddDefaultToDropDownList(_ddlIndustry);

                    //Generate the items to be displayed in the Industry drop down list
                    //Get the main items
                    string industryText = string.Empty;
                    foreach (IndustrySearchItem industry in returnValue.Industries.Rows)
                    {
                        if (industry.ParentId == 0)
                        {
                            industryText = industry.Name;
                            _ddlIndustry.Items.Add(new ListItem(industryText, industry.Id.ToString()));

                            // Call method to get the sub items
                            GetIndustrySubItems(returnValue.Industries.Rows, industryText, industry.Id);
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
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }

        private void GetIndustrySubItems(IndustrySearchItem[] industrySearchItems, string industryText, int industryId)
        {
            foreach (IndustrySearchItem industry in industrySearchItems)
            {
                // If the IndustryParentID is not 0 and the IndustryParentID equals the industry id of the item
                // you specified then add a sub item.
                if (industry.ParentId != 0 && industry.ParentId == industryId)
                {
                    string itemText = industryText + "->" + industry.Name;
                    _ddlIndustry.Items.Add(new ListItem(itemText, industry.Id.ToString()));

                    //Call recursively to get the sub items
                    GetIndustrySubItems(industrySearchItems, itemText, industry.Id);
                }
            }
        }
        #endregion

        #region BindGridView Service Contact

        public int GetServiceContactRowsCount(string serviceOrgId, bool forceRefresh)
        {
            return _serviceContactRowCount;
        }

        /// <summary>
        /// Searches for clients that match the search criteria.
        /// </summary>
        public ServiceContactSearchItem[] SearchContactService(int startRow, int pageSize, string sortBy, string serviceOrgId, bool forceRefresh)
        {
            TimeServiceClient timeService = null;
            ServiceContactSearchItem[] serviceContact = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    Guid orgId = new Guid(serviceOrgId);

                    timeService = new TimeServiceClient();
                    ServiceContactSearchReturnValue returnValue = timeService.ServiceContactSearch(_logonId, collectionRequest, orgId, sortBy);

                    if (returnValue.Success)
                    {
                        _serviceContactRowCount = returnValue.ServiceContact.TotalRowCount;
                        serviceContact = returnValue.ServiceContact.Rows;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
                }
                return serviceContact;
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

        protected void _odsSearchServiceContact_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding grid
            if (e.Exception != null)
            {
                ErrorEventArgs errorEventArgs = new ErrorEventArgs();
                errorEventArgs.Message = e.Exception.InnerException.Message;
                OnError(errorEventArgs);
                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefreshServiceContact.Value = "false";
        }
        #endregion

        #region BindGridView Service

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetServiceRowsCount(string industry, bool forceRefresh)
        {         
            return _serviceRowCount;
        }

        /// <summary>
        /// Searches for clients that match the search criteria.
        /// </summary>
        public ServiceSearchItem[] SearchService(int startRow, int pageSize, string sortBy, string industry, bool forceRefresh)
        {
            TimeServiceClient timeService = null;
            ServiceSearchItem[] services = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    ServiceSearchCriteria criteria = new ServiceSearchCriteria();
                    criteria.IndustryId = Convert.ToInt32(industry);
                    criteria.OrderBy = sortBy;

                    timeService = new TimeServiceClient();
                    ServiceSearchReturnValue returnValue = timeService.ServiceSearchOnIndustry(_logonId,
                                                collectionRequest, criteria);

                    if (returnValue.Success)
                    {
                        _serviceRowCount = returnValue.Service.TotalRowCount;
                        services = returnValue.Service.Rows;
                    }
                    else
                    {
                        throw new Exception(returnValue.Message);
                    }
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

        protected void _odsSearchService_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding grid
            if (e.Exception != null)
            {
                ErrorEventArgs errorEventArgs = new ErrorEventArgs();
                errorEventArgs.Message = e.Exception.InnerException.Message;
                OnError(errorEventArgs);
                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefresh.Value = "false";
        }
        #endregion

        #region Search_Click

        protected void _imgBtnSearch_Click(object sender, ImageClickEventArgs e)
        {
            _modalpopupServiceSearch.Show();
        }

        #endregion Search_Click

        #region Set Industry

        /// <summary>
        /// Selects the industry from the dropdown list.
        /// </summary>
        private void SetIndustry()
        {
            try
            {
                if (_ddlIndustry.Items.FindByValue(_industryId.ToString()) != null)
                {
                    //If the industry exists make it the current selection
                    //Bind the service grid for the selected value
                    _ddlIndustry.SelectedValue = _industryId.ToString();
                    _btnSearch_Click(null, null);
                    //Reset service contact grid
                    _grdSearchServiceContactList.EmptyDataText = string.Empty;
                    _grdSearchServiceContactList.DataSourceID = "";
                }
                else
                {
                    _ddlIndustry.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Set Industry

        #region Reset

        /// <summary>
        /// Resets the grids and industry dropdown.
        /// </summary>
        public void Reset()
        {
            _ddlIndustry.SelectedIndex = 0;

            //Reset service grid
            _grdSearchServiceList.EmptyDataText = string.Empty;
            _grdSearchServiceList.DataSourceID = "";

            //Reset service contact grid
            _grdSearchServiceContactList.EmptyDataText = string.Empty;
            _grdSearchServiceContactList.DataSourceID = "";
        }

        #endregion Reset
    }
}
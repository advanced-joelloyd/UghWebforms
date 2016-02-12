using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using IRIS.Law.WebServiceInterfaces.Time;

namespace IRIS.Law.WebApp.Pages.Contact
{
    public partial class SearchContact : BasePage
    {
        #region Private Variables

        private int _serviceRowCount;
        private int _serviceContactRowCount;
        private LogonReturnValue _logonSettings;

        #endregion

        #region Constants

        private const string NO_SERVICES_FOUND = "No service(s) found.";
        private const string NO_SERVICE_CONTACTS_FOUND = "No service contact(s) found.";

        #endregion

        #region Public Properties

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

        #endregion

        #region Protected Methods

        /// <summary>
        /// Page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
            _logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];

            //Set the page size for the grids
            _grdSearchServiceList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
            _grdSearchServiceContactList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

            if (!IsPostBack)
            {
                BindIndustries();

                SetIndustry();
            }
        }

        protected void _contactSearch_ContactSelected(object sender, EventArgs e)
        {
            try
            {
                _ucViewContactDetails.MemberId = _contactSearch.MemberId;
                _ucViewContactDetails.OrganisationId = _contactSearch.OrganisationId;
                _ucViewContactDetails.IsServiceContact = false;
                _ucViewContactDetails.DisplayPopup();
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                        Label lblServiceName = (Label)e.Row.FindControl("_lblServiceName");
                        lblServiceName.Text = service.Name.Substring(0, 20) + "...";
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
                throw ex;
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
                    ServiceContactSearchItem serviceContact = (ServiceContactSearchItem)e.Row.DataItem;
                    LinkButton lblContactName = (LinkButton)e.Row.FindControl("_lnkContactName");
                    Label _lblPersonTitle = (Label)e.Row.FindControl("_lblPersonTitle");
                    Label _lblPersonSurname = (Label)e.Row.FindControl("_lblPersonSurname");
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
                            displayName = _lblPersonTitle.Text.Trim() + " " + lblContactName.Text.Trim() + " " + _lblPersonSurname.Text.Trim();
                        }
                    }

                    lblContactName.Text = displayName;

                    // Truncates fields having length more than 20
                    // Truncate large person name
                    if (serviceContact.PersonName.Length > 20)
                    {
                        lblContactName.Text = serviceContact.PersonName.Substring(0, 20) + "...";
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
                throw ex;
            }
        }
        #endregion

        #region Gridview RowCommand

        protected void _grdSearchServiceContactList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                try
                {
                    int rowIndex = ((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex;
                    _ucViewContactDetails.MemberId = (Guid)_grdSearchServiceContactList.DataKeys[rowIndex].Value;
                    _ucViewContactDetails.IsServiceContact = true;
                    _ucViewContactDetails.DisplayPopup();
                }
                catch (Exception ex)
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = ex.Message;
                }
            }
        }

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
                    }
                }
            }
            catch (Exception ex)
            {
                _lblMessage.CssClass = "errorMessage";
                _lblMessage.Text = ex.Message;
            }
        }
        #endregion

        #endregion

        #region Private Methods

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
                _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                _lblMessage.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {

                _lblMessage.Text = ex.Message;
                _lblMessage.CssClass = "errorMessage";
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
                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefresh.Value = "false";
        }

        

        #endregion


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

        #endregion

    }
}

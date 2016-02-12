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
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ContactSearch : System.Web.UI.UserControl
    {
        int _contactSearchRowCount;

        #region Public Properties

        private bool _displayContactAsLink;

        /// <summary>
        /// True - Displays the contact name as a clickable link.
        /// False - Displays the contact name as a label.
        /// </summary>
        /// <value>
        /// 	<c>true</c> display contact as link; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayContactAsLink
        {
            get
            {
                return _displayContactAsLink;
            }
            set
            {
                _displayContactAsLink = value;
            }
        }

        /// <summary>
        /// If this property is set to "True", Search textbox & Search Image will be visible & the gridview height is fixed
        /// If this is "False", Search textbox & Search Image will not be visible & the gridview height is not fixed
        /// </summary>
        public bool DisplayPopup
        {
            get
            {
                return _imgSearchContactPopup.Visible;
            }
            set
            {
                _tblContactName.Visible = value;
                if (!value)
                {
                    _grdviewDivHeight.Style["height"] = "100%";
                    _modalpopupContactSearch.Hide();
                }
                else
                {
                    _grdviewDivHeight.Style["height"] = "350px";
                }
            }
        }

        /// <summary>
        /// Gets the selected organisation id.
        /// </summary>
        /// <value>The organisation id.</value>
        public Guid OrganisationId
        {
            get
            {
                if (_hdnSelectedOrgId != null && _hdnSelectedOrgId.Value != string.Empty)
                {
                    return new Guid(_hdnSelectedOrgId.Value);
                }
                return DataConstants.DummyGuid;
            }
        }

        /// <summary>
        /// Gets the selected member id.
        /// </summary>
        /// <value>The member id.</value>
        public Guid MemberId
        {
            get
            {
                if (_hdnSelectedMemId != null && _hdnSelectedMemId.Value != string.Empty)
                {
                    return new Guid(_hdnSelectedMemId.Value);
                }
                return DataConstants.DummyGuid;
            }
        }

        /// <summary>
        /// Gets or sets the name of the contact that is selected.
        /// </summary>
        /// <value>The name of the contact.</value>
        public string ContactName
        {
            get
            {
                return _txtContactName.Text;
            }
            set
            {
                _txtContactName.Text = value;
            }
        }

        private bool _displayContactNameTextbox;
        /// <summary>
        /// Gets or sets a value indicating whether contact name textbox is displayed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if contact name textbox is visible; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayContactNameTextbox
        {
            get
            {
                return _displayContactNameTextbox;
            }
            set
            {
                _displayContactNameTextbox = value;
            }
        }

        private bool _displayServiceNameAsLinkable = true;
        /// <summary>
        /// Sets a value indicating whether service name is displayed as Label or Link
        /// </summary>
        /// <value>
        /// 	<c>if true</c>service name Link is otherwise, <c>Label is displayed</c>.
        /// </value>
        public bool DisplayServiceNameAsLinkable
        {
            set
            {
                _displayServiceNameAsLinkable = value;
            }
        }

        #endregion Public Properties

        #region Public Events

        public delegate void ContactSelectedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the user clicks on the contact name link.
        /// </summary>
        public event ContactSelectedEventHandler ContactSelected;
        protected virtual void OnContactSelected(EventArgs e)
        {
            if (ContactSelected != null)
            {
                ContactSelected(this, e);
            }
        }

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
        
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the client side changes to the value if we add this attribute in the markup
                _txtContactName.Attributes.Add("readonly", "readonly");

                _txtContactName.Visible = _displayContactNameTextbox;

                //If the search is displayed as a popup then display a textbox to hold the selected value
                if (!DisplayPopup)
                {
                    _divCancelButton.Visible = false;
                    _trCloseLink.Style["display"] = "none";
                }
                else
                {
                    _modalpopupContactSearch.CancelControlID = _btnCancel.ClientID;
                    _pnlContactSearch.Style.Add("display", "none");
                    _trCloseLink.Style["display"] = "";
                }

                if (DisplayContactAsLink)
                {
                    _grdSearchContactList.Columns[0].Visible = true;
                }
                else
                {
                    _grdSearchContactList.Columns[1].Visible = true;
                }
                if (_displayServiceNameAsLinkable)
                {
                    _tdSectionHeader.Visible = true;
                }
                else
                {
                    _tdSectionHeader.Visible = false;
                }
            }
            
            _grdSearchContactList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
        }

        protected void _btnSearch_Click(object sender, EventArgs e)
        {
            _grdSearchContactList.PageIndex = 0;
            _grdSearchContactList.DataSourceID = _odsContactSearch.ID;
            _modalpopupContactSearch.Show();
            _hdnRefresh.Value = "true";
        }

        protected void _btnReset_Click(object sender, EventArgs e)
        {
            _modalpopupContactSearch.Show();
        }

        protected void _grdSearchContactList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                if (e.CommandSource.GetType() == typeof(LinkButton))
                {
                    _txtContactName.Text = ((LinkButton)e.CommandSource).Text;
                    GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    HiddenField hdnOrgId = (HiddenField)row.FindControl("_hdnOrgId");
                    HiddenField hdnMemId = (HiddenField)row.FindControl("_hdnMemId");
                    _hdnSelectedOrgId.Value = hdnOrgId.Value;
                    _hdnSelectedMemId.Value = hdnMemId.Value;
                    OnContactSelected(EventArgs.Empty);
                    _modalpopupContactSearch.Hide();
                }
            }
            else
            {
                _modalpopupContactSearch.Show();
            }

        }

        protected void _grdSearchContactList_RowDataBound(Object sender, GridViewRowEventArgs e)
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
                            img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchContactList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                            // checking if the header link is the user's choice
                            if (_grdSearchContactList.SortExpression == lnk.CommandArgument)
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

        #region BindGridView

        public int ContactSearchRowCount(string contactName, string organisation, string houseNo,
                                                 string POBox, string postCode, string town, bool forceRefresh)
        {
            return _contactSearchRowCount;
        }

        public ContactSearchItem[] SearchContact(int startRow, int pageSize, string sortBy, string contactName, string organisation, 
                                                 string houseNo, string POBox, string postCode, string town, 
                                                 bool forceRefresh)
        {
            ContactServiceClient contactService = null;
            ContactSearchItem[] contacts = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                    
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = forceRefresh;
                    collectionRequest.StartRow = startRow;
                    collectionRequest.RowCount = pageSize;

                    ContactSearchCriteria criteria = new ContactSearchCriteria();
                    criteria.ContactName = contactName;
                    criteria.HouseNumber = houseNo;
                    criteria.Organisation = organisation;
                    criteria.POBox = POBox;
                    criteria.PostCode = postCode;
                    criteria.Town = town;
                    criteria.OrderBy = sortBy;

                    contactService = new ContactServiceClient();

                    ContactSearchReturnValue returnValue =
                                                contactService.ContactSearch(_logonId,
                                                                             collectionRequest,
                                                                             criteria);
                    if (returnValue.Success)
                    {
                        contacts = returnValue.Contacts.Rows;
                        _contactSearchRowCount = returnValue.Contacts.TotalRowCount;
                    }
                    else
                    {
                        ErrorEventArgs e = new ErrorEventArgs();
                        e.Message = returnValue.Message;
                        OnError(e);
                    }
                }
                return contacts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                    contactService.Close();
            }
        }

        protected void _odsContactSearch_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
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
    }
}
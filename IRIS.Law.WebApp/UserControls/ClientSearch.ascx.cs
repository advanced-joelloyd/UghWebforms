using System;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebApp.App_Code;
using System.Linq;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Earner;
using System.Configuration;
using System.Web;
using IRIS.Law.WebApp.MasterPages;
using IRIS.Law.WebServiceInterfaces.Matter;
using System.Web.UI;
using System.Drawing;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ClientSearch : System.Web.UI.UserControl
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        private int _clientRowCount;
        private int _clientMatterRowCount;
        private LogonReturnValue _logonSettings;
        private bool _hideLoading;
        private bool _width100pc;
        private bool IsReset = false;

        public const string NoClientSelected = "";


        protected override void OnInit(EventArgs e)
        {
            if (!AddAsLink)
            {
            //    this._modalpopupClientSearch.TargetControlID = this._lnkClientSelect.ClientID;
            }
            else
            {
            //    this._modalpopupClientSearch.TargetControlID = this._lnkClientSelect.ClientID;
            }

            base.OnInit(e);

        }

        #region Properties

        #region Width100pc
        public bool Width100pc
        {
            get { return _width100pc; }
            set { _width100pc = value; }
        }
        #endregion


        #region HideLoading
        public bool HideLoading
        {
            get { return _hideLoading; }
            set { _hideLoading = value; }
        }
        #endregion


        #region SearchText
        public string SearchText
        {
            get { return _txtClientReference.Text; }
            set { _txtClientReference.Text = value; }
        }
        #endregion

        #region SearchClientRefTextBoxClientID
        public string SearchClientRefTextBoxClientID
        {
            get { return _txtClientReference.ClientID; }
        }
        #endregion

        #region DisplayClientReference
        public bool DisplayClientName
        {
            get;
            set;
        }
        #endregion

        public bool AddAsLink { get; set; }


        #region DisplayPopup
        /// <summary>
        /// If this property is set to "True", Search textbox & Search Image will be visible & the gridview height is fixed
        /// If this is "False", Search textbox & Search Image will not be visible & the gridview height is not fixed
        /// </summary>
        public Boolean DisplayPopup
        {
            get
            {
                if (_hdnDisplayPopup != null && _hdnDisplayPopup.Value != string.Empty)
                {
                    return Convert.ToBoolean(_hdnDisplayPopup.Value);
                }
                return false;
            }
            set
            {

                if (!AddAsLink)
                {
                    _txtClientReference.Visible = value;
                    _imgTextControl.Visible = value;



                    this._lnkClientSelect.Visible = false;
                }
                else
                {
                    this._lnkClientSelect.Visible = true;
                    _txtClientReference.Visible = false;
                    _imgTextControl.Visible = false;

                    this._imgTextControl.Width = 1;
                    this._imgTextControl.Height = 1;

                }

                if (!value)
                {
                    _grdviewDivHeight.Style["height"] = "100%";
                    _modalpopupClientSearch.Hide();
                }
                else
                {
                    _grdviewDivHeight.Style["height"] = "350px";
                }
                if (_hdnDisplayPopup != null)
                {
                    _hdnDisplayPopup.Value = value.ToString();
                }
            }
        }
        #endregion

        #region EnableValidation
        /// <summary>
        /// Gets or sets a value indicating whether the client field is mandatory
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        public bool EnableValidation { get; set; }
        #endregion

        #region ClientId

        public Guid ClientId
        {
            get
            {
                if (_hdnClientId != null && _hdnClientId.Value != string.Empty)
                {
                    return new Guid(_hdnClientId.Value);
                }
                return DataConstants.DummyGuid;
            }
            set
            {
                if (_hdnClientId != null)
                {
                    _hdnClientId.Value = value.ToString();
                }
            }
        }
        #endregion

        public string LinkText { get; set; }

        #region Matter Ref

        public string MatterReference
        {
            get
            {
                if (_hdnProjectRef != null)
                {
                    return _hdnProjectRef.Value;
                }
                return string.Empty;
            }
        }
        #endregion

        #region ProjectId

        public Guid ProjectId
        {
            get
            {
                if (_hdnProjectId != null && _hdnProjectId.Value != string.Empty)
                {
                    return new Guid(_hdnProjectId.Value);
                }
                return DataConstants.DummyGuid;
            }
        }
        #endregion

        #region IsMember
        public bool IsMember
        {
            get
            {
                if (_hdnIsMember != null && _hdnIsMember.Value != string.Empty)
                {
                    return Convert.ToBoolean(_hdnIsMember.Value);
                }
                return true;
            }
            set
            {
                if (_hdnIsMember != null)
                {
                    _hdnIsMember.Value = value.ToString();
                }
            }
        }
        #endregion

        #region SetSession
        /// <summary>
        /// Gets or sets a value indicating whether the Session for Client Matter is to be set or not
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        private bool _setSession = true;
        public bool SetSession
        {
            get
            {
                return _setSession;
            }
            set
            {
                _setSession = value;
            }
        }
        #endregion

        #region ClientRef
        public string ClientRef
        {
            get
            {
                if (_hdnClientRef != null)
                {
                    return _hdnClientRef.Value;
                }
                return string.Empty;
            }
        }
        #endregion

        #region ClientName
        public string ClientName
        {
            get
            {
                if (_hdnClientName != null)
                {
                    return _hdnClientName.Value;
                }
                return string.Empty;
            }
        }
        #endregion

        #region MemberId
        private Guid _memberId;
        public Guid MemberId
        {
            get
            {
                return _memberId;
            }
            set
            {
                _memberId = value;
            }
        }
        #endregion

        #region OrganisationId
        private Guid _organisationId;
        public Guid OrganisationId
        {
            get
            {
                return _organisationId;
            }
            set
            {
                _organisationId = value;
            }
        }
        #endregion

        #region DisplayClientNameTextbox

        private bool _displayClientNameTextbox = true;

        /// <summary>
        /// Gets or sets a value indicating whether to display client name textbox next to the search button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the client name textbox is displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayClientNameTextbox
        {
            get
            {
                return _displayClientNameTextbox;
            }
            set
            {
                _displayClientNameTextbox = value;
            }
        }

        #endregion DisplayClientNameTextbox

        #region DisplayMatterLinkable

        private bool _displayMatterLinkable;
        /// <summary>
        /// This property needs to be set as false on pages 
        /// where matters for clients is required to be non-linkable.
        /// </summary>
        public bool DisplayMatterLinkable
        {
            set
            {
                _displayMatterLinkable = value;
            }
        }

        #endregion

        #region DisplayMattersForClientGridview

        private bool _displayMattersForClientGridview;
        public bool DisplayMattersForClientGridview
        {
            set
            {
                _displayMattersForClientGridview = value;

                if (value)
                {
                    _grdviewDivHeight.Style["height"] = "150px";
                    _tableMattersForClient.Style["display"] = "";
                }
                else
                {
                    _grdviewDivHeight.Style["height"] = "350px";
                    _tableMattersForClient.Style["display"] = "none";
                }
                ShowHideMattersLink();
            }
        }

        #endregion DisplayMattersForClientGridview

        private string _asyncPostbackTriggers;
        /// <summary>
        /// Comma seperated control ids that trigger the asynchronous postback of the address.
        /// </summary>
        /// <value>The async postback triggers.</value>
        public string AsyncPostbackTriggers
        {
            get
            {
                return _asyncPostbackTriggers;
            }
            set
            {
                _asyncPostbackTriggers = value;
            }
        }

        #endregion

        #region DisableClient

        /// <summary>
        /// Disables the client in cheque request edit mode.
        /// </summary>
        public bool DisableClient
        {
            set
            {
                _txtClientReference.Enabled = !value;
                _imgTextControl.Visible = value;
            }
        }

        #endregion

        #region Validation Group

        public string ValidationGroup
        {
            get { return _rfvClientReference.ValidationGroup; }
            set { _rfvClientReference.ValidationGroup = value; }
        }

        #endregion

        #region public Events

        public delegate void ClientReferenceChangedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the client reference changes.
        /// </summary>
        public event ClientReferenceChangedEventHandler ClientReferenceChanged;
        protected virtual void OnClientReferenceChanged(EventArgs e)
        {
            if (ClientReferenceChanged != null)
            {
                ClientReferenceChanged(this, e);
            }
        }

        public delegate void MatterSelectedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the matter is selected.
        /// </summary>
        public event MatterSelectedEventHandler MatterSelected;
        protected virtual void OnMatterSelected(EventArgs e)
        {
            if (MatterSelected != null)
            {
                MatterSelected(this, e);
            }
        }
        #endregion

        #region Page load
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];
            }
            
            //Set the page size for the grids
            _grdSearchClientList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

            this._lnkClientSelect.Text = this.LinkText;

            if (AddAsLink)
            {
                this._modalpopupClientSearch.TargetControlID = "_lnkClientSelect";

                this._lnkClientSelect.Visible = true;

                this._imgTextControl.Width = 1;
                this._imgTextControl.Height = 1;
                this._txtClientReference.Visible = false;
            }
            else
            {
                this._modalpopupClientSearch.TargetControlID = "_imgTextControl";

                this._lnkClientSelect.Visible = false;
            //    this._txtClientReference.Visible = true;

                this._imgTextControl.Width = 20;
                this._imgTextControl.Height = 20;
            }

            if (_logonSettings.UserType == (int)DataConstants.UserType.Client)
            {
                this._lnkClientSelect.Visible = false;
                _imgTextControl.Visible = false;
            }

            if (_hideLoading)
            {
                _pnlUpdateProgressClientSearch.Visible = false;
              
            }

            if (_width100pc)
            {
                _tbSearch.Width = "100%";
            }

            if (!IsPostBack)
            {
                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the client side changes to the value if we add this attribute in the markup
                _txtClientReference.Attributes.Add("readonly", "readonly");

                if (EnableValidation == false)
                {
                    _pnlValidation.Visible = false;
                }

                //Disply the selected client's name or reference
                if (DisplayClientName)
                {
                    _txtClientReference.CssClass = "textBox";
                }
                else
                {
                    _txtClientReference.CssClass = "textBoxSmall";
                }

                if (DisplayPopup == false)
                {
                    _divCancelButton.Visible = false;
                    _trCloseLink.Style["display"] = "none";
                }
                else
                {
                    _modalpopupClientSearch.CancelControlID = _btnCancel.ClientID;
                    _ClientSearchContainer.Style.Add("display", "none");
                    //_pnlClientSearch.Style.Add("display", "none");
                    _trCloseLink.Style["display"] = "";
                    //_tbSearch.Style.Add("width", "620px");
                }

                if (!this.AddAsLink)
                    _txtClientReference.Visible = _displayClientNameTextbox;
                else
                    _txtClientReference.Visible = false;

                BindPartner();
                ShowHideMattersLink();
            } 
            _lblMessage.Text = string.Empty;
        }

        /// <summary>
        /// Shows the hide matters link.
        /// </summary>
        private void ShowHideMattersLink()
        {
            //Display a link which the user can click to view the client matters
            _grdSearchClientList.Columns[0].Visible = _displayMattersForClientGridview;
        }

        private void BindPartner()
        {
            EarnerServiceClient earnerService = null;
            try
            {
                PartnerSearchCriteria criteria = new PartnerSearchCriteria();
                earnerService = new EarnerServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                PartnerSearchReturnValue returnValue = new PartnerSearchReturnValue();
                returnValue = earnerService.PartnerSearch(_logonSettings.LogonId, collectionRequest, criteria);

                if (returnValue.Success)
                {
                    foreach (PartnerSearchItem partner in returnValue.Partners.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(partner.PersonTitle, partner.Name, partner.Surname);
                        item.Value = partner.PartnerId.ToString();
                        _ddlPartner.Items.Add(item);
                    }
                    _ddlPartner.Items.Insert(0, new ListItem("All Partners", ""));
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
                if (earnerService != null) 
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
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
                //Set force refresh to true so that data is not retrieved from the cache
            try
            {
                _hdnRefresh.Value = "true";

                if (DisplayPopup == false)
                {
                    _pnlClientSearch.Style.Add("display", "");
                }
                _grdSearchClientList.Visible = true;
                _grdSearchClientList.PageIndex = 0;
                _grdSearchClientList.DataSourceID = _odsSearchClient.ID;

                _grdMattersForClient.Visible = false;

                _modalpopupClientSearch.Show();
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

        #region Reset button event
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            IsReset = true;

            _txtSurname.Text="";
            _txtNINo.Text="";
            _ccDOBDate.DateText="";
            _txtPostcode.Text="";
            _txtTown.Text="";
            _ddlPartner.SelectedValue="";
            _grdSearchClientList.Visible=false;
            _grdMattersForClient.Visible = false;
            _lblMessage.Text="";


            _modalpopupClientSearch.Show();
        }
        #endregion

        #region BindGridView

        /// <summary>
        /// Gets the client rows count used to create the pager for the grid.
        /// </summary>
        public int GetClientRowsCount(string name, string NINo, string partner,
                                    string DOB, string postcode, string town, bool forceRefresh)
        {
            //GetClientRowsCount is directly called by the objectdatasource which expects a method 
            //with the same parameters as the method used to retrieve the data i.e SearchClient

            //Client row count is saved when we retrieve the clients based on the search criteria
            return _clientRowCount;
        }

        /// <summary>
        /// Searches for clients that match the search criteria.
        /// </summary>
        public ClientSearchItem[] SearchClient(int startRow, int pageSize, string sortBy,  string name, string NINo, string partner,
                                    string DOB, string postcode, string town, bool forceRefresh)
        {
            ClientServiceClient clientService = null;
            ClientSearchItem[] clients = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    if (name == null || name.Trim() == string.Empty)
                        if (NINo == null || NINo == string.Empty)
                            if (partner == null || partner.Trim() == string.Empty)
                                if (DOB == null || DOB.Trim() == string.Empty)
                                    if (postcode == null || postcode.Trim() == string.Empty)
                                        if (town == null || town.Trim() == string.Empty)
                                                throw new Exception("Please enter search criteria");


                    if (!IsReset)
                    {
                        // LSC - Insert Wildcards - 28/08/2010
                        if (!string.IsNullOrWhiteSpace(name) && !name.Contains('%'))
                        {
                            name = "%" + name.Trim() + "%";
                        }

                        Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                        CollectionRequest collectionRequest = new CollectionRequest();
                        collectionRequest.ForceRefresh = forceRefresh;
                        collectionRequest.StartRow = startRow;
                        collectionRequest.RowCount = pageSize;


                        ClientSearchCriteria criteria = new ClientSearchCriteria();
                        criteria.Name = name != null ? name.Replace("'", "''")  : name;
                        criteria.NINumber = NINo != null ? NINo.Replace("'", "''")  : NINo; 
                        criteria.OrganisationId = DataConstants.DummyGuid;
                        criteria.MemberId = DataConstants.DummyGuid;
                        criteria.OrderBy = sortBy;
                        Guid partnerId = DataConstants.DummyGuid;

                        if (partner != null && partner != string.Empty)
                        {
                            partnerId = new Guid(partner);
                        }
                        criteria.Partner = partnerId;

                        if (DOB != null && DOB.Length > 0)
                        {
                            criteria.DateOfBirthFrom = Convert.ToDateTime(DOB.Trim());
                            criteria.DateOfBirthTo = Convert.ToDateTime(DOB.Trim());
                        }
                        else
                        {
                            criteria.DateOfBirthFrom = null;
                            criteria.DateOfBirthTo = null;
                        }

                        criteria.PostCode = postcode != null ? postcode.Replace("'", "''")  : postcode;
                        criteria.Town = town != null ? town.Replace("'", "''")  : town;

                        clientService = new ClientServiceClient();
                        ClientSearchReturnValue returnValue = clientService.ClientSearch(_logonId,
                                                    collectionRequest, criteria);

                        if (returnValue.Success)
                        {
                            _clientRowCount = returnValue.Clients.TotalRowCount;
                            clients = returnValue.Clients.Rows;
                        }
                        else
                        {
                            if (returnValue.Message == "SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.")
                                throw new Exception("Date is invalid");
                            else
                                throw new Exception(returnValue.Message);

                            
                        }
                    }
                }
                return clients;
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


        protected void OnSorted(Object sender, EventArgs e)
        {
            //_grdSearchClientList;
            _odsSearchClient.Select();
        }

        protected void _odsSearchClient_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding client grid
            if (e.Exception != null)
            {
                _grdSearchClientList.Visible = false;
                _lblMessage.CssClass = "errorMessage";

                if (e.Exception.InnerException.Message.Contains("System.ServiceModel.Channels.ServiceChannel") || e.Exception.InnerException.Message.ToLower().Contains("could not connect to")
                    || e.Exception.InnerException.Message.ToLower().Contains("could not connect to"))
                    _lblMessage.Text = DataConstants.WSEndPointErrorMessage;
                else
                     _lblMessage.Text = e.Exception.InnerException.Message;

                e.ExceptionHandled = true;
            }

            //Set force refresh to false so that data is retrieved from cache during paging
            _hdnRefresh.Value = "false";
        }
        #endregion

        #region Client Search Gridview Events

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdSearchClientList_RowCommand(object sender, GridViewCommandEventArgs e)
        { 
            _grdMattersForClient.Visible = true;

            if (e.CommandName == "select")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int rowId = row.RowIndex;
                if (row.Cells[0].FindControl("_lnkCliRef") != null)
                {
                    Guid memberId = new Guid(_grdSearchClientList.DataKeys[rowId].Values["MemberId"].ToString());
                    Guid organisationId = new Guid(_grdSearchClientList.DataKeys[rowId].Values["OrganisationId"].ToString());
                    _hdnClientName.Value = ((Label)_grdSearchClientList.Rows[rowId].Cells[0].FindControl("_lblPersonName")).Text;

                    if (_setSession)
                    {
                        HttpContext.Current.Session[SessionName.ClientRef] = ((LinkButton)row.Cells[0].FindControl("_lnkCliRef")).Text;
                        HttpContext.Current.Session[SessionName.ClientName] = ((Label)row.Cells[0].FindControl("_lblPersonName")).Text;

                        IRIS.Law.WebApp.App_Code.AppFunctions.SetClientMatterDetailsInSession(memberId, organisationId, ((Label)row.Cells[0].FindControl("_lblPersonName")).Text, null, null);
                        ((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();
                    }
                    else
                    {
                        _hdnClientRef.Value = ((LinkButton)row.Cells[0].FindControl("_lnkCliRef")).Text;
                        _hdnClientName.Value = ((Label)row.Cells[0].FindControl("_lblPersonName")).Text;
                    }

                    if (DisplayPopup == true)
                    {
                        if (!DisplayClientName)
                        {
                            _txtClientReference.Text = ((LinkButton)row.Cells[0].FindControl("_lnkCliRef")).Text;
                        }
                        else
                        {
                            _txtClientReference.Text = ((Label)row.Cells[0].FindControl("_lblPersonName")).Text;
                        }

                        if ((new Guid(_grdSearchClientList.DataKeys[rowId].Values["MemberId"].ToString()) != DataConstants.DummyGuid))
                        {
                            _hdnClientId.Value = memberId.ToString();
                            _hdnIsMember.Value = "true";
                        }
                        else
                        {
                            _hdnClientId.Value = organisationId.ToString();
                            _hdnIsMember.Value = "false";
                        }

                        OnClientReferenceChanged(EventArgs.Empty);
                        _modalpopupClientSearch.Hide();
                    }
                    else
                    {
                        if (_setSession)
                        {
                            HttpContext.Current.Session[SessionName.MemberId] = memberId;
                            HttpContext.Current.Session[SessionName.OrganisationId] = organisationId;
                        }
                        else
                        {
                            _memberId = memberId;
                            _organisationId = organisationId;
                        }
                        Response.Redirect("~/Pages/Client/EditClient.aspx", true);
                    }
                }
            }
            else if (e.CommandName == "GetMatters")
            {
                int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                _hdnRefreshClientMatters.Value = "true";
                _hdnMemberId.Value = _grdSearchClientList.DataKeys[rowId].Values["MemberId"].ToString();
                _hdnOrganisationId.Value = _grdSearchClientList.DataKeys[rowId].Values["OrganisationId"].ToString();
                _hdnClientRef.Value = ((LinkButton)_grdSearchClientList.Rows[rowId].Cells[0].FindControl("_lnkCliRef")).Text;
                _hdnClientName.Value = ((Label)_grdSearchClientList.Rows[rowId].Cells[0].FindControl("_lblPersonName")).Text;

                if ((new Guid(_grdSearchClientList.DataKeys[rowId].Values["MemberId"].ToString()) != DataConstants.DummyGuid))
                {
                    _hdnClientId.Value = _hdnMemberId.Value;
                    _hdnIsMember.Value = "true";
                }
                else
                {
                    _hdnClientId.Value = _hdnOrganisationId.Value;
                    _hdnIsMember.Value = "false";
                }

                _grdMattersForClient.DataSourceID = "_odsClientMatters";
                _modalpopupClientSearch.Show();
            }
            else
            {
                _modalpopupClientSearch.Show();
            }
        }
        #endregion

        protected void _grdSearchClientList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                            img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchClientList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                            // checking if the header link is the user's choice
                            if (_grdSearchClientList.SortExpression == lnk.CommandArgument)
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
                Label lblPersonName = (Label)e.Row.FindControl("_lblPersonName");
                LinkButton lnkReference = (LinkButton)e.Row.FindControl("_lnkCliRef");
                ClientSearchItem client = (ClientSearchItem)e.Row.DataItem;

                if (client.DateOfDeath != DataConstants.BlankDate && client.DateOfDeath != null)
                {
                    lblPersonName.Text = client.Name + " (Deceased)";
                }
                else
                {
                    lblPersonName.Text = client.Name;
                }

                if (client.DateOfBirth == DataConstants.BlankDate)
                {
                    Label lblDOB = (Label)e.Row.FindControl("_lblDOB");
                    lblDOB.Text = string.Empty;
                }

                if (client.IsArchived)
                {
                    lnkReference.ForeColor = Color.Red;
                }
            }
        }

        #endregion Client Search Gridview Events

        #region Client Matters Tab

        /// <summary>
        /// Displays the client matters.
        /// </summary>
        public MatterSearchItem[] GetClientMatters(int startRow, int pageSize,
                                                   string memberId, string organisationId, bool forceRefresh)
        {
            MatterServiceClient matterService = null;
            MatterSearchItem[] clientMatters = null;
            try
            {
                Guid logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                Guid orgId = new Guid(organisationId);
                Guid memId = new Guid(memberId);

                matterService = new MatterServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;
                collectionRequest.ForceRefresh = forceRefresh;
                MatterSearchCriteria searchCriteria = new MatterSearchCriteria();

                if (memId != DataConstants.DummyGuid)
                {
                    searchCriteria.MemberId = memId;
                }
                else
                {
                    searchCriteria.OrganisationId = orgId;
                }

                MatterSearchReturnValue returnValue = matterService.MatterSearch(logonId,
                                                collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _clientMatterRowCount = returnValue.Matters.TotalRowCount;
                    clientMatters = returnValue.Matters.Rows;
                }

                return clientMatters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (matterService != null)
                {
                    if (matterService.State != System.ServiceModel.CommunicationState.Faulted)   
                        matterService.Close();
                }
            }
        }

        public int GetClientMattersRowsCount(bool forceRefresh, string memberId, string organisationId)
        {
            return _clientMatterRowCount;
        }

        protected void _grdMattersForClient_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                int rowId = ((GridViewRow)((LinkButton)(e.CommandSource)).NamingContainer).RowIndex;
                _hdnProjectId.Value = _grdMattersForClient.DataKeys[rowId].Values["Id"].ToString();
                _hdnProjectRef.Value = ((LinkButton)_grdMattersForClient.Rows[rowId].Cells[0].FindControl("_lnkMatterReference")).Text;
                
                Guid? projectId = new Guid(_hdnProjectId.Value);
                string matterDesc = _hdnProjectRef.Value;

                Guid memberId = new Guid(  _hdnMemberId.Value);
                Guid orgId = new Guid(_hdnOrganisationId.Value);
                string clientRef =  _hdnClientRef.Value;
                string clientName = _hdnClientName.Value;

                IRIS.Law.WebApp.App_Code.AppFunctions.
                       SetClientMatterDetailsInSession(memberId, orgId, clientName, projectId, matterDesc);
                ((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();


                _modalpopupClientSearch.Hide();
                OnMatterSelected(EventArgs.Empty);

            }
        }

        protected void _grdMattersForClient_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                MatterSearchItem searchItem = (MatterSearchItem)e.Row.DataItem;
                //Truncate large descriptions
                if (searchItem.Description.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblDescription");
                    lbldescription.Text = searchItem.Description.Substring(0, 20) + "...";
                }

                //Truncate large descriptions
                if (searchItem.KeyDescription.Length > 20)
                {
                    Label lbldescription = (Label)e.Row.FindControl("_lblKeyDescription");
                    lbldescription.Text = searchItem.KeyDescription.Substring(0, 20) + "...";
                }

                //Hide blank dates
                if (searchItem.OpenedDate == DataConstants.BlankDate)
                {
                    Label lblOpenedDate = (Label)e.Row.FindControl("_lblOpened");
                    lblOpenedDate.Text = string.Empty;
                }

                //Hide blank dates
                if (searchItem.ClosedDate == DataConstants.BlankDate)
                {
                    Label lblClosedDate = (Label)e.Row.FindControl("_lblClosed");
                    lblClosedDate.Text = string.Empty;
                }

                // Add '-' after the matter ref
                LinkButton matterLink = ((LinkButton)e.Row.FindControl("_lnkMatterReference"));

                if (matterLink.Text.Length > 6)
                {
                    matterLink.Text = matterLink.Text.Insert(6, "-");
                }

                if (!_displayMatterLinkable)
                {
                    Label _lblMatterReference = (Label)e.Row.FindControl("_lblMatterReference");
                    matterLink.Visible = false;
                    _lblMatterReference.Visible = true;

                    // Add '-' after the matter ref
                    if (_lblMatterReference.Text.Length > 6)
                    {
                        _lblMatterReference.Text = _lblMatterReference.Text.Insert(6, "-");
                    }
                }
            }
        }

        protected void _odsClientMatters_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while retrieving matters for the client
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
            _hdnRefreshClientMatters.Value = "false";
        }

        #endregion

        #region Close Search
 
        protected void _LnkBtnClose_Close(object sender, EventArgs e) 
        {
            _modalpopupClientSearch.Hide();
        }

        #endregion
        protected void Page_Init(object sender, EventArgs e)
        {
            if (_asyncPostbackTriggers != null && _asyncPostbackTriggers.Length > 0)
            {
                string[] controls = _asyncPostbackTriggers.Split(',');
                foreach (string controlId in controls)
                {
                    AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                    trigger.ControlID = controlId;
                    _updPnlClientSearch.Triggers.Add(trigger);
                    _updPnlMattersForClient.Triggers.Add(trigger);
                }
            }
        }
    }
}

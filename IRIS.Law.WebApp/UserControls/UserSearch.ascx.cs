using System;
using System.Web.UI.WebControls;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Drawing;
using IRIS.Law.WebApp.MasterPages;
using IRIS.Law.WebApp.App_Code;

using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Utilities;
using IRIS.Law.WebServiceInterfaces.Logon;





namespace IRIS.Law.WebApp.UserControls
{
    public partial class UserSearch : System.Web.UI.UserControl
    {
        //keeps track of the number of rows returned by the search. required to create the grid pager
        private int _userRowCount;
        private int _userMatterRowCount;
        private LogonReturnValue _logonSettings;
        private bool _hideLoading;
        private bool _width100pc;
        private bool IsReset = false;

        public const string NoUserSelected = "";

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
            get { return _txtName.Text; }
            set { _txtName.Text = value; }
        }
        #endregion


        public string Test
        {
            get { return _txtName.Text; }
        }

        #region DisableClient

        /// <summary>
        /// Disables the client in cheque request edit mode.
        /// </summary>
        public bool DisableClient
        {
            set
            {
                this._txtSelectedName.Enabled = !value;
             
                _imgTextControl.Visible = value;
            }
        }

        #endregion

        public bool DisplayUserName
        {
            get;
            set;
        }

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
                _txtName.Visible = value;
                _imgTextControl.Visible = value;
                if (!value)
                {
                    _grdviewDivHeight.Style["height"] = "100%";
                    _modalpopupUserSearch.Hide();
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
        /// Gets or sets a value indicating whether the user field is mandatory
        /// </summary>
        /// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
        public bool EnableValidation { get; set; }
        #endregion

        //#region UserId

        //public Guid UserId
        //{
        //    get
        //    {
        //        if (_hdnUserId != null && _hdnUserId.Value != string.Empty)
        //        {
        //            return new Guid(_hdnUserId.Value);
        //        }
        //        return DataConstants.DummyGuid;
        //    }
        //    set
        //    {
        //        if (_hdnUserId != null)
        //        {
        //            _hdnUserId.Value = value.ToString();
        //        }
        //    }
        //}
        //#endregion

        #region SetSession
        /// <summary>
        /// Gets or sets a value indicating whether the Session for User Matter is to be set or not
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

        #region UserId
        public string UserId
        {
            get
            {
                if (_hdnUserId != null)
                {
                    return _hdnUserId.Value;
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



        #region DisplayUserNameTextbox

        private bool _displayUserNameTextbox = true;

        /// <summary>
        /// Gets or sets a value indicating whether to display user name textbox next to the search button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the user name textbox is displayed; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayUserNameTextbox
        {
            get
            {
                return _displayUserNameTextbox;
            }
            set
            {
                _displayUserNameTextbox = value;
            }
        }

        #endregion DisplayUserNameTextbox





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

   

        #region Validation Group

        public string ValidationGroup
        {
            get { return _rfvUserReference.ValidationGroup; }
            set { _rfvUserReference.ValidationGroup = value; }
        }

        #endregion

        #region public Events

        public delegate void UserReferenceChangedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the user reference changes.
        /// </summary>
        public event UserReferenceChangedEventHandler UserReferenceChanged;
        protected virtual void OnUserReferenceChanged(EventArgs e)
        {
            if (UserReferenceChanged != null)
            {
                UserReferenceChanged(this, e);
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
            _grdSearchUserList.PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);

            if (_logonSettings.UserType == (int)DataConstants.UserType.Client)
                _imgTextControl.Visible = false;

            if (_hideLoading)
            {
                _pnlUpdateProgressUserSearch.Visible = false;

            }

            if (_width100pc)
            {
                _tbSearch.Width = "100%";
            }



            if (!IsPostBack)
            {
                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the user side changes to the value if we add this attribute in the markup

                this._txtSelectedName.Attributes.Add("readonly", "readonly");

                if (EnableValidation == false)
                {
                    _pnlValidation.Visible = false;
                }

                //Disply the selected user's name or reference
                //if (DisplayUserName)
                //{
                //    _txtUserReference.CssClass = "textBox";
                //}
                //else
                //{
                //    _txtUserReference.CssClass = "textBoxSmall";
                //}

                if (DisplayPopup == false)
                {
                    _divCancelButton.Visible = false;
                    _trCloseLink.Style["display"] = "none";
                }
                else
                {
                    _modalpopupUserSearch.CancelControlID = _btnCancel.ClientID;
                    _UserSearchContainer.Style.Add("display", "none");
                    //_pnlUserSearch.Style.Add("display", "none");
                    _trCloseLink.Style["display"] = "";
                    //_tbSearch.Style.Add("width", "620px");
                }
                
                this._txtSelectedName.Visible = _displayUserNameTextbox;

            
            }
            _lblMessage.Text = string.Empty;
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
                    _pnlUserSearch.Style.Add("display", "");
                }
                _grdSearchUserList.Visible = true;
                _grdSearchUserList.PageIndex = 0;
                _grdSearchUserList.DataSourceID = _odsSearchUser.ID;

             

                _modalpopupUserSearch.Show();
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

            this._txtName.Text = "";
         
            _grdSearchUserList.Visible = false;
       
            _lblMessage.Text = "";


            _modalpopupUserSearch.Show();
        }
        #endregion

        #region BindGridView

        /// <summary>
        /// Gets the user rows count used to create the pager for the grid.
        /// </summary>
        public int GetUserRowsCount(string name, bool forceRefresh)
        {
            //GetUserRowsCount is directly called by the objectdatasource which expects a method 
            //with the same parameters as the method used to retrieve the data i.e SearchUser

            //User row count is saved when we retrieve the users based on the search criteria
            return _userRowCount;
        }

        /// <summary>
        /// Searches for users that match the search criteria.
        /// </summary>
        public UserSearchItem[] SearchUser(int startRow, int pageSize, string sortBy, string name,  bool forceRefresh)
        {
            UtilitiesServiceClient userService = null;
            UserSearchItem[] users = null;
            try
            {
                if (HttpContext.Current.Session[SessionName.LogonSettings] != null)
                {
                    if (name == null || name.Trim() == string.Empty)
                        throw new Exception("Please enter search criteria");


                    if (!IsReset)
                    {
                        // LSC - Insert Wildcards - 28/08/2010
                        if (!name.Contains('%')) { name = "%" + name.Trim() + "%"; }

                        Guid _logonId = ((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId;
                        CollectionRequest collectionRequest = new CollectionRequest();
                        collectionRequest.ForceRefresh = forceRefresh;
                        collectionRequest.StartRow = startRow;
                        collectionRequest.RowCount = pageSize;
                        

                        UserSearchCriteria criteria = new UserSearchCriteria();
                        criteria.UserType = 1;
                        criteria.Name = name;

                        userService = new UtilitiesServiceClient();
                        UserSearchReturnValue returnValue = userService.UserSearch(_logonId, collectionRequest, criteria);
 
                        //int idx =0;
                        //while (idx < returnValue.Users.Rows.Count())
                        //{

                        //    idx++;
                        //}

                        if (returnValue.Success)
                        {
                            _userRowCount = returnValue.Users.TotalRowCount;
                            users = returnValue.Users.Rows;

                            //users[0].UserId;
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
                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (userService != null)
                {
                    if (userService.State != System.ServiceModel.CommunicationState.Faulted)
                        userService.Close();
                }
            }
        }


        protected void OnSorted(Object sender, EventArgs e)
        {
            //_grdSearchUserList;
            _odsSearchUser.Select();
        }

        protected void _odsSearchUser_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            //Handle exceptions that may occur while binding user grid
            if (e.Exception != null)
            {
                _grdSearchUserList.Visible = false;
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

        #region User Search Gridview Events

        #region GridView RowCommand
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _grdSearchUserList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           
            

            if (e.CommandName == "select")
            {
                GridViewRow row = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                int rowId = row.RowIndex;
                if (row.Cells[0].FindControl("_lnkUser") != null)
                {
                 
               //     _hdnUserId.Value = ((Label)_grdSearchUserList.Rows[rowId].Cells[0].FindControl("_lblPersonName")).Text;

                    
                    if (_setSession)
                    {
                        //HttpContext.Current.Session[SessionName.us] = ((LinkButton)row.Cells[0].FindControl("_lnkCliRef")).Text;
                        //HttpContext.Current.Session[SessionName.UserName] = ((Label)row.Cells[0].FindControl("_lblPersonName")).Text;

                        HttpContext.Current.Session[SessionName.UserId] = ((LinkButton)row.Cells[0].FindControl("_lnkUser")).Text;
                    }
                    else
                    {
                        _hdnUserId.Value = _grdSearchUserList.DataKeys[rowId].Values["UserId"].ToString();

                        //_hdnUserRef.Value = ((LinkButton)row.Cells[0].FindControl("_lnkCliRef")).Text;
                        //_hdnUserName.Value = ((Label)row.Cells[0].FindControl("_lblPersonName")).Text;
                    }

                    if (DisplayPopup == true)
                    {
                   
                        SetSelectedUser(_grdSearchUserList.DataKeys[rowId].Values["UserId"].ToString(),((LinkButton)row.Cells[0].FindControl("_lnkUser")).Text);

                        OnUserReferenceChanged(EventArgs.Empty);
                        _modalpopupUserSearch.Hide();
                    }
                  
                }
            }
         
          
        }

        public void SetSelectedUser(string userId, string user)
        {
            _txtSelectedName.Text = user;
            _hdnUserId.Value = userId.ToString();
            
        }

        public string SelectedUserId
        {
            get
            {
                return _hdnUserId.Value;
            }
        }

        public string SelectedUserName
        {
            get
            {
                return _txtSelectedName.Text;
            }
        }

        #endregion

        protected void _grdSearchUserList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                            img.ImageUrl = "~/Images/PNGs/sort_az_" + (_grdSearchUserList.SortDirection == SortDirection.Ascending ? "descending" : "ascending") + ".png";
                            // checking if the header link is the user's choice
                            if (_grdSearchUserList.SortExpression == lnk.CommandArgument)
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

                LinkButton _lnkUser = (LinkButton)e.Row.FindControl("_lnkUser");
                UserSearchItem user = (UserSearchItem)e.Row.DataItem;


                _lnkUser.Text = user.Forename + " " + user.Surname + "(" + user.UserName + ")";
               

            
            }
        }

        #endregion User Search Gridview Events



        #region Close Search

        protected void _LnkBtnClose_Close(object sender, EventArgs e)
        {
            _modalpopupUserSearch.Hide();
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
                    _updPnlUserSearch.Triggers.Add(trigger);
               
                }
            }
        }
    }
}

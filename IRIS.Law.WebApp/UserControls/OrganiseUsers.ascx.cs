using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class OrganiseUsers : System.Web.UI.UserControl
    {
        LogonReturnValue _logonSettings;

        #region public Events

        public delegate void UsersAddedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the client reference changes.
        /// </summary>
        public event UsersAddedEventHandler UsersAdded;
        protected virtual void OnUsersAdded(EventArgs e)
        {
            if (UsersAdded != null)
            {
                UsersAdded(this, e);
            }
        }
        #endregion

        #region DisplayPopup
        /// <summary>
        /// If this property is set to "True", Search textbox & Search Image will be visible & the gridview height is fixed
        /// If this is "False", Search textbox & Search Image will not be visible & the gridview height is not fixed
        /// </summary>
        public Boolean DisplayPopup
        {
            get { return _btnAttendees.Visible; }
            set
            {
                _btnAttendees.Visible = value;
                if (!value)
                {
                    _modalpopupAttendees.Hide();
                }
            }
        }
        #endregion

        #region CurrentUsers
        private string _currentUsers;
        public string CurrentUsers
        {
            get
            {
                return _currentUsers;
            }
            set
            {
                _currentUsers = value;
            }
        }
        #endregion

        #region CurrentUsersID
        private string _currentUsersID;
        public string CurrentUsersID
        {
            get
            {
                return _currentUsersID;
            }
            set
            {
                _currentUsersID = value;
            }
        }
        #endregion

        #region ButtonText
        /// <summary>
        /// Property sets the Text of the Button to the supplied value
        /// </summary>
        private string _buttonText;
        public string ButtonText
        {
            get { return _btnAttendees.Text; }
            set
            {
                _btnAttendees.Text = value;
            }
        }
        #endregion

        #region ButtonTooltip
        /// <summary>
        /// Property sets the Tooltip of the Button to the supplied value
        /// </summary>
        private string _buttonTooltip;
        public string ButtonTooltip
        {
            get { return _btnAttendees.ToolTip; }
            set
            {
                _btnAttendees.ToolTip = value;
            }
        }
        #endregion

        #region SingleUserSelect
        /// <summary>
        /// Property controls either 1 single user or many users can be selected
        /// </summary>
        private bool _singleUserSelect = false;
        public bool SingleUserSelect
        {
            get { return _singleUserSelect; }
            set
            {
                _singleUserSelect = value;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[SessionName.LogonSettings] == null)
                {
                    Response.Redirect("~/Login.aspx?SessionExpired=1", true);
                }
                else
                {
                    _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                }

                _lblError.Text = string.Empty;
                if (!IsPostBack)
                {
                    BindUsers();

                    if (DisplayPopup == false)
                    {
                        _divCancel.Visible = false;
                        _trCloseLink.Style["display"] = "none";
                    }
                    else
                    {
                        _modalpopupAttendees.CancelControlID = _btnCancel.ClientID;
                        _pnlOrganiseUsers.Style.Add("display", "none");
                        _trCloseLink.Style["display"] = "";
                    }

                    if (_singleUserSelect)      // can only select a single user
                    {
                        _hiddenSingleOrMultiple.Value = "single";
                        _btnAddAll.Visible = false;
                        _divAddAll.Visible = false;
                        _btnRemoveAll.Visible = false;
                        _divRemoveAll.Visible = false;
                    }
                    else                       // can select multiple users
                    {
                        _hiddenSingleOrMultiple.Value = "multi";
                        _btnAddAll.Visible = true;
                        _divAddAll.Visible = true;
                        _btnRemoveAll.Visible = true;
                        _divRemoveAll.Visible = true;
                    }


                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        #region BindUsers
        private void BindUsers()
        {
            DiaryServiceClient diaryService = null;
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
                    _listAvailableUsers.DataSource = returnValue.DiaryMembers.Rows;
                    _listAvailableUsers.DataTextField = "MemberDisplayName";
                    _listAvailableUsers.DataValueField = "MemberID";
                    _listAvailableUsers.DataBind();
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

        #region Save
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_hiddenCurrentUsers.Value.Length > 0)
                {
                    //for (int i = 0; i < _listCurrentUsers.Items.Count; i++)
                    //{
                    //    _currentUsers += _listCurrentUsers.Items[i].Value + "; ";
                    //}
                    _currentUsers = _hiddenCurrentUsers.Value;
                    _currentUsersID = _hiddenCurrentUsersID.Value;
 
                    OnUsersAdded(EventArgs.Empty);
                    _modalpopupAttendees.Hide();
                }
                else
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = "You must have at least one Member in your list";
                    _modalpopupAttendees.Show();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _modalpopupAttendees.Show();
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _modalpopupAttendees.Show();
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.MasterPages;

namespace IRIS.Law.WebApp.UserControls
{
	public partial class ClientMatterDetails : System.Web.UI.UserControl
	{
		private LogonReturnValue _logonSettings;
        private Dictionary<string, bool> objPerm;
		#region EnableValidation
		/// <summary>
		/// Gets or sets a value indicating whether the matter is mandatory
		/// </summary>
		/// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
		public bool EnableValidation { get; set; }
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
                _clientSearch.SetSession = value;
                _setSession = value;
            }
        }
        #endregion

        #region DisableClientMatter
        public bool DisableClientMatter
        {
            set
            {
                _ddlMatterReference.Enabled = !value;
                _clientSearch.DisableClient = !value;
            }
        }

        #endregion

        #region DisplayReset
        public bool DisplayResetButton
        {
            set
            {
                _divResetCliMatter.Visible = value;
            }
        }
        #endregion

        #region ProjectId
        //private Guid _projectId = DataConstants.DummyGuid;

        private Guid _projectId
        {
            get
            {
                if (ViewState["_projectId"] == null)
                    return DataConstants.DummyGuid; ;

                return new Guid(ViewState["_projectId"].ToString());
            }
            set
            {
                ViewState["_projectId"] = value;
            }
        }


        public Guid ProjectId
        {
            get
            {
                return _projectId;
            }
            set
            {
                _projectId = value;
            }
        }
        #endregion

        #region MemberId
        //private Guid _memberId;

        private Guid _memberId
        {
            get
            {
                if (ViewState["_memberId"] == null)
                    return DataConstants.DummyGuid; ;

                return new Guid(ViewState["_memberId"].ToString());
            }
            set
            {
                ViewState["_memberId"] = value;
            }
        }

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
        //private Guid _organisationId;

        private Guid _organisationId
        {
            get
            {
                if (ViewState["_organisationId"] == null)
                    return DataConstants.DummyGuid; ;

                return new Guid(ViewState["_organisationId"].ToString());
            }
            set
            {
                ViewState["_organisationId"] = value;
            }
        }


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

        #region ClientRef
        private string _clientRef;
        public string ClientRef
        {
            get
            {
                return _clientRef;
            }
            set
            {
                _clientRef = value;
            }
        }
        #endregion

        #region ClientName
        private string _clientName;
        public string ClientName
        {
            get
            {
                return _clientName;
            }
            set
            {
                _clientName = value;
            }
        }
        #endregion

        #region DisplayMatterLinkable
                
        /// <summary>
        /// This property needs to be set as false on pages 
        /// where matters for clients is required to be non-linkable.
        /// </summary>
        public bool DisplayMatterLinkable
        {
            set
            {                
                _clientSearch.DisplayMatterLinkable = value;            
            }
        }

        #endregion

        #region LoadData
        private bool _loadData = true;
        public bool LoadData
        {
            get
            {
                return _loadData;
            }
            set
            {
                _loadData = value;
            }
        }
        #endregion

        #region MatterReferenceClientID
        public string MatterReferenceClientID
        {
            get { return _ddlMatterReference.ClientID; }
        }
        #endregion

        #region ResetButtonClientID
        public string ResetButtonClientID
        {
            get { return _btnResetClientMatter.ClientID; }
        }
        #endregion

		#region Use Client/Matter context

		/// <summary>
		/// Gets or sets a value indicating whether the control should check if client/matter is in context.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if client/matter is in context; otherwise, <c>false</c>.
		/// </value>
		public bool CheckClientMatterContext { get; set; }

		#endregion

		#region Message
		private string _message;
		public string Message
		{
			get
			{
				return _message;
			}
		}
		#endregion

		#region DisplayArchivedImage
		public bool DisplayArchivedImage
		{
			set
			{
				_imgMatterArchieved.Visible = value;
			}
		}
		#endregion

		#region Delegate
		public delegate void MatterChangedEventHandler(object sender, EventArgs e);
		/// <summary>
		/// Occurs when the client reference changes.
		/// </summary>
		public event MatterChangedEventHandler MatterChanged;

		protected virtual void OnMatterChanged(EventArgs e)
		{
			if (MatterChanged != null)
			{
				MatterChanged(this, e);
			}
		}

		#endregion

		#region IsClientMember
		private bool _isClientMember;
		public bool IsClientMember
		{
			get
			{
				return _isClientMember;
			}
			set
			{
				_isClientMember = value;
			}
		}
		#endregion

		#region DisplayModuleLinks
		public bool DisplayModuleLinks
		{
			set
			{
                //_tblModuleLinks.Visible = value;
			}
		}
        #endregion

        #region Validation Group

        public string ValidationGroup
        {
            get { return _rfvMatter.ValidationGroup; }
            set 
            { 
                _rfvMatter.ValidationGroup = value;
                _clientSearch.ValidationGroup = value;
            }
        }

        #endregion

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
			_message = string.Empty;

			//enable/disable the validators
			_rfvMatter.Enabled = EnableValidation;
			_clientSearch.EnableValidation = EnableValidation;

            objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];
            _divResetCliMatter.Visible = objPerm[SessionName.SearchClientVisible];

            if (Request.QueryString["PrintPage"] != null)
            {
                _divResetCliMatter.Visible = false;
                _clientSearch.Visible = false;
            }

			if (!IsPostBack)
			{
                if (_setSession)
                {
                    if (HttpContext.Current.Session[SessionName.ClientRef] != null)
                    {
                        if (Request.QueryString["PrintPage"] != null)
                        {
                            _txtReference.Text = HttpContext.Current.Session[SessionName.ClientRef].ToString();
                            _txtReference.Visible = true;
                            _clientSearch.Visible = false;
                        }
                        else
                        {
                            _clientSearch.SearchText = HttpContext.Current.Session[SessionName.ClientRef].ToString();
                            _linkClientName.Text = Convert.ToString(HttpContext.Current.Session[SessionName.ClientName]);
                        }
                    }

                    if (HttpContext.Current.Session[SessionName.OrganisationId] != null && HttpContext.Current.Session[SessionName.MemberId] != null)
                    {
                        Guid memberId = (Guid)HttpContext.Current.Session[SessionName.MemberId];
                        if (memberId == DataConstants.DummyGuid)
                        {
                            _isClientMember = false;
                        }
                        else
                        {
                            _isClientMember = true;
                        }
                        LoadClientMatterDetails();
                    }
                }
                else
                {
                    if (_loadData)
                    {
                        _clientSearch.SetSession = false;
                        _clientSearch.SearchText = _clientSearch.ClientRef;
                        _linkClientName.Text = _clientSearch.ClientName;

                        if (_memberId == DataConstants.DummyGuid)
                        {
                            _isClientMember = false;
                        }
                        else
                        {
                            _isClientMember = true;
                        }
                        LoadClientMatterDetailsWithoutUsingSession();
                    }
                }
			}
		}


		#region MatterReference Selected Changed event
		/// <summary>
		/// On Change of Matter, the matter details will load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void _ddlMatterReference_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
                if (_setSession)
                {
                    HttpContext.Current.Session[SessionName.ProjectId] = GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 0);
                    _projectId = new Guid(GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 0));

                    _lnkMatter.Text = GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 1);

                    if (HttpContext.Current.Session[SessionName.ProjectId] != null)
                    {
                        IRIS.Law.WebApp.App_Code.AppFunctions.SetClientMatterDetailsInSession((Guid)HttpContext.Current.Session[SessionName.MemberId], (Guid)HttpContext.Current.Session[SessionName.OrganisationId], Convert.ToString(HttpContext.Current.Session[SessionName.ClientName]), (new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString())), _lnkMatter.Text);
                        if (Request.QueryString["PrintPage"] == null)
                        {
                            ((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();
                        }
                    }
                }
                else
                {
                    _projectId = new Guid(GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 0));
                    _lnkMatter.Text = GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 1);
                }

				if (MatterChanged != null)
				{
					OnMatterChanged(e);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion

		#region Client Search Event
		/// <summary>
		/// This will fire after selection of Client from Client Search popup.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void _clientSearch_ClientReferenceChanged(object sender, EventArgs e)
		{
			_isClientMember = _clientSearch.IsMember;

            if (_setSession)
            {
                if (_isClientMember)
                {
                    HttpContext.Current.Session[SessionName.MemberId] = _clientSearch.ClientId;
                    HttpContext.Current.Session[SessionName.OrganisationId] = DataConstants.DummyGuid;
                }
                else
                {
                    HttpContext.Current.Session[SessionName.MemberId] = DataConstants.DummyGuid;
                    HttpContext.Current.Session[SessionName.OrganisationId] = _clientSearch.ClientId;
                }

                HttpContext.Current.Session[SessionName.ProjectId] = null;
                LoadClientMatterDetails();

                if (_message.Trim().Length == 0)
                {
                    //_ddlMatterReference.SelectedIndex = _ddlMatterReference.Items.Count - 1;
                    //Session[SessionName.ProjectId] = GetValueOnIndexFromArray(_ddlMatterReference.SelectedValue, 0);

                    _ddlMatterReference_SelectedIndexChanged(this, e);
                }
            }
            else
            {
                if (_isClientMember)
                {
                    _memberId = _clientSearch.ClientId;
                    _organisationId = DataConstants.DummyGuid;
                }
                else
                {
                    _memberId = DataConstants.DummyGuid;
                    _organisationId = _clientSearch.ClientId;
                }

                _projectId = DataConstants.DummyGuid;
                _clientRef = _clientSearch.ClientRef;
                _clientName = _clientSearch.ClientName;

                LoadClientMatterDetailsWithoutUsingSession();

                if (_message.Trim().Length == 0)
                {
                    _ddlMatterReference_SelectedIndexChanged(this, e);
                }
            }
		}
        #endregion

        #region LoadClientMatterDetailsWithoutUsingSession
        public void LoadClientMatterDetailsWithoutUsingSession()
        {
            _clientSearch.SetSession = false;
            try
            {
                MatterServiceClient matterService = new MatterServiceClient();
                try
                {
                    MatterSearchReturnValue matterReturnValue = new MatterSearchReturnValue();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = true;
                    MatterSearchCriteria criteria = new MatterSearchCriteria();
                    if (_isClientMember)
                    {
                        criteria.MemberId = _memberId ;
                        criteria.OrganisationId = DataConstants.DummyGuid;
                    }
                    else
                    {
                        criteria.MemberId = DataConstants.DummyGuid;
                        criteria.OrganisationId = _organisationId ;
                    }
                    matterReturnValue = matterService.MatterSearch(((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId,
                                                                    collectionRequest, criteria);

                    if (matterReturnValue.Success)
                    {
                        if (matterReturnValue != null)
                        {
                            _ddlMatterReference.Items.Clear();

                            _clientSearch.SearchText = _clientRef;
                            _linkClientName.Text = _clientName;
                            _lnkMatter.Text = string.Empty;

                            if (matterReturnValue.Matters.Rows.Length == 0)
                            {
                                _projectId = DataConstants.DummyGuid;
                                _message = "No Matter(s) found for this client.";

                                if (MatterChanged != null)
                                {
                                    OnMatterChanged(System.EventArgs.Empty);
                                }
                                return;
                            }
                                                       

                            for (int i = 0; i < matterReturnValue.Matters.Rows.Length; i++)
                            {
                                ListItem item = new ListItem();
                                item.Text = matterReturnValue.Matters.Rows[i].Reference.Substring(6) + " - " + matterReturnValue.Matters.Rows[i].Description;
                                item.Value = matterReturnValue.Matters.Rows[i].Id.ToString() + "$" + matterReturnValue.Matters.Rows[i].Description;

                                // This will be used if this method is called from some content page, which will set the default matter                              
                                if (_projectId != DataConstants.DummyGuid)
                                {
                                    if (_projectId == matterReturnValue.Matters.Rows[i].Id)
                                    {
                                        _ddlMatterReference.SelectedIndex = -1;
                                        item.Selected = true;
                                        _lnkMatter.Text = matterReturnValue.Matters.Rows[i].Description;
                                    }
                                }

                                _ddlMatterReference.Items.Add(item);
                            }

                            if (_projectId == DataConstants.DummyGuid)
                            {
                                _ddlMatterReference.SelectedIndex = -1;
                                if (_ddlMatterReference.Items.Count > 0)
                                {
                                    _ddlMatterReference.SelectedIndex = _ddlMatterReference.Items.Count - 1;
                                    _lnkMatter.Text = GetValueOnIndexFromArray(_ddlMatterReference.Items[_ddlMatterReference.Items.Count - 1].Value, 1);
                                    _projectId = new Guid(GetValueOnIndexFromArray(_ddlMatterReference.Items[_ddlMatterReference.Items.Count - 1].Value, 0));
                                }
                            }
                        }
                    }
                    else
                    {
                        _message = matterReturnValue.Message;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _message = ex.Message;
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

		#region LoadClientMatterDetails
		public void LoadClientMatterDetails()
		{
			try
			{
				MatterServiceClient matterService = new MatterServiceClient();
				try
				{
					MatterSearchReturnValue matterReturnValue = new MatterSearchReturnValue();
					CollectionRequest collectionRequest = new CollectionRequest();
					collectionRequest.ForceRefresh = true;
					MatterSearchCriteria criteria = new MatterSearchCriteria();
					if (_isClientMember)
					{
                        criteria.MemberId = new Guid(HttpContext.Current.Session[SessionName.MemberId].ToString()); ;
						criteria.OrganisationId = DataConstants.DummyGuid;
					}
					else
					{
						criteria.MemberId = DataConstants.DummyGuid;
                        criteria.OrganisationId = new Guid(HttpContext.Current.Session[SessionName.OrganisationId].ToString()); ;
					}
                    matterReturnValue = matterService.MatterSearch(((LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings]).LogonId,
																	collectionRequest, criteria);

					if (matterReturnValue.Success)
					{
						if (matterReturnValue != null)
						{
							_ddlMatterReference.Items.Clear();

                            _clientSearch.SearchText = Convert.ToString(HttpContext.Current.Session[SessionName.ClientRef]);
                            _linkClientName.Text = Convert.ToString(HttpContext.Current.Session[SessionName.ClientName]);
                            _lnkMatter.Text = string.Empty;

							if (matterReturnValue.Matters.Rows.Length == 0)
							{
                                HttpContext.Current.Session[SessionName.ProjectId] = null;
								_message = "No Matter(s) found for this client.";

								if (MatterChanged != null)
								{
									OnMatterChanged(System.EventArgs.Empty);
								}
								return;
							}

							for (int i = 0; i < matterReturnValue.Matters.Rows.Length; i++)
							{
								ListItem item = new ListItem();
								item.Text = matterReturnValue.Matters.Rows[i].Reference.Substring(6) + " - " + matterReturnValue.Matters.Rows[i].Description;
								item.Value = matterReturnValue.Matters.Rows[i].Id.ToString() + "$" + matterReturnValue.Matters.Rows[i].Description;

								// This will be used if this method is called from some content page, which will set the default matter                              
                                if (HttpContext.Current.Session[SessionName.ProjectId] != null)
								{
                                    if (new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString()) == matterReturnValue.Matters.Rows[i].Id)
									{
										item.Selected = true;
                                        _lnkMatter.Text = matterReturnValue.Matters.Rows[i].Description;
									}
								}

								_ddlMatterReference.Items.Add(item);
							}

                            if (HttpContext.Current.Session[SessionName.ProjectId] == null)
							{
								_ddlMatterReference.SelectedIndex = -1;
								if (_ddlMatterReference.Items.Count > 0)
								{
									_ddlMatterReference.SelectedIndex = _ddlMatterReference.Items.Count - 1;
                                    _lnkMatter.Text = GetValueOnIndexFromArray(_ddlMatterReference.Items[_ddlMatterReference.Items.Count - 1].Value, 1);
                                    HttpContext.Current.Session[SessionName.ProjectId] = GetValueOnIndexFromArray(_ddlMatterReference.Items[_ddlMatterReference.Items.Count - 1].Value, 0);
								}
							}

                            if (HttpContext.Current.Session[SessionName.ProjectId] != null)
							{
                                IRIS.Law.WebApp.App_Code.AppFunctions.SetClientMatterDetailsInSession((Guid)HttpContext.Current.Session[SessionName.MemberId], (Guid)HttpContext.Current.Session[SessionName.OrganisationId], Convert.ToString(HttpContext.Current.Session[SessionName.ClientName]), (new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString())), _lnkMatter.Text);
								((ILBHomePage)Page.Master).DisplayClientMatterDetailsInContext();
							}
						}
					}
					else
					{
						_message = matterReturnValue.Message;
						return;
					}
				}
				catch (Exception ex)
				{
					_message = ex.Message;
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

		#region GetValueOnIndexFromArray
		/// <summary>
		/// If string is Matter Value
		/// index = 0 -> ProjectId
		/// index = 1 -> Matter Description
		/// </summary>
		/// <param name="strValue"></param>
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

        protected void _btnResetClientMatter_Click(object sender, EventArgs e)
        {
            try
            {
                _setSession = false;
                _clientSearch.SearchText = string.Empty;
                _linkClientName.Text = string.Empty;
                _linkClientName.Text = string.Empty;
                _lnkMatter.Text = string.Empty;
                _ddlMatterReference.Items.Clear();
                _projectId = DataConstants.DummyGuid;
                _message = string.Empty;

                if (MatterChanged != null)
                {
                    OnMatterChanged(e);
                }
            }
            catch (Exception ex)
            {
                _message = ex.Message;
            }
        }

        protected void _lnkMatter_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[SessionName.ProjectId]==null)
            {
                HttpContext.Current.Session[SessionName.ProjectId] = _projectId;
            }
            else
            {
                if (new Guid(HttpContext.Current.Session[SessionName.ProjectId].ToString()) != DataConstants.DummyGuid && _projectId != DataConstants.DummyGuid)
                {
                    HttpContext.Current.Session[SessionName.ProjectId] = _projectId;
                }
            }

            Response.Redirect("~/Pages/Matter/EditMatter.aspx");
        }

        protected void _linkClientName_Click(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[SessionName.MemberId] == null)
            {
                HttpContext.Current.Session[SessionName.MemberId] = _memberId;
            }
            else
            {
                if (new Guid(HttpContext.Current.Session[SessionName.MemberId].ToString()) != DataConstants.DummyGuid && _memberId != DataConstants.DummyGuid)
                {
                    HttpContext.Current.Session[SessionName.MemberId] = _memberId;
                }
            }

            if (HttpContext.Current.Session[SessionName.OrganisationId] == null)
            {
                HttpContext.Current.Session[SessionName.OrganisationId] = _organisationId;
            }
            else
            {
                if (new Guid(HttpContext.Current.Session[SessionName.OrganisationId].ToString()) != DataConstants.DummyGuid && _organisationId != DataConstants.DummyGuid)
                {
                    HttpContext.Current.Session[SessionName.OrganisationId] = _organisationId;
                }
            }


            Response.Redirect("~/Pages/Client/EditClient.aspx");
        }
	}
}

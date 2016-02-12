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
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Matter;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class MatterSearchMobile : System.Web.UI.UserControl
    {
        #region Constants

        private const string AllClients = "All Clients";

        #endregion

        #region Private Members

        Guid _logonId;

        #endregion

        #region Public Properties

        private Guid _defaultFeeEarnerId;

        /// <summary>
        /// Sets the default fee earner to be selected.
        /// </summary>
        /// <value>The default fee earner.</value>
        public Guid DefaultFeeEarnerId
        {
            set
            {
                _defaultFeeEarnerId = value;
            }
        }

        public Guid FeeEarnerId
        {
            get
            {
                if (_ddlFeeEarner.SelectedValue != "")
                    return new Guid(_ddlFeeEarner.SelectedValue);
                else
                    return DataConstants.DummyGuid;
            }
        }

        private Guid _projectId;

        /// <summary>
        /// Gets or sets the selected project id.
        /// </summary>
        /// <value>The project id.</value>
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

        public Guid ProjectIdConfirm
        {
            get
            {
                return new Guid(_ddlClientMatters.SelectedValue.Trim());
            }
            set
            {
                _projectId = value;
            }    
        }

        private bool _isMatterMandatory;

        /// <summary>
        /// Gets or sets a value indicating whether matter is mandatory.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if matter is mandatory; otherwise, <c>false</c>.
        /// </value>
        public bool IsMatterMandatory
        {
            get 
            {
                return _isMatterMandatory;
            }
            set
            {
                _isMatterMandatory = value;
            }
        }

        private int _clientRowCount = 0;

        public int ClientRowCount
        {
            get
            {
                return _clientRowCount;
            }
            set
            {
                _clientRowCount = value;
            }
        }

        #endregion

        #region Custom Events

        #region Matter Changed

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

        #region Search Successful

        public delegate void SearchSuccessfulEventHandler(object sender, SuccessEventArgs e);

        /// <summary>
        /// Occurs when search is successful.
        /// </summary>
        public event SearchSuccessfulEventHandler SearchSuccessful;

        protected virtual void OnSearchSuccessful(SuccessEventArgs e)
        {
            if (SearchSuccessful != null)
            {
                SearchSuccessful(this, e);
            }
        }

        #endregion

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

        public void Reset()
        {
            _txtSearch.Text = "All Clients";
            _ddlClientMatters.Items.Clear();
            _ddlClients.Items.Clear();
        }

        #endregion

        #region Constructor

        public MatterSearchMobile()
        {
            _projectId = DataConstants.DummyGuid;
        }

        #endregion

        #region Control Events

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
            }

            if (!IsPostBack)
            {
                try
                {
                    GetFeeEarners();
                    SetDefaultFeeEarner();
                    LoadClientMatterDetails();
                }
                catch (Exception ex)
                {
                    ErrorEventArgs error = new ErrorEventArgs();
                    error.Message = ex.Message;
                    OnError(error);
                }
            }

            EnableDisableValidators();
        }

        protected void _imgBtnMatterSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Guid memberId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 0));
                Guid organisationId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 1));
                GetClientMatters(memberId, organisationId);
                SelectLastMatter();
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }

        protected void _imgBtnSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (_txtSearch.Text.Trim() == string.Empty)
            {
                _txtSearch.Text = AllClients;
            }

            ClientServiceClient clientService = null;
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;

                ClientSearchCriteria criteria = new ClientSearchCriteria();
                if (_txtSearch.Text.Trim() == string.Empty || _txtSearch.Text.Trim() == AllClients)
                {
                    criteria.Name = string.Empty;
                }
                else
                {
                    criteria.Name = _txtSearch.Text.Trim();
                }

                if (_ddlFeeEarner.SelectedIndex > 0)
                {
                    criteria.Partner = new Guid(_ddlFeeEarner.SelectedValue);
                }

                clientService = new ClientServiceClient();
                ClientSearchReturnValue returnValue = clientService.ClientSearch(_logonId,
                                            collectionRequest, criteria);

                _ddlClients.Items.Clear();
                _ddlClientMatters.Items.Clear();

                if (returnValue.Success)
                {
                    if (returnValue.Clients.Rows.Length > 0)
                    {
                        foreach (ClientSearchItem client in returnValue.Clients.Rows)
                        {
                            ListItem item = new ListItem();
                            item.Text = client.ClientReference.Trim() + " - " + client.Name;
                            item.Value = client.MemberId.ToString() + "$" + client.OrganisationId.ToString();
                            _ddlClients.Items.Add(item);
                        }
                    }
                    else
                    {
                        SuccessEventArgs success = new SuccessEventArgs();
                        success.Message = "Search is complete. There are no results to display.";
                        OnSearchSuccessful(success);
                    }

                    if (_ddlClients.Items.Count > 0)
                    {
                        _clientRowCount = _ddlClients.Items.Count;

                        Guid memberId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 0));
                        Guid organisationId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 1));
                        GetClientMatters(memberId, organisationId);
                        SelectLastMatter();
                    }
                    
                }
                else
                {
                    ErrorEventArgs error = new ErrorEventArgs();
                    error.Message = returnValue.Message.Replace("WebClientSearch requires some parameters", "Please select a Fee Earner or use the client search.");
                    OnError(error);
                }
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
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

        protected void _ddlClientMatters_SelectedIndexChanged(object sender, EventArgs e)
        {
            _projectId = new Guid(_ddlClientMatters.SelectedValue.Trim());
            OnMatterChanged(EventArgs.Empty);
        }

        protected void _ddlClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Guid memberId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 0));
                Guid organisationId = new Guid(GetValueOnIndexFromArray(_ddlClients.SelectedValue, 1));
                GetClientMatters(memberId, organisationId);
                SelectLastMatter();
            }
            catch (Exception ex)
            {
                ErrorEventArgs error = new ErrorEventArgs();
                error.Message = ex.Message;
                OnError(error);
            }
        }

        #endregion

        #region Private Methods

        private void EnableDisableValidators()
        {
            if (!_isMatterMandatory)
            {
                //Disable validators
                _rfvClientReference.Enabled = false;
                _rfvMatter.Enabled = false;
                _mfClientReference.Visible = false;
                _mfMatterReference.Visible = false;
            }
        }

        /// <summary>
        /// Selects the last matter created for the client.
        /// </summary>
        private void SelectLastMatter()
        {
            if (_ddlClientMatters.Items.Count > 0)
            {
                _ddlClientMatters.SelectedIndex = _ddlClientMatters.Items.Count - 1;
                _projectId = new Guid(_ddlClientMatters.SelectedValue.Trim());
            }
            else
            {
                _projectId = DataConstants.DummyGuid;
            }
            OnMatterChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets the fee earners.
        /// </summary>
        private void GetFeeEarners()
        {
            EarnerServiceClient earnerService = null;
            try
            {
                earnerService = new EarnerServiceClient();

                PartnerSearchCriteria criteria = new PartnerSearchCriteria();
                CollectionRequest collectionRequest = new CollectionRequest();

                PartnerSearchReturnValue returnValue = new PartnerSearchReturnValue();
                returnValue = earnerService.PartnerSearch(_logonId, collectionRequest, criteria);

                if (returnValue.Success)
                {
                    _ddlFeeEarner.Items.Clear();
                    foreach (PartnerSearchItem partner in returnValue.Partners.Rows)
                    {
                        ListItem item = new ListItem();
                        item.Text = CommonFunctions.MakeFullName(partner.PersonTitle, partner.Name, partner.Surname);
                        item.Value = partner.PartnerId.ToString();
                        _ddlFeeEarner.Items.Add(item);
                    }
                    _ddlFeeEarner.Items.Insert(0, new ListItem("All Partners", ""));
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
                if (earnerService != null)
                {
                    if (earnerService.State != System.ServiceModel.CommunicationState.Faulted)
                        earnerService.Close();
                }
            }
        }

        /// <summary>
        /// Sets the default fee earner.
        /// </summary>
        private void SetDefaultFeeEarner()
        {
            ListItem feeEarner = _ddlFeeEarner.Items.FindByValue(_defaultFeeEarnerId.ToString());
            if (feeEarner != null)
            {
                _ddlFeeEarner.SelectedValue = feeEarner.Value;
            }
        }

        /// <summary>
        /// Gets the matters for the selected client.
        /// </summary>
        private void GetClientMatters(Guid memberId, Guid organisationId)
        {
            _ddlClientMatters.Items.Clear();
            if (_ddlClients.Items.Count > 0)
            {
                MatterServiceClient matterService = null;
                try
                {
                    matterService = new MatterServiceClient();
                    MatterSearchReturnValue matterReturnValue = new MatterSearchReturnValue();
                    CollectionRequest collectionRequest = new CollectionRequest();
                    collectionRequest.ForceRefresh = true;
                    MatterSearchCriteria criteria = new MatterSearchCriteria();

                    criteria.MemberId = memberId;
                    criteria.OrganisationId = organisationId;

                    matterReturnValue = matterService.MatterSearch(_logonId,
                                                                    collectionRequest, criteria);

                    if (matterReturnValue.Success)
                    {
                        if (matterReturnValue.Matters.Rows.Length > 0)
                        {
                            foreach (MatterSearchItem matter in matterReturnValue.Matters.Rows)
                            {
                                ListItem item = new ListItem();
                                item.Text = matter.Reference.Substring(6) + " - " + matter.Description;
                                item.Value = matter.Id.ToString();
                                _ddlClientMatters.Items.Add(item);
                            }
                        }
                        else
                        {
                            SuccessEventArgs success = new SuccessEventArgs();
                            success.Message = "No Matters found for this client.";
                            OnSearchSuccessful(success);
                        }
                    }
                    else
                    {
                        throw new Exception(matterReturnValue.Message);
                    }
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
        }

        /// <summary>
        /// If string is Branch Value
        /// index = 0 -> MemberId
        /// index = 1 -> OrganisationId
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

        /// <summary>
        /// Loads the client matter details.
        /// </summary>
        private void LoadClientMatterDetails()
        {
            if (_projectId != DataConstants.DummyGuid)
            {
                Guid memberId;
                Guid organisationId;

                try
                {
                    MatterServiceClient matterService = new MatterServiceClient();
                    try
                    {
                        MatterReturnValue matterReturnValue = new MatterReturnValue();
                        matterReturnValue = matterService.GetMatter(_logonId, _projectId);

                        if (matterReturnValue.Success)
                        {
                            #region Load Client Details

                            if (matterReturnValue.ClientDetails.IsMember)
                            {
                                memberId = matterReturnValue.ClientDetails.MemberId;
                                organisationId = DataConstants.DummyGuid;
                            }
                            else
                            {
                                memberId = DataConstants.DummyGuid;
                                organisationId = matterReturnValue.ClientDetails.MemberId;
                            }

                            ListItem item = new ListItem();
                            item.Text = matterReturnValue.ClientDetails.FullName;
                            item.Value = memberId.ToString() + "$" + organisationId.ToString();
                            _ddlClients.Items.Add(item);

                            //Get the matters for the client
                            GetClientMatters(memberId, organisationId);

                            //Select the matter
                            if (_ddlClientMatters.Items.Count > 0)
                            {
                                ListItem matter = _ddlClientMatters.Items.FindByValue(_projectId.ToString());
                                if (matter != null)
                                {
                                    _ddlClientMatters.SelectedValue = matter.Value;
                                    OnMatterChanged(EventArgs.Empty);
                                }
                            }
                            else
                            {
                                throw new Exception("Error loading matter");
                            }

                            #endregion
                        }
                        else
                        {
                            throw new Exception(matterReturnValue.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
        }
        #endregion
    }

    #region Event Args

    public class SuccessEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    #endregion
}
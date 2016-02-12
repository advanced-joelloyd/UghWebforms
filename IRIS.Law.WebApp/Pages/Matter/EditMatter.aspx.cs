using System;
using System.Data;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces.BranchDept;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Earner;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces.Matter;
using IRIS.Law.WebServiceInterfaces.Time;
using AjaxControlToolkit;
using NLog;

namespace IRIS.Law.WebApp.Pages.Matter
{
    public partial class EditMatter : BasePage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Properties
        private LogonReturnValue _logonSettings;
        string _disabledTabList = String.Empty;



        #endregion

        #region Page Init
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString["PrintPage"] != null)
            {
                this.MasterPageFile = "~/MasterPages/Print.Master";

            }

        }

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {
                AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                trigger.ControlID = _ddlWorkType.UniqueID;
                _updPanelAdditionalInfo.Triggers.Add(trigger);
                _updPanelPublicFunding.Triggers.Add(trigger);

                AsyncPostBackTrigger triggerFeeEarner = new AsyncPostBackTrigger();
                triggerFeeEarner.ControlID = _ddlFeeEarner.UniqueID;
                _updPanelPublicFunding.Triggers.Add(triggerFeeEarner);

                AsyncPostBackTrigger triggerUFNDate = new AsyncPostBackTrigger();
                triggerUFNDate.ControlID = _ccUFNDate.DateTextBoxUniqueID;
                _updPanelError.Triggers.Add(triggerUFNDate);

                AsyncPostBackTrigger triggerUFNNumber = new AsyncPostBackTrigger();
                triggerUFNNumber.ControlID = _txtUFNNumber.UniqueID;
                _updPanelError.Triggers.Add(triggerUFNNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
                _lblError.Text = string.Empty;

                //Add readonly attribute to controls whose value can be modified through javascript
                //We cannot retrieve the client side changes to the value if we add this attribute in the markup
                _txtMatterClosedDate.Attributes.Add("readonly", "readonly");
                _txtMatterClosedTime.Attributes.Add("readonly", "readonly");

                SetControlAccessibility();

                GenerateReadOnlyTabList();
                if (null != _disabledTabList && _disabledTabList.Length > 0)
                    RegisterClientScript();


                if (_logonSettings.UserType == (int)DataConstants.UserType.Client ||
                       _logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
                {
                    _divAddContactButton.Visible = false;
                    _divAttachContactButton.Visible = false;
                }

                if (!IsPostBack)
                {
                    if (Session[SessionName.ProjectId] == null)
                    {
                        _lblError.Text = "Please provide an projectId";
                        return;
                    }

                    LoadClientMatter();

                    if (Request.UrlReferrer != null)
                    {
                        string pageName = Request.UrlReferrer.AbsolutePath;
                        pageName = AppFunctions.GetPageNameByUrl(pageName);

                        //if we are coming from the search pg then display a back button
                        if (pageName == "SearchMatter.aspx")
                        {
                            _divBackButton.Visible = true;
                        }
                        else if (pageName == "AddAssociationForMatter.aspx")
                        {
                            //If the user was redirected from associations page then select the contact tab
                            int contactIndex = _tcEditMatter.Tabs.IndexOf(_pnlContacts);
                            _tcEditMatter.ActiveTabIndex = contactIndex;
                        }
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                _logger.ErrorException("Endpoint not found", ex);
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not open Edit Matter page", ex);
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
            }
        }
        #endregion

        /// <summary>
        /// Registers the client script that disables tab controls based user type.
        /// </summary>
        private void RegisterClientScript()
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "DisableTabs",
                                "DisableTabs('" + _disabledTabList + "');", true);
        }

        /// <summary>
        /// Sets the visibility according to user type and permissions
        /// </summary>
        private void SetControlAccessibility()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];

            _pnlDetails.Visible = objPerm[SessionName.MatterDetailsVisible];

            _pnlAdditionalInfo.Visible = objPerm[SessionName.AddInfoVisible];

            _pnlContacts.Visible = objPerm[SessionName.MatterContactVisible];

            _pnlPublicFunding.Visible = objPerm[SessionName.MatterPublicFundingVisible];

            _btnSave.Enabled = !(objPerm[SessionName.MatterDetailsReadOnly] && objPerm[SessionName.AddInfoReadOnly] && objPerm[SessionName.MatterContactReadOnly]);
        }

        /// <summary>
        /// Generates a comma separated list of read only tabs
        /// </summary>
        private void GenerateReadOnlyTabList()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];

            if (objPerm[SessionName.MatterDetailsReadOnly] && _pnlDetails.Visible)
                _disabledTabList += _pnlDetails.ClientID + ",";


            if (objPerm[SessionName.AddInfoReadOnly] && _pnlAdditionalInfo.Visible)
                _disabledTabList += _pnlAdditionalInfo.ClientID + ",";

            if (objPerm[SessionName.MatterContactReadOnly] && _pnlContacts.Visible)
                _disabledTabList += _pnlContacts.ClientID + ",";

            if (objPerm[SessionName.MatterPublicFundingReadOnly] && _pnlPublicFunding.Visible)
                _disabledTabList += _pnlPublicFunding.ClientID + ",";

            if (null != _disabledTabList && _disabledTabList.Length > 0)
                _disabledTabList = _disabledTabList.Substring(0, _disabledTabList.Length - 1);

        }

        #region Functions

        #region LoadMatter
        private void LoadClientMatter()
        {
            MatterServiceClient matterService = new MatterServiceClient();
            try
            {
                MatterReturnValue matterReturnValue = new MatterReturnValue();
                matterReturnValue = matterService.GetMatter(_logonSettings.LogonId, new Guid(Session[SessionName.ProjectId].ToString()));

                if (matterReturnValue.Success)
                {
                    if (matterReturnValue != null)
                    {

                        #region Load Client Details


                        if (_logonSettings.UserType != (int)DataConstants.UserType.Client)
                        {

                            // Store ClientID
                            if (matterReturnValue.ClientDetails.IsMember)
                            {
                                Session[SessionName.MemberId] = matterReturnValue.ClientDetails.MemberId;
                                Session[SessionName.OrganisationId] = DataConstants.DummyGuid;
                            }
                            else
                            {
                                Session[SessionName.MemberId] = DataConstants.DummyGuid;
                                // Changed for LCSILB-18429 -- OrganisationID shouldn't be assigned MemberID
                                //Session[SessionName.OrganisationId] = matterReturnValue.ClientDetails.MemberId;                                
                                Session[SessionName.OrganisationId] = matterReturnValue.ClientDetails.OrganisationId ;
                            }

                            Session[SessionName.ClientRef] = matterReturnValue.ClientDetails.Reference;
                            Session[SessionName.ClientName] = matterReturnValue.ClientDetails.FullName;

                        }

                        _cliMatDetails.IsClientMember = matterReturnValue.ClientDetails.IsMember;
                        if (_cliMatDetails.Message != null)
                        {
                            if (_cliMatDetails.Message.Trim().Length > 0)
                            {
                                _lblError.Text = "Loading failed for Client Matter Details.<br>Exception occured is: " + _cliMatDetails.Message;
                                return;
                            }
                        }

                        #endregion

                        // Details Tab
                        #region Load Details Tab
                        _txtDescription.Text = matterReturnValue.Matter.Description;
                        _txtKeyDescription.Text = matterReturnValue.Matter.KeyDescription;

                        BindFeeEarner();
                        if (matterReturnValue.Matter.FeeEarnerMemberId != null)
                        {
                            if (_ddlFeeEarner.Items.FindByValue(matterReturnValue.Matter.FeeEarnerMemberId.ToString()) != null)
                            {
                                _ddlFeeEarner.Items.FindByValue(matterReturnValue.Matter.FeeEarnerMemberId.ToString()).Selected = true;

                                if (_logonSettings.UserType != (int)DataConstants.UserType.Staff)
                                {

                                    _hlFeeEarnerEmail.NavigateUrl = "mailto:" + FindFeeEarnerEmail(matterReturnValue.Matter.FeeEarnerMemberId);
                                    _hlFeeEarnerEmail.Text = FindFeeEarnerEmail(matterReturnValue.Matter.FeeEarnerMemberId);

                                }
                            }
                        }

                        BindSupervisor();
                        if (matterReturnValue.Matter.PartnerMemberId != null)
                        {
                            if (_ddlSupervisor.Items.FindByValue(matterReturnValue.Matter.PartnerMemberId.ToString()) != null)
                            {
                                _ddlSupervisor.Items.FindByValue(matterReturnValue.Matter.PartnerMemberId.ToString()).Selected = true;
                            }
                        }

                        BindClientBank();
                        if (_ddlClientBank.Items.FindByValue(matterReturnValue.Matter.ClientBankId.ToString()) != null)
                        {
                            _ddlClientBank.Items.FindByValue(matterReturnValue.Matter.ClientBankId.ToString()).Selected = true;
                        }

                        BindOfficeBank();
                        if (_ddlOfficeBank.Items.FindByValue(matterReturnValue.Matter.OfficeBankId.ToString()) != null)
                        {
                            _ddlOfficeBank.Items.FindByValue(matterReturnValue.Matter.OfficeBankId.ToString()).Selected = true;
                        }

                        BindDepositBank();
                        if (_ddlDepositBank.Items.FindByValue(matterReturnValue.Matter.DepositBankId.ToString()) != null)
                        {
                            _ddlDepositBank.Items.FindByValue(matterReturnValue.Matter.DepositBankId.ToString()).Selected = true;
                        }

                        BindBranch();
                        if (matterReturnValue.Matter.BranchReference != null)
                        {
                            for (int i = 0; i <= _ddlBranch.Items.Count - 1; i++)
                            {
                                if (GetValueOnIndexFromArray(_ddlBranch.Items[i].Value, 0).Trim() == matterReturnValue.Matter.BranchReference)
                                {
                                    _ddlBranch.Items[i].Selected = true;
                                    BranchOnSelection();
                                }
                            }
                        }

                        switch (matterReturnValue.ClientDetails.TypeId)
                        {
                            case 2:
                                ViewState["IsPrivateClient"] = true;
                                break;
                            case 3:
                                ViewState["IsPrivateClient"] = true;
                                break;
                            default:
                                ViewState["IsPrivateClient"] = matterReturnValue.ClientDetails.IsMember;
                                break;
                        }

                        ViewState["MatterTypeId"] = matterReturnValue.Matter.MatterTypeId;

                        if (_ddlDepartment.Items.FindByValue(matterReturnValue.Matter.DepartmentId.ToString()) != null)
                        {
                            _ddlDepartment.Items.FindByValue(matterReturnValue.Matter.DepartmentId.ToString()).Selected = true;
                            DepartmentOnSelection();
                        }

                        if (matterReturnValue.Matter.WorkTypeId != null)
                        {
                            if (_ddlWorkType.Items.FindByValue(matterReturnValue.Matter.WorkTypeId.ToString()) != null)
                            {
                                _ddlWorkType.Items.FindByValue(matterReturnValue.Matter.WorkTypeId.ToString()).Selected = true;
                                WorkTypeOnSelection();
                            }
                        }

                        if (matterReturnValue.Matter.OpenDate != DataConstants.BlankDate)
                        {
                            _ccOpenDate.DateText = matterReturnValue.Matter.OpenDate.ToString("dd/MM/yyyy");
                        }
                        if (matterReturnValue.Matter.NextReviewDate != DataConstants.BlankDate)
                        {
                            _ccNextReview.DateText = matterReturnValue.Matter.NextReviewDate.ToString("dd/MM/yyyy");
                        }
                        if (matterReturnValue.Matter.CostReviewDate != DataConstants.BlankDate)
                        {
                            _ccCostReview.DateText = matterReturnValue.Matter.CostReviewDate.ToString("dd/MM/yyyy");
                        }
                        if (matterReturnValue.Matter.LastSavedDate != DataConstants.BlankDate)
                        {
                            _ccLastSaved.DateText = matterReturnValue.Matter.LastSavedDate.ToString("dd/MM/yyyy");
                        }
                        if (matterReturnValue.Matter.ClosedDate != DataConstants.BlankDate)
                        {
                            _ccClosedDate.DateText = matterReturnValue.Matter.ClosedDate.ToString("dd/MM/yyyy");
                            _cliMatDetails.DisplayArchivedImage = true;

                            //Check UserSecuritySettings first before enabling.
                            _txtFileAwayRef.Enabled = ((LogonReturnValue)Session[SessionName.LogonSettings]).CanUserEditArchivedMatter;
                            _ccDestroyDate.Enabled = ((LogonReturnValue)Session[SessionName.LogonSettings]).CanUserEditArchivedMatter;
                        }
                        else
                        {
                            _cliMatDetails.DisplayArchivedImage = false;
                            _txtFileAwayRef.Enabled = false;
                            _ccDestroyDate.Enabled = false;
                        }

                        if (matterReturnValue.Matter.DestructDate != DataConstants.BlankDate)
                        {
                            _ccDestroyDate.DateText = matterReturnValue.Matter.DestructDate.ToString("dd/MM/yyyy");
                        }
                        _txtFileAwayRef.Text = matterReturnValue.Matter.FileNo;

                        if (matterReturnValue.Matter.CompletedDate == DataConstants.BlankDate)
                        {
                            _rdoBtnMatterCompletedNo.Checked = true;
                            _rdoBtnMatterCompletedYes.Checked = false;
                            _txtMatterClosedDate.Text = string.Empty;
                            _txtMatterClosedTime.Text = "00:00";
                        }
                        else
                        {
                            _rdoBtnMatterCompletedYes.Checked = true;
                            _rdoBtnMatterCompletedNo.Checked = false;
                            _txtMatterClosedDate.Text = matterReturnValue.Matter.CompletedDate.ToString("dd/MM/yyyy");
                            _txtMatterClosedTime.Text = matterReturnValue.Matter.CompletedDate.ToString("HH:mm");
                        }

                        if (matterReturnValue.Matter.SpanType1Ref != null || matterReturnValue.Matter.SpanType2Ref != null)
                        {
                            if (!string.IsNullOrEmpty(matterReturnValue.Matter.SpanType1Ref) &&
                                !string.IsNullOrEmpty(matterReturnValue.Matter.SpanType2Ref))
                            {
                                _tdSpanType.Style["display"] = "";
                                _lblSpanType1.Text = matterReturnValue.Matter.SpanType1Ref;
                                _lblSpanType2.Text = matterReturnValue.Matter.SpanType2Ref;
                            }
                            else
                            {
                                _tdSpanType.Style["display"] = "none";
                            }
                        }

                        if (ViewState["IsPublicFunded"] == null)
                        {
                            ViewState["IsPublicFunded"] = false;
                        }

                        BindChargeRate(Convert.ToBoolean(ViewState["IsPublicFunded"]));
                        _ddlChargeRate.SelectedIndex = -1;
                        if (matterReturnValue.Matter.ChargeDescriptionId != null)
                        {
                            for (int i = 0; i <= _ddlChargeRate.Items.Count - 1; i++)
                            {
                                if (GetValueOnIndexFromArray(_ddlChargeRate.Items[i].Value, 0).Trim() == matterReturnValue.Matter.ChargeDescriptionId.ToString())
                                {
                                    _ddlChargeRate.Items[i].Selected = true;
                                }
                            }
                        }

                        BindCourtTypes();
                        if (_ddlCourtType.Items.FindByValue(matterReturnValue.Matter.CourtId.ToString()) != null)
                        {
                            _ddlCourtType.Items.FindByValue(matterReturnValue.Matter.CourtId.ToString()).Selected = true;
                        }
                        if (!Convert.ToBoolean(ViewState["IsPublicFunded"]))
                        {
                            _ddlCourtType.Enabled = true;
                        }
                        else
                        {
                            _ddlCourtType.Enabled = false;
                        }
                        #endregion

                        //Additional Info
                        #region Load Additional Info Tab

                        _txtQuote.Text = matterReturnValue.Matter.Quote.ToString("0.00");
                        _txtDisbsLimit.Text = matterReturnValue.Matter.DisbsLimit.ToString("0.00");
                        _txtTimeLimit.Text = matterReturnValue.Matter.TimeLimit.ToString("0.00");
                        _txtWIPLimit.Text = matterReturnValue.Matter.WIPLimit.ToString("0.00");
                        _txtOverallMatterLimit.Text = matterReturnValue.Matter.OverallLimit.ToString("0.00");
                        _txtStatus.Text = matterReturnValue.Matter.Status;
                        _txtIndicators.Text = matterReturnValue.Matter.Indicators;
                        _txtBankReference.Text = matterReturnValue.Matter.BankReference;

                        BindCashCollection();
                        if (_ddlCashCollection.Items.FindByValue(matterReturnValue.Matter.CashCollectionId.ToString()) != null)
                        {
                            _ddlCashCollection.Items.FindByValue(matterReturnValue.Matter.CashCollectionId.ToString()).Selected = true;
                        }
                        _txtCreditLimit.Text = matterReturnValue.Matter.TotalLockup.ToString("0.00");

                        BindBusinessSource();
                        if (_ddlBusinessSource.Items.FindByValue(matterReturnValue.Matter.BusinessSourceId.ToString()) != null)
                        {
                            _ddlBusinessSource.Items.FindByValue(matterReturnValue.Matter.BusinessSourceId.ToString()).Selected = true;
                        }

                        BindPersonDealing();
                        if (_ddlPersonDealing.Items.FindByValue(matterReturnValue.Matter.PersonDealingId.ToString()) != null)
                        {
                            _ddlPersonDealing.Items.FindByValue(matterReturnValue.Matter.PersonDealingId.ToString()).Selected = true;
                        }

                        BindCampaigns();

                        _txtOurReference.Text = matterReturnValue.Matter.OurReference;
                        _txtPreviousReference.Text = matterReturnValue.Matter.PreviousReference;
                        _txtSalutationEnvelope.Text = matterReturnValue.Matter.SalutationEnvelope;
                        _txtSalutationLetter.Text = matterReturnValue.Matter.SalutationLetter;
                        _txtLetterHead.Text = matterReturnValue.Matter.LetterHead;

                        #endregion

                        //Public Funding
                        #region Load Public Funding Tab

                        _chkPublicFunded.Checked = matterReturnValue.Matter.MatterLegalAided;
                        _chkSQM.Checked = matterReturnValue.Matter.Franchised;
                        _chkLondonRate.Checked = matterReturnValue.Matter.isLondonRate;

                        if (matterReturnValue.Matter.UFNDate != DataConstants.BlankDate)
                        {
                            _ccUFNDate.DateText = matterReturnValue.Matter.UFNDate.ToString("dd/MM/yyyy");
                        }
                        _txtUFNNumber.Text = matterReturnValue.Matter.UFN;
                        _txtCertificateNumber.Text = matterReturnValue.Matter.PFCertificateNo;
                        _txtCertificateLimits.Text = matterReturnValue.Matter.PFCertificateNoLimits;

                        #endregion

                        #region LoadContacts

                        if (_logonSettings.UserType == (int)DataConstants.UserType.Staff)
                        {
                            LoadContacts();
                        }

                        #endregion
                    }
                    else
                    {
                        throw new Exception("Load failed.");
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
                if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                    matterService.Close();
            }
        }

        private void LoadContacts()
        {
            MatterServiceClient matterService = new MatterServiceClient();
            try
            {
                MatterAssociationReturnValue returnValue;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                MatterAssociationSearchCriteria criteria = new MatterAssociationSearchCriteria();
                criteria.ApplicationId = 1;//PMS
                criteria.ProjectId = (Guid)Session[SessionName.ProjectId];
                returnValue = matterService.MatterAssociationSearch(_logonSettings.LogonId, collectionRequest,
                                                                    criteria);

                if (returnValue.Success)
                {
                    DisplayContacts(returnValue.Associations.Rows);
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
                if (matterService.State != System.ServiceModel.CommunicationState.Faulted)
                    matterService.Close();
            }
        }

        private void DisplayContacts(MatterAssociationSearchItem[] associations)
        {
            if (associations.Length > 0)
            {
                string role = "";
                bool isNextNewGroup = false;
                int noOfContacts = 0;
                TableRow trMain = null;
                TableCell tcMain = null;
                Panel pnlContact = null;
                Table tblGroup = null;
                Label lblGroupHeader = null;

                for (int i = 0; i <= associations.Length - 1; i++)
                {
                    isNextNewGroup = false;
                    string _role = associations[i].Role.Replace(" ", "");


                    if (role != _role)
                    {
                        noOfContacts = 1;

                        tcMain = null;
                        trMain = null;
                        tcMain = new TableCell();
                        trMain = new TableRow();
                        pnlContact = new Panel();
                        lblGroupHeader = new Label();
                        tblGroup = new Table();

                        //Collapsible Panel
                        CollapsiblePanelExtender collapsePanel = new CollapsiblePanelExtender();


                        collapsePanel.ID = "_cpe_" + _role;
                        collapsePanel.Collapsed = true;
                        collapsePanel.ExpandedImage = "~/Images/GIFs/up.gif";
                        collapsePanel.CollapsedImage = "~/Images/GIFs/down.gif";
                        collapsePanel.TargetControlID = "_pnlContact_" + _role;
                        collapsePanel.ExpandControlID = "_pnlHeader_" + _role;
                        collapsePanel.CollapseControlID = "_pnlHeader_" + _role;
                        collapsePanel.ImageControlID = "_img_" + _role;
                        tcMain.Controls.Add(collapsePanel);

                        //Panel Header having Arrow Image and Label
                        Panel pnlHeader = new Panel();
                        pnlHeader.Style["Width"] = "99.9%";
                        pnlHeader.ID = "_pnlHeader_" + _role;
                        Image imgSelectType = new Image();
                        imgSelectType.ID = "_img_" + _role;

                        lblGroupHeader.Style["Width"] = "80%";
                        lblGroupHeader.CssClass = "collapseLabel";
                        lblGroupHeader.ID = "_lblHeader" + _role;

                        pnlHeader.Controls.Add(imgSelectType);
                        pnlHeader.Controls.Add(lblGroupHeader);
                        tcMain.Controls.Add(pnlHeader);

                        tblGroup.ID = "_tbl_" + _role;
                        tblGroup.CellSpacing = 0;
                        tblGroup.Style["Width"] = "99%";

                        pnlContact.ID = "_pnlContact_" + _role;

                        //Creating Headers
                        TableHeaderRow tblHeaderRowContact = new TableHeaderRow();
                        tblHeaderRowContact.CssClass = "gridViewHeader";

                        TableHeaderCell tblHeaderCellRole = new TableHeaderCell();
                        tblHeaderCellRole.Text = "Role";
                        tblHeaderCellRole.Style["Width"] = "15%";
                        tblHeaderCellRole.HorizontalAlign = HorizontalAlign.Left;

                        TableHeaderCell tblHeaderCellName = new TableHeaderCell();
                        tblHeaderCellName.Text = "Name";
                        tblHeaderCellName.Style["Width"] = "25%";
                        tblHeaderCellName.HorizontalAlign = HorizontalAlign.Left;

                        TableHeaderCell tblHeaderCellDescription = new TableHeaderCell();
                        tblHeaderCellDescription.Text = "Description";
                        tblHeaderCellDescription.Style["Width"] = "25%";
                        tblHeaderCellDescription.HorizontalAlign = HorizontalAlign.Left;

                        TableHeaderCell tblHeaderCellEmail = new TableHeaderCell();
                        tblHeaderCellEmail.Text = "Email";
                        tblHeaderCellEmail.Style["Width"] = "20%";
                        tblHeaderCellEmail.HorizontalAlign = HorizontalAlign.Left;

                        TableHeaderCell tblHeaderCellTelephone = new TableHeaderCell();
                        tblHeaderCellTelephone.Text = "Telephone";
                        tblHeaderCellTelephone.Style["Width"] = "15%";
                        tblHeaderCellTelephone.HorizontalAlign = HorizontalAlign.Left;

                        tblHeaderRowContact.Cells.Add(tblHeaderCellRole);
                        tblHeaderRowContact.Cells.Add(tblHeaderCellName);
                        tblHeaderRowContact.Cells.Add(tblHeaderCellDescription);
                        tblHeaderRowContact.Cells.Add(tblHeaderCellEmail);
                        tblHeaderRowContact.Cells.Add(tblHeaderCellTelephone);

                        tblGroup.Rows.Add(tblHeaderRowContact);
                    }
                    else
                    {
                        noOfContacts += 1;
                    }

                    // Add rows for conflict check
                    TableRow tblRowContact = new TableRow();
                    tblRowContact.CssClass = "gridViewRow";

                    if ((i % 2) != 0)
                    {
                        tblRowContact.CssClass = "gridViewRowAlternate";
                    }

                    TableCell tblCellRole = new TableCell();
                    tblCellRole.Text = _role;

                    TableCell tblCellName = new TableCell();
                    tblCellName.Text = associations[i].Title + " " + associations[i].Name + " " + associations[i].Surname;

                    TableCell tblCellDescription = new TableCell();
                    tblCellDescription.Text = associations[i].Description;

                    TableCell tblCellEmail = new TableCell();
                    tblCellEmail.Text = associations[i].WorkEmail;

                    TableCell tblCellTelephone = new TableCell();
                    tblCellTelephone.Text = associations[i].WorkTelephone;

                    tblRowContact.Cells.Add(tblCellRole);
                    tblRowContact.Cells.Add(tblCellName);
                    tblRowContact.Cells.Add(tblCellDescription);
                    tblRowContact.Cells.Add(tblCellEmail);
                    tblRowContact.Cells.Add(tblCellTelephone);
                    tblGroup.Rows.Add(tblRowContact);

                    if (i == associations.Length - 1)
                    {
                        isNextNewGroup = true;
                    }
                    else if (_role != associations[i + 1].Role)
                    {
                        isNextNewGroup = true;
                    }

                    if (isNextNewGroup)
                    {
                        pnlContact.Controls.Add(tblGroup);

                        lblGroupHeader.Text = "&nbsp;&nbsp;" + _role + " (" + noOfContacts.ToString();

                        if (noOfContacts == 1)
                        {
                            lblGroupHeader.Text = lblGroupHeader.Text + " Contact";
                        }
                        else
                        {
                            lblGroupHeader.Text = lblGroupHeader.Text + " Contacts";
                        }
                        lblGroupHeader.Text = lblGroupHeader.Text + ") ";

                        tcMain.Controls.Add(pnlContact);
                        trMain.Cells.Add(tcMain);
                        _tblConflictCheckGroup.Rows.Add(trMain);

                        TableRow tblRowSeparation = new TableRow();
                        TableCell tblCellSeparation = new TableCell();
                        tblCellSeparation.CssClass = "TitleSeparation";
                        tblRowSeparation.Cells.Add(tblCellSeparation);
                        _tblConflictCheckGroup.Rows.Add(tblRowSeparation);
                    }

                    role = _role;
                }

                _lblContact.Text = string.Format("Contacts ({0})", associations.Length);
            }
        }

        #endregion

        #region BranchOnSelection
        /// <summary>
        /// Used when the Branch is changed
        /// </summary>
        private void BranchOnSelection()
        {
            BranchDeptServiceClient branchClient = new BranchDeptServiceClient();
            try
            {
                Guid branchOrgId = new Guid(GetValueOnIndexFromArray(_ddlBranch.SelectedValue, 1));
                _chkLondonRate.Checked = Convert.ToBoolean(GetValueOnIndexFromArray(_ddlBranch.SelectedValue, 2));
                if (branchOrgId != null)
                {
                    BindDepartment(branchOrgId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (branchClient.State != System.ServiceModel.CommunicationState.Faulted)
                    branchClient.Close();
            }
        }
        #endregion

        #region DepartmentOnSelection
        /// <summary>
        /// Used when the Department is changed
        /// </summary>
        private void DepartmentOnSelection()
        {
            try
            {
                BindWorkType();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region WorkTypeOnSelection
        /// <summary>
        /// Used when the Work Type is changed
        /// </summary>
        private void WorkTypeOnSelection()
        {
            MatterServiceClient workTypeClient = new MatterServiceClient();
            try
            {
                WorkTypeSearchCriteria workTypeCriteria = new WorkTypeSearchCriteria();
                workTypeCriteria.Id = new Guid(_ddlWorkType.SelectedValue);

                WorkTypeSearchReturnValue workTypeReturnValue = workTypeClient.GetValuesOnWorkTypeSelected(_logonSettings.LogonId, workTypeCriteria);

                if (workTypeReturnValue.Success)
                {
                    _chkPublicFunded.Checked = workTypeReturnValue.IsPublicFunded;
                    _txtQuote.Text = workTypeReturnValue.Quote.ToString("0.00");
                    _txtDisbsLimit.Text = workTypeReturnValue.DisbLimit.ToString("0.00");
                    _txtTimeLimit.Text = workTypeReturnValue.TimeLimit.ToString("0.00");
                    _txtWIPLimit.Text = workTypeReturnValue.WipLimit.ToString("0.00");
                    _txtOverallMatterLimit.Text = workTypeReturnValue.OverallLimit.ToString("0.00");
                    _chkSQM.Checked = workTypeReturnValue.Franchised;

                    ViewState["IsPublicFunded"] = workTypeReturnValue.IsPublicFunded;
                    BindChargeRate(workTypeReturnValue.IsPublicFunded);

                    _ddlChargeRate.SelectedIndex = -1;
                    if (workTypeReturnValue.ChargeRateDescriptionId != null)
                    {
                        for (int i = 0; i < _ddlChargeRate.Items.Count; i++)
                        {
                            if (GetValueOnIndexFromArray(_ddlChargeRate.Items[i].Value, 0).Trim() == workTypeReturnValue.ChargeRateDescriptionId.ToString())
                            {
                                _ddlChargeRate.Items[i].Selected = true;
                            }
                        }
                    }

                    if (Convert.ToBoolean(workTypeReturnValue.IsPublicFunded))
                    {
                        _ddlCourtType.Enabled = false;
                    }
                    else
                    {
                        _ddlCourtType.Enabled = true;
                    }

                    if ((workTypeReturnValue.WorkCategoryUFN == 1) || (workTypeReturnValue.WorkCategoryUFN == 3))
                    {
                        _tdlblUFN.Style["display"] = "";
                        _tdUFNDateNumber.Style["display"] = "";
                    }
                    else
                    {
                        _tdlblUFN.Style["display"] = "none";
                        _tdUFNDateNumber.Style["display"] = "none";
                    }
                }
                else
                {
                    throw new Exception(workTypeReturnValue.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (workTypeClient.State != System.ServiceModel.CommunicationState.Faulted)
                    workTypeClient.Close();
            }
        }
        #endregion

        #region ChargeRateOnSelection
        /// <summary>
        /// Used when the Charge Rate is changed
        /// </summary>
        private void ChargeRateOnSelection()
        {
            try
            {
                _ddlCourtType.SelectedIndex = -1;
                for (int i = 0; i <= _ddlCourtType.Items.Count - 1; i++)
                {
                    if (_ddlCourtType.Items[i].Value.Trim() == GetValueOnIndexFromArray(_ddlChargeRate.SelectedValue, 1))
                    {
                        _ddlCourtType.Items[i].Selected = true;
                    }
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
        /// If strAnyValue is Branch Value
        /// index = 0 -> BranchRef
        /// index = 1 -> OrgId
        /// index = 2 -> IsLondonRate
        /// 
        /// If strAnyValue is Charge Rate
        /// index = 0 -> ChargeDescriptionID
        /// index = 1 -> CourtId
        /// </summary>
        /// <param name="strAnyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueOnIndexFromArray(string strAnyValue, int index)
        {
            try
            {
                string[] arrayBranch = strAnyValue.Split('$');
                return arrayBranch[index].Trim();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetDate
        /// <summary>
        /// Used to check if Date is blank, then set to BlankDate
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        private DateTime GetDate(string strDate)
        {
            try
            {
                if (strDate.Length > 0)
                {
                    return Convert.ToDateTime(strDate);
                }
                else
                {
                    return DataConstants.BlankDate;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region AddDefaultToDropDownList
        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Not Set", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region CheckMandatoryFields
        /// <summary>
        /// If Javascript fails to do Validation, then this will help to validate form fields
        /// </summary>
        /// <returns></returns>
        private string CheckMandatoryFields()
        {
            string strErrorMessage = string.Empty;

            try
            {
                if (_txtDescription.Text.Trim().Length == 0)
                {
                    strErrorMessage = "Description is mandatory<br>";
                }

                if (_ddlFeeEarner.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Fee Earner is mandatory<br>";
                }

                if (_ddlSupervisor.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Supervisor is mandatory<br>";
                }

                if (_ddlClientBank.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Client Bank is mandatory<br>";
                }

                if (_ddlOfficeBank.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Office Bank is mandatory<br>";
                }

                if (_ddlDepositBank.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Deposit Bank is mandatory<br>";
                }

                if (_ddlBranch.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Branch Reference is mandatory<br>";
                }

                if (_ddlDepartment.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Department is mandatory<br>";
                }

                if (_ddlChargeRate.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Charge Rate is mandatory<br>";
                }

                if (_ddlCourtType.SelectedValue.Length == 0)
                {
                    strErrorMessage = "Court Type is mandatory<br>";
                }

                if (_ccOpenDate.DateText.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccOpenDate.DateText);
                    }
                    catch
                    {
                        _lblError.Text = "Invalid Open Date.<br>";
                        strErrorMessage = "Invalid Open Date.<br>";
                    }
                }

                if (_ccDestroyDate.DateText.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccDestroyDate.DateText);
                    }
                    catch
                    {
                        _lblError.Text = "Invalid Destroy Date.<br>";
                        strErrorMessage = "Invalid Destroy Date.<br>";
                    }
                }

                if (_ccUFNDate.DateText.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                    }
                    catch
                    {
                        _lblError.Text = "Invalid UFN Date.<br>";
                        strErrorMessage = "Invalid UFN Date.<br>";
                    }
                }

                if (ViewState["HasChangedUFN"] != null)
                {
                    if (bool.Parse(ViewState["HasChangedUFN"].ToString()))
                    {
                        MatterServiceClient earnerService = new MatterServiceClient();
                        try
                        {
                            if (_txtUFNNumber.Text.Trim().Length > 0 || _ccUFNDate.DateText.Trim().Length > 0)
                            {
                                try
                                {
                                    DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                                }
                                catch
                                {
                                    _lblError.CssClass = "errorMessage";
                                    _lblError.Text = "Invalid UFN Date.";
                                    strErrorMessage = "Invalid UFN Date.";

                                }

                                Guid earnerId = new Guid(_ddlFeeEarner.SelectedValue);
                                DateTime UFNDate = Convert.ToDateTime(_ccUFNDate.DateText);
                                string UFNNumber = _txtUFNNumber.Text.Trim();

                                if (UFNNumber == string.Empty)
                                    UFNNumber = null;

                                CollectionRequest collectionReq = new CollectionRequest();

                                UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId, earnerId, UFNDate, UFNNumber);

                                if (item.Success)
                                {
                                    _txtUFNNumber.Text = item.Number;
                                }
                                else
                                {
                                    _lblError.CssClass = "errorMessage";
                                    strErrorMessage = item.Message;
                                    _lblError.Text = item.Message;
                                }
                            }
                            else
                            {
                                _ccUFNDate.DateText = string.Empty;
                            }
                        }
                        catch (System.ServiceModel.EndpointNotFoundException)
                        {
                            _lblError.Text = DataConstants.WSEndPointErrorMessage;
                            _lblError.CssClass = "errorMessage";
                            strErrorMessage = DataConstants.WSEndPointErrorMessage;
                        }
                        catch (Exception ex)
                        {
                            _lblError.CssClass = "errorMessage";
                            _lblError.Text = ex.Message;
                            strErrorMessage = ex.Message;
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
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strErrorMessage;
        }
        #endregion

        #region ResetControls
        /// <summary>
        /// Form Controls will set to blank
        /// </summary>
        private void ResetControls()
        {
            try
            {
                // Details Tab
                _txtDescription.Text = string.Empty;
                _txtKeyDescription.Text = string.Empty;
                _ddlFeeEarner.Items.Clear();
                _ddlSupervisor.Items.Clear();
                _ddlWorkType.Items.Clear();
                _ddlClientBank.Items.Clear();
                _ddlDepositBank.Items.Clear();
                _ddlOfficeBank.Items.Clear();
                _ddlBranch.Items.Clear();
                _ddlDepartment.Items.Clear();
                _ddlChargeRate.Items.Clear();
                _ddlCourtType.Items.Clear();
                _ccOpenDate.DateText = string.Empty;
                _ccNextReview.DateText = string.Empty;
                _ccCostReview.DateText = string.Empty;
                _ccLastSaved.DateText = string.Empty;
                _ccClosedDate.DateText = string.Empty;
                _ccDestroyDate.DateText = string.Empty;
                _txtFileAwayRef.Text = string.Empty;
                _txtMatterClosedDate.Text = string.Empty;
                _txtMatterClosedTime.Text = string.Empty;
                _rdoBtnMatterCompletedYes.Checked = false;
                _rdoBtnMatterCompletedNo.Checked = true;
                _tdSpanType.Style["display"] = "none";

                //Additional Info Tab
                _txtQuote.Text = string.Empty;
                _txtDisbsLimit.Text = string.Empty;
                _txtTimeLimit.Text = string.Empty;
                _txtWIPLimit.Text = string.Empty;
                _txtOverallMatterLimit.Text = string.Empty;
                _txtStatus.Text = string.Empty;
                _txtIndicators.Text = string.Empty;
                _txtBankReference.Text = string.Empty;
                _ddlCashCollection.Items.Clear();
                _txtCreditLimit.Text = string.Empty;
                _txtOurReference.Text = string.Empty;
                _txtPreviousReference.Text = string.Empty;
                _ddlBusinessSource.Items.Clear();
                _ddlPersonDealing.Items.Clear();
                _ddlSourceCampaign.Items.Clear();
                _txtSalutationEnvelope.Text = string.Empty;
                _txtSalutationLetter.Text = string.Empty;
                _txtLetterHead.Text = string.Empty;

                // Public Fund Tab
                _chkPublicFunded.Checked = false;
                _chkSQM.Checked = false;
                _chkLondonRate.Checked = false;
                _ccUFNDate.DateText = string.Empty;
                _txtUFNNumber.Text = string.Empty;
                _txtCertificateNumber.Text = string.Empty;
                _txtCertificateLimits.Text = string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        #region BindDropDownLists

        #region BindFeeEarner

        private string FindFeeEarnerEmail(Guid PartnerId)
        {
            string _feeEarnerEmail = "";

            EarnerServiceClient partnerClient = new EarnerServiceClient();
            try
            {
                PartnerSearchCriteria partnerCriteria = new PartnerSearchCriteria();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                PartnerSearchReturnValue partnerReturnValue = partnerClient.PartnerSearch(_logonSettings.LogonId, collectionRequest, partnerCriteria);

                if (partnerReturnValue.Success)
                {
                    if (partnerReturnValue.Partners != null)
                    {
                        for (int i = 0; i < partnerReturnValue.Partners.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();

                            if (PartnerId == partnerReturnValue.Partners.Rows[i].PartnerId)
                            {
                                _feeEarnerEmail = partnerReturnValue.Partners.Rows[i].Email;
                            }

                        }
                    }
                }
                else
                {
                    _lblError.Text = partnerReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (partnerClient.State != System.ServiceModel.CommunicationState.Faulted)
                    partnerClient.Close();
            }

            return _feeEarnerEmail;
        }

        private void BindFeeEarner()
        {
            _ddlFeeEarner.Items.Clear();
            EarnerServiceClient partnerClient = new EarnerServiceClient();
            try
            {
                PartnerSearchCriteria partnerCriteria = new PartnerSearchCriteria();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                PartnerSearchReturnValue partnerReturnValue = partnerClient.PartnerSearch(_logonSettings.LogonId, collectionRequest, partnerCriteria);

                if (partnerReturnValue.Success)
                {
                    if (partnerReturnValue.Partners != null)
                    {
                        for (int i = 0; i < partnerReturnValue.Partners.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = CommonFunctions.MakeFullName(partnerReturnValue.Partners.Rows[i].PersonTitle, partnerReturnValue.Partners.Rows[i].Name, partnerReturnValue.Partners.Rows[i].Surname);
                            item.Value = partnerReturnValue.Partners.Rows[i].PartnerId.ToString();
                            _ddlFeeEarner.Items.Add(item);
                        }
                    }
                }
                else
                {
                    _lblError.Text = partnerReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (partnerClient.State != System.ServiceModel.CommunicationState.Faulted)
                    partnerClient.Close();
            }
            AddDefaultToDropDownList(_ddlFeeEarner);
        }
        #endregion

        #region BindSupervisor
        private void BindSupervisor()
        {
            _ddlSupervisor.Items.Clear();
            EarnerServiceClient partnerClient = new EarnerServiceClient();
            try
            {
                PartnerSearchCriteria partnerCriteria = new PartnerSearchCriteria();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                PartnerSearchReturnValue partnerReturnValue = partnerClient.PartnerSearch(_logonSettings.LogonId, collectionRequest, partnerCriteria);

                if (partnerReturnValue.Success)
                {
                    if (partnerReturnValue.Partners != null)
                    {
                        for (int i = 0; i < partnerReturnValue.Partners.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = CommonFunctions.MakeFullName(partnerReturnValue.Partners.Rows[i].PersonTitle, partnerReturnValue.Partners.Rows[i].Name, partnerReturnValue.Partners.Rows[i].Surname);
                            item.Value = partnerReturnValue.Partners.Rows[i].PartnerId.ToString();
                            _ddlSupervisor.Items.Add(item);
                        }
                    }
                }
                else
                {
                    _lblError.Text = partnerReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (partnerClient.State != System.ServiceModel.CommunicationState.Faulted)
                    partnerClient.Close();
            }
            AddDefaultToDropDownList(_ddlSupervisor);
        }
        #endregion

        #region BindDepositBank
        private void BindDepositBank()
        {
            _ddlDepositBank.Items.Clear();
            BankServiceClient bankClient = new BankServiceClient();
            try
            {
                BankSearchCriteria bankCriteria = new BankSearchCriteria();
                bankCriteria.BankSearchTypes = (int)DataConstants.BankSearchTypes.Deposit;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                BankSearchReturnValue bankReturnValue = bankClient.BankSearch(_logonSettings.LogonId, collectionRequest, bankCriteria);

                if (bankReturnValue.Success)
                {
                    if (bankReturnValue.Banks != null)
                    {
                        _ddlDepositBank.DataSource = bankReturnValue.Banks.Rows;
                        _ddlDepositBank.DataTextField = "Description";
                        _ddlDepositBank.DataValueField = "BankId";
                        _ddlDepositBank.DataBind();
                    }
                }
                else
                {
                    _lblError.Text = bankReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bankClient.State != System.ServiceModel.CommunicationState.Faulted)
                    bankClient.Close();
            }
            //AddDefaultToDropDownList(_ddlDepositBank);
        }
        #endregion

        #region BindOfficeBank
        private void BindOfficeBank()
        {
            _ddlOfficeBank.Items.Clear();
            BankServiceClient bankClient = new BankServiceClient();
            try
            {
                BankSearchCriteria bankCriteria = new BankSearchCriteria();
                bankCriteria.BankSearchTypes = (int)DataConstants.BankSearchTypes.Office;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                BankSearchReturnValue bankReturnValue = bankClient.BankSearch(_logonSettings.LogonId, collectionRequest, bankCriteria);

                if (bankReturnValue.Success)
                {
                    if (bankReturnValue.Banks != null)
                    {
                        _ddlOfficeBank.DataSource = bankReturnValue.Banks.Rows;
                        _ddlOfficeBank.DataTextField = "Description";
                        _ddlOfficeBank.DataValueField = "BankId";
                        _ddlOfficeBank.DataBind();
                    }
                }
                else
                {
                    _lblError.Text = bankReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bankClient.State != System.ServiceModel.CommunicationState.Faulted)
                    bankClient.Close();
            }
            AddDefaultToDropDownList(_ddlOfficeBank);
        }
        #endregion

        #region BindClientBank
        private void BindClientBank()
        {
            _ddlClientBank.Items.Clear();
            BankServiceClient bankClient = new BankServiceClient();
            try
            {
                BankSearchCriteria bankCriteria = new BankSearchCriteria();
                bankCriteria.BankSearchTypes = (int)DataConstants.BankSearchTypes.Client;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                BankSearchReturnValue bankReturnValue = bankClient.BankSearch(_logonSettings.LogonId, collectionRequest, bankCriteria);

                if (bankReturnValue.Success)
                {
                    if (bankReturnValue.Banks != null)
                    {
                        _ddlClientBank.DataSource = bankReturnValue.Banks.Rows;
                        _ddlClientBank.DataTextField = "Description";
                        _ddlClientBank.DataValueField = "BankId";
                        _ddlClientBank.DataBind();
                    }
                }
                else
                {
                    _lblError.Text = bankReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bankClient.State != System.ServiceModel.CommunicationState.Faulted)
                    bankClient.Close();
            }
            AddDefaultToDropDownList(_ddlClientBank);
        }
        #endregion

        #region BindBranch
        private void BindBranch()
        {
            _ddlBranch.Items.Clear();
            BranchDeptServiceClient branchClient = new BranchDeptServiceClient();
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                collectionRequest.ForceRefresh = true;

                BranchSearchReturnValue branchReturnValue = branchClient.BranchSearch(_logonSettings.LogonId, collectionRequest);

                if (branchReturnValue.Success)
                {
                    if (branchReturnValue.Branches != null)
                    {
                        for (int i = 0; i < branchReturnValue.Branches.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = branchReturnValue.Branches.Rows[i].Name.ToString();
                            item.Value = branchReturnValue.Branches.Rows[i].Reference.ToString().Trim() + "$" + branchReturnValue.Branches.Rows[i].OrganisationId.ToString().Trim() + "$" + branchReturnValue.Branches.Rows[i].IsLondonRate.ToString().Trim();
                            _ddlBranch.Items.Add(item);
                        }
                    }
                }
                else
                {
                    _lblError.Text = branchReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (branchClient.State != System.ServiceModel.CommunicationState.Faulted)
                    branchClient.Close();
            }
            AddDefaultToDropDownList(_ddlBranch);
        }
        #endregion

        #region BindDepartment
        private void BindDepartment(Guid OrgId)
        {
            _ddlDepartment.Items.Clear();
            BranchDeptServiceClient deptClient = new BranchDeptServiceClient();
            try
            {
                DepartmentSearchReturnValue deptReturnValue = new DepartmentSearchReturnValue();
                DepartmentSearchCriteria deptCriteria = new DepartmentSearchCriteria();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                collectionRequest.ForceRefresh = true;
                deptCriteria.OrganisationId = OrgId;

                deptReturnValue = deptClient.DepartmentSearch(_logonSettings.LogonId, collectionRequest, deptCriteria);

                if (deptReturnValue.Success)
                {
                    if (deptReturnValue != null)
                    {
                        if (deptReturnValue.Departments != null)
                        {
                            _ddlDepartment.DataSource = deptReturnValue.Departments.Rows;
                            _ddlDepartment.DataTextField = "Name";
                            _ddlDepartment.DataValueField = "Id";
                            _ddlDepartment.DataBind();
                        }
                    }
                }
                else
                {
                    _lblError.Text = deptReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (deptClient.State != System.ServiceModel.CommunicationState.Faulted)
                    deptClient.Close();
            }
            AddDefaultToDropDownList(_ddlDepartment);
        }
        #endregion

        #region BindWorkType
        private void BindWorkType()
        {
            _ddlWorkType.Items.Clear();
            MatterServiceClient workTypeClient = new MatterServiceClient();
            try
            {
                _ddlWorkType.Items.Clear();

                WorkTypeSearchCriteria workTypeCriteria = new WorkTypeSearchCriteria();

                if (ViewState["IsPrivateClient"] != null && ViewState["MatterTypeId"] != null)
                {
                    workTypeCriteria.MatterTypeId = Convert.ToInt32(ViewState["MatterTypeId"]);
                    workTypeCriteria.IsPrivateClient = Convert.ToBoolean(ViewState["IsPrivateClient"]);
                }
                workTypeCriteria.OrganisationID = new Guid(GetValueOnIndexFromArray(_ddlBranch.SelectedValue, 1));
                workTypeCriteria.DepartmentId = Convert.ToInt32(_ddlDepartment.SelectedValue);

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                collectionRequest.ForceRefresh = true;

                WorkTypeSearchReturnValue workTypeReturnValue = workTypeClient.WorkTypeSearch(_logonSettings.LogonId, collectionRequest, workTypeCriteria);

                if (workTypeReturnValue.Success)
                {
                    if (workTypeReturnValue.WorkTypes != null)
                    {
                        _ddlWorkType.DataSource = workTypeReturnValue.WorkTypes.Rows;
                        _ddlWorkType.DataTextField = "Description";
                        _ddlWorkType.DataValueField = "Id";
                        _ddlWorkType.DataBind();
                    }
                }
                else
                {
                    _lblError.Text = workTypeReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (workTypeClient.State != System.ServiceModel.CommunicationState.Faulted)
                    workTypeClient.Close();
            }
            AddDefaultToDropDownList(_ddlWorkType);
        }
        #endregion

        #region BindChargeRate
        private void BindChargeRate(bool IsPublicFunded)
        {
            _ddlChargeRate.Items.Clear();
            TimeServiceClient chargeRateClient = new TimeServiceClient();
            try
            {
                ChargeRateSearchCriteria chargeRateCriteria = new ChargeRateSearchCriteria();
                chargeRateCriteria.IsPublicFunded = IsPublicFunded;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                collectionRequest.ForceRefresh = true;

                ChargeRateSearchReturnValue chargeRateReturnValue = chargeRateClient.ChargeRateOnPublicFundingSearch(_logonSettings.LogonId, collectionRequest, chargeRateCriteria);

                if (chargeRateReturnValue.Success)
                {
                    if (chargeRateReturnValue.ChargeRates != null)
                    {
                        for (int i = 0; i < chargeRateReturnValue.ChargeRates.Rows.Length; i++)
                        {
                            // Don't Add "Unknown PF" value to the list
                            if (chargeRateReturnValue.ChargeRates.Rows[i].DescriptionId != new Guid("6e5431b2-cdf3-4360-8cbf-93654b83bd85"))
                            {
                                ListItem item = new ListItem();
                                item.Text = chargeRateReturnValue.ChargeRates.Rows[i].Description.ToString();
                                item.Value = chargeRateReturnValue.ChargeRates.Rows[i].DescriptionId.ToString() + "$" + chargeRateReturnValue.ChargeRates.Rows[i].CourtId.ToString();
                                _ddlChargeRate.Items.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    _lblError.Text = chargeRateReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (chargeRateClient.State != System.ServiceModel.CommunicationState.Faulted)
                    chargeRateClient.Close();
            }
            AddDefaultToDropDownList(_ddlChargeRate);
        }
        #endregion

        #region BindCourtTypes
        private void BindCourtTypes()
        {
            _ddlCourtType.Items.Clear();
            TimeServiceClient courtTypeClient = new TimeServiceClient();
            try
            {
                CourtTypeSearchCriteria courtTypeCriteria = new CourtTypeSearchCriteria();
                courtTypeCriteria.IncludeArchived = false;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;
                collectionRequest.ForceRefresh = true;

                CourtTypeReturnValue courtTypeReturnValue = courtTypeClient.CourtTypeSearch(_logonSettings.LogonId, collectionRequest, courtTypeCriteria);

                if (courtTypeReturnValue.Success)
                {
                    _ddlCourtType.DataSource = courtTypeReturnValue.CourtTypes.Rows;
                    _ddlCourtType.DataTextField = "Description";
                    _ddlCourtType.DataValueField = "Id";
                    _ddlCourtType.DataBind();
                }
                else
                {
                    _lblError.Text = courtTypeReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (courtTypeClient.State != System.ServiceModel.CommunicationState.Faulted)
                    courtTypeClient.Close();
            }
            //AddDefaultToDropDownList(_ddlCourtType);
        }
        #endregion

        #region BindCashCollection
        /// <summary>
        /// Gets the cash collection procedures.
        /// </summary>
        private void BindCashCollection()
        {
            _ddlCashCollection.Items.Clear();
            MatterServiceClient cashCollectionService = null;
            try
            {
                cashCollectionService = new MatterServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                CashCollectionSearchCriteria searchCriteria = new CashCollectionSearchCriteria();
                searchCriteria.IncludeArchived = false;
                CashCollectionSearchReturnValue returnValue = cashCollectionService.CashCollectionSearch(_logonSettings.LogonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _ddlCashCollection.DataSource = returnValue.CashCollection.Rows;
                    _ddlCashCollection.DataTextField = "Description";
                    _ddlCashCollection.DataValueField = "Id";
                    _ddlCashCollection.DataBind();
                }
                else
                {
                    _lblError.Text = returnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cashCollectionService != null)
                {
                    if (cashCollectionService.State != System.ServiceModel.CommunicationState.Faulted)
                        cashCollectionService.Close();
                }
            }
            AddDefaultToDropDownList(_ddlCashCollection);
        }
        #endregion

        #region BindBusinessSource
        /// <summary>
        /// Gets the business sources.
        /// </summary>
        private void BindBusinessSource()
        {
            _ddlBusinessSource.Items.Clear();
            ContactServiceClient businessSourceService = null;
            try
            {
                businessSourceService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                BusinessSourceSearchCriteria searchCriteria = new BusinessSourceSearchCriteria();
                BusinessSourceReturnValue returnValue = businessSourceService.BusinessSourceSearch(_logonSettings.LogonId, collectionRequest, searchCriteria);
                if (returnValue.Success)
                {
                    _ddlBusinessSource.DataSource = returnValue.BusinessSources.Rows;
                    _ddlBusinessSource.DataTextField = "Description";
                    _ddlBusinessSource.DataValueField = "Id";
                    _ddlBusinessSource.DataBind();
                }
                else
                {
                    _lblError.Text = returnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (businessSourceService != null)
                {
                    if (businessSourceService.State != System.ServiceModel.CommunicationState.Faulted)
                        businessSourceService.Close();
                }
            }
            //AddDefaultToDropDownList(_ddlBusinessSource);
        }
        #endregion

        #region BindPersonDealing
        private void BindPersonDealing()
        {
            _ddlPersonDealing.Items.Clear();
            EarnerServiceClient earnerClient = new EarnerServiceClient();
            try
            {
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                PartnerSearchReturnValue partnerReturnValue = earnerClient.PersonDealingSearch(_logonSettings.LogonId, collectionRequest);

                if (partnerReturnValue.Success)
                {
                    if (partnerReturnValue.Partners != null)
                    {
                        for (int i = 0; i < partnerReturnValue.Partners.Rows.Length; i++)
                        {
                            ListItem item = new ListItem();
                            item.Text = CommonFunctions.MakeFullName(partnerReturnValue.Partners.Rows[i].PersonTitle, partnerReturnValue.Partners.Rows[i].Name, partnerReturnValue.Partners.Rows[i].Surname);
                            item.Value = partnerReturnValue.Partners.Rows[i].UId.ToString();
                            _ddlPersonDealing.Items.Add(item);
                        }
                    }
                }
                else
                {
                    _lblError.Text = partnerReturnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (earnerClient.State != System.ServiceModel.CommunicationState.Faulted)
                    earnerClient.Close();
            }
            AddDefaultToDropDownList(_ddlPersonDealing);
        }
        #endregion

        #region BindCampaigns
        /// <summary>
        /// Gets the campaigns.
        /// </summary>
        private void BindCampaigns()
        {
            _ddlSourceCampaign.Items.Clear();
            ContactServiceClient campaignService = null;
            try
            {
                campaignService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                CampaignSearchCriteria searchCriteria = new CampaignSearchCriteria();
                CampaignSearchReturnValue returnValue = campaignService.CampaignSearch(_logonSettings.LogonId, collectionRequest, searchCriteria);

                if (returnValue.Success)
                {
                    _ddlSourceCampaign.DataSource = returnValue.Campaigns.Rows;
                    _ddlSourceCampaign.DataTextField = "Description";
                    _ddlSourceCampaign.DataValueField = "CampaignId";
                    _ddlSourceCampaign.DataBind();
                }
                else
                {
                    _lblError.Text = returnValue.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (campaignService != null)
                {
                    if (campaignService.State != System.ServiceModel.CommunicationState.Faulted)
                        campaignService.Close();
                }
            }
            AddDefaultToDropDownList(_ddlSourceCampaign);
        }
        #endregion

        #endregion

        #region Button Events

        #region Back Button
        protected void _btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchMatter.aspx", true);
        }
        #endregion

        #region Save Button
        /// <summary>
        /// Matter gets saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            MatterServiceClient matterService = new MatterServiceClient();
            try
            {
                if (CheckMandatoryFields().Trim().Length > 0)
                {
                    _lblError.Text = CheckMandatoryFields();
                }
                else
                {

                    IRIS.Law.WebServiceInterfaces.Matter.Matter matter = new IRIS.Law.WebServiceInterfaces.Matter.Matter();
                    matter.Id = new Guid(Session[SessionName.ProjectId].ToString());
                    if (ViewState["MatterTypeId"] != null)
                    {
                        matter.MatterTypeId = Convert.ToInt32(ViewState["MatterTypeId"]);
                    }
                    if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                    {
                        if ((Guid)Session[SessionName.MemberId] != DataConstants.DummyGuid)
                        {
                            matter.ClientId = new Guid(Session[SessionName.MemberId].ToString());
                        }
                        else
                        {
                            matter.ClientId = new Guid(Session[SessionName.OrganisationId].ToString());
                        }
                    }

                    #region Save Matter Details Tab

                    matter.Description = _txtDescription.Text;
                    matter.KeyDescription = _txtKeyDescription.Text;
                    matter.FeeEarnerMemberId = new Guid(_ddlFeeEarner.SelectedValue);
                    matter.PartnerMemberId = new Guid(_ddlSupervisor.SelectedValue);
                    matter.WorkTypeId = new Guid(_ddlWorkType.SelectedValue);
                    matter.ClientBankId = Convert.ToInt32(_ddlClientBank.SelectedValue);
                    matter.OfficeBankId = Convert.ToInt32(_ddlOfficeBank.SelectedValue);
                    matter.DepositBankId = Convert.ToInt32(_ddlDepositBank.SelectedValue);
                    matter.BranchReference = GetValueOnIndexFromArray(_ddlBranch.SelectedValue, 0);
                    matter.BranchId = new Guid(GetValueOnIndexFromArray(_ddlBranch.SelectedValue, 1));
                    matter.DepartmentId = Convert.ToInt32(_ddlDepartment.SelectedValue);
                    matter.ChargeDescriptionId = new Guid(GetValueOnIndexFromArray(_ddlChargeRate.SelectedValue, 0));
                    matter.CourtId = Convert.ToInt32(_ddlCourtType.SelectedValue);
                    matter.OpenDate = GetDate(_ccOpenDate.DateText);
                    matter.NextReviewDate = GetDate(_ccNextReview.DateText);
                    matter.CostReviewDate = GetDate(_ccCostReview.DateText);
                    matter.LastSavedDate = GetDate(_ccLastSaved.DateText);
                    matter.ClosedDate = GetDate(_ccClosedDate.DateText);
                    matter.DestructDate = GetDate(_ccDestroyDate.DateText);
                    matter.FileNo = _txtFileAwayRef.Text;
                    if (_rdoBtnMatterCompletedYes.Checked)
                    {
                        matter.CompletedDate = DateTime.Now;
                    }
                    else
                    {
                        matter.CompletedDate = DataConstants.BlankDate;
                    }

                    #endregion

                    #region Save Matter Additional Info Tab

                    matter.Quote = Convert.ToDecimal(_txtQuote.Text);
                    matter.DisbsLimit = Convert.ToDecimal(_txtDisbsLimit.Text);
                    matter.TimeLimit = Convert.ToDecimal(_txtTimeLimit.Text);
                    matter.WIPLimit = Convert.ToDecimal(_txtWIPLimit.Text);
                    matter.OverallLimit = Convert.ToDecimal(_txtOverallMatterLimit.Text);
                    matter.Status = _txtStatus.Text;
                    matter.Indicators = _txtIndicators.Text;
                    matter.BankReference = _txtBankReference.Text;
                    if (_ddlCashCollection.SelectedValue != "")
                    {
                        matter.CashCollectionId = Convert.ToInt32(_ddlCashCollection.SelectedValue);
                    }
                    if (_txtCreditLimit.Text != "")
                    {
                        matter.TotalLockup = Convert.ToDecimal(_txtCreditLimit.Text);
                    }
                    matter.OurReference = _txtOurReference.Text;
                    matter.PreviousReference = _txtPreviousReference.Text;
                    if (_ddlBusinessSource.SelectedValue != "")
                    {
                        matter.BusinessSourceId = Convert.ToInt32(_ddlBusinessSource.SelectedValue);
                    }
                    if (_ddlSourceCampaign.SelectedValue != "")
                    {
                        matter.SourceCampaignId = Convert.ToInt32(_ddlSourceCampaign.SelectedValue);
                    }
                    if (_ddlPersonDealing.SelectedValue != "")
                    {
                        matter.PersonDealingId = Convert.ToInt32(_ddlPersonDealing.SelectedValue);
                    }
                    matter.SalutationEnvelope = _txtSalutationEnvelope.Text;
                    matter.SalutationLetter = _txtSalutationLetter.Text;
                    matter.LetterHead = _txtLetterHead.Text;

                    #endregion

                    #region Save Matter Public Funding Tab

                    matter.IsPublicFunding = _chkPublicFunded.Checked;
                    matter.Franchised = _chkSQM.Checked;
                    matter.isLondonRate = _chkLondonRate.Checked;
                    matter.UFNDate = GetDate(_ccUFNDate.DateText);
                    matter.UFN = _txtUFNNumber.Text;
                    matter.PFCertificateNo = _txtCertificateNumber.Text;
                    matter.PFCertificateNoLimits = _txtCertificateLimits.Text;

                    #endregion

                    ReturnValue returnValue = matterService.UpdateMatter(_logonSettings.LogonId, matter);

                    if (returnValue.Success)
                    {
                        // This is used, because UFN Number is auto generated and which is to be shown after Save.
                        ResetControls();
                        LoadClientMatter();

                        _lblError.CssClass = "successMessage";
                        _lblError.Text = "Matter Saved Successfully.";

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), Guid.NewGuid().ToString(), "messageHide('" + _lblError.ClientID + "')", true);
                    }
                    else
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = returnValue.Message;
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
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
        #endregion

        #endregion

        #region DropDownListEvents

        #region Branch
        /// <summary>
        /// On change of Branch, Department gets populated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_ddlBranch.SelectedValue.Trim().Length > 0)
                {
                    BranchOnSelection();
                }
                else
                {
                    _ddlDepartment.Items.Clear();
                    _ddlWorkType.Items.Clear();
                    _ddlChargeRate.Items.Clear();
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

        #region Department
        /// <summary>
        /// On change of Department, work type gets populated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_ddlDepartment.SelectedValue.Trim().Length > 0)
                {
                    DepartmentOnSelection();
                }
                else
                {
                    _ddlWorkType.Items.Clear();
                    _ddlChargeRate.Items.Clear();
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

        #region Work Type
        /// <summary>
        /// On change of Work type, charge rates, court types and some values are populated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlWorkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_ddlWorkType.SelectedValue.Trim().Length > 0)
                {
                    WorkTypeOnSelection();
                }
                else
                {
                    _ddlChargeRate.Items.Clear();
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

        #region Charge Rate
        /// <summary>
        /// On Change of Charge Rate, default Court Type will set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _ddlChargeRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_ddlChargeRate.SelectedValue.Trim().Length > 0)
                {
                    ChargeRateOnSelection();
                }
                else
                {
                    _ddlCourtType.SelectedIndex = -1;
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

        #region Matter Changed Event
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
                ResetControls();
                if (Session[SessionName.ProjectId] == null)
                {
                    if (_cliMatDetails.Message != null)
                    {
                        if (_cliMatDetails.Message.Trim().Length > 0)
                        {
                            _lblError.Text = _cliMatDetails.Message;
                            return;
                        }
                    }
                }
                else
                {
                    LoadClientMatter();
                    _dfDocumentFiles.Rebind();
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

        #endregion

        #region WebMethods
        /// <summary>
        /// Get Server Date & Time
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetSystemDate()
        {
            return DateTime.Now.Date.ToString("dd/MM/yyyy") + "$" + DateTime.Now.ToLocalTime().ToString("HH:mm");
        }

        #endregion

        #region UFN Date Text Changed
        /// <summary>
        /// On Change of UFN Date or Fee Earner, the UFN Number will be autogenerated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _txtUFNDate_TextChanged(object sender, EventArgs e)
        {

            MatterServiceClient earnerService = new MatterServiceClient();
            try
            {
                if (_ccUFNDate.DateText.Trim().Length > 0 && _ddlFeeEarner.SelectedValue.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                    }
                    catch
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = "Invalid UFN Date.";
                        return;
                    }

                    ViewState["HasChangedUFN"] = true;

                    Guid earnerId = new Guid(_ddlFeeEarner.SelectedValue);
                    DateTime UFNDate = Convert.ToDateTime(_ccUFNDate.DateText);
                    string UFNNumber = null;

                    CollectionRequest collectionReq = new CollectionRequest();

                    UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId, earnerId, UFNDate, UFNNumber);

                    if (item.Success)
                    {
                        _txtUFNNumber.Text = item.Number;
                    }
                    else
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = item.Message;
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
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

        #region UFN Number Text Changed
        /// <summary>
        /// This is to check if the UFN Number entered is correct or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _txtUFNNumber_TextChanged(object sender, EventArgs e)
        {

            MatterServiceClient earnerService = new MatterServiceClient();
            try
            {
                if (_txtUFNNumber.Text.Trim().Length > 0 && _ccUFNDate.DateText.Trim().Length > 0)
                {
                    try
                    {
                        DateTime dtParse = DateTime.Parse(_ccUFNDate.DateText);
                    }
                    catch
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = "Invalid UFN Date.";
                        return;
                    }

                    ViewState["HasChangedUFN"] = true;

                    Guid earnerId = new Guid(_ddlFeeEarner.SelectedValue);
                    DateTime UFNDate = Convert.ToDateTime(_ccUFNDate.DateText);
                    string UFNNumber = _txtUFNNumber.Text.Trim();

                    CollectionRequest collectionReq = new CollectionRequest();

                    UFNReturnValue item = earnerService.UFNValidation(_logonSettings.LogonId, earnerId, UFNDate, UFNNumber);

                    if (item.Success)
                    {
                        _txtUFNNumber.Text = item.Number;
                    }
                    else
                    {
                        _lblError.CssClass = "errorMessage";
                        _lblError.Text = item.Message;
                    }
                }
                else
                {
                    _ccUFNDate.DateText = string.Empty;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.CssClass = "errorMessage";
                _lblError.Text = ex.Message;
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

    }
}

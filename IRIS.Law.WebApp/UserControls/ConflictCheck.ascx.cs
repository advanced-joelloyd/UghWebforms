using System;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.Services.Pms.Client;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ConflictCheck : System.Web.UI.UserControl
    {
        #region Private Variables

        private LogonReturnValue _logonSettings;

        #endregion

        #region Properties

        #region FirstClientConflictNoteContent

        private string _firstClientConflictNoteContent;

        /// <summary>
        /// 
        /// </summary>
        public string FirstClientConflictNoteContent
        {
            get { return _firstClientConflictNoteContent; }
        }

        #endregion

        #region ContactConflictNoteContent

        private string _contactConflictNoteContent;

        /// <summary>
        /// 
        /// </summary>
        public string ContactConflictNoteContent
        {
            get { return _contactConflictNoteContent; }
        }

        #endregion

        #region SecondClientConflictNoteContent

        private string _secondClientConflictNoteContent;

        /// <summary>
        /// 
        /// </summary>
        public string SecondClientConflictNoteContent
        {
            get { return _secondClientConflictNoteContent; }
        }

        #endregion

        #region IsSecondClient

        private bool _isSecondClient;

        /// <summary>
        /// Needs to be set only from add client
        /// </summary>
        public bool IsSecondClient
        {
            get { return _isSecondClient; }
            set { _isSecondClient = value; }
        }

        #endregion

        #region Surname

        private string _surName;

        /// <summary>
        /// 
        /// </summary>
        public string Surname
        {
            get { return _surName; }
            set { _surName = value; }
        }

        #endregion

        #region ClientType

        private ClientType _clientType;

        /// <summary>
        /// 
        /// </summary>
        public ClientType ClientType
        {
            get { return _clientType; }
            set { _clientType = value; }
        }

        #endregion

        #region ContactType

        private int _contactType;

        /// <summary>
        /// 
        /// </summary>
        public int ContactType
        {
            get
            {
                return _contactType;
            }
            set
            {
                _contactType = value;
            }
        }

        #endregion

        #region Person

        private Person _person;

        /// <summary>
        /// 
        /// </summary>
        public Person Person
        {
            get { return _person; }
            set { _person = value; }
        }

        #endregion

        #region Organisation

        private Organisation _organisation;

        /// <summary>
        /// 
        /// </summary>
        public Organisation Organisation
        {
            get { return _organisation; }
            set { _organisation = value; }
        }

        #endregion

        #region Address

        private Address _address;

        /// <summary>
        /// 
        /// </summary>
        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #endregion

        #region AdditionalAddressDetails

        private AdditionalAddressElement[] _additionalAddressDetails;

        /// <summary>
        /// Property to get\set the value of _additionalAddressDetails
        /// </summary>
        public AdditionalAddressElement[] AdditionalDetails
        {
            get
            {
                return _additionalAddressDetails;
            }
            set
            {
                _additionalAddressDetails = value;
            }
        }

        #endregion

        #region ConflictCheckStandardReturnValue

        private ConflictCheckStandardReturnValue returnValue;

        /// <summary>
        /// Details required for getting conflict check
        /// </summary>
        public ConflictCheckStandardReturnValue ReturnConflictCheck
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// Loads conflict check page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _tblConflictCheck.Style["display"] = "";
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
        }

        #endregion

        #region Public Methods

        #region ConflictCheck

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSecondClient"></param>
        /// <returns></returns>
        public ConflictCheckStandardReturnValue PerformConflictCheck()
        {
            ConflictCheckStandardReturnValue dsConflictCheckFields;
            ClientServiceClient clientService = new ClientServiceClient();

            IRIS.Law.WebServiceInterfaces.Contact.AdditionalAddressElement[] addressElementsFirst;
            addressElementsFirst = null;

            if (_additionalAddressDetails != null && _additionalAddressDetails.Length == 10)
            {
                //if (!_isSecondClient || _isSecondClient)
                //{
                addressElementsFirst = new AdditionalAddressElement[10];
                #region Set First Person's and Second Person's Additional Address Information
                for (int i = 0; i <= 9; i++)
                {
                    addressElementsFirst[i] = new AdditionalAddressElement();
                    switch (i)
                    {
                        case 0:
                            addressElementsFirst[0].ElementText = _additionalAddressDetails[0].ElementText;
                            break;
                        case 1:
                            addressElementsFirst[1].ElementText = _additionalAddressDetails[1].ElementText;
                            break;
                        case 2:
                            addressElementsFirst[2].ElementText = _additionalAddressDetails[2].ElementText;
                            break;
                        case 3:
                            addressElementsFirst[3].ElementText = _additionalAddressDetails[3].ElementText;
                            break;
                        case 4:
                            addressElementsFirst[4].ElementText = _additionalAddressDetails[4].ElementText;
                            break;
                        case 5:
                            addressElementsFirst[5].ElementText = _additionalAddressDetails[5].ElementText;
                            break;
                        case 6:
                            addressElementsFirst[6].ElementText = _additionalAddressDetails[6].ElementText;
                            break;
                        case 7:
                            addressElementsFirst[7].ElementText = _additionalAddressDetails[7].ElementText;
                            break;
                        case 8:
                            addressElementsFirst[8].ElementText = _additionalAddressDetails[8].ElementText;
                            break;
                        case 9:
                            addressElementsFirst[9].ElementText = _additionalAddressDetails[9].ElementText;
                            break;
                    }
                    //}
                #endregion
                }
            }

            CollectionRequest collectionRequest = new CollectionRequest();
            collectionRequest.StartRow = 0;
            dsConflictCheckFields = clientService.ConflictCheck(_logonSettings.LogonId,
                                                                    collectionRequest,
                                                                    _clientType,
                                                                    _person,
                                                                    _organisation,
                                                                    _address,
                                                                    addressElementsFirst,
                                                                    _logonSettings.ConflictCheckRoles
                                                                    );

            return dsConflictCheckFields;
        }

        #endregion

        #region BindConflictCheckGridView

        /// <summary>
        /// Binds conflicts on surname while adding new client or contact.
        /// </summary>
        /// <param name="returnConflictCheck"></param>
        /// <param name="isSecondClient"></param>
        public void BindConflictCheckGridView()
        {
            try
            {
                if (returnValue.IsConflict)
                {
                    _tblConflictCheckGroup.Rows.Clear();

                    if (returnValue.Summary.Length > 0)
                    {
                        _lblConflictSurname.Text = "&nbsp;&nbsp;&nbsp;" + returnValue.Summary.ToString();
                    }

                    if (returnValue.ConflictCheckStandard.Rows.Length > 0)
                    {
                        string selectType = "";
                        string firstClientConflictNoteContentTemp = string.Empty;
                        bool isNextNewGroup = false;
                        int NoOfConflicts = 0;
                        TableRow trMain = null;
                        TableCell tcMain = null;
                        Panel pnlConflictCheck = null;
                        Table tblGroup = null;
                        Label lblGroupHeader = null;

                        _firstClientConflictNoteContent = string.Empty;
                        _firstClientConflictNoteContent += Environment.NewLine;

                        // This property is for contacts.
                        _contactConflictNoteContent = string.Empty;
                        _contactConflictNoteContent += Environment.NewLine;

                        for (int i = 0; i <= returnValue.ConflictCheckStandard.Rows.Length - 1; i++)
                        {
                            string noOfPossibleMatches = string.Empty;
                            isNextNewGroup = false;

                            if (selectType != returnValue.ConflictCheckStandard.Rows[i].SelectType)
                            {
                                NoOfConflicts = 1;

                                tcMain = null;
                                trMain = null;
                                tcMain = new TableCell();
                                trMain = new TableRow();
                                pnlConflictCheck = new Panel();
                                lblGroupHeader = new Label();
                                tblGroup = new Table();

                                string type_id = returnValue.ConflictCheckStandard.Rows[i].SelectType.Replace(' ', '_');

                                //Collapsible Panel
                                AjaxControlToolkit.CollapsiblePanelExtender collapsePanel = new AjaxControlToolkit.CollapsiblePanelExtender();
                                collapsePanel.ID = "_cpe_" + type_id;
                                collapsePanel.Collapsed = true;
                                collapsePanel.ExpandedImage = "~/Images/GIFs/up.gif";
                                collapsePanel.CollapsedImage = "~/Images/GIFs/down.gif";
                                collapsePanel.TargetControlID = "_pnlConflictCheck_" + type_id;
                                collapsePanel.ExpandControlID = "_pnlHeader_" + type_id;
                                collapsePanel.CollapseControlID = "_pnlHeader_" + type_id;
                                collapsePanel.ImageControlID = "_img_" + type_id;
                                tcMain.Controls.Add(collapsePanel);

                                //Panel Header having Arrow Image and Label
                                Panel pnlHeader = new Panel();
                                pnlHeader.Style["Width"] = "99.9%";
                                pnlHeader.ID = "_pnlHeader_" + type_id;
                                Image imgSelectType = new Image();
                                imgSelectType.ID = "_img_" + type_id;

                                lblGroupHeader.Style["Width"] = "80%";
                                lblGroupHeader.CssClass = "collapseLabel";
                                lblGroupHeader.ID = "_lblHeader" + type_id;

                                pnlHeader.Controls.Add(imgSelectType);
                                pnlHeader.Controls.Add(lblGroupHeader);
                                tcMain.Controls.Add(pnlHeader);

                                tblGroup.ID = "_tbl_" + type_id;
                                tblGroup.CellSpacing = 0;
                                tblGroup.Style["Width"] = "99%";

                                pnlConflictCheck.ID = "_pnlConflictCheck_" + type_id;

                                //Creating Headers
                                TableHeaderRow tblRowMatchIn = new TableHeaderRow();
                                tblRowMatchIn.CssClass = "gridViewHeader";
                                TableHeaderCell tblCellMatchIn = new TableHeaderCell();
                                tblCellMatchIn.Text = "Match In";
                                tblCellMatchIn.Style["Width"] = "25%";
                                tblCellMatchIn.HorizontalAlign = HorizontalAlign.Left;
                                TableHeaderCell tblCellRef = new TableHeaderCell();
                                tblCellRef.Text = "Ref";
                                tblCellRef.Style["Width"] = "25%";
                                tblCellRef.HorizontalAlign = HorizontalAlign.Left;
                                TableHeaderCell tblCellName = new TableHeaderCell();
                                tblCellName.Text = "Name";
                                tblCellName.Style["Width"] = "25%";
                                tblCellName.HorizontalAlign = HorizontalAlign.Left;
                                TableHeaderCell tblCellAddress = new TableHeaderCell();
                                tblCellAddress.Text = "Address";
                                tblCellAddress.Style["Width"] = "25%";
                                tblCellAddress.HorizontalAlign = HorizontalAlign.Left;

                                tblRowMatchIn.Cells.Add(tblCellMatchIn);
                                tblRowMatchIn.Cells.Add(tblCellRef);
                                tblRowMatchIn.Cells.Add(tblCellName);
                                tblRowMatchIn.Cells.Add(tblCellAddress);

                                tblGroup.Rows.Add(tblRowMatchIn);
                            }
                            else
                            {
                                NoOfConflicts += 1;
                            }

                            // Add rows for conflict check
                            TableRow tblRowConflictCheck = new TableRow();
                            tblRowConflictCheck.CssClass = "gridViewRow";

                            if ((i % 2) != 0)
                            {
                                tblRowConflictCheck.CssClass = "gridViewRowAlternate";
                            }

                            TableCell tblCellMatchInValue = new TableCell();
                            tblCellMatchInValue.Text = returnValue.ConflictCheckStandard.Rows[i].SelectType;

                            TableCell tblCellRefValue = new TableCell();
                            string cliMatNo = returnValue.ConflictCheckStandard.Rows[i].ClientRef.Trim();
                            if (cliMatNo.Length > 6)
                            {
                                cliMatNo = cliMatNo.Insert(6, "-");
                            }
                            tblCellRefValue.Text = cliMatNo;

                            TableCell tblCellNameValue = new TableCell();

                            if (Convert.ToBoolean(returnValue.ConflictCheckStandard.Rows[i].IsMember))
                            {
                                tblCellNameValue.Text = returnValue.ConflictCheckStandard.Rows[i].PersonTitle + " " + returnValue.ConflictCheckStandard.Rows[i].PersonName + " " + returnValue.ConflictCheckStandard.Rows[i].PersonSurName;
                            }
                            else
                            {
                                tblCellNameValue.Text = returnValue.ConflictCheckStandard.Rows[i].OrgName;
                            }
                            TableCell tblCellAddressValue = new TableCell();
                            tblCellAddressValue.Text = returnValue.ConflictCheckStandard.Rows[i].AddressStreetNo + " " + returnValue.ConflictCheckStandard.Rows[i].AddressHouseName + " " + returnValue.ConflictCheckStandard.Rows[i].AddressLine1;

                            tblRowConflictCheck.Cells.Add(tblCellMatchInValue);
                            tblRowConflictCheck.Cells.Add(tblCellRefValue);
                            tblRowConflictCheck.Cells.Add(tblCellNameValue);
                            tblRowConflictCheck.Cells.Add(tblCellAddressValue);
                            tblGroup.Rows.Add(tblRowConflictCheck);

                            if (i == returnValue.ConflictCheckStandard.Rows.Length - 1)
                            {
                                isNextNewGroup = true;
                            }
                            else if (returnValue.ConflictCheckStandard.Rows[i].SelectType != returnValue.ConflictCheckStandard.Rows[i + 1].SelectType)
                            {
                                isNextNewGroup = true;
                            }

                            if (isNextNewGroup)
                            {
                                pnlConflictCheck.Controls.Add(tblGroup);

                                lblGroupHeader.Text = "&nbsp;&nbsp;" + returnValue.ConflictCheckStandard.Rows[i].SelectType + " (" + NoOfConflicts.ToString();

                                if (NoOfConflicts == 1)
                                {
                                    lblGroupHeader.Text = lblGroupHeader.Text + " Conflict";
                                }
                                else
                                {
                                    lblGroupHeader.Text = lblGroupHeader.Text + " Conflicts";
                                }
                                lblGroupHeader.Text = lblGroupHeader.Text + ") ";

                                switch (returnValue.ConflictCheckStandard.Rows[i].SelectType)
                                {
                                    case "Otherside":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Red.Name;
                                        break;
                                    case "Buyer":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Green.Name;
                                        break;
                                    case "Seller":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Purple.Name;
                                        break;
                                    case "General Contact":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Red.Name;
                                        break;
                                    case "Consolidated Sanctions":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Orange.Name;
                                        break;
                                    case "Investment Ban":
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Brown.Name;
                                        break;
                                    default:
                                        lblGroupHeader.Style["Color"] = System.Drawing.Color.Black.Name;
                                        break;
                                }

                                tcMain.Controls.Add(pnlConflictCheck);
                                trMain.Cells.Add(tcMain);
                                _tblConflictCheckGroup.Rows.Add(trMain);

                                TableRow tblRowSeparation = new TableRow();
                                TableCell tblCellSeparation = new TableCell();
                                tblCellSeparation.CssClass = "TitleSeparation";
                                tblRowSeparation.Cells.Add(tblCellSeparation);
                                _tblConflictCheckGroup.Rows.Add(tblRowSeparation);

                                noOfPossibleMatches += NoOfConflicts.ToString() + " possible match(es) found in " + returnValue.ConflictCheckStandard.Rows[i].SelectType + " details" + Environment.NewLine;
                            }

                            selectType = returnValue.ConflictCheckStandard.Rows[i].SelectType;

                            firstClientConflictNoteContentTemp += returnValue.ConflictCheckStandard.Rows[i].SelectType + " / ";
                            firstClientConflictNoteContentTemp += cliMatNo + " / ";

                            if (Convert.ToBoolean(returnValue.ConflictCheckStandard.Rows[i].IsMember))
                            {
                                firstClientConflictNoteContentTemp += returnValue.ConflictCheckStandard.Rows[i].PersonTitle + " " + returnValue.ConflictCheckStandard.Rows[i].PersonName + " " + returnValue.ConflictCheckStandard.Rows[i].PersonSurName + " / ";
                            }
                            else
                            {
                                firstClientConflictNoteContentTemp += returnValue.ConflictCheckStandard.Rows[i].OrgName + " / ";
                            }

                            firstClientConflictNoteContentTemp += returnValue.ConflictCheckStandard.Rows[i].AddressStreetNo + " " + returnValue.ConflictCheckStandard.Rows[i].AddressHouseName + " " + returnValue.ConflictCheckStandard.Rows[i].AddressLine1 + " / ";
                            firstClientConflictNoteContentTemp += Environment.NewLine;

                            if (noOfPossibleMatches != string.Empty)
                            {
                                _firstClientConflictNoteContent = noOfPossibleMatches + firstClientConflictNoteContentTemp;

                                // Property to be used on contacts form.
                                _contactConflictNoteContent = noOfPossibleMatches + firstClientConflictNoteContentTemp; ;
                                firstClientConflictNoteContentTemp = string.Empty;
                            }
                        }
                        if (!_isSecondClient)
                        {
                            _firstClientConflictNoteContent = _lblConflictSurname.Text.Replace("&nbsp;", "") + Environment.NewLine + Environment.NewLine + _firstClientConflictNoteContent;

                            // Property to be used on contacts form.
                            _contactConflictNoteContent = _lblConflictSurname.Text.Replace("&nbsp;", "") + Environment.NewLine + Environment.NewLine + _contactConflictNoteContent;
                        }
                        else
                        {
                            _secondClientConflictNoteContent = _lblConflictSurname.Text.Replace("&nbsp;", "") + Environment.NewLine + Environment.NewLine + _firstClientConflictNoteContent;
                        }
                    }
                }
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ConflictResults : System.Web.UI.UserControl
    {
        /// <summary>
        /// Enum for Context Entity Type
        /// </summary>
        public enum EntityType : int
        {
            Client = 1,
            Contact = 2,
            Unknown = 3
        }

        /// <summary>
        /// Common class for ConflictCheckResults
        /// </summary>
        private class ConflictCheckResult
        {
            public string SelectType;
            public string ClientRef;
            public string PersonFullName;
            public string FullAddress;
            public string OrgName;
            public int IsMember;
        }

        /// <summary>
        /// ConflictCheckSummary value
        /// </summary>
        private string _conflictCheckSummary = string.Empty;

        /// <summary>
        /// Common List class for ConflictCheckResults
        /// </summary>
        private List<ConflictCheckResult> _conflictCheckResults;

        /// <summary>
        /// Conflict Check Count
        /// </summary>
        private int _conflictCheckCount;

        private EntityType _contextType;

        /// <summary>
        /// The Entity Type in Context
        /// </summary>
        public EntityType ContextType
        {
            get { return _contextType; }
            set { _contextType = value; }
        }

        private bool _isConflict;

        /// <summary>
        /// Indicates a conflict exists
        /// </summary>
        public bool IsConflict
        {
            get { return _isConflict; }
            set { _isConflict = value; }
        }

        /// <summary>
        /// Loads conflict results page
        /// </summary>
        /// <param name="sender">The sender accepts object value</param>
        /// <param name="e">The e accepts EventArgs value</param>
        /// <returns></returns>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Binds conflicts on surname while adding new client or contact.
        /// </summary>
        /// <returns></returns>
        public void BindConflictResultsGridView()
        {
            _tblConflictResultsGroup.Rows.Clear();

            if (_conflictCheckSummary.Length > 0)
            {
                _lblConflictSurname.Text = "&nbsp;&nbsp;&nbsp;" + _conflictCheckSummary.ToString();
            }

            if (_conflictCheckResults.Count > 0)
            {
                string selectType = string.Empty;
                string selectTypeAsID = string.Empty;
                bool isNextNewGroup = false;
                int NoOfConflicts = 0;
                TableRow trMain = null;
                TableCell tcMain = null;
                Panel pnlConflictResults = null;
                Table tblGroup = null;
                Label lblGroupHeader = null;

                for (int i = 0; i <= _conflictCheckResults.Count - 1; i++)
                {
                    isNextNewGroup = false;

                    if (selectType != _conflictCheckResults[i].SelectType)
                    {
                        NoOfConflicts = 1;

                        tcMain = null;
                        trMain = null;
                        tcMain = new TableCell();
                        trMain = new TableRow();
                        pnlConflictResults = new Panel();
                        lblGroupHeader = new Label();
                        tblGroup = new Table();
                        selectTypeAsID = _conflictCheckResults[i].SelectType.Replace(" ", "");

                        //Collapsible Panel
                        AjaxControlToolkit.CollapsiblePanelExtender collapsePanel = new AjaxControlToolkit.CollapsiblePanelExtender();
                        collapsePanel.ID = "_cpe_" + selectTypeAsID;
                        collapsePanel.Collapsed = true;
                        collapsePanel.ExpandedImage = "~/Images/GIFs/up.gif";
                        collapsePanel.CollapsedImage = "~/Images/GIFs/down.gif";
                        collapsePanel.TargetControlID = "_pnlConflictResults_" + selectTypeAsID;
                        collapsePanel.ExpandControlID = "_pnlHeader_" + selectTypeAsID;
                        collapsePanel.CollapseControlID = "_pnlHeader_" + selectTypeAsID;
                        collapsePanel.ImageControlID = "_img_" + selectTypeAsID;
                        tcMain.Controls.Add(collapsePanel);

                        //Panel Header having Arrow Image and Label
                        Panel pnlHeader = new Panel();
                        pnlHeader.Style["Width"] = "99.9%";
                        pnlHeader.ID = "_pnlHeader_" + selectTypeAsID;
                        Image imgSelectType = new Image();
                        imgSelectType.ID = "_img_" + selectTypeAsID;

                        lblGroupHeader.Style["Width"] = "80%";
                        lblGroupHeader.CssClass = "collapseLabel";
                        lblGroupHeader.ID = "_lblHeader" + selectTypeAsID;

                        pnlHeader.Controls.Add(imgSelectType);
                        pnlHeader.Controls.Add(lblGroupHeader);
                        tcMain.Controls.Add(pnlHeader);

                        tblGroup.ID = "_tbl_" + selectTypeAsID;
                        tblGroup.CellSpacing = 0;
                        tblGroup.Style["Width"] = "99%";

                        pnlConflictResults.ID = "_pnlConflictResults_" + selectTypeAsID;

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
                    TableRow tblRowConflictResults = new TableRow();
                    tblRowConflictResults.CssClass = "gridViewRow";

                    if ((i % 2) != 0)
                    {
                        tblRowConflictResults.CssClass = "gridViewRowAlternate";
                    }

                    TableCell tblCellMatchInValue = new TableCell();
                    tblCellMatchInValue.Text = _conflictCheckResults[i].SelectType;

                    TableCell tblCellRefValue = new TableCell();
                    tblCellRefValue.Text = this.SetClientRefText(_conflictCheckResults[i].ClientRef);

                    TableCell tblCellNameValue = new TableCell();
                    tblCellNameValue.Text = this.SetNameText(_conflictCheckResults[i].IsMember,
                        _conflictCheckResults[i].PersonFullName,
                        _conflictCheckResults[i].OrgName);

                    TableCell tblCellAddressValue = new TableCell();
                    tblCellAddressValue.Text = _conflictCheckResults[i].FullAddress;

                    tblRowConflictResults.Cells.Add(tblCellMatchInValue);
                    tblRowConflictResults.Cells.Add(tblCellRefValue);
                    tblRowConflictResults.Cells.Add(tblCellNameValue);
                    tblRowConflictResults.Cells.Add(tblCellAddressValue);
                    tblGroup.Rows.Add(tblRowConflictResults);

                    if (i == _conflictCheckResults.Count - 1)
                    {
                        isNextNewGroup = true;
                    }
                    else if (_conflictCheckResults[i].SelectType != _conflictCheckResults[i + 1].SelectType)
                    {
                        isNextNewGroup = true;
                    }

                    if (isNextNewGroup)
                    {
                        pnlConflictResults.Controls.Add(tblGroup);

                        lblGroupHeader.Text = this.SetHeaderText(_conflictCheckResults[i].SelectType, NoOfConflicts);
                        lblGroupHeader.Style["Color"] = this.SetHeaderColour(_conflictCheckResults[i].SelectType);

                        tcMain.Controls.Add(pnlConflictResults);
                        trMain.Cells.Add(tcMain);
                        _tblConflictResultsGroup.Rows.Add(trMain);

                        TableRow tblRowSeparation = new TableRow();
                        TableCell tblCellSeparation = new TableCell();
                        tblCellSeparation.CssClass = "TitleSeparation";
                        tblRowSeparation.Cells.Add(tblCellSeparation);
                        _tblConflictResultsGroup.Rows.Add(tblRowSeparation);
                    }
                    selectType = _conflictCheckResults[i].SelectType;
                }
            }
            else
            {
                TableRow tblRowSeparation = new TableRow();
                TableCell tblCellSeparation = new TableCell();
                tblCellSeparation.CssClass = "TitleSeparation";
                tblRowSeparation.Cells.Add(tblCellSeparation);
                _tblConflictResultsGroup.Rows.Add(tblRowSeparation);

                TableRow tblRowConflictResults = new TableRow();
                tblRowConflictResults.CssClass = "gridViewRow";
                TableCell tblCellTextValue = new TableCell();
                tblCellTextValue.Text = "No matches found";
                tblRowConflictResults.Cells.Add(tblCellTextValue);
                _tblConflictResultsGroup.Rows.Add(tblRowConflictResults);

                tblRowSeparation = new TableRow();
                tblCellSeparation = new TableCell();
                tblCellSeparation.CssClass = "TitleSeparation";
                tblRowSeparation.Cells.Add(tblCellSeparation);
                _tblConflictResultsGroup.Rows.Add(tblRowSeparation);
            }
        }

        /// <summary>
        /// Format the Client Ref for Display
        /// </summary>
        /// <param name="clientRef">The clientRef accepts string value</param>
        /// <returns>string</returns>
        private string SetClientRefText(string clientRef)
        {
            string cliMatNo = string.Empty;
            cliMatNo = clientRef.Trim();
            if (cliMatNo.Length > 6)
            {
                cliMatNo = cliMatNo.Insert(6, "-");
            }

            return (string)cliMatNo;
        }

        /// <summary>
        /// Format the Name for Display
        /// </summary>
        /// <param name="isMember">The isMember accepts int value</param>
        /// <param name="personName">The personName accepts string value</param>
        /// <param name="organisationName">The organisationName accepts string value</param>
        /// <returns>string</returns>
        private string SetNameText(int isMember, string personName, string organisationName)
        {
            string nameText = string.Empty;

            if (Convert.ToBoolean(isMember))
            {
                nameText = personName;
            }
            else
            {
                nameText = organisationName;
            }

            return (string)nameText;
        }

        /// <summary>
        /// Formats the Group Header Text
        /// </summary>
        /// <param name="selectType">The selectType accepts string value</param>
        /// <param name="noOfConflicts">The noOfConflicts accepts int value</param>
        /// <returns>string</returns>
        private string SetHeaderText(string selectType, int noOfConflicts)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("&nbsp;&nbsp;");
            sb.Append(selectType);
            sb.Append(" (");
            sb.Append(noOfConflicts.ToString());
            if (noOfConflicts == 1)
            {
                sb.Append(" Conflict");
            }
            else
            {
                sb.Append(" Conflicts");
            }
            sb.Append(") ");

            return (string)sb.ToString();
        }

        /// <summary>
        /// Sets the Colour of the Group Header Text
        /// </summary>
        /// <param name="selectType">The selectType accepts string value</param>
        /// <returns>string</returns>
        private string SetHeaderColour(string selectType)
        {
            string colour = string.Empty;
            switch (selectType)
            {
                case "Otherside":
                    colour = System.Drawing.Color.Red.Name;
                    break;
                case "Buyer":
                    colour = System.Drawing.Color.Green.Name;
                    break;
                case "Seller":
                    colour = System.Drawing.Color.Purple.Name;
                    break;
                case "General Contact":
                    colour = System.Drawing.Color.Red.Name;
                    break;
                case "Consolidated Sanctions":
                    colour = System.Drawing.Color.Orange.Name;
                    break;
                case "Investment Ban":
                    colour = System.Drawing.Color.Brown.Name;
                    break;
                default:
                    colour = System.Drawing.Color.Black.Name;
                    break;
            }

            return (string)colour;
        }

    }
}
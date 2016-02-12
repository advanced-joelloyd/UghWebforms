using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using System.Drawing;

namespace IRIS.Law.WebApp.UserControls
{
    /// <summary>
    /// ClassName: Iris.Ews.Server.Web.UserControls.lookupUserControl
    /// Class Id: Iris.Ews.Server.Web.UserControls.PS_lookupUserControl
    /// Columns[0] => Identifier/Key column
    /// Colcumns[2] => Description Column
    /// </summary>
    public partial class lookupUserControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip
        {
            get
            {
                if (_txtLookupReference != null)
                {
                    return _txtLookupReference.ToolTip;
                }

                return string.Empty;
            }
            set
            {
                if (_txtLookupReference != null)
                {
                    _txtLookupReference.ToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the lookup id.
        /// </summary>
        /// <value>The lookup id.</value>
        public string LookupId
        {
            get
            {
                if (_hdnLookupId != null && _hdnLookupId.Value != string.Empty)
                {
                    return _hdnLookupId.Value;
                }

                return string.Empty;
            }
            set
            {
                if (_hdnLookupId != null)
                {
                    _hdnLookupId.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the display name value.
        /// </summary>
        /// <value>The display name value.</value>
        public string DisplayNameValue
        {
            get
            {
                if (_txtLookupReference != null && _txtLookupReference.Text != string.Empty)
                {
                    return _txtLookupReference.Text.Trim();
                }

                return string.Empty;
            }
            set
            {
                if (_txtLookupReference != null)
                {
                    _txtLookupReference.Text = value.Trim();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        /// <value>The name of the attribute.</value>
        public string AttributeValue
        {
            get
            {
                if (_hdnLookupValue != null && _hdnLookupValue.Value != string.Empty)
                {
                    return _hdnLookupValue.Value;
                }

                return string.Empty;
            }
            set
            {
                if (_hdnLookupValue != null)
                {
                    _hdnLookupValue.Value = value.ToString();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        /// <value>The name of the attribute.</value>
        public string AttributeName
        {
            get
            {
                if (_hdnLookupName != null && _hdnLookupName.Value != string.Empty)
                {
                    return _hdnLookupName.Value;
                }

                return string.Empty;
            }
            set
            {
                if (_hdnLookupName != null)
                {
                    _hdnLookupName.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>The window title.</value>
        public string WindowTitle
        {
            get
            {
                if (_hdnLookupTitle != null && _hdnLookupTitle.Value != string.Empty)
                {
                    return _hdnLookupTitle.Value;
                }

                return string.Empty;
            }
            set
            {
                if (_hdnLookupTitle != null)
                {
                    _hdnLookupTitle.Value = value;
                }
            }
        }

        /// <summary>
        /// Contains the source data of the lookup values when the lookup button is clicked.
        /// </summary>
        private DataTable SourceData
        {
            get
            {
                if (Session["SourceData"] != null)
                {
                    return (DataTable)Session["SourceData"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Session["SourceData"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set width of text control based on width from Designer Studio
            _txtLookupReference.Width = GetTextControlWidth();

            if (!IsPostBack)
            {
                _txtLookupReference.Attributes.Add("onKeyDown", "GetKeyPress1(event,'" + _hiddenShowpopUP.ClientID + "')");
                _btnCancel.Attributes.Add("onclick", "ClearSelectedRowIndex(event,'" + gvLookup.ClientID + "','" + _hdnLookupId.ClientID + "')");
                _txtLookupReference.Text = DisplayNameValue.Trim();
                lblLookupWindowTitle.Text = WindowTitle.Trim();
            }
            else
            {
                // Handle the JavaScript post back required by the parent page
                if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].ToUpper().Contains("MYDBLCLICK"))
                {
                    string[] values = Request.Form["__EVENTARGUMENT"].Split('~');

                    if (values != null && values.Length == 3)
                    {
                        if (AttributeName == values[2])
                        {
                            this.LookupId = values[0];
                            gvLookup.SelectedIndex = int.Parse(values[1]);
                            GridViewRow row = gvLookup.SelectedRow;
                            _hiddenShowpopUP.Value = "true";
                            _txtLookupReference.Text = row.Cells[1].Text.Trim(); // Identifier/Key column
                            _hdnLookupRef.Value = row.Cells[1].Text.Trim();
                            AttributeValue = row.Cells[2].Text.Trim();  // Description column
                            _modalpopupLookupSearch.Hide();
                        }

                        lookupUserControl userControl;

                        // Clear WorkType and ChargeRate value if Department value has been selected
                        if (!string.IsNullOrEmpty(values[2]) && values[2].ToUpper().Trim().Equals("MATTER.DEPARTMENT"))
                        {
                            if (this.Parent.FindControl("WorkType") != null)
                            {
                                userControl = (lookupUserControl)this.Parent.FindControl("WorkType");
                                userControl.Clear();
                            }

                            if (this.Parent.FindControl("ChargeRate") != null)
                            {
                                userControl = (lookupUserControl)this.Parent.FindControl("ChargeRate");
                                userControl.Clear();
                            }
                        }

                        // Clear ChargeRate value if WorkType value has been selected
                        else if (!string.IsNullOrEmpty(values[2]) && values[2].ToUpper().Trim().Equals("MATTER.WORKTYPE"))
                        {
                            if (this.Parent.FindControl("ChargeRate") != null)
                            {
                                userControl = (lookupUserControl)this.Parent.FindControl("ChargeRate");
                                userControl.Clear();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the _btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void _btnCancel_Click(object sender, EventArgs e)
        {
            _modalpopupLookupSearch.Hide();
            _txtLookupReference.Focus();
        }

        /// <summary>
        /// Handles the filter by capturing the text changed event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        protected void FilterTextChanged(object sender, EventArgs e)
        {
            DataTable filteredData = new DataTable();
            DataRow[] filteredRows = null;
            TextBox textbox = new TextBox();
            int column = 0;

            if (this.SourceData != null)
            {
                textbox = (TextBox)sender;

                if (textbox.ID.Equals(txtRef.ID))
                {
                    column = 1;
                }
                else if (textbox.ID.Equals(txtDescription.ID))
                {
                    column = 2;
                }

                filteredRows = this.SourceData.Select(string.Format("{0} LIKE '%{1}%'", this.SourceData.Columns[column], textbox.Text.Trim()));

                filteredData = this.SourceData.Clone();

                foreach (DataRow dataRow in filteredRows)
                {
                    filteredData.ImportRow(dataRow);
                }

                gvLookup.DataSource = filteredData;
                gvLookup.DataBind();
            }
        }

        public void ShowAlertMessage(string error)
        {
            Page page = HttpContext.Current.Handler as Page;
            if (page != null)
            {
                // error = error.Replace("'", "\'");
                // ScriptManager.RegisterStartupScript(page, page.GetType(), "err_msg", "alert('" + error + "');", true);

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "selectAndFocus", "$get('" + error + "').focus();$get('" + error + "').select();", true);

            }
        }

        /// <summary>
        /// Handles the RowDataBound event of the gvLookup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gvLookup_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string PK = string.Empty;

                if (AttributeName.ToUpper().EndsWith(".TITLE"))
                {
                    PK = e.Row.Cells[1].Text.Trim();
                }
                else
                {
                    PK = e.Row.Cells[0].Text.Trim();
                }

                e.Row.Attributes.Add("onClick",
                                     "ChangeRowColor('" + gvLookup.ClientID + "','" + e.Row.RowIndex + "','" +
                                     _hdnLookupId.ClientID + "','" + _btnSelect.ClientID + "','" + AttributeName + "','" + PK + "')");

                if (AttributeName.ToUpper().EndsWith(".TITLE"))
                {
                    e.Row.Attributes.Add("ondblclick", "Javascript:__doPostBack('myDblClick','" + e.Row.Cells[1].Text.Trim() + "~" + e.Row.RowIndex + "~" + AttributeName + "');");
                }
                else
                {
                    e.Row.Attributes.Add("ondblclick", "Javascript:__doPostBack('myDblClick','" + e.Row.Cells[0].Text.Trim() + "~" + e.Row.RowIndex + "~" + AttributeName + "');");
                }
            }
        }

        /// <summary>
        /// Gets the controls width, which is set in Designer Studio.
        /// </summary>
        /// <returns>Unit containing the width of the control.</returns>
        protected Unit GetControlWidth()
        {
            // Default to 100% as a safety check to ensure screen still loads
            Unit width = new Unit("100%");
            UserControl lookupControl = new UserControl();
            string styleAttribute = string.Empty;
            string[] styleAttributes = null;

            // Find itself in the parent page
            if (Parent.FindControl(this.ID) != null)
            {
                lookupControl = (UserControl)Parent.FindControl(this.ID);

                // Extract the style attribute
                if (lookupControl.Attributes["style"] != null && !string.IsNullOrEmpty(lookupControl.Attributes["style"].ToString()))
                {
                    styleAttributes = lookupControl.Attributes["Style"].ToString().Split(';');
                }

                // Find width portion and clean string    
                if (styleAttributes != null && styleAttributes.Length > 0)
                {
                    // If any of this errors, then the default Unit will be returned
                    styleAttribute = styleAttributes.Last(w => w.ToUpper().Replace(" ", string.Empty).Contains("WIDTH:"));
                    width = Unit.Parse(styleAttribute.ToUpper().Replace(" ", string.Empty).Replace("WIDTH:", string.Empty));
                }
            }

            return width;
        }

        /// <summary>
        /// Gets the width to use for the textbox control.
        /// </summary>
        /// <returns></returns>
        private Unit GetTextControlWidth()
        {
            // Default to 100% width
            Unit textWidth = new Unit("100%");
            Unit controWidth;
            Bitmap image;

            // Get width of main control and get the image from the ImageButton
            controWidth = GetControlWidth();
            image = new Bitmap(Server.MapPath(_imgTextControl.ImageUrl));

            // Ensure width is in pixels and deduct the image width from the width assigned in Designer Studio
            if (image != null && controWidth != null && controWidth.Type == UnitType.Pixel)
            {
                textWidth = new Unit(controWidth.Value - image.Width);
            }

            return textWidth;
        }

        /// <summary>
        /// Binds source data to the GridView and caches the data for filter use.
        /// </summary>
        /// <param name="data"></param>
        private void BindSourceData(DataTable data)
        {
            if (data != null && data.Columns.Count > 2)
            {
                ((BoundField)gvLookup.Columns[0]).DataField = data.Columns[0].ColumnName;
                ((BoundField)gvLookup.Columns[1]).DataField = data.Columns[1].ColumnName;
                ((BoundField)gvLookup.Columns[2]).DataField = data.Columns[2].ColumnName;
            }

            gvLookup.DataSource = data;
            gvLookup.DataBind();

            _hdnGridRowCount.Value = gvLookup.Rows.Count.ToString();

            this.SourceData = data;
        }

        /// <summary>
        /// Clears the lookup control.
        /// </summary>
        public void Clear()
        {
            this._txtLookupReference.Text = string.Empty;
            this.txtRef.Text = string.Empty;
            this.txtDescription.Text = string.Empty;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ImportDocument : System.Web.UI.UserControl
    {
        #region EnableImportDocument
        public bool EnableImportDocument
        {
            set
            {
                _btnImportDocument.Enabled = value;
            }
        }
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

        #region Editmode
        private bool _editMode;
        public bool Editmode
        {
            set
            {
                _editMode = value;
            }
        }
        #endregion

        #region ShowModalpopup
        public bool ShowModalpopup
        {
            set
            {
                if (_editMode)
                {
                    _txtFileName.Visible = true;
                    _fileName.Visible = false;
                    _ddlDocType.Enabled = false;

                    // Values hardcoded for mockup
                    _lblError.Text = string.Empty;
                    _txtFileName.Text = "ILB - Refactoring.doc";
                    _txtDocument.Text = "High Level Architecture";
                    _ccCreatedDate.DateText = "17/07/2009";
                    _txtNotes.Text = "Claim Cost Summary Sheet - Investigation";
                    _ddlFeeEarner.SelectedIndex = 1;
                }
                else
                {
                    _txtFileName.Visible = false;
                    _fileName.Visible = true;
                    _ddlDocType.Enabled = true;
                }

                if (value)
                {
                    _modalpopupDocImport.Show();
                }
            }
        }
        #endregion

        #region Delegate
        public delegate void DocumentImportedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the client reference changes.
        /// </summary>
        public event DocumentImportedEventHandler DocumentImported;

        protected virtual void OnDocumentImported(EventArgs e)
        {
            if (DocumentImported != null)
            {
                DocumentImported(this, e);
            }
        }

        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _pnlImportDocSearch.Style["display"] = "none";
                BindDocumentTypes();
                BindFeeEarner();
            }

        }
        #endregion

        #region Import button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _lblError.Text = "Document information saved successfully.";
                _lblError.CssClass = "successMessage";
                _modalpopupDocImport.Show();

                OnDocumentImported(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Bind Document Types

        /// <summary>
        /// Gets the matters for the client.
        /// </summary>
        private void BindDocumentTypes()
        {
            _ddlDocType.DataSource = DataTables.GetImportDocumentTypes();
            _ddlDocType.DataTextField = "Description";
            _ddlDocType.DataBind();
        }

        #endregion

        #region BindFeeEarner
        private void BindFeeEarner()
        {
            _ddlFeeEarner.DataSource = DataTables.GetFeeEarners();
            _ddlFeeEarner.DataTextField = "Name";
            _ddlFeeEarner.DataBind();
            AddDefaultToDropDownList(_ddlFeeEarner);
        }


        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region ResetImportDocControls
        private void ResetImportDocControls()
        {
            _lblError.Text = string.Empty;
            _ddlDocType.SelectedIndex = 0;
            _txtDocument.Text = string.Empty;
            _ccCreatedDate.DateText = string.Empty;
            _txtNotes.Text = string.Empty;
            _ddlFeeEarner.SelectedIndex = 0;
            _chkEncryptFile.Checked = false;
            _chkLockDocument.Checked = false;
            _chkUseVersioning.Checked = false;
        }
        #endregion

        #region ImportDocument Button Load
        protected void _btnImportDocument_Click(object sender, EventArgs e)
        {
            try
            {
                ResetImportDocControls();
                _editMode = false;
                ShowModalpopup = true;
            }
            catch(Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }
        #endregion
    }
}
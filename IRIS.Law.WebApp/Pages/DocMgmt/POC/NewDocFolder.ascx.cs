using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using IRIS.Law.WebApp.Common; 

namespace IRIS.Law.WebApp.UserControls
{
    public partial class NewDocFolder : System.Web.UI.UserControl
    {  
        #region EnableNewFolder
        public bool EnableNewFolder
        {
            set
            {
                _btnNewFolder.Enabled = value;
            }
        }
        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public delegate void MatterFolderAdded(object sender, EventArgs e);

        public event MatterFolderAdded FolderAdded;
        protected virtual void OnFolderAdded(EventArgs e)
        {
            if (FolderAdded != null)
            {
                FolderAdded(this, e);
            }
        }

        protected void _btnOpenFolderControl_Click(object sender, EventArgs e)
        {
            _PnlNewFolderControls.Visible = true;
        }

        protected void _btnCancel_Click(object sender, EventArgs e)
        {
            _lblError.Text = string.Empty;
            _txtFolderName.Text = string.Empty;
            _PnlNewFolderControls.Visible = false; 
        }

        protected void _btnNewDocFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (_txtFolderName.Text.Trim() != string.Empty)
                {
                    WSDocManagement.DocSvc docSvc = new WSDocManagement.DocSvc();
                    bool FolderAlreadyExists = false;

                    foreach (DataRow _Dr in docSvc.GetFoldersByMatter(Session[SessionName.ProjectId].ToString().Trim()).Tables[0].Rows)
                    {
                        if (_Dr["DocAttributeValue"].ToString().Trim().ToLower() == _txtFolderName.Text.Trim().ToLower())
                        {
                            FolderAlreadyExists = true;
                            break;
                        }
                    }

                    if (!FolderAlreadyExists)
                    {
                        // Save PlaceHolder (Folder)
                        docSvc.SaveToDocTable(0, 1, "Placeholder_Folder.msf", 1, "PLACEHOLDER_FOLDER", string.Empty, null, false, DateTime.Now, DateTime.Now, false, false, false);

                        int NewFolderDocId = docSvc.GetNewestFolderDocId();

                        // Save Doc Attributes 
                        docSvc.SaveToDocAttributes(NewFolderDocId, "1", 5);
                        docSvc.SaveToDocAttributes(NewFolderDocId, _txtFolderName.Text.Trim(), 20);

                        // Save Project Map Docs
                        docSvc.InsertProjectMapDocs(NewFolderDocId, Session[SessionName.ProjectId].ToString().Trim());

                        _txtFolderName.Text = string.Empty;
                        _lblError.Text = "Folder has been successfully created.";

                        if (FolderAdded != null)
                        {
                            OnFolderAdded(e);
                        }
                    }
                    else { _lblError.Text = "This matter already contains a folder with this name."; }
                    
                }
                else { _lblError.Text = "Please enter a valid folder name."; }
                
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebApp.Common;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.IO;
using IRIS.Law.WebApp.DocumentManagement;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using WSDocManagement; 

namespace IRIS.Law.WebApp.DocumentManagement.POC
{
    public partial class DocumentFolders : System.Web.UI.UserControl
    {

        public string SelectedDocID
        {
            get { return _TViewFolders.SelectedValue.ToString(); }
        }

        public void ReloadFolders()
        {
            CheckForProjectId();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckForProjectId();
            }  
        }


        public delegate void MatterFolderSelected(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the Folder is selected.
        /// </summary>
        public event MatterFolderSelected FolderSelected;
        protected virtual void OnFolderSelected(EventArgs e)
        {
            if (FolderSelected != null)
            {
                FolderSelected(this, e);
            }
        } 

        protected void _TViewFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (_TViewFolders.SelectedNode.Text.Trim() != "Documents")
            {
                Session["SelectedFolder"] = _TViewFolders.SelectedNode.Value.Trim();
            }
            else { Session["SelectedFolder"] = null; }
            
            if (FolderSelected != null)
            {
                OnFolderSelected(e);
            }
        }

        private void CheckForProjectId()
        {
            _LblStatus.Text = string.Empty;
            _LblStatus.Visible = false; 

            if (Session[SessionName.ProjectId] != null)
            {
                string _ProjectId = Session[SessionName.ProjectId].ToString();

                WSDocManagement.DocSvc docSvc = new WSDocManagement.DocSvc();
                DataSet _DsFolders = docSvc.GetFoldersByMatter(_ProjectId);
                 
                if (_TViewFolders.Nodes.Count > 0) { _TViewFolders.Nodes.Clear(); }

                TreeNode _Tn = new TreeNode("Documents");
                _TViewFolders.Nodes.Add(_Tn);

                if (_DsFolders.Tables[0].Rows.Count != 0)
                {   
                    foreach (DataRow _Dr in _DsFolders.Tables[0].Rows)
                    {
                        string DocAttValue = _Dr["DocAttributeValue"].ToString().Trim();

                        //_Tn = new TreeNode(DocAttValue + "  (" + docSvc.GetFileByMatter(Session[SessionName.ProjectId].ToString(), DocAttValue).Tables[0].Rows.Count + ")");
                        _Tn = new TreeNode(DocAttValue);
                        _Tn.Value = _Dr["DocAttributeValue"].ToString().Trim();

                        _TViewFolders.Nodes[0].ChildNodes.Add(_Tn);

                        if (Session["SelectedFolder"] != null)
                        {
                            if (Session["SelectedFolder"].ToString().Trim() == _Dr["DocAttributeValue"].ToString().Trim())
                            {
                                _TViewFolders.Nodes[0].ChildNodes[_TViewFolders.Nodes[0].ChildNodes.Count - 1].Selected = true;
                            }
                        }
                    }

                    _TViewFolders.Nodes[0].Expand(); 
                } 
            }


            //if (Session[SessionName.ProjectId] != null)
            //{
            //    string ProjectId = Session[SessionName.ProjectId].ToString().Trim();

            //    _LblStatus.Text = "Project (" + ProjectId + ") has been selected!";
            //}
        }

    }
}
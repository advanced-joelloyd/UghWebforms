using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.Common;
using IRIS.Law.PmsCommonData;
using IRIS.Law.WebServiceInterfaces.Document;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using System.Data; 

namespace IRIS.Law.WebApp.DocumentManagement.POC
{
    public partial class ViewMatterHistory : System.Web.UI.Page
    {
        public List<string> ApprovedFiles()
        {
            WSDocManagement.DocSvc docSvc = new WSDocManagement.DocSvc();
            string Folder = "";
            string _ProjectId = Session[SessionName.ProjectId].ToString();
            if (Session["SelectedFolder"] != null) { Folder = Session["SelectedFolder"].ToString().Trim(); }

            DataSet _Ds = docSvc.GetFileByMatter(_ProjectId, Folder);

            List<string> ApprovedDocIds = new List<string>();

            foreach (DataRow _Dr in docSvc.GetFileByMatter(_ProjectId, Folder).Tables[0].Rows)
            {
                if (_Dr["DocFileName"].ToString().Trim() != "Placeholder_Folder.msf")
                {
                    ApprovedDocIds.Add(_Dr["DocFileName"].ToString().Trim());
                }
            }

            return ApprovedDocIds;
        }

        #region GetMatterDocuments
        /// <summary>
        /// Gets the documents for the matter.
        /// </summary>
        public DocumentSearchItem[] GetMatterDocuments(int startRow, int pageSize)
        {
            DocumentServiceClient documentService = new DocumentServiceClient();
            DocumentSearchItem[] matterDocs = null;
            try
            {
                Guid logonId = ((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId;
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = startRow;
                collectionRequest.RowCount = pageSize;
                collectionRequest.ForceRefresh = true;

                if (Session[SessionName.ProjectId] != null)
                {
                    DocumentSearchReturnValue returnValue = documentService.MatterDocumentSearch(logonId, (Guid)Session[SessionName.ProjectId], "");
                    if (returnValue != null)
                    {
                        if (returnValue.Success)
                        {
                            if (returnValue.Document != null)
                            {
                                List<DocumentSearchItem> _Dsi = new List<DocumentSearchItem>();

                                List<string> AppFiles = ApprovedFiles();

                                foreach (DocumentSearchItem _Dr in returnValue.Document.Rows) { if (AppFiles.Contains(_Dr.FileName.Trim())) { _Dsi.Add(_Dr); } }
                                    
                                matterDocs = _Dsi.ToArray();
                                //_LblRowCount.Text = _Dsi.Count.ToString();
                                //matterDocs = returnValue.Document.Rows;
                                //_matterDocumentRowCount = returnValue.Document.Rows.Length;
                            }
                        }
                        else
                        {
                            throw new Exception(returnValue.Message);
                        }
                    }
                }
                else
                {
                    throw new Exception("No Project Id found.");
                }
                return matterDocs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (documentService != null)
                {
                    documentService.Close();
                }
            }
        }
        #endregion
 

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack) 
            {
                if (Session["SelectedFolder"] != null && Request.QueryString["ReUploadDoc"] == null && Request.QueryString["EditDoc"] == null)
                {
                    Session["SelectedFolder"] = null;  
                }
                else
                {
                    //DocumentFiles1.Rebind();
                }
            }
            //NewFolder1.EnableNewFolder = true;
            DocumentImport1.SetButtonEnabled(true);
            //_searchDocFiles.EnableSearchDocument = true;

            //if logonId not present redirect to Login page
            if (Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }

            _lblError.CssClass = "errorMessage";
            _lblError.Text = string.Empty;

            //ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(_grdDocFiles);

            if (!IsPostBack)
            {
                if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                {
                    if ((Guid)Session[SessionName.MemberId] != DataConstants.DummyGuid || (Guid)Session[SessionName.OrganisationId] != DataConstants.DummyGuid)
                    {
                        try
                        {
                            DocumentImport1.SetButtonEnabled(true);
                            NewFolder1.EnableNewFolder = true;
                            _searchDocFiles.EnableSearchDocument = true;
                            _btnDelFolder.Enabled = true;
                            _cliMatDetails.IsClientMember = true;

                            // If Member is Organisation, then set IsClientMember to false
                            if (new Guid(Session[SessionName.OrganisationId].ToString()) != DataConstants.DummyGuid)
                            {
                                _cliMatDetails.IsClientMember = false;
                            }
                            _cliMatDetails.LoadClientMatterDetails(); 
                        }
                        catch (Exception ex)
                        {
                            _lblError.Text = ex.Message;
                        }
                    }
                }
            }
        }

        protected void _ReUploadDoc_DocReUploaded(object sender, EventArgs e)
        {
            DocumentFiles1.Rebind();
        }

        protected void DocumentFiles1_MatterItemReUpload(object sender, EventArgs e)
        {
            Response.Redirect("ViewMatterHistory.aspx?ReUploadDoc=true");
            //_ReUploadDoc.PanelVisibility(true); 
        }

        protected void DocumentImport1_ItemChanged(object sender, EventArgs e)
        {
            DocumentFolders.ReloadFolders();
            DocumentFiles1.Rebind();
        }

        protected void DocumentFiles1_MatterItemSelected(object sender, EventArgs e)
        {
            Response.Redirect("ViewMatterHistory.aspx?EditDoc=true");
            //DocumentImport1.ShowPanel(true, true);
        }
 
        protected void NewFolder1_MatterFolderAdded(object sender, EventArgs e)
        {
            DocumentFolders.ReloadFolders();
        }

        protected void DocumentFolders_MatterFolderChanged(object sender, EventArgs e)
        {
            DocumentFiles1.Rebind();
        }

        protected void _cliMatDetails_MatterChanged(object sender, EventArgs e)
        {
            try
            {
                _lblError.Text = "";

                Session["SelectedFolder"] = null;
  
                DocumentFolders.ReloadFolders();
                DocumentFiles1.Rebind(); 

                if (Session[SessionName.ProjectId] == null)
                {
                    DocumentImport1.SetButtonEnabled(false);
                    NewFolder1.EnableNewFolder = false;
                    _searchDocFiles.EnableSearchDocument = false;
                    _btnDelFolder.Enabled = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "test1", "$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').attr('disabled', true);", true);

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
                    _searchDocFiles.EnableSearchDocument = true;
                    DocumentImport1.SetButtonEnabled(true);
                    _btnDelFolder.Enabled = true;
                    NewFolder1.EnableNewFolder = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "test1", "$('#" + _searchDocFiles.SearchForDocumentButtonClientID + "').attr('disabled', false);", true); 
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
        }

        protected void _btnRefresh_Click(object sender, EventArgs e)
        {
            //Rebind TreeView and DocGrid
            DocumentFolders.ReloadFolders();
            DocumentFiles1.Rebind();
        }

        protected void _btnDelFolder_Click(object sender, EventArgs e)
        { 
            try
            {
                WSDocManagement.DocSvc docSvc = new WSDocManagement.DocSvc();
                docSvc.DeleteFolder(DocumentFolders.SelectedDocID, Session[SessionName.ProjectId].ToString());
                // DocumentFolders.ReloadFolders();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RefreshFilesOnly()
        {
            DocumentFiles1.Rebind();
        }
    }
}

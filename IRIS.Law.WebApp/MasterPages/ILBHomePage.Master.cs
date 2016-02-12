using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using System.Web;
using NLog;

namespace IRIS.Law.WebApp.MasterPages
{
    public partial class ILBHomePage : System.Web.UI.MasterPage
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private LogonReturnValue _logonSettings;
        #region Public Properties

        /// <summary>
        /// Gets or sets the text disaplyed in the page header.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            get
            {
                return _lblHeaderText.Text;
            }
            set
            {
                _lblHeaderText.Text = value;
            }
        }


        public void ShowHideAJAXProgress(bool show)
        {
            _pnlUpdateProgress.Visible = show;

        }

        #endregion

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            // If Session is expired, do not execute further code.
            if (Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
            } 


            if (!IsPostBack)
            {
                _lblUsername.Text = "User : " + Convert.ToString(Session[SessionName.LogonName]);
                DisplayClientMatterDetailsInContext();
            }

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            _lnkChangePassword.NavigateUrl = IRIS.Law.WebApp.App_Code.AppFunctions.GetASBaseUrl() + "/Pages/Password/ChangePassword.aspx";
            
            if (_logonSettings.UserType == (int)DataConstants.UserType.Staff)
            {
                _lnkChangePassword.Visible = false;
            }

            this.LegalHomeLink.HRef = Solicitors.Branding.Strings.LawBusinessHomePageUrl;
            this.LegalHomeLink.InnerText = string.Format("{0} Home", Solicitors.Branding.Strings.DivisionName);

            this._HypLnkFeedback.Text = string.Format("Feedback to {0}", Solicitors.Branding.Strings.DivisionName);

            this.LegalCopyrightLink.HRef = Solicitors.Branding.Strings.LawBusinessHomePageUrl;
            this.LegalCopyrightLink.InnerText = Solicitors.Branding.Strings.CopyrightNotice;
        }
        #endregion
 
        #region Function GetWizardNodesFromSiteConfigXML
        private System.Xml.XmlNodeList GetWizardNodesFromSiteConfigXML()
        {
            try
            {
                String strSiteConfigXMLFilePath = String.Empty;
                System.Xml.XmlDocument objConfigDoc = new System.Xml.XmlDocument();
                strSiteConfigXMLFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["SiteXMLConfigFilePath"];

                if (System.IO.File.Exists(strSiteConfigXMLFilePath))
                {
                    objConfigDoc.Load(strSiteConfigXMLFilePath);

                    System.Xml.XmlNode rootConfig = objConfigDoc.DocumentElement;
                    return rootConfig.SelectNodes("wizard");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region AddSubMenusToMainMenuConfiguration
        private void AddSubMenusToMainMenuConfiguration(XmlNode mainMenuNode, XmlDocument userPermission)
        {
            try
            {
                System.Xml.XmlNodeList wizardNodeList = GetWizardNodesFromSiteConfigXML();
                int intWizStep = 0;
                if (wizardNodeList != null)
                {
                    foreach (System.Xml.XmlNode wizardNode in wizardNodeList)
                    {
                        XmlElement xmlelem = userPermission.CreateElement("subMenuItem");
                        xmlelem.SetAttribute("displayText", wizardNode.SelectSingleNode("wizardname").InnerText);

                        //This is used if From ViewStyle.aspx, if Edit link is clicked, 
                        // and then if user clicks on 'Menu' link on submenu, the querystring Edit=1 should be appended
                        // This querystring is neccessary, to display if ChangeStyle is to be seen in edit mode or add mode
                        string editQuery = string.Empty;
                        if(!string.IsNullOrEmpty(Convert.ToString( Request.QueryString["Edit"])))
                        {
                            editQuery = "&Edit=" + Convert.ToString( Request.QueryString["Edit"]);
                        }
                        xmlelem.SetAttribute("link", "~/Pages/SiteConfig/ChangeStyle.aspx?WizardActiveIndex=" + intWizStep + editQuery);
                        xmlelem.SetAttribute("users", "1");
                        xmlelem.SetAttribute("enabled", "true");
                        xmlelem.SetAttribute("display", "true");
                        mainMenuNode.AppendChild(xmlelem);
                        intWizStep += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
 
        #region DisplayClientMatterDetailsInContext
        /// <summary>
        /// This will display Client Matter Description on Header
        /// This is made public, so that all content page can able to change Header Text, if Client/Matter is changed
        /// </summary>
        public void DisplayClientMatterDetailsInContext()
        {
            try
            {
                _lblHeaderText.Text = string.Empty;
                // Display Client Matter Information on Header
                if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                {
                    if ((Guid)Session[SessionName.MemberId] != DataConstants.DummyGuid || (Guid)Session[SessionName.OrganisationId] != DataConstants.DummyGuid)
                    {
                        if (Session[SessionName.ClientName] != null)
                        {
                            if (Convert.ToString(Session[SessionName.ClientName]).Trim().Length > 0)
                            {
                                string editClientUrl = ResolveClientUrl("~/Pages/Client/EditClient.aspx");
                                string clientName = Convert.ToString(Session[SessionName.ClientName]).Trim();
                                string tooltip = string.Empty;
                                //truncate large client names and display a tooltip
                                if (clientName.Length > 40)
                                {
                                    tooltip = clientName;
                                    clientName = clientName.Substring(0, 37) + "...";
                                }

                                _lblHeaderText.Text = string.Format("<b> Client : </b><a href='{0}' title='{1}' runat='server'>{2}</a>", editClientUrl,
                                                                                                                            tooltip,
                                                                                                                            clientName);
                            }
                        }

                        if (Session[SessionName.ProjectId] != null && _lblHeaderText.Text.Trim().Length > 0)
                        {
                            if (Session[SessionName.MatterDesc] != null)
                            {
                                if (Convert.ToString(Session[SessionName.MatterDesc]).Trim().Length > 0)
                                {
                                    string editMatterUrl = ResolveClientUrl("~/Pages/Matter/EditMatter.aspx");
                                    string matterDesc = Convert.ToString(Session[SessionName.MatterDesc]).Trim();
                                    string tooltip = string.Empty;
                                    //truncate large descriptions and display a tooltip
                                    if (matterDesc.Length > 70)
                                    {
                                        tooltip = matterDesc;
                                        matterDesc = matterDesc.Substring(0, 67) + "...";
                                    }

                                    _lblHeaderText.Text += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Matter : </b><a href='{0}' title='{1}' runat='server'>{2}</a>", editMatterUrl,
                                                                                                                                                                               tooltip,
                                                                                                                                                                               matterDesc);
                                }
                            }
                        }
                    }
                }
            }
            catch (TimeoutException ex)
            {
                _logger.ErrorException("Timed out trying to display Client/Matter details in Context", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not display Client/Matter details in Context", ex);
                throw;
            }
        }
        #endregion

        #region Logout

        protected void _lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx", true);
        }

        #endregion

        #region GetDBVersion
        public string GetDBVersion()
        {
            const string DBVERSION_SESSIONVALUE = "DBVERSION";
            string dbVersion = string.Empty;

            if (Session[DBVERSION_SESSIONVALUE] == null)
            {
                dbVersion = new UtilitiesServiceClient().GetDBVersion(_logonSettings.LogonId).DBVersion;
                Session[DBVERSION_SESSIONVALUE] = dbVersion;
            }
            else
            {
                dbVersion = Session[DBVERSION_SESSIONVALUE].ToString();
            }

            return dbVersion;
        }
        #endregion GetDBVersion
    }
}

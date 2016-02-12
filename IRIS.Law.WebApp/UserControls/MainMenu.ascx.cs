using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Configuration;
using System.Diagnostics;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class MainMenu : System.Web.UI.UserControl
    {
        private LogonReturnValue _logonSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];
        }

        protected static string GetASBaseUrl()
        {
            string virtualDirectory = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;

            string baseUrl = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority,
                  virtualDirectory);

            return baseUrl;
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                if (!IsPostBack && Session[SessionName.LogonSettings] != null)
                {
                    CreateMenu();
                }
            }
            catch (Exception ex)
            {
                // TODO 
            }
            base.OnPreRender(e);
        }

        #region CreateMenu
        /// <summary>
        /// Creates the menu for the user based on their permissions.
        /// </summary>
        private void CreateMenu()
        {
            //Load the xml file with the permissions
            XmlDocument userPermission = new XmlDocument();
            try
            {
                userPermission.Load(Server.MapPath("~/UserPermission.xml"));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            bool isPageFound = false;
            bool SubMenuItems = false;
            _LitMenu.Text += "<ul id='qm0' class='qmmc'> " + Environment.NewLine;

            if (userPermission.HasChildNodes)
            {
                int userType = ((LogonReturnValue)Session[SessionName.LogonSettings]).UserType;

                if (userPermission.DocumentElement.HasChildNodes)    //Check for menu items
                {
                    foreach (XmlNode mainMenuNode in userPermission.DocumentElement.ChildNodes)
                    {
                        if (mainMenuNode.NodeType != XmlNodeType.Comment)
                        {
                            if (CheckUserPermission(mainMenuNode.Attributes["users"].Value, userType))
                            {
                                List<XmlNode> accessiblePages = GetAccessiblePagesForUser(mainMenuNode, userType);

                                if (accessiblePages.Count > 0) { SubMenuItems = true; }

                                if (mainMenuNode.Attributes["enabled"].Value.ToUpper() == true.ToString().ToUpper())
                                {
                                    if (mainMenuNode.Attributes["displayText"].Value == "Diary" && !((LogonReturnValue)Session[SessionName.LogonSettings]).IsUsingILBDiary)
                                    {
                                        SubMenuItems = false;
                                    }
                                    else
                                    {
                                        if ((mainMenuNode.Attributes["displayText"].Value == "Site Styling" || mainMenuNode.Attributes["displayText"].Value == "Site Configuration") && !((LogonReturnValue)Session[SessionName.LogonSettings]).WebMaster)
                                        {
                                            SubMenuItems = false;
                                        }
                                        else
                                        {
                                            if (mainMenuNode.Attributes["link"] == null)
                                            {
                                                _LitMenu.Text += "<li class='qmparent'><a href='#'>" + mainMenuNode.Attributes["displayText"].Value + "</a> " + Environment.NewLine;
                                            }
                                            else
                                            {
                                                _LitMenu.Text += "<li class='qmparent'><a href='" + GetASBaseUrl() + mainMenuNode.Attributes["link"].Value + "'>" + mainMenuNode.Attributes["displayText"].Value + "</a> " + Environment.NewLine;
                                            }
                                        }
                                    }
                                }

                                if (SubMenuItems)
                                {
                                    //Create the sub menu items
                                    _LitMenu.Text += "<ul> " + Environment.NewLine;

                                    foreach (XmlNode subMenuNode in accessiblePages)
                                    {
                                        if (mainMenuNode.Attributes["enabled"].Value.ToUpper() == true.ToString().ToUpper()
                                            && subMenuNode.Attributes["enabled"].Value.ToUpper() == true.ToString().ToUpper())
                                        {
                                            if (subMenuNode.Attributes["display"] == null)
                                            {
                                                //if (subMenuNode.Attributes["link"].Value != "#")
                                                //{
                                                //    _LitMenu.Text += "<li><a href='" + GetASBaseUrl() + subMenuNode.Attributes["link"].Value + "'>" + subMenuNode.Attributes["displayText"].Value + "</a></li> " + Environment.NewLine;
                                                //}
                                                //else
                                                //{
                                                //    _LitMenu.Text += "<li><a href='#'>" + subMenuNode.Attributes["displayText"].Value + "</a></li> " + Environment.NewLine;
                                                //}

                                                if (subMenuNode.Attributes["link"].Value != "#")
                                                {
                                                    _LitMenu.Text += "<li><a href='" + GetASBaseUrl() + subMenuNode.Attributes["link"].Value + "'>" + subMenuNode.Attributes["displayText"].Value + "</a></li> " + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    _LitMenu.Text += "<li><a href='#'>" + subMenuNode.Attributes["displayText"].Value + "</a></li> " + Environment.NewLine;
                                                }
                                            }
                                        }
                                    }

                                    // Check for additional Admin Controls
                                    if (mainMenuNode.Attributes["displayText"].Value == "Site Styling")
                                    {
                                        if (((LogonReturnValue)Session[SessionName.LogonSettings]).WebMaster)
                                        {
                                            AddSubMenusToMainMenuConfiguration();
                                        }
                                    }

                                    _LitMenu.Text += "</ul></li> " + Environment.NewLine;
                                }
                                else
                                {
                                    _LitMenu.Text += "</li> " + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            _LitMenu.Text += "<li class='qmclear'>&nbsp;</li></ul> " + Environment.NewLine;
            _LitMenu.Text += "</ul>";

            //if we haven't found the requested page in the menu means the user does not have rights
            //to view the current page. Take appropriate action
            if (isPageFound == false)
            {
                //Response.Redirect("~/Login.aspx?AccessDenied=1");
            }
        }
        #endregion

        #region AddSubMenusToMainMenuConfiguration
        private void AddSubMenusToMainMenuConfiguration()
        {
            try
            {
                string editQuery = string.Empty;

                System.Xml.XmlNodeList wizardNodeList = GetWizardNodesFromSiteConfigXML();
                int intWizStep = 0;
                if (wizardNodeList != null)
                {
                    foreach (System.Xml.XmlNode wizardNode in wizardNodeList)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["Edit"])))
                        {
                            editQuery = "&Edit=" + Convert.ToString(Request.QueryString["Edit"]);
                        }

                        _LitMenu.Text += "<li><a href='" + GetASBaseUrl() + "/Pages/SiteConfig/ChangeStyle.aspx?WizardActiveIndex=" + intWizStep + editQuery + "'>" + wizardNode.SelectSingleNode("wizardname").InnerText + "</a></li> " + Environment.NewLine;
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

        #region Function GetWizardNodesFromSiteConfigXML
        private System.Xml.XmlNodeList GetWizardNodesFromSiteConfigXML()
        {
            try
            {
                String strSiteConfigXMLFilePath = String.Empty;
                System.Xml.XmlDocument objConfigDoc = new System.Xml.XmlDocument();
                strSiteConfigXMLFilePath = Server.MapPath("~") + @"\" + ConfigurationSettings.AppSettings["SiteXMLConfigFilePath"];

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


        #region CheckForSelectedLink
        /// <summary>
        /// Checks if the current link is the selected link.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private bool CheckForSelectedLink(string url)
        {
            string currentPg = AppFunctions.GetPageNameByUrl(Request.Url.AbsolutePath);
            return url.Contains(currentPg);
        }
        #endregion

        #region GetAccessiblePagesForUser
        /// <summary>
        /// Gets the list of accessible pages for user.
        /// </summary>
        /// <param name="mainMenuNode">The main menu node.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        private List<XmlNode> GetAccessiblePagesForUser(XmlNode mainMenuNode, int userType)
        {
            List<XmlNode> accessiblePages = new List<XmlNode>();
            foreach (XmlNode childNode in mainMenuNode)
            {
                //if (childNode.NodeType != XmlNodeType.Comment)
                //{
                //    if (CheckUserPermission(childNode.Attributes["users"].Value, userType))
                //    {
                //        accessiblePages.Add(childNode);
                //    }
                //}


                if (childNode.NodeType != XmlNodeType.Comment)
                {
                    Debug.WriteLine("-" + childNode.Attributes["link"].Value);

                    if (CheckUserPermission(childNode.Attributes["users"].Value, userType))
                    {

                        if (childNode.Attributes != null && childNode.Attributes["link"] != null)
                        {

                            //Debug.WriteLine("-" + childNode.Attributes["link"].Value);

                            switch (childNode.Attributes["link"].Value)
                            {
                                case "/Pages/Task/TaskDetails.aspx":
                                    if (_logonSettings.CanAddTask)
                                        accessiblePages.Add(childNode);
                                    break;
                                default:
                                    accessiblePages.Add(childNode);
                                    break;
                            }
                        }
                        else
                        {
                            accessiblePages.Add(childNode);
                        }

                    }
                }
            }
            return accessiblePages;
        }
        #endregion

        #region CheckUserPermission
        /// <summary>
        /// Checks if the user has permission to view the page.
        /// </summary>
        /// <param name="validUserTypes">The valid user types.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        private bool CheckUserPermission(string validUserTypes, int userType)
        {
            bool hasPermission = false;
            string[] validUsers = validUserTypes.Split(',');
            foreach (string user in validUsers)
            {
                if (user == userType.ToString())
                {
                    hasPermission = true;
                    break;
                }
            }
            return hasPermission;
        }
        #endregion

    }
}
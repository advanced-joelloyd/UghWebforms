using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI.HtmlControls;
using System.IO;
using ColorExtender;
using IRIS.Law.WebApp.App_Code;
using System.Xml;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.Pages.SiteConfig
{
    public partial class ChangeStyle : BasePage
    {
        private String[] _strImagesArray;
        private bool isSaved = false;
        private bool isPreviewed = false;
        private string strFileName;
        private string imgPath = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {  
            _lblHeaderCSSFile.Text = string.Empty;

            AddWizardSteps();

            if (Session[SessionName.StyleSheet] != null)
            {
                if (!File.Exists(Convert.ToString(Session[SessionName.StyleSheet])))
                {
                    Session[SessionName.StyleSheet] = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                }
            }



            Button _btnStartSave = (Button)_wizardStyle.FindControl("StartNavigationTemplateContainerID").FindControl("_btnStartSave");
            Button _btnStepSave = (Button)_wizardStyle.FindControl("StepNavigationTemplateContainerID").FindControl("_btnStepSave");
            Button _btnFinishSave = (Button)_wizardStyle.FindControl("FinishNavigationTemplateContainerID").FindControl("_btnFinishSave");

            _btnStartSave.Enabled = false;
            _btnStepSave.Enabled = false;
            _btnFinishSave.Enabled = false;

            if (Convert.ToString(Request.QueryString["Edit"]) == "1")
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Session[SessionName.StyleSheetToBeEdited])))
                {
                    Button _btnSaveChangedCSS1 = (Button)_wizardStyle.FindControl("StartNavigationTemplateContainerID").FindControl("_btnSaveChangedCSS1");
                    Button _btnSaveChangedCSS2 = (Button)_wizardStyle.FindControl("StepNavigationTemplateContainerID").FindControl("_btnSaveChangedCSS2");
                    Button _btnSaveChangedCSS3 = (Button)_wizardStyle.FindControl("FinishNavigationTemplateContainerID").FindControl("_btnSaveChangedCSS3");

                    _btnSaveChangedCSS1.Enabled = false;
                    _btnSaveChangedCSS2.Enabled = false;
                    _btnSaveChangedCSS3.Enabled = false;

                    _btnStartSave.Enabled = true;
                    _btnStepSave.Enabled = true;
                    _btnFinishSave.Enabled = true;

                    string editableCss = Convert.ToString(Session[SessionName.StyleSheetToBeEdited]);
                    editableCss = editableCss.Replace(@"\\","/").Replace(@"\", "/");
                    _lblHeaderCSSFile.CssClass = "successMessage";
                    _lblHeaderCSSFile.Text = "You are currently editing style '" + editableCss.Substring(editableCss.LastIndexOf("/") + 1) + "'";
                }
            }

            Button _btnToPostback = (Button)_wizardStyle.FindControl("StartNavigationTemplateContainerID").FindControl("_btnApply1");
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(_btnToPostback);

            Button _btnToPostback1 = (Button)_wizardStyle.FindControl("StepNavigationTemplateContainerID").FindControl("_btnApply2");
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(_btnToPostback1);

            Button _btnToPostback2 = (Button)_wizardStyle.FindControl("FinishNavigationTemplateContainerID").FindControl("_btnApply3");
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(_btnToPostback2);

            Button _btnToPostback3 = (Button)_wizardStyle.FindControl("StartNavigationTemplateContainerID").FindControl("_btnSaveChangedCSS1");
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(_btnToPostback3);

            if (!IsPostBack)
            {
                if (Request.QueryString["WizardActiveIndex"].Length > 0)
                {
                    _wizardStyle.ActiveStepIndex = Convert.ToInt16(Request.QueryString["WizardActiveIndex"]);
                }
            }
        }

        #region Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!((LogonReturnValue)Session[SessionName.LogonSettings]).WebMaster)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);

                return;
            }
        }
        #endregion

        #region AddWizardSteps
        private void AddWizardSteps()
        {
            try
            {
                Int16 counter = 0;
                String strCSSFilePath = "";
                if (Session[SessionName.StyleSheet] == null)
                {
                    strCSSFilePath = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                }
                else
                {
                    strCSSFilePath = Convert.ToString(Session[SessionName.StyleSheet]);
                }

                System.Xml.XmlNodeList wizardNodeList = GetWizardNodesFromSiteConfigXML();
                System.Xml.XmlNodeList classNodeList;
                if (wizardNodeList != null)
                {
                    foreach (System.Xml.XmlNode wizardNode in wizardNodeList)
                    {
                        WizardStep wizStep = new WizardStep();
                        wizStep.Title = wizardNode.SelectSingleNode("wizardname").InnerText;
                        wizStep.ID = wizardNode.SelectSingleNode("wizardname").InnerText;

                        Table tblWizHeader = new Table();
                        TableRow tblRowHeader = new TableRow();
                        TableCell tblCell = new TableCell();
                        tblCell.CssClass = "sectionHeader";

                        Label _lblText = new Label();
                        _lblText.Text = wizardNode.SelectSingleNode("wizardname").InnerText;
                        tblCell.Controls.Add(_lblText);

                        TableRow tblBlankRow = new TableRow();
                        TableCell tblBlankCell = new TableCell();
                        tblBlankCell.Text = "";
                        tblBlankRow.Cells.Add(tblBlankCell);

                        tblRowHeader.Cells.Add(tblCell);
                        tblWizHeader.Rows.Add(tblRowHeader);
                        tblWizHeader.Rows.Add(tblBlankRow);
                     
                        tblWizHeader.Style["Width"] = "100%";
                        tblWizHeader.ID = "_tblWizHeader_" + wizardNode.SelectSingleNode("wizardname").InnerText;

                        Table tblWizard = new Table();
                        tblWizard.ID = "_tblWizard_" + wizardNode.SelectSingleNode("wizardname").InnerText;
                        tblWizard.Style["Width"] = "100%";
                        classNodeList = wizardNode.SelectNodes("class");
                        AddControlsToWizard(tblWizard, strCSSFilePath, classNodeList);
                        wizStep.Controls.Add(tblWizHeader);
                        wizStep.Controls.Add(tblWizard);

                        _wizardStyle.WizardSteps.Add(wizStep);
                        if (counter == 0)
                        {
                            _wizardStyle.ActiveStepIndex = 0;
                        }
                        counter += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ReloadControlsInWizard
        private void ReloadControlsInWizard(string strCSSFilePath)
        {
            try
            {
                foreach (WizardStep _wizStep in _wizardStyle.WizardSteps)
                {
                    foreach (Control ctrl in _wizStep.Controls)
                    {
                        if (ctrl != null)
                        {
                            if (ctrl.ID != null)
                            {
                                if (ctrl.ID.StartsWith("_tblWizard_"))
                                {
                                    Table _tblMain1 = (Table)ctrl;

                                    _tblMain1.Rows.Clear();

                                    System.Xml.XmlNodeList wizardNodeList = GetWizardNodesFromSiteConfigXML();
                                    System.Xml.XmlNodeList classNodeList;
                                    if (wizardNodeList != null)
                                    {
                                        foreach (System.Xml.XmlNode wizardNode in wizardNodeList)
                                        {
                                            if (wizardNode.SelectSingleNode("wizardname").InnerText == ctrl.ID.Substring(ctrl.ID.LastIndexOf("_") + 1))
                                            {
                                                classNodeList = wizardNode.SelectNodes("class");
                                                AddControlsToWizard(_tblMain1, strCSSFilePath, classNodeList);
                                            }
                                        }
                                    }

                                }
                            }
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

        #region AddControlsToWizard
        private void AddControlsToWizard(Table tblWizard, String strCSSFilePath, System.Xml.XmlNodeList configClassNodeList)
        {
            try
            {
                String strValue = String.Empty;
                System.Xml.XmlDocument objDoc = new System.Xml.XmlDocument();
                System.Xml.XmlDocument objConfigDoc = new System.Xml.XmlDocument();
                _strImagesArray = new string[10];
                Int32 intImageCount = 0;
                if (strCSSFilePath == "" || strCSSFilePath == null)
                {
                    strCSSFilePath = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                }
                

                string Xml = ConvertToXML(strCSSFilePath);

                objDoc.LoadXml(Xml);


                System.Xml.XmlNodeList cssRuleNodeList, cssDeclarationList;
                System.Xml.XmlNode rootCSS = objDoc.DocumentElement;

                cssRuleNodeList = rootCSS.SelectNodes("rule");

                // Loop done if style is to apply only for particular class
                foreach (System.Xml.XmlNode classNode in configClassNodeList)
                {
                    foreach (System.Xml.XmlNode cssRule in cssRuleNodeList)
                    {
                        if (cssRule.SelectSingleNode("selector").InnerText.Replace("ï»¿","") == classNode.SelectSingleNode("classname").InnerText)
                        {
                            TableRow _tblRow = new TableRow();
                            TableCell _tblCell = new TableCell();
                            Label _lblRule = new Label();
                            _lblRule.Text = GetUserDefinedClass(classNode);// +" : ";
                            _lblRule.ID = "_selector" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "");

                            Panel _pnlHeader = new Panel();
                            _pnlHeader.ID = "_pnlHeaderClass_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "");
                            _pnlHeader.Controls.Add(_lblRule);
                            _pnlHeader.CssClass = "bodyTab";
                            _pnlHeader.Style["Width"] = "99.9%";

                            _tblCell.Controls.Add(_pnlHeader);
                            _tblCell.ColumnSpan = 2;

                            _tblRow.Cells.Add(_tblCell);
                            tblWizard.Rows.Add(_tblRow);

                            cssDeclarationList = cssRule.SelectNodes("declaration");
                            foreach (System.Xml.XmlNode cssdeclarationNode in cssDeclarationList)
                            {
                                System.Xml.XmlNodeList configPropertyNodeList;
                                configPropertyNodeList = classNode.SelectNodes("property");
                                foreach (System.Xml.XmlNode configPropertyNode in configPropertyNodeList)
                                {
                                    if (cssdeclarationNode.SelectSingleNode("property").InnerText == configPropertyNode.SelectSingleNode("propertyname").InnerText)
                                    {
                                        TableRow _tblRowDeclaration = new TableRow();
                                        TableCell _tblCellDeclaration = new TableCell();
                                        TableCell _tblCellPropertyValue = new TableCell();

                                        Label _lblDeclarationProperty = new Label();
                                        _lblDeclarationProperty.Text = GetUserDefinedProperty(configPropertyNode) + " : ";
                                        _lblDeclarationProperty.ID = cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText;
                                        _tblCellDeclaration.Controls.Add(_lblDeclarationProperty);
                                        _tblCellDeclaration.CssClass = "boldTxt";
                                        _tblCellDeclaration.Attributes.Add("width", "10%");
                                        _tblCellDeclaration.HorizontalAlign = HorizontalAlign.Right;


                                        if (configPropertyNode.SelectSingleNode("colorpicker").InnerText == "1")
                                        {
                                            ColorExtender.ColorPicker color = new ColorPicker();
                                            color.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_color";
                                            color.Color = cssdeclarationNode.SelectSingleNode("value").InnerText;
                                            color.ForeColor = System.Drawing.Color.Black;
                                            color.CssClass = "textBox";
                                            _tblCellPropertyValue.Controls.Add(color);
                                        }
                                        else if (configPropertyNode.SelectSingleNode("image").InnerText == "1")
                                        {
                                            FileUpload _fileUpload = new FileUpload();
                                            _fileUpload.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_image";
                                            _fileUpload.CssClass = "textBox";
                                            _fileUpload.Style["Width"] = "300px";
                                            _tblCellPropertyValue.Controls.Add(_fileUpload);

                                            String image = cssdeclarationNode.SelectSingleNode("value").InnerText.Replace("url(", "");
                                            image = image.Replace(")", "");
                                            string[] imageNameArray = image.Split('/');
                                            image = imageNameArray[imageNameArray.Length - 1];

                                            HiddenField _hdnImageName = new HiddenField();
                                            _hdnImageName.ID = "_hdnDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_image";
                                            _hdnImageName.Value = image;
                                            _tblCellPropertyValue.Controls.Add(_hdnImageName);

                                            _strImagesArray[intImageCount] = image;
                                            intImageCount += 1;
                                        }
                                        else
                                        {
                                            string strWhat = configPropertyNode.SelectSingleNode("propertyname").InnerText.ToLower();
                                            switch (strWhat)
                                            {
                                                case "font-family":
                                                    DropDownList _ddlDeclarationPropertyValueFontFamliy = new DropDownList();
                                                    BindFontFamily(_ddlDeclarationPropertyValueFontFamliy, cssdeclarationNode.SelectSingleNode("value").InnerText);
                                                    _ddlDeclarationPropertyValueFontFamliy.ID = "_ddlDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_ddl";
                                                    _ddlDeclarationPropertyValueFontFamliy.Attributes.Add("runat", "server");
                                                    _ddlDeclarationPropertyValueFontFamliy.CssClass = "textBox";
                                                    _tblCellPropertyValue.Controls.Add(_ddlDeclarationPropertyValueFontFamliy);
                                                    break;
                                                case "font-size":
                                                    DropDownList _ddlDeclarationPropertyValueFontSize = new DropDownList();
                                                    BindFontSize(_ddlDeclarationPropertyValueFontSize, cssdeclarationNode.SelectSingleNode("value").InnerText);
                                                    _ddlDeclarationPropertyValueFontSize.ID = "_ddlDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_ddl";
                                                    _ddlDeclarationPropertyValueFontSize.Attributes.Add("runat", "server");
                                                    _ddlDeclarationPropertyValueFontSize.CssClass = "textBox";
                                                    _tblCellPropertyValue.Controls.Add(_ddlDeclarationPropertyValueFontSize);
                                                    break;
                                                case "font-weight":
                                                    DropDownList _ddlDeclarationPropertyValueFontWeight = new DropDownList();
                                                    BindFontWeight(_ddlDeclarationPropertyValueFontWeight, cssdeclarationNode.SelectSingleNode("value").InnerText);
                                                    _ddlDeclarationPropertyValueFontWeight.ID = "_ddlDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_ddl";
                                                    _ddlDeclarationPropertyValueFontWeight.Attributes.Add("runat", "server");
                                                    _ddlDeclarationPropertyValueFontWeight.CssClass = "textBox";
                                                    _tblCellPropertyValue.Controls.Add(_ddlDeclarationPropertyValueFontWeight);
                                                    break;
                                                default:
                                                    TextBox _txtDeclarationPropertyValue = new TextBox();
                                                    _txtDeclarationPropertyValue.Text = cssdeclarationNode.SelectSingleNode("value").InnerText;
                                                    _txtDeclarationPropertyValue.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_txt";
                                                    _txtDeclarationPropertyValue.Attributes.Add("runat", "server");
                                                    _txtDeclarationPropertyValue.CssClass = "textBox";
                                                    _tblCellPropertyValue.Controls.Add(_txtDeclarationPropertyValue);
                                                    break;
                                            }
                                        }
                                        _tblCellPropertyValue.Attributes.Add("width", "50%");
                                        _tblCellPropertyValue.HorizontalAlign = HorizontalAlign.Left;

                                        _tblRowDeclaration.Cells.Add(_tblCellDeclaration);
                                        _tblRowDeclaration.Cells.Add(_tblCellPropertyValue);
                                        tblWizard.Rows.Add(_tblRowDeclaration);

                                        if (configPropertyNode.SelectSingleNode("image").InnerText == "1")
                                        {
                                            TableRow _tblRowLabel = new TableRow();
                                            TableCell _tblCellLabel = new TableCell();
                                            TableCell _tblBlank11 = new TableCell();
                                            Label _lblfileName = new Label();
                                            String image = cssdeclarationNode.SelectSingleNode("value").InnerText.Replace("url(", "");
                                            image = image.Replace(")", "");
                                            string[] imageNameArray = image.Split('/');
                                            image = imageNameArray[imageNameArray.Length - 1];
                                            _lblfileName.Text = "(To Refresh, in IE, press (Ctrl + F5) if changed image is not rendered.)";
                                            _lblfileName.ID = "_lblLogoWarning_" + cssRule.SelectSingleNode("selector").InnerText.Replace(":", "") + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_image";
                                            _tblCellLabel.Controls.Add(_lblfileName);

                                            _tblRowLabel.Cells.Add(_tblBlank11);
                                            _tblRowLabel.Cells.Add(_tblCellLabel);
                                            _tblRowLabel.CssClass = "labelValue";
                                            tblWizard.Rows.Add(_tblRowLabel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                if (Session["xmlDoc"] == null)
                {
                    Session["xmlDoc"] = objDoc;
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

        #region BindFontFamily
        private void BindFontFamily(DropDownList ddlFontFamily, string value)
        {
            try
            {
                ListItem itemMix = new ListItem("Arial, Helvetica, sans-serif", "arial, helvetica, sans-serif");
                ListItem itemAerial = new ListItem("Arial", "arial");
                ListItem itemCalibri = new ListItem("Calibri", "calibri");
                ListItem itemCourierNew = new ListItem("Courier New", "courier new");
                ListItem itemTahoma = new ListItem("Tahoma", "tahoma");
                ListItem itemTimes = new ListItem("Times New Roman", "times new roman");
                ListItem itemVerdana = new ListItem("Verdana", "verdana");
                ddlFontFamily.Items.Add(itemMix);
                ddlFontFamily.Items.Add(itemAerial);
                ddlFontFamily.Items.Add(itemCalibri);
                ddlFontFamily.Items.Add(itemCourierNew);
                ddlFontFamily.Items.Add(itemTahoma);
                ddlFontFamily.Items.Add(itemTimes);
                ddlFontFamily.Items.Add(itemVerdana);

                if (!string.IsNullOrEmpty(value))
                {
                    if (ddlFontFamily.Items.FindByValue(value.ToLower()) != null)
                    {
                        ddlFontFamily.Items.FindByValue(value.ToLower()).Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BindFontSize
        private void BindFontSize(DropDownList ddlFontSize, string value)
        {
            try
            {
                for (int i = 8; i <= 18; i++)
                {
                    ddlFontSize.Items.Add(i.ToString());
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (ddlFontSize.Items.FindByValue(value.ToLower().Replace("px", "")) != null)
                    {
                        ddlFontSize.Items.FindByValue(value.ToLower().Replace("px", "")).Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region BindFontWeight
        private void BindFontWeight(DropDownList ddlFontWeight, string value)
        {
            try
            {
                ListItem itemNormal = new ListItem("Normal", "normal");
                ListItem itemBold = new ListItem("Bold", "bold");
                ddlFontWeight.Items.Add(itemNormal);
                ddlFontWeight.Items.Add(itemBold);
                
                if (!string.IsNullOrEmpty(value))
                {
                    if (ddlFontWeight.Items.FindByValue(value.ToLower()) != null)
                    {
                        ddlFontWeight.Items.FindByValue(value.ToLower()).Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region LoadConfig
        private void LoadConfig(String strCSSFilePath)
        {
            try
            {
                String strSiteConfigXMLFilePath = String.Empty;
                String strValue = String.Empty;
                System.Xml.XmlDocument objDoc = new System.Xml.XmlDocument();
                System.Xml.XmlDocument objConfigDoc = new System.Xml.XmlDocument();
                _strImagesArray = new string[10];
                Int32 intImageCount = 0;
                if (strCSSFilePath == "" || strCSSFilePath == null)
                {
                    strCSSFilePath = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                }

                strSiteConfigXMLFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["SiteXMLConfigFilePath"];

                // Convert CSS to XML
                if (System.IO.File.Exists(strCSSFilePath) && System.IO.File.Exists(strSiteConfigXMLFilePath))
                {
                    objDoc.LoadXml(ConvertToXML(strCSSFilePath));
                    objConfigDoc.Load(strSiteConfigXMLFilePath);

                    System.Xml.XmlNodeList cssRuleNodeList, cssDeclarationList;
                    System.Xml.XmlNodeList configClassNodeList, configAllNodeList;
                    System.Xml.XmlNode rootCSS = objDoc.DocumentElement;
                    System.Xml.XmlNode rootConfig = objConfigDoc.DocumentElement;

                    configClassNodeList = rootConfig.SelectNodes("class");
                    configAllNodeList = rootConfig.SelectNodes("all");
                    cssRuleNodeList = rootCSS.SelectNodes("rule");

                    // Loop done if style is to apply only for particular class
                    foreach (System.Xml.XmlNode classNode in configClassNodeList)
                    {
                        foreach (System.Xml.XmlNode cssRule in cssRuleNodeList)
                        {
                            if (cssRule.SelectSingleNode("selector").InnerText == classNode.SelectSingleNode("classname").InnerText)
                            {
                                TableRow _tblRow = new TableRow();
                                TableCell _tblCell = new TableCell();
                                Label _lblRule = new Label();
                                _lblRule.Text = GetUserDefinedClass(classNode) + " : ";
                                _lblRule.ID = "_selector" + cssRule.SelectSingleNode("selector").InnerText;
                                _tblCell.Controls.Add(_lblRule);
                                _tblCell.CssClass = "boldTxt";

                                _tblRow.Cells.Add(_tblCell);
                                _tblMain.Rows.Add(_tblRow);

                                cssDeclarationList = cssRule.SelectNodes("declaration");
                                foreach (System.Xml.XmlNode cssdeclarationNode in cssDeclarationList)
                                {
                                    System.Xml.XmlNodeList configPropertyNodeList;
                                    configPropertyNodeList = classNode.SelectNodes("property");
                                    foreach (System.Xml.XmlNode configPropertyNode in configPropertyNodeList)
                                    {
                                        if (cssdeclarationNode.SelectSingleNode("property").InnerText == configPropertyNode.SelectSingleNode("propertyname").InnerText)
                                        {
                                            TableRow _tblRowDeclaration = new TableRow();
                                            TableCell _tblCellDeclaration = new TableCell();
                                            TableCell _tblCellPropertyValue = new TableCell();

                                            Label _lblBlank = new Label();
                                            _lblBlank.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                            _tblCellDeclaration.Controls.Add(_lblBlank);

                                            Label _lblDeclarationProperty = new Label();
                                            _lblDeclarationProperty.Text = GetUserDefinedProperty(configPropertyNode) + " : ";
                                            _lblDeclarationProperty.ID = cssRule.SelectSingleNode("selector").InnerText + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText;
                                            _tblCellDeclaration.Controls.Add(_lblDeclarationProperty);
                                            _tblCellDeclaration.CssClass = "boldTxt";
                                            _tblCellDeclaration.Attributes.Add("width", "25%");
                                            _tblCellDeclaration.HorizontalAlign = HorizontalAlign.Right;


                                            if (configPropertyNode.SelectSingleNode("colorpicker").InnerText == "1")
                                            {
                                                ColorExtender.ColorPicker color = new ColorPicker();
                                                color.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_color";
                                                color.Color = "Black";
                                                _tblCellPropertyValue.Controls.Add(color);
                                            }
                                            else if (configPropertyNode.SelectSingleNode("image").InnerText == "1")
                                            {
                                                FileUpload _fileUpload = new FileUpload();
                                                _fileUpload.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_image";
                                                _fileUpload.CssClass = "button";
                                                _tblCellPropertyValue.Controls.Add(_fileUpload);

                                                Label _lblfileName = new Label();
                                                String image = cssdeclarationNode.SelectSingleNode("value").InnerText.Replace("url(", "");
                                                image = image.Replace(")", "");
                                                string[] imageNameArray = image.Split('/');
                                                image = imageNameArray[imageNameArray.Length - 1];
                                                _lblfileName.Text = "&nbsp;&nbsp;&nbsp;upload image with same file name '" + image + "' ";
                                                _tblCellPropertyValue.Controls.Add(_lblfileName);

                                                HiddenField _hdnImageName = new HiddenField();
                                                _hdnImageName.ID = "_hdnDeclaration_" + cssRule.SelectSingleNode("selector").InnerText + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_image";
                                                _hdnImageName.Value = image;
                                                _tblCellPropertyValue.Controls.Add(_hdnImageName);

                                                _strImagesArray[intImageCount] = image;
                                                intImageCount += 1;
                                            }
                                            else
                                            {
                                                TextBox _txtDeclarationPropertyValue = new TextBox();
                                                _txtDeclarationPropertyValue.Text = cssdeclarationNode.SelectSingleNode("value").InnerText;
                                                _txtDeclarationPropertyValue.ID = "_txtDeclaration_" + cssRule.SelectSingleNode("selector").InnerText + "_" + cssdeclarationNode.SelectSingleNode("property").InnerText + "_txt";
                                                _txtDeclarationPropertyValue.Attributes.Add("runat", "server");
                                                _tblCellPropertyValue.Controls.Add(_txtDeclarationPropertyValue);
                                            }
                                            _tblCellPropertyValue.Attributes.Add("width", "50%");
                                            _tblCellPropertyValue.HorizontalAlign = HorizontalAlign.Left;

                                            _tblRowDeclaration.Cells.Add(_tblCellDeclaration);
                                            _tblRowDeclaration.Cells.Add(_tblCellPropertyValue);
                                            _tblMain.Rows.Add(_tblRowDeclaration);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Session["xmlDoc"] == null)
                    {
                        Session["xmlDoc"] = objDoc;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Preview Button Event
        protected void _btnPreview_Click(object sender, EventArgs e)
        {


            string strCSSFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"] + "/" + Convert.ToString(ConfigurationSettings.AppSettings["TemperoryCSSFileNameStartWith"]);
            string strUniquePart = Session["LogonName"] + "_" + GetTimeStamp();
            strCSSFilePath = strCSSFilePath + "_" + strUniquePart + ".css";

            string defaultCSS = "";
            //if (Session[SessionName.StyleSheet] == null)
                defaultCSS = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
            //else
            //    defaultCSS = Session[SessionName.StyleSheet].ToString();

            imgPath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["UploadImageFilePath"] + "/logo_default_" + strUniquePart + ".gif";
            //Delete any previously created preview files for the user
            AppFunctions.DeleteUsersPreviewCSS(Session[SessionName.LogonName].ToString(), Server.MapPath("~") + @"\" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"]);

            CopyFile(defaultCSS, strCSSFilePath);
            isPreviewed = true;
            ReadAndSaveUserInputToCSS(strCSSFilePath);
            HighLightWizardSteps(_wizardStyle.ActiveStepIndex);
            Session[SessionName.StyleSheet] = strCSSFilePath;
        }

        /// <summary>
        /// Delete the earlier preview css files/images created for the logged in user
        /// </summary>
        /// 
        
        #endregion
   
        #region GetTimeStamp
        private string GetTimeStamp()
        {
            try
            {
                return DateTime.Now.Ticks.ToString();
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Cancel button event
        protected void _btnStartCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["PreviewLogo"] = null;
                AppFunctions.DeleteUsersPreviewCSS(Session[SessionName.LogonName].ToString(), Server.MapPath("~") + @"\" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"]);
                AppFunctions.DeleteUsersPreviewImages(Session[SessionName.LogonName].ToString(), Server.MapPath("~") + @"\" + ConfigurationSettings.AppSettings["UploadImageFilePath"]);
                Session[SessionName.StyleSheet] = Session[SessionName.UserStyleSheet];

                Response.Redirect("ViewStyle.aspx");
                //Response.Redirect("ChangeStyle.aspx?WizardActiveIndex=" + Convert.ToString(Request.QueryString["WizardActiveIndex"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Save button event
        protected void _btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strCSSFilePath = Convert.ToString(Session[SessionName.StyleSheetToBeEdited]);
                strCSSFilePath = strCSSFilePath.Replace(@"\", "/").Replace(@"\\", "/");

                string srcFilename = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                

                //Check if already the filename exists
                if (!File.Exists(strCSSFilePath))
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "The file '" + strCSSFilePath.Substring(strCSSFilePath.LastIndexOf("/") + 1) + "' cannot be found.";
                    _mpeSaveCSS.Show();
                    return;
                }

                CopyFile(srcFilename, strCSSFilePath);

                // LSC - Rename Logo_Default Image
                RenameImageFile(false);

                isSaved = true;

                string editableCss = Convert.ToString(Session[SessionName.StyleSheetToBeEdited]);
                editableCss = editableCss.Replace(@"\\", "/").Replace(@"\", "/");
                editableCss = editableCss.Substring(editableCss.LastIndexOf("/") + 1);

                strFileName = editableCss.Replace(".css","");
                ReadAndSaveUserInputToCSS(strCSSFilePath);

                HighLightWizardSteps(_wizardStyle.ActiveStepIndex);

                _lblHeaderCSSFile.Text = "Changes " + " saved successfully.";

                Session[SessionName.StyleSheet] = Session[SessionName.StyleSheetToBeEdited];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CopyFile
        private void CopyFile(String srcFile, String destFile)
        {
            try
            {
                File.Copy(srcFile, destFile, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ReadAndSaveUserInputToCSS
        private void ReadAndSaveUserInputToCSS(String strCSSFilePath)
        {
            System.Xml.XmlDocument objDoc = (System.Xml.XmlDocument)Session["xmlDoc"];
            try
            { 
                _lblError.Text = String.Empty;
                System.Xml.XmlNodeList nodeList, declarationList;
                System.Xml.XmlNode root = objDoc.DocumentElement;
                nodeList = root.SelectNodes("rule");
                String selector = String.Empty;
                String property = String.Empty;
                String control = String.Empty;
                foreach (WizardStep _wizStep in _wizardStyle.WizardSteps)
                {
                    foreach (Control ctrl in _wizStep.Controls)
                    {
                        if (ctrl != null)
                        {
                            if (ctrl.ID != null)
                            {
                                if (ctrl.ID.StartsWith("_tblWizard_"))
                                {
                                    Table _tblMain1 = (Table)ctrl;


                                    foreach (TableRow _tblRow in _tblMain1.Rows)
                                    {
                                        if (_tblRow.Cells.Count == 2)
                                        {
                                            if (_tblRow.Cells[1].Controls[0].ID.StartsWith("_txtDeclaration_") || _tblRow.Cells[1].Controls[0].ID.StartsWith("_ddlDeclaration_"))
                                            {
                                                //string[] ruleArr = _tblRow.Cells[1].Controls[0].ID.Split('_');
                                                if (!_tblRow.Cells[1].Controls[0].ID.Contains("xp2"))
                                                {
                                                    string[] ruleArr = _tblRow.Cells[1].Controls[0].ID.Split('_');
                                                    selector = ruleArr[2].Replace(":", "");
                                                    property = ruleArr[3];
                                                    control = ruleArr[4];
                                                }
                                                else
                                                {
                                                    string[] ruleArr = _tblRow.Cells[1].Controls[0].ID.Split('_');
                                                    property = ruleArr[ruleArr.Length - 2];
                                                    control = ruleArr[ruleArr.Length - 1];
                                                    selector = _tblRow.Cells[1].Controls[0].ID.Replace(property,"").Replace(control,"").Replace(ruleArr[1],"");
                                                    selector = selector.Remove(selector.Length - 2, 2);
                                                    selector = selector.Remove(0, 2);
                                                }

                                                foreach (System.Xml.XmlNode rule in nodeList)
                                                {
                                                    if (string.Equals(rule.SelectSingleNode("selector").InnerText.Replace("ï»¿", "").Replace(":", ""), selector.Replace("ï»¿", ""), StringComparison.Ordinal))
                                                    {
                                                        declarationList = rule.SelectNodes("declaration");
                                                        foreach (System.Xml.XmlNode declaration in declarationList)
                                                        {
                                                            if (declaration.SelectSingleNode("property").InnerText == property)
                                                            {
                                                                if (control == "txt")
                                                                {
                                                                    if (((Control)_tblRow.Cells[1].Controls[0]).ID != null)
                                                                    {
                                                                        if (((Control)_tblRow.Cells[1].Controls[0]).ID.StartsWith("_txt"))
                                                                        {
                                                                            
                                                                            declaration.SelectSingleNode("value").InnerText = ((TextBox)_tblRow.Cells[1].Controls[0]).Text;
                                                                        }
                                                                    }
                                                                }
                                                                else if (control == "ddl")
                                                                {
                                                                    if (((Control)_tblRow.Cells[1].Controls[0]).ID != null)
                                                                    {
                                                                        if (((Control)_tblRow.Cells[1].Controls[0]).ID.StartsWith("_ddl"))
                                                                        {                                                                            
                                                                            declaration.SelectSingleNode("value").InnerText = ((DropDownList)_tblRow.Cells[1].Controls[0]).SelectedValue;

                                                                            if (property.ToLower() == "font-size")
                                                                            {
                                                                                declaration.SelectSingleNode("value").InnerText += "px";
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else if (control == "color")
                                                                {
                                                                    declaration.SelectSingleNode("value").InnerText = ((ColorPicker)_tblRow.Cells[1].Controls[0]).Color;
                                                                }
                                                                else if (control == "image")
                                                                {
                                                                    FileUpload _fileUpld = ((FileUpload)_tblRow.Cells[1].Controls[0]);
                                                                    String strHdnfld = "_hdn" + _fileUpld.ID.Substring(4);
                                                                    strHdnfld = ((HiddenField)_tblRow.Cells[1].FindControl(strHdnfld)).Value;

                                                                    //if (_fileUpld.FileName != string.Empty)
                                                                    //    strHdnfld = _fileUpld.FileName;

                                                                    string[] arrFile;
                                                                    string strTempFile = String.Empty;
                                                                    string strReplace = String.Empty;
                                                                    if (isSaved)
                                                                    {
                                                                        if (!strHdnfld.Contains("_" + strFileName))
                                                                        {
                                                                            arrFile = strHdnfld.Split('.');
                                                                            strTempFile = arrFile[0] + "_" + strFileName + "." + arrFile[1];
                                                                        }
                                                                        else
                                                                        {
                                                                            strTempFile = strHdnfld;
                                                                        }

                                                                        //if (_fileUpld.FileName != string.Empty)
                                                                        //{
                                                                        //    strTempFile = "logo_" + _fileUpld.FileName.Trim() + ".gif";
                                                                        //}

                                                                        if (_txtCSSName.Text.Trim() != string.Empty)
                                                                        {
                                                                            strTempFile = "logo_" + _txtCSSName.Text.Trim() + ".gif";
                                                                        }
                                                                        
                                                                        strReplace = declaration.SelectSingleNode("value").InnerText.Substring(declaration.SelectSingleNode("value").InnerText.LastIndexOf('/') + 1);
                                                                        strReplace = strReplace.Substring(0, strReplace.Length - 1);
                                                                        declaration.SelectSingleNode("value").InnerText = declaration.SelectSingleNode("value").InnerText.Replace(strReplace, strTempFile);
                                                                        
                                                                        strHdnfld = strTempFile;
                                                                        strTempFile = String.Empty;
                                                                        isSaved = false;
                                                                    }
                                                                    else if (isPreviewed)
                                                                    { 
                                                                        int indexToSearch = strCSSFilePath.LastIndexOf("/") + 1;
                                                                        int length = strCSSFilePath.Length - indexToSearch;
                                                                        strHdnfld = "logo_" + strCSSFilePath.Substring(indexToSearch, length).Split('.')[0] + ".gif";
                                                                        strReplace = declaration.SelectSingleNode("value").InnerText.Substring(declaration.SelectSingleNode("value").InnerText.LastIndexOf('/') + 1);
                                                                        strReplace = strReplace.Substring(0, strReplace.Length - 1);
                                                                        if (_fileUpld.HasFile)
                                                                        {
                                                                            if (_fileUpld.PostedFile.FileName != string.Empty)
                                                                            {
                                                                                declaration.SelectSingleNode("value").InnerText = declaration.SelectSingleNode("value").InnerText.Replace(strReplace, strHdnfld);
                                                                                if (ViewState["PreviewLogo"] == null)
                                                                                {
                                                                                    ViewState.Add("PreviewLogo",declaration.SelectSingleNode("value").InnerText);
                                                                                }
                                                                                else
                                                                                {
                                                                                    ViewState["PreviewLogo"] = declaration.SelectSingleNode("value").InnerText;
                                                                                }
                                                                            } 
                                                                        }
                                                                        else
                                                                        {
                                                                            declaration.SelectSingleNode("value").InnerText = GetUserSetLogo(); 
                                                                        }
                                                                        isPreviewed = false;
                                                                    }

                                                                    UploadFile(_fileUpld, strHdnfld);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                // End of Table Main1
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.IO.File.WriteAllText(strCSSFilePath, ConvertToCSS(objDoc.InnerXml.Replace("ï»¿", "")));
            }
        }
        #endregion

        #region ResetImages
        private void ResetImages()
        {
            try
            {
                string strFileNameOnServer = ConfigurationSettings.AppSettings["UploadImageFilePath"] + "/" + "logo.gif";
                string strTempUserFileNameOnServer = ConfigurationSettings.AppSettings["TempUserUploadImageFilePath"] + "/" + "logo.gif";
                string strBaseLocation = Server.MapPath("~") + "/";
                File.Copy(strBaseLocation + strTempUserFileNameOnServer, strBaseLocation + strFileNameOnServer, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetListofImagesFromConfig
        private string[] GetListofImagesFromConfig()
        {
            string[] strFileNames;
            strFileNames = new string[10];
            try
            {
                strFileNames = Directory.GetFiles(Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["TempUserUploadImageFilePath"], "*.gif");
                return strFileNames;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region SaveImageFile
        private void SaveImageFile(string fileNameToSave)
        {
            try
            {
                string strFileNameOnServer = ConfigurationSettings.AppSettings["UploadImageFilePath"] + "/" + fileNameToSave;
                string strTempUserFileNameOnServer = ConfigurationSettings.AppSettings["TempUserUploadImageFilePath"] + "/" + fileNameToSave;
                string strBaseLocation = Server.MapPath("~") + "/";
                CopyFile(strBaseLocation + strFileNameOnServer, strBaseLocation + strTempUserFileNameOnServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetFileNameFromFilePath
        private string GetFileNameFromFilePath(String filePath)
        {
            try
            {
                filePath = filePath.Replace("\\", "/");
                string[] strPathArray = filePath.Split('/');
                string fileName = String.Empty;
                if (strPathArray.Length > 0)
                {
                    fileName = strPathArray[strPathArray.Length - 1];
                }
                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UploadFile
        private void UploadFile(FileUpload uplTheFile, string fileNameToSave)
        {
            try
            {
                if (uplTheFile.HasFile)
                {
                    if (uplTheFile.PostedFile.FileName != "")
                    {
                        if (isSaved)
                        {
                            string[] arrFile = fileNameToSave.Split('.');
                            fileNameToSave = arrFile[0] + "_" + strFileName + "." + arrFile[1];
                        }
                        string strFileNameOnServer = ConfigurationSettings.AppSettings["UploadImageFilePath"] + "/" + fileNameToSave;
                        string strBaseLocation = Server.MapPath("~") + "/";

                        if ("" == strFileNameOnServer)
                        {
                            return;
                        }

                        if (null != uplTheFile.PostedFile)
                        {
                            try
                            {
                                uplTheFile.PostedFile.SaveAs(strBaseLocation + strFileNameOnServer);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
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

        #region Function GetUserDefinedProperty
        private String GetUserDefinedProperty(System.Xml.XmlNode configPropertyNode)
        {
            try
            {
                String strUserDefinedPropertyName = String.Empty;
                if (configPropertyNode.SelectSingleNode("userdefinedpropertyname").InnerText != "")
                {
                    strUserDefinedPropertyName = configPropertyNode.SelectSingleNode("userdefinedpropertyname").InnerText;
                }
                else
                {
                    strUserDefinedPropertyName = configPropertyNode.SelectSingleNode("propertyname").InnerText;
                }
                return strUserDefinedPropertyName;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Function GetUserDefinedClass
        private String GetUserDefinedClass(System.Xml.XmlNode classNode)
        {
            try
            {
                String strUserDefinedClassName = String.Empty;
                if (classNode.SelectSingleNode("userdefinedclassname").InnerText != "")
                {
                    strUserDefinedClassName = classNode.SelectSingleNode("userdefinedclassname").InnerText;
                }
                else
                {
                    strUserDefinedClassName = classNode.SelectSingleNode("classname").InnerText;
                }
                return strUserDefinedClassName;
            }
            catch
            {
                return "";
            }
        }
        #endregion


        #region HighLightWizardSteps
        private void HighLightWizardSteps(int WizActiveIndex)
        {
            try
            {
                // TODO : HighLight not working for Config Menus
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "testScript", "fnHighLightWizardMenus('" + WizActiveIndex + "', '" + WizActiveIndex + "', '" + Convert.ToString(Session["ConfigWizardID"]) + "');", true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        // LSC - New Methods ..... summary TODO

        private void RenameImageFile(bool SaveNew)
        {
            try
            {
                imgPath = Server.MapPath("~") + @"\" + ConfigurationSettings.AppSettings["UploadImageFilePath"];
                string NewImg = string.Empty, PrevImg = string.Empty, FullPath = string.Empty;
                string[] strCSS = new string[0];
 
                if (ViewState["PreviewLogo"] != null)
                {
                    string[] SplitLogoPath = ViewState["PreviewLogo"].ToString().ToLower().Split('/');
                    PrevImg = SplitLogoPath[SplitLogoPath.Length - 1].Replace(")", "");
 
                    FullPath = imgPath + @"\" + PrevImg;

                    if (SaveNew)
                    {
                        NewImg = imgPath + @"\" + "logo_" + _txtCSSName.Text.Trim() + ".gif";
                    }
                    else
                    {
                        strCSS = Session[SessionName.StyleSheetToBeEdited].ToString().Split('/');
                        NewImg = imgPath + @"\" + "logo_" + strCSS[strCSS.Length - 1].Replace(".css", ".gif");
                    }

                    CopyFile(FullPath, NewImg);

                    FileInfo _Fi = new FileInfo(FullPath);
                    _Fi.Delete(); // Delete Previous version
                }
            }
            catch (Exception ex) { _lblHeaderCSSFile.Visible = true; _lblHeaderCSSFile.Text = ex.Message; }
        }

        private string GetUserSetLogo()
        { 
            string ImgName = string.Empty;

 
            if (ViewState["PreviewLogo"] == null)
            {
                if (Session[SessionName.UsedSavedPreview] == null) // Use Default
                {
                    FileInfo _Fi = new FileInfo(AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~")));

                    if (Session[SessionName.StyleSheet] != null)
                    {
                        _Fi = new FileInfo(Session[SessionName.StyleSheet].ToString());
                    }

                    ImgName = "url(images/" + "logo_" + _Fi.Name.Replace(".css", ".gif") + ")";
                    
                }
                else // Use Session Preview CSS
                {
                    ImgName = Session[SessionName.UsedSavedPreview].ToString().Trim().Replace(@"\\", "/").Replace(@"\", "/");
                    ImgName = "logo_" + ImgName.Substring(ImgName.LastIndexOf("/") + 1).ToLower();
                    ImgName = ImgName.Replace(".css", ".gif");
                    //if (ImgName.ToLower() == "logo_iris.gif")
                    //{
                    //   ImgName = "url(images/logo.gif)";
                    //}
                    //else
                    //{
                        ImgName = "url(images/" + ImgName + ")";
                    //}
                }
            }
            else // Use New Preview Logo
            {
                ImgName = ViewState["PreviewLogo"].ToString().Trim();
            }

            return ImgName;
        }



        protected void _wizardStyle_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            _wizardStyle.ActiveStepIndex = _wizardStyle.ActiveStepIndex - 1;
            HighLightWizardSteps(_wizardStyle.ActiveStepIndex);
        }

        protected void _wizardStyle_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            ((Wizard)sender).ActiveStepIndex = ((Wizard)sender).ActiveStepIndex + 1;
            HighLightWizardSteps(_wizardStyle.ActiveStepIndex);
        }

        // OnActiveStepChanged event handler
        public void OnActiveStepChanged(object sender, EventArgs e)
        {
        }

        void ReportState(object sender, string eventName, int? currentStepIdx, int? nextStepIdx)
        {
            Wizard wiz = (Wizard)sender;
            _lblError.Text = "<b>" + eventName + "</b>" +
                "ActiveStepIndex = " + wiz.ActiveStepIndex.ToString() +
                ((currentStepIdx.HasValue) ? "CurrentStepIndex = " + currentStepIdx.ToString() : "") +
                ((nextStepIdx.HasValue) ? "NextStepIndex = " + nextStepIdx.ToString() : "");
        }

        #region OK button click event
        /// <summary>
        /// Save the CSS on OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_txtCSSName.Text))
                {
                    _lblMessage.CssClass = "errorMessage";
                    _lblMessage.Text = "Please specify file name in which style will be created.";
                    _mpeSaveCSS.Show();
                    return;
                }

                string destFilename = _txtCSSName.Text + ".css";
                string strCSSFilePath = Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];
                strCSSFilePath = strCSSFilePath + "/" + destFilename;

                string srcFilename = AppFunctions.GetDefaultThemeCssFilePath(Server.MapPath("~"));
                
                if (File.Exists(strCSSFilePath))
                {
                    _lblError.CssClass = "errorMessage";
                    _lblError.Text = "A file with the name you specified already exists. Specify a different file name.";
                    return;
                }

                CopyFile(srcFilename, strCSSFilePath);

                // LSC - Rename Logo_Default Image
                RenameImageFile(true);

                isSaved = true;
                strFileName = _txtCSSName.Text;
                ReadAndSaveUserInputToCSS(strCSSFilePath);

                HighLightWizardSteps(_wizardStyle.ActiveStepIndex);
            }

            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }

            _mpeSaveCSS.Hide();
        }
        #endregion

        
        #region XML CSS Conversion
        /// <summary>
        /// Convert CSS File to XML
        /// </summary>
        /// <param name="CSSFilePath"></param>
        /// <returns></returns>
        private string ConvertToXML(string CSSFilePath)
        {
            System.IO.StreamReader CSS = new System.IO.StreamReader(CSSFilePath);

            try
            {
                StringBuilder xmlData = new StringBuilder();

                string fileData;
                string xmlParentTag = "";
                string xmlLineData = "";
                //string xmlData = "";
                string xmlPrevData = "";
                string PropData = "";
                string PropFinalData = "";
                string[] xmlDetail;
                string[] xmlPropDetail;

                xmlData.Append("<css>"); //Parent tag
                while (!CSS.EndOfStream)
                {
                    xmlLineData = "";
                    fileData = CSS.ReadLine();
                    if (fileData.Contains("{")) 
                    {
                        //if { is on same line with tag like "body{"
                        if (fileData.Trim().IndexOf("{") > 0)
                        {
                            xmlPrevData = fileData.Trim().Substring(0, fileData.Trim().IndexOf("{") - 1);
                        }
                        //if @ is found.
                        if (xmlPrevData.Contains("@"))
                        {
                            xmlPrevData = "";
                        }
                        else
                        {
                            //Making Selector tag.
                            xmlParentTag = "<rule><selector>" + xmlPrevData.Trim() + "</selector>";
                            xmlLineData = xmlParentTag + "<comment/>";
                        }
                    }
                    else if (fileData.Contains("}"))
                    {
                        if (!xmlPrevData.Contains("}"))
                        {
                            //spliting properties in array
                            xmlDetail = PropData.Split(';');
                            for (int i = 0; i < xmlDetail.Length; i++)
                            {
                                if (xmlDetail[i].Contains(":"))
                                {
                                    //spliting property name and property value
                                    xmlPropDetail = xmlDetail[i].Split(':');
                                    PropFinalData = PropFinalData + "<declaration>" + "<property>" + xmlPropDetail[0].Trim() + "</property>";
                                    PropFinalData = PropFinalData + "<value>" + xmlPropDetail[1].Replace(";", "").Trim() + "</value>";
                                    PropFinalData = PropFinalData + "<media>screen</media></declaration>";
                                }
                            }
                            xmlLineData = PropFinalData + "</rule>";
                            PropFinalData = "";
                            PropData = "";
                        }
                    }
                    else
                    {
                        //storing all property of a particular class/ID in single variable
                        if (fileData.Contains(":"))
                        {
                            PropData = PropData + fileData;
                        }
                    }

                    if (!string.IsNullOrEmpty(fileData))
                    {
                        xmlPrevData = fileData;
                    }
                    if (!string.IsNullOrEmpty(xmlLineData))
                    {
                        //xmlData = xmlData + xmlLineData;
                        xmlData.Append(xmlLineData);
                    }
                }
                //xmlData = xmlData + "</css>";
                xmlData.Append ( "</css>");
                return xmlData.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CSS.Close();
            }
        }

       
        /// <summary>
        /// Creates Css file reading the XML file
        /// </summary>
        /// <param name="nodes">Nodes of the XML file</param>
        private string ConvertToCSS(string strXML)
        {

            XmlNode nodes;
            XmlDocument objConfigDoc = new XmlDocument();
            objConfigDoc.InnerXml = strXML;
            nodes = objConfigDoc.DocumentElement;
            try
            {
                StringBuilder strCss = new StringBuilder();
                //check the Node "css"
                if (nodes.ParentNode.SelectNodes("css").Count > 0)
                {
                    XmlNodeList cssRuleNodeList;
                    
                    //Get the Node "rule"
                    cssRuleNodeList = nodes.SelectNodes("rule");
                    foreach (XmlNode nodeRule in cssRuleNodeList)
                    {
                        //Check if any comment is added in the XML
                        if (nodeRule.SelectSingleNode("comment") != null)
                        {
                            string strComment = nodeRule.SelectSingleNode("comment").InnerText.Trim();
                            if (strComment.Length != 0)
                            {
                                strCss.Append("/*");
                                strCss.Append(strComment);
                                strCss.Append("*/");
                                strCss.AppendLine();
                            }
                        }
                        //append the name of the element to the string builder
                        string strSelector = nodeRule.SelectSingleNode("selector").InnerText.Trim();
                        strCss.Append(strSelector);
                        strCss.AppendLine();
                        //append "{" to the string builder
                        strCss.Append("{");
                        strCss.AppendLine();
                        foreach (XmlNode nodeDeclaration in nodeRule.SelectNodes("declaration"))
                        {
                            strCss.Append(nodeDeclaration.SelectSingleNode("property").InnerText.Trim());
                            strCss.Append(":");
                            strCss.Append(nodeDeclaration.SelectSingleNode("value").InnerText.Trim());
                            strCss.Append(";");
                            strCss.AppendLine();
                        }
                        //append "}" to the string builder
                        strCss.Append("}");
                        strCss.AppendLine();
                    }
                }
                return strCss.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

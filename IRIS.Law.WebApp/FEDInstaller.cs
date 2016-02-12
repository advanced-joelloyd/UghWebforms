using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.IO;


namespace IRIS.Law.WebApp
{
    [RunInstaller(true)]
    public partial class FEDInstaller : Installer
    {
        public FEDInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            UpdateWebConfig();
            base.Install(stateSaver);
        }

        void UpdateWebConfig()
        {
            string _smtpHost = Context.Parameters["SMTPHOST"];
            string _smtpPort = Context.Parameters["SMTPPORT"];
            string _smtpUserName = Context.Parameters["SMTPUSERNAME"];
            string _smtpPassword = Context.Parameters["SMTPPASSWORD"];
            string _iISVersion = Context.Parameters["IISVERSION"];
            string _forceIISVersion = Context.Parameters["FORCEIISVERSION"];
            string _webServiceURL = Context.Parameters["WEBSERVICEURL"];
            string _sSL = Context.Parameters["SSL"];
            string _domainName = Context.Parameters["DOMAINNAME"];
            string _userName = Context.Parameters["USERNAME"];
            string _password = Context.Parameters["PASSWORD"];
            string _folderlocation = Context.Parameters["TargetDir"];

            if (_iISVersion.Contains("7"))
            {
                _iISVersion = "7";
            }
            else
            {
                _iISVersion = "6";
            }

            if (!string.IsNullOrEmpty(_forceIISVersion))
            {
                _iISVersion = _forceIISVersion;
            }

            System.Xml.XmlDocument xmlDocument;
            xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(_folderlocation + "Web.Config");

            XmlNodeList elemList = xmlDocument.GetElementsByTagName("add");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].ParentNode.Name == "appSettings")
                {
                    if (elemList[i].Attributes["key"].InnerText == "SMTPHost")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpHost;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "SMTPPort")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpPort;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "SMTPUserName")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpUserName;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "SMTPPassword")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpPassword;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "IISVersion")
                    {
                        elemList[i].Attributes["value"].InnerText = _iISVersion;
                    }
                }
            }

            if (_sSL != string.Empty)
            {
                

                if (_sSL=="1")
                    _webServiceURL = _webServiceURL.Replace("http", "https");
            }

            XmlNodeList elemList2 = xmlDocument.GetElementsByTagName("endpoint");
            for (int i = 0; i < elemList2.Count; i++)
            {
                switch (elemList2[i].Attributes["contract"].InnerText)
                {
                    case "ILogonService":
                    case "IClientService":
                    case "ITimeService":
                    case "IBankService":
                    case "IBranchDeptService":
                    case "IContactService":
                    case "IEarnerService":
                    case "IMatterService":
                    case "IDocumentService":
                    case "IAccountsService":
                    case "IDiaryService":
                    case "IUtilitiesService":
                        elemList2[i].Attributes["address"].InnerText = elemList2[i].Attributes["address"].InnerText.Replace("http://localhost:9999/", _webServiceURL);
                        break;
                }
                elemList2[i].Attributes["address"].InnerText = elemList2[i].Attributes["address"].InnerText
                    .Replace("http://localhost:3333", "http://localhost")
                    .Replace("http://localhost:2222", "http://localhost");
            }

            XmlTextWriter tw = new XmlTextWriter(_folderlocation + "Web.Config", Encoding.ASCII);
            try
            {
                tw.Formatting = Formatting.Indented; //this preserves indentation
                xmlDocument.Save(tw);
            }
            finally
            {
                tw.Close();
            }

            if (_domainName != string.Empty && _userName != string.Empty && _password != string.Empty)
            {
                StreamReader srConfig = new StreamReader(_folderlocation + "Web.Config");

                string strConfig = srConfig.ReadToEnd();
                srConfig.Close();
                
                using (StreamWriter sw = new StreamWriter(_folderlocation + "Web.Config", false))
                {
                    sw.Write(strConfig.Replace("<identity impersonate=\"false\" userName=\"******\" password=\"******\" />"
                                                , "<identity impersonate=\"true\" userName=\"" + _domainName + "\\" + _userName + "\" password=\"" + _password + "\" />"));
                    sw.Close();
                }

                


            }
            else
            {
                if (_iISVersion == "7")
                {
                    if (_domainName != string.Empty && _userName != string.Empty && _password != string.Empty)
                    {
                        StreamReader srConfig = new StreamReader(_folderlocation + "Web.Config");

                        string strConfig = srConfig.ReadToEnd();
                        srConfig.Close();

                        using (StreamWriter sw = new StreamWriter(_folderlocation + "Web.Config", false))
                        {
                            sw.Write(
                                strConfig.Replace(
                                    "<identity impersonate=\"false\" userName=\"******\" password=\"******\" />"
                                    , "<identity impersonate=\"true\" />"));
                            sw.Close();
                        }
                    }
                }
            }

            if (_sSL != string.Empty)
            {


                if (_sSL == "1")
                {
                    StreamReader srConfig = new StreamReader(_folderlocation + "Web.Config");

                    string strConfig = srConfig.ReadToEnd();
                    srConfig.Close();

                    using (StreamWriter sw = new StreamWriter(_folderlocation + "Web.Config", false))
                    {
                        strConfig = strConfig.Replace("binding=\"wsHttpBinding\""
                                                    , "binding=\"CustomBinding\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_ILogonService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IClientService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_ITimeService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IBankService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IBranchDeptService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IContactService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IEarnerService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IMatterService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IDocumentService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IAccountsService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"WSHttpBinding_IDiaryService\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");

                        strConfig = strConfig.Replace("<!--<mtomMessageEncoding maxBufferSize=\"524288\"  />-->"
                                                    , "<mtomMessageEncoding maxBufferSize=\"524288\"  />");

                        sw.Write(strConfig);

                        sw.Close();
                    }

                }
            }

        }

        
        
    }
}

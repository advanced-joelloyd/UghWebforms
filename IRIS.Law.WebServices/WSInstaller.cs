using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Xml;
using System.Text;
using System.IO;
using System.Xml.Linq;


namespace IRIS.Law.WebServices
{
    [RunInstaller(true)]
    public partial class WSInstaller : Installer
    {
        public WSInstaller()
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
            string _serverName = Context.Parameters["SERVERNAME"];
            string _databaseName = Context.Parameters["DATABASENAME"];
            // No longer needed
            //string _dBVersion = Context.Parameters["DBVERSION"];
            string _maintenenceMode = Context.Parameters["MAINTENENCEMODE"];
            string _sSL = Context.Parameters["SSL"];
            string _folderlocation = Context.Parameters["TargetDir"];

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

                    if (elemList[i].Attributes["key"].InnerText == "SMTPUserName")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpUserName;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "SMTPPassword")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpPassword;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "SMTPPort")
                    {
                        elemList[i].Attributes["value"].InnerText = _smtpPort;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "IISVersion")
                    {
                        elemList[i].Attributes["value"].InnerText = string.IsNullOrEmpty(_forceIISVersion) ? _iISVersion : _forceIISVersion;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "ServerName")
                    {
                        elemList[i].Attributes["value"].InnerText = _serverName;
                    }

                    if (elemList[i].Attributes["key"].InnerText == "DatabaseName")
                    {
                        elemList[i].Attributes["value"].InnerText = _databaseName;
                    }

                    // No longer needed
                    //if (elemList[i].Attributes["key"].InnerText == "DBVersion")
                    //{
                    //    elemList[i].Attributes["value"].InnerText = _dBVersion;
                    //}

                    if (elemList[i].Attributes["key"].InnerText == "MaintenenceMode")
                    {
                        if (_maintenenceMode!=string.Empty)
                            if (_maintenenceMode=="1")
                                elemList[i].Attributes["value"].InnerText = "True";
                            else
                                elemList[i].Attributes["value"].InnerText = "False";

                        
                    }
                }
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

            var xdoc = XDocument.Load(_folderlocation + "Web.Config");
            var endpoints = xdoc
                .Element("configuration")
                .Element("system.serviceModel")
                .Element("client")
                .Descendants("endpoint");
            var serviceFacade = endpoints.Single(x => x.Attribute("name").Value == "BasicHttpBinding_IHostTokenIssuer");
            serviceFacade.Attribute("address").Value = "http://localhost/Iris.Ews.Integration.ServiceFacade/HostTokenIssuer.svc";
            var writer = new XmlTextWriter(_folderlocation + "Web.Config", null);
            writer.Formatting = Formatting.Indented;
            xdoc.WriteTo(writer);
            writer.Flush();
            writer.Close();


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
                        strConfig = strConfig.Replace("bindingConfiguration=\"MainBinding\""
                                                    , "bindingConfiguration=\"EndpointHttps\"");
                        strConfig = strConfig.Replace("bindingConfiguration=\"DocumentBinding\""
                                                    , "bindingConfiguration=\"EndpointHttpsDoc\"");

                        sw.Write(strConfig);

                        sw.Close();
                    }

                }
            }

        }
    }
}

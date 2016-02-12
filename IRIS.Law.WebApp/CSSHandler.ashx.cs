using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IRIS.Law.WebApp.App_Code;
using System.IO;
using System.Web.SessionState;
using System.Configuration;

namespace IRIS.Law.WebApp
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class CSSHandler : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //string cssPath = context.Request.PhysicalPath;

            string cssPath = "";
            string sessionCss = Convert.ToString(context.Session[SessionName.StyleSheet]); 
            string IRISDefaultCSSPath = string.Empty;

            if (sessionCss == "")
            {
                string strDefaultCSSFolderPath = context.Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"];

                string[] files = System.IO.Directory.GetFiles(strDefaultCSSFolderPath, "*.css");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = files[i].Replace(@"\", "/").Replace(@"\\", "/");

                    if (!files[i].ToUpper().Contains("NIFTYCORNERS.CSS") && !files[i].ToUpper().Contains("MASTER.CSS") && !files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                    { 
                        cssPath = files[i]; 
                    }
                    if (files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                    {
                        IRISDefaultCSSPath = files[i];
                    }
                } 
            }

            if (cssPath == string.Empty) { cssPath = IRISDefaultCSSPath; }

            //if (!cssPath.Contains("niftyCorners.css"))
            //{
                //Return user defined style
                //if ((string)context.Session[SessionName.LogonName] == "msh")
                //{
                //    cssPath = @"C:\styleRed.css";
                //}
                //else
                //{
                //    //TODO
                //}

                
                if (!string.IsNullOrEmpty(Convert.ToString(sessionCss)))
                {
                    if (File.Exists(Convert.ToString(sessionCss)))
                    {
                        cssPath = sessionCss;
                    }
                    else
                    {
                        string strCustomCSSFolderPath = context.Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["ChangeStyleCSSFolderPath"];

                        if (File.Exists(strCustomCSSFolderPath + "\\" + Convert.ToString(sessionCss)))
                        {
                            cssPath = strCustomCSSFolderPath + "\\" + Convert.ToString(sessionCss);
                        }
                        else
                        {
                            string strDefaultCSSFolderPath = context.Server.MapPath("~") + "/" + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"];
                                                                                    
                            string[] files = System.IO.Directory.GetFiles(strDefaultCSSFolderPath, "*.css");
                            for (int i = 0; i < files.Length; i++)
                            {
                                files[i] = files[i].Replace(@"\", "/").Replace(@"\\", "/");

                                if (!files[i].ToUpper().Contains("NIFTYCORNERS.CSS") && !files[i].ToUpper().Contains("MASTER.CSS") && !files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                                {
                                    cssPath = files[i];
                                }
                                if (files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                                {
                                    IRISDefaultCSSPath = files[i];
                                    cssPath = IRISDefaultCSSPath;
                                }
                            }
                        }
                    }
                }
            //}

            StreamReader reader = new StreamReader(cssPath);

            // Open the file, read the contents and replace the variables
            using (reader)
            {
                string css = reader.ReadToEnd();
                context.Response.ContentType = "text/css";
                context.Response.Write(css);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

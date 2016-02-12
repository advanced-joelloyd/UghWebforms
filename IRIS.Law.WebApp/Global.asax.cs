using System;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Web.UI;
using NLog;

namespace IRIS.Law.WebApp
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            string acceptEncoding = app.Request.Headers["Accept-Encoding"];
            Stream prevUncompressedStream = app.Response.Filter;

            if (!(app.Context.CurrentHandler is Page) ||
                app.Request["HTTP_X_MICROSOFTAJAX"] != null)
                return;

            if (acceptEncoding == null || acceptEncoding.Length == 0)
                return;

            acceptEncoding = acceptEncoding.ToLower();

            if (acceptEncoding.Contains("deflate") || acceptEncoding == "*")
            {
                // defalte
                app.Response.Filter = new DeflateStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "deflate");
            }
            else if (acceptEncoding.Contains("gzip"))
            {
                // gzip
                app.Response.Filter = new GZipStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "gzip");
            }
        }

        // Thank you Rick Strahl - http://www.west-wind.com/weblog/posts/2011/May/02/ASPNET-GZip-Encoding-Caveats
        // This should prevent the illegible characters symptom, e.g. https://jira.iris.co.uk/browse/LCSWORKFLOW-2445
        protected void Application_PreSendRequestHeaders()
        {
            try
            {
                if (HttpContext.Current == null)
                {
                    return; // seems some requests come through a context, which is bizarre
                }
                // ensure that if GZip/Deflate Encoding is applied that headers are set
                // also works when error occurs if filters are still active
                HttpResponse response = HttpContext.Current.Response;
                if (response.Filter is GZipStream && response.Headers["Content-Encoding"] != "gzip")
                {
                    response.AppendHeader("Content-Encoding", "gzip");
                }
                else if (response.Filter is DeflateStream && response.Headers["Content-Encoding"] != "deflate")
                {
                    response.AppendHeader("Content-Encoding", "deflate");
                }
            }
            catch (Exception)
            {
                // JIRA LCSILBFEE-345 response doesnt have header and throw exp when code access response.Headers["Content-Encoding"]
                return;
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.Info("Application started");
            var dllPath = Server.MapPath("~/bin/IRIS.Law.WebApp.dll");
            Logger.Info("Iris.Law.WebApp.dll created {0}, modified {1}", File.GetCreationTime(dllPath), File.GetLastWriteTime(dllPath));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            Logger.FatalException("Unhandled exception", lastError);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.Info("Application stopped");
        }
    }
}
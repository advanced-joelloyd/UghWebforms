using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NLog;

namespace IRIS.Law.WebServices
{
    public static class LoggingExtensions
    {
        public static void LogAppSettings(this Logger logger)
        {
            foreach (string key in ConfigurationManager.AppSettings.Keys)
            {
                var setting = Regex.IsMatch(key, "password|pwd", RegexOptions.IgnoreCase)
                    ? "<password removed>"
                    : ConfigurationManager.AppSettings[key];
                logger.Debug("AppSetting - {0} = {1}", key, setting.DePasswordify());
            }
        }

        public static void LogConnectionStrings(this Logger logger)
        {
            foreach (ConnectionStringSettings connectionStringSetting in ConfigurationManager.ConnectionStrings)
            {
                logger.Debug("ConnectionString - {0} = {1}", connectionStringSetting.Name, connectionStringSetting.ConnectionString.DePasswordify());
            }
        }

        private static string DePasswordify(this string value) //beat that for a method name :-)
        {
            return Regex.Replace(value, "(password=)([^\";]+)", "$1<password removed>", RegexOptions.IgnoreCase);
        }

        public static LogTimer CreateLogTimer(this Logger logger)
        {
            return new LogTimer(logger);
        }

        public static MethodLogger TraceMethod(this Logger logger, string methodName)
        {
            return new MethodLogger(logger, methodName);
        }
    }

    public class LogTimer
    {
        private readonly Logger _logger;
        private Stopwatch _sw;

        public LogTimer(Logger logger)
        {
            _logger = logger;
            _sw = Stopwatch.StartNew();
        }

        public void Info(string message)
        {
            _sw.Stop();
            _logger.Info(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds));
        }

        public void Info(string message, params object[] args)
        {
            _sw.Stop();
            _logger.Info(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds), args);
        }

        public void Debug(string message)
        {
            _sw.Stop();
            _logger.Debug(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds));
        }

        public void Debug(string message, params object[] args)
        {
            _sw.Stop();
            _logger.Debug(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds), args);
        }

        public void Trace(string message)
        {
            _sw.Stop();
            _logger.Trace(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds));
        }

        public void Trace(string message, params object[] args)
        {
            _sw.Stop();
            _logger.Trace(message + string.Format(", {0:N0}ms elapsed", _sw.ElapsedMilliseconds), args);
        }
    }

    public class MethodLogger : IDisposable
    {
        private readonly Logger _logger;
        private readonly string _methodName;
        private Stopwatch _sw;

        public MethodLogger(Logger logger, string methodName)
        {
            _logger = logger;
            _methodName = methodName;
            _logger.Trace(methodName + " - Entered");
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw.Stop();
            _logger.Trace(_methodName + string.Format(" - Exited, {0:N0}ms elapsed", _sw.ElapsedMilliseconds));
        }

        public void Exited()
        {
            Dispose();
        }
    }
}
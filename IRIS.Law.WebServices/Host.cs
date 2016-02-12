namespace IRIS.Law.WebServices
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.ServiceModel;
    using System.Threading;
    using System.Timers;
    using IRIS.Law.PmsCommonData;
    using IRIS.Law.WebServiceInterfaces.Logon;
    using IRIS.Law.WebServiceInterfaces.Matter;
    using IRISLegal;

    public static class Host
    {
        static Host()
        {
            // This static constructor will be called the first time Host is referenced
            // (which will be the first time a service is called under IIS)
            _purgeTimer.Interval = _purgeMinutes * 60 * 1000;

            _purgeTimer.Elapsed += new ElapsedEventHandler(PurgeTimerElapsed);

            _purgeTimer.Start();
        }

        /// <summary>
        /// Is dataset caching turned on
        /// </summary>
        private const bool _DataSetCachingOn = false;
        public static bool DataSetCachingOn
        {
            get
            {
                return _DataSetCachingOn;
            }
        }

        /// <summary>
        /// How often the user logins and datasets cache purge is run
        /// </summary>
        private const int _purgeMinutes = 5;

        /// <summary>
        /// How long it is before logins time out
        /// </summary>
        private const int _logonTimeoutMinutes = 30;

        /// <summary>
        /// How long it is before a dataset cache is removed if unused
        /// </summary>
        private const int _dataSetCacheTimeoutMinutes = 5;

        /// <summary>
        /// How long it is before file transfer data is removed if unused
        /// </summary>
        private const int _fileTransferDataTimeoutMinutes = 5;

        /// <summary>
        /// Chunk size in bytes used for upload and download of files
        /// </summary>
        private const int _fileTransferSize = 10 * 1024;
        public static int FileTransferChunkSize
        {
            get
            {
                return _fileTransferSize;
            }
        }

        #region Only used for self hosting (not IIS)
        //private static ServiceHost _ServiceHost;

        private static ServiceHost _logonHost;

        private static ServiceHost _matterHost;

        public static void StartServices()
        {
            // Start the service in another thread and it will run requests on multiple threads
            // otherwise it will run in the UI thread and only be able to process one request at a time
            Thread startServicesThread = new Thread(StartServicesBackground);
            startServicesThread.IsBackground = true;
            startServicesThread.Start();
        }

        private static void StartServicesBackground()
        {
            //BasicHttpBinding Binding = new BasicHttpBinding();
            WSHttpBinding binding = new WSHttpBinding();

            //_ServiceHost = new ServiceHost(typeof(ServiceMain),
            //    new Uri("http://localhost:8000/ServiceHost"));

            //_ServiceHost.AddServiceEndpoint(typeof(ILogonService),
            //    Binding, "LogonService");

            //_ServiceHost.AddServiceEndpoint(typeof(IMatterService),
            //    Binding, "MatterService");

            //_ServiceHost.Open();

            _logonHost = new ServiceHost(typeof(LogonService),
                new Uri("http://localhost:8000/LogonServiceHost"));

            _logonHost.AddServiceEndpoint(typeof(ILogonService),
                binding, "LogonService");

            _logonHost.Open();

            _matterHost = new ServiceHost(typeof(MatterService),
                new Uri("http://localhost:8000/MatterServiceHost"));

            _matterHost.AddServiceEndpoint(typeof(IMatterService),
                binding, "MatterService");

            _matterHost.Open();

            _purgeTimer.Start();
        }

        public static void StopServices()
        {
            //if (_ServiceHost.State == CommunicationState.Faulted)
            //    _ServiceHost.Abort();
            //else
            //    _ServiceHost.Close();

            if (_logonHost.State == CommunicationState.Faulted)
                _logonHost.Abort();
            else
                _logonHost.Close();

            if (_matterHost.State == CommunicationState.Faulted)
                _matterHost.Abort();
            else
                _matterHost.Close();

            _purgeTimer.Stop();

            lock (_loggedOnUsers)
                _loggedOnUsers.Clear();
        }

        #endregion

        /// <summary>
        /// Users that have logged on
        /// </summary>
        private static SortedList<Guid, UserState> _loggedOnUsers = new SortedList<Guid, UserState>();
        // Use a sorted list rather than a dictionary because it allows sequential access to its values.
        // This is needed so we don't have to lock the whole thing while we purge it.
        //private static Dictionary<Guid, UserState> _LoggedOnUsers = new Dictionary<Guid, UserState>();

        /// <summary>
        /// Add the current ApplicationSettings instance into the list of logged on users
        /// </summary>
        /// <returns></returns>
        public static Guid AddLoggedOnUser()
        {
            Guid logonId = Guid.NewGuid();

            lock (_loggedOnUsers)
                _loggedOnUsers.Add(logonId, new UserState(ApplicationSettings.Instance));

            // TODO: Temp test code
            //ApplicationSettings.Instance.WebLogonId = LogonId;

            return logonId;
        }

        /// <summary>
        /// AJC 6/10/2011 required to force in a known user for ILB
        /// </summary>
        /// <param name="LogonId"></param>
        public static void AddSpecialLoggedOnUser(Guid LogonId)
        {
            lock (_loggedOnUsers)
                _loggedOnUsers.Add(LogonId, new UserState(ApplicationSettings.Instance));
        }

        /// <summary>
        /// Remove the logged on user
        /// </summary>
        /// <param name="logonId"></param>
        public static void RemoveLoggedOnUser(Guid logonId)
        {
            lock (_loggedOnUsers)
                _loggedOnUsers.Remove(logonId);
        }

        /// <summary>
        /// Load the logged on user by putting their ApplicationSettings into the
        /// list of currently calling sessions
        /// </summary>
        /// <param name="logonId"></param>
        public static void LoadLoggedOnUser(Guid logonId)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["MaintenenceMode"]))
                throw new Exception("Request failed, site is undergoing maintenence. Please logout and try again later.");
            
            UserState userState = GetUserState(logonId);
            userState.Accessed();
            ApplicationSettings.AddSession(userState.ApplicationSettings);

            // If OS has default culture settings other than culture defined in the config, then set it to config culture
            if (System.Configuration.ConfigurationManager.AppSettings["CultureInfo"] != null)
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != System.Configuration.ConfigurationManager.AppSettings["CultureInfo"])
                {
                    string Lang = System.Configuration.ConfigurationManager.AppSettings["CultureInfo"];
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Lang);
                }
            }
        }

        /// <summary>
        /// Get the user state using the logon id
        /// </summary>
        /// <param name="LogonId"></param>
        /// <returns></returns>
        public static UserState GetUserState(Guid logonId)
        {
            UserState userState;

            lock (_loggedOnUsers)
                if (!_loggedOnUsers.TryGetValue(logonId, out userState))
                    throw new Exception("User is not logged on");

            return userState;
        }

        /// <summary>
        /// Unload the logged on user by removing their ApplicationSettings from the
        /// list of currently calling sessions
        /// </summary>
        public static void UnloadLoggedOnUser()
        {
            ApplicationSettings.RemoveSession();
        }

        /// <summary>
        /// Get the data sets cache for a logged on user
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        public static DataSetsCache GetDataSetsCache(Guid logonId)
        {
            return GetUserState(logonId).DataSetsCache;
        }

        /// <summary>
        /// Get current file transfer data associated with the user
        /// </summary>
        /// <param name="logonId"></param>
        /// <returns></returns>
        public static FileTransferData GetFileTransferData(Guid logonId)
        {
            return GetUserState(logonId).FileTransferData;
        }
		
        /// <summary>
        /// Get document storage data associated with the current file transfer
        /// </summary>
        /// <param name="logonId"></param>
        /// <returns></returns>
        public static DocumentStorageData GetDocumentStorageData(Guid logonId)
        {
            Object Data = GetFileTransferData(logonId).AssociatedData;

            if (Data == null)
                Data = new DocumentStorageData();

            return (DocumentStorageData)Data;
        }

        private static System.Timers.Timer _purgeTimer = new System.Timers.Timer();

        private static void PurgeTimerElapsed(object source, ElapsedEventArgs e)
        {
            PurgeLoggedOnUsers(
                new TimeSpan(0, _logonTimeoutMinutes, 0),
                new TimeSpan(0, _dataSetCacheTimeoutMinutes, 0),
                new TimeSpan(0, _fileTransferDataTimeoutMinutes, 0));
        }

        /// <summary>
        /// Purge the logged on users to remove timed out users and datasets
        /// </summary>
        /// <param name="loginTimeout"></param>
        /// <param name="dataSetsCacheTimeout"></param>
        public static void PurgeLoggedOnUsers(TimeSpan loginTimeout, TimeSpan dataSetsCacheTimeout,
            TimeSpan FileTransferCacheTimeout)
        {
            int index;
            Guid logonId;
            UserState userState;
            bool removed;

            lock (_loggedOnUsers)
                index = _loggedOnUsers.Count - 1;

            // Go backwards through the logged on users so we don't have to lock
            // _LoggedOnUsers for the entire loop
            while (index > -1)
            {
                // Lock so we can retrieve one element
                lock (_loggedOnUsers)
                {
                    // Check the count again because items may have been removed
                    if (_loggedOnUsers.Count == 0)
                        // No items so quit while loop
                        break;

                    if (index > _loggedOnUsers.Count - 1)
                        // Items have been removed so reduce index
                        index = _loggedOnUsers.Count - 1;

                    // Get one element
                    logonId = _loggedOnUsers.Keys[index];
                    userState = _loggedOnUsers.Values[index];
                }

                removed = false;

                // AJC 6/10/2011 need a secret logon so ILB can access the web services - it must not get purged
                if (logonId != IlbCommon.IlbWebServiceSecretLogonId)
                    if (userState.LastAccess.Add(loginTimeout) < DateTime.Now)
                    {
                        // Login has timed out, lock to remove item
                        lock (_loggedOnUsers)
                            // Check time out again
                            if (userState.LastAccess.Add(loginTimeout) < DateTime.Now)
                            {
                                _loggedOnUsers.Remove(logonId);
                                removed = true;
                            }
                    }

                if (!removed)
                {
                    // User remains so purge their datasets
                    userState.DataSetsCache.Purge(dataSetsCacheTimeout);

                    // Purge file transfer data
                    userState.FileTransferData.Purge(dataSetsCacheTimeout);
                }

                index--;
            }


            // Old way which locks for entire loop
            //List<Guid> RemoveList = new List<Guid>();

            //lock (_LoggedOnUsers)
            //{
            //    foreach (KeyValuePair<Guid, UserState> User in _LoggedOnUsers)
            //        if (User.Value.LastAccess.Add(LoginTimeout) < DateTime.Now)
            //            RemoveList.Add(User.Key);

            //    foreach (Guid LogonId in RemoveList)
            //        _LoggedOnUsers.Remove(LogonId);

            //    foreach (KeyValuePair<Guid, UserState> User in _LoggedOnUsers)
            //        User.Value.DataSetsCache.Purge(DataSetsCacheTimeout);
            //}


        }

        public static void FilterWebParameters(Object ParameterObject)
        {
        }

        public static string FilterWebParameter(string Parameter)
        {
            return Parameter.Replace("'", "''");
        }
    }

    /// <summary>
    /// Holds information for a logged on user
    /// </summary>
    public class UserState
    {
        public UserState(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;

            Accessed();
        }

        private ApplicationSettings _applicationSettings;
        /// <summary>
        /// User's application settings
        /// </summary>
        public ApplicationSettings ApplicationSettings
        {
            get
            {
                Accessed();

                return _applicationSettings;
            }
        }

        private DataSetsCache _dataSetsCache = new DataSetsCache();
        /// <summary>
        /// Cached data sets for the user
        /// </summary>
        public DataSetsCache DataSetsCache
        {
            get
            {
                return _dataSetsCache;
            }
        }
		
		// TODO this is to be removed
		private FileStreamsCache _fileStreamsCache = new FileStreamsCache();
        /// <summary>
        /// Cached file streams for the user
        /// </summary>
        public FileStreamsCache FileStreamsCache
        {
            get
            {
                return _fileStreamsCache;
            }
        }

        private FileTransferData _FileTransferData = new FileTransferData();
        /// <summary>
        /// Cached file transfer data for the user
        /// </summary>
        public FileTransferData FileTransferData
        {
            get
            {
                return _FileTransferData;
            }
            set
            {
                _FileTransferData = value;
            }
        }

        //private DocumentStorageData _DocumentStorageData = new DocumentStorageData();
        ///// <summary>
        ///// Cached document storage data for the user which
        ///// is used while a file is being uploaded
        ///// </summary>
        //public DocumentStorageData DocumentStorageData
        //{
        //    get
        //    {
        //        return _DocumentStorageData;
        //    }
        //    set
        //    {
        //        _DocumentStorageData = value;
        //    }
        //}

        private Object _lockToken = new Object();

        /// <summary>
        /// Update the last access time
        /// </summary>
        public void Accessed()
        {
            // Must lock access to the last accessed time
            // as it is set by the user's process and checked
            // by the purge process.
            lock (_lockToken)
                _lastAccess = DateTime.Now;
        }

        private DateTime _lastAccess;
        /// <summary>
        /// Time that the user settings were last accessed
        /// </summary>
        public DateTime LastAccess
        {
            get
            {
                lock (_lockToken)
                    return _lastAccess;
            }
        }
    }

    
}

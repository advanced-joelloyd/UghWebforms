// TODO this is to be removed
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace IRIS.Law.WebServices
{
    /// <summary>
    /// Holds a collection of data sets
    /// </summary>
    public class FileStreamsCache
    {
        /// <summary>
        /// Store caches by name
        /// </summary>
        private Dictionary<string, FileStreamCache> _caches = new Dictionary<string, FileStreamCache>();

        /// <summary>
        /// Add a dataset to the cache
        /// </summary>
        /// <param name="dataSet">The dataset</param>
        /// <param name="name">A name for the dataset used to retrieve it later</param>
        /// <param name="queryCriteria">The criteria used to create the dataset.  
        /// This is used so that later when you retrieve the dataset you only get
        /// it back if it has the same criteria</param>
        public void Add(FileStream fileStream, string name)
        {
            lock (_caches)
                if (_caches.ContainsKey(name))
                {
                    _caches.Remove(name);
                }

            FileStreamCache fileStreamCache = new FileStreamCache();
            fileStreamCache.FileStream = fileStream;
            lock (_caches)
                _caches.Add(name, fileStreamCache);
        }

        /// <summary>
        /// Get a filestream from the cache
        /// </summary>
        /// <param name="name">The name given to the filestream</param>
        /// <returns></returns>
        public FileStream Retreive(string name)
        {
            FileStreamCache cache;

            lock (_caches)
                if (_caches.TryGetValue(name, out cache))
                {
                    return cache.FileStream;
                }

            return null;
        }

        /// <summary>
        /// Close FileStream for user
        /// </summary>
        /// <param name="name"></param>
        public void CloseFileStream(string name)
        {
            try
            {
                lock (_caches)
                {
                    FileStreamCache removeCache;
                    if (_caches.TryGetValue(name, out removeCache))
                    {
                        if (removeCache.FileStream != null)
                        {
                            removeCache.FileStream.Close();
                            removeCache.FileStream = null;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Purge the cache to remove filestreams not used for a while
        /// </summary>
        /// <param name="lastAccessed">Timeout time</param>
        public void Purge(TimeSpan lastAccessed)
        {
            lock (_caches)
            {
                List<string> removeList = new List<string>();

                foreach (KeyValuePair<string, FileStreamCache> cache in _caches)
                {
                    if (cache.Value.LastAccess.Add(lastAccessed) < DateTime.Now)
                    {
                        removeList.Add(cache.Key);
                    }
                }
                
                //FileStreamCache removeCache;
                foreach (string name in removeList)
                {        
                    //removeCache = null;
                    //if (_caches.TryGetValue(name, out removeCache))
                    //{
                    //    removeCache.FileStream.Close();
                    //}            
                    CloseFileStream(name);
                    _caches.Remove(name);
                }
            }
        }
    }

    /// <summary>
    /// A datasets cache item
    /// </summary>
    public class FileStreamCache
    {
        private FileStream _fileStream;
        /// <summary>
        /// The data set
        /// </summary>
        public FileStream FileStream
        {
            get
            {
                Accessed();

                return _fileStream;
            }
            set
            {
                _fileStream = value;

                Accessed();
            }
        }

        private void Accessed()
        {
            _lastAccess = DateTime.Now;
        }

        private DateTime _lastAccess;
        /// <summary>
        /// The last time that the filestream was accessed
        /// </summary>
        public DateTime LastAccess
        {
            get
            {
                return _lastAccess;
            }
        }
    }
}

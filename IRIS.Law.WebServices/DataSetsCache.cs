using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IRIS.Law.WebServices
{
	/// <summary>
	/// Holds a collection of data sets
	/// </summary>
	public class DataSetsCache
	{
		/// <summary>
		/// Store caches by name
		/// </summary>
		private Dictionary<string, DataSetCache> _caches = new Dictionary<string, DataSetCache>();

		/// <summary>
		/// Add a dataset to the cache
		/// </summary>
		/// <param name="dataSet">The dataset</param>
		/// <param name="name">A name for the dataset used to retrieve it later</param>
		/// <param name="queryCriteria">The criteria used to create the dataset.  
		/// This is used so that later when you retrieve the dataset you only get
		/// it back if it has the same criteria</param>
		public void Add(DataSet dataSet, string name, string queryCriteria)
		{
			lock (_caches)
				if (_caches.ContainsKey(name))
				{
					_caches.Remove(name);
				}

			DataSetCache dataSetCache = new DataSetCache();

			dataSetCache.DataSet = dataSet;
			dataSetCache.QueryCriteria = queryCriteria;

			lock (_caches)
				_caches.Add(name, dataSetCache);
		}

		/// <summary>
		/// Get a dataset from the cache
		/// </summary>
		/// <param name="name">The name given to the dataset</param>
		/// <param name="queryCriteria">The criteria used to create the dataset</param>
		/// <returns></returns>
		public DataSet Retreive(string name, string queryCriteria)
		{
			DataSetCache cache;

			lock (_caches)
				if (_caches.TryGetValue(name, out cache))
				{
					if (cache.QueryCriteria == queryCriteria)
					{
						return cache.DataSet;
					}
					else
					{
						return null;
					}
				}

			return null;
		}

		/// <summary>
		/// Purge the cache to remove datasets not used for a while
		/// </summary>
		/// <param name="lastAccessed">Timeout time</param>
		public void Purge(TimeSpan lastAccessed)
		{
			lock (_caches)
			{
				List<string> removeList = new List<string>();

				foreach (KeyValuePair<string, DataSetCache> cache in _caches)
				{
					if (cache.Value.LastAccess.Add(lastAccessed) < DateTime.Now)
					{
						removeList.Add(cache.Key);
					}
				}

				foreach (string name in removeList)
				{
					_caches.Remove(name);
				}
			}
		}
	}

	/// <summary>
	/// A datasets cache item
	/// </summary>
	public class DataSetCache
	{
		private DataSet _dataSet;
		/// <summary>
		/// The data set
		/// </summary>
		public DataSet DataSet
		{
			get
			{
				Accessed();

				return _dataSet;
			}
			set
			{
				_dataSet = value;

				Accessed();
			}
		}

		private string _queryCriteria;
		/// <summary>
		/// The criteria used to create the datase
		/// </summary>
		public string QueryCriteria
		{
			get
			{
				return _queryCriteria;
			}
			set
			{
				_queryCriteria = value;
			}
		}

		private void Accessed()
		{
			_lastAccess = DateTime.Now;
		}

		private DateTime _lastAccess;
		/// <summary>
		/// The last time that the dataset was accessed
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

using System;
using System.Collections.Generic;
using System.Text;
using IRIS.Law.WebServiceInterfaces;
using System.Data;
using System.Reflection;
using IRIS.Law.PmsCommonData;

namespace IRIS.Law.WebServices
{
	/// <summary>
	/// Creates a DataList for the entity type specified
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataListCreator<T> where T : new()
	{
		/// <summary>
		/// Create a DataList from a dataset
		/// </summary>
		/// <param name="logonId">Logon id</param>
		/// <param name="queryName">Name given to the data set</param>
		/// <param name="queryCriteria">The criteria used to create the dataset.
		/// If there are mutiple critera parameters they should be concatenated into
		/// one string.
		/// This can be empty if the dataset will always be created by an identical query
		/// or if the criteria has been added on to the end of the query name to
		/// make the cache available for longer.</param>
		/// <param name="collectionRequest">Information about the collection being requested</param>
		/// <param name="mappings">Mappings to map each dataset field to the data list 
		/// entity property name</param>
		/// <returns></returns>
		public DataList<T> Create(Guid logonId, string queryName, string queryCriteria,
			CollectionRequest collectionRequest,
			ImportMapping[] mappings)
		{
			if (queryName == null || queryName.Trim() == string.Empty)
				throw new Exception("QueryName must be specified");

			if (collectionRequest == null)
				throw new Exception("CollectionRequest must be specified");

			if (collectionRequest.StartRow < 0)
				throw new Exception("StartRow must be zero or greater");

			DataSet dataSet = null;

            DataSetsCache dataSetsCache = null;

            if (Host.DataSetCachingOn)
            {
                // Get the datasets cache for the logged on user
                dataSetsCache = Host.GetDataSetsCache(logonId);

                //if we move from a higher page to page 1 then the start row will be 0
                //we do not require to get a new dataset at this time
                //if (collectionRequest.StartRow > 0 && !collectionRequest.ForceRefresh)

                if (!collectionRequest.ForceRefresh)
                    // The start row is greater than zero and not forcing refresh
                    // so try to get a cached dataset.  
                    dataSet = dataSetsCache.Retreive(queryName, queryCriteria);
            }

			if (dataSet == null)
			{
				// Create a new dataset
				ReadDataSetEventArgs args = new ReadDataSetEventArgs();

				// Call the event to read the dataset
				OnReadDataSet(args);

				dataSet = args.DataSet;

				if (dataSet == null)
					throw new Exception("No data set has been created");

                if (Host.DataSetCachingOn)
				    // Add the dataset to the cache
				    dataSetsCache.Add(dataSet, queryName, queryCriteria);
			}

			// Assume there is only one table in the dataset.  
			// This may need to be enhanced later.
            if (dataSet.Tables.Count != 1)
                throw new Exception("Database query did not return 1 table");

			DataTable table = dataSet.Tables[0];

			DataList<T> dataList = new DataList<T>();

			dataList.FirstRowNumber = collectionRequest.StartRow;
			dataList.TotalRowCount = table.Rows.Count;

			// Copy the rows from the dataset to the data list
			ImportData(dataList, mappings, table,
				collectionRequest.StartRow, collectionRequest.RowCount);

			return dataList;
		}

        /// <summary>
        /// Creates a DataList from a Collection.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public DataList<T> Create<U>(IList<U> source, ImportMapping[] mappings) where U : new()
        {
            DataList<T> dataList = new DataList<T>();
            try
            {
                int rowIndex = 0;

                while (rowIndex < source.Count)
                {
                    T Row = new T();

                    Type sourceType = source[0].GetType();
                    Type destinationType = Row.GetType();

                    foreach (ImportMapping mapping in mappings)
                    {
                        PropertyInfo pInfo = destinationType.GetProperty(mapping.PropertyName);
                        PropertyInfo pInfoSource = sourceType.GetProperty(mapping.FieldName);

                        if (pInfo == null)
                        {
                            throw new Exception(EntityName + "." + mapping.PropertyName + " does not exist");
                        }
                        else if (pInfoSource == null)
                        {
                            throw new Exception(sourceType.Name + "." + mapping.FieldName + " does not exist");
                        }

                        U item = source[rowIndex];

                        //Get property value
                        PropertyInfo prop = sourceType.GetProperty(mapping.FieldName);
                        object value = (object)prop.GetValue(source[rowIndex], null);

                        //Add value to datalist row
                        pInfo.SetValue(Row, value, null);
                    }

                    dataList.Rows.Add(Row);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataList;
        }

		/// <summary>
		/// Name of the entity
		/// </summary>
		public string EntityName
		{
			get
			{
				return typeof(T).Name;
			}
		}

		protected virtual void OnReadDataSet(ReadDataSetEventArgs e)
		{
			if (ReadDataSet != null)
				ReadDataSet(this, e);
		}

		public delegate void ReadDataSetEventHandler(object sender, ReadDataSetEventArgs e);

		/// <summary>
		/// You must subscribe to this even to read the dataset
		/// </summary>
		public event ReadDataSetEventHandler ReadDataSet;

		/// <summary>
		/// Copy the rows from the dataset to the data list
		/// </summary>
		/// <param name="DataList"></param>
		/// <param name="Mappings"></param>
		/// <param name="Table"></param>
		/// <param name="StartRow"></param>
		/// <param name="RowCount"></param>
		private void ImportData(DataList<T> dataList, ImportMapping[] mappings, DataTable table, int startRow, int rowCount)
		{
			try
			{
				int rowIndex = startRow;

				while (rowIndex < table.Rows.Count)
				{
					T Row = new T();

					foreach (ImportMapping mapping in mappings)
					{
						PropertyInfo pInfo = Row.GetType().GetProperty(mapping.PropertyName);

						if (pInfo == null)
							throw new Exception(EntityName + "." + mapping.PropertyName + " does not exist");

						if (Convert.IsDBNull(table.Rows[rowIndex][mapping.FieldName]))
							pInfo.SetValue(Row, null, null);
						else
							pInfo.SetValue(Row, table.Rows[rowIndex][mapping.FieldName], null);
					}

					dataList.Rows.Add(Row);

					if (rowCount > 0)
						if (rowCount == dataList.Rows.Count)
							break;

					rowIndex++;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	/// <summary>
	/// Describes how to map together a dataset field name and a data list entity propery name
	/// </summary>
	public class ImportMapping
	{
		public ImportMapping(string propertyName, string fieldName)
		{
			_propertyName = propertyName;
			_fieldName = fieldName;
		}

		private string _propertyName;
		/// <summary>
		/// Data list entity propery name
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
		}

		private string _fieldName;
		/// <summary>
		/// Data set field name
		/// </summary>
		public string FieldName
		{
			get
			{
				return _fieldName;
			}
		}
	}

	public class ReadDataSetEventArgs : EventArgs
	{
		public ReadDataSetEventArgs()
		{
		}

		private DataSet _dataSet;
		public DataSet DataSet
		{
			get
			{
				return _dataSet;
			}
			set
			{
				_dataSet = value;
			}
		}
	}
}

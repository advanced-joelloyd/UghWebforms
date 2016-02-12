using System.ComponentModel;
using System.Data;
using System;
using System.IO;
using System.Configuration;
using System.Web;

namespace IRIS.Law.WebApp.App_Code
{
    [DataObject]
    public partial class StyleSheet
    {
        private static int styleSheetCount;

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static DataTable GetStyleSheets(string customCSSPath, int startIndex, int pageSize)
        {
            DataTable dt;
            dt = new DataTable();
            string strCSSFilePath = customCSSPath;
            DataColumn myDataColumn = new DataColumn();
            myDataColumn.AllowDBNull = false;
            myDataColumn.ColumnName = " ID";
            myDataColumn.DataType = System.Type.GetType("System.Int32");
            myDataColumn.Unique = true;
            dt.Columns.Add(myDataColumn);
            dt.Columns.Add("Files", Type.GetType("System.String"));
            dt.Columns.Add("Date", Type.GetType("System.String"));
            dt.Columns.Add("Default", Type.GetType("System.String"));

            int num = 1;
            string searchPattern = Convert.ToString(ConfigurationSettings.AppSettings["TemperoryCSSFileNameStartWith"]);

            foreach (String name in System.IO.Directory.GetFiles(strCSSFilePath))
            {
                if (!name.Contains(searchPattern))
                {
                    FileInfo myInfo = new FileInfo(name);
                    dt.Rows.Add(new object[] { num++, Path.GetFileName(name), myInfo.LastWriteTime.Date.ToShortDateString(), "Set As Default" });
                }
            }

            styleSheetCount = dt.Rows.Count;


            //create new empty table to hold the resultant rows 
            DataTable PagedProductsTable = dt.Clone();            

            //   i = NewPageIndex*PageSize gives us the starting row of new page
            for (int i = startIndex; i < startIndex + pageSize && i < dt.Rows.Count; i++)
            {
                //add the rows 
                PagedProductsTable.ImportRow(dt.Rows[i]);
            }
            return PagedProductsTable;
        }



        public static int GetTotalStyleSheetCount(string customCSSPath)
        {
            return styleSheetCount;
        }
    }
}
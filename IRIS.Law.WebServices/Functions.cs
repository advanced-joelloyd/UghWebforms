using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Iris.Ews.Integration.Model;

namespace IRIS.Law.WebServices
{
    public class Functions
    {
        public readonly static string SQLErrorMessage = "Server error: Database error. Please contact the server administrator";

        public static DataTable SortDataTable(DataTable dt, string sort)
        {
            DataTable newDT = dt.Clone();
            int rowCount = dt.Rows.Count;
            DataRow[] foundRows = dt.Select(null, sort); // Sort with Column name
            for (int i = 0; i < rowCount; i++)
            {
                object[] arr = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arr[j] = foundRows[i][j];
                }
                DataRow data_row = newDT.NewRow();
                data_row.ItemArray = arr;
                newDT.Rows.Add(data_row);
            }
            //clear the incoming dt
            dt.Rows.Clear();
            for (int i = 0; i < newDT.Rows.Count; i++)
            {
                object[] arr = new object[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arr[j] = newDT.Rows[i][j];
                }
                DataRow data_row = dt.NewRow();
                data_row.ItemArray = arr;
                dt.Rows.Add(data_row);
            }

            return dt;
        }
        public static bool ValidateIWSToken(HostSecurityToken oHostSecurityToken)
        {
            HostTokenIssuerClient oHostTokenIssuer = new HostTokenIssuerClient();
            Boolean success = false;
            if (oHostTokenIssuer.ValidateToken(oHostSecurityToken))
            {
                oHostTokenIssuer.Close();
                success = true;
            }
            return success;
        }

        public static Guid GetLogonIdFromToken(HostSecurityToken oHostSecurityToken)
        {
            var LogonId = (from o in oHostSecurityToken.Claims
                          where o.Key.Equals("LogonId")
                          select o.Value).SingleOrDefault();

            return new Guid(Convert.ToString(LogonId));
        }

        // -1 is the user for Rekoop Integration  and has limited access to web services
        public static void RestrictRekoopIntegrationUser(int userID)
        {
            if (userID == -1)
            {
                throw new Exception("Access denied");
            }
        }
    }
}

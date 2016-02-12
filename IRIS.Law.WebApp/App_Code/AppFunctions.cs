using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IRIS.Law.WebApp.App_Code;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

namespace IRIS.Law.WebApp.App_Code
{
    public class AppFunctions
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        public static void DeleteUsersPreviewCSS(string UserName, string strPath)
        {
            string searchPattern = "*_" + UserName + "_*";
            string[] files = System.IO.Directory.GetFiles(strPath, searchPattern);

            for (int i = 0; i < files.Length; i++)
                File.Delete(files[i]);
        }

        public static void DeleteUsersPreviewImages(string UserName, string strPath)
        { 
            string searchPattern = "*_" + UserName + "_*";
            string[] files = System.IO.Directory.GetFiles(strPath, searchPattern);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].ToLower().Contains("logo_default__" + UserName.ToLower().Trim()))
                {
                    File.Delete(files[i]);
                }
            }
        }

        #region AddDefaultToDropDownList
        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        public static void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region Convert Units to Minutes

        /// <summary>
        /// Method to convert units into an amount of time eg 1 unit = 0:06 mins
        /// </summary>
        /// <param name="elapsedTime">The elapsed time.</param>
        /// <returns></returns>
        public static string ConvertUnits(int elapsedTime)
        {
            bool isNegative = false;

            if (elapsedTime < 0)
            {
                isNegative = true;
            }
            string retValue = "";
            int mins = 0;
            int hours = 0;
            mins = Math.Abs(elapsedTime);

            if (mins > 59)
            {
                hours = decimal.ToInt32(mins / 60);
                decimal remainder = decimal.Remainder(mins, 60);
                mins = (int)remainder;
            }

            if (mins < 10)
            {
                retValue = hours.ToString() + ":0" + mins.ToString();
            }
            else
            {
                retValue = hours.ToString() + ":" + mins.ToString();
            }

            if (isNegative)
            {
                retValue = "-" + retValue;
            }
            return retValue;
        }
        #endregion

        #region Generate Random Password
        public static string GeneratePassword(int length)
        {
            string retValue = "";
            PasswardGen ps = new PasswardGen();
            retValue = ps.GenPassWithCap(length);
            return retValue;
        }
        public static string GenerateNumericPassword(int length)
        {
            string retValue = "";
            PasswardGen ps = new PasswardGen();
            retValue = ps.GenPassWithNumbers(length);
            return retValue;
        }

		public static string GetPageNameByUrl(string URL)
		{
			string resPageName = string.Empty;
			System.IO.FileInfo PageFileInfo = new System.IO.FileInfo(URL);
			resPageName = PageFileInfo.Name;
			return resPageName;
		}

        #endregion

        #region SetClientMatterDetailsInSession
        public static void SetClientMatterDetailsInSession(Guid memberId, Guid organisationId, string clientName, Guid? projectId, string matterDesc)
        {
            HttpContext.Current.Session[SessionName.MemberId] = memberId;
            HttpContext.Current.Session[SessionName.OrganisationId] = organisationId;
            HttpContext.Current.Session[SessionName.ClientName] = clientName;
            HttpContext.Current.Session[SessionName.ProjectId] = projectId;
            HttpContext.Current.Session[SessionName.MatterDesc] = matterDesc;
        }
        #endregion  
 
        #region GetDefaultThemeCssFilePath
        public static string GetDefaultThemeCssFilePath(string applicationPath)
        {
            try
            {
                string strDefaultCSSFolderPath = applicationPath + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"];
                string[] files = System.IO.Directory.GetFiles(strDefaultCSSFolderPath, "*.css");
                string strDefaultThemeCSSFilePath = string.Empty;
                string IRISDefaultCSSPath = string.Empty;

                if (files.Length > 0)
                {
                    //strDefaultThemeCSSFilePath = files[0];
                    for (int i = 0; i < files.Length; i++)
                    {
                        files[i] = files[i].Replace(@"\", "/").Replace(@"\\", "/");
 
                        if (!files[i].ToUpper().Contains("NIFTYCORNERS.CSS") && !files[i].ToUpper().Contains("MASTER.CSS") && !files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                        {
                            strDefaultThemeCSSFilePath = files[i];
                        }
                        if (files[i].ToUpper().Contains("IRISLEGAL.CSS"))
                        {
                            IRISDefaultCSSPath = files[i];
                        }
                    }

                    if (strDefaultThemeCSSFilePath == string.Empty) { strDefaultThemeCSSFilePath = IRISDefaultCSSPath; }

                    //strDefaultThemeCSSFilePath = applicationPath + ConfigurationSettings.AppSettings["DefaultCSSFolderPath"] + "\\style.css";
                }
                return strDefaultThemeCSSFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CalculateAge

        public static string CalculateAge(DateTime dob, DateTime dod)
        {
            string clientAge = string.Empty;
            //Calculate the age from the dob
            if (dob != DataConstants.BlankDate)
            {
                if (dod != DataConstants.BlankDate)
                {
                    if (dod >= dob)
                    {
                        TimeSpan age = dod.Subtract(dob);
                        clientAge = Convert.ToString(Math.Floor(age.TotalDays / 365.25));
                    }
                }
                else
                {
                    TimeSpan age = DateTime.Today.Subtract(dob);
                    clientAge = Convert.ToString(Math.Floor(age.TotalDays / 365.25));
                }
            }

            return clientAge;
        }

        #endregion

        #region GetASBaseUrl
        public static string GetASBaseUrl()
        {
            string virtualDirectory = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;

            string baseUrl = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority, virtualDirectory);

            return baseUrl;
        }

        #endregion
    }
}

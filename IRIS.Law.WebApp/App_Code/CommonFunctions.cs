using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IRIS.Law.WebApp.App_Code
{
    public class CommonFunctions
    {
        public CommonFunctions()
        {
        }

        /// <summary>
        /// A method to return if we are currently running from Visual Studio (design mode)
        /// </summary>
        /// <returns>If running in design mode</returns>
        public static bool DesignMode()
        {
            return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }

        /// <summary>
        /// A method to return a nicely formatted name string from title, forename and surname parameters.
        /// </summary>
        /// <param name="title">The title to format.</param>
        /// <param name="forenames">The forename to format.</param>
        /// <param name="surname">The surname to format.</param>
        /// <returns>String, A formatted name without spaces before or after.</returns>
        public static string MakeFullName(string title, string forenames, string surname)
        {
            string retValue = String.Empty;

            if (surname != "Default Contact" && surname != "To Whom It May Concern")
            {
                retValue += (title.Trim().Length > 0) ? title.Trim() + " " : title.Trim();
                retValue += forenames.Trim() + " ";
                retValue += surname.Trim();
                retValue = retValue.TrimStart();
            }

            retValue = (surname.ToLower() == "default contact") ? surname : retValue;

            return retValue;
        }

        public static string MakeFullName(string title, string forenames, string surname, string employer)
        {
            string retValue = "";

            if (surname != "Default Contact" && surname != "To Whom It May Concern")
            {
                retValue += (title.Trim().Length > 0) ? title.Trim() + " " : title.Trim();
                retValue += forenames.Trim() + " ";
                retValue += surname.Trim();
                retValue = retValue.TrimStart();
            }
            else
            {
                if (employer != "")
                    retValue = "To whom ever it may concern at " + employer;
            }

            return retValue;
        }

        public static string MakeFullName(string title, string forenames, string surname, string employer, string formatoptions)
        {
            string retValue = "";

            if (formatoptions == "")
            {
                if (surname != "Default Contact" && surname != "To Whom It May Concern")
                {
                    retValue += (title.Trim().Length > 0) ? title.Trim() + " " : title.Trim();
                    retValue += forenames.Trim() + " ";
                    retValue += surname.Trim();
                    retValue = retValue.TrimStart();
                }
                else
                {
                    if (employer != "")
                        retValue = "To whom ever it may concern at " + employer;
                }
            }
            else
            {
                char[] options = formatoptions.ToCharArray();

                foreach (char _char in options)
                {
                    if (_char == 't')
                    {
                        retValue += " " + title.Trim() + " ";
                    }

                    if (_char == 'f')
                    {

                        retValue += " " + forenames.Trim() + " ";
                    }

                    if (_char == 's')
                    {
                        if (surname != "Default Contact" && surname != "To Whom It May Concern")
                        {
                            retValue += " " + surname.Trim() + " ";
                        }
                    }
                }
            }
            retValue = retValue.Replace("     ", " ");
            retValue = retValue.Replace("    ", " ");
            retValue = retValue.Replace("   ", " ");
            retValue = retValue.Replace("  ", " ");
            retValue = retValue.Trim();
            return retValue;
        }

        /// <summary>
        /// A method to return a nicely formatted address string from individual address line parameters.
        /// </summary>
        /// <returns>String, A formatted address.</returns>
        public static string MakeFullAddress(string addressLine1, string addressLine2, string addressLine3,
            string addressTown, string addressCounty, string addressPostCode, string addressDxTown, string addressDxNumber,
            string addressCountry, string addressOrgName, string addressDept, string addressPoBox, string addressSubBldg,
            string addressStreetNo, string addressHouseName, string addressDepLocality)
        {
            string retValue = "";
            if (addressOrgName.Length > 0) retValue += addressOrgName.Trim() + "\r\n";
            if (addressDept.Length > 0) retValue += addressDept.Trim() + "\r\n";
            if (addressPoBox.Length > 0) retValue += addressPoBox.Trim() + "\r\n";
            if (addressSubBldg.Length > 0) retValue += addressSubBldg.Trim() + "\r\n";
            if (addressHouseName.Length > 0) retValue += addressHouseName.Trim() + "\r\n";
            if ((addressStreetNo.Length + addressLine1.Length) > 0)
            {
                if (addressStreetNo.Length > 0)
                {
                    retValue += addressStreetNo.Trim() + " " + addressLine1.Trim() + "\r\n";
                }
                else
                {
                    retValue += addressLine1.Trim() + "\r\n";
                }
            }
            if (addressLine2.Length > 0) retValue += addressLine2.Trim() + "\r\n";
            if (addressDepLocality.Length > 0) retValue += addressDepLocality.Trim() + "\r\n";
            if (addressLine3.Length > 0) retValue += addressLine3.Trim() + "\r\n";
            if (addressTown.Length > 0) retValue += addressTown.Trim() + "\r\n";
            if (addressCounty.Length > 0) retValue += addressCounty.Trim() + "\r\n";
            if (addressPostCode.Length > 0) retValue += addressPostCode.ToUpper().Trim() + "\r\n";

            retValue += "\r\n";
            if (addressDxTown.Length > 0 || addressDxNumber.Length > 0)
            {
                retValue += ("DX: " + addressDxNumber.Trim() + " " + addressDxTown.Trim() + "\r\n");
            }
            if (addressCountry.Length > 0) retValue += addressCountry.Trim() + "\r\n";
            retValue = retValue.Trim();

            return retValue;
        }


        public static string MakeFullAddressNoCarriageReturns(string addressLine1, string addressLine2, string addressLine3,
            string addressTown, string addressCounty, string addressPostCode, string addressDxTown, string addressDxNumber,
            string addressCountry, string addressOrgName, string addressDept, string addressPoBox, string addressSubBldg,
            string addressStreetNo, string addressHouseName, string addressDepLocality)
        {
            string retValue = "";

            if (addressOrgName.Length > 0) retValue += addressOrgName.Trim() + " ";
            if (addressDept.Length > 0) retValue += addressDept.Trim() + " ";
            if (addressPoBox.Length > 0) retValue += addressPoBox.Trim() + " ";
            if (addressSubBldg.Length > 0) retValue += addressSubBldg.Trim() + " ";
            if (addressHouseName.Length > 0) retValue += addressHouseName.Trim() + " ";
            if (addressStreetNo.Length > 0) retValue += addressStreetNo.Trim() + " ";
            if (addressLine1.Length > 0) retValue += addressLine1.Trim() + " ";
            if (addressLine2.Length > 0) retValue += addressLine2.Trim() + " ";
            if (addressDepLocality.Length > 0) retValue += addressDepLocality.Trim() + " ";
            if (addressLine3.Length > 0) retValue += addressLine3.Trim() + " ";
            if (addressTown.Length > 0) retValue += addressTown.Trim() + " ";
            if (addressCounty.Length > 0) retValue += addressCounty.Trim() + " ";
            if (addressPostCode.Length > 0) retValue += addressPostCode.Trim() + " ";
            if (addressCountry.Length > 0) retValue += addressCountry.Trim() + " ";
            retValue = retValue.Trim();

            return retValue;
        }

        public static string MakeAddressHorizontal(string address)
        {
            int index = 0;

            while (address.IndexOf("\r\n") >= 0)
            {
                index = address.IndexOf("\r\n\r\n\r\n\r\n");
                if (index > 0)
                {
                    address = address.Remove(index, 8);
                    address = address.Insert(index, " ");
                }

                index = address.IndexOf("\r\n\r\n\r\n");
                if (index > 0)
                {
                    address = address.Remove(index, 6);
                    address = address.Insert(index, " ");
                }

                index = address.IndexOf("\r\n\r\n");
                if (index > 0)
                {
                    address = address.Remove(index, 4);
                    address = address.Insert(index, " ");
                }

                index = address.IndexOf("\r\n");
                if (index > 0)
                {
                    address = address.Remove(index, 2);
                    address = address.Insert(index, " ");
                }
            }
            return address;
        }

        #region MakeAddressSingleLine
        public static string MakeAddressSingleLine(string addressStreetNo, string addressHouseName, string addressLine1,
            string addressTown, string addressPostCode)
        {
            string retValue = "";

            if (addressHouseName.Length > 0) retValue += addressHouseName.Trim() + ", ";
            if (addressStreetNo.Length > 0) retValue += addressStreetNo.Trim() + " ";
            if (addressLine1.Length > 0) retValue += addressLine1.Trim() + ", ";
            if (addressTown.Length > 0) retValue += addressTown.Trim() + ", ";
            if (addressPostCode.Length > 0) retValue += addressPostCode.Trim();
            retValue = retValue.Trim();

            return retValue;
        }
        #endregion

        /// <summary>
        /// Converts a Yes or No to a Boolean
        /// </summary>
        /// <param name="value">The value accepts string value</param>
        /// <returns>bool</returns>
        public static bool ConvertYesOrNoToBoolean(string value)
        {
            bool newValue;
            if (value.ToLower() == "yes")
            {
                newValue = true;
            }
            else
            {
                newValue = false;
            }
            return (bool)newValue;
        }

        /// <summary>
        /// Converts a Yes or No to a Boolean
        /// </summary>
        /// <param name="value">The value accepts bool value</param>
        /// <returns>string</returns>
        public static string ConvertBooleanToYesOrNo(bool value)
        {
            string newValue;
            if (value)
            {
                newValue = "Yes";
            }
            else
            {
                newValue = "No";
            }
            return (string)newValue;
        }

    }
}

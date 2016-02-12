using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.PmsCommonData;
using IRIS.Law.Services.Pms.Security;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.Services.Pms.SystemParameter;
using System.Configuration;
using System.Net.Mail;
using IRIS.Law.PmsCommonServices.CommonServices;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "LogonService" here, you must also update the reference to "LogonService" in Web.config and in the associated .svc file.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class LogonService : ILogonService
    {
        #region ILogonService Members

        /// <summary>
        /// Log on to the services
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public LogonReturnValue Logon(string userName, string password)
        {
            LogonReturnValue returnValue = new LogonReturnValue();

            try
            {
                // Create a new instance of ApplicationSettings and add it to 
                // the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
                ApplicationSettings.NewSession();

                try
                {
                    // If OS has default culture settings other than culture defined in the config, then set it to config culture
                    if (System.Configuration.ConfigurationManager.AppSettings["CultureInfo"] != null)
                    {
                        if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != System.Configuration.ConfigurationManager.AppSettings["CultureInfo"])
                        {
                            string Lang = System.Configuration.ConfigurationManager.AppSettings["CultureInfo"];
                            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Lang);
                        }
                    }

                    // TODO: The query for this does not use a stored procedure so need to protect against SQL injection.
                    // This is due to change in the next release anyway so will probably become a stored procedure.

                    if (bool.Parse(ConfigurationManager.AppSettings["MaintenenceMode"]))
                        throw new Exception("Request failed, site is undergoing maintenence. Please try again later.");


                    if (!SrvSecurityCommon.Logon(userName, password))
                        throw new Exception("Invalid logon details");

                    //ApplicationSettings.Instance.UserType = DataConstants.UserType.ThirdParty;
                    returnValue.LogonId = Host.AddLoggedOnUser();


                    returnValue.UserType = (int)UserInformation.Instance.UserType;
                    //"dbUid = -1" is used for Rekoop integration and doesn't need licence. 
                    if (returnValue.UserType == 1)
                        if (UserInformation.Instance.DbUid  != -1 && !UserSecuritySettings.IsUserLicenced(DataConstants.Application.PMS, true))
                            throw new Exception("The number of licenses has been exceeded");

                    //returnValue.UserType = 3;
                    // Soon the following two new fields will need their values setting from info stored against the user in ILB
                    // TODO : Comment it, Hardcoded for testing custom styling
                    returnValue.WebMaster = UserSecuritySettings.GetUserSecuitySettings(246); // 245 = Web Master
                    returnValue.WebStyleSheet = UserInformation.Instance.Stylesheet;
                    returnValue.OrganisationId = UserInformation.Instance.UserOrgId;
                    returnValue.MemberId = UserInformation.Instance.UserMemberId;
                    returnValue.IsMember = UserInformation.Instance.UserMemberId != DataConstants.DummyGuid;
                    returnValue.UserDefaultPartner = UserInformation.Instance.UserDefaultPartner;
                    returnValue.UserDefaultBranch = UserInformation.Instance.UserDefaultBranch;
                    returnValue.UserDefaultDepartment = UserInformation.Instance.UserDefaultDepartment;
                    returnValue.UserDefaultWorkType = UserInformation.Instance.UserDefaultWorkType;
                    returnValue.UserDefaultFeeMemberId = UserInformation.Instance.UserDefaultFeeMemberId;
                    returnValue.DbUid = UserInformation.Instance.DbUid;
                    returnValue.DatabaseRole = UserInformation.Instance.UserDbRole;
                    returnValue.IsPostCodeLookupEnabled = SrvSystemParameterCommon.IsPostCodeLookupEnabled();
                    returnValue.TimeUnits = ApplicationSettings.Instance.TimeUnits;
                    returnValue.AutomaticVersioning = ApplicationSettings.Instance.AutomaticVersioning;
                    if (!ApplicationSettings.Instance.ConflictCheckRoles.ToLower().Equals("client/client other side"))
                    {
                        returnValue.ConflictCheckRoles = true;
                    }
                    else
                    {
                        returnValue.ConflictCheckRoles = false;
                    }
                    // TODO : Hardcoded 154 -> PmsCommon.CommonServices.UserSecurityTypes.PmsSettings.LockUnlockDocuments
                    // Since enum PmsSettings is inside PmsCommon and which cannot be refactored
                    returnValue.CanUserLockDocument = IRIS.Law.PmsCommonServices.CommonServices.UserSecuritySettings.GetUserSecuitySettings(154);
                    // TODO : Hardcoded 169 -> PmsCommon.CommonServices.UserSecurityTypes.PmsSettings.EditArchivedMatters
                    // Since enum PmsSettings is inside PmsCommon and which cannot be refactored
                    returnValue.CanUserEditArchivedMatter = IRIS.Law.PmsCommonServices.CommonServices.UserSecuritySettings.GetUserSecuitySettings(169);

                    if (ApplicationSettings.Instance.DiaryProviderDllPath == "IRIS.Law.DiaryProviders.MSDiaryProvider.dll")
                    {
                        returnValue.IsUsingILBDiary = true;
                    }
                    else
                    {
                        returnValue.IsUsingILBDiary = false;
                    }



                    returnValue.IsFirstTimeLoggedIn = false;

                    if (UserInformation.Instance.UserLoggedIn == DataConstants.BlankDate)
                        returnValue.IsFirstTimeLoggedIn = true;
                }
                finally
                {
                    // Remove the current ApplicationSettings from the list of concurrent sessions.
                    // ApplicationSettings.RemoveSession();
                }

            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }

        /// <summary>
        /// Log off from the services.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <returns></returns>
        public ReturnValue Logoff(Guid logonId)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                Host.RemoveLoggedOnUser(logonId);
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region Password Update
        /// <summary>
        /// Change Password method.
        /// </summary>
        /// <returns></returns>
        public ReturnValue ChangePassword(Guid logonId, string userName, string password, string newPassword)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                                throw new Exception("Access denied");
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvPassword srvPassword = new SrvPassword();
                    srvPassword.Load(userName);
                    srvPassword.OriginalPassword = password;
                    srvPassword.NewPassword = newPassword;
                    srvPassword.RepeatPassword = newPassword;
                    srvPassword.RequireChange = false;
                    string error;

                    if (srvPassword.ValidateAll(out error))
                    {
                        srvPassword.Save(out error);
                    }

                    if (error == string.Empty)
                    {
                        returnValue.Success = true;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Message = error;
                    }

                }
                finally
                {
                    // Remove the logged on user's ApplicationSettings from the 
                    // list of concurrent sessions
                    Host.UnloadLoggedOnUser();

                }

            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }


        /// <summary>
        /// Request Password method.
        /// </summary>
        /// <returns></returns>
        public ReturnValue RequestPassword(string userName)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                try
                {
                    ApplicationSettings.NewSession();

                    SrvPmsPassGen newPassword = new SrvPmsPassGen();
                    string Password;
                    string error;
                    string warning;
                    SrvPassword srvPassword = new SrvPassword();

                    UserInformation.Instance.DbLogon= userName;

                    srvPassword.Load(userName);
                    //srvPassword.OriginalPassword = srvPassword.UserPassword;

                    if (srvPassword.UserName==null)
                        throw new Exception("Not a valid user");

                    //Make sure the password we have generated passes validation
                    do
                    {
                        Password = newPassword.GenStrongPass(10);
                        srvPassword.NewPassword = Password;
                    } while (srvPassword.ValidateNewPassword(out error, out warning) == false);

                    srvPassword.RepeatPassword = Password;
                    srvPassword.RequireChange = false;
                    srvPassword.ResetUserLoggedIn = true;

                    try
                    {
                        srvPassword.Save();
                    }
                    catch(Exception ex) {
                        returnValue.Success = false;
                        returnValue.Message = ex.Message;
                    }

                    if (error == string.Empty)
                    {
                        returnValue.Success = true;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Message = error;
                    }

                    //Send E-mail;

                    //create the mail message
                    MailMessage mail = new MailMessage();

                    //set the addresses
                    mail.From = new MailAddress(userName);
                    mail.To.Add(userName);

                    //set the content
                    mail.Subject = "Fee Earner Desktop - New Password";

                    string emailBody = "<p style='font-family:arial;font-size:8pt'><b>Your new password for the Fee Earner Desktop is - </b> " + Password + " </p><br/>";
                    emailBody += "<br/><br/><b>Please do not reply to this e-mail.</b>";
                    emailBody += "</table>";

                    mail.Body = emailBody;
                    mail.IsBodyHtml = true;

                    //send the message
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"].ToString());


                    if (ConfigurationManager.AppSettings["SMTPUserName"].ToString() != string.Empty && ConfigurationManager.AppSettings["SMTPPassword"].ToString() != string.Empty)
                    {
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUserName"].ToString(), ConfigurationManager.AppSettings["SMTPPassword"].ToString());
                        smtp.Credentials = credentials;
                        smtp.UseDefaultCredentials = false;
                    }
                    else
                    {
                        smtp.UseDefaultCredentials = true;
                    }


                    if (ConfigurationManager.AppSettings["IISVersion"].ToString() == "7")
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    else
                        smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    
                    smtp.Send(mail);
                }
                finally
                {
                    ApplicationSettings.RemoveSession();
                }


            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.Success = false;
                returnValue.Message = Functions.SQLErrorMessage;
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }
            

            return returnValue;
        }

        #endregion
    }
}

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
using IRIS.Law.WebServiceInterfaces.IWSProvider.Logon;
using Iris.Ews.Integration.Model;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "LogonService" here, you must also update the reference to "LogonService" in Web.config and in the associated .svc file.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.LogonServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_LogonServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class LogonServiceIWS : ILogonServiceIWS
    {
        #region ILogonService Members

        /// <summary>
        /// Log on to the services
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ReturnValue LogonVerify(HostSecurityToken oHostSecurityToken)
        {
            ReturnValue returnValue = new ReturnValue();
            try
            {
                UserState userState = null;
                userState = Host.GetUserState(Functions.GetLogonIdFromToken(oHostSecurityToken));
                returnValue.Success = true;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Bank;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.Services.Pms.Bank;
using IRIS.Law.PmsCommonData;
using System.Data;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Bank;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "BankService" here, you must also update the reference to "BankService" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.BankServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_BankServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class BankServiceIWS : IBankServiceIWS
    {
        #region IBankService Members
        BankService oBankService;
        public BankSearchReturnValue BankSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            BankSearchCriteria criteria)
        {
            BankSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oBankService = new BankService();
                returnValue = oBankService.BankSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new BankSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion
    }
}

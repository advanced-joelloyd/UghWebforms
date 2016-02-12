using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using IRIS.Law.PmsCommonData;
using IRIS.Law.PmsCommonData.Accounts;
using IRIS.Law.PmsCommonData.Time;
using IRIS.Law.PmsCommonServices.CommonServices;
using IRIS.Law.Services.Accounts.AntiBill;
using IRIS.Law.Services.Accounts.AntiDisbLedger;
using IRIS.Law.Services.Accounts.BillWoLedger;
using IRIS.Law.Services.Accounts.Bledger;
using IRIS.Law.Services.Accounts.ChequeRequest;
using IRIS.Law.Services.Accounts.Cledger;
using IRIS.Law.Services.Accounts.DisbLedger;
using IRIS.Law.Services.Accounts.DisbusmentType;
using IRIS.Law.Services.Accounts.Dledger;
using IRIS.Law.Services.Accounts.DraftBill;
using IRIS.Law.Services.Accounts.LaLedger;
using IRIS.Law.Services.Accounts.PayLedger;
using IRIS.Law.Services.Accounts.Period;
using IRIS.Law.Services.Accounts.TimeWoff;
using IRIS.Law.Services.Accounts.VATRate;
using IRIS.Law.Services.Pms.Address;
using IRIS.Law.Services.Pms.Bank;
using IRIS.Law.Services.Pms.Branch;
using IRIS.Law.Services.Pms.Earner;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.Services.Pms.Time;
using IRIS.Law.Services.Pms.User;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Accounts;
using Iris.Ews.Integration.Model;
using IRIS.Law.WebServiceInterfaces.IWSProvider.Accounts;
namespace IRIS.Law.WebServices.IWSProvider
{
    // NOTE: If you change the class name "AccountsServiceIWS" here, you must also update the reference to "AccountsServiceIWS" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.AccountsServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_AccountsServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class AccountsServiceIWS : IAccountsServiceIWS
    {
        #region GetClientBankIdByProjectId
        AccountsService oAccountService;
        /// <summary>
        /// Gets the client bank id by project id
        /// This method sets client bank id for adding client cheque request
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">Project id to get client bank id</param>        
        /// <returns>Returns client bank id by project id</returns>
        public ClientBankIdReturnValue GetClientBankIdByProjectId(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            ClientBankIdReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetClientBankIdByProjectId(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new ClientBankIdReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetDefaultChequeRequestDetails

        /// <summary>
        /// Gets the client bank id by project id
        /// Get OfficeVattable
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">Project id to get client bank id</param>  
        public ChequeRequestReturnValue GetDefaultChequeRequestDetails(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDefaultChequeRequestDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region SaveChequeRequest

        #region SaveClientChequeRequest

        /// <summary>
        /// Adds or edits client cheque requests depending on the 'IsClientChequeRequest' property
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="chequeRequest">ChequeRequest properties to add/edit cheque request.</param>
        /// <returns>Returns cheque request id after adding or editting cheque request.</returns>
        public ChequeRequestReturnValue SaveClientChequeRequest(HostSecurityToken oHostSecurityToken, ChequeRequest clientChequeRequest)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.SaveClientChequeRequest(Functions.GetLogonIdFromToken(oHostSecurityToken), clientChequeRequest);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region SaveOfficeChequeRequest

        /// <summary>
        /// Adds or edits office cheque requests depending on the 'IsClientChequeRequest' property
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="chequeRequest">ChequeRequest properties to add/edit cheque request.</param>
        /// <returns>Returns cheque request id after adding or editting cheque request.</returns>
        public ChequeRequestReturnValue SaveOfficeChequeRequest(HostSecurityToken oHostSecurityToken, ChequeRequest chequeRequest)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.SaveOfficeChequeRequest(Functions.GetLogonIdFromToken(oHostSecurityToken), chequeRequest);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #endregion

        #region LoadOfficeChequeRequestDetails

        /// <summary>
        /// Loads office cheque request details
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="officeChequeRequestId">Office Cheque Request id to get details</param>
        /// <returns>Loads office cheque request details</returns>
        public ChequeRequestReturnValue LoadOfficeChequeRequestDetails(HostSecurityToken oHostSecurityToken, int officeChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadOfficeChequeRequestDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), officeChequeRequestId);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region LoadClientChequeRequestDetails

        /// <summary>
        /// Loads client cheque request details
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="officeChequeRequestId">Client Cheque Request id to get details</param>
        /// <returns>Loads client cheque request details</returns>
        public ChequeRequestReturnValue LoadClientChequeRequestDetails(HostSecurityToken oHostSecurityToken, int clientChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadClientChequeRequestDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), clientChequeRequestId);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region DisbursmentTypeSearch

        /// <summary>
        /// Gets disbursements types for archived.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">List of disbursements type</param>
        /// <param name="criteria">Criteria toget archived disbursements types.</param>
        /// <returns>Returns list of disursement types.</returns>
        public DisbursmentTypeReturnValue DisbursmentTypeSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, DisbursmentTypeSearchCriteria criteria)
        {
            DisbursmentTypeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.DisbursmentTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new DisbursmentTypeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region LoadClientChequeRequestDetailsForPrinting

        /// <summary>
        /// Loads cheque request details
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="clientChequeRequestId">Client cheque request id toget cheque request details.</param>
        /// <returns>Returns client cheque request details by client cheque request id.</returns>
        public ChequeRequestReturnValue LoadClientChequeRequestDetailsForPrinting(HostSecurityToken oHostSecurityToken, int clientChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadClientChequeRequestDetailsForPrinting(Functions.GetLogonIdFromToken(oHostSecurityToken), clientChequeRequestId);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }

            return returnValue;
        }

        #endregion

        #region LoadClientChequeRequestDetailsForPrinting

        /// <summary>
        /// Loads office cheque request details for printing
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="clientChequeRequestId">Office cheque request id toget cheque request details.</param>
        /// <returns>Returns office cheque request details by office cheque request id.</returns>
        public ChequeRequestReturnValue LoadOfficeChequeRequestDetailsForPrinting(HostSecurityToken oHostSecurityToken, int officeChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadOfficeChequeRequestDetailsForPrinting(Functions.GetLogonIdFromToken(oHostSecurityToken), officeChequeRequestId);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Cheque Requests Authorisation

        #region GetUnauthorisedClientChequeRequests

        /// <summary>
        /// Gets unauthorised client cheque requests
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">List of unauthorised client cheque requests</param>
        /// <param name="searchCriteria">IsAuthorised and IsPosted flags should be false to get list of 
        /// unauthorised cheque requests</param>
        /// <returns>Returns list of unauthorised client cheque requests</returns>
        public ChequeAuthorisationReturnValue GetUnauthorisedClientChequeRequests(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, ChequeAuthorisationSearchCriteria searchCriteria)
        {
            ChequeAuthorisationReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetUnauthorisedClientChequeRequests(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, searchCriteria);
            }
            else
            {
                returnValue = new ChequeAuthorisationReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetUnauthorisedOfficeChequeRequests

        /// <summary>
        /// Gets unauthorised office cheque requests
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">List of unauthorised client cheque requests</param>
        /// <param name="searchCriteria">IsAuthorised and IsPosted flags should be false to get list of 
        /// unauthorised office cheque requests</param>
        /// <returns>Returns list of unauthorised office cheque requests</returns>
        public ChequeAuthorisationReturnValue GetUnauthorisedOfficeChequeRequests(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, ChequeAuthorisationSearchCriteria searchCriteria)
        {
            ChequeAuthorisationReturnValue returnValue = null;

            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetUnauthorisedClientChequeRequests(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, searchCriteria);
            }
            else
            {
                returnValue = new ChequeAuthorisationReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #endregion

        #region DeleteChequeRequests

        /// <summary>
        /// Deletes cheque requests for client or office.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="selectedChequeRequests">Collection of cheque requests for client or office for deletion</param>
        /// <param name="isClientChequeRequest">Flag whether deletion is for client or office</param>
        /// <returns>Deletes cheque requests for client or office.</returns>
        public ChequeRequestReturnValue DeleteChequeRequests(HostSecurityToken oHostSecurityToken, List<int> selectedChequeRequestsIds, bool isClientChequeRequest)
        {
            ChequeRequestReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.DeleteChequeRequests(Functions.GetLogonIdFromToken(oHostSecurityToken), selectedChequeRequestsIds, isClientChequeRequest);
            }
            else
            {
                returnValue = new ChequeRequestReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region AuthoriseChequeRequests

        /// <summary>
        /// Authorise unauthorised cheque requests for client and office.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="selectedChequeRequestsIds">Selected cheque request ids for authorisation</param>
        /// <param name="isClientChequeRequest"></param>
        /// <returns>Authorise selected unauthorised cheque request ids.</returns>
        public ReturnValue AuthoriseChequeRequests(HostSecurityToken oHostSecurityToken, List<int> selectedChequeRequestsIds, bool isClientChequeRequest)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.AuthoriseChequeRequests(Functions.GetLogonIdFromToken(oHostSecurityToken), selectedChequeRequestsIds, isClientChequeRequest);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetDisbursementsDetails

        /// <summary>
        /// Loads disbursement details by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="accounts">Retrieves project id to load disbursement details.</param>
        /// <returns>Loads disbursement details by project id</returns>
        public DisbursementsSearchReturnValue GetDisbursementsDetails(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            DisbursementsSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDisbursementsDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new DisbursementsSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetFinancialInfoByProjectId

        /// <summary>
        /// Loads financial information for office, client and deposit by project id.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">Project id usedto retrieve financial information</param>
        /// <returns>Retrieves financial information by project id.</returns>
        public FinancialInfoReturnValue GetFinancialInfoByProjectId(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            FinancialInfoReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetFinancialInfoByProjectId(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new FinancialInfoReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetDepositBalancesDetails

        /// <summary>
        /// Gets deposit balances by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Gets collection of deposit by project id</param>
        /// <param name="projectId">Project id requiredto get deposit details</param>
        /// <returns>Retrieves deposit details by project id</returns>
        public BalancesSearchReturnValue GetDepositBalancesDetails(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = null;

            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDepositBalancesDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BalancesSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetClientBalancesDetails

        /// <summary>
        /// Gets client balance details by project id.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Gets collection of client balances by project id</param>
        /// <param name="projectId">Project id required to get client balances details</param>
        /// <returns>Retrieves client balances by project id</returns>
        public BalancesSearchReturnValue GetClientBalancesDetails(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetClientBalancesDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BalancesSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetOfficeBalancesDetails

        /// <summary>
        /// Gets offices balances by project id.
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Gets collection of office balances by project id</param>
        /// <param name="projectId">Project id required to get office balances details</param>
        /// <returns>Retrieves office balances by project id</returns>
        public BalancesSearchReturnValue GetOfficeBalancesDetails(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetOfficeBalancesDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BalancesSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetFinancialBalances

        /// <summary>
        /// Loads financial balances by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">To load financial balances</param>
        /// <returns>Returns properties of financial details</returns>
        public FinancialBalancesReturnValue GetFinancialBalances(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            FinancialBalancesReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetFinancialBalances(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new FinancialBalancesReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Bills Ledger

        #region LoadAllBills

        /// <summary>
        /// Loads all the bills by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">To load all the bills</param>
        /// <returns>Loads all the bills by project id</returns>
        public BillsLedgerReturnValue LoadAllBills(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadAllBills(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BillsLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region LoadWriteOffBills

        /// <summary>
        /// Loads write off bills by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">To load write off bills</param>
        /// <returns>Loads write off bills by project id</returns>
        public BillsLedgerReturnValue LoadWriteOffBills(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadWriteOffBills(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BillsLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region LoadUnclearedBills

        /// <summary>
        /// Loads uncleared bills by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">To load uncleared bills</param>
        /// <returns>Loads uncleared bills by project id</returns>
        public BillsLedgerReturnValue LoadUnclearedBills(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.LoadUnclearedBills(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new BillsLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #endregion

        #region TimeTransactions

        #region GetTimeLedger

        /// <summary>
        /// Loads all the bills by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">To load all the bills</param>
        /// <returns>Loads all the bills by project id</returns>
        public TimeLedgerSearchReturnValue GetTimeLedger(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, string timeFilter, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetTimeLedger(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, timeFilter, projectId);
            }
            else
            {
                returnValue = new TimeLedgerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetWriteOffTimeTransactions

        /// <summary>
        /// Loads write off time transactions by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Loads listof write off time transactions</param>
        /// <param name="projectId">To load write off transactions.</param>
        /// <returns>Loads write off time transactions</returns>
        public TimeLedgerSearchReturnValue GetWriteOffTimeTransactions(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetWriteOffTimeTransactions(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new TimeLedgerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetWriteOffReversalTimeTransactions

        /// <summary>
        /// Loads write off time transactions by project id
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Loads listof write off time transactions</param>
        /// <param name="projectId">To load write off transactions.</param>
        /// <returns>Loads write off time transactions</returns>
        public TimeLedgerSearchReturnValue GetWriteOffReversalTimeTransactions(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetWriteOffReversalTimeTransactions(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, projectId);
            }
            else
            {
                returnValue = new TimeLedgerSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #endregion

        #region Draft Bills

        /// <summary>
        /// Gets unposted draft bills 
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">List of unposted draft bills</param>
        /// <returns>Returns list of unposted draft bills</returns>
        public DraftBillSearchReturnValue GetUnpostedDraftBills(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest)
        {
            DraftBillSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetUnpostedDraftBills(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest);
            }
            else
            {
                returnValue = new DraftBillSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Submits selected draft bills
        /// </summary>
         /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="selectedDraftBillIds"></param>
        /// <returns></returns>
        public DraftBillReturnValue SubmitDraftBill(HostSecurityToken oHostSecurityToken, List<int> selectedDraftBillIds)
        {
            DraftBillReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.SubmitDraftBill(Functions.GetLogonIdFromToken(oHostSecurityToken), selectedDraftBillIds);
            }
            else
            {
                returnValue = new DraftBillReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #region GetDraftBill

        /// <summary>
        /// Returns draft bill details by draft bill id
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="draftBillId">Draft bill id to populate</param>
        /// <returns></returns>
        public DraftBillReturnValue GetDraftBill(HostSecurityToken oHostSecurityToken, int draftBillId)
        {
            DraftBillReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDraftBill(Functions.GetLogonIdFromToken(oHostSecurityToken), draftBillId);
            }
            else
            {
                returnValue = new DraftBillReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        public VatRateSearchReturnValue VatRateSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                      VatRateSearchCriteria criteria)
        {
            VatRateSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.VatRateSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new VatRateSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public BranchVatReturnValue GetBranchVatForProject(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            BranchVatReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetBranchVatForProject(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new BranchVatReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public DisbursementLedgerReturnValue GetDisbursementLedgerVatableTransaction(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            DisbursementLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDisbursementLedgerVatableTransaction(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new DisbursementLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public DisbursementLedgerReturnValue GetDisbursementLedgerNonVatableTransaction(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            DisbursementLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetDisbursementLedgerNonVatableTransaction(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new DisbursementLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public AnticipatedDisbursementLedgerReturnValue GetAnticipatedDisbursementLedgerVatableTransaction(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            AnticipatedDisbursementLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetAnticipatedDisbursementLedgerVatableTransaction(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new AnticipatedDisbursementLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public AnticipatedDisbursementLedgerReturnValue GetAnticipatedDisbursementLedgerNonVatableTransaction(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            AnticipatedDisbursementLedgerReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetAnticipatedDisbursementLedgerNonVatableTransaction(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new AnticipatedDisbursementLedgerReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        public TimeTransactionReturnValue GetTimeTransaction(HostSecurityToken oHostSecurityToken, Guid projectId)
        {
            TimeTransactionReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.GetTimeTransaction(Functions.GetLogonIdFromToken(oHostSecurityToken), projectId);
            }
            else
            {
                returnValue = new TimeTransactionReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Adds the draft bill.
        /// </summary>
        /// <param name="oHostSecurityToken">The HostSecurityToken with Logon Id.</param>
        /// <param name="bill">The bill.</param>
        /// <returns></returns>
        public DraftBillReturnValue AddDraftBill(HostSecurityToken oHostSecurityToken, DraftBill bill)
        {
            DraftBillReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.AddDraftBill(Functions.GetLogonIdFromToken(oHostSecurityToken), bill);
            }
            else
            {
                returnValue = new DraftBillReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        //Get Clearance Types
        #region GetClearanceTypes

        public ClearanceTypeSearchReturnValue ClearanceTypes(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            ClearanceTypeSearchCriteria criteria)
        {
            ClearanceTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oAccountService = new AccountsService();
                returnValue = oAccountService.ClearanceTypes(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest,criteria);
            }
            else
            {
                returnValue = new ClearanceTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion
    }
}

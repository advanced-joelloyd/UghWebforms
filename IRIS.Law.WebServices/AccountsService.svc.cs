
using IRISLegal;

namespace IRIS.Law.WebServices
{
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
    using IRIS.Law.PmsBusiness;
    using IRIS.Law.PmsCommonData.Accounts.DraftBill;

    // NOTE: If you change the class name "AccountsService" here, you must also update the reference to "AccountsService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class AccountsService : IAccountsService
    {
        #region GetClientBankIdByProjectId

        /// <summary>
        /// Gets the client bank id by project id
        /// This method sets client bank id for adding client cheque request
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">Project id to get client bank id</param>        
        /// <returns>Returns client bank id by project id</returns>
        public ClientBankIdReturnValue GetClientBankIdByProjectId(Guid logonId, Guid projectId)
        {
            ClientBankIdReturnValue returnValue = new ClientBankIdReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                   
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    returnValue.ClientBankId = SrvMatterCommon.GetClientBankIdByProjectId(projectId);
                    returnValue.Success = true;
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

        #endregion

        #region GetDefaultChequeRequestDetails

        /// <summary>
        /// Gets the client bank id by project id
        /// Get OfficeVattable
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">Project id to get client bank id</param>  
        public ChequeRequestReturnValue GetDefaultChequeRequestDetails(Guid logonId, Guid projectId)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    ChequeRequest chequeReq = new ChequeRequest();
                    chequeReq.BankId = SrvMatterCommon.GetClientBankIdByProjectId(projectId);
                    chequeReq.BankOfficeId = SrvMatterCommon.GetOfficeBankIdByProjectId(projectId);
                    Guid matterBranchId = SrvMatterCommon.GetMatterBranchGuid(projectId);
                    if (matterBranchId != PmsCommonData.DataConstants.DummyGuid)
                    {
                        bool branchNoVat = SrvBranchCommon.GetBranchNoVAT(matterBranchId);
                        if (branchNoVat)
                        {
                            chequeReq.OfficeVATTable = IlbCommon.YesNo.No;
                        }
                    }

                    returnValue.ChequeRequest = chequeReq;
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

        #endregion

        #region SaveChequeRequest

        #region SaveClientChequeRequest

        /// <summary>
        /// Adds or edits client cheque requests depending on the 'IsClientChequeRequest' property
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="chequeRequest">ChequeRequest properties to add/edit cheque request.</param>
        /// <returns>Returns cheque request id after adding or editting cheque request.</returns>
        public ChequeRequestReturnValue SaveClientChequeRequest(Guid logonId, ChequeRequest clientChequeRequest)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    //243 = RequestClientCredits
                    //242 = RequestClientDebits

                    if (clientChequeRequest.ClientChequeRequestsIsCredit)
                        if (!UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.RequestClientCredits))
                            //if (!UserSecuritySettings.GetUserSecuitySettings(243))
                            throw new Exception("You do not have sufficient permissions to carry out this request");

                    if (!clientChequeRequest.ClientChequeRequestsIsCredit)
                        if (!UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.RequestClientDebits))
                            //if (!UserSecuritySettings.GetUserSecuitySettings(242))
                            throw new Exception("You do not have sufficient permissions to carry out this request");


                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;
                    string errorCaption = string.Empty;

                    // Creates object for client cheque request service class                    
                    SrvClientChequeRequest srvClientChequeRequest = new SrvClientChequeRequest();

                    // Sets properties
                    srvClientChequeRequest.Id = clientChequeRequest.ChequeRequestId;
                    srvClientChequeRequest.ProjectId = clientChequeRequest.ProjectId;
                    srvClientChequeRequest.Date = clientChequeRequest.ChequeRequestDate;
                    srvClientChequeRequest.ClientBankId = clientChequeRequest.BankId;
                    srvClientChequeRequest.Description = clientChequeRequest.ChequeRequestDescription;
                    srvClientChequeRequest.Payee = clientChequeRequest.ChequeRequestPayee;
                    srvClientChequeRequest.Amount = clientChequeRequest.ChequeRequestAmount;
                    srvClientChequeRequest.Reference = clientChequeRequest.ChequeRequestReference;
                    srvClientChequeRequest.MemberId = clientChequeRequest.MemberId;
                    srvClientChequeRequest.ClearanceDaysChq = clientChequeRequest.ClientChequeRequestsClearanceDaysChq;
                    srvClientChequeRequest.ClearanceDaysElec = clientChequeRequest.ClientChequeRequestsClearanceDaysElec;
                    srvClientChequeRequest.ClearanceTypeId = clientChequeRequest.ClearanceTypeId;
                    srvClientChequeRequest.IsCredit = clientChequeRequest.ClientChequeRequestsIsCredit;

                    if (clientChequeRequest.ProceedIfOverDrawn)
                    {
                        // If the 'ProceedIfOverDrawn' flag is false then it should be true for next
                        // save click event. As its false for warning messages.
                        srvClientChequeRequest.ProceedIfOverDrawn = true;
                    }

                    returnValue.Success = srvClientChequeRequest.Save(out errorMessage, out warningMessage, out errorCaption);

                    if (returnValue.Success)
                    {
                        // if client cheque request is saved then authorises it.
                        if (clientChequeRequest.IsChequeRequestAuthorised && UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseClientChequeRequest))
                        {
                            if (UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseChequeRequestHigherAmount))
                            {
                                // As Suggested by client to use the overloaded method and requestId property
                                if (srvClientChequeRequest.RequesterId == UserInformation.Instance.DbUid)
                                {
                                    SrvClientChequeRequestCommon.AddAuthoriseClientChequeRequests(srvClientChequeRequest.Id, UserInformation.Instance.UserMemberId, DateTime.Now);
                                }
                                else
                                {
                                    SrvClientChequeRequestCommon.AddAuthoriseClientChequeRequests(srvClientChequeRequest.Id, UserInformation.Instance.UserMemberId, DateTime.Now, srvClientChequeRequest.RequesterId, srvClientChequeRequest.Description);
                                }
                            }
                            else
                            {
                                Decimal clientChequeRequestHigherAmount = Convert.ToDecimal(UserSecuritySettings.GetUserSecuitySettingsFeatureValue((int)AccountsSettings.ClientChequeRequestHigherAmount));

                                if (Convert.ToDecimal(srvClientChequeRequest.Amount) > clientChequeRequestHigherAmount)
                                {
                                    errorMessage = "You are not allowed to Authorise Client Cheque Requests over the value of £" +
                                                     clientChequeRequestHigherAmount.ToString("0.00") + ".";
                                }
                                else
                                {
                                    SrvClientChequeRequestCommon.AddAuthoriseClientChequeRequests(srvClientChequeRequest.Id, UserInformation.Instance.UserMemberId, DateTime.Now);
                                }
                            }
                        }

                        clientChequeRequest.ChequeRequestId = srvClientChequeRequest.Id;
                        returnValue.ChequeRequest = clientChequeRequest;
                    }

                    returnValue.Message = errorMessage;
                    returnValue.WarningMessage = warningMessage;
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

        #endregion

        #region SaveOfficeChequeRequest

        /// <summary>
        /// Adds or edits office cheque requests depending on the 'IsClientChequeRequest' property
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="chequeRequest">ChequeRequest properties to add/edit cheque request.</param>
        /// <returns>Returns cheque request id after adding or editting cheque request.</returns>
        public ChequeRequestReturnValue SaveOfficeChequeRequest(Guid logonId, ChequeRequest chequeRequest)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    //163 - Create Office Cheque Requests

                    //if (!UserSecuritySettings.GetUserSecuitySettings(163))
                    if (!UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.CreateOfficeChequeRequest))
                        throw new Exception("You do not have sufficient permissions to carry out this request");

                    SrvOfficeChequeRequest srvOfficeChequeRequest = new SrvOfficeChequeRequest();
                    srvOfficeChequeRequest.Id = chequeRequest.ChequeRequestId;
                    srvOfficeChequeRequest.ProjectId = chequeRequest.ProjectId;
                    srvOfficeChequeRequest.Date = chequeRequest.ChequeRequestDate;
                    srvOfficeChequeRequest.DisbursementTypeId = chequeRequest.DisbursementTypeId;
                    srvOfficeChequeRequest.Description = chequeRequest.ChequeRequestDescription;
                    srvOfficeChequeRequest.Payee = chequeRequest.ChequeRequestPayee;
                    srvOfficeChequeRequest.OfficeBankId = chequeRequest.BankId;
                    srvOfficeChequeRequest.Amount = chequeRequest.ChequeRequestAmount;
                    srvOfficeChequeRequest.OfficeVATTable = chequeRequest.OfficeVATTable;
                    srvOfficeChequeRequest.VATRateId = chequeRequest.VATRateId;
                    srvOfficeChequeRequest.VATAmount = chequeRequest.VATAmount;
                    srvOfficeChequeRequest.IsAnticipated = chequeRequest.IsChequeRequestAnticipated;
                    srvOfficeChequeRequest.MemberId = chequeRequest.MemberId;

                    string errorMessage = string.Empty;
                    string caption = string.Empty;
                    bool isSaveSuccessful = srvOfficeChequeRequest.Save(out errorMessage, out caption);

                    if (chequeRequest.ChequeRequestId == 0)
                    {
                        chequeRequest.ChequeRequestId = srvOfficeChequeRequest.Id;
                    }

                    returnValue.ChequeRequest = chequeRequest;
                    returnValue.Success = isSaveSuccessful;

                    if (returnValue.Success)
                    {
                        // if client cheque request is saved then authorises it.
                        if (chequeRequest.IsChequeRequestAuthorised && UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseClientChequeRequest))
                        {
                            if (UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseChequeRequestHigherAmount))
                            {

                                SrvOfficeChequeRequestCommon.AddAuthoriseOfficeChequeRequests(srvOfficeChequeRequest.Id, UserInformation.Instance.UserMemberId, DateTime.Now);
                            }
                            else
                            {
                                Decimal officeChequeRequestHigherAmount = Convert.ToDecimal(UserSecuritySettings.GetUserSecuitySettingsFeatureValue((int)AccountsSettings.OfficeChequeRequestHigherAmount));

                                if (Convert.ToDecimal(srvOfficeChequeRequest.Amount) > officeChequeRequestHigherAmount)
                                {
                                    errorMessage = "You are not allowed to Authorise Off Cheque Requests over the value of £" +
                                                     officeChequeRequestHigherAmount.ToString("0.00") + ".";
                                }
                                else
                                {

                                    SrvOfficeChequeRequestCommon.AddAuthoriseOfficeChequeRequests(srvOfficeChequeRequest.Id, UserInformation.Instance.UserMemberId, DateTime.Now);
                                }
                            }
                        }

                        chequeRequest.ChequeRequestId = srvOfficeChequeRequest.Id;
                        returnValue.ChequeRequest = chequeRequest;
                    }

                    returnValue.Message = errorMessage;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #endregion

        #region LoadOfficeChequeRequestDetails

        /// <summary>
        /// Loads office cheque request details
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="officeChequeRequestId">Office Cheque Request id to get details</param>
        /// <returns>Loads office cheque request details</returns>
        public ChequeRequestReturnValue LoadOfficeChequeRequestDetails(Guid logonId, int officeChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvOfficeChequeRequest srvOfficeChequeRequest = new SrvOfficeChequeRequest();
                    srvOfficeChequeRequest.Id = officeChequeRequestId;
                    srvOfficeChequeRequest.Load();

                    ChequeRequest chequeReq = new ChequeRequest();
                    chequeReq.ProjectId = srvOfficeChequeRequest.ProjectId;
                    chequeReq.ChequeRequestDate = srvOfficeChequeRequest.Date;
                    chequeReq.DisbursementTypeId = srvOfficeChequeRequest.DisbursementTypeId;
                    chequeReq.ChequeRequestDescription = srvOfficeChequeRequest.Description;
                    chequeReq.ChequeRequestPayee = srvOfficeChequeRequest.Payee;
                    chequeReq.BankId = srvOfficeChequeRequest.OfficeBankId;
                    chequeReq.ChequeRequestAmount = srvOfficeChequeRequest.Amount;
                    chequeReq.OfficeVATTable = srvOfficeChequeRequest.OfficeVATTable;
                    chequeReq.VATRateId = srvOfficeChequeRequest.VATRateId;
                    chequeReq.VATAmount = srvOfficeChequeRequest.VATAmount;
                    chequeReq.IsChequeRequestAuthorised = srvOfficeChequeRequest.IsAuthorised;
                    chequeReq.MemberId = srvOfficeChequeRequest.MemberId;
                    chequeReq.IsChequeRequestAnticipated = srvOfficeChequeRequest.IsAnticipated;
                    returnValue.ChequeRequest = chequeReq;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region LoadClientChequeRequestDetails

        /// <summary>
        /// Loads client cheque request details
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="officeChequeRequestId">Client Cheque Request id to get details</param>
        /// <returns>Loads client cheque request details</returns>
        public ChequeRequestReturnValue LoadClientChequeRequestDetails(Guid logonId, int clientChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvClientChequeRequest srvClientChequeRequest = new SrvClientChequeRequest();
                    srvClientChequeRequest.Id = clientChequeRequestId;
                    srvClientChequeRequest.Load();

                    ChequeRequest chequeRequest = new ChequeRequest();
                    chequeRequest.ProjectId = srvClientChequeRequest.ProjectId;
                    chequeRequest.ChequeRequestReference = srvClientChequeRequest.Reference;
                    chequeRequest.ChequeRequestDate = srvClientChequeRequest.Date;
                    chequeRequest.ChequeRequestDescription = srvClientChequeRequest.Description;
                    chequeRequest.ChequeRequestPayee = srvClientChequeRequest.Payee;
                    chequeRequest.BankId = srvClientChequeRequest.ClientBankId;
                    chequeRequest.ChequeRequestAmount = srvClientChequeRequest.Amount;
                    chequeRequest.IsChequeRequestAuthorised = srvClientChequeRequest.IsAuthorised;
                    chequeRequest.MemberId = srvClientChequeRequest.MemberId;
                    chequeRequest.ClientChequeRequestsIsCredit = srvClientChequeRequest.IsCredit;
                    chequeRequest.ClientChequeRequestsClearanceDaysElec = srvClientChequeRequest.ClearanceDaysElec;
                    chequeRequest.ClientChequeRequestsClearanceDaysChq = srvClientChequeRequest.ClearanceDaysChq;
                    chequeRequest.ClearanceTypeId = srvClientChequeRequest.ClearanceTypeId;

                    returnValue.ChequeRequest = chequeRequest;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region DisbursmentTypeSearch

        /// <summary>
        /// Gets disbursements types for archived.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">List of disbursements type</param>
        /// <param name="criteria">Criteria toget archived disbursements types.</param>
        /// <returns>Returns list of disursement types.</returns>
        public DisbursmentTypeReturnValue DisbursmentTypeSearch(Guid logonId, CollectionRequest collectionRequest, DisbursmentTypeSearchCriteria criteria)
        {
            DisbursmentTypeReturnValue returnValue = new DisbursmentTypeReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of banks
                    DataListCreator<DisbursmentTypeSearchItem> dataListCreator = new DataListCreator<DisbursmentTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvDisbursmentTypeLookup.GetDisbursmentTypes(criteria.IsExternal, criteria.IsIncludeArchived);
                    };

                    // Create the data list
                    returnValue.DisbursementType = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "DisbursmentTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "DisbTypeId"),
                            new ImportMapping("Description", "DisbTypeDescription")
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region LoadClientChequeRequestDetailsForPrinting

        /// <summary>
        /// Loads cheque request details
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="clientChequeRequestId">Client cheque request id toget cheque request details.</param>
        /// <returns>Returns client cheque request details by client cheque request id.</returns>
        public ChequeRequestReturnValue LoadClientChequeRequestDetailsForPrinting(Guid logonId, int clientChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();
           
            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    ChequeRequest clientChequeRequest = new ChequeRequest();

                    DsClientChequeRequestsDetails dsClientChequeRequestsDetails = SrvClientChequeRequestLookup.GetClientChequeRequestDetails(clientChequeRequestId);

                    // Gets printable properties from the dataset
                    foreach (DataRow drClientChequeRequest in dsClientChequeRequestsDetails.uvw_ClientChequeRequestsDetails.Rows)
                    {
                        Guid projectId = (Guid)drClientChequeRequest["projectId"];

                        // Gets the matter details by project id
                        SrvMatter srvMatter = new SrvMatter();
                        srvMatter.Load(projectId);

                        Guid clientId = srvMatter.ClientId;
                        bool isMember = srvMatter.IsMember; 
                        string matterDescription = srvMatter.MatterDescription;
                        string matterReference = srvMatter.MatterReference;

                        // Inserts "-" in between matter reference
                        matterReference = matterReference.Insert(6, "-");

                        string personName = srvMatter.ClientName;

                        // Gets the partner name by partner member id for project id
                        Guid partnerId = srvMatter.MatterPartnerMemberId;
                        string partnerName = SrvEarnerCommon.GetFeeEarnerNameByFeeEarnerId(partnerId);

                        // Client name should be related to project id                        
                        DateTime clientChequeRequestDate = (DateTime)drClientChequeRequest["ClientChequeRequestsDate"];
                        string feeEarnerReference = (string)drClientChequeRequest["feeRef"];
                        string userName = (string)drClientChequeRequest["userName"];
                        string clientChequeRequestDescription = (string)drClientChequeRequest["ClientChequeRequestsDesc"];
                        string clientChequeRequestPayee = (string)drClientChequeRequest["ClientChequeRequestsPayee"];
                        int bankId = (int)drClientChequeRequest["bankId"];
                        string clientChequeRequestBankName = this.GetBankByBankId(bankId);
                        decimal clientChequeRequestAmount = (decimal)drClientChequeRequest["ClientChequeRequestsAmount"];
                        bool isClientChequeAuthorised = (bool)drClientChequeRequest["ClientChequeRequestsIsAuthorised"];
                        string matBranchRef = (string)drClientChequeRequest["matBranchRef"];
                        string ClearanceTypeDesc = (string)drClientChequeRequest["ClearanceTypeDesc"];

                        clientChequeRequest.PersonName = personName;
                        clientChequeRequest.IsChequeRequestAuthorised = isClientChequeAuthorised;
                        clientChequeRequest.ChequeRequestDate = clientChequeRequestDate;
                        clientChequeRequest.ChequeRequestDescription = clientChequeRequestDescription;
                        clientChequeRequest.ChequeRequestPayee = clientChequeRequestPayee;
                        clientChequeRequest.BankName = clientChequeRequestBankName;
                        clientChequeRequest.ChequeRequestAmount = clientChequeRequestAmount;
                        clientChequeRequest.UserName = userName;
                        clientChequeRequest.FeeEarnerReference = feeEarnerReference;
                        clientChequeRequest.MatterReference = matterReference;
                        clientChequeRequest.MatterDescription = matterDescription;
                        clientChequeRequest.PartnerName = partnerName;
                        clientChequeRequest.ClearanceTypeDesc = ClearanceTypeDesc;
                        clientChequeRequest.MatBranchRef = matBranchRef;

                        // Gets address details for client
                        if (isMember)
                        {
                            DsMemAddress dsMemAddress = SrvAddressLookup.GetMemberAddresses(clientId);
                            if (dsMemAddress.Address.Rows.Count > 0)
                            {
                                clientChequeRequest.AddressLine1 = dsMemAddress.Address[0].AddressLine1.ToString().Trim();
                                clientChequeRequest.AddressLine2 = dsMemAddress.Address[0].AddressLine2.ToString().Trim();
                                clientChequeRequest.AddressLine3 = dsMemAddress.Address[0].AddressLine3.ToString().Trim();
                                clientChequeRequest.AddressTown = dsMemAddress.Address[0].AddressTown.ToString().Trim();
                                clientChequeRequest.AddressCounty = dsMemAddress.Address[0].AddressCounty.ToString().Trim();
                                clientChequeRequest.AddressPostcode = dsMemAddress.Address[0].AddressPostCode.ToString().Trim();
                            }
                        }
                        else // if a client is not member client look address detail fo organisation client. 
                        {
                            DsOrgAddress dsOrgAddress = SrvAddressLookup.GetOrganisationAddresses(clientId);
                            if (dsOrgAddress.Address.Rows.Count > 0)
                            {
                                clientChequeRequest.AddressLine1 = dsOrgAddress.Address[0].AddressLine1.ToString().Trim();
                                clientChequeRequest.AddressLine2 = dsOrgAddress.Address[0].AddressLine2.ToString().Trim();
                                clientChequeRequest.AddressLine3 = dsOrgAddress.Address[0].AddressLine3.ToString().Trim();
                                clientChequeRequest.AddressTown = dsOrgAddress.Address[0].AddressTown.ToString().Trim();
                                clientChequeRequest.AddressCounty = dsOrgAddress.Address[0].AddressCounty.ToString().Trim();
                                clientChequeRequest.AddressPostcode = dsOrgAddress.Address[0].AddressPostCode.ToString().Trim();                            
                            }
                        }
                    }

                    returnValue.ChequeRequest = clientChequeRequest;
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

        #endregion

        #region LoadOfficeChequeRequestDetailsForPrinting

        /// <summary>
        /// Loads office cheque request details for printing
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="clientChequeRequestId">Office cheque request id toget cheque request details.</param>
        /// <returns>Returns office cheque request details by office cheque request id.</returns>
        public ChequeRequestReturnValue LoadOfficeChequeRequestDetailsForPrinting(Guid logonId, int officeChequeRequestId)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    ChequeRequest officeChequeRequest = new ChequeRequest();

                    DsOfficeChequeRequests dsOfficeChequeRequestsDetails = SrvOfficeChequeRequestLookup.GetOfficeChequeRequestsByOfficeChequeRequestId(officeChequeRequestId);

                    // Gets printable properties from the dataset
                    foreach (DataRow drOfficeChequeRequest in dsOfficeChequeRequestsDetails.OfficeChequeRequests.Rows)
                    {
                        Guid projectId = (Guid)drOfficeChequeRequest["projectId"];
                        

                        // Gets the matter description by project id
                        SrvMatter srvMatter = new SrvMatter();
                        srvMatter.Load(projectId);

                        Guid clientId = srvMatter.ClientId;
                        string matterDescription = srvMatter.MatterDescription;
                        string matterReference = srvMatter.MatterReference;
                        bool isMember = srvMatter.IsMember; 
                        

                        // Inserts "-" in between matter reference
                        matterReference = matterReference.Insert(6, "-");

                        string personName = srvMatter.ClientName;

                        // Gets the partner name by partner member id for project id
                        Guid partnerId = srvMatter.MatterPartnerMemberId;
                        string partnerName = SrvEarnerCommon.GetFeeEarnerNameByFeeEarnerId(partnerId);

                        // Gets the fee earner name by fee earner id
                        Guid feeEarnerId = srvMatter.FeeEarnerMemberId;
                        string feeEarnerName = SrvEarnerCommon.GetFeeEarnerNameByFeeEarnerId(feeEarnerId);

                        // This member id is from application settings for accounts.
                        Guid userMemberId = (Guid)drOfficeChequeRequest["memberId"];
                        DateTime officeChequeRequestDate = (DateTime)drOfficeChequeRequest["OfficeChequeRequestsDate"];

                        // Gets user name by member id
                        DsSystemUsers dsSystemUsers = SrvUserLookup.GetUser(userMemberId.ToString());
                        string userName = string.Empty;
                        if (dsSystemUsers.uvw_SystemUsers.Count > 0)
                        {
                            userName = dsSystemUsers.uvw_SystemUsers[0].name;
                        }
                        else
                        {
                            DsPersonDealing dsPersonDealing = SrvMatterLookup.GetPersonDealingLookup();
                            for (int index = 0; index < dsPersonDealing.uvw_PersonDealingLookup.Count; index++)
                            {
                                if (dsPersonDealing.uvw_PersonDealingLookup[index].MemberID == userMemberId)
                                {
                                    userName = dsPersonDealing.uvw_PersonDealingLookup[index].name;
                                }
                            }
                        }

                        int bankId = (int)drOfficeChequeRequest["bankId"];
                        string officeChequeRequestDescription = (string)drOfficeChequeRequest["OfficeChequeRequestsDesc"];
                        string officeChequeRequestPayee = (string)drOfficeChequeRequest["OfficeChequeRequestsPayee"];
                        int officeChequeRequestVATRateId = (int)drOfficeChequeRequest["VatRateId"];

                        // Gets VAT rate by VAT rate id
                        DsVatRates dsVATRates = SrvVATRateLookup.GetVatRates(officeChequeRequestVATRateId);
                        string officeChequeRequestVATRateReference = (string)dsVATRates.VatRates[0].VatRateRef;

                        // Gets the bank name by bank id
                        string officeChequeRequestBankName = this.GetBankByBankId(bankId);
                        decimal officeChequeRequestAmount = (decimal)drOfficeChequeRequest["OfficeChequeRequestsAmount"];
                        decimal officeChequeRequestVATAmount = (decimal)drOfficeChequeRequest["OfficeChequeRequestsVATAmount"];
                        bool isOfficeChequeAuthorised = (bool)drOfficeChequeRequest["OfficeChequeRequestsIsAuthorised"];
                        bool isOfficeChequeAnticipated = (bool)drOfficeChequeRequest["OfficeChequeRequestIsAnticipated"];

                        officeChequeRequest.UserName = userName;
                        officeChequeRequest.PersonName = personName;
                        officeChequeRequest.PartnerName = partnerName;
                        officeChequeRequest.FeeEarnerReference = feeEarnerName;
                        officeChequeRequest.BankName = officeChequeRequestBankName;
                        officeChequeRequest.ChequeRequestDate = officeChequeRequestDate;
                        officeChequeRequest.ChequeRequestPayee = officeChequeRequestPayee;
                        officeChequeRequest.VATAmount = officeChequeRequestVATAmount;
                        officeChequeRequest.VATRate = officeChequeRequestVATRateReference;
                        officeChequeRequest.ChequeRequestAmount = officeChequeRequestAmount;
                        officeChequeRequest.MatterDescription = matterDescription;
                        officeChequeRequest.MatterReference = matterReference;
                        officeChequeRequest.IsChequeRequestAuthorised = isOfficeChequeAuthorised;
                        officeChequeRequest.IsChequeRequestAnticipated = isOfficeChequeAnticipated;
                        officeChequeRequest.ChequeRequestDescription = officeChequeRequestDescription;

                        // Gets addressline1,addressline2,addressline3 for client.
                        if (isMember)
                        {
                            DsMemAddress dsMemAddress = SrvAddressLookup.GetMemberAddresses(clientId);
                            if (dsMemAddress.Address.Rows.Count > 0)
                            {
                                officeChequeRequest.AddressLine1 = dsMemAddress.Address[0].AddressLine1.ToString().Trim();
                                officeChequeRequest.AddressLine2 = dsMemAddress.Address[0].AddressLine2.ToString().Trim();
                                officeChequeRequest.AddressLine3 = dsMemAddress.Address[0].AddressLine3.ToString().Trim();
                                officeChequeRequest.AddressTown = dsMemAddress.Address[0].AddressTown.ToString().Trim();
                                officeChequeRequest.AddressCounty = dsMemAddress.Address[0].AddressCounty.ToString().Trim();
                                officeChequeRequest.AddressPostcode = dsMemAddress.Address[0].AddressPostCode.ToString().Trim();
                            }
                        }
                        else // Look for organisation address if the client is not member. 
                        {
                            DsOrgAddress dsOrgAddress = SrvAddressLookup.GetOrganisationAddresses(clientId);
                            if (dsOrgAddress.Address.Rows.Count > 0)
                            {
                                officeChequeRequest.AddressLine1 = dsOrgAddress.Address[0].AddressLine1.ToString().Trim();
                                officeChequeRequest.AddressLine2 = dsOrgAddress.Address[0].AddressLine2.ToString().Trim();
                                officeChequeRequest.AddressLine3 = dsOrgAddress.Address[0].AddressLine3.ToString().Trim();
                                officeChequeRequest.AddressTown = dsOrgAddress.Address[0].AddressTown.ToString().Trim();
                                officeChequeRequest.AddressCounty = dsOrgAddress.Address[0].AddressCounty.ToString().Trim();
                                officeChequeRequest.AddressPostcode = dsOrgAddress.Address[0].AddressPostCode.ToString().Trim();
                            }

                        }
                    }

                    returnValue.ChequeRequest = officeChequeRequest;
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

        #endregion

        #region Cheque Requests Authorisation

        #region GetUnauthorisedClientChequeRequests

        /// <summary>
        /// Gets unauthorised client cheque requests
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">List of unauthorised client cheque requests</param>
        /// <param name="searchCriteria">IsAuthorised and IsPosted flags should be false to get list of 
        /// unauthorised cheque requests</param>
        /// <returns>Returns list of unauthorised client cheque requests</returns>
        public ChequeAuthorisationReturnValue GetUnauthorisedClientChequeRequests(Guid logonId, CollectionRequest collectionRequest, ChequeAuthorisationSearchCriteria searchCriteria)
        {
            ChequeAuthorisationReturnValue returnValue = new ChequeAuthorisationReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {

                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<ChequeAuthorisationSearchItem> dataListCreator = new DataListCreator<ChequeAuthorisationSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsClientChequeRequestsDetails dsClientChequeRequestsDetails = SrvClientChequeRequestLookup.GetClientChequeRequestDetails(searchCriteria.IsAuthorised, searchCriteria.IsPosted, searchCriteria.IncludeDebit, searchCriteria.IncludeCredit);

                        DataSet dsUnauthorisedClientChequeRequests = new DataSet();
                        dsUnauthorisedClientChequeRequests.Tables.Add("UnauthorisedClientChequeRequests");
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClientChequeRequestId", typeof(int));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ProjectId", typeof(Guid));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClientChequeRequestDate", typeof(DateTime));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("UserName", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("MatterReference", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("FeeEarnerReference", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClientChequeRequestDescription", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClientChequeRequestAmount", typeof(decimal));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClientChequeRequestBank", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("IsClientChequeRequestAnticipated", typeof(bool));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("matBranchRef", typeof(string));
                        dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Columns.Add("ClearanceTypeDesc", typeof(string));

                        foreach (DataRow unauthorisedClientChequeRequestRow in dsClientChequeRequestsDetails.uvw_ClientChequeRequestsDetails.Rows)
                        {
                            int clientChequeRequestId = Convert.ToInt32(unauthorisedClientChequeRequestRow["ClientChequeRequestsId"].ToString());
                            DateTime clientChequeRequestDate = (DateTime)unauthorisedClientChequeRequestRow["ClientChequeRequestsDate"];
                            string userName = (string)unauthorisedClientChequeRequestRow["userName"];
                            Guid projectId = (Guid)unauthorisedClientChequeRequestRow["ProjectId"];
                            string matterReference = (string)unauthorisedClientChequeRequestRow["matRef"];
                            string feeEarnerReference = (string)unauthorisedClientChequeRequestRow["feeRef"];
                            string clientChequeRequestDescription = (string)unauthorisedClientChequeRequestRow["ClientChequeRequestsDesc"];
                            decimal clientChequeRequestAmount = (decimal)unauthorisedClientChequeRequestRow["ClientChequeRequestsAmount"];
                            bool isClientChequeRequestAnticipated = (bool)unauthorisedClientChequeRequestRow["ClientChequeRequestsIsArchived"];
                            string matBranchRef = (string)unauthorisedClientChequeRequestRow["matBranchRef"];
                            string ClearanceTypeDesc = (string)unauthorisedClientChequeRequestRow["ClearanceTypeDesc"];

                            clientChequeRequestAmount = Decimal.Round(clientChequeRequestAmount, 2);

                            // Gets the bank name by bank id
                            int bankId = Convert.ToInt32(unauthorisedClientChequeRequestRow["BankId"]);
                            string clientChequeRequestBankName = this.GetBankByBankId(bankId);

                            // Adds unauthorised client cheque requests after manipulating dataset to get bank name by bank id
                            dsUnauthorisedClientChequeRequests.Tables["UnauthorisedClientChequeRequests"].Rows.Add(clientChequeRequestId,
                                                                                                                    projectId,
                                                                                                                    clientChequeRequestDate,
                                                                                                                    userName,
                                                                                                                    matterReference,
                                                                                                                    feeEarnerReference,
                                                                                                                    clientChequeRequestDescription,
                                                                                                                    clientChequeRequestAmount.ToString(),
                                                                                                                    clientChequeRequestBankName,
                                                                                                                    isClientChequeRequestAnticipated, matBranchRef, ClearanceTypeDesc);
                        }

                        e.DataSet = dsUnauthorisedClientChequeRequests;
                    };

                    // Create the data list
                    returnValue.ChequeRequests = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetUnauthorisedClientChequeRequests",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        searchCriteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("UserName", "UserName"),   
                                            new ImportMapping("ProjectId", "ProjectId"),                                            
                                            new ImportMapping("MatterReference","MatterReference"),
                                            new ImportMapping("FeeEarnerReference","FeeEarnerReference"),
                                            new ImportMapping("ChequeRequestId", "ClientChequeRequestId"),  
                                            new ImportMapping("BankName","ClientChequeRequestBank"),  
                                            new ImportMapping("ChequeRequestDate", "ClientChequeRequestDate"),                                            
                                            new ImportMapping("ChequeRequestAmount","ClientChequeRequestAmount"), 
                                            new ImportMapping("ChequeRequestDescription","ClientChequeRequestDescription"),
                                            new ImportMapping("IsChequeRequestAnticipated","IsClientChequeRequestAnticipated") , 
                                            new ImportMapping("MatterBranchRef","matBranchRef"),
                                            new ImportMapping("ClearanceTypeDesc","ClearanceTypeDesc") 
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region GetUnauthorisedOfficeChequeRequests

        /// <summary>
        /// Gets unauthorised office cheque requests
        /// </summary>Function.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid)
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">List of unauthorised client cheque requests</param>
        /// <param name="searchCriteria">IsAuthorised and IsPosted flags should be false to get list of 
        /// unauthorised office cheque requests</param>
        /// <returns>Returns list of unauthorised office cheque requests</returns>
        public ChequeAuthorisationReturnValue GetUnauthorisedOfficeChequeRequests(Guid logonId, CollectionRequest collectionRequest, ChequeAuthorisationSearchCriteria searchCriteria)
        {
            ChequeAuthorisationReturnValue returnValue = new ChequeAuthorisationReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<ChequeAuthorisationSearchItem> dataListCreator = new DataListCreator<ChequeAuthorisationSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsOfficeChequeRequestsDetails dsOfficeChequeRequestsDetails = SrvOfficeChequeRequestLookup.GetOfficeChequeRequestDetails(searchCriteria.IsAuthorised, searchCriteria.IsPosted);

                        DataSet dsUnauthorisedOfficeChequeRequests = new DataSet();
                        dsUnauthorisedOfficeChequeRequests.Tables.Add("UnauthorisedOfficeChequeRequests");
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("OfficeChequeRequestId", typeof(int));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("ProjectId", typeof(Guid));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("OfficeChequeRequestDate", typeof(DateTime));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("UserName", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("MatterReference", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("FeeEarnerReference", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("OfficeChequeRequestDescription", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("OfficeChequeRequestAmount", typeof(decimal));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("VATRateReferernce", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("VATRateAmount", typeof(decimal));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("OfficeChequeRequestBank", typeof(string));
                        dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Columns.Add("IsOfficeChequeRequestAnticipated", typeof(bool));

                        foreach (DataRow unauthorisedClientChequeRequestRow in dsOfficeChequeRequestsDetails.uvw_OfficeChequeRequestsDetails.Rows)
                        {
                            int officeChequeRequestId = Convert.ToInt32(unauthorisedClientChequeRequestRow["OfficeChequeRequestsId"].ToString());
                            DateTime officeChequeRequestDate = (DateTime)unauthorisedClientChequeRequestRow["OfficeChequeRequestsDate"];
                            string userName = (string)unauthorisedClientChequeRequestRow["userName"];
                            Guid projectId = (Guid)unauthorisedClientChequeRequestRow["ProjectId"];
                            string matterReference = (string)unauthorisedClientChequeRequestRow["matRef"];
                            string feeEarnerReference = (string)unauthorisedClientChequeRequestRow["feeRef"];
                            string officeChequeRequestDescription = (string)unauthorisedClientChequeRequestRow["OfficeChequeRequestsDesc"];
                            decimal officeChequeRequestAmount = (decimal)unauthorisedClientChequeRequestRow["OfficeChequeRequestsAmount"];
                            bool isOfficeChequeRequestAnticipated = (bool)unauthorisedClientChequeRequestRow["OfficeChequeRequestIsAnticipated"];
                            string vatRateReference = (string)unauthorisedClientChequeRequestRow["VATRateRef"];
                            decimal vatRateAmount = (decimal)unauthorisedClientChequeRequestRow["OfficeChequeRequestsVATAmount"];

                            officeChequeRequestAmount = Decimal.Round(officeChequeRequestAmount, 2);
                            vatRateAmount = Decimal.Round(vatRateAmount, 2);

                            // Gets the bank name by bank id
                            int bankId = Convert.ToInt32(unauthorisedClientChequeRequestRow["BankId"]);
                            string clientChequeRequestBankName = this.GetBankByBankId(bankId);

                            // Adds unauthorised office cheque requests after manipulating dataset.
                            dsUnauthorisedOfficeChequeRequests.Tables["UnauthorisedOfficeChequeRequests"].Rows.Add(officeChequeRequestId,
                                                                                                                    projectId,
                                                                                                                    officeChequeRequestDate,
                                                                                                                    userName,
                                                                                                                    matterReference,
                                                                                                                    feeEarnerReference,
                                                                                                                    officeChequeRequestDescription,
                                                                                                                    officeChequeRequestAmount.ToString(),
                                                                                                                    vatRateReference,
                                                                                                                    vatRateAmount.ToString(),
                                                                                                                    clientChequeRequestBankName,
                                                                                                                    isOfficeChequeRequestAnticipated);
                        }

                        e.DataSet = dsUnauthorisedOfficeChequeRequests;
                    };

                    // Create the data list
                    returnValue.ChequeRequests = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetUnauthorisedOfficeChequeRequests",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        searchCriteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("UserName", "UserName"),   
                                            new ImportMapping("ProjectId", "ProjectId"),                                            
                                            new ImportMapping("MatterReference","MatterReference"),
                                            new ImportMapping("FeeEarnerReference","FeeEarnerReference"),
                                            new ImportMapping("ChequeRequestId", "OfficeChequeRequestId"),  
                                            new ImportMapping("BankName","OfficeChequeRequestBank"),  
                                            new ImportMapping("ChequeRequestDate", "OfficeChequeRequestDate"),                                            
                                            new ImportMapping("ChequeRequestAmount","OfficeChequeRequestAmount"), 
                                            new ImportMapping("VATRate", "VATRateReferernce"),                                            
                                            new ImportMapping("VATAmount","VATRateAmount"),
                                            new ImportMapping("ChequeRequestDescription","OfficeChequeRequestDescription"),
                                            new ImportMapping("IsChequeRequestAnticipated","IsOfficeChequeRequestAnticipated")     
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #endregion

        #region DeleteChequeRequests

        /// <summary>
        /// Deletes cheque requests for client or office.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="selectedChequeRequests">Collection of cheque requests for client or office for deletion</param>
        /// <param name="isClientChequeRequest">Flag whether deletion is for client or office</param>
        /// <returns>Deletes cheque requests for client or office.</returns>
        public ChequeRequestReturnValue DeleteChequeRequests(Guid logonId, List<int> selectedChequeRequestsIds, bool isClientChequeRequest)
        {
            ChequeRequestReturnValue returnValue = new ChequeRequestReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    if (selectedChequeRequestsIds.Count > 0)
                    {
                        SrvClientChequeRequest srvClientChequeRequest = null;
                        SrvOfficeChequeRequest srvOfficeChequeRequest = null;

                        if (isClientChequeRequest)
                        {
                            srvClientChequeRequest = new SrvClientChequeRequest();
                        }
                        else
                        {
                            srvOfficeChequeRequest = new SrvOfficeChequeRequest();
                        }

                        for (int index = 0; index < selectedChequeRequestsIds.Count; index++)
                        {
                            int chequeRequestId = selectedChequeRequestsIds[index];

                            if (isClientChequeRequest)
                            {
                                srvClientChequeRequest.Delete(chequeRequestId, UserInformation.Instance.UserMemberId, DateTime.Now);
                            }
                            else
                            {
                                srvOfficeChequeRequest.Delete(chequeRequestId, UserInformation.Instance.UserMemberId, DateTime.Now);
                            }
                        }
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

        #endregion

        #region AuthoriseChequeRequests

        /// <summary>
        /// Authorise unauthorised cheque requests for client and office.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="selectedChequeRequestsIds">Selected cheque request ids for authorisation</param>
        /// <param name="isClientChequeRequest"></param>
        /// <returns>Authorise selected unauthorised cheque request ids.</returns>
        public ReturnValue AuthoriseChequeRequests(Guid logonId, List<int> selectedChequeRequestsIds, bool isClientChequeRequest)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);             

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }
                    decimal clientChequeRequestHigherAmount = decimal.Zero;
                    decimal officeChequeRequestHigherAmount = decimal.Zero;

                    
                    if (selectedChequeRequestsIds.Count > 0)
                    {
                        // Verifies whether cheque request is allowed for  authorisation
                        // else it will set error message.
                        if (this.CheckUserAllowedToAuthoriseChqReqHigherAmount(selectedChequeRequestsIds, isClientChequeRequest, out clientChequeRequestHigherAmount, out officeChequeRequestHigherAmount))
                        {
                            for (int index = 0; index < selectedChequeRequestsIds.Count; index++)
                            {
                                int chequeRequestId = Convert.ToInt32(selectedChequeRequestsIds[index].ToString().Trim());

                                if (isClientChequeRequest)
                                {
                                    //if (!UserSecuritySettings.GetUserSecuitySettings(151)
                                    // || !UserSecuritySettings.GetUserSecuitySettings(162))

                                    if (!UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseClientChequeRequest)
                                        || !UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AutomaticallyAuthoriseChequeRequests))
                                        throw new Exception("You do not have sufficient permissions to carry out this request");

                                    SrvClientChequeRequestCommon.AddAuthoriseClientChequeRequests(chequeRequestId, UserInformation.Instance.UserMemberId, DateTime.Now);
                                }
                                else
                                {
                                    //if (!UserSecuritySettings.GetUserSecuitySettings(164)
                                    //|| !UserSecuritySettings.GetUserSecuitySettings(162))
                                    if (!UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseOfficeChequeRequest)
                                            || !UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AutomaticallyAuthoriseChequeRequests))
                                        throw new Exception("You do not have sufficient permissions to carry out this request");

                                    SrvOfficeChequeRequestCommon.AddAuthoriseOfficeChequeRequests(chequeRequestId, UserInformation.Instance.UserMemberId, DateTime.Now);
                                }
                            }
                        }
                        else
                        {
                            if (isClientChequeRequest)
                            {
                                returnValue.Message = "You are not allowed to Authorise Office Cheque Requests over the value of £" +
                                                        clientChequeRequestHigherAmount.ToString("0.00") + ".";
                            }
                            else
                            {
                                returnValue.Message = "You are not allowed to Authorise Office Cheque Requests over the value of £" +
                                                        officeChequeRequestHigherAmount.ToString("0.00") + ".";
                            }
                        }
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

        #endregion

        #region GetDisbursementsDetails

        /// <summary>
        /// Loads disbursement details by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="accounts">Retrieves project id to load disbursement details.</param>
        /// <returns>Loads disbursement details by project id</returns>
        public DisbursementsSearchReturnValue GetDisbursementsDetails(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            DisbursementsSearchReturnValue returnValue = new DisbursementsSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }
                   
                    // Create a data list creator for a list of matters
                    DataListCreator<DisbursementSearchItem> dataListCreator = new DataListCreator<DisbursementSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsDisbLedgerTransactions dsDisbLedgerTransactions = SrvDisbLedgerLookup.GetDisbursementsLedgerTransactions(projectId);

                        DataSet dsDisbursements = new DataSet();
                        dsDisbursements.Tables.Add("Disbursements");
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingId", typeof(int));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingDate", typeof(DateTime));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingReference", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingDisbursementType", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingDescription", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingPaid", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("PostingVAT", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("Amount", typeof(decimal));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("BillingStatus", typeof(string));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("Claimed", typeof(decimal));
                        dsDisbursements.Tables["Disbursements"].Columns.Add("Balance", typeof(decimal));

                        DataView dvDisbursements = new DataView(dsDisbLedgerTransactions.uvw_DisbLedgerTransactions);
                        if (dvDisbursements.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsDisbLedgerTransactions.uvw_DisbLedgerTransactions.Columns["postingDetailsDate"]);
                            dvDisbursements.Sort = columnOrderByPostingDate + " " + "asc";

                            string postingPaid = string.Empty;
                            string unbilledStatus = "Billed";
                            decimal balance = decimal.Zero;
                            decimal unbilled = decimal.Zero;
                            decimal claimed = decimal.Zero;
                            decimal amount = decimal.Zero;

                            foreach (DataRowView disbursementRowView in dvDisbursements)
                            {
                                int postingId = Convert.ToInt32(disbursementRowView.Row["PostingId"].ToString().Trim());
                                DateTime postingDate = (DateTime)disbursementRowView.Row["PostingDetailsDate"];
                                string postingReference = (string)disbursementRowView.Row["PostingDetailsRef"].ToString();
                                string postingDisbursementType = (string)disbursementRowView.Row["DisbTypeDescription"].ToString();
                                string postingDescription = (string)disbursementRowView.Row["PostingDetailsDescription"].ToString();
                                int disbursementStatusId = ((int)disbursementRowView.Row["DisbStatusId"]);

                                switch (disbursementStatusId)
                                {
                                    case 1:
                                        postingPaid = "P";
                                        break;
                                    case 2:
                                        postingPaid = "U";
                                        break;
                                    case 3:
                                        postingPaid = "C";
                                        break;
                                    case 4:
                                        postingPaid = "I";
                                        break;
                                    case 5:
                                        postingPaid = "X";
                                        break;
                                    case 6:
                                        postingPaid = "x";
                                        break;
                                    case 7:
                                        postingPaid = "P";
                                        break;
                                }

                                string postingVAT = string.Empty;
                                int VATRateId = ((int)disbursementRowView.Row["VatRateId"]);
                                if (VATRateId == 1)
                                {
                                    postingVAT = "N";
                                }
                                else
                                {
                                    postingVAT = "V";
                                }

                                amount = (decimal)disbursementRowView.Row["DisbLedgerMasterAmount"] + (decimal)disbursementRowView.Row["DisbLedgerMasterVatAmount"];
                                unbilled = (decimal)disbursementRowView.Row["DisbLedgerMasterAmount"] + (decimal)disbursementRowView.Row["DisbLedgerMasterVatAmount"] - (decimal)disbursementRowView.Row["BillDisbAllocAmount"];
                                claimed = (decimal)disbursementRowView.Row["ClaimedDisbs"];

                                balance = balance + unbilled;
                                amount = Decimal.Round(amount, 2);
                                unbilled = Decimal.Round(unbilled, 2);
                                claimed = Decimal.Round(claimed, 2);
                                balance = Decimal.Round(balance, 2);

                                unbilledStatus = (unbilled != Decimal.Zero ? unbilled.ToString("0.00") : "Billed");

                                dsDisbursements.Tables["Disbursements"].Rows.Add(postingId,
                                                                                 postingDate,
                                                                                 postingReference,
                                                                                 postingDisbursementType,
                                                                                 postingDescription,
                                                                                 postingPaid,
                                                                                 postingVAT,
                                                                                 amount.ToString("0.00"),
                                                                                 unbilledStatus,
                                                                                 claimed.ToString("0.00"),
                                                                                 balance.ToString("0.00"));
                            }
                        }

                        e.DataSet = dsDisbursements;
                    };

                    // Create the data list
                    returnValue.Disbursements = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetDisbursementsDetails",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("PostingType", "PostingDisbursementType"),
                                            new ImportMapping("PostingDescription","PostingDescription"),
                                            new ImportMapping("PostingPaid", "PostingPaid"),
                                            new ImportMapping("PostingVAT","PostingVAT"),
                                            new ImportMapping("Amount","Amount"),    
                                            new ImportMapping("BillingStatus","BillingStatus"),
                                            new ImportMapping("Balance","Balance"),
                                            new ImportMapping("Claimed","Claimed")                                           
                            }
                        );

                    returnValue.Success = true;
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

        #endregion

        #region GetFinancialInfoByProjectId

        /// <summary>
        /// Loads financial information for office, client and deposit by project id.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">Project id usedto retrieve financial information</param>
        /// <returns>Retrieves financial information by project id.</returns>
        public FinancialInfoReturnValue GetFinancialInfoByProjectId(Guid logonId, Guid projectId)
        {
            FinancialInfoReturnValue returnValue = new FinancialInfoReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);                    
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    decimal clientTotalBalance = decimal.Zero;
                    decimal officeTotalBalance = decimal.Zero;
                    decimal depositTotalBalance = decimal.Zero;

                    if (projectId != DataConstants.DummyGuid)
                    {
                        clientTotalBalance = SrvCledgerCommon.SumCledgerAmount(projectId);
                        depositTotalBalance = SrvDledgerCommon.SumDledgerAmount(projectId);

                        decimal billsLedgerAmount = SrvBledgerCommon.SumBledgerAmount(projectId);
                        decimal disbursementLedgerAmount = SrvDisbLedgerCommon.SumDisbLedgerAmount(projectId);
                        decimal disbursementLedgerVATAmount = SrvDisbLedgerCommon.SumDisbLedgerVATAmount(projectId);
                        decimal billsWaveOffLedgerAmount = SrvBillWoLedgerCommon.SumBillWoLedgerrAmount(projectId);
                        decimal payLedgerAmount = SrvPayLedgerCommon.SumPayLedgerAmount(projectId);
                        decimal unpaidDisbursementsAmount = SrvDisbLedgerCommon.SumUnpaidBilledDisbLedgerAmount(projectId);
                        decimal unpaidBilledDisbursementsVATAmount = SrvDisbLedgerCommon.SumUnpaidBilledDisbLedgerVATAmount(projectId);

                        officeTotalBalance += billsLedgerAmount;
                        officeTotalBalance += disbursementLedgerAmount;
                        officeTotalBalance += disbursementLedgerVATAmount;
                        officeTotalBalance += billsWaveOffLedgerAmount;
                        officeTotalBalance += payLedgerAmount;
                        unpaidDisbursementsAmount += unpaidBilledDisbursementsVATAmount;

                        unpaidDisbursementsAmount = decimal.Round(unpaidDisbursementsAmount, 2);

                        if (officeTotalBalance - unpaidDisbursementsAmount < decimal.Zero)
                        {
                            returnValue.WarningMessage = "Office Account Potentially in Credit due to unpaid Billed Disbs";
                        }
                    }

                    returnValue.ClientBalance = Convert.ToString(decimal.Round(clientTotalBalance, 2));
                    returnValue.OfficeBalance = Convert.ToString(decimal.Round(officeTotalBalance, 2));
                    returnValue.DepositBalance = Convert.ToString(decimal.Round(depositTotalBalance, 2));

                    returnValue.Success = true;
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

        #endregion

        #region GetDepositBalancesDetails

        /// <summary>
        /// Gets deposit balances by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Gets collection of deposit by project id</param>
        /// <param name="projectId">Project id requiredto get deposit details</param>
        /// <returns>Retrieves deposit details by project id</returns>
        public BalancesSearchReturnValue GetDepositBalancesDetails(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = new BalancesSearchReturnValue();


            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }
                    // Create a data list creator for a list of deposit made by client
                    DataListCreator<BalancesSearchItem> dataListCreator = new DataListCreator<BalancesSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsDledgerTransactions dsDepositLedgerTransactions = SrvDledgerLookup.GetDledgerTransactions(projectId);

                        DataSet dsDepositBalances = new DataSet();
                        dsDepositBalances.Tables.Add("DepositBalances");
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingId", typeof(int));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingDate", typeof(DateTime));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingReference", typeof(string));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingType", typeof(string));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingDescription", typeof(string));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingBank", typeof(string));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("PostingBankRef", typeof(string));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("Debit", typeof(decimal));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("Credit", typeof(decimal));
                        dsDepositBalances.Tables["DepositBalances"].Columns.Add("Balance", typeof(decimal));

                        DataView dvDepositBalances = new DataView(dsDepositLedgerTransactions.uvw_DledgerTransactions);
                        if (dvDepositBalances.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsDepositLedgerTransactions.uvw_DledgerTransactions.Columns["PostingDetailsDate"]);
                            dvDepositBalances.Sort = columnOrderByPostingDate + " " + "asc";

                            string postingBank = string.Empty;
                            decimal debit = decimal.Zero;
                            decimal credit = decimal.Zero;
                            decimal balance = decimal.Zero;

                            foreach (DataRowView depositBalancesRowView in dvDepositBalances)
                            {
                                int postingId = Convert.ToInt32(depositBalancesRowView.Row["PostingId"].ToString().Trim());
                                DateTime postingDate = (DateTime)depositBalancesRowView.Row["PostingDetailsDate"];
                                string postingReference = (string)depositBalancesRowView.Row["PostingDetailsRef"].ToString();
                                string postingType = (string)depositBalancesRowView.Row["PostingTypesRef"].ToString();
                                string postingDescription = (string)depositBalancesRowView.Row["PostingDetailsDescription"].ToString();
                                string bankReference = (string)depositBalancesRowView.Row["bankRef"].ToString().Trim();
                                if (bankReference == "0")
                                {
                                    postingBank = "N/A";
                                }
                                else
                                {
                                    int bankId = Convert.ToInt32(depositBalancesRowView.Row["bankId"]);
                                    postingBank = this.GetBankByBankId(bankId);
                                }

                                if ((decimal)depositBalancesRowView.Row["DledgerMasterAmount"] < Decimal.Zero)
                                {
                                    // Negative Posting Amount
                                    debit = 0 - (decimal)depositBalancesRowView.Row["DledgerMasterAmount"];
                                    credit = Decimal.Zero;
                                    balance = balance - debit;
                                }
                                else
                                {
                                    // Positive Posting Amount
                                    credit = (decimal)depositBalancesRowView.Row["DledgerMasterAmount"];
                                    debit = Decimal.Zero;
                                    balance = balance + credit;
                                }

                                debit = Decimal.Round(debit, 2);
                                credit = Decimal.Round(credit, 2);
                                balance = Decimal.Round(balance, 2);

                                // setup credit/debit/blance decimal to diaplsy as 0.00 in dataset
                                string strDebit = string.Empty;
                                string strCredit = string.Empty;
                                string strBalance = string.Empty;

                                if (debit == Decimal.Zero)
                                {
                                    strDebit = "0.00";
                                }
                                else
                                {
                                    strDebit = debit.ToString();
                                }
                                if (credit == Decimal.Zero)
                                {
                                    strCredit = "0.00";
                                }
                                else
                                {
                                    strCredit = credit.ToString();
                                }
                                if (balance == Decimal.Zero)
                                {
                                    strBalance = "0.00";
                                }
                                else
                                {
                                    strBalance = balance.ToString();
                                }

                                // Adds deposit balances details to dataset after calculations
                                dsDepositBalances.Tables["DepositBalances"].Rows.Add(postingId,
                                                                                     postingDate,
                                                                                     postingReference,
                                                                                     postingType,
                                                                                     postingDescription,
                                                                                     postingBank,
                                                                                     bankReference,
                                                                                     strDebit,
                                                                                     strCredit,
                                                                                     strBalance);
                            }
                        }

                        e.DataSet = dsDepositBalances;
                    };

                    // Create the data list
                    returnValue.Balances = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "LoadDepositBalancesDetails",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("PostingType", "PostingType"),
                                            new ImportMapping("PostingDescription","PostingDescription"),
                                            new ImportMapping("PostingBank", "PostingBank"),
                                            new ImportMapping("PostingBankRef", "PostingBankRef"),
                                            new ImportMapping("Debit","Debit"),
                                            new ImportMapping("Credit","Credit"),   
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );

                    returnValue.Success = true;
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

        #endregion

        #region GetClientBalancesDetails

        /// <summary>
        /// Gets client balance details by project id.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Gets collection of client balances by project id</param>
        /// <param name="projectId">Project id required to get client balances details</param>
        /// <returns>Retrieves client balances by project id</returns>
        public BalancesSearchReturnValue GetClientBalancesDetails(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = new BalancesSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of deposit made by client
                    DataListCreator<BalancesSearchItem> dataListCreator = new DataListCreator<BalancesSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsCledgerTransactions dsClientLedgerTransactions = SrvCledgerLookup.GetCledgerTransactions(projectId);

                        DataSet dsClientBalances = new DataSet();
                        dsClientBalances.Tables.Add("ClientBalances");
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingId", typeof(int));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingDate", typeof(DateTime));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingReference", typeof(string));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingType", typeof(string));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingDescription", typeof(string));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingBank", typeof(string));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("PostingBankRef", typeof(string));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("Debit", typeof(decimal));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("Credit", typeof(decimal));
                        dsClientBalances.Tables["ClientBalances"].Columns.Add("Balance", typeof(decimal));

                        DataView dvClientBalances = new DataView(dsClientLedgerTransactions.uvw_CledgerTransactions);
                        if (dvClientBalances.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsClientLedgerTransactions.uvw_CledgerTransactions.Columns["PostingDetailsDate"]);
                            dvClientBalances.Sort = columnOrderByPostingDate + " " + "asc";

                            string postingBank = string.Empty;
                            string postingBankRef = string.Empty;
                            decimal debit = decimal.Zero;
                            decimal credit = decimal.Zero;
                            decimal balance = decimal.Zero;

                            foreach (DataRowView clientBalancesRowView in dvClientBalances)
                            {
                                bool addRecord = true;
                                if ((int)clientBalancesRowView.Row["bankTypeID"] != 1 && (int)clientBalancesRowView.Row["PostingTypeId"] != 3)
                                {
                                    if ((int)clientBalancesRowView.Row["PostingTypeId"] != 3)
                                    {
                                        addRecord = false;
                                    }
                                    if ((int)clientBalancesRowView.Row["PostingTypeId"] == 33)
                                    {
                                        addRecord = true;
                                    }
                                    if ((int)clientBalancesRowView.Row["PostingTypeId"] == 41)
                                    {
                                        addRecord = true;
                                    }
                                    if ((int)clientBalancesRowView.Row["PostingTypeId"] == 69 && (int)clientBalancesRowView.Row["bankTypeID"] == 0)
                                    {
                                        addRecord = true;
                                    }
                                }

                                if (addRecord)
                                {
                                    int postingId = Convert.ToInt32(clientBalancesRowView.Row["PostingId"].ToString().Trim());
                                    DateTime postingDate = (DateTime)clientBalancesRowView.Row["PostingDetailsDate"];
                                    string postingReference = (string)clientBalancesRowView.Row["PostingDetailsRef"].ToString();
                                    string postingType = (string)clientBalancesRowView.Row["PostingTypesRef"].ToString(); ;
                                    string postingDescription = (string)clientBalancesRowView.Row["PostingDetailsDescription"].ToString();

                                    if ((string)clientBalancesRowView.Row["bankRef"].ToString().Trim() == "0" ||
                                        (int)clientBalancesRowView.Row["bankTypeID"] == 3)
                                    {
                                        postingBank = "N/A";
                                        postingBankRef = "N/A";
                                    }
                                    else
                                    {
                                        int bankId = Convert.ToInt32(clientBalancesRowView.Row["bankId"]);
                                        postingBank = this.GetBankByBankId(bankId);
                                        postingBankRef = (string)clientBalancesRowView.Row["bankRef"].ToString();
                                    }
                                    if ((decimal)clientBalancesRowView.Row["CledgerMasterAmount"] < Decimal.Zero)
                                    {
                                        // Negative Posting Amount
                                        debit = 0 - (decimal)clientBalancesRowView.Row["CledgerMasterAmount"];
                                        credit = Decimal.Zero;
                                        balance = balance - debit;
                                    }
                                    else
                                    {
                                        // Positive Posting Amount
                                        credit = (decimal)clientBalancesRowView.Row["CledgerMasterAmount"];
                                        debit = Decimal.Zero;
                                        balance = balance + credit;
                                    }

                                    debit = Decimal.Round(debit, 2);
                                    credit = Decimal.Round(credit, 2);
                                    balance = Decimal.Round(balance, 2);

                                    // setup credit/debit/blance decimal to diaplsy as 0.00 in dataset
                                    string strDebit = string.Empty;
                                    string strCredit = string.Empty;
                                    string strBalance = string.Empty;

                                    if (debit == Decimal.Zero)
                                    {
                                        strDebit = "0.00";
                                    }
                                    else
                                    {
                                        strDebit = debit.ToString();
                                    }
                                    if (credit == Decimal.Zero)
                                    {
                                        strCredit = "0.00";
                                    }
                                    else
                                    {
                                        strCredit = credit.ToString();
                                    }
                                    if (balance == Decimal.Zero)
                                    {
                                        strBalance = "0.00";
                                    }
                                    else
                                    {
                                        strBalance = balance.ToString();
                                    }

                                    // Adds deposit details to dataset after calculations
                                    dsClientBalances.Tables["ClientBalances"].Rows.Add(postingId,
                                                                                         postingDate,
                                                                                         postingReference,
                                                                                         postingType,
                                                                                         postingDescription,
                                                                                         postingBank,
                                                                                         postingBankRef,
                                                                                         strDebit,
                                                                                         strCredit,
                                                                                         strBalance);
                                }
                            }
                        }

                        e.DataSet = dsClientBalances;
                    };

                    // Create the data list
                    returnValue.Balances = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "LoadClientBalancesDetails",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("PostingType", "PostingType"),
                                            new ImportMapping("PostingDescription","PostingDescription"),
                                            new ImportMapping("PostingBank", "PostingBank"),
                                            new ImportMapping("PostingBankRef", "PostingBankRef"),
                                            new ImportMapping("Debit","Debit"),
                                            new ImportMapping("Credit","Credit"),   
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );

                    returnValue.Success = true;
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

        #endregion

        #region GetOfficeBalancesDetails

        /// <summary>
        /// Gets offices balances by project id.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Gets collection of office balances by project id</param>
        /// <param name="projectId">Project id required to get office balances details</param>
        /// <returns>Retrieves office balances by project id</returns>
        public BalancesSearchReturnValue GetOfficeBalancesDetails(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BalancesSearchReturnValue returnValue = new BalancesSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of deposit made by client
                    DataListCreator<BalancesSearchItem> dataListCreator = new DataListCreator<BalancesSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DataSet dsOfficeBalances = this.GetOfficeBalances(projectId);

                        // Creates new dataset of same schema as of office balances
                        DataSet dsNewOfficeBalances = dsOfficeBalances.Clone();

                        DataView dvOfficeBalances = new DataView(dsOfficeBalances.Tables["OfficeBalances"]);
                        if (dvOfficeBalances.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsOfficeBalances.Tables["OfficeBalances"].Columns["PostingDate"]);
                            dvOfficeBalances.Sort = columnOrderByPostingDate + " " + "asc";

                            decimal debit = decimal.Zero;
                            decimal credit = decimal.Zero;
                            decimal balance = decimal.Zero;

                            foreach (DataRowView officeBalancesRowView in dvOfficeBalances)
                            {
                                int postingId = Convert.ToInt32(officeBalancesRowView.Row["PostingId"].ToString().Trim());
                                DateTime postingDate = (DateTime)officeBalancesRowView.Row["PostingDate"];
                                string postingReference = (string)officeBalancesRowView.Row["PostingReference"].ToString();
                                string postingType = (string)officeBalancesRowView.Row["PostingType"].ToString(); ;
                                string postingDescription = (string)officeBalancesRowView.Row["PostingDescription"].ToString();
                                string postingBank = (string)officeBalancesRowView.Row["PostingBank"].ToString();
                                string postingBankRef = (string)officeBalancesRowView.Row["PostingBankRef"].ToString();
                                debit = (decimal)officeBalancesRowView.Row["Debit"];
                                credit = (decimal)officeBalancesRowView.Row["Credit"];
                                balance = balance + debit - credit;
                                balance = Decimal.Round(balance, 2);

                                // Adds deposit details to dataset after calculations
                                dsNewOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                                     postingDate,
                                                                                     postingReference,
                                                                                     postingType,
                                                                                     postingDescription,
                                                                                     postingBank,
                                                                                     postingBankRef,
                                                                                     debit.ToString("0.00"),
                                                                                     credit.ToString("0.00"),
                                                                                     balance.ToString("0.00"));
                            }
                        }

                        e.DataSet = dsNewOfficeBalances;
                    };

                    // Create the data list
                    returnValue.Balances = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetOfficeBalancesDetails",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("PostingType", "PostingType"),
                                            new ImportMapping("PostingDescription","PostingDescription"),
                                            new ImportMapping("PostingBank", "PostingBank"),
                                            new ImportMapping("PostingBankRef", "PostingBankRef"),
                                            new ImportMapping("Debit","Debit"),
                                            new ImportMapping("Credit","Credit"),   
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );

                    returnValue.Success = true;
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

        #endregion

        #region GetFinancialBalances

        /// <summary>
        /// Loads financial balances by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">To load financial balances</param>
        /// <returns>Returns properties of financial details</returns>
        public FinancialBalancesReturnValue GetFinancialBalances(Guid logonId, Guid projectId)
        {
            FinancialBalancesReturnValue returnValue = new FinancialBalancesReturnValue();
            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // WIP balances
                    returnValue = this.GetWIPAndTimeTransactionInfo(projectId, "Unbilled", returnValue);

                    // Anticiapted Balances
                    decimal anticipatedDisbursementLedgerBalance = SrvAntiDisbLedgerCommon.SumAntiDisbLedgerAmount(projectId);
                    decimal anticipatedBillBalance = SrvAntiBillCommon.SumAntiBillLedgerAmount(projectId);
                    decimal pfClaimsBalance = SrvLaLedgerCommon.SumLaLedgerForMatter(projectId);

                    returnValue.AnticipatedDisbursements = Convert.ToString(decimal.Round(anticipatedDisbursementLedgerBalance, 2));
                    returnValue.AnticipatedBills = Convert.ToString(decimal.Round(anticipatedBillBalance, 2));
                    returnValue.AnticipatedPFClaims = Convert.ToString(decimal.Round(pfClaimsBalance, 2));

                    // Cost Balances
                    decimal costBilled = SrvBledgerCommon.SumCostBilled(projectId);
                    decimal unbilledPaidDisbursements = SrvDisbLedgerCommon.SumUnbilledPaidDisbLedgerAmount(projectId);
                    decimal unbilledNetTotal = SrvDisbLedgerCommon.SumTotalNetUnbilledDisbLedgerAmount(projectId);
                    unbilledPaidDisbursements = unbilledNetTotal;
                    decimal unpaidBilledDisbursements = SrvDisbLedgerCommon.SumUnpaidBilledDisbLedgerAmount(projectId); ;

                    returnValue.CostBills = Convert.ToString(decimal.Round(costBilled, 2));
                    returnValue.CostUnbilledDisbursements = Convert.ToString(decimal.Round(unbilledPaidDisbursements, 2));
                    returnValue.CostUnpaidBilledDisbursements = Convert.ToString(decimal.Round(unpaidBilledDisbursements, 2));

                    // Time Transactions
                    returnValue = this.GetWIPAndTimeTransactionInfo(projectId, "All", returnValue);

                    // Movement Details                   
                    FinancialBalancesReturnValue lastFinancialDetails = this.GetLastFinancialDetails(projectId);
                    returnValue.LastBill = lastFinancialDetails.LastBill;
                    returnValue.LastFinancial = lastFinancialDetails.LastFinancial;
                    returnValue.LastTime = lastFinancialDetails.LastTime;
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region Bills Ledger

        #region LoadAllBills

        /// <summary>
        /// Loads all the bills by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">To load all the bills</param>
        /// <returns>Loads all the bills by project id</returns>
        public BillsLedgerReturnValue LoadAllBills(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = new BillsLedgerReturnValue();
          
            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                try
                {
                   
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            // Can view own bills
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<BillsLedgerSearchItem> dataListCreator = new DataListCreator<BillsLedgerSearchItem>();
                   
                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsBledgerTransactions dsBledgerTransactions = SrvBledgerLookup.GetBledgerTransactions(projectId);

                        DataSet dsAllBills = new DataSet();
                        dsAllBills.Tables.Add("AllBills");
                        dsAllBills.Tables["AllBills"].Columns.Add("PostingId", typeof(int));
                        dsAllBills.Tables["AllBills"].Columns.Add("PostingDate", typeof(DateTime));
                        dsAllBills.Tables["AllBills"].Columns.Add("PostingReference", typeof(string));
                        dsAllBills.Tables["AllBills"].Columns.Add("PostingType", typeof(string));
                        dsAllBills.Tables["AllBills"].Columns.Add("Debit", typeof(decimal));
                        dsAllBills.Tables["AllBills"].Columns.Add("Credit", typeof(decimal));
                        dsAllBills.Tables["AllBills"].Columns.Add("Paid", typeof(decimal));
                        dsAllBills.Tables["AllBills"].Columns.Add("OutStanding", typeof(decimal));
                        dsAllBills.Tables["AllBills"].Columns.Add("Balance", typeof(decimal));

                        DataView dvAllBills = new DataView(dsBledgerTransactions.uvw_BledgerTransactions);
                        if (dvAllBills.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsBledgerTransactions.uvw_BledgerTransactions.Columns["PostingDetailsDate"]);
                            dvAllBills.Sort = columnOrderByPostingDate + " " + "asc";

                            int factor = 1;
                            string postingBank = string.Empty;
                            string postingType = string.Empty;
                            decimal debit = decimal.Zero;
                            decimal credit = decimal.Zero;
                            decimal balance = decimal.Zero;
                            decimal outStanding = decimal.Zero;
                            decimal paid = decimal.Zero;

                            foreach (DataRowView allBillsRowView in dvAllBills)
                            {
                                int postingId = Convert.ToInt32(allBillsRowView.Row["PostingId"].ToString());
                                DateTime postingDate = (DateTime)allBillsRowView.Row["PostingDetailsDate"];
                                string postingReference = (string)allBillsRowView.Row["PostingDetailsRef"].ToString();

                                if ((decimal)allBillsRowView.Row["BledgerMasterBillAmount"] < Decimal.Zero)
                                {
                                    // Credit note.
                                    credit = Math.Abs((decimal)allBillsRowView.Row["BledgerMasterBillAmount"]);
                                    debit = decimal.Zero;
                                    postingType = "Credit Note";
                                    factor = -1;
                                }
                                else
                                {
                                    // Bill.
                                    debit = Math.Abs((decimal)allBillsRowView.Row["BledgerMasterBillAmount"]);
                                    postingType = "Bill";
                                    credit = decimal.Zero;
                                    factor = 1;
                                }

                                paid = factor * ((decimal)allBillsRowView.Row["BillWoAmount"] + (decimal)allBillsRowView.Row["PayAllocAmount"]);
                                outStanding = factor * (credit + debit - Math.Abs(paid));

                                balance = balance + outStanding;
                                outStanding = Decimal.Round(outStanding, 2);
                                debit = Decimal.Round(debit, 2);
                                credit = Decimal.Round(credit, 2);
                                balance = Decimal.Round(balance, 2);

                                // Adds deposit details to dataset after calculations
                                dsAllBills.Tables["AllBills"].Rows.Add(postingId,
                                                                         postingDate,
                                                                         postingReference,
                                                                         postingType,
                                                                         debit.ToString("0.00"),
                                                                         credit.ToString("0.00"),
                                                                         paid.ToString("0.00"),
                                                                         outStanding.ToString("0.00"),
                                                                         balance.ToString("0.00"));
                            }
                        }

                        e.DataSet = dsAllBills;
                    };

                    // Create the data list
                    returnValue.Bills = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "LoadAllBills",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("BillDate", "PostingDate"),
                                            new ImportMapping("BillReference", "PostingReference"),
                                            new ImportMapping("BillType", "PostingType"),                                                                                        
                                            new ImportMapping("Debit","Debit"),
                                            new ImportMapping("Credit","Credit"),   
                                            new ImportMapping("Paid","Paid"), 
                                            new ImportMapping("OutStanding","OutStanding"), 
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region LoadWriteOffBills

        /// <summary>
        /// Loads write off bills by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">To load write off bills</param>
        /// <returns>Loads write off bills by project id</returns>
        public BillsLedgerReturnValue LoadWriteOffBills(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = new BillsLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            // Can view own bills
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<BillsLedgerSearchItem> dataListCreator = new DataListCreator<BillsLedgerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsBillWoTransactions dsBillWoTransactions = SrvBillWoLedgerLookup.GetBillWoLedgerTransactions(projectId);

                        DataSet dsWriteOffBills = new DataSet();
                        dsWriteOffBills.Tables.Add("WriteOffBills");
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("PostingId", typeof(int));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("PostingDate", typeof(DateTime));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("PostingReference", typeof(string));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("PostingType", typeof(string));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("Debit", typeof(decimal));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("Credit", typeof(decimal));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("Paid", typeof(decimal));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("OutStanding", typeof(decimal));
                        dsWriteOffBills.Tables["WriteOffBills"].Columns.Add("Balance", typeof(decimal));

                        DataView dvWriteOffBills = new DataView(dsBillWoTransactions.uvw_BillWoTransactions);
                        if (dvWriteOffBills.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsBillWoTransactions.uvw_BillWoTransactions.Columns["PostingDetailsDate"]);
                            dvWriteOffBills.Sort = columnOrderByPostingDate + " " + "asc";

                            int factor = 1;
                            string postingBank = string.Empty;
                            string postingType = string.Empty;
                            decimal debit = decimal.Zero;
                            decimal credit = decimal.Zero;
                            decimal balance = decimal.Zero;
                            decimal outStanding = decimal.Zero;
                            decimal paid = decimal.Zero;

                            foreach (DataRowView writeOfBillsRowView in dvWriteOffBills)
                            {
                                int postingId = Convert.ToInt32(writeOfBillsRowView.Row["PostingId"].ToString());
                                DateTime postingDate = (DateTime)writeOfBillsRowView.Row["PostingDetailsDate"];
                                string postingReference = (string)writeOfBillsRowView.Row["PostingDetailsRef"].ToString();
                                postingType = "Write Off";

                                if ((decimal)writeOfBillsRowView.Row["BillWoLedgerWorkingAmount"] < Decimal.Zero)
                                {
                                    // Credit note.
                                    credit = Math.Abs((decimal)writeOfBillsRowView.Row["BillWoLedgerWorkingAmount"]);
                                    debit = decimal.Zero;
                                    factor = -1;
                                }
                                else
                                {
                                    // Bill.
                                    debit = Math.Abs((decimal)writeOfBillsRowView.Row["BillWoLedgerWorkingAmount"]);
                                    credit = decimal.Zero;
                                    factor = 1;
                                }

                                paid = factor * Math.Abs((decimal)writeOfBillsRowView.Row["BillWoAmount"]);
                                outStanding = factor * (debit + credit - Math.Abs(paid));

                                balance = balance + outStanding;
                                outStanding = Decimal.Round(outStanding, 2);
                                debit = Decimal.Round(debit, 2);
                                credit = Decimal.Round(credit, 2);
                                balance = Decimal.Round(balance, 2);

                                // Adds deposit details to dataset after calculations
                                dsWriteOffBills.Tables["WriteOffBills"].Rows.Add(postingId,
                                                                             postingDate,
                                                                             postingReference,
                                                                             postingType,
                                                                             debit.ToString("0.00"),
                                                                             credit.ToString("0.00"),
                                                                             paid.ToString("0.00"),
                                                                             outStanding.ToString("0.00"),
                                                                             balance.ToString("0.00"));
                            }
                        }

                        e.DataSet = dsWriteOffBills;
                    };

                    // Create the data list
                    returnValue.Bills = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "LoadWriteOffBills",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("BillDate", "PostingDate"),
                                            new ImportMapping("BillReference", "PostingReference"),
                                            new ImportMapping("BillType", "PostingType"),                                                                                        
                                            new ImportMapping("Debit","Debit"),
                                            new ImportMapping("Credit","Credit"),   
                                            new ImportMapping("Paid","Paid"), 
                                            new ImportMapping("OutStanding","OutStanding"), 
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region LoadUnclearedBills

        /// <summary>
        /// Loads uncleared bills by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">To load uncleared bills</param>
        /// <returns>Loads uncleared bills by project id</returns>
        public BillsLedgerReturnValue LoadUnclearedBills(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            BillsLedgerReturnValue returnValue = new BillsLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                        // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            // Can view own bills
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<BillsLedgerSearchItem> dataListCreator = new DataListCreator<BillsLedgerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsBledgerTransactions dsBledgerTransactions = SrvBledgerLookup.GetBledgerTransactions(projectId);

                        DataSet dsUnclearedBills = new DataSet();
                        dsUnclearedBills.Tables.Add("UnclearedBills");
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("PostingId", typeof(int));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("PostingDate", typeof(DateTime));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("PostingReference", typeof(string));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("PostingType", typeof(string));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("Amount", typeof(decimal));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("Uncleared", typeof(decimal));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("VATAmount", typeof(decimal));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("Disbursements", typeof(decimal));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("Costs", typeof(decimal));
                        dsUnclearedBills.Tables["UnclearedBills"].Columns.Add("Balance", typeof(decimal));

                        DataView dvUnclearedBills = new DataView(dsBledgerTransactions.uvw_BledgerTransactions);
                        if (dvUnclearedBills.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsBledgerTransactions.uvw_BledgerTransactions.Columns["PostingDetailsDate"]);
                            dvUnclearedBills.Sort = columnOrderByPostingDate + " " + "asc";

                            int factor = 1;
                            string postingBank = string.Empty;
                            string postingType = string.Empty;
                            decimal amount = decimal.Zero;
                            decimal uncleared = decimal.Zero;
                            decimal vatAmount = decimal.Zero;
                            decimal disbursement = decimal.Zero;
                            decimal costs = decimal.Zero;
                            decimal balance = decimal.Zero;
                            DateTime postingDate = DataConstants.BlankDate;

                            foreach (DataRowView unclearedBillsRowView in dvUnclearedBills)
                            {
                                int postingId = Convert.ToInt32(unclearedBillsRowView.Row["PostingId"].ToString());
                                postingDate = (DateTime)unclearedBillsRowView.Row["PostingDetailsDate"];

                                string postingReference = (string)unclearedBillsRowView.Row["PostingDetailsRef"].ToString();

                                if ((decimal)unclearedBillsRowView.Row["BledgerMasterBillAmount"] < Decimal.Zero)
                                {
                                    // Credit note.
                                    postingType = "Credit Note";
                                    factor = -1;
                                }
                                else
                                {
                                    // Bill.
                                    postingType = "Bill";
                                    factor = 1;
                                }

                                amount = (decimal)unclearedBillsRowView.Row["BledgerMasterBillAmount"];
                                uncleared = factor * (Math.Abs((decimal)unclearedBillsRowView.Row["BledgerMasterBillAmount"]) - (decimal)unclearedBillsRowView.Row["BillWoAmount"] - (decimal)unclearedBillsRowView.Row["PayAllocAmount"]);
                                vatAmount = factor * (Math.Abs((decimal)unclearedBillsRowView.Row["BledgerMasterVatAmount"]) - (decimal)unclearedBillsRowView.Row["PayAllocVatAmount"] - (decimal)unclearedBillsRowView.Row["BillWoVatAmount"]);
                                disbursement = factor * (Math.Abs((decimal)unclearedBillsRowView.Row["BledgerMasterVatDisbTot"]) + Math.Abs((decimal)unclearedBillsRowView.Row["BledgerMasterNVatDisbTot"]) - (decimal)unclearedBillsRowView.Row["PayDisbAmount"] - (decimal)unclearedBillsRowView.Row["BillWoDisbAmount"]);
                                costs = factor * (Math.Abs((decimal)unclearedBillsRowView.Row["BledgerMasterProfit"]) - (decimal)unclearedBillsRowView.Row["PayAllocProfitAmount"] - (decimal)unclearedBillsRowView.Row["BillWoProfitAmount"]);
                                balance = balance + uncleared;

                                uncleared = Decimal.Round(uncleared, 2);
                                vatAmount = Decimal.Round(vatAmount, 2);
                                disbursement = Decimal.Round(disbursement, 2);
                                costs = Decimal.Round(costs, 2);
                                balance = Decimal.Round(balance, 2);

                                // Adds deposit details to dataset after calculations
                                dsUnclearedBills.Tables["UnclearedBills"].Rows.Add(postingId,
                                                                             postingDate,
                                                                             postingReference,
                                                                             postingType,
                                                                             amount.ToString("0.00"),
                                                                             uncleared.ToString("0.00"),
                                                                             vatAmount.ToString("0.00"),
                                                                             disbursement.ToString("0.00"),
                                                                             costs.ToString("0.00"),
                                                                             balance.ToString("0.00"));
                            }
                        }

                        e.DataSet = dsUnclearedBills;
                    };

                    // Create the data list
                    returnValue.Bills = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "LoadUnclearedBills",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("BillDate", "PostingDate"),
                                            new ImportMapping("BillReference", "PostingReference"),
                                            new ImportMapping("BillType", "PostingType"),                                                                                        
                                            new ImportMapping("Amount","Amount"),
                                            new ImportMapping("Uncleared","Uncleared"),
                                            new ImportMapping("VATAmount","VATAmount"),
                                            new ImportMapping("Disbursements","Disbursements"),
                                            new ImportMapping("Costs","Costs"),                                                                                        
                                            new ImportMapping("Balance","Balance")                                           
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
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
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="projectId">To load all the bills</param>
        /// <returns>Loads all the bills by project id</returns>
        public TimeLedgerSearchReturnValue GetTimeLedger(Guid logonId, CollectionRequest collectionRequest, string timeFilter, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = new TimeLedgerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<TimeLedgerSearchItem> dataListCreator = new DataListCreator<TimeLedgerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        NetTimeTransactionsReturnValue totalTimeTransactions = new NetTimeTransactionsReturnValue();
                        DsTimeTransactions dsTimeTransactions = SrvTimeLookup.GetTimeTransactions(projectId, timeFilter, false);

                        DataSet dsAllTimeTransactions = new DataSet();
                        dsAllTimeTransactions.Tables.Add("AllTimeTransactions");
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("TimeId", typeof(int));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("TimeDate", typeof(DateTime));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("TimeType", typeof(string));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("FeeEarnerReference", typeof(string));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("Time", typeof(string));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("TotalTimeElapsed", typeof(int));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("Cost", typeof(decimal));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("CostBalance", typeof(decimal));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("Charge", typeof(decimal));
                        dsAllTimeTransactions.Tables["AllTimeTransactions"].Columns.Add("ChargeBalance", typeof(decimal));

                        int timeElapsed = 0;
                        decimal cost = decimal.Zero;
                        decimal costBalance = decimal.Zero;
                        decimal charge = decimal.Zero;
                        decimal chargeBalance = decimal.Zero;

                        DataView dvAllTimeTransactions = new DataView(dsTimeTransactions.uvw_TimeTransactions);
                        if (dvAllTimeTransactions.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsTimeTransactions.uvw_TimeTransactions.Columns["TimeDate"]);
                            dvAllTimeTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                            foreach (DataRowView allTimeTransactionsRowView in dvAllTimeTransactions)
                            {
                                int timeId = Convert.ToInt32(allTimeTransactionsRowView.Row["TimeId"].ToString());

                                DateTime timeDate = (DateTime)allTimeTransactionsRowView.Row["TimeDate"];
                                string timeType = (string)allTimeTransactionsRowView.Row["TimeTypeDescription"].ToString();
                                string time = this.ConvertUnits((int)allTimeTransactionsRowView.Row["TimeElapsed"], ApplicationSettings.Instance.TimeUnits);
                                timeElapsed = timeElapsed + (int)allTimeTransactionsRowView.Row["TimeElapsed"];
                                int totalTimeElapsed = timeElapsed;
                                string feeEarnerReference = (string)allTimeTransactionsRowView.Row["feeRef"].ToString();
                                cost = (decimal)allTimeTransactionsRowView.Row["TimeCost"];
                                charge = (decimal)allTimeTransactionsRowView.Row["TimeCharge"];
                                costBalance = costBalance + cost;
                                chargeBalance = chargeBalance + charge;
                                cost = Decimal.Round(cost, 2);
                                charge = Decimal.Round(charge, 2);
                                chargeBalance = Decimal.Round(chargeBalance, 2);
                                costBalance = Decimal.Round(costBalance, 2);

                                if ((Convert.ToDecimal(allTimeTransactionsRowView.Row["ClaimedChargeAmount"]) == Convert.ToDecimal(allTimeTransactionsRowView.Row["TimeCharge"])) &&
                                    (Convert.ToDecimal(allTimeTransactionsRowView.Row["ClaimedCostAmount"]) == Convert.ToDecimal(allTimeTransactionsRowView.Row["TimeCost"])) &&
                                    (Convert.ToInt32(allTimeTransactionsRowView.Row["ClaimedCount"]) > 0))
                                {
                                    // Adds time transaction details to dataset after calculations
                                    dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Add(timeId,
                                                                                                 timeDate.ToShortDateString(),
                                                                                                 timeType,
                                                                                                 feeEarnerReference,
                                                                                                 time,
                                                                                                 totalTimeElapsed,
                                                                                                 cost.ToString("0.00"),
                                                                                                 costBalance.ToString("0.00"),
                                                                                                 charge.ToString("0.00"),
                                                                                                 chargeBalance.ToString("0.00"));

                                }
                                else if ((int)allTimeTransactionsRowView.Row["WoffCount"] > 0)
                                {
                                    // Time is wrtten off! 
                                    // Adds time transaction details to dataset after calculations
                                    dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Add(timeId,
                                                                                                 timeDate.ToShortDateString(),
                                                                                                 timeType,
                                                                                                 feeEarnerReference,
                                                                                                 time,
                                                                                                 totalTimeElapsed,
                                                                                                 cost.ToString("0.00"),
                                                                                                 costBalance.ToString("0.00"),
                                                                                                 charge.ToString("0.00"),
                                                                                                 chargeBalance.ToString("0.00"));

                                }
                                else if ((Convert.ToDecimal(allTimeTransactionsRowView.Row["BillTimeChargeAllocAmount"]) >= Convert.ToDecimal(allTimeTransactionsRowView.Row["TimeCharge"])) &&
                                         (Convert.ToDecimal(allTimeTransactionsRowView.Row["BillTimeCostAllocAmount"]) >= Convert.ToDecimal(allTimeTransactionsRowView.Row["TimeCost"])) &&
                                         (Convert.ToInt32(allTimeTransactionsRowView.Row["BilledCount"]) > 0))
                                {
                                    //there has been some allocations done so the time must be billed
                                    dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Add(timeId,
                                                                                                 timeDate.ToShortDateString(),
                                                                                                 timeType,
                                                                                                 feeEarnerReference,
                                                                                                 time,
                                                                                                 totalTimeElapsed,
                                                                                                 cost.ToString("0.00"),
                                                                                                 costBalance.ToString("0.00"),
                                                                                                 charge.ToString("0.00"),
                                                                                                 chargeBalance.ToString("0.00"));
                                }
                                else
                                {
                                    // no time allocations doen so time must be unbilled
                                    //create listview items
                                    //there has been some allocations done so the time must be billed
                                    dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Add(timeId,
                                                                                                 timeDate.ToShortDateString(),
                                                                                                 timeType,
                                                                                                 feeEarnerReference,
                                                                                                 time,
                                                                                                 totalTimeElapsed,
                                                                                                 cost.ToString("0.00"),
                                                                                                 costBalance.ToString("0.00"),
                                                                                                 charge.ToString("0.00"),
                                                                                                 chargeBalance.ToString("0.00"));
                                }
                            }

                            if (dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Count > 0)
                            {
                                int rowCount = dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows.Count;
                                timeElapsed = (int)dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows[rowCount - 1]["TotalTimeElapsed"];
                                chargeBalance = (decimal)dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows[rowCount - 1]["ChargeBalance"];
                                costBalance = (decimal)dsAllTimeTransactions.Tables["AllTimeTransactions"].Rows[rowCount - 1]["CostBalance"];

                                if (timeFilter.Trim() == "All")
                                {
                                    totalTimeTransactions.TotalCharge = chargeBalance;
                                    totalTimeTransactions.TotalCost = costBalance;
                                    totalTimeTransactions.TotalTimeElapsed = this.ConvertUnits(timeElapsed, ApplicationSettings.Instance.TimeUnits);
                                }

                                // These properties need to be set for time filter for "All" and rest of the time filters
                                totalTimeTransactions.FilterTime = this.ConvertUnits(timeElapsed, ApplicationSettings.Instance.TimeUnits);
                                totalTimeTransactions.FilterCost = costBalance;
                                totalTimeTransactions.FilterCharge = chargeBalance;

                                returnValue.TotalTimeTransactions = totalTimeTransactions;
                            }
                        }

                        e.DataSet = dsAllTimeTransactions;
                    };

                    // Create the data list
                    returnValue.TimeTransactions = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetTimeLedger",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("TimeId", "TimeId"),
                                            new ImportMapping("TimeDate", "TimeDate"),
                                            new ImportMapping("TimeType", "TimeType"),                                            
                                            new ImportMapping("FeeEarnerReference", "FeeEarnerReference"),                                                                                        
                                            new ImportMapping("Time","Time"),
                                            new ImportMapping("Cost","Cost"),   
                                            new ImportMapping("CostBalance","CostBalance"), 
                                            new ImportMapping("Charge","Charge"), 
                                            new ImportMapping("ChargeBalance","ChargeBalance"),
                                            new ImportMapping("TotalTimeElapsed", "TotalTimeElapsed")                   
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region GetWriteOffTimeTransactions

        /// <summary>
        /// Loads write off time transactions by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Loads listof write off time transactions</param>
        /// <param name="projectId">To load write off transactions.</param>
        /// <returns>Loads write off time transactions</returns>
        public TimeLedgerSearchReturnValue GetWriteOffTimeTransactions(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = new TimeLedgerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<TimeLedgerSearchItem> dataListCreator = new DataListCreator<TimeLedgerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsTimeWoffGroupedTransactions dsTimeWoffGroupedTransactions = SrvTimeWoffLookup.GetTimeWoffGroupedTransactions(projectId);

                        DataSet dsTimeWriteOffTransactions = new DataSet();
                        dsTimeWriteOffTransactions.Tables.Add("TimeWriteOffTransactions");
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("Postingid", typeof(int));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("PostingDate", typeof(DateTime));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("PostingReference", typeof(string));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("PostingDescription", typeof(string));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("Time", typeof(string));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("Charge", typeof(decimal));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("Cost", typeof(decimal));
                        dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Columns.Add("CostBalance", typeof(decimal));

                        DataView dvTimeWriteOffTransactions = new DataView(dsTimeWoffGroupedTransactions.uvw_TimeWoffGroupedTransactions);
                        if (dvTimeWriteOffTransactions.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsTimeWoffGroupedTransactions.uvw_TimeWoffGroupedTransactions.Columns["PostingDetailsDate"]);
                            dvTimeWriteOffTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                            decimal chargeAmount = decimal.Zero;
                            decimal costAmount = decimal.Zero;

                            foreach (DataRowView timeWriteOffTransactionsRowView in dvTimeWriteOffTransactions)
                            {
                                if ((string)timeWriteOffTransactionsRowView.Row["PostingTypesRef"].ToString().Trim() == "TWO")
                                {
                                    int postingId = Convert.ToInt32(timeWriteOffTransactionsRowView.Row["PostingId"]);

                                    DateTime postingDate = (DateTime)timeWriteOffTransactionsRowView.Row["PostingDetailsDate"];
                                    string postingReference = (string)timeWriteOffTransactionsRowView.Row["PostingDetailsRef"].ToString();
                                    string postingDescription = (string)timeWriteOffTransactionsRowView.Row["PostingDetailsDescription"].ToString();
                                    chargeAmount = (decimal)timeWriteOffTransactionsRowView.Row["TimeWoffMasterChargeAmount"];
                                    costAmount = (decimal)timeWriteOffTransactionsRowView.Row["TimeWoffMasterCostAmount"];
                                    string time = this.ConvertUnits((int)timeWriteOffTransactionsRowView.Row["TimeWoffMintues"], ApplicationSettings.Instance.TimeUnits);

                                    //create datatable of time write off transactions
                                    dsTimeWriteOffTransactions.Tables["TimeWriteOffTransactions"].Rows.Add(postingId,
                                                                                                            postingDate.ToShortDateString(),
                                                                                                            postingReference,
                                                                                                            postingDescription,
                                                                                                            time,
                                                                                                            chargeAmount.ToString("0.00"),
                                                                                                            costAmount.ToString("0.00"));
                                }
                            }
                        }

                        e.DataSet = dsTimeWriteOffTransactions;
                    };

                    // Create the data list
                    returnValue.TimeTransactions = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetWriteOffTimeTransactions",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("Time", "Time"),                                                                                        
                                            new ImportMapping("Charge","Charge"),
                                            new ImportMapping("Cost","Cost"),                                                                                         
                                            new ImportMapping("PostingDescription","PostingDescription")  
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region GetWriteOffReversalTimeTransactions

        /// <summary>
        /// Loads write off time transactions by project id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Loads listof write off time transactions</param>
        /// <param name="projectId">To load write off transactions.</param>
        /// <returns>Loads write off time transactions</returns>
        public TimeLedgerSearchReturnValue GetWriteOffReversalTimeTransactions(Guid logonId, CollectionRequest collectionRequest, Guid projectId)
        {
            TimeLedgerSearchReturnValue returnValue = new TimeLedgerSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<TimeLedgerSearchItem> dataListCreator = new DataListCreator<TimeLedgerSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsTimeWoffGroupedTransactions dsTimeWoffReversalGroupedTransactions = SrvTimeWoffLookup.GetTimeWoffGroupedTransactions(projectId);

                        DataSet dsTimeWriteOffReversalTransactions = new DataSet();
                        dsTimeWriteOffReversalTransactions.Tables.Add("TimeWriteOffReversalTransactions");
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("Postingid", typeof(int));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("PostingDate", typeof(DateTime));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("PostingReference", typeof(string));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("PostingDescription", typeof(string));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("Time", typeof(string));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("Charge", typeof(decimal));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("Cost", typeof(decimal));
                        dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Columns.Add("CostBalance", typeof(decimal));

                        DataView dvTimeWriteOffReversalTransactions = new DataView(dsTimeWoffReversalGroupedTransactions.uvw_TimeWoffGroupedTransactions);
                        if (dvTimeWriteOffReversalTransactions.Count != 0)
                        {
                            string columnOrderByPostingDate = Convert.ToString(dsTimeWoffReversalGroupedTransactions.uvw_TimeWoffGroupedTransactions.Columns["PostingDetailsDate"]);
                            dvTimeWriteOffReversalTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                            decimal chargeAmount = decimal.Zero;
                            decimal costAmount = decimal.Zero;

                            foreach (DataRowView timeWriteOffReversalTransactionsRowView in dvTimeWriteOffReversalTransactions)
                            {
                                if ((string)timeWriteOffReversalTransactionsRowView.Row["PostingTypesRef"].ToString().Trim() == "TWR")
                                {
                                    int postingId = Convert.ToInt32(timeWriteOffReversalTransactionsRowView.Row["PostingId"]);

                                    DateTime postingDate = (DateTime)timeWriteOffReversalTransactionsRowView.Row["PostingDetailsDate"];
                                    string postingReference = (string)timeWriteOffReversalTransactionsRowView.Row["PostingDetailsRef"].ToString();
                                    string postingDescription = (string)timeWriteOffReversalTransactionsRowView.Row["PostingDetailsDescription"].ToString();
                                    chargeAmount = (decimal)timeWriteOffReversalTransactionsRowView.Row["TimeWoffMasterChargeAmount"];
                                    costAmount = (decimal)timeWriteOffReversalTransactionsRowView.Row["TimeWoffMasterCostAmount"];
                                    string time = this.ConvertUnits((int)timeWriteOffReversalTransactionsRowView.Row["TimeWoffMintues"], ApplicationSettings.Instance.TimeUnits);

                                    //create datatable of time write off reversal transactions
                                    dsTimeWriteOffReversalTransactions.Tables["TimeWriteOffReversalTransactions"].Rows.Add(postingId,
                                                                                                                            postingDate.ToShortDateString(),
                                                                                                                            postingReference,
                                                                                                                            postingDescription,
                                                                                                                            time,
                                                                                                                            chargeAmount.ToString("0.00"),
                                                                                                                            costAmount.ToString("0.00"));
                                }
                            }
                        }

                        e.DataSet = dsTimeWriteOffReversalTransactions;
                    };

                    // Create the data list
                    returnValue.TimeTransactions = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetWriteOffReversalTimeTransactions",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        projectId.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("PostingId", "PostingId"),
                                            new ImportMapping("PostingDate", "PostingDate"),
                                            new ImportMapping("PostingReference", "PostingReference"),
                                            new ImportMapping("Time", "Time"),                                                                                        
                                            new ImportMapping("Charge","Charge"),
                                            new ImportMapping("Cost","Cost"),                                                                                          
                                            new ImportMapping("PostingDescription","PostingDescription")  
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #endregion

        #region Draft Bills

        /// <summary>
        /// Gets unposted draft bills 
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">List of unposted draft bills</param>
        /// <returns>Returns list of unposted draft bills</returns>
        public DraftBillSearchReturnValue GetUnpostedDraftBills(Guid logonId, CollectionRequest collectionRequest)
        {
            DraftBillSearchReturnValue returnValue = new DraftBillSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of bills 
                    DataListCreator<DraftBillSearchItem> dataListCreator = new DataListCreator<DraftBillSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsDraftBills dsDraftBills = SrvDraftBillLookup.GetUnPostedDraftBills();

                        DataSet dsUnpostedDraftBills = new DataSet();
                        dsUnpostedDraftBills.Tables.Add("UnpostedDraftBills");
                        dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Columns.Add("DraftBillId", typeof(int));
                        dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Columns.Add("DraftBillDate", typeof(DateTime));
                        dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Columns.Add("UserName", typeof(string));
                        dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Columns.Add("MatterReference", typeof(string));
                        dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Columns.Add("DraftBillDescription", typeof(string));

                        DataView dvUnpostedDraftBills = new DataView(dsDraftBills.DraftBills);
                        if (dvUnpostedDraftBills.Count != 0)
                        {
                            string columnOrderByDraftBillsDate = Convert.ToString(dsDraftBills.DraftBills.Columns["DraftBillsDate"]);
                            dvUnpostedDraftBills.Sort = columnOrderByDraftBillsDate + " " + "asc";


                            foreach (DataRowView unpostedDraftBillsRowView in dvUnpostedDraftBills)
                            {
                                int draftBillId = (int)unpostedDraftBillsRowView.Row["DraftBillsId"];

                                Guid projectId = (Guid)unpostedDraftBillsRowView.Row["ProjectId"];
                                Guid userMemberId = (Guid)unpostedDraftBillsRowView.Row["UserMemberId"];

                                // Gets matter reference by project id
                                string matterReference = SrvMatterCommon.GetMatterReference(projectId);

                                // Inserts "-" in between matter reference
                                matterReference = matterReference.Insert(6, "-");

                                // Gets user name by member id
                                DsSystemUsers dsSystemUsers = SrvUserLookup.GetUser(userMemberId.ToString());

                                string userName = string.Empty;
                                if (dsSystemUsers.uvw_SystemUsers.Count > 0)
                                {
                                    userName = dsSystemUsers.uvw_SystemUsers[0].name;
                                }
                                else
                                {
                                    DsPersonDealing dsPersonDealing = SrvMatterLookup.GetPersonDealingLookup();
                                    for (int index = 0; index < dsPersonDealing.uvw_PersonDealingLookup.Count; index++)
                                    {
                                        if (dsPersonDealing.uvw_PersonDealingLookup[index].MemberID == userMemberId)
                                        {
                                            userName = dsPersonDealing.uvw_PersonDealingLookup[index].name;
                                        }
                                    }
                                }

                                DateTime draftBillDate = (DateTime)unpostedDraftBillsRowView.Row["DraftBillsDate"];
                                string draftBillDescription = (string)unpostedDraftBillsRowView.Row["PostingDescDetails"];





                                // Create dataset for sorted unposted draft bills
                                dsUnpostedDraftBills.Tables["UnpostedDraftBills"].Rows.Add(draftBillId,
                                                                                            draftBillDate.ToShortDateString(),
                                                                                            userName,
                                                                                            matterReference,
                                                                                            draftBillDescription);

                            }
                        }

                        e.DataSet = dsUnpostedDraftBills;
                    };

                    // Create the data list
                    returnValue.UnpostedDraftBills = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetUnpostedDraftBills",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        string.Empty,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                                            new ImportMapping("DraftBillId", "DraftBillId"),
                                            new ImportMapping("DraftBillDate", "DraftBillDate"),
                                            new ImportMapping("UserName", "UserName"),
                                            new ImportMapping("MatterReference", "MatterReference"),                                                                                        
                                            new ImportMapping("DraftBillDescription","DraftBillDescription")                                          
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        /// <summary>
        /// Submits selected draft bills
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="selectedDraftBillIds"></param>
        /// <returns></returns>
        public DraftBillReturnValue SubmitDraftBill(Guid logonId, List<int> selectedDraftBillIds)
        {
            DraftBillReturnValue returnValue = new DraftBillReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    for (int index = 0; index < selectedDraftBillIds.Count; index++)
                    {
                        int draftBillId = selectedDraftBillIds[index];
                        SrvDraftBillCommon.SubmitDraftBill(draftBillId);
                    }

                    returnValue.Success = true;
                    returnValue.Message = string.Empty;
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

        #region GetDraftBill

        /// <summary>
        /// Returns draft bill details by draft bill id
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="draftBillId">Draft bill id to populate</param>
        /// <returns></returns>
        public DraftBillReturnValue GetDraftBill(Guid logonId, int draftBillId)
        {
            DraftBillReturnValue returnValue = new DraftBillReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvDraftBill srvDraftBill = new SrvDraftBill();

                    srvDraftBill.Id = draftBillId;
                    srvDraftBill.Load(draftBillId);

                    DraftBill draftBill = new DraftBill();
                    draftBill.DraftBillId = srvDraftBill.Id;
                    draftBill.DraftBillDate = srvDraftBill.Date;
                    draftBill.ProfitCosts = srvDraftBill.ProfitCosts;
                    draftBill.VATAmount = srvDraftBill.VATAmount;
                    draftBill.VATRateId = srvDraftBill.VATRateId;
                    draftBill.BilledTimeUpto = srvDraftBill.BilledTimeUpto;
                    draftBill.IsProcessed = srvDraftBill.IsProcessed;
                    draftBill.IsSubmitted = srvDraftBill.IsSubmitted;
                    draftBill.ProjectId = srvDraftBill.ProjectId;
                    draftBill.PostingDescription = srvDraftBill.PostingDescriptionDetails;
                    draftBill.UnbilledPaidNonVATableNotes = srvDraftBill.UnbPaidNonVatNotes;
                    draftBill.UnbilledPaidVATableNotes = srvDraftBill.UnbPaidVatNotes;
                    draftBill.AntiNonVATableNotes = srvDraftBill.AntiNonVatNotes;
                    draftBill.AntiVATableNotes = srvDraftBill.AntiVatNotes;

                    returnValue.DraftBill = draftBill;
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

        #endregion

        public VatRateSearchReturnValue VatRateSearch(Guid logonId, CollectionRequest collectionRequest,
                                                      VatRateSearchCriteria criteria)
        {
            VatRateSearchReturnValue returnValue = new VatRateSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of ratings
                    DataListCreator<VatRateSearchItem> dataListCreator = new DataListCreator<VatRateSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvVATRateLookup.GetAllVatRates(criteria.IncludeNonVatable, criteria.IncludeArchived);
                    };

                    // Create the data list
                    returnValue.VatRates = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "VatRateSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "VatRateId"),
                            new ImportMapping("Description", "VatRateDescription"),
                            new ImportMapping("IsDefault", "VatRateDefault"),
                            new ImportMapping("Percentage", "VatRatePercentage")                            
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public BranchVatReturnValue GetBranchVatForProject(Guid logonId, Guid projectId)
        {
            BranchVatReturnValue returnValue = new BranchVatReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    Guid matterBranchId = SrvMatterCommon.GetMatterBranchGuid(projectId);
                    returnValue.BranchNoVat = SrvBranchCommon.GetBranchNoVAT(matterBranchId);
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public DisbursementLedgerReturnValue GetDisbursementLedgerVatableTransaction(Guid logonId, Guid projectId)
        {
            DisbursementLedgerReturnValue returnValue = new DisbursementLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    Collection<SrvDisbsLedgerTransactions> transactions =
                        SrvDraftBillCommon.GetDisbsLedgerVatableTransactions(projectId);

                    // Create a data list creator for a list
                    DataListCreator<DisbursementLedgerTransaction> dataListCreator = new DataListCreator<DisbursementLedgerTransaction>();

                    returnValue.Transactions = dataListCreator.Create<SrvDisbsLedgerTransactions>(transactions,
                        new ImportMapping[] {
                            new ImportMapping("Date", "PostingDetailsDate"),
                            new ImportMapping("Reference", "PostingDetailsReference"),
                            new ImportMapping("Description", "PostingDetailsDescription"),
                            new ImportMapping("Amount", "Amount"),
                            new ImportMapping("Billed", "Billed"),
                            new ImportMapping("VatRateId", "VatRateId"),
                            new ImportMapping("BillDisbursementAllocationAmount", "BillDisbursementAllocationAmount"),
                            new ImportMapping("PostingId", "PostingId"),
                            new ImportMapping("UnBilled", "UnBilled"),
                            new ImportMapping("IsBilled", "IsBilled")
                    });
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public DisbursementLedgerReturnValue GetDisbursementLedgerNonVatableTransaction(Guid logonId, Guid projectId)
        {
            DisbursementLedgerReturnValue returnValue = new DisbursementLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    Collection<SrvDisbsLedgerTransactions> transactions =
                                SrvDraftBillCommon.GetDisbsLedgerNonVatableTransactions(projectId);

                    // Create a data list creator for a list
                    DataListCreator<DisbursementLedgerTransaction> dataListCreator = new DataListCreator<DisbursementLedgerTransaction>();

                    returnValue.Transactions = dataListCreator.Create<SrvDisbsLedgerTransactions>(transactions,
                        new ImportMapping[] {
                            new ImportMapping("Date", "PostingDetailsDate"),
                            new ImportMapping("Reference", "PostingDetailsReference"),
                            new ImportMapping("Description", "PostingDetailsDescription"),
                            new ImportMapping("Amount", "Amount"),
                            new ImportMapping("Billed", "Billed"),
                            new ImportMapping("VatRateId", "VatRateId"),
                            new ImportMapping("BillDisbursementAllocationAmount", "BillDisbursementAllocationAmount"),
                            new ImportMapping("PostingId", "PostingId"),
                            new ImportMapping("UnBilled", "UnBilled"),
                            new ImportMapping("IsBilled", "IsBilled")
                            });
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public AnticipatedDisbursementLedgerReturnValue GetAnticipatedDisbursementLedgerVatableTransaction(Guid logonId, Guid projectId)
        {
            AnticipatedDisbursementLedgerReturnValue returnValue = new AnticipatedDisbursementLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list
                    DataListCreator<AnticipatedDisbursementLedgerTransaction> dataListCreator = new DataListCreator<AnticipatedDisbursementLedgerTransaction>();

                    Collection<SrvAntiDisbsLedgerTransaction> transactions =
                            SrvDraftBillCommon.GetAntiDisbLedgerVatableTransactions(projectId);

                    returnValue.Transactions = dataListCreator.Create<SrvAntiDisbsLedgerTransaction>(transactions,
                        new ImportMapping[] {
                            new ImportMapping("Date", "PostingDetailsDate"),
                            new ImportMapping("Reference", "PostingDetailsReference"),
                            new ImportMapping("Description", "PostingDetailsDescription"),
                            new ImportMapping("Amount", "Amount"),
                            new ImportMapping("Billed", "Billed"),
                            new ImportMapping("UnBilled", "UnBilled"),
                            new ImportMapping("PostingId", "PostingId"),
                            new ImportMapping("VatRateId", "VatRateId"),
                            new ImportMapping("IsBilled", "IsBilled")
                            });
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public AnticipatedDisbursementLedgerReturnValue GetAnticipatedDisbursementLedgerNonVatableTransaction(Guid logonId, Guid projectId)
        {
            AnticipatedDisbursementLedgerReturnValue returnValue = new AnticipatedDisbursementLedgerReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list
                    DataListCreator<AnticipatedDisbursementLedgerTransaction> dataListCreator = new DataListCreator<AnticipatedDisbursementLedgerTransaction>();

                    Collection<SrvAntiDisbsLedgerTransaction> transactions =
                        SrvDraftBillCommon.GetAntiDisbLedgerNonVatableTransactions(projectId);

                    returnValue.Transactions = dataListCreator.Create<SrvAntiDisbsLedgerTransaction>(transactions,
                                             new ImportMapping[] {
                                                    new ImportMapping("Date", "PostingDetailsDate"),
                                                    new ImportMapping("Reference", "PostingDetailsReference"),
                                                    new ImportMapping("Description", "PostingDetailsDescription"),
                                                    new ImportMapping("Amount", "Amount"),
                                                    new ImportMapping("Billed", "Billed"),
                                                    new ImportMapping("VatRateId", "VatRateId"),
                                                    new ImportMapping("UnBilled", "UnBilled"),
                                                    new ImportMapping("PostingId", "PostingId"),
                                                    new ImportMapping("IsBilled", "IsBilled")
                                                    });
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        public TimeTransactionReturnValue GetTimeTransaction(Guid logonId, Guid projectId)
        {
            TimeTransactionReturnValue returnValue = new TimeTransactionReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
               
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list
                    DataListCreator<TimeTransaction> dataListCreator = new DataListCreator<TimeTransaction>();

                    Collection<SrvTimeTransaction> transactions =
                            SrvDraftBillCommon.GetTimeTransaction(projectId);

                    returnValue.Transactions = dataListCreator.Create<SrvTimeTransaction>(transactions,
                        new ImportMapping[] {
                            new ImportMapping("Date", "TimeDate"),
                            new ImportMapping("FeeEarner", "FeeReference"),
                            new ImportMapping("Description", "TimeTypeDescription"),
                            new ImportMapping("Charge", "TimeCharge"),
                            new ImportMapping("Cost", "TimeCost"),
                            new ImportMapping("TimeId", "TimeId"),
                            new ImportMapping("Time", "TimeElapsed")
                            });
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        /// <summary>
        /// Adds the draft bill.
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="bill">The bill.</param>
        /// <returns></returns>
        public DraftBillReturnValue AddDraftBill(Guid logonId, DraftBill bill)
        {
            DraftBillReturnValue returnValue = new DraftBillReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Ensure we have permission
                    if (!UserSecuritySettings.GetUserSecuitySettings(145))
                        throw new Exception("You do not have sufficient permissions to carry out this request");

                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;

                    SrvDraftBill srvDraftBill = new SrvDraftBill();
                    srvDraftBill.ProjectId = bill.ProjectId;
                    bool success = srvDraftBill.ValidateProjectId(out errorMessage, out warningMessage);
                    if (success)
                    {
                        srvDraftBill.SetDefaultDisbursementLedger();
                    }
                    else
                    {
                        throw new Exception(errorMessage);
                    }
                    srvDraftBill.Date = bill.DraftBillDate.Date;
                    srvDraftBill.ProfitCosts = bill.ProfitCosts;
                    srvDraftBill.VATAmount = bill.VATAmount;
                    srvDraftBill.VATRateId = bill.VATRateId;
                    srvDraftBill.BilledTimeUpto = bill.BilledTimeUpto;
                    srvDraftBill.PostingDescriptionDetails = bill.PostingDescription;
                    srvDraftBill.UnbPaidNonVatNotes = bill.UnbilledPaidNonVATableNotes;
                    srvDraftBill.UnbPaidVatNotes = bill.UnbilledPaidVATableNotes;
                    srvDraftBill.AntiNonVatNotes = bill.AntiNonVATableNotes;
                    srvDraftBill.AntiVatNotes = bill.AntiVATableNotes;
                    this.SetDraftBillTransValues(bill.UnBilledPaidNonVatableList, srvDraftBill.UnBilledPaidNonVatableDisbsList);
                    this.SetDraftBillTransValues(bill.UnBilledPaidVatableList, srvDraftBill.UnBilledPaidVatableDisbsList);
                    this.SetDraftBillAnticipatedTransValues(bill.AnticipatedDisbursementNonVatableList, srvDraftBill.AntiDisbsNonVatableList);
                    this.SetDraftBillAnticipatedTransValues(bill.AnticipatedDisbursementVatableList, srvDraftBill.AntiDisbsVatableList);
                    this.SetTimeValues(bill.TimeTransactions, srvDraftBill.TimeTransaction);

                    var primaryClientAssociation = new BrAssociations().GetAssociationForMatterByProjectIdAndRoleId(srvDraftBill.ProjectId, 1);

                    if (primaryClientAssociation.ProjectAssociations.Count > 0)
                    {
                        var billPayer = new DraftBillPayer()
                        {
                            MemberId = primaryClientAssociation.ProjectAssociations[0].MemberID,
                            OrgId = primaryClientAssociation.ProjectAssociations[0].OrgID,
                            Amount = srvDraftBill.GetBillTotal(),
                            BillPayerStatusId = 1,
                            CashCollActDue = DataConstants.BlankDate,
                            Narrative = string.Empty,
                            AddressId = primaryClientAssociation.ProjectAssociations[0].MemberID == DataConstants.DummyGuid 
                                ? SrvAddressLookup.GetOrganisationAddressForBilling(primaryClientAssociation.ProjectAssociations[0].OrgID) 
                                : SrvAddressLookup.GetMemberAddressForBilling(primaryClientAssociation.ProjectAssociations[0].MemberID)
                        };


                        srvDraftBill.BillPayers.Add(billPayer);
                    }

                    returnValue.Success = srvDraftBill.Save(out errorMessage);
                    bill.DraftBillId = srvDraftBill.Id;
                    returnValue.DraftBill = bill;
                    returnValue.Message = errorMessage;
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

        #endregion

        //Get Clearance Types
        #region GetClearanceTypes

        public ClearanceTypeSearchReturnValue ClearanceTypes(Guid logonId, CollectionRequest collectionRequest,
            ClearanceTypeSearchCriteria criteria)
        {
            ClearanceTypeSearchReturnValue returnValue = new ClearanceTypeSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);
                
                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }


                    // Create a data list creator for a list of clearance types
                    DataListCreator<ClearanceTypeSearchItem> dataListCreator = new DataListCreator<ClearanceTypeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvClientChequeRequestLookup.GetClearanceTypes(criteria.IncludeArchived, criteria.IsCredit);
                    };

                    // Create the data list
                    returnValue.ClearanceTypes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "GetClearanceTypes",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("ClearanceTypeId", "ClearanceTypeId"),
                            new ImportMapping("ClearanceTypeDesc", "ClearanceTypeDesc"),
                            new ImportMapping("ClearanceTypeChqDays", "ClearanceTypeChqDays"),
                            new ImportMapping("ClearanceTypeElecDays", "ClearanceTypeElecDays"),
                            new ImportMapping("ClearanceTypeIsCredit", "ClearanceTypeIsCredit"),
                            new ImportMapping("ClearanceTypeArchived", "ClearanceTypeArchived"),
                            }
                        );
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the draft bill trans values.
        /// </summary>
        /// <param name="transactions">The data contract transactions.</param>
        /// <param name="srvDisbsLedgerTrans">The service layer property to which the values will be asigned.</param>
        private void SetDraftBillTransValues(List<DisbursementLedgerTransaction> transactions, Collection<SrvDisbsLedgerTransactions> srvDisbsLedgerTrans)
        {
            try
            {
                foreach (DisbursementLedgerTransaction disbsLedgerTrans in transactions)
                {
                    SrvDisbsLedgerTransactions srvTrans = srvDisbsLedgerTrans.First(t => t.PostingId == disbsLedgerTrans.PostingId);
                    if (srvTrans != null)
                    {
                        srvTrans.Billed = disbsLedgerTrans.Billed;
                    }
                    else
                    {
                        throw new Exception("Posting id not found in collection");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the draft bill trans values.
        /// </summary>
        /// <param name="transactions">The data contract transactions.</param>
        /// <param name="srvAntiDisbsLedgerTrans">The service layer property to which the values will be asigned.</param>
        private void SetDraftBillAnticipatedTransValues(List<AnticipatedDisbursementLedgerTransaction> transactions, Collection<SrvAntiDisbsLedgerTransaction> srvAntiDisbsLedgerTrans)
        {
            try
            {
                foreach (AnticipatedDisbursementLedgerTransaction disbsLedgerTrans in transactions)
                {
                    SrvAntiDisbsLedgerTransaction srvTrans = srvAntiDisbsLedgerTrans.First(t => t.PostingId == disbsLedgerTrans.PostingId);
                    if (srvTrans != null)
                    {
                        srvTrans.Billed = disbsLedgerTrans.Billed;
                    }
                    else
                    {
                        throw new Exception("Posting id not found in collection");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the time trans values.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="srvTimeTransaction">The service layer property to which the values will be asigned.</param>
        private void SetTimeValues(List<TimeTransaction> transactions, Collection<SrvTimeTransaction> srvTimeTransaction)
        {
            try
            {
                foreach (TimeTransaction timeTransaction in transactions)
                {
                    SrvTimeTransaction srvTrans = srvTimeTransaction.First(t => t.TimeId == timeTransaction.TimeId);
                    if (srvTrans != null)
                    {
                        srvTrans.Bill = timeTransaction.Bill;
                    }
                    else
                    {
                        throw new Exception("Time id not found in collection");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Convert Units to Minutes

        /// <summary>
        /// Method to convert units into an amount of time eg 1 unit = 0:06 mins
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="units"></param>
        /// <param name="minsPerUnit"></param>
        /// <returns>Returns units into time</returns>
        private string ConvertUnits(int units, int minsPerUnit)
        {
            bool isNegative = false;
            string returnValue = string.Empty;

            try
            {
                if (units < 0)
                {
                    isNegative = true;
                }

                int mins = 0;
                int hours = 0;
                mins = Math.Abs(units);

                if (mins > 59)
                {
                    hours = decimal.ToInt32(mins / 60);
                    decimal remainder = decimal.Remainder(mins, 60);
                    mins = (int)remainder;
                }

                if (mins < 10)
                {
                    returnValue = hours.ToString() + ":0" + mins.ToString();
                }
                else
                {
                    returnValue = hours.ToString() + ":" + mins.ToString();
                }

                if (isNegative)
                {
                    returnValue = "-" + returnValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;
        }

        #endregion

        #region GetTimeTransactionInfo

        /// <summary>
        /// Gets WIP transaction and last time by project id
        /// </summary>
        /// <param name="projectId">To load WIP transactions and last time.</param>
        /// <param name="returnValue">WIP transaction properties and last time property populated</param>
        /// <returns>Gets WIP transactions and last time by project id</returns>
        private FinancialBalancesReturnValue GetWIPAndTimeTransactionInfo(Guid projectId, string timeFilter, FinancialBalancesReturnValue returnValue)
        {
            try
            {
                DsTimeTransactions dsTimeTransactions = SrvTimeLookup.GetTimeTransactions(projectId, timeFilter, false);
                
                int timeElapsed = 0;
                decimal cost = decimal.Zero;
                decimal costBalance = decimal.Zero;
                decimal charge = decimal.Zero;
                decimal chargeBalance = decimal.Zero;

                DataView dvAllTimeTransactions = new DataView(dsTimeTransactions.uvw_TimeTransactions);
                if (dvAllTimeTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsTimeTransactions.uvw_TimeTransactions.Columns["TimeDate"]);
                    dvAllTimeTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView allTimeTransactionsRowView in dvAllTimeTransactions)
                    {
                        int timeId = Convert.ToInt32(allTimeTransactionsRowView.Row["TimeId"].ToString());

                        DateTime timeDate = (DateTime)allTimeTransactionsRowView.Row["TimeDate"];
                        string timeType = (string)allTimeTransactionsRowView.Row["TimeTypeDescription"].ToString();
                        string time = ConvertUnits((int)allTimeTransactionsRowView.Row["TimeElapsed"], ApplicationSettings.Instance.TimeUnits);
                        timeElapsed = timeElapsed + (int)allTimeTransactionsRowView.Row["TimeElapsed"];
                        int totalTimeElapsed = timeElapsed;
                        string feeEarnerReference = (string)allTimeTransactionsRowView.Row["feeRef"].ToString();
                        cost = (decimal)allTimeTransactionsRowView.Row["TimeCost"];
                        charge = (decimal)allTimeTransactionsRowView.Row["TimeCharge"];
                        costBalance = costBalance + cost;
                        chargeBalance = chargeBalance + charge;
                        cost = Decimal.Round(cost, 2);
                        charge = Decimal.Round(charge, 2);
                        chargeBalance = Decimal.Round(chargeBalance, 2);
                        costBalance = Decimal.Round(costBalance, 2);

                    }
                }
                if (timeFilter.Trim() == "All")
                {
                    returnValue.Time = ConvertUnits(timeElapsed, ApplicationSettings.Instance.TimeUnits);
                    returnValue.TimeCost = costBalance.ToString("F2");
                    returnValue.TimeChargeOut = chargeBalance.ToString("F2");
                }
                else
                {
                    returnValue.WIPChargeOut = chargeBalance.ToString("0.00");
                    returnValue.WIPCost = costBalance.ToString("0.00");
                    returnValue.WIPTime = ConvertUnits(timeElapsed, ApplicationSettings.Instance.TimeUnits);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;
        }

        #endregion

        #region GetLastFinancialDate

        /// <summary>
        /// Gets last financial date for financial balances by project id
        /// </summary>        
        /// <param name="projectId">To get last financial date by project id</param>
        /// <returns>Returns last financial date.</returns>
        private FinancialBalancesReturnValue GetLastFinancialDetails(Guid projectId)
        {
            FinancialBalancesReturnValue returnValue = new FinancialBalancesReturnValue();

            try
            {
                DateTime lastFinancialDate = DataConstants.BlankDate;
                DateTime lastTime = DataConstants.BlankDate;
                DateTime lastBill = DataConstants.BlankDate;

                // Movement Details
                DateTime postingDate = DataConstants.BlankDate;

                // Gets last financial date by project id from different datasets.                    
                // Sets last financial date from client ledger transactions.
                DsCledgerTransactions dsCledgerTransactions = SrvCledgerLookup.GetCledgerTransactions(projectId);
                DataView dvCledgerTransactions = new DataView(dsCledgerTransactions.uvw_CledgerTransactions);
                if (dvCledgerTransactions.Count != 0)
                {
                    foreach (DataRowView drvDLedgerTransaction in dvCledgerTransactions)
                    {
                        postingDate = (DateTime)drvDLedgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Sets last financial date from deposit ledger transactions.
                DsDledgerTransactions dsDLedgerTransactions = SrvDledgerLookup.GetDledgerTransactions(projectId);
                DataView dvDLedgerTransactions = new DataView(dsDLedgerTransactions.uvw_DledgerTransactions);
                if (dvDLedgerTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsDLedgerTransactions.uvw_DledgerTransactions.Columns["PostingDetailsDate"]);
                    dvDLedgerTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvDLedgerTransaction in dvDLedgerTransactions)
                    {
                        postingDate = (DateTime)drvDLedgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Sets last financial date from disbursements ledger transactions.
                DsDisbLedgerTransactions dsDisbLedgerTransactions = SrvDisbLedgerLookup.GetDisbursementsLedgerTransactions(projectId);
                DataView dvDisbLedgerTransactions = new DataView(dsDisbLedgerTransactions.uvw_DisbLedgerTransactions);
                if (dvDisbLedgerTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsDisbLedgerTransactions.uvw_DisbLedgerTransactions.Columns["PostingDetailsDate"]);
                    dvDLedgerTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvDisbLedgerTransaction in dvDisbLedgerTransactions)
                    {
                        postingDate = (DateTime)drvDisbLedgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Sets last financial date from anticipated disbursements ledger transactions.
                DsAntiDisbLedgerTransactions dsAntiDisbLedgerTransactions = SrvDisbLedgerLookup.GetAntiDisbursementsLedgerTransactions(projectId);
                DataView dvAntiDisbLedgerTransactions = new DataView(dsAntiDisbLedgerTransactions.uvw_AntiDisbsTransactions);
                if (dvAntiDisbLedgerTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsAntiDisbLedgerTransactions.uvw_AntiDisbsTransactions.Columns["PostingDetailsDate"]);
                    dvAntiDisbLedgerTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvDisbLedgerTransaction in dvAntiDisbLedgerTransactions)
                    {
                        postingDate = (DateTime)drvDisbLedgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Sets last financial date from anticipated bills ledger transactions.
                DsAntiBillTransactions dsAntiBillTransactions = SrvAntiBillLookup.GetAntiBillTransactions(projectId);
                DataView dvAntiBillTransactions = new DataView(dsAntiBillTransactions.uvw_AntiBillTransactions);
                if (dvAntiBillTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsAntiBillTransactions.uvw_AntiBillTransactions.Columns["PostingDetailsDate"]);
                    dvAntiBillTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvAntiBillTransaction in dvAntiBillTransactions)
                    {
                        postingDate = (DateTime)drvAntiBillTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Sets last financial date from bills ledger transactions.
                DsBledgerTransactions dsBledgerTransactions = SrvBledgerLookup.GetBledgerTransactions(projectId);
                DataView dvBledgerTransactions = new DataView(dsBledgerTransactions.uvw_BledgerTransactions);
                if (dvBledgerTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsBledgerTransactions.uvw_BledgerTransactions.Columns["PostingDetailsDate"]);
                    dvBledgerTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvBledgerTransaction in dvBledgerTransactions)
                    {
                        postingDate = (DateTime)drvBledgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }

                        // Last time can be reterieved only from the bills ledger and time transaction for filter "All"
                        if (postingDate > DataConstants.BlankDate)
                        {
                            lastBill = postingDate;
                        }
                    }
                }

                // Sets last financial date from pay ledger transactions.
                DsPayLedgerTransactions dsPayLedgerTransactions = SrvPayLedgerLookup.GetPayLedgerTransactions(projectId);
                DataView dvPayLedgerTransactions = new DataView(dsPayLedgerTransactions.uvw_PayLedgerTransactions);
                if (dvPayLedgerTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsPayLedgerTransactions.uvw_PayLedgerTransactions.Columns["PostingDetailsDate"]);
                    dvBledgerTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvPayLedgerTransaction in dvPayLedgerTransactions)
                    {
                        postingDate = (DateTime)drvPayLedgerTransaction.Row["PostingDetailsDate"];

                        if (postingDate > lastFinancialDate)
                        {
                            lastFinancialDate = postingDate;
                        }
                    }
                }

                // Gets Last time
                DsTimeTransactions dsTimeTransactions = SrvTimeLookup.GetTimeTransactions(projectId, "All", false);
                DataView dvTimeTransactions = new DataView(dsTimeTransactions.uvw_TimeTransactions);
                if (dvTimeTransactions.Count != 0)
                {
                    string columnOrderByPostingDate = Convert.ToString(dsTimeTransactions.uvw_TimeTransactions.Columns["TimeDate"]);
                    dvTimeTransactions.Sort = columnOrderByPostingDate + " " + "asc";

                    foreach (DataRowView drvTimeTransactions in dvTimeTransactions)
                    {
                        // Gets last time
                        DateTime timeDate = (DateTime)drvTimeTransactions.Row["TimeDate"];

                        if (timeDate > DataConstants.BlankDate)
                        {
                            lastTime = timeDate;
                        }
                    }
                }

                returnValue.LastFinancial = lastFinancialDate;
                returnValue.LastTime = lastTime;
                returnValue.LastBill = lastBill;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;
        }

        #endregion

        #region GetOfficeTransactions

        /// <summary>
        /// Retrieves office transactions by project id
        /// </summary>
        /// <param name="dsBillWaveOffTransactions">Waved off bills to set office transactions</param>
        /// <param name="dsPayLedgerTransactions">Pay ledger transactions to set office transactions</param>
        /// <returns>Retrieves office transactions by project id</returns>
        private DataSet GetOfficeBalances(Guid projectId)
        {
            DataSet dsOfficeBalances = null;

            try
            {
                DsBledgerTransactions dsBledgerTransactions = SrvBledgerLookup.GetBledgerTransactions(projectId);

                DsPayLedgerTransactions dsPayLedgerTransactions = SrvPayLedgerLookup.GetPayLedgerTransactions(projectId);

                DsBillWoTransactions dsBillWoTransactions = SrvBillWoLedgerLookup.GetBillWoLedgerTransactions(projectId);

                DsDisbLedgerTransactions dsDisbursementLedgerTransactions = SrvDisbLedgerLookup.GetDisbursementsLedgerTransactions(projectId);

                DateTime postingDate = DataConstants.BlankDate;
                string postingReference = string.Empty;
                string postingType = string.Empty;
                string postingDescription = string.Empty;
                string postingBank = string.Empty;
                string postingBankRef = string.Empty;
                int postingId = 0;
                decimal debit = decimal.Zero;
                decimal credit = decimal.Zero;
                int index = 0;

                dsOfficeBalances = new DataSet();
                dsOfficeBalances.Tables.Add("OfficeBalances");
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingId", typeof(int));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingDate", typeof(DateTime));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingReference", typeof(string));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingType", typeof(string));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingDescription", typeof(string));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingBank", typeof(string));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("PostingBankRef", typeof(string));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("Debit", typeof(decimal));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("Credit", typeof(decimal));
                dsOfficeBalances.Tables["OfficeBalances"].Columns.Add("Balance", typeof(decimal));

                for (index = 0; index < dsBledgerTransactions.uvw_BledgerTransactions.Rows.Count; index++)
                {
                    postingId = dsBledgerTransactions.uvw_BledgerTransactions[index].PostingId;
                    postingDate = dsBledgerTransactions.uvw_BledgerTransactions[index].PostingDetailsDate;
                    postingReference = dsBledgerTransactions.uvw_BledgerTransactions[index].PostingDetailsRef;
                    postingType = dsBledgerTransactions.uvw_BledgerTransactions[index].PostingTypesRef;
                    postingDescription = dsBledgerTransactions.uvw_BledgerTransactions[index].PostingDetailsDescription;

                    postingBank = "N/A";
                    postingBankRef = "N/A";

                    switch (dsBledgerTransactions.uvw_BledgerTransactions[index].PostingTypesRef.Trim())
                    {
                        case "OBL":
                            debit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterProfit) +
                                    Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterNVatDisbTot) +
                                    Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterVatDisbTot);
                            break;

                        case "COB":
                            debit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterProfit);
                            break;

                        case "BCP":
                            debit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterProfit);
                            break;
                        default:
                            debit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterProfit);
                            break;

                    }
                    credit = Decimal.Zero;
                    if (dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterBillAmount < Decimal.Zero)
                    {
                        credit = debit;
                        debit = decimal.Zero;
                    }

                    // Adds bills ledger transactions datarow to office balances dataset
                    dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                         postingDate,
                                                                         postingReference,
                                                                         postingType,
                                                                         postingDescription,
                                                                         postingBank,
                                                                         postingBankRef,
                                                                         debit,
                                                                         credit);

                    if (dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterVatAmount > decimal.Zero)
                    {
                        credit = decimal.Zero;
                        debit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterVatAmount);
                    }
                    else
                    {
                        debit = decimal.Zero;
                        credit = Math.Abs(dsBledgerTransactions.uvw_BledgerTransactions[index].BledgerMasterVatAmount);

                    }

                    // Sets posting decription as VAT for adding datarow for VAT information.
                    postingDescription = "V.A.T";

                    // Adds VAT information datarow to office balances dataset 
                    dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                         postingDate,
                                                                         postingReference,
                                                                         postingType,
                                                                         postingDescription,
                                                                         postingBank,
                                                                         postingBankRef,
                                                                         debit,
                                                                         credit);
                }

                index = 0;

                for (index = 0; index < dsPayLedgerTransactions.uvw_PayLedgerTransactions.Rows.Count; index++)
                {
                    postingId = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PostingId;
                    postingDate = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PostingDetailsDate;
                    postingReference = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PostingDetailsRef;
                    postingType = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PostingTypesRef;
                    postingDescription = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PostingDetailsDescription;
                    postingBank = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].bankRef;
                    postingBankRef = dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].bankRef;
                    if (dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].bankRef.Trim() == "0")
                    {
                        postingBank = "N/A";
                    }
                    else
                    {
                        int bankId = Convert.ToInt32(dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].bankId);
                        postingBank = GetBankByBankId(bankId);
                    }

                    if (dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PayLedgerWorkingAmount > Decimal.Zero)
                    {
                        credit = Decimal.Zero;
                        debit = Math.Abs(dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PayLedgerWorkingAmount);
                    }
                    else
                    {
                        debit = Decimal.Zero;
                        credit = Math.Abs(dsPayLedgerTransactions.uvw_PayLedgerTransactions[index].PayLedgerWorkingAmount);
                    }

                    // Adds pay ledger transactions datarow to office balances dataset
                    dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                         postingDate,
                                                                         postingReference,
                                                                         postingType,
                                                                         postingDescription,
                                                                         postingBank,
                                                                         postingBankRef,
                                                                         debit,
                                                                         credit);
                }

                index = 0;

                for (index = 0; index < dsBillWoTransactions.uvw_BillWoTransactions.Rows.Count; index++)
                {
                    postingId = dsBillWoTransactions.uvw_BillWoTransactions[index].PostingId;
                    postingDate = dsBillWoTransactions.uvw_BillWoTransactions[index].PostingDetailsDate;
                    postingReference = dsBillWoTransactions.uvw_BillWoTransactions[index].PostingDetailsRef;
                    postingType = dsBillWoTransactions.uvw_BillWoTransactions[index].PostingTypesRef;
                    postingDescription = dsBillWoTransactions.uvw_BillWoTransactions[index].PostingDetailsDescription;

                    postingBank = "N/A";
                    postingBankRef = "N/A";

                    if (dsBillWoTransactions.uvw_BillWoTransactions[index].BillWoLedgerWorkingAmount > Decimal.Zero)
                    {
                        credit = Decimal.Zero;
                        debit = Math.Abs(dsBillWoTransactions.uvw_BillWoTransactions[index].BillWoLedgerWorkingAmount);
                    }
                    else
                    {
                        debit = Decimal.Zero;
                        credit = Math.Abs(dsBillWoTransactions.uvw_BillWoTransactions[index].BillWoLedgerWorkingAmount);
                    }

                    // Adds bills wave off transactions datarow to office balances dataset
                    dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                        postingDate,
                                                                        postingReference,
                                                                        postingType,
                                                                        postingDescription,
                                                                        postingBank,
                                                                        postingBankRef,
                                                                        debit,
                                                                        credit);
                }

                index = 0;

                for (index = 0; index < dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions.Rows.Count; index++)
                {
                    postingId = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].PostingId;
                    postingDate = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].PostingDetailsDate;
                    postingReference = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].PostingDetailsRef;
                    postingDescription = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].PostingDetailsDescription;
                    postingType = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].PostingTypesRef;


                    if (dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].bankRef.Trim() == "0")
                    {
                        postingBank = "N/A";
                        postingBankRef = "N/A";
                    }
                    else
                    {
                        int bankId = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].bankId;
                        postingBank = GetBankByBankId(bankId);
                        postingBankRef = dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].bankRef;
                    }

                    if (dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterAmount > Decimal.Zero)
                    {
                        credit = Decimal.Zero;
                        debit = Math.Abs(dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterAmount);
                    }
                    else
                    {
                        debit = Decimal.Zero;
                        credit = Math.Abs(dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterAmount);
                    }

                    // Adds disbursements ledger transactions datarow to office balances dataset
                    dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                        postingDate,
                                                                        postingReference,
                                                                        postingType,
                                                                        postingDescription,
                                                                        postingBank,
                                                                        postingBankRef,
                                                                        debit,
                                                                        credit);

                    if (dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].VatRateId > 1 &&
                        dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].VatRateId != 3)
                    {
                        // There is a vat element to the disbledger posting, so go away and insert an extra item into the dataset to show VAT
                        if (dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterVatAmount > decimal.Zero)
                        {
                            credit = decimal.Zero;
                            debit = Math.Abs(dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterVatAmount);
                        }
                        else
                        {
                            debit = decimal.Zero;
                            credit = Math.Abs(dsDisbursementLedgerTransactions.uvw_DisbLedgerTransactions[index].DisbLedgerMasterVatAmount);

                        }

                        // For VAT information posting description should be "VAT"
                        postingDescription = "V.A.T";

                        // Adds VAT information datarow to office balances dataset
                        dsOfficeBalances.Tables["OfficeBalances"].Rows.Add(postingId,
                                                                            postingDate,
                                                                            postingReference,
                                                                            postingType,
                                                                            postingDescription,
                                                                            postingBank,
                                                                            postingBankRef,
                                                                            debit,
                                                                            credit);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsOfficeBalances;
        }

        #endregion

        #region GetBankByBankId

        /// <summary>
        /// Retrieves bank name by bank id
        /// </summary>        
        /// <param name="bankId">Bank id required to get bank name</param>
        /// <returns>Returns bank name by bank id</returns>
        private string GetBankByBankId(int bankId)
        {
            string bankName = string.Empty;

            try
            {
                if (bankId != 0)
                {
                    DsBanks banks = SrvBankLookup.GetBanks(bankId);

                    // Gets bank name by bank id
                    bankName = Convert.ToString(banks.Tables["Banks"].Rows[0]["BankName"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bankName;
        }

        #endregion

        #region CheckUserAllowedToAuthoriseChqReqHigherAmount

        /// <summary>
        /// Checks whether selected cheque request id is allowed to authorise
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="selectedChequeRequestIds">Selected cheque request ids for authorisation.</param>
        /// <param name="isClientChequeRequest">Checks whether cheque request authorisation is for client or office.</param>
        /// <returns>Returns flag to determine whether selected cheque request is allowed for authorisation.</returns>
        private bool CheckUserAllowedToAuthoriseChqReqHigherAmount(List<int> selectedChequeRequestIds, bool isClientChequeRequest, out decimal clientChequeRequestHigherAmount, out decimal officeChequeRequestHigherAmount)
        {
            Boolean returnValue = true;

            try
            {
                clientChequeRequestHigherAmount = decimal.Zero;
                officeChequeRequestHigherAmount = decimal.Zero;

                bool authoriseChequeRequestHigherAmount = UserSecuritySettings.GetUserSecuitySettings((int)AccountsSettings.AuthoriseChequeRequestHigherAmount);

                if (isClientChequeRequest)
                {
                    try
                    {
                        clientChequeRequestHigherAmount = Convert.ToDecimal(UserSecuritySettings.GetUserSecuitySettingsFeatureValue((int)AccountsSettings.ClientChequeRequestHigherAmount));
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        officeChequeRequestHigherAmount = Convert.ToDecimal(UserSecuritySettings.GetUserSecuitySettingsFeatureValue((int)AccountsSettings.OfficeChequeRequestHigherAmount));
                    }
                    catch { }
                }

                if (!authoriseChequeRequestHigherAmount)
                {
                    Int32 chequeRequestId = 0;
                    Decimal chequeRequestsAmount = Decimal.Zero;

                    for (int index = 0; index < selectedChequeRequestIds.Count; index++)
                    {
                        chequeRequestId = selectedChequeRequestIds[index];

                        if (isClientChequeRequest)
                        {
                            if (clientChequeRequestHigherAmount != Decimal.Zero)
                            {
                                chequeRequestsAmount = SrvClientChequeRequestCommon.GetClientChequeRequestsAmountByClientChequeRequestId(chequeRequestId);

                                if (chequeRequestsAmount > clientChequeRequestHigherAmount)
                                {
                                    returnValue = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (officeChequeRequestHigherAmount != Decimal.Zero)
                            {
                                chequeRequestsAmount = SrvOfficeChequeRequestCommon.GetOfficeChequeRequestsAmountAndVATByOfficeChequeRequestId(chequeRequestId);

                                if (chequeRequestsAmount > officeChequeRequestHigherAmount)
                                {
                                    returnValue = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnValue;
        }

        #endregion

        #endregion
    }
}

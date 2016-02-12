namespace IRIS.Law.WebServices.IWSProvider
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using Iris.Ews.Integration.Model;

    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Client;
    using IRIS.Law.WebServiceInterfaces.Contact;
    using IRIS.Law.WebServiceInterfaces.IWSProvider.Client;
    using IRIS.Law.WebServices;

    // NOTE: If you change the class name "ClientServiceIWS" here, you must also update the reference to "ClientServiceIWS" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.ClientServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_ClientServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class ClientServiceIWS : IClientServiceIWS
    {
        #region IClientServiceIWS Members
        ClientService oClientService;

        #region GetClient
        public ClientReturnValue GetClient(HostSecurityToken oHostSecurityToken, Guid memberId, Guid organisationId)
        {
            ClientReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.GetClient(Functions.GetLogonIdFromToken(oHostSecurityToken), memberId, organisationId);
            }
            else
            {
                returnValue = new ClientReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Add Client

        /// <summary>
        /// Add a new client
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="client">client details</param>
        /// <param name="person">Person details</param>
        /// <param name="organisation">Organisation details</param>
        /// <param name="addresses">Addresses</param>
        /// <param name="addressInformation"></param>
        /// <returns></returns>
        public ClientReturnValue AddClient(HostSecurityToken oHostSecurityToken, Client client,
            Person person,
            Organisation organisation,
            List<Address> addresses,
            List<AdditionalAddressElement> addressInformation)
        {
            ClientReturnValue returnValue = null;
            try
            {
                if (Functions.ValidateIWSToken(oHostSecurityToken))
                {
                    oClientService = new ClientService();
                    returnValue = oClientService.AddClient(Functions.GetLogonIdFromToken(oHostSecurityToken), client, person, organisation, addresses, addressInformation);
                }
                else
                {
                    returnValue = new ClientReturnValue();
                    returnValue.Success = false;
                    returnValue.Message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                returnValue = new ClientReturnValue();
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }
            return returnValue;

        }

        #endregion

        #region Update Client

        /// <summary>
        /// Update an existing client
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="client">Client details</param>
        /// <param name="person"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        public IRIS.Law.WebServiceInterfaces.ReturnValue UpdateClient(HostSecurityToken oHostSecurityToken, Client client,
                                                        Person person, Organisation organisation)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.UpdateClient(Functions.GetLogonIdFromToken(oHostSecurityToken), client, person, organisation);
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

        #region Update Client Organisation

        /// <summary>
        /// Update an existing client organisation
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="organisation">Organisation details</param>
        /// <returns></returns>
        public ReturnValue UpdateClientOrganisation(HostSecurityToken oHostSecurityToken, Organisation organisation)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.UpdateClientOrganisation(Functions.GetLogonIdFromToken(oHostSecurityToken), organisation);
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

        #region Update Client Person

        /// <summary>
        /// Update an existing client person
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="person">Person details</param>
        /// <returns></returns>
        public ReturnValue UpdateClientPerson(HostSecurityToken oHostSecurityToken, Person person)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.UpdateClientPerson(Functions.GetLogonIdFromToken(oHostSecurityToken), person);
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

        #region Update Client Address

        /// <summary>
        /// Update an existing client's address or add a new one
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="memberId">Member id of the client</param>
        /// <param name="organisationId">Orgainisation id of the client</param>
        /// <param name="address">Address to update or new address to add.
        /// If the Address.Id = 0 then a new address is being added.</param>
        /// <returns></returns>
        public ReturnValue UpdateClientAddress(HostSecurityToken oHostSecurityToken, Guid memberId,
            Guid organisationId, Address address)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.UpdateClientAddress(Functions.GetLogonIdFromToken(oHostSecurityToken), memberId, organisationId, address);
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

        #region Client Search

        /// <summary>
        /// Get a list of clients that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested</param>
        /// <param name="criteria">Client search criteria</param>
        /// <returns></returns>
        public ClientSearchReturnValue ClientSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            ClientSearchCriteria criteria)
        {
            ClientSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.ClientSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new ClientSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Construct UCN

        /// <summary>
        /// Constructs the UCN.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="forename">The forename.</param>
        /// <param name="surname">The surname.</param>
        /// <returns></returns>
        public ReturnValue ConstructUCN(HostSecurityToken oHostSecurityToken, DateTime dateOfBirth, string forename,
                                        string surname)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.ConstructUCN(Functions.GetLogonIdFromToken(oHostSecurityToken), dateOfBirth, forename, surname);
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

        #region Joint Client Candidate Search

        public JointClientCandidateSearchReturnValue JointClientCandidateSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                            JointClientCandidateSearchCriteria criteria)
        {
            JointClientCandidateSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.JointClientCandidateSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new JointClientCandidateSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Gets the clients defaults

        /// <summary>
        /// Gets the client type id and the default branch.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="isMember">if set to <c>true</c> [is member].</param>
        /// <returns></returns>
        public ClientReturnValue GetClientDefaults(HostSecurityToken oHostSecurityToken, Guid clientId)
        {
            ClientReturnValue returnValue = new ClientReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.GetClientDefaults(Functions.GetLogonIdFromToken(oHostSecurityToken), clientId);
            }
            else
            {
                returnValue = new ClientReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region ConflictCheck

        public ConflictCheckStandardReturnValue ConflictCheck(HostSecurityToken oHostSecurityToken,
            CollectionRequest collectionRequest,
            IRISLegal.IlbCommon.ContactType clientType,
            Person person,
            Organisation organisation,
            Address addresses,
            List<AdditionalAddressElement> addressInformation,
            bool checkOnAllRoles)
        {
            ConflictCheckStandardReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.ConflictCheck(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest,
                   clientType, person, organisation, addresses, addressInformation, checkOnAllRoles);
            }
            else
            {
                returnValue = new ConflictCheckStandardReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetClientAddress

        /// <summary>
        /// Get the address for the specified client  
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public AddressSearchReturnValue GetClientAddresses(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                        AddressSearchCriteria criteria)
        {
            AddressSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.GetClientAddresses(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest,
                   criteria);
            }
            else
            {
                returnValue = new AddressSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Get Client Name
        /// <summary>
        /// Get client name
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="memOrOrgId">Member or Org id</param>
        /// <param name="isMember">boolean value (is member)</param>
        /// <returns></returns>
        public ClientDetailReturnValue GetClientDetail(HostSecurityToken oHostSecurityToken, Guid memOrOrgId, bool isMember)
        {
            ClientDetailReturnValue returnValue = new ClientDetailReturnValue();
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.GetClientDetail(Functions.GetLogonIdFromToken(oHostSecurityToken), memOrOrgId,
                   isMember);
            }
            else
            {
                returnValue = new ClientDetailReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Rating

        public RatingSearchReturnValue RatingSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                    RatingSearchCriteria criteria)
        {
            RatingSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oClientService = new ClientService();
                returnValue = oClientService.RatingSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest,
                   criteria);
            }
            else
            {
                returnValue = new RatingSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion
        #endregion
    }
}

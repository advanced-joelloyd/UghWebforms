namespace IRIS.Law.WebServices.IWSProvider
{
    using System;
    using System.ServiceModel;
    using Iris.Ews.Integration.Model;
    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Contact;
    using IRIS.Law.WebServiceInterfaces.IWSProvider.Contact;

    // NOTE: If you change the class name "ContactService" here, you must also update the reference to "ContactService" in Web.config.
    /// <summary>
    /// Class Name: IRIS.Law.WebServices.IWSProvider.ContactServiceIWS
    /// Class Id: IRIS.Law.WebServices.IWSProvider.PS_ContactServiceIWS
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class ContactServiceIWS : IContactServiceIWS
    {
        #region IContactService Members
        ContactService oContactService;
        #region Address
        /// <summary>
        /// Gets the address types.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Address Type search criteria.</param>
        /// <returns></returns>
        public AddressTypeReturnValue GetAddressTypes(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                AddressTypeSearchCriteria criteria)
        {
            AddressTypeReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetAddressTypes(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new AddressTypeReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Saves the address for the contact.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public AddressReturnValue SaveAddress(HostSecurityToken oHostSecurityToken, Address address)
        {
            AddressReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SaveAddress(Functions.GetLogonIdFromToken(oHostSecurityToken), address);
            }
            else
            {
                returnValue = new AddressReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Saves the additional address element.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="additionalElement">The additional element.</param>
        /// <returns></returns>
        public ReturnValue SaveAdditionalAddressElement(HostSecurityToken oHostSecurityToken,
                                        AdditionalAddressElement additionalElement)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SaveAdditionalAddressElement(Functions.GetLogonIdFromToken(oHostSecurityToken), additionalElement);
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

        #region AssociationRole

        /// <summary>
        /// Get a list of partners that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Association Roles search criteria</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.AssociationRoleSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new AssociationRoleSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region BusinessSource

        /// <summary>
        /// Businesses the source search.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Business Source search criteria</param>
        /// <returns></returns>
        public BusinessSourceReturnValue BusinessSourceSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                BusinessSourceSearchCriteria criteria)
        {
            BusinessSourceReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.BusinessSourceSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new BusinessSourceReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Campaign

        /// <summary>
        /// Campaigns the search.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Campaign search criteria.</param>
        /// <returns></returns>
        public CampaignSearchReturnValue CampaignSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, CampaignSearchCriteria criteria)
        {
            CampaignSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.CampaignSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new CampaignSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Disability

        /// <summary>
        /// Get a list of disability values that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Disability search criteria</param>
        /// <returns></returns>
        public DisabilitySearchReturnValue DisabilitySearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                            DisabilitySearchCriteria criteria)
        {
            DisabilitySearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.DisabilitySearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new DisabilitySearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Ethnicity

        /// <summary>
        /// Get a list of ethnicity values that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Ethnicity search criteria</param>
        /// <returns></returns>
        public EthnicitySearchReturnValue EthnicitySearch(HostSecurityToken oHostSecurityToken, IRIS.Law.WebServiceInterfaces.CollectionRequest collectionRequest, EthnicitySearchCriteria criteria)
        {
            EthnicitySearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.EthnicitySearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new EthnicitySearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Industry

        /// <summary>
        /// Get a list of industries that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Industry search criteria</param>
        /// <returns></returns>
        public IndustrySearchReturnValue IndustrySearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                        IndustrySearchCriteria criteria)
        {
            IndustrySearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.IndustrySearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new IndustrySearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the industry for association role.
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public IndustryForAssociationRoleReturnValue GetIndustryForAssociationRole(HostSecurityToken oHostSecurityToken, IndustrySearchCriteria criteria)
        {
            IndustryForAssociationRoleReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetIndustryForAssociationRole(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria);
            }
            else
            {
                returnValue = new IndustryForAssociationRoleReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region MaritalStatus

        /// <summary>
        /// Get a list of marital status that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Marital Status search criteria</param>
        /// <returns></returns>
        public MaritalStatusSearchReturnValue MaritalStatusSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                MaritalStatusSearchCriteria criteria)
        {
            MaritalStatusSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.MaritalStatusSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new MaritalStatusSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Organisation

        /// <summary>
        /// Get a list of organisation sub types that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Organisation Sub Type search criteria</param>
        /// <returns></returns>
        public OrganisationSubTypeSearchReturnValue OrganisationSubTypeSearch(HostSecurityToken oHostSecurityToken,
                        CollectionRequest collectionRequest, OrganisationSubTypeSearchCriteria criteria)
        {
            OrganisationSubTypeSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.OrganisationSubTypeSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new OrganisationSubTypeSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region PostcodeLookup

        /// <summary>
        /// Get a list of postcodes that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="criteria">Postcode Lookup search criteria</param>
        /// <returns></returns>
        public PostcodeLookupReturnValue PostcodeLookupSearch(HostSecurityToken oHostSecurityToken, PostcodeLookupSearchCriteria criteria)
        {
            PostcodeLookupReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.PostcodeLookupSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), criteria);
            }
            else
            {
                returnValue = new PostcodeLookupReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Sex

        /// <summary>
        /// Get a list of sex that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Sex search criteria</param>
        /// <returns></returns>
        public SexSearchReturnValue SexSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                                SexSearchCriteria criteria)
        {
            SexSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SexSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new SexSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region Title

        /// <summary>
        /// Get a list of titles that match the search criteria
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Title search criteria</param>
        /// <returns></returns>
        public TitleSearchReturnValue TitleSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
            TitleSearchCriteria criteria)
        {
            TitleSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.TitleSearch(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
            }
            else
            {
                returnValue = new TitleSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion

        #region Contacts

        /// <summary>
        /// This method adds the Person, Address and Additional Address information
        /// using contact service to create a general contact.
        /// </summary>
        /// <param name="logonId">User logon ID</param>
        /// <param name="contactAddress">Address information for contact</param>
        /// <param name="person">Person information</param>
        /// <param name="additionalElement">Additional Element</param>
        /// <param name="contactType">Individual / Organisation</param>
        /// <param name="organisation">Organisation Information</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns>Return Value</returns>
        public ReturnValue SaveGeneralContact(HostSecurityToken oHostSecurityToken,
                                              Address contactAddress,
                                              Person person,
                                              AdditionalAddressElement[] additionalElement,
                                              IRISLegal.IlbCommon.ContactType contactType,
                                              Organisation organisation,
                                              string conflictNoteSummary,
                                              string conflictNoteContent)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SaveGeneralContact(Functions.GetLogonIdFromToken(oHostSecurityToken), contactAddress, person, additionalElement, contactType, organisation,
                    conflictNoteSummary, conflictNoteContent);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }



        /// <summary>
        /// Adds a new service.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="serviceAddress">The service address.</param>
        /// <param name="serviceAdditionalElement">The service additional element.</param>
        /// <param name="serviceInfo">The service info.</param>
        /// <param name="serviceContactInfo">The service contact info.</param>
        /// <param name="serviceContactAddress">The service contact address.</param>
        /// <param name="ServiceContactAdditionalElement">The service contact additional element.</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns></returns>
        public ReturnValue SaveService(HostSecurityToken oHostSecurityToken, Address serviceAddress,
                                       AdditionalAddressElement[] serviceAdditionalElement,
                                       ServiceInfo serviceInfo,
                                       ServiceContact serviceContactInfo,
                                       Address serviceContactAddress,
                                       AdditionalAddressElement[] ServiceContactAdditionalElement,
                                       string conflictNoteSummary,
                                       string conflictNoteContent)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SaveService(Functions.GetLogonIdFromToken(oHostSecurityToken), serviceAddress, serviceAdditionalElement,
                    serviceInfo, serviceContactInfo, serviceContactAddress, ServiceContactAdditionalElement,
                    conflictNoteSummary, conflictNoteContent);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        /// <summary>
        /// Saves the service contact.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="contactAddress">The contact address.</param>
        /// <param name="additionalElement">The additional element.</param>
        /// <param name="serviceContact">The service contact.</param>
        /// <param name="conflictNoteSummary">The conflict note summary.</param>
        /// <param name="conflictNoteContent">Content of the conflict note.</param>
        /// <returns></returns>
        public ReturnValue SaveServiceContact(HostSecurityToken oHostSecurityToken, Address contactAddress,
                                       AdditionalAddressElement[] additionalElement,
                                       ServiceContact serviceContact,
                                       string conflictNoteSummary,
                                       string conflictNoteContent)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.SaveServiceContact(Functions.GetLogonIdFromToken(oHostSecurityToken), contactAddress,
                    additionalElement, serviceContact, conflictNoteSummary, conflictNoteContent);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }
        #endregion Contacts

        #region AssociationRolesForApplicationSearch

        /// <summary>
        /// Search for association roles based on the application
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">AssociationRole search criteria.</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleForApplicationSearch(HostSecurityToken oHostSecurityToken,
                    CollectionRequest collectionRequest, AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.AssociationRoleForApplicationSearch(Functions.GetLogonIdFromToken(oHostSecurityToken),
                    collectionRequest, criteria);
            }
            else
            {
                returnValue = new AssociationRoleSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion AssociationRolesForApplicationSearch

        #region AssociationRoleForRoleIdSearch

        /// <summary>
        /// Search for association roles
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">AssociationRole search criteria.</param>
        /// <returns></returns>
        public AssociationRoleSearchReturnValue AssociationRoleForRoleIdSearch(HostSecurityToken oHostSecurityToken,
                    CollectionRequest collectionRequest, AssociationRoleSearchCriteria criteria)
        {
            AssociationRoleSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.AssociationRoleForRoleIdSearch(Functions.GetLogonIdFromToken(oHostSecurityToken),
                    collectionRequest, criteria);
            }
            else
            {
                returnValue = new AssociationRoleSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion AssociationRoleForRoleIdSearch

        #region RoleExtendedInfoSearch

        /// <summary>
        /// Get extended info for the Roles.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="criteria">RoleExtendedInfo search criteria</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <returns></returns>
        public RoleExtendedInfoReturnValue RoleExtendedInfoSearch(HostSecurityToken oHostSecurityToken, RoleExtendedInfoSearchCriteria criteria,
                                                CollectionRequest collectionRequest)
        {
            RoleExtendedInfoReturnValue returnValue = null;

            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.RoleExtendedInfoSearch(Functions.GetLogonIdFromToken(oHostSecurityToken),
                    criteria, collectionRequest);
            }
            else
            {
                returnValue = new RoleExtendedInfoReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion RoleExtendedInfoSearch

        #region AddAssociationForMatter

        /// <summary>
        /// Adds the association for matter.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="association">The association.</param>
        /// <returns></returns>
        public ReturnValue AddAssociationForMatter(HostSecurityToken oHostSecurityToken, AssociationForMatter association)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.AddAssociationForMatter(Functions.GetLogonIdFromToken(oHostSecurityToken),
                    association);
            }
            else
            {
                returnValue = new ReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion AddAssociationForMatter

        #region ContactSearch

        /// <summary>
        /// Search for contacts.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="collectionRequest">Information about the collection being requested.</param>
        /// <param name="criteria">Contact search criteria.</param>
        /// <returns></returns>
        public ContactSearchReturnValue ContactSearch(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest, ContactSearchCriteria criteria)
        {
            ContactSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.ContactSearch(Functions.GetLogonIdFromToken(oHostSecurityToken),collectionRequest,criteria);
            }
            else
            {
                returnValue = new ContactSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetPerson

        /// <summary>
        /// Gets the person.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="memberId">The member id.</param>
        /// <returns></returns>
        public PersonReturnValue GetPerson(HostSecurityToken oHostSecurityToken, Guid memberId)
        {
            PersonReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetPerson(Functions.GetLogonIdFromToken(oHostSecurityToken), memberId);
            }
            else
            {
                returnValue = new PersonReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        /// <summary>
        /// Get details about multiple persons.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="MemberIds">Array of member ids</param>
        /// <returns></returns>
        public PersonSearchReturnValue GetMultiplePersonDetails(HostSecurityToken oHostSecurityToken, Guid[] MemberIds)
        {
            PersonSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetMultiplePersonDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), MemberIds);
            }
            else
            {
                returnValue = new PersonSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #region GetOrganisation

        /// <summary>
        /// Gets the organisation.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        public OrganisationReturnValue GetOrganisation(HostSecurityToken oHostSecurityToken, Guid organisationId)
        {
            OrganisationReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetOrganisation(Functions.GetLogonIdFromToken(oHostSecurityToken), organisationId);
            }
            else
            {
                returnValue = new OrganisationReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        /// <summary>
        /// Get details about multiple organisations.
        /// </summary>
        /// <param name="oHostSecurityToken">HostSecurityToken obtained when security provider of IWS is called</param>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public OrganisationSearchReturnValue GetMultipleOrganisationDetails(HostSecurityToken oHostSecurityToken, Guid[] OrganisationIds)
        {
            OrganisationSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetMultipleOrganisationDetails(Functions.GetLogonIdFromToken(oHostSecurityToken), OrganisationIds);
            }
            else
            {
                returnValue = new OrganisationSearchReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #region GetServiceContact

        public ServiceContactReturnValue GetServiceContact(HostSecurityToken oHostSecurityToken, Guid contactId)
        {
            ServiceContactReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetServiceContact(Functions.GetLogonIdFromToken(oHostSecurityToken), contactId);
            }
            else
            {
                returnValue = new ServiceContactReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion

        #region GetGeneralContact

        public GeneralContactReturnValue GetGeneralContact(HostSecurityToken oHostSecurityToken, Guid memberId, Guid organisationId)
        {
            GeneralContactReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetGeneralContact(Functions.GetLogonIdFromToken(oHostSecurityToken), memberId, organisationId);
            }
            else
            {
                returnValue = new GeneralContactReturnValue();
                returnValue.Success = false;
                returnValue.Message = "Invalid Token";
            }
            return returnValue;
        }

        #endregion GetGeneralContact

        #region GetContactAddress

        /// <summary>
        /// Get the address for the specified contact  
        /// </summary>
        /// <param name="oHostSecurityToken"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public AddressSearchReturnValue GetContactAddresses(HostSecurityToken oHostSecurityToken, CollectionRequest collectionRequest,
                                        AddressSearchCriteria criteria)
        {
            AddressSearchReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.GetContactAddresses(Functions.GetLogonIdFromToken(oHostSecurityToken), collectionRequest, criteria);
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

        #endregion

        #region IContactService Members

        /// <summary>
        /// This method updates the Person / Organisation information
        /// using contact service .
        /// </summary>
        /// <param name="oHostSecurityToken">User logon ID</param>
        /// <param name="person">Person information</param>
        /// <param name="contactType">Individual / Organisation</param>
        /// <param name="organisation">Organisation Information</param>
        /// <returns>Return Value</returns>
        public ReturnValue UpdateGeneralContact(HostSecurityToken oHostSecurityToken, Person person, IRISLegal.IlbCommon.ContactType contactType, Organisation organisation)
        {
            ReturnValue returnValue = null;
            if (Functions.ValidateIWSToken(oHostSecurityToken))
            {
                oContactService = new ContactService();
                returnValue = oContactService.UpdateGeneralContact(Functions.GetLogonIdFromToken(oHostSecurityToken), person, contactType, organisation); 
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


    }
}

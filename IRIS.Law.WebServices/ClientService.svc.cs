
namespace IRIS.Law.WebServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using System.Text;
    using IRIS.Law.PmsCommonData;
    using IRIS.Law.PmsCommonServices.CommonServices;
    using IRIS.Law.Services.Pms.Address;
    using IRIS.Law.Services.Pms.Client;
    using IRIS.Law.Services.Pms.Industry;
    using IRIS.Law.Services.Pms.Matter;
    using IRIS.Law.Services.Pms.Organisation;
    using IRIS.Law.Services.Pms.Person;
    using IRIS.Law.Services.Pms.Rating;
    using IRIS.Law.Services.PMS.ConflictCheck;
    using IRIS.Law.WebServiceInterfaces;
    using IRIS.Law.WebServiceInterfaces.Client;
    using IRIS.Law.WebServiceInterfaces.Contact;

    // NOTE: If you change the class name "ClientService" here, you must also update the reference to "ClientService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class ClientService : IClientService
    {
        #region IClientService Members

        #region Get Client
        /// <summary>
        /// Get one client
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the Client service</param>
        /// <param name="memberId">Member id</param>
        /// <param name="organisationId">Organisation id</param>
        /// <returns></returns>
        public ClientReturnValue GetClient(Guid logonId, Guid memberId, Guid organisationId)
        {
            ClientReturnValue returnValue = new ClientReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvClientCommon.WebAllowedToAccessClient(memberId, organisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.Client:
                            if (!ApplicationSettings.Instance.IsUser(memberId, organisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvClient srvClient = new SrvClient();

                    srvClient.Load(memberId, organisationId);

                    Client client = new Client();
                    client.MemberId = srvClient.MemberId;
                    client.OrganisationId = srvClient.OrganisationId;
                    client.PartnerId = srvClient.ClientPartnerId;
                    client.Branch = srvClient.ClientBranch.Trim();
                    client.OpenDate = srvClient.ClientOpenDate;
                    client.PreviousReference = srvClient.PreviousReference.Trim();
                    client.BusinessSourceId = srvClient.SourceId;
                    client.RatingId = srvClient.RatingId;
                    client.IsWebCaseTracking = srvClient.IsClientDoUpload;
                    client.NetPassword = srvClient.ClientNetPassword;
                    client.Group = srvClient.ClientGroup.Trim();
                    client.HOUCN = srvClient.ClientHOUCN.Trim();
                    client.UCN = srvClient.ClientUCN.Trim();
                    client.IsArchived = srvClient.IsClientArchived;
                    client.Type = srvClient.ClientType;
                    client.IsReceivingMarketing = srvClient.IsReceivingMarketing;
                    client.CashCollectionId = srvClient.CliCashCollID;
                    client.TotalLockup = srvClient.CliTotalLockup;
                    client.Reference = srvClient.ClientText.Trim();
                    client.CampaignId = srvClient.CampaignId;

                    returnValue.Client = client;

                    if (srvClient.IsMember)
                    {
                        Person person = new Person();

                        person.MemberId = srvClient.Person.PersonId;
                        person.Title = srvClient.Person.Title;
                        person.ForeName = srvClient.Person.ForeName;
                        person.Surname = srvClient.Person.Surname;
                        client.FullName = IRIS.Law.PmsCommonData.CommonServices.CommonFunctions.MakeFullName(person.Title, person.ForeName, person.Surname);
                        person.MaritalStatusId = srvClient.Person.MaritalId;
                        person.PreviousName = srvClient.Person.PersonPreviousName;
                        person.Occupation = srvClient.Person.PersonOccupation;
                        person.Sex = srvClient.Person.Sex;
                        person.DOB = srvClient.Person.PersonDOB;
                        person.DOD = srvClient.Person.PersonDOD;
                        person.PlaceOfBirth = srvClient.Person.PersonPlaceOfBirth;
                        person.BirthName = srvClient.Person.PersonBirthName;
                        person.SalutationLettterFormal = srvClient.Person.PersonSalletForm;
                        person.SalutationLettterInformal = srvClient.Person.PersonSalutationlettterInformal;
                        person.SalutationLettterFriendly = srvClient.Person.PersonSalLet;
                        person.SalutationEnvelope = srvClient.Person.PersonSalEnv;
                        person.EthnicityId = srvClient.Person.PersonEthnicityId;
                        person.DisabilityId = srvClient.Person.PersonDisability;
                        person.IsInArmedForces = srvClient.Person.PersonInArmedForces;
                        person.ArmedForcesNo = srvClient.Person.PersonArmedForcesNo;
                        person.NINo = srvClient.Person.PersonNINo;

                        returnValue.Person = person;
                    }

                    if (srvClient.IsOrganisation)
                    {
                        Organisation organisation = new Organisation();

                        organisation.OrganisationId = srvClient.Organisation.OrganisationId;
                        organisation.Name = srvClient.Organisation.Name;
                        client.FullName = organisation.Name;
                        organisation.RegisteredName = srvClient.Organisation.RegisteredName;
                        organisation.RegisteredNo = srvClient.Organisation.RegisteredNumber;
                        organisation.VATNo = srvClient.Organisation.VATNumber;
                        organisation.IndustryId = srvClient.Organisation.IndustryId;
                        organisation.SubTypeId = srvClient.Organisation.SubTypesId;

                        returnValue.Organisation = organisation;
                    }

                    returnValue.Addresses = new List<Address>();
                    returnValue.AdditionalAddressElements = new List<AdditionalAddressElement>();
                    foreach (SrvAddress address in srvClient.Addresses)
                    {
                        Address clientAddress = new Address();

                        clientAddress.Id = address.AddressId;
                        clientAddress.TypeId = address.AddressTypeId;
                        clientAddress.Line1 = address.AddressLine1;
                        clientAddress.Line2 = address.AddressLine2;
                        clientAddress.Line3 = address.AddressLine3;
                        clientAddress.Town = address.AddressTown;
                        clientAddress.County = address.AddressCounty;
                        clientAddress.PostCode = address.AddressPostCode;
                        clientAddress.DXTown = address.AddressDXTown;
                        clientAddress.DXNumber = address.AddressDXNumber;
                        clientAddress.Country = address.AddressCountry;
                        clientAddress.IsMailingAddress = address.IsMailingAddress;
                        clientAddress.IsBillingAddress = address.IsBillingAddress;
                        clientAddress.Comment = address.AddressComment;
                        clientAddress.OrganisationName = address.AddressOrgName;
                        clientAddress.Department = address.AddressDepartment;
                        clientAddress.PostBox = address.AddressPostBox;
                        clientAddress.SubBuilding = address.AddressSubBuilding;
                        clientAddress.StreetNumber = address.AddressStreetNumber;
                        clientAddress.HouseName = address.AddressHouseName;
                        clientAddress.DependantLocality = address.AddressDependantLocality;
                        clientAddress.LastVerified = address.AddressLastVerified;

                        returnValue.Addresses.Add(clientAddress);

                        //Get the contact details from the main address
                        if (address.AddressTypeId == 1)
                        {
                            DsAdditionalAddElTypes dsAdditionalAddElTypes = SrvAddressLookup.GetAdditionalAddressElTypes();
                            // integer array which specifies the order of the addAddressElements to display
                            // eg { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 } would display the items as listed in the
                            // table but in the reverse order
                            int[] additionalTypeOrder = new int[11] { 1, 2, 3, 10, 4, 5, 6, 7, 8, 9, 11 };
                            bool addItem = true;

                            for (int x = 0; x < dsAdditionalAddElTypes.AdditionalAddElTypes.Rows.Count; x++)
                            {
                                // Loop through element type table and get the next id type you want
                                for (int i = 0; i < dsAdditionalAddElTypes.AdditionalAddElTypes.Rows.Count; i++)
                                {
                                    int typeId = (int)dsAdditionalAddElTypes.AdditionalAddElTypes[i].AddElTypeID;
                                    if (typeId == additionalTypeOrder[x])
                                    {
                                        string typeText = (string)dsAdditionalAddElTypes.AdditionalAddElTypes[i].AddElTypeText;
                                        string elementText = string.Empty;
                                        string elementComment = string.Empty;

                                        // Loop through member or organisation addEl tables to see if there are any matches.
                                        if (srvClient.IsMember)
                                        {
                                            addItem = true;
                                        }
                                        else
                                        {
                                            if (typeId == 1 || typeId == 7 || typeId == 10)
                                            {
                                                addItem = false;
                                            }
                                            else
                                            {
                                                addItem = true;
                                            }
                                        }

                                        if (addItem)
                                        {
                                            for (int j = 0; j < address.AdditionalInfoElements.Count; j++)
                                            {
                                                if (address.AdditionalInfoElements[j].AddressElTypeId == typeId)
                                                {
                                                    elementText = address.AdditionalInfoElements[j].AddressElementText;
                                                    elementComment = address.AdditionalInfoElements[j].AddressElComment;
                                                    break;
                                                }
                                            }

                                            AdditionalAddressElement additionalAddressElement = new AdditionalAddressElement();
                                            additionalAddressElement.TypeId = typeId;
                                            additionalAddressElement.TypeText = typeText;
                                            additionalAddressElement.ElementText = elementText;
                                            additionalAddressElement.ElementComment = elementComment;
                                            additionalAddressElement.AddressId = address.AddressId;
                                            returnValue.AdditionalAddressElements.Add(additionalAddressElement);
                                        }
                                    }
                                }
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

        #region Add Client

        /// <summary>
        /// Add a new client subject to user type, permissions and licensing
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="client">client details</param>
        /// <param name="person">Person details</param>
        /// <param name="organisation">Organisation details</param>
        /// <param name="addresses">Addresses list</param>
        /// <param name="addressInformation">Address information list</param>
        /// <returns>Returns one client entity</returns>
        public ClientReturnValue AddClient(
            Guid logonId,
            Client client,
            Person person,
            Organisation organisation,
            List<Address> addresses,
            List<AdditionalAddressElement> addressInformation)
        {
            ClientReturnValue returnValue = new ClientReturnValue();

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
                    if (!UserSecuritySettings.GetUserSecuitySettings(2))
                    {
                        throw new Exception("You do not have sufficient permissions to carry out this request");
                    }

                    // Verify Annual Licence
                    if (!IRIS.Law.PmsBusiness.LicenseDetails.AnnualLicenseIsValid())
                    {
                        throw new Exception("Unable to add Client. Your Annual Licence has expired or is invalid.");
                    }

                    SrvClient srvClient = new SrvClient();

                    // Load client fields
                    srvClient.ClientType = client.Type;
                    srvClient.ClientPartnerId = client.PartnerId;
                    srvClient.ClientBranch = client.Branch;
                    srvClient.ClientNetPassword = client.NetPassword;
                    srvClient.ConflictNoteSummary = client.ConflictNoteSummary;
                    srvClient.ConflictNoteContent = client.ConflictNoteContent;

                    // Load person fields
                    switch (client.Type)
                    {
                        case IRISLegal.IlbCommon.ContactType.Individual:
                            srvClient.Person.Title = person.Title;
                            srvClient.Person.setDefaultSex();
                            srvClient.Person.Surname = person.Surname;
                            srvClient.Person.ForeName = person.ForeName;
                            break;
                        case IRISLegal.IlbCommon.ContactType.Organisation:
                            srvClient.Organisation = new SrvOrganisation();
                            srvClient.Organisation.Name = organisation.Name;
                            break;
                        default:
                            throw new Exception("Unknown ClientType");
                    }

                    // This will be used if Client Type selected is multiple 
                    // so that second person will be associated with the first person
                    if (client.AssociationId != DataConstants.DummyGuid)
                    {
                        srvClient.AssociationRoleId = client.AssociationRoleId;
                        srvClient.AssociationId = client.AssociationId;
                    }

                    // Load address of person
                    if (addresses != null)
                    {
                        foreach (Address address in addresses)
                        {
                            SrvAddress srvAddress = new SrvAddress();

                            srvAddress.AddressTypeId = address.TypeId;
                            srvAddress.AddressStreetNumber = address.StreetNumber;
                            srvAddress.AddressPostCode = address.PostCode;
                            srvAddress.AddressHouseName = address.HouseName;
                            srvAddress.AddressLine1 = address.Line1;
                            srvAddress.AddressLine2 = address.Line2;
                            srvAddress.AddressLine3 = address.Line3;
                            srvAddress.AddressTown = address.Town;
                            srvAddress.AddressCounty = address.County;
                            srvAddress.AddressCountry = address.Country;
                            srvAddress.AddressDXTown = address.DXTown;
                            srvAddress.AddressDXNumber = address.DXNumber;
                            srvAddress.IsMailingAddress = address.IsMailingAddress;
                            srvAddress.IsBillingAddress = address.IsBillingAddress;

                            srvAddress.AddressOrgName = address.OrganisationName;
                            srvAddress.AddressComment = address.Comment;
                            srvAddress.AddressDepartment = address.Department;
                            srvAddress.AddressPostBox = address.PostBox;
                            srvAddress.AddressSubBuilding = address.SubBuilding;
                            srvAddress.AddressStreetNumber = address.StreetNumber;
                            srvAddress.AddressDependantLocality = address.DependantLocality;
                            srvAddress.AddressLastVerified = address.LastVerified;

                            // Save Additional Address Info to Address Object
                            if (addressInformation != null)
                            {
                                for (int i = 0; i <= addressInformation.Count - 1; i++)
                                {
                                    srvAddress.AdditionalInfoElements[i].AddressElementText =
                                        addressInformation[i].ElementText;
                                }
                            }

                            // TODO more address fields
                            srvClient.Addresses.Add(srvAddress);
                        }
                    }

                    string errorMessage;

                    returnValue.Success = srvClient.Save(out errorMessage);
                    returnValue.Message = errorMessage;

                    client.MemberId = srvClient.MemberId;
                    client.IsMember = srvClient.IsMember;

                    client.OrganisationId = srvClient.OrganisationId;

                    returnValue.Client = client;
                    returnValue.Person = person;
                    returnValue.Organisation = organisation;
                    returnValue.Addresses = addresses;
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

        #region Update Client

        /// <summary>
        /// Update an existing client
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="client">Client details</param>
        /// <param name="person"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        public IRIS.Law.WebServiceInterfaces.ReturnValue UpdateClient(Guid logonId, Client client,
                                                        Person person, Organisation organisation)
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
                            if (!ApplicationSettings.Instance.IsUser(client.MemberId, client.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvClient srvClient = new SrvClient();
                    srvClient.Load(client.MemberId, client.OrganisationId);

                    srvClient.MemberId = client.MemberId;
                    srvClient.OrganisationId = client.OrganisationId;
                    srvClient.ClientPartnerId = client.PartnerId;
                    srvClient.ClientBranch = client.Branch;
                    srvClient.ClientOpenDate = client.OpenDate;
                    srvClient.PreviousReference = client.PreviousReference;
                    srvClient.SourceId = client.BusinessSourceId;
                    srvClient.RatingId = client.RatingId;
                    srvClient.IsClientDoUpload = client.IsWebCaseTracking;
                    srvClient.ClientNetPassword = client.NetPassword;
                    srvClient.ClientGroup = client.Group;
                    srvClient.ClientHOUCN = client.HOUCN;
                    srvClient.ClientUCN = client.UCN;
                    srvClient.IsClientArchived = client.IsArchived;
                    srvClient.ClientType = client.Type;
                    srvClient.IsReceivingMarketing = client.IsReceivingMarketing;
                    srvClient.CliCashCollID = client.CashCollectionId;
                    srvClient.CliTotalLockup = client.TotalLockup;
                    srvClient.CampaignId = client.CampaignId;

                    //Since SrvClient saves the Person/Organisation details when we call save
                    //we need to pass the Person/Organisation ojects
                    srvClient.Person.PersonId = person.MemberId;
                    srvClient.Person.Title = person.Title;
                    srvClient.Person.ForeName = person.ForeName;
                    srvClient.Person.Surname = person.Surname;
                    srvClient.Person.MaritalId = person.MaritalStatusId;
                    srvClient.Person.PersonPreviousName = person.PreviousName;
                    srvClient.Person.PersonOccupation = person.Occupation;
                    srvClient.Person.Sex = person.Sex;
                    srvClient.Person.PersonDOB = person.DOB;
                    srvClient.Person.PersonDOD = person.DOD;
                    srvClient.Person.PersonPlaceOfBirth = person.PlaceOfBirth;
                    srvClient.Person.PersonBirthName = person.BirthName;
                    srvClient.Person.PersonSalletForm = person.SalutationLettterFormal;
                    srvClient.Person.PersonSalutationlettterInformal = person.SalutationLettterInformal;
                    srvClient.Person.PersonSalLet = person.SalutationLettterFriendly;
                    srvClient.Person.PersonSalEnv = person.SalutationEnvelope;
                    srvClient.Person.PersonEthnicityId = person.EthnicityId;
                    srvClient.Person.PersonDisability = person.DisabilityId;
                    srvClient.Person.PersonInArmedForces = person.IsInArmedForces;
                    srvClient.Person.PersonArmedForcesNo = person.ArmedForcesNo;
                    srvClient.Person.PersonNINo = person.NINo;

                    //Update the organisation details
                    srvClient.Organisation.OrganisationId = organisation.OrganisationId;
                    srvClient.Organisation.Name = organisation.Name;
                    srvClient.Organisation.RegisteredName = organisation.RegisteredName;
                    srvClient.Organisation.RegisteredNumber = organisation.RegisteredNo;
                    srvClient.Organisation.VATNumber = organisation.VATNo;
                    srvClient.Organisation.SubTypesId = organisation.SubTypeId;

                    string errorMessage;
                    returnValue.Success = srvClient.Save(out errorMessage);
                    if (!returnValue.Success)
                    {
                        throw new Exception(errorMessage);
                    }

                    if (client.MemberId == DataConstants.DummyGuid)
                    {
                        //check if industry has been assigned and update the value
                        if (organisation.IndustryId != 0)
                        {
                            errorMessage = string.Empty;
                            SrvIndustry srvIndustry = new SrvIndustry();
                            srvIndustry.Id = organisation.IndustryId;
                            srvIndustry.MemberId = DataConstants.DummyGuid;
                            srvIndustry.OrganisationId = organisation.OrganisationId;
                            returnValue.Success = srvIndustry.Save(out errorMessage);
                        }
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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region Update Client Organisation

        /// <summary>
        /// Update an existing client organisation
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="organisation">Organisation details</param>
        /// <returns></returns>
        public ReturnValue UpdateClientOrganisation(Guid logonId, Organisation organisation)
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
                            if (!ApplicationSettings.Instance.IsUser(DataConstants.DummyGuid, organisation.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Later need to check permissions to see if they are allowed
                    // to update this client.

                    SrvOrganisation srvOrganisation = new SrvOrganisation();
                    srvOrganisation.Load(organisation.OrganisationId);

                    //Update the organisation details
                    srvOrganisation.Name = organisation.Name;
                    srvOrganisation.RegisteredName = organisation.RegisteredName;
                    srvOrganisation.RegisteredNumber = organisation.RegisteredNo;
                    srvOrganisation.VATNumber = organisation.VATNo;
                    srvOrganisation.SubTypesId = organisation.SubTypeId;

                    string errorMessage;
                    returnValue.Success = srvOrganisation.Save(out errorMessage);
                    if (!returnValue.Success)
                    {
                        throw new Exception(errorMessage);
                    }

                    //check if industry has been assigned and update the value
                    if (organisation.IndustryId != -1)
                    {
                        errorMessage = string.Empty;
                        SrvIndustry srvIndustry = new SrvIndustry();
                        srvIndustry.Id = organisation.IndustryId;
                        srvIndustry.MemberId = DataConstants.DummyGuid;
                        srvIndustry.OrganisationId = organisation.OrganisationId;
                        returnValue.Success = srvIndustry.Save(out errorMessage);
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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region Update Client Person

        /// <summary>
        /// Update an existing client person
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="person">Person details</param>
        /// <returns></returns>
        public ReturnValue UpdateClientPerson(Guid logonId, Person person)
        {
            ReturnValue returnValue = new ReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    // Later here we will need to check the feature
                    // permission that they are allowed to update a client.
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                            if (!ApplicationSettings.Instance.IsUser(person.MemberId, DataConstants.DummyGuid))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvPerson srvPerson = new SrvPerson();
                    srvPerson.Load(person.MemberId);
                    srvPerson.Title = person.Title;
                    srvPerson.ForeName = person.ForeName;
                    srvPerson.Surname = person.Surname;
                    srvPerson.MaritalId = person.MaritalStatusId;
                    srvPerson.PersonPreviousName = person.PreviousName;
                    srvPerson.PersonOccupation = person.Occupation;
                    srvPerson.Sex = person.Sex;
                    srvPerson.PersonDOB = person.DOB;
                    srvPerson.PersonDOD = person.DOD;
                    srvPerson.PersonPlaceOfBirth = person.PlaceOfBirth;
                    srvPerson.PersonBirthName = person.BirthName;
                    srvPerson.PersonSalletForm = person.SalutationLettterFormal;
                    srvPerson.PersonSalutationlettterInformal = person.SalutationLettterInformal;
                    srvPerson.PersonSalLet = person.SalutationLettterFriendly;
                    srvPerson.PersonSalEnv = person.SalutationEnvelope;
                    srvPerson.PersonEthnicityId = person.EthnicityId;
                    srvPerson.PersonDisability = person.DisabilityId;
                    srvPerson.PersonInArmedForces = person.IsInArmedForces;
                    srvPerson.PersonArmedForcesNo = person.ArmedForcesNo;
                    srvPerson.PersonNINo = person.NINo;

                    string errorMessage;

                    returnValue.Success = srvPerson.Save(out errorMessage);
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

        #region Update Client Address

        /// <summary>
        /// Update an existing client's address or add a new one
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="memberId">Member id of the client</param>
        /// <param name="organisationId">Orgainisation id of the client</param>
        /// <param name="address">Address to update or new address to add.
        /// If the Address.Id = 0 then a new address is being added.</param>
        /// <returns></returns>
        public ReturnValue UpdateClientAddress(Guid logonId, Guid memberId,
            Guid organisationId, Address address)
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
                            if (!ApplicationSettings.Instance.IsUser(memberId, organisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvClient srvClient = new SrvClient();

                    srvClient.Load(memberId, organisationId);

                    SrvAddress srvAddress = null;

                    if (address.Id != 0)
                    {
                        // Updating an existing address so check it exists on this client
                        foreach (SrvAddress addr in srvClient.Addresses)
                        {
                            if (addr.AddressId == address.Id)
                            {
                                // Found it
                                srvAddress = addr;
                                break;
                            }
                        }

                        if (srvAddress == null)
                            throw new Exception("Address does not exist for client");
                    }
                    else
                    {
                        // A new address is being added
                        srvAddress = new SrvAddress();

                        srvAddress.MemberId = memberId;
                        srvAddress.OrganisationId = organisationId;
                    }

                    srvAddress.AddressTypeId = address.TypeId;
                    srvAddress.AddressLine1 = address.Line1;
                    // TODO more address fields

                    string errorMessage;

                    returnValue.Success = srvAddress.Save(out errorMessage);
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

        #region Client Search

        /// <summary>
        /// Get a list of clients that match the search criteria
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="collectionRequest">Information about the collection being requested</param>
        /// <param name="criteria">Client search criteria</param>
        /// <returns></returns>
        public ClientSearchReturnValue ClientSearch(Guid logonId, CollectionRequest collectionRequest,
            ClientSearchCriteria criteria)
        {
            ClientSearchReturnValue returnValue = new ClientSearchReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    switch (UserInformation.Instance.UserType)
                    {
                        // Can do everything
                        case DataConstants.UserType.Staff:
                        case DataConstants.UserType.ThirdParty:
                            // The query will return only clients that the third party is allowed to see
                            break;
                        case DataConstants.UserType.Client:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<ClientSearchItem> dataListCreator = new DataListCreator<ClientSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvClientCommon.WebClientSearch(criteria.OrganisationId,
                                                                    criteria.MemberId,
                                                                    criteria.Name,
                                                                    criteria.BusinessSource,
                                                                    criteria.ClientReference,
                                                                    criteria.DateOfBirthFrom,
                                                                    criteria.DateOfBirthTo,
                                                                    criteria.PostCode,
                                                                    criteria.Partner,
                                                                    criteria.Branch,
                                                                    criteria.NINumber,
                                                                    criteria.ClientGroup,
                                                                    criteria.Town);

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("Table");
                            e.DataSet.Tables.Add(dt);
                        }

                    };

                    // Create the data list
                    DataList<ClientSearchItem> clientList = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ClientSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("MemberId", "memId"),
                            new ImportMapping("OrganisationId", "orgId"),
                            new ImportMapping("ClientReference", "cliRef"),
                            new ImportMapping("Name", "Name"),
                            new ImportMapping("Address","Address"),
                            new ImportMapping("PartnerName", "Partner"),
                            new ImportMapping("Partner","cliPartner"),
                            new ImportMapping("DateOfBirth","DateofBirth"),
                            new ImportMapping("NationalInsuranceNo","personNHINo"),
                            new ImportMapping("DateOfDeath","PersonDOD")
                            }
                        );


                    clientList.Rows.ForEach(delegate(ClientSearchItem item)
                    {
                        if (item.MemberId != DataConstants.DummyGuid)
                        {
                            if (SrvClientCommon.IsClientArchived(item.MemberId))
                            {
                                item.IsArchived = true;
                            }
                            else
                            {
                                item.IsArchived = false;
                            }
                        }

                        if (item.OrganisationId != DataConstants.DummyGuid)
                        {
                            if (SrvClientCommon.IsClientArchived(item.OrganisationId))
                            {
                                item.IsArchived = true;
                            }
                            else
                            {
                                item.IsArchived = false;
                            }
                        }

                    });

                    returnValue.Clients = clientList;
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

        #region Construct UCN

        /// <summary>
        /// Constructs the UCN.
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the logon service</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="forename">The forename.</param>
        /// <param name="surname">The surname.</param>
        /// <returns></returns>
        public ReturnValue ConstructUCN(Guid logonId, DateTime dateOfBirth, string forename,
                                        string surname)
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
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    returnValue.Message = SrvClientCommon.ConstructUCN(dateOfBirth, forename, surname);
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

        #region Joint Client Candidate Search

        public JointClientCandidateSearchReturnValue JointClientCandidateSearch(Guid logonId, CollectionRequest collectionRequest,
                                            JointClientCandidateSearchCriteria criteria)
        {
            JointClientCandidateSearchReturnValue returnValue = new JointClientCandidateSearchReturnValue();

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

                    // Create a data list creator for a list of clients
                    DataListCreator<JointClientCandidateSearchItem> dataListCreator = new DataListCreator<JointClientCandidateSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        //Add the table to the dataset as the lookup is returning a datatable
                        e.DataSet = new System.Data.DataSet();
                        e.DataSet.Tables.Add(SrvMatterLookup.GetJointClientCandidatesforListView(criteria.ClientId, criteria.IsMember));
                    };

                    // Create the data list
                    returnValue.JointClientCandidates = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "JointClientCandidates",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Name", "Text"),
                            new ImportMapping("Tag", "Tag"),
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

        #region Gets the clients defaults

        /// <summary>
        /// Gets the client type id and the default branch.
        /// </summary>
        /// <param name="logonId">The logon id.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="isMember">if set to <c>true</c> [is member].</param>
        /// <returns></returns>
        public ClientReturnValue GetClientDefaults(Guid logonId, Guid clientId)
        {
            ClientReturnValue returnValue = new ClientReturnValue();

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

                    string errorMessage = string.Empty;
                    string warningMessage = string.Empty;
                    SrvMatter srvMatter = new SrvMatter();
                    srvMatter.ClientId = clientId;
                    bool success = srvMatter.ValidateClientId(out errorMessage, out warningMessage);
                    if (success)
                    {
                        returnValue.Client = new Client();
                        returnValue.Client.TypeId = srvMatter.ClientTypeId;
                        returnValue.Client.Branch = srvMatter.DefaultBranchId;
                    }
                    else
                    {
                        returnValue.Success = false;
                        returnValue.Message = errorMessage;
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

        #region ConflictCheck

        public ConflictCheckStandardReturnValue ConflictCheck(Guid logonId,
            CollectionRequest collectionRequest,
            IRISLegal.IlbCommon.ContactType clientType,
            Person person,
            Organisation organisation,
            Address addresses,
            List<AdditionalAddressElement> addressInformation, 
            bool checkOnAllRoles)
        {
            ConflictCheckStandardReturnValue returnValue = new ConflictCheckStandardReturnValue();
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
                    DataListCreator<ConflictCheckStandardItem> dataListCreator = new DataListCreator<ConflictCheckStandardItem>();
                    DsConflictCheckStandard dsConflictCheckStandard = new DsConflictCheckStandard();

                    //Assign all control values to ConflictCheckFields
                    SrvConflictCheckFields conflictFields = new SrvConflictCheckFields();
                    SrvConflictCheck conflictCheck = new SrvConflictCheck();
                    conflictFields.ConflictCheckClientType = clientType;

                    if (person != null)
                    {
                        conflictFields.PersonSurname = person.Surname;
                        conflictFields.PersonName = person.ForeName;
                        conflictFields.PersonTitle = person.Title;
                    }
                    if (organisation != null)
                    {
                        conflictFields.OrgName = organisation.Name;
                    }
                    if (addresses != null)
                    {
                        conflictFields.AddressLine1 = addresses.Line1;
                        conflictFields.AddressLine2 = addresses.Line2;
                        conflictFields.AddressLine3 = addresses.Line3;
                        conflictFields.AddressCountry = addresses.Country;
                        conflictFields.AddressCounty = addresses.County;
                        conflictFields.AddressDepLocality = addresses.DependantLocality;
                        conflictFields.AddressDept = addresses.Department;
                        conflictFields.AddressDXNumber = addresses.DXNumber;
                        conflictFields.AddressDXTown = addresses.DXTown;
                        conflictFields.AddressHouseName = addresses.HouseName;
                        conflictFields.AddressOrgName = addresses.OrganisationName;
                        conflictFields.AddressPoBox = addresses.PostBox;
                        conflictFields.AddressPostCode = addresses.PostCode;
                        conflictFields.AddressStreetNo = addresses.StreetNumber;
                        conflictFields.AddressSubBldg = addresses.SubBuilding;
                        conflictFields.AddressTown = addresses.Town;
                    }

                    if (addressInformation != null)
                    {
                        for (int i = 0; i <= addressInformation.Count - 1; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    conflictFields.ConflictCheck1 = addressInformation[i].ElementText;
                                    break;
                                case 1:
                                    conflictFields.ConflictCheck2 = addressInformation[i].ElementText;
                                    break;
                                case 2:
                                    conflictFields.ConflictCheck3 = addressInformation[i].ElementText;
                                    break;
                                case 3:
                                    conflictFields.ConflictCheck4 = addressInformation[i].ElementText;
                                    break;
                                case 4:
                                    conflictFields.ConflictCheck5 = addressInformation[i].ElementText;
                                    break;
                                case 5:
                                    conflictFields.ConflictCheck6 = addressInformation[i].ElementText;
                                    break;
                                case 6:
                                    conflictFields.ConflictCheck7 = addressInformation[i].ElementText;
                                    break;
                                case 7:
                                    conflictFields.ConflictCheck8 = addressInformation[i].ElementText;
                                    break;
                                case 8:
                                    conflictFields.ConflictCheck9 = addressInformation[i].ElementText;
                                    break;
                                case 9:
                                    conflictFields.ConflictCheck10 = addressInformation[i].ElementText;
                                    break;
                            }
                        }
                    }

                    //dsConflictCheckStandard = conflictCheck.DoConflictCheck(conflictFields, ApplicationSettings.Instance.DbUid);
                    IRIS.Law.PmsCommonData.DsConflictCheckStandard dsConflictCheckStandard1 = null;
                    IRIS.Law.PmsCommonData.DsConflictCheckAddress dsConflictCheckAddress1 = null;
                    IRIS.Law.PmsCommonData.DsConflictCheckAdditionalAddress dsConflictCheckAdditionalAddress1 = null;
                    IRIS.Law.PmsCommonData.DsConflictCheckExtendedInfo dsConflictCheckExtendedInfo1 = null;
                    var sbSummary = conflictCheck.DoConflictCheck(conflictFields, out dsConflictCheckStandard1, out dsConflictCheckAddress1, out dsConflictCheckAdditionalAddress1, out dsConflictCheckExtendedInfo1, DataConstants.DummyGuid, checkOnAllRoles);
                    
                    
                    
                    if (dsConflictCheckStandard1.uvw_ConflictCheckStandard.Rows.Count > 0 || dsConflictCheckAddress1.uvw_ConflictCheckAddress.Rows.Count > 0 || dsConflictCheckAdditionalAddress1.uvw_ConflictCheckAdditionalAddress.Rows.Count > 0 || dsConflictCheckExtendedInfo1.uvw_ConflictCheckExtendedInfo.Rows.Count > 0)
                    {
                        returnValue.IsConflict = true;
                    }
                    else
                    {
                        returnValue.IsConflict = false;
                    }

                    returnValue.Summary = new StringBuilder(sbSummary);

                    collectionRequest.ForceRefresh = true;

                    //returnValue.IsConflict = conflictCheck.IsConflict;

                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        // Perform Sorting on SelectType
                        DataView dvConflictCheck = new DataView(dsConflictCheckStandard1.Tables[0]);
                        dvConflictCheck.Sort = "SelectType asc";

                        DataSet dsSorted = new DataSet();
                        dsSorted.Tables.Add(dvConflictCheck.ToTable());
                        e.DataSet = dsSorted;
                        //e.DataSet = dsConflictCheckStandard1;
                    };

                    returnValue.ConflictCheckStandard = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ConflictCheckStandard",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        "",
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("ClientRef", "cliRef"),
                            new ImportMapping("SelectType", "SelectType"),
                            new ImportMapping("PersonTitle", "PersonTitle"),
                            new ImportMapping("PersonName", "PersonName"),
                            new ImportMapping("PersonSurName", "PersonSurName"),
                            new ImportMapping("AddressStreetNo", "AddressStreetNo"),
                            new ImportMapping("AddressHouseName", "AddressHouseName"),
                            new ImportMapping("AddressLine1", "AddressLine1"),
                            new ImportMapping("OrgName", "OrgName"),
                            new ImportMapping("IsMember", "IsMember")
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = "Could not load conflict check fields for the current user." + Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region GetClientAddress

        /// <summary>
        /// Get the address for the specified client  
        /// </summary>
        /// <param name="logonId"></param>
        /// <param name="collectionRequest"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public AddressSearchReturnValue GetClientAddresses(Guid logonId, CollectionRequest collectionRequest,
                                        AddressSearchCriteria criteria)
        {
            AddressSearchReturnValue returnValue = new AddressSearchReturnValue();
            SrvAddress srvAddress;

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
                            if (!ApplicationSettings.Instance.IsUser(criteria.MemberId, criteria.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvClientCommon.WebAllowedToAccessClient(criteria.MemberId, criteria.OrganisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of banks
                    DataListCreator<Address> dataListCreator = new DataListCreator<Address>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        if (criteria.MemberId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetMemberAddresses(criteria.MemberId);
                        }
                        else if (criteria.OrganisationId != DataConstants.DummyGuid)
                        {
                            e.DataSet = SrvAddressLookup.GetOrganisationAddresses(criteria.OrganisationId);
                        }
                    };

                    // Create the data list
                    returnValue.Addresses = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "ClientAddresses",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "AddressId"),
                            new ImportMapping("TypeId", "AddressTypeID"),
                            new ImportMapping("Line1", "AddressLine1"),
                            new ImportMapping("Line2", "AddressLine2"),
                            new ImportMapping("Line3", "AddressLine3"),
                            new ImportMapping("Town", "AddressTown"),
                            new ImportMapping("County", "AddressCounty"),
                            new ImportMapping("PostCode", "AddressPostCode"),
                            new ImportMapping("DXTown", "AddressDXTown"),
                            new ImportMapping("DXNumber", "AddressDX"),
                            new ImportMapping("Country", "AddressCountry"),
                            new ImportMapping("IsMailingAddress", "AddressMailingAddress"),
                            new ImportMapping("IsBillingAddress", "AddressBillingAddress"),
                            new ImportMapping("Comment", "AddressComment"),
                            new ImportMapping("OrganisationName", "AddressOrgName"),
                            new ImportMapping("Department", "AddressDept"),
                            new ImportMapping("PostBox", "AddressPoBox"),
                            new ImportMapping("SubBuilding", "AddressSubBldg"),
                            new ImportMapping("StreetNumber", "AddressStreetNo"),
                            new ImportMapping("HouseName", "AddressHouseName"),
                            new ImportMapping("DependantLocality", "AddressDepLocality"),
                            new ImportMapping("LastVerified", "AddressLastVerified"),
                            }
                        );

                    int intCtr;
                    foreach (Address address in returnValue.Addresses.Rows)
                    {
                        srvAddress = new SrvAddress();
                        srvAddress.MemberId = criteria.MemberId;
                        srvAddress.OrganisationId = criteria.OrganisationId;
                        srvAddress.Load(address.Id);
                        address.AdditionalAddressElements = new List<AdditionalAddressElement>();
                        for (intCtr = 0; intCtr < srvAddress.AdditionalInfoElements.Count; intCtr++)
                        {
                            AdditionalAddressElement additionalAddressElement = new AdditionalAddressElement();
                            additionalAddressElement.ElementComment = srvAddress.AdditionalInfoElements[intCtr].AddressElComment;
                            additionalAddressElement.ElementText = srvAddress.AdditionalInfoElements[intCtr].AddressElementText;
                            additionalAddressElement.TypeId = srvAddress.AdditionalInfoElements[intCtr].AddressElTypeId;
                            address.AdditionalAddressElements.Add(additionalAddressElement);
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
            catch (Exception Ex)
            {
                returnValue.Success = false;
                returnValue.Message = Ex.Message;
            }

            return returnValue;
        }

        #endregion

        #region Get Client Name
        /// <summary>
        /// Get client name
        /// </summary>
        /// <param name="logonId">Logon id obtained when logging on to the Client service</param>
        /// <param name="memOrOrgId">Member or Org id</param>
        /// <param name="isMember">boolean value (is member)</param>
        /// <returns></returns>
        public ClientDetailReturnValue GetClientDetail(Guid logonId, Guid memOrOrgId, bool isMember)
        {
            ClientDetailReturnValue returnValue = new ClientDetailReturnValue();

            try
            {
                // Get the logged on user from the current logons and add their 
                // ApplicationSettings the list of concurrent sessions.
                // ApplicationSettings.Instance can now be used to get the 
                // ApplicationSettings for this session.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Guid memberId = DataConstants.DummyGuid;
                    Guid organisationId = DataConstants.DummyGuid;

                    if (isMember)
                        memberId = memOrOrgId;
                    else
                        organisationId = memOrOrgId;
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.ThirdParty:
                            if (!SrvClientCommon.WebAllowedToAccessClient(memberId, organisationId))
                                throw new Exception("Access denied");
                            break;
                        case DataConstants.UserType.Client:
                            if (!ApplicationSettings.Instance.IsUser(memberId, organisationId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    returnValue.ClientReference = SrvClientCommon.GetClientReference(memOrOrgId);
                    returnValue.Name = SrvClientCommon.GetClientName(memOrOrgId, isMember);
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


        #region Rating

        public RatingSearchReturnValue RatingSearch(Guid logonId, CollectionRequest collectionRequest,
                                                    RatingSearchCriteria criteria)
        {
            RatingSearchReturnValue returnValue = new RatingSearchReturnValue();

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
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Can do everything
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of ratings
                    DataListCreator<RatingSearchItem> dataListCreator = new DataListCreator<RatingSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        e.DataSet = SrvRatingLookup.GetRating(criteria.IncludeArchived);

                        DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], "RatingDesc");
                        e.DataSet.Tables.Remove(e.DataSet.Tables[0]);
                        e.DataSet.Tables.Add(dt);
                    };

                    // Create the data list
                    returnValue.Ratings = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "RatingSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        criteria.ToString(),
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "RatingID"),
                            new ImportMapping("Description", "RatingDesc"),
                            new ImportMapping("IsArchived", "RatingArchived")
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
    }
}

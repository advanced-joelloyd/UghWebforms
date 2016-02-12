using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace IRIS.Law.WebApp.App_Code
{
    /// <summary>
    /// Used to get the data for mockups
    /// </summary>
    public static class DataTables
    {
        public static DataTable GetFeeEarners()
        {
            DataTable feeEarners = new DataTable();
            feeEarners.Columns.Add("Name");
            feeEarners.Rows.Add("Mr Andrew Gareth Brown");
            feeEarners.Rows.Add("Mr Andrew Shaw");
            feeEarners.Rows.Add("Mr Ben Williams");
            feeEarners.Rows.Add("Mr Chris - Inactive Matters Nott");
            feeEarners.Rows.Add("Mr Chris Turnbull");
            feeEarners.Rows.Add("Mr David Alwyn Jones");
            return feeEarners;
        }

        public static DataTable GetWorkTypes()
        {
            DataTable workTypes = new DataTable();
            workTypes.Columns.Add("Description");
            workTypes.Rows.Add("Domestic Conveyancing");
            workTypes.Rows.Add("First Time Buyers");
            workTypes.Rows.Add("Right to Buy");
            workTypes.Rows.Add("Probate Sales");
            workTypes.Rows.Add("Re-Mortgage Work");
            workTypes.Rows.Add("Other Sales");
            return workTypes;
        }

        public static DataTable GetClientBanks()
        {
            DataTable clientBanks = new DataTable();
            clientBanks.Columns.Add("Name");
            clientBanks.Rows.Add("Allied Irish Bank - Client Account");
            clientBanks.Rows.Add("Client monies held by CL Insurance");
            return clientBanks;
        }

        public static DataTable GetOfficeBanks()
        {
            DataTable officeBanks = new DataTable();
            officeBanks.Columns.Add("Name");
            officeBanks.Rows.Add("Allied Irish Bank - Office Account");
            officeBanks.Rows.Add("Royal Bank Of Scotland");
            return officeBanks;
        }

        public static DataTable GetBranches()
        {
            DataTable branches = new DataTable();
            branches.Columns.Add("Name");
            branches.Rows.Add("Cardiff");
            branches.Rows.Add("Grantham");
            branches.Rows.Add("Liverpool");
            branches.Rows.Add("Cambridge");
            branches.Rows.Add("Worcester");
            return branches;
        }

        public static DataTable GetDepartments()
        {
            DataTable departments = new DataTable();
            departments.Columns.Add("Name");
            departments.Rows.Add("Residential Property");
            departments.Rows.Add("Commercial Property");
            departments.Rows.Add("Corporate");
            departments.Rows.Add("Litigation");
            departments.Rows.Add("Employment");
            departments.Rows.Add("Criminal");
            return departments;
        }

        public static DataTable GetChargeRates()
        {
            DataTable chargeRates = new DataTable();
            chargeRates.Columns.Add("Description");
            chargeRates.Rows.Add("Advice & Assistance - Investigation - No Court");
            chargeRates.Rows.Add("Advice & Assistance - Investigation - Police Station");
            chargeRates.Rows.Add("Advice & Assistance - Proceedings - No Court");
            chargeRates.Rows.Add("Advocacy & Assistance - Investigation - Police Station");
            chargeRates.Rows.Add("Advocacy & Assistance - Proceedings - Duty Court");
            chargeRates.Rows.Add("Advocacy & Assistance - Investigation - Magistrate Court");
            return chargeRates;
        }

        public static DataTable GetCourtTypes()
        {
            DataTable courtTypes = new DataTable();
            courtTypes.Columns.Add("Description");
            courtTypes.Rows.Add("No Court");
            courtTypes.Rows.Add("Police Station");
            courtTypes.Rows.Add("Duty Court");
            courtTypes.Rows.Add("Magistrates Court");
            courtTypes.Rows.Add("County Court");
            return courtTypes;
        }

        public static DataTable GetDepositBanks()
        {
            DataTable depositBanks = new DataTable();
            depositBanks.Columns.Add("Description");
            depositBanks.Rows.Add("Not Set");
            depositBanks.Rows.Add("Deposit Bank");
            return depositBanks;
        }

        public static DataTable GetBusinessSources()
        {
            DataTable businessSources = new DataTable();
            businessSources.Columns.Add("Description");
            businessSources.Rows.Add("Not Set");
            businessSources.Rows.Add("Evening Post");
            businessSources.Rows.Add("Referral");
            businessSources.Rows.Add("Yellow Pages");
            return businessSources;
        }

        public static DataTable GetPersonDealing()
        {
            DataTable personDealing = new DataTable();
            personDealing.Columns.Add("Name");
            personDealing.Rows.Add("Mr Al Newman");
            personDealing.Rows.Add("Mr Andrew Hawley");
            personDealing.Rows.Add("Mr Andrew Reilly");
            personDealing.Rows.Add("Mr Anthony Gaffney");
            personDealing.Rows.Add("Mr Chris Clarkson");
            return personDealing;
        }

        public static DataTable GetRatings()
        {
            DataTable ratings = new DataTable();
            ratings.Columns.Add("Description");
            ratings.Rows.Add("Excellent Client");
            ratings.Rows.Add("Not Set");
            ratings.Rows.Add("Poor Rated Client");
            ratings.Rows.Add("Regular Client");
            return ratings;
        }

        public static DataTable GetAdditionalAddressInfo()
        {
            DataTable additionalAddressInfo = new DataTable();
            additionalAddressInfo.Columns.Add("Description");
            additionalAddressInfo.Columns.Add("Details");
            additionalAddressInfo.Columns.Add("Notes");
            additionalAddressInfo.Rows.Add("Work Telephone 1", "99887745121", "");
            additionalAddressInfo.Rows.Add("Work Telephone 2");
            additionalAddressInfo.Rows.Add("Mobile Telephone 1");
            additionalAddressInfo.Rows.Add("Mobile Telephone 2");
            additionalAddressInfo.Rows.Add("Fax");
            return additionalAddressInfo;
        }

        public static DataTable GetSubTypes()
        {
            DataTable subTypes = new DataTable();
            subTypes.Columns.Add("Description");
            subTypes.Rows.Add("Partnership");
            subTypes.Rows.Add("Sole Trader");
            subTypes.Rows.Add("Private Companies, LLPs");
            subTypes.Rows.Add("Other");
            subTypes.Rows.Add("Church");
            subTypes.Rows.Add("Estate Executors");
            return subTypes;
        }

        /// <summary>
        /// Gets the address types.
        /// </summary>
        /// <param name="isMember">if set to <c>true</c> [is member].</param>
        /// <returns>Address types for member if true else organisation address types</returns>
        public static DataTable GetAddressTypes(bool isMember)
        {
            DataTable addressTypes = new DataTable();
            addressTypes.Columns.Add("Type");
            addressTypes.Rows.Add("Main");
            addressTypes.Rows.Add("Previous");
            addressTypes.Rows.Add("Alternate Billing");
            if (isMember == false)
            {
                addressTypes.Rows.Add("Registered Office");
                addressTypes.Rows.Add("Other Trading");
            }
            return addressTypes;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="addressType">Type of the address.</param>
        /// <returns></returns>
        public static DataTable GetAddress(string addressType)
        {
            DataTable address = new DataTable();
            address.Columns.Add("HouseNumber");
            address.Columns.Add("Postcode");
            address.Columns.Add("HouseName");
            address.Columns.Add("AddressLine1");
            address.Columns.Add("AddressLine2");
            address.Columns.Add("AddressLine3");
            address.Columns.Add("Town");
            address.Columns.Add("County");
            address.Columns.Add("Country");
            address.Columns.Add("DXTown");
            address.Columns.Add("DXNumber");
            address.Columns.Add("IsMailingAddress", typeof(bool));
            address.Columns.Add("IsBillingAddress", typeof(bool));
            //Additional info
            address.Columns.Add("Organisation");
            address.Columns.Add("Department");
            address.Columns.Add("POBox");
            address.Columns.Add("SubBuildingName");
            address.Columns.Add("DependantLocality");
            address.Columns.Add("Comment");
            address.Columns.Add("LastVerified");

            if (addressType == "Main")
            {
                address.Rows.Add("", "NG31 5IP", "", "35 Britain Drive", "", "", "Grantham", "Lincolnshire", "", "", "", true, true);
                address.Rows[0]["Comment"] = "Address comment";
            }
            else
            {
                address.Rows.Add("");
                address.Rows[0]["IsMailingAddress"] = false;
                address.Rows[0]["IsBillingAddress"] = false;
            }

            return address;
        }

        public static DataTable GetTitles()
        {
            DataTable titles = new DataTable();
            titles.Columns.Add("Title");
            titles.Rows.Add("Dr");
            titles.Rows.Add("Miss");
            titles.Rows.Add("Mr");
            titles.Rows.Add("Mrs");
            titles.Rows.Add("Ms");
            return titles;
        }

        public static object GetSexData()
        {
            DataTable sex = new DataTable();
            sex.Columns.Add("Sex");
            sex.Rows.Add("Male");
            sex.Rows.Add("Female");
            sex.Rows.Add("Prefer not to say");
            return sex;
        }

        public static object GetMaritialStatus()
        {
            DataTable maritalStatus = new DataTable();
            maritalStatus.Columns.Add("Status");
            maritalStatus.Rows.Add("Unknown/Other");
            maritalStatus.Rows.Add("Married");
            maritalStatus.Rows.Add("Divorced");
            maritalStatus.Rows.Add("Seperated");
            maritalStatus.Rows.Add("Widowed");
            maritalStatus.Rows.Add("Cohabiting");
            return maritalStatus;
        }

        public static object GetEthnicity()
        {
            DataTable ethnicity = new DataTable();
            ethnicity.Columns.Add("Description");
            ethnicity.Rows.Add("Other");
            ethnicity.Rows.Add("White British");
            ethnicity.Rows.Add("White Irish");
            ethnicity.Rows.Add("Chinese");
            ethnicity.Rows.Add("Mixed White and Asian");
            ethnicity.Rows.Add("Unknown");
            return ethnicity;
        }

        public static object GetDisablity()
        {
            DataTable disability = new DataTable();
            disability.Columns.Add("Description");
            disability.Rows.Add("Not considered disabled");
            disability.Rows.Add("Unknown");
            disability.Rows.Add("Physical Impairment");
            disability.Rows.Add("Sensory Impairment");
            disability.Rows.Add("Cognitive Impairment");
            disability.Rows.Add("Other");
            return disability;
        }

        public static DataTable GetMatters()
        {
            DataTable matters = new DataTable();
            matters.Columns.Add("Ref");
            matters.Columns.Add("Description");
            matters.Columns.Add("Department");
            matters.Columns.Add("Branch");
            matters.Columns.Add("FeeEarnerRef");
            matters.Columns.Add("FeeEarner");
            matters.Columns.Add("WorkType");
            matters.Columns.Add("UFN");
            matters.Columns.Add("Opened");
            matters.Columns.Add("Closed");
            matters.Columns.Add("KeyDescription");
            matters.Rows.Add("A00001-0001", "Span matter for Grantham", "Criminal", "Cardiff", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0002", "Span matter for London", "Criminal", "Grantham", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0003", "Span matter for Manchester", "Criminal", "Liverpool", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0004", "SPOCC matter for Grantham", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "10/09/2008");
            matters.Rows.Add("A00001-0005", "Grantham File Reviews", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "07/10/2008");
            matters.Rows.Add("A00001-0006", "NonChargeable Time recording", "Criminal", "Cardiff", "BEN", "Mr Ben Williams", "Re-Mortgage Work", "", "28/06/2009", "02/07/2010", "9961/1");
            matters.Rows.Add("A00001-0007", "Chargeable Time recording", "Litigation", "Cambridge", "CT", "Mr Chris Turnbull", "Probate Sales", "", "08/06/2009", "08/07/2010", "9917/1");
            matters.Rows.Add("A00001-0008", "33 Terfyn, Ynysawdre, Bridgend", "Employment", "Cambridge", "CT", "Mr Chris Turnbull", "Domestic Conveyancing", "", "03/02/2006", "26/07/2006", "58682/345");
            matters.Rows.Add("A00001-0009", "Terms & Conditions", "Employment", "Cambridge", "DAJ", "Mr David Alwyn Jones", "Right to Buy", "", "03/02/2006", "26/07/2006", "85301/2");
            matters.Rows.Add("A00001-0001", "Span matter for Grantham", "Criminal", "Cardiff", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0002", "Span matter for London", "Criminal", "Grantham", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0003", "Span matter for Manchester", "Criminal", "Liverpool", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0004", "SPOCC matter for Grantham", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "10/09/2008");
            matters.Rows.Add("A00001-0005", "Grantham File Reviews", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "07/10/2008");
            matters.Rows.Add("A00001-0006", "NonChargeable Time recording", "Criminal", "Cardiff", "BEN", "Mr Ben Williams", "Re-Mortgage Work", "", "28/06/2009", "02/07/2010", "9961/1");
            matters.Rows.Add("A00001-0007", "Chargeable Time recording", "Litigation", "Cambridge", "CT", "Mr Chris Turnbull", "Probate Sales", "", "08/06/2009", "08/07/2010", "9917/1");
            matters.Rows.Add("A00001-0008", "33 Terfyn, Ynysawdre, Bridgend", "Employment", "Cambridge", "CT", "Mr Chris Turnbull", "Domestic Conveyancing", "", "03/02/2006", "26/07/2006", "58682/345");
            matters.Rows.Add("A00001-0009", "Terms & Conditions", "Employment", "Cambridge", "DAJ", "Mr David Alwyn Jones", "Right to Buy", "", "03/02/2006", "26/07/2006", "85301/2");
            matters.Rows.Add("A00001-0001", "Span matter for Grantham", "Criminal", "Cardiff", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0002", "Span matter for London", "Criminal", "Grantham", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0003", "Span matter for Manchester", "Criminal", "Liverpool", "AGB", "Mr Andrew Gareth Brown", "Probate Sales", "", "24/09/2008");
            matters.Rows.Add("A00001-0004", "SPOCC matter for Grantham", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "10/09/2008");
            matters.Rows.Add("A00001-0005", "Grantham File Reviews", "Criminal", "Cardiff", "AS", "Mr Andrew Shaw", "Probate Sales", "", "07/10/2008");
            matters.Rows.Add("A00001-0006", "NonChargeable Time recording", "Criminal", "Cardiff", "BEN", "Mr Ben Williams", "Re-Mortgage Work", "", "28/06/2009", "02/07/2010", "9961/1");
            matters.Rows.Add("A00001-0007", "Chargeable Time recording", "Litigation", "Cambridge", "CT", "Mr Chris Turnbull", "Probate Sales", "", "08/06/2009", "08/07/2010", "9917/1");
            matters.Rows.Add("A00001-0008", "33 Terfyn, Ynysawdre, Bridgend", "Employment", "Cambridge", "CT", "Mr Chris Turnbull", "Domestic Conveyancing", "", "03/02/2006", "26/07/2006", "58682/345");
            matters.Rows.Add("A00001-0009", "Terms & Conditions", "Employment", "Cambridge", "DAJ", "Mr David Alwyn Jones", "Right to Buy", "", "03/02/2006", "26/07/2006", "85301/2");
            return matters;
        }

        public static DataTable GetClients()
        {
            DataTable clients = new DataTable();
            clients.Columns.Add("cliref");
            clients.Columns.Add("OrgName");
            clients.Columns.Add("AddressLine1");
            clients.Columns.Add("PersonName");
            clients.Columns.Add("PersonDOB");
            clients.Columns.Add("cliBranch");

            clients.Rows.Add("A00001", "Paper Chambers", "Road 1, Worli", "Mr Liam Atkinson", "01/09/1950", "Cardiff");
            clients.Rows.Add("A00002", "Duncan & Toplis", "Road 5, Worli", "PQR", "01/09/1951", "Grantham");
            clients.Rows.Add("G00001", "British Gas", "Road 2, Worli", "Mr Guam Guy", "01/09/1952", "Liverpool");
            clients.Rows.Add("H00001", "Syscap Finance", "Road 2, Worli", "Mr David Heart", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00001", "HMP Dartmoor", "Road 4, Worli", "Mr David Johnson", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00002", "Gumpton & Partners", "Road 2, Worli", "Mr Derek Jones", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00003", "British Telecom", "Road 8, Worli", "Mr Jed Jones", "01/09/1952", "Liverpool");
            clients.Rows.Add("A00001", "Paper Chambers", "Road 1, Worli", "Mr Liam Atkinson", "01/09/1950", "Cardiff");
            clients.Rows.Add("A00002", "Duncan & Toplis", "Road 5, Worli", "PQR", "01/09/1951", "Grantham");
            clients.Rows.Add("G00001", "British Gas", "Road 2, Worli", "Mr Guam Guy", "01/09/1952", "Liverpool");
            clients.Rows.Add("H00001", "Syscap Finance", "Road 2, Worli", "Mr David Heart", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00001", "HMP Dartmoor", "Road 4, Worli", "Mr David Johnson", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00002", "Gumpton & Partners", "Road 2, Worli", "Mr Derek Jones", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00003", "British Telecom", "Road 8, Worli", "Mr Jed Jones", "01/09/1952", "Liverpool");
            clients.Rows.Add("A00001", "Paper Chambers", "Road 1, Worli", "Mr Liam Atkinson", "01/09/1950", "Cardiff");
            clients.Rows.Add("A00002", "Duncan & Toplis", "Road 5, Worli", "PQR", "01/09/1951", "Grantham");
            clients.Rows.Add("G00001", "British Gas", "Road 2, Worli", "Mr Guam Guy", "01/09/1952", "Liverpool");
            clients.Rows.Add("H00001", "Syscap Finance", "Road 2, Worli", "Mr David Heart", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00001", "HMP Dartmoor", "Road 4, Worli", "Mr David Johnson", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00002", "Gumpton & Partners", "Road 2, Worli", "Mr Derek Jones", "01/09/1952", "Liverpool");
            clients.Rows.Add("J00003", "British Telecom", "Road 8, Worli", "Mr Jed Jones", "01/09/1952", "Liverpool");
            return clients;
        }

        public static object GetAssociationRoles()
        {
            DataTable associationRoles = new DataTable();
            associationRoles.Columns.Add("AssociationRole");
            associationRoles.Rows.Add("General");
            associationRoles.Rows.Add("Banker");
            associationRoles.Rows.Add("Auditor");
            associationRoles.Rows.Add("Spouse");
            associationRoles.Rows.Add("Child");
            return associationRoles;
        }

        public static DataTable GetDocumentFiles()
        {
            DataTable docs = new DataTable();
            docs.Columns.Add("DocDesc");
            docs.Columns.Add("FeeEarner");
            docs.Columns.Add("CreationDate");
            docs.Columns.Add("DocName");
            docs.Columns.Add("DocNotes");
            docs.Columns.Add("DocType");
            docs.Columns.Add("DocPath");
            docs.Rows.Add("High Level Architecture", "Mr Andrew Gareth Brown", "17/07/2009", "ILB - Refactoring.doc", "Claim Cost Summary Sheet - Investigation", "Criminal", @"C:\IrisLawBusiness\Documents\2009\July\Week 03\ILB...docx");
            docs.Rows.Add("CDS_12", "Mr Andrew Shaw", "02/01/2009", "S000010002.1.1.rpt", "Claim Cost", "", @"C:\IrisLawBusiness\Documents\2009\July\Week 03\S000010002.1.1.rpt");
            return docs;
        }

        public static DataTable GetDocumentTypes()
        {
            DataTable docTypes = new DataTable();
            docTypes.Columns.Add("Description");
            docTypes.Rows.Add("MS Word Documents");
            docTypes.Rows.Add("MS Excel Documents");
            docTypes.Rows.Add("Plain Text");
            docTypes.Rows.Add("XML Documents");
            docTypes.Rows.Add("Reports");
            docTypes.Rows.Add("Email Messages");
            docTypes.Rows.Add("All Files");
            return docTypes;
        }

        public static DataTable GetImportDocumentTypes()
        {
            DataTable docTypes = new DataTable();
            docTypes.Columns.Add("Description");
            docTypes.Rows.Add("General");
            docTypes.Rows.Add("Document");
            return docTypes;
        }

        public static DataTable GetServices()
        {
            DataTable services = new DataTable();
            services.Columns.Add("ServiceName");
            services.Columns.Add("Address1");
            services.Columns.Add("Town");
            services.Columns.Add("Postcode");
            services.Rows.Add("Duncon & Toplis", "4 Castlegate", "Grantham", "NG31 6SE");
            services.Rows.Add("Barclays Bank", "", "", "");
            services.Rows.Add("Lloyds Tsb", "42 St Peters Hill", "Grantham", "NG31 6SE");
            services.Rows.Add("Royal Bank of Scotland", "24 Grosvenor Place", "London", "SW1X 7HN");
            return services;
        }

        public static DataTable GetUsers()
        {
            DataTable feeEarners = new DataTable();
            feeEarners.Columns.Add("FeeEarner");
            feeEarners.Rows.Add("Al Newman");
            feeEarners.Rows.Add("Andrew Hawley");
            feeEarners.Rows.Add("Chris Clarkson");
            feeEarners.Rows.Add("Chris Rose");
            feeEarners.Rows.Add("Dan Young");
            feeEarners.Rows.Add("Daren Bogan");
            feeEarners.Rows.Add("Karl Keagler");
            feeEarners.Rows.Add("Sean Town");
            feeEarners.Rows.Add("Thomas Wills");
            return feeEarners;
        }

        public static DataTable GetAppointmentsList()
        {
            DataTable appointments = new DataTable();
            appointments.Columns.Add("Id");
            appointments.Columns.Add("Date");
            appointments.Columns.Add("StartTime");
            appointments.Columns.Add("EndTime");
            appointments.Columns.Add("Duration");
            appointments.Columns.Add("Venue");
            appointments.Columns.Add("Subject");
            appointments.Columns.Add("Notes");
            appointments.Columns.Add("Matter");
            appointments.Columns.Add("Client");

            appointments.Rows.Add("1", "20/09/2009", "12:00", "14:00", "02:00", "Grantham", "ASP.Net Training", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("2", "21/09/2009", "09:00", "12:00", "03:00", "London", "Family Problem", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("3", "22/09/2009", "10:00", "16:00", "04:00", "Manchester", "Divorce", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("4", "23/09/2009", "11:00", "13:30", "04:30", "Birmingum", "Conveying", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("5", "24/09/2009", "15:00", "16:00", "01:00", "London", "Probate", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("6", "25/09/2009", "13:00", "15:00", "02:00", "Nothingam", "Wills", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("7", "26/09/2009", "11:00", "13:00", "02:00", "Norwich", "Deeds", "Notes", "ASP.Net", "Darren");
            appointments.Rows.Add("8", "27/09/2009", "17:00", "18:00", "01:00", "Knutsford", "Personel Injury", "Notes", "ASP.Net", "Darren");

            return appointments;
        }

        public static DataTable GetTaskList()
        {
            DataTable appointments = new DataTable();
            appointments.Columns.Add("Id");
            appointments.Columns.Add("DueDate");
            appointments.Columns.Add("Status");
            appointments.Columns.Add("Subject");
            appointments.Columns.Add("PercentComplete");
            appointments.Columns.Add("Notes");
            appointments.Columns.Add("MatterRef");

            appointments.Rows.Add("1", "27/09/2009", "Not Set", "Diary Mockup", "100 %", "Notes", "(A00001-0072) TC244");
            appointments.Rows.Add("2", "28/09/2009", "Not Set", "Standard Task", "100 %", "Notes", "(A00014-0001) Accident");
            appointments.Rows.Add("3", "29/09/2009", "Not Set", "Sale Exchange", "0 %", "Notes", "(C00004-0055) Issue 12836");
            appointments.Rows.Add("4", "30/09/2009", "Not Set", "Sale Completion", "0 %", "", "");
            appointments.Rows.Add("5", "01/10/2009", "Not Set", "Follow up NCIS report", "100 %", "Notes", "(C00001-0005) AML Task");
            appointments.Rows.Add("6", "02/10/2009", "Not Set", "Claimant Pt 36 Offer Expires  09/09/2009", "100 %", "Notes", "(A00001-0052) Conveyancing");
            appointments.Rows.Add("7", "03/10/2009", "Not Set", "Diary Implementation", "0 %", "", "(A00001-0072) Family");

            return appointments;
        }

        public static DataTable GetTaskStatus()
        {
            DataTable taskStatus = new DataTable();
            taskStatus.Columns.Add("statusText");
            taskStatus.Columns.Add("statusValue");

            //ListItem itemOutStanding = new ListItem("Outstanding Only", "Outstanding");
            //ListItem itemCompleted = new ListItem("Completed Only", "Completed");
            //ListItem itemAll = new ListItem("All Items", "All");
            taskStatus.Rows.Add("Outstanding Only", "Outstanding");
            taskStatus.Rows.Add("Completed Only", "Completed");
            taskStatus.Rows.Add("All Items", "All");
            return taskStatus;
        }

        public static DataTable GetTaskType()
        {
            DataTable taskType = new DataTable();
            taskType.Columns.Add("type");

            taskType.Rows.Add("Standard Task");
            taskType.Rows.Add("Key Date");
            taskType.Rows.Add("Limitation Date");
            return taskType;
        }

        public static DataTable GetReasonType()
        {
            DataTable reasonType = new DataTable();
            reasonType.Columns.Add("reason");

            reasonType.Rows.Add("Cancellation");
            return reasonType;
        }

        public static DataTable GetCategoryType()
        {
            DataTable categoryType = new DataTable();
            categoryType.Columns.Add("category");

            categoryType.Rows.Add("Internal");
            categoryType.Rows.Add("External");
            return categoryType;
        }

        public static DataTable GetDisbursementTypes()
        {
            DataTable disbursementTypes = new DataTable();
            disbursementTypes.Columns.Add("DisbursementTypes");
            disbursementTypes.Rows.Add("HMCS: Court Fee");
            disbursementTypes.Rows.Add("H M Land Registry");
            disbursementTypes.Rows.Add("Office Copies");
            disbursementTypes.Rows.Add("Land Search Fee");
            disbursementTypes.Rows.Add("Inland Revenue: Stamp Duty");
            return disbursementTypes;
        }

        public static DataTable GetVATRates()
        {
            DataTable vatRates = new DataTable();
            vatRates.Columns.Add("VATRate");
            vatRates.Rows.Add("New Value Rate");
            vatRates.Rows.Add("Vatable - Zero Rated");
            vatRates.Rows.Add("Vatable - Rated N");
            vatRates.Rows.Add("Vatable - Exempt");
            vatRates.Rows.Add("Vatable - E.C Rate");
            vatRates.Rows.Add("Standard Rate 1");
            return vatRates;
        }

        /// <summary>
        /// Gets Office cheque requests
        /// </summary>
        /// <returns>Returns office cheque requests</returns>
        public static DataTable GetOfficeChequeRequests()
        {
            DataTable officeChequeRequests = new DataTable();
            officeChequeRequests.Columns.Add("MatterReference");
            officeChequeRequests.Columns.Add("Date");
            officeChequeRequests.Columns.Add("UserRef");
            officeChequeRequests.Columns.Add("FeeEarner");
            officeChequeRequests.Columns.Add("Description");
            officeChequeRequests.Columns.Add("Amount");
            officeChequeRequests.Columns.Add("VATRate");
            officeChequeRequests.Columns.Add("VATAmount");
            officeChequeRequests.Columns.Add("Bank");
            officeChequeRequests.Columns.Add("BankName");

            officeChequeRequests.Rows.Add("A000010004", "08/09/2009", "STEVEJ", "SJ", "H M Land Registry", "10.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000010005", "09/09/2009", "MSH", "DB", "HMCS: Court Fee", "200.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000030006", "11/09/2009", "MSH", "KB", "HMCS: Court Fee", "300.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000010098", "12/09/2009", "MSH", "AN", "H M Land Registry", "100.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000020014", "15/09/2009", "MSH", "AN", "Office Copies", "220.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000010024", "18/09/2009", "MSH", "AN", "H M Land Registry", "120.00", "NV", "0.00", "001", "Allied Irish Bank - Office");
            officeChequeRequests.Rows.Add("A000010034", "21/09/2009", "MSH", "KB", "HMCS: Court Fee", "300.00", "NV", "0.00", "001", "Allied Irish Bank - Office");

            return officeChequeRequests;
        }

        /// <summary>
        /// Gets client cheque requests
        /// </summary>
        /// <returns>Returns client cheque requests</returns>
        public static DataTable GetClientChequeRequests()
        {
            DataTable clientChequeRequests = new DataTable();
            clientChequeRequests.Columns.Add("MatterReference");
            clientChequeRequests.Columns.Add("Date");
            clientChequeRequests.Columns.Add("UserRef");
            clientChequeRequests.Columns.Add("FeeEarner");
            clientChequeRequests.Columns.Add("Description");
            clientChequeRequests.Columns.Add("Amount");
            clientChequeRequests.Columns.Add("Bank");
            clientChequeRequests.Columns.Add("BankName");

            clientChequeRequests.Rows.Add("A000010090", "08/09/2009", "STEVEJ", "SJ", "Description Testing", "10.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("A000010035", "19/09/2009", "MSH", "DB", "Court Fee", "200.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("A000030009", "21/09/2009", "MSH", "KB", "Court Fee", "300.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("A000010003", "22/09/2009", "MSH", "AN", "Desction Testing", "100.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("C000020014", "25/09/2009", "MSH", "AN", "Office Copies", "220.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("C000010024", "28/09/2009", "MSH", "AN", "Description Testing", "120.00", "001", "Allied Irish Bank - Client Account");
            clientChequeRequests.Rows.Add("S000010034", "01/10/2009", "MSH", "KB", "Description Testing", "300.00", "001", "Allied Irish Bank - Client Account");

            return clientChequeRequests;
        }

        /// <summary>
        /// Gets office bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetOfficeBills()
        {
            DataTable officeBills = new DataTable();
            officeBills.Columns.Add("Date");
            officeBills.Columns.Add("Reference");
            officeBills.Columns.Add("Type");
            officeBills.Columns.Add("Description");
            officeBills.Columns.Add("Bank");
            officeBills.Columns.Add("Debit");
            officeBills.Columns.Add("Credit");
            officeBills.Columns.Add("Balance");

            officeBills.Rows.Add("08/09/2009", "Reference", "OBP", "Desction Testing", "Allied Irish Bank - Client Account", "0.00", "1110.00", "-1110.00");
            officeBills.Rows.Add("18/09/2009", "Reference Test", "OBP", "Court Fee", "Allied Irish Bank - Client Account", "30.00", "0.00", "-1080.00");
            officeBills.Rows.Add("08/09/2009", "Reference", "OBP", "Desction Testing", "Allied Irish Bank - Client Account", "330.00", "1110.00", "-750.00");
            officeBills.Rows.Add("18/09/2009", "Reference Test", "OBP", "Court Fee", "Allied Irish Bank - Client Account", "221.00", "0.00", "-529.00");
            officeBills.Rows.Add("08/09/2009", "Reference", "OBP", "Desction Testing", "Allied Irish Bank - Client Account", "880.00", "1110.00", "351.00");
            officeBills.Rows.Add("18/09/2009", "Reference Test", "OBP", "Court Fee", "Allied Irish Bank - Client Account", "0.00", "131.06", "219.94");

            return officeBills;
        }

        /// <summary>
        /// Gets deposit bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDepositBills()
        {
            DataTable depositBills = new DataTable();
            depositBills.Columns.Add("Date");
            depositBills.Columns.Add("Reference");
            depositBills.Columns.Add("Type");
            depositBills.Columns.Add("Description");
            depositBills.Columns.Add("Bank");
            depositBills.Columns.Add("Debit");
            depositBills.Columns.Add("Credit");
            depositBills.Columns.Add("Balance");

            depositBills.Rows.Add("08/09/2009", "Reference", "CDT", "Description Testing", "Allied Irish Bank - Client Account", "0.00", "220.00", "220.00");
            depositBills.Rows.Add("18/09/2009", "Reference Test", "CDT", "Description Test", "Allied Irish Bank - Client Account", "200.00", "0.00", "20.00");

            return depositBills;
        }

        /// <summary>
        /// Gets client bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetClientBills()
        {
            DataTable clientBills = new DataTable();
            clientBills.Columns.Add("Date");
            clientBills.Columns.Add("Reference");
            clientBills.Columns.Add("Type");
            clientBills.Columns.Add("Description");
            clientBills.Columns.Add("Bank");
            clientBills.Columns.Add("Debit");
            clientBills.Columns.Add("Credit");
            clientBills.Columns.Add("Balance");

            clientBills.Rows.Add("08/09/2009", "Reference", "CDT", "Description Testing", "Allied Irish Bank - Client Account", "220.00", "0.00", "-220.00");
            clientBills.Rows.Add("18/09/2009", "Reference Test", "CDT", "Description Test", "Allied Irish Bank - Client Account", "0.00", "20.00", "-20.00");

            return clientBills;
        }

        /// <summary>
        /// Gets disbursement ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDisbursementsLedger()
        {
            DataTable disbursementLedger = new DataTable();
            disbursementLedger.Columns.Add("Date");
            disbursementLedger.Columns.Add("Reference");
            disbursementLedger.Columns.Add("Disbursement");
            disbursementLedger.Columns.Add("Description");
            disbursementLedger.Columns.Add("VAT");
            disbursementLedger.Columns.Add("Paid");
            disbursementLedger.Columns.Add("Amount");
            disbursementLedger.Columns.Add("Claimed");
            disbursementLedger.Columns.Add("UnBilled");
            disbursementLedger.Columns.Add("Balance");

            disbursementLedger.Rows.Add("08/09/2009", "Reference", "HMCS: Court Fee", "Description Testing", "N", "P", "330.00", "0.00", "330.00", "330.00");
            disbursementLedger.Rows.Add("18/09/2009", "Reference Test", "HMCS: Court Fee", "Description Test", "N", "P", "221.00", "0.00", "221.00", "551.00");
            disbursementLedger.Rows.Add("18/09/2009", "Reference", "Water Authority Search", "Description Testing", "V", "P", "748.00", "0.00", "748.00", "1299.94");
            disbursementLedger.Rows.Add("18/09/2009", "Reference Test", "HMCS: Court Fee", "Description Test", "N", "P", "0.00", "-220.00", "-220.00", "1079.94");

            return disbursementLedger;
        }

        /// <summary>
        /// Gets time status
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTimeStatus()
        {
            DataTable timeStatus = new DataTable();
            timeStatus.Columns.Add("TimeStatus");

            timeStatus.Rows.Add("All");
            timeStatus.Rows.Add("Unbilled");
            timeStatus.Rows.Add("Billed");
            timeStatus.Rows.Add("Claimed");
            timeStatus.Rows.Add("W/Off");

            return timeStatus;
        }

        /// <summary>
        /// Gets all the time ledgers for time status "All"
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllTimeLedger()
        {
            DataTable timeLedger = new DataTable();
            timeLedger.Columns.Add("Date");
            timeLedger.Columns.Add("TimeType");
            timeLedger.Columns.Add("Earner");
            timeLedger.Columns.Add("Time");
            timeLedger.Columns.Add("Cost");
            timeLedger.Columns.Add("CostBalance");
            timeLedger.Columns.Add("Charge");
            timeLedger.Columns.Add("Balance");

            timeLedger.Rows.Add("08/09/2009", "Letter Out", "TW", "1:00", "100.00", "100.00", "65.00", "65.00");
            timeLedger.Rows.Add("18/09/2009", "Telephone Out", "KA", "1:00", "0.00", "100.00", "34.50", "99.50");
            timeLedger.Rows.Add("18/09/2009", "Telephone Out", "CR", "1:00", "200.00", "300.00", "41.00", "140.50");
            timeLedger.Rows.Add("18/09/2009", "Attending Client", "DB", "1:00", "100.00", "400.00", "74.00", "214.50");
            timeLedger.Rows.Add("18/09/2009", "Letter In", "DB", "1:00", "100.00", "500.00", "23.50", "238.00");
            timeLedger.Rows.Add("18/09/2009", "Preparation", "AN", "120:00", "24000.00", "24000.00", "5964.00", "5964.00");

            return timeLedger;
        }

        /// <summary>
        /// Gets unbilled time ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUnBilledTimeLedger()
        {
            DataTable unBilledTimeLedger = new DataTable();
            unBilledTimeLedger.Columns.Add("Date");
            unBilledTimeLedger.Columns.Add("TimeType");
            unBilledTimeLedger.Columns.Add("Earner");
            unBilledTimeLedger.Columns.Add("Time");
            unBilledTimeLedger.Columns.Add("Cost");
            unBilledTimeLedger.Columns.Add("CostBalance");
            unBilledTimeLedger.Columns.Add("Charge");
            unBilledTimeLedger.Columns.Add("Balance");

            unBilledTimeLedger.Rows.Add("08/09/2009", "Letter Out", "TW", "1:00", "100.00", "100.00", "65.00", "65.00");
            unBilledTimeLedger.Rows.Add("18/09/2009", "Telephone Out", "KA", "1:00", "0.00", "100.00", "34.50", "99.50");
            unBilledTimeLedger.Rows.Add("18/09/2009", "Telephone Out", "CR", "1:00", "200.00", "300.00", "41.00", "140.50");
            unBilledTimeLedger.Rows.Add("18/09/2009", "Attending Client", "DB", "1:00", "100.00", "400.00", "74.00", "214.50");
            unBilledTimeLedger.Rows.Add("18/09/2009", "Letter In", "DB", "1:00", "100.00", "500.00", "23.50", "238.00");

            return unBilledTimeLedger;
        }

        /// <summary>
        /// Gets claimed time ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetClaimedTimeLedger()
        {
            DataTable claimedTimeLedger = new DataTable();
            claimedTimeLedger.Columns.Add("Date");
            claimedTimeLedger.Columns.Add("TimeType");
            claimedTimeLedger.Columns.Add("Earner");
            claimedTimeLedger.Columns.Add("Time");
            claimedTimeLedger.Columns.Add("Cost");
            claimedTimeLedger.Columns.Add("CostBalance");
            claimedTimeLedger.Columns.Add("Charge");
            claimedTimeLedger.Columns.Add("Balance");

            claimedTimeLedger.Rows.Add("18/09/2009", "Preparation", "AN", "120:00", "24000.00", "24000.00", "5964.00", "5964.00");

            return claimedTimeLedger;
        }

        /// <summary>
        /// Gets unbilled time ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWriteOffTimeLedger()
        {
            DataTable writeOffTimeLedger = new DataTable();
            writeOffTimeLedger.Columns.Add("Date");
            writeOffTimeLedger.Columns.Add("TimeType");
            writeOffTimeLedger.Columns.Add("Earner");
            writeOffTimeLedger.Columns.Add("Time");
            writeOffTimeLedger.Columns.Add("Cost");
            writeOffTimeLedger.Columns.Add("CostBalance");
            writeOffTimeLedger.Columns.Add("Charge");
            writeOffTimeLedger.Columns.Add("Balance");

            writeOffTimeLedger.Rows.Add("18/09/2009", "Preparation", "AN", "120:00", "24000.00", "24000.00", "5964.00", "5964.00");

            return writeOffTimeLedger;
        }

        /// <summary>
        /// Gets unbilled time ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTimeWriteOffTimeLedger()
        {
            DataTable writeOffTimeLedger = new DataTable();
            writeOffTimeLedger.Columns.Add("Date");
            writeOffTimeLedger.Columns.Add("Reference");
            writeOffTimeLedger.Columns.Add("Description");
            writeOffTimeLedger.Columns.Add("Time");
            writeOffTimeLedger.Columns.Add("Charge");
            writeOffTimeLedger.Columns.Add("Cost");

            writeOffTimeLedger.Rows.Add("18/09/2009", "Write Off", "Time Write Off", "1:00", "34.50", "0.00");

            return writeOffTimeLedger;
        }

        /// <summary>
        /// Gets unbilled time ledger
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTimeWriteOffReversalTimeLedger()
        {
            DataTable writeOffReversalTimeLedger = new DataTable();
            writeOffReversalTimeLedger.Columns.Add("Date");
            writeOffReversalTimeLedger.Columns.Add("Reference");
            writeOffReversalTimeLedger.Columns.Add("Description");
            writeOffReversalTimeLedger.Columns.Add("Time");
            writeOffReversalTimeLedger.Columns.Add("Charge");
            writeOffReversalTimeLedger.Columns.Add("Cost");

            writeOffReversalTimeLedger.Rows.Add("18/09/2009", "Write Off", "Time Write Off", "-1:00", "-34.50", "0.00");

            return writeOffReversalTimeLedger;
        }

        /// <summary>
        /// Gets all the bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllBills()
        {
            DataTable billsLedger = new DataTable();
            billsLedger.Columns.Add("Date");
            billsLedger.Columns.Add("Reference");
            billsLedger.Columns.Add("Type");
            billsLedger.Columns.Add("Debit");
            billsLedger.Columns.Add("Credit");
            billsLedger.Columns.Add("Paid");
            billsLedger.Columns.Add("Outstanding");
            billsLedger.Columns.Add("Balance");

            billsLedger.Rows.Add("18/09/2009", "Reference", "Bill", "12121.00", "0.00", "121.00", "12000.00", "12000.00");

            return billsLedger;
        }

        /// <summary>
        /// Gets write offs bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWriteOffBills()
        {
            DataTable writeOffBillsLedger = new DataTable();
            writeOffBillsLedger.Columns.Add("Date");
            writeOffBillsLedger.Columns.Add("Reference");
            writeOffBillsLedger.Columns.Add("Type");
            writeOffBillsLedger.Columns.Add("Debit");
            writeOffBillsLedger.Columns.Add("Credit");
            writeOffBillsLedger.Columns.Add("Paid");
            writeOffBillsLedger.Columns.Add("Outstanding");
            writeOffBillsLedger.Columns.Add("Balance");

            writeOffBillsLedger.Rows.Add("18/09/2009", "Reference Test", "Write Off", "0.00", "121.00", "-121.00", "0.00", "0.00");

            return writeOffBillsLedger;
        }

        /// <summary>
        /// Gets uncleared bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUnclearedBills()
        {
            DataTable unclearedBillsLedger = new DataTable();
            unclearedBillsLedger.Columns.Add("Date");
            unclearedBillsLedger.Columns.Add("Reference");
            unclearedBillsLedger.Columns.Add("Type");
            unclearedBillsLedger.Columns.Add("Amount");
            unclearedBillsLedger.Columns.Add("Uncleared");
            unclearedBillsLedger.Columns.Add("VAT");
            unclearedBillsLedger.Columns.Add("Disbursement");
            unclearedBillsLedger.Columns.Add("Costs");
            unclearedBillsLedger.Columns.Add("Balance");

            unclearedBillsLedger.Rows.Add("18/09/2009", "Reference", "Bill", "12121.00", "12000.00", "0.00", "12000.00", "0.00", "12000.00");

            return unclearedBillsLedger;
        }

        /// <summary>
        /// Gets unbilled paid non-vatable disbursements
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUnbilledPaidNonVatableDisbursements()
        {
            DataTable unbilledPaidNonVatableDisbursements = new DataTable();
            unbilledPaidNonVatableDisbursements.Columns.Add("Date");
            unbilledPaidNonVatableDisbursements.Columns.Add("Reference");
            unbilledPaidNonVatableDisbursements.Columns.Add("Description");
            unbilledPaidNonVatableDisbursements.Columns.Add("Amount");
            unbilledPaidNonVatableDisbursements.Columns.Add("Billed");
            unbilledPaidNonVatableDisbursements.Columns.Add("UnBilled");

            unbilledPaidNonVatableDisbursements.Rows.Add("18/09/2009", "Reference", "Testing Disbs", "330.00", "330.00", "0.00");
            unbilledPaidNonVatableDisbursements.Rows.Add("18/09/2009", "Reference", "New Disbs", "221.00", "221.00", "0.00");
            unbilledPaidNonVatableDisbursements.Rows.Add("18/09/2009", "Reference", "Court Fee", "-220.00", "0.00", "-220.00");

            return unbilledPaidNonVatableDisbursements;
        }

        /// <summary>
        /// Gets unbilled paid vatable disbursements
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUnbilledPaidVatableDisbursements()
        {
            DataTable unbilledPaidVatableDisbursements = new DataTable();
            unbilledPaidVatableDisbursements.Columns.Add("Date");
            unbilledPaidVatableDisbursements.Columns.Add("Reference");
            unbilledPaidVatableDisbursements.Columns.Add("Description");
            unbilledPaidVatableDisbursements.Columns.Add("Amount");
            unbilledPaidVatableDisbursements.Columns.Add("Billed");
            unbilledPaidVatableDisbursements.Columns.Add("UnBilled");

            unbilledPaidVatableDisbursements.Rows.Add("18/09/2009", "Ref Disbs", "Water Authority Search", "748.94", "748.94", "0.00");

            return unbilledPaidVatableDisbursements;
        }


        /// <summary>
        /// Gets anticipated non-vatable disbursements
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAnticipatedNonVatableDisbursements()
        {
            DataTable anticipatedNonVatableDisbursements = new DataTable();
            anticipatedNonVatableDisbursements.Columns.Add("Date");
            anticipatedNonVatableDisbursements.Columns.Add("Reference");
            anticipatedNonVatableDisbursements.Columns.Add("Description");
            anticipatedNonVatableDisbursements.Columns.Add("AntiDisbLedgerPayee");
            anticipatedNonVatableDisbursements.Columns.Add("Amount");
            anticipatedNonVatableDisbursements.Columns.Add("Billed");
            anticipatedNonVatableDisbursements.Columns.Add("UnBilled");

            anticipatedNonVatableDisbursements.Rows.Add("18/09/2009", "Ref Disbs", "Testing disbs", "", "220.00", "220.00", "0.00");

            return anticipatedNonVatableDisbursements;
        }

        /// <summary>
        /// Gets anticipated vatable disbursements
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAnticipatedVatableDisbursements()
        {
            DataTable anticipatedNonVatableDisbursements = new DataTable();
            anticipatedNonVatableDisbursements.Columns.Add("Date");
            anticipatedNonVatableDisbursements.Columns.Add("Reference");
            anticipatedNonVatableDisbursements.Columns.Add("Description");
            anticipatedNonVatableDisbursements.Columns.Add("AntiDisbLedgerPayee");
            anticipatedNonVatableDisbursements.Columns.Add("Amount");
            anticipatedNonVatableDisbursements.Columns.Add("Billed");
            anticipatedNonVatableDisbursements.Columns.Add("UnBilled");

            anticipatedNonVatableDisbursements.Rows.Add("18/09/2009", "Ref Disbs", "Anticipated Vatable disbs", "", "20.00", "20.00", "0.00");

            return anticipatedNonVatableDisbursements;
        }

        /// <summary>
        /// Gets unbilled time for draft bill
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUnbilledTime()
        {
            DataTable unbilledTime = new DataTable();
            unbilledTime.Columns.Add("Date");
            unbilledTime.Columns.Add("FeeEarner");
            unbilledTime.Columns.Add("Type");
            unbilledTime.Columns.Add("Description");
            unbilledTime.Columns.Add("Charge");
            unbilledTime.Columns.Add("Cost");
            unbilledTime.Columns.Add("Time");

            unbilledTime.Rows.Add("18/09/2009", "TW", "UnBilled", "Letter Out", "65.00", "100.00", "1:00");
            unbilledTime.Rows.Add("18/09/2009", "KA", "UnBilled", "Telephone Out", "34.50", "0.00", "1:00");
            unbilledTime.Rows.Add("18/09/2009", "CR", "UnBilled", "Telephone Out", "41.00", "200.00", "1:00");
            unbilledTime.Rows.Add("18/09/2009", "DB", "UnBilled", "Attending Client", "74.00", "100.00", "1:00");
            unbilledTime.Rows.Add("18/09/2009", "DB", "UnBilled", "Letter In", "23.50", "100.00", "1:00");
            unbilledTime.Rows.Add("18/09/2009", "TW", "UnBilled", "Preparation", "297.50", "2000.00", "10:00");

            return unbilledTime;
        }

        /// <summary>
        /// Gets draft bills
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDraftBills()
        {
            DataTable draftBills = new DataTable();
            draftBills.Columns.Add("Date");
            draftBills.Columns.Add("User");
            draftBills.Columns.Add("MatterReference");
            draftBills.Columns.Add("Description");

            draftBills.Rows.Add("18/09/2009", "MSH", "100001-0012", "Description Testing");

            return draftBills;
        }

        /// <summary>
        /// Gets general contacts
        /// </summary>
        /// <returns></returns>
        public static DataTable GetGeneralContacts()
        {
            DataTable generalContacts = new DataTable();
            generalContacts.Columns.Add("Name");
            generalContacts.Columns.Add("Role");
            generalContacts.Columns.Add("Description");

            generalContacts.Rows.Add("Mrs Liam Atkinson", "General", "Testing Description");

            return generalContacts;
        }

        /// <summary>
        /// Gets witnes contacts
        /// </summary>
        /// <returns></returns>
        public static DataTable GetWitnessContacts()
        {
            DataTable witnessContacts = new DataTable();
            witnessContacts.Columns.Add("Name");
            witnessContacts.Columns.Add("Role");
            witnessContacts.Columns.Add("Description");

            witnessContacts.Rows.Add("Mrs Liam Atkinson", "Witness", "Testing Description");
            witnessContacts.Rows.Add("Mr David Smith", "Witness", "Testing Description");

            return witnessContacts;
        }

        /// <summary>
        /// Get association roles for matter
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRoles()
        {
            DataTable roles = new DataTable();
            roles.Columns.Add("AssociationRole");
            roles.Rows.Add("Agent");
            roles.Rows.Add("Child");
            roles.Rows.Add("Client");
            roles.Rows.Add("Co-defendant");
            roles.Rows.Add("Counsel");
            roles.Rows.Add("Court");
            roles.Rows.Add("Detention Centre");
            roles.Rows.Add("Expert");
            roles.Rows.Add("General");
            roles.Rows.Add("LSC Office");
            roles.Rows.Add("Otherside");
            roles.Rows.Add("Otherside Solicitor");
            roles.Rows.Add("Partner");
            roles.Rows.Add("Power of Attorney");
            roles.Rows.Add("PrimaryXClient");
            roles.Rows.Add("Shareholder");

            return roles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAdditionalAssociationInfoForChild()
        {
            DataTable additionalAssociationInfoForChild = new DataTable();
            additionalAssociationInfoForChild.Columns.Add("FieldDescription");
            additionalAssociationInfoForChild.Rows.Add("Child Category");
            additionalAssociationInfoForChild.Rows.Add("Family Relationship");

            return additionalAssociationInfoForChild;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAdditionalAssociationInfoForCoDefendant()
        {
            DataTable additionalAssociationInfoForCoDefendant = new DataTable();
            additionalAssociationInfoForCoDefendant.Columns.Add("FieldDescription");
            additionalAssociationInfoForCoDefendant.Rows.Add("Matter");
            additionalAssociationInfoForCoDefendant.Rows.Add("LA Order Granted");
            additionalAssociationInfoForCoDefendant.Rows.Add("Application Signed");

            return additionalAssociationInfoForCoDefendant;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAdditionalAssociationInfoForCounsel()
        {
            DataTable additionalAssociationInfoForCounsel = new DataTable();
            additionalAssociationInfoForCounsel.Columns.Add("FieldDescription");
            additionalAssociationInfoForCounsel.Rows.Add("Family Claim Type");
            additionalAssociationInfoForCounsel.Rows.Add("In relation to");

            return additionalAssociationInfoForCounsel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAdditionalAssociationInfoForCourt()
        {
            DataTable additionalAssociationInfoForCourt = new DataTable();
            additionalAssociationInfoForCourt.Columns.Add("FieldDescription");
            additionalAssociationInfoForCourt.Rows.Add("Court");
            additionalAssociationInfoForCourt.Rows.Add("Case Number");
            additionalAssociationInfoForCourt.Rows.Add("Language Interpreter");
            additionalAssociationInfoForCourt.Rows.Add("Special Measure For Witness?");
            additionalAssociationInfoForCourt.Rows.Add("Disabled/Special Facilities or Assistance");
            additionalAssociationInfoForCourt.Rows.Add("Family Claim Type");

            return additionalAssociationInfoForCourt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetChildCategory()
        {
            DataTable childCategory = new DataTable();
            childCategory.Columns.Add("Details");
            childCategory.Rows.Add("Of Both Parties");
            childCategory.Rows.Add("Of the Family");
            childCategory.Rows.Add("Not of the Family");

            return childCategory;
        }

        public static DataTable GetInRelationTo()
        {
            DataTable inRelationTo = new DataTable();
            inRelationTo.Columns.Add("Details");
            inRelationTo.Rows.Add("");
            inRelationTo.Rows.Add("Claimant");
            inRelationTo.Rows.Add("Defendant");
            inRelationTo.Rows.Add("Part 20 Claimant");
            inRelationTo.Rows.Add("Part 20 Defendant");

            return inRelationTo;
        }

        public static DataTable GetServiceContacts()
        {
            DataTable services = new DataTable();
            services.Columns.Add("ContactName");
            services.Columns.Add("ContactPosition");
            services.Rows.Add("Contact 1", "test 1");
            services.Rows.Add("Contact 2", "test 2");
            services.Rows.Add("Contact 3", "test 3");
            services.Rows.Add("Contact 4", "test 4");
            return services;
        }
    }
}

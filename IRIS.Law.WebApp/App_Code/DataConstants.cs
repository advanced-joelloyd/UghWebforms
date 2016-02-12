using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IRIS.Law.WebApp.App_Code
{
    public class DataConstants
    {
        public readonly static Guid DummyGuid = new Guid("9D786D21-A326-11D7-B3D0-009027D6386F");
        public readonly static DateTime BlankDate = new DateTime(1753, 01, 01);
        public readonly static string BlankTxtDate = "  /  /    ";
        //PSFixedFeeStartDate is declared in PfBillingApplication.PfBillingServices.PfBillingFunctions
        public readonly static DateTime PSFixedFeeStartDate = new DateTime(2008, 01, 14);
        public readonly static int UNRESERVABLE_UFNRANGE_START = 900;
        public readonly static int UNRESERVABLE_UFNRANGE_END = 999;
        public readonly static int REGOLATORY_MAX_MONTHS = 6;
        public readonly static string WSEndPointErrorMessage = "Server error: Connection timed out. Please contact the server administrator";

        public enum BudgetTypes
        {
            BudgetPrivateMatterStarts = 1,
            BudgetPublicMatterStarts = 2,
            BudgetHours = 3,
            BudgetFees = 4,
            BudgetPostedValue = 5,
            BudgetDraftBilledValue = 6,
            BudgetClaimedValue = 7,
            BudgetBilledValue = 8,
            BudgetWrittenOffValue = 9,
            BudgetFeesWrittenOff = 10


        }

        public enum SystemAddressTypes : int
        {
            Main = 1,
            Previous = 2,
            RegisteredOffice = 3,
            OtherTrading = 4,
            AlternateBilling = 5,
            Conveyancing = 6
        }

        public enum MemberTypes : int
        {
            Unknown = 0,
            Client = 1,
            ServiceContact = 2,
            GeneralContact = 3,
            SystemUser = 4,
            SystemGroup = 5,
            DiaryResource = 6
        }

        public enum OrganisationTypes : int
        {
            NotUsed = 1,
            CustomerFirm = 2,
            CustomerBranch = 3,
            Client = 4,
            Service = 5,
            GeneralContact = 6
        }

        public enum BankSearchTypes : int
        {
            All = 0,
            Client = 1,
            Office = 2,
            Deposit = 3,
            ClientOffice = 4,
            ClientDeposit = 5,
            OfficeDeposit = 6
        }

        public enum RelatesToType
        {
            Project,
            OrgClient,
            MemberClient,
            Member,
            Org,
            AMLProject,
            AMLOrgClient,
            AMLMemberClient,
            Module,
            Will,
            Deed,
            Employee,
            CriminalPFBilling,
            CivilPFBilling
        }

        public enum Application : int
        {
            PMS = 1,
            Accounts = 2,
            AML = 3,
            Conveyancing = 4,
            PublicFundingBilling = 5,
            Family = 6,
            Wills = 7,
            Deeds = 8,
            Investments = 9,
            Probate = 10,
            Crime = 11,
            Remortgaging = 12,
            PersonalInjury = 13,
            ClientCommentary = 14,
            Licensing = 15,
            ULR = 16,
            ChestManagement = 17,
            Diary = 18,
            CivilBilling = 19,
            HumanResources = 20,
            CashFlowForecast = 22,
            Marketing = 21,
            CreditControl = 23
        }

        public enum AssociationRoles
        {
            PrimaryClient = 1,
            Client = 2,
            Spouse = 6,
            Child = 7,
            Dependant = 11,
            Beneficiary = 21,
            Testator = 49,
            PrimaryExecutor = 50,
            Executor = 51,
            GuardianInEventOfDeath = 52,
            Mortagee = 56
        }

        public enum WillTypes
        {
            CurrentWill = 1,
            PreviousWill = 2
        }

        public enum WillDocTypes
        {
            Will = 1,
            Codicil = 2
        }

        public enum ConveyancingTypes : int
        {
            Sale = 1,
            Purchase = 2,
            SaleAndPurchase = 3,
            Transfer = 4,
            ReMortgage = 5,
            Other = 6
        }

        public enum TenureTypes
        {
            Freehold = 1,
            Leasehold = 2,
            FreeHoldUnitWithinCommonhold = 3,
            NotApplicable = 4
        }

        public enum AddressTypes
        {
            Main = 1,
            Previous = 2,
            RegisteredOffice = 3,
            OtherTrading = 4,
            AlternateBilling = 5,
            Conveyancing = 6
        }

        public enum PropertyTypes
        {
            Sale = 1,
            Purchase = 2,
            Transfer = 3,
            ReMortgage = 4,
            Other = 5
        }

        public enum ReportCategories
        {
            PMS = 1,
            Accounts = 2,
            PurchaseLedger = 3,
            CounselLedger = 4,
            MoneyLaundering = 5,
            Diary = 6
        }

        public enum ReportSubCategories
        {
            Financial = 1,
            Time = 2,
            PublicFunded = 3,
            Auditing = 4,
            Disbursements = 5,
            BillsPayments = 6,
            ManagementReports = 7,
            Miscellaneous = 8,
            Interest = 9,
            Deeds = 10
        }

        public enum DocOwner
        {
            Project = 1,
            Member = 2,
            Org = 3,
            Will = 4,
            Convey = 5
        }

        public enum DiaryControlTypes : int
        {
            SaleCompletionDate = 1,
            SaleExchangeDate = 2,
            PurchaseCompletionDate = 3,
            PurchaseExchangeDate = 4,
            CostReview = 5,
            CaseReview = 6,
            PILimitationDate = 7,
            PIP36ClaimantOffer = 8,
            PIP36DefendantOffer = 9,
            PoliceStationBailBack = 10,
            CourtAttendance = 11,
            SaleAnticipatedCompletionDate = 12,
            PurchaseAnticipatedCompletionDate = 13,
            FamilyCourtHearingAttendance = 14,
            FamilyClaimDetailTask = 15
        }

        public enum MatterTypes : int
        {
            Standard = 1,
            Spocc = 2,
            FileReview = 3,
            PoliceStationStandby = 4,
            Span = 5,
            Firm = 6,
            CivilMonthlyPayment = 7,
            CriminalMonthlyPayment = 8
        }

        public enum CriminalPFBClaimTypes : int
        {
            NotSet = 1,
            Unknown = 2,
            CoreCostsCDS11 = 3,
            StandardFeeCDS11 = 4,
            NSFCDS7 = 5,
            Crown5144A = 6,
            Crown5144 = 7
        }

        public enum CriminalPFBClaimFixedFeeTypes : int
        {
            FileReviewClaim = -1,
            None = 0,
            PoliceStationPhoneCalls = 1,
            PoliceStationFixedFee = 2,
            PoliceStationExceptional = 11
        }

        public enum SystemFeaturesDataTypes : int
        {
            Boolean = 1,
            String = 2,
            Text = 3,
            Decimal = 4,
            DateTime = 5
        }

        public enum UserType : int
        {
            Staff = 1,
            Client = 2,
            ThirdParty = 3
        }

        public DataConstants()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}

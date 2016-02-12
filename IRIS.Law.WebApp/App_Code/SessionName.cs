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
	public static class SessionName
	{

        /// <summary>
        /// User Id
        /// </summary>
        public const string UserId = "UserId";

        /// <summary>
        /// Process activity ID
        /// </summary>
        public const string ProcessActivityId = "ProcessActivityId";

        /// <summary>
        /// Task activity ID
        /// </summary>
        public const string TaskActivityId = "TaskActivityId";

        /// <summary>
        /// Stores the directroy for a saved .css file which is being previewed
        /// </summary>
        public const string UsedSavedPreview = "UsedSavedPreview";
        /// <summary>
        /// Stores the directroy for the new logo
        /// </summary>
        public const string LogonName = "LogonName";

		/// <summary>
		/// Stores the client's login id in the session. Used in the mobile pages where we are not storing
		/// the logon return value but only the login id
		/// </summary>
		public const string LogonId = "LogonId";

		/// <summary>
		/// Used in edit client page to load the client information
		/// </summary>
		public const string MemberId = "MemberId";

		/// <summary>
		/// Used in edit client page to load the client information
		/// </summary>
		public const string OrganisationId = "OrganisationId";

		/// <summary>
		/// Stores the client's contact details in the session after loading the client
		/// </summary>
		public const string ContactDetails = "ContactDetails";

		/// <summary>
		/// Stores the client's addresses in the session after loading the client
		/// </summary>
        public const string ClientAddresses = "ClientAddresses";

        /// <summary>
        /// Stores the id of the client mailing address if just loaded address is mailing
        /// </summary>
        public const string ClientMailingAddressId = "ClientMailingAddressId";

        /// <summary>
        /// Stores the id of the client billing address if just loaded address is mailing
        /// </summary>
        public const string ClientBillingAddressId = "ClientBillingAddressId";

        /// <summary>
        /// Used in edit matter page to load the matter information
        /// </summary>
        public const string ProjectId = "ProjectId";

        /// <summary>
        /// Stores the login users default settings in the session.
        /// </summary>
        public const string LogonSettings = "LogonSettings";

        /// <summary>
        /// Used this information to keep user in context.
        /// </summary>
        public const string ClientName = "ClientName";

        /// <summary>
        /// Used this information to keep user in context.
        /// </summary>
        public const string ClientRef = "ClientRef";

        /// <summary>
        /// Used this information to keep user in context.
        /// </summary>
        public const string MatterDesc = "MatterDesc";

        /// <summary>
        /// Set this session while editing a time entry.
        /// </summary>
        public const string TimeId = "TimeId";

        /// <summary>
        /// Set this session while moving from Time Entry screen to Additional Time Detail screen.
        /// </summary>
        public const string TimeDetails = "TimeDetails";

        /// <summary>
        /// Set this session while editing a time entry. Reqd to get the matter details
        /// </summary>
        public const string MatterReference = "MatterReference";

        /// <summary>
        /// Used in edit document page to load the document information
        /// </summary>
        public const string DocumentId = "DocumentId";

        /// <summary>
        /// Used in time recording mobile to set default earner for the search
        /// </summary>
        public const string DefaultFeeEarner = "DefaultFeeEarner";

        /// <summary>
        /// Set this session while editing an appointment.
        /// </summary>
        public const string AppointmentId = "AppointmentId";

        /// <summary>
        /// Set this session while editing an appointment.
        /// </summary>
        public const string OfficeChequeRequestId = "OfficeChequeRequestId";

        /// <summary>
        /// Set this session while editing an task.
        /// </summary>
        public const string TaskId = "TaskId";

        /// <summary>
        /// Set this session to task name.
        /// </summary>
        public const string TaskName = "TaskName";

        /// <summary>
        /// Used in populating cheque request details by cheque request id(Client/Office) for printing.
        /// </summary>
        public const string ChequeRequestId = "ChequeRequestId";

        /// <summary>
        /// Used in populating total time by project id in time ledger
        /// </summary>
        public const string TotalTime = "TotalTime";

        /// <summary>
        /// Used in populating total cost balance by project id in time ledger
        /// </summary>
        public const string TotalCost = "TotalCost";

        /// <summary>
        /// Used in populating total charge balance by project id in time ledger
        /// </summary>
        public const string TotalCharge = "TotalCharge";

        /// <summary>
        /// Used for rendering stylesheet
        /// </summary>
        public const string StyleSheet = "StyleSheet";

        /// <summary>
        /// Used to populate values from ViewStyle.aspx page to ChangeStyle.aspx page
        /// </summary>
        public const string StyleSheetToBeEdited = "StyleSheetToBeEdited";

        /// <summary>
        /// Used for security purpose. If above session 'StyleSheet' value is lost, this session value will be helpful
        /// </summary>
        public const string UserStyleSheet = "UserStyleSheet";
        
        /// <summary>
        /// Stores the Project Id for Matter
        /// </summary>
        public const string TaskProjectId = "TaskProjectId";

        /// <summary>
        /// Used for control accessibility
        /// </summary>
        #region Control Accessibility properties

        public const string AddressVisible = "AddressVisible";
        public const string AddressReadOnly = "AddressReadOnly";

        public const string ContactVisible = "ContactVisible";
        public const string ContactReadOnly = "ContactReadOnly";

        public const string GeneralVisible = "GeneralVisible";
        public const string GeneralReadOnly = "GeneralReadOnly";

        public const string IndividualVisible = "IndividualVisible";
        public const string IndividualReadOnly = "IndividualReadOnly";

        public const string OrgVisible = "OrgVisible";
        public const string OrgReadOnly = "OrgReadOnly";

        public const string ClientMatterVisible = "ClientMatterVisible";
        public const string ClientMatterReadOnly = "ClientMatterReadOnly";

        public const string MatterDetailsVisible = "MatterDetailsVisible";
        public const string MatterDetailsReadOnly = "MatterDetailsReadOnly";

        public const string AddInfoVisible = "AddInfoVisible";
        public const string AddInfoReadOnly = "AddInfoReadOnly";

        public const string MatterContactVisible = "MatterContactVisible";
        public const string MatterContactReadOnly = "MatterContactReadOnly";

        public const string MatterPublicFundingVisible = "MatterPublicFundingVisible";
        public const string MatterPublicFundingReadOnly = "MatterPublicFundingReadOnly";

        public const string UploadDocVisible = "UploadDocVisible";
        public const string UploadDocReadOnly = "UploadDocReadOnly";

        public const string ReUploadDocVisible = "ReUploadDocVisible";
        public const string ReUploadDocReadOnly = "ReUploadDocReadOnly";

        public const string EditDocVisible = "EditDocVisible";
        public const string EditDocReadOnly = "EditDocReadOnly";

        public const string SearchClientVisible = "SearchClientVisible";

        public const string UserDocColIsPublic = "UserDocIsPublic";

        #endregion

        /// <summary>
        /// Stores the login users default control accessiblity settings in the session.
        /// </summary>
        public const string ControlSettings = "ControlSettings";

        /// <summary>
        /// Stores the task list user search
        /// </summary>
        public const string TaskListUser = "TaskListUser";

    }
}

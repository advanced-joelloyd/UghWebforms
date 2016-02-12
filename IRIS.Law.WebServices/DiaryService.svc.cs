using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Autofac;
using IRIS.Law.Diary.Services;
using IRIS.Law.Diary.Services.Diary;
using IRIS.Law.PmsBusiness;
using IRIS.Law.WebServiceInterfaces.Diary;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.PmsCommonData;
using IRIS.Law.DiaryServices.Diary.BookingEntry;
using IRIS.Law.DiaryServices.Diary.Data;
using System.Data;
using System.Collections.ObjectModel;
using IRIS.Law.DiaryData.DataSets;
using IRIS.Law.Services.Pms.Matter;
using IRIS.Law.Services.Pms.Service;
using IRIS.Law.DiaryServices.Diary.DiaryParameter;
using IRIS.Law.DiaryData.StateData;
using IRIS.Law.DiaryServices.Diary.Occurrence;
using IRIS.Law.Services.Pms.DiaryProvider;
using IRIS.Law.PmsCommonData.Diary;
using IRIS.Law.Services.Pms.Occurence;
using IRIS.Law.PmsCommonData.CommonServices;
using IRIS.Law.PmsCommonServices.CommonServices;
using IRIS.Law.Services.Pms.User;
using System.Diagnostics;
using IRISLegal.UserDefined.Domain.Enquiry;

namespace IRIS.Law.WebServices
{
    // NOTE: If you change the class name "DiaryService" here, you must also update the reference to "DiaryService" in Web.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [ErrorLoggerBehaviour]
    public class DiaryService : IDiaryService
    {
        #region GetDiaryMembers
        public DiaryMemberSearchReturnValue GetDiaryMembers(Guid logonId, CollectionRequest collectionRequest)
        {
            DiaryMemberSearchReturnValue returnValue = new DiaryMemberSearchReturnValue();

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
                            // TODO later this should just be restricted to staff only
                            //Clients and Third Party users can create public tasks
                            //throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of banks
                    DataListCreator<DiaryMemberSearchItem> dataListCreator = new DataListCreator<DiaryMemberSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DiaryViewMembersDts dsMembers = SrvDiaryBookingMemberLookup.GetAllDiaryMembers();

                        DataView dvMembers = new DataView(dsMembers.Tables[0]);
                        //dvMembers.RowFilter = " (DocDescription <> 'PLACEHOLDER_FOLDER')";
                        dvMembers.Sort = "MemberDisplayName asc";

                        DataSet dsMembers1 = new DataSet();
                        dsMembers1.Tables.Add(dvMembers.ToTable());
                        e.DataSet = dsMembers1;
                    };

                    // Create the data list
                    returnValue.DiaryMembers = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "DiaryMemberSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        string.Empty,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("MemberID", "MemberID"),
                            new ImportMapping("MemberDisplayName", "MemberDisplayName"),
                            new ImportMapping("MemberCode", "MemberCode")
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

        #region GetDiaryMemberDetails
        private DiaryMemberSearchItem GetDiaryMemberDetails(string memberId)
        {
            DiaryMemberSearchItem diaryMember = new DiaryMemberSearchItem();
            try
            {
                diaryMember.IsMember = true;

                DiaryViewMembersDts dsMembers = SrvDiaryBookingMemberLookup.GetAllDiaryMembers();
                DiaryViewMembersDts.DiaryViewMembersDataTable membersDs = dsMembers.DiaryViewMembers;

                DiaryViewMembersDts.DiaryViewMembersRow[] groupRows = (DiaryViewMembersDts.DiaryViewMembersRow[])membersDs.Select(string.Format("MemberTypeID = {0}", (int)DataConstants.MemberTypes.SystemGroup));
                DiaryViewMembersDts.DiaryViewMembersRow[] individualRows = (DiaryViewMembersDts.DiaryViewMembersRow[])membersDs.Select(string.Format("MemberTypeID <> {0}", (int)DataConstants.MemberTypes.SystemGroup));

                //Add the individuals
                foreach (DiaryViewMembersDts.DiaryViewMembersRow member in individualRows)
                {
                    if (member.MemberID == memberId)
                    {
                        diaryMember.MemberCode = member.MemberCode;
                        diaryMember.MemberDisplayName = member.MemberDisplayName;
                        diaryMember.MemberID = member.MemberID;

                        if (member.MemberTypeID == 6)
                        {
                            diaryMember.IsResource = true;
                        }
                        else
                        {
                            diaryMember.IsMember = true;
                        }
                    }
                }

                foreach (DiaryViewMembersDts.DiaryViewMembersRow group in groupRows)
                {
                    if (group.MemberID == memberId)
                    {
                        diaryMember.MemberCode = group.MemberCode;
                        diaryMember.MemberDisplayName = group.MemberDisplayName;
                        diaryMember.MemberID = group.MemberID;

                        diaryMember.IsGroup = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return diaryMember;
        }
        #endregion

        #region ResolveGroupMembers
        private string[] ResolveGroupMembers(int groupId)
        {
            return SrvBookingEntryCommon.ResolveGroupMembers(groupId);
        }
        #endregion

        #region IsOccurrenceViewableBy
        private bool IsOccurrenceViewableBy(int occurrenceId)
        {
            try
            {
                return SrvBookingEntryCommon.IsOccurrenceViewableBy(occurrenceId, UserInformation.Instance.UserMemberId);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region AppointmentSearch
        public AppointmentSearchReturnValue AppointmentSearch(Guid logonId, CollectionRequest collectionRequest,
                                AppointmentSearchCriteria criteria)
        {
            AppointmentSearchReturnValue returnValue = new AppointmentSearchReturnValue();

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
                    DataListCreator<Appointment> dataListCreator = new DataListCreator<Appointment>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DiaryMemberSearchItem diaryMembers = this.GetDiaryMemberDetails(criteria.MemberID);
                        string UsersChosenMemberID = criteria.MemberID;
                        DateTime usersDiaryDate = DateTime.Today;
                        int CurrentDiaryDays = 1;
                        bool ShowCancelled = false;
                        bool ShowTasks = false;
                        bool ExcludeEmptyDiaryDaysForUsers = true;
                        bool ExcludeEmptyDiaryDaysForGroups = true;
                        string memberIds;
                        string[] usersChosenMemberIDs = new string[] { };
                        int usersDiaryDaysView = 7;

                        DiaryViewDts dvDiary = new DiaryViewDts();

                        if (criteria.Date == DataConstants.BlankDate)
                        {
                            usersDiaryDate = DateTime.Today;
                        }
                        else
                        {
                            usersDiaryDate = Convert.ToDateTime(criteria.Date);
                        }

                        if (!diaryMembers.IsGroup)
                        {
                            UsersChosenMemberID = diaryMembers.MemberID;
                            dvDiary = SrvBookingEntryLookup.GetDiaryBookingsForMember(UsersChosenMemberID, usersDiaryDate,
                                CurrentDiaryDays, ShowCancelled, ShowTasks, usersDiaryDaysView, ExcludeEmptyDiaryDaysForUsers);
                        }
                        else
                        {
                            usersChosenMemberIDs = this.ResolveGroupMembers(int.Parse(diaryMembers.MemberID));
                            memberIds = string.Join(",", usersChosenMemberIDs);

                            dvDiary = SrvBookingEntryLookup.GetDiaryBookingsForMembers(memberIds, usersDiaryDate, CurrentDiaryDays, ShowCancelled, ShowTasks, usersDiaryDaysView, ExcludeEmptyDiaryDaysForGroups);
                        }

                        // Add New Column Editable
                        dvDiary.Tables[0].Columns.Add("isEdit");

                        for (int i = 0; i < dvDiary.Tables[0].Rows.Count; i++)
                        {
                            dvDiary.Tables[0].Rows[i]["isEdit"] = this.IsOccurrenceViewableBy(Convert.ToInt32(dvDiary.Tables[0].Rows[i]["OccurrenceID"]));
                        }

                        e.DataSet = dvDiary;

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("DiaryEntries");
                            e.DataSet.Tables.Add(dt);
                        }
                    };

                    returnValue.Appointments = dataListCreator.Create(logonId,
                                                "AppointmentSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                                            criteria.ToString(),
                                            collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                                            new ImportMapping[] {
                                                new ImportMapping("Id", "OccurrenceID"),
                                                new ImportMapping("StartDate", "Date"),
                                                new ImportMapping("Subject", "OccSpecificText"),
                                                new ImportMapping("StartTime", "StartTime"),
                                                new ImportMapping("EndTime", "EndTime"),
                                                new ImportMapping("Notes", "OccurrenceNoteText"),
                                                new ImportMapping("VenueText", "VenueDescription"),
                                                new ImportMapping("Duration", "EstTime"),
                                                new ImportMapping("IsEditable", "isEdit")
                                                // More fields are required here but the service layer query may not be 
                                                // providing them.  This is enough for a test.
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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region GetAppointmentDetails
        public AppointmentReturnValue GetAppointmentDetails(Guid logonId, Int32 appointmentId)
        {
            AppointmentReturnValue returnValue = new AppointmentReturnValue();

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

                    string attendeesNames = string.Empty;
                    string attendeesIds = string.Empty;


                    Appointment appointmentDetails = new Appointment();
                    SrvBookingEntry bookingEntryData = new SrvBookingEntry();
                    bookingEntryData.OccurrenceData.Id = appointmentId;

                    bookingEntryData.Load(bookingEntryData.OccurrenceData.Id);

                    this.GetBookingMembers(bookingEntryData.OccurrenceData.Id, ref attendeesNames, ref attendeesIds);

                    appointmentDetails.AttendeesName = attendeesNames;
                    appointmentDetails.Attendees = attendeesIds;

                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        appointmentDetails.Id = bookingEntryData.OccurrenceData.Id;

                        appointmentDetails.VenueId = bookingEntryData.OccurrenceData.VenueId;

                        DsServiceSearch dsServiceSearch = SrvServiceLookup.ServiceSearch(bookingEntryData.OccurrenceData.VenueId);
                        if (dsServiceSearch.uvw_ServicesSearch.Count > 0)
                        {
                            appointmentDetails.VenueText = dsServiceSearch.uvw_ServicesSearch[0].OrgName + " - " + dsServiceSearch.uvw_ServicesSearch[0].IndustryName;
                        }
                        else
                        {
                            appointmentDetails.VenueText = "Not Set";
                        }

                        DsMattersForBooking.MatterForBookingDataTable dsMattersForBooking = bookingEntryData.DsBookingsMatters.MatterForBooking;
                        if (dsMattersForBooking.Rows.Count > 0)
                        {
                            appointmentDetails.ProjectId = new Guid(dsMattersForBooking[0].ProjectId.ToString());
                        }
                        else
                        {
                            appointmentDetails.ProjectId = DataConstants.DummyGuid;
                        }

                        appointmentDetails.StartTime = bookingEntryData.OccurrenceData.StartTime;
                        appointmentDetails.EndTime = bookingEntryData.OccurrenceData.EndTime;
                        appointmentDetails.StartDate = bookingEntryData.OccurrenceData.StartDate;
                        appointmentDetails.Subject = bookingEntryData.OccurrenceData.Text;
                        appointmentDetails.Notes = bookingEntryData.OccurrenceData.OccurrenceNotes;

                        if (bookingEntryData.OccurrenceData.ReminderDate != Convert.ToDateTime("01/01/1753"))
                        {
                            appointmentDetails.IsReminderSet = true;
                            if (bookingEntryData.OccurrenceData.ReminderMinutesBefore == 0)
                            {
                                appointmentDetails.ReminderType = "On";
                            }
                            else
                            {
                                appointmentDetails.ReminderType = "Before";
                            }

                            appointmentDetails.ReminderBeforeTime = bookingEntryData.OccurrenceData.ReminderMinutesBefore.ToString();
                            appointmentDetails.ReminderDate = bookingEntryData.OccurrenceData.ReminderDate;
                            appointmentDetails.ReminderTime = bookingEntryData.OccurrenceData.ReminderTime;
                        }
                        else
                        {
                            appointmentDetails.IsReminderSet = false;
                            appointmentDetails.ReminderType = "On";
                            appointmentDetails.ReminderBeforeTime = "15 minutes";
                            appointmentDetails.ReminderTime = "";
                        }
                    }
                    returnValue.Appointment = appointmentDetails;
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

        #region GetMemberTaskDetails
        public TaskReturnValue GetMemberTaskDetails(Guid logonId, Int32 taskId)
        {
            TaskReturnValue returnValue = new TaskReturnValue();

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

                    string attendeesNames = string.Empty;
                    string attendeesIds = string.Empty;


                    Task taskDetails = new Task();
                    SrvBookingEntry bookingEntryData = new SrvBookingEntry();
                    bookingEntryData.OccurrenceData.Id = taskId;

                    bookingEntryData.Load(bookingEntryData.OccurrenceData.Id);

                    this.GetBookingMembers(bookingEntryData.OccurrenceData.Id, ref attendeesNames, ref attendeesIds);

                    taskDetails.AttendeesName = attendeesNames;
                    taskDetails.Attendees = attendeesIds;

                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        taskDetails.Id = bookingEntryData.OccurrenceData.Id;

                        DsMattersForBooking.MatterForBookingDataTable dsMattersForBooking = bookingEntryData.DsBookingsMatters.MatterForBooking;
                        if (dsMattersForBooking.Rows.Count > 0)
                        {
                            taskDetails.ProjectId = new Guid(dsMattersForBooking[0].ProjectId.ToString());
                        }
                        else
                        {
                            taskDetails.ProjectId = DataConstants.DummyGuid;
                        }

                        //taskDetails.StartTime = bookingEntryData.OccurrenceData.StartTime;
                        //taskDetails.EndTime = bookingEntryData.OccurrenceData.EndTime;
                        taskDetails.TypeId = bookingEntryData.OccurrenceData.BookingTypeId;
                        taskDetails.DueDate = bookingEntryData.OccurrenceData.DueDate;
                        taskDetails.Subject = bookingEntryData.OccurrenceData.Text;
                        taskDetails.Notes = bookingEntryData.OccurrenceData.OccurrenceNotes;

                        if (bookingEntryData.OccurrenceData.Progress == 100)
                        {
                            taskDetails.IsCompleted = true;
                        }
                        else
                        {
                            taskDetails.IsCompleted = false;
                        }
                        taskDetails.IsPublic = bookingEntryData.OccurrenceData.IsOccurrencePublic;

                        taskDetails.IsPrivate = bookingEntryData.OccurrenceData.IsPrivateBooking;

                        if (bookingEntryData.OccurrenceData.ReminderDate != Convert.ToDateTime("01/01/1753"))
                        {
                            taskDetails.IsReminderSet = true;
                            taskDetails.ReminderDate = bookingEntryData.OccurrenceData.ReminderDate;
                            taskDetails.ReminderTime = bookingEntryData.OccurrenceData.ReminderTime;
                            taskDetails.ReminderBeforeTime = bookingEntryData.OccurrenceData.ReminderMinutesBefore.ToString();
                        }
                        else
                        {
                            taskDetails.IsReminderSet = false;
                            taskDetails.ReminderTime = string.Empty;
                            taskDetails.ReminderBeforeTime = "0";
                        }
                        taskDetails.ReminderType = "On";

                    }
                    returnValue.Task = taskDetails;
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

        #region GetMatterTaskDetails
        // If the user is a client or a third party and the task is not public then it should throw an access denied exception
        public TaskReturnValue GetMatterTaskDetails(Guid logonId, Guid projectId, Int32 taskId)
        {
            TaskReturnValue returnValue = new TaskReturnValue();

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
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(projectId))
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    string attendeesNames = string.Empty;
                    string attendeesIds = string.Empty;


                    Task taskDetails = new Task();
                    SrvBookingEntry bookingEntryData = new SrvBookingEntry();
                    bookingEntryData.OccurrenceData.Id = taskId;

                    bookingEntryData.Load(bookingEntryData.OccurrenceData.Id);

                    switch (UserInformation.Instance.UserType)
                    {
                        case DataConstants.UserType.Staff:
                            // Can do everything
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            // Check Task is public
                            if (!bookingEntryData.OccurrenceData.IsOccurrencePublic)
                                throw new Exception("Access denied");
                            break;
                        default:
                            throw new Exception("Unknown UserType");
                    }

                    this.GetBookingMembers(bookingEntryData.OccurrenceData.Id, ref attendeesNames, ref attendeesIds);

                    taskDetails.AttendeesName = attendeesNames;
                    taskDetails.Attendees = attendeesIds;

                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        taskDetails.Id = bookingEntryData.OccurrenceData.Id;

                        DsMattersForBooking.MatterForBookingDataTable dsMattersForBooking = bookingEntryData.DsBookingsMatters.MatterForBooking;
                        if (dsMattersForBooking.Rows.Count > 0)
                        {
                            DataRow[] drTemp = dsMattersForBooking.Select("ProjectId = '" + projectId.ToString() + "' ");
                            if (drTemp.Length > 0)
                            {
                                taskDetails.ProjectId = new Guid(drTemp[0]["ProjectId"].ToString());
                            }
                        }
                        else
                        {
                            taskDetails.ProjectId = DataConstants.DummyGuid;
                            // Document not belongs to any matter
                            throw new Exception("Access denied. Task not belongs to any matter.");
                        }
                        if (new Guid(taskDetails.ProjectId.ToString()) != projectId)
                        {
                            throw new Exception("Access denied.");
                        }

                        //taskDetails.StartTime = bookingEntryData.OccurrenceData.StartTime;
                        //taskDetails.EndTime = bookingEntryData.OccurrenceData.EndTime;
                        taskDetails.TypeId = bookingEntryData.OccurrenceData.BookingTypeId;
                        taskDetails.DueDate = bookingEntryData.OccurrenceData.DueDate;
                        taskDetails.Subject = bookingEntryData.OccurrenceData.Text;
                        taskDetails.Notes = bookingEntryData.OccurrenceData.OccurrenceNotes;

                        if (bookingEntryData.OccurrenceData.Progress == 100)
                        {
                            taskDetails.IsCompleted = true;
                        }
                        else
                        {
                            taskDetails.IsCompleted = false;
                        }
                        taskDetails.IsPublic = bookingEntryData.OccurrenceData.IsOccurrencePublic;

                        taskDetails.IsPrivate = bookingEntryData.OccurrenceData.IsPrivateBooking;

                        if (bookingEntryData.OccurrenceData.ReminderDate != Convert.ToDateTime("01/01/1753"))
                        {
                            taskDetails.IsReminderSet = true;
                            taskDetails.ReminderDate = bookingEntryData.OccurrenceData.ReminderDate;
                            taskDetails.ReminderTime = bookingEntryData.OccurrenceData.ReminderTime;
                            taskDetails.ReminderBeforeTime = bookingEntryData.OccurrenceData.ReminderMinutesBefore.ToString();
                        }
                        else
                        {
                            taskDetails.IsReminderSet = false;
                            taskDetails.ReminderTime = string.Empty;
                            taskDetails.ReminderBeforeTime = "0";
                        }
                        taskDetails.ReminderType = "On";

                    }
                    returnValue.Task = taskDetails;
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

        #region GetBookingMembers

        /// <summary>
        /// Gets the booking members.
        /// </summary>
        /// <returns></returns>
        private void GetBookingMembers(int occurenceId, ref string attendeesNames, ref string attendeesIds)
        {
            try
            {
                DiaryViewMembersDts bookingMembersForOccurrence = new DiaryViewMembersDts();
                bookingMembersForOccurrence = SrvDiaryBookingMemberLookup.GetBookingMembers(occurenceId);

                for (int i = 0; i < bookingMembersForOccurrence.DiaryViewMembers.Rows.Count; i++)
                {
                    attendeesNames = attendeesNames + bookingMembersForOccurrence.DiaryViewMembers[i].MemberDisplayName.TrimEnd(null) + "; ";
                    attendeesIds = attendeesIds + bookingMembersForOccurrence.DiaryViewMembers[i].MemberID.TrimEnd(null) + "; ";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Save Appointment
        public AppointmentReturnValue SaveAppointment(Guid logonId, Appointment appointmentDetails)
        {
            AppointmentReturnValue returnValue = new AppointmentReturnValue();

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
                            // Can add/edit subject to permissions
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    SrvBookingEntry bookingEntryData = new SrvBookingEntry();
                    bookingEntryData.OccurrenceData.Id = appointmentDetails.Id;

                    bookingEntryData.Load(bookingEntryData.OccurrenceData.Id);

                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        bookingEntryData.ActionEditMode = ActionEditMode.Amend;
                    }

                    // Ensure we have permission
                    if (bookingEntryData.ActionEditMode == ActionEditMode.Create)
                    {
                        // Create new bookings
                        if (!UserSecuritySettings.GetUserSecuitySettings(100))
                            throw new Exception("You do not have sufficient permissions to carry out this request");
                    }
                    else
                    {
                        // Edit existing bookings
                        if (!UserSecuritySettings.GetUserSecuitySettings(102))
                            throw new Exception("You do not have sufficient permissions to carry out this request");
                    }

                    DiaryViewMembersDts diaryViewMembers = this.GetDiaryViewMembersDts(appointmentDetails.Attendees);
                    this.SetBookingMembersData(diaryViewMembers, ref bookingEntryData);

                    if (bookingEntryData.ActionEditMode == ActionEditMode.Create)
                    {
                        // Add the first attendee as the organiser
                        bookingEntryData.OrganiserId = new Guid(diaryViewMembers.DiaryViewMembers[0].MemberID);                        
                    }

                    #region Add Matter to Appointment
                    // Adding Matter to the Appointment
                    Guid oldProjectId = DataConstants.DummyGuid;
                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        DsMattersForBooking.MatterForBookingDataTable dsMattersForBooking = bookingEntryData.DsBookingsMatters.MatterForBooking;
                        if (bookingEntryData.DsBookingsMatters.MatterForBooking.Rows.Count > 0)
                        {
                            if (appointmentDetails.ProjectId != null)
                            {
                                oldProjectId = new Guid(dsMattersForBooking[0].ProjectId.ToString());

                                // If New Matter is same as that of the Old Matter, then don't add any new matters
                                if (oldProjectId != appointmentDetails.ProjectId)
                                {
                                    // Check if old matter been replaced by new matter is already present in the list or not
                                    // If New Matter is been replaced, then remove Old Matter
                                    // If New Matter been added is already present in the list of existing matters, then throw error
                                    if (!(bookingEntryData.DsBookingsMatters.MatterForBooking.Select(string.Format("ProjectId = '{0}'", appointmentDetails.ProjectId.ToString())).Length > 0))
                                    {
                                        // Remove Old Matter
                                        if (bookingEntryData.DsBookingsMatters.MatterForBooking.FindByProjectId(oldProjectId.ToString()) != null)
                                        {
                                            bookingEntryData.DsBookingsMatters.MatterForBooking.RemoveMatterForBookingRow(bookingEntryData.DsBookingsMatters.MatterForBooking.FindByProjectId(oldProjectId.ToString()));
                                            Collection<SrvBookingMatter> bookingMatterTableIndividual = bookingEntryData.BookingMatterTable;
                                            this.AddMatterData(ref bookingMatterTableIndividual, bookingEntryData.DsBookingsMatters);
                                        }
                                    }
                                    else if (bookingEntryData.DsBookingsMatters.MatterForBooking.Select(string.Format("ProjectId = '{0}'", appointmentDetails.ProjectId.ToString())).Length > 1)
                                    {
                                        throw new Exception("This matter is already present for this appointment");
                                    }
                                }
                            }
                        }
                    }
                    if (appointmentDetails.ProjectId != null)
                    {
                        if (appointmentDetails.ProjectId != DataConstants.DummyGuid)
                        {
                            this.AddMatter(appointmentDetails.ProjectId, ref bookingEntryData);
                        }
                    }
                    #endregion

                    if (appointmentDetails.Id == 0)
                    {
                        bookingEntryData.OccurrenceData.BookingText = appointmentDetails.Subject;
                        bookingEntryData.OccurrenceData.BookingColour = (System.Drawing.Color.Black);
                    }
                    else
                    {
                        bookingEntryData.OccurrenceData.StartDate = appointmentDetails.StartDate;
                    }

                    #region Date & Time Calculations
                    TimeSpan tsStartTime;
                    TimeSpan tsEndTime;
                    try
                    {
                        tsStartTime = TimeSpan.Parse(appointmentDetails.StartTime);
                        tsEndTime = TimeSpan.Parse(appointmentDetails.EndTime);
                        DateTime dtStartDate = appointmentDetails.StartDate;
                        //If the end time is smaller than the start time
                        //then the appointment must have spanned midnight 
                        //and therefore the enddate should be amended to
                        //include the extra day.
                        if (tsEndTime < tsStartTime)
                        {
                            bookingEntryData.OccurrenceData.EndDate = dtStartDate.AddDays(1);
                        }
                        else
                        {
                            bookingEntryData.OccurrenceData.EndDate = dtStartDate;
                        }
                    }
                    catch
                    {
                        bookingEntryData.OccurrenceData.EndDate = DateTime.Now;
                    }
                    bookingEntryData.OccurrenceData.DueDate = DateTime.Now;
                    bookingEntryData.OccurrenceData.StartTime = appointmentDetails.StartTime;
                    bookingEntryData.OccurrenceData.EndTime = appointmentDetails.EndTime;

                    if (string.IsNullOrEmpty(appointmentDetails.StartTime))
                    {
                        appointmentDetails.StartTime = ":";
                    }
                    if (string.IsNullOrEmpty(appointmentDetails.EndTime))
                    {
                        appointmentDetails.EndTime = ":";
                    }

                    appointmentDetails.StartTime = SrvBookingEntryCommon.EnsureTimeFormat(appointmentDetails.StartTime);
                    appointmentDetails.EndTime = SrvBookingEntryCommon.EnsureTimeFormat(appointmentDetails.EndTime);
                    if (!SrvBookingEntryCommon.IsValidTime(Convert.ToString(appointmentDetails.StartDate), appointmentDetails.StartTime, true))
                    {
                        throw new Exception("Invalid Start Time");
                    }
                    if (!SrvBookingEntryCommon.IsValidTime(Convert.ToString(appointmentDetails.StartDate), appointmentDetails.EndTime, true))
                    {
                        throw new Exception("Invalid End Time");
                    }

                    bookingEntryData.OccurrenceData.EstimatedTime = SrvBookingEntryCommon.CalculateDuration(appointmentDetails.StartTime, "", appointmentDetails.EndTime);

                    #endregion

                    this.LoadBookingPriority(ref bookingEntryData);

                    bookingEntryData.OccurrenceData.IsOccurrenceProvisional = false;
                    bookingEntryData.OccurrenceData.IsOccurrenceIgnoredBook = false;
                    bookingEntryData.OccurrenceData.OccurrenceNotes = appointmentDetails.Notes;
                    bookingEntryData.OccurrenceData.Text = appointmentDetails.Subject;
                    bookingEntryData.OccurrenceData.IsPrivateBooking = false;


                    // Set Status Default to "Not Set"
                    this.LoadBookingStatus(ref bookingEntryData, "Not Set");

                    // Set Type Default to "Appointment"
                    this.LoadBookingTypes(ref bookingEntryData);

                    bookingEntryData.BookingDates = Convert.ToString(appointmentDetails.StartDate);
                    bookingEntryData.RecurrenceData.IsSaturdayInclude = false;
                    bookingEntryData.RecurrenceData.IsSundayInclude = false;

                    #region Set Reminder details
                    if (appointmentDetails.IsReminderSet)
                    {
                        if (appointmentDetails.ReminderType == "Before")
                        {
                            bookingEntryData.OccurrenceData.ReminderDate = appointmentDetails.ReminderDate;
                            bookingEntryData.OccurrenceData.ReminderTime = appointmentDetails.ReminderTime;
                            bookingEntryData.OccurrenceData.ReminderMinutesBefore = Convert.ToInt16(appointmentDetails.ReminderBeforeTime);
                        }
                        else
                        {
                            bookingEntryData.OccurrenceData.ReminderDate = appointmentDetails.ReminderDate;
                            bookingEntryData.OccurrenceData.ReminderTime = appointmentDetails.ReminderTime;
                            bookingEntryData.OccurrenceData.ReminderMinutesBefore = 0;
                        }
                    }
                    else
                    {
                        bookingEntryData.OccurrenceData.ReminderDate = Convert.ToDateTime("01/01/1753");
                        bookingEntryData.OccurrenceData.ReminderTime = "09:00";
                        bookingEntryData.OccurrenceData.ReminderMinutesBefore = 0;
                    }

                    bookingEntryData.OccurrenceData.ReminderFutureAction = 0;
                    #endregion

                    bookingEntryData.OccurrenceData.TaskStartDate = Convert.ToDateTime("01/01/1753");
                    bookingEntryData.ReminderRepeatForOccurrences = false;
                    bookingEntryData.OccurrenceData.VenueId = appointmentDetails.VenueId;


                    string errorMessage = string.Empty;
                    bool returnValueRows = false;

                    returnValueRows = bookingEntryData.Save(out errorMessage);
                    if (returnValueRows == false)
                    {
                        if (errorMessage != string.Empty)
                        {
                            returnValue.Success = false;
                            returnValue.Message = errorMessage;
                        }
                    }
                    else
                    {
                        appointmentDetails.Id = bookingEntryData.OccurrenceData.Id;
                        returnValue.Appointment = appointmentDetails;
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

        #region Save Task
        public TaskReturnValue SaveTask(Guid logonId, Task taskDetails)
        {
            TaskReturnValue returnValue = new TaskReturnValue();

            try
            {
                // Get the logged on user from the current logons acnd add their 
                // ApplicationSettings the list of concurrent sessions.
                Host.LoadLoggedOnUser(logonId);

                try
                {
                    Functions.RestrictRekoopIntegrationUser(UserInformation.Instance.DbUid);
                    if (taskDetails.ProjectId == Guid.Empty || taskDetails.ProjectId == DataConstants.DummyGuid)
                    {
                        // Non matter task
                        switch (UserInformation.Instance.UserType)
                        {
                            case DataConstants.UserType.Staff:
                                // Can add/edit subject to permissions
                                break;
                            case DataConstants.UserType.Client:
                            case DataConstants.UserType.ThirdParty:
                                if (!UserSecuritySettings.GetUserSecuitySettings(273))
                                {
                                    throw new Exception("Access denied");
                                }
                                break;

                            default:
                                throw new Exception("Access denied");
                        }
                    }
                    else
                        // Matter task
                        switch (UserInformation.Instance.UserType)
                        {
                            case DataConstants.UserType.Staff:
                                // Can add/edit subject to permissions
                                break;
                            case DataConstants.UserType.Client:
                            case DataConstants.UserType.ThirdParty:
                                if (!SrvMatterCommon.WebAllowedToAccessMatter(taskDetails.ProjectId)
                                    || !UserSecuritySettings.GetUserSecuitySettings(273))
                                {
                                    throw new Exception("Access denied");
                                }
                                break;
                            default:
                                throw new Exception("Access denied");
                        }

                    SrvBookingEntry bookingEntryData = new SrvBookingEntry();
                    bookingEntryData.OccurrenceData.Id = taskDetails.Id;

                    var brClients = new BrClients();
                    if (!string.IsNullOrEmpty(taskDetails.ClientId) && !taskDetails.IsContactTask)
                    {
                        var client = brClients.GetClientFromID(new Guid(taskDetails.ClientId));             
                        ClientMatterBooking.AddClientMatterToBooking(bookingEntryData, client.Clients[0].memId, client.Clients[0].orgId, DataConstants.DummyGuid);
                    }

                    bookingEntryData.Load(bookingEntryData.OccurrenceData.Id);

                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        // Editing a task
                        bookingEntryData.ActionEditMode = ActionEditMode.Amend;

                        //check that task is public and throw an access denied error if not
                        switch (UserInformation.Instance.UserType)
                        {
                            case DataConstants.UserType.Staff:
                                // Can add/edit subject to permissions
                                break;
                            case DataConstants.UserType.Client:
                            case DataConstants.UserType.ThirdParty:
                                //Must check that task is public and throw an access denied error if not
                                if (!taskDetails.IsPublic)
                                    throw new Exception("Access denied");
                                break;
                            default:
                                throw new Exception("Unknown UserType");
                        }

                    }
                    else
                    {
                        // Adding a new task
                        bookingEntryData.OccurrenceData.BookingText = taskDetails.Subject;
                        bookingEntryData.OccurrenceData.BookingColour = (System.Drawing.Color.Black);

                        // If the starttime/Est time is null, it will not be listed in the search tasks.
                        // To fix this issue, added the below code
                        // Ref:2757
                        if (string.IsNullOrEmpty(bookingEntryData.OccurrenceData.StartTime))
                        {
                            bookingEntryData.OccurrenceData.StartTime = ":";
                        }
                        if (string.IsNullOrEmpty(bookingEntryData.OccurrenceData.EstimatedTime))
                        {
                            bookingEntryData.OccurrenceData.EstimatedTime = ":";
                        }
                    }

                    if (UserInformation.Instance.UserType == DataConstants.UserType.Staff)
                    {
                        // Ensure staff have permission
                        if (bookingEntryData.ActionEditMode == ActionEditMode.Create)
                        {
                            // Create new bookings
                            if (!UserSecuritySettings.GetUserSecuitySettings(100))
                                throw new Exception("You do not have sufficient permissions to carry out this request");
                        }
                        else
                        {
                            // Edit existing bookings
                            if (!UserSecuritySettings.GetUserSecuitySettings(102))
                                throw new Exception("You do not have sufficient permissions to carry out this request");
                        }
                    }

                    DiaryViewMembersDts diaryViewMembers = this.GetDiaryViewMembersDts(taskDetails.Attendees);
                    this.SetBookingMembersData(diaryViewMembers, ref bookingEntryData);

                    #region Add Matter to Task
                    // Adding Matter to the Appointment
                    Guid oldProjectId = DataConstants.DummyGuid;
                    if (bookingEntryData.OccurrenceData.Id != 0)
                    {
                        DsMattersForBooking.MatterForBookingDataTable dsMattersForBooking = bookingEntryData.DsBookingsMatters.MatterForBooking;
                        if (bookingEntryData.DsBookingsMatters.MatterForBooking.Rows.Count > 0)
                        {
                            if (taskDetails.ProjectId != null)
                            {
                                oldProjectId = new Guid(dsMattersForBooking[0].ProjectId.ToString());

                                // If New Matter is same as that of the Old Matter, then don't add any new matters
                                if (oldProjectId != taskDetails.ProjectId)
                                {
                                    // Check if old matter been replaced by new matter is already present in the list or not
                                    // If New Matter is been replaced, then remove Old Matter
                                    // If New Matter been added is already present in the list of existing matters, then throw error
                                    if (!(bookingEntryData.DsBookingsMatters.MatterForBooking.Select(string.Format("ProjectId = '{0}'", taskDetails.ProjectId.ToString())).Length > 0))
                                    {
                                        // Remove Old Matter
                                        if (bookingEntryData.DsBookingsMatters.MatterForBooking.FindByProjectId(oldProjectId.ToString()) != null)
                                        {
                                            bookingEntryData.DsBookingsMatters.MatterForBooking.RemoveMatterForBookingRow(bookingEntryData.DsBookingsMatters.MatterForBooking.FindByProjectId(oldProjectId.ToString()));
                                            Collection<SrvBookingMatter> bookingMatterTableIndividual = bookingEntryData.BookingMatterTable;
                                            this.AddMatterData(ref bookingMatterTableIndividual, bookingEntryData.DsBookingsMatters);
                                        }
                                    }
                                    else if (bookingEntryData.DsBookingsMatters.MatterForBooking.Select(string.Format("ProjectId = '{0}'", taskDetails.ProjectId.ToString())).Length > 1)
                                    {
                                        throw new Exception("This matter is already present for this appointment");
                                    }
                                }
                            }
                        }
                    }

                    // string.IsNullOrEmpty(taskDetails.ClientId) will return false if we are adding the task to a client
                    if (taskDetails.ProjectId != Guid.Empty && string.IsNullOrEmpty(taskDetails.ClientId))
                    {
                        if (taskDetails.ProjectId != DataConstants.DummyGuid)
                        {
                            this.AddMatter(taskDetails.ProjectId, ref bookingEntryData);
                        }
                    }

                    #endregion

                    this.LoadBookingPriority(ref bookingEntryData);

                    bookingEntryData.OccurrenceData.OccurrenceNotes = taskDetails.Notes;
                    bookingEntryData.OccurrenceData.Text = taskDetails.Subject;
                    bookingEntryData.OccurrenceData.DueDate = taskDetails.DueDate;
                    bookingEntryData.OccurrenceData.BookingTypeId = taskDetails.TypeId;
                    bookingEntryData.OccurrenceData.IsOccurrencePublic = taskDetails.IsPublic;
                    bookingEntryData.RecurrenceData.IsTaskEntry = true;
                    bookingEntryData.OccurrenceData.TaskStartDate = Convert.ToDateTime("01/01/1753");

                    if (taskDetails.IsCompleted)
                    {
                        bookingEntryData.OccurrenceData.EndDate = DateTime.Now;
                        // Set Status Default to "Completed"
                        this.LoadBookingStatus(ref bookingEntryData, "Completed");
                        bookingEntryData.OccurrenceData.Progress = 100;
                    }
                    else
                    {
                        bookingEntryData.OccurrenceData.EndDate = DataConstants.BlankDate;
                        // Set Status Default to "Not Set"
                        this.LoadBookingStatus(ref bookingEntryData, "Not Set");
                        bookingEntryData.OccurrenceData.Progress = 0;
                    }

                    // If the status id is sent then set the status based on the id.
                    if (taskDetails.StatusId > 0)
                    {
                        this.LoadBookingStatusById(ref bookingEntryData, taskDetails.StatusId);
                    }

                    bookingEntryData.BookingDates = Convert.ToString(taskDetails.DueDate);

                    bookingEntryData.OccurrenceData.IsPrivateBooking = taskDetails.IsPrivate;

                    if (taskDetails.IsReminderSet)
                    {
                        bookingEntryData.OccurrenceData.ReminderDate = taskDetails.ReminderDate;
                        bookingEntryData.OccurrenceData.ReminderTime = taskDetails.ReminderTime;
                    }
                    else
                    {
                        bookingEntryData.OccurrenceData.ReminderDate = Convert.ToDateTime("01/01/1753");
                        bookingEntryData.OccurrenceData.ReminderTime = "09:00";
                    }

                    bookingEntryData.OccurrenceData.ReminderMinutesBefore = 0;
                    bookingEntryData.OccurrenceData.ReminderFutureAction = 0;
                    bookingEntryData.ReminderRepeatForOccurrences = false;

                    string errorMessage = string.Empty;
                    bool returnValueRows = false;

                    returnValueRows = bookingEntryData.Save(out errorMessage);
                    if (returnValueRows == false)
                    {
                        if (errorMessage != string.Empty)
                        {
                            returnValue.Success = false;
                            returnValue.Message = errorMessage;
                        }
                    }
                    else
                    {
                        taskDetails.Id = bookingEntryData.OccurrenceData.Id;
                        returnValue.Task = taskDetails;
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

        #region LoadBookingPriority

        private void LoadBookingPriority(ref SrvBookingEntry bookingEntryData)
        {
            for (int i = 0; i < bookingEntryData.DiaryParamsDataSet.OccurrencePriorities.Count; i++)
            {
                if (bookingEntryData.DiaryParamsDataSet.OccurrencePriorities[i].OPDefault)
                {
                    bookingEntryData.OccurrenceData.Priority = bookingEntryData.DiaryParamsDataSet.OccurrencePriorities[i].OPCode;
                }
            }
        }
        #endregion

        #region LoadBookingTypes

        private void LoadBookingTypes(ref SrvBookingEntry bookingEntryData)
        {
            for (int i = 0; i < bookingEntryData.DiaryParamsDataSet.BookingTypes.Count; i++)
            {
                if (bookingEntryData.DiaryParamsDataSet.BookingTypes[i].BTDescription == "Appointment")
                {
                    bookingEntryData.OccurrenceData.BookingTypeId = bookingEntryData.DiaryParamsDataSet.BookingTypes[i].BookingTypeID;
                }
            }

        }
        #endregion

        #region LoadBookingStatus

        /// <summary>
        /// Load the Booking Status by name
        /// </summary>
        /// <param name="bookingEntryData"></param>
        /// <param name="status"></param>
        private void LoadBookingStatus(ref SrvBookingEntry bookingEntryData, string status)
        {
            for (int i = 0; i < bookingEntryData.DiaryParamsDataSet.OccurrenceStatus.Count; i++)
            {
                if (bookingEntryData.DiaryParamsDataSet.OccurrenceStatus[i].OccStatus == status)
                {
                    bookingEntryData.OccurrenceData.Status = bookingEntryData.DiaryParamsDataSet.OccurrenceStatus[i].OccStatusID;
                }
            }
        }

        /// <summary>
        /// Load the Booking Status by Id
        /// </summary>
        /// <param name="bookingEntryData"></param>
        /// <param name="statusId"></param>
        private void LoadBookingStatusById(ref SrvBookingEntry bookingEntryData, int statusId)
        {
            for (int i = 0; i < bookingEntryData.DiaryParamsDataSet.OccurrenceStatus.Count; i++)
            {
                if (bookingEntryData.DiaryParamsDataSet.OccurrenceStatus[i].OccStatusID == statusId)
                {
                    bookingEntryData.OccurrenceData.Status = bookingEntryData.DiaryParamsDataSet.OccurrenceStatus[i].OccStatusID;
                }
            }
        }
        #endregion

        #region AddMatter
        private void AddMatter(Guid projectId, ref SrvBookingEntry bookingEntryData)
        {
            try
            {
                var enquiryRepo = PmsInfrastructure.Container.Instance.Resolve<IEnquiryRepository>();
                var isActiveEnquiry = enquiryRepo.IsActiveEnquiry(projectId);

                DsMattersForBooking dsBookingMatter1 = SrvMatterLookup.GetMatterForBooking(projectId);
                DsMattersForBooking dsBookingsMatters = new DsMattersForBooking();

                if (dsBookingMatter1.MatterForBooking.Rows.Count == 0 && !isActiveEnquiry)
                    throw new ApplicationException("The matter could not be found");
                
                if (isActiveEnquiry)
                {
                    dsBookingsMatters.MatterForBooking.AddMatterForBookingRow(projectId.ToString(),
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    DateTime.Now,
                    string.Empty,
                    enquiryRepo.GetPrimaryClientForEnquiry(projectId).ToString(),
                    string.Empty,
                    DataConstants.BlankDate,
                    string.Empty,
                    string.Empty,
                    string.Empty);
                }
                else
                {
                    dsBookingsMatters.MatterForBooking.AddMatterForBookingRow(dsBookingMatter1.MatterForBooking[0].ProjectId,
                    dsBookingMatter1.MatterForBooking[0].matRef,
                    dsBookingMatter1.MatterForBooking[0].matDescription,
                    dsBookingMatter1.MatterForBooking[0].matKeyDescription,
                    dsBookingMatter1.MatterForBooking[0].matOpenDate,
                    dsBookingMatter1.MatterForBooking[0].feeRef,
                    dsBookingMatter1.MatterForBooking[0].matPartnerMemId,
                    dsBookingMatter1.MatterForBooking[0].matUFN,
                    dsBookingMatter1.MatterForBooking[0].matClosedDate,
                    dsBookingMatter1.MatterForBooking[0].matBranchRef,
                    dsBookingMatter1.MatterForBooking[0].WorkTypeCode,
                    dsBookingMatter1.MatterForBooking[0].matDeptRef);
                }
                

                Collection<SrvBookingMatter> bookingMatterTableIndividual = new Collection<SrvBookingMatter>();
                AddMatterData(ref bookingMatterTableIndividual, dsBookingsMatters);
                bookingEntryData.BookingMatterTable = bookingMatterTableIndividual;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region AddMatterData

        /// <summary>
        /// Adds the matter data.
        /// </summary>
        private void AddMatterData(ref Collection<SrvBookingMatter> bookingMatterTable, DsMattersForBooking dsBookingsMatters)
        {
            bookingMatterTable.Clear();
            for (int i = 0; i < dsBookingsMatters.MatterForBooking.Rows.Count; i++)
            {
                SrvBookingMatter bookingMatter = new SrvBookingMatter();
                bookingMatter.ProjectId = dsBookingsMatters.MatterForBooking[i].ProjectId;
                bookingMatter.Reference = dsBookingsMatters.MatterForBooking[i].matRef;
                bookingMatter.Description = dsBookingsMatters.MatterForBooking[i].matDescription;
                bookingMatter.KeyDescription = dsBookingsMatters.MatterForBooking[i].matKeyDescription;
                bookingMatter.OpenDate = dsBookingsMatters.MatterForBooking[i].matOpenDate;
                bookingMatter.feeReference = dsBookingsMatters.MatterForBooking[i].feeRef;
                bookingMatter.PartnerMemberId = dsBookingsMatters.MatterForBooking[i].matPartnerMemId;
                bookingMatter.Ufn = dsBookingsMatters.MatterForBooking[i].matUFN;
                bookingMatter.ClosedDate = dsBookingsMatters.MatterForBooking[i].matClosedDate;
                bookingMatter.BranchReference = dsBookingsMatters.MatterForBooking[i].matBranchRef;
                bookingMatter.WorkTypeCode = dsBookingsMatters.MatterForBooking[i].WorkTypeCode;
                bookingMatter.DepartmentReference = dsBookingsMatters.MatterForBooking[i].matDeptRef;

                bookingMatterTable.Add(bookingMatter);
            }
        }

        #endregion

        #region SetBookingMembers

        /// <summary>
        /// Sets the Booking Members Data.
        /// </summary>
        /// <param name="diaryViewMembers">The diary view members.</param>
        private void SetBookingMembersData(DiaryViewMembersDts diaryViewMembers, ref SrvBookingEntry bookingEntryData)
        {
            SrvBookingMemberData bookingMemberData = null;
            bookingEntryData.BookingMemberData.Clear();
            for (int i = 0; i < diaryViewMembers.DiaryViewMembers.Rows.Count; i++)
            {
                bookingMemberData = new SrvBookingMemberData();
                bookingMemberData.MemberId = diaryViewMembers.DiaryViewMembers[i].MemberID;
                bookingMemberData.MemberCode = diaryViewMembers.DiaryViewMembers[i].MemberCode;
                bookingMemberData.MemberDiaryDisplay = diaryViewMembers.DiaryViewMembers[i].MemberDiaryDisplay;
                bookingMemberData.MemberDiaryPosition = diaryViewMembers.DiaryViewMembers[i].MemberDiaryPosition;
                bookingMemberData.MemberDisplayName = diaryViewMembers.DiaryViewMembers[i].MemberDisplayName;
                bookingMemberData.MemberType = diaryViewMembers.DiaryViewMembers[i].MemberType;
                bookingMemberData.MemberTypeId = diaryViewMembers.DiaryViewMembers[i].MemberTypeID;
                bookingMemberData.MemberTypePerson = diaryViewMembers.DiaryViewMembers[i].MemberTypePerson;
                bookingEntryData.BookingMemberData.Add(bookingMemberData);
            }
            bookingEntryData.IsbookingMembersChanged = true;
        }

        #endregion

        #region GetDiaryViewMembersDts
        private DiaryViewMembersDts GetDiaryViewMembersDts(string attendees)
        {
            DiaryViewMembersDts diaryViewMembersDts = null;
            DiaryViewMembersDts diaryCurrentMembers = null;

            try
            {
                if (attendees.Length > 0)
                {
                    string[] arrAttendees = attendees.Split(';');
                    if (arrAttendees.Length > 0)
                    {
                        diaryViewMembersDts = new DiaryViewMembersDts();
                        diaryViewMembersDts = SrvDiaryBookingMemberLookup.GetAllDiaryMembers();

                        diaryCurrentMembers = new DiaryViewMembersDts();

                        for (int i = 0; i < arrAttendees.Length; i++)
                        {
                            if (arrAttendees[i].Trim().Length > 0)
                            {
                                string memberId = arrAttendees[i].Trim();

                                if (diaryViewMembersDts.DiaryViewMembers.FindByMemberID(memberId) != null)
                                {
                                    diaryCurrentMembers.DiaryViewMembers.ImportRow(diaryViewMembersDts.DiaryViewMembers.FindByMemberID(memberId));
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

            return diaryCurrentMembers;
        }
        #endregion

        #region GetBookingCancelledReasons
        public CancellationCodeSearchReturnValue GetBookingCancelledReasons(Guid logonId, CollectionRequest collectionRequest)
        {
            CancellationCodeSearchReturnValue returnValue = new CancellationCodeSearchReturnValue();

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

                    // Create a data list creator for a list of banks
                    DataListCreator<CancellationCodeSearchItem> dataListCreator = new DataListCreator<CancellationCodeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DataSet dsReasons = new DataSet();
                        DataColumn dcCCode = new DataColumn("CRCode");
                        DataColumn dcCRDescription = new DataColumn("CRDescription");
                        DataTable dtReason = new DataTable();
                        dtReason.Columns.Add(dcCCode);
                        dtReason.Columns.Add(dcCRDescription);

                        DiaryParamsDts dts = SrvDiaryParameterLookup.GetDiaryParameter();
                        foreach (DiaryParamsDts.CancelledReasonsRow dr in dts.CancelledReasons.Rows)
                        {
                            if (!dr.CRDefault)
                            {
                                DataRow drNew = dtReason.NewRow();
                                drNew["CRCode"] = dr.CRCode;
                                drNew["CRDescription"] = dr.CRDescription;
                                dtReason.Rows.Add(drNew);
                            }
                        }
                        dsReasons.Tables.Add(dtReason);

                        e.DataSet = dsReasons;
                    };

                    // Create the data list
                    returnValue.CancellationCodes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "CancellationCodeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        string.Empty,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Code", "CRCode"),
                            new ImportMapping("Description", "CRDescription")
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

        #region GetBookingCancelledCategories
        public CancellationCodeSearchReturnValue GetBookingCancelledCategories(Guid logonId, CollectionRequest collectionRequest)
        {
            CancellationCodeSearchReturnValue returnValue = new CancellationCodeSearchReturnValue();

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

                    // Create a data list creator for a list of banks
                    DataListCreator<CancellationCodeSearchItem> dataListCreator = new DataListCreator<CancellationCodeSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DataSet dsCategories = new DataSet();
                        DataColumn dcCCode = new DataColumn("CCCode");
                        DataColumn dcCRDescription = new DataColumn("CCDescription");
                        DataTable dtReason = new DataTable();
                        dtReason.Columns.Add(dcCCode);
                        dtReason.Columns.Add(dcCRDescription);

                        DiaryParamsDts dts = SrvDiaryParameterLookup.GetDiaryParameter();
                        foreach (DiaryParamsDts.CancelledCategoriesRow dr in dts.CancelledCategories.Rows)
                        {
                            if (!dr.CCDefault)
                            {
                                DataRow drNew = dtReason.NewRow();
                                drNew["CCCode"] = dr.CCCode;
                                drNew["CCDescription"] = dr.CCDescription;
                                dtReason.Rows.Add(drNew);
                            }
                        }
                        dsCategories.Tables.Add(dtReason);

                        e.DataSet = dsCategories;
                    };

                    // Create the data list
                    returnValue.CancellationCodes = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "CategoriesCodeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        string.Empty,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Code", "CCCode"),
                            new ImportMapping("Description", "CCDescription")
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

        #region GetTaskTypes
        public DiaryParameterReturnValue GetTaskTypes(Guid logonId, CollectionRequest collectionRequest)
        {
            DiaryParameterReturnValue returnValue = new DiaryParameterReturnValue();

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

                    // Create a data list creator for a list of banks
                    DataListCreator<DiaryParameterSearchItem> dataListCreator = new DataListCreator<DiaryParameterSearchItem>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DataSet dsTaskType = new DataSet();
                        DataColumn dcBookingTypeID = new DataColumn("BookingTypeID", typeof(System.Int32));
                        DataColumn dcBTDescription = new DataColumn("BTDescription");
                        DataTable dtTaskType = new DataTable();
                        dtTaskType.Columns.Add(dcBookingTypeID);
                        dtTaskType.Columns.Add(dcBTDescription);

                        DiaryParamsDts dts = SrvDiaryParameterLookup.GetDiaryParameter();
                        foreach (DiaryParamsDts.TaskTypesRow dr in dts.TaskTypes.Rows)
                        {
                            //if (!dr.CRDefault)
                            {
                                DataRow drNew = dtTaskType.NewRow();
                                drNew["BookingTypeID"] = dr.BookingTypeID;
                                drNew["BTDescription"] = dr.BTDescription;
                                dtTaskType.Rows.Add(drNew);
                            }
                        }
                        dsTaskType.Tables.Add(dtTaskType);

                        e.DataSet = dsTaskType;
                    };

                    // Create the data list
                    returnValue.DiaryParamters = dataListCreator.Create(logonId,
                        // Give the query a name so it can be cached
                        "TaskTypeSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                        string.Empty,
                        collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                        new ImportMapping[] {
                            new ImportMapping("Id", "BookingTypeID"),
                            new ImportMapping("Description", "BTDescription")
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

        #region DeleteBooking
        public ReturnValue DeleteBooking(Guid logonId, DeleteData deleteData)
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
                            // Ensure we have permission
                            if (!UserSecuritySettings.GetUserSecuitySettings(103))
                                throw new Exception("You do not have sufficient permissions to carry out this request");
                            break;
                        case DataConstants.UserType.Client:
                        case DataConstants.UserType.ThirdParty:
                            throw new Exception("Access denied");
                        default:
                            throw new Exception("Access denied");
                    }

                    BookingDeleteData bookingDeleteData = new BookingDeleteData();
                    bookingDeleteData.OccurrenceID = deleteData.OccurenceId;
                    bookingDeleteData.MemberID = Convert.ToString(UserInformation.Instance.UserMemberId);
                    bookingDeleteData.IsBookingATask = deleteData.IsBookingATask;
                    bookingDeleteData.BlockBookingDeleteDataSet = new BookingDeleteDts();
                    bookingDeleteData.GroupBookingDeleteDataSet = new BookingDeleteDts();

                    try
                    {
                        string occurrenceMember;
                        DiaryMemberSearchItem diaryMembers = this.GetDiaryMemberDetails(deleteData.MemberId);

                        if (diaryMembers.IsGroup)
                        {
                            DiaryBookingsDts dsOccurrence = SrvOccurrenceLookup.GetOccurrenceById(deleteData.OccurenceId);
                            if (dsOccurrence.Occurrence.Rows.Count == 1)
                            {
                                occurrenceMember = dsOccurrence.Occurrence[0].MemberID.ToString();
                            }
                            else
                            {
                                occurrenceMember = Guid.Empty.ToString();
                            }
                        }
                        else
                        {
                            occurrenceMember = deleteData.MemberId;
                        }

                        // Firstly get the block bookings	
                        bookingDeleteData.BlockBookingDeleteDataSet = SrvBookingEntryLookup.GetDiaryBookingsForDelete(occurrenceMember, deleteData.OccurenceId, 1);

                        if (bookingDeleteData.BlockBookingDeleteDataSet != null)
                        {
                            // Now get group booking
                            bookingDeleteData.GroupBookingDeleteDataSet = SrvBookingEntryLookup.GetDiaryBookingsForDelete(occurrenceMember, deleteData.OccurenceId, 2);
                        }

                        if (bookingDeleteData.GroupBookingDeleteDataSet != null)
                        {
                            BookingDeleteDts tempDts = SrvBookingEntryLookup.GetBookingMembersForDelete(deleteData.OccurenceId);
                            foreach (BookingDeleteDts.BookingMembersDeleteRow dr in tempDts.BookingMembersDelete.Rows)
                            {
                                bookingDeleteData.GroupBookingDeleteDataSet.BookingMembersDelete.AddBookingMembersDeleteRow(dr.MemberID, dr.MemberDisplayName);
                            }
                        }

                        bookingDeleteData.IsBlockBooking = (bookingDeleteData.BlockBookingDeleteDataSet.BookingDelete.Rows.Count > 1);
                        bookingDeleteData.IsGroupBooking = (bookingDeleteData.GroupBookingDeleteDataSet.BookingMembersDelete.Rows.Count > 1);

                        if (bookingDeleteData.IsBlockBooking)
                        {
                            for (int i = 0; i < bookingDeleteData.GroupBookingDeleteDataSet.BookingDelete.Rows.Count; i++)
                            {
                                bookingDeleteData.GroupBookingDeleteDataSet.BookingDelete.Rows[i]["DeleteFlag"] = 1;
                            }
                        }
                        else if (bookingDeleteData.IsGroupBooking)
                        {
                            for (int i = 0; i < bookingDeleteData.GroupBookingDeleteDataSet.BookingDelete.Rows.Count; i++)
                            {
                                bookingDeleteData.GroupBookingDeleteDataSet.BookingDelete.Rows[i]["DeleteFlag"] = 1;
                            }
                        }

                        // Delete Occurrences
                        SrvBookingEntry srvBooking = new SrvBookingEntry();
                        string occurrenceIDs = string.Empty;
                        foreach (DataRow dr in bookingDeleteData.GroupBookingDeleteDataSet.BookingDelete.Rows)
                        {
                            if ((bool)dr["DeleteFlag"])
                            {
                                occurrenceIDs += (dr["OccurrenceID"].ToString() + ",");
                            }
                        }

                        if (!string.IsNullOrEmpty(occurrenceIDs))
                        {
                            occurrenceIDs = occurrenceIDs.Substring(0, occurrenceIDs.Length - 1);
                            string[] strOccurerenceIds = occurrenceIDs.Split(',');
                            int[] occurerenceIds = new int[strOccurerenceIds.Length];

                            for (int i = 0; i < strOccurerenceIds.Length; i++)
                            {
                                occurerenceIds[i] = Convert.ToInt32(strOccurerenceIds[i]);
                            }

                            if (!srvBooking.Delete(occurerenceIds, deleteData.CategoryCode, deleteData.ReasonCode, deleteData.CancellationText))
                            {
                                throw new Exception("Some of the supplied occurrences are already cancelled!");
                            }
                        }
                        else
                        {
                            throw new Exception("Please provide valid Occurrence Ids.");
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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

        #region MemberTaskSearch
        public TaskSearchReturnValue MemberTaskSearch(Guid logonId, CollectionRequest collectionRequest,
                                TaskSearchCriteria criteria)
        {
            TaskSearchReturnValue returnValue = new TaskSearchReturnValue();

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
                    DataListCreator<Task> dataListCreator = new DataListCreator<Task>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DiaryMemberSearchItem diaryMembers = this.GetDiaryMemberDetails(criteria.MemberID);
                        string UsersChosenMemberID = criteria.MemberID;
                        DateTime usersDiaryDate = DateTime.Today;
                        bool ShowCancelled = false;
                        string memberIds;
                        string[] usersChosenMemberIDs = new string[] { };
                        DateTime taskFromDate = usersDiaryDate;
                        DateTime taskToDate = usersDiaryDate;

                        TimeSpan timeSpan = taskToDate - taskFromDate;
                        int taskNoOfDays = (timeSpan.Days + 1);

                        int taskBookingID = 0;
                        int statusID = 0;
                        string occurrencePriority = string.Empty;
                        int dateFilter = 0;
                        int incItems = 1;
                        int progress = 0;

                        DiaryViewDts dvDiary = new DiaryViewDts();

                        if (criteria.StartDate != DataConstants.BlankDate)
                        {
                            taskFromDate = Convert.ToDateTime(criteria.StartDate);
                        }

                        if (!diaryMembers.IsGroup)
                        {
                            UsersChosenMemberID = diaryMembers.MemberID;
                            dvDiary = SrvBookingEntryLookup.GetAllTasksForMember(UsersChosenMemberID, taskFromDate,
                                ShowCancelled, taskNoOfDays, taskBookingID, statusID, occurrencePriority, dateFilter, incItems, progress);
                        }
                        else
                        {
                            usersChosenMemberIDs = this.ResolveGroupMembers(int.Parse(diaryMembers.MemberID));
                            memberIds = string.Join(",", usersChosenMemberIDs);

                            dvDiary = SrvBookingEntryLookup.GetAllTasksForMember(memberIds, taskFromDate,
                                ShowCancelled, taskNoOfDays, taskBookingID, statusID, occurrencePriority, dateFilter, incItems, progress);
                        }

                        // Add New Column Editable
                        dvDiary.DiaryEntries.Columns.Add("isEdit");
                        // Add New Column Matters     
                        dvDiary.DiaryEntries.Columns.Add("OccurrenceMatter");
                        // Add New Column IsLimitation
                        dvDiary.DiaryEntries.Columns.Add("IsLimitationTask", typeof(Boolean));
                        // Add New Column NewRecordedDueDate for filtering
                        dvDiary.DiaryEntries.Columns.Add("NewRecordedDueDate", typeof(DateTime));

                        System.Collections.ArrayList listIds = new System.Collections.ArrayList();
                        foreach (DiaryViewDts.DiaryEntriesRow row in dvDiary.DiaryEntries.Rows)
                        {
                            row["isEdit"] = this.IsOccurrenceViewableBy(row.OccurrenceID);
                            row["IsLimitationTask"] = this.IsALimitationTask(Convert.ToInt32(row.BookingTypeID));

                            if (!listIds.Contains(row.BookingID))
                            {
                                listIds.Add(row.BookingID);
                            }

                            if (row["RecordedDueDate"].ToString() != "")
                            {
                                row["NewRecordedDueDate"] = Convert.ToDateTime(row["RecordedDueDate"]);
                            }
                        }

                        StringBuilder ids = new StringBuilder();
                        foreach (int bookingId in listIds)
                        {
                            ids.Append(bookingId.ToString());
                            ids.Append(",");
                        }

                        DiaryViewMattersDts matterDts = new DiaryViewMattersDts();
                        if (ids.Length > 0)
                        {
                            ids.Length--;

                            matterDts = SrvBookingEntryLookup.GetMattersForBookings(ids.ToString());
                        }

                        foreach (DiaryViewDts.DiaryEntriesRow row in dvDiary.DiaryEntries.Rows)
                        {
                            DiaryViewMattersDts.BookingMatterRow[] matterRows;
                            matterRows = (DiaryViewMattersDts.BookingMatterRow[])matterDts.BookingMatter.Select(string.Format("BookingID = {0}", row.BookingID));
                            StringBuilder sb = new StringBuilder();
                            foreach (DiaryViewMattersDts.BookingMatterRow matterRow in matterRows)
                            {
                                sb.AppendFormat("{0}$({1}-{2})  {3}~", matterRow.ProjectID, matterRow.MatterRef.Substring(0, 6), matterRow.MatterRef.Substring(6, 4), matterRow.matDescription);
                            }
                            if (sb.Length > 0)
                            {
                                string matters = sb.ToString().Substring(0, sb.Length - 1);
                                row["OccurrenceMatter"] = matters;
                            }
                        }

                        DataView dvDiaryView = new DataView(dvDiary.Tables[0]);
                        string filter = string.Empty;

                        switch (criteria.Status)
                        {
                            case "Outstanding":
                                filter += " (OccStatusDesc <> 'Completed') and ";
                                break;
                            case "Completed":
                                filter += " (OccStatusDesc = 'Completed') and ";
                                break;
                        }

                        bool isStartDate = false;

                        if (criteria.StartDate != DataConstants.BlankDate)
                        {
                            filter += " ((NewRecordedDueDate >= #" + Convert.ToDateTime(criteria.StartDate).ToString("yyyy-MM-dd") + "#) ";
                            isStartDate = true;
                        }
                        if (criteria.ToDate != DataConstants.BlankDate)
                        {
                            if (isStartDate)
                            {
                                filter += " and ";
                            }

                            filter += "(NewRecordedDueDate <= #" + Convert.ToDateTime(criteria.ToDate).ToString("yyyy-MM-dd") + "#)";

                            if (isStartDate)
                            {
                                filter += ") ";
                            }

                            filter += " and ";

                        }
                        else
                        {
                            filter += ") and ";
                        }

                        if (!string.IsNullOrEmpty(filter))
                        {
                            filter = filter.Trim().Substring(0, filter.Trim().Length - 3);
                        }

                        dvDiaryView.RowFilter = filter;

                        DataSet dsFiltered = new DataSet();
                        dsFiltered.Tables.Add(dvDiaryView.ToTable());

                        e.DataSet = dsFiltered;

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("DiaryEntries");
                            e.DataSet.Tables.Add(dt);
                        }
                    };

                    returnValue.Tasks = dataListCreator.Create(logonId,
                                                "TaskSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                                            criteria.ToString(),
                                            collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                                            new ImportMapping[] {
                                                new ImportMapping("Id", "OccurrenceID"),
                                                new ImportMapping("RecordedDueDate", "RecordedDueDate"),
                                                new ImportMapping("Subject", "OccSpecificText"),
                                                new ImportMapping("StatusDesc", "OccStatusDesc"),
                                                new ImportMapping("Progress", "OccProgress"),
                                                new ImportMapping("Notes", "OccurrenceNoteText"),
                                                new ImportMapping("Matters", "OccurrenceMatter"),
                                                new ImportMapping("IsEditable", "isEdit"),
                                                new ImportMapping("IsLimitationTask", "IsLimitationTask")
                                                // More fields are required here but the service layer query may not be 
                                                // providing them.  This is enough for a test.
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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region MatterTaskSearch
        public TaskSearchReturnValue MatterTaskSearch(Guid logonId, CollectionRequest collectionRequest, TaskSearchCriteria criteria)
        {
            TaskSearchReturnValue returnValue = new TaskSearchReturnValue();

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
                            if (!SrvMatterCommon.WebAllowedToAccessMatter(criteria.ProjectID))
                                throw new Exception("Access denied");

                            break;
                        default:
                            throw new Exception("Access denied");
                    }

                    // Create a data list creator for a list of matters
                    DataListCreator<Task> dataListCreator = new DataListCreator<Task>();

                    // Declare an inline event (annonymous delegate) to read the 
                    // dataset if it is required
                    dataListCreator.ReadDataSet += delegate(object Sender, ReadDataSetEventArgs e)
                    {
                        DsOccurencesForProject dvDiary = new DsOccurencesForProject();

                        dvDiary = SrvOccurenceLookup.GetOccurencesForProject(criteria.ProjectID);

                        // Add New Column Editable
                        dvDiary.uvw_OccurencesForProject.Columns.Add("isEdit");
                        dvDiary.uvw_OccurencesForProject.Columns.Add("AttendeeName");
                        dvDiary.uvw_OccurencesForProject.Columns.Add("IsLimitationTask", typeof(Boolean));
                        // Add New Column NewOccDueDate for filtering
                        dvDiary.uvw_OccurencesForProject.Columns.Add("NewOccDueDate", typeof(DateTime));

                        foreach (IRIS.Law.PmsCommonData.Diary.DsOccurencesForProject.uvw_OccurencesForProjectRow row in dvDiary.uvw_OccurencesForProject)
                        {
                            row["isEdit"] = this.IsOccurrenceViewableBy(Convert.ToInt32(row.OccurrenceID));
                            row["IsLimitationTask"] = this.IsALimitationTask(Convert.ToInt32(row.BookingTypeID));
                            row["AttendeeName"] = CommonFunctions.MakeFullName(row.PersonTitle, row.PersonName, row.PersonSurname);

                            if (row["OccDueDate"].ToString() != "")
                            {
                                row["NewOccDueDate"] = Convert.ToDateTime(row["OccDueDate"]);
                            }
                        }

                        DataView dvDiaryView = new DataView(dvDiary.Tables[0]);
                        string filter = string.Empty;

                        switch (criteria.Status)
                        {
                            case "Outstanding":
                                filter += " (OccStatus <> 'Completed') and ";
                                break;
                            case "Completed":
                                filter += " (OccStatus = 'Completed') and ";
                                break;
                        }

                        // Filter only BookingType which is of Task
                        // BookingTypeID: 4 - Key Date, 5 - Limitation Date, 12 - Standard Task
                        CollectionRequest collection = new CollectionRequest();
                        DiaryParameterReturnValue taskTypes = this.GetTaskTypes(logonId, collection);
                        string bookingTypeId = string.Empty;
                        for (int i = 0; i < taskTypes.DiaryParamters.Rows.Count; i++)
                        {
                            bookingTypeId += taskTypes.DiaryParamters.Rows[i].Id.ToString() + ",";
                        }
                        if (bookingTypeId.Length > 0)
                        {
                            bookingTypeId = bookingTypeId.Trim().Substring(0, bookingTypeId.Trim().Length - 1);
                        }
                        if (!string.IsNullOrEmpty(bookingTypeId))
                        {
                            filter += " (BookingTypeID in (" + bookingTypeId + ")) and ";
                        }
                        //filter += " (BookingTypeID in (4, 5, 12)) and ";

                        //if (criteria.StartDate != DataConstants.BlankDate)
                        //{
                        //    filter += " (OccDueDate = '" + criteria.StartDate.ToString() + "') and ";
                        //}

                        bool isStartDate = false;

                        if (criteria.StartDate != DataConstants.BlankDate)
                        {
                            filter += " ((NewOccDueDate >= #" + Convert.ToDateTime(criteria.StartDate).ToString("yyyy-MM-dd") + "#) ";
                            isStartDate = true;
                        }
                        if (criteria.ToDate != DataConstants.BlankDate)
                        {
                            if (isStartDate)
                            {
                                filter += " and ";
                            }

                            filter += "(NewOccDueDate <= #" + Convert.ToDateTime(criteria.ToDate).ToString("yyyy-MM-dd") + "#)";

                            if (isStartDate)
                            {
                                filter += ") ";
                            }

                            filter += " and ";

                        }
                        else
                        {
                            filter += ") and ";
                        }

                        if (!string.IsNullOrEmpty(criteria.MemberID))
                        {
                            DiaryMemberSearchItem diaryMembers = this.GetDiaryMemberDetails(criteria.MemberID);
                            string memberIds = string.Empty;
                            if (!diaryMembers.IsGroup)
                            {
                                memberIds += "Convert('" + criteria.MemberID + "', 'System.Guid'), ";
                            }
                            else
                            {
                                string[] usersChosenMemberIDs = this.ResolveGroupMembers(int.Parse(diaryMembers.MemberID));
                                if (usersChosenMemberIDs.Length > 0)
                                {
                                    for (int i = 0; i < usersChosenMemberIDs.Length; i++)
                                    {
                                        memberIds += "Convert('" + usersChosenMemberIDs[i] + "', 'System.Guid'),";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(memberIds))
                            {
                                memberIds = memberIds.Trim().Substring(0, memberIds.Trim().Length - 1);
                                filter += " (MemberID in (" + memberIds + ")) and ";
                            }
                        }
                        //If user is client or third party then can only return public tasks.
                        switch (UserInformation.Instance.UserType)
                        {
                            case DataConstants.UserType.Staff:
                                // Can do everything
                                break;
                            case DataConstants.UserType.Client:
                            case DataConstants.UserType.ThirdParty:
                                // Return only Public tasks
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    filter += " (OccIsPublic = 1) and ";
                                }
                                break;
                            default:
                                throw new Exception("Unknown UserType");
                        }


                        if (!string.IsNullOrEmpty(filter))
                        {
                            filter = filter.Trim().Substring(0, filter.Trim().Length - 3);
                        }

                        dvDiaryView.RowFilter = filter;
                        dvDiaryView.Sort = "OccDueDate desc";

                        DataSet dsFiltered = new DataSet();
                        dsFiltered.Tables.Add(dvDiaryView.ToTable());

                        e.DataSet = dsFiltered;

                        if (criteria.OrderBy != string.Empty)
                        {
                            DataTable dt = Functions.SortDataTable(e.DataSet.Tables[0], criteria.OrderBy);
                            e.DataSet.Tables.Remove("uvw_OccurencesForProject");
                            e.DataSet.Tables.Add(dt);
                        }
                    };

                    returnValue.Tasks = dataListCreator.Create(logonId,
                                            "TaskMatterSearch",
                        // Tell it the query criteria used so if the cache is accessed 
                        // again it knows if it is the same query
                                            criteria.ToString(),
                                            collectionRequest,
                        // Import mappings to map the dataset row fields to the data 
                        // list entity properties
                                            new ImportMapping[] {
                                                new ImportMapping("Id", "OccurrenceID"),
                                                new ImportMapping("DueDate", "OccDueDate"),
                                                new ImportMapping("Subject", "OccSpecificText"),
                                                new ImportMapping("StatusDesc", "OccStatus"),
                                                //new ImportMapping("Progress", "OccProgress"),
                                                new ImportMapping("AttendeesName", "AttendeeName"),
                                                //new ImportMapping("Matters", "OccurrenceMatter"),
                                                new ImportMapping("IsEditable", "isEdit"),
                                                new ImportMapping("IsLimitationTask", "IsLimitationTask")
                                                // More fields are required here but the service layer query may not be 
                                                // providing them.  This is enough for a test.
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
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Message = ex.Message;
            }

            return returnValue;
        }
        #endregion

        #region IsALimitationTask
        private Boolean IsALimitationTask(int bookingTypeID)
        {
            try
            {
                // 5 - LimitationDate
                return (bookingTypeID == 5);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}

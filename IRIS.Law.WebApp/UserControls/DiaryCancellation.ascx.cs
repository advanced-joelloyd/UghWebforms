using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Diary;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class DiaryCancellation : System.Web.UI.UserControl
    {
        private LogonReturnValue _logonSettings;

        #region WhichPage
        public string WhichPage
        {
            set
            {
                _lblHeader.Text = value;

                if (value.ToUpper() == "TASK")
                {
                    _lblLabel.Text = "Please enter a reason and description for the task deletion.";
                }
                else if (value.ToUpper() == "APPOINTMENT")
                {
                    _lblLabel.Text = "Please enter a reason and description for the appointment deletion.";
                }
            }
        }
        #endregion

        #region ModalPopupBehaviorId
        public string ModalPopupBehaviorId
        {
            get
            {
                return _modalpopupCancellation.BehaviorID;
            }
        }
        #endregion

        #region TaskAppointmentId
        public string TaskAppointmentId
        {
            get
            {
                if (_hdnId != null)
                {
                    return _hdnId.Value;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region ReasonCode
        public string ReasonCode
        {
            get
            {
                if (_ddlReason != null)
                {
                    return _ddlReason.SelectedValue;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region CategoryCode
        public string CategoryCode
        {
            get
            {
                if (_ddlCategory != null)
                {
                    return _ddlCategory.SelectedValue;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region CancellationText
        public string CancellationText
        {
            get
            {
                if (_txtDescription != null)
                {
                    return _txtDescription.Text;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region public Events

        public delegate void CancellationFinishedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the client reference changes.
        /// </summary>
        public event CancellationFinishedEventHandler CancellationFinished;
        protected virtual void OnCancellationFinished(EventArgs e)
        {
            if (CancellationFinished != null)
            {
                CancellationFinished(this, e);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session[SessionName.LogonSettings] == null)
            {
                Response.Redirect("~/Login.aspx?SessionExpired=1", true);
            }
            else
            {
                _logonSettings = (LogonReturnValue)HttpContext.Current.Session[SessionName.LogonSettings];
            }

            if (!IsPostBack)
            {
                _pnlCancellation.Style["display"] = "none";

                BindReasonType();
                BindCategoryType();
            }
        }

        #region BindReasonType
        private void BindReasonType()
        {
            DiaryServiceClient diaryService = null;
            try
            {
                diaryService = new DiaryServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                CancellationCodeSearchReturnValue returnValue = new CancellationCodeSearchReturnValue();
                returnValue = diaryService.GetBookingCancelledReasons(_logonSettings.LogonId, collectionRequest);

                if (returnValue.Success)
                {
                    _ddlReason.DataSource = returnValue.CancellationCodes.Rows;
                    _ddlReason.DataTextField = "Description";
                    _ddlReason.DataValueField = "Code";
                    _ddlReason.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
            finally
            {
                if (diaryService != null)
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }
            }
            //try
            //{
            //    _ddlReason.DataSource = DataTables.GetReasonType();
            //    _ddlReason.DataTextField = "reason";
            //    _ddlReason.DataValueField = "reason";
            //    _ddlReason.DataBind();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region BindCategoryType
        private void BindCategoryType()
        {
            DiaryServiceClient diaryService = null;
            try
            {
                diaryService = new DiaryServiceClient();

                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.ForceRefresh = true;
                collectionRequest.StartRow = 0;
                collectionRequest.RowCount = 0;

                CancellationCodeSearchReturnValue returnValue = new CancellationCodeSearchReturnValue();
                returnValue = diaryService.GetBookingCancelledCategories(_logonSettings.LogonId, collectionRequest);

                if (returnValue.Success)
                {
                    _ddlCategory.DataSource = returnValue.CancellationCodes.Rows;
                    _ddlCategory.DataTextField = "Description";
                    _ddlCategory.DataValueField = "Code";
                    _ddlCategory.DataBind();
                }
                else
                {
                    throw new Exception(returnValue.Message);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                _lblError.Text = DataConstants.WSEndPointErrorMessage;
                _lblError.CssClass = "errorMessage";
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
            finally
            {
                if (diaryService != null)
                {
                    if (diaryService.State != System.ServiceModel.CommunicationState.Faulted)
                        diaryService.Close();
                }
            }
            //try
            //{
            //    _ddlCategory.DataSource = DataTables.GetCategoryType();
            //    _ddlCategory.DataTextField = "category";
            //    _ddlCategory.DataValueField = "category";
            //    _ddlCategory.DataBind();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

        #region Reset button event
        protected void _btnReset_Click(object sender, EventArgs e)
        {
            _modalpopupCancellation.Show();
        }
        #endregion

        #region Finish button click event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                OnCancellationFinished(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
            }
            _modalpopupCancellation.Hide();
        }
        #endregion
    }
}
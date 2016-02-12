using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ContactGridView : System.Web.UI.UserControl
    {
        LogonReturnValue _logonSettings;

        #region Public Properties

        #region Message
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        #endregion

        #region MessageCssClass
        private string _messageCssClass;
        public string MessageCssClass
        {
            get
            {
                return _messageCssClass;
            }
            set
            {
                _messageCssClass = value;
            }
        }
        #endregion

        #region Delegate
        public delegate void ErrorEventHandler(object sender, EventArgs e);
        /// <summary>
        /// Occurs when the client reference changes.
        /// </summary>
        public event ErrorEventHandler ErrorOccured;

        protected virtual void OnErrorOccured(EventArgs e)
        {
            if (ErrorOccured != null)
            {
                ErrorOccured(this, e);
            }
        }

        #endregion

        #endregion

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

            //Set the page size for the grid
            _grdContactDetails.PageSize = 11;// Convert.ToInt32(ConfigurationManager.AppSettings["GridViewPageSize"]);
            if (!IsPostBack)
            {
                DisplayContactDetails();
                SetControlAccessibility();
            }
        }
        #endregion

        #region BindDatasource
        public void DisplayContactDetails()
        {
            _grdContactDetails.DataSource = Session[SessionName.ContactDetails];
            _grdContactDetails.DataBind();
        }
        #endregion

        /// <summary>
        /// Sets the visibility according to user type and permissions
        /// </summary>
        private void SetControlAccessibility()
        {
            Dictionary<string, bool> objPerm = (Dictionary<string, bool>)Session[SessionName.ControlSettings];
            
            if (_logonSettings.UserType == (int)DataConstants.UserType.Client)
            {
                if (!(_logonSettings.MemberId == new Guid(Session[SessionName.MemberId].ToString()) &&
                    _logonSettings.OrganisationId == new Guid(Session[SessionName.OrganisationId].ToString())))
                {
                    _grdContactDetails.Columns[0].Visible = false;
                }
            }

            if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty)
            {
                if (!(Request.QueryString["mydetails"] == "true"))
                {
                    _grdContactDetails.Columns[0].Visible = false;
                }
            }
        }

        protected void _grdContactDetails_RowEditing(object sender, GridViewEditEventArgs e)
        {
            _grdContactDetails.EditIndex = e.NewEditIndex;
            DisplayContactDetails();
        }

        protected void _grdContactDetails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            _grdContactDetails.EditIndex = -1;
            DisplayContactDetails();
        }

        protected void _grdContactDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow
                && (e.Row.RowState != (DataControlRowState.Edit | DataControlRowState.Alternate)
                && e.Row.RowState != DataControlRowState.Edit))
            {
                Label lblElementText = (Label)e.Row.FindControl("_lblElementText");
                if (IsAUrl(lblElementText.Text))
                {
                    if (lblElementText.Text.IndexOf("http") == -1 && lblElementText.Text.IndexOf("https") == -1)
                    {
                        lblElementText.Text = string.Format("<a class=\"link\" href=\"http://{0}\" target=\"_blank\">{0}</a>", lblElementText.Text);
                    }
                    else
                    {
                        lblElementText.Text = string.Format("<a class=\"link\" href=\"{0}\" target=\"_blank\">{0}</a>", lblElementText.Text);
                    }
                }

                if (IsAnEMailAddress(lblElementText.Text))
                {
                    // If the item is an E-Mail address launch an instance of the default mail client.
                    string mailStr = "mailto:" + lblElementText.Text.Trim();
                    lblElementText.Text = "<a class='link' href='" + mailStr + "'>" + lblElementText.Text + "</a>";
                    return;
                }
            }
        }

        protected void _grdContactDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ContactServiceClient contactService = null;

            try
            {
                TextBox txtElementText = (TextBox)_grdContactDetails.Rows[e.RowIndex].FindControl("_txtElementText");
                contactService = new ContactServiceClient();
                AdditionalAddressElement element = new AdditionalAddressElement();

                Guid _memberId = DataConstants.DummyGuid;
                Guid _organisationId = DataConstants.DummyGuid;

                if (Session[SessionName.MemberId] != null && Session[SessionName.OrganisationId] != null)
                {
                    _memberId = (Guid)Session[SessionName.MemberId];
                    _organisationId = (Guid)Session[SessionName.OrganisationId];
                }

                if (_logonSettings.UserType == (int)DataConstants.UserType.ThirdParty && Request.QueryString["mydetails"] == "true")
                {
                    _memberId = _logonSettings.MemberId;
                    _organisationId = _logonSettings.OrganisationId;
                }

                element.MemberId = _memberId;
                element.OrganisationId = _organisationId;
                element.TypeId = (int)_grdContactDetails.DataKeys[e.RowIndex].Values["TypeId"];
                element.ElementText = txtElementText.Text.Trim();
                element.ElementComment = ((TextBox)_grdContactDetails.Rows[e.RowIndex].FindControl("_txtElementComment")).Text.Trim();
                element.AddressId = (int)_grdContactDetails.DataKeys[e.RowIndex].Values["AddressId"];

                ReturnValue returnValue = contactService.SaveAdditionalAddressElement(((LogonReturnValue)Session[SessionName.LogonSettings]).LogonId, element);
                if (returnValue.Success)
                {
                    //update the modified additional info in the session
                    AdditionalAddressElement[] additionalAddressElements = (AdditionalAddressElement[])Session[SessionName.ContactDetails];
                    for (int i = 0; i < additionalAddressElements.Length; i++)
                    {
                        if (additionalAddressElements[i].TypeId == element.TypeId)
                        {
                            additionalAddressElements[i].ElementText = element.ElementText;
                            additionalAddressElements[i].ElementComment = element.ElementComment;
                            break;
                        }
                    }

                    _messageCssClass = "successMessage";
                    _message = "Edit successful";
                }
                else
                {
                    _messageCssClass = "errorMessage";
                    _message = returnValue.Message;
                }

                _grdContactDetails.EditIndex = -1;
                DisplayContactDetails();
            }
            catch (Exception ex)
            {
                _messageCssClass = "errorMessage";
                _message = ex.Message;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }

                if (ErrorOccured != null)
                {
                    OnErrorOccured(System.EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Method to determine whether the ListViewItem is a URL.
        /// </summary>
        /// <param name="addressElementText">The ListViewItem text to check.</param>
        /// <returns>Boolean, whether the ListViewItem is a URL.</returns>
        private bool IsAUrl(string addressElementText)
        {
            bool retValue = false;

            // Check to see if its a URL.
            if (addressElementText.IndexOf("http") > -1 | addressElementText.IndexOf("https") > -1 | addressElementText.IndexOf("www") > -1)
            {
                retValue = true;
            }
            return retValue;
        }

        /// <summary>
        /// Method to determine whether the ListViewItem is an E-Mail address.
        /// </summary>
        /// <param name="addressElementText">The ListViewItem text to check.</param>
        /// <returns>Boolean, whether the ListViewItem is an E-Mail address.</returns>
        private bool IsAnEMailAddress(string addressElementText)
        {
            bool retValue = false;

            // Check to see if its an E-Mail address.
            if (addressElementText.IndexOf("@") > -1 & addressElementText.IndexOf(".") > -1)
            {
                retValue = true;
            }
            return retValue;
        }

        protected void _grdContactDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                _grdContactDetails.EditIndex = -1;
                _grdContactDetails.PageIndex = e.NewPageIndex;
                DisplayContactDetails();
            }
            catch (Exception ex)
            {
                _messageCssClass = "errorMessage";
                _message = ex.Message;

                if (ErrorOccured != null)
                {
                    OnErrorOccured(System.EventArgs.Empty);
                }
            }
        }
    }
}
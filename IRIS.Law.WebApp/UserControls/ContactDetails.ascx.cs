using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces.Logon;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class ContactDetails : System.Web.UI.UserControl
    {
        private LogonReturnValue _logonSettings;

        #region Public Label Properties

        public string LabelSurname
        {
            set
            {
                _lblContactSurname.Text = value;
            }
        }

        public string LabelForename
        {
            set
            {
                _lblContactForename.Text = value;
            }
        }

        public string LabelTitle
        {
            set
            {
                _lblContactTitle.Text = value;
            }
        }

        public string LabelSex
        {
            set
            {
                _lblContactSex.Text = value;
            }
        }

        public string LabelPosition
        {
            set
            {
                _lblContactPosition.Text = value;
            }
        }

        public string LabelDescription
        {
            set
            {
                _lblContactDescription.Text = value;
            }
        }

        #endregion

        #region Public Enable Properties

        public bool EnableSurname
        {
            set
            {
                if (!value)
                {
                    _trContactSurname.Style["display"] = "none";
                }
            }
        }

        public bool EnableForename
        {
            set
            {
                if (!value)
                {
                    _trContactForename.Style["display"] = "none";
                }
            }
        }

        public bool EnableTitle
        {
            set
            {
                if (!value)
                {
                    _trContactTitle.Style["display"] = "none";
                }
            }
        }

        public bool EnableSex
        {
            set
            {
                if (!value)
                {
                    _trContactSex.Style["display"] = "none";
                }
            }
        }

        public bool EnablePosition
        {
            set
            {
                if (!value)
                {
                    _trContactPosition.Style["display"] = "none";
                }
            }
        }

        public bool EnableDescription
        {
            set
            {
                if (!value)
                {
                    _trContactDescription.Style["display"] = "none";
                }
            }
        }

        #endregion

        #region Public Properties

        public string SurName
        {
            get
            {
                return _txtContactSurname.Text;
            }
        }

        public string ForeName
        {
            get
            {
                return _txtContactForename.Text;
            }
        }

        public string Title
        {
            get
            {
                return _ddlContactTitle.Text;
            }
        }

        public string Sex
        {
            get
            {
                return _ddlContactSex.Text;
            }
        }

        public string Position
        {
            get
            {
                return _txtContactPosition.Text;
            }
        }

        public string Description
        {
            get
            {
                return _txtContactDescription.Text;
            }
        }
        #endregion Public Property

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[SessionName.LogonSettings] == null)
                {
                    Response.Redirect("~/Login.aspx?SessionExpired=1", true);
                    return;
                }
                _logonSettings = (LogonReturnValue)Session[SessionName.LogonSettings];

                if (!IsPostBack)
                {
                    BindTitle();
                    BindSex();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region BindTitle
        public void BindTitle()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                TitleSearchCriteria titleCriteria = new TitleSearchCriteria();
                TitleSearchReturnValue titleReturnValue = contactService.TitleSearch(_logonSettings.LogonId, collectionRequest, titleCriteria);
                if (titleReturnValue.Title != null)
                {
                    _ddlContactTitle.DataSource = titleReturnValue.Title.Rows;
                    _ddlContactTitle.DataTextField = "TitleId";
                    _ddlContactTitle.DataValueField = "TitleId";
                    _ddlContactTitle.DataBind();
                }
                AddDefaultToDropDownList(_ddlContactTitle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }
        #endregion

        #region BindSex
        public void BindSex()
        {
            ContactServiceClient contactService = null;
            try
            {
                contactService = new ContactServiceClient();
                CollectionRequest collectionRequest = new CollectionRequest();
                collectionRequest.StartRow = 0;

                SexSearchCriteria sexCriteria = new SexSearchCriteria();
                SexSearchReturnValue sexReturnValue = contactService.SexSearch(_logonSettings.LogonId, collectionRequest, sexCriteria);
                if (sexReturnValue.Sex != null)
                {
                    _ddlContactSex.DataSource = sexReturnValue.Sex.Rows;
                    _ddlContactSex.DataTextField = "Description";
                    _ddlContactSex.DataValueField = "Id";
                    _ddlContactSex.DataBind();
                }
                AddDefaultToDropDownList(_ddlContactSex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (contactService != null)
                {
                    if (contactService.State != System.ServiceModel.CommunicationState.Faulted)
                        contactService.Close();
                }
            }
        }
        #endregion

        #region AddDefaultToDropDownList
        /// <summary>
        /// Add default value "Select" to the dropdownlist
        /// </summary>
        /// <param name="ddlList"></param>
        private void AddDefaultToDropDownList(DropDownList ddlList)
        {
            ListItem listSelect = new ListItem("Select", "");
            ddlList.Items.Insert(0, listSelect);
        }
        #endregion

        #region ClearFields
        /// <summary>
        /// Clears the fields.
        /// </summary>
        public void ClearFields()
        {
            _txtContactSurname.Text = string.Empty;
            _txtContactForename.Text = string.Empty;
            _ddlContactTitle.SelectedIndex = -1;
            _ddlContactSex.SelectedIndex = -1;
            _txtContactPosition.Text = string.Empty;
            _txtContactDescription.Text = string.Empty;
        }
        #endregion
    }
}
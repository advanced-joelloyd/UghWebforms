using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class AddSubtractDays : System.Web.UI.UserControl
    {
        public int DefaultNoOfDays
        {
            set
            {
                _txtNoOfDays.Text = value.ToString();
            }
        }

        public int MaxLength
        {
            set
            {
                _txtNoOfDays.MaxLength = value;
            }
        }

        private string _onNoOfChangeEvent = string.Empty;
        public string OnNoOfChangeEvent
        {
            set
            {
                _onNoOfChangeEvent = value;
                //_txtNoOfDays.Attributes.Add("onchange", "javascript:" + value);
            }
        }

        public string UnitsTextBoxClientID
        {
            get
            {
                return _txtNoOfDays.ClientID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _btnAddNoOfDays.Attributes.Add("onclick", "javascript:AddUnits('" + _txtNoOfDays.ClientID + "');" + _onNoOfChangeEvent + "return false;");
                _btnSubtractNoOfDays.Attributes.Add("onclick", "javascript:SubtractUnits('" + _txtNoOfDays.ClientID + "');" + _onNoOfChangeEvent + "return false;");
                //if (string.IsNullOrEmpty(_onNoOfChangeEvent))
                //{
                //}
                //else
                //{
                //    _btnAddNoOfDays.Attributes.Add("onclick", "javascript:if(!AddUnits('" + _txtNoOfDays.ClientID + "')){" + _onNoOfChangeEvent + "}");
                //    _btnSubtractNoOfDays.Attributes.Add("onclick", "javascript:if(!SubtractUnits('" + _txtNoOfDays.ClientID + "')){" + _onNoOfChangeEvent + "}");
                //}
            }
        }
    }
}
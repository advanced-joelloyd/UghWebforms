using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class Recurrence : System.Web.UI.UserControl
    {
        public string WhichPage
        {
            set
            {
                _whichPage.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _radioWeekly.Checked = true;
                _pnlRecurrence.Style["display"] = "none";
            }
        }


        #region Save
        protected void _btnSave_Click(object sender, EventArgs e)
        {
        }
        #endregion

        #region Recurrence
        protected void _btnRecurrence_Click(object sender, EventArgs e)
        {
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using IRIS.Law.WebApp.App_Code;

namespace IRIS.Law.WebApp.Pages.Information
{
    public partial class ContactUs : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
              
               _text.Text = GetText();
              
            }
        }

        private string GetText()
        {

            string text = "";

            if (!this.Page.IsPostBack)
            {
                try
                {
                    text = File.ReadAllText(Server.MapPath("~/PageTextFiles/ContactUs.txt"));
                }
                catch (Exception ex)
                {
                    _lblError.Text = ex.Message;
                    _lblError.CssClass = "errorMessage";
                }

            }

            return text;
        }
    }
}

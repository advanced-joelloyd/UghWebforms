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

namespace IRIS.Law.WebApp.Pages.Admin
{
    public partial class ContactUsAdministration : BasePage
    {

               
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                try
                {
                    _htmleditor.Content = File.ReadAllText(Server.MapPath("~/PageTextFiles/ContactUs.txt"));
                }
                catch (Exception ex)
                {
                    _lblError.Text = ex.Message;
                    _lblError.CssClass = "errorMessage";
                }

            }
        }

        protected void _btnSave_Click(object sender, EventArgs e)
        {
            
            
            try
            {
                using (StreamWriter sw = new StreamWriter(Server.MapPath("~/PageTextFiles/ContactUs.txt"),false))
                {
                    sw.Write(_htmleditor.Content);
                    sw.Close();
                }

                _lblError.Text = "Text Saved!";
                _lblError.CssClass = "successMessage";

            }
            catch (Exception ex)
            {
                _lblError.Text = ex.Message;
                _lblError.CssClass = "errorMessage";
            }
            

        }

       
    }
}

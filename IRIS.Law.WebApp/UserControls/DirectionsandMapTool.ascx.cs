using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class DirectionsandMapTool : System.Web.UI.UserControl
    {
        private string address;

        public string Address
        {
            set { address = value; }
            get { return address; }
        }

        public void Rebind()
        {
            if (address == "")
            {
                _lblError.Text = "No Address Entered";
                _btnGoogleLink.Enabled = false;
            }
            else
            {

                _txtDirectionTo.Text = address;
                _lblPostcode.Text = address;
                _btnGoogleLink.Enabled = true;

            }
           
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (address == "")
                {
                    _lblError.Text = "No Address Entered";
                    _btnGoogleLink.Enabled = false;
                }
                else
                {

                    _txtDirectionTo.Text = address;
                    _lblPostcode.Text = address;
                    _btnGoogleLink.Enabled = true;

                }
            }
        }

        protected void _chkDirections_CheckedChanged(object sender, EventArgs e)
        {
            if (_chkDirections.Checked)
            {
                _pnlMapDirections.Visible = true;
            }
            else
            {
                _pnlMapDirections.Visible = false;
            }
        }

        protected void _btnGoogleLink_Click(object sender, EventArgs e)
        {
            string view = "";
            
            switch (_ddlMapView.SelectedValue)
            {
                case "Map":
                    view = "z=14";
                    break;
                case "Satellite":
                    view = "t=h&z=14";
                    break;
                case "Terrain":
                    view = "t=p&z=14";
                    break;
            }

            //string jscript = "javascript:w = window.open('http://maps.google.co.uk/maps?hl=en&safe=active&q=" + _lblPostcode.Text + "&ie=UTF8&om=1&hq=&hnear=" + _lblPostcode.Text + "&" + view + "',null,'');";

            string jscript = "var w = window.open(); w.opener = null; w.document.location = 'http://maps.google.co.uk/maps?hl=en&safe=active&q=" + _lblPostcode.Text + "&ie=UTF8&om=1&hq=&hnear=" + _lblPostcode.Text + "&" + view + "';";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                       jscript, true);
        }

        protected void _btnGoogleDirections_Click(object sender, EventArgs e)
        {
            string view = "";

            switch (_ddlMapView.SelectedValue)
            {
                case "Map":
                    view = "z=11";
                    break;
                case "Satellite":
                    view = "t=h&z=11";
                    break;
                case "Terrain":
                    view = "t=p&z=11";
                    break;
            }

            string jscript = "window.open('http://maps.google.co.uk/maps?f=d&source=s_d&saddr=" + _txtDirectionFrom.Text + "&daddr=" + _txtDirectionTo.Text + "&hl=en&geocode=&mra=ls&sll=52.938705,-1.129226&sspn=0.029795,0.073557&g=" + _txtDirectionTo.Text + "&ie=UTF8&" + view + "');";

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
                       jscript, true);
        }
    }
}
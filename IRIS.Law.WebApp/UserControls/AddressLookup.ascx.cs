using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRIS.Law.WebServiceInterfaces.Contact;
using IRIS.Law.WebServiceInterfaces.Client;
using IRIS.Law.WebApp.App_Code;
using IRIS.Law.WebServiceInterfaces;
using System.Text;
using IRIS.Law.WebServiceInterfaces.Logon;
using System.Reflection;
using System.Xml;

namespace IRIS.Law.WebApp.UserControls
{
    public partial class AddressLookup : System.Web.UI.UserControl
    {
        public string AddressControlsList
        {
            get { return Server.HtmlDecode(_hdnAddressList.Value); }
            set { _hdnAddressList.Value = Server.HtmlEncode(value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _listAddress.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(_listAddress, "DBmove"));
        }

        private void AssignParentControls(string listboxItemValue, XmlDocument xdoc)
        {

            if (!string.IsNullOrEmpty(listboxItemValue))
            {
                string[] attributesValues = listboxItemValue.Split('~');

                XmlNode node = xdoc.SelectSingleNode("//address/HouseName");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[3];
                }
                node = xdoc.SelectSingleNode("//address/HouseNumber");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[2];
                }
                node = xdoc.SelectSingleNode("//address/Line1");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[4];
                }
                node = xdoc.SelectSingleNode("//address/Country");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[10];
                }
                node = xdoc.SelectSingleNode("//address/Town");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[7];
                }

                node = xdoc.SelectSingleNode("//address/County");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[8];
                }
                node = xdoc.SelectSingleNode("//address/PostCode");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[9];
                }
                node = xdoc.SelectSingleNode("//address/DepLocality");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[5];
                }

                node = xdoc.SelectSingleNode("//address/Line3");
                if (node != null)
                {
                    (this.Parent.FindControl(node.InnerText) as TextBox).Text = attributesValues[6];
                }
            }
                        
        }

        protected void OkButton_onclick(object sender, EventArgs arg)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(AddressControlsList);

            AssignParentControls(_listAddress.SelectedValue, xdoc);
        }
    }
}

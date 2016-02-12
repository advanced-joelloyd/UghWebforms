using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IRIS.Law.WebApp.MasterPages
{
    public partial class ILBLoginPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            companyHyperlink.HRef = Solicitors.Branding.Strings.LawBusinessHomePageUrl;
            companyHyperlink.InnerText = Solicitors.Branding.Strings.DivisionName;

            copyrightHyperlink.HRef = Solicitors.Branding.Strings.LawBusinessHomePageUrl;
            copyrightHyperlink.InnerText = Solicitors.Branding.Strings.CopyrightNotice;

        }
    }
}

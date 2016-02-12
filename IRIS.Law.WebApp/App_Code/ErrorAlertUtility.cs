using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRIS.Law.WebApp.UserControls;

namespace IRIS.Law.WebApp.App_Code
{
  public class ErrorAlertUtility
    {
        public void CallAlertMessage(System.Web.UI.Page page, string message,string type)
        {
            if (page.MasterPageFile != null)
            {
                uscMsgBox uc = (uscMsgBox)page.Master.FindControl("uscErrorMsgBox");

                if (uc != null)
                {
                    if(string.IsNullOrEmpty(type))
                    uc.AddMessage(message, uscMsgBox.enmMessageType.Attention);
                    else if (type.ToLower().Equals("confirm"))
                    {
                        uc.AddMessage(message, uscMsgBox.enmMessageType.Attention,true,false,string.Empty);
                    }
                }
            }
        }

    }
}

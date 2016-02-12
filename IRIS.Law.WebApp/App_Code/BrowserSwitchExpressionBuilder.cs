// IRIS.Law.WebApp.App_Code.BrowserSwitchExpressionBuilder
// This is the Custom Expression builder which will allow property values to be set 
// and retrieved in a control during page parsing
// The expression builder will return the requested value to the page
// This will check, if browser is Safari & the requested page is AddClient.aspx 
// then the value returned is false, else true
// With this value, you can set any property of the control 
// Aim to do custom expression builder is, 
// In Add Client screen, we are using combination of UpdatePanel, Modalpopup & Wizard Control
// In Safari the above three combination doesn't work
// so we will set the EnablePartialRendering of Add Client screen to false
// so that the Add Client screen will do postback on each events

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Compilation;
using System.CodeDom;

namespace IRIS.Law.WebApp.App_Code
{
    public class BrowserSwitchExpressionBuilder : ExpressionBuilder
    {

        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            if ((entry.DeclaringType == null) || (entry.PropertyInfo == null))
            {
                return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.GetType()), "GetIsNotSafari", new CodeExpression[] { new CodePrimitiveExpression(entry.Expression.Trim()) });
            }
            return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(base.GetType()), "GetIsNotSafari", new CodeExpression[] { new CodePrimitiveExpression(entry.Expression.Trim()), new CodeTypeOfExpression(entry.DeclaringType), new CodePrimitiveExpression(entry.PropertyInfo.Name) });
        }
        public static object GetIsNotSafari(string key, Type targetType, string propertyName)
        {
            if (System.Web.HttpContext.Current.Request.Browser.Browser != "AppleMAC-Safari")
            {
                return true;
            }
            else
            {
			    string url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                string[] arrfile = url.Split('/');
                string _strFileName = arrfile[arrfile.Length - 1].ToLower();

                if (_strFileName == "addclient.aspx")
                {
                    return false;
                }

                return true;                
            }
        }
        public static object GetIsNotSafari(string key)
        {
            if (System.Web.HttpContext.Current.Request.Browser.Browser != "AppleMAC-Safari")
            {
                return true;
            }
            else
            {
                string url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                string[] arrfile = url.Split('/');
                string _strFileName = arrfile[arrfile.Length - 1].ToLower();

                if (_strFileName == "addclient.aspx")
                {
                    return false;
                }

                return true;
            }
        }
        public override bool SupportsEvaluate
        {
            get
            {
                return true;
            }
        }
        public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return BrowserSwitchExpressionBuilder.GetIsNotSafari(entry.Expression, target.GetType(), entry.PropertyInfo.Name);
        }

    }
}

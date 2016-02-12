<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginMobile.aspx.cs" Inherits="IRIS.Law.WebApp.LoginMobile" %>
<?xml version="1.0" encoding="utf-8" ?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML BASIC 1.1//EN" "http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
    <meta name="viewport" content="width=400" />
    <link rel="Stylesheet" href="CSS/Master.css" />
</head>
<body style="margin: 0px">
    <form id="form1" runat="server">
    <table width="100%" style="text-align: center;">
        <tr>
            <td style="height: 8px">
            </td>
        </tr>
        <tr>
            <td class="boldTxt" style="padding-top: 10px; text-indent: 5px; width: 30%">
                User Name
            </td>
            <td align="left" valign="bottom">
                <asp:TextBox ID="_txtUsername" runat="server" CssClass="textBox" onmousemove="showToolTip(event);return false;"
                    onmouseout="hideToolTip();"></asp:TextBox>
                <asp:RequiredFieldValidator ID="_rfvUsername" Display="None" runat="server" ControlToValidate="_txtUsername"
                    ErrorMessage="Username is mandatory"></asp:RequiredFieldValidator>
                <span class="mandatoryField">*</span>
            </td>
        </tr>
        <tr>
            <td class="boldTxt" style="text-indent: 5px; width: 30%">
                Password
            </td>
            <td align="left" valign="bottom">
                <asp:TextBox ID="_txtPassword" runat="server" TextMode="Password" CssClass="textBox"
                    onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                <asp:RequiredFieldValidator ID="_rfvPassword" Display="None" runat="server" ControlToValidate="_txtPassword"
                    ErrorMessage="Password is mandatory"></asp:RequiredFieldValidator>
                <span class="mandatoryField">*</span>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td align="left">
                <table cellpadding="2">
                    <tr>
                        <td>
                            <asp:Button ID="_btnLogin" runat="server" Text="Login" CausesValidation="true" OnClick="_btnLogin_Click"
                                CssClass="buttonMobile" EnableTheming="false" />
                        </td>
                        <td>
                            <asp:Button ID="_btnReset" runat="server" Text="Reset" CausesValidation="false" OnClick="_btnReset_Click"
                                CssClass="buttonMobile" EnableTheming="false" OnClientClick="return Reset();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="border-top: solid 1px #1e7b84;">
                <img src="Images/logoSmall.png" alt="Logo" height="25px" width="50px" align="right" />
            </td>
        </tr>
    </table>
    <div id="bubble_tooltip" class="errorMessageTooltip">
    </div>

    <script src="./Javascript/jquery-1.3.2.js" type="text/javascript" defer="defer"></script>

    <script src="./Javascript/Validation.js" type="text/javascript" defer="defer"></script>

    <script src="./Javascript/Bubble-Tooltip.js" type="text/javascript" defer="defer"></script>

    <script type="text/javascript">
        function Reset() {
            $("#<%= _txtUsername.ClientID%>").val("");
            $("#<%= _txtPassword.ClientID%>").val("");
            ResetValidators("");
            return false;
        }

        function ResetValidators(GroupName) {
            for (var vI = 0; vI < Page_Validators.length; vI++) {
                var vVal = Page_Validators[vI];
                if (IsValidationGroupMatch(vVal, GroupName)) {
                    vVal.isvalid = true;
                    ValidatorUpdateDisplay(vVal);
                }
            }
        }
    </script>

    </form>
</body>
</html>

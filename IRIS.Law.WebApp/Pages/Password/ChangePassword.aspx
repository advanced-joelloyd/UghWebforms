<%@ Page Title="Change Password" Language="C#" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" MasterPageFile="~/MasterPages/ILBHomePage.Master" Inherits="IRIS.Law.WebApp.Pages.Password.ChangePassword" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:content id="Content1" contentplaceholderid="_cphMain" runat="server">
    <script type="text/javascript">
        var browser = navigator.appName;
        //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
        //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
        //on box-items. This is not yet implemented in IE so round the corners using javascript
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }
        function RoundedCorners() {
            Nifty("div.button");
        }
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                Change Password
            </td>
        </tr>
        <tr>
            <td style="width: 25px;">
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2">
    <tr>
        <td class="boldTxt">Please enter your new password.</td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td class="boldTxt">Current Password: </td>
        <td><asp:TextBox id="_txtCurrentPassword" TextMode="Password" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
        
            <asp:RequiredFieldValidator ID="_rfvCurrentPassword" CssClass="errorMessage" runat="server" ControlToValidate="_txtCurrentPassword"
                  ErrorMessage="Password is mandatory" Display="None" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="boldTxt">New Password: </td>
        <td>
            <ajaxToolkit:PasswordStrength ID="PS" runat="server"
                TargetControlID="_txtNewPassword"
                DisplayPosition="RightSide"
                StrengthIndicatorType="Text"
                PreferredPasswordLength="8"
                PrefixText="Strength:"
                TextCssClass="TextIndicator_TextBox"
                MinimumNumericCharacters="2"
                MinimumSymbolCharacters="0"
                RequiresUpperAndLowerCaseCharacters="true"
                TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent"
                TextStrengthDescriptionStyles="TextIndicator_TextBox_Strength1;TextIndicator_TextBox_Strength2;TextIndicator_TextBox_Strength3;TextIndicator_TextBox_Strength4;TextIndicator_TextBox_Strength5"
                CalculationWeightings="50;15;15;20" />    
                
                 
            <asp:TextBox id="_txtNewPassword" TextMode="Password" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
        
            <asp:RequiredFieldValidator ID="_rfvPassword" CssClass="errorMessage" runat="server" ControlToValidate="_txtNewPassword"
                  ErrorMessage="Password is mandatory" Display="None" ></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="boldTxt">Confirm New Password: </td>
        <td><asp:TextBox id="_txtConfirmNewPassword" TextMode="Password" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
             
             
                                                    
            <asp:CompareValidator ID="_cvPassword" runat="server" Operator="Equal" CssClass="errorMessage" ControlToValidate="_txtConfirmNewPassword" ControlToCompare="_txtNewPassword" ErrorMessage="The passwords do not match"></asp:CompareValidator>
                        
            <asp:RequiredFieldValidator ID="_rfvConfirmPassword" Display="None" runat="server" ControlToValidate="_txtConfirmNewPassword"
                                                    ErrorMessage="Password is mandatory"></asp:RequiredFieldValidator>                                                  
        </td>
    </tr>
    <tr>
        <td align="right" colspan="2">
            <div class="button"><asp:Button ID="_btnSubmit" runat="server" 
                CausesValidation="True" Text="Submit" onclick="_btnSubmit_Click" /></div>
        </td>
        
    </tr>
    </table>
    

    
</asp:content>

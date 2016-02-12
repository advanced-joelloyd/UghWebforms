<%@ Page Language="C#" AutoEventWireup="true" Title="Forgotten Password" MasterPageFile="~/MasterPages/ILBLoginPage.Master" CodeBehind="ForgottenPassword.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Password.ForgottenPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

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
    
    <!-- MAIN CONTENT -->
    <table cellpadding="0" cellspacing="0" style="width: 100%; padding-left: 30px; padding-right: 30px;
        padding-top: 15px; text-align: left; height: 225px;">
        <tr>
            <td class="loginBoldTxt" colspan="2">
                  Forgotten Password Your Password Details?
             </td>
        </tr>
        <tr>
            <!-- ROW -->
            <td >
                 <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   
                </asp:UpdatePanel>
                <br />
                <table cellpadding="2">
                <tr>
                    <td colspan="2" class="boldTxt">We'll email you your new password. Please fill out the details below.</td>
                </tr>
                <tr>
                    <td> <br /></td>
                </tr>
                <tr>
                    <td class="boldTxt" align="right">Email address: </td>
                    <td><asp:TextBox ID="_txtEmail" runat="server" Width="200" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                        
                        <asp:RequiredFieldValidator ID="_rfvEmail" CssClass="errorMessage" runat="server" ControlToValidate="_txtEmail"
                                ErrorMessage="Email is mandatory"></asp:RequiredFieldValidator>
                                                            
                        <asp:RegularExpressionValidator id="valRegEx" runat="server"
                            ControlToValidate="_txtEmail" Display="None"
                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                            ErrorMessage="Your entry is not a valid e-mail address."
                            >
                        </asp:RegularExpressionValidator>
                        
                       
                    
                        </td>
                </tr>
                <tr>
                    <td class="boldTxt" align="right">Please re-confirm your email address: </td>
                    <td><asp:TextBox ID="_txtEmailConfirm" runat="server" Width="200" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                        
                         <asp:RequiredFieldValidator ID="_rfvEmailConfirm" CssClass="errorMessage" runat="server" ControlToValidate="_txtEmailConfirm"
                                ErrorMessage="Email is mandatory"></asp:RequiredFieldValidator>
                        
                        <asp:CompareValidator ID="_cvEmail" runat="server" Operator="Equal" ControlToValidate="_txtEmailConfirm" ControlToCompare="_txtEmail" Display="None" ErrorMessage="The email addresses do not match"></asp:CompareValidator>
                        
                       
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" valign="top" align="right">Type the characters you see in the image: </td>
                    <td>
                        <img height="30" alt="" src="Captcha.aspx" width="80" /> 
                        <br /><br />
                        <asp:TextBox ID="_txtCaptcha" runat="server" Width="75" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                        
                         <asp:RequiredFieldValidator ID="_rfvCaptcha" Display="None" runat="server" ControlToValidate="_txtCaptcha"
                                ErrorMessage="Captcha is mandatory"></asp:RequiredFieldValidator>
                         
                        <asp:Label runat="server" ID="_lblCaptchaError" CssClass="errorMessage"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2">
                        <table cellpadding="0" cellspacing="0">
                        <tr>
                            
                            <td> <div class="button"><asp:Button ID="_btnSend" runat="server" 
                                CausesValidation="True" Text="Send" onclick="_btnSend_Click" /></div></td>
                            <td>&nbsp;</td>    
                           <td><div class="button"><asp:Button ID="_btnback" runat="server" 
                                CausesValidation="False" Text="Back" onclick="_btnBack_Click" /></div></td>
                            
                        </tr>
                        </table>
                        
                    
                       
                    </td>
                </tr>
                </table>
            </td>
            
        </tr>
    </table>
    
    
    
</asp:Content>
            
            
            
            
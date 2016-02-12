<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBLoginPage.Master"
    CodeBehind="Login.aspx.cs" Inherits="IRIS.Law.WebApp.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

    <!-- MAIN CONTENT -->
    <table cellpadding="0" cellspacing="0" style="width: 100%; padding-left: 30px; padding-right: 30px;
        padding-top: 15px; text-align: left; height: 225px">
        <tr>
            <!-- ROW -->
            <td style="width: 45%">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td style="width: 2%;" align="center" valign="middle">
                            &nbsp;
                        </td>
                        <td style="width: 95%;" class="loginBoldTxt">
                            <label runat="server" ID="welcomeMessage"></label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 25px;">
                            &nbsp;
                        </td>
                        <td style="height: 25px;">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <p class="loginNormalTxt">
                                Monitor the progress of your case from the comfort of your own home or office, 24
                                hours a day, 7 days a week.</p>
                            <p class="loginNormalTxt">
                                Secure instant access to the latest status, regardless of location or time of day.</p>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 5%;" align="right" valign="middle">
                <table style="border-right: solid 1pt gray; height: 140px">
                    <tr>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 45%; background-color: White">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="height: 28px;">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="text-align: center; font-size: 15px;" class="boldTxt">
                                        Login</div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table>
                                        <tr>
                                            <td>
                                                <span class="boldTxt">User Name</span>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtUsername" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvUsername" Display="None" runat="server" ControlToValidate="_txtUsername"
                                                    ErrorMessage="Username is mandatory"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="boldTxt">Password</span>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtPassword" TextMode="Password" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvPassword" Display="None" runat="server" ControlToValidate="_txtPassword"
                                                    ErrorMessage="Password is mandatory"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 9px;" colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td align="right">
                                                <div class="button" style="float: left;">
                                                    <asp:Button ID="_btnLogin" OnClick="_btnLogin_Click" runat="server" CausesValidation="True"
                                                        Text="Login" />
                                                </div>
                                                <div class="button">
                                                    <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 9px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 15px;">
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" align="center">
                                    <a href="Pages/Password/ForgottenPassword.aspx" class="boldTxt">Forgotten your login details?</a>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        function fnReset() {
            document.getElementById("<%=_txtUsername.ClientID %>").value = "";
            document.getElementById("<%=_txtPassword.ClientID %>").value = "";
            document.getElementById("<%=_lblError.ClientID %>").innerText = "";
            return false;
        }
    </script>

</asp:Content>

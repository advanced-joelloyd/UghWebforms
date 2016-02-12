<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressLookup.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.AddressLookup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<table width="40px">
    <tr>
        <td>
           <asp:HiddenField ID="_hiddenShowpopUP" runat="server" Value="false"/>
            <asp:ImageButton ID="_btnAddressLookup" runat="server" AlternateText="Address Lookup"
                CausesValidation="false" Height="20px" ImageUrl="~/Images/PNGs/addressLookup.png"
                ToolTip="Address Lookup" Width="20px" Style="margin-bottom: 0px" 
                OnClick="_btnAddressLookup_Click" />
                <asp:HiddenField ID="_hdnAddressList" runat="server" />
        </td>
        <td>
            <ajaxToolkit:ModalPopupExtender ID="_mpePostcodeLookup" runat="server" BackgroundCssClass="modalBackground" 
                PopupControlID="_pnlAddressOnPostCode" TargetControlID="_hiddenShowpopUP" CancelControlID="CancelButton" >
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server">
    </asp:ScriptManagerProxy>
</table>
<asp:Panel ID="_pnlAddressOnPostCode" runat="server" Style="display: block; background-color: #ffffff">
    <table width="100%">
        <tr>
            <td>
                <div id="MapControl">
                    <asp:Button ID="hiddenTargetControlForModalPopup" runat="server" Style="display: none" />
                    <div>
                        <p style="text-align: center">
                            <table border="0" width="100%">
                                <tr>
                                    <td align="left" class="sectionHeader">
                                        Online Address Verification
                                    </td>
                                </tr>
                                                            <tr ID="_trServiceError" runat="server" style="display: none;">
                                                                <td align="left">
                                                                    <asp:Label ID="_lblServiceError" runat="server" CssClass="errorMessage"></asp:Label>
                                                                </td>
                                                            </tr>
                                <tr>
                                    <td>
                                        <asp:ListBox ID="_listAddress" runat="server" Height="100px" Width="100%">
                                            </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <table width="100%">
                                            <tr>
                                                <td align="left" class="boldTxt" style="width: 70%">
                                                    Please choose address and press OK to select.<br />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;Double click on items to narrow down results.
                                                </td>
                                                <td align="right">
                                                    <div class="button" style="text-align: center;">
                                                      <%--<input id="OkButton" runat="server" class="button" type="button" value="OK" />--%>
                                                      <asp:Button runat="server" OnClick="OkButton_onclick" ID="btnOK" Text="OK" />
                                                    </div>
                                                </td>
                                                <td align="right">
                                                    <div class="button" style="text-align: center;">
                                                        <input id="CancelButton" runat="server" class="button" type="button" value="Cancel" />
                                                    </div>
                                                </td>
                                                <caption>
                                                    `
                                                </caption>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </p>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>

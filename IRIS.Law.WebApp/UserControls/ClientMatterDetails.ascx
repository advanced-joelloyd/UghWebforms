<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientMatterDetails.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ClientMatterDetails" %>
<%@ Register TagPrefix="CS" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
<table width="100%" border="0">
    <tr>
        <td class="boldTxt" style="width: 100px; vertical-align:top">
            Reference
        </td>
        <td colspan="1" style="width: 140px;">
            <CS:ClientSearch ID="_clientSearch" runat="Server" DisplayPopup="true" DisplayMattersForClientGridview = "true" OnClientReferenceChanged="_clientSearch_ClientReferenceChanged" />
           
            <asp:TextBox runat="server" ID="_txtReference" Visible="false"></asp:TextBox>
        </td>
        <td valign="top">
            <asp:UpdatePanel ID="_updEditMatter11" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td valign="top" style="width:180px;">
                                <asp:DropDownList ID="_ddlMatterReference" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_ddlMatterReference_SelectedIndexChanged"
                                    onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();">
                                </asp:DropDownList>
                                <asp:Image runat="server" AlternateText="This matter is archived" ID="_imgMatterArchieved"
                                    ImageUrl="~/Images/archived.png" Visible="false" />
                                <asp:RequiredFieldValidator ID="_rfvMatter" runat="server" ErrorMessage="Matter is mandatory"
                                    Display="None" ControlToValidate="_ddlMatterReference"></asp:RequiredFieldValidator>
                            </td>
                            <td valign="top"> 
                                <div id="ResetBtnContainer">
                                <div id="_divResetCliMatter" runat="server" class="button" style="text-align: center;"
                                    visible="false">
                                    <asp:Button ID="_btnResetClientMatter" runat="server" Text="Reset" OnClick="_btnResetClientMatter_Click" CausesValidation="false" />
                                </div></div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="_clientSearch" EventName="ClientReferenceChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 100px;">
            Description
        </td>
        <td class="labelValue" colspan="2">
            <asp:UpdatePanel ID="_updEditMatter33" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
                    <asp:LinkButton class="link" ID="_lnkMatter" CausesValidation="false" runat="server" onclick="_lnkMatter_Click"></asp:LinkButton>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="_clientSearch" EventName="ClientReferenceChanged" />
                    <asp:AsyncPostBackTrigger ControlID="_ddlMatterReference" />
                    <asp:AsyncPostBackTrigger ControlID="_btnResetClientMatter" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 100px;">
            Client
        </td>
        <td>
            <asp:UpdatePanel ID="_updEditMatter22" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
                    <asp:LinkButton class="link" ID="_linkClientName" CausesValidation="false" runat="server" 
                        onclick="_linkClientName_Click"></asp:LinkButton>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="_clientSearch" EventName="ClientReferenceChanged" />
                    <asp:AsyncPostBackTrigger ControlID="_ddlMatterReference" />
                    <asp:AsyncPostBackTrigger ControlID="_btnResetClientMatter" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td align="right">
            <%--<table width="100%" id="_tblModuleLinks" runat="server" visible="false">
                <tr>
                    <td>
                        <a class="link" href="#">Conveyancing</a> <a class="link" href="#">Wills</a> <a class="link"
                            href="#">Probate</a> <a class="link" href="#">Personal Injury</a> <a class="link"
                                href="#">Family</a>
                    </td>
                </tr>
            </table>--%>
        </td>
    </tr>
</table>

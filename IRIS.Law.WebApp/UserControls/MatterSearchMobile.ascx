<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatterSearchMobile.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.MatterSearchMobile" %>
<div id="bubble_tooltip" class="errorMessageTooltip">
</div>
<table width="350px">
    <tr>
        <td class="boldTxtMobile" style="width: 100px;">
            Fee Earner :
        </td>
        <td>
            <asp:DropDownList ID="_ddlFeeEarner" runat="server" Width="195px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="boldTxtMobile" style="width: 100px;">
            Client Search :
        </td>
        <td>
            <table cellpadding="0" cellspacing="0" width="200px">
            <tr>
                <td><asp:TextBox ID="_txtSearch" runat="server" Width="170px" Text="All Clients"></asp:TextBox></td>
                <td style="width:10px"></td>
                <td align="left" valign="top"><div class="searchButtonBg">
                    <asp:ImageButton AlternateText="Client Search" ID="_imgBtnSearch" runat="server"
                        ImageUrl="../Images/PNGs/searchButton.png" ToolTip="Client Search"
                        CausesValidation="false" OnClick="_imgBtnSearch_Click"/></div></td>
            </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="boldTxtMobile" style="width: 100px;">
            Client Ref :
        </td>
        <td>
            <table cellpadding="0" cellspacing="0">
            <tr>
            <td>
                    <asp:DropDownList ID="_ddlClients" runat="server" onmousemove="showToolTip(event);return false;"
                       onmouseout="hideToolTip();" OnSelectedIndexChanged="_ddlClients_SelectedIndexChanged"
                       AutoPostBack="true" Width="195px">
                 </asp:DropDownList>
                 <asp:RequiredFieldValidator ID="_rfvClientReference" runat="server" ErrorMessage="Client is mandatory"
                       Display="None" ControlToValidate="_ddlClients"></asp:RequiredFieldValidator>
                 <span class="mandatoryField" id="_mfClientReference" runat="server">&nbsp;*</span>
                
            </td>
             <td style="width:2px"></td>
             <td align="left" valign="top"><div class="searchButtonBg">
                 <asp:ImageButton AlternateText="Matter Search" ID="_imgbtnMatterSearch" runat="server"
                 ImageUrl="../Images/PNGs/searchButton.png" ToolTip="Matter Search" 
                     CausesValidation="false" EnableTheming="false" 
                     OnClick="_imgBtnMatterSearch_Click"/></div></td>
              </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="boldTxtMobile" style="width: 100px;">
            Matter Ref :
        </td>
        <td>
            <asp:DropDownList ID="_ddlClientMatters" runat="server" onmousemove="showToolTip(event);return false;"
                onmouseout="hideToolTip();" Width="195px" AutoPostBack="true" OnSelectedIndexChanged="_ddlClientMatters_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="_rfvMatter" runat="server" ErrorMessage="Matter is mandatory"
                Display="None" ControlToValidate="_ddlClientMatters"></asp:RequiredFieldValidator>
            <span class="mandatoryField" id="_mfMatterReference" runat="server">&nbsp;*</span>
        </td>
    </tr>
</table>

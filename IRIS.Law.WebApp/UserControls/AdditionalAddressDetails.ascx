<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdditionalAddressDetails.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.AdditionalAddressDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<table width="100%">
    <tr id="_trHomeTel" runat="server">
        <td class="boldTxt" style="width: 150px;">
            Home Telephone
        </td>
        <td>
            <asp:TextBox ID="_txtHomeTelephone" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Work Telephone 1
        </td>
        <td>
            <asp:TextBox ID="_txtWorkTel1" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Work Telephone 2
        </td>
        <td>
            <asp:TextBox ID="_txtWorkTel2" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="_trDDI" runat="server">
        <td class="boldTxt" style="width: 150px;">
            DDI
        </td>
        <td>
            <asp:TextBox ID="_txtDDI" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Mobile Telephone 1
        </td>
        <td>
            <asp:TextBox ID="_txtMob1" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Mobile Telephone 2
        </td>
        <td>
            <asp:TextBox ID="_txtMob2" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Fax
        </td>
        <td>
            <asp:TextBox ID="_txtFax" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="_trHomeEmail" runat="server">
        <td class="boldTxt" style="width: 150px;">
            Home E-Mail Address
        </td>
        <td>
            <asp:TextBox ID="_txtHomeEmail" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            Work E-Mail Address
        </td>
        <td>
            <asp:TextBox ID="_txtWorkEmail" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="boldTxt" style="width: 150px;">
            URL
        </td>
        <td>
            <asp:TextBox ID="_txtURL" runat="server"></asp:TextBox>
        </td>
    </tr>
</table>

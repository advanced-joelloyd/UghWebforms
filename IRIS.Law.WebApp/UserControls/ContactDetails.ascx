<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactDetails.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ContactDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<table width="100%" runat="server" id="_tblContactDetails">
   <%-- <tr id="_trContactDetailsHeader" runat="server">
        <td colspan="2">
            <ajaxtoolkit:roundedcornersextender id="RoundedCornersExtender3" runat="server" targetcontrolid="_pnlContactHeader"
                radius="5" corners="All" bordercolor="#96a7b8" />
            <asp:Panel ID="_pnlContactHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                <asp:Label ID="_lblContactHeader" runat="server" Text="Contact Details"></asp:Label>
            </asp:Panel>
        </td>
    </tr>--%>
    <tr id="_trContactSurname" runat="server">
        <td class="boldTxt" style="width: 150px;">
            <asp:Label ID="_lblContactSurname" runat="server" Text="Surname"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="_txtContactSurname" runat="server" onmousemove="showToolTip(event);return false;"
                onmouseout="hideToolTip();"></asp:TextBox>
            <asp:RequiredFieldValidator ID="_rfvContactSurname" runat="server" ControlToValidate="_txtContactSurname"
                Display="None" ErrorMessage="Surname is mandatory"></asp:RequiredFieldValidator>
            <span class="mandatoryField">*</span>
        </td>
    </tr>
    <tr id="_trContactForename" runat="server">
        <td class="boldTxt" style="width: 150px;">
            <asp:Label ID="_lblContactForename" runat="server" Text="Forename"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="_txtContactForename" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="_trContactTitle" runat="server">
        <td class="boldTxt" style="width: 150px;">
            <asp:Label ID="_lblContactTitle" runat="server" Text="Title"></asp:Label>
        </td>
        <td>
            <asp:DropDownList ID="_ddlContactTitle" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="_trContactSex" runat="server">
        <td class="boldTxt" style="width: 150px;">
            <asp:Label ID="_lblContactSex" runat="server" Text="Sex"></asp:Label>
        </td>
        <td>
            <asp:DropDownList ID="_ddlContactSex" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="_trContactPosition" runat="server">
        <td class="boldTxt" style="width: 150px;">
            <asp:Label ID="_lblContactPosition" runat="server" Text="Position"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="_txtContactPosition" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="_trContactDescription" runat="server">
        <td class="boldTxt" valign="top" style="width: 150px; padding-top: 5px;">
            <asp:Label ID="_lblContactDescription" runat="server" Text="Description"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="_txtContactDescription" runat="server" TextMode="MultiLine" Height="120px"
                Width="95%"></asp:TextBox>
        </td>
    </tr>
</table>

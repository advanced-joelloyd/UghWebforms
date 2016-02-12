<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="SearchContact.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Contact.SearchContact"
    Title="Search Contact" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="ContactSearch" TagName="ContactSearch" Src="~/UserControls/ContactSearch.ascx" %>
<%@ Register TagPrefix="ServiceSearch" TagName="ServiceSearch" Src="~/UserControls/ServiceSearch.ascx" %>
<%@ Register TagPrefix="ClientSearch" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
<%@ Register TagPrefix="ContactDetails" TagName="ViewContactDetails" Src="~/UserControls/ViewContactDetails.ascx" %>
<asp:Content ID="_contentSearchContact" ContentPlaceHolderID="_cphMain" runat="server">

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

        function SetSearchOnContactType() {
            if ($("#<%=_ddlContactType.ClientID %>").val() == "General Contact") {
                $("#<%=_trContactSearch.ClientID%>").css("display", "");
                $("#<%=_trServiceSearch.ClientID%>").css("display", "none");
            }
            else {
                $("#<%=_trContactSearch.ClientID%>").css("display", "none");
                $("#<%=_trServiceSearch.ClientID%>").css("display", "");
            }
        }

        function ResetControls() {
            $("#<%=_ddlIndustry.ClientID%>").val("");
            $("#<%=_grdSearchServiceList.ClientID%>").css("display", "none");
            $("#<%=_grdSearchServiceContactList.ClientID%>").css("display", "none");           
            return false;
        }

    </script>

    <table width="100%" border="0">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updContactDetails" runat="server" UpdateMode="Conditional"
                    ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <ContactDetails:ViewContactDetails runat="server" ID="_ucViewContactDetails"></ContactDetails:ViewContactDetails>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_contactSearch" EventName="ContactSelected" />
                        <asp:AsyncPostBackTrigger ControlID="_grdSearchServiceContactList" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="sectionHeader">
                Search Contact
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                        <asp:HiddenField ID="_hdnServiceName" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="boldTxt" style="width: 100px;">
                Contact Type
            </td>
            <td>
                <asp:DropDownList ID="_ddlContactType" runat="server" onchange="SetSearchOnContactType();">
                    <asp:ListItem Selected="True" Text="General Contact" Value="General Contact"></asp:ListItem>
                    <asp:ListItem Text="Service" Value="Service"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr id="_trContactSearch" runat="server">
            <td>
                <ContactSearch:ContactSearch ID="_contactSearch" runat="server" DisplayPopup="false"
                    DisplayContactNameTextbox="false" DisplayContactAsLink="true" DisplayServiceNameAsLinkable="false"
                    OnContactSelected="_contactSearch_ContactSelected" />
            </td>
        </tr>
        <tr id="_trServiceSearch" runat="server" style="display: none;">
            <td width="100%">
                <asp:Panel ID="_pnlServiceSearch" runat="server" DefaultButton="_btnSearch">
                    <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                        <tr>
                            <td style="width: 100%">
                                <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                    <tr>
                                        <td class="boldTxt" style="width: 125px;">
                                            Industry
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlIndustry" runat="server" SkinID="Large">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="padding-right: 50px;">
                                <table width="100%">
                                    <tr>
                                        <td align="right" style="padding-right: 15px;">
                                            <table>
                                                <tr>
                                                    <td align="right">
                                                        <div class="button" style="text-align: center;">
                                                            <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetControls();" />
                                                        </div>
                                                    </td>
                                                    <td align="right">
                                                        <div class="button" style="text-align: center;">
                                                            <asp:Button ID="_btnSearch" runat="server" CausesValidation="True" Text="Search"
                                                                OnClick="_btnSearch_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td>
                                
                                <asp:UpdatePanel ID="Update_pnlServiceSearch" runat="server" UpdateMode="Conditional"
                                    >
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" align="left">
                                                    Service
                                                </td>
                                            </tr>
                                        </table>
                                        <div id="_grdviewDivHeight" runat="server" style="height: 200px; overflow: auto;">
                                            <asp:GridView ID="_grdSearchServiceList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                BorderWidth="0" GridLines="None" Width="97.5%" AllowSorting="true" OnRowCommand="_grdSearchServiceList_RowCommand"
                                                CssClass="successMessage" OnRowDataBound="_grdSearchServiceList_RowDataBound"
                                                DataKeyNames="Id">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server" class="link" ID="_lnkSelectService" CommandName="contact"
                                                                CausesValidation="false" Text="List Contacts"></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Service Name" SortExpression="OrgName">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblServiceName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'
                                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Address 1"  SortExpression="AddressHouseName">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblAddress" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressHouseName")%>'
                                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "AddressHouseName")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Address Street No" Visible="false"  SortExpression="AddressStreetNo">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblAddressStreetNo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressStreetNo")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Address Line 1" Visible="false" SortExpression="AddressLine1">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblAddressLine1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressLine1")%>'
                                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "AddressLine1")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Town"  SortExpression="AddressTown">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblTown" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressTown")%>'
                                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "AddressTown")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="PostCode"  SortExpression="AddressPostcode">
                                                        <ItemTemplate>
                                                            <asp:Label ID="_lblPostcode" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressPostcode")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="_odsSearchService" runat="server" SelectMethod="SearchService"
                                                TypeName="IRIS.Law.WebApp.Pages.Contact.SearchContact" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                SelectCountMethod="GetServiceRowsCount" StartRowIndexParameterName="startRow"
                                                OnSelected="_odsSearchService_Selected" SortParameterName="sortBy">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="_ddlIndustry" Name="industry" PropertyName="Text" />
                                                    <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                                                        Type="Boolean" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </div>
                                        <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchServiceList" EventName="PageIndexChanging" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="Update_pnlServiceContactSearch" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%" id="_tableServiceContact" runat="server">
                                            <tr>
                                                <td class="sectionHeader" align="left">
                                                    Service Contacts
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="_grdviewContactDivHeight" runat="server" style="height: 150px; overflow: auto;">
                                                        <asp:GridView ID="_grdSearchServiceContactList" runat="server" AllowPaging="true"
                                                            AutoGenerateColumns="false" BorderWidth="0" AllowSorting="true" GridLines="None" Width="97.5%" CssClass="successMessage"
                                                            OnRowDataBound="_grdSearchServiceContactList_RowDataBound" DataKeyNames="MemberId"
                                                            OnRowCommand="_grdSearchServiceContactList_RowCommand">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Name" SortExpression="PersonName">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" class="link" ID="_lnkContactName" CommandName="select"
                                                                            CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "PersonName")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PersonName")%>'></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Title" SortExpression="PersonTitle">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblPersonTitle" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PersonTitle")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Surname" SortExpression="PersonSurname">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblPersonSurname" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PersonSurname")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PersonSurname")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Position" SortExpression="AssocDescription">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblPosition" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Position")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Position")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="25%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <asp:HiddenField ID="_hdnRefreshServiceContact" runat="server" Value="true" />
                                                        <asp:HiddenField ID="_hdnServiceOrgId" runat="server" />
                                                        <asp:ObjectDataSource ID="_odsServiceContactSearch" runat="server" SelectMethod="SearchContactService"
                                                            TypeName="IRIS.Law.WebApp.Contact.SearchContact" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                            SelectCountMethod="GetServiceContactRowsCount" StartRowIndexParameterName="startRow"
                                                            OnSelected="_odsSearchServiceContact_Selected" SortParameterName="sortBy">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="_hdnServiceOrgId" Name="serviceOrgId" PropertyName="Value" />
                                                                <asp:ControlParameter ControlID="_hdnRefreshServiceContact" Name="forceRefresh" PropertyName="Value"
                                                                    Type="Boolean" />
                                                            </SelectParameters>
                                                        </asp:ObjectDataSource>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchServiceList" EventName="RowCommand" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceSearch.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ServiceSearch" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

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

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_initializeRequest(InitializeRequest);
    prm.add_endRequest(EndRequest);

    function InitializeRequest(sender, args) {
        if (prm.get_isInAsyncPostBack())
            args.set_cancel(true);
        //disable the search button to prevent the user from clicking multiple times 
        //while the request is being processed
        $("#<%=_btnSearch.ClientID %>").attr("disabled", true);
    }
    function EndRequest(sender, args) {
        $("#<%=_btnSearch.ClientID %>").attr("disabled", false);
    }

    function ResetControls() {
        $("#<%=_ddlIndustry.ClientID%>").val("");
        $("#<%=_grdSearchServiceList.ClientID%>").css("display", "none");
        $("#<%=_grdSearchServiceContactList.ClientID%>").css("display", "none");
        Page_ClientValidate();
        return false;
    }

    function CancelPopupClick() {
        return false;
    }

    function CancelSearchServicePopup() {
        $find('_modalpopupServiceSearchBehavior').hide();
    }
</script>

<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div style="float: left;">
                <asp:UpdatePanel ID="asd" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="_hdnServiceName" runat="server" />
                        <asp:TextBox ID="_txtService" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                            onmouseout="hideToolTip();"></asp:TextBox>
                        <span id="_spnMandatory" runat="server" class="mandatoryField">*</span> <asp:ImageButton
                            AlternateText="Service Search" ID="_imgBtnSearch" runat="server" ImageUrl="~/Images/PNGs/searchButton.png"
                            ToolTip="Service Search" SkinID="SearchImageIcon" Height="21px" CausesValidation="false"
                            OnClick="_imgBtnSearch_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="float: left;" id="_pnlValidation" runat="server">
                <asp:RequiredFieldValidator ID="_rfvServiceReference" runat="server" ErrorMessage="Service is mandatory"
                    Display="None" ControlToValidate="_txtService"></asp:RequiredFieldValidator>
            </div>
            <input id="_btnDummy" runat="server" type="button" value="." style="height: 1px;
                width: 1px; display: none;" disabled="disabled" />
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupServiceSearch" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlServiceSearch" OnCancelScript="javascript:CancelPopupClick();"
                TargetControlID="_btnDummy" BehaviorID="_modalpopupServiceSearchBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:Panel ID="_pnlServiceSearch" runat="server" Style="background-color: #ffffff"
                Width="800px">
                <table width="100%">
                    <tr id="_trCloseLink" runat="server">
                        <td align="right">
                            <a id="linkClose" onclick="CancelSearchServicePopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="left">
                            <asp:UpdatePanel ID="_updPnlErrorMessage" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                                </ContentTemplate>
                               
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left">
                            Search Service
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                            <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch">
                                <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                                    <tr>
                                        <td style="width: 100%">
                                            <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                                <tr>
                                                    <td class="boldTxt" style="width: 125px;">
                                                        Industry
                                                    </td>
                                                    <td>
                                                        <asp:UpdatePanel ID="_updPnlIndustry" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:DropDownList ID="_ddlIndustry" runat="server" SkinID="Large">
                                                                </asp:DropDownList>
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="_imgBtnSearch" EventName="Click" />
                                                            </Triggers>
                                                        </asp:UpdatePanel>
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
                                                                        <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClick="_btnReset_Click"
                                                                            OnClientClick="return ResetControls();" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                        <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;">
                                                                        <asp:Button ID="_btnSearch" runat="server" CausesValidation="True" OnClick="_btnSearch_Click"
                                                                            Text="Search" ValidationGroup="ClientSearch" />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:UpdateProgress ID="_updateProgressServiceSearch" runat="server" AssociatedUpdatePanelID="Update_pnlServiceSearch">
                                <ProgressTemplate>
                                    <div class="textBox">
                                        <img id="_imgServiceSearchLoadIndicator" runat="server" alt="" src="~/Images/indicator.gif" />&nbsp;Loading...
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
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
                                    <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                                        <asp:GridView ID="_grdSearchServiceList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                            BorderWidth="0" GridLines="None" Width="97.5%" AllowSorting="true" OnRowCommand="_grdSearchServiceList_RowCommand"
                                            CssClass="successMessage" OnRowDataBound="_grdSearchServiceList_RowDataBound"
                                            DataKeyNames="Id">
                                            <Columns>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" class="link" ID="_lnkSelectService" CommandName="contact"
                                                            CausesValidation="false" Text="List Contacts"></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Service Name" SortExpression="OrgName">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" class="link" ID="_lnkServiceName" CommandName="select"
                                                            CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Address 1" SortExpression="AddressHouseName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblAddress" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressHouseName")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "AddressHouseName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="35%" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Address Street No" Visible="false" SortExpression="AddressStreetNo">
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
                                                <asp:TemplateField HeaderText="Town" SortExpression="AddressTown">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblTown" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressTown")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "AddressTown")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="PostCode" SortExpression="AddressPostcode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblPostcode" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AddressPostcode")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:ObjectDataSource ID="_odsSearchService" runat="server" SelectMethod="SearchService"
                                            TypeName="IRIS.Law.WebApp.UserControls.ServiceSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                            SelectCountMethod="GetServiceRowsCount" SortParameterName="sortBy" StartRowIndexParameterName="startRow"
                                            OnSelected="_odsSearchService_Selected">
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
                                    <asp:AsyncPostBackTrigger ControlID="_imgBtnSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="Update_pnlServiceContactSearch" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%" id="_tableServiceContact" runat="server" style="display: none">
                                        <tr>
                                            <td class="sectionHeader" align="left">
                                                Service Contacts
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="_grdviewContactDivHeight" runat="server" style="height: 150px; overflow: auto;">
                                                    <asp:GridView ID="_grdSearchServiceContactList" runat="server" AllowPaging="true"
                                                        AutoGenerateColumns="false" BorderWidth="0" AllowSorting="true" GridLines="None" Width="97.5%" OnRowCommand="_grdSearchServiceContactList_RowCommand"
                                                        CssClass="successMessage" OnRowDataBound="_grdSearchServiceContactList_RowDataBound"
                                                        DataKeyNames="MemberId">
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
                                                        TypeName="IRIS.Law.WebApp.UserControls.ServiceSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                        SelectCountMethod="GetServiceContactRowsCount" SortParameterName="sortBy" StartRowIndexParameterName="startRow"
                                                        OnSelected="_odsSearchServiceContact_Selected">
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
                                    <asp:AsyncPostBackTrigger ControlID="_imgBtnSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>

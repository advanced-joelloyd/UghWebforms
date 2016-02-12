<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewTimeLedger.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewTimeLedger"
    Title="View Time Ledger" %>

<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentTimeLedger" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        var browser = navigator.appName;
        //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
        //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
        //on box-items. This is not yet implemented in IE so round the corners using javascript
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }
        
    </script>

    <asp:UpdateProgress ID="_updateTimeLedgerProgressMatter" runat="server">
        <ProgressTemplate>
            <div class="textBox" runat="server" id="_divProgressMatter">
                <img id="_imgTimeLedgerLoadIndicator" src="~/Images/indicator.gif" runat="server"
                    alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" border="0">
        <tr>
            <td class="sectionHeader" colspan="2">
              
              Time Ledger
              
             </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Matter Details</asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <CliMat:ClientMatterDetails runat="server" ID="_cliMatDetails" OnMatterChanged="_cliMatDetails_MatterChanged" />
            </td>
            <td>
                <asp:UpdatePanel ID="_updPanelFinancialInfo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="3" class="sectionHeader">
                                    Financial Info
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt">
                                    Office
                                </td>
                                <td class="boldTxt">
                                    Client
                                </td>
                                <td class="boldTxt">
                                    Deposit
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="_txtOffice" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtClient" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDeposit" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnTimeTransactionsRefresh" runat="server" Value="true" />
                </div>
            </td>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnWriteOffTimeTransactionsRefresh" runat="server" Value="true" />
                </div>
            </td>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnWriteOffReversalTimeTransactionsRefresh" runat="server"
                        Value="true" />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ajaxToolkit:TabContainer ID="_tcTimeLedger" runat="server" CssClass="ajax__tab_xp2"
                    Width="100%" ActiveTabIndex="0" Height="400px">
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlTimeLedger" HeaderText="Time">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelTimeLedger" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%" border="0">
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Time Status
                                            </td>
                                            <td style="width: 790px;">
                                                <asp:DropDownList ID="_ddlTimeStatus" AutoPostBack="true" OnSelectedIndexChanged="_ddlTimeStatus_SelectedIndexChanged"
                                                    runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div style="height: 320px">
                                                    <asp:GridView ID="_grdTimeTransactions" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                        BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="TimeId" Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblTimeId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TimeId")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Date">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("TimeDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Time Type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblTimeType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TimeType")%>'
                                                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "TimeType")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Earner">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'
                                                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Time">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblTime" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Time")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Cost">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblCost" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cost")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Cost Balance">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblCostBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CostBalance")%>'
                                                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "CostBalance")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Charge">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblCharge" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Charge")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Charge Balance">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="_lblBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChargeBalance")%>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                <HeaderStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt">
                                                <asp:Label ID="_lblTimeStatus" Text="All" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtTimeStatusTime" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                                <asp:TextBox ID="_txtTimeStatusCost" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                                &nbsp;&nbsp;<asp:TextBox ID="_txtTimeStatusBalance" Text="0.00" ReadOnly="true" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr valign="bottom">
                                            <td class="boldTxt">
                                                Total
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtTotalTime" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                                <asp:TextBox ID="_txtTotalCost" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                                &nbsp;&nbsp;<asp:TextBox ID="_txtTotalBalance" Text="0.00" ReadOnly="true" SkinID="NumericTextBox"
                                                    runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_ddlTimeStatus" />
                                    <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsTimeTransactions" runat="server" SelectMethod="LoadTimeLedger"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewTimeLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetAllTimeTransactionsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsTimeTransaction_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnTimeTransactionsRefresh" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                    <asp:ControlParameter ControlID="_ddlTimeStatus" Name="timeFilter" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlTimeWriteOff" HeaderText="Write Offs">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelWriteOffTimeTransactions" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdWriteOffTimeTransactions" runat="server" AllowPaging="true"
                                                    OnRowDataBound="_grdWriteOffTimeTransactions_RowDataBound" AutoGenerateColumns="false"
                                                    BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="PostingId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPostingId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("PostingDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Reference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingDescription")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingDescription")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Time">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Time")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Charge">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCharge" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Charge")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "Charge")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Cost">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCost" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cost")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsWriteOffTimeTransactions" runat="server" SelectMethod="LoadWriteOffTimeTransactions"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewTimeLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetWriteOffTimeTransactionsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsWriteOffTimeTransaction_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnWriteOffTimeTransactionsRefresh" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlWriteOffReversalTimeTransactions" HeaderText="Write Offs/Reversals">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelWriteOffReversalTimeTransactions" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdWriteOffReversalTimeTransactions" runat="server" AllowPaging="true"
                                                    OnRowDataBound="_grdWriteOffReversalTimeTransactions_RowDataBound" AutoGenerateColumns="false"
                                                    BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="PostingId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPostingId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("PostingDate","{0:dd/MM/yyyy}") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Reference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingDescription")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingDescription")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Time">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Time")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Charge">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCharge" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Charge")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Cost">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCost" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Cost")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsWriteOffReversalTimeTransactions" runat="server" SelectMethod="LoadWriteOffReversalTimeTransactions"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewTimeLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetWriteOffReversalTimeTransactionsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsWriteOffReversalTimeTransaction_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnWriteOffReversalTimeTransactionsRefresh" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>
            </td>
        </tr>
    </table>
</asp:Content>

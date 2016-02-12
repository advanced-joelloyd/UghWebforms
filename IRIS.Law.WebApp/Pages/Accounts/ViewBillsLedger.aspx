<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewBillsLedger.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewBillsLedger"
    Title="View Bills Ledger" %>

<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentDepositBills" ContentPlaceHolderID="_cphMain" runat="server">

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

    <asp:UpdateProgress ID="_updateProgressMatter" runat="server">
        <ProgressTemplate>
            <div class="textBox" runat="server" id="_divProgressMatter">
                <img id="_imgBillsLedgerLoadIndicator" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" border="0">
        <tr>
            <td class="sectionHeader" colspan="2">
               <%if (Request.QueryString["mybill"] == "true")
                 { %>
               My Bill
               <%}
                 else
                 {%>
               Bills Ledger
               <%} %>
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
                                    <asp:TextBox ID="_txtOffice" ReadOnly="true" SkinID="NumericTextBox" Text="0.00"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtClient" ReadOnly="true" SkinID="NumericTextBox" Text="0.00"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDeposit" ReadOnly="true" SkinID="NumericTextBox" Text="0.00"
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
                    <asp:HiddenField ID="_hdnAllBillsRefresh" runat="server" Value="true" />
                </div>
            </td>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnUnclearedBillsRefresh" runat="server" Value="true" />
                </div>
            </td>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnWriteOffBillsRefresh" runat="server" Value="true" />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ajaxToolkit:TabContainer ID="_tcBillsLedger" runat="server" CssClass="ajax__tab_xp2"
                    Width="100%" ActiveTabIndex="0" Height="310px">
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlAllBills" HeaderText="All Bills">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelAllBills" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdAllBills" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    OnRowDataBound="_grdAllBills_RowDataBound" BorderWidth="0" GridLines="None" Width="100%"
                                                    CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="PostingId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPostingId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("BillDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Reference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Debit">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDebit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Debit")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Credit">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCredit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Credit")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Paid">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPaid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Paid")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="O/S">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblOutstanding" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OutStanding")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Balance">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Balance")%>'></asp:Label>
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
                            <asp:ObjectDataSource ID="_odsAllBills" runat="server" SelectMethod="LoadAllBills"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewBillsLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetAllBillsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsAllBills_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnAllBillsRefresh" Name="forceRefresh" PropertyName="Value"
                                        Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlUnclearedBills" HeaderText="Uncleared Bills">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelUnclearedBills" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdUnclearedBills" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    OnRowDataBound="_grdUnclearedBills_RowDataBound" BorderWidth="0" GridLines="None"
                                                    Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="PostingId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPostingId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("BillDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Reference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Amount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Uncleared">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblUncleared" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Uncleared")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VAT">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblVAT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "VATAmount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Disbursement">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDisbursement" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Disbursements")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="20%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Costs">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCosts" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Costs")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Balance">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Balance")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
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
                            <asp:ObjectDataSource ID="_odsUnclearedBills" runat="server" SelectMethod="LoadUnclearedBills"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewBillsLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetUnclearedBillsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsUnclearedBills_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnUnclearedBillsRefresh" Name="forceRefresh" PropertyName="Value"
                                        Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlWriteOffs" HeaderText="Write Offs">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelWriteOffs" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdWriteOffs" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    OnRowDataBound="_grdWriteOffs_RowDataBound" BorderWidth="0" GridLines="None"
                                                    Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="PostingId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPostingIdDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("BillDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Reference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillReference")%>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Type">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BillType")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Debit">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDebit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Debit")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Credit">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblCredit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Credit")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "Credit")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Paid">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblPaid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Paid")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "Paid")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="O/S">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblOutstanding" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OutStanding")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Balance">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Balance")%>'></asp:Label>
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
                            <asp:ObjectDataSource ID="_odsWriteOffBills" runat="server" SelectMethod="LoadWriteOffBills"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewBillsLedger" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetWriteOffBillsRowsCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsWriteOffBills_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnWriteOffBillsRefresh" Name="forceRefresh" PropertyName="Value"
                                        Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>
            </td>
        </tr>
    </table>
</asp:Content>

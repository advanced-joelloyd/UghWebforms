<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewDepositBills.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewDepositBills"
    Title="View Deposit Bills" %>

<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentDepositBills" ContentPlaceHolderID="_cphMain" runat="server">
    <asp:UpdateProgress ID="_updateProgressMatter" runat="server">
        <ProgressTemplate>
            <div class="textBox" runat="server" id="_divProgressMatter">
                <img id="_imgDepositBillsLoadIndicator" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" border="0">
        <tr>
            <td class="sectionHeader" colspan="2">
              
              Deposit Ledger
              
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
            <td colspan="2">
                <asp:UpdatePanel ID="_updPanelDepositBills" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="_grdDepositBills" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            OnRowDataBound="_grdDepositBills_RowDataBound" BorderWidth="0" GridLines="None"
                            Width="100%" CssClass="successMessage" EmptyDataText="There are no results to display.">
                            <Columns>
                                <asp:TemplateField HeaderText="Date" Visible="false">
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
                                        <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingType")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingType")%>'>
                                        </asp:Label>
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
                                <asp:TemplateField HeaderText="Bank">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblBank" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingBankRef")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingBank")%>'></asp:Label>
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
                                <asp:TemplateField HeaderText="Balance">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblBalance" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Balance")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div>
                            <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="_odsDepositBalances" runat="server" SelectMethod="LoadDepositBalancesDetails"
                    TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewDepositBills" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetDepositBalancesRowsCount" StartRowIndexParameterName="startRow"
                    OnSelected="_odsDepositBalances_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

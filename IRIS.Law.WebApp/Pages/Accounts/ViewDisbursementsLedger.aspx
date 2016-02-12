<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewDisbursementsLedger.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewDisbursementsLedger"
    Title="View Disbursements Ledger" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentDisbursementLedger" ContentPlaceHolderID="_cphMain" runat="server">
    <asp:UpdateProgress ID="_updateProgressMatter" runat="server">
        <ProgressTemplate>
            <div class="textBox" runat="server" id="_divProgressMatter">
                <img id="_imgDisbursementLoadIndicator" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" border="0">
        <tr>
            <td class="sectionHeader" colspan="2">
              
              Disbursements Ledger
              
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
                <asp:UpdatePanel ID="_updPanelDisbursementsLedger" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="_grdDisbursementsLedger" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            OnRowDataBound="_grdDisbursementsLedger_RowDataBound" BorderWidth="0" GridLines="None"
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
                                        <asp:Label ID="_lblReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingReference")%>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Disbursement">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDisbursementType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingType")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "PostingType")%>'> </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"PostingDescription")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem,"PostingDescription")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="VAT">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblVAT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingVAT")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Paid">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblPaid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PostingPaid")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"Amount")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Claimed">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblClaimed" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Claimed")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="UnBilled">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblUnBilled" runat="server" Text='<%#DataBinder.Eval(Container.DataItem,"BillingStatus")%>'></asp:Label>
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
                <asp:ObjectDataSource ID="_odsDisbursements" runat="server" SelectMethod="LoadDisbursements"
                    TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewDisbursementsLedger" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetDisbursementsRowsCount"
                    StartRowIndexParameterName="startRow" OnSelected="_odsLoadDisbursements_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

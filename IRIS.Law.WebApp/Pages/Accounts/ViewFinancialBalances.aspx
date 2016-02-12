<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewFinancialBalances.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewFinancialBalances"
    Title="View Financial Balances" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentFinancialBalances" ContentPlaceHolderID="_cphMain" runat="server">
    <asp:UpdateProgress ID="_updateProgressMatter" runat="server">
        <ProgressTemplate>
            <div class="textBox" runat="server" id="_divProgressMatter">
                <img id="_imgFinancialInfoLoadIndicator" src="~/Images/indicator.gif" runat="server"
                    alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" border="0">
        <tr>
            <td class="sectionHeader" colspan="2">
              
              Financial Balances
              
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
            <td colspan="6">
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
                                    <asp:TextBox ID="_txtOfficeFinancialInfo" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtClientFinancialInfo" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDepositFinancialInfo" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
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
                <asp:UpdatePanel ID="_updPanelError" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%">
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    Balances
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Office
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtOffice" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Client
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtClient" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Deposit
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDeposit" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    W.I.P
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Time
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWIPTime" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Cost
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWIPCost" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Charge Out
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWIPChargeout" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    Anticipated
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Disbursements
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDisbursements" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Bills
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtAnticipatedBills" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    PF Claims
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtPFClaims" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    Time
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Time
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtTime" ReadOnly="true" Text="0.00" SkinID="NumericTextBox" runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Cost
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtTimeCost" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Charge Out
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtTimeChargeout" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    Cost
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Bills
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtCostBills" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Unbilled Disbs
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtUnbilledDisbursements" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                                <td class="boldTxt" style="width: 120px;">
                                    Unpaid Billed Disbs
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtUnpaidbilledDisbursements" ReadOnly="true" Text="0.00" SkinID="NumericTextBox"
                                        runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6" class="sectionHeader">
                                    Movement
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Last Bill
                                </td>
                                <td>
                                    <CC:CalendarControl ID="_ccLastBill" Enabled="false" runat="server" />
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Last Financial
                                </td>
                                <td>
                                    <CC:CalendarControl ID="_ccLastFinancial" Enabled="false" runat="server" />
                                </td>
                                <td class="boldTxt" style="width: 100px;">
                                    Last Time
                                </td>
                                <td>
                                    <CC:CalendarControl ID="_ccLastTime" Enabled="false" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

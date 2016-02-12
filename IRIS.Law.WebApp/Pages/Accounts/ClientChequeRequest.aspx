<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ClientChequeRequest.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ClientChequeRequest"
    Title="Add Client Cheque Request" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentAddClientChequeRequest" ContentPlaceHolderID="_cphMain"
    runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                        <asp:HiddenField ID="_hdnClientChequeRequestId" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="_btnReset" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="sectionHeader">
                <%if (Request.QueryString["edit"] == "true")
                  { %>
                Edit Client Cheque Request
                <%}
                  else
                  {%>
                Add Client Cheque Request
                <%} %>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Matter Details</asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <CliMat:ClientMatterDetails runat="server" EnableValidation="true" ID="_cliMatDetails"
                    OnMatterChanged="_cliMatDetails_MatterChanged" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <asp:Panel ID="_pnlTimeDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Client Electronic Cheque Request</asp:Panel>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="_updPnlAppointment" runat="server" UpdateMode="Conditional"
        ChildrenAsTriggers="false">
        <ContentTemplate>
            <table width="100%" border="0">
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Date
                    </td>
                    <td colspan="2">
                        <asp:UpdatePanel ID="_upPanelDate" runat="server">
                            <ContentTemplate>
                                <CC:CalendarControl ID="_ccPostDate" runat="server" EnableValidation="true" InvalidValueMessage="Valid Date Required"
                                    OnDateChanged="_ccPostDate_DateChanged" />
                                <span class="mandatoryField">*</span>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td class="boldTxt">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    Posting Period :
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="_uppnlPostingDate" runat="server">
                                        <ContentTemplate>
                                            <asp:Label ID="_lblPostingPeriod" runat="server"></asp:Label>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="_ccPostDate" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Description
                    </td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="_upPanelDescription" runat="server">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtDescription" runat="server" Style="width: 355px;" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();" onchange="SetPayee();" onblur="SetPayee();"></asp:TextBox>
                                <span class="mandatoryField">*</span>
                                <asp:RequiredFieldValidator ID="_rfvDescription" runat="server" ErrorMessage="Description is mandatory"
                                    Display="None" ControlToValidate="_txtDescription"></asp:RequiredFieldValidator>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Reference
                    </td>
                    <td style="width: 12%;">
                        <asp:UpdatePanel ID="_upPanelReference" runat="server">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtReference" runat="server" SkinID="Small" MaxLength="20" Width="130"
                                    onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();" AutoPostBack="true"
                                    OnTextChanged="_txtReference_TextChanged"></asp:TextBox>
                                <span class="mandatoryField">*</span>
                                <asp:RequiredFieldValidator ID="_rfvReference" runat="server" ErrorMessage="Reference is mandatory"
                                    Display="None" ControlToValidate="_txtReference"></asp:RequiredFieldValidator>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 5px">
                    </td>
                    <td>
                        <asp:UpdatePanel ID="_upPanelPayee" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="0" border="0" width="210px">
                                    <tr>
                                        <td class="boldTxt" style="width: 50px;">
                                            <asp:Label runat="server" ID="_lblPayee" Visible="false" Text="Payee"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtPayee" Visible="false" runat="server" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <span class="mandatoryField" visible="false" runat="server" id="_spanPayee">*</span>
                                        </td>
                                        <td>
                                            <asp:RequiredFieldValidator Enabled="false" ID="_rfvPayee" runat="server" ErrorMessage="Payee is mandatory"
                                                Display="None" ControlToValidate="_txtPayee"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Bank
                    </td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="_updPanelError" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="_ddlClientBank" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="_rfvClientBank" runat="server" ErrorMessage="Client Bank is mandatory"
                                    Display="None" ControlToValidate="_ddlClientBank"></asp:RequiredFieldValidator>
                                <span class="mandatoryField">*</span>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Debit/Credit
                    </td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="_updPanelDebitCredit" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList runat="server" ID="_ddlClientDebitCredit" AutoPostBack="true" OnSelectedIndexChanged="_ddlClientDebitCredit_SelectedIndexChanged">
                                    <asp:ListItem Text="Credit" Value="Credit"></asp:ListItem>
                                    <asp:ListItem Text="Debit" Value="Debit"></asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Amount
                    </td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="_updPanelAmount" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtAmount" runat="server" Text="0.00" MaxLength="11" onmousemove="showToolTip(event);return false;"
                                    OnTextChanged="_txtAmount_TextChanged" AutoPostBack="true" onmouseout="hideToolTip();" SkinID="NumericTextbox" onblur="ResetAmount();"></asp:TextBox>
                                <span class="mandatoryField">*</span>
                                <asp:RegularExpressionValidator ID="revtxtAmount" ControlToValidate="_txtAmount"
                                    runat="server" ErrorMessage="The format of the number is incorrect" Font-Size="X-Small"
                                    Font-Names="arial" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="_rfvAmount" runat="server" ErrorMessage="Amount is mandatory"
                                    Display="None" ControlToValidate="_txtAmount" InitialValue="0.00"></asp:RequiredFieldValidator>
                                <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtAmount" runat="server" TargetControlID="_txtAmount"
                                    FilterType="Custom, Numbers" ValidChars="." />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;" valign="top">
                        Clearance Type
                    </td>
                    <td valign="top">
                        <asp:UpdatePanel ID="_updPanelClearanceType" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList runat="server" ID="_dllClearanceType" AutoPostBack="true" OnSelectedIndexChanged="_dllClearanceType_SelectedIndexChanged">
                                </asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlClientDebitCredit" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td colspan="2">
                        <asp:UpdatePanel ID="_updPanelClearancedays" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td class="boldTxt">
                                            Clearance Days (Chq)
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="_txtClearanceDaysChq" Width="30" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();" onkeypress="return CheckNumeric(event);"></asp:TextBox>
                                            <span class="mandatoryField">*</span>
                                            <asp:RequiredFieldValidator ID="_rfvClearanceDaysChq" runat="server" ErrorMessage="Clearanace Days (Chq) is mandatory"
                                                Display="None" ControlToValidate="_txtClearanceDaysChq"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Clearance Days (Elec)
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="_txtClearanceDaysElec" Width="30" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();" onkeypress="return CheckNumeric(event);"></asp:TextBox>
                                            <span class="mandatoryField">*</span>
                                            <asp:RequiredFieldValidator ID="_rfvClearanceDaysElec" runat="server" ErrorMessage="Clearanace Days (Elec) is mandatory"
                                                Display="None" ControlToValidate="_txtClearanceDaysElec"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlClientDebitCredit" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="_dllClearanceType" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td class="boldTxt">
                        Print Cheque Request?
                    </td>
                    <td>
                        <asp:UpdatePanel ID="_updPanelPrintRequest" runat="server">
                            <ContentTemplate>
                                <asp:CheckBox ID="_chkBxPrintChequeRequest" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt">
                        Authorise Cheque Request?
                    </td>
                    <td>
                        <asp:UpdatePanel ID="_updPanelAuthorise" runat="server">
                            <ContentTemplate>
                                <asp:CheckBox ID="_chkBxAuthorise" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
            <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="_btnReset" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="_updPnlClientRequestButtons" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 15px;">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;" id="_divBtnAddNew" runat="server">
                                        <asp:Button ID="_btnAddNew" runat="server" Text="New" CausesValidation="false" OnClick="_btnAddNew_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;" runat="server" id="_divReset">
                                        <asp:Button ID="_btnReset" runat="server" Text="Reset" OnClick="_btnReset_Click"
                                            CausesValidation="false" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnSave" runat="server" Text="Save" OnClick="_btnSave_Click" CausesValidation="true" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">

        function Reset() {


            $("#<%= _txtAmount.ClientID %>").val("0.00");
            $("#<%= _txtDescription.ClientID %>").val("");
            $("#<%= _txtPayee.ClientID %>").val("");
            $("#<%= _txtReference.ClientID %>").val("");

        }

        // Resets amount if left blank
        function ResetAmount() {
            var amount = $("#<%= _txtAmount.ClientID %>").val();

            if (amount == '') {
                $("#<%= _txtAmount.ClientID %>").val("0.00");
            }
        }

        function SetPayee() {
            var description = document.getElementById("<%=_txtDescription.ClientID%>").value;

            if (description.indexOf(":") != -1) {
                $("#<%= _txtPayee.ClientID %>").val(description.substring(0, description.indexOf(":")).trim());
            }
            else {
                $("#<%= _txtPayee.ClientID %>").val(description.trim());
            }

            $("#<%= _txtPayee.ClientID %>").focus();
        }

        
        
    </script>

</asp:Content>

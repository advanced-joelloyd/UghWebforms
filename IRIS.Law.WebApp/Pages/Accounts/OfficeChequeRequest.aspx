<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="OfficeChequeRequest.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.OfficeChequeRequest"
    Title="Add Office Cheque Request" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentAddOfficeChequeRequest" ContentPlaceHolderID="_cphMain"
    runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                        <asp:HiddenField ID="_hdnOfficeChequeRequestId" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="_btnReset" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="sectionHeader" valign="bottom">
                <%if (Request.QueryString["edit"] == "true")
                  { %>
                Edit Office Cheque Request
                <%}
                  else
                  {%>
                Add Office Cheque Request
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
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <CliMat:ClientMatterDetails runat="server" SetSession="true" EnableValidation="true"
                            OnMatterChanged="_cliMatDetails_MatterChanged" ID="_cliMatDetails" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <asp:Panel ID="_pnlTimeDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Office Electronic Cheque Request</asp:Panel>
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
                                <CC:CalendarControl ID="_ccPostDate" OnDateChanged="_ccPostDate_DateChanged" runat="server" EnableValidation="true" InvalidValueMessage="Valid Date Required" />
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
                    <td class="boldTxt">
                        Disb Type
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="_ddlDisbursementType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_ddlDisbursementType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Description
                    </td>
                    <td colspan="3">
                        <asp:UpdatePanel ID="_upDescription" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtDescription" runat="server" Style="width: 355px;" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();" onchange="SetPayee();"></asp:TextBox>
                                <span class="mandatoryField">*</span>
                                <asp:RequiredFieldValidator ID="_rfvDescription" runat="server" ErrorMessage="Description is mandatory"
                                    Display="None" ControlToValidate="_txtDescription"></asp:RequiredFieldValidator>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlDisbursementType" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Reference
                    </td>
                    <td>
                        <asp:TextBox ID="_txtReference" runat="server" SkinID="Small" Text="CHQ" ReadOnly="true"></asp:TextBox>
                    </td>
                    <td class="boldTxt" style="width: 100px;">
                        Payee
                    </td>
                    <td>
                        <asp:UpdatePanel ID="_upPayee" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtPayee" runat="server" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();"></asp:TextBox>
                                <span class="mandatoryField">*</span>
                                <asp:RequiredFieldValidator ID="_rfvPayee" runat="server" ErrorMessage="Payee is mandatory"
                                    Display="None" ControlToValidate="_txtPayee"></asp:RequiredFieldValidator>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlDisbursementType" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Bank
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="_ddlBank" runat="server" SkinID="Large">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Amount
                    </td>
                    <td>
                        <asp:UpdatePanel ID="_updPanelAmount" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtAmount" runat="server" Text="0.00" MaxLength="11" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();" SkinID="NumericTextbox" onblur="ResetAmount();" AutoPostBack="true"
                                    OnTextChanged="_txtAmount_TextChanged"></asp:TextBox>
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
                    <td id="_tdLblVATRate" runat="server" class="boldTxt" style="width: 100px;">
                        VAT Rate
                    </td>
                    <td id="_tdDdlVATRate" runat="server">
                        <asp:DropDownList ID="_ddlVATRate" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_ddlVATRate_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        VAT Y/N
                    </td>
                    <td>
                        <asp:DropDownList ID="_ddlVAT" runat="server" SkinID="Small" AutoPostBack="true"
                            onchange="DisplayVATRate();" OnSelectedIndexChanged="_ddlVAT_SelectedIndexChanged">
                            <asp:ListItem>No</asp:ListItem>
                            <asp:ListItem>Yes</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td id="_tdLblVATAmount" runat="server" class="boldTxt" style="width: 100px;">
                        VAT Amount
                    </td>
                    <td id="_tdTxtVATAmount" runat="server">
                        <asp:UpdatePanel ID="_updPanelVATAmount" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TextBox ID="_txtVATAmount" runat="server"></asp:TextBox>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlVATRate" />
                                <asp:AsyncPostBackTrigger ControlID="_txtAmount" />
                                <asp:AsyncPostBackTrigger ControlID="_ddlVAT" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            <table border="0">
                <tr>
                    <td class="boldTxt">
                        Create as Anticipated?
                    </td>
                    <td>
                        <asp:CheckBox ID="_chkBxAnticipated" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt">
                        Print Cheque Request?
                    </td>
                    <td>
                        <asp:CheckBox ID="_chkBxPrintChequeRequest" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt">
                        Authorise Cheque Request?
                    </td>
                    <td>
                        <asp:CheckBox ID="_chkBxAuthorise" runat="server" />
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
    <asp:UpdatePanel ID="_updPnlOfficeRequestButtons" runat="server">
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
                                    <div class="button" runat="server" id="_divReset" style="text-align: center;">
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
            $("#<%= _ddlVAT.ClientID %>").val("No");
            $("#<%= _txtAmount.ClientID %>").val("0.00");
            $("#<%= _txtVATAmount.ClientID %>").val("0.00");
            $("#<%= _txtDescription.ClientID %>").val("");
            $("#<%= _txtPayee.ClientID %>").val("");
        }

        // Resets amount if left blank
        function ResetAmount() {
            var amount = $("#<%= _txtAmount.ClientID %>").val();

            if (amount == '') {
                $("#<%= _txtAmount.ClientID %>").val("0.00");
            }
        }

        function DisplayVATRate() {
            var isVATable = document.getElementById("<%=_ddlVAT.ClientID%>").value;

            switch (isVATable) {
                case "No":
                    document.getElementById("<%= _tdLblVATRate.ClientID %>").style.display = 'none';
                    document.getElementById("<%= _tdDdlVATRate.ClientID %>").style.display = 'none';
                    document.getElementById("<%= _tdLblVATAmount.ClientID %>").style.display = 'none';
                    document.getElementById("<%= _tdTxtVATAmount.ClientID %>").style.display = 'none';
                    break;
                case "Yes":
                    document.getElementById("<%= _tdLblVATRate.ClientID %>").style.display = '';
                    document.getElementById("<%= _tdDdlVATRate.ClientID %>").style.display = '';
                    document.getElementById("<%= _tdLblVATAmount.ClientID %>").style.display = '';
                    document.getElementById("<%= _tdTxtVATAmount.ClientID %>").style.display = '';
                    document.getElementById("<%=_txtVATAmount.ClientID%>").value = '0.00';
                    break;
            }
        }

        function SetPayee() {
            var description = document.getElementById("<%=_txtDescription.ClientID%>").value;

            $("#<%= _txtPayee.ClientID %>").val(description);
            $("#<%= _txtPayee.ClientID %>").focus();
        }

        $(document).ready(function() {
            //$("#<%= _txtAmount.ClientID %>").numeric();
        });

        function GetVATAmountOnVatRate() {
            var selectedVATRatePerc = GetValueOnIndexFromArray($("#<%=_ddlVATRate.ClientID %>").val(), 1);
            var vatRatePerc = selectedVATRatePerc;
            var vatAmount = $("#<%= _txtAmount.ClientID %>").val();

            if ($("#<%=_ddlVAT.ClientID %>").val() == "Yes") {
                $("#<%= _txtVATAmount.ClientID %>").val(roundNumber((parseFloat(vatAmount) / (parseFloat(vatRatePerc) + 100)) * parseFloat(vatRatePerc), 2));
            }
            else {
                $("#<%= _txtVATAmount.ClientID %>").val("0.00");
            }
        }

        function GetValueOnIndexFromArray(strValue, index) {
            var arrayBranch = strValue.split('$');
            return arrayBranch[index].trim();
        }

        function roundNumber(number, decimals) {
            var newString; // The new rounded number
            decimals = Number(decimals);
            if (decimals < 1) {
                newString = (Math.round(number)).toString();
            } else {
                var numString = number.toString();
                if (numString.lastIndexOf(".") == -1) {// If there is no decimal point
                    numString += "."; // give it one at the end
                }
                var cutoff = numString.lastIndexOf(".") + decimals; // The point at which to truncate the number
                var d1 = Number(numString.substring(cutoff, cutoff + 1)); // The value of the last decimal place that we'll end up with
                var d2 = Number(numString.substring(cutoff + 1, cutoff + 2)); // The next decimal, after the last one we want
                if (d2 >= 5) {// Do we need to round up at all? If not, the string will just be truncated
                    if (d1 == 9 && cutoff > 0) {// If the last digit is 9, find a new cutoff point
                        while (cutoff > 0 && (d1 == 9 || isNaN(d1))) {
                            if (d1 != ".") {
                                cutoff -= 1;
                                d1 = Number(numString.substring(cutoff, cutoff + 1));
                            } else {
                                cutoff -= 1;
                            }
                        }
                    }
                    d1 += 1;
                }
                if (d1 == 10) {
                    numString = numString.substring(0, numString.lastIndexOf("."));
                    var roundedNum = Number(numString) + 1;
                    newString = roundedNum.toString() + '.';
                } else {
                    newString = numString.substring(0, cutoff) + d1.toString();
                }
            }
            if (newString.lastIndexOf(".") == -1) {// Do this again, to the new string
                newString += ".";
            }
            var decs = (newString.substring(newString.lastIndexOf(".") + 1)).length;
            for (var i = 0; i < decimals - decs; i++) newString += "0";
            //var newNumber = Number(newString);// make it a number if you like
            return newString; // Output the result to the form field (change for your purposes)
        }		
    </script>

</asp:Content>

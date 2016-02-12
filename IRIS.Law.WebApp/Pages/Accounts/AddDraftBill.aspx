<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="AddDraftBill.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.AddDraftBill"
    Title="Add Draft Bill" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentAddOfficeChequeRequest" ContentPlaceHolderID="_cphMain"
    runat="server">

    <script type="text/javascript">
        var browser = navigator.appName;
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }

        var grd = new Array();
        grd[0] = "<%=_grdUnBilledPaidNonVatable.ClientID %>";
        grd[1] = "<%=_grdUnbilledPaidVatable.ClientID %>";
        grd[2] = "<%=_grdAnticipatedNonVatable.ClientID %>";
        grd[3] = "<%=_grdAnticipatedVatable.ClientID %>";
        grd[4] = "<%=_grdUnBilledTime.ClientID %>";

        function SelectUnselectAll(sender, gridviewId) {
            var objChequeRequest = document.getElementById(gridviewId);
            if (objChequeRequest != null) {
                var checkBoxes = objChequeRequest.getElementsByTagName("input");

                for (var i = 0; i < checkBoxes.length; i++) {
                    if (objChequeRequest.getElementsByTagName("input").item(i).disabled == false) {
                        objChequeRequest.getElementsByTagName("input").item(i).checked = sender.checked;
                    }
                }
            }
        }

        function CalcUnbilled(amount, billed, unbilled) {
            var amount = parseFloat($("#" + amount).text().replace(",", ""));
            var billedAmt = parseFloat($("#" + billed).val());
            var unBilledLabel = $("#" + unbilled);
            var unBilledAmt = parseFloat(unBilledLabel.text());
            if (!isNaN(billedAmt)) {
                var value = amount - billedAmt;
                unBilledLabel.text(value.toFixed(2));
            }
        }

        function Billed(amount, billed, unbilled) {
            var amount = parseFloat($("#" + amount).text().replace(",", ""));
            var billedAmtTextBox = $("#" + billed);
            var unBilledLabel = $("#" + unbilled);

            billedAmtTextBox.val(amount.toFixed(2));
            unBilledLabel.text("0.00");
            return false;
        }

        function GetVATAmountOnVatRate() {
            var vatRatePerc = GetValueOnIndexFromArray($("#<%=_ddlVATRate.ClientID %>").val(), 1);
            var costs = parseFloat($("#<%= _txtCosts.ClientID %>").val());
            $("#<%= _txtTotalCost.ClientID %>").val(costs.toFixed(2));

            var vatAmt = parseFloat(costs * (parseFloat(vatRatePerc) / 100));
            $("#<%= _txtVAT.ClientID %>").val(vatAmt.toFixed(2));
            var disbTotal = parseFloat($("#<%= _hdnDisbTotal.ClientID %>").val());
            var disbVatTotal = parseFloat($("#<%= _hdnDisbVatTotal.ClientID %>").val());
            var totalVat = vatAmt + disbVatTotal;
            $("#<%= _txtTotalVAT.ClientID %>").val(totalVat.toFixed(2));
            $("#<%= _txtTotal.ClientID %>").val((costs + disbTotal + totalVat).toFixed(2));
        }

        function ReCalculateVat() {
            var costs = parseFloat($("#<%= _txtCosts.ClientID %>").val());
            var disbTotal = parseFloat($("#<%= _hdnDisbTotal.ClientID %>").val());
            var disbVatTotal = parseFloat($("#<%= _hdnDisbVatTotal.ClientID %>").val());
            var vatAmt = parseFloat($("#<%= _txtVAT.ClientID %>").val());
            var totalVat = vatAmt + disbVatTotal;
            $("#<%= _txtTotalVAT.ClientID %>").val(totalVat.toFixed(2));
            $("#<%= _txtTotal.ClientID %>").val((costs + disbTotal + totalVat).toFixed(2));
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

        $(document).ready(function() {
            $("#<%= _txtCosts.ClientID %>").change(GetVATAmountOnVatRate);
            $("#<%= _txtVAT.ClientID %>").change(ReCalculateVat);

            $("#<%=_ccUnbilledTimeUpto.DateTextBoxClientID%>").change(Filter);
            Filter($("#<%=_ccUnbilledTimeUpto.DateTextBoxClientID%>"));
        });

        function ConvertDate(dateText) {
            var array = dateText.split('/');
            var yyyyMMdd = array[2] + "/" + array[1] + "/" + array[0];
            var date = new Date(yyyyMMdd);
            return date;
        }

        //selects the main checkbox if all the child checkboxes are selected
        //unselect it if any of the child checkboxes are not selected
        //disables the post time entry button if no entry is selected
        function SelectUnselectHeaderCheckbox() {
            var checkboxes = $("#<%=_grdUnBilledTime.ClientID %> input:checkbox");
            var allCheckboxesSelected = true;
            var isSelected = false;
            for (i = 1; i < checkboxes.length; i++) {
                if (!checkboxes[i].checked) {
                    allCheckboxesSelected = false;
                }
                else {
                    isSelected = true;
                }
            }
            checkboxes[0].checked = allCheckboxesSelected;
        }

        function Filter(sender) {
            var textDate;
            if (sender.type == 'change') {
                textDate = $(this).val()
            }
            else {
                var val = $(sender).val();
                if (typeof val == 'undefined') {
                    return;
                }
                else {
                    textDate = val;
                    //on form load textbox has date and time. take only the date part
                    textDate = textDate.substring(0, 10);
                }
            }
            textDate = ConvertDate(textDate);
            $("#<%= _grdUnBilledTime.ClientID %> tr:has(td)").each(function() {
                var gridDate = ConvertDate($('td:first', $(this)).next().text());
                if (gridDate > textDate) {
                    $(this).hide();
                }
                else {
                    $(this).show();
                }
            });
            var index = 0;
            $("#<%= _grdUnBilledTime.ClientID %> tr:has(td)").each(function() {
                $(this).removeClass("gridViewRowAlternate");
                $(this).removeClass("gridViewRow");
                if ($(this).css("display") != 'none') {
                    index % 2 == 0 ? $(this).addClass("gridViewRow") : $(this).addClass("gridViewRowAlternate");
                    index++;
                }
            });
        }
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                        <asp:HiddenField ID="_hdnBindDraftBillGrids" runat="server" Value="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                Add New Draft Bill
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="_updPnlAddDraftBill" runat="server">
        <ContentTemplate>
            <asp:Wizard ID="_wizardAddDraftBill" runat="server" DisplaySideBar="False" ActiveStepIndex="0"
                Width="100%" EnableTheming="True" Height="400px" StepStyle-VerticalAlign="Top"
                OnNextButtonClick="StartNextButton_Click">
                <StepStyle VerticalAlign="Top" />
                <StartNextButtonStyle CssClass="button" />
                <FinishCompleteButtonStyle CssClass="button" />
                <StepNextButtonStyle CssClass="button" />
                <StartNavigationTemplate>
                    <table width="100%">
                        <tr>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStartNavCancel" OnClick="_btnWizardStartNavCancel_Click"
                                                    runat="server" Text="Reset" CausesValidation="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Next" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </StartNavigationTemplate>
                <FinishPreviousButtonStyle CssClass="button" />
                <NavigationButtonStyle CssClass="button" />
                <StepNavigationTemplate>
                    <table width="100%">
                        <tr>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                                    Text="Previous" OnClick="StepPreviousButton_Click" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepNavCancel" OnClick="_btnWizardStartNavCancel_Click"
                                                    runat="server" Text="Reset" CausesValidation="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next"
                                                    OnClick="StepNextButton_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </StepNavigationTemplate>
                <FinishNavigationTemplate>
                    <table width="100%">
                        <tr>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepPreviousButton" runat="server" CausesValidation="False"
                                                    CommandName="MovePrevious" Text="Previous" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardFinishNavCancel" OnClick="_btnWizardStartNavCancel_Click"
                                                    runat="server" Text="Reset" CausesValidation="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepFinishButton" runat="server" CausesValidation="true"
                                                    CommandName="MoveComplete" Text="Finish" OnClick="_btnWizardStepFinishButton_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </FinishNavigationTemplate>
                <StepPreviousButtonStyle CssClass="button" />
                <WizardSteps>
                    <asp:WizardStep ID="_wizardStepDraftBillGeneralDetails" runat="server" Title="Draft Bill : General Details">
                        <table width="100%">
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
                        </table>
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="3">
                                    <asp:Panel ID="_pnlDraftBillDetails" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Draft Bill : General Details</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 100px;">
                                    Date
                                </td>
                                <td style="width: 200px;">
                                    <CC:CalendarControl ID="_ccDraftBillDate" runat="server" EnableValidation="true"
                                        InvalidValueMessage="Valid Date Required" OnDateChanged="_ccDraftBillDate_DateChanged" />
                                    <span class="mandatoryField">*</span>
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
                                                        <asp:AsyncPostBackTrigger ControlID="_ccDraftBillDate" />
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
                                    <asp:TextBox ID="_txtDraftBillDescription" runat="server" onmousemove="showToolTip(event);return false;"
                                        onmouseout="hideToolTip();">
                                    </asp:TextBox>
                                    <span class="mandatoryField">*</span>
                                    <asp:RequiredFieldValidator ID="_rfvDescription" runat="server" ErrorMessage="Description is mandatory"
                                        Display="None" ControlToValidate="_txtDraftBillDescription">
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepDraftBillDisbursements" runat="server" Title="Draft Bill : Disbursements">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlDraftBillDisbursements" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Draft Bill : Disbursements</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <ajaxToolkit:TabContainer ID="_tcDraftBillDisbursements" runat="server" CssClass="ajax__widetab_xp2"
                                        Width="100%" ActiveTabIndex="0" Height="375px">
                                        <ajaxToolkit:TabPanel runat="server" ID="_pnlClientChequeRequests" HeaderText="Unbilled Paid N-VATable">
                                            <ContentTemplate>
                                                <asp:UpdatePanel ID="_updPanelUnBilledPaidNonVatable" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <div style="height: 260px; overflow: auto;">
                                                            <asp:GridView ID="_grdUnBilledPaidNonVatable" runat="server" AutoGenerateColumns="false"
                                                                BorderWidth="0" GridLines="None" Width="99.9%" CssClass="successMessage" EmptyDataText="There are no results to display."
                                                                OnRowDataBound="_grdUnbilledPaidNonVatable_RowDataBound">
                                                                <Columns>
                                                                    <asp:BoundField DataField="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-HorizontalAlign="Left"
                                                                        HeaderText="Date" />
                                                                    <asp:BoundField DataField="Reference" HeaderText="Reference" HeaderStyle-HorizontalAlign="Left" />
                                                                    <asp:TemplateField HeaderText="Description">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'>
                                                                            </asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Amount">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="_lblAmount" runat="server" Text='<%# String.Format("{0:F2}", Eval("Amount"))%>'>
                                                                            </asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Billed">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="_txtBilled" runat="server" SkinID="NumericTextBox" Text='<%# String.Format("{0:F2}", Eval("Billed"))%>'>
                                                                            </asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Unbilled">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="_lblUnBilled" runat="server" Text='<%# String.Format("{0:F2}", Eval("UnBilled"))%>'>
                                                                            </asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Bill">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="_lnkBtnBill" CssClass="link" runat="server" ToolTip="Bill" Text="Bill"></asp:LinkButton>
                                                                            <asp:HiddenField ID="_hdnPostingId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>' />
                                                                            <asp:HiddenField ID="_hdnBillDisbursementAllocationAmount" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BillDisbursementAllocationAmount")%>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                        <HeaderStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                        <span class="boldTxt">Notes :&nbsp;</span>
                                                        <asp:TextBox ID="_txtNonVatableNotes" runat="server" Width="90%" TextMode="MultiLine"
                                                            Rows="6"></asp:TextBox>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="_pnlUnbilledPaidVatable" HeaderText="Unbilled Paid VATable">
                                            <ContentTemplate>
                                                <asp:UpdatePanel ID="_updPanelUnbilledPaidVatable" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <div style="height: 260px; overflow: auto;">
                                                                        <asp:GridView ID="_grdUnbilledPaidVatable" runat="server" AutoGenerateColumns="false"
                                                                            BorderWidth="0" GridLines="None" Width="99.9%" CssClass="successMessage" EmptyDataText="There are no results to display."
                                                                            OnRowDataBound="_grdUnbilledPaidVatable_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Date"
                                                                                    HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Reference" HeaderText="Reference" HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField HeaderText="Description">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Amount">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblAmount" runat="server" Text='<%# String.Format("{0:F2}", Eval("Amount"))%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Billed">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="_txtBilled" runat="server" SkinID="NumericTextBox" Text='<%# String.Format("{0:F2}", Eval("Billed"))%>'>
                                                                                        </asp:TextBox>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Unbilled">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblUnBilled" runat="server" Text='<%# String.Format("{0:F2}", Eval("UnBilled"))%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Bill">
                                                                                    <ItemTemplate>
                                                                                        <asp:LinkButton ID="_lnkBtnBill" CssClass="link" runat="server" ToolTip="Bill" Text="Bill"></asp:LinkButton>
                                                                                        <asp:HiddenField ID="_hdnBillDisbursementAllocationAmount" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BillDisbursementAllocationAmount")%>' />
                                                                                        <asp:HiddenField ID="_hdnVatRateId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "VatRateId")%>' />
                                                                                        <asp:HiddenField ID="_hdnPostingId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <span class="boldTxt">Notes :&nbsp;</span>
                                                                    <asp:TextBox ID="_txtVatableNotes" runat="server" Width="90%" TextMode="MultiLine"
                                                                        Rows="6"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="_pnlAnticipatedNonVatable" HeaderText="Anticipated N-VATable">
                                            <ContentTemplate>
                                                <asp:UpdatePanel ID="_updPanelAnticipatedNonVatable" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <div style="height: 260px; overflow: auto;">
                                                                        <asp:GridView ID="_grdAnticipatedNonVatable" runat="server" AutoGenerateColumns="false"
                                                                            BorderWidth="0" GridLines="None" Width="99.9%" CssClass="successMessage" EmptyDataText="There are no results to display."
                                                                            OnRowDataBound="_grdAnticipatedNonVatable_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Date"
                                                                                    HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Reference" HeaderText="Reference" HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField HeaderText="Description">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Amount">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblAmount" runat="server" Text='<%# String.Format("{0:F2}", Eval("Amount"))%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Billed">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="_txtBilled" runat="server" SkinID="NumericTextBox" Text='<%# String.Format("{0:F2}", Eval("Billed"))%>'>
                                                                                        </asp:TextBox>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Unbilled">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblUnBilled" runat="server" Text='<%# String.Format("{0:F2}", Eval("UnBilled"))%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Bill">
                                                                                    <ItemTemplate>
                                                                                        <asp:LinkButton ID="_lnkBtnBill" CssClass="link" runat="server" ToolTip="Bill" Text="Bill"></asp:LinkButton>
                                                                                        <asp:HiddenField ID="_hdnPostingId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <span class="boldTxt">Notes :&nbsp;</span>
                                                                    <asp:TextBox ID="_txtAnticipatedNonVatableNotes" runat="server" Width="90%" TextMode="MultiLine"
                                                                        Rows="6"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="_pnlAnticipatedVatable" HeaderText="Anticipated VATable">
                                            <ContentTemplate>
                                                <asp:UpdatePanel ID="_updPanelAnticipatedVatable" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <div style="height: 260px; overflow: auto;">
                                                                        <asp:GridView ID="_grdAnticipatedVatable" runat="server" AutoGenerateColumns="false"
                                                                            BorderWidth="0" GridLines="None" Width="99.9%" CssClass="successMessage" EmptyDataText="There are no results to display."
                                                                            OnRowDataBound="_grdAnticipatedVatable_RowDataBound">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Date"
                                                                                    HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:BoundField DataField="Reference" HeaderText="Reference" HeaderStyle-HorizontalAlign="Left" />
                                                                                <asp:TemplateField HeaderText="Description">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Amount">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblAmount" runat="server" Text='<%# String.Format("{0:F2}", Eval("Amount"))%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Billed">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="_txtBilled" runat="server" SkinID="NumericTextBox" Text='<%# String.Format("{0:F2}", Eval("Billed"))%>'>						                                                                      
                                                                                        </asp:TextBox>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Unbilled">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="_lblUnBilled" runat="server" Text='<%# String.Format("{0:F2}", Eval("UnBilled"))%>'
                                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "UnBilled")%>'>
                                                                                        </asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Bill">
                                                                                    <ItemTemplate>
                                                                                        <asp:LinkButton ID="_lnkBtnBill" CssClass="link" runat="server" ToolTip="Bill" Text="Bill"></asp:LinkButton>
                                                                                        <asp:HiddenField ID="_hdnVatRateId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "VatRateId")%>' />
                                                                                        <asp:HiddenField ID="_hdnPostingId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PostingId")%>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Right" />
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                    <span class="boldTxt">Notes :&nbsp;</span>
                                                                    <asp:TextBox ID="_txtAnticipatedVatableNotes" runat="server" Width="90%" TextMode="MultiLine"
                                                                        Rows="6"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                    </ajaxToolkit:TabContainer>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepUnBilledTime" runat="server" Title="Draft Bill : Unbilled Time">
                        <table width="100%" border="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlDraftBillUnBilledTime" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Draft Bill : Unbilled Time</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" border="0">
                                        <tr>
                                            <td class="boldTxt" style="width: 120px;">
                                                Unbilled Time up to
                                            </td>
                                            <td>
                                                <CC:CalendarControl ID="_ccUnbilledTimeUpto" runat="server" OnDateChanged="CcUnbilledTimeUptoDateChanged" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="height: 315px; overflow: auto;">
                                        <asp:GridView ID="_grdUnBilledTime" runat="server" AutoGenerateColumns="false" BorderWidth="0"
                                            GridLines="None" Width="95%" CssClass="successMessage" EmptyDataText="There are no results to display."
                                            OnRowDataBound="_grdUnBilledTime_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="_chkBxSelectAll" onclick="SelectUnselectAll(this, grd[4]);" ToolTip="Select All"
                                                            Checked="true" runat="server" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="_chkBxSelect" runat="server" Checked="true" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Date" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Date"
                                                    HeaderStyle-HorizontalAlign="Left" />
                                                <asp:TemplateField HeaderText="Fee Earner">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarner")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarner")%>'>
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'>
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Charge">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblCharge" runat="server" Text='<%# String.Format("{0:F2}", Eval("Charge"))%>'>
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cost">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblCost" runat="server" Text='<%# String.Format("{0:F2}", Eval("Cost"))%>'>
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Time">
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblTime" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Time")%>'>
                                                        </asp:Label>
                                                        <asp:HiddenField ID="_hdnTimeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TimeId")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepPreviewTotal" runat="server" Title="Draft Bill : Preview">
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="5">
                                    <asp:Panel ID="_pnlDraftBillPreview" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Draft Bill : Preview</asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <asp:UpdatePanel ID="_upPanelVatRate" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table width="100%" border="0">
                                    <tr>
                                        <td style="width: 30%;">
                                            &nbsp;
                                        </td>
                                        <td class="boldTxt" style="width: 15%;" align="right">
                                            Costs
                                        </td>
                                        <td class="boldTxt" style="width: 15%;" align="right">
                                            Disbursements
                                        </td>
                                        <td class="boldTxt" style="width: 15%;" align="right">
                                            VAT
                                        </td>
                                        <td class="boldTxt" style="width: 35%;" align="right">
                                            VAT Rate
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            To our professional charges
                                        </td>
                                        <td align="right">
                                            <asp:TextBox ID="_txtCosts" AutoPostBack="true" ontextchanged="_txtCosts_TextChanged" SkinID="NumericTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="right">
                                            <asp:TextBox ID="_txtVAT" SkinID="NumericTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            <asp:DropDownList ID="_ddlVATRate" runat="server" OnSelectedIndexChanged="_ddlVATRate_SelectedIndexChanged"
                                                AutoPostBack="true">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="_hdnDisbTotal" runat="server" Value="0" />
                                <asp:HiddenField ID="_hdnDisbVatTotal" runat="server" Value="0" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <table width="100%" border="0">
                            <tr>
                                <td class="boldTxt" colspan="5">
                                    Paid Disbursements:
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:Table ID="_tblPaidDisbursements" runat="server" CellPadding="1" CellSpacing="0"
                                        Width="100%" EnableViewState="true">
                                    </asp:Table>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" colspan="5">
                                    Unpaid Disbursements:
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:Table ID="_tblUnPaidDisbursements" runat="server" CellPadding="1" CellSpacing="0"
                                        Width="100%" EnableViewState="true">
                                    </asp:Table>
                                </td>
                            </tr>
                        </table>
                        <asp:UpdatePanel ID="_updPanelTotalVAT" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <table width="100%" border="0">
                                    <tr>
                                        <td class="boldTxt" style="width: 30%;">
                                            Sub Total
                                        </td>
                                        <td align="right" style="width: 15%;">
                                            <asp:TextBox ID="_txtTotalCost" SkinID="NumericTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td align="right" style="width: 15%;">
                                            <asp:TextBox ID="_txtTotalDisbursements" ReadOnly="true" SkinID="NumericTextBox"
                                                runat="server"></asp:TextBox>
                                        </td>
                                        <td align="right" style="width: 15%;">
                                            <asp:TextBox ID="_txtTotalVAT" ReadOnly="true" SkinID="NumericTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td style="width: 35%;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" colspan="3" align="right">
                                            Total
                                        </td>
                                        <td align="right">
                                            <asp:TextBox ID="_txtTotal" SkinID="NumericTextBox" ReadOnly="true" runat="server"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="_ddlVATRate" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </asp:WizardStep>
                </WizardSteps>
            </asp:Wizard>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

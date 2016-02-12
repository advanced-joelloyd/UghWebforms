<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Recurrence.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.Recurrence" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="ASD" TagName="AddSubtractDays" Src="~/UserControls/AddSubtractDays.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script language="javascript">
    var browser = navigator.appName;
    //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
    //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
    //on box-items. This is not yet implemented in IE so round the corners using javascript
    if (browser == "Microsoft Internet Explorer") {
        Sys.Application.add_load(HideUnhideRecurrence);
    }

    function CancelRecurrencePopupClick() {
        return false;
    }

    function HideUnhideRecurrence() {

        var radioDaily = document.getElementById("<%=_radioDaily.ClientID%>");
        var radioWeekly = document.getElementById("<%=_radioWeekly.ClientID%>");
        var radioMonthly = document.getElementById("<%=_radioMonthly.ClientID%>");
        var radioYearly = document.getElementById("<%=_radioYearly.ClientID%>");

        var tableDaily = document.getElementById("<%=_tableDaily.ClientID%>");
        var tableWeekly = document.getElementById("<%=_tableWeekly.ClientID%>");
        var tableMonthly = document.getElementById("<%=_tableMonthly.ClientID%>");
        var tableYearly = document.getElementById("<%=_tableYearly.ClientID%>");

        tableDaily.style.display = "none";
        tableWeekly.style.display = "none";
        tableMonthly.style.display = "none";
        tableYearly.style.display = "none";

        if (radioDaily.checked) {
            tableDaily.style.display = "";
        }
        else if (radioWeekly.checked) {
            tableWeekly.style.display = "";
        }
        else if (radioMonthly.checked) {
            tableMonthly.style.display = "";
        }
        else if (radioYearly.checked) {
            tableYearly.style.display = "";
        }

    }
</script>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div class="button" style="width: 172px; float:right; text-align: center;" id="_divRecurrence"
                title="Recurrence" runat="server">
                <asp:Button ID="_btnRecurrence" runat="server" CausesValidation="False" Text="Recurrence"
                    ToolTip="Recurrence" OnClick="_btnRecurrence_Click" />
            </div>
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupRecurrence" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlRecurrence" OnCancelScript="javascript:CancelRecurrencePopupClick();"
                TargetControlID="_btnRecurrence" CancelControlID="_btnCancel" BehaviorID="_modalpopupRecurrenceBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<asp:Panel ID="_pnlRecurrence" runat="server" Style="background-color: #ffffff" Width="800px"
    Height="400px">
    <asp:UpdateProgress ID="_updateProgress" runat="server">
        <ProgressTemplate>
            <div class="textBox">
                <img id="_imgLoading" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%" class="panel">
        <tr>
            <td>
                <table width="100%" id="_tableHeader">
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="_updPnlError" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" CssClass="labelValue" runat="server" Text=""></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" runat="server" id="_tdHeader">
                            <asp:Label ID="_whichPage" runat="server"></asp:Label>
                            Recurrence
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" border="0">
                    <tr>
                        <td class="boldTxt">
                            <asp:RadioButton ID="_radioDaily" runat="server" GroupName="Recurrence" Text="Daily"
                                onclick="HideUnhideRecurrence();" />
                        </td>
                        <td class="boldTxt">
                            <asp:RadioButton ID="_radioWeekly" runat="server" GroupName="Recurrence" Text="Weekly"
                                onclick="HideUnhideRecurrence();" />
                        </td>
                        <td class="boldTxt">
                            <asp:RadioButton ID="_radioMonthly" runat="server" GroupName="Recurrence" Text="Monthly"
                                onclick="HideUnhideRecurrence();" />
                        </td>
                        <td class="boldTxt">
                            <asp:RadioButton ID="_radioYearly" runat="server" GroupName="Recurrence" Text="Yearly"
                                onclick="HideUnhideRecurrence();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" id="_tableDaily" runat="server">
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            Start
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccStartDateDaily" InvalidValueMessage="Valid Date Required"
                                runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            <asp:RadioButton ID="_radioEveryDays" runat="server" GroupName="StartDaily" />
                            Every
                        </td>
                        <td>
                            <ASD:AddSubtractDays ID="_asdEveryDays" runat="server" MaxLength="3" />
                            &nbsp;<span class="boldTxt">day(s)</span>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" colspan="2">
                            <asp:RadioButton ID="_radioEveryWeekDay" runat="server" GroupName="StartDaily" />
                            Every weekday
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" id="_tableWeekly" runat="server">
                    <tr>
                        <td>
                            <table id="Table1" width="100%" runat="server">
                                <tr>
                                    <td class="boldTxt" style="width: 100px;">
                                        Start
                                    </td>
                                    <td>
                                        <CC:CalendarControl ID="_ccStartDateWeekly" InvalidValueMessage="Valid Date Required"
                                            runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt">
                                        Recur Every
                                    </td>
                                    <td>
                                        <ASD:AddSubtractDays ID="_asdRecurEvery" runat="server" MaxLength="3" />
                                        &nbsp;<span class="boldTxt">week(s) on</span>
                                    </td>
                                </tr>
                            </table>
                            <table width="100%">
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkMonday" runat="server" Text="Monday" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkTuesday" runat="server" Text="Tuesday" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkWednesday" runat="server" Text="Wednesday" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkThursday" runat="server" Text="Thursday" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkFriday" runat="server" Text="Friday" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkSaturday" runat="server" Text="Saturday" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkSunday" runat="server" Text="Sunday" />
                                    </td>
                                    <td>
                                        &nbsp;
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
                <table width="100%" id="_tableMonthly" runat="server">
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            Start
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccStartDateMonthly" InvalidValueMessage="Valid Date Required"
                                runat="server" />
                            &nbsp;&nbsp; <span class="boldTxt">Number of days(s)</span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <ASD:AddSubtractDays ID="_asdNoOfDaysMonthly" runat="server" MaxLength="1" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            <asp:RadioButton ID="_radioDay" runat="server" GroupName="StartMonthly" Text="Day" />
                        </td>
                        <td>
                            <ASD:AddSubtractDays ID="_asdDayMonthly" runat="server" MaxLength="1" />
                            &nbsp; <span class="boldTxt">of every </span>
                            <ASD:AddSubtractDays ID="_asdDayMonthMonthly" runat="server" MaxLength="3" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            <asp:RadioButton ID="RadioButton2" runat="server" GroupName="StartMonthly" Text="The" />
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlWeekCountMonthly" runat="server">
                            </asp:DropDownList>
                            <asp:DropDownList ID="_ddlWeekDayMonthly" runat="server">
                            </asp:DropDownList>
                            &nbsp; <span class="boldTxt">of every </span>
                            <ASD:AddSubtractDays ID="_asdTheMonthMonthly" runat="server" MaxLength="3" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" id="_tableYearly" runat="server">
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            Start
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccStartDateYearly" InvalidValueMessage="Valid Date Required"
                                runat="server" />
                            &nbsp;&nbsp; <span class="boldTxt">Number of days(s)</span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <ASD:AddSubtractDays ID="_asdNoOfDaysYearly" runat="server" MaxLength="1" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" style="width: 100px;">
                            <asp:RadioButton ID="_radioEveryMonthYearly" runat="server" GroupName="StartYearly"
                                Text="Every" />
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlMonthYearly" runat="server">
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <ASD:AddSubtractDays ID="_asdDaysYearly" runat="server" MaxLength="1" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            <asp:RadioButton ID="_radioTheMonthYearly" runat="server" GroupName="StartYearly"
                                Text="The" />
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlWeekCountYearly" runat="server">
                            </asp:DropDownList>
                            <asp:DropDownList ID="_ddlWeekDayYearly" runat="server">
                            </asp:DropDownList>
                            &nbsp; <span class="boldTxt">of every </span>
                            <ASD:AddSubtractDays ID="_asdTheMonthYearly" runat="server" MaxLength="3" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <table id="_tableEndDate" width="100%">
                    <tr>
                        <td class="boldTxt" width="100px;">
                            <asp:RadioButton ID="_radioEndAfter" runat="server" GroupName="End" />
                            End After
                        </td>
                        <td>
                            <ASD:AddSubtractDays ID="AddSubtractDays1" runat="server" MaxLength="3" />
                            &nbsp;<span class="boldTxt">recurrences</span>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" width="100px;">
                            <asp:RadioButton ID="_radioEndBy" runat="server" GroupName="End" />
                            End By
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccEndDateDaily" InvalidValueMessage="Valid Date Required"
                                runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table id="_tableButtons" width="100%">
        <tr>
            <td align="left" style="padding-left: 15px;">
                <div id="_divDelete" runat="server" class="button" style="float: left; text-align: center;">
                    <asp:Button ID="_btnDelete" runat="server" Text="Delete" />
                </div>
            </td>
            <td align="right" style="padding-right: 15px;">
                <table>
                    <tr>
                        <td align="right">
                            <div id="_divCancel" runat="server" class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnCancel" runat="server" Text="Cancel" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnSave" runat="server" Text="Save" OnClick="_btnSave_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="AppointmentDetails.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Appointment.AppointmentDetails"
    Title="Appointment Details" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="TC" TagName="TimeControl" Src="~/UserControls/TimeControl.ascx" %>
<%@ Register TagPrefix="OU" TagName="OrganiseUsers" Src="~/UserControls/OrganiseUsers.ascx" %>
<%@ Register TagPrefix="SS" TagName="ServiceSearch" Src="~/UserControls/ServiceSearch.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<asp:Content ID="_contentAppointmentDetails" ContentPlaceHolderID="_cphMain" runat="server">

    <script language="javascript">
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

        Sys.Application.add_load(function() {
            $('[readonly]').addClass("readonly");
        });

        function SetReminderState() {
            if ($("#<%=_ddlReminder.ClientID %>").val() == "On") {
                $("#<%=_tdReminderOn.ClientID %>").css("display", "");
                $("#<%=_tdReminderBefore.ClientID %>").css("display", "none");
            }
            else if ($("#<%=_ddlReminder.ClientID %>").val() == "Before") {
                $("#<%=_tdReminderOn.ClientID %>").css("display", "none");
                $("#<%=_tdReminderBefore.ClientID %>").css("display", "");
            }
        }
    </script>
    <table width="100%">
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="_updPnlError" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                        <asp:HiddenField ID="_hdnAppointmentId" runat="server" />
                        <asp:HiddenField ID="_hdnProjectId" runat="server" />
                        <asp:HiddenField ID="_hdnVenueId" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" valign="top">
                Appointment Details
            </td>
        </tr>
    </table>
    <table width="100%" border="0">
        <tr>
            <td valign="top">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <CliMat:ClientMatterDetails runat="server" ID="_cliMatDetails" SetSession="false"
                            DisplayResetButton="true" OnMatterChanged="_cliMatDetails_MatterChanged" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td valign="top">
                <asp:Panel ID="_pnlCreateAppointment" runat="server" Width="99.9%" CssClass="bodyTab">
                    Appointment
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td valign="top" width="100px;">
                                        <OU:OrganiseUsers ID="_AppointmentOU" runat="server" ButtonText="Attendees" ButtonTooltip="Attendees" OnUsersAdded="_AppointmentOU_UsersAdded">
                                        </OU:OrganiseUsers>
                                    </td>
                                    <td>
                                        <asp:UpdatePanel ID="_updPnlOrganiseUsers" runat="server">
                                            <ContentTemplate>
                                                <asp:TextBox ID="_txtAttendees" TextMode="MultiLine" Height="30px" Width="95%" runat="server"
                                                    onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvAttendees" runat="server" ErrorMessage="Attendees is mandatory"
                                                    Display="None" ControlToValidate="_txtAttendees"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                                <asp:HiddenField ID="_hdnAttendeesMemberId" runat="server" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="_AppointmentOU" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="boldTxt" width="100px;">
                                        Subject
                                    </td>
                                    <td>
                                        <asp:TextBox ID="_txtSubject" TextMode="MultiLine" Height="30px" Width="95%" runat="server"
                                            onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="_rfvSubject" runat="server" ErrorMessage="Subject is mandatory"
                                            Display="None" ControlToValidate="_txtSubject"></asp:RequiredFieldValidator>
                                        <span class="mandatoryField">*</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="95%" border="0">
                                <tr>
                                    <td class="boldTxt" width="100px;">
                                        Venue
                                    </td>
                                    <td colspan="3">
                                        <SS:ServiceSearch ID="_ssAppointmentVenue" runat="server" DisplayPopup="true" DisplayServiceText="True"
                                            OnServiceSelected="_ssAppointmentVenue_ServiceSelected"></SS:ServiceSearch>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt" width="100px;">
                                        Date
                                    </td>
                                    <td colspan="3">
                                        <CC:CalendarControl ID="_ccDate" InvalidValueMessage="Valid Date Required" EnableValidation="true" runat="server" />
                                    </td>
                                    <td class="boldTxt" width="80px;">
                                        Start Time
                                    </td>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                            <ContentTemplate>
                                                <TC:TimeControl ID="_tcStartTime" InvalidValueMessage="Valid Start Time Required"
                                                    runat="server" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="_chkReminder" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td class="boldTxt" width="80px;">
                                        End Time
                                    </td>
                                    <td>
                                        <TC:TimeControl ID="_tcEndTime" InvalidValueMessage="Valid End Time Required" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt" width="100px;">
                                        Reminder
                                    </td>
                                    <td colspan="3" align="left">
                                        <asp:UpdatePanel ID="_updPnlReminder" runat="server">
                                            <ContentTemplate>
                                                <table border="0" cellspacing="0" cellpadding="0" style="padding-top: 4px;">
                                                    <tr>
                                                        <td align="left" style="width: 20px;">
                                                            <asp:CheckBox ID="_chkReminder" Style="margin: -4px" runat="server" OnCheckedChanged="_chkReminder_Checked"
                                                                AutoPostBack="true" />
                                                        </td>
                                                        <td id="_tdReminderType" runat="server" align="left" style="display: none; width: 170px;">
                                                            <asp:DropDownList ID="_ddlReminder" runat="server" onchange="SetReminderState();">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td id="_tdReminderBefore" runat="server" style="display: none; width: 260px;" align="left">
                                                            <asp:DropDownList ID="_ddlReminderBeforeTime" runat="server">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td id="_tdReminderOn" runat="server" align="left" style="display: none; width: 260px;">
                                                            <CC:CalendarControl ID="_ccReminderDate" InvalidValueMessage="Valid Reminder Date Required"
                                                                runat="server" />
                                                            <span class="boldTxt">At</span>
                                                            <TC:TimeControl ID="_tcReminderTime" InvalidValueMessage="Valid Reminder Time Required"
                                                                runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td class="boldTxt" valign="top" style="width: 100px; padding-top: 5px;">
                                        Notes
                                    </td>
                                    <td>
                                        <asp:TextBox ID="_txtNotes" TextMode="MultiLine" Height="120px" Width="95%" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="_updPnlAppointmentButtons" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 15px;">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;" id="_divBackButton" visible="false"
                                        runat="server">
                                        <asp:Button ID="_btnBack" runat="server" Text="Back" OnClientClick="javascript:window.location='ViewAppointment.aspx';return false;"
                                            OnClick="_btnBack_Click" CausesValidation="false" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div id="_divNew" runat="server" class="button" style="float: right; text-align: center;">
                                        <asp:Button ID="_btnNew" CausesValidation="false" runat="server" Text="New" OnClick="_btnNew_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div id="_divAppointments" runat="server" class="button" style="float: right; text-align: center; width:85px">
                                        <asp:Button ID="_btnAppointments" CausesValidation="false" runat="server" Text="Appointments" OnClick="_btnAppointment_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div id="_divReset" runat="server" class="button" style="float: right; text-align: center;">
                                        <asp:Button ID="_btnReset" CausesValidation="false" runat="server" Text="Reset" OnClick="_btnReset_Click" />
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Title="Task Details" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="TaskDetails.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Task.TaskDetails"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="OU" TagName="OrganiseUsers" Src="~/UserControls/OrganiseUsers.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

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
    </script>

    <asp:UpdateProgress ID="_updateProgressTask" runat="server">
        <ProgressTemplate>
            <div class="textBox">
                <img id="_imgLoading" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlError" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" CssClass="labelValue" runat="server" Text=""></asp:Label>
                        <asp:HiddenField ID="_hdnProjectId" runat="server" />
                        <asp:HiddenField ID="_hdnTaskId" runat="server" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <div id="taskdetails" runat="server">
    <table width="100%">
        <tr>
            <td class="sectionHeader">
                Task Details
            </td>
        </tr>
    </table>
    <table width="100%" border="0">
        <tr>
            <td>
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
            <td>
                <asp:Panel ID="_pnlCreateTask" runat="server" Width="99.9%" CssClass="bodyTab">
                    Task
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td valign="bottom">
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <table width="100%">
                                <tr>
                                    <td width="100px;" valign="top">
                                        <OU:OrganiseUsers ID="_TaskOU" runat="server" ButtonText="Attendees" ButtonTooltip="Attendees" OnUsersAdded="_TaskOU_UsersAdded">
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
                                                <asp:AsyncPostBackTrigger ControlID="_TaskOU" />
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
                                        Type
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="_ddlType" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                    <td class="boldTxt" width="100px;">
                                        Due Date
                                    </td>
                                    <td>
                                        <CC:CalendarControl ID="_ccDueDate" InvalidValueMessage="Valid Due Date Required"
                                            runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt" width="100px;">
                                        Completed
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkCompleted" Style="margin: -4px" runat="server" />
                                    </td>
                                    <td class="boldTxt" width="150px;">
                                        Public
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="_chkExposeToThirdParties" Style="margin: -4px" runat="server" />
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
    <asp:UpdatePanel ID="_updPnlTaskButtons" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 15px;">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;" id="_divBackButton" visible="false"
                                        runat="server">
                                        <asp:Button ID="_btnBack" runat="server" Text="Back" OnClick="_btnBack_Click" CausesValidation="false" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div id="_divNew" runat="server" class="button" style="float: right; text-align: center;">
                                        <asp:Button ID="_btnNew" CausesValidation="false" runat="server" Text="New" OnClick="_btnNew_Click" />
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
    </div>
</asp:Content>

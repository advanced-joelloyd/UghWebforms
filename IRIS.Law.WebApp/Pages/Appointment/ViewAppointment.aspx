<%@ Page Title="View Appointments" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="ViewAppointment.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Appointment.ViewAppointment" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="DC" TagName="DiaryCancellation" Src="~/UserControls/DiaryCancellation.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        var browser = navigator.appName;
        //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
        //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
        //on box-items. This is not yet implemented in IE so round the corners using javascript
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("div.button");
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(InitializeRequest);
        prm.add_endRequest(EndRequest);

        function InitializeRequest(sender, args) {
            if (prm.get_isInAsyncPostBack())
                args.set_cancel(true);
            //disable the search button to prevent the user from clicking multiple times 
            //while the request is being processed
            $("#<%=_btnSearch.ClientID %>").attr("disabled", true);
        }
        function EndRequest(sender, args) {
            $("#<%=_btnSearch.ClientID %>").attr("disabled", false);
        }

        function ResetControls() {
            $("#<%=_ccDate.DateTextBoxClientID%>").val("");
            document.getElementById("<%=_ddlUsers.ClientID%>").selectedIndex = 0;
            $("#<%=_grdSearchAppointmentList.ClientID%>").css("display", "none");
            $("#<%=_lblError.ClientID%>").text("");
            Page_ClientValidate();
            return false;
        }
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updatePnlError" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" align="left">
                View Appointments
            </td>
        </tr>
        <tr>
            <td width="100%">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch">
                    <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                        <tr>
                            <td style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                            <tr>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Select Users
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlUsers" runat="server">
                                                    </asp:DropDownList>
                                                    <asp:HiddenField ID="_hdnUser" runat="server" />
                                                </td>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Select Date
                                                </td>
                                                <td>
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <CC:CalendarControl ID="_ccDate" InvalidValueMessage="Invalid Date" ValidationGroup="AppointmentSearch"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="padding-right: 50px;">
                                <table width="100%">
                                    <tr>
                                        <td align="right" style="padding-right: 15px;">
                                            <table>
                                                <tr>
                                                    <td align="right">
                                                        <div class="button" style="text-align: center;">
                                                            <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetControls();" />
                                                        </div>
                                                    </td>
                                                    <td align="right">
                                                        <div class="button" style="text-align: center;">
                                                            <asp:Button ID="_btnSearch" runat="server" CausesValidation="True" OnClick="_btnSearch_Click"
                                                                Text="Search" ValidationGroup="AppointmentSearch" />
                                                        </div>
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
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                
                <asp:UpdatePanel ID="Update_pnlAppointmentSearch" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                            <DC:DiaryCancellation ID="_dcAppointmentDelete" runat="server" WhichPage="Appointment"
                                OnCancellationFinished="_dcAppointmentDelete_CancellationFinishedChanged" />
                            <asp:GridView ID="_grdSearchAppointmentList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                BorderWidth="0" GridLines="None" Width="99%" OnRowDataBound="_grdSearchAppointmentList_RowDataBound"
                                OnRowCommand="_grdSearchAppointmentList_RowCommand" DataKeyNames="Id" EmptyDataText="No appointment(s) found."
                                CssClass="successMessage" AllowSorting="true">
                                <Columns>
                                    <asp:TemplateField HeaderText="Id" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Id")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Subject" SortExpression="OccSpecificText">
                                        <ItemTemplate>
                                            <asp:LinkButton CommandName="select" CssClass="link" ID="_linkSubject" ToolTip='<%#DataBinder.Eval(Container.DataItem, "Subject")%>' 
                                            Text='<%#DataBinder.Eval(Container.DataItem, "Subject")%>' runat="server" CausesValidation="false"></asp:LinkButton>
                                            <asp:Label Visible="false" ID="_lblEdit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsEditable")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date" SortExpression="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblDate" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Start" SortExpression="StartTime">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblStartTime" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "StartTime")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Finish" SortExpression="EndTime">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblFinishDate" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "EndTime")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Venue" SortExpression="VenueDescription">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblVenue" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "VenueText")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes" SortExpression="OccurrenceNoteText">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblNotes" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'
                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Width="60px" HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:LinkButton CommandName="delete" CssClass="link" ID="_linkDelete" ToolTip="Delete Appointment"
                                                runat="server" CausesValidation="false">Delete</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="_dcAppointmentDelete" EventName="CancellationFinished" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="_odsSearchAppointment" runat="server" SelectMethod="SearchAppointment"
                    TypeName="IRIS.Law.WebApp.Pages.Appointment.ViewAppointment" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetAppointmentRowsCount" StartRowIndexParameterName="startRow"
                    OnSelected="_odsSearchAppointment_Selected" SortParameterName="sortBy">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="_ddlUsers" Name="user" PropertyName="Text" />
                        <asp:ControlParameter ControlID="_ccDate" Name="date" PropertyName="DateText" />
                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
            </td>
        </tr>
    </table>
</asp:Content>

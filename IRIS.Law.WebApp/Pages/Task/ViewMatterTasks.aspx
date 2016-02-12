<%@ Page Title="View Matter Tasks" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="ViewMatterTasks.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Task.ViewMatterTasks" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="DC" TagName="DiaryCancellation" Src="~/UserControls/DiaryCancellation.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
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
            $("#<%=_ccDateFrom.DateTextBoxClientID%>").val("");
            //$("#<%=_ccDateTo.DateTextBoxClientID%>").val("");
            $("#<%=_ddlStatus.ClientID%>").val("Outstanding");
            document.getElementById("<%=_ddlUsers.ClientID%>").selectedIndex = 0;
            //$("#<%=_ddlUsers.ClientID%>").val("");
            $("#<%=_grdSearchTaskList.ClientID%>").css("display", "none");
            $("#<%=_lblError.ClientID%>").text("");

            var d = new Date();
            var curr_date = d.getDate();
            var curr_month = d.getMonth() + 1;
            var curr_year = d.getFullYear();
            var curr_formatdate = "" + curr_date + "/" + curr_month + "/" + curr_year + "";
            $("#<%=_ccDateTo.DateTextBoxClientID%>").val(curr_formatdate);
            
            
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
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_grdSearchTaskList" />
                        <asp:AsyncPostBackTrigger ControlID="_dcTaskDelete" EventName="CancellationFinished" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" align="left">
                View Matter Tasks
            </td>
        </tr>
        <tr>
            <td width="100%">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch">
                    <table width="100%">
                        <tr>
                            <td colspan="4">
                                <CliMat:ClientMatterDetails runat="server" ID="_cliMatDetails" ValidationGroup="TaskSearch"
                                    OnMatterChanged="_cliMatDetails_MatterChanged" EnableValidation="true" />
                            </td>
                        </tr>
                    </table>
                    <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                        <tr>
                            <td style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                            <tr>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Status
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlStatus" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Select Users
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlUsers" runat="server">
                                                    </asp:DropDownList>
                                                    <asp:HiddenField ID="_hdnUser" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    From
                                                </td>
                                                <td>
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <CC:CalendarControl ID="_ccDateFrom" InvalidValueMessage="Invalid From Date" ValidationGroup="TaskSearch"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td class="boldTxt">
                                                    To
                                                </td>
                                                <td>
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <CC:CalendarControl ID="_ccDateTo" InvalidValueMessage="Invalid To Date" ValidationGroup="TaskSearch"
                                                                    runat="server"  />
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
                                                                Text="Search" ValidationGroup="TaskSearch" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                
                   
                <asp:UpdatePanel ID="_updPnlTaskSearch" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                            <DC:DiaryCancellation ID="_dcTaskDelete" runat="server" WhichPage="Task" OnCancellationFinished="_dcTaskDelete_CancellationFinishedChanged" />
                            <asp:GridView ID="_grdSearchTaskList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                BorderWidth="0" GridLines="None" Width="99%" DataKeyNames="Id" EmptyDataText="No task(s) found."
                                CssClass="successMessage" AllowSorting="true" OnRowCommand="_grdSearchTaskList_RowCommand" OnRowDataBound="_grdSearchTaskList_RowDataBound">
                                <Columns>
                                <asp:TemplateField HeaderText="Id" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Id")%>'></asp:Label>
                                            <asp:Label ID="_lblIsLimitationTask" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsLimitationTask")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Subject" HeaderStyle-Width="35%" SortExpression="OccSpecificText">
                                        <ItemTemplate>
                                         <asp:LinkButton CommandName="select" CssClass="link" ID="_linkSubject" Text='<%#DataBinder.Eval(Container.DataItem, "Subject")%>'
                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Subject")%>' runat="server" CausesValidation="false">Edit</asp:LinkButton>
                                            <asp:Label Visible="false" ID="_lblEdit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsEditable")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Due Date" HeaderStyle-Width="15%" SortExpression="OccDueDate">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblDueDate" runat="server" Text='<%# Eval("DueDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Attendee" HeaderStyle-Width="25%" SortExpression="AttendeeName">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblAttendeesName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AttendeesName")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-Width="20%" SortExpression="OccStatus">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StatusDesc")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemStyle Width="60px" HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:LinkButton CommandName="delete" CssClass="link" ID="_linkDelete" ToolTip="Delete Task"
                                                runat="server" CausesValidation="false">Delete</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="_dcTaskDelete" EventName="CancellationFinished" />
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="_odsSearchTask" runat="server" SelectMethod="SearchTask"
                    TypeName="IRIS.Law.WebApp.Pages.Task.ViewMatterTasks" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetTaskRowsCount" SortParameterName="sortBy" StartRowIndexParameterName="startRow" OnSelected="_odsSearchTask_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="_ddlStatus" Name="taskStatus" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="_ddlUsers" Name="user" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="_ccDateFrom" Name="fromDate" PropertyName="DateText" />
                        <asp:ControlParameter ControlID="_ccDateTo" Name="toDate" PropertyName="DateText" />
                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
            </td>
        </tr>
    </table>
</asp:Content>

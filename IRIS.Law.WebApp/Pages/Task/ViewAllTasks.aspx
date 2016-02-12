<%@ Page Title="View All Tasks" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="ViewAllTasks.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Task.ViewAllTasks" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="DC" TagName="DiaryCancellation" Src="~/UserControls/DiaryCancellation.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
            document.getElementById("<%=_ddlStatus.ClientID%>").selectedIndex = 0;
            document.getElementById("<%=_ddlUsers.ClientID%>").selectedIndex = 0;
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

        function CancelViewMattersPopup() {
            $find('_mpeViewMattersBehavior').hide();
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
                View All Tasks
            </td>
        </tr>
        <tr>
            <td width="100%">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch">
                    <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                        <tr>
                            <td style="width: 100%">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table cellpadding="1" cellspacing="1" border="0" style="width: 100%;">
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
                                                    <CC:CalendarControl ID="_ccDateFrom" InvalidValueMessage="Invalid From Date" ValidationGroup="TaskSearch"
                                                        runat="server" />
                                                </td>
                                                <td class="boldTxt">
                                                    To
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccDateTo" InvalidValueMessage="Invalid To Date"
                                                        ValidationGroup="TaskSearch" runat="server" />
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
                <input id="_btnDummy" runat="server" type="button" value="." style="height: 1px;
                    width: 1px; display: none;" disabled="disabled" />
                <ajaxToolkit:ModalPopupExtender ID="_mpeViewMatters" runat="server" BackgroundCssClass="modalBackground"
                    DropShadow="true" PopupControlID="_pnlViewMatters" OnCancelScript="return false;"
                    TargetControlID="_btnDummy" BehaviorID="_mpeViewMattersBehavior">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="_pnlViewMatters" runat="server" Style="background-color: #ffffff;display: none;padding:2px;" 
                Width="400px">
                    <div align="right">
                        <a id="linkClose" onclick="CancelViewMattersPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                    </div>
                    <div class="sectionHeader">
                        Matters
                    </div>
                    <asp:UpdatePanel ID="_updMatters" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Repeater ID="_rptMatters" runat="server" OnItemCommand="_rptMatters_ItemCommand">
                                <ItemTemplate>
                                    <div class="gridViewRow">
                                        <asp:LinkButton ID="_lnkMatterRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MatterDesc")%>'
                                            CommandName="ViewMatters" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>'></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="gridViewRowAlternate">
                                        <asp:LinkButton ID="_lnkMatterRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MatterDesc")%>'
                                            CommandName="ViewMatters" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>'></asp:LinkButton>
                                    </div>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_grdSearchTaskList" EventName="RowCommand" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
                
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
                                    <asp:TemplateField HeaderText="Subject" HeaderStyle-Width="25%" SortExpression="OccSpecificText">
                                        <ItemTemplate>
                                            <asp:Label Visible="false" ID="_lblEdit" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsEditable")%>'></asp:Label>
                                            <asp:LinkButton CommandName="select" CssClass="link" ID="_linkSubject" ToolTip='<%#DataBinder.Eval(Container.DataItem, "Subject")%>'
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Subject")%>' runat="server" CausesValidation="false">Edit</asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Due Date" SortExpression="RecordedDueDate">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblDueDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "RecordedDueDate")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-Width="10%" SortExpression="OccStatusDesc">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StatusDesc")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="% Complete" HeaderStyle-Width="9%" SortExpression="OccProgress">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblPercentComplete" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Progress")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes" HeaderStyle-Width="15%" SortExpression="OccurrenceNoteText">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblNotes" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'
                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Matter" HeaderStyle-Width="20%" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblMatterRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Matters")%>'
                                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Matters")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Matter" HeaderStyle-Width="20%" SortExpression="OccurrenceMatter">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="_lnkMatterRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Matters")%>'
                                                CommandName="ViewMatters" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Matters")%>'></asp:LinkButton>
                                            <asp:HiddenField ID="_hdnMatter" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Matters")%>'  />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Matter" HeaderStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Panel ID="_panelMatter" runat="server">
                                            </asp:Panel>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>--%>
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
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="_odsSearchTask" runat="server" SelectMethod="SearchTask"
                    TypeName="IRIS.Law.WebApp.Pages.Task.ViewAllTasks" EnablePaging="True" MaximumRowsParameterName="pageSize"
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

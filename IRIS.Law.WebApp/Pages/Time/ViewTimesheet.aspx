<%@ Page Title="Your unposted time" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="ViewTimesheet.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Time.ViewTimesheet" %>

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
            Nifty("div.buttonDisabled");
        }

        //select/unselect all the chekboxes and enable/disable the post button
        function SelectUnselectAll(sender) {
            $("#<%=_grdTodaysTimesheet.ClientID %> input:checkbox").each(function() {
                this.checked = sender.checked;
            });
            //enable/disable the post button
            $("#<%=_btnPostSelected.ClientID %>").attr("disabled", !sender.checked);

            if (!sender.checked) {
                $("#<%=_btnPostSelected.ClientID %>").addClass("buttonDisabled");
            }
            else {
                $("#<%=_btnPostSelected.ClientID %>").removeClass("buttonDisabled");
            }
        }

        //uncheck all the checkboxes when the pg loads 
        Sys.Application.add_load(function() {
            $("#<%=_grdTodaysTimesheet.ClientID %> input:checkbox").each(function() {
                this.checked = false;
            });
        });

        //selects the main checkbox if all the child checkboxes are selected
        //unselect it if any of the child checkboxes are not selected
        //disables the post time entry button if no entry is selected
        function SelectUnselectHeaderCheckbox() {
            var checkboxes = $("#<%=_grdTodaysTimesheet.ClientID %> input:checkbox");
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
            //enable/disable the post button
            $("#<%=_btnPostSelected.ClientID %>").attr("disabled", !isSelected);

            if (!isSelected) {
                $("#<%=_btnPostSelected.ClientID %>").addClass("buttonDisabled");
            }
            else {
                $("#<%=_btnPostSelected.ClientID %>").removeClass("buttonDisabled");
            }
        }
    </script>

    <table width="99%">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlTimesheetError" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                Your unposted time
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlTimesheet" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="_grdTodaysTimesheet" runat="server" AutoGenerateColumns="False"
                            GridLines="None" Width="100%" AllowPaging="true" ShowFooter="true" CellPadding="2"
                            OnRowCommand="_grdTodaysTimesheet_RowCommand" OnRowDataBound="_grdTodaysTimesheet_RowDataBound"
                            DataKeyNames="TimeId,ProjectId" DataSourceID="_odsTimesheet" CssClass="successMessage"
                            EmptyDataText="No unposted entries for today." AllowSorting="true">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="_chkSelectAll" runat="server" Checked="false" onclick="SelectUnselectAll(this);" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="_chkSelect" runat="server" onclick="SelectUnselectHeaderCheckbox();" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="_lnkBtnEditTimeEntry" CommandName="select" CssClass="link" runat="server">Edit</asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="matRef" HeaderText="Matter Reference">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="_lnkBtnEditMatter" CssClass="link" runat="server" CommandName="EditMatter"
                                            Text='<%# DataBinder.Eval(Container.DataItem, "MatterReference")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="matDescription" HeaderText="Matter Description">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="_lblMatterDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MatterDescription")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "MatterDescription")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="TimeTypeDescription" HeaderText="Time Type">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblTimeType" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TimeTypeDescription")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="TimeDate" SortExpression="TimeDate" DataFormatString="{0:d}" HeaderText="Date" HeaderStyle-HorizontalAlign="Left" />
                                <asp:TemplateField SortExpression="TimeComment" HeaderText="Notes">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblNotes" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TimeComments")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "TimeComments")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="TimeElapsed" HeaderText="Time">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="_lblTime" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="TimeElapsed" HeaderText="Units">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="_lblUnits" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle Width="5%" HorizontalAlign="Right" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="TimeCharge" HeaderText="Charge">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblCharge" runat="server" Text='<%# "&pound;" +  Eval("TimeCharge", "{0:0.00}")%>'
                                            Style="padding-right: 5px;"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="_lblTotal" runat="server" Style="padding-right: 5px;"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                        <asp:ObjectDataSource ID="_odsTimesheet" runat="server" SelectMethod="BindTodaysTimesheet"
                            TypeName="IRIS.Law.WebApp.Pages.Time.ViewTimesheet" EnablePaging="True" MaximumRowsParameterName="pageSize"
                            SelectCountMethod="GetTimesheetRowsCount" StartRowIndexParameterName="startRow"
                            OnSelected="_odsTimesheet_Selected" SortParameterName="sortBy">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="_hdnRefresh" Type="Boolean" Name="forceRefresh"
                                    PropertyName="Value" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnPostSelected" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="_updPnlPostButton" runat="server">
        <ContentTemplate>
            <div class="buttonDisabled" style="float: right; margin-right: 15px; text-align: center;
                width: 120px;">
                <asp:Button ID="_btnPostSelected" runat="server" Text="Post Selected" EnableTheming="false"
                    CausesValidation="true" Enabled="false" OnClick="_btnPostSelected_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="_btnSubmit" runat="server" Text="Submit" Enabled="false" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="_mdlPopUpCofirmationBox" runat="server" TargetControlID="_btnSubmit"
        PopupControlID="_pnlModalPopUpCofirmationBox" CancelControlID="_btnCancel" BackgroundCssClass="modalBackground"
        DropShadow="true" />
    <asp:Panel ID="_pnlModalPopUpCofirmationBox" runat="server" Style="background-color: #ffffff;
        display: none;" Width="350px">
        <table width="100%" border="0">
            <tr>
                <td class="sectionHeader" colspan="2">
                    Warning
                </td>
            </tr>
            <tr>
                <td class="labelValue" colspan="2">
                    You are posting time sheets that contain entries outside of the current financial
                    period. Do you wish to continue?
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <div class="button" style="text-align: center;">
                        <asp:Button ID="_btnOk" runat="server" Text="Ok" CausesValidation="false" OnClick="_btnOk_Click" />
                    </div>
                </td>
                <td>
                    <div class="button" style="text-align: center;">
                        <asp:Button ID="_btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewDraftBills.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.ViewDraftBills"
    Title="View Draft Bill(s)" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentViewDraftBills" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
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
            Nifty("div.buttonDisabled");
        }

        function SelectUnselectAll(sender) {
            var objChequeRequest = document.getElementById('<%=_grdDraftBills.ClientID %>');
            if (objChequeRequest != null) {
                var checkBoxes = objChequeRequest.getElementsByTagName("input");

                for (var i = 0; i < checkBoxes.length; i++) {
                    if (objChequeRequest.getElementsByTagName("input").item(i).disabled == false) {
                        objChequeRequest.getElementsByTagName("input").item(i).checked = sender.checked;
                    }
                }

                //enable/disable submit button
                $("#<%=_btnSubmit.ClientID %>").attr("disabled", !sender.checked);

                if (!sender.checked) {
                    $("#<%=_btnSubmit.ClientID %>").addClass("buttonDisabled");
                }
                else {
                    $("#<%=_btnSubmit.ClientID %>").removeClass("buttonDisabled");
                }
            }
        }

        //selects the main checkbox if all the child checkboxes are selected
        //unselect it if any of the child checkboxes are not selected
        //disables the post time entry button if no entry is selected
        function EnableDisableButtons(gridViewId) {
            var obj = document.getElementById(gridViewId);
            var checkboxes = obj.getElementsByTagName("input");
            var allCheckboxesSelected = true;
            var isSelected = false;

            for (i = 1; i < checkboxes.length; i++) {
                if (obj.getElementsByTagName("input").item(i).type == "checkbox") {
                    if (obj.getElementsByTagName("input").item(i).disabled == false) {
                        if (!checkboxes[i].checked) {
                            allCheckboxesSelected = false;
                        }
                        else {
                            isSelected = true;
                        }
                    }
                }
            }

            // if any of the checkbox is unchecked in any row in grid view, 
            // then uncheck header checkbox
            checkboxes[0].checked = allCheckboxesSelected;

            //enable/disable the submit button
            $("#<%=_btnSubmit.ClientID %>").attr("disabled", !isSelected);

            if (!isSelected) {
                $("#<%=_btnSubmit.ClientID %>").addClass("buttonDisabled");                
            }
            else {
                $("#<%=_btnSubmit.ClientID %>").removeClass("buttonDisabled");                
            }
        }                 
       
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:Label ID="_lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                View Draft Bill(s)
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPanelDraftBills" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="_grdDraftBills" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                            BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" OnRowDataBound="_grdDraftBills_RowDataBound"
                            EmptyDataText="There are no results to display.">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="_chkBxSelectAll" ToolTip="Select All" runat="server" onclick="SelectUnselectAll(this);"
                                            Text="" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="_chkBxSelect" runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="DraftBillId" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDraftBillId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DraftBillId")%>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("DraftBillDate","{0:dd/MM/yyyy}") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Matter Reference">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblMatterReference" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MatterReference")%>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="User">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblUser" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DraftBillDescription")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "DraftBillDescription")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSubmit" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="_odsDraftBills" runat="server" SelectMethod="LoadDraftBills"
                    TypeName="IRIS.Law.WebApp.Pages.Accounts.ViewDraftBills" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetDraftBillsRowsCount" StartRowIndexParameterName="startRow"
                    OnSelected="_odsDraftBills_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                            Type="Boolean" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    <div class="buttonDisabled" style="text-align: center; float: right;">
        <asp:Button ID="_btnSubmit" runat="server" Text="Submit" EnableTheming="false" Enabled="false" OnClick="_btnSubmit_Click" />
    </div>
    <ajaxToolkit:ConfirmButtonExtender ID="_cnfrmBtnSumitDraftBill" runat="server" TargetControlID="_btnSubmit"
        DisplayModalPopupID="_mdlPopUpCofirmationBox" />
    <ajaxToolkit:ModalPopupExtender ID="_mdlPopUpCofirmationBox" runat="server" TargetControlID="_btnSubmit"
        PopupControlID="_pnlModalPopUpCofirmationBox" CancelControlID="_btnCancel" BackgroundCssClass="modalBackground"
        DropShadow="true" />
    <asp:Panel ID="_pnlModalPopUpCofirmationBox" runat="server" Style="background-color: #ffffff;
        display: none;" Width="350px">
        <table width="100%" border="0">
            <tr>
                <td class="sectionHeader" colspan="2">
                    Submit Draft Bill?
                </td>
            </tr>
            <tr>
                <td class="labelValue" colspan="2">
                    Are you sure you wish to submit this Draft Bill to accounts?
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <div class="button" style="text-align: center;">
                        <asp:Button ID="_btnOk" runat="server" Text="Ok" CausesValidation="true" OnClick="_btnSubmit_Click" />
                    </div>
                </td>
                <td>
                    <div class="button" style="text-align: center;">
                        <asp:Button ID="_btnCancel" runat="server" Text="Cancel" CausesValidation="true" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
</asp:Content>

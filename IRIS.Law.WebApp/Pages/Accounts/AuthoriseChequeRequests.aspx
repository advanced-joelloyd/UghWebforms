<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="AuthoriseChequeRequests.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.AuthoriseChequeRequests"
    Title="Authorise Cheque Request(s)" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentViewOfficeChequeRequests" ContentPlaceHolderID="_cphMain"
    runat="server">  

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
            Nifty("div.buttonDisabled");            
        }

        var grdClientChqReqCredit = "<%=_grdClientChequeRequestsCredit.ClientID %>";
        var grdClientChqReqDebit = "<%=_grdClientChequeRequestsDebit.ClientID %>";
        var grdOfficeChqReq = "<%=_grdOfficeChequeRequests.ClientID %>";
        function SelectUnselectAll(sender, gridviewId) {
            var objChequeRequest = document.getElementById(gridviewId);
            if (objChequeRequest != null) {
                var checkBoxes = objChequeRequest.getElementsByTagName("input");

                for (var i = 0; i < checkBoxes.length; i++) {
                    if (objChequeRequest.getElementsByTagName("input").item(i).disabled == false) {
                        objChequeRequest.getElementsByTagName("input").item(i).checked = sender.checked;
                    }
                }

                //enable/disable the delete and authorise buttons
                $("#<%=_btnDelete.ClientID %>").attr("disabled", !sender.checked);
                $("#<%=_btnAuthorise.ClientID %>").attr("disabled", !sender.checked);

                if (!sender.checked) {
                    $("#<%=_btnAuthorise.ClientID %>").addClass("buttonDisabled");
                    $("#<%=_btnDelete.ClientID %>").addClass("buttonDisabled");
                }
                else {
                    $("#<%=_btnAuthorise.ClientID %>").removeClass("buttonDisabled");
                    $("#<%=_btnDelete.ClientID %>").removeClass("buttonDisabled");
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

            //enable/disable the delete and authorise buttons
            $("#<%=_btnDelete.ClientID %>").attr("disabled", !isSelected);
            $("#<%=_btnAuthorise.ClientID %>").attr("disabled", !isSelected);

            if (!isSelected) {
                $("#<%=_btnAuthorise.ClientID %>").addClass("buttonDisabled");
                $("#<%=_btnDelete.ClientID %>").addClass("buttonDisabled");
            }
            else {
                $("#<%=_btnAuthorise.ClientID %>").removeClass("buttonDisabled");
                $("#<%=_btnDelete.ClientID %>").removeClass("buttonDisabled");
            }
        }      
    </script>

    
    <table width="100%" border="0">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="sectionHeader">
                Authorise Cheque Request(s)
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnRefreshClientChequeRequestDebit" runat="server" Value="true" />
                    <asp:HiddenField ID="_hdnRefreshClientChequeRequestCredit" runat="server" Value="true" />
                </div>
            </td>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnRefreshOfficeChequeRequest" runat="server" Value="true" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <asp:HiddenField ID="_hdnClientChequeRequestDebit" runat="server" />
                    <asp:HiddenField ID="_hdnClientChequeRequestCredit" runat="server" />
                </div>
            </td>
        </tr>
        <tr> 
            <td colspan="2">
                <ajaxToolkit:TabContainer ID="_tcAuthoriseChequeRequests" AutoPostBack="true" runat="server" 
                    CssClass="ajax__tab_xp2" Width="100%" ActiveTabIndex="0" OnActiveTabChanged="_tcAuthoriseChequeRequests_ActiveTabChanged">
                     <ajaxToolkit:TabPanel runat="server" ID="_pnlClientChequeRequestsDebit" HeaderText="Client Debit">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelClientChequeRequestsDebit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdClientChequeRequestsDebit" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" OnRowDataBound="_grdClientChequeRequestsDebit_RowDataBound"
                                                    OnRowCommand="_grdClientChequeRequestsDebit_RowCommand" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="_chkBxSelectAll" ToolTip="Select All" runat="server" onclick="SelectUnselectAll(this,grdClientChqReqDebit);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="_chkBxSelect" runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Edit">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="_lnkBtnEdit" CssClass="link" runat="server" ToolTip="Edit" CommandName="edit"
                                                                    Text="Edit"></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ChequeRequestId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblChequeRequestId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestId")%>'>
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
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("ChequeRequestDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="User Ref">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblUseRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Earner">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Bank">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBank" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" Width="10%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestAmount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" Width="10%" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnDelete" />
                                    <asp:AsyncPostBackTrigger ControlID="_btnAuthorise" />
                                    <asp:AsyncPostBackTrigger ControlID="_tcAuthoriseChequeRequests" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsClientChequeRequestsDebit" runat="server" SelectMethod="LoadUnauthorisedClientChequeRequestsDebit"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.AuthoriseChequeRequests" EnablePaging="True"
                                MaximumRowsParameterName="pageSize" SelectCountMethod="GetUnauthorisedClientChequeRequestsDebitRowsCount"
                                StartRowIndexParameterName="startRow" OnSelected="_odsUnauthorisedClientChequeRequestsDebit_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnRefreshClientChequeRequestDebit" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlClientChequeRequestsCredit" HeaderText="Client Credit">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelClientChequeRequestsCredit" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdClientChequeRequestsCredit" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" OnRowDataBound="_grdClientChequeRequestsCredit_RowDataBound"
                                                    OnRowCommand="_grdClientChequeRequestsCredit_RowCommand" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="_chkBxSelectAll" ToolTip="Select All" runat="server" onclick="SelectUnselectAll(this,grdClientChqReqCredit);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="_chkBxSelect" runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Edit">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="_lnkBtnEdit" CssClass="link" runat="server" ToolTip="Edit" CommandName="edit"
                                                                    Text="Edit"></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="gridSpacing" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ChequeRequestId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblChequeRequestId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestId")%>'>
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
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("ChequeRequestDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="gridSpacing"  />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="User Ref">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblUseRef" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Earner">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="20%" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Client Account">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBank" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="gridSpacing" />
                                                            <HeaderStyle HorizontalAlign="Left"  />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Clearance Type">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblClearanceType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ClearanceTypeDesc")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "ClearanceTypeDesc")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="gridSpacing" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Branch">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBranch" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MatterBranchRef")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "MatterBranchRef")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left"  />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestAmount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" Width="10%" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnDelete" />
                                    <asp:AsyncPostBackTrigger ControlID="_btnAuthorise" />
                                    <asp:AsyncPostBackTrigger ControlID="_tcAuthoriseChequeRequests" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsClientChequeRequestsCredit" runat="server" SelectMethod="LoadUnauthorisedClientChequeRequestsCredit"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.AuthoriseChequeRequests" EnablePaging="True"
                                MaximumRowsParameterName="pageSize" SelectCountMethod="GetUnauthorisedClientChequeRequestsCreditRowsCount"
                                StartRowIndexParameterName="startRow" OnSelected="_odsUnauthorisedClientChequeRequestsCredit_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnRefreshClientChequeRequestCredit" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" ID="_pnlOfficeChequeRequests" HeaderText="Office">
                        <ContentTemplate>
                            <asp:UpdatePanel ID="_updPanelOfficeChequeRequests" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="_grdOfficeChequeRequests" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                    BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage" OnRowCommand="_grdOfficeChequeRequests_RowCommand"
                                                    OnRowDataBound="_grdOfficeChequeRequests_RowDataBound" EmptyDataText="There are no results to display.">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="_chkBxSelectAll" ToolTip="Select All" runat="server" onclick="SelectUnselectAll(this,grdOfficeChqReq);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="_chkBxSelect" runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Edit">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="_lnkBtnEdit" CssClass="link" runat="server" ToolTip="Edit" CommandName="edit"
                                                                    Text="Edit"></asp:LinkButton>
                                                            </ItemTemplate>
                                                           <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Anticipated">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="_chkBxAnticipated" Enabled="false" runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Anticipated" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAnticipated" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsChequeRequestAnticipated")%>' />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ChequeRequestId" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblChequeRequestId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestId")%>'>
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
                                                        <asp:TemplateField HeaderText="Date">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDate" runat="server" Text='<%# Eval("ChequeRequestDate","{0:dd/MM/yyyy}") %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="User Ref">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblUseName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "UserName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Fee Earner">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerReference")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestDescription")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Bank">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblBank" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "BankName")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChequeRequestAmount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Right" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VAT Rate">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblVATRate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "VATRate")%>'
                                                                    ToolTip='<%#DataBinder.Eval(Container.DataItem, "VATRate")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="VAT Amount">
                                                            <ItemTemplate>
                                                                <asp:Label ID="_lblVATAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "VATAmount")%>'></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnDelete" />
                                    <asp:AsyncPostBackTrigger ControlID="_btnAuthorise" />
                                    <asp:AsyncPostBackTrigger ControlID="_tcAuthoriseChequeRequests" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="_odsOfficeChequeRequests" runat="server" SelectMethod="LoadUnauthorisedOfficeChequeRequests"
                                TypeName="IRIS.Law.WebApp.Pages.Accounts.AuthoriseChequeRequests" EnablePaging="True"
                                MaximumRowsParameterName="pageSize" SelectCountMethod="GetUnauthorisedOfficeChequeRequestsRowsCount"
                                StartRowIndexParameterName="startRow" OnSelected="_odsUnauthorisedOfficeChequeRequests_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_hdnRefreshOfficeChequeRequest" Name="forceRefresh"
                                        PropertyName="Value" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="right" style="padding-right: 15px;" colspan="2">
                <table>
                    <tr>
                        <td align="right">
                            <div class="buttonDisabled" style="text-align: center;">
                                <asp:UpdatePanel ID="_upPanelDelete" runat="server">
                                    <ContentTemplate>
                                        <asp:Button ID="_btnDelete" runat="server" Text="Delete" EnableTheming="false" Enabled="false" OnClick="_btnDelete_Click" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                        <td align="right">
                            <div class="buttonDisabled" style="text-align: center;">
                                <asp:UpdatePanel ID="_upPanelAuthorise" runat="server">
                                    <ContentTemplate>
                                        <asp:Button ID="_btnAuthorise" runat="server" Text="Authorise" EnableTheming="false" Enabled="false" OnClick="_btnAuthorise_Click" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>    
</asp:Content>


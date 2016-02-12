<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchDocumentFiles.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.SearchDocumentFiles" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script language="javascript">

    function ResetSearchDocFilesControls(calledFromParentPage) {
        $("#<%=_lblError.ClientID %>").text("");
        document.getElementById("<%=_txtSearchText.ClientID%>").value = "";
        document.getElementById("<%=_ddlFileType.ClientID%>").selectedIndex = document.getElementById("<%=_ddlFileType.ClientID%>").length - 1;
        document.getElementById("<%=_chkDeepSearch.ClientID%>").checked = true;
        document.getElementById("<%=_chkMatchCase.ClientID%>").checked = false;
        document.getElementById("<%=_chkSearchSubfolders.ClientID%>").checked = false;
        if (document.getElementById("<%= _grdDocSearch.ClientID%>") != null) {
            document.getElementById("<%= _grdDocSearch.ClientID%>").style.display = "none";
        }
        if (calledFromParentPage == "1") {
            return true;
        }
        else {
            return false;
        }
    }

    function CancelSearchDocFilesPopupClick() {
        return false;
    }
    function CancelSearchDocFilesPopup() {
        $find('_modalpopupDocSearchBehavior').hide();
    }

    function ShowProgress1() {
        $("#<%=_updateProgressDocSearch1.ClientID %>").show();
    }

    function HideProgress1() {
        $("#<%=_updateProgressDocSearch1.ClientID %>").hide();
    }

    function GetDocument1(srcFile) {
        // Create an IFRAME.
        var iframe = document.createElement("iframe");
        
        // Point the IFRAME to GenerateFile, with the
        //   desired region as a querystring argument.
        iframe.src = srcFile;

        // This makes the IFRAME invisible to the user.
        iframe.style.display = "none";

        // Add the IFRAME to the page.  This will trigger
        //   a request to GenerateFile now.
        document.body.appendChild(iframe);
    } 
</script>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <% if (_btnSearchDocuments.Enabled) { %>
            <div class="button" style="width: 172px; text-align: center;">
            <% } else {  %> 
            <div class="buttonDisabled" style="width: 172px; text-align: center;">
            <%} %>
                <asp:Button ID="_btnSearchDocuments" runat="server" CausesValidation="False" OnClientClick="return ResetSearchDocFilesControls('1');"
                    Text="Search For Document Files" ToolTip="Search For Document Files" Enabled="false"
                    OnClick="_btnSearchDocuments_Click" />
            </div>
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupDocSearch" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlDocSearch" OnCancelScript="javascript:CancelSearchDocFilesPopupClick();"
                TargetControlID="_btnSearchDocuments" CancelControlID="_btnCancel" BehaviorID="_modalpopupDocSearchBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<table style="float: center;" cellpadding="0" cellspacing="0">
    <tr>
        <td align="center">
            <asp:Panel ID="_pnlDocSearch" runat="server" Style="background-color: #ffffff" Width="800px"
                Height="500px">
                <table width="99%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="right" colspan="4">
                            <a id="linkClose" onclick="CancelSearchDocFilesPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="left">
                            <asp:UpdatePanel ID="_updPnlErrorMessage" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnSearchFiles" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="_grdDocSearch" EventName="RowCommand" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left" colspan="4">
                            Search For Files
                        </td>
                    </tr>
                    <tr style="height: 5px;">
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" width="100%" align="left">
                            <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearchFiles">
                                <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                                    <tr>
                                        <td class="boldTxt" style="width: 125px;">
                                            Search for
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="_txtSearchText"></asp:TextBox>
                                        </td>
                                        <td class="boldTxt" style="width: 125px;">
                                            Document Type
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlFileType" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" class="boldTxt">
                                            <asp:CheckBox ID="_chkDeepSearch" runat="server" Text="Deep Search within files" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" class="boldTxt">
                                            <asp:CheckBox ID="_chkMatchCase" runat="server" Text="Match Case" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" class="boldTxt">
                                            <asp:CheckBox ID="_chkSearchSubfolders" Enabled="false" runat="server" Text="Search Subfolders" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="padding-right: 50px;" colspan="4">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" style="padding-right: 15px;">
                                                        <table>
                                                            <tr>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;">
                                                                        <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetSearchDocFilesControls('0');" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                        <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="buttonDisabled" style="text-align: center;">
                                                                        <asp:Button ID="_btnSearchFiles" runat="server" CausesValidation="False" Text="Search" EnableTheming="false"
                                                                            OnClick="_btnSearchFiles_Click" />
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
                </table>
                <table width="99%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left">
                            <asp:UpdateProgress ID="_updateProgressDocSearch1" runat="server" AssociatedUpdatePanelID="_updPnlSearchDocFiles">
                                <ProgressTemplate>
                                    <div class="textBox">
                                        <img id="Img1" runat="server" alt="" src="~/Images/indicator.gif" />&nbsp;Loading...
                                    </div>
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                            <br />
                            <asp:UpdatePanel ID="_updPnlSearchDocFiles" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div style="height: 250px; overflow: auto;">
                                        <asp:GridView ID="_grdDocSearch" runat="server" AutoGenerateColumns="False" GridLines="None"
                                            DataKeyNames="Id" OnRowDataBound="_grdDocSearch_RowDataBound" OnRowCommand="_grdDocSearch_RowCommand"
                                            Width="99%" EmptyDataText="No document(s) found." CssClass="successMessage">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-Width="30%">
                                                    <HeaderTemplate>
                                                        Description
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:LinkButton CommandName="fileDownload" CssClass="link" ID="_linkFileDownload" OnClientClick="ShowProgress1();"
                                                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>' runat="server"
                                                            CausesValidation="false" Text='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'>                                        
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Created Date
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'
                                                            ToolTip='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Fee Earner
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'
                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnSearchFiles" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:ObjectDataSource ID="odsSearchForMatterDocument" runat="server" SelectMethod="GetSearchForMatterDocuments"
                                TypeName="IRIS.Law.WebApp.UserControls.SearchDocumentFiles" EnablePaging="True"
                                MaximumRowsParameterName="pageSize" SelectCountMethod="GetMatterDocumentsRowsCount"
                                StartRowIndexParameterName="startRow" OnSelected="odsSearchForMatterDocument_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_txtSearchText" Name="searchText" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_ddlFileType" Name="fileType" PropertyName="SelectedIndex"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="_chkDeepSearch" Name="deepSearch" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="_chkMatchCase" Name="matchCase" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="_chkSearchSubfolders" Name="searchSubFolders" PropertyName="Checked"
                                        Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>

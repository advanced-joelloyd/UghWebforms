<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="ViewStyle.aspx.cs" Inherits="Iris.ILB.Web.Pages.SiteConfig.ViewStyle"
    Title="View Styles" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

    <script language="javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        }

        function showSaveAsModalPopupViaClient(cssFile) {
            if (cssFile != null) {
                var modalPopupBehavior = $find('_mpeSaveCSSBehavior');
                document.getElementById('<%=_hdnCSSFile.ClientID%>').value = cssFile;
                document.getElementById('<%=_lblMessage.ClientID%>').innerText = "";
                document.getElementById('<%=_lblHeader.ClientID%>').innerText = " Copy style '" + cssFile + "' as";
                document.getElementById("<%=_txtCSSName.ClientID%>").value = "";
                modalPopupBehavior.show();
            }
            return false;
        }

        function CancelSaveCSSPopup() {
            $find('_mpeSaveCSSBehavior').hide();
        }
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnOK" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" align="left">
                View Stylesheets
            </td>
        </tr>
    </table>
    <br />
    <input id="_btnDummy" runat="server" type="button" value="." style="height: 1px;
        width: 1px; display: none;" disabled="disabled" />
    <ajaxToolkit:ModalPopupExtender ID="_mpeSaveCSS" runat="server" BackgroundCssClass="modalBackground"
        DropShadow="true" PopupControlID="_pnlSaveCSS" OnCancelScript="return false;"
        CancelControlID="_btnCancel" TargetControlID="_btnDummy" BehaviorID="_mpeSaveCSSBehavior">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel ID="_pnlSaveCSS" runat="server" Style="background-color: #ffffff; display: none;
        padding: 2px;" Width="400px">
        <table width="100%">
            <tr>
                <td>
                    <asp:UpdatePanel ID="_updPnlCopyAs" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_btnOK" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="sectionHeader" align="left">
                    <asp:Label ID="_lblHeader" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td width="100%">
                    <asp:Panel ID="_pnlSave" runat="server" DefaultButton="_btnOK">
                        <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                            <tr>
                                <td align="left">
                                    <asp:Label ID="_lblLabel" runat="server" CssClass="labelValue"></asp:Label>
                                    <asp:HiddenField ID="_hdnCSSFile" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%">
                                    <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                        <tr>
                                            <td class="boldTxt" valign="top" style="width: 85px; padding-top: 5px;">
                                                Name:
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtCSSName" TextMode="SingleLine" Width="95%" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
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
                                                                <asp:Button ID="_btnOK" runat="server" CausesValidation="True" OnClick="_btnOK_Click"
                                                                    Text="OK" />
                                                            </div>
                                                        </td>
                                                        <td align="left">
                                                            <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel"
                                                                    OnClientClick="CancelSaveCSSPopup();" />
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
        </table>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="_grdViewStyle" runat="server" AllowPaging="True" DataSourceID="_odsStyleSheet"
                AutoGenerateColumns="false" BorderWidth="0" GridLines="None" Width="99%" OnRowCommand="_grd_RowCommand"
                OnRowDataBound="_grdViewStyle_RowDataBound" EmptyDataText="No stylesheet(s) found."
                CssClass="successMessage">
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:LinkButton CommandName="edit" CssClass="link" ID="_linkEdit" ToolTip="Edit"
                                runat="server" CausesValidation="false">Edit</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="50px" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton CommandName="preview" CssClass="link" ID="_linkReupload" ToolTip="Preview"
                                runat="server" CausesValidation="false">Preview</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="35px" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton CommandName="copy" CssClass="link" ID="_linkCopy" ToolTip="Copy"
                                runat="server" CausesValidation="false">Copy</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="60px" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton CommandName="archive" CssClass="link" ID="_linkArchive" ToolTip="Archive"
                                runat="server" CausesValidation="false">Archive</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Stylesheet">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="_lblCSS" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Files")%>'
                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Files")%>'></asp:Label>
                            <asp:Label ID="_lblDefault" runat="server" Text=" (default)"></asp:Label>
                            <%-- <asp:LinkButton runat="server" CssClass="link" ID="_lnkCss" CommandName="select"
                        CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Files")%>'>
                    </asp:LinkButton>--%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Modified Date">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="_lblModifedDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Date")%>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Select">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="link" ID="_lnkDefault" CommandName="selectdefault"
                                CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Default")%>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="_btnOK" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="_odsStyleSheet" runat="server" SelectMethod="GetStyleSheets"
        TypeName="IRIS.Law.WebApp.App_Code.StyleSheet,IRIS.Law.WebApp" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetTotalStyleSheetCount" StartRowIndexParameterName="startIndex">
        <SelectParameters>
            <asp:ControlParameter ControlID="_lblCustomCSSFolderPath" Name="customCSSPath" PropertyName="Value" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField ID="_lblCustomCSSFolderPath" runat="server"></asp:HiddenField>
    <br /><br />
    <table cellpadding="2" width="100%"><tr><td align="right">
        <div class="button" style="text-align: center; width:90px">
             <asp:Button ID="_btnArchived" runat="server" OnClick="_btnArchived_Click"
                   Text="Archived Styles" />
        </div>
    </td></tr></table>
</asp:Content>

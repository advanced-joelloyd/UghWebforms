<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportDocument.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ImportDocument" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>

<script language="javascript">

    function ResetImportDocumentControls() {
        document.getElementById("<%=_lblError.ClientID%>").innerText = "";
        if (document.getElementById("<%=_fileName.ClientID%>" != null)) {
            document.getElementById("<%=_fileName.ClientID%>").outerHTML = "<INPUT class=textBox id=<%=_fileName.ClientID%> style='WIDTH: 300px' type=file value='' />";
        }
        document.getElementById("<%=_ddlDocType.ClientID%>").selectedIndex = 0;
        document.getElementById("<%=_txtDocument.ClientID%>").value = "";
        document.getElementById("<%=_ccCreatedDate.DateTextBoxClientID%>").value = "";
        document.getElementById("<%=_txtNotes.ClientID%>").value = "";
        document.getElementById("<%=_ddlFeeEarner.ClientID%>").selectedIndex = 0;
        document.getElementById("<%=_chkUseVersioning.ClientID%>").checked = false;
        document.getElementById("<%=_chkEncryptFile.ClientID%>").checked = false;
        document.getElementById("<%=_chkLockDocument.ClientID%>").checked = false;
        return false;
    }

    function CancelPopupClick() {
        return false;
    }

    function CancelImportDocPopup() {
        $find('_modalpopupDocImportBehavior').hide();
    }
</script>

<asp:UpdateProgress ID="_updateProgressImportDoc" runat="server" AssociatedUpdatePanelID="_updPnlImportDoc">
    <ProgressTemplate>
        <div class="textBox">
            <img id="Img1" runat="server" alt="" src="~/Images/indicator.gif" />&nbsp;Loading...
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div class="button" style="width: 120px; text-align: center;" id="_divImportDocument" title="Import Document"
                runat="server">
                <asp:Button ID="_btnImportDocument" ToolTip="Import Document" OnClick="_btnImportDocument_Click" runat="server" OnClientClick="ResetImportDocumentControls();" CausesValidation="False" Text="Import Document" Enabled="false" />
            </div>
            <asp:Button ID="btnHiddenImportDoc" runat="Server" Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupDocImport" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlImportDocSearch" OnCancelScript="javascript:CancelPopupClick();"
                TargetControlID="btnHiddenImportDoc" CancelControlID="_btnCancel" BehaviorID="_modalpopupDocImportBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td align="center">
            <asp:Panel ID="_pnlImportDocSearch" runat="server" Style="background-color: #ffffff"
                Width="470px" Height="565px">
                <table width="99%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="right">
                            <a id="linkClose" href="#" onclick="CancelImportDocPopup();" class="link">
                                Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left">
                            Document Details
                        </td>
                    </tr>
                    <tr style="height: 5px;">
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:UpdatePanel ID="_updPnlImportDoc" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%" align="left">
                            <asp:Panel ID="_pnlImportDocContents" runat="server" DefaultButton="_btnSave">
                                <table border="0" class="panel" width="100%">
                                    <tr>
                                        <td class="boldTxt" style="width: 90px;">
                                            Name
                                        </td>
                                        <td>
                                            <input id="_fileName" type="file" style="width: 300px;" runat="server" onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();" />
                                            <asp:TextBox ID="_txtFileName" Enabled="false" Visible="false" runat="server" Width="300px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="_rfvFileName" runat="server" ControlToValidate="_fileName" Display="None" ErrorMessage="Document Name is mandatory"></asp:RequiredFieldValidator>
                                            <span class="mandatoryField">*</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 90px;">
                                            Type
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlDocType" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 90px;">
                                            Document
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="_txtDocument" SkinID="Large"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 90px;">
                                            Created Date
                                        </td>
                                        <td>
                                            <CC:CalendarControl ID="_ccCreatedDate" InvalidValueMessage="Invalid Created Date"
                                                runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top" style="width: 90px; padding-top: 5px;">
                                            Notes
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtNotes" runat="server" SkinID="Small" TextMode="MultiLine" Height="155px"
                                                Width="300px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 90px;">
                                            Fee Earner
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlFeeEarner" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="boldTxt">
                                            <asp:CheckBox ID="_chkUseVersioning" runat="server" Text="Use Versioning" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="boldTxt">
                                            <asp:CheckBox ID="_chkEncryptFile" runat="server" Text="Encrypt File" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="boldTxt">
                                            <asp:CheckBox ID="_chkLockDocument" runat="server" Text="Lock Document" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="padding-right: 10px;" colspan="2">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" style="padding-right: 15px;">
                                                        <table>
                                                            <tr>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;">
                                                                        <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetImportDocumentControls();" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;" id="_divCancelButton"
                                                                        runat="server">
                                                                        <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;">
                                                                        <asp:Button ID="_btnSave" runat="server" CausesValidation="True" OnClick="_btnSave_Click"
                                                                            Text="Save" />
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
            </asp:Panel>
        </td>
    </tr>
</table>

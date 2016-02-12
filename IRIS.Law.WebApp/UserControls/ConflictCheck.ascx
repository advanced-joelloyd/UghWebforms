<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConflictCheck.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ConflictCheck" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<table id="_tblConflictCheck" width="100%" runat="server" style="display: none;">
    <tr>
        <td style="height: 400px" valign="top">
            <table width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="_pnlConflict" runat="server" Width="99.9%" CssClass="bodyTab">
                            Conflict Records</asp:Panel>
                        <asp:Label CssClass="labelValue" ID="_lblConflictSurname" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;Conflict check has been performed on Surname(A)">
                        </asp:Label>
                    </td>
                </tr>
                <tr class="TitleSeparation">
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="border: 1px" class="conflictCheckBg">
                            <asp:Table ID="_tblConflictCheckGroup" Width="100%" runat="server" CellSpacing="0">
                            </asp:Table>
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>    
</table>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConflictResults.ascx.cs" Inherits="IRIS.Law.WebApp.UserControls.ConflictResults" %>

<table id="_tblConflictResults" width="100%" runat="server">
    <tr>
        <td style="height: 400px" valign="top">
            <table width="100%">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="_lblConflictSurname" runat="server" CssClass="labelValue" />
                    </td>
                </tr>
                <tr class="TitleSeparation">
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="border: 1px" class="conflictResultsBg">
                            <asp:Table ID="_tblConflictResultsGroup" Width="100%" runat="server" CellSpacing="0" />
                        </div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>    
</table>

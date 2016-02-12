<%@ Page Title="Time Entry" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="AddTimeEntry.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Time.AddTimeEntry" %>

<%@ PreviousPageType VirtualPath="~/Pages/Time/ViewTimesheet.aspx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">
    <table width="100%" border="0">
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="sectionHeader">
                <%if (Request.QueryString["edit"] == "true")
                  { %>
                Edit Time Entry
                <%}
                  else
                  {%>
                Add New Time Entry
                <%} %>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Matter Details</asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <CliMat:ClientMatterDetails runat="server" ID="_cliMatDetails" OnMatterChanged="_cliMatDetails_MatterChanged"
                    EnableValidation="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <asp:Panel ID="_pnlTimeDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                    Time Details</asp:Panel>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="_updPnlTime" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td class="boldTxt" style="width: 100px;">
                        Time Type
                    </td>
                    <td>
                        <asp:DropDownList ID="_ddlTimeType" runat="server" onmousemove="showToolTip(event);return false;"
                            onmouseout="hideToolTip();">
                        </asp:DropDownList>
                        <span class="mandatoryField">*</span>
                        <asp:RequiredFieldValidator ID="_rfvTimeType" runat="server" ErrorMessage="Time Type is mandatory"
                            Display="None" ControlToValidate="_ddlTimeType"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt">
                        Units
                    </td>
                    <td>
                        <asp:TextBox ID="_txtUnits" runat="server" Text="1" SkinID="Small" onmousemove="showToolTip(event);return false;"
                            onmouseout="hideToolTip();" onkeypress="return CheckNumeric(event);" onkeyup="CheckUnits(this)"></asp:TextBox>
                        <span class="mandatoryField">*</span>
                        <asp:RequiredFieldValidator ID="_rfvUnits" runat="server" ErrorMessage="Units is mandatory"
                            Display="None" ControlToValidate="_txtUnits"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" valign="top">
                        Notes
                    </td>
                    <td>
                        <asp:TextBox ID="_txtNotes" TextMode="MultiLine" Rows="10" runat="server" SkinID="Large"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="_updPnlTimeButtons" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td align="right" style="padding-right: 15px;" colspan="2">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;" id="_divBtnAddNew" runat="server">
                                        <asp:Button ID="_btnAddNew" runat="server" CssClass="button" Text="New" CausesValidation="false"
                                            OnClick="_btnAddNew_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <input id="_btnReset" type="button" value="Reset" onclick="Reset();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnSave" runat="server" Text="Save" CssClass="button" CausesValidation="true"
                                            OnClick="_btnSave_Click" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function Reset() {
            $("#<%= _ddlTimeType.ClientID %>").val("001 - Preparation");
            $("#<%= _txtUnits.ClientID %>").val("1");
            $("#<%= _txtNotes.ClientID %>").val("");
        }

        //Check if the user has entered 0
        function CheckUnits(sender) {
            if (parseInt(sender.value) == 0) {
                sender.value = "";
            }

            var strValue = sender.value

            if (strValue.indexOf("-") != -1) {
                if (Left(strValue, 1) != "-") {
                    sender.value = "1";
                }
                else {

                    var subStrValue = strValue.substring(1);

                    if (subStrValue.indexOf("-") != -1) {
                        sender.value = "1";
                    }
                }
             
                   
            }
        }

        function Left(str, n) {
            if (n <= 0)
                return "";
            else if (n > String(str).length)
                return str;
            else
                return String(str).substring(0, n);
        }

    </script>

</asp:Content>

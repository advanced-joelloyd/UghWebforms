<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DirectionsandMapTool.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.DirectionsandMapTool" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

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

    function CancelMapPopupClick() {
        return false;
    }

    function CancelMapPopup() {
        $find('_modalpopupMapBehavior').hide();
    }
</script>

<div class="button">
    <asp:Button ID="_btnMap" runat="server" CausesValidation="False" Text="Map" />
</div>
<ajaxToolkit:ModalPopupExtender ID="_modalpopupAttendees" runat="server" BackgroundCssClass="modalBackground"
    DropShadow="true" PopupControlID="_pnlMappingTool" OnCancelScript="javascript:CancelMapPopupClick();"
    TargetControlID="_btnMap" CancelControlID="_btnCancel" BehaviorID="_modalpopupMapBehavior">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel runat="server" ID="_pnlMappingTool" HorizontalAlign="Center" runat="server"
    Style="background-color: #ffffff" Width="500px" Height="330px">
    <table width="99%;" style="height: 70%" border="0" class="panel" id="_tableMapTool"
        runat="server">
        <tr>
            <td colspan="3">
                <table width="100%" id="_tableHeader">
                    <tr id="_trCloseLink" runat="server">
                        <td align="right">
                            <a id="linkClose" onclick="CancelMapPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:UpdatePanel ID="_updPnlError" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" CssClass="errorMessage" runat="server" Text=""></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left" runat="server" id="_tdHeader">
                            Map and Directions
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="right">
                <table cellpadding="2">
                    <tr>
                        <td class="boldTxt">
                            View:
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlMapView" runat="server">
                                <asp:ListItem Text="Map" Value="Map"></asp:ListItem>
                                <asp:ListItem Text="Satellite" Value="Satellite"></asp:ListItem>
                                <asp:ListItem Text="Terrain" Value="Terrain"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="left">
                <table cellpadding="2">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="_lblPostcode" CssClass="boldTxt"></asp:Label>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <div class="button">
                                <asp:Button ID="_btnGoogleLink" runat="server" CausesValidation="False" Text="Go to Map"
                                    OnClick="_btnGoogleLink_Click" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="left">
                            <br />
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:CheckBox runat="server" ID="_chkDirections" Text="Directions" OnCheckedChanged="_chkDirections_CheckedChanged"
                                        AutoPostBack="true" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="_pnlMapDirections" Visible="false">
                            <table cellpadding="2" cellspacing="0">
                                <tr>
                                    <td class="boldTxt">
                                        From:
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="_txtDirectionFrom"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt">
                                        To:
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="_txtDirectionTo"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" colspan="2">
                                        <div class="button">
                                            <asp:Button ID="_btnGoogleDirections" runat="server" CausesValidation="False" Text="Directions"
                                                OnClick="_btnGoogleDirections_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <table id="_tableButtons" width="100%">
        <tr>
            <td align="right" style="padding-right: 15px;">
                <table>
                    <tr>
                        <td align="right">
                            <div id="_divCancel" runat="server" class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

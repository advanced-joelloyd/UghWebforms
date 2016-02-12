<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiaryCancellation.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.DiaryCancellation" %>
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

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_initializeRequest(InitializeRequest);
    prm.add_endRequest(EndRequest);

    function InitializeRequest(sender, args) {
        if (prm.get_isInAsyncPostBack())
            args.set_cancel(true);
        //disable the search button to prevent the user from clicking multiple times 
        //while the request is being processed
        $("#<%=_btnFinish.ClientID %>").attr("disabled", true);
    }
    function EndRequest(sender, args) {
        $("#<%=_btnFinish.ClientID %>").attr("disabled", false);
    }

    function CancelPopupClick() {
        return false;
    }

    function CancellationCancelPopup() {
        $find('_modalpopupCancellationBehavior').hide();
    }

    function showDiaryCancellationModalPopupViaClient(id, isLimitationTask) {
        if (id != null) {
            var modalPopupBehavior = $find('_modalpopupCancellationBehavior');
            document.getElementById('<%=_hdnId.ClientID%>').value = id;

            if (isLimitationTask == "true") {
                document.getElementById("<%=_trLimitationError.ClientID%>").style.display = "";
            }
            else {
                document.getElementById("<%=_trLimitationError.ClientID%>").style.display = "none";
            }
            ResetCancellationControls();
            modalPopupBehavior.show();
        }
        return false;
    }

    function ResetCancellationControls() {
        $("#<%=_ddlReason.ClientID%>").val("Cancellation");
        $("#<%=_ddlCategory.ClientID%>").val("Internal");
        $("#<%=_txtDescription.ClientID%>").val("");
        $("#<%=_lblError.ClientID%>").text("");
        Page_ClientValidate();
        return false;
    }
</script>

<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:Button runat="server" ID="hiddenTargetControlForModalPopup" CausesValidation="false"
                Style="display: none" />
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupCancellation" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlCancellation" CancelControlID="_btnCancel"
                OnCancelScript="javascript:CancelPopupClick();" TargetControlID="hiddenTargetControlForModalPopup"
                BehaviorID="_modalpopupCancellationBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <asp:Panel ID="_pnlCancellation" runat="server" Style="background-color: #ffffff"
                Width="500px">
                <table width="100%">
                    <tr id="_trCloseLink" runat="server">
                        <td align="right">
                            <a id="linkClose" onclick="CancellationCancelPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="_lblError" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left">
                            <asp:Label ID="_lblHeader" runat="server"></asp:Label>&nbsp cancellation
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                            <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnFinish">
                                <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="_lblLabel" runat="server" CssClass="labelValue"></asp:Label>
                                            <asp:HiddenField ID="_hdnId" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100%">
                                            <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                                <tr>
                                                    <td class="boldTxt" style="width: 125px;">
                                                        Reason
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="_ddlReason" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="boldTxt" style="width: 125px;">
                                                        Category
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="_ddlCategory" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr id="_trLimitationError" runat="server" style="display:none;">
                                                    <td colspan="2" class="errorMessage">
                                                        Are you sure you wish to delete this limitation date task?
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="boldTxt" valign="top" style="width: 125px; padding-top: 5px;">
                                                        Description
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="_txtDescription" TextMode="MultiLine" Height="120px" Width="95%"
                                                            runat="server"></asp:TextBox>
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
                                                                        <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClick="_btnReset_Click"
                                                                            OnClientClick="return ResetCancellationControls();" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                        <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel"
                                                                            OnClientClick="CancellationCancelPopup();" />
                                                                    </div>
                                                                </td>
                                                                <td align="right">
                                                                    <div class="button" style="text-align: center;">
                                                                        <asp:Button ID="_btnFinish" runat="server" CausesValidation="True" OnClick="_btnFinish_Click"
                                                                            Text="Finish" />
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
        </td>
    </tr>
</table>

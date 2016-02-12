<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentImport.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.DocumentImport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>

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

    function DocumentImportPopupClick() {
        return false;
    }
    function DocumentImportPopup() {
        $find('_modalpopupDocumentImportBehavior').hide();
    }

    function ResetImportDocumentControls() {
        $("#<%=_lblError.ClientID %>").text("");

        document.getElementById("<%=_txtDocument.ClientID%>").value = "";
        document.getElementById("<%=_ccCreatedDate.DateTextBoxClientID%>").value = "";
        document.getElementById("<%=_txtNotes.ClientID%>").value = "";
        document.getElementById("<%=_ddlFeeEarner.ClientID%>").selectedIndex = 0;
        document.getElementById("<%=_chkUseVersioning.ClientID%>").checked = false;
        document.getElementById("<%=_chkEncryptFile.ClientID%>").checked = false;
        document.getElementById("<%=_chkLockDocument.ClientID%>").checked = false;
        return false;
    }


    function DisplayDocumentName() {
        if (CheckFileExtension()) {
            var filePath = document.getElementById("<%=_fileName.ClientID%>").value;
            document.getElementById("<%=_txtDocument.ClientID%>").value = filePath.substring(filePath.lastIndexOf('\\') + 1, filePath.length);
        }
    }

    function CheckFileExtension() {
        //debugger;
        // If Add mode, then check file type
        $("#<%=_lblError.ClientID %>").text("");
        var validFile = false;
        var UploadFileTypes = "<%=UploadFileTypes%>";
        var UploadFileTypesErrorMessage = "<%=UploadFileTypesErrorMessage%>";
        if (document.getElementById("<%=_fileName.ClientID%>") != null) {
            if (document.getElementById("<%=_fileName.ClientID%>").value != "") {
                var arrayfileTypes = UploadFileTypes.split('|');
                for (var i = 0; i < arrayfileTypes.length; i++) {
                    if (document.getElementById("<%=_fileName.ClientID%>").value.lastIndexOf(arrayfileTypes[i].trim()) > 0) {
                        validFile = true;
                    }
                }
            }
            if (!validFile) {
                $("#<%=_lblError.ClientID %>").text(UploadFileTypesErrorMessage);
                return false;
            }
        }
        return true;
    }
    function showDiv() {
        //        document.getElementById('myHiddenDiv').style.display = "";
        //        setTimeout('document.images["myAnimatedImage"].src="../Images/indicator.gif"', 200);
    } 

</script>

<!-- Document Import Button --->
<div class="button" style="width: 130px; text-align: center;" id="_divNewDocFolder"
    title="New Folder" runat="server">
    <asp:Button ID="_btnImportDocument" runat="server" CausesValidation="False" Text="Import Document"
        ToolTip="ImportDocument" Enabled="false" OnClick="_btnImportDocument_Click" />
</div>

<%--<asp:UpdatePanel ID="_UpdPnlDocumentImporting" ChildrenAsTriggers="true" runat="server"
    EnableViewState="true" UpdateMode="Conditional">
    <ContentTemplate>--%>
        <asp:Panel ID="_PnlDocumentImportingPopUp" runat="server" Style="width: 100%; height: 100%"
            Visible="false">
            <div style="position: absolute; left: 0%; top: 0%; width: 100%; height: 1600px; z-index: 2000;
                background-color: Gray; filter: alpha(opacity=70); opacity: 0.7;">
            </div>
            <div style="position: absolute; left: 35%; top: 20%; width: 450px; z-index: 2001;">
                <ajaxToolkit:RoundedCornersExtender ID="RoundedCornersExtender1" runat="server" Radius="6"
                    Corners="Top" BorderColor="White" TargetControlID="_PnlDocImportContainerTop" />
                <ajaxToolkit:RoundedCornersExtender ID="RoundedCornersExtender2" runat="server" Radius="6"
                    Corners="Bottom" BorderColor="White" TargetControlID="_PnlDocImportContainerBottom" />
                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%">
                    <tr>
                        <!-- ROW -->
                        <td>
                            <asp:Panel ID="_PnlDocImportContainerTop" runat="server" Style="width: 448px; height: 1px;
                                background-color: White" />
                        </td>
                    </tr>
                    <tr>
                        <!-- ROW -->
                        <td style="padding: 5px; background-color: White; width: 100%">
                            <table cellpadding="" cellspacing="8" style="width: 100%">
                                <tr>
                                    <!-- ROW -->
                                    <td>
                                        <table style="float: left;" width="99%" border="0" cellpadding="0" cellspacing="0">
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
                                                <td width="100%" align="left">
                                                    <table border="0" class="panel" width="100%">
                                                        <%--<tr>
                                                <td class="boldTxt" style="width: 90px;">
                                                    Type
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlDocType" runat="server" onchange="DisplayDocumentName();">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>--%>
                                                        <tr id="_trFileUpload" runat="server">
                                                            <td class="boldTxt" style="width: 90px;">
                                                                Name
                                                            </td>
                                                            <td>
                                                                <asp:FileUpload onchange="DisplayDocumentName();" ID="_fileName" runat="server" Width="300px"
                                                                    onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();" />
                                                                <span class="mandatoryField">*</span>
                                                                <asp:RequiredFieldValidator ID="_rfvFileName" runat="server" ControlToValidate="_fileName"
                                                                    Display="None" ErrorMessage="Document Name is mandatory"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="boldTxt" style="width: 90px;">
                                                                Description
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
                                                                <asp:TextBox ID="_txtNotes" runat="server" SkinID="Small" TextMode="MultiLine" Height="70px"
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
                                                            <!-- ROW -->
                                                            <td class="boldTxt" style="width: 90px;">
                                                                Folder
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="_DdlFolder" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="padding-top: 5px">
                                                                <table cellpadding="0" cellspacing="4" style="width: 100%">
                                                                    <tr>
                                                                        <!-- ROW -->
                                                                        <td class="boldTxt">
                                                                            <asp:CheckBox ID="_chkUseVersioning" runat="server" Text="Use Versioning" />
                                                                        </td>
                                                                        <td class="boldTxt">
                                                                            <asp:CheckBox ID="_chkLockDocument" runat="server" Text="Lock Document" Enabled="false" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <!-- ROW -->
                                                                        <td class="boldTxt">
                                                                            <asp:CheckBox ID="_chkPublic" runat="server" Text="Public" />
                                                                        </td>
                                                                        <td class="boldTxt">
                                                                            <asp:CheckBox ID="_chkEncryptFile" runat="server" Text="Encrypt File" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="right">
                                                                <%--<asp:UpdatePanel ID="_UpdPnlErrorAndButtons" runat="server" UpdateMode="Conditional">
                                                                    <ContentTemplate> --%>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="_lblError" runat="server" CssClass="errorMessage" />
                                                                        </td>
                                                                        <td align="right">
                                                                            <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                                <asp:Button ID="_btnBack" runat="server" CausesValidation="False" Text="Close" OnClick="_btnBack_Click" />
                                                                            </div>
                                                                        </td>
                                                                        <td align="right">
                                                                            <div class="button" style="text-align: center;">
                                                                                <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetImportDocumentControls();" />
                                                                            </div>
                                                                        </td>
                                                                        <td align="right">
                                                                            <div class="button" style="text-align: center;">
                                                                                <asp:Button ID="_btnSave" runat="server" CausesValidation="True" OnClick="_btnSave_Click"
                                                                                    OnClientClick="if(CheckFileExtension()){ showDiv();}" Text="Save" />
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <%--</ContentTemplate>
                                                                </asp:UpdatePanel>--%>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <!-- ROW -->
                        <td>
                            <asp:Panel ID="_PnlDocImportContainerBottom" runat="server" Style="width: 448px;
                                height: 1px; background-color: White" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
<%--    </ContentTemplate>
</asp:UpdatePanel>--%>

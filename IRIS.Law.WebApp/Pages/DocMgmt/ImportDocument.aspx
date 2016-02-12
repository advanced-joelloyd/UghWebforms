<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="ImportDocument.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.DocMgmt.ImportDocument"
    Title="Import Document" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentImportDocument" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
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
            document.getElementById("<%=_chkPublic.ClientID%>").checked = false;


            var fil = document.getElementById("<%=_fileName.ClientID%>"); fil.select(); n = fil.createTextRange(); n.execCommand('delete'); fil.focus();
            
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
                        if (document.getElementById("<%=_fileName.ClientID%>").value.toLowerCase().lastIndexOf(arrayfileTypes[i].trim()) > 0) {
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
            document.getElementById('myHiddenDiv').style.display = "";
            setTimeout('document.images["myAnimatedImage"].src="../../Images/GIFs/indicator.gif"', 200);
        } 
    </script>
    <span id='myHiddenDiv' style='display: none;' class="textBox">
    <img src='../Images/indicator.gif' id='myAnimatedImage'> Loading.....
    </span>
    <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
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
                            
                            <asp:FileUpload  onchange="DisplayDocumentName();" ID="_fileName" runat="server" Width="300px" onmousemove="showToolTip(event);return false;"
                                onmouseout="hideToolTip();" />
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
                            <asp:CheckBox ID="_chkPublic" runat="server" Text="Public" />
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
                            <asp:CheckBox ID="_chkLockDocument" runat="server" Text="Lock Document" Enabled="false" />
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
                            <table>
                                <tr>
                                    <td align="right">
                                        <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                            <asp:Button ID="_btnBack" OnClick="_btnBack_Click" runat="server" CausesValidation="False"
                                                Text="Back" />
                                        </div>
                                    </td>
                                    <td align="right">
                                        <div class="button" style="text-align: center;">
                                            <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetImportDocumentControls();" />
                                        </div>
                                    </td>
                                    <td align="right">
                                        <div class="button" style="text-align: center;">
                                            <asp:Button ID="_btnSave" runat="server" CausesValidation="True" OnClick="_btnSave_Click" onclientclick="if(CheckFileExtension()){ showDiv();}"
                                                Text="Save" />
                                        </div>
                                    </td>
                                    <td align="right">
                                        <div class="button" style="text-align: left; width:90px;" runat="server" id="_divUploadAgain">
                                            <asp:Button ID="_btnImport" runat="server" OnClick="_btnImport_Click" Enabled="false"
                                                Text="Import Another" />
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
</asp:Content>

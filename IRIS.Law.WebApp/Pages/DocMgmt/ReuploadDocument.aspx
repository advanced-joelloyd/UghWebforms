<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="ReuploadDocument.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.DocMgmt.ReuploadDocument"
    Title="Reupload Document" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentImportDocument" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        }

        function ResetReuploadDocumentControls() {
            $("#<%=_lblErrorReupload.ClientID %>").text("");
            //document.getElementById("<%=_fileName.ClientID%>").outerHTML = "<INPUT class=textBox id=<%=_fileName.ClientID%> style='WIDTH: 300px' type=file value='' />";
            document.getElementById("<%=_txtNotes.ClientID%>").value = "";
            return false;
        }

        function CheckFileName() {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate();
            }
            if (Page_IsValid) {
                $("#<%=_lblErrorReupload.ClientID %>").text("");
                document.getElementById("<%=_lblErrorReupload.ClientID%>").classname = "errorMessage";
                if (document.getElementById("<%=_hdnDocFileName.ClientID%>").value == '') {
                    $("#<%=_lblErrorReupload.ClientID %>").text("Existing file name to compare doesnot exists.");
                    return false;
                }
                var filePath = document.getElementById("<%=_fileName.ClientID%>").value;
                if (document.getElementById("<%=_hdnDocFileName.ClientID%>").value != filePath.substring(filePath.lastIndexOf('\\') + 1, filePath.length)) {
                    $("#<%=_lblErrorReupload.ClientID %>").text("File should be uploaded with the file name '" + document.getElementById("<%=_hdnDocFileName.ClientID%>").value + "'");
                    return false;
                }
                return true;
            }
            else {
                return false;
            }
        }
        function showDiv() {
            document.getElementById('myHiddenDiv').style.display = "";
            setTimeout('document.images["myAnimatedImage"].src="../Images/indicator.gif"', 200);
        } 
    </script>

    <span id='myHiddenDiv' style='display: none;' class="textBox">
    <img src='../Images/indicator.gif' id='myAnimatedImage'> Loading.....
    </span>
    <asp:Label ID="_lblErrorReupload" runat="server" CssClass="errorMessage"></asp:Label>
    <table width="99%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td class="sectionHeader" align="left">
                Re-Upload Document
            </td>
        </tr>
        <tr style="height: 5px;">
            <td>
            </td>
        </tr>
        <tr>
            <td width="100%" align="left">
                <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                    <tr>
                        <td class="boldTxt" style="width: 90px;">
                            Name
                        </td>
                        <td>
                            <asp:FileUpload ID="_fileName" runat="server" Width="300px" onmousemove="showToolTip(event);return false;"
                                onmouseout="hideToolTip();" />
                            <asp:RequiredFieldValidator ID="_rfvFileName" runat="server" ControlToValidate="_fileName"
                                Display="None" ErrorMessage="Document Name is mandatory" ValidationGroup="ReuploadDoc"></asp:RequiredFieldValidator>
                            <span class="mandatoryField">*</span>
                            <br />
                            <asp:Label ID="_lblFileNameNote" CssClass="textBox" runat="server"></asp:Label>                            
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
                        <td colspan="2">
                            <asp:HiddenField ID="_hdnDocFileName" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="right">
                <table>
                    <tr>
                        <td align="right">
                            <div class="button" style="text-align: center;" id="Div1" runat="server">
                                <asp:Button ID="_btnBack" OnClick="_btnBack_Click" runat="server" CausesValidation="False"
                                    Text="Back" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnResetReupload" runat="server" CausesValidation="False" Text="Reset"
                                    OnClientClick="return ResetReuploadDocumentControls();" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnSaveReupload" runat="server" CausesValidation="True" Text="Save" OnClientClick="if(CheckFileName()){ showDiv();}"
                                    OnClick="_btnSaveReupload_Click" ValidationGroup="ReuploadDoc" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

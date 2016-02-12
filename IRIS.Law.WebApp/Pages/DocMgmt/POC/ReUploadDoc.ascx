<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReUploadDoc.ascx.cs" Inherits="IRIS.Law.WebApp.DocumentManagement.POC.ReUploadDoc" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %> 

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
        function showDiv2() {
            document.getElementById('myHiddenDiv').style.display = "";
            setTimeout('document.images["myAnimatedImage"].src="../Images/indicator.gif"', 200);
        } 
    </script>

<asp:Panel ID="_PnlReUploadDocControls" runat="server" style="width:100%; height:100%" Visible="false">

    <div style="position:absolute; left:0%; top:0%; width:100%; height:1600px; z-index:2000; background-color: Gray;  filter: alpha(opacity=70);  opacity: 0.7;"></div>

    <div style="position:absolute;  left:35%; top:20%; width:400px; z-index:2001;">
     
        <ajaxToolkit:RoundedCornersExtender ID="_RndContainerTop" runat="server" Radius="6" Corners="Top" BorderColor="White" TargetControlID="_PnlUploadDocControlContainerTop" />
        <ajaxToolkit:RoundedCornersExtender ID="_RndContainerBottom" runat="server" Radius="6" Corners="Bottom" BorderColor="White" TargetControlID="_PnlUploadDocControlContainerBottom" />
     
        <table cellpadding="0" cellspacing="0" style="width:100%; height:100%">
            <tr> <!-- ROW -->
                <td><asp:Panel ID="_PnlUploadDocControlContainerTop" runat="server" style="width:398px; height:1px; background-color:White" /></td>
            </tr>
            <tr> <!-- ROW -->   
                <td style="padding:5px; background-color:White; width:100%"> 
                    <%--<asp:UpdatePanel ID="_UpdPnlNewFolder" runat="server" UpdateMode="Always">
                        <ContentTemplate> --%>
                            <table cellpadding="" cellspacing="8" style="width:100%">
                                <tr> <!-- ROW -->
                                    <td class="sectionHeader" align="left" colspan="2">
                                        Re-Upload Document
                                    </td>
                                </tr>
                                <tr>  <!-- ROW -->
                                    <td class="boldTxt" style="width: 90px;">
                                        Name
                                    </td>
                                    <td>
                                        <asp:FileUpload ID="_fileName" runat="server" Width="250px" onmousemove="showToolTip(event);return false;"
                                            onmouseout="hideToolTip();" />
                                        <asp:RequiredFieldValidator ID="_rfvFileName" runat="server" ControlToValidate="_fileName"
                                            Display="None" ErrorMessage="Document Name is mandatory" ValidationGroup="ReuploadDoc"></asp:RequiredFieldValidator>
                                        <span class="mandatoryField">*</span>
                                        <br />
                                        <asp:Label ID="_lblFileNameNote" CssClass="textBox" runat="server"></asp:Label>                            
                                    </td>
                                </tr> 
                                <tr> <!-- ROW -->
                                    <td class="boldTxt" valign="top" style="width: 90px; padding-top: 5px;">
                                        Notes
                                    </td>
                                    <td>
                                        <asp:TextBox ID="_txtNotes" runat="server" SkinID="Small" TextMode="MultiLine" Height="105px" Width="250px" />
                                    </td>
                                </tr>
                                <tr> <!-- ROW -->
                                <td align="right" style="padding-top:10px" colspan="2">
                                    <table>
                                        <tr>
                                            <td>
                                                <span id='myHiddenDiv' style='display: none;' class="textBox">
                                                    <img src='../../Images/indicator.gif' id='myAnimatedImage'> Loading.....
                                                </span>
                                            </td>
                                            <td><asp:Label ID="_lblErrorReupload" runat="server" Text="" /></td>
                                            <td align="right">
                                                <div class="button" style="text-align: center;" id="Div2" runat="server">
                                                    <asp:Button ID="_btnBack" OnClick="_btnBack_Click" runat="server" CausesValidation="False"
                                                        Text="Close" />
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
                                                    <asp:Button ID="_btnSaveReupload" runat="server" CausesValidation="True" Text="Save" OnClientClick="if(CheckFileName()){ showDiv2();}"
                                                        OnClick="_btnSaveReupload_Click" ValidationGroup="ReuploadDoc" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr> 
                            </table>  
                        <%--</ContentTemplate>
                    </asp:UpdatePanel>--%>
                </td>
            </tr>
            <tr> <!-- ROW -->
                <td><asp:Panel ID="_PnlUploadDocControlContainerBottom" runat="server" style="width:398px; height:1px; background-color:White" /></td>
            </tr>
        </table> 
    </div>
 
    <asp:HiddenField ID="_hdnDocFileName" runat="server" /> 
    
</asp:Panel>
 
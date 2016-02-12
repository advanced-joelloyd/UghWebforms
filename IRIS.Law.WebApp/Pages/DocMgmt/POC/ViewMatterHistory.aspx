<%@ Page Language="C#" AutoEventWireup="true" Title="View Matter History - IRIS Law Business"
    MasterPageFile="~/MasterPages/ILBHomePage.Master" CodeBehind="ViewMatterHistory.aspx.cs"
    Inherits="IRIS.Law.WebApp.DocumentManagement.POC.ViewMatterHistory" %>

<%@ Register TagPrefix="SDF" TagName="SearchDocumentFiles" Src="~/UserControls/SearchDocumentFiles.ascx" %>
<%@ Register TagPrefix="RUD" TagName="ReUploadDoc" Src="~/DocumentManagement/POC/ReUploadDoc.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register TagPrefix="DF" TagName="DocumentFiles" Src="~/DocumentManagement/POC/DocumentFiles.ascx" %>
<%@ Register TagPrefix="NF" TagName="NewFolder" Src="~/DocumentManagement/POC/NewDocFolder.ascx" %>
<%@ Register TagPrefix="DI" TagName="DocumentImport" Src="~/DocumentManagement/POC/DocumentImport.ascx" %>
<%@ Register TagPrefix="DFLD" TagName="DocumentFolders" Src="~/DocumentManagement/POC/DocumentFolders.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(InitializeRequest);
        prm.add_endRequest(EndRequest);
        var postBackElement;
        function InitializeRequest(sender, args) {
            if (prm.get_isInAsyncPostBack())
                args.set_cancel(true);
            postBackElement = args.get_postBackElement();
            if (postBackElement.id == "<%=_cliMatDetails.MatterReferenceClientID%>") {
                var left = $("#" + postBackElement.id).position().left + $("#" + postBackElement.id).width() + 20;
                var top = $("#" + postBackElement.id).position().top;
                $("#ctl00__cphMain__divUpdateProgress").css("position", "absolute");
                $("#ctl00__cphMain__divUpdateProgress").css("left", left + "px");
                $("#ctl00__cphMain__divUpdateProgress").css("top", top + "px");
            }
            else {
                $("#ctl00__cphMain__divUpdateProgress").css("position", "");

            }
        }
        function EndRequest(sender, args) {
            $get('<%=_updateProgressDocSearch.ClientID %>').style.display = 'none';
        }

        // This will find the co-ordinates of the control
        function findPos(obj) {
            var curleft = curtop = 0;
            curleft = obj.clientWidth + 35;
            if (obj.offsetParent) {
                do {
                    curleft += obj.offsetLeft;
                    curtop += obj.offsetTop;
                } while (obj = obj.offsetParent);
            }
            return [curleft, curtop];
        }

        function ShowProgress() {
            $("#<%=_updateProgressDocSearch.ClientID %>").show();
        }

        function HideProgress() {
            $("#<%=_updateProgressDocSearch.ClientID %>").hide();
        }

        function ConfirmDeleteFolder() {
            return confirm('Are you sure you want to delete this folder?');
        }
       
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                View Matter History
            </td>
        </tr>
        <tr>
            <td style="width: 25px;">
            </td>
        </tr>
    </table>
    <CliMat:ClientMatterDetails runat="server" ID="_cliMatDetails" OnMatterChanged="_cliMatDetails_MatterChanged" />
    <asp:UpdateProgress ID="_updateProgressDocSearch" runat="server" AssociatedUpdatePanelID="_updPnlSearchDoc">
        <ProgressTemplate>
            <div class="textBox" id="_divUpdateProgress" runat="server">
                <img id="Img1" src="~/Images/indicator.gif" runat="server" alt="" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="100%">
        <tr>
            <td style="background-color: #F7F6F3">
                <table cellpadding="2" cellspacing="0" style="width:100%">
                    <tr>
                        <td style="width:70%">  
                            <RUD:ReUploadDoc ID="_ReUploadDoc" runat="server" OnDocReUpload="_ReUploadDoc_DocReUploaded"/>
                        </td>
                        <td align="right">
                            <div class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnRefresh" runat="server" CausesValidation="False" 
                                    Text="   Refresh" onclick="_btnRefresh_Click" />
                            </div>
                        </td>
                        <td>
                            <SDF:SearchDocumentFiles ID="_searchDocFiles" runat="Server" />
                        </td>
                        <td>
<%--                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                <ContentTemplate> --%>
                                    <DI:DocumentImport ID="DocumentImport1" runat="Server"  OnItemChanged="DocumentImport1_ItemChanged"/> 
<%--                                </ContentTemplate>
                            </asp:UpdatePanel>--%>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate> 
                                    <NF:NewFolder ID="NewFolder1" runat="Server" OnFolderAdded="NewFolder1_MatterFolderAdded" /> 
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdPnlDeleteFolder" runat="server" UpdateMode="Always">
                                <ContentTemplate> 
                                    <% if (Session["SelectedFolder"] != null && ApprovedFiles().Count < 1)
                                       {  %>
                                    <div class="button" style="width: 90px; text-align: center;">
                                            <asp:Button ID="_btnDelFolder" runat="server" CausesValidation="False" 
                                                Text="Delete Folder" Enabled="false" onclick="_btnDelFolder_Click" OnClientClick="ConfirmDeleteFolder()" />
                                        </div> 
                                    <%}
                                       else
                                       { %>
                                        &nbsp;
                                    <%} %>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td> 
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="border: solid 1pt #F7F6F3">
                <table cellpadding="2" style="width:100%">
                    <tr>
                        <td style="border-right: solid 1pt #F7F6F3; width: 12%; vertical-align:top" class="boldTxt">
                            <table cellpadding="0" cellspacing="0" style="width:100%">
                                <tr><td class="gridViewHeader">Folders :</td></tr>
                                <tr> <!-- ROW -->
                                    <td style="padding-right:10px">
                                        <asp:UpdatePanel ID="_updPnlSearchFolders" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <DFLD:DocumentFolders ID="DocumentFolders" runat="Server" OnFolderSelected="DocumentFolders_MatterFolderChanged" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="NewFolder1" EventName="FolderAdded" /> 
                                                <asp:AsyncPostBackTrigger ControlID="_btnDelFolder" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table> 
                        </td>
                        <td class="boldTxt" style="vertical-align:top">
                            <asp:UpdatePanel ID="_updPnlSearchDoc" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <DF:DocumentFiles ID="DocumentFiles1" runat="Server" OnItemSelected="DocumentFiles1_MatterItemSelected" OnItemReUpload="DocumentFiles1_MatterItemReUpload" />
                                    
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="DocumentFolders" EventName="FolderSelected" /> 
                                </Triggers>
                            </asp:UpdatePanel>
                        
                            
                        </td>
                    </tr> 
                    <tr> <!-- ROW -->
                        <td colspan="2" style="background-color:#F7F6F3">
                            &nbsp;
                        </td>
                    </tr>
                </table> 
            </td>
        </tr>
    </table>
</asp:Content>

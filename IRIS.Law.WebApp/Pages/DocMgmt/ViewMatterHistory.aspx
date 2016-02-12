<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="ViewMatterHistory.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.DocMgmt.ViewMatterHistory"
    Title="View Matter History" %>

<%@ Register TagPrefix="SDF" TagName="SearchDocumentFiles" Src="~/UserControls/SearchDocumentFiles.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
            Nifty("div.buttonDisabled");
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
        
        function GetDocument(srcFile) {
            // Create an IFRAME.
            var iframe = document.createElement("iframe");


            // Point the IFRAME to GenerateFile, with the
            //   desired region as a querystring argument.
            iframe.src = srcFile;

            // This makes the IFRAME invisible to the user.
            iframe.style.display = "none";

            // Add the IFRAME to the page.  This will trigger
            //   a request to GenerateFile now.
            document.body.appendChild(iframe);
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
                        <asp:AsyncPostBackTrigger ControlID="_btnImportDocument" />      
                        <asp:AsyncPostBackTrigger ControlID="_grdDocFiles" />                  
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
    <table width="100%">
        <tr>
            <td align="right">
                <%--<asp:UpdatePanel ID="_updPanelUserControls" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>--%>
                <table>
                    <tr>
                        <td align="right">
                            <SDF:SearchDocumentFiles ID="_searchDocFiles" runat="Server" />
                        </td>
                        <td align="right">
                            <div class="buttonDisabled" style="width: 120px; text-align: center;" id="_divImportDocument"
                                title="Import Document" runat="server">
                                <asp:UpdatePanel ID="_updPanelImportButton" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="_btnImportDocument" ToolTip="Import Document" OnClick="_btnImportDocument_Click"
                                            runat="server" CausesValidation="False" Text="Import Document" EnableTheming="false" Enabled="false" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
                <%--</ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                        <asp:AsyncPostBackTrigger ControlID="_grdDocFiles" />
                    </Triggers>
                </asp:UpdatePanel>--%>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdateProgress ID="_updateProgressDocSearch" runat="server" AssociatedUpdatePanelID="_updPnlSearchDoc">
                    <ProgressTemplate>
                        <div class="textBox" id="_divUpdateProgress" runat="server">
                            <img id="Img1" src="~/Images/GIFs/indicator.gif" runat="server" alt="" />&nbsp;Loading...
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:UpdatePanel ID="_updPnlSearchDoc" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="_grdDocFiles" runat="server" AutoGenerateColumns="False" GridLines="None"
                            DataKeyNames="Id" OnRowDataBound="_grdDocFiles_RowDataBound" OnRowCommand="_grdDocFiles_RowCommand"
                            Width="100%" AllowSorting="true" EmptyDataText="No document(s) found for this matter." CssClass="successMessage">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemStyle Width="30px" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:LinkButton CommandName="select" CssClass="link" ID="_linkEdit" ToolTip="Edit"
                                            runat="server" CausesValidation="false">Edit</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle Width="60px" HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:LinkButton CommandName="reupload" CssClass="link" ID="_linkReupload" ToolTip="Reupload Document"
                                            runat="server" CausesValidation="false">ReUpload</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="25%" HeaderText="Description" SortExpression="FileDescription">
                                    
                                    <ItemTemplate>
                                        <%--<img id="Img2" src='../../Images/PNGs/<%# DataBinder.Eval(Container.DataItem, "ImgfileName")%>'
                                            title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>' alt='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'
                                            style="border: 0px;" />--%>
                                        <img id="Img2" src='../../Images/<%#GetImagePath(DataBinder.Eval(Container.DataItem, "ImgfileName"))%>'
                                            title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>' alt='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'
                                            style="border: 0px;" />
                                        <asp:LinkButton CommandName="fileDownload" CssClass="link" ID="_linkFileDownload"
                                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>' runat="server" OnClientClick="ShowProgress();"
                                            CausesValidation="false" Text='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField SortExpression="FeeEarnerRef" HeaderText="Fee Earner" >
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField SortExpression="IsPublic" HeaderText="Is Public">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_isPublic" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IsPublic")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "IsPublic")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField SortExpression="CreatedDate" HeaderText="Date Created">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'
                                            ToolTip='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                
                                 <asp:TemplateField  SortExpression="CreatedBy" HeaderText="Created By">
                                    
                                    <ItemTemplate>
                                        <asp:Label ID="_lblCreatedBy" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CreatedBy") %>'
                                            ToolTip='<%# DataBinder.Eval(Container.DataItem, "CreatedBy")  %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField SortExpression="FileName" HeaderText="Name">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDocName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FileName")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "FileName")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>
                               <%-- <asp:TemplateField SortExpression="Notes" HeaderText="Notes">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="_lblDocNotes" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'
                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:TemplateField>--%>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:ObjectDataSource ID="odsMatterDocument" runat="server" SelectMethod="GetMatterDocuments"
                    TypeName="IRIS.Law.WebApp.Pages.DocMgmt.ViewMatterHistory" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SortParameterName="sortBy" SelectCountMethod="GetMatterDocumentsRowsCount"
                    StartRowIndexParameterName="startRow" OnSelected="odsMatterDocument_Selected">
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

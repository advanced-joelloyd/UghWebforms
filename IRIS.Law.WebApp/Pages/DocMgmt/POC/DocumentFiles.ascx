<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentFiles.ascx.cs"
    Inherits="IRIS.Law.WebApp.DocumentManagement.POC.DocumentFiles" %>

<script type="text/javascript">

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


<table cellpadding="0" cellspacing="0" style="width:100%">
    <tr><td class="gridViewHeader">Files : <asp:Label ID="_LblRowCount" runat="server" Text="" /></td></tr>
    <tr> <!-- ROW -->
        <td>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="_updPnlSearchDoc" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- GRID -->
                    <asp:GridView ID="_grdDocFiles" runat="server" AutoGenerateColumns="False" GridLines="None"
                        DataKeyNames="Id" OnRowDataBound="_grdDocFiles_RowDataBound" OnRowCommand="_grdDocFiles_RowCommand"
                        Width="100%" EmptyDataText="No document(s) found for this matter." 
                        CssClass="successMessage" ondatabinding="_grdDocFiles_DataBinding" 
                        ondatabound="_grdDocFiles_DataBound">
                        <HeaderStyle BackColor="#DDDDDD" ForeColor="Black" />
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
                            <asp:TemplateField HeaderStyle-Width="35%">
                                <HeaderTemplate>
                                    Description
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <img id="Img2" src='../../Images/<%# DataBinder.Eval(Container.DataItem, "ImgfileName")%>'
                                        title='<%# DataBinder.Eval(Container.DataItem, "ToolTip")%>' alt='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'
                                        style="border: 0px;" />
                                    <asp:LinkButton CommandName="fileDownload" CssClass="link" ID="_linkFileDownload"
                                        ToolTip='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>' runat="server"
                                         CausesValidation="false" Text='<%# DataBinder.Eval(Container.DataItem, "FileDescription")%>'>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Fee Earner
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'
                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerRef")%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Date Created
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="_lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'
                                        ToolTip='<%# Eval("CreatedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Name
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="_lblDocName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FileName")%>'
                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "FileName")%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Notes
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="_lblDocNotes" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'
                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "Notes")%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView> 
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Label ID="_LblStatus" runat="server" Text="" Visible="false" />
        </td>
    </tr>
</table>



<asp:ObjectDataSource ID="odsMatterDocument" runat="server" SelectMethod="GetMatterDocuments"
    TypeName="IRIS.Law.WebApp.DocumentManagement.POC.ViewMatterHistory" EnablePaging="True"
    MaximumRowsParameterName="pageSize" SelectCountMethod="GetMatterDocumentsRowsCount"
    StartRowIndexParameterName="startRow" OnSelected="odsMatterDocument_Selected">
</asp:ObjectDataSource>



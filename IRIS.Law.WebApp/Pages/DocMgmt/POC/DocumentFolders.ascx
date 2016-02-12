<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentFolders.ascx.cs" Inherits="IRIS.Law.WebApp.DocumentManagement.POC.DocumentFolders" %>


<asp:TreeView ID="_TViewFolders" runat="server" ShowLines="false" ExpandDepth="1" SelectedNodeStyle-ForeColor="#0094DE"  onselectednodechanged="_TViewFolders_SelectedNodeChanged" SelectedNodeStyle-CssClass="SelectedFolder">
    <Databindings>
        <asp:TreeNodeBinding DataMember="Documents" Text="Documents"  />
        <asp:TreeNodeBinding DataMember="Folders" TextField="DocAttributeValue" ValueField="DocID"/> 
    </Databindings>
</asp:TreeView>

<br />

<asp:Label ID="_LblStatus" runat="server" Text="" Visible="false" />
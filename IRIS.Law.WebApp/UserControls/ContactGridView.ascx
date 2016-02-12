<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactGridView.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ContactGridView" %>
<asp:GridView ID="_grdContactDetails" runat="server" AutoGenerateColumns="False"
    GridLines="None" Width="100%" OnRowEditing="_grdContactDetails_RowEditing" OnRowDataBound="_grdContactDetails_RowDataBound"
    OnRowCancelingEdit="_grdContactDetails_RowCancelingEdit" OnRowUpdating="_grdContactDetails_RowUpdating"
    DataKeyNames="TypeId,AddressId" OnPageIndexChanging="_grdContactDetails_PageIndexChanging"
    AllowPaging="true">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="_btnEdit" runat="server" CommandName="Edit" CausesValidation="false">Edit</asp:LinkButton>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:LinkButton ID="_btnCancel" runat="server" CommandName="Cancel" CausesValidation="false">Cancel</asp:LinkButton>
                <asp:LinkButton ID="_btnUpdate" runat="server" CommandName="Update" CausesValidation="false">Update</asp:LinkButton>
            </EditItemTemplate>
            <ItemStyle Width="12%" HorizontalAlign="Left" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                Description
            </HeaderTemplate>
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "TypeText") %>
            </ItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
            <ItemStyle Width="20%" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                Details
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="_lblElementText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ElementText") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="_txtElementText" runat="server" Width="85%" Text='<%# DataBinder.Eval(Container.DataItem, "ElementText") %>'></asp:TextBox>
            </EditItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
            <ItemStyle Width="34%" VerticalAlign="Top"/>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                Notes
            </HeaderTemplate>
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "ElementComment") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="_txtElementComment" runat="server" Width="85%" Text='<%# DataBinder.Eval(Container.DataItem, "ElementComment") %>'></asp:TextBox>
            </EditItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
            <ItemStyle Width="34%" VerticalAlign="Top"/>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

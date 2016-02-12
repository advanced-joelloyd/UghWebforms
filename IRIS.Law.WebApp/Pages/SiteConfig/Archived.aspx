<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
         CodeBehind="Archived.aspx.cs" Title="Archived Styles" Inherits="IRIS.Law.WebApp.Pages.SiteConfig.Archived" %>
         
         
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

    <script language="javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        }

        
    </script>

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server"></asp:Label>
                    </ContentTemplate>
                    
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" align="left">
                Archived Stylesheets
            </td>
        </tr>
    </table>
    <br />
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:GridView ID="_grdViewStyle" runat="server" AllowPaging="True" DataSourceID="_odsStyleSheet"
                AutoGenerateColumns="false" BorderWidth="0" GridLines="None" Width="99%" OnRowCommand="_grd_RowCommand"
                OnRowDataBound="_grdViewStyle_RowDataBound" EmptyDataText="No stylesheet(s) found."
                CssClass="successMessage">
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="80px" HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton CommandName="unarchive" CssClass="link" ID="_linkArchive" ToolTip="Un-archive"
                                runat="server" CausesValidation="false">Un-archive</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Stylesheet">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="_lblCSS" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Files")%>'
                                ToolTip='<%#DataBinder.Eval(Container.DataItem, "Files")%>'></asp:Label>
                            <asp:Label ID="_lblDefault" runat="server" Text=" (default)"></asp:Label>
                            <%-- <asp:LinkButton runat="server" CssClass="link" ID="_lnkCss" CommandName="select"
                        CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Files")%>'>
                    </asp:LinkButton>--%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Modified Date">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="_lblModifedDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Date")%>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </asp:TemplateField>
                    
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="_odsStyleSheet" runat="server" SelectMethod="GetStyleSheets"
        TypeName="IRIS.Law.WebApp.App_Code.StyleSheet,IRIS.Law.WebApp" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetTotalStyleSheetCount" StartRowIndexParameterName="startIndex">
        <SelectParameters>
            <asp:ControlParameter ControlID="_lblCustomCSSFolderPath" Name="customCSSPath" PropertyName="Value" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField ID="_lblCustomCSSFolderPath" runat="server"></asp:HiddenField>
    <br /><br />
    <table cellpadding="2" width="100%"><tr><td align="right">
        <div class="button" style="text-align: center">
             <asp:Button ID="_btnBack" runat="server" OnClick="_btnBack_Click"
                   Text="Back" />
        </div>
    </td></tr></table>
</asp:Content>



<%@ Page Title="Home" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" Codebehind="Home.aspx.cs" Inherits="IRIS.Law.WebApp.Home" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:content id="Content1" contentplaceholderid="_cphMain" runat="server">
	<table width="100%">
		<tr>
			<td class="homeText">
                
                 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   
                </asp:UpdatePanel>
                
                
                
                <asp:Label ID="_lblHomePageText" runat="server"></asp:Label>
            </td>
		</tr>
	</table>
</asp:content>

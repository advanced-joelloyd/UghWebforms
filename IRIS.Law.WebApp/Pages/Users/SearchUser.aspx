<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master" CodeBehind="SearchUser.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Users.SearchUser" Title="Search User" %>

<%@ Register TagPrefix="CS" TagName="UserSearch" Src="~/UserControls/UserSearch.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">
	<CS:UserSearch ID="UserSearch" DisplayPopup="false" Width100pc="true" HideLoading="true"  runat="Server" />
</asp:Content>

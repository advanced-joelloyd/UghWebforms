<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
	CodeBehind="SearchClient.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Client.SearchClient"
	Title="Search Client" %>

<%@ Register TagPrefix="CS" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">
	<CS:ClientSearch ID="ClientSearch" DisplayPopup="false" Width100pc="true" HideLoading="true" DisplayClientNameTextbox="false" runat="Server" />
</asp:Content>

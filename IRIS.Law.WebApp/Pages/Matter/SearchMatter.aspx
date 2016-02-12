<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchMatter.aspx.cs" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    Inherits="IRIS.Law.WebApp.Pages.Matter.MatterSearch" Title="Search Matter" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="MS" TagName="MatterSearch" Src="~/UserControls/MatterSearch.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">
    <MS:MatterSearch ID="_matterSearch" ShowCriteria="true" runat="server" />
</asp:Content>

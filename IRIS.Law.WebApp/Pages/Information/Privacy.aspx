﻿<%@ Page Language="C#" AutoEventWireup="true"  Title="Privacy" MasterPageFile="~/MasterPages/ILBHomePage.Master"  CodeBehind="Privacy.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Information.Privacy" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">


<table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                Privacy
            </td>
        </tr>
        <tr>
            <td style="width: 25px;">
            </td>
        </tr>
    </table>
    
     <br />
    <asp:Label runat="server" ID="_text" CssClass="homeText"></asp:Label>
    
</asp:Content>
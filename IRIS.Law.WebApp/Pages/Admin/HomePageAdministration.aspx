<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/MasterPages/ILBHomePage.Master" Title="Home: Page Text Administration" CodeBehind="HomePageAdministration.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Admin.HomePageAdministration" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="HTMLEditor" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_cphMain" runat="server">
<script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        }
        
</script>
  
    
    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   <Triggers>
         
                        <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                Home Page Administration
            </td>
        </tr>
        <tr>
            <td style="width: 25px;">
            </td>
        </tr>
    </table>
    
     <asp:UpdatePanel ID="UpdatePanel2" runat="server" >
     <ContentTemplate>
    
    <table width="100%" cellpadding="2">
    <tr>
        <td>
           
          
           
            <HTMLEditor:Editor runat="server"  
                Height="300px"
                Width="100%" ID="_htmleditor"
                AutoFocus="true"
            />
        
        </td>
    </tr>
    <tr>
        <td align="right">
            <div class="button" style="float: right; text-align: center;">
			    <asp:Button ID="_btnSave" runat="server" Text="Save" onclick="_btnSave_Click" />
			</div>
        </td>
    </tr>
    </table>
    
    </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
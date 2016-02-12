<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewDocFolder.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.NewDocFolder" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script type="text/javascript">
    var browser = navigator.appName;
    //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
    //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
    //on box-items. This is not yet implemented in IE so round the corners using javascript
    if (browser == "Microsoft Internet Explorer") {
        Sys.Application.add_load(RoundedCorners);
    }
    function RoundedCorners() {
        Nifty("div.button");
    }

    function NewDocFolderPopupClick() {
        return false;
    }
    function NewDocFolderPopup() {
        $find('_modalpopupNewDocFolderBehavior').hide();
    }

</script>

 
<!-- New Folder Button --->
<div class="button" style="width: 80px; text-align: center;" id="_divNewDocFolder"
    title="New Folder" runat="server">
    <asp:Button ID="_btnNewFolder" runat="server" CausesValidation="False" Text=" New Folder"
        ToolTip="New Folder" Enabled="false" OnClick="_btnOpenFolderControl_Click" />
</div>

<asp:Panel ID="_PnlNewFolderControls" runat="server" style="width:100%; height:100%" Visible="false">

    <div style="position:absolute; left:0%; top:0%; width:100%; height:1600px; z-index:2000; background-color: Gray;  filter: alpha(opacity=70);  opacity: 0.7;"></div>

    <div style="position:absolute;  left:35%; top:20%; width:400px; z-index:2001;">
     
        <ajaxToolkit:RoundedCornersExtender ID="_RndContainerTop" runat="server" Radius="6" Corners="Top" BorderColor="White" TargetControlID="_PnlFolderControlContainerTop" />
        <ajaxToolkit:RoundedCornersExtender ID="_RndContainerBottom" runat="server" Radius="6" Corners="Bottom" BorderColor="White" TargetControlID="_PnlFolderControlContainerBottom" />
     
        <table cellpadding="0" cellspacing="0" style="width:100%; height:100%">
            <tr> <!-- ROW -->
                <td><asp:Panel ID="_PnlFolderControlContainerTop" runat="server" style="width:398px; height:1px; background-color:White" /></td>
            </tr>
            <tr> <!-- ROW --> 
                <td style="padding:5px; background-color:White; width:100%"> 
                    <asp:UpdatePanel ID="_UpdPnlNewFolder" runat="server" UpdateMode="Always">
                        <ContentTemplate> 
                            <table cellpadding="" cellspacing="8" style="width:100%">
                                <tr> <!-- ROW -->
                                    <td class="sectionHeader" align="left">
                                        New Document Folder
                                    </td>
                                </tr>
                                <tr>  <!-- ROW -->
                                    <td><asp:TextBox ID="_txtFolderName" runat="server" style="width:98%"/></td>
                                </tr> 
                                <tr> <!-- ROW -->
                                <td align="right" style="padding-top:10px">
                                    <table cellpadding="0" cellspacing="0" style="width:100%">
                                        <tr> <!-- ROW -->
                                            <td style="width:80%; vertical-align:top; text-align:left"><asp:Label ID="_lblError" CssClass="errorMessage" runat="server" /></td>
                                            <td align="right" style="vertical-align:top">
                                                <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                    <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Close" OnClick="_btnCancel_Click" />
                                                </div>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td align="right" style="vertical-align:top">
                                                <div class="button" style="text-align: center;">
                                                    <asp:Button ID="_btnNewDocFolder" runat="server" CausesValidation="False" 
                                                        Text="Save" onclick="_btnNewDocFolder_Click" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr> 
                            </table>  
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr> <!-- ROW -->
                <td><asp:Panel ID="_PnlFolderControlContainerBottom" runat="server" style="width:398px; height:1px; background-color:White" /></td>
            </tr>
        </table> 
    </div>
 
</asp:Panel>


 
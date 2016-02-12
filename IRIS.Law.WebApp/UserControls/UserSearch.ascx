<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserSearch.ascx.cs" Inherits="IRIS.Law.WebApp.UserControls.UserSearch" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>


<%@ Register Namespace="Tooltip" Assembly="Tooltip" TagPrefix="tt" %>

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

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_initializeRequest(InitializeRequest);
    prm.add_endRequest(EndRequest);

    function InitializeRequest(sender, args) {
        if (prm.get_isInAsyncPostBack())
            args.set_cancel(true);
        //disable the search button to prevent the user from clicking multiple times 
        //while the request is being processed
        $("#<%=_btnSearch.ClientID %>").attr("disabled", true);
    }
    function EndRequest(sender, args) {
        $("#<%=_btnSearch.ClientID %>").attr("disabled", false);
    }



    function CancelPopupClick() {
        return false;
    }


</script>

<table style="float: left;" cellpadding="0" cellspacing="0">
    <tr>
        <td> 
            <table cellspacing="0" cellpadding="0" style="width:100%; height:25px">
                <tr> <!-- ROW -->
                    <td>
                        <asp:UpdatePanel ID="_updUserRef" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField ID="_hdnUserId" runat="server" />
                                <asp:HiddenField ID="_hdnDisplayPopup" runat="server" />
                                <asp:TextBox ID="_txtSelectedName" runat="server" SkinID="Small" 
                                    onmousemove="showToolTip(event);return false;"  onmouseout="hideToolTip();" 
                                    Width = "175px"></asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                         <div style="float: left;" id="_pnlValidation" runat="server">
                            <asp:RequiredFieldValidator ID="_rfvUserReference" runat="server" ErrorMessage="User is mandatory"
                                Display="None" ControlToValidate="_txtSelectedName"></asp:RequiredFieldValidator>
                        </div>
                    </td>
                    <td>
                            <asp:ImageButton AlternateText="User Search" ID="_imgTextControl" runat="server"
                                ImageUrl="~/Images/PNGs/searchButton.png" ToolTip="User Search" SkinID="SearchImageIcon"
                                Height="21px" CausesValidation="false" />
                    </td>
                    <td>
                        <ajaxToolkit:ModalPopupExtender ID="_modalpopupUserSearch" runat="server" BackgroundCssClass="modalBackground"
                            DropShadow="true" PopupControlID="_UserSearchContainer" OnCancelScript="javascript:CancelPopupClick();"  
                            TargetControlID="_imgTextControl" CancelControlID="_btnCancel">
                        </ajaxToolkit:ModalPopupExtender>
                    </td>  
                </tr>
            </table> 
        </td>
    </tr>
</table>
<asp:Panel ID="_UserSearchContainer" runat="server" Width="100%">
<center>
<% if (Request.RawUrl.ToLower().Contains("searchUser.aspx")) { %>
    <table cellpadding="0" cellspacing="0" width="100%" style="float: left">
<% }  else { %> 
    <table cellpadding="0" cellspacing="0" width="300px" style="float: left">
    <% } %>
        <tr>
            <td>
                <asp:Panel ID="_pnlUserSearch" runat="server" Width="100%">
                    <center>
                    <div style="background-color: #ffffff;">
                    <table id="_tbSearch" runat="server" style="text-align:left">
                        <tr id="_trCloseLink" runat="server">
                            <td align="right">
                                <asp:LinkButton ID="_LnkBtnClose" runat="server" Text="Close" CausesValidation="false" OnClick="_LnkBtnClose_Close" CssClass="link" /> &nbsp;&nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="_updPanelError" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="_lblMessage" runat="server" CssClass="errorMessage"></asp:Label>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnSearch" />
                                </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="sectionHeader" align="left">
                                Select User
                            </td>
                        </tr>
                        <tr>
                            <td width="100%">
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch"> 
                                    <table border="0" cellpadding="1" cellspacing="0" class="panel" width="100%">
                                        <tr>
                                            <td>
                                                <table cellpadding="1" cellspacing="1" border="0" style="width: 200px;">
                                                    <tr>
                                                        <td class="boldTxt" style="width: 50px;">
                                                            Name
                                                        </td>
                                                        <td >
                                                            <asp:TextBox runat="server" ID="_txtName" Width="125"></asp:TextBox>
                                                        </td>
                                                       
                                                    </tr>
                                                  
                                                </table>
                                            </td>
                                            <td valign="top">
                                            
                                            
                                                <%--<asp:Image ImageUrl="~/Images/PNGs/about.png" ToolTip="Tip" runat="server" ID="imgToolTip" />--%>
            
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <table >
                                                    <tr>
                                                        <td align="right" style="padding-right: 5px;">
                                                            <table>
                                                                <tr>
                                                                    <td align="right">
                                                                        <div class="button" style="text-align: center;">
                                                                            <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClick="_btnReset_Click" />
                                                                        </div>
                                                                    </td>
                                                                    <td align="right">
                                                                        <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                            <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                                                                        </div>
                                                                    </td>
                                                                    <td align="right">
                                                                        <div class="button" style="text-align: center;">
                                                                            <asp:Button ID="_btnSearch" runat="server" CausesValidation="True" OnClick="_btnSearch_Click"
                                                                                Text="Search" ValidationGroup="UserSearch" />
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                       
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="_pnlUpdateProgressUserSearch" runat="server">
                            
                                    <asp:UpdateProgress ID="_updateProgressUserSearch" runat="server">
                                        <ProgressTemplate>
                                            <div class="textBox">
                                                <img id="_imgUserSearchLoadIndicator" runat="server" alt="" src="../Images/GIFs/indicator.gif" />&nbsp;Loading...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                
                                </asp:Panel>
                                
                                <asp:UpdatePanel ID="_updPnlUserSearch" runat="server" UpdateMode="Conditional"
                                    ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <table width="100%" id="_tableUsers">
                                            <tr>
                                                <td class="sectionHeader" align="left">
                                                    Users
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                                                        <asp:GridView ID="_grdSearchUserList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                            BorderWidth="0" DataKeyNames="UserId" GridLines="None" Width="97%"
                                                            OnRowCommand="_grdSearchUserList_RowCommand" OnRowDataBound="_grdSearchUserList_RowDataBound"
                                                            EmptyDataText="No users(s) found." CssClass="successMessage" 
                                                            AllowSorting="true">
                                                            <Columns>
                                                                <asp:TemplateField Visible="true">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" class="link" ID="_lnkUser" CommandName="select"
                                                                            CausesValidation="false" ></asp:LinkButton>

                                                                            
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                            
                                                        </asp:GridView>
                                                    </div>
                                                    <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchUserList" EventName="PageIndexChanging" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchUserList" EventName="Sorting" />
                                    </Triggers>
                                </asp:UpdatePanel>
                               
                                <asp:ObjectDataSource ID="_odsSearchUser" runat="server" SelectMethod="SearchUser"
                                    TypeName="IRIS.Law.WebApp.UserControls.UserSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetUserRowsCount" StartRowIndexParameterName="startRow"
                                    OnSelected="_odsSearchUser_Selected" SortParameterName="sortBy">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="_txtName" Name="name" PropertyName="Text" />
                                        
                                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value" Type="Boolean" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>

                             
                                
                            </td>
                        </tr>
                    </table>
                    </div>
                    </center>
                </asp:Panel>
            </td>
        </tr>
    </table>
</center>
</asp:Panel>

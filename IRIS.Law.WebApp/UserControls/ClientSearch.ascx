<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientSearch.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ClientSearch" %>
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

    $(document).ready(function() {
        $("#<%=_btnReset.ClientID%>").click(function() {
            $("#<%=_txtSurname.ClientID%>").val("");
            $("#<%=_txtNINo.ClientID%>").val("");
            $("#<%=_ccDOBDate.DateTextBoxClientID%>").val("");
            $("#<%=_txtPostcode.ClientID%>").val("");
            $("#<%=_txtTown.ClientID%>").val("");
            $("#<%=_ddlPartner.ClientID%>").val("");
            $("#<%=_grdSearchClientList.ClientID%>").css("display", "none");
            $("#<%=_grdMattersForClient.ClientID%>").css("display", "none");
            $("#<%=_lblMessage.ClientID%>").text("");
            return false;
        });
 

    });

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
                        <asp:UpdatePanel ID="_updClientRef" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField ID="_hdnClientRef" runat="server" />
                                <asp:HiddenField ID="_hdnClientName" runat="server" />
                                <asp:HiddenField ID="_hdnClientId" runat="server" />
                                <asp:HiddenField ID="_hdnIsMember" runat="server" />
                                <asp:HiddenField ID="_hdnDisplayPopup" runat="server" />
                                
                                <asp:TextBox ID="_txtClientReference" runat="server" SkinID="Small" onmousemove="showToolTip(event);return false;"
                                    onmouseout="hideToolTip();"></asp:TextBox>
                               
                               

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                         <div style="float: left;" id="_pnlValidation" runat="server">
                            <asp:RequiredFieldValidator ID="_rfvClientReference" runat="server" ErrorMessage="Client is mandatory"
                                Display="None" ControlToValidate="_txtClientReference"></asp:RequiredFieldValidator>
                        </div>
                    </td>
                    <td>
                      
              
                            <asp:ImageButton AlternateText="Client Search" ID="_imgTextControl" runat="server"
                                ImageUrl="~/Images/PNGs/searchButton.png" ToolTip="Client Search" SkinID="SearchImageIcon"
                                Height="21px" CausesValidation="false" />

                            <asp:LinkButton ID="_lnkClientSelect" CssClass="link" runat="server" 
                                Text='Select Client'  > </asp:LinkButton>


                      
                    </td>
                    <td>

                        <ajaxToolkit:ModalPopupExtender ID="_modalpopupClientSearch" runat="server" BackgroundCssClass="modalBackground"
                            DropShadow="true" PopupControlID="_ClientSearchContainer" OnCancelScript="javascript:CancelPopupClick();"  
                            TargetControlID="_imgTextControl"  CancelControlID="_btnCancel">
                        </ajaxToolkit:ModalPopupExtender>

                     <%--   <ajaxToolkit:ModalPopupExtender ID="_modalpopupLinkClientSearch" runat="server" BackgroundCssClass="modalBackground"
                            DropShadow="true" PopupControlID="_ClientSearchContainer" OnCancelScript="javascript:CancelPopupClick();"  
                            TargetControlID="_lnkClientSelect" CancelControlID="_btnCancel">
                        </ajaxToolkit:ModalPopupExtender>--%>

                    </td>  
                </tr>
            </table> 
        </td>
    </tr>
</table>
<asp:Panel ID="_ClientSearchContainer" runat="server" Width="100%">
<center>
<% if (Request.RawUrl.ToLower().Contains("searchclient.aspx")) { %>
    <table cellpadding="0" cellspacing="0" width="100%" style="float: left">
<% }  else { %> 
    <table cellpadding="0" cellspacing="0" width="630px" style="float: left">
    <% } %>
        <tr>
            <td>
                <asp:Panel ID="_pnlClientSearch" runat="server" Width="100%">
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
                                Search Client
                            </td>
                        </tr>
                        <tr>
                            <td width="100%">
                                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch"> 
                                    <table border="0" cellpadding="1" cellspacing="0" class="panel" width="100%">
                                        <tr>
                                            <td style="width: 100%">
                                                <table cellpadding="1" cellspacing="1" border="0" style="width: 100%;">
                                                    <tr>
                                                        <td class="boldTxt" style="width: 125px;">
                                                            Name
                                                        </td>
                                                        <td>
                                                            <asp:TextBox runat="server" ID="_txtSurname"></asp:TextBox>
                                                        </td>
                                                        <td class="boldTxt" style="width: 125px;">
                                                            NI No.
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="_txtNINo" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="boldTxt">
                                                            Partner
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="_ddlPartner" runat="server">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="boldTxt">
                                                            Date of Birth
                                                        </td>
                                                        <td>
                                                            <table border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <CC:CalendarControl ID="_ccDOBDate" InvalidValueMessage="Invalid Date of Birth" ValidationGroup="ClientSearch"
                                                                            runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="boldTxt">
                                                            Postcode
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="_txtPostcode" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td class="boldTxt">
                                                            Town
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="_txtTown" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td valign="top">
                                            
                                            
                                                <%--<asp:Image ImageUrl="~/Images/PNGs/about.png" ToolTip="Tip" runat="server" ID="imgToolTip" />--%>
            
			                                  <%--<tt:TooltipExtender 
					                                id="tteSearchInfo" 
					                                TargetControlID="imgToolTip" 
					                                runat="server"
					                                Delay="1"
					                                Direction="left"
					                                TooltipWidth="200"
					                                >
					                                <TooltipTemplate>
					                                    <p class="toolTip">
						                                Did you know that you can use the wildcard (%) symbol in text boxes.
						                                <br /><br />
						                                When searching for smith you can simply use smi%. This will bring back all the results where the name begins with smi.
						                                <br /><br />
						                                This symbol can also be included inside the string to further enhance the search. Say you want to search for everyone whose name begins with s but contains the letter i. This
						                                 can be achieved using the following; s%i%.  
						                                 </p> 
					                                </TooltipTemplate>
				                                  </tt:TooltipExtender>--%>
                                            
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="padding-right: 50px;" colspan="2">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="right" style="padding-right: 15px;">
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
                                                                                Text="Search" ValidationGroup="ClientSearch" />
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="_pnlUpdateProgressClientSearch" runat="server">
                            
                                    <asp:UpdateProgress ID="_updateProgressClientSearch" runat="server">
                                        <ProgressTemplate>
                                            <div class="textBox">
                                                <img id="_imgClientSearchLoadIndicator" runat="server" alt="" src="../Images/GIFs/indicator.gif" />&nbsp;Loading...
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                
                                </asp:Panel>
                                
                                <asp:UpdatePanel ID="_updPnlClientSearch" runat="server" UpdateMode="Conditional"
                                    ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <table width="100%" id="_tableClients">
                                            <tr>
                                                <td class="sectionHeader" align="left">
                                                    Clients
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                                                        <asp:GridView ID="_grdSearchClientList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                            BorderWidth="0" DataKeyNames="OrganisationId,MemberId" GridLines="None" Width="97%"
                                                            OnRowCommand="_grdSearchClientList_RowCommand" OnRowDataBound="_grdSearchClientList_RowDataBound"
                                                            EmptyDataText="No client(s) found." CssClass="successMessage" 
                                                            AllowSorting="true">
                                                            <Columns>
                                                                <asp:TemplateField Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" class="link" ID="_lnkSelectClient" CommandName="GetMatters"
                                                                            CausesValidation="false" Text='List Matters'></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Client Reference" SortExpression="cliRef">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" class="link" ID="_lnkCliRef" CommandName="select"
                                                                            CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "ClientReference")%>'></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Person Name" SortExpression="Name">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblPersonName" runat="server" Font-Bold="true"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Address" SortExpression="Address">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblAddress" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Address")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Partner" SortExpression="Partner">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblPartner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PartnerName")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="DOB" SortExpression="DateOfBirth">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblDOB" runat="server" Text='<%# Eval("DateOfBirth", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="NI No." SortExpression="personNHINo">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblNINo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "NationalInsuranceNo")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
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
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchClientList" EventName="PageIndexChanging" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchClientList" EventName="Sorting" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:ObjectDataSource ID="_odsSearchClient" runat="server" SelectMethod="SearchClient"
                                    TypeName="IRIS.Law.WebApp.UserControls.ClientSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetClientRowsCount" StartRowIndexParameterName="startRow"
                                    OnSelected="_odsSearchClient_Selected" SortParameterName="sortBy">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="_txtSurname" Name="name" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="_txtNINo" Name="NINo" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="_ddlPartner" Name="partner" PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="_ccDOBDate" Name="DOB" PropertyName="DateText" />
                                        <asp:ControlParameter ControlID="_txtPostcode" Name="postcode" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="_txtTown" Name="town" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                                            Type="Boolean" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                                <asp:UpdatePanel ID="_updPnlMattersForClient" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%" id="_tableMattersForClient" runat="server" style="display: none">
                                            <tr>
                                                <td class="sectionHeader" align="left">
                                                    Matters
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="_grdMattersForClientHeight" runat="server" style="height: 150px; overflow: auto;">
                                                        <asp:GridView ID="_grdMattersForClient" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                                            BorderWidth="0" GridLines="None" Width="97%" EmptyDataText="No matter(s) for client."
                                                            CssClass="successMessage" OnRowCommand="_grdMattersForClient_RowCommand" OnRowDataBound="_grdMattersForClient_RowDataBound"
                                                            DataKeyNames="Id">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Matter Reference">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="_lnkMatterReference" CssClass="link" CommandName="select" runat="server"
                                                                            Text='<%#DataBinder.Eval(Container.DataItem, "Reference")%>' CausesValidation="false">
                                                                        </asp:LinkButton>
                                                                        <asp:Label ID="_lblMatterReference" runat="server" Font-Bold="true" Visible="false"
                                                                            Text='<%#DataBinder.Eval(Container.DataItem, "Reference")%>' ToolTip='<%#DataBinder.Eval(Container.DataItem, "Reference")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Description">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblDescription" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Key Description">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblKeyDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "KeyDescription")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "KeyDescription")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Department">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblDepartment" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DepartmentCode")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "DepartmentName")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Branch">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblBranch" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BranchCode")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "BranchName")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Fee Earner">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerName")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerName")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Work Type">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblWorkType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "WorkTypeCode")%>'
                                                                            ToolTip='<%#DataBinder.Eval(Container.DataItem, "WorkType")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Opened">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblOpened" runat="server" Text='<%# Eval("OpenedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Closed">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="_lblClosed" runat="server" Text='<%# Eval("ClosedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:ObjectDataSource ID="_odsClientMatters" runat="server" SelectMethod="GetClientMatters"
                                            TypeName="IRIS.Law.WebApp.UserControls.ClientSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                            SelectCountMethod="GetClientMattersRowsCount" StartRowIndexParameterName="startRow"
                                            OnSelected="_odsClientMatters_Selected">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="_hdnRefreshClientMatters" Name="forceRefresh" PropertyName="Value"
                                                    Type="Boolean" />
                                                <asp:ControlParameter ControlID="_hdnMemberId" Name="memberId" PropertyName="Value" />
                                                <asp:ControlParameter ControlID="_hdnOrganisationId" Name="organisationId" PropertyName="Value" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                        <asp:HiddenField ID="_hdnProjectRef" runat="server" />
                                        <asp:HiddenField ID="_hdnProjectId" runat="server" />
                                        <asp:HiddenField ID="_hdnRefreshClientMatters" runat="server" />
                                        <asp:HiddenField ID="_hdnMemberId" runat="server" />
                                        <asp:HiddenField ID="_hdnOrganisationId" runat="server" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="_grdSearchClientList" EventName="RowCommand" />
                                    </Triggers>
                                </asp:UpdatePanel>
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

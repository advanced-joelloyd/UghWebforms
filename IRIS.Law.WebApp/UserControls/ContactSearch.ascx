<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactSearch.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ContactSearch" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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

    function ResetContactSearchControls() {
        $("#<%=_txtSurname.ClientID%>").val("");
        $("#<%=_txtOrganisation.ClientID%>").val("");
        $("#<%=_txtHouseNo.ClientID%>").val("");
        $("#<%=_txtPostcode.ClientID%>").val("");
        $("#<%=_txtTown.ClientID%>").val("");
        $("#<%=_txtPOBox.ClientID%>").val("");
        $("#<%=_grdSearchContactList.ClientID%>").css("display", "none");
        $("#<%=_lblMessage.ClientID%>").text("");
        return false;
    }

    function CancelPopupClick() {
        return false;
    }

    function CancelSearchContactPopup() {
        $find('_modalpopupContactSearchBehavior').hide();
    }
</script>

<table style="float: left;" cellpadding="0" cellspacing="0" id="_tblContactName"
    runat="server">
    <tr>
        <td>
            <div style="float: left;">
                <asp:UpdatePanel ID="_updContactName" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="_txtContactName" runat="server"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:ImageButton AlternateText="Contact Search" ID="_imgSearchContactPopup"
                runat="server" ImageUrl="~/Images/PNGs/searchButton.png" ToolTip="Contact Search"
                SkinID="SearchImageIcon" Height="21px" CausesValidation="false" />
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupContactSearch" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlContactSearch" OnCancelScript="javascript:CancelPopupClick();"
                TargetControlID="_imgSearchContactPopup" BehaviorID="_modalpopupContactSearchBehavior"
                CancelControlID="_btnCancel">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<asp:Panel ID="_pnlContactSearch" runat="server" Style="background-color: #ffffff" Width="800px">
    <table width="98%" align="center">
        <tr id="_trCloseLink" runat="server">
            <td align="right">
                <a id="linkClose" onclick="CancelSearchContactPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="_lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader" align="left" runat="server" id="_tdSectionHeader">
                Search Contact
            </td>
        </tr>
        <tr>
            <td width="100%" class="panel">
                <asp:Panel ID="Panel1" runat="server" DefaultButton="_btnSearch">
                    <table cellpadding="1" cellspacing="1" border="0" style="width: 100%;">
                        <tr>
                            <td class="boldTxt" style="width: 125px;">
                                Name
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="_txtSurname"></asp:TextBox>
                            </td>
                            <td class="boldTxt" style="width: 125px;">
                                Organisation
                            </td>
                            <td>
                                <asp:TextBox ID="_txtOrganisation" runat="server"></asp:TextBox>
                            </td>
                            <td valign="top">
                             
                                    <%--<asp:Image ImageUrl="~/Images/PNGs/about.png" ToolTip="Tip" runat="server" 
                                        ID="imgToolTip" />--%>
        
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
                            <td class="boldTxt">
                                House Number
                            </td>
                            <td>
                                <asp:TextBox ID="_txtHouseNo" runat="server"></asp:TextBox>
                            </td>
                            <td class="boldTxt">
                                P O Box
                            </td>
                            <td>
                                <asp:TextBox ID="_txtPOBox" runat="server"></asp:TextBox>
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
                    <table align="right" style="margin-right: 20px; margin-top: 5px;">
                        <tr>
                            <td align="right">
                                <div class="button" style="text-align: center;">
                                    <asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClick="_btnReset_Click"
                                        OnClientClick="return ResetContactSearchControls();" />
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
                                        Text="Search" ValidationGroup="ContactSearch" />
                                </div>
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
                
                <asp:UpdatePanel ID="Update_pnlContactSearch" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="_grdviewDivHeight" runat="server" style="height: 350px; overflow: auto;">
                            <asp:GridView ID="_grdSearchContactList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                                BorderWidth="0" GridLines="None" Width="99%" EmptyDataText="No contact(s) found."
                                OnRowCommand="_grdSearchContactList_RowCommand" CssClass="successMessage" AllowSorting="true" OnRowDataBound="_grdSearchContactList_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Name" Visible="false" SortExpression="Name">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblContactName" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" Visible="false"></asp:Label>
                                            <asp:LinkButton runat="server" class="link" ID="_lnkContactName" CommandName="select"
                                                CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="25%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" Visible="false" SortExpression="Name">
                                        <ItemTemplate>
                                            <asp:Label ID="_lblName" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="25%" />
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Address" HeaderText="Address" HeaderStyle-HorizontalAlign="Left"
                                        ItemStyle-Width="45%" SortExpression="Address" />
                                    <asp:BoundField DataField="Town" HeaderText="Town" HeaderStyle-HorizontalAlign="Left"
                                        ItemStyle-Width="20%" SortExpression="Town" />
                                    <asp:BoundField DataField="PostCode" HeaderText="Postcode" HeaderStyle-HorizontalAlign="Left"
                                        ItemStyle-Width="10%" SortExpression="Postcode" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="_hdnOrgId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "OrganisationId")%>' />
                                            <asp:HiddenField ID="_hdnMemId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MemberId")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                            <asp:HiddenField ID="_hdnSelectedOrgId" runat="server" Value="" />
                            <asp:HiddenField ID="_hdnSelectedMemId" runat="server" Value="" />
                            <asp:ObjectDataSource ID="_odsContactSearch" runat="server" SelectMethod="SearchContact"
                                TypeName="IRIS.Law.WebApp.UserControls.ContactSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="ContactSearchRowCount" StartRowIndexParameterName="startRow"
                                OnSelected="_odsContactSearch_Selected" SortParameterName="sortBy">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="_txtSurname" Name="contactName" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_txtOrganisation" Name="organisation" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_txtHouseNo" Name="houseNo" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_txtPOBox" Name="POBox" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_txtPostcode" Name="postCode" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_txtTown" Name="town" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                                        Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Panel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatterSearch.ascx.cs"
	Inherits="IRIS.Law.WebApp.UserControls.MatterSearch" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CS" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
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

	function ResetMatterSearchControls() {
		$("#<%=_txtDescription.ClientID%>").val("");
		$("#<%=_txtKeyDescription.ClientID%>").val("");

		if ($get("<%=_ddlDepartment.ClientID%>").selectedIndex > 0) {
		    $get("<%=_ddlDepartment.ClientID%>").selectedIndex = 0;
		    $get("<%=_ddlDepartment.ClientID%>").onchange();
		}

		if ($get("<%=_ddlBranch.ClientID%>").selectedIndex > 0) {
		    $get("<%=_ddlBranch.ClientID%>").selectedIndex = 0;
		    $get("<%=_ddlBranch.ClientID%>").onchange();
		}
		
		$("#<%=_ddlFeeEarner.ClientID%>").val("");
		
		$get("<%=_ddlWorkType.ClientID%>").selectedIndex = 0;
		
		$("#<%=_txtReference.ClientID%>").val("");
		$("#<%=_txtPrevReference.ClientID%>").val("");
		$("#<%=_ccOpenedFrom.DateTextBoxClientID%>").val("");
		$("#<%=_ccOpenedTo.DateTextBoxClientID%>").val("");
		$("#<%=_ccClosedFrom.DateTextBoxClientID%>").val("");
		$("#<%=_ccClosedTo.DateTextBoxClientID%>").val("");
		$("#<%=_txtUFN.ClientID%>").val("______/___");
		if ($("#<%=_grdSearchMatterList.ClientID%>") != null) {
		    $("#<%=_grdSearchMatterList.ClientID%>").css("display", "none");
            $("#<%=_lblMessage.ClientID%>").text("");	
		}
		//This value needs to match the NoClientSelected value defined in ClientSearch
		//User type 2 (Client) should not be able to reset Client ref
		var userType = document.getElementById("<%= _hdnUserType.ClientID%>").value

		if (userType != "2") {
		    $("#<%=_clientSearch.SearchClientRefTextBoxClientID%>").val("");
		}
		return false;
	}

	var prm = Sys.WebForms.PageRequestManager.getInstance();
	prm.add_initializeRequest(InitializeRequest);
	prm.add_endRequest(EndRequest);
	var postBackElement;
	var updateProgLeft;
	var updateProgTop;
	function InitializeRequest(sender, args) {
		if (prm.get_isInAsyncPostBack())
			args.set_cancel(true);
		postBackElement = args.get_postBackElement();
		//disable the search button to prevent the user from clicking multiple times 
		//while the request is being processed
		$("#<%=_btnSearch.ClientID %>").attr("disabled", true);
		//Get the original position of the update progress panel so we can restore it later
		
		
	}
	function EndRequest(sender, args) {
		//restore the update panel to its original position
		
		$("#<%=_btnSearch.ClientID %>").attr("disabled", false);
	}
</script>




<table width="100%">
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
		<td colspan="2" class="sectionHeader">
			Search Matter
		</td>
	</tr>	
	<tr>
		<td width="100%">
			<asp:Panel ID="_pnlSearchCriteria" runat="server" DefaultButton="_btnSearch">
				<table width="100%" border="0" cellspacing="0" cellpadding="0" class="panel">
					<tr>
						<td style="width: 100%">
							<table border="0" style="width: 100%;">
								<tr>
									<td class="boldTxt" style="width: 100px;">
										Client Ref.
										<asp:HiddenField ID="_hdnUserType" runat="server" />
									</td>
									<td>
										<CS:ClientSearch ID="_clientSearch" DisplayPopup="true" OnClientReferenceChanged="_clientSearch_ClientReferenceChanged"
											DisplayClientName="True" runat="Server" />
									</td>
									<td class="boldTxt">
										UFN
									</td>
									<td>
										<asp:TextBox ID="_txtUFN" runat="server"></asp:TextBox>
										<ajaxToolkit:MaskedEditExtender ID="_meUFN" runat="server" TargetControlID="_txtUFN"
											Mask="999999/999" MaskType="Number" MessageValidatorTip="true" PromptCharacter="_"
											CultureName="en-GB" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
											ClearMaskOnLostFocus="false" />
									</td>
								</tr>
								<tr>
									<td class="boldTxt" style="width: 125px;">
										<asp:Label ID="_lblDescription" runat="server" Text="Description"></asp:Label>
									</td>
									<td>
										<asp:TextBox runat="server" ID="_txtDescription"></asp:TextBox>
									</td>
									<td class="boldTxt" style="width: 125px;">
										<asp:Label ID="_lblKeyDescription" runat="server" Text="Key Description"></asp:Label>
									</td>
									<td>
										<asp:TextBox ID="_txtKeyDescription" runat="server"></asp:TextBox>
									</td>
								</tr>
								<tr>
									<td class="boldTxt">
										<asp:Label ID="_lblDepartment" runat="server" Text="Department"></asp:Label>
									</td>
									<td>
										<asp:UpdatePanel ID="_updPnlDeparment" runat="server" UpdateMode="Conditional">
											<ContentTemplate>
												<asp:DropDownList ID="_ddlDepartment" runat="server" OnSelectedIndexChanged="_ddlDepartment_SelectedIndexChanged"
													AutoPostBack="true">
												</asp:DropDownList>
											</ContentTemplate>
											<Triggers>
												<asp:AsyncPostBackTrigger ControlID="_ddlBranch" />
											</Triggers>
										</asp:UpdatePanel>
									</td>
									<td class="boldTxt">
										<asp:Label ID="_lblBranch" runat="server" Text="Branch"></asp:Label>
									</td>
									<td>
										<table border="0" cellpadding="0" cellspacing="0">
											<tr>
												<td>
													<asp:DropDownList ID="_ddlBranch" runat="server" OnSelectedIndexChanged="_ddlBranch_SelectedIndexChanged"
														AutoPostBack="true">
													</asp:DropDownList>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td class="boldTxt">
										<asp:Label ID="_lblFeeEarner" runat="server" Text="Fee Earner"></asp:Label>
									</td>
									<td>
										<asp:DropDownList ID="_ddlFeeEarner" runat="server">
										</asp:DropDownList>
									</td>
									<td class="boldTxt">
										<asp:Label ID="_lblWorkType" runat="server" Text="Work Type"></asp:Label>
									</td>
									<td>
										<asp:UpdatePanel ID="_updPnlWorkType" runat="server" UpdateMode="Conditional">
											<ContentTemplate>
												<asp:DropDownList ID="_ddlWorkType" runat="server">
												</asp:DropDownList>
											</ContentTemplate>
											<Triggers>
												<asp:AsyncPostBackTrigger ControlID="_ddlDepartment" />
											</Triggers>
										</asp:UpdatePanel>
									</td>
								</tr>
								<tr>
									<td class="boldTxt">
										<asp:Label ID="_lblOpenedFrom" runat="server" Text="Opened From"></asp:Label>
									</td>
									<td>
										<CC:CalendarControl ID="_ccOpenedFrom" InvalidValueMessage="Invalid Opened From Date"
											runat="server" />
									</td>
									<td class="boldTxt">
										<asp:Label ID="_lblOpenedTo" runat="server" Text="Opened To"></asp:Label>
									</td>
									<td>
										<table border="0" cellpadding="0" cellspacing="0">
											<tr>
												<td>
													<CC:CalendarControl ID="_ccOpenedTo" InvalidValueMessage="Invalid Opened To Date"
														runat="server" />
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td class="boldTxt">
										<asp:Label ID="_lblClosedFrom" runat="server" Text="Closed From"></asp:Label>
									</td>
									<td>
										<CC:CalendarControl ID="_ccClosedFrom" InvalidValueMessage="Invalid Closed From Date"
											runat="server" />
									</td>
									<td class="boldTxt">
										<asp:Label ID="_lblClosedTo" runat="server" Text="Closed To"></asp:Label>
									</td>
									<td>
										<table border="0" cellpadding="0" cellspacing="0">
											<tr>
												<td>
													<CC:CalendarControl ID="_ccClosedTo" InvalidValueMessage="Invalid Closed To Date"
														runat="server" />
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td class="boldTxt">
										<asp:Label ID="_lblReference" runat="server" Text="Reference"></asp:Label>
									</td>
									<td>
										<asp:TextBox ID="_txtReference" runat="server"></asp:TextBox>
									</td>
									<td class="boldTxt">
										<asp:Label ID="_lblPrevReference" runat="server" Text="Previous Reference"></asp:Label>
									</td>
									<td>
										<asp:TextBox ID="_txtPrevReference" runat="server"></asp:TextBox>
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
									<td align="right">
										<table>
											<tr>
												<td align="right">
													<div class="button" style="text-align: center;">
														<asp:Button ID="_btnReset" runat="server" CausesValidation="False" Text="Reset" OnClientClick="return ResetMatterSearchControls();" />
													</div>
												</td>
												<td align="right">
													<div class="button" style="text-align: center;">
														<asp:Button ID="_btnSearch" runat="server" CausesValidation="True" Text="Search"
															OnClick="_btnSearch_Click" />
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
		<td width="10%">
			&nbsp;
		</td>
	</tr>
</table>

<asp:UpdatePanel ID="_updPnlSearchClientList" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:GridView ID="_grdSearchMatterList" runat="server" AllowPaging="true" AutoGenerateColumns="false"
			BorderWidth="0" DataKeyNames="Id" GridLines="None" Width="99%" OnRowDataBound="_grdSearchMatterList_RowDataBound"
			OnRowCommand="_grdSearchMatterList_RowCommand" AllowSorting="true" CssClass="successMessage" EmptyDataText="Search is complete. There are no results to display.">
			<Columns>
				<asp:TemplateField HeaderText="Matter Reference" SortExpression="MatterReference">
					<ItemTemplate>
						<asp:LinkButton ID="_lnkMatterReference" CssClass="link" CommandName="select" runat="server"
							Text='<%#DataBinder.Eval(Container.DataItem, "Reference")%>'>
						</asp:LinkButton>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Description" SortExpression="MatterDescription">
					<ItemTemplate>
						<asp:Label ID="_lblDescription" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Key Description" SortExpression="MatterKeyDescription">
					<ItemTemplate>
						<asp:Label ID="_lblKeyDescription" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "KeyDescription")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "KeyDescription")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Department" SortExpression="DepartmentCode">
					<ItemTemplate>
						<asp:Label ID="_lblDepartment" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DepartmentCode")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "DepartmentName")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Branch" SortExpression="BranchCode">
					<ItemTemplate>
						<asp:Label ID="_lblBranch" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BranchCode")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "BranchName")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Fee Earner" SortExpression="FeeEarner">
					<ItemTemplate>
						<asp:Label ID="_lblFeeEarner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerName")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "FeeEarnerName")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Work Type" SortExpression="WorkTypeCode">
					<ItemTemplate>
						<asp:Label ID="_lblWorkType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "WorkTypeCode")%>'
							ToolTip='<%#DataBinder.Eval(Container.DataItem, "WorkType")%>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Opened" SortExpression="OpenedDate">
					<ItemTemplate>
						<asp:Label ID="_lblOpened" runat="server" Text='<%# Eval("OpenedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Closed" SortExpression="ClosedDate">
					<ItemTemplate>
						<asp:Label ID="_lblClosed" runat="server" Text='<%# Eval("ClosedDate", "{0:dd/MM/yyyy}") %>'></asp:Label>
					</ItemTemplate>
					<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
					<HeaderStyle HorizontalAlign="Left" />
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
		<asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="_btnSearch" EventName="Click" />
	</Triggers>
</asp:UpdatePanel>
<asp:ObjectDataSource ID="_odsSearchMatter" runat="server" SelectMethod="SearchMatter"
	TypeName="IRIS.Law.WebApp.UserControls.MatterSearch" EnablePaging="True" MaximumRowsParameterName="pageSize"
	SelectCountMethod="GetMattersRowsCount" StartRowIndexParameterName="startRow"
	OnSelected="_odsSearchMatter_Selected" SortParameterName="sortBy">
	<SelectParameters>
		<asp:ControlParameter ControlID="_clientSearch" Name="clientReference" PropertyName="SearchText" />
		<asp:ControlParameter ControlID="_txtDescription" Name="description" PropertyName="Text" />
		<asp:ControlParameter ControlID="_txtKeyDescription" Name="keyDescription" PropertyName="Text" />
		<asp:ControlParameter ControlID="_ddlDepartment" Name="department" PropertyName="SelectedValue" />
		<asp:ControlParameter ControlID="_ddlBranch" Name="branch" PropertyName="SelectedValue" />
		<asp:ControlParameter ControlID="_ddlFeeEarner" Name="feeEarner" PropertyName="SelectedValue" />
		<asp:ControlParameter ControlID="_ddlWorkType" Name="workTypeCode" PropertyName="SelectedValue" />
		<asp:ControlParameter ControlID="_ccOpenedFrom" Name="openedFrom" PropertyName="DateText" />
		<asp:ControlParameter ControlID="_ccOpenedTo" Name="openedTo" PropertyName="DateText" />
		<asp:ControlParameter ControlID="_ccClosedFrom" Name="closedFrom" PropertyName="DateText" />
		<asp:ControlParameter ControlID="_ccClosedTo" Name="closedTo" PropertyName="DateText" />
		<asp:ControlParameter ControlID="_txtReference" Name="reference" PropertyName="Text" />
		<asp:ControlParameter ControlID="_txtPrevReference" Name="prevReference" PropertyName="Text" />
		<asp:ControlParameter ControlID="_txtUFN" Name="UFN" PropertyName="Text" />
		<asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value" Type="Boolean" />
	</SelectParameters>
</asp:ObjectDataSource>

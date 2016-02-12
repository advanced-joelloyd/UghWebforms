<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
	CodeBehind="AddMatter.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Matter.AddMatter" Title="Add Matter" %>

<%@ Register TagPrefix="CS" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

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
		
		Sys.Application.add_load(function() {
			//HOUCN length validation
			$("#<%= _txtHOUCN.ClientID %>").change(
		  function() {
		  	var houcn = $("#<%= _txtHOUCN.ClientID %>").val().replace(/_/g, "");
		  	if (houcn.length != 8) {
		  		$("#<%= _updPanelMessage.ClientID %>").css("display", "block");
		  		$("#<%= _lblMessage.ClientID %>").removeClass("successMessage");
		  		$("#<%= _lblMessage.ClientID %>").addClass("errorMessage");
		  		$("#<%= _lblMessage.ClientID %>").text("All characters have not been entered for the HO UCN");
		  	}
		  	else {
		  		$("#<%= _lblMessage.ClientID %>").text("");
		  	}
		  });
		});

		function uppercase(e) {
			var key;
			if (window.event) // IE
			{
				key = e.keyCode;
				if ((key > 0x60) && (key < 0x7B)) {
					window.event.keyCode = key - 0x20;
				}
			}
		}

		var prm = Sys.WebForms.PageRequestManager.getInstance();
		prm.add_initializeRequest(InitializeRequest);
		prm.add_endRequest(EndRequest);
		var postBackElement;
		function InitializeRequest(sender, args) {
			if (prm.get_isInAsyncPostBack())
				args.set_cancel(true);
			postBackElement = args.get_postBackElement();
			if (postBackElement.type == "select-one") {
				var left = $("#" + postBackElement.id).position().left + $("#" + postBackElement.id).width() + 20;
				var top = $("#" + postBackElement.id).position().top;
				$("#ctl00__cphMain__divUpdateProgress").css("position", "absolute");
				$("#ctl00__cphMain__divUpdateProgress").css("left", left + "px");
				$("#ctl00__cphMain__divUpdateProgress").css("top", top + "px");
			}
			else {
				$("#ctl00__cphMain__divUpdateProgress").css("position", "");
			}
		}
		function EndRequest(sender, args) {
			
		}
	</script>

	
	<asp:UpdatePanel ID="_updPanelMessage" runat="server">
		<ContentTemplate>
			<asp:Label ID="_lblMessage" runat="server"></asp:Label>
		</ContentTemplate>
	</asp:UpdatePanel>
	<table width="100%">
		<tr>
			<td class="sectionHeader">
				Add New Matter
			</td>
		</tr>
		<tr>
			<td>
			</td>
		</tr>
	</table>
	<asp:UpdatePanel ID="_updPnlAddMatter" runat="server">
        <ContentTemplate>
	
	
	<asp:Wizard ID="_wizardAddMatter" runat="server" DisplaySideBar="false" ActiveStepIndex="0"
		Width="100%" EnableTheming="True" OnActiveStepChanged="ValidateStepChange" Height="420px" StepStyle-VerticalAlign="Top">
		<StepStyle VerticalAlign="Top" />
		<StartNextButtonStyle CssClass="button" />
		<FinishCompleteButtonStyle CssClass="button" />
		<StepNextButtonStyle CssClass="button" />
		<FinishPreviousButtonStyle CssClass="button" />
		<NavigationButtonStyle CssClass="button" />
		<StepPreviousButtonStyle CssClass="button" />
		<StartNavigationTemplate>
			<table width="100%">
				<tr>
					<td align="right">
						<table>
							<tr>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStartNavCancel" OnClick="_btnWizardStartNavCancel_Click"
											runat="server" Text="Reset" CausesValidation="false" />
									</div>
								</td>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStartNext" runat="server" CommandName="MoveNext" OnClick="_btnWizardStartNext_Click"
											Text="Next" />
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</StartNavigationTemplate>
		<StepNavigationTemplate>
			<table width="100%">
				<tr>
					<td align="right">
						<table>
							<tr>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStepPreviousButton" runat="server" CommandName="MovePrevious"
											Text="Previous" CausesValidation="false" />
									</div>
								</td>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStepNextButton" runat="server" CommandName="MoveNext" Text="Next" />
									</div>
								</td>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStepNavCancel" OnClick="_btnWizardStartNavCancel_Click"
											runat="server" Text="Reset" CausesValidation="false" />
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</StepNavigationTemplate>
		<FinishNavigationTemplate>
			<table width="100%">
				<tr>
					<td align="right">
						<table>
							<tr>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardFinishPreviousButton" runat="server" CommandName="MovePrevious"
											Text="Previous" CausesValidation="false" />
									</div>
								</td>
								
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardFinishNavCancel" OnClick="_btnWizardStartNavCancel_Click"
											runat="server" Text="Reset" CausesValidation="false" />
									</div>
								</td>
								<td align="right">
									<div class="button" style="text-align: center;">
										<asp:Button ID="_btnWizardStepFinishButton" runat="server" CausesValidation="true"
											CommandName="MoveComplete" OnClick="_btnWizardStepFinishButton_Click" Text="Finish" />
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</FinishNavigationTemplate>
		<WizardSteps>
			<asp:WizardStep ID="_wizardStepMatterDetails" runat="server" Title="Matter Details">
				<table width="100%">
					<tr>
						<td colspan="2">
							<asp:Panel ID="_pnlClientDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
								Client</asp:Panel>
						</td>
					</tr>
					<tr>
						<td class="boldTxt" style="width: 150px;">
							Client
						</td>
						<td>
						    <table cellpadding="2">
						    <tr>
						        <td><CS:ClientSearch ID="_clientSearch" DisplayPopup="true" OnClientReferenceChanged="_clientSearch_ClientReferenceChanged"
								EnableValidation="true" runat="Server" DisplayClientName="true" /></td>
						        <td><span class="mandatoryField">&nbsp;*</span></td>
						    </tr>
						    </table>
							
							
						</td>
					</tr>
					<tr>
						<td class="boldTxt" style="vertical-align: top; padding-top: 7px; width: 150px;">
							Multiple Clients
						</td>
						<td>
							<asp:UpdatePanel ID="_updClientAssociates" runat="server">
								<ContentTemplate>
									<div style="height: 100px; width: 300px; overflow: auto; border: solid 1px #7F9DB9;">
										<asp:CheckBoxList ID="_chklstClientAssociates" runat="server">
										</asp:CheckBoxList>
									</div>
								</ContentTemplate>
							</asp:UpdatePanel>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
								Matter</asp:Panel>
						</td>
					</tr>
				</table>
				<asp:UpdatePanel ID="_updPnlMatterInfo" runat="server">
					<ContentTemplate>
						<table width="100%">
							<tr id="_trMatterType" runat="server" visible="false">
								<td class="boldTxt" style="width: 150px;">
									Matter Type
								</td>
								<td>
									<asp:DropDownList ID="_ddlMatterType" runat="server" SkinID="Large">
									</asp:DropDownList>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Description
								</td>
								<td>
									<asp:TextBox ID="_txtDescription" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();"></asp:TextBox>
									<asp:RequiredFieldValidator ID="_rfvDescription" runat="server" ErrorMessage="Description is mandatory"
										Display="None" ControlToValidate="_txtDescription"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Branch
								</td>
								<td>
									<asp:DropDownList ID="_ddlBranch" runat="server" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();" AutoPostBack="True" OnSelectedIndexChanged="_ddlBranch_SelectedIndexChanged">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvBranch" runat="server" ErrorMessage="Branch is mandatory"
										Display="None" ControlToValidate="_ddlBranch"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Department
								</td>
								<td>
									<asp:DropDownList ID="_ddlDepartment" runat="server" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();" AutoPostBack="True" OnSelectedIndexChanged="_ddlDepartment_SelectedIndexChanged">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvDepartment" runat="server" ErrorMessage="Department is mandatory"
										Display="None" ControlToValidate="_ddlDepartment"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Work Type
								</td>
								<td>
									<asp:DropDownList ID="_ddlWorkType" runat="server" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();" AutoPostBack="True" OnSelectedIndexChanged="_ddlWorkType_SelectedIndexChanged">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvWorkType" runat="server" ErrorMessage="Work Type is mandatory"
										Display="None" ControlToValidate="_ddlWorkType"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Fee Earner
								</td>
								<td>
									<asp:DropDownList ID="_ddlFeeEarner" runat="server" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();" OnSelectedIndexChanged="_txtUFNDate_TextChanged"
										AutoPostBack="true">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvFeeEarner" runat="server" ErrorMessage="Fee Earner is mandatory"
										Display="None" ControlToValidate="_ddlFeeEarner"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr id="_trUFN" runat="server" visible="false">
								<td class="boldTxt">
									UFN
								</td>
								<td>
									<CC:CalendarControl ID="_ccUFNDate" InvalidValueMessage="Invalid UFN Date" runat="server"
										OnDateChanged="_txtUFNDate_TextChanged" />
									/&nbsp;<asp:TextBox ID="_txtUFNNumber" runat="server" onkeypress="return CheckNumeric(event);" onkeyup="CheckUnits(this)" SkinID="Small" OnTextChanged="_txtUFNNumber_TextChanged"
										AutoPostBack="true"></asp:TextBox>
								</td>
							</tr>
							<tr id="_trHOUCN" runat="server" visible="false">
								<td class="boldTxt">
									HO UCN
								</td>
								<td>
									<asp:TextBox ID="_txtHOUCN" runat="server" onKeypress="uppercase(event)"></asp:TextBox>
									<ajaxToolkit:MaskedEditExtender ID="_meeUCN" TargetControlID="_txtHOUCN" Mask="L9999999"
										MaskType="None" ClearMaskOnLostFocus="false" runat="server" />
								</td>
							</tr>
							<tr id="_trUCN" runat="server" visible="false">
								<td class="boldTxt">
									UCN
								</td>
								<td>
									<asp:TextBox ID="_txtUCN" runat="server" ReadOnly="true"></asp:TextBox>
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
			</asp:WizardStep>
			<asp:WizardStep ID="_wizardStepSystemInfo" runat="server" Title="System Info">
				<table width="100%">
					<tr>
						<td>
							<asp:Panel ID="_pnlSystemInfoHeader" runat="server" Width="99.9%" CssClass="bodyTab">
								System Info</asp:Panel>
						</td>
					</tr>
				</table>
				<asp:UpdatePanel ID="_updPnlSystemInfo" runat="server">
					<ContentTemplate>
						<table width="100%">
							<tr>
								<td class="boldTxt" style="width: 150px;">
									Charge Rate
								</td>
								<td>
									<asp:DropDownList ID="_ddlChargeRate" runat="server" SkinID="Large" AutoPostBack="true"
										OnSelectedIndexChanged="_ddlChargeRate_SelectedIndexChanged" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvChargeRate" runat="server" ErrorMessage="Charge Rate is mandatory"
										Display="None" ControlToValidate="_ddlChargeRate"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Court Type
								</td>
								<td>
									<asp:DropDownList ID="_ddlCourtType" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvCourtType" runat="server" ErrorMessage="Court Type is mandatory"
										Display="None" ControlToValidate="_ddlCourtType"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Client Bank
								</td>
								<td>
									<asp:DropDownList ID="_ddlClientBank" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_frvClientBank" runat="server" ErrorMessage="Client Bank is mandatory"
										Display="None" ControlToValidate="_ddlClientBank"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Office Bank
								</td>
								<td>
									<asp:DropDownList ID="_ddlOfficeBank" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_frvOfficeBank" runat="server" ErrorMessage="Office Bank is mandatory"
										Display="None" ControlToValidate="_ddlOfficeBank"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td class="boldTxt">
									Supervisor
								</td>
								<td>
									<asp:DropDownList ID="_ddlSupervisor" runat="server" onmousemove="showToolTip(event);return false;"
										onmouseout="hideToolTip();">
									</asp:DropDownList>
									<asp:RequiredFieldValidator ID="_rfvSupervisor" runat="server" ErrorMessage="Supervisor is mandatory"
										Display="None" ControlToValidate="_ddlSupervisor"></asp:RequiredFieldValidator>
									<span class="mandatoryField">*</span>
								</td>
							</tr>
							<tr>
								<td>
									&nbsp;
								</td>
								<td>
									<asp:CheckBox ID="_chkPublicFunding" runat="server" Text="Public Funding" Enabled="false"
										AutoPostBack="true" OnCheckedChanged="_chkPublicFunding_CheckedChanged" />
									<asp:CheckBox ID="_chkSQM" runat="server" Text="SQM" Enabled="false" />
								</td>
							</tr>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
			</asp:WizardStep>
		</WizardSteps>
	</asp:Wizard>
	</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>

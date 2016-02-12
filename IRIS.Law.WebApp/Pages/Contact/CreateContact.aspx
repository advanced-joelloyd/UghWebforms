<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="CreateContact.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Contact.CreateContact"
    Title="Create Contact" %>

<%@ Register TagPrefix="Address" TagName="AddressDetails" Src="~/UserControls/AddressDetails.ascx" %>
<%@ Register TagPrefix="SS" TagName="ServiceSearch" Src="~/UserControls/ServiceSearch.ascx" %>
<%@ Register TagPrefix="CD" TagName="ContactDetails" Src="~/UserControls/ContactDetails.ascx" %>
<%@ Register TagPrefix="AAD" TagName="AdditionalAddressDetails" Src="~/UserControls/AdditionalAddressDetails.ascx" %>
<%@ Register TagPrefix="UC" TagName="ConflictCheck" Src="~/UserControls/ConflictCheck.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentCreateContact" ContentPlaceHolderID="_cphMain" runat="server">

    <script src="../Javascript/Address.js" type="text/javascript"></script>

    <script type="text/javascript">
        
        function EnableDisableValidatorsForServiceType() {
            var serviceType = document.getElementById("<%=_ddlServiceType.ClientID%>").value;

            ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvOrgName.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvServiceName.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvIndustry.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_ssService.ValidatorClientID%>"), false);

            switch (serviceType) {
                case "General Contact":
                    //ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), true);
                    EnableDisableValidatorsForContactType();
                    break;
                case "Service":
                    ValidatorEnable(document.getElementById("<%=_rfvServiceName.ClientID%>"), true);
                    ValidatorEnable(document.getElementById("<%=_rfvIndustry.ClientID%>"), true);
                    break;
                case "Service Contact":
                    ValidatorEnable(document.getElementById("<%=_ssService.ValidatorClientID%>"), true);
                    break;
            }
        }

        function EnableDisableValidatorsForContactType() {
            var clientType = document.getElementById("<%=_ddlContactType.ClientID%>").value;

            switch (clientType) {
                case "Individual":
                    ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), true);
                    break;
                case "Organisation":
                    ValidatorEnable(document.getElementById("<%=_rfvOrgName.ClientID%>"), true);
                    break;
            }
        }

        function HideUnhideControlsOnContactType(ddlContactType) {

            var clientType = document.getElementById(ddlContactType).value;
            document.getElementById("<%=_trSurname.ClientID%>").style.display = "none";
            document.getElementById("<%=_trForename.ClientID%>").style.display = "none";
            document.getElementById("<%=_trTitle.ClientID%>").style.display = "none";
            document.getElementById("<%=_trOrgName.ClientID%>").style.display = "none";

            switch (clientType) {
                case "Individual":
                    document.getElementById("<%=_trSurname.ClientID%>").style.display = "";
                    document.getElementById("<%=_trForename.ClientID%>").style.display = "";
                    document.getElementById("<%=_trTitle.ClientID%>").style.display = "";
                    break;
                case "Organisation":
                    document.getElementById("<%=_trOrgName.ClientID%>").style.display = "";
                    break;
            }
            return false;
        }

        function HideUnhideContactTypeonServiceType(ddlServiceType) {

            var ServiceType = document.getElementById(ddlServiceType).value;

            ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvOrgName.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvServiceName.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvIndustry.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_ssService.ValidatorClientID%>"), false);

            document.getElementById("<%=_tblGeneralContact.ClientID%>").style.display = "none";
            document.getElementById("<%=_tblService.ClientID%>").style.display = "none";
            document.getElementById("<%=_tblServiceContact.ClientID%>").style.display = "none";

            var message = document.getElementById("<%=_lblError.ClientID%>").innerText;
            if (message != "") {
                document.getElementById("<%=_lblError.ClientID%>").innerText = "";
            }

            switch (ServiceType) {
                case "General Contact":
                    document.getElementById("<%=_tblGeneralContact.ClientID%>").style.display = "";
                    $("#<%=_lblContactHeader.ClientID%>").text("Add New General Contact");
                    break;
                case "Service":
                    document.getElementById("<%=_tblService.ClientID%>").style.display = "";
                    $("#<%=_lblContactHeader.ClientID%>").text("Add New Service");
                    break;
                case "Service Contact":
                    document.getElementById("<%=_tblServiceContact.ClientID%>").style.display = "";
                    $("#<%=_lblContactHeader.ClientID%>").text("Add New Service Contact");
                    break;
            }
            return false;
        }

        
    </script>
    
    <script type="text/javascript">
        var browser = navigator.appName;
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }

        
        
    </script>

    
    <asp:UpdatePanel ID="_updPnlCreateContact" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="sectionHeader">
                        <asp:Label ID="_lblContactHeader" runat="server" Text="Add New General Contact"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Wizard ID="_wizardContact" runat="server" DisplaySideBar="False" ActiveStepIndex="0"
                Width="100%" EnableTheming="True" Height="400px" StepStyle-VerticalAlign="Top">
                <StepStyle VerticalAlign="Top" />
                <StartNextButtonStyle CssClass="button" />
                <FinishCompleteButtonStyle CssClass="button" />
                <StepNextButtonStyle CssClass="button" />
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
                                                <asp:Button ID="StartNextButton" OnClientClick="EnableDisableValidatorsForServiceType()"
                                                    runat="server" CommandName="MoveNext" OnClick="StartNextButton_Click" Text="Next" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </StartNavigationTemplate>
                <FinishPreviousButtonStyle CssClass="button" />
                <NavigationButtonStyle CssClass="button" />
                <StepNavigationTemplate>
                    <table width="100%">
                        <tr>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepNavCancel" OnClick="_btnWizardStartNavCancel_Click"
                                                    runat="server" Text="Reset" CausesValidation="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                                    Text="Previous" OnClick="StepPreviousButton_Click" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" OnClick="StepNextButton_Click"
                                                    Text="Next" />
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
                                                <asp:Button ID="_btnWizardFinishNavCancel" OnClick="_btnWizardStartNavCancel_Click"
                                                    runat="server" Text="Reset" CausesValidation="false" />
                                                <asp:HiddenField ID="_hdnSaveStatus" runat="server" Value="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepPreviousButton1" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                                    Text="Previous" OnClick="StepFinishPreviousButton_Click" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepFinishButton" runat="server" CausesValidation="true" CommandName="MoveComplete"
                                                    OnClick="StepFinishButton_Click" Text="Finish" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </FinishNavigationTemplate>
                <StepPreviousButtonStyle CssClass="button" />
                <WizardSteps>
                    <asp:WizardStep ID="_wizardStepClientDetails" runat="server" Title="Matter Details">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlCreateService" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Create</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr id="Tr2" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Type
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="_ddlServiceType" CausesValidation="false" runat="server">
                                                    <asp:ListItem Selected="True" Text="General Contact" Value="General Contact"></asp:ListItem>
                                                    <asp:ListItem Text="Service" Value="Service"></asp:ListItem>
                                                    <asp:ListItem Text="Service Contact" Value="Service Contact"></asp:ListItem>
                                                </asp:DropDownList>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="TitleSeparation">
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" id="_tblGeneralContact" runat="server" style="display: none">
                                        <tr id="_trContactType" runat="server">
                                            <td colspan="2">
                                                <asp:Panel ID="_pnlContactTypeHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                                    Contact Type</asp:Panel>
                                            </td>
                                        </tr>
                                        <tr id="_trContactTypedropdown" runat="server">
                                            <td colspan="2">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr id="Tr1" runat="server">
                                                        <td class="boldTxt" style="width: 150px;">
                                                            Contact Type
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="_ddlContactType" CausesValidation="false" runat="server">
                                                                <asp:ListItem Selected="True" Text="Individual" Value="Individual"></asp:ListItem>
                                                                <asp:ListItem Text="Organisation" Value="Organisation"></asp:ListItem>
                                                            </asp:DropDownList>
                                                            <span class="mandatoryField">*</span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr class="TitleSeparation">
                                            <td colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                                    Name</asp:Panel>
                                            </td>
                                        </tr>
                                        <tr id="_trSurname" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Surname
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtSurname" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvSurname" Display="None" ErrorMessage="Surname is mandatory"
                                                    runat="server" ControlToValidate="_txtSurname"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                        <tr id="_trForename" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Forename
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtForename" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="_trTitle" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Title
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="_ddlTitle" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="_trOrgName" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Name
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtOrgName" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvOrgName" runat="server" ControlToValidate="_txtOrgName"
                                                    Display="None" ErrorMessage="Name is mandatory"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="_tblServiceContact" runat="server" style="display: none" width="100%">
                                        <tr id="_trServiceContactHeader" runat="server">
                                            <td colspan="2">
                                                <asp:Panel ID="_pnlServiceContactHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                                    Service</asp:Panel>
                                            </td>
                                        </tr>
                                        <tr id="_trServiceContactService" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Service
                                            </td>
                                            <td>
                                            <asp:UpdatePanel ID="_updPnlServiceSearch" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                                            <ContentTemplate>
                                                <SS:ServiceSearch ID="_ssService" runat="server" DisplayPopup="true" EnableValidation="true"
                                                    DisplayServiceContactGridview="false" DisplayServiceText="True" OnServiceSelected="_ssServiceSearch_ServiceSelected">
                                                </SS:ServiceSearch>
                                            </ContentTemplate>
                                            </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                    <table id="_tblService" runat="server" style="display: none" width="100%">
                                        <tr id="_trServiceHeader" runat="server">
                                            <td colspan="2">
                                                <asp:Panel ID="_pnlServiceHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                                    Service Details</asp:Panel>
                                            </td>
                                        </tr>
                                        <tr id="_trServiceName" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Service Name
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtServiceName" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="_rfvServiceName" runat="server" ControlToValidate="_txtServiceName"
                                                    Display="None" ErrorMessage="Service Name is mandatory"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                        <tr id="_trServiceIndustry" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Industry
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="_ddlIndustry" SkinID="Large" runat="server" onmousemove="showToolTip(event);return false;"
                                                    onmouseout="hideToolTip();">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="_rfvIndustry" runat="server" ControlToValidate="_ddlIndustry"
                                                    Display="None" InitialValue="0" ErrorMessage="Industry is mandatory"></asp:RequiredFieldValidator>
                                                <span class="mandatoryField">*</span>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAddressDetails" runat="server" Title="Address Details">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlAddressDetails" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="_lblAddressDetailsHeader" runat="server" Text="Address Details"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <Address:AddressDetails ID="_addressDetails" IsMailingAddressEnabled="false" IsBillingAddressEnabled="false"
                                        IsBillingAddress="true" IsMailingAddress="true" HideMapControl="true" runat="Server" IsLastVerifiedEnabled="false" />
                                </td>
                            </tr>
                        </table>
                        
                       
                        
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepContactDetails" runat="server" Title="Contact Details">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlContactDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="_lblContactDetailsHeader" runat="server" Text="Contact Details"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <CD:ContactDetails ID="_cdContactDetails" runat="server" EnableDescription="false" />
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAddressContactInfo" runat="server" Title="Additional Contact Info">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlAddContactInfo" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="_lblContactInfoHeader" runat="server" Text="Additional Contact Info"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <AAD:AdditionalAddressDetails ID="_aadContactInfo" runat="server"></AAD:AdditionalAddressDetails>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepServiceContactDetails" runat="server" Title="Contact Details">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlServiceContactDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="_lblServiceContactDetailsHeader" runat="server" Text="Contact Details"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <CD:ContactDetails ID="_cdServiceContactDetails" runat="server" LabelSurname="Contact Surname"
                                        LabelForename="Contact Forename" LabelTitle="Contact Title" LabelPosition="Contact Position"
                                        LabelDescription="Contact Description" LabelSex="Contact Sex" EnableSex="false" />
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepServiceAddressDetails" runat="server" Title="Address Details">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlServiceAddressDetails" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="Label4" runat="server" Text="Contact Address Details"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="_chkUseSameAddress" SkinID="Label" runat="server" AutoPostBack="true"
                                        OnCheckedChanged="_chkUseSameAddress_Click" Text="Use same address as first address" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <Address:AddressDetails ID="_addressServiceDetails" IsMailingAddressEnabled="false"
                                        IsBillingAddressEnabled="false" IsBillingAddress="true" IsMailingAddress="true" HideMapControl="true"
                                        runat="Server" IsLastVerifiedEnabled="false" />
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepServiceAddressInfo" runat="server" Title="Additional Contact Info">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlAddContactInfo1" runat="server" Width="99.9%" CssClass="bodyTab">
                                        <asp:Label ID="_lblAddContactInfo1" runat="server" Text="Contact Additional Address Details"></asp:Label>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        <AAD:AdditionalAddressDetails ID="_aadServiceAddressInfo" runat="server" LabelHeader="Contact Additional Address Details">
                        </AAD:AdditionalAddressDetails>
                    </asp:WizardStep>
                </WizardSteps>
            </asp:Wizard>
            <table width="100%" runat="server" id="_tblConflictCheck" style="display: none;">
                <tr>
                    <td>
                        <UC:ConflictCheck runat="server" ID="_conflictCheck" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnConflictCancel" OnClick="_btnConflictCancel_Click" runat="server"
                                            Text="Reset" CausesValidation="false" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnConflickOK" runat="server" CausesValidation="false" OnClick="_btnConflickOK_Click"
                                            Text="Add Contact" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    CodeBehind="AddClient.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Client.AddClient" Title="Add New Client" %>

<%@ Register TagPrefix="Address" TagName="AddressDetails" Src="~/UserControls/AddressDetails.ascx" %>
<%@ Register TagPrefix="UC" TagName="ConflictCheck" Src="~/UserControls/ConflictCheck.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_cphTestMaster" ContentPlaceHolderID="_cphMain" runat="Server">

    <script src="../../Javascript/Address.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function HideUnhideControlsOnClientType(ddlClientType) {

            var clientType = document.getElementById(ddlClientType).value;
            document.getElementById("<%=_trSurname.ClientID%>").style.display = "none";
            document.getElementById("<%=_trForename.ClientID%>").style.display = "none";
            document.getElementById("<%=_trTitle.ClientID%>").style.display = "none";
            document.getElementById("<%=_trOrgName.ClientID%>").style.display = "none";
            document.getElementById("<%=_trMatter.ClientID%>").style.display = "none";

            ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), false);
            ValidatorEnable(document.getElementById("<%=_rfvOrgName.ClientID%>"), false);

            switch (clientType) {
                case "Individual":
                    document.getElementById("<%=_trSurname.ClientID%>").style.display = "";
                    document.getElementById("<%=_trForename.ClientID%>").style.display = "";
                    document.getElementById("<%=_trTitle.ClientID%>").style.display = "";
                    document.getElementById("<%=_lblName.ClientID%>").innerHTML = "Name";
                    break;
                case "Organisation":
                    document.getElementById("<%=_trOrgName.ClientID%>").style.display = "";
                    document.getElementById("<%=_lblName.ClientID%>").innerHTML = "Name";
                    break;
                case "Multiple":
                    document.getElementById("<%=_trSurname.ClientID%>").style.display = "";
                    document.getElementById("<%=_trForename.ClientID%>").style.display = "";
                    document.getElementById("<%=_trTitle.ClientID%>").style.display = "";
                    document.getElementById("<%=_trMatter.ClientID%>").style.display = "";
                    document.getElementById("<%=_lblName.ClientID%>").innerHTML = "First Name";
                    break;
            }
            return false;
        }

        function EnableDisableValidators() {
            var clientType = document.getElementById("<%=_ddlClientType.ClientID%>").value;

            switch (clientType) {
                case "Individual":
                    ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), true);
                    break;
                case "Organisation":
                    ValidatorEnable(document.getElementById("<%=_rfvOrgName.ClientID%>"), true);
                    break;
                case "Multiple":
                    ValidatorEnable(document.getElementById("<%=_rfvSurname.ClientID%>"), true);
                    break;
            }
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
   
    <asp:UpdatePanel ID="_updPnlAddClient" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="sectionHeader">
                        Add New Client
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="_lblError" CssClass="errorMessage" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Wizard ID="_wizardAddClient" runat="server" DisplaySideBar="False" ActiveStepIndex="0"
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
                                                <asp:Button ID="StartNextButton" OnClientClick="EnableDisableValidators()" runat="server"
                                                    CommandName="MoveNext" Text="Next" />
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
                                                    Text="Previous" />
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
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="StepPreviousButton1" runat="server" CausesValidation="False" CommandName="MovePrevious"
                                                    Text="Previous" />
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
                                <td colspan="2">
                                    <asp:Panel ID="_pnlClientDetailsHeader" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Client Type</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr id="Tr1" runat="server">
                                            <td class="boldTxt" style="width: 150px;">
                                                Client Type
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="_ddlClientType" CausesValidation="false" runat="server">
                                                    <asp:ListItem Selected="True" Text="Individual" Value="Individual"></asp:ListItem>
                                                    <asp:ListItem Text="Organisation" Value="Organisation"></asp:ListItem>
                                                    <asp:ListItem Text="Multiple" Value="Multiple"></asp:ListItem>
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
                                        <asp:Label ID="_lblName" runat="server" Text="Name" CssClass="labelValue"></asp:Label>
                                    </asp:Panel>
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
                            <tr id="_trMatter" runat="server">
                                <td style="width: 150px;">
                                </td>
                                <td>
                                    <asp:CheckBox ID="_chkMatter" runat="server" Checked="true" Text="Make this the primary client"
                                        CssClass="labelValue" OnCheckedChanged="_chkMatter_Click" />
                                </td>
                            </tr>
                            <tr class="TitleSeparation">
                                <td colspan="2">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlSystemFields" runat="server" Width="99.9%" CssClass="bodyTab">
                                        System Fields</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Partner
                                </td>
                                <td>
                                    <asp:DropDownList ID="_ddlPartner" runat="server" onmousemove="showToolTip(event);return false;"
                                        onmouseout="hideToolTip();">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="_rfvPartner" runat="server" ControlToValidate="_ddlPartner"
                                        Display="None" ErrorMessage="Partner is mandatory"></asp:RequiredFieldValidator>
                                    <span class="mandatoryField">*</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Branch
                                </td>
                                <td>
                                    <asp:DropDownList ID="_ddlBranch" runat="server" onmousemove="showToolTip(event);return false;"
                                        onmouseout="hideToolTip();">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="_rfvBranch" runat="server" ControlToValidate="_ddlBranch"
                                        Display="None" ErrorMessage="Branch is mandatory"></asp:RequiredFieldValidator>
                                    <span class="mandatoryField">*</span>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAddressDetails" runat="server" Title="Address Details">
                        <Address:AddressDetails ID="_addressDetails" IsLastVerifiedEnabled="false" IsMailingAddressEnabled="false" IsBillingAddressEnabled="false"
                            IsBillingAddress="true" IsMailingAddress="true" runat="Server" HideMapControl="true" />
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAddressContactInfo" runat="server" Title="Additional Contact Info">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlAddContactInfo" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Additional Contact Info</asp:Panel>
                                </td>
                            </tr>
                            <tr id="_trHomeTel" runat="server">
                                <td class="boldTxt" style="width: 150px;">
                                    Home Telephone
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtHomeTelephone" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work Telephone 1
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWorkTel1" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work Telephone 2
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWorkTel2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="_trDDI" runat="server">
                                <td class="boldTxt" style="width: 150px;">
                                    DDI
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtDDI" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Mobile Telephone 1
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtMob1" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Mobile Telephone 2
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtMob2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Fax
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtFax" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="_trHomeEmail" runat="server">
                                <td class="boldTxt" style="width: 150px;">
                                    Home E-Mail Address
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtHomeEmail" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work E-Mail Address
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtWorkEmail" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    URL
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtURL" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAddSecondPerson" runat="server" Title="Additional Contact Info">
                        <table width="100%">
                            <tr>
                                <td>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Panel ID="_pnlSecondPerson" runat="server" Width="99.9%" CssClass="bodyTab">
                                                Second Person</asp:Panel>
                                        </td>
                                    </tr>
                                    <tr class="TitleSeparation">
                                    </tr>
                                    <tr id="_trSecondSurname" runat="server">
                                        <td class="boldTxt" style="width: 150px;">
                                            Surname
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtSecondSurname" runat="server" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="_rfvSecondSurname" runat="server" ControlToValidate="_txtSecondSurname"
                                                Display="None" ErrorMessage="Surname is mandatory"></asp:RequiredFieldValidator>
                                            <span class="mandatoryField">*</span>
                                        </td>
                                    </tr>
                                    <tr id="_trSecondForename" runat="server">
                                        <td class="boldTxt" style="width: 150px;">
                                            Forename
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtSecondForename" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr id="_trSecondTitle" runat="server">
                                        <td class="boldTxt" style="width: 150px;">
                                            Title
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlSecondTitle" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="_trSecondRelationship" runat="server">
                                        <td class="boldTxt" style="width: 150px;">
                                            Relationship
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlSecondRelationship" runat="server" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="_rfvRelationship" runat="server" ControlToValidate="_ddlSecondRelationship"
                                                Display="None" ErrorMessage="Relationship is mandatory"></asp:RequiredFieldValidator>
                                            <span class="mandatoryField">*</span>
                                        </td>
                                    </tr>
                                    <tr id="_trSecondMatter" runat="server">
                                        <td style="width: 150px;">
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="_chkSecondMatter" runat="server" Checked="false" Text="Make this the primary client"
                                                CssClass="labelValue" OnCheckedChanged="_chkSecondMatter_Click" />
                                        </td>
                                    </tr>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardSecondAddressDetails" runat="server" Title="Second Address Details">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="_chkUseSameAddress" SkinID="Label" runat="server" AutoPostBack="true"
                                        OnCheckedChanged="_chkUseSameAddress_Click" Text="Use same address as first address" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <Address:AddressDetails ID="_addressDetailsSecond" IsMailingAddressEnabled="false"
                                        IsBillingAddressEnabled="false" IsBillingAddress="true" IsMailingAddress="true" HideMapControl="true"
                                        runat="Server" />
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepSecondAdditionalContactInfo" runat="server" Title="Additional Contact Info">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlSecondAddContactInfo" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Additional Contact Info</asp:Panel>
                                </td>
                            </tr>
                            <tr class="TitleSeparation">
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox ID="_chkSameContactInfo" SkinID="Label" runat="server" Text="Same as first contact info"
                                        AutoPostBack="true" OnCheckedChanged="_chkSameContactInfo_Click" />
                                </td>
                            </tr>
                            <tr class="TitleSeparation">
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Home Telephone
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondHomeTel" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work Telephone 1
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondWorkTel1" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work Telephone 2
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondWorkTel2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    DDI
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondDDI" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Mobile Telephone 1
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondMob1" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Mobile Telephone 2
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondMob2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Fax
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondFax" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Home E-Mail Address
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondHomeEmail" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    Work E-Mail Address
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondWorkEmail" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="boldTxt" style="width: 150px;">
                                    URL
                                </td>
                                <td>
                                    <asp:TextBox ID="_txtSecondURL" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                </WizardSteps>
            </asp:Wizard>
            <table id="_tblConflictCheck" runat="server" width="100%">
                <tr>
                    <td colspan="2">
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
                                            Text="Add Client" />
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

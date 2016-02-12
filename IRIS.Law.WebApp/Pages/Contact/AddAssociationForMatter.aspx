<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="AddAssociationForMatter.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Contact.AddAssociationForMatter"
    Title="Add Association for Matter" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="ContactSearch" TagName="ContactSearch" Src="~/UserControls/ContactSearch.ascx" %>
<%@ Register TagPrefix="ServiceSearch" TagName="ServiceSearch" Src="~/UserControls/ServiceSearch.ascx" %>
<%@ Register TagPrefix="ClientSearch" TagName="ClientSearch" Src="~/UserControls/ClientSearch.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register TagPrefix="UC" TagName="ConflictCheck" Src="~/UserControls/ConflictCheck.ascx" %>
<asp:Content ID="_contentAddAssociationForMatter" ContentPlaceHolderID="_cphMain"
    runat="server">

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
    </script>

    <asp:UpdatePanel ID="_updPnlAddAssociationforMatter" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                            <ContentTemplate>
                                <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="sectionHeader">
                        Add Associations for Matter
                    </td>
                </tr>
            </table>
            <asp:Wizard ID="_wizardAddAssociationsForMatter" runat="server" DisplaySideBar="False"
                ActiveStepIndex="0" Width="100%" EnableTheming="True" Height="400px" StepStyle-VerticalAlign="Top">
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
                                                <asp:Button ID="_btnWizardStartNavCancel" runat="server" Text="Reset" CausesValidation="false"
                                                    OnClick="_btnWizardNavCancel_Click" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStartNextButton" runat="server" CommandName="MoveNext"
                                                    Text="Next" OnClick="_btnWizardStartNextButton_Click" />
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
                                                <asp:Button ID="_btnWizardStepPreviousButton" runat="server" CausesValidation="False"
                                                    CommandName="MovePrevious" Text="Previous" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepNavCancel" runat="server" Text="Reset" CausesValidation="false"
                                                    OnClick="_btnWizardNavCancel_Click" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepNextButton" runat="server" CommandName="MoveNext" Text="Next" />
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
                                                <asp:Button ID="_btnWizardStepPreviousButton" runat="server" CausesValidation="False"
                                                    CommandName="MovePrevious" Text="Previous" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardFinishNavCancel" runat="server" Text="Reset" OnClick="_btnWizardNavCancel_Click"
                                                    CausesValidation="false" />
                                            </div>
                                        </td>
                                        <td align="right">
                                            <div class="button" style="text-align: center;">
                                                <asp:Button ID="_btnWizardStepFinishButton" runat="server" CausesValidation="true"
                                                    CommandName="MoveComplete" Text="Finish" OnClick="_btnWizardStepFinishButton_Click" />
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
                    <asp:WizardStep ID="_wizardStepAddAssociation" runat="server" Title="Add New : Roles and Associations">
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlMatterDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                                        Matter Details</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <CliMat:ClientMatterDetails runat="server" EnableValidation="true" ID="_cliMatDetails"
                                        OnMatterChanged="_cliMatDetails_MatterChanged" DisplayMatterLinkable="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlRoleDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                                        Role</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Role
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="_ddlRole" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_ddlRole_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="_pnlAssociationDetailsHeader" runat="server" Width="99.9%" class="bodyTab">
                                        Association</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table border="0">
                                        <tr id="_trContactSearch">
                                            <td class="boldTxt" style="width: 100px;">
                                                Contact
                                            </td>
                                            <td>
                                                <ContactSearch:ContactSearch ID="_contactSearch" runat="server" DisplayPopup="true"
                                                    DisplayContactNameTextbox="false" OnContactSelected="_contactSearch_ContactSelected"
                                                    DisplayContactAsLink="true" OnError="Search_Error" />
                                            </td>
                                        </tr>
                                        <tr id="_trServiceSearch">
                                            <td class="boldTxt">
                                                Service Contact
                                            </td>
                                            <td>
                                                <ServiceSearch:ServiceSearch ID="_serviceSearch" runat="server" DisplayPopup="true"
                                                    DisplayServiceContactGridview="true" DisplayServiceContactTextbox="false" OnServiceSelected="_serviceSearch_ServiceSelected"
                                                    OnSearchButtonClick="_btnServiceSearch_SearchButtonClick" OnServiceContactSelected="_serviceSearch_ServiceContactSelected"
                                                    OnError="Search_Error" />
                                            </td>
                                        </tr>
                                        <tr id="_trClientSearch">
                                            <td class="boldTxt">
                                                <asp:UpdatePanel ID="_updClientSearch" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Label ID="_lblClientSearch" runat="server"></asp:Label>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="_ddlRole" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                            <td>
                                                <ClientSearch:ClientSearch ID="_clientSearch" runat="server" DisplayPopup="true"
                                                    DisplayClientNameTextbox="false" OnClientReferenceChanged="_clientSearch_ClientSelected"
                                                    OnMatterSelected="_clientSearch_MatterSelected" AsyncPostbackTriggers="_ddlRole"
                                                    SetSession="false" _displayMattersForClientGridview="false" />
                                            </td>
                                        </tr>
                                        <tr id="_trFeeEarnerSearch">
                                            <td class="boldTxt">
                                                Fee Earner
                                            </td>
                                            <td>
                                                <asp:UpdatePanel ID="_updFeeEarner" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        &nbsp;<asp:DropDownList ID="_ddlFeeEarner" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_ddlFeeEarner_SelectedIndexChanged">
                                                        </asp:DropDownList>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="_contactSearch" />
                                                        <asp:AsyncPostBackTrigger ControlID="_serviceSearch" />
                                                        <asp:AsyncPostBackTrigger ControlID="_ddlRole" />
                                                        <asp:AsyncPostBackTrigger ControlID="_clientSearch" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Name
                                            </td>
                                            <td>
                                                <asp:UpdatePanel ID="_updPnlContactName" UpdateMode="Conditional" runat="server">
                                                    <ContentTemplate>
                                                        &nbsp;<asp:TextBox ID="_txtName" runat="server" ReadOnly="true" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                                            onmouseout="hideToolTip();"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="_rfvName" runat="server" ControlToValidate="_txtName"
                                                            Display="None" ErrorMessage="Name is mandatory"></asp:RequiredFieldValidator>
                                                        <span class="mandatoryField">*</span>
                                                        <asp:HiddenField ID="_hdnOrganisationId" runat="server" />
                                                        <asp:HiddenField ID="_hdnMemberId" runat="server" />
                                                        <asp:HiddenField ID="_hdnLinkedProjectId" runat="server" />
                                                        <asp:HiddenField ID="_hdnIsSpecialisedSearch" runat="server" Value="false" />
                                                        <asp:HiddenField ID="_hdnSearchDisplay" runat="server" Value="" />
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="_contactSearch" />
                                                        <asp:AsyncPostBackTrigger ControlID="_serviceSearch" />
                                                        <asp:AsyncPostBackTrigger ControlID="_ddlRole" />
                                                        <asp:AsyncPostBackTrigger ControlID="_clientSearch" />
                                                        <asp:AsyncPostBackTrigger ControlID="_ddlFeeEarner" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAdditionalAssociationInfo1" runat="server" Title="Add New : Additional Association Info">
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlAdditionalAssociationInfo" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Add New : Additional Association Info</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" border="0">
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Description
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="_txtDescription" runat="server" SkinID="Large"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Date From
                                            </td>
                                            <td style="width: 101px;">
                                                <CC:CalendarControl ID="_ccDateFrom" runat="server" EnableValidation="false" />
                                            </td>
                                            <td class="boldTxt" style="width: 100px;">
                                                Date To
                                            </td>
                                            <td>
                                                <CC:CalendarControl ID="_ccDateTo" runat="server" EnableValidation="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Reference
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="_txtReference" runat="server" SkinID="Large"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Letter Heading
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="_txtLetterHeading" runat="server" SkinID="Large"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="boldTxt" style="width: 100px;">
                                                Comment
                                            </td>
                                            <td colspan="3">
                                                <asp:TextBox ID="_txtCommenting" runat="server" SkinID="Large"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                    <asp:WizardStep ID="_wizardStepAdditionalAssociationInfo2" runat="server" Title="Add New : Additional Association Info">
                        <table width="100%" border="0">
                            <tr>
                                <td>
                                    <asp:Panel ID="_pnlAdditionalAssociationInfo2" runat="server" Width="99.9%" CssClass="bodyTab">
                                        Add New : Additional Association Info</asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="_grdAdditionalAssociationInfo" runat="server" AllowPaging="true"
                                        AutoGenerateColumns="false" BorderWidth="0" GridLines="None" Width="100%" CssClass="successMessage"
                                        OnRowDataBound="_grdAdditionalAssociationInfo_RowDataBound" EmptyDataText="There are no results to display.">
                                        <Columns>
                                            <asp:BoundField DataField="TypeName" HeaderText="Field Description" HeaderStyle-HorizontalAlign="Left"
                                                ItemStyle-Width="40%" />
                                            <asp:TemplateField HeaderText="Details">
                                                <ItemTemplate>
                                                    <CC:CalendarControl ID="_cc" runat="server" EnableValidation="false" />
                                                    <asp:TextBox ID="_txt" runat="server"></asp:TextBox>
                                                    <asp:CheckBox ID="_chk" runat="server" />
                                                    <asp:DropDownList ID="_ddl" runat="server">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Notes">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="_txtNotes" SkinID="Large" runat="server"></asp:TextBox>
                                                    <asp:HiddenField ID="_hdnDataType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "DataType")%>' />
                                                    <asp:HiddenField ID="_hdnTypeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TypeId")%>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="_hdnRefreshRoleExtInfo" runat="server" Value="true" />
                                </td>
                            </tr>
                        </table>
                    </asp:WizardStep>
                </WizardSteps>
            </asp:Wizard>
            <table id="_tblConflictCheck" runat="server" width="100%" visible="false">
                <tr>
                    <td colspan="2">
                        <UC:ConflictCheck ID="_conflictCheck" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <table>
                            <tr>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnConflickOK" runat="server" CausesValidation="false" OnClick="_btnConflickOK_Click"
                                            Text="Add Contact" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnConflictCancel" OnClick="_btnWizardNavCancel_Click" runat="server"
                                            Text="Reset" CausesValidation="false" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">

        function ShowHideSearch(search) {
            if (search.ClientSearch == "true")
                $("#_trClientSearch").css("display", "");
            else
                $("#_trClientSearch").css("display", "none");

            if (search.ContactSearch == "true")
                $("#_trContactSearch").css("display", "");
            else
                $("#_trContactSearch").css("display", "none");

            if (search.ServiceSearch == "true")
                $("#_trServiceSearch").css("display", "");
            else
                $("#_trServiceSearch").css("display", "none");

            if (search.FeeEarnerSearch == "true")
                $("#_trFeeEarnerSearch").css("display", "");
            else
                $("#_trFeeEarnerSearch").css("display", "none");
        }
    </script>

</asp:Content>

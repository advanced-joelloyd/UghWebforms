<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="EditClient.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Client.EditClient"
    Title="Client Details" EnableEventValidation="false" %>

<%@ Register Src="~/UserControls/AddressDetails.ascx" TagName="AddressDetails" TagPrefix="address" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="ContactGV" TagName="ContactGridView" Src="~/UserControls/ContactGridView.ascx" %>
<asp:Content ID="_contentEditClient" ContentPlaceHolderID="_cphMain" runat="server">

    <script src="../../Javascript/EditClient.js" type="text/javascript"></script>

    <script type="text/javascript">
        var btnPassword = "<%= _btnPassword.ClientID %>";
        var txtPassword = "<%= _txtPassword.ClientID %>";
        var lblMessage = "<%= _lblMessage.ClientID %>";
        var txtDOB = "<%= _ccDOBDate.DateTextBoxClientID %>";
        var txtDOD = "<%= _ccDODDate.DateTextBoxClientID %>";
        var txtSurname = "<%= _txtSurname.ClientID %>";
        var txtForename = "<%= _txtForenames.ClientID %>";
        var chkArmedForces = "<%= _chkArmedForces.ClientID %>";
        var txtArmedForcesNo = "<%= _txtArmedForcesNo.ClientID %>";
        var txtHOUCN = "<%= _txtHOUCN.ClientID %>";
        var updPnlMessage = "<%= _updPnlMessage.ClientID %>";
        var txtUCN = "<%= _txtUCN.ClientID %>";
        var txtAge = "<%=_txtAge.ClientID%>";

        function CheckUserType() {
            var userType = document.getElementById("<%= _hdnUserType.ClientID%>").value;

            if (userType == "2") {
                document.getElementById("_imgSearchClient").style.display = "none";
            }
        }

        window.onload = CheckUserType;


        function messageHide(ctrl) {
            setTimeout('$("#'+ctrl+'").fadeOut("slow")', 10000);
        }
		
    </script>

    <div id="_printPreview">
    </div>
    <div id="_PageContents">
        <table width="100%">
            <tr>
                <td class="sectionHeader">
                    <%if (Request.QueryString["mydetails"] == "true")
                      { %>
                    My Details
                    <%}
                      else
                      {%>
                    Client Details
                    <%} %>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="_lblMessage" CssClass="labelValue" runat="server" Text=""></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <table width="100%">
            <tr>
                <td valign="bottom">
                    <asp:UpdatePanel ID="_updPnlClientName" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="_hdnUserType" runat="server" Value="1" />
                            <table width="100%">
                                <tr>
                                    <td class="boldTxt" style="text-indent: 0px;" width="100px;">
                                        Reference
                                    </td>
                                    <td>
                                        <asp:TextBox ID="_txtClientReference" runat="server" SkinID="Small" Text="" ReadOnly="true"></asp:TextBox>
                                        <a href="SearchClient.aspx">
                                            <img src="../../Images/PNGs/searchButton.png" alt="Client Search" class="searchButtonBg"
                                                id="_imgSearchClient" style="height: 21px; border: none;" /></a>
                                        <asp:Image runat="server" AlternateText="This client is archived" ID="_imgClientArchieved"
                                            ImageUrl="~/Images/PNGs/archived.png" Visible="false" />
                                    </td>
                                    <td rowspan="3" align="right" valign="top">
                                        <asp:ValidationSummary runat="server" HeaderText="You have some errors, please correct them before you can save."
                                            Font-Size="X-Small" Font-Names="arial" ID="_vsEditMatter" ShowSummary="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="boldTxt" style="text-indent: 0px;">
                                        Name
                                    </td>
                                    <td>
                                        <asp:Label ID="_lblClientName" runat="server" CssClass="labelValue"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="right" valign="bottom">
                    <%--<table width="100%">
					<tr>
						<td align="right">
							<a href="#" class="link">View Wills</a> <a href="#" class="link">View Deeds</a>
						</td>
					</tr>
				</table>--%>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <ajaxToolkit:TabContainer ID="_tcEditClient" runat="server" CssClass="ajax__tab_xp2"
                        Width="100%" ActiveTabIndex="0">
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlAddressDetails" HeaderText="Address">
                            <HeaderTemplate>
                                Address
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table width="100%" border="0">
                                    <tr>
                                        <td class="sectionHeader">
                                            Addresses
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Type
                                            <asp:DropDownList ID="_ddlAddressType" runat="server" AutoPostBack="True" Style="margin-left: 15px;"
                                                OnSelectedIndexChanged="_ddlAddressType_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <address:AddressDetails ID="_ucAddress" runat="server" AsyncPostbackTriggers="_ddlAddressType"
                                                IsDataChangedCheckRequired="true" IsLastVerifiedEnabled="false" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlContactDetails" HeaderText="Contact Details">
                            <HeaderTemplate>
                                Contact Details
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="_updContactDetails" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <ContactGV:ContactGridView ID="_contactGridView" runat="server" OnErrorOccured="_contactGridView_ErrorOccured" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlGeneralDetails" HeaderText="General Details">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td class="sectionHeader" colspan="4">
                                            General Information
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
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
                                        <td class="boldTxt" style="width: 150px;">
                                            Web Case Tracking
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="_chkWebCaseTracking" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
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
                                        <td class="boldTxt">
                                            Password
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtPassword" runat="server" Style="float: left; margin-right: 5px;"></asp:TextBox>
                                            <div id="Regenerate">
                                                <div class="button" style="float: left; width: 90px;">
                                                    <input id="_btnPassword" runat="server" type="button" value="Re-Generate" />
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Open Date
                                        </td>
                                        <td>
                                            <CC:CalendarControl ID="_ccOpenDate" InvalidValueMessage="Invalid Open Date" runat="server" />
                                        </td>
                                        <td class="boldTxt">
                                            Group
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtGroup" runat="server" MaxLength="5"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Previous Reference
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtPreviousReference" runat="server"></asp:TextBox>
                                        </td>
                                        <td class="boldTxt">
                                            <asp:Label ID="_lblHOUCN" runat="server" Text="HO UCN"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtHOUCN" runat="server" onKeypress="uppercase(event)"></asp:TextBox>
                                            <ajaxToolkit:MaskedEditExtender ID="_meeUCN" TargetControlID="_txtHOUCN" Mask="L9999999"
                                                MaskType="None" ClearMaskOnLostFocus="true" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Business Source
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlBusinessSource" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="boldTxt">
                                            <asp:Label ID="_lblUCN" runat="server" Text="UCN"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtUCN" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Source Campaign
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlSourceCampaign" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="boldTxt">
                                            Receives Marketing
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="_chkReceivesMarketing" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt">
                                            Rating
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlRating" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="boldTxt">
                                            Archive Client
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="_chkArchiveClient" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%">
                                    <tr>
                                        <td class="sectionHeader" colspan="2">
                                            Credit Control Information
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 150px;">
                                            Cash Collection
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlCashCollection" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" style="width: 150px;">
                                            Credit Limit
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtCreditLimit" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtCreditLimit" runat="server" TargetControlID="_txtCreditLimit"
                                                FilterType="Custom, Numbers" ValidChars="." />
                                            <asp:RegularExpressionValidator ID="revtxtCreditLimit" ControlToValidate="_txtCreditLimit"
                                                runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlIndividualDetails" HeaderText="Individual Details">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="_UpdPnlIndivDetails" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Personal Information
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt" style="width: 150px;">
                                                    Title
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlTitle" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="boldTxt" style="width: 150px;">
                                                    Sex
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlSex" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Forenames
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtForenames" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    D.O.B
                                                </td>
                                                <td>
                                                    <table cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <CC:CalendarControl ID="_ccDOBDate" InvalidValueMessage="Invalid Date of Birth" runat="server" />
                                                            </td>
                                                            <td style="width:5px"></td>
                                                            <td>
                                                                <span class="boldTxt">Age</span>
                                                            </td>
                                                            <td style="width:5px"></td>
                                                            <td>
                                                                <asp:TextBox ID="_txtAge" runat="server" SkinID="Small" ReadOnly="true"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Surname
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtSurname" runat="server" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="_rfvSurname" runat="server" ControlToValidate="_txtSurname"
                                                        Display="None" ErrorMessage="Surname is mandatory"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt">
                                                    D.O.D
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccDODDate" InvalidValueMessage="Invalid D.O.D." runat="server"
                                                         />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Marital or CP Status
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlMaritalStatus" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="boldTxt">
                                                    Place of Birth
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtPlaceOfBirth" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Previous Name
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtPreviousName" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    Birth Name
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtBirthName" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Occupation
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtOccupation" runat="server"></asp:TextBox>
                                                </td>
                                                <td colspan="2">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Additional Information
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Sal'n Letter(Formal)
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtFormalSalutation" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    Ethnicity
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlEthnicity" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Sal'n Letter(Informal)
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtInformalSalutation" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    Disability
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlDisability" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Sal'n Letter(Friendly)
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtFriendlySalutation" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    Armed Forces
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="_chkArmedForces" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Salutation Envelope
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtEnvelopeSalutation" runat="server"></asp:TextBox>
                                                </td>
                                                <td class="boldTxt">
                                                    Armed Forces No.
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtArmedForcesNo" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    &nbsp;
                                                </td>
                                                <td class="boldTxt">
                                                    NI No.
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtNINo" runat="server" onKeypress="uppercase(event)"></asp:TextBox>
                                                    <ajaxToolkit:MaskedEditExtender ID="_meeNINo" TargetControlID="_txtNINo" Mask="LL 99 99 99 L"
                                                        OnFocusCssClass="MaskedEditFocus" MaskType="None" ClearMaskOnLostFocus="false"
                                                        runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlOrganisationDetails" HeaderText="Org. Details">
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td class="sectionHeader" colspan="4">
                                            Organisation Details
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top" style="width: 150px;">
                                            Name
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtOrganisationName" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                                onmouseout="hideToolTip();"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="_rfvOrganisationName" runat="server" ControlToValidate="_txtOrganisationName"
                                                Display="None" ErrorMessage="Name is mandatory"></asp:RequiredFieldValidator>
                                            <span class="mandatoryField">*</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top">
                                            Registered Name
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtRegisteredName" runat="server" SkinID="Large"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top">
                                            Registered No
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtRegisteredNo" runat="server" SkinID="Large"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top">
                                            VAT No
                                        </td>
                                        <td>
                                            <asp:TextBox ID="_txtVATNo" runat="server" SkinID="Large"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top">
                                            Industry
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlIndustry" runat="server" SkinID="Large">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="boldTxt" valign="top">
                                            Sub Type
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="_ddlSubType" runat="server" SkinID="Large">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlMatters" HeaderText="Matters">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="_updClientMatters" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="_grdClientMatters" runat="server" AutoGenerateColumns="False" GridLines="None"
                                            Width="100%" AllowPaging="true" OnRowDataBound="_grdClientMatters_RowDataBound"
                                            EmptyDataText="No matters for client" DataKeyNames="Id" OnRowCommand="_grdClientMatters_RowCommand"
                                            CssClass="successMessage">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Matter Reference">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="_lnkMatterReference" CssClass="link" CommandName="select" runat="server"
                                                            Text='<%#DataBinder.Eval(Container.DataItem, "Reference")%>'>
                                                        </asp:LinkButton>
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
                                        <asp:ObjectDataSource ID="_odsClientMatters" runat="server" SelectMethod="GetClientMatters"
                                            TypeName="IRIS.Law.WebApp.Pages.Client.EditClient" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                            SelectCountMethod="GetClientMattersRowsCount" StartRowIndexParameterName="startRow"
                                            OnSelected="_odsClientMatters_Selected">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="_hdnRefresh" Name="forceRefresh" PropertyName="Value"
                                                    Type="Boolean" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                        <asp:HiddenField ID="_hdnRefresh" runat="server" Value="true" />
                                        <br />
                                        <table cellpadding="0" width="100%">
                                            <tr height=27>
                                                <td align="right">
                                                    <div class="button" style="text-align: center; height: 17px; " id="_divAddMatterButton" runat="server">
                                                        <%--<asp:Button ID="_btnAddMatter" runat="server" Text="Add Matter" OnClientClick="pageRedirect('/Matter/AddMatter.aspx');return false;"
                                                            CausesValidation="false"
                                                            OnClientClick="pageRedirect('/Matter/AddMatter.aspx');return false;" 
                                                             />--%>
                                                        <asp:LinkButton ID="_btnAddMatter" runat="server" Text="Add Matter" CausesValidation="false" PostBackUrl="~/Pages/Matter/AddMatter.aspx" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right" style="padding-right: 15px;">
                    <asp:UpdatePanel ID="_updPnlEditClient" runat="server">
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td align="right">
                                        <div class="button" style="text-align: center; width: 100px" id="_divPrintBtn" visible="true"
                                            runat="server">
                                            <center>
                                                <asp:Button ID="_btnPrint" CausesValidation="false" runat="server" Text="Print" />
                                            </center>
                                        </div>
                                    </td>
                                    <td align="right">
                                        <div id="_divBack" runat="server" visible="false" class="button" style="float: right;
                                            text-align: center;">
                                            <asp:Button ID="_btnBack" runat="server" Text="Back" OnClientClick="javascript:window.location='SearchClient.aspx';return false;"
                                                OnClick="_btnBack_Click" />
                                        </div>
                                    </td>
                                    <td align="right">
                                        <div class="button" style="float: right; text-align: center;">
                                            <asp:Button ID="_btnSave" runat="server" Text="Save" OnClick="_btnSave_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

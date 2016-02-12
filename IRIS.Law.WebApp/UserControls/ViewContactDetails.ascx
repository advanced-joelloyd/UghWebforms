<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewContactDetails.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.ViewContactDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register Src="~/UserControls/AddressDetails.ascx" TagName="AddressDetails" TagPrefix="address" %>
<%@ Register Src="~/UserControls/AdditionalAddressDetails.ascx" TagName="AdditionalAddressDetails" TagPrefix="AAD" %>
<script type="text/javascript">
    function CancelViewContactDetailsPopupClick() {
        return false;
    }

    function CancelViewContactDetailsPopup() {
        $find('_mpeViewContactDetailsBehavior').hide();
    }
        var browser = navigator.appName;
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }
</script>

<input id="_btnDummy" runat="server" type="button" value="." style="height: 1px;
    width: 1px; display: none;" disabled="disabled" />
<ajaxToolkit:ModalPopupExtender ID="_mpeViewContactDetails" runat="server" BackgroundCssClass="modalBackground"
    DropShadow="True" PopupControlID="_pnlContactDetails" OnCancelScript="javascript:CancelViewContactDetailsPopupClick();"
    TargetControlID="_btnDummy" BehaviorID="_mpeViewContactDetailsBehavior">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel ID="_pnlContactDetails" runat="server" Style="background-color: #ffffff;">
     <table id ="tblClose" width =98%><tr><td align =right><a onclick="CancelViewContactDetailsPopup();" class="link"
                        href="#">Close</a>&nbsp;&nbsp;</td></tr></table>
      <ajaxToolkit:TabContainer ID="_tcContactDetail" runat="server" CssClass="ajax__tab_xp2" Width="98%" style="padding:5px">
      
            <ajaxToolkit:TabPanel runat="server" ID="_pnlAddressDetails" HeaderText="Address">
                    <HeaderTemplate>
                        Address
                    </HeaderTemplate>
                    <ContentTemplate>
	                        <table width="100%" border="0" >
	                          <tr>
			                        <td>
				                        <asp:UpdatePanel ID="_updPnlMessage" runat="server">
					                        <ContentTemplate>
						                        <asp:Label ID="_lblMessage" CssClass="labelValue" runat="server" Text=""></asp:Label>
					                        </ContentTemplate>
				                        </asp:UpdatePanel>
			                        </td>
		                        </tr>
		                          <tr>
	 	                            <td class="sectionHeader" >
			                            Addresses
		                            </td>
			                         <td colspan="4" align="right">
				                       
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
								                        IsDataChangedCheckRequired="true"  />
						                        </td>
					                        </tr>
				                        </table>
			             </ContentTemplate>
            </ajaxToolkit:TabPanel>
 
            <ajaxToolkit:TabPanel ID="_pnlIndividual" runat="server" HeaderText="ContactDetails">
            <HeaderTemplate>
                Individual Details
            </HeaderTemplate>
            <ContentTemplate>
            <table width="100%"  border="0">
                <tr>
                <td colspan="4" align="right">
                   &nbsp;
                </td>
                </tr>
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
                <asp:DropDownList id="_ddlTitle" runat="server" Enabled="false">
                </asp:DropDownList>
                </td>
                 <tr>
                <td class="boldTxt" style="width: 150px;">
                    Sex
                </td>
                 <td>
                    <asp:DropDownList ID="_ddlSex" runat="server" Enabled="false">
                    </asp:DropDownList>
                </td>
                </tr>
                  <tr>
                        <td class="boldTxt">
                            Forenames
                        </td>
                        <td>
                            <asp:TextBox ID="_txtForenames" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            D.O.B
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccDOBDate" InvalidValueMessage="Invalid Date of Birth" runat="server"
                                Enabled="false" />
                            &nbsp;&nbsp;&nbsp;<span class="boldTxt">Age</span>
                            <asp:TextBox ID="_txtAge" runat="server" SkinID="Small" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="boldTxt">
                            Surname
                        </td>
                        <td>
                            <asp:TextBox ID="_txtSurname" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            D.O.D
                        </td>
                        <td>
                            <CC:CalendarControl ID="_ccDODDate" Enabled="false" InvalidValueMessage="Invalid D.O.D."
                                runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Marital or CP Status
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlMaritalStatus" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td class="boldTxt">
                            Place of Birth
                        </td>
                        <td>
                            <asp:TextBox ID="_txtPlaceOfBirth" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Previous Name
                        </td>
                        <td>
                            <asp:TextBox ID="_txtPreviousName" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Birth Name
                        </td>
                        <td>
                            <asp:TextBox ID="_txtBirthName" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Occupation
                        </td>
                        <td>
                            <asp:TextBox ID="_txtOccupation" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Source Campaign
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlSourceCampaign" runat="server" Enabled="false">
                            </asp:DropDownList>
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
                            <asp:TextBox ID="_txtFormalSalutation" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Ethnicity
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlEthnicity" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Sal'n Letter(Informal)
                        </td>
                        <td>
                            <asp:TextBox ID="_txtInformalSalutation" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Disability
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlDisability" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Sal'n Letter(Friendly)
                        </td>
                        <td>
                            <asp:TextBox ID="_txtFriendlySalutation" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Armed Forces
                        </td>
                        <td>
                            <asp:CheckBox ID="_chkArmedForces" runat="server" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Salutation Envelope
                        </td>
                        <td>
                            <asp:TextBox ID="_txtEnvelopeSalutation" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                        <td class="boldTxt">
                            Armed Forces No.
                        </td>
                        <td>
                            <asp:TextBox ID="_txtArmedForcesNo" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Receives Marketing
                        </td>
                        <td>
                            <asp:CheckBox ID="_chkReceivesMarketing" runat="server" Enabled="false" />
                        </td>
                        <td class="boldTxt">
                            NI No.
                        </td>
                        <td>
                            <asp:TextBox ID="_txtNINo" runat="server" Enabled="false"></asp:TextBox>
                        </td>
                </tr>

            </table>
            </ContentTemplate>
             </ajaxToolkit:TabPanel>
             
            <ajaxToolkit:TabPanel ID="_pnlOrganisation" runat="server" HeaderText="ContactDetails1">
            <HeaderTemplate>
                Organisation Details
            </HeaderTemplate>
            <ContentTemplate>
               <table width="100%" border="0" >
                <tr>
                  <td colspan="4" align="right">
                &nbsp;
                  
                  </td>
                </tr>
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
                            <asp:TextBox ID="_txtOrganisationName" runat="server" SkinID="Large" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                      <tr>
                        <td class="boldTxt" valign="top">
                            Registered Name
                        </td>
                        <td>
                            <asp:TextBox ID="_txtRegisteredName" runat="server" SkinID="Large" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="boldTxt" valign="top">
                            Registered No
                        </td>
                        <td>
                            <asp:TextBox ID="_txtRegisteredNo" runat="server" SkinID="Large" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="boldTxt" valign="top">
                            VAT No
                        </td>
                        <td>
                            <asp:TextBox ID="_txtVATNo" runat="server" SkinID="Large" Enabled="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt" valign="top">
                            Industry
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlIndustry" runat="server" SkinID="Large" Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                      <tr>
                        <td class="boldTxt" valign="top">
                            Sub Type
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlSubType" runat="server" SkinID="Large" Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Source Campaign
                        </td>
                        <td>
                            <asp:DropDownList ID="_ddlSourceCampaignOrg" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="boldTxt">
                            Receives Marketing
                        </td>
                        <td>
                            <asp:CheckBox ID="_chkReceivesMarketingOrg" runat="server" Enabled="false"/>
                        </td>
                    </tr>
            </table>
            </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
            <ajaxToolkit:TabPanel runat="server" ID="_pnlAdditionalContact" HeaderText ="Additional Contact Details">
                <HeaderTemplate>
                    Contact Details
                </HeaderTemplate>
                <ContentTemplate >
                    <table>
                        <tr>
                            <td>
                                <AAD:AdditionalAddressDetails ID="_aadContactInfo" runat="server"></AAD:AdditionalAddressDetails>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            
       
    </ajaxToolkit:TabContainer>
</asp:Panel> 
<script type="text/javascript">
    document.getElementById("tblClose").style.display = "none";
 </script>


    
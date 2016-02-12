<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressDetails.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.AddressDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="Map" TagName="MapControl" Src="~/UserControls/DirectionsandMapTool.ascx" %>

<script src="../../Javascript/Address.js" type="text/javascript"></script>

<ajaxToolkit:TabContainer CssClass="ajax__tab_xp2" runat="server" ID="_tcClientSearch"
    ActiveTabIndex="0" Width="100%">
    <ajaxToolkit:TabPanel runat="server" ID="_pnlAddress" HeaderText="Address Details">
        <HeaderTemplate>
            Address Details
        </HeaderTemplate>
        <ContentTemplate>
            <asp:UpdatePanel ID="_updPnlAddressDetails" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="_hfAddressId" runat="server" />
                    <asp:HiddenField ID="_hfAddressTypeId" runat="server" />
                    <asp:HiddenField ID="_hfIsDataChanged" runat="server" Value="false" />
                    <table width="100%">
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                House Number
                            </td>
                            <td>
                                <asp:TextBox ID="_txtHouseNumber" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Postcode
                            </td>
                            <td> 
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="_txtPostcode" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                                        </td>
                                        <td>
                                            <div id="MapControl">
                                            <asp:ImageButton ID="_btnAddressLookup" runat="server" 
                                                AlternateText="Address Lookup" CausesValidation="false" Height="20px" 
                                                ImageUrl="~/Images/PNGs/addressLookup.png" OnClick="_btnAddressLookup_Click" 
                                                ToolTip="Address Lookup" Visible="false" Width="20px" />
                                            <asp:Button ID="hiddenTargetControlForModalPopup" runat="server" 
                                                Style="display: none" />
                                            <ajaxToolkit:ModalPopupExtender ID="_mpePostcodeLookup" runat="server" 
                                                BackgroundCssClass="modalBackground" CancelControlID="CancelButton" 
                                                DropShadow="true" OkControlID="OkButton" 
                                                OnCancelScript="javascript:CancelClickOnOnlineAddressVerification();" 
                                                PopupControlID="_pnlAddressOnPostCode" 
                                                TargetControlID="hiddenTargetControlForModalPopup">
                                            </ajaxToolkit:ModalPopupExtender>
                                            <asp:Panel ID="_pnlAddressOnPostCode" runat="server" 
                                                Style="display: none; background-color: #ffffff" Width="550px">
                                                <div>
                                                    <p style="text-align: center">
                                                        <table border="0" width="100%">
                                                            <tr>
                                                                <td align="left" class="sectionHeader">
                                                                    Online Address Verification
                                                                </td>
                                                            </tr>
                                                            <tr style="height: 5px">
                                                            </tr>
                                                            <tr ID="_trServiceError" runat="server" style="display: none;">
                                                                <td align="left">
                                                                    <asp:Label ID="_lblServiceError" runat="server" CssClass="errorMessage"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr ID="_trServiceErrorSpace" runat="server" 
                                                                style="display: none; height: 5px;">
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:ListBox ID="_listAddress" runat="server" Height="100px" Width="100%">
                                                                    </asp:ListBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td align="left" class="boldTxt" style="width:70%">
                                                                                Please choose address and press OK to select.<br />
                                                                                &nbsp;&nbsp;&nbsp;&nbsp;Double click on items to narrow down results.
                                                                            </td>
                                                                            <td align="right">
                                                                                <div class="button" style="text-align: center;">
                                                                                     <input ID="OkButton" runat="server" class="button" type="button" value="OK" />
                                                                                </div>
                                                                            </td>
                                                                            <td align="right">
                                                                                <div class="button" style="text-align: center;">
                                                                                    <input ID="CancelButton" runat="server" class="button" type="button" 
                                                                                        value="Cancel" />
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </p>
                                                </div>
                                            </asp:Panel>
                                            </div>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <div id="MapControl2">
                                                <Map:MapControl ID="_ucMap" runat="server"/>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                House Name
                            </td>
                            <td>
                                <asp:TextBox ID="_txtHouseName" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Address
                            </td>
                            <td>
                                <asp:TextBox ID="_txtAddress1" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="_txtAddress2" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="_txtAddress3" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Town
                            </td>
                            <td>
                                <asp:TextBox ID="_txtTown" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                County
                            </td>
                            <td>
                                <asp:TextBox ID="_txtCounty" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Country
                            </td>
                            <td>
                                <asp:TextBox ID="_txtCountry" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Large"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                DX Address
                            </td>
                            <td>
                                <asp:TextBox ID="_txtDXAddress1" runat="server" onkeypress="SetDataChanged()" 
                                    SkinID="Small"></asp:TextBox>
                                <asp:TextBox ID="_txtDXAddress2" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Mailing Address
                            </td>
                            <td>
                                <asp:CheckBox ID="_chkMailAddress" runat="server" onclick="SetDataChanged()" />
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Billing Address
                            </td>
                            <td>
                                <asp:CheckBox ID="_chkBillAddress" runat="server" onclick="SetDataChanged()" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="_pnlByUniqueReference" HeaderText="Additional Info">
        <HeaderTemplate>
            Additional Info
        </HeaderTemplate>
        <ContentTemplate>
            <asp:UpdatePanel ID="_updPnlAdditionalInfo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table width="100%">
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Organisation
                            </td>
                            <td>
                                <asp:TextBox ID="_txtOrganisation" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Department
                            </td>
                            <td>
                                <asp:TextBox ID="_txtDepartment" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                P O Box
                            </td>
                            <td>
                                <asp:TextBox ID="_txtPOBox" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Sub Building Name
                            </td>
                            <td>
                                <asp:TextBox ID="_txtSubBuildingName" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Dependant Locality
                            </td>
                            <td>
                                <asp:TextBox ID="_txtDeptLoc" runat="server" onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="boldTxt" style="width: 150px;">
                                Comment
                            </td>
                            <td>
                                <asp:TextBox ID="_txtComment" TextMode="MultiLine" Height="100px" Width="200px" runat="server"
                                    onkeypress="SetDataChanged()"></asp:TextBox>
                            </td>
                        </tr>
                        <tr runat="server" id="_trLastVerified" visible="false">
                            <td class="boldTxt" style="width: 150px;">
                                Last Verified
                            </td>
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <CC:CalendarControl ID="_ccLastVerifiedDate" InvalidValueMessage="Invalid Last Verified Date"
                                                runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>

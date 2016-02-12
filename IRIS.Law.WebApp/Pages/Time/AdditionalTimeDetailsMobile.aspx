<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdditionalTimeDetailsMobile.aspx.cs"
    Inherits="IRIS.Law.WebApp.Pages.Time.AdditionalTimeDetailsMobile" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="UserControl" TagName="MatterSearch" Src="~/UserControls/MatterSearchMobile.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Additional Time Details</title> 
	<link rel="Stylesheet" href="../CSS/Master.css" />
	<style type="text/css">
	
        .sectionHeader
        {
	        font-family: Arial, Helvetica, sans-serif;
	        font-size: 13px;
	        font-weight: bold;
	        color: #1e7b84;
	        border-bottom: solid 1px #1e7b84;
        }
        
        .searchButtonBg
        {
	        background-color: #1e7b84;
	        margin-top: 1px;
	        margin-left: 5px;
	        height:21px;
		    width:22px;
		    padding-bottom:0px;
        }
        
	</style>
	
</head>
<body>
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="_smAdditionalTimeDetails" runat="server" CombineScripts="false">
    </ajaxToolkit:ToolkitScriptManager>
    <table width="320px" border="0">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="_ddlAdvocacyLocation" />
                        <asp:AsyncPostBackTrigger ControlID="_ddlAttendanceLocation" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr> 
            <td class="sectionHeader">
                Additional Time Details
            </td>
        </tr>
    </table>
    <asp:Panel ID="_pnlAdvocacy" runat="server" Visible="false" Width="320px" Height="100%">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px">
                    Advocacy Location :
                </td>
                <td>
                    <asp:DropDownList ID="_ddlAdvocacyLocation" runat="server" OnSelectedIndexChanged="_ddlAdvocacyLocation_SelectedIndexChanged"
                        AutoPostBack="true" Width="195px">
                    </asp:DropDownList>
                </td>
                <td style="width:1px"></td>
                <td align="left" valign="top"><div class="searchButtonBg"><asp:ImageButton AlternateText="Advocacy Location" ID="_imgbtnAdvocacyLocation" runat="server"
                 ImageUrl="../Images/searchButton.png" ToolTip="Advocacy Location" CausesValidation="false" EnableTheming="false" OnClick="_imgBtnAdvocacyLocation_Click"/></div></td>
            </tr>
            <tr>
                <td class="boldTxtMobile" style="width: 100px">
                    Service :
                </td>
                <td>
                    <asp:UpdatePanel ID="_updAdvocacyLocation" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList Width="195px" Enabled="false" ID="_ddlServiceAdvocacyLocation"
                                runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlAdvocacyLocation" />
                            <asp:AsyncPostBackTrigger ControlID="_imgbtnAdvocacyLocation" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile" valign="top" style="padding-top: 5px; width: 100px">
                    Hearing Type :
                </td>
                <td>
                    
                        <asp:CheckBoxList ID="_chklstHearingType" runat="server">
                        </asp:CheckBoxList>
                    
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlAttendance" runat="server" Visible="false" Width="320px" Height="100%">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Attendance Location :
                </td>
                <td>
                    <asp:DropDownList ID="_ddlAttendanceLocation" AutoPostBack="true" runat="server"
                        OnSelectedIndexChanged="_ddlAttendanceLocation_SelectedIndexChanged" Width="195px">
                    </asp:DropDownList>
                </td>
                <td style="width:1px"></td>
                <td align="left" valign="top"><div class="searchButtonBg"><asp:ImageButton AlternateText="Attendance Location" ID="_imgBtnAttendanceLocation" runat="server"
                 ImageUrl="../Images/searchButton.png" ToolTip="Attendance Location" CausesValidation="false" EnableTheming="false" OnClick="_imgBtnAttendanceLocation_Click"/></div></td>
            </tr>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Service :
                </td>
                <td>
                    <asp:UpdatePanel ID="_updAttendLocation" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList Enabled="false" ID="_ddlServiceAttendanceLocation" runat="server"
                                Width="195px">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlAttendanceLocation" />
                            <asp:AsyncPostBackTrigger ControlID="_imgBtnAttendanceLocation" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile" valign="top" style="padding-top: 5px;">
                    Attendance Individuals :
                </td>
                <td>
                    
                        <asp:CheckBoxList ID="_chklstAttendanceIndividuals" runat="server">
                        </asp:CheckBoxList>
                    
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlPoliceStationCalls" runat="server" Visible="false" Height="215px"
        Width="320px">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Police Station Contacted :
                </td>
                <td>
                    <asp:DropDownList ID="_ddlPoliceStationCalls" runat="server" OnSelectedIndexChanged="_ddlPoliceStationCalls_SelectedIndexChanged"
                        AutoPostBack="true" Width="195px">
                    </asp:DropDownList>
                </td>
                <td style="width:2px"></td>
                <td align="left" valign="top"><div class="searchButtonBg"><asp:ImageButton AlternateText="Police Station Contacted" ID="_imgBtnPoliceStationContacted" runat="server"
                 ImageUrl="../Images/searchButton.png" ToolTip="Police Station Contacted" CausesValidation="false" EnableTheming="false" OnClick="_imgBtnPoliceStationContacted_Click"/></div></td>
            </tr>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Service :
                </td>
                <td>
                    <asp:UpdatePanel ID="_updPoliceStationCalls" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList Width="195px" Enabled="false" ID="_ddlServicePoliceStationCalls"
                                runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlPoliceStationCalls" />
                            <asp:AsyncPostBackTrigger ControlID="_imgBtnPoliceStationContacted" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlTravel" runat="server" Visible="false" Width="320px" Height="215px">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Miles :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="_txtMiles" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile" valign="top" style="padding-top: 5px; width: 100px;">
                    Fares<br />Description :
                </td>
                <td>
                    <asp:TextBox ID="_txtFaresDescription" runat="server" SkinID="Small" TextMode="MultiLine"
                        Height="155px" Width="189px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile" valign="top" style="width: 100px;">
                    Fares :
                </td>
                <td>
                    <asp:TextBox ID="_txtFares" Text="0.00" runat="server" SkinID="Small" MaxLength="11"></asp:TextBox>
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtFares" runat="server"
                          TargetControlID="_txtFares"         
                          FilterType="Custom, Numbers"
                          ValidChars="." />
                    <asp:RegularExpressionValidator ID="revtxtFares"  ControlToValidate="_txtFares" runat="server" 
                         ErrorMessage="The format of the number is incorrect" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlRunningTime" runat="server" Visible="false" Height="215px" Width="320px">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Running Time of Tape :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="_txtHour" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)"></asp:TextBox>
                    :
                    <asp:TextBox runat="server" ID="_txtMinutes" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)" onchange="CheckMinutes(this)"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumJRFormFilling" runat="server" Visible="false" Width="320px"
        Height="215px">
        <span class="boldTxtMobile" style="padding-left: 15px;">Is this JR/Form filling?</span>
        <asp:CheckBox ID="_chkCivilImmAsylumJRFormFilling" runat="server" />
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumMentalHearingAdjourned" runat="server" Visible="false"
        Width="320px" Height="215px">
        <span class="boldTxtMobile" style="padding-left: 15px;">Was the hearing adjourned?</span>
        <asp:CheckBox ID="_chkCivilImmAsylumMentalHearingAdjourned" runat="server" />
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumSubstantiveHearing" runat="server" Visible="false"
        Width="320px" Height="215px">
        <span class="boldTxtMobile" style="padding-left: 15px;">Was a substantive hearing attended?</span>
        <asp:CheckBox ID="_chkCivilImmAsylumSubstantiveHearing" runat="server" />
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumTravelWaitingDetCentre" runat="server" Visible="false"
        Height="215px" Width="320px">
        <asp:Label ID="_lblCivilImmAsylumTravelWaitingDetCentre" CssClass="boldTxtMobile"
            runat="server" Text="Is this travel/Waiting to/at a detention centre?" Style="padding-left: 10px;">
        </asp:Label>
        <asp:CheckBox ID="_chkCivilImmAsylumTravelWaitingDetCentre" runat="server" onclick="EnableDisableDetention(this);" />
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Service
                </td>
                <td>
                    <asp:DropDownList Width="195px" ID="_ddlServiceCivilImmAsylumTravelWaitingDetCentre"
                        runat="server" Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCourtDutyAttendance" runat="server" Visible="false" Height="215px"
        Width="320px">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Court :
                </td>
                <td>
                    <asp:DropDownList Width="195px" ID="_ddlCourtDutyAttCourt" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile">
                    Number of Dependents/ Suspects dealt with :
                </td>
                <td>
                    
                    <asp:TextBox ID="_txtCourtDutyAttNoOfDef" Text="0.00" runat="server" SkinID="Small" MaxLength="11"></asp:TextBox>
                                                
                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtCourtDutyAttNoOfDef" runat="server"
                          TargetControlID="_txtCourtDutyAttNoOfDef"         
                          FilterType="Custom, Numbers"
                          ValidChars="." />
                                                    
                     <asp:RegularExpressionValidator ID="revtxtCourtDutyAttNoOfDef"  ControlToValidate="_txtCourtDutyAttNoOfDef" runat="server" 
                         ErrorMessage="The format of the number is incorrect" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                
                </td>
            </tr>
        </table>
        <span class="boldTxtMobile" style="padding-left: 10px;">Were any of the Defendants/Suspects
            a youth?</span>
        <asp:CheckBox ID="_chkCourtDutyAtYouth" runat="server" />
    </asp:Panel>
    <asp:Panel ID="_pnlPoliceStationAttendance" runat="server" Visible="false" Height="215px"
        Width="320px">
        <table>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Police Station Attended :
                </td>
                <td>
                    <asp:DropDownList ID="_ddlPoliceStationAttendance" runat="server" OnSelectedIndexChanged="_ddlPoliceStationAttendance_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                 <td style="width:2px"></td>
                <td align="left" valign="top"><div class="searchButtonBg"><asp:ImageButton AlternateText="Police Station Attended" ID="_imgBtnPoliceStationAttended" runat="server"
                 ImageUrl="../Images/searchButton.png" ToolTip="Police Station Attended" CausesValidation="false" EnableTheming="false" OnClick="_imgBtnPoliceStationAttended_Click"/></div></td>
            </tr>
            <tr>
                <td class="boldTxtMobile" style="width: 100px;">
                    Service :
                </td>
                <td>
                    <asp:DropDownList Width="195px" Enabled="false" ID="_ddlServicePoliceStationAttendance"
                        runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlFileReviews" runat="server" Visible="false" Height="215px" Width="320px">
        <table>
            <tr>
                <td class="boldTxtMobile">
                    <UserControl:MatterSearch ID="_msFileReviews" runat="server" OnMatterChanged="_msFileReviews_MatterChanged"
                        OnError="_msFileReviews_Error" OnSearchSuccessful="_msFileReviews_SearchSuccessful">
                    </UserControl:MatterSearch>
                </td>
            </tr>
            <tr>
                <td class="boldTxtMobile">
                    <asp:RadioButton ID="_rdoBtnPaper" runat="server" GroupName="FileReviewType" Text="Paper" />
                    <asp:RadioButton ID="_rdoBtnFaceToFace" runat="server" GroupName="FileReviewType"
                        Text="Face to Face" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <table width="100%">
        <tr>
            <td>
                <asp:Button ID="_btnLogout" runat="server" Text="Logout" CssClass="buttonMobile" CausesValidation="true"
                    OnClick="_btnLogout_Click" EnableTheming="false" />
                <asp:Button ID="_btnBack" runat="server" Text="Back" CssClass="buttonMobile" CausesValidation="false"
                    OnClick="_btnBack_Click" EnableTheming="false" />
                <asp:Button ID="_btnCancel" runat="server" Text="Reset" CssClass="buttonMobile" CausesValidation="false"
                    OnClick="_btnCancel_Click" EnableTheming="false" />
                <asp:Button ID="_btnSave" runat="server" Text="Save" CssClass="buttonMobile" CausesValidation="true"
                    OnClick="_btnSave_Click" EnableTheming="false" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="border-top: solid 1px #1e7b84;">
                <img src="../Images/logoSmall.png" alt="Logo" height="25px" width="50px" align="right" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        //Check if the user has entered 0
        function CheckUnits(sender) {
            if (parseInt(sender.value) == 0) {
                sender.value = "";
            }
        }

        //If the minutes value is more that 59, set
        //textbox value as 00  
        function CheckMinutes(sender) {
            if (sender.value > 59)
                sender.value = 0;
        }

        function EnableDisableDetention(sender) {
            var ddl = $("#_ddlServiceCivilImmAsylumTravelWaitingDetCentre");
            ddl.attr("selectedIndex", 0); 
            ddl.attr("disabled", !sender.checked);
        }
    </script>

    <script src="../Javascript/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../Javascript/Validation.js" type="text/javascript"></script>

    <script src="../Javascript/Bubble-Tooltip.js" type="text/javascript"></script>
    </form>
</body>
</html>

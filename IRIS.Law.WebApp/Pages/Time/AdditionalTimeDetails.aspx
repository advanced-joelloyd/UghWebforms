<%@ Page Title="Additional Time Details" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="AdditionalTimeDetails.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Time.AdditionalTimeDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">
    <table width="100%" border="0">
        <tr>
            <td>
                <asp:UpdatePanel ID="_updPnlMessage" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
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
    <asp:Panel ID="_pnlAdvocacy" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Advocacy Location
                </td>
                <td>
                    <asp:DropDownList ID="_ddlAdvocacyLocation" runat="server" OnSelectedIndexChanged="_ddlAdvocacyLocation_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Service
                </td>
                <td>
                    <asp:UpdatePanel ID="_updAdvocacyLocation" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList SkinID="Large" Enabled="false" ID="_ddlServiceAdvocacyLocation"
                                runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlAdvocacyLocation" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" valign="top" style="width: 130px; padding-top: 5px;">
                    Hearing Type
                </td>
                <td>
                    <div style="height: 200px; width: 300px; overflow: auto; border: solid 1px #7F9DB9;">
                        <asp:CheckBoxList ID="_chklstHearingType" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlAttendance" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Attendance Location
                </td>
                <td>
                    <asp:DropDownList ID="_ddlAttendanceLocation" AutoPostBack="true" runat="server"
                        OnSelectedIndexChanged="_ddlAttendanceLocation_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Service
                </td>
                <td>
                    <asp:UpdatePanel ID="_updAttendLocation" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList SkinID="Large" Enabled="false" ID="_ddlServiceAttendanceLocation"
                                runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlAttendanceLocation" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" valign="top" style="width: 150px; padding-top: 5px;">
                    Attendance Individuals
                </td>
                <td>
                    <div style="height: 200px; width: 300px; overflow: auto; border: solid 1px #7F9DB9;">
                        <asp:CheckBoxList ID="_chklstAttendanceIndividuals" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlPoliceStationCalls" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 150px;">
                    Police Station Contacted
                </td>
                <td>
                    <asp:DropDownList ID="_ddlPoliceStationCalls" runat="server" OnSelectedIndexChanged="_ddlPoliceStationCalls_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Service
                </td>
                <td>
                    <asp:UpdatePanel ID="_updPoliceStationCalls" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList SkinID="Large" Enabled="false" ID="_ddlServicePoliceStationCalls"
                                runat="server">
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_ddlPoliceStationCalls" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlTravel" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Miles
                </td>
                <td>
                    <asp:TextBox runat="server" ID="_txtMiles" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" valign="top" style="width: 130px; padding-top: 5px;">
                    Fares Description
                </td>
                <td>
                    <asp:TextBox ID="_txtFaresDescription" runat="server" SkinID="Small" TextMode="MultiLine"
                        Height="155px" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Fares
                </td>
                <td>
                    <asp:TextBox ID="_txtFares" runat="server" Text="0.00" SkinID="Small" MaxLength="11" onmousemove="showToolTip(event);return false;"
                             onmouseout="hideToolTip();"></asp:TextBox>
                                                
                     <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtFares" runat="server"
                            TargetControlID="_txtFares"         
                            FilterType="Custom, Numbers"
                            ValidChars="." />
                                                    
                      <asp:RegularExpressionValidator ID="revtxtFares"  ControlToValidate="_txtFares" runat="server" 
                          ErrorMessage="The format of the number is incorrect" Display="None" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlRunningTime" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 150px;">
                    Running Time of Tape
                </td>
                <td>
                    <asp:TextBox runat="server" ID="_txtHour" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)"></asp:TextBox>
                    :
                    <asp:TextBox runat="server" ID="_txtMinutes" SkinID="Small" onkeypress="return CheckNumeric(event);"
                        onkeyup="CheckUnits(this)"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumJRFormFilling" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 150px;">
                    Is this JR/Form filling?
                </td>
                <td>
                    <asp:CheckBox ID="_chkCivilImmAsylumJRFormFilling" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumMentalHearingAdjourned" runat="server" Visible="false"
        Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 170px;">
                    Was the hearing adjourned?
                </td>
                <td>
                    <asp:CheckBox ID="_chkCivilImmAsylumMentalHearingAdjourned" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumSubstantiveHearing" runat="server" Visible="false"
        Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 220px;">
                    Was a substantive hearing attended?
                </td>
                <td>
                    <asp:CheckBox ID="_chkCivilImmAsylumSubstantiveHearing" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCivilImmAsylumTravelWaitingDetCentre" runat="server" Visible="false"
        Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 270px;">
                    <asp:Label ID="_lblCivilImmAsylumTravelWaitingDetCentre" runat="server" Text="Is this travel/Waiting to/at a detention centre?">
                    </asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="_chkCivilImmAsylumTravelWaitingDetCentre" runat="server" onclick="EnableDisableDetention(this);"/>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Service
                </td>
                <td>
                    <asp:DropDownList SkinID="Large" ID="_ddlServiceCivilImmAsylumTravelWaitingDetCentre" Enabled="false"
                        runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlCourtDutyAttendance" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Court
                </td>
                <td>
                    <asp:DropDownList SkinID="Large" ID="_ddlCourtDutyAttCourt" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" valign="top" style="width: 260px; padding-top: 5px;">
                    Number of Dependents/Suspects dealt with
                </td>
                <td>
                   
                    <asp:TextBox ID="_txtCourtDutyAttNoOfDef" runat="server" Text="0.00" SkinID="Small" MaxLength="11" onmousemove="showToolTip(event);return false;"
                             onmouseout="hideToolTip();"></asp:TextBox>
                                                
                     <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtCourtDutyAttNoOfDef" runat="server"
                            TargetControlID="_txtCourtDutyAttNoOfDef"         
                            FilterType="Custom, Numbers"
                            ValidChars="." />
                                                    
                      <asp:RegularExpressionValidator ID="revtxtCourtDutyAttNoOfDef"  ControlToValidate="_txtCourtDutyAttNoOfDef" runat="server" 
                          ErrorMessage="The format of the number is incorrect" Display="None" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 280px;">
                    Were any of the Defendants/Suspects a youth?
                </td>
                <td>
                    <asp:CheckBox ID="_chkCourtDutyAtYouth" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlPoliceStationAttendance" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt" style="width: 150px;">
                    Police Station Attended
                </td>
                <td>
                    <asp:DropDownList ID="_ddlPoliceStationAttendance" runat="server" OnSelectedIndexChanged="_ddlPoliceStationAttendance_SelectedIndexChanged" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 130px;">
                    Service
                </td>
                <td>
                    <asp:DropDownList SkinID="Large" ID="_ddlServicePoliceStationAttendance"
                        runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="_pnlFileReviews" runat="server" Visible="false" Height="362px">
        <table width="100%" border="0">
            <tr>
                <td class="boldTxt">
                    <CliMat:ClientMatterDetails runat="server" SetSession="false" ID="_cliMatDetailsFileReview" EnableValidation="false" OnMatterChanged="_cliMatDetailsFileReview_MatterChanged" />
                </td>
            </tr>
            <tr>
                <td class="boldTxt">
                    <asp:RadioButton ID="_rdoBtnPaper" runat="server" GroupName="FileReviewType" Text="Paper" />
                    <asp:RadioButton ID="_rdoBtnFaceToFace" runat="server" GroupName="FileReviewType"
                        Text="Face to Face" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <table width="100%">
        <tr>
            <td align="right" style="padding-right: 15px;" colspan="2">
                <table>
                    <tr>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnBack" runat="server" Text="Back" CssClass="button" CausesValidation="false"
                                    OnClick="_btnBack_Click" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnCancel" runat="server" Text="Reset" CssClass="button" CausesValidation="false"
                                    OnClick="_btnCancel_Click" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnSave" runat="server" Text="Save" CssClass="button" CausesValidation="true"
                                    OnClick="_btnSave_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
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

        function EnableDisableDetention(sender) {
            var ddl = $("#<%=_ddlServiceCivilImmAsylumTravelWaitingDetCentre.ClientID %>");
            ddl.attr("selectedIndex", 0);
            ddl.attr("disabled", !sender.checked);
        }
    </script>

</asp:Content>

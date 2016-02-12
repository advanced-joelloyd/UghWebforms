<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="EditMatter.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Matter.EditMatter"
    Title="Matter Details" %>

<%@ Register TagPrefix="CC" TagName="CalendarControl" Src="~/UserControls/CalendarControl.ascx" %>
<%@ Register TagPrefix="CliMat" TagName="ClientMatterDetails" Src="~/UserControls/ClientMatterDetails.ascx" %>
<%@ Register TagPrefix="DF" TagName="DocumentFiles" Src="~/UserControls/DocumentFiles.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="_contentEditMatter" ContentPlaceHolderID="_cphMain" runat="server">

    <script type="text/javascript">
 
     
        var browser = navigator.appName;
        //W3C has offered some new options for borders in CSS3, of which one is border-radius. 
        //Both Mozila/Firefox and Safari 3 have implemented this function, which allows you to create round corners 
        //on box-items. This is not yet implemented in IE so round the corners using javascript
        if (browser == "Microsoft Internet Explorer") {
            Sys.Application.add_load(RoundedCorners);
        }

        function RoundedCorners() {
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }

        Sys.Application.add_load(function() {
            $('[readonly]').addClass("readonly");
        });

        var prm = Sys.WebForms.PageRequestManager.getInstance(); 
        var postBackElement;
        

        // This will find the co-ordinates of the control
        function findPos(obj) {
            var curleft = curtop = 0;
            curleft = obj.clientWidth + 35;
            if (obj.offsetParent) {
                do {
                    curleft += obj.offsetLeft;
                    curtop += obj.offsetTop;
                } while (obj = obj.offsetParent);
            }
            return [curleft, curtop];
        }

        // This will Enable/Disable Public Funding Tab
        function ToggleTabPanel(value) {
                if (value == "False" || (!value)) {
                    $find('<%=_pnlPublicFunding.ClientID%>')._tab.style.display = "none";
                }
                else {
                    $find('<%=_pnlPublicFunding.ClientID%>')._tab.style.display = "";
                }
            } 
        
        Sys.Application.add_load(function() {
        if (document.getElementById("<%=_chkPublicFunded.ClientID%>")) {
                var test = document.getElementById("<%=_chkPublicFunded.ClientID%>").checked;
                ToggleTabPanel(test);
            }
        });

        Sys.Application.add_load(function() {
            $("#" + "<%=_rdoBtnMatterCompletedYes.ClientID%>").click(function() { 
                $.ajax({
                    type: "POST",
                    url: "EditMatter.aspx/GetSystemDate",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg) {
                        var dateTime = msg.d.split("$");
                        $("#" + "<%=_txtMatterClosedDate.ClientID%>").val(dateTime[0]);
                        $("#" + "<%=_txtMatterClosedTime.ClientID%>").val(dateTime[1]); 
                    },
                    error: function(msg) { 
                    }
                });
            });
        });


 


        jQuery('.numbersOnly').keyup(function() {
            this.value = this.value.replace(/[^0-9\.]/g, '');
        }); 
        

    </script>

    <script type="text/javascript">
        function GetDateTime() {
            var closedDate = document.getElementById("<%=_txtMatterClosedDate.ClientID%>");
            var closedTime = document.getElementById("<%=_txtMatterClosedTime.ClientID%>");
            if (document.getElementById("<%=_rdoBtnMatterCompletedYes.ClientID%>").checked) {
                var currrentDateTime = new Date();
                closedDate.value = currrentDateTime.format("dd/MM/yyyy");
                closedTime.value = currrentDateTime.format("HH:mm");
            }
            else {
                closedDate.value = "";
                closedTime.value = "00:00";
            }
        }

        
    </script>

    <script type="text/javascript">
        function GetDateTime() {
            var closedDate = document.getElementById("<%=_txtMatterClosedDate.ClientID%>");
            var closedTime = document.getElementById("<%=_txtMatterClosedTime.ClientID%>");
            if (document.getElementById("<%=_rdoBtnMatterCompletedYes.ClientID%>").checked) {
                var currrentDateTime = new Date();
                closedDate.value = currrentDateTime.format("dd/MM/yyyy");
                closedTime.value = currrentDateTime.format("HH:mm");
            }
            else {
                closedDate.value = "";
                closedTime.value = "00:00";
            }
        }

        function messageHide(ctrl) {
            setTimeout('$("#' + ctrl + '").fadeOut("slow")', 10000);
        }
        
    </script>

    <div id="_printPreview">
    </div>
    <div id="_PageContents">
        <table width="100%">
            <tr>
                <td class="sectionHeader">
                    Matter Details
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="_updPanelError" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="_lblError" CssClass="errorMessage" runat="server" Text=""></asp:Label>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                            <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <table cellpadding="2" width="100%">
            <tr>
                <td>
                    <div id="MatterDetails">
                        <CliMat:ClientMatterDetails runat="server" DisplayModuleLinks="True" ID="_cliMatDetails"
                            OnMatterChanged="_cliMatDetails_MatterChanged" />
                    </div>
                </td>
                <td align="right" valign="top">
                    <asp:ValidationSummary runat="server" HeaderText="You have some errors, please correct them before you can save."
                        Font-Size="X-Small" Font-Names="arial" ID="_vsEditMatter" ShowSummary="true" />
                </td>
            </tr>
        </table>
        <table width="100%">
            <tr>
                <td>
                    <ajaxToolkit:TabContainer ID="_tcEditMatter" runat="server" CssClass="ajax__tab_xp2"
                        Width="100%" ActiveTabIndex="0">
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlDetails" HeaderText="Details">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" colspan="3">
                                                    Key Matter Details
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 125px;" class="boldTxt">
                                                    Description
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="_txtDescription" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="_rfvDescription" runat="server" ErrorMessage="Description is mandatory"
                                                        Display="None" ControlToValidate="_txtDescription"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Key Description
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="_txtKeyDescription" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Fee Earner
                                                </td>
                                                <td colspan="2">
                                                    <asp:DropDownList ID="_ddlFeeEarner" runat="server" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();" OnSelectedIndexChanged="_txtUFNDate_TextChanged"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvFeeEarner" runat="server" ErrorMessage="Fee Earner is mandatory"
                                                        Display="None" ControlToValidate="_ddlFeeEarner"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span> &nbsp;&nbsp;
                                                    <asp:HyperLink ID="_hlFeeEarnerEmail" runat="server" />
                                                    <asp:HiddenField runat="server" ID="_ufnFromRange" />
                                                    <asp:HiddenField runat="server" ID="_ufnToRange" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Supervisor
                                                </td>
                                                <td colspan="2">
                                                    <asp:DropDownList ID="_ddlSupervisor" runat="server" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvSupervisor" runat="server" ErrorMessage="Supervisor is mandatory"
                                                        Display="None" ControlToValidate="_ddlSupervisor"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Work Type
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlWorkType" runat="server" AutoPostBack="true" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();" OnSelectedIndexChanged="_ddlWorkType_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvWorkType" runat="server" ErrorMessage="Work Type is mandatory"
                                                        Display="None" ControlToValidate="_ddlWorkType"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td id="_tdSpanType" runat="server" style="display: none;">
                                                    <table>
                                                        <tr>
                                                            <td class="boldTxt">
                                                                Span Type 1 :
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="_lblSpanType1" runat="server" CssClass="labelValue"></asp:Label>
                                                            </td>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                            <td class="boldTxt">
                                                                Span Type 2 :
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="_lblSpanType2" runat="server" CssClass="labelValue"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="height: 10px">
                                                </td>
                                            </tr>
                                        </table>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Account and Key Date Details
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt" style="width: 125px">
                                                    Client Bank
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlClientBank" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvClientBank" runat="server" ErrorMessage="Client Bank is mandatory"
                                                        Display="None" ControlToValidate="_ddlClientBank"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt" style="width: 125px">
                                                    Open Date
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccOpenDate" InvalidValueMessage="Invalid Open Date" runat="server" />
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
                                                    <asp:RequiredFieldValidator ID="_rfvOfficeBank" runat="server" ErrorMessage="Office Bank is mandatory"
                                                        Display="None" ControlToValidate="_ddlOfficeBank"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt">
                                                    Next Review
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccNextReview" InvalidValueMessage="Invalid Next Review Date"
                                                        Enabled="false" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Deposit Bank
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlDepositBank" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td class="boldTxt">
                                                    Cost Review
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccCostReview" InvalidValueMessage="Invalid Cost Review Date"
                                                        Enabled="false" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Branch Reference
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlBranch" runat="server" OnSelectedIndexChanged="_ddlBranch_SelectedIndexChanged"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvBranch" runat="server" ErrorMessage="Branch is mandatory"
                                                        Display="None" ControlToValidate="_ddlBranch"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt">
                                                    Last Saved
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccLastSaved" InvalidValueMessage="Invalid Last Saved Date"
                                                        Enabled="false" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Department
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlDepartment" runat="server" OnSelectedIndexChanged="_ddlDepartment_SelectedIndexChanged"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvDepartment" runat="server" ErrorMessage="Department is mandatory"
                                                        Display="None" ControlToValidate="_ddlDepartment"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt">
                                                    Closed Date
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccClosedDate" InvalidValueMessage="Invalid Closed Date"
                                                        Enabled="false" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Charge Rate
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlChargeRate" runat="server" SkinID="Large" onmousemove="showToolTip(event);return false;"
                                                        onmouseout="hideToolTip();" OnSelectedIndexChanged="_ddlChargeRate_SelectedIndexChanged"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="_rfvChargeRate" runat="server" ErrorMessage="Charge Rate is mandatory"
                                                        Display="None" ControlToValidate="_ddlChargeRate"></asp:RequiredFieldValidator>
                                                    <span class="mandatoryField">*</span>
                                                </td>
                                                <td class="boldTxt">
                                                    Destroy Date
                                                </td>
                                                <td>
                                                    <CC:CalendarControl ID="_ccDestroyDate" Enabled="false" InvalidValueMessage="Invalid Destroy Date"
                                                        runat="server" />
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
                                                <td class="boldTxt">
                                                    File Away Ref
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtFileAwayRef" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="height: 10px">
                                                </td>
                                            </tr>
                                        </table>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" colspan="2">
                                                    Is Matter Completed
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 125px;" class="boldTxt">
                                                    <asp:RadioButton ID="_rdoBtnMatterCompletedYes" runat="server" GroupName="MatterCompleted"
                                                        Text="Yes" />
                                                    <asp:RadioButton ID="_rdoBtnMatterCompletedNo" runat="server" GroupName="MatterCompleted"
                                                        Text="No" onclick="GetDateTime()" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtMatterClosedDate" runat="server" SkinID="Small"></asp:TextBox>
                                                    <asp:TextBox ID="_txtMatterClosedTime" runat="server" SkinID="Small"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlAdditionalInfo" HeaderText="Additional Info">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="_updPanelAdditionalInfo" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Limits and Status
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Quote
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtQuote" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtQuote" runat="server" TargetControlID="_txtQuote"
                                                        FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtQuote" ControlToValidate="_txtQuote" runat="server"
                                                        ErrorMessage="The format of the number is incorrect" Display="None" ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                                <td class="boldTxt" style="width: 125px;">
                                                    Status
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtStatus" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Disbs Limit
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtDisbsLimit" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtDisbsLimit" runat="server" TargetControlID="_txtDisbsLimit"
                                                        FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtDisbsLimit" ControlToValidate="_txtDisbsLimit"
                                                        runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                        ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                                <td class="boldTxt">
                                                    Indicators
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtIndicators" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Time Limit
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtTimeLimit" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtTimeLimit" runat="server" TargetControlID="_txtTimeLimit"
                                                        FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtTimeLimit" ControlToValidate="_txtTimeLimit"
                                                        runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                        ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                                <td class="boldTxt">
                                                    Bank Reference
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="_txtBankReference" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    WIP Limit
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtWIPLimit" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtWIPLimit" runat="server" TargetControlID="_txtWIPLimit"
                                                        FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtWIPLimit" ControlToValidate="_txtWIPLimit"
                                                        runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                        ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Overall Matter Limit
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtOverallMatterLimit" runat="server" Text="0.00" SkinID="Small"
                                                        MaxLength="11" onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtOverallMatterLimit" runat="server"
                                                        TargetControlID="_txtOverallMatterLimit" FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtOverallMatterLimit" ControlToValidate="_txtOverallMatterLimit"
                                                        runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                        ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="height: 10px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Credit Control Information
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Cash Collection
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="_ddlCashCollection" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Credit Limit
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtCreditLimit" runat="server" Text="0.00" SkinID="Small" MaxLength="11"
                                                        onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="ftbetxtCreditLimit" runat="server" TargetControlID="_txtCreditLimit"
                                                        FilterType="Custom, Numbers" ValidChars="." />
                                                    <asp:RegularExpressionValidator ID="revtxtCreditLimit" ControlToValidate="_txtCreditLimit"
                                                        runat="server" ErrorMessage="The format of the number is incorrect" Display="None"
                                                        ValidationExpression="^\d+(\.\d\d)?$"> </asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" style="height: 10px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="sectionHeader" colspan="4">
                                                    Misc Info
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Our Reference
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtOurReference" Text="A00001-0001/AS" runat="server" SkinID="Large"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Previous Reference
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtPreviousReference" runat="server"></asp:TextBox>
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
                                                    Source Campaign
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="_ddlSourceCampaign" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Person Dealing
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="_ddlPersonDealing" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Salutation Envelope
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtSalutationEnvelope" runat="server" SkinID="Large"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Salutation Letter
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtSalutationLetter" runat="server" SkinID="Large"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt" valign="top">
                                                    Letter Head
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="_txtLetterHead" runat="server" SkinID="Large" TextMode="MultiLine"
                                                        Rows="5"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlPublicFunding" HeaderText="Public Funding">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="_updPanelPublicFunding" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table width="100%">
                                            <tr>
                                                <td class="sectionHeader">
                                                    General Info
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                        <table border="0">
                                            <tr>
                                                <td class="boldTxt">
                                                    Public Funded
                                                </td>
                                                <td class="textBox">
                                                    <asp:CheckBox ID="_chkPublicFunded" runat="server" Checked="true" Enabled="false" />
                                                </td>
                                                <td class="boldTxt" style="width: 75px;" runat="server" id="_tdlblUFN">
                                                    UFN
                                                </td>
                                                <td runat="server" id="_tdUFNDateNumber">
                                                    <CC:CalendarControl ID="_ccUFNDate" InvalidValueMessage="Invalid UFN Date" runat="server"
                                                        OnDateChanged="_txtUFNDate_TextChanged" />
                                                    <asp:TextBox ID="_txtUFNNumber" runat="server" SkinID="Small" onkeypress="return CheckNumeric(event);"
                                                        onkeyup="CheckUnits(this)" OnTextChanged="_txtUFNNumber_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    SQM
                                                </td>
                                                <td colspan="3">
                                                    <asp:CheckBox ID="_chkSQM" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    London Rate
                                                </td>
                                                <td colspan="3">
                                                    <asp:CheckBox ID="_chkLondonRate" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Certificate Number
                                                </td>
                                                <td colspan="3" style="padding-left: 5px;">
                                                    <asp:TextBox ID="_txtCertificateNumber" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="boldTxt">
                                                    Certificate Limits
                                                </td>
                                                <td colspan="3" style="padding-left: 5px;">
                                                    <asp:TextBox ID="_txtCertificateLimits" runat="server" SkinID="Large" TextMode="MultiLine"
                                                        Rows="5"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlContacts" HeaderText="Address">
                            <HeaderTemplate>
                                Contacts
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table width="100%">
                                    <tr>
                                        <td style="height: 300px" valign="top">
                                            <asp:Label CssClass="sectionHeader" ID="_lblContact" runat="server" Width="100%"></asp:Label>
                                            <br />
                                            <br />
                                            <div class="conflictCheckBg">
                                                <asp:Table ID="_tblConflictCheckGroup" Width="100%" runat="server" CellSpacing="0">
                                                </asp:Table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table cellpadding="0" width="100%">
                                    <tr>
                                        <td align="right">
                                            <table cellpadding="2">
                                                <tr>
                                                    <td>
                                                        <div class="button" style="text-align: right;" id="_divAddContactButton" runat="server">
                                                            <asp:LinkButton ID="_btnCreateContact" runat="server" Text="Create" PostBackUrl="~/Pages/Contact/CreateContact.aspx"  CausesValidation="false" />
                                                        
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div class="button" style="text-align: right;" id="_divAttachContactButton" runat="server">
                                                            <asp:LinkButton ID="_btnAttachContact" runat="server" Text="Attach" PostBackUrl="~/Pages/Contact/AddAssociationForMatter.aspx" CausesValidation="false" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <%-- OnClientClick="pageRedirect('/DocumentManagement/ViewMatterHistory.aspx');return false;"--%>
                        <ajaxToolkit:TabPanel runat="server" ID="_pnlDocuments" HeaderText="Documents">
                            <HeaderTemplate>
                                Documents
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="_updplDocuments" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <DF:DocumentFiles ID="_dfDocumentFiles" runat="Server" ReadOnly="True" />
                                        <br />
                                        <table cellpadding="0" width="100%">
                                            <tr>
                                                <td align="right">
                                                    <div class="button" style="text-align: right;" id="_divUploadDocument" runat="server">
                                                        <asp:Button ID="_btnUploadDocument" runat="server" Text="Import" PostBackUrl="~/Pages/DocMgmt/ViewMatterHistory.aspx" CausesValidation="false" />
                                                   
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="_btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="_cliMatDetails" EventName="MatterChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </td>
            </tr>
            <tr>
                <td align="right" style="padding-right: 15px;">
                    <table>
                        <tr>
                            <td align="right">
                                <div class="button" style="text-align: center; width: 100px" id="_divPrintBtn" visible="true"
                                    runat="server">
                                    <center>
                                        <asp:Button ID="_btnPrint" CausesValidation="false" runat="server" Text="Print" OnClientClick="javascript:window.open('editmatter.aspx?PrintPage=true&view=Matter', 'Report', 'height=500,width=700,scrollbars=1,status=no,toolbar=no,menubar=no,location=no');return false;" /></center>
                                </div>
                            </td>
                            <td align="right">
                                <div class="button" style="text-align: center;" id="_divBackButton" visible="false"
                                    runat="server">
                                    <asp:Button ID="_btnBack" runat="server" Text="Back" OnClientClick="javascript:window.location='SearchMatter.aspx';return false;"
                                        OnClick="_btnBack_Click" CausesValidation="false" />
                                </div>
                            </td>
                            <td align="right">
                                <div class="button" style="text-align: center;">
                                    <asp:Button ID="_btnSave" runat="server" Text="Save" OnClick="_btnSave_Click" CausesValidation="true" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

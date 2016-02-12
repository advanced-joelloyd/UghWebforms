<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master"
    AutoEventWireup="true" CodeBehind="ChangeStyle.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.SiteConfig.ChangeStyle" %>

<%@ Register Assembly="ColorExtender" Namespace="ColorExtender" TagPrefix="ce1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_cphMain" runat="server">

    <script src="../Javascript/niftycube.js" type="text/javascript"></script>

    <script type="text/javascript">
        Sys.Application.add_load(RoundedCorners);

        function RoundedCorners() {
            Nifty("div.button");
        } 	
    </script>

    <script language="javascript" type="text/javascript">
        var WizIndex = 0;
        function fnSaveStyle() {
            return confirm("This will apply the changed styles throughout the application. Do you wish to continue?");
        }
        function fnResetStyle() {
            return confirm("This will apply the last saved styles throughout the application. Do you wish to continue?");
        }

        function fnValidateImage() {
            var controls = document.getElementsByTagName("input");
            var uploadcontrol;
            var msg = "";
            for (var i = 0; i < controls.length; i++) {
                if (controls[i].type != "file")
                    continue;
                else {
                    uploadcontrol = controls[i];
                    var logopath = uploadcontrol.value;
                    var found = logopath.search(/.gif/i);

                    //if (logopath.length > 0 && found < 0) {
                    //    msg = "Logo selected should be a GIF image. \n";
                    //}
                }
            }

            if (msg.length > 0) {
                alert(msg);
                return false;
            }
            return true;
        }

        function CancelSaveCSSPopup() {
            $find('_mpeSaveCSSBehavior').hide();
        }

        function DisplaySaveAsDialog() {
            if(fnValidateImage())
                $find('_mpeSaveCSSBehavior').show();
            return false;
        }
    </script>

    <input id="_btnDummy" runat="server" type="button" value="." style="height: 1px;
        width: 1px; display: none;" disabled="disabled" />
    <ajaxToolkit:ModalPopupExtender ID="_mpeSaveCSS" runat="server" BackgroundCssClass="modalBackground"
        DropShadow="true" PopupControlID="_pnlSaveCSS" OnCancelScript="return false;"
        TargetControlID="_btnDummy" BehaviorID="_mpeSaveCSSBehavior">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel ID="_pnlSaveCSS" runat="server" Style="background-color: #ffffff; display: none;
        padding: 2px;" Width="400px">
        <table width="100%">
            <tr>
                <td>
                    <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="sectionHeader" align="left">
                    <asp:Label ID="_lblHeader" runat="server"></asp:Label>&nbsp Save CSS As...
                    
                </td>
            </tr>
            <tr>
                <td width="100%">
                    <asp:Panel ID="_pnlSave" runat="server" DefaultButton="_btnOK">
                        <table border="0" cellpadding="0" cellspacing="0" class="panel" width="100%">
                            <tr>
                                <td align="left">
                                    <asp:Label ID="_lblLabel" runat="server" CssClass="labelValue"></asp:Label>
                                    <asp:HiddenField ID="_hdnId" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%">
                                    <table cellpadding="0" cellspacing="1" border="0" style="width: 100%;">
                                        <tr>
                                            <td class="boldTxt" valign="top" style="width: 125px; padding-top: 5px;">
                                                Name:
                                            </td>
                                            <td>
                                                <asp:TextBox ID="_txtCSSName" TextMode="SingleLine" Width="95%" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="padding-right: 50px;">
                                    <table width="100%">
                                        <tr>
                                            <td align="right" style="padding-right: 15px;">
                                                <table>
                                                    <tr>
                                                        <td align="right">
                                                            <div class="button" style="text-align: center;">
                                                                <asp:Button ID="_btnOK" runat="server" CausesValidation="True" OnClick="_btnOK_Click"
                                                                    Text="OK" />
                                                            </div>
                                                        </td>
                                                        <td align="left">
                                                            <div class="button" style="text-align: center;" id="_divCancelButton" runat="server">
                                                                <asp:Button ID="_btnCancel" runat="server" CausesValidation="False" Text="Cancel"
                                                                    OnClientClick="CancelSaveCSSPopup();" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="_lblError" runat="server" CssClass="textBox"></asp:Label>
    <asp:Label ID="_lblHeaderCSSFile" runat="server"></asp:Label>
    <asp:Wizard ID="_wizardStyle" runat="server" DisplaySideBar="false" Width="100%"
        OnPreviousButtonClick="_wizardStyle_PreviousButtonClick" OnNextButtonClick="_wizardStyle_NextButtonClick"
        EnableTheming="True" Height="400px" StepStyle-VerticalAlign="Top">
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
                                        <asp:Button ID="_btnApply1" runat="server" Text="Preview" OnClick="_btnPreview_Click"
                                            OnClientClick="javascript:return fnValidateImage();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnSaveChangedCSS1" runat="server" Text="Save As" OnClientClick="return DisplaySaveAsDialog();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnStartSave" runat="server" Text="Save" OnClick="_btnSave_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnStartCancel" runat="server" Text="Cancel" OnClick="_btnStartCancel_Click" />
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
                                        <asp:Button ID="_btnApply2" runat="server" Text="Preview" CssClass="button" OnClick="_btnPreview_Click"
                                            OnClientClick="javascript:return fnValidateImage();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnSaveChangedCSS2" runat="server" Text="Save As" CssClass="button"
                                            OnClientClick="return DisplaySaveAsDialog();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnStepSave" runat="server" Text="Save" OnClick="_btnSave_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnStepCancel" runat="server" Text="Cancel" OnClick="_btnStartCancel_Click" />
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
                                        <asp:Button ID="_btnApply3" runat="server" Text="Preview" CssClass="button" OnClick="_btnPreview_Click"
                                            OnClientClick="javascript:return fnValidateImage();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnSaveChangedCSS3" runat="server" Text="Save As" CssClass="button"
                                            OnClientClick="return DisplaySaveAsDialog();" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnFinishSave" runat="server" Text="Save" OnClick="_btnSave_Click" />
                                    </div>
                                </td>
                                <td align="right">
                                    <div class="button" style="text-align: center;">
                                        <asp:Button ID="_btnFinishCancel" runat="server" Text="Cancel" OnClick="_btnStartCancel_Click" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </FinishNavigationTemplate>
        <WizardSteps>
        </WizardSteps>
    </asp:Wizard>
    <table width="100%" border="0" class="panel" style="display: none;">
        <tr>
            <td style="width: 100%">
                <asp:Table BorderWidth="2" runat="server" ID="_tblMain" Width="100%">
                </asp:Table>
            </td>
        </tr>
    </table>
     
</asp:Content>


<%@ Page Language="C#" Title="" AutoEventWireup="true" MasterPageFile="~/MasterPages/ILBHomePage.Master" CodeBehind="Feedback.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Feedback.IrisFeedback" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:content id="Content1" contentplaceholderid="_cphMain" runat="server">
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

    <table width="100%">
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="_lblError" runat="server" CssClass="errorMessage"></asp:Label>
                    </ContentTemplate>
                   
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="sectionHeader">
                <label runat="server" ID="feedbackTitle"></label>
            </td>
        </tr>
        <tr>
            <td style="width: 25px;">
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2">
    <tr>
        <td class="boldTxt" style="text-indent:0px">Please fill out our site usage survey. Your feedback is important to us and will help us to improve the site.</td>
    </tr>
    <tr>
        <td>&nbsp;</td>
    </tr>
    <tr>
        <td>
            <table cellpadding="2" cellspacing="0">
            
            <tr>
                <td class="boldTxt" style="width:360px; text-indent:0px">How would you rate the overall design of the site?</td>
                <td  style="width:10px">&nbsp;</td>
                <td class="boldTxt" style="width:70px">Very&nbsp;Poor</td>
                <td>
                    <asp:radiobuttonlist id="_rdbtnListRateOverallSite"  Font-Names="arial" Font-Size="Small" runat="server" RepeatDirection="Horizontal">
                        <asp:listitem id="_ltRateOverallSiteOption1" runat="server" value="1" />
                        <asp:listitem id="_ltRateOverallSiteOption2" runat="server" value="2" />
                        <asp:listitem id="_ltRateOverallSiteOption3" runat="server" value="3" />
                        <asp:listitem id="_ltRateOverallSiteOption4" runat="server" value="4" />
                        <asp:listitem id="_ltRateOverallSiteOption5" runat="server" value="5" Selected="True" />
                        <asp:listitem id="_ltRateOverallSiteOption6" runat="server" value="6" />
                        <asp:listitem id="_ltRateOverallSiteOption7" runat="server" value="7" />
                        <asp:listitem id="_ltRateOverallSiteOption8" runat="server" value="8" />
                        <asp:listitem id="_ltRateOverallSiteOption9" runat="server" value="9" />
                        <asp:listitem id="_ltRateOverallSiteOption10" runat="server" value="10" />
                  </asp:radiobuttonlist>


                </td>
                 <td class="boldTxt" style="width:100px">Excellent</td>
                   
            </tr>
            </table>
        </td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td>
            <table cellpadding="2" cellspacing="0">
            
            <tr>
                <td class="boldTxt" style="width:360px; text-indent:0px" >How easy was it to find the information you were looking for?</td>
                <td style="width:10px">&nbsp;</td>
                <td class="boldTxt" style="width:70px">Very&nbsp;Hard</td>
                <td>
                    <asp:radiobuttonlist id="_rdbtnListFindInformation"  Font-Names="arial" Font-Size="Small" runat="server" RepeatDirection="Horizontal">
                        <asp:listitem id="_ltFindInformationOption1" runat="server" value="1" />
                        <asp:listitem id="_ltFindInformationOption2" runat="server" value="2" />
                        <asp:listitem id="_ltFindInformationOption3" runat="server" value="3" />
                        <asp:listitem id="_ltFindInformationOption4" runat="server" value="4" />
                        <asp:listitem id="_ltFindInformationOption5" runat="server" value="5" Selected="True" />
                        <asp:listitem id="_ltFindInformationOption6" runat="server" value="6" />
                        <asp:listitem id="_ltFindInformationOption7" runat="server" value="7" />
                        <asp:listitem id="_ltFindInformationOption8" runat="server" value="8" />
                        <asp:listitem id="_ltFindInformationOption9" runat="server" value="9" />
                        <asp:listitem id="_ltFindInformationOption10" runat="server" value="10" />
                  </asp:radiobuttonlist>


                </td>
                <td class="boldTxt" style="width:100px">Extremely&nbsp;Easy</td>
            </tr>
            </table>
        </td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td>
            <table cellpadding="2" cellspacing="0">
            
            <tr>
                <td class="boldTxt" style="width:360px; text-indent:0px">How often do you visit the site? </td>
                <td  style="width:10px">&nbsp;</td>
                <td class="boldTxt" style="width:70px">Rarely</td>
                <td>
                    <asp:radiobuttonlist id="_rdbtnListVisitSite"  Font-Names="arial" Font-Size="Small" runat="server" RepeatDirection="Horizontal">
                        <asp:listitem id="_ltVisitSiteOption1" runat="server" value="1" />
                        <asp:listitem id="_ltVisitSiteOption2" runat="server" value="2" />
                        <asp:listitem id="_ltVisitSiteOption3" runat="server" value="3" />
                        <asp:listitem id="_ltVisitSiteOption4" runat="server" value="4" />
                        <asp:listitem id="_ltVisitSiteOption5" runat="server" value="5" Selected="True" />
                        <asp:listitem id="_ltVisitSiteOption6" runat="server" value="6" />
                        <asp:listitem id="_ltVisitSiteOption7" runat="server" value="7" />
                        <asp:listitem id="_ltVisitSiteOption8" runat="server" value="8" />
                        <asp:listitem id="_ltVisitSiteOption9" runat="server" value="9" />
                        <asp:listitem id="_ltVisitSiteOption10" runat="server" value="10" />
                  </asp:radiobuttonlist>


                </td>
                <td class="boldTxt" style="width:100px">Very&nbsp;Frequently</td>
                
            </tr>
            </table>
        </td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td>
            <table cellpadding="2" cellspacing="0">
            <tr>
                <td class="boldTxt" style="text-indent:0px">Do you have any suggestions on how this website can be improved? </td>
            </tr>
            <tr>
                <td><asp:TextBox TextMode="MultiLine" ID="_txtSuggestions" runat="server" Rows="5" Width="500"></asp:TextBox></td>
            </tr>
            </table>
        </td>
    </tr>
    <tr><td>&nbsp;</td></tr>
    <tr>
        <td align="right" colspan="2">
            <div class="button"><asp:Button ID="_btnSubmit" OnClick="_btnSubmit_Click" runat="server" 
                CausesValidation="True" Text="Submit"  /></div>
        </td>
    </tr>
    </table>
    

    
</asp:content>
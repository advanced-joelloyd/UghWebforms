<%@ Page Language="C#" MasterPageFile="~/MasterPages/ILBHomePage.Master" AutoEventWireup="true"
    CodeBehind="PrintableOfficeChequeRequest.aspx.cs" Inherits="IRIS.Law.WebApp.Pages.Accounts.PrintableOfficeChequeRequest"
    Title="Print - Office Cheque Request" %>

<asp:Content ID="_contentPrintableOfficeChequeRequest" ContentPlaceHolderID="_cphMain"
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
            Nifty("span.ajax__tab_tab", "small transparent top");
            Nifty("div.button");
        }

        function ClickHereToPrint() {
            try {
                var oIframe = document.getElementById('ifrmPrint');
                var oContent = document.getElementById('divToPrint').innerHTML;
                var oDoc = (oIframe.contentWindow || oIframe.contentDocument);
                if (oDoc.document) oDoc = oDoc.document;
                oDoc.write("<head><title>title</title>");
                oDoc.write("</head><body onload='this.focus(); this.print();'>");
                oDoc.write(oContent + "</body>");
                oDoc.close();
            }
            catch (e) {
                self.print();
            }
        }     
        
    </script>

    <iframe id="ifrmPrint" src='#' style="width: 0px; height: 0px;"></iframe>
    <div id="divToPrint" style="border-style: solid; border-width: thin; border-color: #1e7b84;">
        <table width="100%" border="0">
            <tr>
                <td colspan="2">
                    <asp:Label ID="_lblMessage" runat="server"></asp:Label>
                    <asp:HiddenField ID="_hdnOfficeChequeRequestId" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center" class="sectionHeader">
                    Office Payment Cheque Request
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 100px;">
                    Name
                </td>
                <td>
                    <asp:Label ID="_lblName" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 100px; vertical-align: top;">
                    Address
                </td>
                <td>
                    <table border="0" width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressLine1" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressLine2" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressLine3" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressTown" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressCounty" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="_lblAddressPostcode" runat="server" CssClass="labelValue"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 120px;">
                    Matter Reference
                </td>
                <td>
                    <asp:Label ID="_lblMatterReference" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 120px;">
                    Matter Description
                </td>
                <td>
                    <asp:Label ID="_lblMatterDescription" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 100px;">
                    Fee Earner
                </td>
                <td>
                    <asp:Label ID="_lblFeeEarner" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 100px;">
                    Partner
                </td>
                <td>
                    <asp:Label ID="_lblPartner" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="boldTxt" style="width: 100px;">
                    User
                </td>
                <td>
                    <asp:Label ID="_lblUserName" runat="server" CssClass="labelValue"></asp:Label>
                </td>
            </tr>
        </table>
        <div style="height: 200px;">
            <table width="100%" border="0">
                <tr>
                    <td colspan="9">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="boldTxt" align="left">
                        Date
                    </td>
                    <td class="boldTxt" align="left">
                        Description
                    </td>
                    <td class="boldTxt" align="left">
                        Payee
                    </td>
                    <td class="boldTxt" align="left">
                        Bank
                    </td>
                    <td class="boldTxt" align="right">
                        VAT Rate
                    </td>
                    <td class="boldTxt" align="right">
                        VAT Amount
                    </td>
                    <td class="boldTxt" align="right">
                        Amount
                    </td>
                    <td class="boldTxt" align="right">
                        Anticipated
                    </td>
                    <td class="boldTxt" align="right">
                        Authorised
                    </td>
                </tr>
                <tr>
            
                    <td>
                        <asp:Label ID="_lblOfficeChequeRequestDate" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="_lblOfficeChequeRequestDescription" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="_lblOfficeChequeRequestPayee" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="_lblOfficeChequeRequestBank" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="_lblOfficeChequeRequestVATRate" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="_lblOfficeChequeRequestVATAmount" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="_lblOfficeChequeRequestAmount" runat="server" CssClass="labelValue"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:CheckBox ID="_chkBxOfficeChequeRequestAnticipated" runat="server" Enabled="false"
                            Text="" />
                    </td>
                    <td align="right">
                        <asp:CheckBox ID="_chkBxOfficeChequeRequestAuthorised" runat="server" Enabled="false"
                            Text="" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table width="100%" border="0">
        <tr>
            <td align="right" style="padding-right: 15px;">
                <table>
                    <tr>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <input type="button" value="Print" causesvalidation="false" onclick="ClickHereToPrint();" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="text-align: center;">
                                <asp:Button ID="_btnCancel" runat="server" CausesValidation="false" Text="Cancel"
                                    OnClick="_btnCancel_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

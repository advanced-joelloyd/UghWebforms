
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uscMsgBox.ascx.cs" Inherits="IRIS.Law.WebApp.UserControls.uscMsgBox" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="iws" %>



<asp:UpdatePanel ID="udpMsj" runat="server" UpdateMode="Conditional" RenderMode="Inline">

    <ContentTemplate>
        <asp:Button ID="btnD" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Button ID="btnD2" runat="server" Text="" Style="display: none" Width="0" Height="0" />
        <asp:Panel ID="pnlMsg" runat="server" CssClass="mp" Style=" background-color:#d4d0c8" >
           <table style=" width:auto"><tr><td>
            <asp:Panel ID="pnlMsgHD" runat="server" CssClass="mpHd">
               <asp:Label  runat =server ID="lblMessage">&nbsp;Message</asp:Label>
            </asp:Panel>
            </td></tr><tr><td>
            <asp:GridView ID="grvMsg" runat="server" ShowHeader="false" Width="100%" AutoGenerateColumns="false" CssClass="tabularinfo1">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Image ID="imgErr" runat="server" ImageUrl="~/Images/PNGs/err.png"
                                            Visible=' <%# (((Message)Container.DataItem).MessageType == enmMessageType.Error) ? true : false %>' />
                                        <asp:Image ID="imgSuc" runat="server" ImageUrl="~/Images/PNGs/suc.png"
                                            Visible=' <%# (((Message)Container.DataItem).MessageType == enmMessageType.Success) ? true : false %>' />
                                        <asp:Image ID="imgAtt" runat="server" ImageUrl="~/Images/PNGs/att.png"
                                            Visible=' <%# (((Message)Container.DataItem).MessageType == enmMessageType.Attention) ? true : false %>' />
                                        <asp:Image ID="imgInf" runat="server" ImageUrl="~/Images/PNGs/inf.png"
                                            Visible=' <%# (((Message)Container.DataItem).MessageType == enmMessageType.Info) ? true : false %>' />
                                    </td>
                                    <td  >
                                        <%# Eval("MessageText")%>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
             </td></tr> <tr><td align=center>
            <div class="mpClose" style="text-align:center">
                <asp:Button ID="btnOK" runat="server" Text="OK" CausesValidation="false" Width="60px" />
                <asp:Button ID="btnPostOK" runat="server" Text="OK" CausesValidation="false" OnClick="btnPostOK_Click"
                    Visible="false" Width="60px" />
                <asp:Button ID="btnPostCancel" runat="server" Text="Cancel" CausesValidation="false"
                    OnClick="btnPostCancel_Click" Visible="false" Width="60px" />
                    <br /><br/>
            </div>
              </td></tr></table>
        </asp:Panel>
        <iws:ModalPopupExtender ID="mpeMsg" runat="server" TargetControlID="btnD"  
            PopupControlID="pnlMsg" PopupDragHandleControlID="pnlMsgHD" BackgroundCssClass="mpBg"
            DropShadow="true" OkControlID="btnOK">
        </iws:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>

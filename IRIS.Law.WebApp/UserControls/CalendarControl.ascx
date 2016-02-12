<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarControl.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.CalendarControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

        <asp:TextBox ID="_txtDate" AutoPostBack="true" runat="server" SkinID="Small" OnTextChanged="_txtDate_TextChanged"
            CausesValidation="false" onmousemove="showToolTip(event);return false;" onmouseout="hideToolTip();"></asp:TextBox>

<asp:ImageButton ID="_imgBtnDate" ImageUrl="~/images/PNGs/Calendar.png" SkinID="SearchImageIcon"
    runat="server" AlternateText="Select Date" CausesValidation="false" />
<ajaxToolkit:CalendarExtender ID="_ceDate" runat="server" PopupButtonID="_imgBtnDate"
    CssClass="AjaxCalendar" Format="dd/MM/yyyy" TargetControlID="_txtDate" Enabled="True">
</ajaxToolkit:CalendarExtender>
<ajaxToolkit:MaskedEditExtender ID="_meeDate" runat="server" TargetControlID="_txtDate"
    Mask="99/99/9999" MaskType="Date" CultureName="en-GB" CultureDateFormat="DMY"
    CultureDatePlaceholder="/" Century="2000" AutoComplete="False" Enabled="True" ErrorTooltipEnabled="True" MessageValidatorTip="true"
    OnInvalidCssClass="textBoxError">
</ajaxToolkit:MaskedEditExtender>
<ajaxToolkit:MaskedEditValidator ID="_mevDate" runat="server" ControlExtender="_meeDate"
    ControlToValidate="_txtDate" Display="None" MinimumValue="01/01/1753" MinimumValueMessage="Invalid Date">
</ajaxToolkit:MaskedEditValidator>
<div style="float: left;" id="_pnlValidation" runat="server" visible="false">
    <asp:RequiredFieldValidator ID="_rfvDate" runat="server" ErrorMessage="Date is mandatory"
        Display="None" ControlToValidate="_txtDate"></asp:RequiredFieldValidator>
</div>
    

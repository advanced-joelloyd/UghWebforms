<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeControl.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.TimeControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:TextBox ID="_txtTime" runat="server" Width="30px" onmousemove="showToolTip(event);return false;"
							onmouseout="hideToolTip();"  CausesValidation="false" />
<ajaxtoolkit:maskededitextender id="_meeTime" runat="server" targetcontrolid="_txtTime" 
    mask="99:99" messagevalidatortip="true" OnInvalidCssClass="textBoxError"
    masktype="Time" acceptampm="false" errortooltipenabled="True" />
    
<ajaxtoolkit:maskededitvalidator id="_mevTime" runat="server" controlextender="_meeTime" controltovalidate="_txtTime" 
    Display="None" />

<div style="float: left;" id="_pnlValidation" runat="server" visible="false">
	<asp:RequiredFieldValidator ID="_rfvTime" runat="server" ErrorMessage="Time is mandatory"
		Display="None" ControlToValidate="_txtTime"></asp:RequiredFieldValidator>
</div>
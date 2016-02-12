<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTimeEntryMobile.aspx.cs"
    Inherits="IRIS.Law.WebApp.Pages.Time.AddTimeEntryMobile" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="UserControl" TagName="MatterSearch" Src="~/UserControls/MatterSearchMobile.ascx" %>
<?xml version="1.0" encoding="utf-8" ?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML BASIC 1.1//EN" "http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=400" />
     <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
    <title>Add Time Entry</title>
    <link rel="Stylesheet" href="../CSS/Master.css" />
	<style type="text/css">
	    ._btnResetWidth
	    {
	        font-family: Arial, Helvetica, sans-serif;
	        font-size: 11px;
	        font-weight: bold;
	        width: 85px;
	    }
	    
	    ._btnSaveWidth
	    {
	        font-family: Arial, Helvetica, sans-serif;
	        font-size: 11px;
	        font-weight: bold;
	        width: 185px;
	    }
	    
	    ._plusminusButton
	    {
	        font-family: Arial, Helvetica, sans-serif;
	        font-size: 11px;
	        font-weight: bold;
	        width: 30px;
	        text-align:center;
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
    <ajaxToolkit:ToolkitScriptManager ID="_smTimeEntryMobile" runat="server" CombineScripts="false">
    </ajaxToolkit:ToolkitScriptManager>
    <asp:UpdateProgress ID="_updateProgressAddTime" runat="server">
        <ProgressTemplate>
            <div class="textBox"> 
                <img id="_imgLoading" src="~/Images/indicator.gif" runat="server" alt="Loading" />&nbsp;Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="_updPnlTime" runat="server">
        <ContentTemplate>
            <div style="padding-left: 5px; width: 320px;">
                <asp:Label ID="_lblMessage" runat="server"></asp:Label>
            </div>
            <UserControl:MatterSearch ID="_msAddTimeEntry" runat="server" OnMatterChanged="_msAddTimeEntry_MatterChanged" 
            OnError="_msAddTimeEntry_Error" OnSearchSuccessful="_msAddTimeEntry_SearchSuccessful"></UserControl:MatterSearch>
            <table width="320px">
                <tr>
                    <td class="boldTxtMobile" style="width: 100px;">
                        Time Type :
                    </td>
                    <td>
                        <asp:DropDownList ID="_ddlTimeType" runat="server" onmousemove="showToolTip(event);return false;"
                            onmouseout="hideToolTip();" Width="195px">
                        </asp:DropDownList>
                        <span class="mandatoryField">&nbsp;*</span>
                        <asp:RequiredFieldValidator ID="_rfvTimeType" runat="server" ErrorMessage="Time Type is mandatory"
                            Display="None" ControlToValidate="_ddlTimeType"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="boldTxtMobile" style="width:100px">
                        Units :
                    </td>
                    <td>
                        <asp:Button ID="_btnSubtractUnits" runat="server" EnableTheming="false" CausesValidation="false"
                            Text="-" OnClick="_btnSubtractUnits_Click" CssClass="_plusminusButton" />
                        <asp:TextBox ID="_txtUnits" runat="server" Text="1" onmousemove="showToolTip(event);return false;"
                            onmouseout="hideToolTip();" Style="margin: 0px 5px 0px 5px; width:106px" onkeypress="return CheckNumeric(event);"
                            MaxLength="9" onkeyup="CheckUnits(this)"></asp:TextBox>
                        <asp:Button ID="_btnAddUnits" runat="server" EnableTheming="false" CausesValidation="false"
                            Text="+" OnClick="_btnAddUnits_Click" CssClass="_plusminusButton" />
                        <asp:RequiredFieldValidator ID="_rfvUnits" runat="server" ErrorMessage="Units is mandatory"
                            Display="None" ControlToValidate="_txtUnits"></asp:RequiredFieldValidator>
                        <span class="mandatoryField">&nbsp;*</span>
                    </td>
                </tr>
            </table>
     </ContentTemplate>
    </asp:UpdatePanel>  
    <table width="100%">
        <tr>
            <td class="boldTxtMobile" valign="top" style="width: 100px;">
                Notes :
            </td>
            <td rowspan="2">
                <asp:TextBox ID="_txtNotes" TextMode="MultiLine" Rows="5" runat="server" Width="189px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td valign="bottom" style="width: 100px;">
               <asp:Button ID="_btnLogout" runat="server" EnableTheming="false" CausesValidation="false"
                    OnClick="_btnLogout_Click" Text="Logout" CssClass="_btnResetWidth" />
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="left" style="width:95px">
                <asp:Button ID="_btnReset" Text="Reset" 
                    class="_btnResetWidth" CausesValidation="false" OnClick="_btnReset_Click" runat="server" EnableTheming="false" />
            </td>
            <td align="left">
                <asp:Button ID="_btnSave" runat="server" EnableTheming="false" Text="Save" OnClick="_btnSave_Click"
                    CssClass="_btnSaveWidth" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="border-top: solid 1px #1e7b84;">
                <img src="../Images/logoSmall.png" alt="Logo" height="25px" width="50px" align="right" />
            </td>
        </tr>
    </table>
   
       
    <script src="../Javascript/jquery-1.3.2.js" type="text/javascript" defer="defer"></script>

    <script src="../Javascript/Validation.js" type="text/javascript" defer="defer"></script>

    <script src="../Javascript/Bubble-Tooltip.js" type="text/javascript" defer="defer"></script>

    <script type="text/javascript">
        function CancelPopupClick() {
            return false;
        }

        function CancelSearchClientPopup() {
            $find('_modalpopupClientSearchBehavior').hide();
        }

        function Reset() {
            document.getElementById("_txtName").value = "";
            if (document.getElementById("_grdClientSearch") != null) {
                document.getElementById("_grdClientSearch").style.display = "none";
            }
        }

        function ResetTime() {
            document.getElementById("_ddlTimeType").value = "3ef2937c-c31b-430c-82ed-5701a84f258e";
            document.getElementById("_txtUnits").value = "1";
            document.getElementById("_txtNotes").value = "";
            document.getElementById("_lblMessage").innerText = "";
            //return false;
        }

        
        //Check if the user has entered 0
        function CheckUnits(sender) {
            if (parseInt(sender.value) == 0) {
                sender.value = 1;
            }
        }
    </script>

    </form>
</body>
</html>

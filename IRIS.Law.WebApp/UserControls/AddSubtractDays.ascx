<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddSubtractDays.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.AddSubtractDays" %>
   <%-- 
<script language="javascript">

    function SubtractUnits(objUnits) {
        var units = document.getElementById(objUnits);
        var currentUnits = parseInt(units.value);
        if (isNaN(currentUnits)) {
            units.value = 1;
        }
        else if (currentUnits > 1) {
            units.value = currentUnits - 1;
        }

        //ChangeSettingsOnDays(state, units.value);
        //return false;
    }

    function AddUnits(objUnits) {
        var units = document.getElementById(objUnits);
        var currentUnits = parseInt(units.value);
        if (isNaN(currentUnits)) {
            units.value = 1;
        }
        else {
            if (currentUnits < 999) {
                units.value = currentUnits + 1;
            }
        }
        //ChangeSettingsOnDays(state, units.value);
        //return false;
    }

    //Check if the user has entered 0
    function CheckUnits(sender) {
        if (parseInt(sender.value) == 0) {
            sender.value = 1;
        }
    }
</script>--%>
<asp:Button ID="_btnSubtractNoOfDays" runat="server" EnableTheming="false" CausesValidation="false"
    Text="-" Width="15px" CssClass="buttonMobile" />
<asp:TextBox ID="_txtNoOfDays" runat="server" Text="1" onmousemove="showToolTip(event);return false;"
    onmouseout="hideToolTip();" Width="50px" onkeypress="return CheckNumeric(event);"
    onkeyup="CheckUnits(this)"></asp:TextBox>
<asp:Button ID="_btnAddNoOfDays" runat="server" EnableTheming="false" CausesValidation="false"
    Text="+" Width="15px" CssClass="buttonMobile" />

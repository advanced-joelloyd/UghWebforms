<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="lookupUserControl.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.lookupUserControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<script type="text/javascript">
    var previousRow;
    var SelectedRow = null;
    var SelectedRowIndex = null;
    var RowSelected = null;
    var clientid = null;

    function ChangeRowColor(gridview, row, lookup, btnSelect, AttributeName, PK) {
        SelectedRowIndex = row;
        RowSelected = row;
        var presentRow = document.getElementById(gridview).rows[row];
        if (previousRow != null) {
            previousRow.style.backgroundColor = 'White';
        }
        // change the selected row color and set the previous row
        presentRow.style.backgroundColor = "skyblue";

        document.getElementById(lookup).value = $(presentRow).find("td:first").html();
        previousRow = presentRow;
        SelectedRow = presentRow;

        var hdnSelectedRowIndex = document.getElementById('<% =hdnSelectedRowIndex.ClientID %>');
        hdnSelectedRowIndex.value = row;

        // Add onclick attribute to Select button depending on row selected
        var buttonValue = document.getElementById(btnSelect);
        var postbackValue = "__doPostBack('myDblClick', '" + PK + "~" + row + "~" + AttributeName + "')";
        buttonValue.setAttribute('onclick', postbackValue);

        // Add focus to the button so the Enter key will click the button
        buttonValue.focus();
    }

    function GetKeyPress1(e, hiddencontrol) {
        // debugger;
        var e = e ? e : window.event;
        var KeyCode = e.which ? e.which : e.keyCode;
        if (KeyCode == 9) {
            document.getElementById(hiddencontrol).value = "true";
        }
    }

    function ClearSelectedRowIndex(e, gridview, control) {
        // debugger;
        var hdnLookupID = document.getElementById(control);
        hdnLookupID.value = '';
        if (SelectedRowIndex != null) {
            if (previousRow != null) {
                previousRow.style.backgroundColor = 'White';
                RowSelected = null;
            }
            return false;
        }
        else {
            return false;
        }
    }

    // The following methods are required for the filtering functionality
    Sys.Application.add_load(page_load);
    Sys.Application.add_unload(page_unload);

    function page_load() {
        $addHandler($get('<% =txtRef.ClientID %>'), 'keydown', onFilterTextChangedRef);
        $addHandler($get('<% =txtDescription.ClientID %>'), 'keydown', onFilterTextChangedDescription);
    }

    function page_unload() {
        $removeHandler($get('<% =txtRef.ClientID %>'), 'keydown', onFilterTextChangedRef);
        $removeHandler($get('<% =txtDescription.ClientID %>'), 'keydown', onFilterTextChangedDescription);
    }

    var timeoutID = 0;

    function onFilterTextChangedRef(e) {

        // Clear any delays
        if (timeoutID) {
            window.clearTimeout(timeoutID);
        }

        // Executes a code snippet or a function after specified delay
        timeoutID = window.setTimeout(updateFilterTextRef, 1000);
    }

    function onFilterTextChangedDescription(e) {

        // Clear any delays
        if (timeoutID) {
            window.clearTimeout(timeoutID);
        }

        // Wait for a second then query data
        timeoutID = window.setTimeout(updateFilterTextDescription, 1000);
    }

    function updateFilterTextRef() {
        __doPostBack('<% =txtRef.ClientID %>', '');
    }

    function updateFilterTextDescription() {
        __doPostBack('<% =txtDescription.ClientID %>', '');
    }

    $(function () {

        $(document).keydown(function (event) {
            var key = event.keyCode;

            if (key == 27) {

                var mpeId = "<%= _modalpopupLookupSearch.ClientID %>"
                $find(mpeId).hide();
            }
        });
    });

</script>
<table style="float: left; position: absolute; width: <%=GetControlWidth().ToString()%>;"
    cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <table cellspacing="0" cellpadding="0" style="width: <%=GetControlWidth().ToString()%>;
                height: 100%;">
                <tr>
                    <!-- ROW -->
                    <td>
                        <asp:UpdatePanel ID="_updClientRef" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField ID="_hdnLookupRef" runat="server" />
                                <asp:HiddenField ID="_hdnLookupName" runat="server" />
                                <asp:HiddenField ID="_hdnLookupValue" runat="server" />
                                <asp:HiddenField ID="_hdnLookupId" runat="server" />
                                <asp:HiddenField ID="_hdnLookupTitle" runat="server" />
                                <asp:HiddenField ID="_hiddenShowpopUP" runat="server" Value="false" />
                                <asp:HiddenField ID="_hdnGridRowCount" runat="server" />
                                <asp:HiddenField ID="hdnSelectedRowIndex" runat="server" />
                                <asp:TextBox ID="_txtLookupReference" runat="server" Height="16px"></asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <asp:ImageButton AlternateText="Lookup Search" ID="_imgTextControl" runat="server"
                            CssClass="mpeTarget" ToolTip="Lookup Search" SkinID="SearchImageIcon" Height="21px"
                            CausesValidation="false" ImageUrl="~/Images/PNGs/search-btn.png" OnClick="_imgTextControl_Click" />
                        <%--  <asp:Button ID="btnDummyButton" runat="server" Style="display:none;" />--%>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField runat="server" ID="hdnModalTarget" />
<ajaxToolkit:ModalPopupExtender ID="_modalpopupLookupSearch" runat="server" BackgroundCssClass="modalBackground"
    PopupControlID="_LookupSearchContainer" TargetControlID="hdnModalTarget">
</ajaxToolkit:ModalPopupExtender>
<asp:Panel ID="_LookupSearchContainer" runat="server" Width="100%" Height="50%">
    <style type="text/css">
        .hideMe
        {
            display: none;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <center>
                <table border="2px" style="background-color: #F8F8F8; border-style: double; border-color: Black"
                    cellpadding="15">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" width="400px" style="float: left; padding-left: 10px;
                                padding-bottom: 15px; padding-right: 10px">
                                <tr>
                                    <td colspan="3" align="left">
                                        <asp:Label ID="lblLookupWindowTitle" runat="server" ForeColor="#6E8DBE" Font-Bold="true"
                                            Font-Size="Medium" Text="Window Title"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <hr style="color: #6E8DBE; background-color: #6E8DBE; height: 1px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:TextBox ID="txtRef" runat="server" Width="60px" AutoPostBack="false" OnTextChanged="FilterTextChanged"
                                            TabIndex="1"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="100" Width="180px" AutoPostBack="false"
                                            OnTextChanged="FilterTextChanged" TabIndex="2"></asp:TextBox>
                                        <input id="Focus" type="hidden" />
                                    </td>
                                    <td>
                                        <asp:Button ID="_btnSelect" runat="server" Text="Select" Font-Bold="true" Width="60px"
                                            TabIndex="4" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div id="xxx" style="overflow: auto; height: 200px; background-color: White">
                                            <asp:GridView ID="gvLookup" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                GridLines="None" Width="100%" TabIndex="3" OnRowDataBound="gvLookup_RowDataBound"
                                                ShowHeader="False">
                                                <Columns>
                                                    <asp:BoundField ItemStyle-CssClass="hideMe" ItemStyle-Width="1%" />
                                                    <asp:BoundField ItemStyle-Width="34%" ItemStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField ItemStyle-Width="65%" ItemStyle-HorizontalAlign="Left" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td valign="top">
                                        <asp:Button ID="_btnCancel" runat="server" Text="Cancel" BackColor="" Font-Bold="true"
                                            Width="60px" TabIndex="5" OnClick="_btnCancel_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </center>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganiseUsers.ascx.cs"
    Inherits="IRIS.Law.WebApp.UserControls.OrganiseUsers" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<script language="javascript" type="text/javascript">

    function CancelOrgUsersPopupClick() {
        return false;
    }

    function CancelOrgUsersPopup() {
        $find('_modalpopupOrganiseUsersBehavior').hide();
    }

    Sys.Application.add_load(createListObjects);

    var selectedList;
    var availableList;
    var currentAttendees = "<%=CurrentUsersID%>";
    function createListObjects() {
        availableList = document.getElementById("<%=_listAvailableUsers.ClientID%>");
        selectedList = document.getElementById("<%=_listCurrentUsers.ClientID%>");
    }

    function delAttribute() {
        var selIndex = selectedList.selectedIndex;
        if (selIndex < 0)
            return;
        availableList.appendChild(
      selectedList.options.item(selIndex))
        selectNone(selectedList, availableList);
        setSize(availableList, selectedList);
        //sortlist(availableList);

        var _hiddenSingleOrMultiple = document.getElementById("<%=_hiddenSingleOrMultiple.ClientID%>");
        if (_hiddenSingleOrMultiple.value == "single") {
            var btn = document.getElementById("<%=_btnAddSingle.ClientID%>");
            btn.disabled = false;
        }

    }
    function addAttribute() {
        var addIndex = availableList.selectedIndex;
        if (addIndex < 0)
            return;
        selectedList.appendChild(
      availableList.options.item(addIndex));
        selectNone(selectedList, availableList);
        setSize(selectedList, availableList);
        sortlist(selectedList);
        var _hiddenSingleOrMultiple = document.getElementById("<%=_hiddenSingleOrMultiple.ClientID%>");
        if (_hiddenSingleOrMultiple.value == "single") {
            if (availableList.options.length > 0)
            {
                var btn = document.getElementById("<%=_btnAddSingle.ClientID%>");
                btn.disabled = true;
            }
        }
    }

    function setSize(list1, list2) {
        var browser = navigator.appName;
        if (browser == "Microsoft Internet Explorer") {
            list1.size = getSize(list1);
            list2.size = getSize(list2);
        }
    }

    function selectNone(list1, list2) {
        list1.selectedIndex = -1;
        list2.selectedIndex = -1;
        addIndex = -1;
        selIndex = -1;
    }

    function getSize(list) {
        /* Mozilla ignores whitespace, 
        IE doesn't - count the elements 
        in the list */
        var len = list.childNodes.length;
        var nsLen = 0;
        //nodeType returns 1 for elements
        for (i = 0; i < len; i++) {
            if (list.childNodes.item(i).nodeType == 1)
                nsLen++;
        }
        if (nsLen < 2)
            return 2;
        else
            return nsLen;
    }

    function delAll() {
        var len = selectedList.length - 1;
        for (i = len; i >= 0; i--) {
            availableList.appendChild(selectedList.item(i));
        }
        selectNone(selectedList, availableList);
        setSize(selectedList, availableList);
        //sortlist(availableList);
    }

    function addAll() {
        var len = availableList.length - 1;
        for (i = len; i >= 0; i--) {
            selectedList.appendChild(availableList.item(i));
        }
        selectNone(selectedList, availableList);
        setSize(selectedList, availableList);
        sortlist(selectedList);
    }

    function CheckCurrentUsersList() {
        var _hiddenCurrentUsers = document.getElementById("<%=_hiddenCurrentUsers.ClientID%>");
        var _hiddenCurrentUsersID = document.getElementById("<%=_hiddenCurrentUsersID.ClientID%>");
        _hiddenCurrentUsers.value = "";
        _hiddenCurrentUsersID.value = "";
        for (i = 0; i < selectedList.childNodes.length; i++) {
            if (selectedList.childNodes.item(i).nodeType == 1) {
                _hiddenCurrentUsers.value += selectedList.childNodes.item(i).text;
                _hiddenCurrentUsersID.value += selectedList.childNodes.item(i).value;
                var _hiddenSingleOrMultiple = document.getElementById("<%=_hiddenSingleOrMultiple.ClientID%>");
                if (_hiddenSingleOrMultiple.value != "single") {
                    _hiddenCurrentUsers.value += "; ";
                    _hiddenCurrentUsersID.value += "; ";
                }
            }
        }
        if (_hiddenCurrentUsers.value.length == 0) {
            $("#<%=_lblError.ClientID %>").text("You must have at least one Member in your list");
            return false;
        }
        else {
            currentAttendees = _hiddenCurrentUsersID.value;
            return true;
        }
    }

    function sortlist(lb) {
        //var lb = document.getElementById('mylist');
        var temp1 = lb;
        arrTexts = new Array();
        arrValues = new Array();

        for (i = 0; i < lb.length; i++) {
            arrTexts[i] = lb.options[i].text;
            arrValues[i] = lb.options[i].value;
        }

        var arrTextSorted = new Array();
        arrTextSorted = arrTexts.sort();
        //debugger;
        for (i = 0; i < lb.length; i++) {
            lb.options[i].text = arrTextSorted[i];
            for (j = 0; j < temp1.length; j++) {
                if (arrTextSorted[i].match(temp1.options[j].text)) {
                    lb.options[i].value = temp1.options[j].value;
                    break;
                }
            }
        }
    }


//    function sortlist(lb) {
//        //var lb = document.getElementById('mylist');
//        arrTexts = new Array();

//        for (i = 0; i < lb.length; i++) {
//            arrTexts[i] = lb.options[i].text;
//        }

//        arrTexts.sort();

//        for (i = 0; i < lb.length; i++) {
//            lb.options[i].text = arrTexts[i];
//            lb.options[i].value = arrTexts[i];
//        }
//    }


    function BindCurrentAttendees() {
        $("#<%=_lblError.ClientID %>").text("");
        delAll();
        if (currentAttendees.length > 0) {
            var currentAttendees_array = currentAttendees.split(";");
            var i;
            for (i = 0; i < currentAttendees_array.length; i++) {
                var attendee = currentAttendees_array[i].trim();
                var listAvailable = document.getElementById("<%=_listAvailableUsers.ClientID%>");
                var len = listAvailable.childNodes.length;

                //nodeType returns 1 for elements
                for (j = 0; j < len; j++) {
                    if (listAvailable.childNodes.item(j).nodeType == 1) {
                        if (listAvailable.childNodes.item(j).value.toUpperCase() == attendee.toUpperCase()) {
                            listAvailable.childNodes.item(j).selected = true;
                        }
                    }
                }

                addAttribute();
            }
        }
    }

</script>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <div class="button" style="width: 80px; float: right; text-align: center;" id="_divAttendees"
                title="Attendees" runat="server">
                <asp:Button ID="_btnAttendees" runat="server" CausesValidation="False"
                    OnClientClick="return BindCurrentAttendees();" />
            </div>
            <ajaxToolkit:ModalPopupExtender ID="_modalpopupAttendees" runat="server" BackgroundCssClass="modalBackground"
                DropShadow="true" PopupControlID="_pnlOrganiseUsers" OnCancelScript="javascript:CancelOrgUsersPopupClick();"
                TargetControlID="_btnAttendees" CancelControlID="_btnCancel" BehaviorID="_modalpopupOrganiseUsersBehavior">
            </ajaxToolkit:ModalPopupExtender>
        </td>
    </tr>
</table>
<asp:Panel ID="_pnlOrganiseUsers" HorizontalAlign="Center" runat="server" Style="background-color: #ffffff"
    Width="500px" Height="475px">
    <table width="99%;" style="height: 70%" border="0" class="panel" id="_tableAvailableUsers"
        runat="server">
        <tr>
            <td colspan="3">
                <table width="100%" id="_tableHeader">
                    <tr id="_trCloseLink" runat="server">
                        <td align="right">
                            <a id="linkClose" onclick="CancelOrgUsersPopup();" class="link" href="#">Close</a>&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:UpdatePanel ID="_updPnlError" runat="server">
                                <ContentTemplate>
                                    <asp:Label ID="_lblError" CssClass="errorMessage" runat="server" Text=""></asp:Label>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="_btnSave" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="sectionHeader" align="left" runat="server" id="_tdHeader">
                            Organise Users
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="boldTxt" align="left" style="text-indent:0px">
                Available Users
            </td>
            <td>
                &nbsp;
            </td>
            <td class="boldTxt" align="left" style="text-indent:0px">
                Current Users
            </td>
        </tr>
        <tr>
            <td style="width: 40%; height: 100%" valign="top" align="center">
                <asp:ListBox Width="100%" Height="100%" ID="_listAvailableUsers" runat="server" CssClass="textBox">
                </asp:ListBox>
            </td>
            <td style="width: 20%; height: 100%" valign="top" align="center">
                <table width="100%" style="height: 100%">
                    <tr>
                        <td valign="top">
                            <div class="button" style="text-align: center;" id="_divAddSingle" title="Add User"
                                runat="server">
                                <asp:Button ID="_btnAddSingle" OnClientClick="addAttribute();return false;" CausesValidation="false"
                                    ToolTip="Add" Text="Add >>" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <div class="button" style="text-align: center;" id="_divRemoveSingle" title="Add User"
                                runat="server">
                                <asp:Button ID="_btnRemoveSingle" OnClientClick="delAttribute();return false;" CausesValidation="false"
                                    ToolTip="Remove User" Text="<< Remove" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 30%">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom">
                            <div class="button" style="text-align: center;" id="_divAddAll" title="Add User"
                                runat="server">
                                <asp:Button ID="_btnAddAll" OnClientClick="addAll();return false;" Text="Add All"
                                    runat="server" CausesValidation="false" ToolTip="Add All Users" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom">
                            <div class="button" style="text-align: center;" id="_divRemoveAll" title="Add User"
                                runat="server">
                                <asp:Button ID="_btnRemoveAll" OnClientClick="delAll();return false;" Text="Remove All"
                                    runat="server" CausesValidation="false" ToolTip="Remove All Users" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 40%; height: 100%" valign="top" align="center">
                <asp:ListBox Width="100%" Height="100%" ID="_listCurrentUsers" runat="server" CssClass="textBox">
                </asp:ListBox>
                <asp:HiddenField ID="_hiddenCurrentUsers" runat="server" />
                <asp:HiddenField ID="_hiddenCurrentUsersID" runat="server" />
                <asp:HiddenField ID="_hiddenSingleOrMultiple" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <table id="_tableButtons" width="100%">
        <tr>
            <td align="right" style="padding-right: 15px;">
                <table>
                    <tr>
                        <td align="right">
                            <div id="_divCancel" runat="server" class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
                            </div>
                        </td>
                        <td align="right">
                            <div class="button" style="float: right; text-align: center;">
                                <asp:Button ID="_btnSave" runat="server" Text="Save" CausesValidation="false" OnClientClick="return CheckCurrentUsersList();"
                                    OnClick="_btnSave_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

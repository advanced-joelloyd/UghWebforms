Sys.Application.add_load(IPhoneCheck);
//Rounded corner buttons not displaying correctly on the iphone
//so remove the css class and let the browser display its default buttons
function IPhoneCheck() {
    //iphone user coming to the site
    var agent = navigator.userAgent.toLowerCase();
    var is_iphone = (agent.indexOf('iphone') != -1);
    if (is_iphone) {
        //Dont apply rounded corners to the buttons
        $(".button").removeClass("button");
    }
}

//Converts the date from dd/MM/yyyy format to yyyy-MM-dd format
function ConvertDate(ddMMyyyy) {
    if (ddMMyyyy.length == 0) {
        ddMMyyyy = "01/01/1753";
    }
    var date = ddMMyyyy.split('/');
    return (date[2] + "-" + date[1] + "-" + date[0]);
}

function uppercase(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
        if ((key > 0x60) && (key < 0x7B)) {
            window.event.keyCode = key - 0x20;
        }
    }
}

/*********************/
// Used in User Control AddSubtractDays.ascx
/*********************/
function SubtractUnits(objUnits) {
    var units = document.getElementById(objUnits);
    var currentUnits = parseInt(units.value);
    if (isNaN(currentUnits)) {
        units.value = 1;
    }
    else if (currentUnits > 1) {
        units.value = currentUnits - 1;
    }
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
}

//Check if the user has entered 0
function CheckUnits(sender) {
    if (parseInt(sender.value) == 0) {
        sender.value = 1;
    }
}

//Disables the child controls within a tab based on permissions
function DisableTabs(tabIds) {
    var tabs = tabIds.split(",");
    for (var i = 0; i < tabs.length; i++) {
        //Disable textboxes
        var domArr = $get(tabs[i]).getElementsByTagName('input');
        DisableItem(domArr);

        //Disable dropdowns
        domArr = $get(tabs[i]).getElementsByTagName('select');
        //ReadyOnlyItem(domArr);
        DisableItem(domArr);

        //Disable textarea
        var domArr = $get(tabs[i]).getElementsByTagName('textarea');
        DisableItem(domArr);

      
    }
}

function DisableItemButton(itemArray) {
    for (var i = 0; i < itemArray.length; i++) {
        itemArray[i].disabled = true;
    }
}

function DisableItem(itemArray) {
    for (var i = 0; i < itemArray.length; i++) {

        if (itemArray[i].type == "checkbox" || itemArray[i].type == "radio" || itemArray[i].type == "button" || itemArray[i].type == "submit" || itemArray[i].type == "image") {
            itemArray[i].disabled = true;
        }
        else {
            
                itemArray[i].disabled = false;
                itemArray[i].setAttribute('readOnly', 'readonly');
            
        }
        
    
    }
}

function ReadyOnlyItem(itemArray) {
    for (var i = 0; i < itemArray.length; i++) {
        if (itemArray[i].getAttribute("id") != "ctl00__cphMain__tcEditClient__pnlAddressDetails__ddlAddressType") {
            itemArray[i].setAttribute('readOnly', 'readonly');

           if (itemArray[i].length > 0) {

                var dropdownValue = itemArray[i].options[itemArray[i].selectedIndex].text;

                itemArray[i].options.length = 1;

                var optn = document.createElement("OPTION");
                optn.text = dropdownValue;
                optn.value = "";
                itemArray[i].options.add(optn);

                itemArray[i].remove(0);
            }
            
            
            
        }
    }
}
/*********************/
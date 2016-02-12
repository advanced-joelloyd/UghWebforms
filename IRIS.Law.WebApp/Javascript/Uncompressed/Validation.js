/*Common validations functions*/

function ValidatorUpdateDisplay(val) {
    var controlWidth = $("#" + val.controltovalidate).css("width");
    if (val.isvalid) {
        //if (val.id == $("#" + val.controltovalidate).attr("ErrorProvider")) {
        $("#" + val.controltovalidate).removeClass("textBoxError");
        $("#" + val.controltovalidate).addClass("textBox");
        $("#" + val.controltovalidate).attr("ErrorMessage", null);
        //}
        if (val.id.indexOf("_rfvDate") > 0) {
            if (document.getElementById(val.controltovalidate).value != "") {

                var ctrlDate = new Date(document.getElementById(val.controltovalidate).value);
                var compare = new Date("01/01/1753");

                if (ctrlDate < compare) {
                    $("#" + val.controltovalidate).removeClass("textBox");
                    $("#" + val.controltovalidate).addClass("textBoxError");
                }

            }
        }
    }
    else {
        $("#" + val.controltovalidate).removeClass("textBox");
        $("#" + val.controltovalidate).addClass("textBoxError");
        //set the error message to the controls ErrorMessage attribute
        if (val.errormessage != undefined && val.errormessage != null) {
            var enableValidation = true;
            if (val.id.indexOf("_mevTime") > 0 || val.id.indexOf("_mevDate") > 0) {
                enableValidation = false;
            }
            if (enableValidation) {
                $("#" + val.controltovalidate).attr("ErrorMessage", val.errormessage);
                //$("#" + val.controltovalidate).attr("ErrorProvider", val.id);
            }
        }
    }

    $("#" + val.controltovalidate).css("width", controlWidth);

    if (typeof (val.display) == "string") {
        if (val.display == "None") {
            return;
        }

        if (val.display == "Dynamic") {
            val.style.display = val.isvalid ? "none" : "inline";
            return;
        }
    }

    if ((navigator.userAgent.indexOf("Mac") > -1) && (navigator.userAgent.indexOf("MSIE") > -1)) {
        val.style.display = "inline";
    }
    val.style.visibility = val.isvalid ? "hidden" : "visible";
}

///<CommentsBlock>
///<MethodName>doDecimalTextbox</MethodName>
///<returns>void</returns> 
///<Description>
///Method is used to Accept numbers with one decimal point and upto 2 decimal places only
///</Description>
///</CommentsBlock>
function decimalTextbox(textBox, e) {
    if (e.keyCode == 46 || e.keyCode == 8 || e.keyCode == 37 || e.keyCode == 39) {
        return;
    }

    var textHandle = textBox.value;

    if (isNaN(textHandle)) {
        textBox.value = '0.00';
    }
    else {
        if (textHandle.indexOf(".") != -1) {
            var prfx = textHandle.substring(0, textHandle.indexOf("."));
            if (prfx.length > 9) {
                prfx = prfx.substring(0, 9);
            }

            var sfx = textHandle.substring((textHandle.indexOf(".") + 1), textHandle.length);
            if (sfx.length > 2) {
                sfx = sfx.substring(0, 2);
            }
            textBox.value = prfx + "." + sfx;
        }
    }
}

function CheckNumeric(event) {
    var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;

    var charStr = String.fromCharCode(keyCode);

    // Allow only backspace and delete
    if (keyCode == 8) {
        // let it happen, don't do anything
    }
    else {
        // Ensure that it is a number and stop the keypress
        if (keyCode < 48 || keyCode > 57) {
            if (charStr != "-") {

                return false;
            }
        }
    }
}


/*
* Allows only valid characters to be entered into input boxes.
*
* @name     numeric
* @param    decimal      Decimal separator (e.g. '.' or ',' - default is '.')
* @param    places       The number of decimal places

* @example  $(".numeric").numeric();
* @example  $(".numeric").numeric(",");
* @example  $(".numeric").numeric(null, places);
*
*/
jQuery.fn.numeric = function(decimal, places, allowNegative) {
    decimal = decimal || ".";
    places = typeof places == "number" ? places : 2;
    allowNegative = allowNegative == null ? true : allowNegative;

    this.keypress(
		function(e) {
		    var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
		    // allow enter/return key (only when in an input box)
		    if (key == 13 && this.nodeName.toLowerCase() == "input") {
		        return true;
		    }
		    else if (key == 13) {
		        return false;
		    }
		    var allow = false;
		    // allow Ctrl+A
		    if ((e.ctrlKey && key == 97 /* firefox */) || (e.ctrlKey && key == 65) /* opera */) return true;
		    // allow Ctrl+X (cut)
		    if ((e.ctrlKey && key == 120 /* firefox */) || (e.ctrlKey && key == 88) /* opera */) return true;
		    // allow Ctrl+C (copy)
		    if ((e.ctrlKey && key == 99 /* firefox */) || (e.ctrlKey && key == 67) /* opera */) return true;
		    // allow Ctrl+Z (undo)
		    if ((e.ctrlKey && key == 122 /* firefox */) || (e.ctrlKey && key == 90) /* opera */) return true;
		    // allow or deny Ctrl+V (paste), Shift+Ins
		    if ((e.ctrlKey && key == 118 /* firefox */) || (e.ctrlKey && key == 86) /* opera */
			|| (e.shiftKey && key == 45)) return true;
		    // if a number was not pressed
		    if (key < 48 || key > 57) {

		        /* '-' only allowed at start */
		        if (key == 45 && this.value.length == 0 && allowNegative) {
		            return true;
		        }

		        /* only one decimal separator allowed */
		        if (key == decimal.charCodeAt(0) && this.value.indexOf(decimal) != -1) {
		            allow = false;
		        }
		        // check for other keys that have special purposes
		        if (
					key != 8 /* backspace */ &&
					key != 9 /* tab */ &&
					key != 13 /* enter */ &&
					key != 35 /* end */ &&
					key != 36 /* home */ &&
					key != 37 /* left */ &&
					key != 39 /* right */ &&
					key != 46 /* del */
				) {
		            allow = false;
		        }
		        else {
		            // for detecting special keys (listed above)
		            // IE does not support 'charCode' and ignores them in keypress anyway
		            if (typeof e.charCode != "undefined") {
		                // special keys have 'keyCode' and 'which' the same (e.g. backspace)
		                if (e.keyCode == e.which && e.which != 0) {
		                    allow = true;
		                }
		                // or keyCode != 0 and 'charCode'/'which' = 0
		                else if (e.keyCode != 0 && e.charCode == 0 && e.which == 0) {
		                    allow = true;
		                }
		            }
		        }
		        // if key pressed is the decimal and it is not already in the field
		        if (key == decimal.charCodeAt(0) && this.value.indexOf(decimal) == -1) {
		            allow = true;
		        }
		    }
		    else {
		        allow = true;
		    }
		    return allow;
		}
	)
	.blur(
		function() {
		    //Format the final input
		    var val = jQuery(this).val();
		    val = parseFloat(val);
		    if (isNaN(val)) {
		        var num = 0;
		        jQuery(this).val(num.toFixed(places));
		    }
		    else {
		        jQuery(this).val(val.toFixed(places));
		    }
		}
	)
    return this;
}

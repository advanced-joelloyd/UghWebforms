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

$(document).ready(function() {
    // Add the page method call as an onclick handler for regenerate password
    $("#" + btnPassword).click(function() {
        $.ajax({
            type: "POST",
            url: "EditClient.aspx/ReGeneratePassword",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(msg) {
                $("#" + txtPassword).val(msg.d);
            }
        });
    });

    // Add the page method call as an onchange handler to calculate age
    $("#" + txtDOB).change(CalculateAge);
    $("#" + txtDOD).change(CalculateAge);

    //Construct UCN
    $("#" + txtSurname).change(ConstructUCN);
    $("#" + txtForename).change(ConstructUCN);
    $("#" + txtDOB).change(ConstructUCN);

    //function to enable/disable armed forces textbox based on selection
    $("#" + chkArmedForces).click(
		  function() {
		      $("#" + txtArmedForcesNo).attr('readonly', !this.checked);
		      if (this.checked) {
		          $("#" + txtArmedForcesNo).removeClass("readonly");
		      }
		      else {
		          $("#" + txtArmedForcesNo).addClass("readonly");
		          $("#" + txtArmedForcesNo).val("");
		      }
		  });

    //HOUCN length validation
    $("#" + txtHOUCN).change(
		  function() {
		      var houcn = $("#" + txtHOUCN).val().replace(/_/g, "");
		      if (houcn.length != 8) {
		          $("#" + updPnlMessage).css("display", "block");
		          $("#" + lblMessage).removeClass("successMessage");
		          $("#" + lblMessage).addClass("errorMessage");
		          $("#" + lblMessage).text("All characters have not been entered for the HO UCN");
		      }
		      else {
		          $("#" + lblMessage).text("");
		      }
		  });

    $('[readonly]').addClass("readonly");
});

function ConstructUCN() {
    var dateOfBirth = $("#" + txtDOB).val();
    dateOfBirth = ConvertDate(dateOfBirth);
    var forename = $("#" + txtForename).val();
    var surname = $("#" + txtSurname).val();
    $.ajax({
        type: "POST",
        url: "EditClient.aspx/ConstructUCN",
        data: "{'dateOfBirth' : '" + dateOfBirth + "','forename': '" + forename + "','surname': '" + surname + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(msg) {
            $("#" + txtUCN).val(msg.d);
        },
        error: function(msg) {
            //Error handling
        }
    });
}

//Calculate the age based on the dob/dod
function CalculateAge() {
    var dob = $("#" + txtDOB).val();
    var dod = $("#" + txtDOD).val();

    if (dob.length > 0 && dod.length > 0) {
        //Check if the dod > dob
        if (!CheckDOD(dob, dod)) {
            //Invalid value entered
            $("#" + txtDOD).val("");
            dod = "";
        }
    }

    dob = ConvertDate(dob);
    dod = ConvertDate(dod);

    $.ajax({
        type: "POST",
        url: "EditClient.aspx/CalculateAge",
        data: "{'dob': '" + dob + "','dod': '" + dod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(msg) {
            $("#" + txtAge).val(msg.d);
        }
    });
}

//Checks if the dod entered is greater than the dob
function CheckDOD(dob, dod) {
    var birthDate = dob.split('/');
    var deathDate = dod.split('/');
    var todayDate = new Date();
    dob = new Date(birthDate[2] + "/" + birthDate[1] + "/" + birthDate[0]);
    dod = new Date(deathDate[2] + "/" + deathDate[1] + "/" + deathDate[0]);

    var returnRes;

    returnRes = (dod - dob) >= 0;

    if (!(todayDate - dod) >= 0)
        returnRes = (todayDate - dod) >= 0

    return (returnRes);
}

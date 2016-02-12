var browser = navigator.appName;

function RoundedCorners() {
    Nifty("span.ajax__tab_tab", "small transparent top");
    Nifty("div.button");
}

// Toggle a field's read/write status based upon the value of a checkbox
function CheckboxToggleField(chk, field) {
    var checked = chk.attr("checked");
    
    field.attr("readonly", !checked);
    if (checked) {
        field.removeClass("readonly");
    } else {
        field.addClass("readonly");
        field.val("");
    }
}

if (browser == "Microsoft Internet Explorer") {
    Sys.Application.add_load(RoundedCorners);
}

$(document).ready(function () {
    $("#" + btnPassword).click(function () {
        $.ajax({
            type: "POST",
            url: "EditClient.aspx/ReGeneratePassword",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (a) {
                $("#" + txtPassword).val(a.d);
            }
        });
    });
    $("#" + txtDOB).change(CalculateAge);
    $("#" + txtDOD).change(CalculateAge);
    $("#" + txtSurname).change(ConstructUCN);
    $("#" + txtForename).change(ConstructUCN);
    $("#" + txtDOB).change(ConstructUCN);

    // Re-evaluate the read/write status of the Armed Forces textbox based upon its checkbox
    CheckboxToggleField($("#" + chkArmedForces), $("#" + txtArmedForcesNo));
    // And ensure this happens when the checkbox is clicked, too
    $("#" + chkArmedForces).click(function () {
        CheckboxToggleField($("#" + chkArmedForces), $("#" + txtArmedForcesNo));
    });

    $("#" + txtHOUCN).change(function () {
        var a = $("#" + txtHOUCN).val().replace(/_/g, "");
        if (a.length != 8) {
            $("#" + updPnlMessage).css("display", "block");
            $("#" + lblMessage).removeClass("successMessage");
            $("#" + lblMessage).addClass("errorMessage");
            $("#" + lblMessage).text("All characters have not been entered for the HO UCN");
        } else $("#" + lblMessage).text("");
    });
    $("[readonly]").addClass("readonly");
});

function ConstructUCN() {
    var a = $("#" + txtDOB).val();
    a = ConvertDate(a);
    var b = $("#" + txtForename).val(),
        c = $("#" + txtSurname).val();
    $.ajax({
        type: "POST",
        url: "EditClient.aspx/ConstructUCN",
        data: "{'dateOfBirth' : '" + a + "','forename': '" + b + "','surname': '" + c + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (a) {
            $("#" + txtUCN).val(a.d);
        },
        error: function () { }
    });
}

function CalculateAge() {
    var b = $("#" + txtDOB).val(),
        a = $("#" + txtDOD).val();
    if (b.length > 0 && a.length > 0) if (!CheckDOD(b, a)) {
        $("#" + txtDOD).val("");
        a = "";
    }

    b = ConvertDate(b);
    a = ConvertDate(a);
    $.ajax({
        type: "POST",
        url: "EditClient.aspx/CalculateAge",
        data: "{'dob': '" + b + "','dod': '" + a + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (a) {
            $("#" + txtAge).val(a.d);
        }
    });
}

function CheckDOD(e, a) {
    var b = e.split("/"),
        c = a.split("/"),
        f = new Date;
    e = new Date(b[2] + "/" + b[1] + "/" + b[0]);
    a = new Date(c[2] + "/" + c[1] + "/" + c[0]);
    var d;
    d = a - e >= 0;
    if (!(f - a) >= 0) d = f - a >= 0;
    return d;
}
function doEditMatterPrint() {

    var containerID = 0;
    var containerID1 = 0;
    var containerID2 = 0;
    var containerID3 = 0;
    var containerID4 = 0;

    var tabContainerClientID = "<%=_tcEditMatter.ClientID %>";

    //containerID is global
    containerID = "ctl00__cphMain__tcEditMatter__pnlDetails";
    containerID1 = "ctl00__cphMain__tcEditMatter__pnlAdditionalInfo";
    containerID2 = "ctl00__cphMain__tcEditMatter__pnlPublicFunding";

    if (containerID == 0 || containerID == "") {
        //alert("Sorry, it is unavailable to print because it's made by dan foster.");
        document.location = "editmatter.aspx?PrintPage=true";
        return;
    }

    // Check if public fundiing should be added to the report
    var PublicFundingInnerHTML = "";

    try {

        if (document.getElementById("ctl00__cphMain__tcEditMatter__pnlPublicFunding__chkPublicFunded")) {

            var IsChecked = document.getElementById("ctl00__cphMain__tcEditMatter__pnlPublicFunding__chkPublicFunded").checked;

            if (IsChecked) { PublicFundingInnerHTML = "<br /><br />" + document.getElementById(containerID2).innerHTML; ; }
        }

        var AdditionalInfo = "";

        if (document.getElementById(containerID1) != null) { AdditionalInfo = "<br /><br />" + document.getElementById(containerID1).innerHTML; }

        var TopBar = '<table cellspacing="4" cellpadding="2" width="100%" class="HeaderTab" style="border-bottom:White"><tr><td>&nbsp;</td><td style="width:60px"><input ID="PRINT" type="button" value="Print" onclick="javascript:location.reload(true);window.print();" /></td><td style="width:60px"><input ID="CLOSE" type="button" value="Close" onclick="window.close();" /></td></tr></table>';

        var containerIDHTML = document.getElementById(containerID).innerHTML;

        document.getElementById('_printPreview').innerHTML = TopBar + "<div id='myDiv' style='padding:8px; font-size:8pt; font-family:Arial, Helvetica, sans-serif'><table width='100%'><tr><td colspan='2' class='sectionHeader'>Matter Details</td></tr></table>" + document.getElementById("MatterDetails").innerHTML
        + "<br />" + containerIDHTML
        + AdditionalInfo
        + PublicFundingInnerHTML.replace("General Info ", "General Info (Public Funding)")

        + "</div>";
        
    }
    catch (error) { }
    var myDiv = document.getElementById('myDiv') //.disabled = true;

    for (var i = 0; i < myDiv.childNodes.length; i++) {
        //alert(myDiv.childNodes[i].tagName);
        getChildNodes(myDiv.childNodes[i]);
    }

    document.getElementById('_PageContents').style.display = "none";

    //getPrint(window.document.body.innerHTML); 
    //document.location.href = "editmatter.aspx";
}
function doEditClientPrint() {

    var containerID = 0;
    var containerID1 = 0;
    var containerID2 = 0;
    var containerID3 = 0;
    var containerID4 = 0;

    //containerID is global
    //containerID = "ctl00__cphMain__tcEditClient__pnlAddressDetails";
    containerID5 = "ctl00__cphMain__tcEditClient__pnlAddressDetails__ucAddress__tcClientSearch__pnlByUniqueReference__updPnlAdditionalInfo";
    containerID4 = "ctl00__cphMain__tcEditClient__pnlAddressDetails__ucAddress__tcClientSearch__pnlAddress__updPnlAddressDetails";
    //containerID1 = tabContainerClientID + "__pnlContactDetails";
    containerID2 = "ctl00__cphMain__tcEditClient__pnlGeneralDetails";
    containerID3 = "ctl00__cphMain__tcEditClient__pnlIndividualDetails";
    containerID6 = "ctl00__cphMain__tcEditClient__pnlOrganisationDetails"; 

    var GeneralDetails = "";
    var IndOrgDetails = "";
    
    if (document.getElementById(containerID2) != null) { GeneralDetails = document.getElementById(containerID2).innerHTML; }

    if (document.getElementById(containerID3) != null)
    { IndOrgDetails = document.getElementById(containerID3).innerHTML; }
    else
    { IndOrgDetails = document.getElementById(containerID6).innerHTML; }

    var TopBar = '<table cellspacing="4" cellpadding="2" width="100%" class="HeaderTab" style="border-bottom:White"><tr><td>&nbsp;</td><td style="width:60px"><INPUT ID="PRINT" type="button" value="Print" onclick="javascript:location.reload(true);window.print();"></td><td style="width:60px"><INPUT ID="CLOSE" type="button" value="Close" onclick="window.close();"></td></tr></table>';

    document.getElementById('_printPreview').innerHTML = TopBar + "<div id='myDiv' style='padding:8px; font-size:8pt; font-family:Arial, Helvetica, sans-serif'>"
    + "<br /><table width='100%'><tr><td colspan='2' class='sectionHeader'>Address Details</td></tr><tr><td style='vertical-align:top'>" + document.getElementById(containerID4).innerHTML + "</td>"
    + "<td style='vertical-align:top'>" + document.getElementById(containerID5).innerHTML + "</td></tr></table>"
    + GeneralDetails
    + "<br /><br />" + IndOrgDetails
    + "</div>";

    var myDiv = document.getElementById('myDiv') //.disabled = true;  
    for (var i = 0; i < myDiv.childNodes.length; i++) { getChildNodes(myDiv.childNodes[i]); }

    document.getElementById('_PageContents').style.display = "none";
 
}

function getChildNodes(myDiv) {

    if (myDiv.tagName != null) {
    
        for (var i = 0; i < myDiv.childNodes.length; i++) {

            var proceed = true;
            var tn = myDiv.childNodes[i].tagName;
            var newElement = document.createElement('div');

            if (tn == "INPUT" || tn == "SELECT" || tn == "A") { myDiv.childNodes[i].disabled = true; }

            if (tn == "DIV") {
                if (myDiv.childNodes[i].id == "MapControl" || myDiv.childNodes[i].id == "MapControl2" || myDiv.childNodes[i].id == "Regenerated" || myDiv.childNodes[i].id == "ResetBtnContainer") {
                    myDiv.childNodes[i].innerHTML = "";

                }
            }

            if (tn == "DIV") {
                if (myDiv.childNodes[i].id == "Regenerate") {

                    myDiv.removeChild(myDiv.childNodes[i]);
                    proceed = false;
                } 
            }


            if (tn == "A" && myDiv.childNodes[i].className == "link") {

                newElement.innerHTML = myDiv.childNodes[i].innerHTML;
                myDiv.replaceChild(newElement, myDiv.childNodes[i]);
            }

            if (tn == "INPUT" && myDiv.childNodes[i].value != "_rdoBtnMatterCompletedYes" && myDiv.childNodes[i].value
                                                                                                    != "_rdoBtnMatterCompletedNo" && myDiv.childNodes[i].type != "hidden" && myDiv.childNodes[i].type != "image"
                                                                                                    && myDiv.childNodes[i].type != "checkbox") {
                newElement.innerHTML = myDiv.childNodes[i].value;
                newElement.setAttribute("style", "font-size:8pt; font-family:arial; vertical-align:bottom;");
                myDiv.replaceChild(newElement, myDiv.childNodes[i]);
            }

            if (tn == "INPUT") {
                if (myDiv.childNodes[i].type == "hidden") {
                    myDiv.removeChild(myDiv.childNodes[i]);
                } 
            }

            if (tn == "INPUT") {
                if (myDiv.childNodes[i].type == "image") {
                    myDiv.removeChild(myDiv.childNodes[i]);
                } 
            }


            if (tn == "TEXTAREA") {
                newElement.innerHTML = myDiv.childNodes[i].value;
                newElement.setAttribute("style", "border:1px White Solid; font-size:8pt; font-family:arial; color:black");
                myDiv.replaceChild(newElement, myDiv.childNodes[i]);
            }

            if (tn == "SELECT") {
                if (myDiv.childNodes[i].selectedIndex != -1) {
                    newElement.innerHTML = myDiv.childNodes[i].options[myDiv.childNodes[i].selectedIndex].text;
                    newElement.setAttribute("style", "font-size:8pt; font-family:arial; vertical-align:bottom;");
                    myDiv.replaceChild(newElement, myDiv.childNodes[i]);
                }
                else { myDiv.removeChild(myDiv.childNodes[i]); }
            }

            if (tn == "INPUT") {

                if (myDiv.tagName == "SPAN" && myDiv.className == "checkBox") {
                    if (myDiv.childNodes[i].checked == true && myDiv.childNodes[i].type == "checkbox") {
                        myDiv.innerHTML = "Yes";
                    }
                    else {
                        myDiv.innerHTML = "No";
                    }
                }
            }

            if (tn == "SPAN") {
                if (myDiv.childNodes[i].className == "mandatoryField" && myDiv.childNodes[i].className != "checkbox") {


                    myDiv.childNodes[i].innerHTML = "";
                }
            }





            if (proceed) {
                getChildNodes(myDiv.childNodes[i]);
            }
        }
    }
}

function getPrint(print_area) {
    //Creating new page
    var pp = window.open();
    //Adding HTML opening tag with <HEAD> … </HEAD> portion 
    pp.document.writeln('<HTML><HEAD><title>Print Preview</title>')
    pp.document.writeln('<base target="_self"></HEAD>')
    //Adding Body Tag
    pp.document.writeln('<body MS_POSITIONING="GridLayout" bottomMargin="0"');
    pp.document.writeln(' leftMargin="0" topMargin="0" rightMargin="0">');
    //Adding form Tag
    pp.document.writeln('<form method="post">');

    //Creating two buttons Print and Close within a HTML table
    pp.document.writeln('<TABLE width=100%><TR><TD></TD></TR><TR><TD align=right>');
    pp.document.writeln('<INPUT ID="PRINT" type="button" value="Print" ');
    pp.document.writeln('onclick="javascript:location.reload(true);window.print();">');
    pp.document.writeln('<INPUT ID="CLOSE" type="button" ' +
                        'value="Close" onclick="window.close();">');
    pp.document.writeln('</TD></TR><TR><TD></TD></TR></TABLE>');

    //Writing print area of the calling page
    pp.document.writeln(print_area);
    //Ending Tag of </form>, </body> and </HTML>
    pp.document.writeln('</form></body></HTML>');
}

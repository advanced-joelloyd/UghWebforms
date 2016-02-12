function OkClickOnOnlineAddressVerification(listAddress, houseNumber, postCode, houseName, addressLine1, town, county, country, depLoc, addressLine2, addressLine3, lastVerified) {
    var _listAddress = document.getElementById(listAddress);
    if (_listAddress != null) {

        //var addressIndex = _listAddress.selectedIndex;
        var address = _listAddress.value; //_listAddress[addressIndex].text; //
        if (address == 0) {
            //alert('Please select an address');
            return false;
        }
        else {
            //alert(address);
            document.getElementById(houseNumber).value = "";
            document.getElementById(postCode).value = "";
            document.getElementById(houseName).value = "";
            document.getElementById(addressLine1).value = "";
            document.getElementById(town).value = "";
            document.getElementById(county).value = "";
            document.getElementById(country).value = "";
            document.getElementById(depLoc).value = "";
            document.getElementById(addressLine2).value = "";
            document.getElementById(addressLine3).value = "";

            var addrArray = address.split("~");
            document.getElementById(houseNumber).value = addrArray[2];
            document.getElementById(houseName).value = addrArray[3];
            document.getElementById(addressLine1).value = addrArray[4];
            document.getElementById(country).value = addrArray[10];
            document.getElementById(town).value = addrArray[7];
            document.getElementById(county).value = addrArray[8];
            document.getElementById(postCode).value = addrArray[9];
            document.getElementById(depLoc).value = addrArray[5];
            document.getElementById(addressLine2).value = addrArray[6];

            var d = new Date();

            var curr_date = String(d.getDate());
            if (curr_date.length == 1) {
                curr_date = "0" + curr_date;
            }

            var curr_month = String(d.getMonth() + 1);
            if (curr_month.length == 1) {
                curr_month = "0" + curr_month;
            }

            var curr_year = d.getFullYear();
            document.getElementById(lastVerified).value = "" + curr_date + "/" + curr_month + "/" + curr_year + "";
        }
    }
    return true;
}

function CancelClickOnOnlineAddressVerification() {
    return false;
}

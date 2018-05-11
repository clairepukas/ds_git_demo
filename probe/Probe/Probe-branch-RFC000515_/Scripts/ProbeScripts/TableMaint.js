var currTbl = '';
var qry = self.location.search.split('&');
currTbl = qry[0].substring(qry.indexOf('tblName=') + 10);
var editDiag;
var retval = "";

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    if (currTbl != '') {
        getAjxData(currTbl, 'get', '');
    }

    //set dialog for edit form
    editDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 900,
        width: 900,
        modal: true,
        close: function () {

        }
    });

});



function showEdit(elem) {
    var idtxt = elem.id.split('_');

    getAjxData('edit', idtxt[2], idtxt[3], idtxt[4], '', idtxt[1], '');
    //$("#dialog-modal").html(ajxret);

    if (retval != "")
    {
        return;
    }

    //set dialog for edit form
    editDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 500,
        width: 700,
        modal: true,
        close: function () {

        }
    });

    

    editDiag.dialog("open");

    $("#frmRsrcEdit input[id^=prop_").on('change', function () {
        calcEditTotals();
    });
    $("#frmRsrcEdit input[id^=act_").on('change', function () {
        calcEditTotals();
    });

    calcEditTotals();

    $("#frmRsrcEdit input[id^=prop_").first().focus();

    //alert(idtxt);

}
function goAdd() {
    var valstr = $(event.srcElement).parent().next().next().val();
    getAjxData(currTbl, 'edit', valstr, 'add');
    if (retval != "") {
        return;
    }

    editDiag.dialog("open");
}

function goDel() {
    var retval = window.confirm('Are you sure you want to delete this record?');
    if (retval) {
        var valstr = $(event.srcElement).parent().next().val();
        getAjxData(currTbl, 'delete', valstr);
    }

}

function goEdit() {

    var valstr = $(event.srcElement).parent().next().val();
    getAjxData(currTbl, 'edit', valstr);
    if (retval != "") {
        return;
    }

    editDiag.dialog('option','title','Edit Record for: ' + currTbl)
    editDiag.dialog("open");

}
function closeEditDialog() {
    editDiag.dialog("close");
}

function saveRecEdit() {
    var addFlag = $("#frmTblEdit input[id=doAdd]").val();
    var origData = $("#frmTblEdit input[id=originalData]").val();
    var jsonData = "{";

    //build jsondata for insert or update   
    $("#frmTblEdit input[type=text]").each(function () {
        var fldId = $(this).prop('id');
        var fldVal = $(this).val();
        jsonData += "\"" + fldId + "\":\"" + fldVal + "\",";

    });

    $("#frmTblEdit select").each(function () {
        var fldId = $(this).prop('id');
        var fldVal = $(this).val();
        jsonData += "\"" + fldId + "\":\"" + fldVal + "\",";

    });

    //remove the lastcomma
    jsonData = jsonData.substring(0, jsonData.lastIndexOf(","));
    jsonData += "}";

    if (addFlag) {
        //add flag set so set up for doing insert else do update
        getAjxData(currTbl, 'insert', origData, jsonData);
    } else {
        //do update
        getAjxData(currTbl, 'update', origData, jsonData);
    }

    closeEditDialog();

}

function getAjxData(tbl, act, oldjsondata, newjsondata) {
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance?tblName=" + tbl + "&actTyp=" + act + "&origData=" + oldjsondata + "&newData=" + newjsondata;
    $.ajax({
        url: acturl,
        type: 'POST',
        async: false,
        success: function (result) {
            //alert("success");
            retval = "";
            if (result.indexOf("Error") > -1) {
                alert(result);
                retval = "error";
                return;
            }
            if (act == 'get') {
                var divelm = document.getElementById("listDiv");
                var rsun = unescape(result);
                divelm.innerHTML = unescape(rsun);
            }
            if (act == 'edit') {
                var divelm = document.getElementById("dialog-modal");
                divelm.innerHTML = result;

                //set dialog for edit form
                editDiag = $("#dialog-modal").dialog({
                    autoOpen: false,
                    height: 600,
                    width: 800,
                    modal: true,
                    close: function () {

                    }
                });
            }
            if (act == "delete") {
                getAjxData(currTbl, 'get', '');
            }
            if (act == "insert") {
                getAjxData(currTbl, 'get', '');
            }
            if (act == "update") {
                getAjxData(currTbl, 'get', '');
            }
        },
        failure: function (result) {
            //alert("Failed");
        }
    });


}

//if key pressed is not a number then disallows the event and no character is entered.
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57)) {
        return false;
    }

    return true;
}

//////////////////////////////////////////////////
//this function gets the event source element and captures the key stroke to
//respond to the navigation of the schedules data grid based on the key pressed
// and help to navigate to the appropriate area on the grid
//////////////////////////////////////////////////
function getKey() {
    //return;
    var kc = event.keyCode;
    var elm = event.srcElement;
    var nrsrc;
    var nhre;
    var eid = elm.id;
    //nrsrc = parseInt(eid.substring(4, eid.indexOf('hre')));
    // nhre = parseInt(eid.substr(eid.indexOf('hre') + 3));

    switch (kc) {
        case 36: //home                  
            // document.all.item('rsrc0hre0').focus();
            break;
        case 35: //end 
            // document.all.item('rsrc' + (endRsrc - 1) + 'hre' + hrArray[hrArray.length]).focus();
            break;
        case 33: //page up
            //if (nrsrc > 15) {
            // //   document.all.item('rsrc' + (nrsrc - 15) + 'hre' + nhre).focus();
            //} else {
            //  //  document.all.item('rsrc0' + 'hre' + nhre).focus();
            //}
            break;
        case 34: //page down
            //if (nrsrc < (endRsrc - 15)) {
            //   // document.all.item('rsrc' + (nrsrc + 15) + 'hre' + nhre).focus();
            //} else {
            //   // document.all.item('rsrc' + (endRsrc - 1) + 'hre' + nhre).focus();
            //}
            break;
        case 13: //return
            //if (nhre < hrArray.length) {
            // document.all.item('rsrc' + nrsrc + 'hre' + (nhre + 1)).focus();
            //} else {
            elm.blur();
            elm.select();
            //}
            break;
        case 37: //left arrow
            //if (nhre > 0) {
            //    //document.all.item('rsrc' + nrsrc + 'hre' + (nhre - 1)).focus();
            //}
            break;
        case 38: //up arrow
            //if (nrsrc > 0) {
            //    //document.all.item('rsrc' + (nrsrc - 1) + 'hre' + nhre).focus();
            //}
            break;
        case 39: //right arrow
            //if (nhre < hrArray.length) {
            //    //document.all.item('rsrc' + nrsrc + 'hre' + (nhre + 1)).focus();
            //}
            break;
        case 40: //down arrow
            //if (nrsrc < (endRsrc - 1)) {
            //    //document.all.item('rsrc' + (nrsrc + 1) + 'hre' + nhre).focus();
            //}
            break;
    }
}
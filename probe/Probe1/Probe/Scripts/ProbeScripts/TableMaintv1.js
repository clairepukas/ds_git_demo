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
    
    if (retval == true) {
        var f = $(event.srcElement).parent().next().val();
        var valstr = $(event.srcElement).parent().next().val();
        getAjxData(currTbl, 'delete', valstr);
    } else {
        
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
    $('#dialog-modal').empty();
    $('#dialog-modal2').empty()
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
            
          
            retval = "";
            if (result.indexOf("Error") > -1) {
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





function createGroup(obj) {
    var acturl = "";
    var groupCase;

    var model = {};
    var gname = "";
    model.tableName = obj;
    var check = "true";
    console.log(44444)
    console.log(obj)
    console.log(44444)
    switch (obj) {

        case 'Groups':
            model.GroupId = $("#GroupId").val();
            model.GroupName = $("#GroupName").val();
            acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/AdminAddGroup";
            groupCase = "Groups";
            gname = $("#GroupName").val();
            break;


        case 'ApprovalActivityStatuses':
            model.ApprovalStatus = $("#ApprovalStatus").val();
            model.ActivityStatus = $("#ActivityStatus").val();
            model.ReportDefault = $("#ReportDefault").val();
            gname = $("#ActivityStatus").val();
            for (var t in model) {
                if (model[t]=="") {
                    check = "false"
                }
            }
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1";
            }
            groupCase = "ApprovalActivityStatuses";
            break;

        case 'BenefitGroups':
            model.BenefitGroupId = $("#BenefitGroupId").val();
            model.BenefitGroupName = $("#BenefitGroupName").val();
            model.BenefitGroupOrder = $("#BenefitGroupOrder").val();
            groupCase = "BenefitGroups";
            gname = $("#BenefitGroupId").val();
            groupCase = "BenefitGroups";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            if (check == "true") {
               
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1";
            }
            break;


        case 'BudgetStatuses':
            model.BudgetStatus = $("#BudgetStatus").val();
            model.BudgetStatusDisplayed = model.BudgetStatus;
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            gname = model.BudgetStatus;
            groupCase = "BudgetStatuses";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            break;

        case 'BudgetTypes':
            model.BudgetType = $("#BudgetType").val();
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            if (check == "true") {

                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            gname = model.BudgetType;
            groupCase = "BudgetTypes";
            break;

        case 'CostSavingsTypes':
            model.CostSavingsTypeId = $("#CostSavingsTypeId").val();
            model.CostSavingsTypes = $("#CostSavingsType").val();
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            if (check == "true") {

                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            gname = model.CostSavingsTypes;

            groupCase = "CostSavingsTypes"
            break;


        case 'Benefits':
            model.BenefitId = $("#BenefitId").val();
            model.BenefitName = $("#BenefitName").val();
            model.BenefitGroupId = $("#BenefitGroupId").val();
            gname = model.BenefitName;
            model.account = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "Benefits";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            break;



        case 'DocumentTypes':
            model.DocumentType = $("#DocumentType").val();
            
            gname = model.DocumentType;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "DocumentTypes";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
           
            break;


        case 'EntityTypes':
            model.EntityType = $("#EntityType").val();
            gname = model.DocumentType;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "EntityTypes";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            
            break;


        case 'Impacts':
            
            model.ImpactId = $("#ImpactId").val();
            model.ImpactName = $("#ImpactName").val();
            model.ImpactDesc = $("#ImpactDesc").val();;

            model.action = "insert";
            for (var t in model) {
                if (model[t] == "" ) {
                    check = "false"
                }
            }
           
            groupCase = "Impacts";
            if (check == "true") {
               
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            break;



        case 'ServiceAgreementTypes':
            model.ServiceAgreementType = $("#ServiceAgreementType").val();
            gname = model.ServiceAgreementType;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "ServiceAgreementTypes";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }

            break;




        case 'IssueStatuses':
            model.IssueStatus = $("#IssueStatus").val();
            gname = model.IssueStatus;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "IssueStatuses";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            
            break;



        case 'TemplateTypes':
            model.TemplateType = $("#TemplateType").val();
            gname = model.TemplateType;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "TemplateTypes";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }

            break;
            
        case 'ProjectType':
            model.ProjectType = $("#ProjectType").val();
            gname = model.ProjectType;
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "ProjectTypes";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
            break;


        case 'Phases':
            model.PhaseId = $("#PhaseId").val();

            model.PhaseName = $("#PhaseName").val();
            model.PhaseDesc = $("#PhaseDesc").val();
            model.PhaseOrder = $("#PhaseOrder").val();
            model.color = $("#color").val();
            model.weeks_duration = $("#weeks_duration").val();

           
            gname = 'Phases';
            model.action = "insert";
            for (var t in model) {
                if (model[t] == "") {
                    check = "false"
                }
            }
            groupCase = "Phases";
            if (check == "true") {
                acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            }
           
            break;


        case 'AttachmentGroups':
            model.AttachmentGroupName = $("#AttachmentGroupName").val()
            model.AttachmentGroupId = $("#AttachmentGroupId").val();
            groupCase = "AttachmentGroups";
            gname = 'AttachmentGroups';
            acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"

            break;

        case 'ServiceAgreementVendors':
         
            model.ServiceAgreementVendorName = $("#ServiceAgreementVendorName").val();
            model.ServiceAgreementVendorId = 1;
            model.updateBy = "estes";
            groupCase = "ServiceAgreementVendors";
            model.action = "insert";
            acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Admin/TableMaintenance1"
            break;


    }
   



    model.table = obj;
    var Tmodel=JSON.stringify(model)
    $.ajax({
        url: acturl,
        type: 'POST',
        data: model,
        tdata: Tmodel,
        gname: gname,
        groupCase:groupCase,
        async: false,
        success: function (result) {
            retIndx = 0;
            var varstrg;
         
            switch (this.groupCase) {

                case 'Groups':
                    var returnObj = $.parseJSON(result);
                    var retCntr = returnObj.length;
                    for (k = 0; k < retCntr; k++) {
                        if (this.gname == returnObj[k][this.groupCase]) {
                            retIndx = k;
                        }
                    }
                    
                    /**retCntr = retCntr - 1;
                    var t = retCntr % 2;
                    var background = "#efeeef;"
                    if (t == 0) {
                        background = "#c2d0d3;"
                    } else {
                        background = "#efeeef;"
                    }
                    varstrg = "<tr style='background-color:" + background + "'><td><a class='btnDel' onclick='javascript:goDel();'><img class='imgdel' src='../images/delete.png' alt='delete record'></a><input id='originalData' type='hidden' value='{&quot;GroupId&quot;:&quot;" + returnObj[retIndx].GroupId + " &quot;,&quot;GroupName&quot;:&quot;" + returnObj[retIndx].GroupName + "&quot;}'></td><td><a class='btnAdd' onclick='javascript:goAdd();'><img class='imgadd' src='../images/add.png' alt='add record'></a></td><td><a class='btnEdit' onclick='javascript:goEdit();'><img class='imgedit' src='../images/pencil.png' alt='edit record'></a><input id='originalData' type='hidden' value='{&quot;GroupId&quot;: " + returnObj[retIndx].GroupId + ",&quot;GroupName&quot;:&quot;" + returnObj[retIndx].GroupName + "&quot;}'></td><td>" + returnObj[retIndx].GroupId + "</td><td>" + returnObj[retIndx].GroupName + "</td></tr>"
                    $(".editTable").children().append(varstrg);**/

                    var url = "TableMaintenance?tblName=Groups&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                            
                        }
                    })



                    break;


                case 'ApprovalActivityStatuses':
                    var url = "TableMaintenance?tblName=ApprovalActivityStatuses&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;


                case 'BenefitGroups':
                    var url = "TableMaintenance?tblName=BenefitGroups&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                   
                    break;


                case 'Benefits':

                    //var pars = $.parseJSON(this.tdata);
                    //stringo = "<tr><td><a class='btnDel' onclick='javascript:goDel();'><img class='imgdel' src='../images/delete.png' alt='delete record'></a><input id='originalData' type='hidden' value='{&quot;BenefitId&quot;:" + pars.BenefitId + ",&quot;BenefitName&quot;:&quot;" + pars.BenefitName + "&quot;,&quot;BenefitGroupId&quot;:" + pars.BenefitGroupId + "}'></td><td><a class='btnAdd' onclick='javascript:goAdd();'><img class='imgadd' src='../images/add.png' alt='add record'></a></td><td><a class='btnEdit' onclick='javascript:goEdit();'><img class='imgedit' src='../images/pencil.png' alt='edit record'></a><input id='originalData' type='hidden' value='{&quot;BenefitId&quot;:" + pars.BenefitId + ",&quot;BenefitName&quot;:&quot;" + pars.BenefitName + "&quot;,&quot;BenefitGroupId&quot;:" + pars.BenefitGroupId + "}'></td><td>" + pars.BenefitId + "</td><td>" + pars.BenefitName + "</td><td>" + pars.BenefitGroupId + "</td></tr>"
                    var url = "TableMaintenance?tblName=Benefits&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;


                case 'BudgetStatuses':
                    var url = "TableMaintenance?tblName=BudgetStatuses&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                           
                        }
                    })
                    break;



                case 'BudgetTypes':
                    var pars = $.parseJSON(this.tdata);
                    stringo = "<tr><td><a class='btnDel' onclick='javascript:goDel();'><img class='imgdel' src='../images/delete.png' alt='delete record'></a><input id='originalData' type='hidden' value='{&quot;BenefitId&quot;:" + pars.BenefitId + ",&quot;BenefitName&quot;:&quot;" + pars.BenefitName + "&quot;,&quot;BenefitGroupId&quot;:" + pars.BenefitGroupId + "}'></td><td><a class='btnAdd' onclick='javascript:goAdd();'><img class='imgadd' src='../images/add.png' alt='add record'></a></td><td><a class='btnEdit' onclick='javascript:goEdit();'><img class='imgedit' src='../images/pencil.png' alt='edit record'></a><input id='originalData' type='hidden' value='{&quot;BenefitId&quot;:" + pars.BenefitId + ",&quot;BenefitName&quot;:&quot;" + pars.BenefitName + "&quot;,&quot;BenefitGroupId&quot;:" + pars.BenefitGroupId + "}'></td><td>" + pars.BenefitId + "</td><td>" + pars.BenefitName + "</td><td>" + pars.BenefitGroupId + "</td></tr>"
                    var url = "TableMaintenance?tblName=BudgetTypes&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;


                case 'CostSavingsTypes':
                    var url = "TableMaintenance?tblName=CostSavingsTypes&actTyp=get&origData=&newData=undefined"
                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;





                case 'DocumentTypes':
                    var url = "TableMaintenance?tblName=DocumentTypes&actTyp=get&origData=&newData=undefined"
                   
                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;


                case 'EntityTypes':
                    var url = "TableMaintenance?tblName=EntityTypes&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })

                    break;


                case 'Impacts':
                    var url = "TableMaintenance?tblName=Impacts&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;



                case 'IssueStatuses':
                    var url = "TableMaintenance?tblName=IssueStatuses&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })
                    break;

                    break;

                case 'ServiceAgreementTypes':
                    var url = "TableMaintenance?tblName=ServiceAgreementTypes&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })

                    break;


                case 'TemplateTypes':
                    var url = "TableMaintenance?tblName=TemplateTypes&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })

                    break;


                case 'ProjectTypes':
                    var url = "TableMaintenance?tblName=ProjectType&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })


                    break



                case 'Phases':
                    var url = "TableMaintenance?tblName=Phases&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })


                    break

                case 'AttachmentGroups':
                    var url = "TableMaintenance?tblName=AttachmentGroups&actTyp=get&origData=&newData=undefined"

                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })


                    break

                case "ServiceAgreementVendors":
                    var url = "TableMaintenance?tblName=ServiceAgreementVendors&actTyp=get&origData=&newData=undefined"
                   
                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (datam) {
                            $(".listDiv").empty().append(datam);
                        }
                    })


                    break
            }
            
            
            closeEditDialog()
        }
    })
}




function editGroup(obj, table) {
    var vals = $(obj).attr("value");
    var tObj = $.parseJSON(vals);
    var stringO = "Edit " + table;
  
    editDiag1=$("#dialog-modal2").dialog({
        resizable: false,
        width: 900,
        height: 500,
        modal: true,
        title: stringO,
        vals:vals,
        buttons: {
            "update": function () {
                switch (table) {
                    case 'BenefitGroups':
                        url = "TableMaintenance?tblName=BenefitGroups&actTyp=update&origData=undefined&newData=add";
                        var benefitGroupModel = {}
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })
                        benefitGroupModel.tblName="BenefitGroups";
                        benefitGroupModel.actTyp="update";
                        benefitGroupModel.origData = vals;
               
                        y = JSON.stringify(sendObj);
                        benefitGroupModel.newData = y;
          
                       
                        $.ajax({
                            url: url,
                            data:benefitGroupModel,
                            type: 'POST',
                            success: function (datam1) {
                                var url1 = "TableMaintenance";
                                bgroupSend={};
                                bgroupSend.tblName="BenefitGroups";
                                bgroupSend.actTyp="get";
                                bgroupSend.origData = "none";
                                bgroupSend.newData="none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data:bgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })

                            }
                        })

                        break;


                    case 'Benefits':
                        url = "TableMaintenance?tblName=Benefits&actTyp=update&origData=undefined&newData=add";
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })

                      

                        benefitsModel = {};
                        benefitsModel.tblName = "Benefits";
                        benefitsModel.actTyp = "update";
                        benefitsModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        benefitsModel.newData = y;


                        $.ajax({
                            url: url,
                            data: benefitsModel,
                            type: 'POST',
                            success: function (datam1) {
                               
                                var url1 = "TableMaintenance";
                                bgroupSend = {};
                                bgroupSend.tblName = "Benefits";
                                bgroupSend.actTyp = "get";
                                bgroupSend.origData = "none";
                                bgroupSend.newData = "none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data: bgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })
                            }
                        })
                        
                        
                        break;


                    case 'CostSavingsTypes':

                       
                        url = "TableMaintenance?tblName=CostSavingsTypes&actTyp=update&origData=undefined&newData=add";
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })



                        CostSavingsModel = {};
                        CostSavingsModel.tblName = "CostSavingsTypes";
                        CostSavingsModel.actTyp = "update";
                        CostSavingsModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        CostSavingsModel.newData = y;


                        $.ajax({
                            url: url,
                            data: CostSavingsModel,
                            type: 'POST',
                            success: function (datam1) {

                                var url1 = "TableMaintenance";
                                bgroupSend = {};
                                bgroupSend.tblName = "CostSavingsTypes";
                                bgroupSend.actTyp = "get";
                                bgroupSend.origData = "none";
                                bgroupSend.newData = "none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data: bgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })
                            }
                        })
                        break;


                    case 'ServiceAgreementType':
                        url = "TableMaintenance?tblName=ServiceAgreementType&actTyp=update&origData=undefined&newData=add";
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })

                        CostSavingsModel = {};
                        CostSavingsModel.tblName = "ServiceAgreementType";
                        CostSavingsModel.actTyp = "update";
                        CostSavingsModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        CostSavingsModel.newData = y;
                        break;



                    case 'Phases':
                        url = "TableMaintenance?tblName=Phases&actTyp=update&origData=undefined&newData=add";
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })


                        PhasesModel = {};
                        PhasesModel.tblName = "Phases";
                        PhasesModel.actTyp = "update";
                        PhasesModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        PhasesModel.newData = y;

                        $.ajax({
                            url: url,
                            data: PhasesModel,
                            type: 'POST',
                            success: function (datam1) {
                                var url1 = "TableMaintenance";
                                bgroupSend = {};
                                bgroupSend.tblName = "Phases";
                                bgroupSend.actTyp = "get";
                                bgroupSend.origData = "none";
                                bgroupSend.newData = "none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data: bgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })

                            }

                        })
                        break;


                    case 'Impacts':
                        url = "TableMaintenance?tblName=Impacts&actTyp=update&origData=undefined&newData=add";
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })


                        ImpactsModel = {};
                        ImpactsModel.tblName = "Impacts";
                        ImpactsModel.actTyp = "update";
                        ImpactsModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        ImpactsModel.newData = y;


                        $.ajax({
                            url: url,
                            data: ImpactsModel,
                            type: 'POST',
                            success: function (datam1) {
                                var url1 = "TableMaintenance";
                                bgroupSend = {};
                                bgroupSend.tblName = "Impacts";
                                bgroupSend.actTyp = "get";
                                bgroupSend.origData = "none";
                                bgroupSend.newData = "none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data: bgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })

                            }

                        })
                        break;


                    case 'ServiceAgreementVendors':
                       
                        url = "TableMaintenance?tblName=ServiceAgreementVendors&actTyp=update&origData=undefined&newData=add";
             
                        var sendObj = {};
                        $(".editTable :input").each(function () {
                            sendObj[$(this).attr("id")] = $(this).val();
                        })


                        vendorModel = {};
                        vendorModel.tblName = "ServiceAgreementVendors";
                        vendorModel.actTyp = "update";
                        vendorModel.origData = vals;
                        y = JSON.stringify(sendObj);
                        vendorModel.newData = y;

                        $.ajax({
                            url: url,
                            data: vendorModel,
                            type: 'POST',
                            success: function (datam1) {

                                var url1 = "TableMaintenance";
                                vgroupSend = {};
                                vgroupSend.tblName = "ServiceAgreementVendors";
                                vgroupSend.actTyp = "get";
                                vgroupSend.origData = "none";
                                vgroupSend.newData = "none"
                                $.ajax({
                                    url: url1,
                                    type: 'POST',
                                    data: vgroupSend,
                                    success: function (data2) {
                                        $("#listDiv").html(data2);
                                        $('#dialog-modal').empty()
                                        $("#dialog-modal2").empty()
                                        $("#dialog-modal2").dialog("close");
                                    }
                                })
                            }

                        })
                        
                        
                        break;


                   
                }
            },
            "cancel": function () {
                $('#dialog-modal').empty()
                $("#dialog-modal2").empty();
                $(this).dialog("close");
                
            }
        },
        open: function () {
            var superObj = $.parseJSON(vals);
            var xStrg = "";
            for (var g in superObj) {
                switch(table){
                    case 'BenefitGroups':
                        url = "TableMaintenance?tblName=BenefitGroups&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val:vals,
                            success: function (datam) {
                                
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })

                        break;


                    case "ServiceAgreementVendors":
                        url = "TableMaintenance?tblName=ServiceAgreementVendors&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val: vals,
                            success: function (datam) {

                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })
                        break;

                    case 'Benefits':
                        url = "TableMaintenance?tblName=Benefits&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val:vals,
                            success: function (datam) {
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })
                        break;

                    case 'CostSavingsTypes':
                        url = "TableMaintenance?tblName=CostSavingsTypes&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val: vals,
                            success: function (datam) {
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })
                      
                        break;



                    case 'ServiceAgreementType':
                        url = "TableMaintenance?tblName=ServiceAgreementType&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val: vals,
                            success: function (datam) {
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })

                        break;


                    case 'Phases':

                        url = "TableMaintenance?tblName=Phases&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val: vals,
                            success: function (datam) {
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }
                            }
                        })
                    
                        break;


                    case 'Impacts':

                        url = "TableMaintenance?tblName=Impacts&actTyp=edit&origData=undefined&newData=add"
                        $.ajax({
                            url: url,
                            type: 'POST',
                            val: vals,
                            success: function (datam) {
                                $('#dialog-modal').empty()
                                $('#dialog-modal2').empty().append(datam);
                                var b = $.parseJSON(this.val)
                                $("#btnsave").hide();
                                $("#btncancel").hide();
                                for (var t in b) {
                                    $("#" + t).val(b[t]);
                                }

                            }
                        })

                        break;
                }

                
            }
          
          
        }
     
   
    })
    editDiag1.dialog("open");
  

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



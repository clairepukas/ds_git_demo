
var projId = "";
var projName = "";
var UpdProjInfoUrl = "";
var ReconcileDatesUrl = "";
var addUpdateUrl = "";
var changeUpdateUrl = "";
var deleteUpdateUrl = "";
var addIssueUrl = "";
var changeIssueUrl = "";
var deleteIssueUrl = "";
var deleteAttachUrl = "";
var addContribUrl = "";
var deleteContribUrl = "";
  
var msg="";


var mgrList = "";
var contribList = "";
var userName = "";
//DatePicker

    $(function () {
        var end = $("#ProjectedEnd").val();

        if(end == "")
        {
            end = "+10y";
        }

        var start = $("#ProjectedStart").val();

        if(start == "")
        {
            start = "-3y";
        }

        $("#ProjectedStart").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 3,
            maxDate: end,
            onClose: function (selectedDate) {
                $("#ProjectedEnd").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#ProjectedEnd").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 3,
            minDate: start,
            onClose: function (selectedDate) {
                $("#ProjectedStart").datepicker("option", "maxDate", selectedDate);
            }
        });

    });

  


    
//Update Form Fields

    $(function (){
        $('#Priority').click(function(event){
            fieldChanged(this);
        });
    });

    function fieldChanged(field) {
        var fid = field.id;
        $.ajax({
            url: UpdProjInfoUrl,
            type: "POST",
            data: $("#projectInfoForm").serialize() + "&changedField=" + field.id,
            async: false
            //dataType: "json"
        })
        .success(function (model) {
            if (field.id == 'DisplayName') {
                location.reload();
            }
        })
        .error(function (msg) {
            alert("There was an error while making update.");
            $('#dialog-message').dialog("close");
        });
    }


    
//Update ActivityStatus

            function activityStatusChanged(field){
                $("#dialog-message").dialog({
                    resizable: false,
                    width: 700,
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            if (field.id == "ActivityStatus") {
                                if (field.value == "Not Started") {
                                    $("#ActualStart").val("");
                                }
                            }
                            $.ajax({
                                url: UpdProjInfoUrl,
                                type: "POST",
                                data: $("#projectInfoForm").serialize() + "&changedField=" + field.id,
                                async: false
                            })
                            .success(function( model ) {
                                location.reload();
                            })
                            .error(function (msg) {
                                alert("There was an error while updating activity status.");
                                $('#dialog-message').dialog("close");
                            });
                        },
                        "No": function () {
                            
                            $(this).dialog("close");
                            location.href = location.href;
                        }
                    }
                });
                $('#dialog-message').dialog('option', 'title', 'Change Activity Status');

                msg = "";
                
                if(field.value == 'In Progress')
                {
                    msg = "You have chosen to update the status of this project to In Progress. This will set the Actual Start Date to the Projected Start Date if there is no Actual Start Date value. Do you want to continue?";
                }
                else if (field.value == 'Completed')
                {
                    msg = "You have chosen to update the status of this project to Completed. This will set the Actual End Date to the Projected End Date if there is no Actual End Date value. This action will set the project in Read Only mode and can only be undone by the Administrator. Do you want to continue?";
                }
                else if (field.value == 'Cancelled')
                {
                    msg = "You have chosen to update the status of this project to Cancelled. This action will set the project in Read Only mode and can only be undone by the Administrator. Do you want to continue?";
                }
                else if (field.value == 'On Hold')
                {
                    msg = "You have chosen to update the status of this project to On Hold. This action will set the project in Read Only mode for all Actual data and can only be undone by the Administrator. Do you want to continue?";
                }
                if (field.value == 'Not Started') {
                    msg = "You have chosen to update the status of this project to Not Started. This will remove the Actual Start Date which will affect budget, schedule and resource data. Do you want to continue?";
                }
                document.getElementById('dialog-message').innerHTML = "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + msg + "</p>";
                
                if (msg == "") {
                    $('#dialog-message').dialog("close");
                }

            }




//Update Project Dates

    
    function dateFieldChanged(field){
        
        if(field.id.indexOf("Projected")>-1)
        {
            if(field.value == ""){
                
                alert('Projected dates cannot be blank. Please select an appropriate date.');
                field.value = field.defaultValue;
                $(field).focus();
                return false;
            }
        }

        if(field.id == "ProjectedStart"){
            var endDt = Date.parse($("#ProjectedEnd").val());
            var strtDt = Date.parse(field.value);
            if(strtDt >= endDt){
                alert('Projected Start date must be less than the Projected End date. Please select an appropriate date.');
                field.value = field.defaultValue;
                $(field).focus();
                return false;
            }
        }

        if(field.id == "ProjectedEnd"){
            var strtDt = Date.parse($("#ProjectedStart").val());
            var endDt = Date.parse(field.value);
            if(endDt <= strtDt){
                alert('Projected End date must be greater than the Projected Start date. Please select an appropriate date.');
                field.value = field.defaultValue;
                $(field).focus();
                return false;
            }
        }

        if (field.id == "ActualStart") {
  
            if ($("#" + field.id).val() == "") {
                alert('You have chosen to delete an Actual Start Date but the project has an active Status.  Setting the Activity Status to “Not Started” first, will automatically set the Actual Start date to blank.');
                location.href = location.href;
                return false;
            }
        }

        msg = "You have chosen a date that will shorten the current project span and may result in loss of data for the Budget and Resources. This action cannot be undone. Do you want to continue?";

    $.ajax({
        url: ReconcileDatesUrl,
        type: "POST",
        data: $("#projectInfoForm").serialize() + "&changedField=" + field.id,
        async: false
    })
              .success(function( doPopup ) {
                  if(doPopup.indexOf('true') > -1)                          
                  { 
                      if(doPopup.indexOf("|")>-1)
                      {
                          msg += "<br/><br/>" + doPopup.substring(doPopup.indexOf("|") + 1, doPopup.length);
                      }
                      doDateDialog(field);
                  }
                  else
                  {
                      if(doPopup.indexOf("|")>-1)
                      {
                          alert(doPopup.substring(doPopup.indexOf("|") + 1, doPopup.length));
                      }
                      fieldChanged(field);
                      location.reload();
                  }
              })
              .error(function (msg) {
                  alert("There was an error while updating dates");
                  $('#dialog-message').dialog("close");
              });
}


function doDateDialog(field){   
    $("#dialog-message").dialog({
        resizable: false,
        width: 700,
        modal: true,
        buttons: {
            "Yes": function () {
                $.ajax({
                    url: UpdProjInfoUrl,
                    type: "POST",
                    data: $("#projectInfoForm").serialize() + "&changedField=" + field.id,
                    async: false
                })
                .success(function( model ) {
                    location.reload();
                })
                .error(function (msg) {
                    alert("There was an error while updating activity status.");
                    $('#dialog-message').dialog("close");
                });
            },
            "No": function () {
                $(this).dialog("close");
                $(field).val($(field)[0].defaultValue);
            }
        }
    });

    $('#dialog-message').dialog('option', 'title', 'Change Project Date');

            

    document.getElementById('dialog-message').innerHTML =   "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>" + msg + "</p>";

}



                 //Handles the view click on the updates section
                function ViewUpdate(updtExpl, updtUser) {
                    $(function () {
                        $("#dialog-message").dialog({
                            width: 600,
                            modal: true,
                            buttons: {
                                "Close": function () {
                                    $(this).dialog("close");
                                }
                            }
                        });
                    });
                    $('#dialog-message').dialog('option', 'title', 'View Project Update');
                    document.getElementById('dialog-message').innerHTML = "<table><tr><td>Project</td><td><center>'" + projName +
                                                                          "'</center></td></tr><tr><td>Owner</td><td><center>" + updtUser +
                                                                          "</center></td></tr><tr><td>Explanation</td>" +
                                                                          "<td><textarea rows='4'>" + updtExpl +
                                                                          "</textarea></td></tr></table>";
                }


//Handles the add click on the updates section
function AddUpdate(currentUser) {
    $(function () {
        $("#dialog-message").dialog({
            width: 600,
            modal: true,
            buttons: {
                "Add Update": function () {
                    var id = document.getElementById("addProjID").value;
                    var txt = document.getElementById("addExpTxt").value;
                    var myData = { projID: id, updateTxt: txt };
                    $.ajax({
                        type: 'POST',
                        url: addUpdateUrl,
                        data: myData,
                        async: false
                    })
                        .success(function (msg) {
                            alert("Add update completed successfully.");
                            $('#dialog-message').dialog("close");
                            location.reload();
                        })
                        .fail(function (msg) {
                            alert("There was an error while adding update.");
                            $('#dialog-message').dialog("close");
                        });
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    $('#dialog-message').dialog('option', 'title', 'Add Project Update');
    document.getElementById('dialog-message').innerHTML = "<form id='addUpdateForm'><fieldset><input type='hidden' id='addProjID' value='" + projId +
                                                          "'/><table><tr><td>Project</td><td><center>'" + projName +
                                                          "'</center></td></tr><tr><td>Owner</td><td><center>" + currentUser +
                                                          "</center></td></tr><tr><td>Explanation</td>" +
                                                          "<td><textarea id='addExpTxt' rows='4' >" +
                                                          "</textarea></td></tr><tr><td><input type='submit' tabindex='-1' style='position:absolute; top:-1000px'></td></tr></table></fieldset></form>";
}

//Handles the edit click on the updates section
function EditUpdate(updtExpl, updtUser, updtOwnrId, updtID) {
    $(function () {
        $("#dialog-message").dialog({
            width: 600,
            modal: true,
            buttons: {
                "Update": function () {
                    var id = document.getElementById("updtID").value;
                    var txt = document.getElementById("expTxt").value;
                    var myData = {projId: projId, updateID: id, updateTxt: txt, updateOwnerId: updtOwnrId };
                    //$.ajaxSetup({ cache: false });
                    $.ajax({
                        type: 'POST',
                        url: changeUpdateUrl,
                        data: myData,
                        async: false
                    })
                        .success(function (msg) {
                            alert("Update completed successfully.");
                            $('#dialog-message').dialog("close");
                            location.reload();
                        })
                        .fail(function (msg) {
                            alert("There was an error while updating.");
                            $('#dialog-message').dialog("close");
                        });
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    $('#dialog-message').dialog('option', 'title', 'Edit Project Update');

    document.getElementById('dialog-message').innerHTML = "<form id='editUpdateForm'><fieldset><input type='hidden' id='updtID' value='" + updtID +
                                                          "'/><table><tr><td>Project</td><td><center>'" + projName +
                                                          "'</center></td></tr><tr><td>Owner</td><td><center>" + updtUser +
                                                          "</center></td></tr><tr><td>Explanation</td>" +
                                                          "<td><textarea id='expTxt' rows='4' >" + updtExpl +
                                                          "</textarea></td></tr><tr><td><input type='submit' tabindex='-1' style='position:absolute; top:-1000px'></td></tr></table></fieldset></form>";
}


//Delete Update
function DeleteUpdate(updtID) {
    $("#dialog-message").dialog({
        resizable: false,
        width: 375,
        modal: true,
        buttons: {
            "Yes": function () {
                var id = updtID
                var myData = {projId: projId, updateID: id };
                //$.ajaxSetup({ cache: false });
                $.ajax({
                    type: 'POST',
                    url: deleteUpdateUrl,
                    data: myData,
                    async: false
                })
                    .success(function (msg) {
                        $('#dialog-message').dialog("close");
                        location.reload();
                    })
                    .fail(function (msg) {
                        alert("There was an error while deleting.");
                        $('#dialog-message').dialog("close");
                    });
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#dialog-message').dialog('option', 'title', 'Delete Project Update');
    document.getElementById('dialog-message').innerHTML =   "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>Are you sure you want to delete this update?</p>"

}


                //Handles the view click on the issues section
                function ViewIssue(issExpl, issUser, issStat, issTgtDate, issRes) {
                    $(function () {
                        $("#dialog-message").dialog({
                            width: 600,
                            modal: true,
                            buttons: {
                                "Close": function () {
                                    $(this).dialog("close");
                                }
                            },
                            open: function (event, ui) {
                                $('#dialog-message').dialog('option', 'title', 'View Project Issue');
                                document.getElementById('dialog-message').innerHTML = "<form><table><fieldset><tr><td>Project</td><td colspan=3>'" + projName +
                                                                        "'</td></tr><tr><td>Owner</td><td colspan=2>" + issUser +
                                                                        "</td></tr><tr><td>Explanation</td>" +
                                                                        "<td colspan=3><textarea rows='4' >" + issExpl +
                                                                        "</textarea></td></tr><tr><td>Status</td><td><input class='textBox' type='text' value='" + issStat +
                                                                        "'/></td><td style='width: 210px'>Required Resolution Date</td><td><input class='textBox' type='text' value='" + issTgtDate +
                                                                        "'</tr><tr><td>Resolution</td><td colspan=3><textarea rows='4' >" + issRes +
                                                                        "</textarea></td></tr></table></fieldset></form>";
                            }
                        });
                    });


                }

//Handles the add click on the updates section
function AddIssue() {
    $(function () {
        $("#dialog-message").dialog({
            width: 600,
            modal: true,
            buttons: {
                "Add Issue": function () {
                    var id = document.getElementById("addProjId").value;
                    var exp = document.getElementById("addExpTxt").value;
                    var stat = document.getElementById("setStatusDDL").value;
                    var tgt = document.getElementById("issTgtDate").value;
                    var ownr = document.getElementById("listOfOwners").value;
                    var res = document.getElementById("issRes").value;

                    if (res != "" && stat != "Closed"){
                        alert("The Issue Resolution field has a value. Please set the Status value to 'Closed'.");
                        $('#setStatusDDL').focus();
                    }
                    else if (stat == "Closed" && res == ""){
                        alert("The Issue Resolution field must have a value to close the Issue.");
                        $('#issRes').focus();
                    }
                    else {

                        if(tgt!=""){

                            var myData = { projID: id, explanation: exp, status: stat, target: tgt, resolution:res, owner:ownr };
                        $.ajax({
                            type: 'POST',
                            url: addIssueUrl,
                            data: myData,
                            async: false
                        }) .success(function (msg) {
                            alert("Add issue completed successfully.");
                            $('#dialog-message').dialog("close");
                            location.reload();
                        })
                            .fail(function (msg) {
                                alert("There was an error while adding issue.");
                                $('#dialog-message').dialog("close");
                            });

                        } else {
                            $("#issTgtDate").css("background-color", "#ccc").on("focus", function () {
                                $(this).css("background-color", "#fff");
                            })
                        }
                    }




                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            open: function (event, ui) {
                $('#dialog-message').dialog('option', 'title', 'Add Project Issue');
                document.getElementById('dialog-message').innerHTML = "<form id='addIssueForm'><fieldset><input type='hidden' id='addProjId' value='" + projId +
                                                                                "'/><table><tr><td>Project</td><td colspan=3>'" + projName +
                                                                                "'</td></tr><tr><td>Owner</td><td colspan=2>" +
                                                                                "<select id='listOfOwners' >" +
                                                                                "</td></tr><tr><td>Explanation</td>" +
                                                                                "<td colspan=3><textarea id='addExpTxt' rows='4' >" +
                                                                                "</textarea></td></tr><tr><td>Status</td><td><select id='setStatusDDL' name='setStatusDDL'><option value='Opened'>Opened</option>" +
                                                                                "<option value='In Progress'>In Progress</option><option value='Closed'>Closed</option></select>" +
                                                                                "</td><td style='width: 220px'>Required Resolution Date</td><td><input class='textBox' type='text' id='issTgtDate' value='" +
                                                                                "'</tr><tr><td>Resolution</td><td colspan=3><textarea  id='issRes' rows='4' >" +
                                                                                "</textarea></td></tr><tr><td><input type='submit' tabindex='-1' style='position:absolute; top:-1000px'></td></tr></table></fieldset></form>";



                //var userList = @Html.Raw(Json.Encode(Model.ManagerList));
                $.each(mgrList, function (i, o) {
                    var id = o.Value;
                    var txt = o.Text;
                    if(id == userName)
                    {
                        $('#listOfOwners').append('<option value="' +id+'" selected=selected >' + txt + '</option>');
                    }
                    else
                    {
                        $('#listOfOwners').append('<option value="' +id+'">' + txt + '</option>');
                    }
                });

                $(function() {
                    $( "#issTgtDate" ).datepicker({
                        changeMonth: true,
                        changeYear: true
                    });
                });
            }

        });
    });


}

//Handles the edit click on the issues section
function EditIssue(issId, issExpl, issUser, issOwnerUName, issStat, issTgtDate, issRes) {
    $(function () {
        $("#dialog-message").dialog({
            width: 600,
            modal: true,
            buttons: {
                "Update": function () {
                    var exp = document.getElementById("issExpl").value;
                    var ddl = $('#listOfOwners').val();
                    var stat = document.getElementById("statusDDL").value;
                    var dt = document.getElementById("issTgtDate").value;
                    var res = document.getElementById("issRes").value;

                    var myData = { issueId: issId, projectId: projId, explanation: exp, status: stat, target: dt, resolution:res, owner: ddl };

                    if (res != "" && stat != "Closed"){
                        alert("The Issue Resolution field has a value. Please set the Status value to 'Closed'.");
                        $('#setStatusDDL').focus();
                    }
                    else if (stat == "Closed" && res == ""){
                        alert("The Issue Resolution field must have a value to close the Issue.");
                        $('#issRes').focus();
                    }
                    else{
                        $.ajax({
                            type: 'POST',
                            url: changeIssueUrl,
                            data: myData,
                            async: false
                        })
                        .success(function (msg) {
                            alert("Update completed successfully.");
                            $('#dialog-message').dialog("close");
                            location.reload();
                        })
                        .fail(function (msg) {
                            alert("There was an error while updating.");
                            $('#dialog-message').dialog("close");
                        });
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            open: function (event, ui) {
                $('#dialog-message').dialog('option', 'title', 'Edit Project Issue');
                document.getElementById('dialog-message').innerHTML = "<form><table><fieldset><input type='hidden' id='issueId' value='" + issId +
                                                                        "'/><tr><td>Project</td><td colspan=3>'" + projName +
                                                                        "'</td></tr><tr><td>Owner</td><td colspan=2>" +
                                                                        "<select id='listOfOwners' >" +
                                                                        "</td></tr><tr><td>Explanation</td>" +
                                                                        "<td colspan=3><textarea rows='4' id='issExpl' >" + issExpl +
                                                                        "</textarea></td></tr><tr><td>Status</td><td><select id='statusDDL' name='statusDDL'>" +
                                                                        "<option value='Unknown'></option>" +
                                                                        "<option value='Opened'>Opened</option>" +
                                                                        "<option value='In Progress'>In Progress</option>" +
                                                                        "<option value='Closed'>Closed</option></select>" +
                                                                        "</td><td style='width: 220px'>Required Resolution Date</td><td><input class='textBox' id='issTgtDate' type='text' value='" + issTgtDate +
                                                                        "'</tr><tr><td>Resolution</td><td colspan=3><textarea id='issRes' rows='4' >" + issRes +
                                                                        "</textarea></td></tr></table></fieldset></form>";



                //var userList = @Html.Raw(Json.Encode(Model.ManagerList));
                $.each(mgrList, function (i, o) {
                    var id = o.Value;
                    var txt = o.Text;
                    if(id == issOwnerUName)
                    {
                        $('#listOfOwners').append('<option value="' +id+'" selected=selected >' + txt + '</option>');
                    }
                    else
                    {
                        $('#listOfOwners').append('<option value="' +id+'">' + txt + '</option>');
                    }
                });



                $('#statusDDL').val(issStat);

                $(function() {
                    $( "#issTgtDate" ).datepicker({
                        changeMonth: true,
                        changeYear: true
                    });
                });
            }
        });
    });

}

//Delete Issue

function DeleteIssue(issId) {
    $("#dialog-message").dialog({
        resizable: false,
        width: 375,
        modal: true,
        buttons: {
            "Yes": function () {
                var id = issId
                var myData = {projId: projId, issueID: id };
                $.ajax({
                    type: 'POST',
                    url: deleteIssueUrl,
                    data: myData,
                    async: false
                })
                    .success(function (msg) {
                        $('#dialog-message').dialog("close");
                        location.reload();
                    })
                    .fail(function (msg) {
                        alert("There was an error while deleting.");
                        $('#dialog-message').dialog("close");
                    });
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#dialog-message').dialog('option', 'title', 'Delete Project Issue');
    document.getElementById('dialog-message').innerHTML =   "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>Are you sure you want to delete this issue?</p>"

}

    $(document).ready(function(){
        var btn = document.getElementById('uploadBtn');
        var fUpload = document.getElementById('uploadFile');

        btn.disabled = true;
        $('#uploadFile').change(
            function(){
                if ($(this).val()){
                    btn.disabled = false; 
                }
                else {
                    btn.disabled = true;
                }
            })              
    });

function toggleUploadDiv(){
    $('#uploadDiv').toggle('show');
}

//Delete Attachment
function DeleteAttachment(id) {
    $("#dialog-message").dialog({
        resizable: false,
        width: 375,
        modal: true,
        buttons: {
            "Yes": function () {
                var myData = {projId: projId, attachmentId: id };
                $.ajax({
                    type: 'POST',
                    url: deleteAttachUrl,
                    data: myData,
                    async: false
                })
                    .success(function (msg) {
                        $('#dialog-message').dialog("close");
                        location.reload();
                    })
                    .fail(function (msg) {
                        alert("There was an error while deleting.");
                        $('#dialog-message').dialog("close");
                    });
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#dialog-message').dialog('option', 'title', 'Delete Project Attachment');
    document.getElementById('dialog-message').innerHTML =   "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>Are you sure you want to delete this attachment?</p>"

}




    //Handles the add click on the updates section
    function AddContributor() {
        $(function () {
            $("#dialog-message").dialog({
                width: 500,
                modal: true,
                buttons: {
                    "Add Contributor": function () {
                        var cont = document.getElementById("listOfUsers").value;
                        var myData = {projectId: projId, uname: cont };
                        $.ajax({
                            type: 'POST',
                            url: addContribUrl,
                            data: myData,
                            async: false
                        })
                            .success(function (msg) {
                                alert("Contributor added successfully.");
                                $('#dialog-message').dialog("close");
                                location.reload();
                            })
                            .fail(function (msg) {
                                alert("There was an error while adding contributor.");
                                $('#dialog-message').dialog("close");
                            });
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                },
                open: function (event, ui) {
                    $('#dialog-message').dialog('option', 'title', 'Add Project Contributor');
                    document.getElementById('dialog-message').innerHTML = "<form id='addContributorForm'><fieldset><input type='hidden' id='projectId' value='" + projId +
                                                                                    "'/><table><tr><td>Project</td><td><center>'" + projName +
                                                                                    "'</center></td></tr><tr><td>Select a Contributor</td><td><center>" +
                                                                                    "<select id='listOfUsers' >" +
                                                                                    "</center></td></tr>" +
                                                                                    "<tr><td><input type='submit' tabindex='-1' style='position:absolute; top:-1000px'></td></tr></table></fieldset></form>";



                    //var userList = @Html.Raw(Json.Encode(Model.ContributorList));
                    $.each(contribList, function (i, o) {
                        var id = o.Value;
                        var txt = o.Text;

                        if(txt == "null")
                            txt = id;

                        if(id != null){
                            $('#listOfUsers').append('<option value="' +id+'">' + txt + '</option>');
                        }

                    });
                }

            });
        });


    }

//Delete Contributor
function DeleteContributor(id) {
    $("#dialog-message").dialog({
        resizable: false,
        width: 375,
        modal: true,
        buttons: {
            "Yes": function () {
                var myData = {projectId: projId, contributorId: id };
                $.ajax({
                    type: 'POST',
                    url: deleteContribUrl,
                    data: myData,
                    async: false
                })
                    .success(function (msg) {
                        $('#dialog-message').dialog("close");
                        location.reload();
                    })
                    .fail(function (msg) {
                        alert("There was an error while deleting.");
                        $('#dialog-message').dialog("close");
                    });
            },
            "No": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#dialog-message').dialog('option', 'title', 'Delete Project Contributor');
    document.getElementById('dialog-message').innerHTML =   "<p><span class='ui-icon ui-icon-alert' style='float:left; margin:0 7px 20px 0;'></span>Are you sure you want to delete this contributor?</p>"

}


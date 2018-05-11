
//var ajxret = "";
$(document).ready(function () {

    setPageView('list');

    //Search function
    $("#searchTxt").keyup(function () {
        var fnd = 0;
        _this = this;

        // Show only matching TR, hide rest of them
        $.each($("#listDiv table tbody").find("tr"), function () {
            //console.log($(this).text());
            if ($(this).text().toLowerCase().indexOf($(_this).val().toLowerCase()) == -1) {
                $(this).hide();
            }
            else {
                $(this).show();
                fnd++;
            }
        });
        $("#srchfnd").text(fnd + " found.");
        $("#fltrfnd").text("");
    });


    if ($("#searchTxt").val() != "") {
        $("#searchTxt").keyup();
    }
});


var infoDiag;
var noteDiag;


function closeNotesDialog() {
    if (noteDiag) {
        noteDiag.dialog("close");
    }
}

function closeInfoDialog() {
    if (infoDiag) {
        infoDiag.dialog("close");
    }
}

function AddEntity() {
    if (infoDiag) {
        var entId = -1;
        var notes = '';
        var entName = '';
        var entType = '';
        var userId = -1;


        $("#frmEntityInfo").find("input").each(function () {
            if (this.id == 'EntityName') {
                entName = this.value;
            }
            if (this.id == 'EntityType') {
                entType = this.value;
            }
        });
        $("#frmEntityInfo").find("select").each(function () {
            if (this.id == 'UserId') {
                userId = $(this).val();
            }
        });
        $("#frmEntityInfo").find("textarea").each(function () {
            if (this.id == 'ent_comments') {
                notes = $(this).text();
            }
        });

        getAjxData('addEntity', entId, entName, entType, notes, userId);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function saveEntityInfo() {
    if (infoDiag) {
        var entId = -1;
        var notes = '';
        var entName = '';
        var entType = '';
        var userId = -1;
        var parentId = 999999999;

        $("#frmEntityInfo").find("input").each(function () {
            if (this.id == 'EntityId') {
                entId = this.value;
            }
            if (this.id == 'EntityName') {
                entName = this.value;
            }
            if (this.id == 'EntityType') {
                entType = this.value;
            }
            if (this.id == 'parentId') {
                parentId = this.value;
            }
        });
        $("#frmEntityInfo").find("select").each(function () {
            if (this.id == 'UserId') {
                userId = $(this).val();
            }
        });
        $("#frmEntityInfo").find("textarea").each(function () {
            if (this.id == 'ent_comments') {
                notes = $(this).text();
            }
        });

        getAjxData('updateEntityInfo', entId, entName, entType, notes, userId,parentId);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function showInfo(elem) {
    var idtxt = elem.id.split('_');

    if (idtxt.length == 1) {
        getAjxData('new');
    } else {
        getAjxData('info',  idtxt[1]);
    }

    //set dialog for resoure information
    infoDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 500,
        width: 750,
        modal: true,
        close: function () {

        }
    });

    infoDiag.dialog("open");

}

function getAjxData(acttyp, ent, entname, enttype, notes, userid,parentId) {
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminEntityHandler?EntId=" + ent + "&ActTyp=" + acttyp + "&entName=" + entname + "&entType=" + enttype + "&notes=" + notes + "&userId=" + userid + "&parentId=" + parentId ;
    //alert(acturl);
    $.ajax({
        url: acturl,
        type: 'POST',
        async: false,
        //data: $(frm).serialize(),
        success: function (result) {
            if (result != '') {
                //alert(result);
                if (result.indexOf('Error') == -1) {
                    //alert(result);
                } else {
                    alert(result);
                }

            }
            $("#dialog-modal").html(result);
        },
        failure: function (result) {
            alert(result);

        }
    });
}

function applySort(srt) {
    $("#sort").val(srt);
    setPageView('list')
}

function setPageView(view) {
    var sort = "";
    if ($("#sort").val()) {
        sort = $("#sort").val();
    }
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminEntityHandler?ActTyp=" + view +"&sort=" + sort;
    //alert(acturl);
    $.ajax({
        url: acturl,
        type: 'POST',
        async: false,
        //data: $(frm).serialize(),
        success: function (result) {
            if (result != '') {
                //alert(result);
                if (result.indexOf('Error') == -1) {
                    //alert(result);
                } else {
                    alert(result);
                }

            }

            var divelm = document.getElementById("listDiv");
            divelm.innerHTML = result;


            ////set the onchange funciton for the delete and edit buttons for Resources
            $('img[id^=delent_]').on('click', function () {
                DelEntity(this);
            });

            $('img[id^=editent_]').on('click', function () {
                showInfo(this);
            });

            ////set the click function for add Resource button
            $('.btnEntityAdd').on('click', function () {
                showInfo(this);
            });



        },
        failure: function (result) {
            alert(result);

        }
    });
}



function DelEntity(rsrcElem) {
    var idtxt = $(rsrcElem).prop('id').split('_');

    if ($(rsrcElem).prop('id').indexOf('delent') > -1) {
        var retval = window.confirm('Are you sure you want to delete this Entity?');
        //alert(retval);        
        if (retval) {
            getAjxData('delEntity', idtxt[1]);

            setPageView('list');
        }
    }

}

function setTopLvl()
{
    $(".parentname").text("Top Level");
    $("#parentId").val('-1');
   
}

function setUnassigned() {
    $(".parentname").text("Unassigned");
    $("#parentId").val('999999999');

}




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

function AddResource() {
    if (infoDiag) {
        var rsrc = -1;
        var cmts = '';
        var rsrcName = '';
        var rsrcTitle = '';
        var rsrcEmail = '';
        var rsrcPhone = '';
        var entitiyId = -1;

        $("#frmRsrcInfo").find("input").each(function () {
            if (this.id == 'ResourceName') {
                rsrcName = this.value;
            }
            if (this.id == 'ResourceTitle') {
                rsrcTitle = this.value;
            }
            if (this.id == 'ResourceEmail') {
                rsrcEmail = this.value;
            }
            if (this.id == 'ResourcePhone') {
                rsrcPhone = this.value;
            }
        });

        $("#frmRsrcInfo").find("textarea").each(function () {
            if (this.id == 'rsrc_comments') {
                cmts = $(this).text();
            }
        });

        getAjxData('addResource', '', rsrc, cmts, rsrcName, rsrcTitle, rsrcEmail, rsrcPhone);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function saveRsrcInfo() {
    if (infoDiag) {
        var rsrc = -1;
        var cmts = '';
        var rsrcName = '';
        var rsrcTitle = '';
        var rsrcEmail = '';
        var rsrcPhone = '';
        var entitiyId = -1;

        $("#frmRsrcInfo").find("input").each(function () {
            if (this.id == 'rsrcId') {
                rsrc = this.value;
            }
            if (this.id == 'ResourceName') {
                rsrcName = this.value;
            }
            if (this.id == 'ResourceTitle') {
                rsrcTitle = this.value;
            }
            if (this.id == 'ResourceEmail') {
                rsrcEmail = this.value;
            }
            if (this.id == 'ResourcePhone') {
                rsrcPhone = this.value;
            }
            if (this.id == 'EntityId') {
                entitiyId = this.value;
            }
        });

        $("#frmRsrcInfo").find("textarea").each(function () {
            if (this.id == 'rsrc_comments') {
                cmts = $(this).text();
            }
        });

        getAjxData('updateRsrcInfo', entitiyId, rsrc, cmts, rsrcName, rsrcTitle, rsrcEmail, rsrcPhone);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function showInfo(elem) {
    var idtxt = elem.id.split('_');

    if (idtxt.length == 1) {
        getAjxData('new');
    } else {
        getAjxData('info', idtxt[2], idtxt[1]);
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

function getAjxData(acttyp, ent, rsrc, cmts, rsrcname, rsrctitle, rsrcemail, rsrcphone, sort) {
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminResourcesHandler?RsrcId=" + rsrc + "&EntId=" + ent + "&ActTyp=" + acttyp + "&comments=" + cmts + "&rsrcName="+rsrcname + "&rsrcTitle=" + rsrctitle + "&rsrcEmail=" + rsrcemail + "&rsrcPhone=" + rsrcphone;
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
                }else{
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

function setPageView(view)
{
    var sort = "";
    if ($("#sort").val()) {
        sort = $("#sort").val();
    }
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminResourcesHandler?ActTyp=" + view + "&sort=" + sort;
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
            $('img[id^=delrsrc_]').on('click', function () {
                DelRsrc(this);
            });

            $('img[id^=editrsrc_]').on('click', function () {
                showInfo(this);
            });

            ////set the click function for add Resource button
            $('.btnRsrcAdd').on('click', function () {
                showInfo(this);
            });            



        },
        failure: function (result) {
            alert(result);

        }
    });
}



function DelRsrc(rsrcElem) {
    var idtxt = $(rsrcElem).prop('id').split('_');

    if ($(rsrcElem).prop('id').indexOf('delrsrc')>-1) {
        var retval = window.confirm('Are you sure you want to delete this Resource?');
        //alert(retval);        
        if (retval)
        {
            getAjxData('delResource', idtxt[2], idtxt[1]);

            setPageView('list');
        } 
    }

}






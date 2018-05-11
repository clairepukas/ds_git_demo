
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

function AddTemplate() {
    if (infoDiag) {
        var tName = '';
        var tDesc = '';
        var tFilTyp = '';
        var tTyp = '';
        var tDocTyp = '';


        $("#frmNewTplt").find("input").each(function () {
            if (this.id == 'TemplateFileName') {
                tName = this.value;
            }
            if (this.id == 'FileType') {
                tFilTyp = this.value;
            }
        });

        $("#frmNewTplt").find("textarea").each(function () {

            if (this.id == 'TemplateDesc') {
                tDesc = this.value;
            }
        });

        $("#frmNewTplt").find("select").each(function () {
            if (this.id == 'TemplateType') {
                tTyp = this.value;
            }
            if (this.id == 'DocumentType') {
                tDocTyp = this.value;
            }
        });
        //tName, tTyp, tDesc, tFilTyp, tDocTyp, sort
        getAjxData('addTemplate', tName, tTyp, tDesc, tFilTyp, tDocTyp);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function saveTpltInfo() {
    if (infoDiag) {
        var tName = '';
        var tDesc = '';
        var tFilTyp = '';
        var tTyp = '';
        var tDocTyp = '';


        $("#frmTpltInfo").find("input").each(function () {
            if (this.id == 'TemplateFileName') {
                tName = this.value;
            }
            if (this.id == 'FileType') {
                tFilTyp = this.value;
            }
        });

        $("#frmTpltInfo").find("textarea").each(function () {

            if (this.id == 'TemplateDesc') {
                tDesc = this.value;
            }
        });

        $("#frmTpltInfo").find("select").each(function () {
            if (this.id == 'TemplateType') {
                tTyp = this.value;
            }
            if (this.id == 'DocumentType') {
                tDocTyp = this.value;
            }
        });
        //tName, tTyp, tDesc, tFilTyp, tDocTyp, sort
        getAjxData('updTemplate', tName, tTyp, tDesc, tFilTyp, tDocTyp);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function showInfo(elem) {
    var idtxt = elem.id.split('_');

    if (elem.id.indexOf('add') > -1)
    {
        getAjxData('new');
    }
    if (elem.id.indexOf('edit') > -1) {
        getAjxData('info', idtxt[1], idtxt[2],'','',idtxt[3]);
    }
    if (elem.id.indexOf('del') > -1) {
        getAjxData('delTemplate', idtxt[1], idtxt[2], '', '', idtxt[3]);
    }
    if (elem.id.indexOf('upld') > -1) {
        getAjxData('upload', idtxt[1], idtxt[2], '', '', idtxt[3]);
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

function getAjxData(acttyp, tName, tTyp, tDesc, tFilTyp, tDocTyp, sort) {
    //string ActTyp = null, string tpltName = null, string tpltType = null, string tpltDesc = null, string tpltFileTyp = null, string tpltDocTyp = null, string sort = null)
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminTemplatesHandler?ActTyp=" + acttyp + "&tpltName=" + tName + "&tpltType=" + tTyp + "&tpltDesc=" + tDesc + "&tpltFileTyp=" + tFilTyp + "&tpltDocTyp=" + tDocTyp + "&sort=" + sort;
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
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminTemplatesHandler?ActTyp=" + view + "&sort=" + sort;
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
            $('img[id^=deltplt_]').on('click', function () {
                DelRsrc(this);
            });

            $('img[id^=edittplt_]').on('click', function () {
                showInfo(this);
            });
            $('img[id^=upldtplt_]').on('click', function () {
                showInfo(this);
            });
            $('img[id^=dnldtplt_]').on('click', function () {
                doDownload(this);
            });
            ////set the click function for add Resource button
            $('.btnTpltAdd').on('click', function () {
                showInfo(this);
            });            



        },
        failure: function (result) {
            alert(result);

        }
    });
}

function doDownload(elem)
{
    var idtxt = $(elem).prop('id').split('_');

    if ($(elem).prop('id').indexOf('dnldtplt_') > -1) {

        var acturl = location.protocol + "//" + location.host + "/Probe/Admin/GetTemplateFile";
        var inputs = '<input type="hidden" id="tName" name="tName" value="' + idtxt[1] + '" />';
        inputs += '<input type="hidden" id="tFilTyp" name="tFilTyp" value="' + idtxt[2]+ '" />';
        inputs += '<input type="hidden" id="tDocTyp" name="tDocTyp" value="' + idtxt[3] + '" />';
        //send request
        var encupld = 'multipart/form-data';

        $('<form id="' + 'frmfly' + '" enctype="' + encupld + '" action="' + acturl + '" method="' + '"POST"' + ' target="' + '_blank' + '"><fieldset>' + inputs + '</fieldset></form>')
        .appendTo('body').submit().remove();

    }
}

function saveTpltUpld()
{

        var acturl = location.protocol + "//" + location.host + "/Probe/Admin/UpldTemplateFile";
        //TODO submit form as multipart form 
        $("#frmTpltUpld").prop('action', acturl);
        $("#frmTpltUpld").prop('enctype', 'multipart/form-data');
        $("#frmTpltUpld").submit();
        //getAjxData('putBinary', idtxt[1], idtxt[2], '', '', idtxt[3]);

        infoDiag.dialog("close");

        setPageView('list');

}

function DelRsrc(elem) {
    var idtxt = $(elem).prop('id').split('_');

    if ($(elem).prop('id').indexOf('deltplt') > -1) {
        var retval = window.confirm('Are you sure you want to delete this Template?');
        //alert(retval);        
        if (retval)
        {
            getAjxData('delTemplate', idtxt[1], idtxt[2],'','', idtxt[3]);

            setPageView('list');
        } 
    }

}






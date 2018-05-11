
//var ajxret = "";

var infoDiag;
var noteDiag;
var rsrcDiag;
var entDiag;
var waitDiag;
var editDiag;

    $(document).ready(function () {

        //$("#dialog-modal").html("<div><p> please wait ...</p><img src='../Images/loader.gif' alt='loading' /><div>");

        //waitDiag = $("#dialog-modal").dialog({
        //    autoOpen: false,
        //    height: 300,
        //    width: 500,
        //    modal: true,
        //    close: function () {
        //    }
        //});

      

        //$.ajaxSetup(
        //{
        //    cache: false,
        //    beforeSend: function () {
        //        waitDiag.dialog('open');
        //    },
        //    complete: function () {
        //        waitDiag.dialog('close');
        //    },
        //    success: function () {
        //        waitDiag.dialog('close');
        //    }
        //});

        setPageView('all');

    });


    function showEdit(elem) {
        var idtxt = elem.id.split('_');

        getAjxData('edit', idtxt[3], idtxt[4],'', idtxt[1]);

        //getAjxData(acttyp, ent, rsrc, cmts, yr, mhrs, rsrctitle, rsrcemail, rsrcphone, parentent)

        //set dialog for edit form
        editDiag = $("#dialog-modal").dialog({
            autoOpen: false,
            height: 900,
            width: 900,
            modal: true,
            close: function () {

            }
        });


        editDiag.dialog("open");

        $("#frmRsrcEdit input[id=allBase]").on('change', function () {
            $("#frmRsrcEdit input[id^=base_]").each(function () {
                $(this).val($("#frmRsrcEdit input[id=allBase]").val());
            });
            calcEditTotals();
        });
        $("#frmRsrcEdit input[id=allCore]").on('change', function () {
            $("#frmRsrcEdit input[id^=core_]").each(function () {
                $(this).val($("#frmRsrcEdit input[id=allCore]").val());
            });
            calcEditTotals();
        });

        $("#frmRsrcEdit input[id^=base_]").on('change', function () {
            calcEditTotals();
        });
        $("#frmRsrcEdit input[id^=core_]").on('change', function () {
            calcEditTotals();
        });



        calcEditTotals();

        $("#frmRsrcEdit input[id^=base_]").first().focus();

        //alert(idtxt);

    }
    function calcEditTotals() {
        var basetot = 0;
        var coretot = 0;
        $("#frmRsrcEdit input[id^=base_]").each(function () {
            basetot += parseInt($(this).val());
        });
        $("#frmRsrcEdit input[id^=core_]").each(function () {
            coretot += parseInt($(this).val());
        });

        $("#frmRsrcEdit input[id=totBase]").val(basetot);
        $("#frmRsrcEdit input[id=totCore]").val(coretot);
    }
    function closeEditDialog() {
        if (editDiag) {
            editDiag.dialog("close");
        }
    }
    function saveRsrcEdits() {
        if (editDiag) {
            var user = -1;
            var rsrc = -1;
            var yr = -1;
            var ent = -1;
            var cmts = '';

            user = $("#frmRsrcEdit input[id=userId]").val();
            rsrc = $("#frmRsrcEdit input[id=rsrcId]").val();
            yr = $("#frmRsrcEdit input[id=year]").val();
            ent = $("#frmRsrcEdit input[id=entId]").val();

            var recArr = new Array();

            $("#frmRsrcEdit input[id^=base_]").each(function () {
                var cmts = '';
                var basemhrs = 0;
                var coremhrs = 0;

                var idxarr = this.id.split('_');
                basemhrs = $(this).val();
                coremhrs = $("#frmRsrcEdit input[id=core_" + idxarr[1] + "]").val();
                cmts = $("#frmRsrcEdit textarea[id=cmts_" + idxarr[1] + "]").text();

                var rsrcJson = "{\"ResourceId\":" + rsrc + ",\"Year\":\"" + yr + "\",\"Month\":\"" + idxarr[1] + "\",\"BaseManHours\":\"" + basemhrs + "\",\"CoreManHours\":\"" + coremhrs + "\",\"Comments\":\"" + cmts + "\",\"updateBy\":\"none\"}";
                recArr.push(rsrcJson);

            });

            $("input[id=baseval_" + yr + "_" + user + "_" + ent + "_" + rsrc + "]").val($("#frmRsrcEdit input[id=totBase]").val());
            $("input[id=coreval_" + yr + "_" + user + "_" + ent + "_" + rsrc + "]").val($("#frmRsrcEdit input[id=totCore]").val());

            for (i = 0; i < recArr.length; i++) {
                getAjxData('updateManHrs', ent, rsrc, cmts, yr, recArr[i]);
                //getAjxData(acttyp, ent, rsrc, cmts, yr, mhrs, rsrctitle, rsrcemail, rsrcphone, parentent)
            }

            calcTotals();
            editDiag.dialog("close");
        }
    }
function showRMHNotes(elem) {
    var idtxt = elem.id.split('_');

    getAjxData('rmhnotes', idtxt[3], idtxt[4], '', idtxt[1], '');
    //$("#dialog-modal").html(ajxret);


    //set dialog for notes
    noteDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 400,
        width: 700,
        modal: true,
        close: function () {

        }
    });


    //noteDiag.css("widget");
    noteDiag.dialog("open");

    //alert(idtxt);

}
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
function closeRsrcDialog() {
    if (rsrcDiag) {
        rsrcDiag.dialog("close");
    }
}
function closeEntDialog() {
    if (entDiag) {
        entDiag.dialog("close");
    }
}
function saveRsrcInfo() {
    if (infoDiag) {
        var rsrc = -1;
        var cmts = '';
        var rsrcTitle = '';
        var rsrcEmail = '';
        var rsrcPhone = '';

        $("#frmRsrcInfo").find("input").each(function () {
            if (this.id == 'rsrcId') {
                rsrc = this.value;
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

        getAjxData('updateRsrcInfo', '', rsrc, cmts, '', '',rsrcTitle,rsrcEmail,rsrcPhone);


        infoDiag.dialog("close");
    }
}

function saveRsrcComments() {
    if (noteDiag) {
        var rsrc = -1;
        var yr = -1;
        var cmts = '';
        $("#frmComments").find("input").each(function () {
            if(this.id == 'rsrcId')
            {
                rsrc = this.value;
            }
        });
        $("#frmComments").find("textarea").each(function () {
            if (this.id == 'ta_comments') {
                cmts = $(this).text();
            }
        });

        getAjxData('updateRsrcNotes', '',rsrc,cmts,'','');


        noteDiag.dialog("close");
    }
}
function saveRMHComments() {
    if (noteDiag) {
        var rsrc = -1;
        var yr = -1;
        var cmts = '';
        $("#frmComments").find("input").each(function () {
            if (this.id == 'rsrcId') {
                rsrc = this.value;
            }
            if (this.id == 'year') {
                yr = this.value;
            }
        });
        $("#frmComments").find("textarea").each(function () {
            if (this.id == 'ta_comments') {
                cmts = $(this).text();
            }
        });

        getAjxData('updateRMHNotes', '', rsrc, cmts, yr, '');


        noteDiag.dialog("close");
    }
}
function showInfo(elem) {
    var idtxt = elem.id.split('_');

    //set dialog for information

    getAjxData('info', idtxt[2], idtxt[3]);
    //$("#dialog-modal").html(ajxret);


    //set dialog for notes
     infoDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 500,
        width: 750,
        modal: true,
        close: function () {

        }
    });


    //infoDiag.css("widget");
    infoDiag.dialog("open");
    //alert(idtxt);
}

function getAjxData(acttyp, ent, rsrc, cmts, yr, mhrs, rsrctitle, rsrcemail, rsrcphone,parentent) {


    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminEntityResourcesHandler?RsrcId=" + rsrc + "&EntId=" + ent + "&ActTyp=" + acttyp + "&comments=" + cmts +  "&yr=" + yr + "&mhrs=" + mhrs + "&rsrcTitle=" + rsrctitle + "&rsrcEmail=" + rsrcemail + "&rsrcPhone=" + rsrcphone + "&parentent=" + parentent;
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


function setPageView(view)
{
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminEntityResourcesHandler?ActTyp=" + view;
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

            var divelm = document.getElementById("treeDiv");
            divelm.innerHTML = result;
            //assigns css to parent UL elements
            $('.tree li').each(function () {
                if ($(this).children('ul').length > 0) {
                    $(this).addClass('parent');
                }
            });

            //assigns the click function to display the UL giving the animation concept of expanding the node
            $('.tree li.parent > a').click(function () {
                $(this).parent().toggleClass('active');
                $(this).parent().children('ul').slideToggle('fast');

                //update localStorage with this id and active state
                if ($(this).parent().hasClass("active"))
                {
                    localStorage.setItem("probe.parent." + $(this).parent()[0].id, "active");
                }else
                {
                    localStorage.setItem("probe.parent." + $(this).parent()[0].id,"");
                }
                

            });

            //sets background of item that is a parent to hilite current node and identify all child nodes
            $('.tree li > div > input').focus(function () {
                $(this).parent().parent().css('background', 'lightblue');
            });

            //sets background of item that is a parent on losing focus back to normal
            $('.tree li > div > input').blur(function () {
                $(this).parent().parent().css('background', 'none');
            });

            //for the toggle all button to toggle the state of all node elements
            $('#expand').click(function () {

                $('.tree li').each(function () {
                    if ($(this).prop('class').indexOf('active') == -1) {
                        $(this).toggleClass('active');
                        $(this).children('ul').slideToggle('fast');
                        localStorage.setItem("probe.parent." + $(this)[0].id, "active");
                    }
                });
            });

            $('#collapse').click(function () {

                $('.tree li').each(function () {
                    if ($(this).prop('class').indexOf('active') > -1) {
                        $(this).toggleClass('active');
                        $(this).children('ul').slideToggle('fast');

                        localStorage.setItem("probe.parent." + $(this)[0].id, "");
                    }
                });
            });
            // initially call roll up function for every parent item.
            calcTotals();


            //on change for inputs to call the roll up function to populate parent totals of child elements
            $('.tree li > div > input[id^=baseval]').on('change', function () {
                //call update of the project resource BaseManHrs value
                var idtxt = $(this).prop('id').split('_');
                getAjxData('updateBaseManHrs', idtxt[2], idtxt[4], '', idtxt[1], $(this).val());
                calcTotals();
            });
            $('.tree li > div > input[id^=coreval]').on('change', function () {
                //call update of the project resource ActualManHrs value
                var idtxt = $(this).prop('id').split('_');
                getAjxData('updateCoreManHrs', idtxt[2],  idtxt[4], '', idtxt[1], $(this).val());
                calcTotals();
            });

            //set the onchange funciton for the checkboxes for Resources
            $('.tree input[type=checkbox]').on('change', function () {
                updateProjRsrc(this);
            });
            //set the click function for teh notes and information buttons
            $('.tree img[id^=rsrcnote]').click(function () {
                showRsrcNotes(this);
            });            
            $('.tree img[id^=edit]').click(function () {
                showEdit(this);
            });
            $('.tree img[id^=info]').click(function () {
                showInfo(this);
            });
            $('.tree img + a').click(function () {
                $(this).prev().click();
                //showInfo(this);
            });


            $('.tree img[id^=addEntity]').click(function () {
                getOpenEntities(this);
            });
            $('.tree img[id^=addRsrc]').click(function () {
                getOpenResources(this);
            });
            $('.tree img[id^=delEntity]').click(function () {
                remEntity(this);
            });
            $('.tree img[id^=delRsrc]').click(function () {
                
                remRsrc(this);
            });
            //set width of yrtitles divs to width of yearset div
            var yrsetWidth = $(".yearset")[0].childNodes.length * 100;
            $(".yrtitle").prop("width", yrsetWidth);


            //expand all active elements
            $('.tree li.parent').each(function () {


                    //TODO : check array of nodes in the local storage array and set this parent node active if found.
                    var lastState = localStorage.getItem("probe.parent." + $(this)[0].id);
                    if (lastState != null)
                    {
                        if (lastState == "active") {
                            $(this).toggleClass('active');
                            $(this).children('ul').slideToggle('fast');
                        }
                    }
                
            });
            //$('#expand').click();


        },
        failure: function (result) {
            alert(result);

        }
    });
}

function getOpenEntities(elem) {
    var idtxt = elem.id.split('_');

    getAjxData('getOpenEntityList', idtxt[1]);
    //$("#dialog-modal").html(ajxret);


    //set dialog for notes
    entDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 200,
        width: 400,
        modal: true,
        close: function () {

        }
    });


    //noteDiag.css("widget");
    entDiag.dialog("open");
}

function getOpenResources(elem) {
    var idtxt = elem.id.split('_');

    getAjxData('getOpenRsrcList', idtxt[1]);
    //$("#dialog-modal").html(ajxret);


    //set dialog for notes
    rsrcDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 200,
        width: 400,
        modal: true,
        close: function () {

        }
    });


    //noteDiag.css("widget");
    rsrcDiag.dialog("open");
}

function addEntity(elm) {

    var entId = $("#frmEntList > input[id=entId]").val();
   var parentId = $("#frmEntList > select").val();
    
   //getAjxData(acttyp, ent, rsrc, cmts, yr, mhrs, rsrctitle, rsrcemail, rsrcphone, parentent)
    getAjxData('addEntity', parentId,'','','','','','','',entId);
    closeEntDialog();

    setPageView('all');
}

function addRsrc(elm) {
    var entId = $("#frmRsrcList > input[id=entId]").val();
    var rsrcId = $("#frmRsrcList > select").val();
    //$(".loader").css('display', 'block');
    getAjxData('addRsrc', entId,rsrcId);
    closeRsrcDialog();

    setPageView('all');

    //alert('addRsrc');
}

function remEntity(elem) {
    var idtxt = elem.id.split('_');
        
    getAjxData('remEntity', idtxt[1]);

    setPageView('all');
   // alert('remEntity');
}

function remRsrc(elm) {
    var idxtxt = elm.id.split('_');

    var entId = idxtxt[2];
    var rsrcId = idxtxt[3];


    getAjxData('remRsrc', entId, rsrcId);


    setPageView('all');

}


function updateProjRsrc(rsrcElem) {
    var idtxt = $(rsrcElem).prop('id').split('_');
    if ($(rsrcElem).prop('checked') == false) {
        var retval = window.confirm('Are you sure you want to remove the Resource from this project?');
        //alert(retval);        
        if (retval) {            
            getAjxData('delResource', idtxt[1], '', idtxt[3], '', '');

            //clear proposed values
            $(rsrcElem).parent().find("input[id^=propval_]").each(function () {
                    $(this).val(0);
                });
            //clear actual values
            $(rsrcElem).parent().find("input[id^=actval_]").each(function () {
                    $(this).val(0);
                });

            calcTotals();
            if ($(".radioDiv").children("input:gt(0)").prop('checked'))
            {
                setPageView('project');
                $('#expand').click();
            }

        } else {

            $(rsrcElem).prop('checked', 'true');
        }
    }else
    {
        getAjxData('addResource', idtxt[1], '', idtxt[3], '', '');
                        
    }

}

function calcTotals() {
    $('.tree ul > li').each(function () {
        rollUpAllTotalElementValues(this);
    });
}


//roll up function to populate parent level totals of child totals elements
function rollUpAllTotalElementValues(parent) {

    //get the year totals inputs
    var baseInputs = $($(parent).children("div")).children("input[id^=basetot_]");
    var coreInputs = $($(parent).children("div")).children("input[id^=coretot_]");

    $(baseInputs).each(function () {
        var yrtxt = $(this).prop('id').split('_')[1];
        var basetot = 0;
        $(this).parent().parent().find("input[id^=baseval_" + yrtxt + "]").each(function () {
            basetot += parseInt($(this).val());
        });
        $(this).val(basetot);

    });

    $(coreInputs).each(function () {
        var yrtxt = $(this).prop('id').split('_')[1];
        var coretot = 0;
        $(this).parent().parent().find("input[id^=coreval_" + yrtxt + "]").each(function () {
            coretot += parseInt($(this).val());
        });

        $(this).val(coretot);

    });


    //get the ul children and call this function recursively
    $(this).children("ul > li").each(function () {
        rollUpAllTotalElementValues(this);
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


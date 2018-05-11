
    //var ajxret = "";
    $(document).ready(function () {

        setPageView('all');
        $("#loaderImg").css("display", "none");
    });


    var infoDiag;
    var editDiag;

function showEdit(elem) {
    var idtxt = elem.id.split('_');

    //getAjxData(acttyp, proj, ent, rsrc, cmts, yr, mhrs)

    getAjxData('edit', idtxt[2], idtxt[3], idtxt[4],'',idtxt[1],'');
    //$("#dialog-modal").html(ajxret);


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

     $("#frmRsrcEdit input[id=allProp]").on('change', function () {
         $("#frmRsrcEdit input[id^=prop_]").each(function () {
             $(this).val($("#frmRsrcEdit input[id=allProp]").val());
         });
         calcEditTotals();
     });
     $("#frmRsrcEdit input[id=allAct]").on('change', function () {
         $("#frmRsrcEdit input[id^=act_]").each(function () {
             $(this).val($("#frmRsrcEdit input[id=allAct]").val());
         });
         calcEditTotals();
     });

     $("#frmRsrcEdit input[id^=prop_").on('change',function () {
         calcEditTotals();
     });
     $("#frmRsrcEdit input[id^=act_").on('change',function () {
         calcEditTotals();
     });

     calcEditTotals();

     $("#frmRsrcEdit input[id^=prop_").first().focus();

    //alert(idtxt);

}
function calcEditTotals() {
    var proptot = 0;
    var acttot = 0;
    $("#frmRsrcEdit input[id^=prop_]").each(function () {
        proptot += parseInt($(this).val());
    });
    $("#frmRsrcEdit input[id^=act_]").each(function () {
        acttot += parseInt($(this).val());
    });

    $("#frmRsrcEdit input[id=totProposed]").val(proptot);
    $("#frmRsrcEdit input[id=totActual]").val(acttot);
}
function closeEditDialog() {
    if (editDiag) {
        editDiag.dialog("close");
    }
}
function closeInfoDialog() {
    if (infoDiag) {
        infoDiag.dialog("close");
    }
}
function saveRsrcEdits() {
    if (editDiag) {
        var rsrc = -1;
        var proj = -1;
        var yr = -1;
        var ent = -1;
        var cmts = '';
        proj = $("#frmRsrcEdit input[id=projId]").val();
        rsrc = $("#frmRsrcEdit input[id=rsrcId]").val();
        yr = $("#frmRsrcEdit input[id=year]").val();
        ent = $("#frmRsrcEdit input[id=entId]").val();

        var recArr = new Array();

        $("#frmRsrcEdit input[id^=prop_").each(function () {
            var cmts = '';
            var propmhrs = 0;
            var actmhrs = 0;

            var idxarr = this.id.split('_');
            propmhrs = $(this).val();
            actmhrs = $("#frmRsrcEdit input[id=act_" + idxarr[1] + "]").val();
            if (!actmhrs) {
                actmhrs = "0";
            }
            cmts = $("#frmRsrcEdit textarea[id=cmts_" + idxarr[1] + "]").text();

            var rsrcJson = "{\"ProjectId\":" + proj + ",\"ResourceId\":" + rsrc + ",\"Year\":\"" + yr + "\",\"Month\":\"" + idxarr[1] + "\",\"ProposedManHrs\":\"" + propmhrs + "\",\"ActualManHrs\":\"" + actmhrs + "\",\"Comments\":\"" + cmts + "\",\"EntityId\":\"0\",\"ResourceName\":\"none\",\"ResourceTitle\":\"none\",\"ResourceEmail\":\"none\",\"ResourcePhone\":\"none\",\"updateBy\":\"none\"}";
            recArr.push(rsrcJson);
            
        });

        $("input[id=propval_" + yr + "_" + proj + "_" + ent + "_" + rsrc + "]").val($("#frmRsrcEdit input[id=totProposed]").val());
        $("input[id=actval_" + yr + "_" + proj + "_" + ent + "_" + rsrc + "]").val($("#frmRsrcEdit input[id=totActual]").val());

        for (i = 0; i < recArr.length; i++) {
            getAjxData('updateEdits', proj, '', rsrc, cmts, yr, recArr[i]);
        }

        calcTotals();
        editDiag.dialog("close");
    }
}

function showInfo(elem) {
    var idtxt = elem.id.split('_');

    //set dialog for information

    getAjxData('info', idtxt[1], idtxt[2], idtxt[3]);
    //$("#dialog-modal").html(ajxret);



     infoDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 300,
        width: 500,
        modal: true,
        close: function () {

        }
    });


    //infoDiag.css("widget");
    infoDiag.dialog("open");
    //alert(idtxt);
}

function getAjxData(acttyp, proj, ent, rsrc, cmts, yr, mhrs) {
    var acturl = location.protocol + "//" + location.host + "/Probe/Home/ResourceHandler?ProjId=" + proj + "&RsrcId=" + rsrc + "&EntId=" + ent + "&ActTyp=" + acttyp + "&comments=" + cmts +  "&yr=" + yr + "&mhrs=" + mhrs;
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
                    //alert(result);
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
    var acturl = location.protocol + "//" + location.host + "/Probe/Home/ResourceHandler?ActTyp=" + view;
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
                    //$("#dialog-modal").html(result);
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
                if ($(this).parent().hasClass("active")) {
                    localStorage.setItem("probe.project.parent." + $(this).parent()[0].id, "active");
                } else {
                    localStorage.setItem("probe.project.parent." + $(this).parent()[0].id, "");
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
                        localStorage.setItem("probe.project.parent." + $(this)[0].id, "active");
                    }
                });
            });

            $('#collapse').click(function () {

                $('.tree li').each(function () {
                    if ($(this).prop('class').indexOf('active') > -1) {
                        $(this).toggleClass('active');
                        $(this).children('ul').slideToggle('fast');
                        localStorage.setItem("probe.project.parent." + $(this)[0].id, "");
                    }
                });
            });
            // initially call roll up function for every parent item.
            calcTotals();


            //on change for inputs to call the roll up function to populate parent totals of child elements
            $('.tree li > div > input[id^=propval]').on('change', function () {
                //call update of the project resource ProposedManHrs value
                var idtxt = $(this).prop('id').split('_');
                getAjxData('updateProposedManHrs', idtxt[2], '', idtxt[4], '', idtxt[1], $(this).val());
                calcTotals();
            });
            $('.tree li > div > input[id^=actval]').on('change', function () {
                //call update of the project resource ActualManHrs value
                var idtxt = $(this).prop('id').split('_');
                getAjxData('updateActualManHrs', idtxt[2], '', idtxt[4], '', idtxt[1], $(this).val());
                calcTotals();
            });

            //set the onchange funciton for the checkboxes for Resources
            $('.tree input[type=checkbox]').on('change', function () {
                updateProjRsrc(this);
            });
            //set the click function for the edit and information buttons
            $('.tree img[id^=edit]').click(function () {
                showEdit(this);
            });
            $('.tree img[id^=info]').click(function () {
                showInfo(this);
            });
            $('.tree input[type=checkbox] + img + a').click(function () {
                $(this).prev().click();
                //showInfo(this);
            });
            //set width of yrtitles divs to width of yearset div
            var yrsetWidth = $(".yearset")[0].childNodes.length * 100;
            $(".yrtitle").prop("width", yrsetWidth);

            $('.tree li.parent').each(function () {


                //TODO : check array of nodes in the local storage array and set this parent node active if found.
                var lastState = localStorage.getItem("probe.project.parent." + $(this)[0].id);
                if (lastState != null) {
                    if (lastState == "active") {
                        $(this).toggleClass('active');
                        $(this).children('ul').slideToggle('fast');
                    }
                }

            });


        },
        failure: function (result) {
            alert(result);

        }
    });
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
    var propInputs = $($(parent).children("div")).children("input[id^=proptot_]");
    var actInputs = $($(parent).children("div")).children("input[id^=acttot_]");

    $(propInputs).each(function () {
        var yrtxt = $(this).prop('id').split('_')[1];
        var proptot = 0;
        $(this).parent().parent().find("input[id^=propval_" + yrtxt + "]").each(function () {
            proptot += parseInt($(this).val());
        });
        $(this).val(proptot);

    });

    $(actInputs).each(function () {
        var yrtxt = $(this).prop('id').split('_')[1];
        var acttot = 0;
        $(this).parent().parent().find("input[id^=actval_" + yrtxt + "]").each(function () {
            acttot += parseInt($(this).val());
        });
        $(this).val(acttot);

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


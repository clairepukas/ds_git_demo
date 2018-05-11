
var phaseData = "";
var schedDtStart = ""; // new Date('@schedStart');
var schedDtEnd = "";//new Date('@schedEnd');

$.ajaxSetup({ cache: false });

function setTimespan(tspan) {
    document.getElementById('timespan').value = tspan;
    DrawGraph(parseInt(document.getElementById('zoom').value), parseInt(tspan));
}

function setZoom(zoom) {
    zoomVal = parseInt(document.getElementById('zoom').value);
    if (zoom == 'add') {
        zoomVal++;
    } else {
        zoomVal--;
    }
    DrawGraph(zoomVal, parseInt(document.getElementById('timespan').value));
}

function setHeader(xhr) { xhr.setHeader('Access-Control-Allow-Origin', '*'); }
function setErrorMsg(xhr, textStatus, errorThrown) { alert(errorThrown); }// { $("#putStringHere").val("Error encountered"); }

function DrawGraph(colw, celltimespan) {
    if (!colw) {
        colw = 20;
    } else {
        if (colw < 5 || colw > 28) {
            return;
        }
    }

    document.getElementById('zoom').value = colw;

   // var schedDtStart = new Date('@schedStart');
   // var schedDtEnd = new Date('@schedEnd');
    //var celltimespan = 1;
    var canvas = document.getElementById('phasesGraph');
    var gcontainer = document.getElementById('graphContainer');
    var ctx = canvas.getContext('2d');
    var columnCnt = Math.floor((schedDtEnd.getTime() - schedDtStart.getTime()) / 3600000 / 24 / celltimespan) + 2;


    //var colw = 14;
    var gridlft = ((colw / 2) * 20);
    var gridYtop = ((colw / 2) * 8);
    var gridYbottom = (Object.keys(phaseData).length) * (colw * 3) + gridYtop;
    var lineHeight = 10;

    canvas.width = (columnCnt * colw) + ((colw / 2) * 32);
    canvas.height = gridYbottom + 50;
    gcontainer.style.width = canvas.width + 20;
    gcontainer.style.height = canvas.height + 20;

    ctx.font = ((colw / 2) * 8) + "% Arial";
    ctx.lineWidth = 2;
    ctx.strokeStyle = "lightgray";


    //alert(canvas.width + " - " + gcontainer.style.width);
    //loop thru column count and draw verticals
    try {
        for (i = 0; i < columnCnt; i++) {
            ctx.beginPath();

            //draw vertical bar
            ctx.moveTo((i * colw) + gridlft + colw, gridYtop);
            ctx.lineTo((i * colw) + gridlft + colw, gridYbottom);
            ctx.stroke();

            //draw top year/month label
            var toplbl = "";
            //if ((i > 0) && ((i / 2) != i)) {
            //calculate year and month for label
            var startmils = schedDtStart.getTime();
            var addmils = i * (celltimespan * (3600000 * 24)); //days of milliseconds
            var curdt = new Date(startmils + addmils);
            var curyr = curdt.getFullYear();
            var curmo = curdt.getMonth() + 1;
            //toplbl = curmo + " / " + curyr;
            toplbl = (curdt.getMonth() + 1) + "/" + curdt.getDate() + "/" + curdt.getFullYear();


            ctx.save();
            var x = (i * colw) + gridlft + colw;
            ctx.translate(x, gridYtop - (colw / 2));
            ctx.rotate(-Math.PI / 4);
            ctx.textAlign = 'left';
            ctx.fillText(toplbl, 0, lineHeight / 2);
            ctx.restore();
            //}

        }
    } catch (err) {
        alert(err.message);
    }
    //alert(i + " - " +colw);

    //process the phases data for labels, grid lines and date lines

    var h = 0;
    for (var prop in phaseData) {
        if (phaseData.hasOwnProperty(prop)) {

            //draw horizontal grid lines
            ctx.beginPath();
            ctx.moveTo(gridlft + colw, gridYtop + (h * (colw * 3)));
            ctx.lineTo((colw * columnCnt) + gridlft, gridYtop + (h * (colw * 3)));
            ctx.strokeStyle = "lightgray";
            ctx.lineWidth = 2;
            ctx.stroke();

            // draw label
            ctx.beginPath();
            ctx.font = ((colw) * 8) + "% Arial";
            ctx.fillText(prop, (colw / 2), gridYtop + (h * (colw * 3)) + lineHeight + colw);


            //fill date span line
            ctx.beginPath();
            ctx.lineWidth = colw;

            var strtdt = new Date(phaseData[prop].start.valueOf());
            var enddt = new Date(phaseData[prop].end.valueOf());
            var clr = phaseData[prop].color.valueOf();
            ctx.strokeStyle = clr;

            var phasestrtpos = ((strtdt.getTime() - schedDtStart.getTime()) / 3600000 / 24 / celltimespan) * colw + colw;
            var phaselen = ((enddt.getTime() - strtdt.getTime()) / 3600000 / 24 / celltimespan) * colw;

            ctx.moveTo(gridlft + phasestrtpos, gridYtop + (h * (colw * 3)) + lineHeight + (colw - (colw / 5)));
            ctx.lineTo(gridlft + +phasestrtpos + phaselen, gridYtop + (h * (colw * 3)) + lineHeight + (colw - (colw / 5)));
            ctx.stroke();

            h++;
        }
    }

    //draw last horizontal grid line
    ctx.beginPath();
    ctx.moveTo(gridlft + colw, gridYtop + (h * (colw * 3)));
    ctx.lineTo((colw * columnCnt) + gridlft, gridYtop + (h * (colw * 3)));
    ctx.strokeStyle = "lightgray";
    ctx.lineWidth = 2;
    ctx.stroke();

    //draw first vertical
    ctx.beginPath();
    ctx.lineWidth = 5;
    ctx.strokeStyle = "green";
    ctx.moveTo((colw) + gridlft, gridYtop);
    ctx.lineTo((colw) + gridlft, gridYbottom);
    ctx.stroke();

    //draw last vertical
    ctx.beginPath();
    ctx.lineWidth = 5;
    ctx.strokeStyle = "red";
    ctx.moveTo((i * colw) + gridlft, gridYtop);
    ctx.lineTo((i * colw) + gridlft, gridYbottom);
    ctx.stroke();

    //draw current date vertical
    ctx.beginPath();
    ctx.lineWidth = colw;
    var nowpos = 0;
    //var nowdt = new Date();
    var nowmil = new Date().getTime();
    if (nowmil > schedDtEnd.getTime()) {
        nowmil = schedDtEnd.getTime();
    }
    if (nowmil < schedDtStart.getTime()) {
        nowmil = 0;
    }
    if (nowmil > 0) {
        nowpos = ((nowmil - schedDtStart.getTime()) / 3600000 / 24 / celltimespan) * colw + colw;

        ctx.lineWidth = 5;
        ctx.strokeStyle = "black";
        ctx.moveTo(nowpos + gridlft, gridYtop);
        ctx.lineTo(nowpos + gridlft, gridYbottom);
        ctx.stroke();
    }

}

function doPhaseUpdate()
{
    var acturl = location.protocol + "//" + location.host + "/Probe/Home/UpdatePhase"; ///?pid=" + pid + "&tbl='Projects" +  "&fld=" + phase + "&val=" + val;
    //alert(acturl);
    $.ajax({
        url: acturl,
        type: 'post',
        data: $('#phaseForm').serialize(),
        headers:'Access-Control-Allow-Origin,*',
        success: function (result) {
            //alert(result);
            if (result != '') {
                    
                if (result.indexOf('Error') == -1)
                {
                    getPhaseData();                        
                }

            }

        },
        failure: function (result) {
            alert(result);
        },
        //beforeSend: setHeader,
        error: setErrorMsg
    });
}

function getPhaseData()
{
    var acturl = location.protocol + "//" + location.host + "/Probe/Home/GetPhaseData?ProjId=" + document.getElementById('projId').value; 
    //alert(acturl);
    $.ajax({
        url: acturl,
        type: 'post',
        headers:'Access-Control-Allow-Origin,*',
        success: function (result) {
            if (result != '') {
                //alert(result);
                if (result.indexOf('Error') == -1)
                {
                    phaseData = eval(result);
                    DrawGraph(parseInt(document.getElementById('zoom').value), parseInt(document.getElementById('timespan').value));
                }

            }

        },
        failure: function (result) {
            alert(result);
        },
        //beforeSend: setHeader,
        error: setErrorMsg
    });
}

$(function () {
    $(".dpicker").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 3,
        onClose: function (selectedDate) {
            $("To").datepicker("option", "minDate", selectedDate);
            document.getElementById("editfld").value = this.id;
            doPhaseUpdate();
        }
    });

    //$("input[type='range']").change(new function () {
    //    if (this.id) {
    //        var outid = "#" + this.id + "lbl";
    //        document.querySelector(outid).value = this.value;
    //    }
    //});
});

function outputUpdate(elm, vol) {
    var obj = document.querySelector(elm);
    obj.innerText = vol + '%';
    document.getElementById(elm.substr(1)).innerText = vol + "%";

}
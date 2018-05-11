$(function () {    
    var rptfrmelem = document.getElementById("rptfrm");
    if (rptfrmelem) {
        resizeRptFrame();
    }
    //$('#rptfrm').load(function () {
    //    resizeRptFrame();
    //});
});


function resizeRptFrame() {
    var iFrames = $('#rptfrm');
    iFrames.load(function () {
        var ht = this.contentWindow.document.body.offsetHeight;
        if (ht < 600) {
            ht = 600;
        }
        this.style.height = (ht + 100) + 'px';
    });
}


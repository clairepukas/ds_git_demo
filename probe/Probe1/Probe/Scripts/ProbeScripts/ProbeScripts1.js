function updateFields(obj){
    var str= "/Probe/Home/UpdateProjectData";
    var id=$(obj).attr("id");

    var model={};
    model[id]=$(obj).val();

    model.ProjectId=@ViewBag.pid;
    model.warrantyAgreementDate=$("#warrantyAgreementDate").val();
    switch(id){
        case 'warrantyAgreement':
            if($(obj).val()=="" || $(obj).val()=="Unknown"){
                model.warrantyAgreementDate=null;
                model.warrantyAgreement=null;
                $("#warrantyAgreementDate").val("");
            }
            break;

        case 'warrantyAgreementDate':
            model.dbase="Projects";
            model.warrantyAgreement=$("#warrantyAgreement").val();
            break;
    }

    $.ajax({
        url: str,
        type: "POST",
        data: model,
        async: false
        //dataType: "json"
    }).success(function(data) {

    }).error(function (msg) {
        alert("There was an error while making update.");;
    });
}
﻿
function FilterProposed(srch, chk) {
    var returl = "";
    if (chk)
    { returl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/Proposed/?srch=" + srch + "&fltr=1"; }
    else
    { returl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/Proposed/?srch=" + srch + "&fltr=0"; }
    location.href = returl;
}

function FilterCompleted(srch) {
    var returl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/Completed/?srch=" + srch;
    location.href = returl;
}

function FilterApproved(srch) {
    var returl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/Approved/?srch=" + srch;
    location.href = returl;
}

function FilterCancelled(srch) {
    var returl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/Cancelled/?srch=" + srch;
    location.href = returl;
}

function FilterChangeLog(srch) {
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/FilterChangeLog/?srchtxt=" + srch;
    $.ajax({
        url: acturl,
        type: 'POST',
        async:false,
        success: function (result) {
            //alert("success");
            if (result.indexOf("Error") > -1) {
                alert(result);
            } else {
                $('.homeTables').html(result);
            }
        },
        failure: function (result) {
            //alert("Failed");
        }
    });
    
}

function UpdateImpact(projectid, impactid, name, chk) {
    if(chk.checked)
    {
        //Call Insert stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/AddImpact?ProjectId=" + projectid + "&ImpactId=" + impactid + "&ImpactName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");

            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
    else
    {
        //Call Delete stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/DelImpact?ProjectId=" + projectid + "&ImpactId=" + impactid + "&ImpactName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");
            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
}

function UpdateOtherImpact(projectid, impactid, name, chk) {

    var txtarea = document.getElementById("TextArea");
    var btn = document.getElementById("Save")

    if (chk.checked)
    {
        txtarea.disabled = false;
        btn.style.display = 'inline';

        //Call Insert stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/AddImpact?ProjectId=" + projectid + "&ImpactId=" + impactid + "&ImpactName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");
            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
    else
    {
        txtarea.disabled = true;
        txtarea.value = "";
        btn.style.display = 'none';

        //Call Delete stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/DelImpact?ProjectId=" + projectid + "&ImpactId=" + impactid + "&ImpactName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");
            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
}

function EditTextArea(projectid, impactid) {

    var txtarea = document.getElementById("TextArea");
    var val = txtarea.value;

    //Call Update stored procedure
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdImpact?ProjectId=" + projectid + "&ImpactId=" + impactid + "&desc=" + val;
    $.ajax({
        url: acturl,
        type: 'POST',
        success: function (result) {
            //alert("success");
        },
        failure: function (result) {
            //alert("Failed");
        }
    });
}

function UpdateBenefit(projectid, benefitid, name, chk) {
    if (chk.checked) {
        //Call Insert stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/AddBenefit?ProjectId=" + projectid + "&BenefitId=" + benefitid + "&BenefitName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");
            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
    else {
        //Call Delete stored procedure
        var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/DelBenefit?ProjectId=" + projectid + "&BenefitId=" + benefitid + "&BenefitName=" + name;
        $.ajax({
            url: acturl,
            type: 'POST',
            success: function (result) {
                //alert("success");
            },
            failure: function (result) {
                //alert("Failed");
            }
        });
    }
}

function UpdateCostSaving(projectid, CostSavingsTypeId, ths) {
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdateCostSaving?ProjectId=" + projectid + "&CostSavingsTypeId=" + CostSavingsTypeId + "&CostSaving=" + ths.value;
    $.ajax({
        url: acturl,
        type: 'POST',
        success: function (result) {
            //alert("success");
        },
        failure: function (result) {
            //alert("Failed");
        }
    });
}

function UpdateRisk(ele) {

    var typ = ele.name;
    var likelihood = 0;
    var impact = 0;

    var a = document.getElementsByName('LikelihoodRadios');
    for (var b = 0; b < a.length; b++)
        if (a[b].checked) {
            likelihood = a[b].value;
        }

    var c = document.getElementsByName('ImpactRadios');
    for (var d = 0; d < c.length; d++)
        if (c[d].checked) {
            impact = c[d].value;
        }

    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdateRisk?UpdateType=" + typ + "&Likelihood=" + likelihood + "&Impact=" + impact;
    $.ajax({
        url: acturl,
        type: 'POST',
        success: function (result) {
            //alert("success");
        },
        failure: function (result) {
            //alert("Failed");
        }
    });
}

function DisplayReport(btn) {

    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/SelectedReport?RptNm=" + btn.value;
    window.open(acturl, '_blank');
}

function SelectTab(tab) {
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/SelectedProject?SelectedTab=" + tab;
    window.location = acturl;
}

function ResetIndex() {
    var dataStore = window.sessionStorage;
    dataStore.setItem('key', 0);
}

function DenyAccess(access) {
    if (access == 'no')
    {
        var inputs = document.getElementsByTagName('input');
        for (var i = 0; i < inputs.length; i += 1)
            inputs[i].disabled = true;

        var inputs = document.getElementsByTagName('textarea');
        for (var i = 0; i < inputs.length; i += 1)
            inputs[i].disabled = true;

    }
}

function SetBudgetAccess(access) {

    var prms = access.split('|');
    var MonthlysInputs = document.getElementById('Monthlys').getElementsByTagName('input');
    var TotalsInputs = document.getElementById('Totals').getElementsByTagName('input');
    //CurrentBudgetedAmount


    var level;
    if($.inArray("Engineer", prms)==-1 && $.inArray("Manager", prms)==-1 && $.inArray("Sponsor", prms)==-1 && $.inArray("Admin", prms)==-1)
    {
        level = 1;//NO ACCESS
    }
    if ($.inArray("Engineer", prms) != -1 || $.inArray("Manager", prms) != -1 || $.inArray("Sponsor", prms) != -1) {
        level = 2;//LOGICAL ACCESS
    }
    if ($.inArray("Admin", prms) != -1) {
        level = 3;//ALL ACCESS
    }

    switch (level) {
        case 1:
            for (i = 0; i < MonthlysInputs.length; ++i) {
                MonthlysInputs[i].disabled = true;
            }
            break;
        case 2:
            //Already done when creating the dynamic sql
            for (i = 0; i < TotalsInputs.length; ++i) {
                if (TotalsInputs[i].id.indexOf("CurrentBudgetedAmount") > -1) {
                    if (TotalsInputs[i].id != "TotalsCurrentBudgetedAmount") {
                        TotalsInputs[i].disabled = false;
                    }
                    if (MonthlysInputs[i].id.indexOf("No Id") > -1) {
                        MonthlysInputs[i].disabled = true;
                    }
                }
            }
            break;
        case 3:
            
            for (i = 0; i < MonthlysInputs.length; ++i) {
                //MonthlysInputs[i].disabled = false;

                if (MonthlysInputs[i].id.indexOf("Totals") == -1) {
                    MonthlysInputs[i].disabled = false;
                }
                if (MonthlysInputs[i].id.indexOf("No Id") > -1) {
                    MonthlysInputs[i].disabled = true;
                }
            }
            for (i = 0; i < TotalsInputs.length; ++i) {
                if (TotalsInputs[i].id.indexOf("CurrentBudgetedAmount") > -1)
                {
                    if (TotalsInputs[i].id != "TotalsCurrentBudgetedAmount") {
                        TotalsInputs[i].disabled = false;
                    }
                    if (MonthlysInputs[i].id.indexOf("No Id") > -1) {
                        MonthlysInputs[i].disabled = true;
                    }
                }
            }
            break;
    }



}

function SetProposedBudgetAccess(access) {

    var prms = access.split('|');
    var MonthlysInputs = document.getElementById('Monthlys').getElementsByTagName('input');

    var level = 1;
    if ($.inArray("Creator", prms) != -1 || $.inArray("Engineer", prms) != -1 || $.inArray("Manager", prms) != -1 || $.inArray("Sponsor", prms) != -1 || $.inArray("Admin", prms) != -1) {
        level = 2;
    }

    switch (level) {
        case 1:
            for (i = 0; i < MonthlysInputs.length; ++i) {
                MonthlysInputs[i].disabled = true;
            }
            break;
        case 2:
            for (i = 0; i < MonthlysInputs.length; ++i) {
                //MonthlysInputs[i].disabled = false;
                if (MonthlysInputs[i].id.indexOf("Totals") != -1) {
                    MonthlysInputs[i].disabled = true;
                }
                if (MonthlysInputs[i].id.indexOf("No Id") > -1) {
                    MonthlysInputs[i].disabled = true;
                }
            }
            break;
    }
}

function runEffect(target) {
    $(target).toggle("Drop");
};

function updBudget(ele) {
    var prms = ele.title.split('|');
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdBudget?ProjectId=" + prms[0] + "&BudgetType=" + prms[1] + "&BudgetStatus=" + prms[2] + "&Amount=" + ele.value.replace("$", "").replace(",", "") + "&OldAmount=" + prms[3] + "&Year=" + prms[4] + "&Month=" + prms[5] + "&MonthName=" + prms[6];
    $.ajax({
        url: acturl,
        type: 'POST',
        async: false,
        success: function (result) {
            //alert("success");

            var newTooltip = prms[0] + "|" + prms[1] + "|" + prms[2] + "|" + ele.value.replace("$", "").replace(",", "") + "|" + prms[4] + "|" + prms[5] + "|" + prms[6];
            ele.title = newTooltip;

            var TotalsInputs = document.getElementById('Totals').getElementsByTagName('input');
            var AnnualsInputs = document.getElementById('Annuals').getElementsByTagName('input');
            var MonthlysInputs = document.getElementById('Monthlys').getElementsByTagName('input');

            //~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate TotalInputs
            //~~~~~~~~~~~~~~~~~~~~~~~
            var grandtot = 0;
            for (i = 0; i < TotalsInputs.length; ++i) {
                if (TotalsInputs[i].id != "Total") {
                    var srch = TotalsInputs[i].id;
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    var tot = 0
                    for (k = 0; k < MonthlysInputs.length; ++k) {
                        if (MonthlysInputs[k].id.indexOf(srch) > -1 && MonthlysInputs[k].id.indexOf("Total") == -1) {
                            tot += parseFloat(MonthlysInputs[k].value.replace("$","").replace(",",""));
                        }
                    }
                    TotalsInputs[i].value = "$" + tot.toFixed(2);
                    grandtot += tot;
                }
            }
            document.getElementById('Total').value = grandtot.toFixed(2);

            //~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate MonthlyTotals
            //~~~~~~~~~~~~~~~~~~~~~~~
            for (i = 0; i < MonthlysInputs.length; ++i) {
                if (MonthlysInputs[i].id.indexOf("Total|") != -1) {
                    var srch = MonthlysInputs[i].id.replace("Total|", "");
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    var tot = 0
                    for (k = 0; k < MonthlysInputs.length; ++k) {
                        if (MonthlysInputs[k].id.indexOf(srch) > -1 && MonthlysInputs[k].id.indexOf("Total") == -1) {
                            tot += parseFloat(MonthlysInputs[k].value.replace("$", "").replace(",", ""));
                        }
                    }
                    MonthlysInputs[i].value = "$" + tot.toFixed(2);
                }
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate AnnualInputs
            //~~~~~~~~~~~~~~~~~~~~~~~~
            for (i = 0; i < AnnualsInputs.length; ++i) {
                if (AnnualsInputs[i].id != "Total") {
                    var srch = AnnualsInputs[i].id;
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    var tot = 0
                    for (k = 0; k < MonthlysInputs.length; ++k) {
                        if (MonthlysInputs[k].id.indexOf(srch) > -1) {
                            tot += parseFloat(MonthlysInputs[k].value.replace("$", "").replace(",", ""));
                        }
                    }
                    AnnualsInputs[i].value = "$" + tot.toFixed(2);
                }
            }
            document.getElementById('Total').value = grandtot.toFixed(2);

            $('.currency').formatCurrency();

        },
        failure: function (result) {
            alert(result);
        }
    });

}


function updApprovedBudget(ele){

    var prms = ele.title.split('|');
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdBudget?ProjectId=" + prms[0] + "&BudgetType=" + prms[1] + "&BudgetStatus=" + prms[2] + "&Amount=" + ele.value.replace("$", "").replace(",", "") + "&OldAmount=" + prms[3] + "&Year=" + prms[4] + "&Month=" + prms[5] + "&MonthName=" + prms[6];
    $.ajax({
        url: acturl,
        type: 'POST',
        async:false,
        success: function (result) {
            //alert("success");

            var newTooltip = prms[0] + "|" + prms[1] + "|" + prms[2] + "|" + ele.value.replace("$", "").replace(",", "") + "|" + prms[4] + "|" + prms[5] + "|" + prms[6];
            ele.title = newTooltip;

            var TotalsInputs = document.getElementById('Totals').getElementsByTagName('input');
            var AnnualsInputs = document.getElementById('Annuals').getElementsByTagName('input');
            var MonthlysInputs = document.getElementById('Monthlys').getElementsByTagName('input');

            //~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate TotalInputs
            //~~~~~~~~~~~~~~~~~~~~~~~
            for (i = 0; i < TotalsInputs.length; ++i) {
                if (TotalsInputs[i].id.indexOf("Budgeted") != -1 || TotalsInputs[i].id.indexOf("Projected") != -1 || TotalsInputs[i].id.indexOf("Actual") != -1) {
                    var srch = TotalsInputs[i].id.replace("Totals", "");
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    if (TotalsInputs[i].id.indexOf("CurrentBudgeted") == -1) {
                        var tot = 0
                        for (k = 0; k < MonthlysInputs.length; ++k) {
                            if (MonthlysInputs[k].id.indexOf(srch) > -1 && MonthlysInputs[k].id.indexOf("Totals") == -1) {
                                tot += parseFloat(MonthlysInputs[k].value.replace("$", "").replace(",", ""));
                            }
                        }
                        TotalsInputs[i].value = "$" + tot.toFixed(2);
                    }
                }
            }

            //~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate MonthlyTotals
            //~~~~~~~~~~~~~~~~~~~~~~~
            for (i = 0; i < MonthlysInputs.length; ++i) {
                if (MonthlysInputs[i].id.indexOf("Totals|") != -1) {
                    var srch = MonthlysInputs[i].id.replace("Totals|", "");
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    var tot = 0
                    for (k = 0; k < MonthlysInputs.length; ++k) {
                        if (MonthlysInputs[k].id.indexOf(srch) > -1 && MonthlysInputs[k].id.indexOf("Totals") == -1) {
                            tot += parseFloat(MonthlysInputs[k].value.replace("$", "").replace(",", ""));
                        }
                    }
                    MonthlysInputs[i].value = "$" + tot.toFixed(2);
                }
            }


            //~~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate AnnualInputs
            //~~~~~~~~~~~~~~~~~~~~~~~~
            for (i = 0; i < AnnualsInputs.length; ++i) {
                //if (AnnualsInputs[i].id.indexOf("Budgeted") == -1) {
                    var srch = AnnualsInputs[i].id;
                    //Loop through MonthlysInputs and sum up input values where id contains srch var
                    var tot = 0
                    for (k = 0; k < MonthlysInputs.length; ++k) {

                        //Debug.writeln("srch:  " + srch);
                        //Debug.writeln("Monthly Input:  " + MonthlysInputs[k].id);
                        //Debug.writeln("Monthly Input Text:  " + MonthlysInputs[k].innerText);
                        //Debug.writeln("Monthly Input DefValue:  " + MonthlysInputs[k].defaultValue);
                        //Debug.writeln("Monthly Input Value:  " + MonthlysInputs[k].value);
                        //Debug.writeln("tot:  " + tot);
                        //Debug.writeln(" ----------------------------------------------------------- ");
                        //Debug.writeln(" ");

                        if (MonthlysInputs[k].id.indexOf(srch) > -1) {
                            tot += parseFloat(MonthlysInputs[k].value.replace("$", "").replace(",", ""));


                        }
                    }
                    AnnualsInputs[i].value = "$" + tot.toFixed(2);
                //}
            }
            $('.currency').formatCurrency();
        },
        failure: function (result) {
            alert(result);
        }
    });
}

function updCurrentBudget(ele) {

    var prms = ele.title.split('|');
    var acturl = "http://" + location.host + "/" + location.pathname.split("/")[1] + "/Home/UpdCurrentBudget?ProjectId=" + prms[0] + "&BudgetType=" + prms[1] + "&CurrentBudgetAmount=" + ele.value.replace("$", "").replace(",", "") + "&OldAmount=" + prms[2];
    $.ajax({
        url: acturl,
        type: 'POST',
        async:false,
        success: function (result) {
            //alert("success");

            var newTooltip = prms[0] + "|" + prms[1] + "|" + ele.value.replace("$", "").replace(",", "");
            ele.title = newTooltip;

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //Recalculate Total CurrentBudgetedAmount
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var TotalsInputs = document.getElementById('Totals').getElementsByTagName('input');
            var tot = 0
            for (i = 0; i < TotalsInputs.length; ++i) {
                if (TotalsInputs[i].id.indexOf("CurrentBudgetedAmount") != -1 && TotalsInputs[i].id != "TotalsCurrentBudgetedAmount") {
                    tot += parseFloat(TotalsInputs[i].value.replace("$", "").replace(",", ""));
                }
                document.getElementById("TotalsCurrentBudgetedAmount").value = tot;
            }
            $('.currency').formatCurrency();
        },
        failure: function (result) {
            alert(result);
        }
    });

}


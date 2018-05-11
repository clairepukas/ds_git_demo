
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

function AddUser() {
    if (infoDiag) {
        var userId = -1;
        var Active = 1;
        var UserName = '';
        var DisplayName = '';
        var FirstName = '';
        var LastName = '';
        var MiddleInitial = '';
        var Email = '';
        var Phone = '';
        var ResourceId = -1;

        $("#frmNewUser").find("input").each(function () {

            if (this.id == 'UserName') {
                UserName = this.value;
            }
            if (this.id == 'DisplayName') {
                DisplayName = this.value;
            }
            if (this.id == 'FirstName') {
                FirstName = this.value;
            }
            if (this.id == 'LastName') {
                LastName = this.value;
            }
            if (this.id == 'MiddleInitial') {
                MiddleInitial = this.value;
            }
            if (this.id == 'Email') {
                Email = this.value;
            }
            if (this.id == 'Phone') {
                Phone = this.value;
            }

        });

        $("#frmNewUser").find("select").each(function () {

            ResourceId = this.value;
        });
        if (ResourceId == "") {
            ResourceId = "null";
        }

        var userJson = "{\"UserId\":" + userId + ",\"active\":" + Active + ",\"UserName\":\"" + UserName + "\",\"DisplayName\":\"" + DisplayName + "\",\"FirstName\":\"" + FirstName + "\",\"LastName\":\"" + LastName + "\",\"MiddleInitial\":\"" + MiddleInitial + "\",\"Email\":\"" + Email + "\",\"Phone\":\"" + Phone + "\",\"ResourceId\":" + ResourceId + "}";



        getAjxData('addUser', userId, userJson);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function saveUserInfo() {
    if (infoDiag) {
        var userId = -1;
        var Active = 0;
        var UserName = '';
        var DisplayName = '';
        var FirstName = '';
        var LastName = '';
        var MiddleInitial = '';
        var Email = '';
        var Phone = '';
        var ResourceId = -1;

        $("#frmUserInfo").find("input").each(function () {

            if (this.id == 'UserId') {
                userId = this.value;
            }
            if (this.id == 'Active') {
                if (this.checked) {
                    Active = "1";
                } else {
                    Active = "0";
                }                
            }
            if (this.id == 'UserName') {
                UserName = this.value;
            }
            if (this.id == 'DisplayName') {
                DisplayName = this.value;
            }
            if (this.id == 'FirstName') {
                FirstName = this.value;
            }
            if (this.id == 'LastName') {
                LastName = this.value;
            }
            if (this.id == 'MiddleInitial') {
                MiddleInitial = this.value;
            }
            if (this.id == 'Email') {
                Email = this.value;
            }
            if (this.id == 'Phone') {
                Phone = this.value;
            }

        });

        $("#frmUserInfo").find("select").each(function () {

            ResourceId = this.value;
        });
        if (ResourceId == "") {
            ResourceId = "null";
        }

        var userJson = "{\"UserId\":" + userId + ",\"active\":" + Active + ",\"UserName\":\"" + UserName + "\",\"DisplayName\":\"" + DisplayName + "\",\"FirstName\":\"" + FirstName + "\",\"LastName\":\"" + LastName + "\",\"MiddleInitial\":\"" + MiddleInitial + "\",\"Email\":\"" + Email + "\",\"Phone\":\"" + Phone + "\",\"ResourceId\":" + ResourceId + "}";

       

        getAjxData('updateUserInfo', userId, userJson);


        infoDiag.dialog("close");

        setPageView('list');
    }
}

function saveUserRoles()
{
    if (infoDiag) {
        var userId = -1;
        var userJson = "[";

        $("#frmUserRoles").find("input").each(function () {
            if (this.id.indexOf("hasRole_") > -1) {
                idtxt = this.id.split("_");
                if (this.checked) {
                    userJson += "{\"UserId\":" + idtxt[1] + ",\"RoleId\":" + idtxt[2] + "},";
                }
            }
        });

        userId = $("#UserId").val();
        userJson = userJson.substring(0, userJson.length - 1) + "]";
       
        getAjxData('updateUserRoles', userId, userJson);

        infoDiag.dialog("close");

    }
}


function showInfo(elem) {
    var idtxt = elem.id.split('_')[1];
    if (elem.id.indexOf("Roles") > -1)
    {
        getAjxData('roles', idtxt);
    }

    if (elem.id == "addUser")
    {
            getAjxData('new');
    }

    if (elem.id.indexOf("editUser_") > -1)
    {
            getAjxData('info', idtxt);
    }
    
    //set dialog for resoure information
     infoDiag = $("#dialog-modal").dialog({
        autoOpen: false,
        height: 500,
        width: 900,
        modal: true,
        close: function () {

        }
    });

     infoDiag.dialog("open");

     $("#btncancel").focus();

}

function getAjxData(acttyp, userId, userJson, sort) {
    
    sort = $("#sort").val();

    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminUsersHandler?ActTyp=" + acttyp + "&userId=" + userId + "&userJson="+userJson + "&sort=" + sort;
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
    var acturl = location.protocol + "//" + location.host + "/Probe/Admin/AdminUsersHandler?ActTyp=" + view + "&sort=" + sort;
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
            $('img[id^=deactUser_]').on('click', function () {
                DelUser(this);
            });
            $('img[id^=actUser_]').on('click', function () {
                DelUser(this);
            });

            $('img[id^=editUser_]').on('click', function () {
                showInfo(this);
            });

            $('img[id^=editUserRoles_]').on('click', function () {
                showInfo(this);
            });

            ////set the click function for add Resource button
            $('.btnUserAdd').on('click', function () {
                showInfo(this);
            });            

            $(".tblUsers td").each(function () {
                if ($(this).text() == 'no') {
                    $(this).parent().css('background', 'pink');
                }
            });


        },
        failure: function (result) {
            alert(result);

        }
    });
}



function DelUser(userElem) {
    var idtxt = $(userElem).prop('id').split('_')[1];
    var retval = false;
    if ($(userElem).prop('id').indexOf('deactUser') > -1) {
        retval = window.confirm('Are you sure you want to set this User to Inactive?');
    }
    if ($(userElem).prop('id').indexOf('actUser') > -1) {
        retval = window.confirm('Are you sure you want to set this User to Active?');
    }
        //alert(retval);        
        if (retval)
        {

            getAjxData('delUser', idtxt);

            setPageView('list');
        } 
    

}









var webUrl = '/AreaMaster/';
var CommonUrl = '/AreaMaster/';

function NewEntry() {
    $("#Active").prop('checked', true);
    $(".form-control").val('').trigger("change");
    $.ajax({
        url: webUrl + "NewEntry",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            $("#AreaMasterList").hide();
            $("#AreaMasterForm").show();
            $("#Code").val(data['nextcode'])
            $("#State").html(data.state)
            $("#Code").prop("readonly", true);
            $("#Description").focus();

        }
    });
}

function CloseEntry() {
    if ($("#AreaMasterList").is(":visible")) {
        window.location.href = "/Home/Index";  // Update this URL as per your needs
    }
    else {
        // Otherwise, reset the form fields and clear content
        $("#AreaMasterList").show();
        $("#AreaMasterForm").hide();

        $("#ID").val('');
        $("#Code").val('');
        $("#Description").val('');
        $("#ArDescription").val('');
        $("#Active").prop('checked', false);
        $("#State").val('')
        // Reset UI elements
        $("#liNew").show();
        $("#liSave").hide();
        $("#liDelete").hide();
        $("#refreshIcon").hide();
    }

}

function SaveEntry() {

    if ($("#Description").val() == '') {
        $("#Description").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Area is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    //if ($("#ArDescription").val() == '') {
    //    $("#ArDescription").focus();
    //    Swal.fire({
    //        icon: 'error',
    //        title: 'Oops!',
    //        text: "Arabic Description is mandatory",
    //        confirmButtonText: 'Okay',
    //        confirmButtonColor: '#d33',
    //    });
    //    return false
    //}
    if ($("#State").val() == '') {
        $("#State").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "State is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }
    if ($("#Active").is(":checked")) {
        var Active = true;
    } else {
        var Active = false
    }
    var model = {
        'ID': $("#ID").val(),
        'code': $("#Code").val(),
        'Description': $("#Description").val(),
        'ArDescription': $("#ArDescription").val(),
        'StateID': $("#State").val(),
        'Active': Active,
    }
    $.ajax({
        url: webUrl + "SaveEntry",
        method: "POST",
        dataType: "JSON",
        data: model,
        beforeSend: function () {
            $(".loader-wrapper").fadeIn("fast"); // will work if it's not removed
        },
        success: function (data) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide(); // keep it for reuse
            });

            if (data.success) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: "Saved successfully!",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }).then(() => {
                    var menuId = $("#MenuID").val();
                    window.location.href = "/AreaMaster/Index?MenuID=" + menuId;
                });

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops!',
                    text: data.message,
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#d33',
                });
            }
        },

        error: function () {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
            });

            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Something went wrong while saving.',
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
        }
    });
}


function RowClick(RowID) {
    $(".form-control").val('');
    EntryEnable('Edit');
    $("#AreaMasterList").hide();
    $("#AreaMasterForm").show();
    $.ajax({
        url: webUrl + "RowClick?id=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#Description").val(data.description)
                $("#ArDescription").val(data.ardescription)
                $("#State").html(data.state)
                $("#ID").val(data.id)
                $("#Code").val(data.code)
                $("#Code").prop("readonly", true);
                if (data.active == "True") {
                    $("#Active").prop('checked', true);
                }
                else {
                    $("#Active").prop('checked', false);
                }
                $("#Description").focus();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops!',
                    text: "Data not fetched",
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#d33',
                });
            }
        },
    });
}



function DeleteEntry() {
    if ($("#ID").val() == null) {
        return false
    }
    $.ajax({
        url: CommonUrl + "Delete?id=" + $("#ID").val(),
        method: "DELETE",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: "Deleted successfully!",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }).then(() => {
                    var menuId = $("#MenuID").val();
                    window.location.href = "/AreaMaster/Index?MenuID=" + menuId;
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops!',
                    text: data['message'],
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#d33',
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: 'An error occurred while deleting.',
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
        }
    });
}
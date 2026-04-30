var webUrl = '/ArticleMaster/';
var CommonUrl = '/ArticleMaster/';

function NewEntry() {
    $("#Active").prop('checked', true);
    $(".form-control").val('').trigger("change");
    $("#ArticleMasterList").hide();
    $("#ArticleMasterForm").show();
    $("#Value").focus();
    //$.ajax({
    //    url: webUrl + "NewEntry",
    //    method: "POST",
    //    dataType: 'JSON',
    //    success: function (data) {
    //        $("#ArticleMasterList").hide();
    //        $("#ArticleMasterForm").show();
    //        $("#Code").val(data['nextcode'])
    //        $("#Code").prop("readonly", true);
    //        $("#Value").focus();

    //    }
    //});
}

function CloseEntry() {
    if ($("#ArticleMasterList").is(":visible")) {
        window.location.href = "/Home/Index";  // Update this URL as per your needs
    }
    else {
        // Otherwise, reset the form fields and clear content
        $("#ArticleMasterList").show();
        $("#ArticleMasterForm").hide();

        $("#ID").val('');
        $("#Code").val('');
        $("#Value").val('');
        $("#Description").val('');
        $("#Active").prop('checked', false);

        // Reset UI elements
        $("#liNew").show();
        $("#liSave").hide();
        $("#liDelete").hide();
        $("#refreshIcon").hide();
    }

}

function SaveEntry() {

    if ($("#Value").val() == '') {
        $("#Value").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Value is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#Description").val() == '') {
        $("#Description").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Description is mandatory",
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
        'Value': $("#Value").val(),
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
                    window.location.href = "/ArticleMaster/Index?MenuID=" + menuId;
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
    $("#ArticleMasterList").hide();
    $("#ArticleMasterForm").show();
    $.ajax({
        url: webUrl + "RowClick?id=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#Description").val(data.description)
                $("#Value").val(data.value)
                $("#ID").val(data.id)
                $("#Code").val(data.code)
                $("#Code").prop("readonly", true);

                if (data.active == "True") {
                    $("#Active").prop('checked', true);
                }
                else {
                    $("#Active").prop('checked', false);
                }
                $("#Value").focus();
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
                    window.location.href = "/ArticleMaster/Index?MenuID=" + menuId;
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
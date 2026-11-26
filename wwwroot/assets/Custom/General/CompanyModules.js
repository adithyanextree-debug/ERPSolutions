
var webUrl = '/CompanyModules/';
var CommonUrl = '/CompanyModules/';

function NewEntry() {
    $(".form-control").val('').trigger("change");
    $("#CompanyModulesList").hide();
    $("#CompanyModulesForm").show();
    $("#ID").val("");
    //$("#ClickRoles").show();
    $.ajax({
        url: webUrl + "NewEntry",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            if (data.success == true) {
                $("#CompanyID").html(data.company);
                $("#ModuleList").html(data.modules);
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops!',
                    text: data.message,
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#d33',
                });
            }
        }
    });
}

function CloseEntry() {
    location.reload();
}

function SaveEntry() {
    if ($("#CompanyID").val() == '') {
        $("#CompanyID").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "Company is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }

    var moduleIDs = $(".ModuleCheck:checked").map(function () {
        return parseInt($(this).val());
    }).get();

    if (moduleIDs.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "Please select at least one module",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }

    var model = {
        CompanyID: parseInt($("#CompanyID").val()),
        ModuleIDs: moduleIDs
    };

    $.ajax({
        url: webUrl + "SaveEntry",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(model),
        beforeSend: function () {
            $(".loader-wrapper").fadeIn("fast");
        },
        success: function (data) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
            });

            if (data.success) {
                Swal.fire("Success", data.message, "success").then(function () {
                    window.location.href = "/CompanyModules/Index?MenuID=" + $("#MenuID").val();
                });
            } else {
                Swal.fire("Error", data.message, "error");
            }
        },
        error: function (xhr) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
            });

            Swal.fire(
                "Error",
                xhr.responseJSON?.message || "Unable to save company modules.",
                "error"
            );
        }
    });
}
function RowClick(RowID) {
    $(".form-control").val('');
    $(".ModuleCheck").prop("checked", false);

    EntryEnable('Edit');
    $("#CompanyModulesList").hide();
    $("#CompanyModulesForm").show();

    $.ajax({
        url: webUrl + "RowClick?ID=" + RowID,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success == true) {

                $("#ID").val(data.id);
                $("#CompanyID").html(data.company);

                // Modules returned by RowClick are already assigned and checked
                $("#ModuleList").html(data.modules)

                UpdateModuleCount();

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops!',
                    text: "Data not fetched",
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#d33'
                });
            }
        }
    });
}
function DeleteEntry() {
    if ($("#ID").val() == null || $("#ID").val() == "") {
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
                    window.location.href = "/CompanyModules/Index?MenuID=" + menuId;
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

function SearchModules() {
        var search = $("#ModuleSearch").val().toLowerCase();

    $("#ModuleList .module-item").each(function () {
            var moduleName = $(this).find("label").text().toLowerCase();
    $(this).toggle(moduleName.includes(search));
        });
}

function UpdateModuleCount() {
        var count = $(".ModuleCheck:checked").length;
    $("#SelectedModuleCount").text(count + " modules selected");
}


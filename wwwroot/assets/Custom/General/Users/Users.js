var webUrl = '/Users/';
var CommonUrl = '/Users/';

function NewEntry() {
    $("#Active").prop('checked', true);
    $(".form-control").val('').trigger("change");
    $("#UsersList").hide();
    $("#UsersForm").show();
    $("#ID").val("");
    $("#Party").attr("data-idvalue", "");
    //$("#ClickRoles").show();
    $.ajax({
        url: webUrl + "NewEntry",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            if (data.success == true) {
                $("#BranchTableTbody").html(data.branchtable);
                $("#Designation").html(data.designation);
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

$(document).on("click", ".addrow", function (event) {
    event.preventDefault();
    var serialno = $(this).attr('serialno');
    var newRowKey = GetNextBranchRowKey();
    $.ajax({
        url: webUrl + "NewRow?Sn=" + newRowKey,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data.success == true) {
                var newrow = data['newrow'];
                $("#BranchTableTbody").append(newrow);
            }
        },
    });
});

$(document).on("click", ".action_delete", function (event) {
    event.preventDefault();
    var $Row = $(this).closest('tr');
    var $TableBody = $Row.closest('tbody');
    var rowCount = $TableBody.find("tr.detail-row").length;
    // Prevent deleting if only one row is left
    if (rowCount <= 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Warning!',
            text: 'You cannot delete the last remaining row. If any changes needed please update the data',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return;
    }

    var id = $(this).attr('element-id');
    var itemid = $("#detailsid" + id).val();

    Swal.fire({
        title: "Are you sure?",
        text: "Do you wanna make to remove it?",
        icon: "warning",
        showCancelButton: true, 
        confirmButtonColor: '#1c7430',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: "No, forget it."
    }).then((result) => {
        if (result.isConfirmed) {
            if (itemid != null && itemid !== "") {
                // Existing saved row - delete through AJAX
                $.ajax({
                    url: CommonUrl + "DeleteEmployeeDetails?ID=" + itemid,
                    method: "DELETE",
                    dataType: "JSON",
                    success: function (data) {
                        if (data['success'] === true) {
                            // Remove the permissions row first
                            $Row.next("tr.rights-row").remove();
                            // Then remove the branch details row
                            $Row.remove();
                            ReindexRows();
                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'success',
                                title: data['message'],
                                showConfirmButton: false,
                                timer: 3000,
                                timerProgressBar: true
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
                            text: 'Unable to delete branch details.',
                            confirmButtonText: 'Okay',
                            confirmButtonColor: '#d33',
                        });
                    }
                });

            } else {
                //  New unsaved row - just remove from DOM
                $Row.next("tr.rights-row").remove();
                $Row.remove();
                ReindexRows();
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: "Entry Deleted Successfully!",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                });
            }
        }
    });
});
function ReindexRows() {
    $("#BranchTableTbody > tr.detail-row").each(function (index) {
        $(this).find("td.serial-no").text(index + 1);
    });
}
function GetNextBranchRowKey() {
    var maxKey = 0;
    $("#BranchTableTbody > tr.detail-row").each(function () {
        var key = parseInt($(this).attr("data-rowkey")) || 0;
        if (key > maxKey) {
            maxKey = key;
        }
    });
    return maxKey + 1;
}

function ClickRoles(id) {

    var branchid = $("#Branch" + id).attr("data-idvalue");

    if (!branchid) {
        Swal.fire({
            icon: "warning",
            title: "Oops!",
            text: "Please select a branch first.",
            confirmButtonText: "Okay",
            confirmButtonColor: "#d33"
        });

        return false;
    }

    $.ajax({
        url: webUrl + "GetRoles?branchid=" + branchid,
        method: "POST",
        dataType: "JSON",

        success: function (data) {

            if (data.success === true) {

                var selectedRole = $("#Roles" + id).val();

                $("#Roles" + id).html(data.roles);

                if (selectedRole) {
                    $("#Roles" + id).val(selectedRole);
                }

                var $row = $("#RoleRow" + id);
                var $icon = $("#ClickRoles" + id + " i");

                if ($row.is(":visible")) {
                    $row.slideUp(200);
                    $icon.removeClass("fa-chevron-up").addClass("fa-chevron-down");
                } else {
                    $row.slideDown(200);
                    $icon.removeClass("fa-chevron-down").addClass("fa-chevron-up");
                }
            }
        },

        error: function (xhr) {
            console.error(xhr.responseText);

            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Unable to load roles for this branch."
            });
        }
    });
}

$(document).on("change", ".branch-role", function () {
    const id = $(this).attr("element-id");
    const roleId = $(this).val();

    const element = document.getElementById("Roles" + id);

    if (element) {
        element.scrollIntoView();
    }

    $("#RoleRow" + id).show();

    if (!roleId) {
        $("#roletable" + id).empty();
        return;
    }

    $.ajax({
        url: webUrl + "Roles?id=" + roleId + "&rowKey=" + id,
        method: "POST",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#roletable" + id).html(data.table);
            }
        }
    });
});

function ChangeRoleStatus(id) {

    var isview = "#IsView" + id;

    if ($(isview).is(":checked")) {
        var isviewflag = true;

    }
    else {
        var isviewflag = false;

    }

    var iscreate = "#IsCreateActive" + id;
    if ($(iscreate).is(":checked")) {
        var iscreateflag = true;
    }
    else {
        var iscreateflag = false;
    }

    var isedit = "#IsEdit" + id;
    if ($(isedit).is(":checked")) {
        var iseditflag = true;
    }
    else {
        var iseditflag = false;
    }
    var iscancel = "#IsCancel" + id;
    if ($(iscancel).is(":checked")) {
        var iscancelflag = true;
    }
    else {
        var iscancelflag = false;
    }
    var isdelete = "#IsDelete" + id;
    if ($(isdelete).is(":checked")) {
        var isdeleteflag = true;
    }
    else {
        var isdeleteflag = false;
    }
    var isapprove = "#IsApprove" + id;
    if ($(isapprove).is(":checked")) {
        var isapproveflag = true;
    }
    else {
        var isapproveflag = false;
    }
    var iseditapproved = "#IsEditApproved" + id;
    if ($(iseditapproved).is(":checked")) {
        var iseditapprovedflag = true;
    }
    else {
        var iseditapprovedflag = false;
    }
    var ishigherapprove = "#IsHigherApprove" + id;
    if ($(ishigherapprove).is(":checked")) {
        var ishigherapproveflag = true;
    }
    else {
        var ishigherapproveflag = false;
    }
    var isprint = "#IsPrint" + id;
    if ($(isprint).is(":checked")) {
        var isprintflag = true;
    }
    else {
        var isprintflag = false;
    }
    var ismail = "#IsEmail" + id;
    if ($(ismail).is(":checked")) {
        var ismailflag = true;
    }
    else {
        var ismailflag = false;
    }
    var userrights = {
        'ID': id,
        'IsView': isviewflag,
        'IsCreate': iscreateflag,
        'IsEdit': iseditflag,
        'IsCancel': iscancelflag,
        'IsDelete': isdeleteflag,
        'IsApprove': isapproveflag,
        'IsEditApproved': iseditapprovedflag,
        'IsHigherApprove': ishigherapproveflag,
        'IsPrint': isprintflag,
        'IsMail': ismailflag
    }
    console.log(userrights)
    $.ajax({
        url: webUrl + "ChangeRolesStatus?id=" + id,
        method: "POST",
        dataType: 'JSON',
        data: userrights,
        success: function (data) {
            if (data.success == true) {
                $(".AlertDivSuccess").show();
                $(".AlerSuccessStatus").show();
                $(".AlerSuccessStatus").html('Success!');
                $(".AlertDivSuccess").html(data.message);
                setTimeout(function () {
                    $(".AlertDivSuccess").hide();
                }, 1000);
            }
        }

    });
}

function CloseEntry() {
    location.reload();
}

function SaveEntry() {
    if ($("#FirstName").val() == '') {
        $("#FirstName").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "FirstName is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#LastName").val() == '') {
        $("#LastName").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "LastName is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#MobileNo").val().trim() == '') {
        $("#MobileNo").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "MobileNo. is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }

    if (!/^\d+$/.test($("#MobileNo").val().trim())) {
        $("#MobileNo").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "Please enter digits only for MobileNo.",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }

    if ($("#Username").val() == '') {
        $("#Username").focus();
        Swal.fire({
            icon: 'warning',
            title: 'Oops!',
            text: "Username is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    var mainBranchCount = $("#BranchTableTbody > tr.detail-row")
        .find(".IsMainBranch:checked").length;
    if (mainBranchCount !== 1) {
        Swal.fire({
            icon: "warning",
            title: "Oops",
            text: "Please select exactly one branch as the Main Branch."
        });
        return;
    }
    //var rawParty = $("#Party").attr("data-idvalue");
    //var details = [];
    //var rights = [];

    ////$("#BranchTable tbody tr").each(function () {
    //$("#BranchTable tbody tr.detail-row").each(function () {
    //    var $row = $(this);
    //    var rowKey = details.length + 1;

    //    var branchID = parseInt($row.find("[id^='Branch']").attr("data-idvalue")) || null;
    //    var departmentID = parseInt($row.find("[id^='Department']").attr("data-idvalue")) || null;
    //    var supervisorID = parseInt($row.find("[id^='Supervisor']").attr("data-idvalue")) || null;

    //    details.push({
    //        ID: parseInt($row.attr("data-id")) || 0,
    //        DetailRowKey: rowKey,
    //        BranchID: branchID,     
    //        DepartmentID: departmentID,
    //        SuperVisorID: supervisorID,
    //        //MaRoleID: parseInt($("#Roles").val()) || null,
    //        MaRoleID: parseInt($row.find(".branch-role").val()) || null,
    //        ActiveFlag: $row.find("[id^='IsActive']").is(":checked"),
    //        IsMainBranch: $row.find("[id^='IsMainBranch']").is(":checked")
    //    });

    //    $("#roletable tbody tr[data-pagemenuid]").each(function () {
    //        var $permissionRow = $(this);
    //        var pageMenuID = parseInt($permissionRow.attr("data-pagemenuid"));

    //        rights.push({
    //            //ID: parseInt($permissionRow.attr("data-id")) || 0,
    //            DetailRowKey: rowKey,
    //            PageMenuID: pageMenuID,
    //            IsView: $permissionRow.find("#IsView" + pageMenuID).is(":checked"),
    //            IsCreate: $permissionRow.find("#IsCreate" + pageMenuID).is(":checked"),
    //            IsEdit: $permissionRow.find("#IsEdit" + pageMenuID).is(":checked"),
    //            IsCancel: $permissionRow.find("#IsCancel" + pageMenuID).is(":checked"),
    //            IsDelete: $permissionRow.find("#IsDelete" + pageMenuID).is(":checked"),
    //            IsApprove: $permissionRow.find("#IsApprove" + pageMenuID).is(":checked"),
    //            IsEditApproved: $permissionRow.find("#IsEditApproved" + pageMenuID).is(":checked"),
    //            IsHigherApprove: $permissionRow.find("#IsHigherApprove" + pageMenuID).is(":checked"),
    //            IsPrint: $permissionRow.find("#IsPrint" + pageMenuID).is(":checked"),
    //            IsEmail: $permissionRow.find("#IsEmail" + pageMenuID).is(":checked"),
    //            IsExcelUpload: false
    //        });
    //    });
    //});

    var rawParty = $("#Party").attr("data-idvalue");
    var details = [];
    var rights = [];
    var validationFailed = false;

    $("#BranchTableTbody > tr.detail-row").each(function () {

        var $row = $(this);

        // Get the paired permissions row
        var $rightsRow = $row.next("tr.rights-row");

        if (!$rightsRow.length) {
            console.error("Permissions row missing for branch:", $row);
            validationFailed = true;
            return false;
        }

        // Get the unique row key from RoleRow1, RoleRow2, etc.
        var roleRowID = $rightsRow.attr("id");
        var rowKey = parseInt(roleRowID.replace("RoleRow", ""));

        if (!rowKey) {
            console.error("Invalid branch row key:", roleRowID);
            validationFailed = true;
            return false;
        }

        // Get role from this branch's dropdown
        var $roleDropdown = $rightsRow.find(".branch-role");
        var roleID = parseInt($roleDropdown.val()) || null;

        if (!roleID) {
            Swal.fire({
                icon: "warning",
                title: "Oops!",
                text: "Please select a role for every branch."
            });

            $rightsRow.show();
            $roleDropdown.focus();

            validationFailed = true;
            return false;
        }

        var branchID = parseInt(
            $row.find("[id^='Branch']").attr("data-idvalue")
        ) || null;

        var departmentID = parseInt(
            $row.find("[id^='Department']").attr("data-idvalue")
        ) || null;

        var supervisorID = parseInt(
            $row.find("[id^='Supervisor']").attr("data-idvalue")
        ) || null;

        // Collect branch details
        details.push({
            ID: parseInt($row.attr("data-id")) || 0,
            DetailRowKey: rowKey,
            BranchID: branchID,
            DepartmentID: departmentID,
            SuperVisorID: supervisorID,
            MaRoleID: roleID,
            ActiveFlag: $row.find("[id^='IsActive']").is(":checked"),
            IsMainBranch: $row.find("[id^='IsMainBranch']").is(":checked")
        });

        // Collect rights only from this branch's permissions table
        $("#roletable" + rowKey + " tbody tr[data-pagemenuid]").each(function () {

            var $permissionRow = $(this);
            var pageMenuID = parseInt(
                $permissionRow.attr("data-pagemenuid")
            );

            rights.push({
                DetailRowKey: rowKey,
                PageMenuID: pageMenuID,
                IsView: $permissionRow.find("[id='IsView" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsCreate: $permissionRow.find("[id='IsCreate" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsEdit: $permissionRow.find("[id='IsEdit" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsCancel: $permissionRow.find("[id='IsCancel" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsDelete: $permissionRow.find("[id='IsDelete" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsApprove: $permissionRow.find("[id='IsApprove" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsEditApproved: $permissionRow.find("[id='IsEditApproved" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsHigherApprove: $permissionRow.find("[id='IsHigherApprove" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsPrint: $permissionRow.find("[id='IsPrint" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsEmail: $permissionRow.find("[id='IsEmail" + rowKey + "_" + pageMenuID + "']").is(":checked"),
                IsExcelUpload: false
            });
        });
    });

    if (validationFailed) {
        return false;
    }

    var model = {
        MaEmployees: {
            ID: parseInt($("#ID").val()) || 0,
            FirstName: $("#FirstName").val(),
            MiddleName: $("#MiddleName").val(),
            LastName: $("#LastName").val(),
            MobileNumber: $("#MobileNo").val(),
            EmailID: $("#Email").val(),
            ResidenceNumber: $("#ResidenceNo").val(),
            Address: $("#Address").val(),
            EmployeeType: 1,
            DesignationID: parseInt($("#Designation").val()) || null,
            IsLocationRestrictedUser: $("#IsLocationRestrictedUser").is(":checked"),
            OfficeNumber: $("#OfficeNo").val(),
            UserName: $("#Username").val(),
            Password: $("#Password").val(),
            GmailID: $("#GmailID").val(),
            AccountID: rawParty && !isNaN(rawParty) && parseInt(rawParty) > 0
                ? parseInt(rawParty) : null,
            Active: $("#Active").is(":checked")
        },
        MaEmployeeDetails: details,
        MaUserRights: rights
    };
    // Log all three models separately
    console.log("========== EMPLOYEE MODEL ==========");
    console.log(model.MaEmployees);
    console.table(model.MaEmployees);

    console.log("========== EMPLOYEE DETAILS MODEL ==========");
    console.log(model.MaEmployeeDetails);
    console.table(model.MaEmployeeDetails);

    console.log("========== USER RIGHTS MODEL ==========");
    console.log(model.MaUserRights);
    console.table(model.MaUserRights);

    // Log complete payload being sent to controller
    console.log("========== COMPLETE SAVE PAYLOAD ==========");
    console.log(JSON.stringify(model, null, 4));
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
                    window.location.href = "/Users/Index?MenuID=" + $("#MenuID").val();
                });
            } else {
                Swal.fire("Error", data.message, "error");
            }
        },
        error: function (xhr) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
            });
            Swal.fire("Error", xhr.responseJSON?.message || "Unable to save user.", "error");
        }
    });
}

function RowClick(RowID) {
    $(".form-control").val('');
    EntryEnable('Edit');
    $("#UsersList").hide();
    $("#UsersForm").show();
    //$("#roletablediv").show();
    $.ajax({
        url: webUrl + "RowClick?ID=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {

                $("#ID").val(data.id);
                $("#FirstName").val(data.firstName);
                $("#MiddleName").val(data.middleName);
                $("#LastName").val(data.lastName);
                $("#MobileNo").val(data.mobileNumber);
                $("#Email").val(data.emailID);
                $("#ResidenceNo").val(data.residenceNumber);
                $("#Address").val(data.address);
                $("#Designation").html(data.designationOptions);
                $("#OfficeNo").val(data.officeNumber);
                $("#Username").val(data.username);
                $("#Password").val(data.password);
                $("#Party").val(data.accountName);
                $("#Party").attr('data-idvalue', data.accountID);
                $("#GmailID").val(data.gmailID);
                //$("#EmployeeType").val(data.employeeType); not used currently
                //$("#PhotoID").val(data.photoID);
                //$("#ImagePath").val(data.imagePath);
                // Active checkbox
                $("#Active").prop("checked", data.active === "True" || data.active === true);

                // Location restricted checkbox
                $("#IsLocationRestrictedUser").prop(
                    "checked",
                    data.isLocationRestrictedUser === "True" ||
                    data.isLocationRestrictedUser === true
                );

                // Render the role permissions table
                //$("#roletable").html(data.userrightstable);
                $("#BranchTableTbody").html(data.branch);
                //$("#Roles").html(data.role);
               

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
    if ($("#ID").val() == null || $("#ID").val() == "" ) {
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
                    window.location.href = "/Users/Index?MenuID=" + menuId;
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
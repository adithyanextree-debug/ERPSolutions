var webUrl = '/CategoryMaster/';
var CommonUrl = '/CategoryMaster/';

function NewEntry() {
    $("#Active").prop('checked', true);
    $(".form-control").val('').trigger("change");
    $.ajax({
        url: webUrl + "NewEntry",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            $("#CategoryMasterList").hide();
            $("#CategoryMasterForm").show();
            $("#Code").val(data['nextcode'])
            $("#Code").prop("readonly", true);
            $("#Value").focus();

        }
    });
}

function CloseEntry() {
    if ($("#CategoryMasterList").is(":visible")) {
        window.location.href = "/Home/Index";  // Update this URL as per your needs
    }
    else {
        // Otherwise, reset the form fields and clear content
        $("#CategoryMasterList").show();
        $("#CategoryMasterForm").hide();

        $("#ID").val('');
        $("#Code").val('');
        $("#Value").val('');
        $("#Description").val('');
        $("#hiddenimg").val('');
        $("#Active").prop('checked', false);
        // Create a new <img> element
        const newImage = document.createElement('categoryimagepreview');
        // Set the src attribute to the image URL
        newImage.src = '~/Resources/demo.jpg';

        // Reset UI elements
        $("#liNew").show();
        $("#liSave").hide();
        $("#liDelete").hide();
        $("#refreshIcon").hide();
    }

}

function SaveEntry() {

    if ($("#Value").val() == null ||$("#Value").val() == "") {
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

    if ($("#Description").val() == null || $("#Description").val() == "") {
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
        'ArabicDescription': $("#ArabicDescription").val(),
        'Description': $("#Description").val(),
        'Value': $("#Value").val(),
        'Active': Active,
        'ImagePath': $("#hiddenimg").val(),
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
                    window.location.href = "/CategoryMaster/Index?MenuID=" + menuId;
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
    $("#CategoryMasterList").hide();
    $("#CategoryMasterForm").show();
    $.ajax({
        url: webUrl + "RowClick?id=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#Description").val(data.description)
                $("#ArabicDescription").val(data.arabicdescription)
                $("#Value").val(data.value)
                $("#ID").val(data.id)
                $("#Code").val(data.code)
                $("#Code").prop("readonly", true);
                $("#categoryimagepreview").attr("src", data.imagePath);
                $("#hiddenimg").val(data.imageName)
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

// When user clicks on image → trigger hidden file input
$(document).on("click", "#categoryimagepreview", function () {
    $("#FileUpload_FormFile").click();
});

$(document).on("change", "input[type='file'][id^='FileUpload_FormFile']", function (event) {
    var input = event.target;

    // Validate item code
    if ($("#Value").val() == null || $("#Value").val() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: 'Value is mandatory!!',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        }).then(() => {
            $("#Value").focus();
        });

        // Clear chosen file
        input.value = "";
        return false;
    }

    // Preview selected image
    if (input.files && input.files[0]) {
        var file = input.files[0];
        var fileSizeKB = file.size / 1024; // Convert to KB

        //  Block files larger than 15 KB
        if (fileSizeKB > 15) {
            Swal.fire({
                icon: 'error',
                title: 'File Too Large!',
                text: 'Please upload an image of size 15 KB or less.',
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });

            // Reset input
            input.value = "";

            // Remove preview
            //$("#categoryimagepreview").attr("src", "");
            //$("#categoryimagepreview").removeClass('avatar avatar-lg');

            return false;
        }

        //  Allowed — preview image
        var src = URL.createObjectURL(file);
        $("#categoryimagepreview").attr("src", src);
        $("#categoryimagepreview").addClass('avatar avatar-lg');

        // Auto-submit if needed
        $("#CategoryImageUploadBtn").click();
    }
});


async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);
    formData.append("Value", $("#Value").val());

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        }).then(response => response.json());

        if (response['success'] === true) {
            $("#categoryimagepreview").attr("path", response["imagepath"]);
            $("#hiddenimg").val(response["imagename"])
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: response['message'],
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });

        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: response['message'],
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33'
            });
        }

    } catch (error) {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: error.message || 'An unexpected error occurred.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
    }
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
                    window.location.href = "/CategoryMaster/Index?MenuID=" + menuId;
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
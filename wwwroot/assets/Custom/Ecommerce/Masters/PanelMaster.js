var webUrl = '/PanelMaster/';
var CommonUrl = '/PanelMaster/';
var imagePath = '/uploads';

function NewEntry() {
    $("#PanelMasterList").hide();
    $("#PanelMasterForm").show();
    $("#PanelType").removeAttr('disabled')
    $.ajax({
        url: webUrl + "PanelMappingTypes",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {

            if (data['nextNo'] != 0) {
                $("#OrderNo").val(data['nextNo']).trigger("change")
            } else {
                $("#OrderNo").val(1)
            }
            $("#PanelType").html(data['categories'])

            $("#SortBy").html(data['sortBy'])

            $("#Active").prop('checked', true);
           
            $("#ID").val('');
          //  $("#itemsdetailsection").html('');
            $("#Title").focus();
        },
    });
}

function SaveEntry() {
    if ($("#Title").val() == '') {
        $("#Title").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Title is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#OrderNo").val() == '') {
        $("#OrderNo").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Order No. is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#PanelType").val() == '') {
        $("#PanelType").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Panel Type is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }

    if ($("#PanelTypeValue").val() == '') {
        $("#PanelTypeValue").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Panel Type Value is mandatory",
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


    var items = [];
    $('.ProductActive').each(function () {
        var element = $(this).find('input[name="Active"]');
        var nearestRow = $(this).closest('tr')
        if (nearestRow.attr("element-id") != null) {
            var ID = nearestRow.attr("element-id")
        } else {
            var ID = 0
        }
        if ($(element).is(":checked")) {
            var status = true;
        } else {
            var status = false
        }
        if ($(this).attr('RowState') != null) {
            var itemAry = {
                'ID': ID,
                'Active': status,
                'ItemID': parseInt(element.attr('data-id')),
                'RowState': parseInt($(this).attr('RowState')),
            }
            items.push(itemAry);
        }
    });

    if ($("#StartDate").val() == "") {
        var startDate = null;
    } else {
        var startDate = $("#StartDate").val();
    }
    if ($("#EndDate").val() == "") {
        var EndDate = null;
    } else {
        var EndDate = $("#EndDate").val();
    }

    var panel = {
        'ID': parseInt($("#ID").val()),
        'Title': $("#Title").val(),
        'ArabicTitle': $("#ArabicTitle").val(),
        'OrderNo': parseInt($("#OrderNo").val()),
        'Remarks': $("#Remarks").val(),
        'Active': Active,
       // 'RowState': RowState,
        'PanelTypeID': parseInt($("#PanelType").val()),
        'CategoryID': parseInt($("#PanelTypeValue").attr('data-idvalue')),
        'StartDate': startDate,
        'EndDate': EndDate,
        'SortBy': $("#SortBy").val(),
    }
    var model = {
        'EcomPanelMaster': panel,
        'EcomPanelProducts': items,
    }

    $.ajax({
        url: webUrl + "SaveEntry",
        method: "POST",
        data: model,
        dataType: 'JSON',
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
                    window.location.href = "/PanelMaster/Index?MenuID=" + menuId;
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
    $("#PanelType").attr("disabled", "disabled")
    $(".form-control").val('');
    EntryEnable('Edit');
    $("#PanelMasterList").hide();
    $("#PanelMasterForm").show();
    $.ajax({
        url: webUrl + "RowClick?id=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#sctiondetails").show()
                var header = data['header'];
                header = JSON.parse(header)[0];
                $("#Title").val(header.Title)
                $("#ID").val(RowID)
                if (header.Active == true) {
                    $("#Active").prop("checked", true)
                } else {
                    $("#Active").prop("checked", false)
                }
                $("#ArabicTitle").val(header.ArabicTitle)
                $("#OrderNo").val(header.OrderNo)
                $("#Remarks").val(header.Remarks)
                if (header.StartDate != null) {
                    var date = new Date(header.StartDate).getFullYear() + '-' + ("0" + (new Date(header.StartDate).getMonth() + 1)).slice(-2) + '-' + ("0" + new Date(header.StartDate).getDate()).slice(-2)
                    $("#StartDate").val(date)
                }
                if (header.EndDate != null) {
                    var date1 = new Date(header.EndDate).getFullYear() + '-' + ("0" + (new Date(header.EndDate).getMonth() + 1)).slice(-2) + '-' + ("0" + new Date(header.EndDate).getDate()).slice(-2)
                    $("#EndDate").val(date1).trigger("change")
                }

                $("#PanelType").html(data.paneltype)
                $("#SortBy").html(data.sortby)
                $("#ItemTable tbody").html(data.innerHTML)

            } else {
                $(".AlertDangerStatus").html('Error!')
                $(".AlertDangerMessage").html(data['message'])
                $(".AlertDivDanger").show();
                setTimeout(function () {
                    $(".AlertDivDanger").hide();
                }, 1000);
            }
        },
    });
}

function CloseEntry() {
    // Check if the PanelMasterList is visible
    if ($("#PanelMasterList").is(":visible")) {
        // Go to home page if visible
        window.location.href = "/Home/Index";  // Update URL as needed
    } else {
        // Otherwise, reset the form
        $("#PanelMasterList").show();
        $("#PanelMasterForm").hide();

        // Clear TextBoxes
        $("#Title").val('');
        $("#ArabicTitle").val('');
        $("#OrderNo").val('');
        $("#Remarks").val('');
        $("#PanelType").val('');
        $("#PanelTypeValue").val('');
        $("#StartDate").val('');
        $("#EndDate").val('');
        $("#SortBy").val('');

        // Hide conditional divs
        $("#DivPanelTypeValue").hide();
        $("#DivShowItemListBtn").hide();

        // Reset checkboxes
        $("#Active").prop('checked', false);

        // Reset UI elements/buttons
        $("#liNew").show();
        $("#liSave").hide();
        $("#liDelete").hide();
        $("#refreshIcon").hide();
    }
}


$(document).on("change", "#PanelType", function (event) {
    if ($("#ID").val() != '') {
        return false
    }
    $("#ItemTable").html("");
    $("#PanelTypeValue").val('').trigger("change");
    $("#itemsdetailsection").hide()
    if ($(this).val() == '') {
        $("#DivLinkToValue").hide()
        $("#lookupDIVPanelTypeValue").hide()
        return false
    }
    var Type = $(this).val();
    if (Type == 1141) { //featured
        $(".PanelTypeLookUp").attr('data-lookupcriteria', 'Items');
        $(".PanelTypeLookUp").attr('data-idcolumn', 'ID');
        $(".PanelTypeLookUp").attr('data-assigncolumnname', 'ItemName');
    }
    else if (Type == 1142) { //newarrival
        $(".PanelTypeLookUp").attr('data-lookupcriteria', 'Items');
        $(".PanelTypeLookUp").attr('data-idcolumn', 'ID');
        $(".PanelTypeLookUp").attr('data-assigncolumnname', 'ItemName');
    }
    else if (Type == 1143) { //deals
        $(".PanelTypeLookUp").attr('data-idcolumn', 'ID');
        $(".PanelTypeLookUp").attr('data-assigncolumnname', 'Value');
        $(".PanelTypeLookUp").attr('data-lookupcriteria', 'ItemName');
    }
    else if (Type == 1144) { //brand
        $(".PanelTypeLookUp").attr('data-idcolumn', 'ID');
        $(".PanelTypeLookUp").attr('data-assigncolumnname', 'Value');
        $(".PanelTypeLookUp").attr('data-lookupcriteria', 'Brands');
    }
    else if (Type == 1145) { //category
        $(".PanelTypeLookUp").attr('data-idcolumn', 'ID');
        $(".PanelTypeLookUp").attr('data-assigncolumnname', 'Value');
        $(".PanelTypeLookUp").attr('data-lookupcriteria', 'ItemCategory');
    }
    $("#DivPanelTypeValue").show()
    $("#DivShowItemListBtn").show()
});
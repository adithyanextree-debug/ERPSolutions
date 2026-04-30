var webUrl = '/POS/';
var CommonUrl = '/CommonFunctions/';

// ENTER KEY GLOBAL HANDLER (Tab + ProductCode logic)
$(document).on('keydown', function (e) {

    if (e.key !== "Enter") {
        return;
    }
    // Allow Enter in textarea
    if (e.target.tagName === "TEXTAREA") {
        return;
    }
    const $target = $(e.target);
    console.log($target);
    // =====================================
    // PRODUCT CODE ENTER LOGIC
    // =====================================
    if ($target.hasClass("productCode")) {
        alert("A")
        e.preventDefault();
        const id = $target.attr("element-id");
        console.log(id + " ID");
        if ($("#Party").val() == '' || $("#Party").val() == null) {
            $("#Party").focus();
            Swal.fire({
                title: "Missing Information",
                text: "Party is mandatory",
                icon: "warning"
            });
            return false;
        }

        // Reset row fields
        $("#ItemUnit" + id).html('');
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemDiscPer" + id).val('');
        $("#ItemDiscAmt" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTotal" + id).val('');
        $("#ItemGrossAmt" + id).val('');
        $("#ItemTaxAmt" + id).val('');

        // Clear total summary
        updateAllSums();
        var data = {
            'ID': $target.attr('data-idvalue'),
            'AccountID': $("#Party").attr('data-idvalue'),
        }
        console.log(data);
        $.ajax({
            url: CommonUrl + "ProductAvailableUnits",
            method: "POST",
            data: data,
            dataType: 'JSON',
            success: function (data) {
                alert("B")
                $("#ItemUnit" + id).html(data['units']);
                var unit = JSON.parse(data['unitDetails'])[0];
                var tax = JSON.parse(data['taxDetails'])[0];
                var ItemUnitPrice = data['itemUnitPrice'];
                var imagesrc = data['imagesrc'];
                $("#productimagepreview" + id).attr("src", "");
                $("#productimagepreview" + id).attr("src", imagesrc);
                if (isNaN(ItemUnitPrice) || ItemUnitPrice == '') {
                    ItemUnitPrice = unit["SellingPrice"];
                }
                if (JSON.parse(data['taxDetails']).length != 0) {
                    if (tax["SalesPerc"] != null && tax["SalesPerc"] != "") {
                        $("#ItemTaxPer" + id).val(tax["SalesPerc"])
                        $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"])
                    } else {
                        $("#ItemTaxPer" + id).attr('taxTypeID', "")
                    }
                }
                if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                    $("#ItemTaxPer" + id).val(parseFloat($("#ItemTaxPer" + id).val()).toFixed(2))
                    $("#ItemTaxPer" + id).css('text-align', 'right');
                }
                $("#ItemQty" + id).val(1)
                $("#ItemRate" + id).val(parseFloat(ItemUnitPrice).toFixed(2));
                $("#ItemRate" + id).css('text-align', 'right');
                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);
                if ($("#ItemQty" + id).val() != null && $("#ItemQty" + id).val() != "") {
                    if ($("#ItemRate" + id).val() != null && $("#ItemRate" + id).val() != "") {
                        var tot = $("#ItemQty" + id).val() * $("#ItemRate" + id).val();
                    } else {
                        var tot = $("#ItemQty" + id).val() * ItemUnitPrice;
                    }
                    $("#ItemGrossAmt" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemGrossAmt" + id).css('text-align', 'right');
                    $("#ItemTotal" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                    $("#ItemAmt" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemAmt" + id).css('text-align', 'right');
                }
                else {
                    if ($("#ItemRate" + id).val() != null && $("#ItemRate" + id).val() != '') {
                        $("#ItemGrossAmt" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemGrossAmt" + id).css('text-align', 'right');
                        $("#ItemTotal" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemTotal" + id).css('text-align', 'right');
                        $("#ItemAmt" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemAmt" + id).css('text-align', 'right');
                    }
                    else {
                        $("#ItemGrossAmt" + id).val(parseFloat(unit["SellingPrice"]).toFixed(2))
                        $("#ItemGrossAmt" + id).css('text-align', 'right');
                        $("#ItemAmt" + id).val(parseFloat(unit["SellingPrice"]).toFixed(2))
                        $("#ItemAmt" + id).css('text-align', 'right');
                    }
                }
                if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                    var amt = $("#ItemGrossAmt" + id).val();
                    var tax = amt * ($("#ItemTaxPer" + id).val() / 100);
                    $("#ItemTaxAmt" + id).val((parseFloat(tax).toFixed(2)));
                    $("#ItemTaxAmt" + id).css('text-align', 'right');
                    var sum = parseFloat(amt) + parseFloat(tax)
                    $("#ItemTotal" + id).val(parseFloat(sum).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                }
                if ($("#ItemDiscPer" + id).val() != null && $("#ItemDiscPer" + id).val() != '') {
                    var amt = $("#ItemGrossAmt" + id).text();
                    var disc = amt * ($("#ItemDiscPer" + id).val() / 100);
                    $("#ItemDiscAmt" + id).val(disc);
                    $("#ItemDiscAmt" + id).css('text-align', 'right');
                    $("#ItemAmt" + id).val(amt - disc);
                    $("#ItemAmt" + id).css('text-align', 'right');
                    var sum
                    if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                        sum = (parseFloat(amt) - parseFloat(disc)) + parseFloat(tax)
                    } else {
                        sum = (parseFloat(amt) - parseFloat(disc))
                    }
                    $("#ItemTotal" + id).val(parseFloat(sum).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                }

                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        qtysum += parseFloat($(this).val())
                    }
                });
                if (!isNaN(qtysum)) {
                    $("#qtySum").html(parseFloat(qtysum).toFixed(2))
                    $("#qtySum").css('text-align', 'center');
                }
                var GrossAmt = 0;
                $('.ItemGrossAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        GrossAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(GrossAmt)) {
                    $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
                    $("#ItemGrossAmtSum").css('text-align', 'right');
                }
                var dicsAmt = 0;
                $('.ItemDiscAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        dicsAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(dicsAmt)) {
                    $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
                    $("#dicsAmtSum").css('text-align', 'right');
                }
                var ItemAmt = 0;
                $('.ItemAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemAmt)) {
                    $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
                    $("#amtSum").css('text-align', 'right');
                }
                var ItemTaxAmt = 0;
                $('.ItemTaxAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTaxAmt += parseFloat($(this).val())
                    }
                });
                if (!isNaN(ItemTaxAmt)) {
                    $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
                    $("#taxAmtSum").css('text-align', 'right');
                    alert(ItemTaxAmt + " ItemTaxAmt")
                    $("#TaxSummary").val(parseFloat(ItemTaxAmt).toFixed(2))
                }
                var ItemTotal = 0;
                $('.ItemTotal').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTotal += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemTotal)) {
                    $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
                    $("#itemTotalSum").css('text-align', 'right');
                    alert(ItemTotal + " ItemTotal")
                    $('#Total').val(toTwoDecimal(ItemTotal));
                }
                if ($("#ItemQty" + id).val() == null) {
                    $("#ItemQty" + id).val(1);
                    $("#ItemQty" + id).val(parseFloat($("#ItemQty" + id).val()).toFixed(2))
                    $("#ItemQty" + id).css('text-align', 'center');
                }
                $("#ItemQty" + id).css('text-align', 'center');
                $("#ItemQty" + id).val(parseFloat($("#ItemQty" + id).val()).toFixed(2))
                $("#ItemQty" + id).focus();

                calculateGrandTotal();
            },
        });

        return; // IMPORTANT → stop default tab movement
    }

    // =====================================
    // DEFAULT ENTER = TAB BEHAVIOR
    // =====================================

    e.preventDefault();
    const focusableElements = $('a, button, input, select, textarea, [tabindex]')
        .filter(':visible')
        .filter(function () {
            return !$(this).prop('disabled') &&
                $(this).attr('tabindex') !== '-1';
        });

    const index = focusableElements.index(document.activeElement);

    if (index > -1) {
        const nextElement = focusableElements.get(index + 1);
        if (nextElement) {
            nextElement.focus();
        }
    }

});

//For refreshing the page same as a New entry
function NewEntry() {
    location.reload();
}

//For getting item price details while changing unit
$(document).on("change", ".ItemUnit", function () {
    const id = $(this).attr('element-id');
    const selectedUnit = $(this).val();

    //if ($(this).data('last') !== selectedUnit) {
    //    $("#productCode" + id).addClass('changedValue');
    //}

    if (!selectedUnit) {
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemDiscPer" + id).val('');
        $("#ItemDiscAmt" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTotal" + id).val('');
        $("#ItemGrossAmt" + id).html('');
        $("#ItemTaxAmt" + id).val('');

        updateAllSums();
        return;
    }

    $.ajax({
        url: CommonUrl + "ProductAvailableUnits",
        method: "POST",
        data: {
            'ID': $("#productCode" + id).attr('data-idvalue'),
            'AccountID': $("#Party").attr('data-idvalue'),
            'Unit': selectedUnit
        },
        dataType: 'JSON',
        success: function (data) {
            $("#ItemUnit" + id).html(data['units']);

            const unitList = JSON.parse(data['unitDetails']);
            const taxList = JSON.parse(data['taxDetails']);
            const unit = unitList.length > 0 ? unitList[0] : {};
            const tax = taxList.length > 0 ? taxList[0] : {};

            let ItemUnitPrice = parseFloat(data['itemUnitPrice']);
            if (isNaN(ItemUnitPrice)) {
                ItemUnitPrice = parseFloat(unit["SellingPrice"]) || 0;
            }

            // Set Rate
            $("#ItemRate" + id).val(toTwoDecimal(ItemUnitPrice));
            $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

            // Tax Info
            if (tax["SalesPerc"]) {
                $("#ItemTaxPer" + id).val(toTwoDecimal(tax["SalesPerc"]));
                $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"]);
                $("#TaxSummary").val(toTwoDecimal(tax["SalesPerc"]))

            } else {
                $("#ItemTaxPer" + id).val('');
                $("#ItemTaxPer" + id).attr('taxTypeID', '');
            }

            // Default Quantity = 1
            $("#ItemQty" + id).val('1');

            // Recalculate fields
            calculateRow(id);
            updateAllSums();

            // Focus back to qty field
            $("#ItemQty" + id).focus();
        }
    });
});

//For adding new row for item add
$(document).on("click", ".addrow", function (event) {
    event.preventDefault();

    var $row = $(this).closest('tr');
    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');
    var item = $("#productCode" + currentId).val();
    var unit = $("#ItemUnit" + currentId).val();
    var qty = $("#ItemQty" + currentId).val();

    if (!item.trim()) {
        Swal.fire({
            title: "Missing Product",
            text: "Please enter the item before proceeding.",
            icon: "warning"
        });
        return;
    }

    if (!unit || unit.trim() === "") {
        Swal.fire({
            title: "Missing Unit",
            text: "Please select a unit for the item.",
            icon: "warning"
        });
        return;
    }

    if (!qty || qty.trim() === "" || isNaN(qty) || parseFloat(qty) <= 0) {
        Swal.fire({
            title: "Invalid Quantity",
            text: "Please enter a valid quantity greater than 0.",
            icon: "warning"
        });
        return;
    }

    $.ajax({
        url: "/POS/NewRow?no=" + serialno,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data.success == true) {
                var newrow = data['newrow'];
                $("#Itemtable tbody").append(newrow);
                ReindexRows();
            }
        },
    });
});

//For deleteing single item
$(document).on("click", ".action_delete", function (event) {
    var $Row = $(this).closest('tr');
    var $TableBody = $Row.closest('tbody');
    var rowCount = $TableBody.find('tr').length;

    // Prevent deleting if only one row is left
    if (rowCount <= 1) {
        Swal.fire({
            icon: 'warning',
            title: 'Warning!',
            text: 'You cannot delete the last remaining row.If any changes needed please update the data',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return; // Exit the click handler
    }

    var id = $(this).attr('element-id');
    var itemid = $("#itemid" + id).val();

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
                $.ajax({
                    url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove(); // Remove the deleted row from DOM
                            ReindexRows();

                            var qtysum = 0;
                            $('.ItemQty').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    qtysum += parseFloat($(this).val())
                                }
                            });
                            if (!isNaN(qtysum)) {
                                $("#qtySum").html(parseFloat(qtysum).toFixed(2))
                                $("#qtySum").css('text-align', 'center');
                            }
                            var GrossAmt = 0;
                            $('.ItemGrossAmt').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    GrossAmt += parseFloat($(this).val())
                                }
                            });
                            if (!isNaN(GrossAmt)) {
                                $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
                                $("#ItemGrossAmtSum").css('text-align', 'right');
                            }
                            var dicsAmt = 0;
                            $('.ItemDiscAmt').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    dicsAmt += parseFloat($(this).val())
                                }
                            });
                            if (!isNaN(dicsAmt)) {
                                $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
                                $("#dicsAmtSum").css('text-align', 'right');
                            }
                            var ItemAmt = 0;
                            $('.ItemAmt').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    ItemAmt += parseFloat($(this).val())
                                }
                            });
                            if (!isNaN(ItemAmt)) {
                                $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
                                $("#amtSum").css('text-align', 'right');
                            }
                            var ItemTaxAmt = 0;
                            $('.ItemTaxAmt').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    ItemTaxAmt += parseFloat($(this).val())
                                }
                            });

                            if (!isNaN(ItemTaxAmt)) {
                                $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
                                $("#taxAmtSum").css('text-align', 'right');
                            }
                            var ItemTotal = 0;
                            $('.ItemTotal').each(function () {
                                if ($(this).val() != null && $(this).val() != '') {
                                    ItemTotal += parseFloat($(this).val())
                                }
                            });

                            if (!isNaN(ItemTotal)) {
                                $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
                                $("#itemTotalSum").css('text-align', 'right');
                            }
                            updateAllSums()
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
                    }
                });
            }
            else {
                $Row.remove(); // Remove the deleted row from DOM
                ReindexRows();

                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        qtysum += parseFloat($(this).val())
                    }
                });
                if (!isNaN(qtysum)) {
                    $("#qtySum").html(parseFloat(qtysum).toFixed(2))
                    $("#qtySum").css('text-align', 'center');
                }
                var GrossAmt = 0;
                $('.ItemGrossAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        GrossAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(GrossAmt)) {
                    $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
                    $("#ItemGrossAmtSum").css('text-align', 'right');
                }
                var dicsAmt = 0;
                $('.ItemDiscAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        dicsAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(dicsAmt)) {
                    $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
                    $("#dicsAmtSum").css('text-align', 'right');
                }
                var ItemAmt = 0;
                $('.ItemAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemAmt)) {
                    $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
                    $("#amtSum").css('text-align', 'right');
                }
                var ItemTaxAmt = 0;
                $('.ItemTaxAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTaxAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemTaxAmt)) {
                    $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
                    $("#taxAmtSum").css('text-align', 'right');
                }
                var ItemTotal = 0;
                $('.ItemTotal').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTotal += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemTotal)) {
                    $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
                    $("#itemTotalSum").css('text-align', 'right');
                }
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

function toFixedNoRound(num, decimals) {
    num = Number(num);
    console.log(num + " num")
    console.log(decimals + " decimals")
    if (isNaN(num)) return "0.00";
    const factor = Math.pow(10, decimals);
    console.log(factor + " factor")
    console.log((Math.trunc(num * factor) / factor).toFixed(decimals) + "RETURN RESULT")
    return (Math.trunc(num * factor) / factor).toFixed(decimals);
}

function toTwoDecimal(val) {
    return parseFloat(val || 0).toFixed(2);
}

function updateSum(selector, targetSelector, align = 'right') {
    let sum = 0;
    $(selector).each(function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) sum += val;
    });
    $(targetSelector).html(toTwoDecimal(sum)).css('text-align', align);
}

function calculateRow(id, changedField = '') {

    let rawQty = $("#ItemQty" + id).val();

    // USER IS CLEARING → DON’T INTERFERE
    if (rawQty === "") return;

    const qty = parseFloat(rawQty);
    if (isNaN(qty)) return;

    const rate = parseFloat($("#ItemRate" + id).val()) || 0;

    const grossAmt = qty * rate;

    let discAmt = parseFloat($("#ItemDiscAmt" + id).val()) || 0;
    let discPer = parseFloat($("#ItemDiscPer" + id).val()) || 0;

    // Only sync opposite field
    if (changedField === 'ItemDiscAmt') {
        discPer = grossAmt ? (discAmt / grossAmt) * 100 : 0;
        $("#ItemDiscPer" + id).val(toTwoDecimal(discPer));
    }
    else if (changedField === 'ItemDiscPer') {
        discAmt = (grossAmt * discPer) / 100;
        $("#ItemDiscAmt" + id).val(toTwoDecimal(discAmt));
    }

    const amt = grossAmt - discAmt;

    let taxAmt = parseFloat($("#ItemTaxAmt" + id).val()) || 0;
    let taxPer = parseFloat($("#ItemTaxPer" + id).val()) || 0;

    if (changedField === 'ItemTaxAmt') {
        taxPer = amt ? (taxAmt / amt) * 100 : 0;
        $("#ItemTaxPer" + id).val(toTwoDecimal(taxPer));
    }
    else if (changedField === 'ItemTaxPer') {
        taxAmt = (amt * taxPer) / 100;
        $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt));
    }

    const total = amt + taxAmt;

    // ONLY SYSTEM FIELDS ARE FORMATTED
    $("#ItemGrossAmt" + id).val(toTwoDecimal(grossAmt));
    $("#ItemAmt" + id).val(toTwoDecimal(amt));
    $("#ItemTotal" + id).val(toTwoDecimal(total));
}

function updateAllSums() {
    updateSum('.ItemQty', '#qtySum', 'center');
    updateSum('.ItemGrossAmt', '#ItemGrossAmtSum');
    updateSum('.ItemDiscAmt', '#dicsAmtSum');
    updateSum('.ItemAmt', '#amtSum');
    updateSum('.ItemTaxAmt', '#taxAmtSum');
    updateSum('.ItemTotal', '#itemTotalSum');
    //  SET TOTAL FIELD FROM ITEM TOTAL SUM
    const total = parseFloat($('#itemTotalSum').text()) || 0;
    const tax = parseFloat($("#taxAmtSum").text()) || 0;
    $("#TaxSummary").val(toTwoDecimal(tax))

    $('#Total').val(toTwoDecimal(total));

    calculateGrandTotal();
}

$(document).on("input", ".ItemQty, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {

    const id = $(this).attr('element-id');
    const changedField = $(this).attr("id").replace(id, '');

    let val = $(this).val();

    // Allow natural typing
    if (val === "" || val === "." || val.endsWith(".")) {
        return;
    }

    // Reject only truly bad values
    if (val.includes('-') || isNaN(val) || Number(val) < 0) {
        return;
    }

    //  ONLY calculations — NO formatting of this field
    calculateRow(id, changedField);
    updateAllSums();
});

$(document).on("blur", ".ItemQty, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {
    const val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
});

function ReindexRows() {
    $('#Itemtable tbody tr').each(function (index) {
        var rowIndex = index + 1; // start from 1
        $(this).find('td.serial-no').text(rowIndex);
    });
}

function calculateGrandTotal() {
    const total = parseFloat($('#Total').val()) || 0;
    const otherDiscount = parseFloat($('#OtherDiscount').val()) || 0;
    //const tax = parseFloat($('#TaxSummary').val()) || 0;

    const grandTotal = total - otherDiscount;

    $('#GrandTotal').val(toTwoDecimal(grandTotal));
    calculatePayment();
}

$(document).on("input", "#OtherDiscount", function () {
    calculateGrandTotal();
});

//$(document).on("input", "#OtherDiscount, #TaxSummary", function () {

//    let val = $(this).val();

//    // Allow empty → treat as 0
//    if (val === "" || val === ".") {
//        calculateGrandTotal();
//        return;
//    }

//    // Block negatives
//    if (val.includes('-') || isNaN(val) || Number(val) < 0) {
//        return;
//    }

//    calculateGrandTotal();
//});

$(document).on("blur", "#OtherDiscount", function () {
    const val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
});

$(document).on("input", ".moneyField", function () {

    const $field = $(this);
    let value = $field.val();

    // Remove everything except digits and dot
    value = value.replace(/[^0-9.]/g, '');

    // Allow only ONE dot
    const parts = value.split('.');
    if (parts.length > 2) {
        value = parts[0] + '.' + parts.slice(1).join('');
    }

    // Limit to 2 decimal places
    if (value.includes('.')) {
        const split = value.split('.');
        split[1] = split[1].substring(0, 2);
        value = split[0] + '.' + split[1];
    }
    $field.val(value);
});

$(document).on("blur", ".moneyField", function () {
    let value = $(this).val();
    if (value.endsWith('.')) {
        value = value.slice(0, -1);
    }
    if (value === '.') {
        value = '';
    }
    $(this).val(value);
});

//For total paid and Balance calculation 24-02-2026
function toNumber(val) {
    return parseFloat(val) || 0;
}

function calculatePayment() {
    // Get item total (prefer #Total, fallback to #itemTotalSum)
    let itemTotal = 0;

    if ($('#GrandTotal').length && $('#GrandTotal').val() !== undefined) {
        itemTotal = toNumber($('#GrandTotal').val());
    } else if ($('#itemTotalSum').length) {
        itemTotal = toNumber($('#itemTotalSum').text());

    }

    const cash = toNumber($('#CashPaid').val());
    const card = toNumber($('#CardPaid').val());

    const totalPaid = cash + card;
    const balance = itemTotal - totalPaid;

    $('#TotalPaid').val(totalPaid.toFixed(2));
    $('#Balance').val(balance.toFixed(2));
}

// Trigger calculation when typing
$('#CashPaid, #CardPaid').on('input', function () {
    calculatePayment();
});

//For saving entry
function SaveEntry() {
    if ($("#ID").val() != null && $("#ID").val() != "") {
        if ($("#VoucherNo").val() == '') {
            $("#VoucherNo").focus();
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Voucher No is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false
        }
    }
    if ($("#VoucherDate").val() == '') {
        $("#VoucherDate").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Voucher Date is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }
    if ($("#Party").val() == '' || $("#Party").val() == null) {
        $("#Party").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Party is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }
    var idValue = $("#ID").val() ? parseInt($("#ID").val()) : null;

    var flag1 = true;
    var flag2 = true;
    var flag3 = true;
    var flag4 = true;

    var items = [];
    var entries = [];
    $('.productCode').each(function () {
        if ($(this).val() != null && $(this).val() != '') {
            var i = $(this).attr('element-id');
            if ($("#ItemUnit" + i).val() == '' || $("#ItemUnit" + i).val() == null) {
                flag1 = false;
                return false;

            }
            if ($("#ItemQty" + i).val() == '' || $("#ItemQty" + i).val() == null || parseInt($("#ItemQty" + i).val()) == 0) {
                flag2 = false;
                return false;

            }
            if ($("#ItemRate" + i).val() == '' || $("#ItemRate" + i).val() == null) {
                flag3 = false;
                return false;
            }
            var InTransItemId = parseInt($("#itemid" + i).val()) || 0;
            var rawItem = $("#productCode" + i).attr('data-idvalue');
            var rawTaxType = $("#ItemTaxPer" + i).attr('taxTypeID');
            var itemAry = {
                'ID': InTransItemId, // Nullable int (assuming InTransItemId can be null)
                'TransactionID': idValue,
                'ItemID': (rawItem !== undefined && rawItem !== null && rawItem.toString().trim() !== "" && !isNaN(rawItem) && parseInt(rawItem) > 0)
                    ? parseInt(rawItem)
                    : null,
                'Unit': $("#ItemUnit" + i).val() || '', // Nullable string
                'Qty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : null, // Nullable float
                'Rate': $("#ItemRate" + i).val() ? parseFloat($("#ItemRate" + i).val()) : null, // Nullable float
                'BasicQty': ($("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1) * ($("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0), // Nullable float
                'TaxPerc': $("#ItemTaxPer" + i).val() ? parseFloat($("#ItemTaxPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxValue': $("#ItemTaxAmt" + i).val() ? parseFloat($("#ItemTaxAmt" + i).val()) : 0, // Nullable float (default to 0)
                'RateDiscPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'RateDisc': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
                'DiscountPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxTypeID': (rawTaxType && !isNaN(rawTaxType) && parseInt(rawTaxType) > 0)
                    ? parseInt(rawTaxType)
                    : null,
                'TempQty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0, // Nullable float (default to 0)
                'Discount': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
                'Factor': $("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1, // Nullable float (default to 1)
                'StockQty': ($("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1) * ($("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0), // Nullable float
                'OutLocID': $("#Warehouse").val() ? parseInt($("#Warehouse").val()) : null, // Nullable int
                'Description': $("#Description").val() || '', // Nullable string
                'RowType': $("#RowType").val(),
                'RowState': (InTransItemId === null || InTransItemId === 0 || InTransItemId === undefined) ? 1 : 2,
            };

            if ($(this).val() != '' && $("#ItemUnit" + i).val() != '' && $("#ItemQty" + i).val() != '' && $("#ItemRate" + i).val() != '') {
                items.push(itemAry);
            }

        }
        else {
            flag4 = false;
            return false;
        }
    });

    var rawProject = $("#Project").attr('data-idvalue');
    var rawParty = $("#Party").attr('data-idvalue');
    // Build transaction object
    var transaction = {
        ID: idValue, // Nullable int
        Date: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        EffectiveDate: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        VoucherID: $("#VoucherType").attr("data-value") ? parseInt($("#VoucherType").attr("data-value")) : null, // Nullable int
        TransactionNo: $("#VoucherNo").val() || '', // Nullable string
        SerialNo: $("#VoucherNo").val() ? parseInt($("#VoucherNo").val()) : null, // Nullable long
        ExchangeRate: 1.0, // Decimal, assuming default of 1.0
        AddedDate: new Date().toISOString(), // DateTime
        AccountID: (rawParty !== undefined && rawParty !== null && rawParty.trim() !== "" && !isNaN(rawParty) && parseInt(rawParty) > 0)
            ? parseInt(rawParty)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    // Build additionals object
    var bank = $("#BankID").attr("data-value");
    var cash = $("#CashID").attr("data-value");

    var additionals = {
        TransactionID: idValue,
        ModeID: cash ? cash : bank,
        AllocationPerc: $("#OtherDiscount").val(),
        AccountID: (rawSalesman !== undefined && rawSalesman !== null && rawSalesman.toString().trim() !== "" && !isNaN(rawSalesman) && parseInt(rawSalesman) > 0)
            ? parseInt(rawSalesman)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };
    // Build entries object
    var entriesarray = {
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    }
    entries.push(entriesarray)
    if (flag4 && flag1 && flag2 && flag3) {
        var model = {
            'InvTransItems': items,
            'FiTransactions': transaction,
            'FiTransactionAdditionals': additionals,
            'FiTransactionEntries': entries
        }
    }
    else {
        if (flag4 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Please enter the Item details",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
        if (flag1 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Item Unit is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
        if (flag2 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Item Quantity is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
        if (flag3 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Item Rate is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }

    }
    console.log(model)

    $.ajax({
        url: webUrl + "InsertTransaction",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(model),
        dataType: 'json',

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
                    window.location.href = "/POS/Index?MenuID=" + menuId;
                });

                $("#ID").val(data.transactionNo);

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

//For getting item lookup and units (Not using now in POS)
$(document).on("keyup", ".productCode1", function (event) {
    const id = $(this).attr("element-id");
    if (event.key === "Enter") {

        if ($("#Party").val() == '' || $("#Party").val() == null) {
            $("#Party").focus();
            Swal.fire({
                title: "Missing Information",
                text: "Party is mandatory",
                icon: "warning"
            });
            return false;
        }

        // Reset row fields
        $("#ItemUnit" + id).html('');
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemDiscPer" + id).val('');
        $("#ItemDiscAmt" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTotal" + id).val('');
        $("#ItemGrossAmt" + id).val('');
        $("#ItemTaxAmt" + id).val('');

        // Clear total summary
        updateAllSums();

        $.ajax({
            url: CommonUrl + "ProductAvailableUnits",
            method: "POST",
            data: {
                'ID': $(this).attr('data-idvalue'),
                'AccountID': $("#Party").attr('data-idvalue'),
            },
            dataType: 'JSON',
            success: function (data) {
                $("#ItemUnit" + id).html(data['units']);
                var unit = JSON.parse(data['unitDetails'])[0];
                var tax = JSON.parse(data['taxDetails'])[0];
                var ItemUnitPrice = data['itemUnitPrice'];
                var imagesrc = data['imagesrc'];
                $("#productimagepreview" + id).attr("src", "");
                $("#productimagepreview" + id).attr("src", imagesrc);
                if (isNaN(ItemUnitPrice) || ItemUnitPrice == '') {
                    ItemUnitPrice = unit["SellingPrice"];
                }
                if (JSON.parse(data['taxDetails']).length != 0) {
                    if (tax["SalesPerc"] != null && tax["SalesPerc"] != "") {
                        $("#ItemTaxPer" + id).val(tax["SalesPerc"])
                        $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"])
                    } else {
                        $("#ItemTaxPer" + id).attr('taxTypeID', "")
                    }
                }
                if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                    $("#ItemTaxPer" + id).val(parseFloat($("#ItemTaxPer" + id).val()).toFixed(2))
                    $("#ItemTaxPer" + id).css('text-align', 'right');
                }
                $("#ItemQty" + id).val(1)
                $("#ItemRate" + id).val(parseFloat(ItemUnitPrice).toFixed(2));
                $("#ItemRate" + id).css('text-align', 'right');
                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);
                if ($("#ItemQty" + id).val() != null && $("#ItemQty" + id).val() != "") {
                    if ($("#ItemRate" + id).val() != null && $("#ItemRate" + id).val() != "") {
                        var tot = $("#ItemQty" + id).val() * $("#ItemRate" + id).val();
                    } else {
                        var tot = $("#ItemQty" + id).val() * ItemUnitPrice;
                    }
                    $("#ItemGrossAmt" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemGrossAmt" + id).css('text-align', 'right');
                    $("#ItemTotal" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                    $("#ItemAmt" + id).val(parseFloat(tot).toFixed(2))
                    $("#ItemAmt" + id).css('text-align', 'right');
                }
                else {
                    if ($("#ItemRate" + id).val() != null && $("#ItemRate" + id).val() != '') {
                        $("#ItemGrossAmt" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemGrossAmt" + id).css('text-align', 'right');
                        $("#ItemTotal" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemTotal" + id).css('text-align', 'right');
                        $("#ItemAmt" + id).val(parseFloat($("#ItemRate" + id).val()).toFixed(2))
                        $("#ItemAmt" + id).css('text-align', 'right');
                    }
                    else {
                        $("#ItemGrossAmt" + id).val(parseFloat(unit["SellingPrice"]).toFixed(2))
                        $("#ItemGrossAmt" + id).css('text-align', 'right');
                        $("#ItemAmt" + id).val(parseFloat(unit["SellingPrice"]).toFixed(2))
                        $("#ItemAmt" + id).css('text-align', 'right');
                    }
                }
                if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                    var amt = $("#ItemGrossAmt" + id).val();
                    var tax = amt * ($("#ItemTaxPer" + id).val() / 100);
                    $("#ItemTaxAmt" + id).val((parseFloat(tax).toFixed(2)));
                    $("#ItemTaxAmt" + id).css('text-align', 'right');
                    var sum = parseFloat(amt) + parseFloat(tax)
                    $("#ItemTotal" + id).val(parseFloat(sum).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                }
                if ($("#ItemDiscPer" + id).val() != null && $("#ItemDiscPer" + id).val() != '') {
                    var amt = $("#ItemGrossAmt" + id).text();
                    var disc = amt * ($("#ItemDiscPer" + id).val() / 100);
                    $("#ItemDiscAmt" + id).val(disc);
                    $("#ItemDiscAmt" + id).css('text-align', 'right');
                    $("#ItemAmt" + id).val(amt - disc);
                    $("#ItemAmt" + id).css('text-align', 'right');
                    var sum
                    if ($("#ItemTaxPer" + id).val() != null && $("#ItemTaxPer" + id).val() != "") {
                        sum = (parseFloat(amt) - parseFloat(disc)) + parseFloat(tax)
                    } else {
                        sum = (parseFloat(amt) - parseFloat(disc))
                    }
                    $("#ItemTotal" + id).val(parseFloat(sum).toFixed(2))
                    $("#ItemTotal" + id).css('text-align', 'right');
                }

                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        qtysum += parseFloat($(this).val())
                    }
                });
                if (!isNaN(qtysum)) {
                    $("#qtySum").html(parseFloat(qtysum).toFixed(2))
                    $("#qtySum").css('text-align', 'center');
                }
                var GrossAmt = 0;
                $('.ItemGrossAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        GrossAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(GrossAmt)) {
                    $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
                    $("#ItemGrossAmtSum").css('text-align', 'right');
                }
                var dicsAmt = 0;
                $('.ItemDiscAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        dicsAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(dicsAmt)) {
                    $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
                    $("#dicsAmtSum").css('text-align', 'right');
                }
                var ItemAmt = 0;
                $('.ItemAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemAmt)) {
                    $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
                    $("#amtSum").css('text-align', 'right');
                }
                var ItemTaxAmt = 0;
                $('.ItemTaxAmt').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTaxAmt += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemTaxAmt)) {
                    $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
                    $("#taxAmtSum").css('text-align', 'right');
                }
                var ItemTotal = 0;
                $('.ItemTotal').each(function () {
                    if ($(this).val() != null && $(this).val() != '') {
                        ItemTotal += parseFloat($(this).val())
                    }
                });

                if (!isNaN(ItemTotal)) {
                    $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
                    $("#itemTotalSum").css('text-align', 'right');
                }
                if ($("#ItemQty" + id).val() == null) {
                    $("#ItemQty" + id).val(1);
                    $("#ItemQty" + id).val(parseFloat($("#ItemQty" + id).val()).toFixed(2))
                    $("#ItemQty" + id).css('text-align', 'center');
                }
                $("#ItemQty" + id).css('text-align', 'center');
                $("#ItemQty" + id).val(parseFloat($("#ItemQty" + id).val()).toFixed(2))
                $("#ItemQty" + id).focus();
            },
        });

        // Set focus anyway
        $("#ItemUnit" + id).focus();
    }
});
var webUrl = '/PurchaseForm/';
var CommonUrl = '/CommonFunctions/';

function NewEntry() {
    $("#PurchaseForm8List").hide();
    $("#PurchaseForm8Form").show();
    $.ajax({
        url: CommonUrl + "NewEntryDetailsPurchase",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            // $("#VoucherNo").val(data['nextVNo'])
            $("#VoucherNo").prop("disabled", true)
            var date = new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2);
            $("#VoucherDate").val(date)
            $("#Account").val(data.account)
            $("#Mode").html(data.mode);

            $("#Itemtable tbody").append(data['newEntry'])
            $("#Warehouse").html(data['warehouses'])
            $("#Party").focus();
        },
    });
}

// Card button
$(document).on('click', '#btnCard', function () {
    if ($('#CardTable tbody tr').length > 0) {
        captureModalSnapshot('CardTable');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).show();
        return;
    }
    $.ajax({
        url: '/PurchaseForm/GetCardDetails',
        type: 'GET',
        data: {},
        success: function (data) {
            $('#CardTable tbody').html(data.cardTable);
            captureModalSnapshot('CardTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).show();
        },
        error: function () {
            Swal.fire('Error', 'Failed to load Card details.', 'error');
        }
    });
});

// Cheque button
//$(document).on('click', '#btnCheque', function () {
//    if ($('#ChequeTable tbody tr').length > 0) {
//        captureModalSnapshot('ChequeTable');
//        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).show();
//        return;
//    }
//    $.ajax({
//        url: '/PurchaseForm/GetChequeDetails',
//        type: 'GET',
//        data: {},
//        success: function (data) {
//            $('#ChequeTable tbody').html(data.chequeTable);
//            captureModalSnapshot('ChequeTable');
//            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).show();
//        },
//        error: function () {
//            Swal.fire('Error', 'Failed to load Cheque details.', 'error');
//        }
//    });
//});

// Cash button

$(document).on('click', '#btnCash', function () {
    if ($('#CashTable tbody tr').length > 0) {
        captureModalSnapshot('CashTable');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).show();
        return;
    }
    $.ajax({
        url: '/PurchaseForm/GetCashDetails',
        type: 'GET',
        data: {},
        success: function (data) {
            $('#CashTable tbody').html(data.cashTable);
            captureModalSnapshot('CashTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).show();
        },
        error: function () {
            Swal.fire('Error', 'Failed to load Cash details.', 'error');
        }
    });
});

// ── Snapshot helpers for dirty-check ──────────────────────────────────────
var modalSnapshots = {};
var modalHtmlSnapshots = {}; // { 'CashTable': 'serialized string', ... }

function getModalDataString(tableId) {
    var parts = [];
    $('#' + tableId + ' tbody tr').each(function () {
        $(this).find('input').each(function () {
            parts.push($(this).val());
        });
    });
    return parts.join('||');
}

function captureModalSnapshot(tableId) {
    modalSnapshots[tableId] = getModalDataString(tableId);
    modalHtmlSnapshots[tableId] = $('#' + tableId + ' tbody').html();

    // sync each input's value ATTRIBUTE to its current value so the
    // stored HTML snapshot actually reflects what's on screen
    $('#' + tableId + ' tbody tr').each(function () {
        $(this).find('input').each(function () {
            $(this).attr('value', $(this).val());
        });
    });
    modalHtmlSnapshots[tableId] = $('#' + tableId + ' tbody').html();
}

function isModalDirty(tableId) {
    var current = getModalDataString(tableId);
    var original = modalSnapshots[tableId] || '';
    return current !== original;
}

// ── Sum the Amount column of a modal table and push the total into the
//    corresponding disabled Payment Details input (Cash / Card / Cheque) ──
function updatePaymentField(sourceTableId, amountClass, fieldId) {
    var total = 0;

    $('#' + sourceTableId + ' tbody tr').each(function () {
        var amt = parseFloat($(this).find('.' + amountClass).val()) || 0;
        total += amt;
    });

    $('#' + fieldId).val(total.toFixed(2));
    $('#' + fieldId).trigger('change'); // kept, harmless if it fires

    // Guaranteed recalculation — doesn't depend on the disabled field's
    // change event actually reaching the delegated handler
    recalcTotalPaidAndBalance();
}
// Shared recalculation used both by the #Cash/#Card/#Cheque change handler
// AND called directly by updatePaymentField() as a guaranteed path —
// since #Cash/#Card/#Cheque are disabled, relying only on .trigger('change')
// to reach the delegated handler is unreliable across browsers/jQuery versions.
function recalcTotalPaidAndBalance() {
    const cash = parseFloat($('#Cash').val()) || 0;
    const card = parseFloat($('#Card').val()) || 0;
    const cheque = parseFloat($('#Cheque').val()) || 0;

    const totalPaid = cash + card + cheque;
    $('#TotalPaid').val(toTwoDecimal(totalPaid));

    calculateBalance();
}

// Save button — Cash modal
$(document).on('click', '#btnSaveCash', function () {
    updatePaymentField('CashTable', 'CashAmount', 'Cash');
    captureModalSnapshot('CashTable');
    bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
});

// Save button — Card modal
$(document).on('click', '#btnSaveCard', function () {
    updatePaymentField('CardTable', 'CardAmount', 'Card');
    captureModalSnapshot('CardTable');
    bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
});

// Save button — Cheque modal
$(document).on('click', '#btnSaveCheque', function () {
    updatePaymentField('ChequeTable', 'ChequeAmount', 'Cheque');
    captureModalSnapshot('ChequeTable');
    bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
});

// Close button — Cash modal
$(document).on('click', '#btnCloseCash, #btnCloseCash2', function () {
    if (!isModalDirty('CashTable')) {
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
        return;
    }
    Swal.fire({
        title: 'Save Changes?',
        text: 'Do you want to save the data entered?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, Save',
        cancelButtonText: 'No, Close',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            updatePaymentField('CashTable', 'CashAmount', 'Cash');
            captureModalSnapshot('CashTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
        }
        else {
            $('#CashTable tbody').html(modalHtmlSnapshots['CashTable']);
            captureModalSnapshot('CashTable');
            updatePaymentField('CashTable', 'CashAmount', 'Cash');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
        }
    });
});

// Close button — Card modal
$(document).on('click', '#btnCloseCard, #btnCloseCard2', function () {
    if (!isModalDirty('CardTable')) {
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
        return;
    }
    Swal.fire({
        title: 'Save Changes?',
        text: 'Do you want to save the data entered?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, Save',
        cancelButtonText: 'No, Close',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            updatePaymentField('CardTable', 'CardAmount', 'Card');
            captureModalSnapshot('CardTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
        }
        else {
            $('#CardTable tbody').html(modalHtmlSnapshots['CardTable']);
            captureModalSnapshot('CardTable');
            updatePaymentField('CardTable', 'CardAmount', 'Card');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
        }
    });
});

// Close button — Cheque modal
$(document).on('click', '#btnCloseCheque, #btnCloseCheque2', function () {
    if (!isModalDirty('ChequeTable')) {
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
        return;
    }
    Swal.fire({
        title: 'Save Changes?',
        text: 'Do you want to save the data entered?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, Save',
        cancelButtonText: 'No, Close',
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            updatePaymentField('ChequeTable', 'ChequeAmount', 'Cheque');
            captureModalSnapshot('ChequeTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
        }
        else {
            $('#ChequeTable tbody').html(modalHtmlSnapshots['ChequeTable']);
            captureModalSnapshot('ChequeTable');
            updatePaymentField('ChequeTable', 'ChequeAmount', 'Cheque');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
        }
    });
});


$(document).on("click", ".addrowCash", function (event) {
    event.preventDefault();
    var serialno = $(this).attr('serialno');
    $.ajax({
        url: "/PurchaseForm/NewRowCash?no=" + serialno,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#CashTable tbody").append(data.newrow);
            }
        },
        error: function () {
            Swal.fire({ icon: 'error', title: 'Error!', text: 'Failed to add row.', confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
        }
    });
});

$(document).on("click", ".addrowCard", function (event) {
    event.preventDefault();
    var serialno = $(this).attr('serialno');
    $.ajax({
        url: "/PurchaseForm/NewRowCard?no=" + serialno,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#CardTable tbody").append(data.newrow);
            }
        },
        error: function () {
            Swal.fire({ icon: 'error', title: 'Error!', text: 'Failed to add row.', confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
        }
    });
});

$(document).on("click", ".addrowCheque", function (event) {
    event.preventDefault();
    var serialno = $(this).attr('serialno');
    $.ajax({
        url: "/PurchaseForm/NewRowCheque?no=" + serialno,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#ChequeTable tbody").append(data.newrow);
            }
        },
        error: function () {
            Swal.fire({ icon: 'error', title: 'Error!', text: 'Failed to add row.', confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
        }
    });
});

//Changed and optimized code on 25-04-2026 START
$(document).on("keyup", ".productCode", function (event) {
    const id = $(this).attr("element-id");

    if (event.keyCode == 13) {
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
                'VoucherID': $("#VoucherID").val()
            },
            dataType: 'JSON',
            success: function (data) {
                if (data.success) {
                    // STEP 1 - Set unit dropdown & image
                    // STEP 2 - Parse unitList, taxList
                    // STEP 3 - Set ItemRate(ItemUnitPrice)
                    // STEP 4 - Set Qty = 1, calculate GrossAmt / Total / Amt
                    // STEP 5 - Set Tax % using taxItem(taxList)
                    // STEP 6 - Calculate TaxAmt using GrossAmt (now correctly set in Step 4)
                    // STEP 7 - Recalculate Total = GrossAmt + TaxAmt
                    // STEP 8 - updateAllSums() + calculateGrandTotal()
                    // STEP 9 - Focus on Qty field

                    console.log(data)
                    $("#ItemUnit" + id).html(data['units']);
                    var imagesrc = data['imagesrc'];
                    $("#productimagepreview" + id).attr("src", "");
                    $("#productimagepreview" + id).attr("src", imagesrc);

                    const unitList = JSON.parse(data['unitDetails']);
                    const unit = unitList.length > 0 ? unitList[0] : {};
                    const taxList = JSON.parse(data['taxDetails']);
                    const taxItem = taxList.length > 0 ? taxList[0] : {};

                    let ItemUnitPrice = parseFloat(data['itemUnitPrice']) || 0;
                    if (!ItemUnitPrice) {
                        ItemUnitPrice = parseFloat(unit["OnlinePrice"]) || 0;
                    }
                    $("#ItemRate" + id).val(parseFloat(ItemUnitPrice).toFixed(2));
                    $("#ItemRate" + id).css('text-align', 'right');
                    $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

                    const qtyField = $("#ItemQty" + id);
                    const rateField = $("#ItemRate" + id);
                    const grossAmtField = $("#ItemGrossAmt" + id);
                    const totalField = $("#ItemTotal" + id);
                    const amtField = $("#ItemAmt" + id);

                    qtyField.val(parseFloat(1).toFixed(2));
                    qtyField.css('text-align', 'center');

                    const qty = parseFloat(qtyField.val()) || 0;
                    const rate = parseFloat(rateField.val()) || 0;

                    if (taxList.length > 0) {
                        if (taxItem["SalesPerc"] != null && taxItem["SalesPerc"] !== "") {
                            $("#ItemTaxPer" + id).val(taxItem["SalesPerc"]);
                            $("#ItemTaxPer" + id).attr('taxTypeID', taxItem["ID"]);
                            $("#ItemTaxPer" + id).attr('taxaccountid', taxItem["TaxAccountID"]);

                        } else {
                            $("#ItemTaxPer" + id).attr('taxTypeID', "");
                            $("#ItemTaxPer" + id).attr('taxaccountid', "");
                        }
                    }

                    if (taxList.length > 0) {
                        if (taxItem["SalesPerc"] != null && taxItem["SalesPerc"] !== "") {
                            $("#ItemTaxPer" + id).val(taxItem["SalesPerc"]);
                            $("#ItemTaxPer" + id).attr('taxTypeID', taxItem["ID"]);
                        } else {
                            $("#ItemTaxPer" + id).attr('taxTypeID', "");
                        }
                    }
                    const taxPerVal = $("#ItemTaxPer" + id).val();
                    if (taxPerVal != null && taxPerVal !== "") {
                        $("#ItemTaxPer" + id).val(parseFloat(taxPerVal).toFixed(2));

                        const amt = parseFloat($("#ItemGrossAmt" + id).val()) || 0;
                        const taxAmt = amt * (parseFloat(taxPerVal) / 100);  // renamed from 'tax' to 'taxAmt'

                        $("#ItemTaxAmt" + id).val(parseFloat(taxAmt).toFixed(2));
                        $("#ItemTaxAmt" + id).css('text-align', 'right');

                        const sum = amt + taxAmt;
                        $("#ItemTotal" + id).val(parseFloat(sum).toFixed(2));
                        $("#ItemTotal" + id).css('text-align', 'right');
                    }

                    updateAllSums();
                    calculateGrandTotal();

                    $("#ItemQty" + id).focus();
                }
            },
        });
        $("#ItemQty" + id).focus();

    }
});

$(document).on("change", ".ItemUnit", function () {
    const id = $(this).attr('element-id');
    const selectedUnit = $(this).val();
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
    if (!selectedUnit) {
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemDiscPer" + id).val('');
        $("#ItemDiscAmt" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTotal" + id).val('');
        $("#ItemGrossAmt" + id).val('');  //  .val() not .html()
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
            'VoucherID': $("#VoucherID").val(),
            'Unit': selectedUnit
        },
        dataType: 'JSON',
        success: function (data) {
            if (data.success) {
                console.log(data);

                $("#ItemUnit" + id).html(data['units']);

                const unitList = JSON.parse(data['unitDetails']);
                const taxList = JSON.parse(data['taxDetails']);
                const unit = unitList.length > 0 ? unitList[0] : {};
                const taxItem = taxList.length > 0 ? taxList[0] : {};  //  renamed from 'tax'

                console.log(unitList);
                console.log(unit);

                // STEP 1 - Set Rate
                let ItemUnitPrice = parseFloat(data['itemUnitPrice']) || 0;
                if (!ItemUnitPrice) {
                    ItemUnitPrice = parseFloat(unit["OnlinePrice"]) || 0;
                }
                console.log("ItemUnitPrice:", ItemUnitPrice);  //  removed alerts

                $("#ItemRate" + id).val(toTwoDecimal(ItemUnitPrice)).css('text-align', 'right');
                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

                // STEP 2 - Set Qty and calculate GrossAmt
                const qtyField = $("#ItemQty" + id);
                const rateField = $("#ItemRate" + id);
                const grossAmtField = $("#ItemGrossAmt" + id);
                const totalField = $("#ItemTotal" + id);
                const amtField = $("#ItemAmt" + id);

                qtyField.val(toTwoDecimal(1)).css('text-align', 'center');

                const qty = parseFloat(qtyField.val()) || 0;
                const rate = parseFloat(rateField.val()) || 0;

                if (qty > 0) {
                    const tot = qty * (rate > 0 ? rate : ItemUnitPrice);
                    grossAmtField.val(toTwoDecimal(tot)).css('text-align', 'right');
                    amtField.val(toTwoDecimal(tot)).css('text-align', 'right');
                }

                // STEP 3 - Set Tax% 
                if (taxList.length > 0) {
                    if (taxItem["SalesPerc"] != null && taxItem["SalesPerc"] !== "") {
                        $("#ItemTaxPer" + id).val(taxItem["SalesPerc"]);
                        $("#ItemTaxPer" + id).attr('taxTypeID', taxItem["ID"]);
                        $("#ItemTaxPer" + id).attr('taxaccountid', taxItem["TaxAccountID"]);

                    } else {
                        $("#ItemTaxPer" + id).attr('taxTypeID', "");
                        $("#ItemTaxPer" + id).attr('taxaccountid', "");
                    }
                }

                // STEP 4 - Calculate TaxAmt using GrossAmt
                const taxPerVal = $("#ItemTaxPer" + id).val();
                if (taxPerVal != null && taxPerVal !== "") {
                    $("#ItemTaxPer" + id).val(toTwoDecimal(taxPerVal));

                    const amt = parseFloat(grossAmtField.val()) || 0;  //  GrossAmt already set
                    const taxAmt = amt * (parseFloat(taxPerVal) / 100);

                    $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt)).css('text-align', 'right');

                    const sum = amt + taxAmt;
                    totalField.val(toTwoDecimal(sum)).css('text-align', 'right');
                }

                // STEP 5 - Update sums and focus
                updateAllSums();
                calculateGrandTotal();
                $("#ItemQty" + id).focus();
            }
        }
    });
});

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
        url: CommonUrl + "NewRow?no=" + serialno,
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

$(document).on("click", ".action_delete", function (event) {
    var $Row = $(this).closest('tr');
    var $TableBody = $Row.closest('tbody');
    var rowCount = $TableBody.find('tr').length;

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
                //  Existing row - delete via AJAX
                $.ajax({
                    url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove();
                            ReindexRows();
                            updateAllSums();        //  Replaces all repeated sum blocks
                            calculateGrandTotal();  //  Recalculate grand total
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
            } else {
                //  New unsaved row - just remove from DOM
                $Row.remove();
                ReindexRows();
                updateAllSums();        //  Replaces all repeated sum blocks
                calculateGrandTotal();  //  Recalculate grand total
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

//Changed and optimized code on 25-04-2026 END
function ReindexRows() {
    $('#Itemtable tbody tr').each(function (index) {
        var rowIndex = index + 1; // start from 1
        $(this).find('td.serial-no').text(rowIndex);
    });
}

function CloseEntry() {
    location.reload()
}

$('#Mode').on('change', function () {
    var selectedText = $(this).find('option:selected').text().trim();

    if (selectedText === 'Credit') {
        $('#Card').prop('disabled', true).val('');
        $('#Cheque').prop('disabled', false);
    } else if (selectedText === 'Cash') {
        $('#Cheque').prop('disabled', true).val('');
        $('#Card').prop('disabled', false);
    } else {
        $('#Card').prop('disabled', false);
        $('#Cheque').prop('disabled', false);
    }
});

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
        if ($(this).val() != '') {
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
            var factor = parseFloat($("#ItemRate" + i).attr('element-factor')) || 1;
            var qty = parseFloat($("#ItemQty" + i).val()) || 0;
            var itemAry = {
                'ID': InTransItemId,
                'TransactionID': idValue,
                'ItemID': (rawItem !== undefined && rawItem !== null && rawItem.toString().trim() !== "" && !isNaN(rawItem) && parseInt(rawItem) > 0)
                    ? parseInt(rawItem)
                    : null,
                'Unit': $("#ItemUnit" + i).val() || '', // Nullable string
                'Qty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : null, // Nullable float
                'BasicQty': factor * qty,
                'Rate': $("#ItemRate" + i).val() ? parseFloat($("#ItemRate" + i).val()) : null, // Nullable float
                'RowType': $("#RowType").val() ? parseInt($("#RowType").val()) : null,  //     
                'Description': $("#Description").val() || '',
                'Discount': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
                'Factor': factor,
                'StockQty': factor * qty,
                'InLocID': $("#Warehouse").val() ? parseInt($("#Warehouse").val()) : null, // Nullable int
                'DiscountPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxPerc': $("#ItemTaxPer" + i).val() ? parseFloat($("#ItemTaxPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxValue': $("#ItemTaxAmt" + i).val() ? parseFloat($("#ItemTaxAmt" + i).val()) : 0, // Nullable float (default to 0)
                'TaxTypeID': (rawTaxType && !isNaN(rawTaxType) && parseInt(rawTaxType) > 0)
                    ? parseInt(rawTaxType)
                    : null,
                'RateDiscPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'RateDisc': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
                'SerialNo': parseInt($(this).closest('tr').find('.serial-no').text()) || null,
                'TaxAccountID': ($("#ItemTaxPer" + i).attr('taxaccountid') && !isNaN($("#ItemTaxPer" + i).attr('taxaccountid')))
                    ? parseInt($("#ItemTaxPer" + i).attr('taxaccountid'))
                    : null,
                'TempQty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0, // Nullable float (default to 0)
                'RowState': (InTransItemId === null || InTransItemId === 0 || InTransItemId === undefined) ? 1 : 2,
            }
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
    var modeId = parseInt($("#Mode").val()) || 0;
    var modeName = $("#Mode option:selected").text();
    // Build transaction object
    var transaction = {
        ID: idValue, // Nullable int
        Date: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        EffectiveDate: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        VoucherID: $("#VoucherType").attr("data-value") ? parseInt($("#VoucherType").attr("data-value")) : null, // Nullable int
        SerialNo: $("#VoucherNo").val() ? parseInt($("#VoucherNo").val()) : null, // Nullable long
        TransactionNo: $("#VoucherNo").val() || '', // Nullable string
        ExchangeRate: 1.0, // Decimal, assuming default of 1.0
        ReferenceNo: $("#Reference").val() || '', // Nullable string
        CommonNarration: $("#Description").val() || '', // Nullable string
        AddedDate: new Date().toISOString(), // DateTime
        AccountID: (rawParty !== undefined && rawParty !== null && rawParty.trim() !== "" && !isNaN(rawParty) && parseInt(rawParty) > 0)
            ? parseInt(rawParty)
            : null,
        CostCentreID: (rawProject !== undefined && rawProject !== null && rawProject.trim() !== "" && !isNaN(rawProject) && parseInt(rawProject) > 0)
            ? parseInt(rawProject)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2,
        // Payment details
        CashAmount: $("#Cash").val(),
        CardAmount: $("#Card").val(),
        CreditAmount: null,
        Discount: null,
        IsCredit: 0,
        TaxAmount: $("#Tax").val(),
        PaymentTypeID: modeId,
    };

    var rawWarehouse = $("#Warehouse").val();
    var Paid = $("#TotalPaid").val();
    var additionals = {
        TransactionID: idValue, // Nullable int
        TypeID: null, // Explicitly set as null
        ModeID: $("#Mode").val() ? parseInt($("#Mode").val()) : null, // Nullable int
        ToLocationID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,
        Name: $("#PartyNameAddress").val() || '', // Nullable string
        Address: $("#TermsofDelivery").val() || '', // Nullable string
        DocumentNo: $("#DispatchNo").val() || '', // Nullable string
        DocumentDate: $("#DispatchDate").val() ? new Date($("#DispatchDate").val()) : null, // Nullable DateTime
        EntryDate: $("#PartyInvoiceDate").val() ? new Date($("#PartyInvoiceDate").val()) : null, // Nullable DateTime
        EntryNo: $("#PartyInvoiceNo").val() || '', // Nullable string
        BankAddress: $("#Attention").val() || '', // Nullable string
        ExpiryDate: $("#ExpiryDate").val() ? new Date($("#ExpiryDate").val()) : null, // Nullable DateTime
        SubmitDate: $("#DeliveryNoteDate").val() ? new Date($("#DeliveryNoteDate").val()) : null, // Nullable DateTime
        PassNo: $("#DeliveryNoteNo").val() || '', // Nullable string
        ReferenceDate: $("#OrderDate").val() ? new Date($("#OrderDate").val()) : null, // Nullable DateTime
        ReferenceNo: $("#OrderNo").val() || '', // Nullable string
        Terms: $("#Terms").val() || '', // Nullable string
        InLocID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,
        AccountID: null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var entriesarray = {

        'RowState': (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2,
        'DueDate': $("#DueDate").val() ? new Date($("#DueDate").val()) : null,
        'TransactionID': idValue,
        'DrCr': 'C',
        'Nature': 'M',
        'Amount': Paid,
        'AccountID': (rawParty !== undefined && rawParty !== null && rawParty.trim() !== "" && !isNaN(rawParty) && parseInt(rawParty) > 0)
            ? parseInt(rawParty)
            : null,
        'FCAmount': Paid,
        'BankDate': null,
        'RefPageTypeID': null,
        'RefPageTableID': null,
        'ReferenceNo': null,
        'Description': null,
        'CommonNarration': null,
        'TranType': 'Party',
        'RefTransID': null,
        'CurrencyID': null,
        'ExchangeRate': null,
        'RefTransactionID': null,
        'ExchRate': null,
        'TaxPerc': null,
        //'ID': $("#EntryID").val() ? parseInt($("#EntryID").val()) : null,
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
    if (modeName == "Cash") { // assuming 1 = Cash
        var balance = parseFloat($("#Balance").val()) || 0;

        if (Math.abs(balance) > 0.01) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: 'For Cash payment, Balance must be 0.',
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33'
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
                    window.location.href = "/PurchaseForm/Index?MenuID=" + menuId;
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
function RowClick(RowID) {
    EntryEnable('Edit')
    $("#PurchaseForm8List").hide();
    $("#PurchaseForm8Form").show();

    $.ajax({
        url: webUrl + "GetInventoryTransaction?ID=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                function toFixedNoRound(num, decimals) {
                    num = Number(num);
                    if (isNaN(num)) return "0.00";
                    const factor = Math.pow(10, decimals);
                    return (Math.trunc(num * factor) / factor).toFixed(decimals);
                }

                var header = data['trans'];
                transaction = JSON.parse(header)[0];
                var fiadditional = data['fiadditional'];
                additionals = JSON.parse(fiadditional)[0];

                $("#vouchernodiv").show();
                $("#ID").val(transaction.ID)
                $("#VoucherNo").val(transaction.TransactionNo)
                $("#VoucherCode").val(transaction.Code);
                $("#Reference").val(transaction.ReferenceNo)
                $("#Mode").html(data['mode'])
                // Helper function to format date or return empty string if invalid
                function formatDate(dateStr) {
                    if (!dateStr || dateStr === '' || dateStr === ' ') return '';
                    const date = new Date(dateStr);
                    return date.getFullYear() + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        ("0" + date.getDate()).slice(-2);
                }
                // Apply the helper function to each date field
                $("#VoucherDate").val(formatDate(transaction.Date));
                $("#Party").val(transaction.AccountName);
                $("#Party").attr('data-idvalue', transaction.AccountID);
                $("#Description").val(transaction.CommonNarration);
                $("#Itemtable tbody").append(data['innerHTML']);
                $("#Warehouse").html(data['warehouses']);
                $("#Project").val(transaction.ProjectName);
                $("#Project").attr('data-idvalue', transaction.CostCentreID);
                $("#Account").val(data.account);

                //Additionals

                $("#PartyInvoiceNo").val(additionals.EntryNo);
                $("#PartyInvoiceDate").val(formatDate(additionals.EntryDate));
                $("#OrderNo").val(additionals.ReferenceNo);
                $("#OrderDate").val(formatDate(additionals.ReferenceDate));
                $("#PartyNameAddress").val(additionals.Name);
                $("#ExpiryDate").val(formatDate(additionals.ExpiryDate));
                $("#Attention").val(additionals.BankAddress);
                $("#DeliveryNoteNo").val(additionals.PassNo);
                $("#DeliveryNoteDate").val(formatDate(additionals.SubmitDate));
                $("#DispatchNo").val(additionals.DocumentNo);
                $("#DispatchDate").val(formatDate(additionals.DocumentDate));
                $("#TermsofDelivery").val(additionals.Address);
                $("#Terms").val(additionals.Terms);

                //Entries
                $("#DueDate").val(formatDate(data.duedate));
                $("#Cash").val(data.cash);
                $("#Card").val(data.card);
                $("#Cheque").val(data.cheque);
                $("#TotalPaid").val(data.totalpaid);
                $("#EntryID").val(data.entryid);

                // -------- SUM CALCULATIONS WITH NO ROUNDING --------
                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') qtysum += Number($(this).val());
                });
                $("#qtySum").html(toFixedNoRound(qtysum, 2)).css('text-align', 'center');

                var GrossAmt = 0;
                $('.ItemGrossAmt').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') GrossAmt += Number($(this).val());
                });
                $("#ItemGrossAmtSum").html(toFixedNoRound(GrossAmt, 2)).css('text-align', 'right');

                var dicsAmt = 0;
                $('.ItemDiscAmt').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') dicsAmt += Number($(this).val());
                });
                $("#dicsAmtSum").html(toFixedNoRound(dicsAmt, 2)).css('text-align', 'right');

                var ItemAmt = 0;
                $('.ItemAmt').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') ItemAmt += Number($(this).val());
                });
                $("#amtSum").html(toFixedNoRound(ItemAmt, 2)).css('text-align', 'right');

                var ItemTaxAmt = 0;
                $('.ItemTaxAmt').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') ItemTaxAmt += Number($(this).val());
                });
                $("#taxAmtSum").html(toFixedNoRound(ItemTaxAmt, 2)).css('text-align', 'right');

                var ItemTotal = 0;
                $('.ItemTotal').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') ItemTotal += Number($(this).val());
                });
                $("#itemTotalSum").html(toFixedNoRound(ItemTotal, 2)).css('text-align', 'right');

                $(".ItemQty").css('text-align', 'center');
                $(".ItemRate, .ItemGrossAmt, .ItemDiscAmt, .ItemAmt, .ItemTaxAmt, .ItemTotal")
                    .css('text-align', 'right');
                $(".ItemDiscPer").css('text-align', 'center');
                $(".ItemTaxPer").css('text-align', 'center');
                updateAllSums();        //  Replaces all repeated sum blocks
                calculateGrandTotal();  //  Recalculate grand total

                // Disable payment fields while editing
                $("#Tax").prop("disabled", true);
                $("#TotalDiscount").prop("disabled", true);
                $("#NetAmount").prop("disabled", true);
                $("#GrandTotal").prop("disabled", true);
                $("#Advance").prop("readonly", true);
                $("#Balance").prop("readonly", true);

                $("#Party").focus();
            }
            else {
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
        url: CommonUrl + "deleteTransactions?id=" + $("#ID").val(),
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
                    window.location.href = "/PurchaseForm/Index?MenuID=" + menuId;
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

    const qty = parseFloat($("#ItemQty" + id).val()) || 0;
    const rate = parseFloat($("#ItemRate" + id).val()) || 0;
    const grossAmt = qty * rate;

    // --- Discount ---
    let discAmt = parseFloat($("#ItemDiscAmt" + id).val()) || 0;
    let discPer = parseFloat($("#ItemDiscPer" + id).val()) || 0;

    if (changedField === 'ItemDiscAmt') {
        discPer = grossAmt ? (discAmt / grossAmt) * 100 : 0;
        $("#ItemDiscPer" + id).val(toTwoDecimal(discPer));
    } else if (changedField === 'ItemDiscPer') {
        discAmt = (grossAmt * discPer) / 100;
        $("#ItemDiscAmt" + id).val(toTwoDecimal(discAmt));
    } else {
        if (discPer > 0) {
            discAmt = (grossAmt * discPer) / 100;
            $("#ItemDiscAmt" + id).val(toTwoDecimal(discAmt));
        } else if (discAmt > 0) {
            discPer = grossAmt ? (discAmt / grossAmt) * 100 : 0;
            $("#ItemDiscPer" + id).val(toTwoDecimal(discPer));
        }
    }

    const amt = grossAmt - discAmt;

    // --- Tax ---
    let taxAmt = parseFloat($("#ItemTaxAmt" + id).val()) || 0;
    let taxPer = parseFloat($("#ItemTaxPer" + id).val()) || 0;

    if (changedField === 'ItemTaxAmt') {
        taxPer = amt ? (taxAmt / amt) * 100 : 0;
        $("#ItemTaxPer" + id).val(toTwoDecimal(taxPer));
    } else if (changedField === 'ItemTaxPer') {
        taxAmt = (amt * taxPer) / 100;
        $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt));
    } else {
        if (taxPer > 0) {
            taxAmt = (amt * taxPer) / 100;
            $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt));
        } else if (taxAmt > 0) {
            taxPer = amt ? (taxAmt / amt) * 100 : 0;
            $("#ItemTaxPer" + id).val(toTwoDecimal(taxPer));
        }
    }

    const total = amt + taxAmt;

    $("#ItemGrossAmt" + id).val(toTwoDecimal(grossAmt)).css('text-align', 'right');
    $("#ItemAmt" + id).val(toTwoDecimal(amt)).css('text-align', 'right');
    $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt)).css('text-align', 'right');
    $("#ItemTotal" + id).val(toTwoDecimal(total)).css('text-align', 'right');
}

function updateAllSums() {
    updateSum('.ItemQty', '#qtySum', 'center');
    updateSum('.ItemGrossAmt', '#ItemGrossAmtSum');
    updateSum('.ItemDiscAmt', '#dicsAmtSum');
    updateSum('.ItemAmt', '#amtSum');
    updateSum('.ItemTaxAmt', '#taxAmtSum');
    updateSum('.ItemTotal', '#itemTotalSum');

    // Get calculated totals from table
    const totalGross = getColumnSum('.ItemGrossAmt');
    const totalDiscount = getColumnSum('.ItemDiscAmt');
    const totalTax = getColumnSum('.ItemTaxAmt');
    const totalAmount = getColumnSum('.ItemTotal');

    // Push into Entries Form
    $('#TotalDiscount').val(toTwoDecimal(totalDiscount));
    $('#Tax').val(toTwoDecimal(totalTax));
    $('#NetAmount').val(toTwoDecimal(totalGross - totalDiscount));

    calculateGrandTotal();
}

$(document).on("input", ".ItemQty, .ItemRate, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {

    const id = $(this).attr('element-id');
    const changedField = $(this).attr("id").replace(/\d+$/, '');

    let val = $(this).val();

    if (val === "." || val.endsWith(".")) {
        return;
    }

    if (val !== "" && (val.includes('-') || isNaN(val) || Number(val) < 0)) {
        return;
    }

    calculateRow(id, changedField);
    updateAllSums();
});


$(document).on("blur", ".ItemQty, .ItemRate, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {
    const val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
    const id = $(this).attr('element-id');
    if (id) {
        calculateRow(id);
        updateAllSums();
    }
});
//03-03-2026
function getColumnSum(selector) {
    let sum = 0;

    $(selector).each(function () {
        const value =
            $(this).val() ||
            $(this).text() ||
            $(this).html();

        const num = parseFloat(value);

        if (!isNaN(num)) {
            sum += num;
        }
    });

    return sum;
}

function calculateGrandTotal() {
    const itemTotal = getColumnSum('.ItemTotal');
    const additional = parseFloat($('#AdditionalCharges').val()) || 0;

    const grandTotal = itemTotal + additional;
    $('#GrandTotal').val(toTwoDecimal(grandTotal));

    calculateBalance();
}

function calculateBalance() {
    const grandTotal = parseFloat($('#GrandTotal').val()) || 0;
    const advance = parseFloat($('#Advance').val()) || 0;
    const totalPaid = parseFloat($('#TotalPaid').val()) || 0;

    const balance = grandTotal - advance - totalPaid;
    $('#Balance').val(toTwoDecimal(balance));
}

$(document).on("input", "#AdditionalCharges", function () {
    calculateGrandTotal();
});

$(document).on("input", "#Advance, #TotalPaid", function () {
    calculateBalance();
});

$(document).on("input change", "#Cash, #Card, #Cheque", function () {
    recalcTotalPaidAndBalance();
});



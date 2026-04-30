

var webUrl = '/POS/';
var CommonUrl = '/CommonFunctions/';

// ENTER KEY GLOBAL HANDLER (Tab + ProductCode logic)
$(document).on('keydown', function (e) {

    if (e.key !== "Enter") {
        return;
    }
    if (e.target.tagName === "TEXTAREA") {
        return;
    }
    const $target = $(e.target);
    console.log($target);

    // =====================================
    // PRODUCT CODE ENTER LOGIC
    // =====================================
    if ($target.hasClass("productCode")) {
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
        $("#ItemTaxPer" + id).val('');

        updateAllSums();

        var data = {
            'ID': $target.attr('data-idvalue'),
            'AccountID': $("#Party").attr('data-idvalue'),
            'VoucherID': $("#VoucherID").val()
        }
        console.log(data);

        $.ajax({
            url: CommonUrl + "ProductAvailableUnits",
            method: "POST",
            data: data,
            dataType: 'JSON',
            success: function (data) {
                $("#ItemUnit" + id).html(data['units']);

                var unitList = JSON.parse(data['unitDetails']);
                var taxList = JSON.parse(data['taxDetails']);
                var unit = unitList.length > 0 ? unitList[0] : {};
                var tax = taxList.length > 0 ? taxList[0] : {};

                var ItemUnitPrice = parseFloat(data['itemUnitPrice']);
                if (isNaN(ItemUnitPrice) || data['itemUnitPrice'] == '') {
                    ItemUnitPrice = parseFloat(unit["SellingPrice"]) || 0;
                }

                var imagesrc = data['imagesrc'];
                $("#productimagepreview" + id).attr("src", "");
                $("#productimagepreview" + id).attr("src", imagesrc);

                // Set Tax
                if (taxList.length > 0 && tax["SalesPerc"] != null && tax["SalesPerc"] != "") {
                    $("#ItemTaxPer" + id).val(toTwoDecimal(tax["SalesPerc"]));
                    $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"]);
                } else {
                    $("#ItemTaxPer" + id).val('');
                    $("#ItemTaxPer" + id).attr('taxTypeID', '');
                }

                // Set Rate and Factor
                $("#ItemRate" + id).val(toTwoDecimal(ItemUnitPrice));
                $("#ItemRate" + id).css('text-align', 'right');
                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

                // Set default Qty
                $("#ItemQty" + id).val('1.00');
                $("#ItemQty" + id).css('text-align', 'center');

                // Recalculate row (tax + disc + total)
                calculateRow(id);

                // Update all column sums and grand total
                updateAllSums();

                // Focus qty
                $("#ItemQty" + id).focus();
            },
        });

        return;
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

// For refreshing the page same as a New entry
function NewEntry() {
    location.reload();
}

// For getting item price details while changing unit
$(document).on("change", ".ItemUnit", function () {
    const id = $(this).attr('element-id');
    const selectedUnit = $(this).val();

    if (!selectedUnit) {
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemDiscPer" + id).val('');
        $("#ItemDiscAmt" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTotal" + id).val('');
        $("#ItemGrossAmt" + id).val('');
        $("#ItemTaxAmt" + id).val('');
        $("#ItemTaxPer" + id).val('');
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

            // Set Tax
            if (taxList.length > 0 && tax["SalesPerc"] != null && tax["SalesPerc"] != "") {
                $("#ItemTaxPer" + id).val(toTwoDecimal(tax["SalesPerc"]));
                $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"]);
            } else {
                $("#ItemTaxPer" + id).val('');
                $("#ItemTaxPer" + id).attr('taxTypeID', '');
            }

            // Default Quantity = 1
            $("#ItemQty" + id).val('1');

            // Recalculate row and all sums
            calculateRow(id);
            updateAllSums();

            // Focus qty field
            $("#ItemQty" + id).focus();
        }
    });
});

// For adding new row for item add
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

// For deleting single item
$(document).on("click", ".action_delete", function (event) {
    var $Row = $(this).closest('tr');
    var $TableBody = $Row.closest('tbody');
    var rowCount = $TableBody.find('tr').length;

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
                $.ajax({
                    url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove();
                            ReindexRows();
                            updateAllSums();
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
                $Row.remove();
                ReindexRows();
                updateAllSums();
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
    if (isNaN(num)) return "0.00";
    const factor = Math.pow(10, decimals);
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

    if (rawQty === "") return;

    const qty = parseFloat(rawQty);
    if (isNaN(qty)) return;

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
        // On qty/rate change or fresh load: recalculate discAmt from discPer
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
        // On qty/rate/disc change or fresh load: always recalculate taxAmt from taxPer
        if (taxPer > 0) {
            taxAmt = (amt * taxPer) / 100;
            $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt));
        }
    }

    const total = amt + taxAmt;

    // Set system-calculated fields
    $("#ItemGrossAmt" + id).val(toTwoDecimal(grossAmt));
    $("#ItemGrossAmt" + id).css('text-align', 'right');
    $("#ItemAmt" + id).val(toTwoDecimal(amt));
    $("#ItemAmt" + id).css('text-align', 'right');
    $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt));
    $("#ItemTaxAmt" + id).css('text-align', 'right');
    $("#ItemTotal" + id).val(toTwoDecimal(total));
    $("#ItemTotal" + id).css('text-align', 'right');
}

function updateAllSums() {
    updateSum('.ItemQty', '#qtySum', 'center');
    updateSum('.ItemGrossAmt', '#ItemGrossAmtSum');
    updateSum('.ItemDiscAmt', '#dicsAmtSum');
    updateSum('.ItemAmt', '#amtSum');
    updateSum('.ItemTaxAmt', '#taxAmtSum');
    updateSum('.ItemTotal', '#itemTotalSum');

    // Set Total and TaxSummary from column sums
    const total = parseFloat($('#itemTotalSum').text()) || 0;
    const tax = parseFloat($("#taxAmtSum").text()) || 0;

    $("#TaxSummary").val(toTwoDecimal(tax));
    $('#Total').val(toTwoDecimal(total));

    calculateGrandTotal();
}

$(document).on("input", ".ItemQty, .ItemRate, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {

    const id = $(this).attr('element-id');
    const changedField = $(this).attr("id").replace(id, '');

    let val = $(this).val();

    // Allow natural typing
    if (val === "" || val === "." || val.endsWith(".")) {
        return;
    }

    // Reject bad values
    if (val.includes('-') || isNaN(val) || Number(val) < 0) {
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
    // Recalculate on blur to ensure everything is in sync
    const id = $(this).attr('element-id');
    if (id) {
        calculateRow(id);
        updateAllSums();
    }
});

function ReindexRows() {
    $('#Itemtable tbody tr').each(function (index) {
        var rowIndex = index + 1;
        $(this).find('td.serial-no').text(rowIndex);
    });
}

function calculateGrandTotal() {
    const total = parseFloat($('#Total').val()) || 0;
    const otherDiscount = parseFloat($('#OtherDiscount').val()) || 0;

    const grandTotal = total - otherDiscount;

    $('#GrandTotal').val(toTwoDecimal(grandTotal));
    calculatePayment();
}

$(document).on("input", "#OtherDiscount", function () {
    calculateGrandTotal();
});

$(document).on("blur", "#OtherDiscount", function () {
    const val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
    calculateGrandTotal();
});


$(document).on("input", ".moneyField", function () {
    const $field = $(this);
    let value = $field.val();

    value = value.replace(/[^0-9.]/g, '');

    const parts = value.split('.');
    if (parts.length > 2) {
        value = parts[0] + '.' + parts.slice(1).join('');
    }

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

// For total paid and Balance calculation
function toNumber(val) {
    return parseFloat(val) || 0;
}

function calculatePayment() {
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

$('#CashPaid, #CardPaid').on('input', function () {
    let val = $(this).val();

    // Allow typing (don't block user mid-input)
    if (!isNaN(val) && val !== "") {
        $(this).data('raw', val); // store raw value
    }

    calculatePayment();
});
$('#CashPaid, #CardPaid').on('blur', function () {
    let val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
});
// For saving entry
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
            return false;
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
        return false;
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
                'ID': InTransItemId,
                'TransactionID': idValue,
                'ItemID': (rawItem !== undefined && rawItem !== null && rawItem.toString().trim() !== "" && !isNaN(rawItem) && parseInt(rawItem) > 0)
                    ? parseInt(rawItem) : null,
                'Unit': $("#ItemUnit" + i).val() || '',
                'Qty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : null,
                'Rate': $("#ItemRate" + i).val() ? parseFloat($("#ItemRate" + i).val()) : null,
                'BasicQty': ($("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1) * ($("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0),
                'TaxPerc': $("#ItemTaxPer" + i).val() ? parseFloat($("#ItemTaxPer" + i).val()) : 0,
                'TaxValue': $("#ItemTaxAmt" + i).val() ? parseFloat($("#ItemTaxAmt" + i).val()) : 0,
                'RateDiscPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0,
                'RateDisc': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0,
                'DiscountPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0,
                'TaxTypeID': (rawTaxType && !isNaN(rawTaxType) && parseInt(rawTaxType) > 0)
                    ? parseInt(rawTaxType) : null,
                'TempQty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0,
                'Discount': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0,
                'Factor': $("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1,
                'StockQty': ($("#ItemRate" + i).attr('element-factor') ? parseFloat($("#ItemRate" + i).attr('element-factor')) : 1) * ($("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0),
                'OutLocID': $("#Warehouse").val() ? parseInt($("#Warehouse").val()) : null,
                'Description': $("#Description").val() || '',
                'RowType': $("#RowType").val(),
                'RowState': (InTransItemId === null || InTransItemId === 0 || InTransItemId === undefined) ? 1 : 2,
            };

            if ($(this).val() != '' && $("#ItemUnit" + i).val() != '' && $("#ItemQty" + i).val() != '' && $("#ItemRate" + i).val() != '') {
                items.push(itemAry);
            }
        } else {
            flag4 = false;
            return false;
        }
    });

    var rawProject = $("#Project").attr('data-idvalue');
    var rawParty = $("#Party").attr('data-idvalue');

    var transaction = {
        ID: idValue,
        Date: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null,
        EffectiveDate: $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null,
        VoucherID: $("#VoucherType").attr("data-value") ? parseInt($("#VoucherType").attr("data-value")) : null,
        TransactionNo: $("#VoucherNo").val() || '',
        SerialNo: $("#VoucherNo").val() ? parseInt($("#VoucherNo").val()) : null,
        ExchangeRate: 1.0,
        AddedDate: new Date().toISOString(),
        AccountID: (rawParty !== undefined && rawParty !== null && rawParty.trim() !== "" && !isNaN(rawParty) && parseInt(rawParty) > 0)
            ? parseInt(rawParty) : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var bank = $("#BankID").attr("data-value");
    var cash = $("#CashID").attr("data-value");
    var rawSalesman = $("#Salesman").attr('data-idvalue');
    var additionals = {
        TransactionID: idValue,
        ModeID: cash ? cash : bank,
        AllocationPerc: $("#OtherDiscount").val(),
        AccountID: (rawSalesman !== undefined && rawSalesman !== null && rawSalesman.toString().trim() !== "" && !isNaN(rawSalesman) && parseInt(rawSalesman) > 0)
            ? parseInt(rawSalesman) : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var entriesarray = {
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };
    entries.push(entriesarray);

    if (flag4 && flag1 && flag2 && flag3) {
        var model = {
            'InvTransItems': items,
            'FiTransactions': transaction,
            'FiTransactionAdditionals': additionals,
            'FiTransactionEntries': entries
        };
    } else {
        if (flag4 == false) {
            Swal.fire({ icon: 'error', title: 'Oops!', text: "Please enter the Item details", confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
            return false;
        }
        if (flag1 == false) {
            Swal.fire({ icon: 'error', title: 'Oops!', text: "Item Unit is mandatory", confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
            return false;
        }
        if (flag2 == false) {
            Swal.fire({ icon: 'error', title: 'Oops!', text: "Item Quantity is mandatory", confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
            return false;
        }
        if (flag3 == false) {
            Swal.fire({ icon: 'error', title: 'Oops!', text: "Item Rate is mandatory", confirmButtonText: 'Okay', confirmButtonColor: '#d33' });
            return false;
        }
    }

    console.log(model);

    $.ajax({
        url: webUrl + "InsertTransaction",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(model),
        dataType: 'json',
        beforeSend: function () {
            $(".loader-wrapper").fadeIn("fast");
        },
        success: function (data) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
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

// For getting item lookup and units (Not using now in POS)
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
        $("#ItemTaxPer" + id).val('');

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

                var unitList = JSON.parse(data['unitDetails']);
                var taxList = JSON.parse(data['taxDetails']);
                var unit = unitList.length > 0 ? unitList[0] : {};
                var tax = taxList.length > 0 ? taxList[0] : {};

                var ItemUnitPrice = parseFloat(data['itemUnitPrice']);
                if (isNaN(ItemUnitPrice) || data['itemUnitPrice'] == '') {
                    ItemUnitPrice = parseFloat(unit["SellingPrice"]) || 0;
                }

                var imagesrc = data['imagesrc'];
                $("#productimagepreview" + id).attr("src", "");
                $("#productimagepreview" + id).attr("src", imagesrc);

                // Set Tax
                if (taxList.length > 0 && tax["SalesPerc"] != null && tax["SalesPerc"] != "") {
                    $("#ItemTaxPer" + id).val(toTwoDecimal(tax["SalesPerc"]));
                    $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxMiscID"]);
                } else {
                    $("#ItemTaxPer" + id).val('');
                    $("#ItemTaxPer" + id).attr('taxTypeID', '');
                }

                // Set Rate and Factor
                $("#ItemRate" + id).val(toTwoDecimal(ItemUnitPrice));
                $("#ItemRate" + id).css('text-align', 'right');
                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

                // Set default Qty
                $("#ItemQty" + id).val('1.00');
                $("#ItemQty" + id).css('text-align', 'center');

                // Recalculate row and all sums
                calculateRow(id);
                updateAllSums();

                $("#ItemQty" + id).focus();
            },
        });

        $("#ItemUnit" + id).focus();
    }
});
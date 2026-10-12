var webUrl = '/PaymentVoucher/';
var CommonUrl = '/PaymentVoucher/';

// ─── New Entry ─────
function NewEntry() {
    $("#PaymentVoucherList").hide();
    $("#PaymentVoucherForm").show();

    $.ajax({
        url: webUrl + "NewEntryDetails",
        method: "POST",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#VoucherNo").prop("disabled", true);

                var now = new Date();
                var date = now.getFullYear() + '-'
                    + ("0" + (now.getMonth() + 1)).slice(-2) + '-'
                    + ("0" + now.getDate()).slice(-2);
                $("#VoucherDate").val(date);

                $("#AccountDetails tbody").append(data.accountdetails);
                $("#VoucherDate").focus();
                ReindexRows();
            } else {
                Swal.fire({
                    icon: 'error', title: 'Oops!',
                    text: data.message || 'Failed to load new entry form.',
                    confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error', title: 'Error!',
                text: 'Something went wrong while loading the form.',
                confirmButtonText: 'Okay', confirmButtonColor: '#d33'
            });
        }
    });
}

// Card button
$(document).on('click', '#btnCard', function () {
    if ($('#CardTable tbody tr').length > 0) {
        captureModalSnapshot('CardTable');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).show();
        return;
    }
    var accountid = $("#AccountID").val($(".AccountCode").attr("data-idvalue")).val();
    $.ajax({
        url: '/PaymentVoucher/GetCardDetails',
        type: 'GET',
        data: { AccountID: accountid },
        success: function (data) {
            $('#CardTable tbody').html(data.cardTable);
            $('#lblCardBalance').html(data.balance);
            captureModalSnapshot('CardTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).show();
        },
        error: function () {
            Swal.fire('Error', 'Failed to load Card details.', 'error');
        }
    });
});

// Cheque button
$(document).on('click', '#btnCheque', function () {
    if ($('#ChequeTable tbody tr').length > 0) {
        captureModalSnapshot('ChequeTable');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).show();
        return;
    }
    var accountid = $("#AccountID").val($(".AccountCode").attr("data-idvalue")).val();
    $.ajax({
        url: '/PaymentVoucher/GetChequeDetails',
        type: 'GET',
        data: {AccountID:accountid},
        success: function (data) {
            $('#ChequeTable tbody').html(data.chequeTable);
            $('#lblChequeBalance').html(data.balance);
            captureModalSnapshot('ChequeTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).show();
        },
        error: function () {
            Swal.fire('Error', 'Failed to load Cheque details.', 'error');
        }
    });
});

// Cash button
$(document).on('click', '#btnCash', function () {
    if ($('#CashTable tbody tr').length > 0) {
        captureModalSnapshot('CashTable');
        bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).show();
        return;
    }
    var accountid = $("#AccountID").val($(".AccountCode").attr("data-idvalue")).val();
    $.ajax({
        url: '/PaymentVoucher/GetCashDetails',
        type: 'GET',
        data: { AccountID: accountid },
        success: function (data) {
            $('#CashTable tbody').html(data.cashTable);
            $('#lblCashBalance').html(data.balance);
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

// ── Push ALL rows from a modal table to Payment Details ───────────────────
function pushToPaymentDetails(sourceTableId, paymentType) {
    var paymentTbody = $('#PaymentDetails tbody');

    paymentTbody.find('tr').filter(function () {
        return $(this).find('td').eq(3).text().trim() === paymentType;
    }).remove();

    $('#' + sourceTableId + ' tbody tr').each(function () {
        var inputs = $(this).find('input');
        var account, description, amount, accountid;

        //if (sourceTableId === 'ChequeTable') {
        //    account = inputs.eq(0).val();
        //    description = inputs.eq(5).val();
        //    amount = inputs.eq(6).val();
        //    accountid = inputs.eq(0).attr('data-idvalue') || null;
        //} else {
        //    account = inputs.eq(0).val();
        //    description = inputs.eq(1).val();
        //    amount = inputs.eq(2).val();
        //    accountid = inputs.eq(0).attr('data-idvalue') || null;
        //}

        if (sourceTableId === 'ChequeTable') {
            account = $(this).find('.ChequeAccountCode').val();
            description = $(this).find('.ChequeAccountDescription').val();
            amount = $(this).find('.ChequeAmount').val();
            accountid = $(this).find('.ChequeAccountCode').attr('data-idvalue') || null;
        }
        else if (sourceTableId === 'CashTable') {
            account = $(this).find('.CashAccountCode').val();
            description = $(this).find('.CashAccountDescription').val();
            amount = $(this).find('.CashAmount').val();
            accountid = $(this).find('.CashAccountCode').attr('data-idvalue') || null;
        }
        else if (sourceTableId === 'CardTable') {
            account = $(this).find('.CardAccountCode').val();
            description = $(this).find('.CardAccountDescription').val();
            amount = $(this).find('.CardAmount').val();
            accountid = $(this).find('.CardAccountCode').attr('data-idvalue') || null;
        }

        if (account === '' && amount === '') return;

        var rowCount = 1;
        paymentTbody.find('tr').each(function () {
            var eid = parseInt($(this).attr('element-id')) || 0;
            if (eid >= rowCount) rowCount = eid + 1;
        });
        var entryId = $(this).attr('dataentry-id') || '';
        var row = '<tr class="PayRow" id="PayRow' + rowCount + '" element-id="' + rowCount + '">' +
            '<td style="width:15cm" class="PaymentAccount" id="PaymentAccount' + rowCount + '" accountid="' + accountid + '">' + account + '</td>' +
            '<td style="width:20cm" id="PAccountDescription' + rowCount + '">' + description + '</td>' +
            '<td style="text-align:right;width:5cm" id="DebitAmount' + rowCount + '">' + amount + '</td>' +
            '<td id="TranType' + rowCount + '" style="width:5cm;min-width:5cm;">' + paymentType + '</td>' +
            '<td style="display:none;" Pitemid="' + entryId + '"></td>' +
            '</tr>';
        paymentTbody.append(row);
    });
}

// Save button — Cash modal
$(document).on('click', '#btnSaveCash', function () {
    pushToPaymentDetails('CashTable', 'Cash');
    captureModalSnapshot('CashTable');
    bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
});

// Save button — Card modal
$(document).on('click', '#btnSaveCard', function () {
    pushToPaymentDetails('CardTable', 'Card');
    captureModalSnapshot('CardTable');
    bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
});

// Save button — Cheque modal
$(document).on('click', '#btnSaveCheque', function () {
    pushToPaymentDetails('ChequeTable', 'Cheque');
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
            pushToPaymentDetails('CashTable', 'Cash');
            captureModalSnapshot('CashTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCashModal')).hide();
        }
        else {
            $('#CashTable tbody').html(modalHtmlSnapshots['CashTable']);
            captureModalSnapshot('CashTable');
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
            pushToPaymentDetails('CardTable', 'Card');
            captureModalSnapshot('CardTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeCardModal')).hide();
        }
        else {
            $('#CardTable tbody').html(modalHtmlSnapshots['CardTable']);
            captureModalSnapshot('CardTable');
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
            pushToPaymentDetails('ChequeTable', 'Cheque');
            captureModalSnapshot('ChequeTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
        }
        else {
            $('#ChequeTable tbody').html(modalHtmlSnapshots['ChequeTable']);
            captureModalSnapshot('ChequeTable');
            bootstrap.Modal.getOrCreateInstance(document.getElementById('paymentModeChequeModal')).hide();
        }
    });
});
// ─── Add Row ─────────────────────────────────────────
$(document).on("click", ".addrow", function (event) {
    event.preventDefault();

    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');
    var item = $("#AccountCode" + currentId).val();

    if (!item || !item.trim()) {
        Swal.fire({
            title: "Missing Account Name",
            text: "Please enter the Account Name before proceeding.",
            icon: "warning"
        });
        return;
    }

    $.ajax({
        url: "/PaymentVoucher/NewRowAccount?no=" + serialno,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#AccountDetails tbody").append(data.newrow);
                ReindexRows();
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error', title: 'Error!',
                text: 'Failed to add a new row. Please try again.',
                confirmButtonText: 'Okay', confirmButtonColor: '#d33'
            });
        }
    });
});

$(document).on("click", ".addrowCash", function (event) {
    event.preventDefault();
    var serialno = $(this).attr('serialno');
    $.ajax({
        url: "/PaymentVoucher/NewRowCash?no=" + serialno,
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
        url: "/PaymentVoucher/NewRowCard?no=" + serialno,
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
        url: "/PaymentVoucher/NewRowCheque?no=" + serialno,
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

// ─── Reindex serial numbers ─────────────────────────
function ReindexRows() {
    $('#AccountDetails tbody tr').each(function (index) {
        $(this).find('td.serial-no').text(index + 1);
    });
}

function ReindexModalRows(tableId) {
    $('#' + tableId + ' tbody tr').each(function (index) {
        $(this).find('td.serial-no').text(index + 1);
    });
}

// ─── Delete Row ────────────────────────────────────────
$(document).on("click", ".action_delete", function () {
    var $row = $(this).closest('tr');
    var $tbody = $row.closest('tbody');
    var rowCount = $tbody.find('tr').length;

    if (rowCount <= 1) {
        Swal.fire({
            icon: 'warning', title: 'Warning!',
            text: 'You cannot delete the last remaining row. If any changes are needed, please update the data.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return;
    }

    var id = $(this).attr('element-id');
    var itemid = $("#itemid" + id).val();

    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to remove it?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#1c7430',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: "No, forget it."
    }).then((result) => {
        if (!result.isConfirmed) return;

        if (itemid != null && itemid !== "") {
            $.ajax({
                url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                method: "DELETE",
                dataType: "JSON",
                success: function (data) {
                    if (data.success === true) {
                        $row.remove();
                        ReindexRows();
                        formatAndRecalculate();
                        //autoFillLastRowBalance();
                        Swal.fire({
                            toast: true, position: 'top-end',
                            icon: 'success',
                            title: data.message || 'Entry deleted successfully!',
                            showConfirmButton: false,
                            timer: 3000, timerProgressBar: true
                        });
                    } else {
                        Swal.fire({
                            icon: 'error', title: 'Oops!',
                            text: data.message || 'Failed to delete entry.',
                            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error', title: 'Error!',
                        text: 'Something went wrong while deleting.',
                        confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                    });
                }
            });
        } else {
            $row.remove();
            ReindexRows();
            formatAndRecalculate();
            //autoFillLastRowBalance();
            Swal.fire({
                toast: true, position: 'top-end',
                icon: 'success', title: "Entry Deleted Successfully!",
                showConfirmButton: false,
                timer: 3000, timerProgressBar: true
            });
        }
    });
});

// ─── Delete Row — Cash Modal ────────────────────────────
$(document).on("click", ".deleterowCash", function (e) {
    e.preventDefault();
    var $row = $(this).closest('tr');
    var $tbody = $row.closest('tbody');

    if ($tbody.find('tr').length <= 1) {
        Swal.fire({
            icon: 'warning', title: 'Warning!',
            text: 'You cannot delete the last remaining row.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to remove this row?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#1c7430',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: 'No, forget it.'
    }).then((result) => {
        if (!result.isConfirmed) return;
        $row.remove();
        ReindexModalRows('CashTable');
        Swal.fire({
            toast: true, position: 'top-end',
            icon: 'success', title: 'Row deleted!',
            showConfirmButton: false,
            timer: 3000, timerProgressBar: true
        });
    });
});

// ─── Delete Row — Card Modal ────────────────────────────
$(document).on("click", ".deleterowCard", function (e) {
    e.preventDefault();
    var $row = $(this).closest('tr');
    var $tbody = $row.closest('tbody');

    if ($tbody.find('tr').length <= 1) {
        Swal.fire({
            icon: 'warning', title: 'Warning!',
            text: 'You cannot delete the last remaining row.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to remove this row?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#1c7430',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: 'No, forget it.'
    }).then((result) => {
        if (!result.isConfirmed) return;
        $row.remove();
        ReindexModalRows('CardTable');
        Swal.fire({
            toast: true, position: 'top-end',
            icon: 'success', title: 'Row deleted!',
            showConfirmButton: false,
            timer: 3000, timerProgressBar: true
        });
    });
});

// ─── Delete Row — Cheque Modal ────────────────────────────
$(document).on("click", ".deleterowCheque", function (e) {
    e.preventDefault();
    var $row = $(this).closest('tr');
    var $tbody = $row.closest('tbody');

    if ($tbody.find('tr').length <= 1) {
        Swal.fire({
            icon: 'warning', title: 'Warning!',
            text: 'You cannot delete the last remaining row.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return;
    }

    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to remove this row?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#1c7430',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: 'No, forget it.'
    }).then((result) => {
        if (!result.isConfirmed) return;
        $row.remove();
        ReindexModalRows('ChequeTable');
        Swal.fire({
            toast: true, position: 'top-end',
            icon: 'success', title: 'Row deleted!',
            showConfirmButton: false,
            timer: 3000, timerProgressBar: true
        });
    });
});

// ─── AccountCode Enter key ───────────────────────────────────────────────────
//$(document).on("keyup", ".AccountCode", function (event) {
//    if (event.keyCode !== 13) return;

//    var id = $(this).attr("element-id");
//    var totals = getPaymentTotals();

//    if (totals.debit > totals.credit) {
//        $("#Credit" + id).val((totals.debit - totals.credit).toFixed(2)).css('text-align', 'right');
//    } else if (totals.credit > totals.debit) {
//        $("#Debit" + id).val((totals.credit - totals.debit).toFixed(2)).css('text-align', 'right');
//    }

//    formatAndRecalculate();
//});

// ─── Format to 2 decimals on blur — Credit (main table) ─────────────────────

$(document).on("blur", ".Credit", function () {
    var val = parseFloat($(this).val());
    if (!isNaN(val) && val > 0) {
        $(this).val(val.toFixed(2)).css('text-align', 'right');
    } else {
        $(this).val('');
    }
});

// ─── Format to 2 decimals on blur — Cash/Card/Cheque Amount (modal tables) ───
$(document).on("blur", ".CashAmount, .CardAmount, .ChequeAmount", function () {
    var val = parseFloat($(this).val());
    if (!isNaN(val) && val > 0) {
        $(this).val(val.toFixed(2)).css('text-align', 'right');
    } else {
        $(this).val('');
    }
});

// ─── Real-time Credit update (main table) ────────────────────────────────────
//$(document).on("keyup change paste", ".Credit", function () {
//    //updatePaymentTotals();
//    autoFillLastRowBalance();
//});

// ─── Real-time amount update — modal tables ───────────────────────────────────

$(document).on("keyup change paste", ".CashAmount, .CardAmount, .ChequeAmount", function () {
    var val = $(this).val().trim();
    if (val !== '' && !isNaN(val)) {
        $(this).css('text-align', 'right');
    }
});

//$(document).on("change", ".AccountCode", function () {
    
//    var i = $(this).attr('element-id');
//    var accountid = $("#AccountCode" + i).attr('data-idvalue');
//    $("#AccountID" + i).val(accountid || '');
//    alert(i)
//    alert(accountid)
//})
// ─── Auto-fill last row balance ──
function autoFillLastRowBalance() {
    var $rows = $('#AccountDetails tbody tr');
    var totalRows = $rows.length;

    if (totalRows < 2) return;

    var $lastRow = $rows.last();
    var lastRowId = $lastRow.find('.Credit').attr('id')
        ? $lastRow.find('.Credit').attr('id').replace('Credit', '')
        : null;

    if (!lastRowId) return;

    var creditSum = 0;

    $rows.not(':last').each(function () {
        var rowId = $(this).find('.Credit').attr('id')
            ? $(this).find('.Credit').attr('id').replace('Credit', '')
            : null;
        if (!rowId) return;
        var c = parseFloat($("#Credit" + rowId).val()) || 0;
        creditSum += c;
    });

    if (creditSum > 0) {
        $("#Credit" + lastRowId).val(creditSum.toFixed(2)).css('text-align', 'right');
    } else {
        $("#Credit" + lastRowId).val('').css('text-align', 'right');
    }

    //updatePaymentTotals();
}

// ─── Helpers ─────────────────────────────────────────────────────────────────
//function toTwoDecimal(val) {
//    return parseFloat(val || 0).toFixed(2);
//}

//function getPaymentTotals() {
//    var debit = 0, credit = 0;
//    $('.Debit').each(function () { var v = parseFloat($(this).val()); if (!isNaN(v)) debit += v; });
//    $('.Credit').each(function () { var v = parseFloat($(this).val()); if (!isNaN(v)) credit += v; });
//    return { debit: debit, credit: credit };
//}

//function updateVoucherSum(selector, targetSelector) {
//    var sum = 0;
//    $(selector).each(function () {
//        var val = parseFloat($(this).val());
//        if (!isNaN(val)) sum += val;
//    });
//    $(targetSelector).text(toTwoDecimal(sum)).css('text-align', 'right');
//}

//function updatePaymentTotals() {
//    updateVoucherSum('.Credit', '#CAmtSum');
//}

function formatAndRecalculate() {
    $('.Credit').each(function () {
        var val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2)).css('text-align', 'right');
        }
    });
    //updatePaymentTotals();
}

// To convert text to date format in date fields
function formatDateInput(input) {
    let value = input.value.replace(/\D/g, '');

    if (value.length >= 2) {
        value = value.substring(0, 2) + '-' + value.substring(2);
    }
    if (value.length >= 5) {
        value = value.substring(0, 5) + '-' + value.substring(5, 9);
    }

    input.value = value;
}
//to avoide the date and month mismatch issue while saving  
function convertToISODate(dateStr) {
    if (!dateStr) return null;

    // expecting dd-mm-yyyy or dd/mm/yyyy
    var parts = dateStr.split(/[-\/]/);

    if (parts.length !== 3) return null;

    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    if (!day || !month || !year) return null;

    var date = new Date(year, month - 1, day);

    // validation (VERY IMPORTANT)
    if (
        date.getFullYear() !== year ||
        date.getMonth() !== month - 1 ||
        date.getDate() !== day
    ) {
        return null;
    }

    return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
}
// ─── Save Entry ───────────────────
function SaveEntry() {
    if ($("#PaymentTransactionID").val() != null && $("#PaymentTransactionID").val() !== "") {
        if ($("#VoucherNo").val().trim() === '') {
            $("#VoucherNo").focus();
            Swal.fire({
                icon: 'error', title: 'Oops!',
                text: "Voucher No is mandatory",
                confirmButtonText: 'Okay', confirmButtonColor: '#d33'
            });
            return false;
        }
    }

    if (!$("#VoucherDate").val() || $("#VoucherDate").val().trim() === '') {
        $("#VoucherDate").focus();
        Swal.fire({
            icon: 'error', title: 'Oops!',
            text: "Voucher Date is mandatory",
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }

    var idValue = $("#PaymentTransactionID").val()
        ? parseInt($("#PaymentTransactionID").val())
        : null;

    var flag1 = true; // Credit Account
    var flag2 = true; // Payment Account

    var flagCreditAmount = true;
    var flagDebitAmount = true;
    var totalCredit = 0;
    var accountentries = [];
    var paymententries = [];
    var accountID = null;
    $('.AccountCode').each(function () {

        var i = $(this).attr('element-id');
        var FiTransEntriesID = parseInt($("#itemid" + i).val()) || 0;
        //var debit = $("#DebitAmount" + i).html().trim();
        var credit = $("#Credit" + i).val().trim();
        var rawItem = $("#AccountCode" + i).attr('data-idvalue');

        //if (debit === '' && credit === '') { flag1 = false; return false; }

        accountID = (rawItem !== undefined && rawItem !== null &&
            rawItem.toString().trim() !== '' &&
            !isNaN(rawItem) && parseInt(rawItem) > 0)
            ? parseInt(rawItem) : null;

        if (accountID === null) { flag1 = false; return false; }

        var amt = parseFloat(credit);

        if (isNaN(amt) || amt <= 0) {
            flagCreditAmount = false;
            return false;
        }

        totalCredit += amt;
        var dueDate = $("#DueDate" + i).val();

        accountentries.push({
            'ID': FiTransEntriesID,
            'TransactionID': idValue,
            'AccountID': accountID,
            'DrCr': 'D',
            'Nature': 'M',
            'Amount': amt,
            'FCAmount': null,
            'BankDate': null,
            'RefPageTypeID': null,
            'RefPageTableID': null,
            'ReferenceNo': null,
            'Description': $("#AccountDescription" + i).val() || null,
            'CommonNarration': $("#AccountDescription" + i).val() || null,
            'TranType': null,
            'DueDate': convertToISODate(dueDate),
            'RefTransID': null,
            'CurrencyID': null,
            'ExchangeRate': null,
            'RefTransactionID': null,
            'ExchRate': null,
            'TaxPerc': null,
            'RowState': (FiTransEntriesID === null || FiTransEntriesID === 0 || FiTransEntriesID === undefined) ? 1 : 2,
        });
    });
    // Calculate Debit Total from Payment Details
    var totalDebit = 0;

    $('.PayRow').each(function () {

        var i = $(this).attr('element-id');
        var FiTransEntriesID =
            parseInt($(this).find('td[Pitemid]').attr('Pitemid')) || 0;
        var debit = $("#DebitAmount" + i).html().trim();
        var trantype = $("#TranType" + i).html().trim();
        var rawItem = $("#PaymentAccount" + i).attr('accountid');

        var PaccountID = (rawItem !== undefined && rawItem !== null &&
            rawItem.toString().trim() !== '' &&
            !isNaN(rawItem) && parseInt(rawItem) > 0)
            ? parseInt(rawItem) : null;

        if (PaccountID === null) {
            flag2 = false;
            return false;
        }
        var amt = parseFloat(debit);

        if (isNaN(amt) || amt <= 0) {
            flagDebitAmount = false;
            return false;
        }

        totalDebit += amt; // only for validation
        var description =
            $("#PAccountDescription" + i).html().trim();

        paymententries.push({
            'ID': FiTransEntriesID,
            'TransactionID': idValue,
            'AccountID': PaccountID,
            'DrCr': 'C',
            'Nature': 'M',
            'Amount': amt,
            'FCAmount': null,
            'BankDate': null,
            'RefPageTypeID': null,
            'RefPageTableID': null,
            'ReferenceNo': null,
            'Description': description,
            'CommonNarration': description,
            'TranType': trantype,
            'DueDate': null,
            'RefTransID': null,
            'CurrencyID': null,
            'ExchangeRate': null,
            'RefTransactionID': null,
            'ExchRate': null,
            'TaxPerc': null,
            'RowState': (FiTransEntriesID === null || FiTransEntriesID === 0 || FiTransEntriesID === undefined) ? 1 : 2,
        });
    });
    console.log(totalCredit + " totalCredit");
    console.log(totalDebit + " totalDebit");

    var chequeentries = [];
    $('#ChequeTable tbody tr').each(function () {
        //var inputs = $(this).find('input');
        //var accountCode = inputs.eq(0);
        //var chequeNo = inputs.eq(1).val();
        //var chequeDate = inputs.eq(2).val();
        //var bankCode = inputs.eq(3);
        //var clrDays = inputs.eq(4).val();
        //var FiChequesID =
        //    parseInt($(this).find('td[chequeitemid]').attr('chequeitemid')) || 0;

        var accountCode = $(this).find('.ChequeAccountCode');
        var chequeNo = $(this).find('.ChequeNo').val();
        var chequeDate = $(this).find('.ChequeDate').val();
        var convertedDate = convertToISODate(chequeDate);
        var bankCode = $(this).find('.BankAccountCode');
        var clrDays = $(this).find('.ChequeClearingDays').val();

        var FiChequesID =
            parseInt($(this).find('td[chequeitemid]').attr('chequeitemid')) || 0;

        if (accountCode.val().trim() === '') return;

        chequeentries.push({
            'ID': FiChequesID,
            'ChequeNo': chequeNo || null,
            'ChequeDate': convertedDate,
            'BankID': parseInt(bankCode.attr('data-idvalue')) || null,
            'BankName': bankCode.val() || null,
            'ClrDays': parseInt(clrDays) || null,
            'PartyID': accountID,
            'RowState': (FiChequesID === null || FiChequesID === 0 || FiChequesID === undefined) ? 1 : 2,
        });
    });

    var entries = accountentries.concat(paymententries);
    var accountRowCount = accountentries.length;

    var hasCash = paymententries.some(x => x.TranType === "Cash");
    var hasCheque = paymententries.some(x => x.TranType === "Cheque");

    if (accountRowCount > 1 && hasCash && hasCheque) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Combination',
            text: 'Both Cash or Bank is not allowed.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }
    if (!flag1) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Account',
            text: 'Please select a valid account.'
        });
        return false;
    }

    if (!flag2) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Account',
            text: 'Please select a payment account.'
        });
        return false;
    }

    if (!flagCreditAmount) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Amount',
            text: 'Please enter a valid credit amount.'
        });
        return false;
    }

    if (!flagDebitAmount) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Amount',
            text: 'Please enter a valid debit amount.'
        });
        return false;
    }
    if (accountentries.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Account Details Required',
            text: 'Please add at least one account entry.'
        });
        return false;
    }
    if (paymententries.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Payment Details Required',
            text: 'Please add at least one payment entry.'
        });
        return false;
    }
    // Check Credit Amount exists
    if (totalCredit <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Credit Amount',
            text: 'Please enter a Credit amount in Account Details.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }

    // Check Debit Amount exists
    if (totalDebit <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Debit Amount',
            text: 'Please enter Payment details.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }

    // Check Debit = Credit
    if (parseFloat(totalCredit.toFixed(2)) !== parseFloat(totalDebit.toFixed(2))) {
        Swal.fire({
            icon: 'warning',
            title: 'Amount Mismatch',
            text: 'Total Credit amount must be equal to Total Debit amount.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
        return false;
    }

    var headerModel = {
        'ID': idValue,
        'Date': $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null,
        'EffectiveDate': $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null,
        'VoucherID': parseInt($("#VoucherType").attr("data-value")) || null,
        'TransactionNo': $("#VoucherNo").val() || null,
        'SerialNo': $("#VoucherNo").val() ? parseInt($("#VoucherNo").val()) : null,
        'IsPostDated': false,
        'ExchangeRate': 1.00,
        'RefPageTypeID': null,
        'RefPageTableID': null,
        'ReferenceNo': $("#Reference").val() || null,
        'FinYearID': null,
        'InstrumentNo': null,
        'InstrumentDate': null,
        'InstrumentBank': null,
        'CommonNarration': $("#Description").val() || null,
        'ApprovedBy': null,
        'AddedDate': new Date().toISOString(),
        'ApprovedDate': null,
        'ApproveNote': null,
        'AccountID': accountID,
        'Description': null,
        'RowState': (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var departmentid = $("#Department").attr('data-idvalue')
    var additionals = {
        TransactionID: idValue,
        TypeID: null,
        ModeID: null,
        ToLocationID: null,
        InLocID: null,
        Name: '',
        DocumentNo: '',
        DocumentDate: null,
        EntryDate: null,
        EntryNo: '',
        BankAddress: '',
        ExpiryDate: null,
        PassNo: '',
        ReferenceDate: null,
        ReferenceNo: '',
        SubmitDate: null,
        Address: '',
        Terms: '',
        AccountID: null,
        DepartmentID: (departmentid !== undefined && departmentid !== null && departmentid.trim() !== "" && !isNaN(departmentid) && parseInt(departmentid) > 0)
            ? parseInt(departmentid)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var payload = {
        'FiTransactions': headerModel,
        'FiTransactionAdditionals': additionals,
        'FiTransactionEntries': entries,
        'FiChequeEntry': chequeentries  // ← For cheque entry in FiCheques table only in DB
    };

    console.log("Payload to be sent:", JSON.stringify(payload));
    $.ajax({
        url: webUrl + "InsertTransaction",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(payload),
        dataType: 'json',
        beforeSend: function () {
            $(".loader-wrapper").fadeIn("fast");
        },
        success: function (data) {
            $(".loader-wrapper").fadeOut("slow", function () { $(this).hide(); });

            if (data.success) {
                if (data.id) $("#PaymentTransactionID").val(data.id);

                Swal.fire({
                    toast: true, position: 'top-end',
                    icon: 'success', title: "Saved successfully!",
                    showConfirmButton: false,
                    timer: 3000, timerProgressBar: true
                }).then(() => { location.reload(); });
            } else {
                Swal.fire({
                    icon: 'error', title: 'Oops!',
                    text: data.message || 'An error occurred while saving.',
                    confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                });
            }
        },
        error: function () {
            $(".loader-wrapper").fadeOut("slow", function () { $(this).hide(); });
            Swal.fire({
                icon: 'error', title: 'Error!',
                text: 'Something went wrong while saving. Please try again.',
                confirmButtonText: 'Okay', confirmButtonColor: '#d33'
            });
        }
    });
}

// ─── Row Click (Edit) ───
function RowClick(RowID) {
    $("#PaymentVoucherList").hide();
    $("#PaymentVoucherForm").show();
    EntryEnable('Edit');

    $.ajax({
        url: webUrl + "getVoucherPaymentEntries?ID=" + RowID,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#vouchernodiv").show();

                var header = data['header'];
                transaction = JSON.parse(header)[0];
                console.log("Fetched Transaction:", transaction);
                var fiadditional = data['additonals'];
                additionals = JSON.parse(fiadditional)[0];
                console.log("Fetched Additionals:", additionals);

                $("#PaymentTransactionID").val(transaction.ID);
                $("#VoucherNo").val(transaction.TransactionNo).prop("disabled", true);

                if (transaction.Code) $("#VoucherCode").val(transaction.Code);

                if (transaction.Date != null) {
                    var d = new Date(transaction.Date);
                    var date = d.getFullYear() + '-'
                        + ("0" + (d.getMonth() + 1)).slice(-2) + '-'
                        + ("0" + d.getDate()).slice(-2);
                    $("#VoucherDate").val(date);
                }

                $("#PaymentTransactionID").val(transaction.ID);
                $("#Description").val(transaction.CommonNarration);
                $("#Reference").val(transaction.ReferenceNo);
                $("#Department").val(additionals.Department);
                $("#Department").attr('data-idvalue', additionals.DepartmentID);
                $('#AccountDetails tbody').html("");
                $('#PaymentDetails tbody').html("");
                $('#CashTable tbody').html("");
                $('#CardTable tbody').html("");
                $('#ChequeTable tbody').html("");
                $("#AccountDetails tbody").append(data.accountDetails);
                $("#PaymentDetails tbody").append(data.paymentDetails);
                $("#CashTable tbody").append(data.cash);
                $("#CardTable tbody").append(data.card);
                $("#ChequeTable tbody").append(data.cheque);
                ReindexRows();

                $("#PaymentVoucherList").hide();
                $("#PaymentVoucherForm").show();

                formatAndRecalculate();
            } else {
                Swal.fire({
                    icon: 'error', title: 'Oops!',
                    text: data.message || "Data not fetched",
                    confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error', title: 'Error!',
                text: 'Something went wrong while loading the entry.',
                confirmButtonText: 'Okay', confirmButtonColor: '#d33'
            });
        }
    });
}

// ─── Delete Full Transaction ────
function DeleteEntry() {
    var transID = $("#PaymentTransactionID").val();

    if (!transID || transID.trim() === '') {
        Swal.fire({
            icon: 'warning', title: 'Nothing to Delete',
            text: 'No saved entry found to delete.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }

    Swal.fire({
        title: "Are you sure?",
        text: "This will permanently delete the entire voucher.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it.',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: CommonUrl + "DeleteTransactions?id=" + transID,
            method: "DELETE",
            dataType: "JSON",
            success: function (data) {
                if (data.success === true) {
                    Swal.fire({
                        toast: true, position: 'top-end',
                        icon: 'success', title: "Deleted successfully!",
                        showConfirmButton: false,
                        timer: 3000, timerProgressBar: true
                    }).then(() => { location.reload(); });
                } else {
                    Swal.fire({
                        icon: 'error', title: 'Oops!',
                        text: data.message || 'Failed to delete.',
                        confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error', title: 'Error!',
                    text: 'An error occurred while deleting.',
                    confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                });
            }
        });
    });
}

// ─── Close ───────────
function CloseEntry() {
    location.reload();
}
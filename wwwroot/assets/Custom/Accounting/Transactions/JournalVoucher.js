var webUrl = '/JournalVoucher/';
var CommonUrl = '/JournalVoucher/';

// ─── New Entry ───────────────────────────────────────────────────────────────
function NewEntry() {
    $("#JournalVoucherList").hide();
    $("#JournalVoucherForm").show();

    // FIX 1: removed async:false (deprecated, blocks browser UI thread)
    $.ajax({
        url: webUrl + "NewEntryDetails",
        method: "POST",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {                          // FIX 2: check success before using data
                $("#VoucherNo").prop("disabled", true);

                var now = new Date();
                var date = now.getFullYear() + '-'
                    + ("0" + (now.getMonth() + 1)).slice(-2) + '-'
                    + ("0" + now.getDate()).slice(-2);      // FIX 3: reuse one Date instance
                $("#VoucherDate").val(date);

                $("#Itemtable tbody").append(data.innerHTML);
                $("#VoucherDate").focus();
                ReindexRows();                                    // FIX 4: set serial no on first row
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

// ─── Add Row ─────────────────────────────────────────────────────────────────
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
        url: "/JournalVoucher/NewRow?no=" + serialno,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#Itemtable tbody").append(data.newrow);
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

// ─── Reindex serial numbers ──────────────────────────────────────────────────
function ReindexRows() {
    $('#Itemtable tbody tr').each(function (index) {
        $(this).find('td.serial-no').text(index + 1);
    });
}

// ─── Delete Row ──────────────────────────────────────────────────────────────
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
            // Saved row — delete via API
            $.ajax({
                url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                method: "DELETE",
                dataType: "JSON",
                success: function (data) {
                    if (data.success === true) {
                        $row.remove();
                        ReindexRows();
                        formatAndRecalculate();

                        //var totals = getJournalTotals();
                        //if (Math.abs(totals.debit - totals.credit) > 0.001) {  // FIX 5: numeric comparison
                        //    Swal.fire({
                        //        icon: 'warning', title: 'Warning!',
                        //        text: 'Debit and Credit totals do not match. Please ensure the totals are equal.',
                        //        confirmButtonText: 'Okay', confirmButtonColor: '#d33'
                        //    });
                        //}
                        //else {
                            Swal.fire({
                                toast: true, position: 'top-end',
                                icon: 'success',
                                title: data.message || 'Entry deleted successfully!',
                                showConfirmButton: false,
                                timer: 3000, timerProgressBar: true
                            });
                        //}
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
            // Unsaved row — remove from DOM only
            $row.remove();
            ReindexRows();
            formatAndRecalculate();
            Swal.fire({
                toast: true, position: 'top-end',
                icon: 'success', title: "Entry Deleted Successfully!",
                showConfirmButton: false,
                timer: 3000, timerProgressBar: true
            });
        }
    });
});

// ─── AccountCode Enter key ───────────────────────────────────────────────────
$(document).on("keyup", ".AccountCode", function (event) {
    if (event.keyCode !== 13) return;

    var id = $(this).attr("element-id");
    var totals = getJournalTotals();

    if (totals.debit > totals.credit) {
        $("#Credit" + id).val((totals.debit - totals.credit).toFixed(2)).css('text-align', 'right');
    } else if (totals.credit > totals.debit) {
        $("#Debit" + id).val((totals.credit - totals.debit).toFixed(2)).css('text-align', 'right');
    }

    formatAndRecalculate();
});

// ─── Real-time Debit/Credit update ───────────────────────────────────────────
$(document).on("keyup change paste", ".Debit, .Credit", function () {
    validateMutualExclusion(this);
    updateJournalTotals();
});

// ─── Helpers ─────────────────────────────────────────────────────────────────

function toTwoDecimal(val) {
    return parseFloat(val || 0).toFixed(2);
}

function getJournalTotals() {
    var debit = 0, credit = 0;
    $('.Debit').each(function () { var v = parseFloat($(this).val()); if (!isNaN(v)) debit += v; });
    $('.Credit').each(function () { var v = parseFloat($(this).val()); if (!isNaN(v)) credit += v; });
    return { debit: debit, credit: credit };
}

function updateVoucherSum(selector, targetSelector) {
    var sum = 0;
    $(selector).each(function () {
        var val = parseFloat($(this).val());
        if (!isNaN(val)) sum += val;
    });
    $(targetSelector).text(toTwoDecimal(sum)).css('text-align', 'right');
}

function updateJournalTotals() {
    updateVoucherSum('.Debit', '#DAmtSum');
    updateVoucherSum('.Credit', '#CAmtSum');
}

function formatAndRecalculate() {
    $('.Debit, .Credit').each(function () {
        var val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2)).css('text-align', 'right');
        }
    });
    updateJournalTotals();
}

function validateMutualExclusion(currentElement) {
    var $input = $(currentElement);
    var id = $input.attr("id").replace(/\D/g, '');
    var isDebit = $input.hasClass("Debit");
    var $other = isDebit ? $("#Credit" + id) : $("#Debit" + id);
    var val = parseFloat($input.val()) || 0;

    $input.css("text-align", "right");

    if (val > 0) {
        $other.val('').prop("readonly", true).css("background-color", "#f0f0f0");
    } else {
        $other.prop("readonly", false).css("background-color", "");
    }
}

// ─── Save Entry ──────────────────────────────────────────────────────────────
function SaveEntry() {

    // VoucherNo required only in Edit mode
    if ($("#JournalTransactionID").val() != null && $("#JournalTransactionID").val() !== "") {
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

    if (!$("#Description").val() || $("#Description").val().trim() === '') {
        $("#Description").focus();
        Swal.fire({
            icon: 'error', title: 'Oops!',
            text: "Description is mandatory",
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }

    var displayDebit = parseFloat($("#DAmtSum").text()) || 0;
    var displayCredit = parseFloat($("#CAmtSum").text()) || 0;
    if (Math.abs(displayDebit - displayCredit) > 0.001) {
        Swal.fire({
            icon: 'error', title: 'Oops!',
            text: "Debit and Credit amounts must tally",
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        $(".AccountCode").last().focus();
        return false;
    }

    var idValue = $("#JournalTransactionID").val()
        ? parseInt($("#JournalTransactionID").val())
        : null;

    var flag1 = true; // amount missing or invalid
    var flag2 = true; // AccountID invalid
    var flag3 = true; // both Debit & Credit filled

    var entries = [];

    $('.AccountCode').each(function () {
        var accountName = $(this).val();
        if (!accountName || accountName.trim() === '') return;
        var i = $(this).attr('element-id');
        var InTransItemId = parseInt($("#itemid" + i).val()) || 0;
        var debit = $("#Debit" + i).val().trim();
        var credit = $("#Credit" + i).val().trim();
        var rawItem = $("#AccountCode" + i).attr('data-idvalue');

        if (debit !== '' && credit !== '') { flag3 = false; return false; }
        if (debit === '' && credit === '') { flag1 = false; return false; }

        var accountID = (rawItem !== undefined && rawItem !== null &&
            rawItem.toString().trim() !== '' &&
            !isNaN(rawItem) && parseInt(rawItem) > 0)
            ? parseInt(rawItem) : null;

        if (accountID === null) { flag2 = false; return false; }

        var drcr, amt;
        if (debit !== '') { drcr = 'D'; amt = parseFloat(debit); }
        else { drcr = 'C'; amt = parseFloat(credit); }

        if (isNaN(amt) || amt <= 0) { flag1 = false; return false; }

        var dueDate = $("#DueDate" + i).val();

        entries.push({
            'ID': InTransItemId,
            'TransactionID': idValue,
            'AccountID': accountID,
            'DrCr': drcr,
            'Nature': 'M',
            'Amount': amt,
            'FCAmount': null,
            'BankDate': null,
            'RefPageTypeID': null,
            'RefPageTableID': null,
            'ReferenceNo': $("#AccountReference").val() || null,
            'Description': $("#AccountDescription" + i).val() || null,
            'CommonNarration': $("#AccountDescription" + i).val() || null,
            'TranType': null,
            'DueDate': (dueDate && dueDate.trim() !== '') ? dueDate : null,
            'RefTransID': null,
            'CurrencyID': 17,
            'ExchangeRate': 1.0,
            'RefTransactionID': null,
            'ExchRate': null,
            'TaxPerc': null,
            'RowState': (InTransItemId === null || InTransItemId === 0 || InTransItemId === undefined) ? 1 : 2,

        });
    });

    if (!flag3) {
        Swal.fire({
            icon: 'warning', title: 'Invalid Entry',
            text: 'A row cannot have both Debit and Credit amounts.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }
    if (!flag1) {
        Swal.fire({
            icon: 'warning', title: 'Missing Amount',
            text: 'Please enter a valid Debit or Credit amount for all entries.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }
    if (!flag2) {
        Swal.fire({
            icon: 'warning', title: 'Invalid Account',
            text: 'Please select a valid Account from the lookup for all entries.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }
    if (entries.length === 0) {
        Swal.fire({
            icon: 'warning', title: 'No Entries',
            text: 'Please add at least one journal entry before saving.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }

    var now = new Date();
    //var addedDate = now.getFullYear()
    //    + '-' + ("0" + (now.getMonth() + 1)).slice(-2)
    //    + '-' + ("0" + now.getDate()).slice(-2)
    //    + 'T' + ("0" + now.getHours()).slice(-2)      // ← T not space
    //    + ':' + ("0" + now.getMinutes()).slice(-2)
    //    + ':' + ("0" + now.getSeconds()).slice(-2)
    //    + '.' + ("00" + now.getMilliseconds()).slice(-3); // ← pad to 3 digits

    var headerModel = {
        'ID': idValue,
        'Date': $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        'EffectiveDate': $("#VoucherDate").val() ? new Date($("#VoucherDate").val()) : null, // Nullable DateTime
        'VoucherID': parseInt($("#VoucherType").attr("data-value")) || null,
        'TransactionNo': $("#VoucherNo").val() || null,
        'SerialNo': $("#VoucherNo").val() ? parseInt($("#VoucherNo").val()) : null, // Nullable long
        'IsPostDated': false,
        'ExchangeRate': 1.00,
        'RefPageTypeID': null,
        'RefPageTableID': null,
        'ReferenceNo': $("#Reference").val() || null,
        'FinYearID': null,
        'InstrumentNo': null,
        'InstrumentDate': null,
        'InstrumentBank': null,
        'CommonNarration': null,
        'ApprovedBy': null,
        'AddedDate': new Date().toISOString(), // DateTime
        'ApprovedDate': null,
        'ApproveNote': null,
        'AccountID': null,
        'Description': $("#Description").val() || null,
        'RowState': (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2

    };

    var payload = {
        'FiTransactions': headerModel,
        'FiTransactionEntries': entries
    };
    console.log("Payload to be sent:", JSON.stringify(payload)); // Debug log
    $.ajax({
        //url: webUrl + "InsertTransaction",
        //method: "POST",
        //dataType: "JSON",
        //data: payload,
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
                if (data.id) $("#JournalTransactionID").val(data.id);

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

// ─── Row Click (Edit) ─────────────────────────────────────────────────────────
function RowClick(RowID) {
    $("#JournalVoucherList").hide();
    $("#JournalVoucherForm").show();
    EntryEnable('Edit');

    $.ajax({
        url: webUrl + "getVoucherJournalEntries?ID=" + RowID,
        method: "GET",
        dataType: "JSON",
        success: function (data) {
            if (data.success === true) {
                $("#vouchernodiv").show();

                var header = JSON.parse(data.header)[0];
                $("#JournalTransactionID").val(header.ID)
                $("#VoucherNo").val(header.TransactionNo).prop("disabled", true);

                // FIX 6: VoucherCode field populated if present
                if (header.Code) $("#VoucherCode").val(header.Code);

                if (header.Date != null) {
                    var d = new Date(header.Date);
                    var date = d.getFullYear() + '-'
                        + ("0" + (d.getMonth() + 1)).slice(-2) + '-'
                        + ("0" + d.getDate()).slice(-2);
                    $("#VoucherDate").val(date);
                }

                $("#JournalTransactionID").val(header.ID);
                $("#Description").val(header.Description);
                $("#Reference").val(header.ReferenceNo);

                $("#Itemtable tbody").append(data.innerHTML);
                ReindexRows();             // FIX 7: reindex rows after load

                $("#ListFinanceJournalDiv").hide();
                $("#InputFinanceJournalDiv").show();

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

// ─── Delete Full Transaction ──────────────────────────────────────────────────
function DeleteEntry() {
    var transID = $("#JournalTransactionID").val();

    // FIX 8: also check for empty string, not just null
    if (!transID || transID.trim() === '') {
        Swal.fire({
            icon: 'warning', title: 'Nothing to Delete',
            text: 'No saved entry found to delete.',
            confirmButtonText: 'Okay', confirmButtonColor: '#d33'
        });
        return false;
    }

    // FIX 9: added confirmation before deleting entire transaction
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

// ─── Close ────────────────────────────────────────────────────────────────────
function CloseEntry() {
    location.reload();
}
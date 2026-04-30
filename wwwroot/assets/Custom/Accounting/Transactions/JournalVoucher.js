var webUrl = '/JournalVoucher/';
var CommonUrl = '/JournalVoucher/';


function NewEntry() {
    $("#JournalVoucherList").hide();
    $("#JournalVoucherForm").show();
    $.ajax({
        url: webUrl + "NewEntryDetails",
        method: "POST",
        dataType: 'JSON',
        async: false,
        success: function (data) {
            //  $("#VoucherNo").val(data['nextVNo'])
            $("#VoucherNo").prop("disabled", true)
            var date = new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2);
            $("#VoucherDate").val(date)
            var row = data['innerHTML'];
            $("#Itemtable tbody").append(row)
            $("#VoucherDate").focus();
        },
    });
}

$(document).on("click", ".addrow", function (event) {
    event.preventDefault();

    var $row = $(this).closest('tr');
    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');
    var item = $("#AccountCode" + currentId).val();

    if (!item.trim()) {
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
        dataType: 'JSON',
        success: function (data) {
            if (data.success == true) {
                var newrow = data['newrow'];
                $("#Itemtable tbody").append(newrow);
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
            text: 'You cannot delete the last remaining row. If any changes are needed, please update the data.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return; // Exit the click handler
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
        if (result.isConfirmed) {
            if (itemid != null && itemid !== "") {
                $.ajax({
                    url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove(); // Remove the deleted row from DOM

                            // Recalculate totals
                            let CreditTotal = 0;
                            $('.Credit').each(function () {
                                const val = parseFloat($(this).val());
                                if (!isNaN(val)) CreditTotal += val;
                            });

                            let DebitTotal = 0;
                            $('.Debit').each(function () {
                                const val = parseFloat($(this).val());
                                if (!isNaN(val)) DebitTotal += val;
                            });

                            // Update footer totals
                            $("#DAmtSum").text(toTwoDecimal(DebitTotal)).css('text-align', 'right');
                            $("#CAmtSum").text(toTwoDecimal(CreditTotal)).css('text-align', 'right');

                            // Recalculate other balances (if any)
                            let flag = null;
                            if (DebitTotal > CreditTotal) {
                                flag = 1; // debit exceeds
                                const diff = DebitTotal - CreditTotal;
                                $("#Credit" + id).val(diff.toFixed(2)).css('text-align', 'right');
                            } else if (CreditTotal > DebitTotal) {
                                flag = 0; // credit exceeds
                                const diff = CreditTotal - DebitTotal;
                                $("#Debit" + id).val(diff.toFixed(2)).css('text-align', 'right');
                            }

                            // Format all existing values
                            $('.Debit').each(function () {
                                const val = parseFloat($(this).val());
                                if (!isNaN(val)) {
                                    $(this).val(val.toFixed(2)).css('text-align', 'right');
                                }
                            });

                            $('.Credit').each(function () {
                                const val = parseFloat($(this).val());
                                if (!isNaN(val)) {
                                    $(this).val(val.toFixed(2)).css('text-align', 'right');
                                }
                            });

                            // Check if Debit and Credit totals are equal after the row is deleted
                            if (DebitTotal !== CreditTotal) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error!',
                                    text: 'Debit and Credit totals do not match. Please ensure the totals are equal.',
                                    confirmButtonText: 'Okay',
                                    confirmButtonColor: '#d33',
                                });
                                return; // Prevent further actions if totals are not equal
                            }

                            // Update footer totals
                            updateJournalTotals();

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
                $Row.remove(); // Remove the deleted row from DOM
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: "Entry Deleted Successfully!",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                });
                // Recalculate totals after row removal
                updateJournalTotals(); // Update the totals in the footer
            }
        }
    });
});


$(document).on("keyup", ".AccountCode", function (event) {
    if (event.keyCode == 13) {
        const id = $(this).attr("element-id");
        // Calculate totals
        let CreditTotal = 0;
        $('.Credit').each(function () {
            const val = parseFloat($(this).val());
            if (!isNaN(val)) CreditTotal += val;
        });

        let DebitTotal = 0;
        $('.Debit').each(function () {
            const val = parseFloat($(this).val());
            if (!isNaN(val)) DebitTotal += val;
        });

        let flag = null;
        if (DebitTotal > CreditTotal) {
            flag = 1; // debit exceeds
            const diff = DebitTotal - CreditTotal;
            $("#Credit" + id).val(diff.toFixed(2)).css('text-align', 'right');
        } else if (CreditTotal > DebitTotal) {
            flag = 0; // credit exceeds
            const diff = CreditTotal - DebitTotal;
            $("#Debit" + id).val(diff.toFixed(2)).css('text-align', 'right');
        }

        // Format all existing values
        $('.Debit').each(function () {
            const val = parseFloat($(this).val());
            if (!isNaN(val)) {
                $(this).val(val.toFixed(2)).css('text-align', 'right');
            }
        });

        $('.Credit').each(function () {
            const val = parseFloat($(this).val());
            if (!isNaN(val)) {
                $(this).val(val.toFixed(2)).css('text-align', 'right');
            }
        });

        // Update footer totals
        updateJournalTotals();
    }
});

function SaveEntry() {
    if ($("#JournalTransactionID").val() != null && $("#JournalTransactionID").val() != "") {
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
    if ($("#Description").val() == '' || $("#Description").val() == null) {
        $("#Description").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Description is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }

    if ($("#CAmtSum").text() != $("#DAmtSum").text()) {
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Debit and Credit amounts must be tally",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        $(".AccountCode").last().focus();
        return false;
    }
    var flag1 = true;
    var items = [];
    $('.AccountCode').each(function () {
        var i = $(this).attr('element-id');
        if ($(this).val() != '') {
            var InTransItemId = $("#itemid" + i).val();
            if ($("#Debit" + i).val() != '') {
                var drcr = 'D';
                var amt = $("#Debit" + i).val();
            } else {
                var amt = $("#Credit" + i).val();
                var drcr = 'C';
            }
            if ($("#Debit" + i).val() < 1 && $("#Credit" + i).val() < 1) {
                flag1 = false;
            }
            var itemAry = {
                'TransactionID': parseInt($("#JournalTransactionID").val()),
                'AccountID': parseInt($("#AccountCode" + i).attr('data-idvalue')),
                'ID': InTransItemId,
                'DrCr': drcr,
                'Nature': 'M',
                'Amount': amt,
                'FCAmount': null,
                'BankDate': null,
                'RefPageTypeID': null,
                'RefPageTableID': null,
                'ReferenceNo': $("#AccountReference").val(),
                'Description': $("#AccountDescription" + i).val(),
                'TranType': null,
                'DueDate': $("#DueDate" + i).val(),
                'RefTransID': null,
                'CurrencyID': 1,
                'ExchangeRate': 1.0,
                'RefTransactionID': null,
                'ExchRate': null,
                'TaxPerc': null,
            }
            items.push(itemAry);
        }
    });
    var model = {
        'ID': $("#JournalTransactionID").val(),
        'Date': $("#VoucherDate").val(),
        'EffectiveDate': $("#VoucherDate").val(),
        'VoucherID': $("#VoucherType").attr("data-value"),
        'TransactionNo': $("#VoucherNo").val(),
        'SerialNo': $("#VoucherNo").val(),
        'IsPostDated': false,
        'CurrencyID': 1,
        'ExchangeRate': 1.00,
        'RefPageTypeID': null,
        'RefPageTableID': null,
        'ReferenceNo': $("#Reference").val(),
        'CompanyID': 1,
        'FinYearID': null,
        'InstrumentType': null,
        'InstrumentNo': null,
        'InstrumentDate': null,
        'InstrumentBank': null,
        'CommonNarration': null,
        'ApprovedBy': null,
        'AddedDate': new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2) + ' ' + new Date().getHours() + ':' + new Date().getMinutes() + ':' + new Date().getMinutes() + '.' + new Date().getMilliseconds(),
        'ApprovedDate': null,
        'ApprovalStatus': 'A',
        'ApproveNote': null,
        'Action': null,
        'StatusID': 806,
        'IsAutoEntry': 0,
        'Posted': 1,
        'Active': 1,
        'Cancelled': 0,
        'AccountID': null,
        'Description': $("#Description").val(),
        'RefTransID': null,
        'EditedBy': null,
        'EditedDate': null,
        'CostCentreID': null,
        'MachineName': null,
    }

    if (flag1 == true) {
        var model = {
            'FiTransactions': model,
            'FiTransactionEntries': items
        }
    }
    else {
        if (flag1 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Please enter Debit or Credit",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
    }

    $.ajax({
        url: webUrl + "SaveVoucherJournalEntries",
        method: "POST",
        dataType: 'JSON',
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
                    location.reload();
                });

                $("#JournalTransactionID").val(data.transactionNo);

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
    $("#JournalVoucherList").hide();
    $("#JournalVoucherForm").show();
    $.ajax({
        url: webUrl + "getVoucherJournalEntries?ID=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                EntryEnable('Edit');
                $("#vouchernodiv").show();
                var header = data['header'];
                header = JSON.parse(header)[0];
                $("#VoucherNo").val(header.TransactionNo)
                $("#VoucherNo").prop("disabled", true)

                if (header.Date != null) {
                    var date = new Date(header.Date).getFullYear() + '-' + ("0" + (new Date(header.Date).getMonth() + 1)).slice(-2) + '-' + ("0" + new Date(header.Date).getDate()).slice(-2)
                    $("#VoucherDate").val(date)
                }

                var row = data['innerHTML'];

                $("#JournalTransactionID").val(header.ID);
                $("#Description").val(header.Description);
                $("#Reference").val(header.ReferenceNo);
                $("#Itemtable tbody").append(row)
                //Future enhancements
                $("#ListFinanceJournalDiv").hide();
                $("#InputFinanceJournalDiv").show();
                $('.Debit').each(function () {
                    if ($(this).val() != '') {
                        $(this).val(parseFloat($(this).val()).toFixed(2))
                        $(this).css('text-align', 'right');
                    }
                });
                $('.Credit').each(function () {
                    if ($(this).val() != '') {
                        $(this).val(parseFloat($(this).val()).toFixed(2))
                        $(this).css('text-align', 'right');
                    }
                });
                var CreditTotal = 0;
                $('.Credit').each(function () {
                    if ($(this).val() != '') {
                        CreditTotal += parseFloat($(this).val())
                    }
                });
                if (!isNaN(CreditTotal)) {
                    $("#CAmtSum").html(parseFloat(CreditTotal).toFixed(2))
                    $("#CAmtSum").css('text-align', 'right');
                }
                var DebitTotal = 0;
                $('.Debit').each(function () {
                    if ($(this).val() != '') {
                        DebitTotal += parseFloat($(this).val())
                    }
                });
                if (!isNaN(DebitTotal)) {
                    $("#DAmtSum").html(parseFloat(DebitTotal).toFixed(2))
                    $("#DAmtSum").css('text-align', 'right');
                }

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
    if ($("#JournalTransactionID").val() == null) {
        return false
    }
    $.ajax({
        url: CommonUrl + "DeleteTransactions?id=" + $("#JournalTransactionID").val(),
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
                     location.reload();
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

function CloseEntry() {
    location.reload()
}
function toTwoDecimal(val) {
    return parseFloat(val || 0).toFixed(2);
}

function updateVoucherSum(selector, targetSelector) {
    let sum = 0;
    $(selector).each(function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) sum += val;
    });
    $(targetSelector).text(toTwoDecimal(sum)).css('text-align', 'right');
}

function updateJournalTotals() {
    updateVoucherSum('.Debit', '#DAmtSum');
    updateVoucherSum('.Credit', '#CAmtSum');
}

function validateMutualExclusion(currentElement) {
    const $input = $(currentElement);
    const id = $input.attr("id").replace(/[^0-9]/g, ''); // Get numeric ID part
    const isDebit = $input.hasClass("Debit");
    const otherInput = isDebit ? $("#Credit" + id) : $("#Debit" + id);
    const val = parseFloat($input.val()) || 0;

    // Right-align the input fields when there is a value
    $input.css("text-align", "right");

    if (val > 0) {
        otherInput.val('').prop("readonly", true).css("background-color", "#f0f0f0");
    } else {
        otherInput.prop("readonly", false).css("background-color", "");
    }
}

// Keyup/change binding for real-time total update and validation
$(document).on("keyup change paste", ".Debit, .Credit", function () {
    validateMutualExclusion(this);
    updateJournalTotals();
});

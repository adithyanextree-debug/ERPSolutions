var webUrl = '/StockAdjustment/';
var CommonUrl = '/CommonFunctions/';

function NewEntry() {
    $("#StockAdjustmentList").hide();
    $("#StockAdjustmentForm").show();
    $.ajax({
        url: CommonUrl + "NewEntryDetailsStock",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            $("#VoucherNo").prop("disabled", true)

            var date = new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2);
            $("#VoucherDate").val(date)
            $("#Itemtable tbody").append(data['newEntry'])
            $("#Warehouse").html(data['warehouses'])
        },
    });
}

//Changed and optimized code on 25-04-2026 START
$(document).on("keyup", ".productCode", function (event) {
    const id = $(this).attr("element-id");

    if (event.keyCode == 13) {

        // Reset row fields
        $("#ItemUnit" + id).html('');
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTaxAmt" + id).val('');

        // Clear total summary
        updateAllSums();

        $.ajax({
            url: CommonUrl + "ProductAvailableUnits",
            method: "POST",
            data: {
                'ID': $(this).attr('data-idvalue'),
                'AccountID': null,
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
                    // STEP 7 - updateAllSums()
                    // STEP 8 - Focus on Qty field

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
                    const amtField = $("#ItemAmt" + id);

                    qtyField.val(parseFloat(1).toFixed(2));
                    qtyField.css('text-align', 'center');

                    const qty = parseFloat(qtyField.val()) || 0;
                    const rate = parseFloat(rateField.val()) || 0;

                    if (qty > 0) {
                        const tot = qty * (rate > 0 ? rate : ItemUnitPrice);

                        amtField.val(parseFloat(tot).toFixed(2)).css('text-align', 'right');
                    }

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
                    const taxPerVal = $("#ItemTaxPer" + id).val();
                    if (taxPerVal != null && taxPerVal !== "") {
                        $("#ItemTaxPer" + id).val(parseFloat(taxPerVal).toFixed(2));

                        const amt = parseFloat($("#ItemAmt" + id).val()) || 0;
                        const taxAmt = amt * (parseFloat(taxPerVal) / 100);  // renamed from 'tax' to 'taxAmt'

                        $("#ItemTaxAmt" + id).val(parseFloat(taxAmt).toFixed(2));
                        $("#ItemTaxAmt" + id).css('text-align', 'right');

                    }
                    updateAllSums();
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
    $("#ItemAmt" + id).val('');
    $("#ItemTaxAmt" + id).val('');

    // Clear total summary
    updateAllSums();
    if (!selectedUnit) {
        $("#ItemQty" + id).val('');
        $("#ItemRate" + id).val('');
        $("#ItemAmt" + id).val('');
        $("#ItemTaxAmt" + id).val('');
        updateAllSums();
        return;
    }

    $.ajax({
        url: CommonUrl + "ProductAvailableUnits",
        method: "POST",
        data: {
            'ID': $("#productCode" + id).attr('data-idvalue'),
            'AccountID': null,
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
                const amtField = $("#ItemAmt" + id);

                qtyField.val(toTwoDecimal(1)).css('text-align', 'center');

                const qty = parseFloat(qtyField.val()) || 0;
                const rate = parseFloat(rateField.val()) || 0;

                if (qty > 0) {
                    const tot = qty * (rate > 0 ? rate : ItemUnitPrice);
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

                    const amt = parseFloat(amtField.val()) || 0;  //  GrossAmt already set
                    const taxAmt = amt * (parseFloat(taxPerVal) / 100);

                    $("#ItemTaxAmt" + id).val(toTwoDecimal(taxAmt)).css('text-align', 'right');
                }

                // STEP 5 - Update sums and focus
                updateAllSums();
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
        url: CommonUrl + "NewRowStockItems?no=" + serialno,
        // url: "/StockAdjustment/NewRowStockItems?no=" + serialno,
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

    var idValue = $("#ID").val() ? parseInt($("#ID").val()) : null;
    var rawWarehouse = $("#Warehouse").val();

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
            var factor = parseFloat($("#ItemRate" + i).attr('element-factor')) || 1;
            var qty = parseFloat($("#ItemQty" + i).val()) || 0;

            var itemAry = {
                'ID': InTransItemId, // Nullable int (assuming InTransItemId can be null)
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
                'Factor': factor,
                'StockQty': factor * qty,
                'InLocID': (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
                    ? parseInt(rawWarehouse)
                    : null, 'TaxPerc': $("#ItemTaxPer" + i).val() ? parseFloat($("#ItemTaxPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxValue': $("#ItemTaxAmt" + i).val() ? parseFloat($("#ItemTaxAmt" + i).val()) : 0, // Nullable float (default to 0)
                'TaxTypeID': (rawTaxType && !isNaN(rawTaxType) && parseInt(rawTaxType) > 0)
                    ? parseInt(rawTaxType)
                    : null,
                'SerialNo': parseInt($(this).closest('tr').find('.serial-no').text()) || null,
                'TempQty': $("#ItemQty" + i).val() ? parseFloat($("#ItemQty" + i).val()) : 0, // Nullable float (default to 0)
                'TaxAccountID': ($("#ItemTaxPer" + i).attr('taxaccountid') && !isNaN($("#ItemTaxPer" + i).attr('taxaccountid')))
                    ? parseInt($("#ItemTaxPer" + i).attr('taxaccountid'))
                    : null,
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
        AccountID: null,
        CostCentreID: (rawProject !== undefined && rawProject !== null && rawProject.trim() !== "" && !isNaN(rawProject) && parseInt(rawProject) > 0)
            ? parseInt(rawProject)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    // Build additionals object
    var additionals = {
        TransactionID: idValue, // Nullable int
        TypeID: null, // Explicitly set as null
        ModeID: null, // Explicitly set as null
        ToLocationID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,

        Terms: $("#Terms").val() || '', // Nullable string
        InLocID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2

    };
    var entriesarray = {
        DueDate: null,
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
    console.log(JSON.stringify(model))
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
                    window.location.href = "/StockAdjustment/Index?MenuID=" + menuId;
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
    $("#StockAdjustmentList").hide();
    $("#StockAdjustmentForm").show();

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
                $("#ID").val(transaction.ID);
                $("#VoucherNo").val(transaction.TransactionNo);
                $("#VoucherCode").val(transaction.Code);
                $("#Reference").val(transaction.ReferenceNo);
                //$("#Mode").html(data['mode']);

                function formatDate(dateStr) {
                    if (!dateStr || dateStr === '' || dateStr === ' ') return '';
                    const date = new Date(dateStr);
                    return date.getFullYear() + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        ("0" + date.getDate()).slice(-2);
                }

                $("#VoucherDate").val(formatDate(transaction.Date));
                $("#Description").val(transaction.CommonNarration);
                $("#Itemtable tbody").append(data['innerHTML']);
                $("#Warehouse").html(data['warehouses']);
                $("#Project").attr('data-idvalue', transaction.CostCentreID);
                $("#Project").val(transaction.ProjectName);

                //Additionals
                $("#Terms").val(additionals.Terms);

                // -------- SUM CALCULATIONS WITH NO ROUNDING --------

                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != null && $(this).val() !== '') qtysum += Number($(this).val());
                });
                $("#qtySum").html(toFixedNoRound(qtysum, 2)).css('text-align', 'center');

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


                $(".ItemQty").css('text-align', 'center');
                $(".ItemRate, .ItemAmt, .ItemTaxAmt")
                    .css('text-align', 'right');
                $(".ItemTaxPer").css('text-align', 'center');
                updateAllSums();        //  Replaces all repeated sum blocks
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
                    window.location.href = "/StockAdjustment/Index?MenuID=" + menuId;
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

    const amt = grossAmt;

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


    // ONLY SYSTEM FIELDS ARE FORMATTED
    $("#ItemAmt" + id).val(toTwoDecimal(amt));
}

function updateAllSums() {
    updateSum('.ItemQty', '#qtySum', 'center');
    updateSum('.ItemAmt', '#amtSum');
    updateSum('.ItemTaxAmt', '#taxAmtSum');
}

// *** FIX: Removed val === "" from early return so clearing a field triggers recalculation ***
$(document).on("input", ".ItemQty, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {

    const id = $(this).attr('element-id');
    const changedField = $(this).attr("id").replace(id, '');

    let val = $(this).val();

    // Allow natural typing of decimals
    if (val === "." || val.endsWith(".")) {
        return;
    }

    // Reject only truly bad values (but allow empty string to fall through)
    if (val !== "" && (val.includes('-') || isNaN(val) || Number(val) < 0)) {
        return;
    }

    // Treat empty as 0 for calculation purposes
    if (val === "") {
        $(this).val(""); // keep it visually empty
    }

    calculateRow(id, changedField);
    updateAllSums();
});


$(document).on("blur", ".ItemQty, .ItemTaxPer, .ItemTaxAmt", function () {
    const val = parseFloat($(this).val());
    if (!isNaN(val)) {
        $(this).val(toTwoDecimal(val));
    }
});

//03-03-2026
function getColumnSum(selector) {
    let sum = 0;
    $(selector).each(function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) sum += val;
    });
    return sum;
}


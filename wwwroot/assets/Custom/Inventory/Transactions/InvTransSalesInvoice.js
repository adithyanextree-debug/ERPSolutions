var webUrl = '/SalesInvoice/';
var CommonUrl = '/CommonFunctions/';

function NewEntry() {
    $("#SalesInvoiceList").hide();
    $("#SalesInvoiceForm").show();
    $.ajax({
        url: CommonUrl + "NewEntryDetailsSales",
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            // $("#VoucherNo").val(data['nextVNo'])
            $("#VoucherNo").prop("disabled", true)
            $("#importdiv").show();
            $("#Mode").html(data.mode);
            $("#SalesArea").html(data.area);

            var date = new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2);
            $("#VoucherDate").val(date)
            $("#Account").val(data.account)

            $("#Itemtable tbody").append(data['newEntry'])
            $("#Warehouse").html(data['warehouses'])
            $("#Party").focus();
        },
    });
}

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
        $("#ItemTaxPer" + id).val('');

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

                    if (qty > 0) {
                        const tot = qty * (rate > 0 ? rate : ItemUnitPrice);

                        grossAmtField.val(parseFloat(tot).toFixed(2)).css('text-align', 'right');
                        totalField.val(parseFloat(tot).toFixed(2)).css('text-align', 'right');
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
       // url: "/SalesInvoice/NewRow?no=" + serialno,
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
                'Description': $("#Description").val() || '', // Nullable string
                'Discount': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
                'Factor': factor,
                'StockQty': factor * qty,
                'OutLocID': $("#Warehouse").val() ? parseInt($("#Warehouse").val()) : null, // Nullable int
                'DiscountPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxPerc': $("#ItemTaxPer" + i).val() ? parseFloat($("#ItemTaxPer" + i).val()) : 0, // Nullable float (default to 0)
                'TaxValue': $("#ItemTaxAmt" + i).val() ? parseFloat($("#ItemTaxAmt" + i).val()) : 0, // Nullable float (default to 0)
                'TaxTypeID': (rawTaxType && !isNaN(rawTaxType) && parseInt(rawTaxType) > 0)
                    ? parseInt(rawTaxType)
                    : null,
                'RateDiscPerc': $("#ItemDiscPer" + i).val() ? parseFloat($("#ItemDiscPer" + i).val()) : 0, // Nullable float (default to 0)
                'RateDisc': $("#ItemDiscAmt" + i).val() ? parseFloat($("#ItemDiscAmt" + i).val()) : 0, // Nullable float (default to 0)
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
        CommonNarration: $("#Description").val() || '', // Nullable string
        ReferenceNo: $("#Reference").val() || '', // Nullable string
        CostCentreID: (rawProject !== undefined && rawProject !== null && rawProject.trim() !== "" && !isNaN(rawProject) && parseInt(rawProject) > 0)
            ? parseInt(rawProject)
            : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2
    };

    var rawSalesman = $("#Salesman").attr('data-idvalue');
    var rawWarehouse = $("#Warehouse").val();
    
    // Build additionals object
    var additionals = {
        TransactionID: idValue, // Nullable int
        TypeID: null, // Explicitly set as null
        ModeID: $("#Mode").val() ? parseInt($("#Mode").val()) : null, // Nullable int
        FromLocationID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,
        OutLocID: (rawWarehouse && !isNaN(rawWarehouse) && parseInt(rawWarehouse) > 0)
            ? parseInt(rawWarehouse)
            : null,
        Name: $("#PartyNameAddress").val() || '', // Nullable string
        DocumentNo: $("#DispatchNo").val() || '', // Nullable string
        DocumentDate: $("#DispatchDate").val() ? new Date($("#DispatchDate").val()) : null, // Nullable DateTime
        EntryDate: $("#PartyInvoiceDate").val() ? new Date($("#PartyInvoiceDate").val()) : null, // Nullable DateTime
        EntryNo: $("#PartyInvoiceNo").val() || '', // Nullable string
        BankAddress: $("#Attention").val() || '', // Nullable string
        ExpiryDate: $("#ExpiryDate").val() ? new Date($("#ExpiryDate").val()) : null, // Nullable DateTime
        PassNo: $("#DeliveryNoteNo").val() || '', // Nullable string
        ReferenceDate: $("#OrderDate").val() ? new Date($("#OrderDate").val()) : null, // Nullable DateTime
        ReferenceNo: $("#OrderNo").val() || '', // Nullable string
        SubmitDate: $("#DeliveryNoteDate").val() ? new Date($("#DeliveryNoteDate").val()) : null, // Nullable DateTime
        InterestAmt: $("#StaffIncentive").val() ? parseFloat($("#StaffIncentive").val()) : null, // Nullable decimal
        InterestPerc: $("#StaffIncentivePerc").val() ? parseFloat($("#StaffIncentivePerc").val()) : null, // Nullable decimal
        Address: $("#TermsofDelivery").val() || '', // Nullable string
        Terms: $("#Terms").val() || '', // Nullable string
        AccountID: (rawSalesman !== undefined && rawSalesman !== null && rawSalesman.toString().trim() !== "" && !isNaN(rawSalesman) && parseInt(rawSalesman) > 0)
            ? parseInt(rawSalesman)
            : null,
        AreaID: $("#SalesArea").val() ? parseInt($("#SalesArea").val()) : null,
        RowState: (idValue === null || idValue === 0 || idValue === undefined) ? 1 : 2

    };
    var entriesarray = {
        DueDate:$("#DueDate").val() ? new Date($("#DueDate").val()) : null,
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
                    window.location.href = "/SalesInvoice/Index?MenuID=" + menuId;
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
    $("#SalesInvoiceList").hide();
    $("#SalesInvoiceForm").show();

    $.ajax({
        url: CommonUrl + "GetInventoryTransaction?ID=" + RowID,
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
                $("#Mode").html(data['mode']);

                function formatDate(dateStr) {
                    if (!dateStr || dateStr === '' || dateStr === ' ') return '';
                    const date = new Date(dateStr);
                    return date.getFullYear() + '-' +
                        ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                        ("0" + date.getDate()).slice(-2);
                }

                $("#VoucherDate").val(formatDate(transaction.Date));
                $("#Party").val(transaction.AccountName);
                $("#Party").attr('data-idvalue', transaction.AccountID);
                $("#Description").val(transaction.CommonNarration);
                $("#Itemtable tbody").append(data['innerHTML']);
                $("#Warehouse").html(data['warehouses']);
                $("#Project").attr('data-idvalue', transaction.CostCentreID);
                $("#Project").val(transaction.ProjectName);
                $("#Account").val(data.account);

                //Additionals

                $("#PartyInvoiceNo").val(additionals.EntryNo);
                $("#PartyInvoiceDate").val(formatDate(additionals.EntryDate));
                $("#OrderNo").val(additionals.ReferenceNo);
                $("#OrderDate").val(formatDate(additionals.ReferenceDate));
                $("#PartyNameAddress").val(additionals.Name);
                $("#ExpiryDate").val(formatDate(additionals.ExpiryDate));
                $("#Attention").val(additionals.BankAddress);
                $("#SalesArea").html(data['area']);
                $("#StaffIncentive").val(additionals.InterestAmt);
                $("#StaffIncentivePerc").val(additionals.InterestPerc);
                $("#DeliveryNoteNo").val(additionals.PassNo);
                $("#DeliveryNoteDate").val(formatDate(additionals.SubmitDate));
                $("#DispatchNo").val(additionals.DocumentNo);
                $("#DispatchDate").val(formatDate(additionals.DocumentDate));
                $("#TermsofDelivery").val(additionals.Address);
                $("#Terms").val(additionals.Terms);
                $("#Salesman").val(additionals.AccountName);
                $("#Salesman").attr('data-idvalue', additionals.AccountID);
                
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
                    window.location.href = "/SalesInvoice/Index?MenuID=" + menuId;
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
    console.log(decimals +" decimals")
    if (isNaN(num)) return "0.00";
    const factor = Math.pow(10, decimals);
    console.log(factor + " factor")
    console.log((Math.trunc(num * factor) / factor).toFixed(decimals)+"RETURN RESULT")
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

//// Unified handler — now tracks which field changed
//$(document).on("input", ".ItemQty, .ItemDiscPer, .ItemDiscAmt, .ItemTaxPer, .ItemTaxAmt", function () {
//    const id = $(this).attr('element-id');
//    const changedField = $(this).attr("id").replace(id, '');
//    // Validate input to ensure no negative or invalid values
//    let val = $(this).val();
//    // Prevent entering negative values or invalid non-numeric characters
//    if (val.includes('-') || isNaN(val) || val < 0) {
//        alert("A")
//        $(this).val('');  // Reset invalid value
//        return;  // Exit the function to prevent further actions
//    }

//    calculateRow(id, changedField);
//    updateAllSums();
//});

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

//03-03-2026
function getColumnSum(selector) {
    let sum = 0;
    $(selector).each(function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) sum += val;
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

$(document).on("input", "#Cash, #Card, #Cheque", function () {

    const cash = parseFloat($('#Cash').val()) || 0;
    const card = parseFloat($('#Card').val()) || 0;
    const cheque = parseFloat($('#Cheque').val()) || 0;

    const totalPaid = cash + card + cheque;
    $('#TotalPaid').val(toTwoDecimal(totalPaid));

    calculateBalance();
});

// ================================
// SIGNALR CONNECTION
// ================================
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/progressHub")
    .build();

connection.start().catch(err => console.error(err));

connection.on("progress", function (progressKey, percent, message) {

    // Show progress bar
    $("#progressContainer").show();

    $("#progressBar").css("width", percent + "%");
    $("#progressText").text(percent + "% - " + message);

    if (percent >= 100) {
        setTimeout(() => $("#progressContainer").fadeOut(), 1500);
    }
});

// ================================
// Excel file change event
// ================================
$(document).on("change", "input[type='file'][id^='excelFile']", function (event) {
    var input = event.target;
    if ($("#Party").val() == null || $("#Party").val() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: 'Please select the Party!',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        }).then(() => {
            $("#Party").focus();
            $(input).val("");
        });

        return false;
    }

    if (input.files && input.files[0]) {
        $("#ExcelUploadBtn").click();
    }
});

// ================================
// AJAX SUBMIT WITH PROGRESS SUPPORT
// ================================
async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);
    // Add Transaction data to formData
    formData.append("ID", $("#ID").val());
    formData.append("Date", $("#VoucherDate").val());
    formData.append("EffectiveDate", $("#VoucherDate").val());
    formData.append("VoucherID", $("#VoucherType").attr("data-value"));
    formData.append("TransactionNo", $("#VoucherNo").val());
    formData.append("SerialNo", $("#VoucherNo").val());
    formData.append("ExchangeRate", 1.00);
    formData.append("AddedDate", new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2) + ' ' + new Date().getHours() + ':' + new Date().getMinutes() + ':' + new Date().getMinutes() + '.' + new Date().getMilliseconds());
    formData.append("AccountID", $("#Party").attr('data-idvalue'));
    formData.append("CommonNarration", $("#Description").val());
    formData.append("ReferenceNo", $("#Reference").val());
    formData.append("CostCentreID", $("#Project").attr('data-idvalue'));

    // Add Additionals
    formData.append("TransactionID", $("#ID").val());
    formData.append("TypeID", null);
    formData.append("ModeID", $("#Mode").val());
    formData.append("FromLocationID", $("#Warehouse").val());
    formData.append("OutLocID", $("#Warehouse").val());
    formData.append("Name", $("#PartyNameAddress").val());
    formData.append("DocumentNo", $("#DispatchNo").val());
    formData.append("DocumentDate", $("#DispatchDate").val());
    formData.append("EntryDate", $("#PartyInvoiceDate").val());
    formData.append("EntryNo", $("#PartyInvoiceNo").val());
    formData.append("BankAddress", $("#Attention").val());
    formData.append("ExpiryDate", $("#ExpiryDate").val());
    formData.append("PassNo", $("#DeliveryNoteNo").val());
    formData.append("ReferenceDate", $("#OrderDate").val());
    formData.append("ReferenceNo", $("#OrderNo").val());
    formData.append("AreaID", $("#SalesArea").val());
    formData.append("SubmitDate", $("#DeliveryNoteDate").val());
    formData.append("InterestAmt", $("#StaffIncentive").val());
    formData.append("InterestPerc", $("#StaffIncentivePerc").val());
    formData.append("Address", $("#TermsofDelivery").val());
    formData.append("Terms", $("#Terms").val());

    // Show loader
    //$(".loader-wrapper").fadeIn("fast");

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        });

        const json = await response.json();
        if (json.success === true) {
            // Store progressKey locally
            window.currentProgressKey = json.progressKey;

            //  CASE: Success but with missing items (warning)
            if (json.message && json.message.trim() !== "") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Completed with Warnings',
                    text: json.message,
                    confirmButtonText: 'Okay',
                    confirmButtonColor: '#f0ad4e'
                }).then(() => {
                    var menuId = $("#MenuID").val();
                    window.location.href = "/SalesInvoice/Index?MenuID=" + menuId;
                });
            }
            // CASE: Pure success
            else {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: "Successfully Added...",
                    showConfirmButton: false,
                    timer: 2500,
                    timerProgressBar: true
                }).then(() => {
                    var menuId = $("#MenuID").val();
                    window.location.href = "/SalesInvoice/Index?MenuID=" + menuId;
                });
            }

        } else {
            // Failure case (only missing / no valid items)
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: json.message,
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

 //try {
    //    const response = await fetch(oFormElement.action, {
    //        method: 'POST',
    //        body: formData
    //    }).then(response => response.json());

    //    if (response['success'] === true) {
    //        Swal.fire({
    //            toast: true,
    //            position: 'top-end',
    //            icon: 'success',
    //            title: "Saved successfully!",
    //            showConfirmButton: false,
    //            timer: 3000,
    //            timerProgressBar: true
    //          }).then(() => {
    //              var menuId = $("#MenuID").val();
    //              window.location.href = "/SalesInvoice/Index?MenuID=" + menuId;
    //          });

            

    //    }
    //    else {
    //        Swal.fire({
    //            icon: 'error',
    //            title: 'Error!',
    //            text: response['message'],
    //            confirmButtonText: 'Okay',
    //            confirmButtonColor: '#d33'
    //        });
    //    }

    //}
    //catch (error) {
    //    Swal.fire({
    //        icon: 'error',
    //        title: 'Error!',
    //        text: error.message || 'An unexpected error occurred.',
    //        confirmButtonText: 'Okay',
    //        confirmButtonColor: '#d33'
    //    });
    //}

//$("#liPrint").on("click", function () {
//    alert("1")
//    PrintTable()
//})

//------excel upload------
//$("#ID").val(data.transactionNo);
//// Append the new rows to the table
//$("#Itemtable tbody").html(response['newentry']);
//// 🔁 Loop through each new row and recalculate totals
//$("#Itemtable tbody tr").each(function () {
//    const row = $(this);
//    const id = row.find('.productCode').attr('element-id'); // Adjust selector based on your HTML
//    if (id) {
//        calculateRow(id,"ItemDiscPer"); // Calculate for each new row
//        //calculateRow(id, "ItemDiscAmt"); // Calculate for each new row
//        calculateRow(id, "ItemTaxPer"); // Calculate for each new row
//        //calculateRow(id, "ItemTaxAmt"); // Calculate for each new row
//    }
//});

//updateAllSums(); // Final sum of totals
function PrintTable() {
    var id = $("#ID").val()
    $.ajax({
        url: webUrl + "PrintInvoice?ID=" + id,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#reportContainer").html(data.table);
                EntryEnable('Print');
            }
        }

    })
}

function printOnlyTable() {
    var printContent = $("#reportContainer").html();
    var printWindow = window.open('', '_blank', 'width=800,height=600');

    //printWindow.document.write('<html><head><title>Invoice</title>');
    //printWindow.document.write('<style type="text/css">');
    //// Your print styles here
    //printWindow.document.write('<style>@media print {@@page { size: landscape; margin: 20mm 10mm 20mm 10mm; min-height: 200mm; } #invoicetable { background: #aaa79e !important; min-height: 100vh; margin: 0; display: flex; justify-content: center; align-items: flex-start; padding: 20px; position: relative; overflow: hidden; } #invoicetable::before { content: ""; position: absolute; bottom: -100px; left: -100px; width: 120%; height: 300px; background: #333; border-top-left-radius: 50% 100px; border-top-right-radius: 50% 100px; z-index: 0; } }</style>');
    //printWindow.document.write('</head><body>');
    printWindow.document.write(printContent);
    //printWindow.document.write('</body></html>');

    // Ensure images are loaded before calling the print method
    var images = printWindow.document.getElementsByTagName('img');
    var loadedImagesCount = 0;

    function checkImagesLoaded() {
        for (var i = 0; i < images.length; i++) {
            if (images[i].complete) {
                loadedImagesCount++;
            }
        }

        // Once all images are loaded, trigger the print
        if (loadedImagesCount === images.length) {
            printWindow.document.close();  // Necessary for IE <= 9
            printWindow.print();
        } else {
            // Wait for a while before checking again
            setTimeout(checkImagesLoaded, 100);
        }
    }

    // Start checking if the images are loaded
    checkImagesLoaded();
}

//Changed and optimized code on 25-04-2026
//function ReindexRows() {
//    $('#Itemtable tbody tr').each(function (index) {
//        var rowIndex = index + 1; // Make it start from 1
//        $(this).find('td.serial-no').text(rowIndex);

//        $(this).find('[id], [element-id]').each(function () {
//            var el = $(this);
//            var oldId = el.attr('id');
//            // Replace any trailing number with new index
//            if (oldId) {
//                var newId = oldId.replace(/\d+$/, rowIndex);
//                el.attr('id', newId);
//            }

//            el.attr('element-id', rowIndex);
//        });

//        // Fix button-specific attributes
//        $(this).find('.addrow').attr('element-id', rowIndex).attr('serialno', rowIndex);
//        $(this).find('.action_delete').attr('element-id', rowIndex);
//    });
//}

//$(document).on("change", ".ItemUnit", function () {
//    const id = $(this).attr('element-id');
//    const selectedUnit = $(this).val();

//    //if ($(this).data('last') !== selectedUnit) {
//    //    $("#productCode" + id).addClass('changedValue');
//    //}

//    if (!selectedUnit) {
//        $("#ItemQty" + id).val('');
//        $("#ItemRate" + id).val('');
//        $("#ItemDiscPer" + id).val('');
//        $("#ItemDiscAmt" + id).val('');
//        $("#ItemAmt" + id).val('');
//        $("#ItemTotal" + id).val('');
//        $("#ItemGrossAmt" + id).html('');
//        $("#ItemTaxAmt" + id).val('');

//        updateAllSums();
//        return;
//    }

//    $.ajax({
//        url: CommonUrl + "ProductAvailableUnits",
//        method: "POST",
//        data: {
//            'ID': $("#productCode" + id).attr('data-idvalue'),
//            'AccountID': $("#Party").attr('data-idvalue'),
//            'VoucherID': $("#VoucherID").val(),
//            'Unit': selectedUnit
//        },
//        dataType: 'JSON',
//        success: function (data) {
//            if (data.success) {
//                console.log(data)
//                $("#ItemUnit" + id).html(data['units']);

//                const unitList = JSON.parse(data['unitDetails']);
//                const taxList = JSON.parse(data['taxDetails']);
//                const unit = unitList.length > 0 ? unitList[0] : {};
//                const tax = taxList.length > 0 ? taxList[0] : {};
//                console.log(unitList)
//                console.log(unit)
//                let ItemUnitPrice = parseFloat(data['itemUnitPrice']);
//                alert(ItemUnitPrice)
//                if (isNaN(ItemUnitPrice)) {
//                    alert(unit["OnlinePrice"])
//                    ItemUnitPrice = parseFloat(unit["OnlinePrice"]) || 0;
//                }

//                // Set Rate
//                $("#ItemRate" + id).val(toTwoDecimal(ItemUnitPrice));
//                $("#ItemRate" + id).attr('element-factor', unit["Factor"]);

//                // Tax Info
//                if (tax["SalesPerc"]) {
//                    $("#ItemTaxPer" + id).val(toTwoDecimal(tax["SalesPerc"]));
//                    $("#ItemTaxPer" + id).attr('taxTypeID', tax["TaxTypeID"]);
//                } else {
//                    $("#ItemTaxPer" + id).val('');
//                    $("#ItemTaxPer" + id).attr('taxTypeID', '');
//                }

//                // Default Quantity = 1
//                $("#ItemQty" + id).val('1');

//                // Recalculate fields
//                calculateRow(id);
//                updateAllSums();
//                calculateGrandTotal();
//                // Focus back to qty field
//                $("#ItemQty" + id).focus();
//            }

//        }
//    });
//});



//$(document).on("click", ".action_delete", function (event) {
//    var $Row = $(this).closest('tr');
//    var $TableBody = $Row.closest('tbody');
//    var rowCount = $TableBody.find('tr').length;

//    // Prevent deleting if only one row is left
//    if (rowCount <= 1) {
//        Swal.fire({
//            icon: 'warning',
//            title: 'Warning!',
//            text: 'You cannot delete the last remaining row.If any changes needed please update the data',
//            confirmButtonText: 'Okay',
//            confirmButtonColor: '#d33',
//        });
//        return; // Exit the click handler
//    }

//    var id = $(this).attr('element-id');
//    var itemid = $("#itemid" + id).val();

//    Swal.fire({
//        title: "Are you sure?",
//        text: "Do you wanna make to remove it?",
//        icon: "warning",
//        showCancelButton: true,
//        confirmButtonColor: '#1c7430',
//        cancelButtonColor: '#d33',
//        confirmButtonText: 'Yes, go ahead.',
//        cancelButtonText: "No, forget it."
//    }).then((result) => {
//        if (result.isConfirmed) {
//            if (itemid != null && itemid !== "") {
//                $.ajax({
//                    url: CommonUrl + "DeleteTransactionEntries?ID=" + id,
//                    method: "DELETE",
//                    dataType: 'JSON',
//                    success: function (data) {
//                        if (data['success'] === true) {
//                            $Row.remove(); // Remove the deleted row from DOM
//                            ReindexRows();

//                            var qtysum = 0;
//                            $('.ItemQty').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    qtysum += parseFloat($(this).val())
//                                }
//                            });
//                            if (!isNaN(qtysum)) {
//                                $("#qtySum").html(parseFloat(qtysum).toFixed(2))
//                                $("#qtySum").css('text-align', 'center');
//                            }
//                            var GrossAmt = 0;
//                            $('.ItemGrossAmt').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    GrossAmt += parseFloat($(this).val())
//                                }
//                            });
//                            if (!isNaN(GrossAmt)) {
//                                $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
//                                $("#ItemGrossAmtSum").css('text-align', 'right');
//                            }
//                            var dicsAmt = 0;
//                            $('.ItemDiscAmt').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    dicsAmt += parseFloat($(this).val())
//                                }
//                            });
//                            if (!isNaN(dicsAmt)) {
//                                $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
//                                $("#dicsAmtSum").css('text-align', 'right');
//                            }
//                            var ItemAmt = 0;
//                            $('.ItemAmt').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    ItemAmt += parseFloat($(this).val())
//                                }
//                            });
//                            if (!isNaN(ItemAmt)) {
//                                $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
//                                $("#amtSum").css('text-align', 'right');
//                            }
//                            var ItemTaxAmt = 0;
//                            $('.ItemTaxAmt').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    ItemTaxAmt += parseFloat($(this).val())
//                                }
//                            });

//                            if (!isNaN(ItemTaxAmt)) {
//                                $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
//                                $("#taxAmtSum").css('text-align', 'right');
//                            }
//                            var ItemTotal = 0;
//                            $('.ItemTotal').each(function () {
//                                if ($(this).val() != null && $(this).val() != '') {
//                                    ItemTotal += parseFloat($(this).val())
//                                }
//                            });

//                            if (!isNaN(ItemTotal)) {
//                                $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
//                                $("#itemTotalSum").css('text-align', 'right');
//                            }
//                            Swal.fire({
//                                toast: true,
//                                position: 'top-end',
//                                icon: 'success',
//                                title: data['message'],
//                                showConfirmButton: false,
//                                timer: 3000,
//                                timerProgressBar: true
//                            });
//                        } else {
//                            Swal.fire({
//                                icon: 'error',
//                                title: 'Oops!',
//                                text: data['message'],
//                                confirmButtonText: 'Okay',
//                                confirmButtonColor: '#d33',
//                            });
//                        }
//                    }
//                });
//            }
//            else {
//                $Row.remove(); // Remove the deleted row from DOM
//                ReindexRows();

//                var qtysum = 0;
//                $('.ItemQty').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        qtysum += parseFloat($(this).val())
//                    }
//                });
//                if (!isNaN(qtysum)) {
//                    $("#qtySum").html(parseFloat(qtysum).toFixed(2))
//                    $("#qtySum").css('text-align', 'center');
//                }
//                var GrossAmt = 0;
//                $('.ItemGrossAmt').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        GrossAmt += parseFloat($(this).val())
//                    }
//                });

//                if (!isNaN(GrossAmt)) {
//                    $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
//                    $("#ItemGrossAmtSum").css('text-align', 'right');
//                }
//                var dicsAmt = 0;
//                $('.ItemDiscAmt').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        dicsAmt += parseFloat($(this).val())
//                    }
//                });

//                if (!isNaN(dicsAmt)) {
//                    $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
//                    $("#dicsAmtSum").css('text-align', 'right');
//                }
//                var ItemAmt = 0;
//                $('.ItemAmt').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        ItemAmt += parseFloat($(this).val())
//                    }
//                });

//                if (!isNaN(ItemAmt)) {
//                    $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
//                    $("#amtSum").css('text-align', 'right');
//                }
//                var ItemTaxAmt = 0;
//                $('.ItemTaxAmt').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        ItemTaxAmt += parseFloat($(this).val())
//                    }
//                });

//                if (!isNaN(ItemTaxAmt)) {
//                    $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
//                    $("#taxAmtSum").css('text-align', 'right');
//                }
//                var ItemTotal = 0;
//                $('.ItemTotal').each(function () {
//                    if ($(this).val() != null && $(this).val() != '') {
//                        ItemTotal += parseFloat($(this).val())
//                    }
//                });

//                if (!isNaN(ItemTotal)) {
//                    $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
//                    $("#itemTotalSum").css('text-align', 'right');
//                }
//                Swal.fire({
//                    toast: true,
//                    position: 'top-end',
//                    icon: 'success',
//                    title: "Entry Deleted Successfully!",
//                    showConfirmButton: false,
//                    timer: 3000,
//                    timerProgressBar: true
//                });
//            }
//        }
//    });
//});

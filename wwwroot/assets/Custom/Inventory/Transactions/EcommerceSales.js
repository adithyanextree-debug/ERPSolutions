var webUrl = '/EcommerceSales/';
var CommonUrl = '/CommonFunctionsControllers/';

function CloseEntry() {
    location.reload();
}

function RowClick(RowID) {
    EntryEnable('Edit');
    $("#EcommerceSalesList").hide();
    $("#EcommerceSalesForm").show();


    $.ajax({
        url: webUrl + "GetInventoryTransaction?ID=" + RowID,
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                //$("#printdiv").show()
                var header = data['trans'];
                transaction = JSON.parse(header)[0];
                //var fiadditional = data['fiadditional'];
                //additionals = JSON.parse(fiadditional)[0];
                $("#vouchernodiv").show();
                $("#ID").val(RowID)
                $(".productCode").removeClass('changedValue')
                $("#VoucherNo").val(transaction.TransactionNo)
                $("#Reference").val(transaction.ReferenceNo)
                $("#Mode").html(data['mode'])
                // Helper function to format date or return empty string if invalid
                function formatDate(dateStr) {
                    if (!dateStr || dateStr === '' || dateStr === ' ') {
                        return '';  // Return empty string if date is null, empty or ' '
                    }
                    const date = new Date(dateStr);
                    return date.getFullYear() + '-' + ("0" + (date.getMonth() + 1)).slice(-2) + '-' + ("0" + date.getDate()).slice(-2);
                }
                // Apply the helper function to each date field
                $("#VoucherDate").val(formatDate(transaction.Date));
                $("#Party").val(transaction.AccountName);
                $("#Party").attr('data-idvalue', transaction.AccountID);
                $("#Description").val(transaction.Description);
                $("#Itemtable tbody").append(data['innerHTML']);
                $("#Warehouse").html(data['warehouses']);
                $("#DeliveryCharge").val(transaction.DeliveryCharge);

                $("#Account").val(data.account);
                if (data.remarks == "Order Cancelled" || data.remarks == "Order Delivered") {
                    $("#UpdateOrderStatusDiv").hide();
                }
                //$("#PartyInvoiceNo").val(additionals.EntryNo);
                //$("#PartyInvoiceDate").val(formatDate(additionals.EntryDate));
                //$("#OrderNo").val(additionals.ReferenceNo);
                //$("#OrderDate").val(formatDate(additionals.ReferenceDate));
                //$("#PartyNameAddress").val(additionals.Name);
                //$("#ExpiryDate").val(formatDate(additionals.ExpiryDate));
                //$("#Attention").val(additionals.BankAddress);
                //$("#SalesArea").val(additionals.AreaID);
                //$("#StaffIncentive").val(additionals.InterestAmt);
                //$("#StaffIncentivePerc").val(additionals.InterestPerc);
                //$("#DeliveryNoteNo").val(additionals.PassNo);
                //$("#DeliveryNoteDate").val(formatDate(additionals.SubmitDate));
                //$("#DispatchNo").val(additionals.DocumentNo);
                //$("#DispatchDate").val(formatDate(additionals.DocumentDate));
                //$("#TermsofDelivery").val(additionals.Address);
                //$("#Terms").val(additionals.Terms);


                $("#StatusTable tbody").append(data['status'])
               // $("#statusDropdown").empty()
                $("#StatusDropdown").html(data['statusdropdown'])

                var qtysum = 0;
                $('.ItemQty').each(function () {
                    if ($(this).val() != '') {
                        qtysum += parseFloat($(this).val())
                    }
                });
                if (!isNaN(qtysum)) {
                    $("#qtySum").html(parseFloat(qtysum).toFixed(2))
                    $("#qtySum").css('text-align', 'center');
                }
                var GrossAmt = 0;
                $('.ItemGrossAmt').each(function () {
                    if ($(this).val() != '') {
                        GrossAmt += parseFloat($(this).val())
                    }
                });
                if (!isNaN(GrossAmt)) {
                    $("#ItemGrossAmtSum").html(parseFloat(GrossAmt).toFixed(2))
                    $("#ItemGrossAmtSum").css('text-align', 'right');
                }
                var dicsAmt = 0;
                $('.ItemDiscAmt').each(function () {
                    if ($(this).val() != '') {
                        dicsAmt += parseFloat($(this).val())
                    }
                });
                if (!isNaN(dicsAmt)) {
                    $("#dicsAmtSum").html(parseFloat(dicsAmt).toFixed(2))
                    $("#dicsAmtSum").css('text-align', 'right');
                }
                var ItemAmt = 0;
                $('.ItemAmt').each(function () {
                    if ($(this).val() != '') {
                        ItemAmt += parseFloat($(this).val())
                    }
                });
                if (!isNaN(ItemAmt)) {
                    $("#amtSum").html(parseFloat(ItemAmt).toFixed(2))
                    $("#amtSum").css('text-align', 'right');
                }
                var ItemTaxAmt = 0;
                $('.ItemTaxAmt').each(function () {
                    if ($(this).val() != '') {
                        ItemTaxAmt += parseFloat($(this).val())
                    }
                });
                if (!isNaN(ItemTaxAmt)) {
                    $("#taxAmtSum").html(parseFloat(ItemTaxAmt).toFixed(2))
                    $("#taxAmtSum").css('text-align', 'right');
                }
                var ItemTotal = 0;
                $('.ItemTotal').each(function () {
                    if ($(this).val() != '') {
                        ItemTotal += parseFloat($(this).val())
                    }
                });
                if (!isNaN(ItemTotal)) {
                    $("#itemTotalSum").html(parseFloat(ItemTotal).toFixed(2))
                    $("#itemTotalSum").css('text-align', 'right');
                }

                $(".ItemQty").css('text-align', 'center');
                $(".ItemRate").css('text-align', 'right');
                $(".ItemGrossAmt").css('text-align', 'right');
                $(".ItemDiscPer").css('text-align', 'center');
                $(".ItemDiscAmt").css('text-align', 'right');
                $(".ItemAmt").css('text-align', 'right');
                $(".ItemTaxPer").css('text-align', 'right');
                $(".ItemTaxAmt").css('text-align', 'right');
                $(".ItemTotal").css('text-align', 'right');

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
    var flag1 = true;
    var flag2 = true;
    var flag3 = true;
    var flag4 = true;

    var items = [];
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
            var InTransItemId = $("#itemid" + i).val();

            var itemAry = {
                'TransactionID': $("#ID").val(),
                'ItemID': $("#productCode" + i).attr('data-idvalue'),
                'ID': InTransItemId,
                'Unit': $("#ItemUnit" + i).val(),
                'Qty': $("#ItemQty" + i).val(),
                'Rate': $("#ItemRate" + i).val(),
                'BasicQty': parseInt($("#ItemRate" + i).attr('element-factor')) * parseInt($("#ItemQty" + i).val()),
                'TaxPerc': $("#ItemTaxPer" + i).val(),
                'TaxValue': $("#ItemTaxAmt" + i).val(),
                'RateDiscPerc': $("#ItemDiscPer" + i).val(),
                'RateDisc': $("#ItemDiscAmt" + i).val(),
                'DiscountPerc': $("#ItemDiscPer" + i).val(),
                'TaxTypeID': $("#ItemTaxPer" + i).attr('taxTypeID'),
                'TempQty': $("#ItemQty" + i).val(),
                'Discount': $("#ItemDiscAmt" + i).val(),
                'Factor': parseInt($("#ItemRate" + i).attr('element-factor')),
                'StockQty': parseInt($("#ItemRate" + i).attr('element-factor')) * parseInt($("#ItemQty" + i).val()),
                'OutLocID': $("#Warehouse").val(),
                'Description': $("#Description").val(),
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
    var Transaction = {
        'ID': $("#ID").val(),
        'Date': $("#VoucherDate").val(),
        'EffectiveDate': $("#VoucherDate").val(),
        'VoucherID': $("#VoucherType").attr("data-value"),
        'TransactionNo': $("#VoucherNo").val(),
        'SerialNo': $("#VoucherNo").val(),
        'ExchangeRate': 1.00,
        'AddedDate': new Date().getFullYear() + '-' + ("0" + (new Date().getMonth() + 1)).slice(-2) + '-' + ("0" + new Date().getDate()).slice(-2) + ' ' + new Date().getHours() + ':' + new Date().getMinutes() + ':' + new Date().getMinutes() + '.' + new Date().getMilliseconds(),
        'AccountID': $("#Party").attr('data-idvalue'),
        'Description': $("#Description").val(),
        'DeliveryCharge': $("#DeliveryCharge").val(),
    }

    var Additionals = {
        'TransactionID': $("#ID").val(),
        'TypeID': null,
        'ModeID': $("#Mode").val(),
        'FromLocationID': $("#Warehouse").val(),
        'OutLocID': $("#Warehouse").val(),
        'Name': $("#PartyNameAddress").val(),
        'DocumentNo': $("#DispatchNo").val(),
        'DocumentDate': $("#DispatchDate").val(),
        'EntryDate': $("#PartyInvoiceDate").val(),
        'EntryNo': $("#PartyInvoiceNo").val(),
        'BankAddress': $("#Attention").val(),
        'ExpiryDate': $("#ExpiryDate").val(),
        'PassNo': $("#DeliveryNoteNo").val(),
        'ReferenceDate': $("#OrderDate").val(),
        'ReferenceNo': $("#OrderNo").val(),
        'AreaID': $("#SalesArea").val(),
        'SubmitDate': $("#DeliveryNoteDate").val(),
        'InterestAmt': $("#StaffIncentive").val(),
        'InterestPerc': $("#StaffIncentivePerc").val(),
        'Address': $("#TermsofDelivery").val(),
        //'Terms': $("#Terms").val(),
    }

    console.log(Additionals)
    if (flag4 && flag1 && flag2 && flag3 == true) {
        var model = {
            'InvTransItems': items,
            'FiTransactions': Transaction,
            'FiTransactionAdditionals': Additionals
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
    $.ajax({
        url: webUrl + "SaveTransactionEntry",
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
                    window.location.href = "/EcommerceSales/Index?MenuID=" + menuId;
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

$(document).on("click", "#UpdateOrderStatusBtn", function () {
    $("#StatusDropdownDiv").show();
    $("#UpdateBtn").show();
    $("#UpdateOrderStatusBtn").hide();
});

$(document).on("click", "#UpdateBtn", function (event) {

    var model = {
        'StatusID': $("#StatusDropdown").val(),
        'VID': $("#ID").val(),
    };

    Swal.fire({
        icon: 'warning',
        title: 'Status Update',
        text: 'Do you wanna update the status?',
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: webUrl + "UpdateOrderStatus",
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
                            title: "Status Updated Successfully!",
                            showConfirmButton: false,
                            timer: 3000,
                            timerProgressBar: true
                        }).then(() => {
                            if (data.remarks == "Order Cancelled" || data.remarks == "Order Delivered") {
                                $("#UpdateOrderStatusDiv").hide();
                            }

                            $("#StatusTable tbody").prepend(data["lastrow"]);
                            $("#StatusDropdown").html(data.status)
                        });

                        $("#ID").val(data.vid);

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
    });
});


var webUrl = '/ItemMaster/';
var CommonUrl = '/ItemMaster/';
var imagePath = '/uploads';

function NewEntry() {
    $("#ItemMasterList").hide();
    $("#ItemMasterForm").show();
    $.ajax({
        url: "/ItemMaster/NewEntryDetails",
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            $("#importdiv").hide();
            var categories = data['categories']
            var units = data['units']
            var brands = data['brands']
            var color = data['color']
            var article = data['article']
            var size = data['size']
            var ItemUnits = data['itemUnits']
            var Barcode = data['barcode']
            var Image = data['image']
            //  var Image = data['image']
            $("#PurchaseUnit").html(units);
            $("#SellingUnit").html(units);
            $("#itemCategory").html(categories);
            $("#itemBrand").html(brands);
            $("#Colour").html(color);
            $("#Article").html(article);
            $("#Size").html(size);
            //$(".select2-show-search").select2();
            $("#UnitTable tbody").append(ItemUnits)
            $("#BarcodeTable tbody").append(Barcode)
            $("#ImageTable tbody").append(Image)
            $("#ItemCode").focus();
        },
    });
}

function NewEntry1() {
    $("#SaveLi").show()
    //$("#NewLi").hide()
    $("#DeleteLi").hide()
    window.location.href = webUrl + "Create"; // navigate to a new page
   
}
function formatFields(selector) {
    $(selector).each(function () {
        let val = $(this).val();
        if (val !== '') {
            let parsed = parseFloat(val);
            if (!isNaN(parsed)) {
                $(this).val(parsed.toFixed(2));
            }
        }
        $(this).css('text-align', 'right');
    });
}

$(".itemSelling, .itemPurchase, .itemOnline, .itemPromotion").css('text-align', 'right');

// Format all fields initially
formatFields('.itemSelling');
formatFields('.itemPurchase');
formatFields('.itemOnline');
formatFields('.itemPromotion');

// Allow only numeric and decimal input
$(document).on("keypress", ".numbersOnly", function (event) {
    let charCode = (event.which) ? event.which : event.keyCode;

    // Allow: backspace, delete, tab, escape, enter and dot (.)
    if (
        charCode != 46 && // dot
        (charCode < 48 || charCode > 57)
    ) {
        event.preventDefault();
    }

    // Allow only one decimal point
    if (charCode == 46 && $(this).val().includes('.')) {
        event.preventDefault();
    }
});

// Format on blur (not keyup)
$(document).on("blur", ".itemSelling, .itemPurchase, .itemOnline, .itemPromotion", function () {
    let val = $(this).val();
    if (val !== '') {
        let parsed = parseFloat(val);
        if (!isNaN(parsed)) {
            $(this).val(parsed.toFixed(2));
        }
    }
});

// For URL slug formatting
$('#ItemName').on("keyup", function () {
    let newval = this.value.replace(/ /g, "-");
    $('#UrlName').val(newval); // Make sure UrlName has a proper ID
});

$(document).on("click", ".addunit", function (event) {
    event.preventDefault();

    var $row = $(this).closest('tr');
    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');
    var unitValue = $("#itemUnit" + currentId).val();
    var factor = $("#itemFactor" + currentId).val();
    var sellingPrice = $("#itemSelling" + currentId).val();

    if (!unitValue || factor.trim() === "" || sellingPrice.trim() === "") {
        Swal.fire({
            title: "Missing Information",
            text: "Please select a Unit, and enter both Factor and Selling Price before adding a new row.",
            icon: "warning"
        });
        return;
    }

    //var lastRow = $("#UnitTable tbody tr").last().attr("id");
    //var lastIndex = 1;

    //if (lastRow && lastRow.startsWith("Row")) {
    //    lastIndex = parseInt(lastRow.replace("Row", ""), 10);
    //    if (isNaN(lastIndex)) lastIndex = 1;
    //}

    //var newIndex = lastIndex + 1;

    $.ajax({
        url: "/ItemMaster/NewRow?no=" + serialno + "&Text=Unit",
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            var ItemUnits = data['itemUnits'];
            $("#UnitTable tbody").append(ItemUnits);
        },
    });
});

$(document).on("click", ".action_deleteunit", function (event) {
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
    var itemunitid = $("#itemunitid" + id).val();

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
            if (itemunitid != null && itemunitid !== "") {
                $.ajax({
                    url: CommonUrl + "DeleteUnits?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove(); // Remove the deleted row from DOM
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
            }
        }
    });
});

$(document).on("click", ".addbarcode", function (event) {
    event.preventDefault();

    var $row = $(this).closest('tr');
    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');

    var barcodeValue = $("#unitbarcode" + currentId).val();

    if (barcodeValue == null || barcodeValue == "") {
        Swal.fire({
            title: "Missing Information",
            text: "Please enter the barcode before adding a new row.",
            icon: "warning"
        });
        return;
    }

    //var lastRow = $("#BarcodeTable tbody tr").last().attr("id");
    //var lastIndex = 1;

    //if (lastRow && lastRow.startsWith("Row")) {
    //    lastIndex = parseInt(lastRow.replace("Row", ""), 10);
    //    if (isNaN(lastIndex)) lastIndex = 1;
    //}

    //var newIndex = lastIndex + 1;

    $.ajax({
        url: "/ItemMaster/NewRow?no=" + serialno +"&Text=Barcode",
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            var ItemBarcodes = data['itemBarcodes'];
            $("#BarcodeTable tbody").append(ItemBarcodes);
        },
    });
});

$(document).on("click", ".action_deletebarcode", function (event) {
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
    var itemunitid = $("#itembarcodeid" + id).val();

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
            if (itemunitid != null && itemunitid !== "") {
                $.ajax({
                    url: CommonUrl + "DeleteBarcodes?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove(); // Remove the deleted row from DOM
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
            }
        }
    });
});

$(document).on("click", ".addimage", function (event) {
    event.preventDefault();

    var $row = $(this).closest('tr');
    var currentId = $(this).attr('element-id');
    var serialno = $(this).attr('serialno');

    var productimageTitle = $("#productimageTitle" + currentId).val();


    if (productimageTitle == null || productimageTitle == "") {
        Swal.fire({
            title: "Missing Information",
            text: "Please select the image before adding a new row.",
            icon: "warning"
        });
        return;
    }

    //var lastRow = $("#ImageTable tbody tr").last().attr("element-id");
    //var lastIndex = 1;

    //if (lastRow && lastRow.startsWith("Row")) {
    //    lastIndex = parseInt(lastRow.replace("Row", ""), 10);
    //    if (isNaN(lastIndex)) lastIndex = 1;
    //}

   /* var newIndex = lastIndex + 1;*/

    $.ajax({
        url: "/ItemMaster/NewRow?no=" + serialno + "&Text=Image",
        method: "GET",
        dataType: 'JSON',
        success: function (data) {
            var image = data['image'];
            $("#ImageTable tbody").append(image);
        },
    });
});

$(document).on("click", ".action_deleteimage", function (event) {
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
    var itemunitid = $("#itemimageid" + id).val();

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
            if (itemunitid != null && itemunitid !== "") {
                $.ajax({
                    url: CommonUrl + "DeleteImageDetails?ID=" + id,
                    method: "DELETE",
                    dataType: 'JSON',
                    success: function (data) {
                        if (data['success'] === true) {
                            $Row.remove(); // Remove the deleted row from DOM
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
            }
        }
    });
});

$(document).on("click", ".productISDefault", function (event) {
    var id = $(this).attr('element-id')
    var checked = $("#productISDefault" + id).is(":checked");
    if (checked == true) {
        $(".productISDefault").prop('checked', false)
        $("#productISDefault" + id).prop('checked', true)
       
    }
  
});

$(document).on("change", "input[type='file'][id^='FileUpload_FormFile']", function (event) {
    var input = event.target;
    var elementId = $(input).attr("id").replace("FileUpload_FormFile", ""); // Get the Sn/id

    // Validate item code
    if ($("#ItemCode").val() == null || $("#ItemCode").val() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: 'Please add Item Code!',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        }).then(() => {
            // Focus the input field
            $("#ItemCode").focus();
        });
        // clear invalid selection
        input.value = "";
        return false;
    }

    if (input.files && input.files[0]) {
        var file = input.files[0];
        var fileSizeKB = file.size / 1024; // size in KB

        //  BLOCK files > 50 KB
        //if (fileSizeKB > 50) {
        //    Swal.fire({
        //        icon: 'error',
        //        title: 'File Too Large!',
        //        text: 'Please upload an image of size 50 KB or less.',
        //        confirmButtonText: 'Okay',
        //        confirmButtonColor: '#d33',
        //    });

        //    // Reset file input
        //    input.value = "";

        //    // Remove preview if any
        //    //$("#productimagepreview" + elementId).attr("src", "");
        //    //$("#productimagepreview" + elementId).removeClass('avatar avatar-lg');

        //    return false;
        //}

        //  File is allowed — preview it
        var src = URL.createObjectURL(file);

        $("#productimagepreview" + elementId).attr("src", src);
        $("#productimagepreview" + elementId).addClass('avatar avatar-lg');

        // Auto-submit
        $("#ProductImageUploadBtn" + elementId).click();
    }
});

async function AJAXSubmit(oFormElement) {
    const formData = new FormData(oFormElement);
    formData.append("ItemCode", $("#ItemCode").val());
    const currentId = $(oFormElement).find("input[name='ImageItemId']").val();

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        }).then(response => response.json());

        if (response['success'] === true) {
            $("#productimagepreview" + currentId).attr("path", response["imagepath"]);
            $("#productimageTitle" + currentId).val(response["imagename"]);
            $("#productimageSize" + currentId).val(response["imagesize"]);
           
            //$(".CurrentRow").each(function () {
            //    $(this).attr('imageID', response['id']);
            //});

            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: response['message'],
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });

        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: response['message'],
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

$(document).on("click", ".productimageSetPrimary", function (event) {
    var id = $(this).attr('element-id')
    var imageID = $("#productimageSetPrimary" + id).attr('imageid')
    if ($(this).hasClass("btn-success")) {
        return false;
    }
    Swal.fire({
        title: "Are you sure?",
        text: "Do you wanna make it default?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#d33',          // red confirm button
        cancelButtonColor: 'rgba(200, 200, 200, 0.3)', // light transparent grey
        confirmButtonText: 'Yes, go ahead.',
        cancelButtonText: "No, forget it."
    }).then((result) => {
        if (result.isConfirmed) {
            if (imageID == "false") {
                $(".productimageSetPrimary").attr('imageid', false)
                $("#productimageSetPrimary" + id).attr('imageid', true)
                $(".productimageSetPrimary").removeClass("btn-success");
                $(".productimageSetPrimary").addClass("btn-danger");
                $("#productimageSetPrimary" + id).removeClass("btn-danger");
                $("#productimageSetPrimary" + id).addClass("btn-success");
            }
            else {
                return false;
            }
           
        }
    }); 
});

function SaveEntry() {
    $("#SaveLi").hide();
    if ($("#ItemCode").val() == '') {
        $("#ItemCode").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Item Code is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }
    if ($("#ItemName").val() == '') {
        $("#ItemName").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Item Name is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false
    }
    if (!$("#ProductActive").is(":checked")) {
        var active = false;
    } else {
        var active = true;
    }
    if (!$("#StockItem").is(":checked")) {
        var StockItem = false;
    } else {
        var StockItem = true;
    }
    if (!$("#IsExpiry").is(":checked")) {
        var IsExpiry = false;
    } else {
        var IsExpiry = true;
    }
    if (!$("#SellOnEcommerce").is(":checked")) {
        var SellOnEcommerce = false;
    } else {
        var SellOnEcommerce = true;
    }
    var flag1 = true;
    var flag2 = true;
    var flag3 = true;
    var flag4 = true;
    var flag5 = true;
    var flag6 = true;
    var hasValidUnitRow = false;
    var hasValidBarcodeRow = false;
    var hasValidImageRow = false;
    var items = [];
    $('.itemUnit').each(function () {
        //if ($(this).hasClass('changedValue')) {
        var i = $(this).attr('element-id');
        if ($(this).val()) {
            if ($("#itemUnit" + i).val() == '') {
                flag1 = false;
                return false;
            }
            if ($("#itemFactor" + i).val() == '') {
                flag2 = false;
                return false;
            }
            if ($("#itemSelling" + i).val() == '') {
                flag3 = false;
                return false;
            }
            var InTransItemId = $("#itemunitid" + i).val();

            if ($("#productISDefault" + i).is(":checked")) {
                var SDefault = true
            } else {
                var SDefault = false
            }
            if ($("#productunitActive" + i).is(":checked")) {
                var value = true
            } else {
                var value = false
            }
            var itemAry = {
                'ID': InTransItemId,
                'ItemID': $("#PurchasetransactionID").val(),
                'Unit': $("#itemUnit" + i).val(),
                'Factor': $("#itemFactor" + i).val(),
                'Barcode': $("#barcode" + i).val(),
                'PurchaseRate': $("#itemPurchase" + i).val(),
                'SellingPrice': $("#itemSelling" + i).val(),
                'OnlinePrice': $("#itemOnline" + i).val(),
                'PromotionPrice': $("#itemPromotion" + i).val(),
                'BasicUnit': "No",
                'Active': value,
                'IsDefault': SDefault,
            }
            if ($("#itemSelling" + i).val() != '' && $("#itemFactor" + i).val() != '' && $("#itemUnit" + i).val() != '') {
                items.push(itemAry);
                hasValidUnitRow = true;

            }
        }
        else {
            if (hasValidUnitRow == false) {
                flag1 = false;
            }
        }
        //}
    });
    var barcodes = [];
    $('.unitbarcode').each(function () {
        //if ($(this).hasClass('changedValue')) {
        var i = $(this).attr('element-id');
        if ($(this).val()) {
            if ($("#unitbarcode" + i).val() == '') {
                flag5 = false;
                return false;
            }
            var InTransItemId = $("#itembarcodeid" + i).val();
           
            if ($("#productbarcodeActive" + i).is(":checked")) {
                var value = true
            }
            else {
                var value = false
            }
            var barcodeitems = {
                'ID': InTransItemId,
                'ItemID': $("#PurchaseTransactionID").val(),
                'UnitID': null,
                'Barcode': $("#unitbarcode" + i).val(),
                'Active': value
            }
            if ($("#unitbarcode" + i).val() != '') {
                barcodes.push(barcodeitems);
                hasValidBarcodeRow = true;
            }
        }
        else {
            if (hasValidBarcodeRow == false) {
                flag5 = false;
            }
        }
    });
    var images = [];
    $('.productimageTitle').each(function () {
        var i = $(this).attr('element-id');

        if ($(this).val()) {
            var InTransItemId = $("#itemimageid" + i).val();
           
            if ($("#productimageTitle" + i).val() == '') {
                flag6 = false;
                return false;
            }
            var Primary = false;
            var attr = $(this).attr('data-value')
            if ($("#productimageSetPrimary" + i).attr('imageid') === "true") {
                Primary = true;
            }
            var img = {
                'ID': InTransItemId,
                'ItemID': $("#PurchaseTransactionID").val(),
                'Active': $("#productimageActive" + i).is(":checked"),
                'ImageSize': $("#productimageSize" + i).val(),
                'Title': $("#productimageTitle" + i).val(),
                'ArabicTitle': $("#productimageArabicTitle" + i).val(),
                'ImagePath': $("#productimagepreview" + i).attr("path"),
                'IsDefault': Primary,
                'OrderNo':i
            }
            if ($("#productimageTitle" + i).val() != '') {
                images.push(img);
                hasValidImageRow = true;
            }
        }
        else {
            if (hasValidImageRow == false) {
                flag6 = false;
            }
        }
    });
    var Master = {
        'ID': $("#PurchaseTransactionID").val(),
        'ItemCode': $("#ItemCode").val(),
        'ItemName': $("#ItemName").val(),
        'ShortDescription': $("#ShortDescription").val(),
        'ShortDescriptionArabic': $("#ShortDescriptionArabic").val(),
        'PartNo': $("#PartNo").val(),
        'OEMNo': $("#OemNo").val(),
        'Unit': "No",//null
        'ArabicName': $("#ArabicName").val(),
        'Active': active,
        'CategoryID': $("#itemCategory").val(),
        'BrandID': $("#itemBrand").val(),
        'ColorID': $("#Colour").val(),
        'ArticleID': $("#Article").val(),
        'SizeID': $("#Size").val(),
        'ModelNo': $("#ModelNo").val(),
        'Manufacturer': $("#ItemManufacturer").val(),
        'PurchaseUnit': $("#PurchaseUnit").val(),
        'SellingUnit': $("#SellingUnit").val(),
        'StockItem': StockItem,
        'IsExpiry': IsExpiry,
        'Weight': $("#ItemWeight").val(),
        'ExpiryPeriod': $("#ExpiryDate").val(),
        'Remarks': $("#remarks").val(),
        //'LongDescription': window.editor7.getText().trim(),
        //'ArabicLongDescription': window.editor8.getText().trim(),
        'LongDescription': window.editor7.root.innerHTML.trim(),
        'ArabicLongDescription': window.editor8.root.innerHTML.trim(),
        //'LongDescription': "",
        //'ArabicLongDescription': "",
        'SellOnEcommerce': SellOnEcommerce,
        'UrlName': $("#UrlName").val(),

    }
   
    if (flag1 && flag2 && flag3 && flag5 && flag6) {
        var model = {
            'InvItemMaster': Master,
            "InvItemUnits": items,
            'InvItemBarcodes': barcodes, //added on 27-07-2023 fro barcode table insertion
            'InvItemImages': images //added on 15-05-2025 fro image table insertion
        }
        console.log(model)
    }
    else {
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
                text: "Item factor is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
        if (flag3 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Item selling price is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
        }
        //if (flag4 == false) {
        //    Swal.fire({
        //        icon: 'error',
        //        title: 'Oops!',
        //        text: "Item Image is mandatory",
        //        confirmButtonText: 'Okay',
        //        confirmButtonColor: '#d33',
        //    });
        //    return false;
        //}
        if (flag5 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Barcode is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;

        }
        if (flag6 == false) {
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Product Image is mandatory",
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33',
            });
            return false;
            //$(".AlertDangerStatus").html('Error!')
            //$(".AlertDangerMessage").html("Product Image Title is mandatory")
            //$(".AlertDivDanger").show();
            //setTimeout(function () {
            //    $(".AlertDivDanger").hide();
            //}, 3000);
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            //return false;

        }

    }
    $.ajax({
        url: webUrl + "SaveProductEntry",
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
                    
                    if (hasActiveSearch()) {
                        // User came via search → keep search results
                        CloseEntry();
                    } else {
                        // Fresh add → reload to show new item
                        var menuId = $("#MenuID").val();
                        window.location.href = "/ItemMaster/Index?MenuID=" + menuId;
                    }

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
function RowClick1(RowID) {
    $("#SaveLi").show()
    $("#DeleteLi").show()
    //$("#NewLi").hide()
    window.location.href = webUrl + "Create?ID=" + RowID; // navigate to edit page
}
function RowClick(RowID) {
    EntryEnable('Edit')
    $("#ItemMasterList").hide();
    $("#ItemMasterForm").show();
    $("#PurchaseTransactionID").val(RowID)
    $.ajax({
        url: webUrl + "ItemMasterDetails?ID=" + RowID,
        method: "POST",
        dataType: 'JSON',
        success: function (data) {
            if (data['success'] == true) {
                $("#importdiv").hide();
                $("#UnitTable tbody").html('');
                $("#ImageTable tbody").html('');
                $("#BarcodeTable tbody").html('');
                var header = data['header'];
                transaction = JSON.parse(header)[0];
                $("#ItemCode").val(transaction.ItemCode)
                $("#ItemName").val(transaction.ItemName)               
                $("#PartNo").val(transaction.PartNo)
                $("#OemNo").val(transaction.OemNo)
                $("#ArabicName").val(transaction.ArabicName)
                $("#ModelNo").val(transaction.ModelNo)
                $("#ItemManufacturer").val(transaction.Manufacturer)
                $("#ItemWeight").val(transaction.Weight)
                $("#ExpiryDate").val(transaction.ExpiryPeriod)
                $("#remarks").val(transaction.Remarks)
                $("#UrlName").val(transaction.UrlName)
                finallink = data.link + 'Items/Items?ID=' + RowID;
                //console.log(finallink);
                //$('.row.link_url span').html('<a href= "' + finallink + '"  class="urlproduct"  target="_blank">URL NAME</a>');

                $("#ShortDescription").val(transaction.ShortDescription)
                $("#ShortDescriptionArabic").val(transaction.ShortDescriptionArabic)

                // Set content into Quill editors
                window.editor7.root.innerHTML = transaction.LongDescription || "";
                window.editor8.root.innerHTML = transaction.ArabicLongDescription || "";

                $("#UnitTable tbody").html(data['itemunits']);
                $("#ImageTable tbody").html(data['itemimages']);
                $("#BarcodeTable tbody").html(data['barcode']);
               
                $("#ProductActive").prop('checked', transaction.Active)
                $("#StockItem").prop('checked', transaction.StockItem)
                $("#IsExpiry").prop('checked', transaction.IsExpiry)
                $("#SellOnEcommerce").prop('checked', transaction.SellOnEcommerce)

                $("#PurchaseUnit").html(data['purchaseunits']);
                $("#SellingUnit").html(data['salesunit']);
                $("#itemCategory").html(data['categories']);
                $("#itemBrand").html(data['brands']);
                $("#Colour").html(data['color']);
                $("#Article").html(data['article']);
                $("#Size").html(data['size']);
                $("#ItemCode").focus();

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
var finallink = "";
$('#UrlName').click(function () {
    //var valueurl = this.value;
    //var link = "https://go2camps.com/";
    //var finallink = link + valueurl;
    var row = $('.row.link_url span');
    var td = row.find('td');
    var id1 = td.eq(0).text();
    var id2 = td.eq(1).text();
    $('.row.link_url span').html('<a href= "' + finallink + '"  class="urlproduct"  target="_blank">URL NAME</a>');
});
function DeleteEntry() {
    if ($("#PurchaseTransactionID").val() == null) {
        return false
    }
    $.ajax({
        url: CommonUrl + "Delete?id=" + $("#PurchaseTransactionID").val(),
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
                    window.location.href = "/ItemMaster/Index?MenuID=" + menuId;
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
    // Check if the ItemMasterList is visible
    if ($("#ItemMasterList").is(":visible")) {
        // If ItemMasterList is visible, go to the home page (Index)
        window.location.href = "/Home/Index";  // Update this URL as per your needs
    } else {
        // Otherwise, reset the form fields and clear content
        $("#ItemMasterList").show();
        $("#ItemMasterForm").hide();
        $("#importdiv").show();

        $("#ItemCode").val('');
        $("#ItemName").val('');
        $("#ArabicName").val('');
        $("#PartNo").val('');
        $("#OemNo").val('');
        $("#ModelNo").val('');
        $("#ItemManufacturer").val('');
        $("#ItemWeight").val('');
        $("#ExpiryDate").val('');
        $("#remarks").val('');
        $("#UrlName").val('');
        $("#ShortDescription").val('');
        $("#ShortDescriptionArabic").val('');

        // Reset Quill Editors
        if (window.editor7 && window.editor7.root) {
            window.editor7.root.innerHTML = '';
        }
        if (window.editor8 && window.editor8.root) {
            window.editor8.root.innerHTML = '';
        }

        // Clear HTML content for tables
        $("#UnitTable tbody").html('');
        $("#ImageTable tbody").html('');
        $("#BarcodeTable tbody").html('');

        // Uncheck checkboxes
        $("#ProductActive").prop('checked', true);
        $("#StockItem").prop('checked', true);
        $("#IsExpiry").prop('checked', false);
        $("#SellOnEcommerce").prop('checked', true);

        // Reset select dropdowns
        $("#PurchaseUnit").html('');
        $("#SellingUnit").html('');
        $("#itemCategory").html('');
        $("#itemBrand").html('');
        $("#Colour").html('');
        $("#Article").html('');
        $("#Size").html('');

        // Reset UI elements
        $("#liNew").show();
        $("#liSave").hide();
        $("#liDelete").hide();
        $("#refreshIcon").hide();
    }
}
//Excel sheet upload
$(document).on("change", "input[type='file'][id^='excelFile']", function (event) {
    var input = event.target;
    // ✅ submit form
    if (input.files && input.files[0]) {
        $("#ExcelUploadBtn").click();
    }
});

async function AJAXSubmit2(oFormElement) {
    const formData = new FormData(oFormElement);

    // Show loader before sending request
    $(".loader-wrapper").fadeIn("fast");

    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        }).then(response => response.json());

        // Hide loader once response is received
        $(".loader-wrapper").fadeOut("slow", function () {
            $(this).hide();
        });

        if (response['success'] === true) {
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
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: response['message'],
                confirmButtonText: 'Okay',
                confirmButtonColor: '#d33'
            });
        }

    } catch (error) {
        // Hide loader on error too
        $(".loader-wrapper").fadeOut("slow", function () {
            $(this).hide();
        });

        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: error.message || 'An unexpected error occurred.',
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33'
        });
    }
}

function Reset() {
    //$("input[name='Category']").val("");
    //$("input[name='item']").val("");
    //$("input[name='brand']").val("");
    //$("input[name='modelno']").val("");
    //$("input[name='barcode']").val("");
    window.location.href = "/ItemMaster/Index"
}
function hasActiveSearch() {

    function hasValue(selector) {
        return String($(selector).val() || '').trim().length > 0;
    }

    return (
        hasValue('input[name="item"]') ||
        hasValue('input[name="brand"]') ||
        hasValue('input[name="modelno"]') ||
        hasValue('input[name="barcode"]') ||
        hasValue('input[name="category"]')
    );
}

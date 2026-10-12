var webUrl = '/PurchaseRegister/';
var CommonUrl = '/PurchaseRegister/';

$(document).ready(function () {

    // Go button
    $('#btnGo').on('click', function () {
        loadReport();
    });

    // Clear button
    $('#btnClear').on('click', function () {
        $('#Party, #Item, #Staff, #Area, #VoucherType, #Counter').val('');
        $('#Party, #Item, #Staff, #Area, #Counter').attr('data-idvalue',"");
        $('#PaymentType').val('');
        $('#IsColumnar, #IsDetailed, #IsInventory, #IsGroupItem').prop('checked', false);
        $('#rbInventory').prop('checked', true);
        $('#gridContainer').html('<p class="text-muted text-center py-4">Select filters and click Go.</p>');

        // Destroy DataTable if exists
        if ($.fn.DataTable.isDataTable('#purchaseTable')) {
            $('#purchaseTable').DataTable().destroy();
        }
    });

    function loadReport() {
        var viewBy = $('input[name="ViewBy"]:checked').val();  // "Inventory" or "Finance"
        var cri = viewBy === "Finance";  // true if Finance, false if Inventory

        var filter = {
            FromDate: $('#FromDate').val() || null,
            ToDate: $('#ToDate').val() || null,
            VTypeID: parseInt($('#VoucherTypeID').val()) || null,
            AccountID: parseInt($('#Party').attr('data-idvalue')) || null,
            ItemID: parseInt($('#Item').attr('data-idvalue')) || null,
            CounterID: parseInt($('#Counter').attr('data-idvalue')) || null,
            PaymentTypeID: parseInt($('#PaymentType').val()) || null,
            IsColumnar: $('#IsColumnar').is(':checked'),
            IsDetailed: $('#IsDetailed').is(':checked'),
            IsInventory: $('#IsInventory').is(':checked'),
            IsGroupItem: $('#IsGroupItem').is(':checked'),
            Criteria: cri || null,

            //AccountID: $('#Staff').attr('data-idvalue'),
            //AreaID: $('#Area').attr('data-idvalue'),
        };

        // Show loader
        $('#gridContainer').html(
            '<div class="text-center py-5">' +
            '<div class="spinner-border text-primary" role="status"></div>' +
            '<p class="mt-2 text-muted">Loading...</p></div>'
        );

        $.ajax({
            url: '/PurchaseRegister/GetData',
            type: 'POST',
            data: filter,
            //headers: {
            //    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            //},
            success: function (html) {
                $('#gridContainer').html(html);

                //  Auto hide no-records alert after 5 seconds
                setTimeout(function () {
                    $('#gridContainer').find('.alert-warning').fadeOut('slow');
                }, 5000);

                // Init DataTable after HTML is injected
                if ($('#purchaseTable').length) {
                    $('#purchaseTable').DataTable({
                        paging: true,
                        pageLength: 25,
                        // ❌ scrollX:true, removed
                        //order: [[2, 'asc']],
                        //columnDefs: [
                        //    { targets: [4, 5], className: 'dt-right' }
                        //],
                        language: {
                            emptyTable: 'No records found'
                        }
                    });
                }
            },
            error: function (xhr) {
                $('#gridContainer').html(
                    '<div class="alert alert-danger">Error loading data. Please try again.</div>'
                );
                console.error('Purchase Register Error:', xhr.responseText);

                //  Auto hide error alert after 5 seconds
                setTimeout(function () {
                    $('#gridContainer').find('.alert-danger').fadeOut('slow');
                }, 5000);
            }
        });
    }
}); 

//function printOnlyTable() {
//    var printContent = document.getElementById("gridContainer").innerHTML;
//    var originalContent = document.body.innerHTML;

//    document.body.innerHTML = printContent;
//    window.print();
//    document.body.innerHTML = originalContent;
//    location.reload();
//    //loadiframe("PurchaseRegister/Index", menuid);

//}
//function printOnlyTable() {
//    var table = document.getElementById("purchaseTable");

//    if (!table) {
//        alert('Please load the report before printing.');
//        return;
//    }

//    var printWindow = window.open('', '_blank');
//    printWindow.document.write(`
//        <html>
//        <head>
//            <title>Purchase Register</title>
//            <style>
//                @page {
//                    size: A4 landscape;
//                    margin: 10mm;
//                }
//                * {
//                    box-sizing: border-box;
//                }
//                body {
//                    margin: 0;
//                    padding: 0;
//                }
//                table {
//                    border-collapse: collapse;
//                    width: 100%;
//                    table-layout: auto;
//                    font-size: 10px;
//                }
//                th, td {
//                    border: 1px solid #333;
//                    padding: 3px 5px;
//                    text-align: left;
//                    white-space: nowrap;
//                }
//                th {
//                    background-color: #f0f0f0;
//                }
//                .text-end {
//                    text-align: right;
//                }
//                .text-center {
//                    text-align: center;
//                }
//            </style>
//        </head>
//        <body>
//            ${table.outerHTML}
//        </body>
//        </html>
//    `);
//    printWindow.document.close();
//    printWindow.focus();
//    printWindow.print();
//    printWindow.close();
//}

function printOnlyTable() {
    if (!$.fn.DataTable.isDataTable('#purchaseTable')) {
        alert('Please load the report before printing.');
        return;
    }

    var dtable = $('#purchaseTable').DataTable();
    var originalPageLen = dtable.page.len();

    // Temporarily show all rows so print captures the full dataset
    dtable.page.len(-1).draw();

    var table = document.getElementById("purchaseTable");
    var printWindow = window.open('', '_blank');
    printWindow.document.write(`
        <html>
        <head>
            <title>Purchase Register</title>
            <style>
                @page {
                    size: A4 landscape;
                    margin: 10mm;
                }
                * {
                    box-sizing: border-box;
                }
                body {
                    margin: 0;
                    padding: 0;
                }
                table {
                    border-collapse: collapse;
                    width: 100%;
                    table-layout: auto;
                    font-size: 10px;
                }
                th, td {
                    border: 1px solid #333;
                    padding: 3px 5px;
                    text-align: left;
                    white-space: nowrap;
                }
                th {
                    background-color: #f0f0f0;
                }
                .text-end {
                    text-align: right;
                }
                .text-center {
                    text-align: center;
                }
            </style>
        </head>
        <body>
            ${table.outerHTML}
        </body>
        </html>
    `);
    printWindow.document.close();

    // Restore original pagination on the live table before/after print
    dtable.page.len(originalPageLen).draw();

    printWindow.focus();
    printWindow.print();
    printWindow.close();
}

function exportToPDF() {
    const element = document.getElementById('purchaseTable');
    const opt = {
        margin: 0.5,
        filename: 'purchaseregister.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' }
    };

    const clone = element.cloneNode(true);
    const wrapper = document.createElement('div');
    wrapper.appendChild(clone);

    html2pdf().from(wrapper).set(opt).save();
}

function exportToExcel() {
    const table = document.querySelector("#purchaseTable");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'purchaseregister.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#purchaseTable");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the purchase register below:\n\n" + html);
    window.location.href = `mailto:?subject=PurchaseRegister&body=${body}`;
}

const menuid = 200;

function CloseEntry() {
    location.reload()
}
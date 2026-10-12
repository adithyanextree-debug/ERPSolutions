var stockTable = null;
var currentFilter = {};

$('#btnGo').on('click', function () {
    loadReport();
});

// Clear button
$('#btnClear').on('click', function () {
    $('#Date, #Item, #Unit, #Size, #Barcode, #ItemOrigin, #Brand, #ItemOrigin, #Category, #Color, #BatchNo, #Supplier, #Customer').val('');
    $('#Item, #Unit, #Size, #Barcode, #ItemOrigin, #Brand, #ItemOrigin, #Category, #Color, #Supplier, #Customer').attr('data-idvalue', "");
    $('#Warehouse').val('');
    $('#ItemWise').prop('checked', false);
    $('#gridContainer').html('<p class="text-muted text-center py-4">Select filters and click Go.</p>');

    // Destroy DataTable if exists
    if (stockTable !== null) {
        stockTable.destroy();
        stockTable = null;
    }
});

function loadReport() {

    var isItemwise = $('#ItemWise').is(':checked');

    currentFilter = {
        Date: $('#Date').val() || null,
        BatchNo: $('#BatchNo').val() || null,
        LocationID: $('#Warehouse').val() || null ,
        ItemID: parseInt($('#Item').attr('data-idvalue')) || null,
        Barcode: $('#Barcode').attr('data-idvalue') && $('#Barcode').attr('data-idvalue') !== "0"
            ? $('#Barcode').attr('data-idvalue') : null,
        OriginID: parseInt($('#ItemOrigin').attr('data-idvalue')) || null,
        ColorID: parseInt($('#Color').attr('data-idvalue')) || null,
        BrandID: parseInt($('#Brand').attr('data-idvalue')) || null,
        SupplierID: parseInt($('#Supplier').attr('data-idvalue')) || null,
        CustomerID: parseInt($('#Customer').attr('data-idvalue')) || null,
        IsItemwise: isItemwise,
    };

    //   Build columns BEFORE DataTable init (no need for pre-ajax call)
    var columns = [
        {
            data: null,
            render: function (data, type, row, meta) {
                return meta.row + meta.settings._iDisplayStart + 1;
            },
            orderable: false
        },
        { data: 'ItemCode' },
        { data: 'ItemName' }
    ];

    if (!isItemwise) {
        columns.push({ data: 'VNo' });
        columns.push({ data: 'VDate' });
        columns.push({ data: 'VType' });
    }

    columns.push(
        { data: 'Location' },
        { data: 'Unit' },
        { data: 'Size' },
        { data: 'QtyInUnit' },
        { data: 'QtyIn' },
        { data: 'RateIn', className: 'dt-right' },
        { data: 'AmountIn', className: 'dt-right' },
        { data: 'QtyOutUnit' },
        { data: 'QtyOut' },
        { data: 'RateOut', className: 'dt-right' },
        { data: 'AmountOut', className: 'dt-right' },
        { data: 'QtyUnit' },
        { data: 'Qty' },
        { data: 'AvgCost', className: 'dt-right' },
        { data: 'StockValue', className: 'dt-right' },
        { data: 'FOCQty' },
        { data: 'ReplaceQty' }
    );

    //   Build table header based on isItemwise (read from filter, not response)
    var headerHtml = `
        <th>#</th>
        <th>Item Code</th>
        <th>Item Name</th>
        ${!isItemwise ? '<th>V No.</th><th>V Date</th><th>V Type</th>' : ''}
        <th>Location</th>
        <th>Unit</th>
        <th>Size</th>
        <th>QtyInUnit</th>
        <th>QtyIn</th>
        <th>RateIn</th>
        <th>AmountIn</th>
        <th>QtyOutUnit</th>
        <th>QtyOut</th>
        <th>RateOut</th>
        <th>AmountOut</th>
        <th>QtyUnit</th>
        <th>Qty</th>
        <th>AvgCost</th>
        <th>StockValue</th>
        <th>FOCQty</th>
        <th>ReplaceQty</th>
    `;

    //   Inject table shell into gridContainer
    $('#gridContainer').html(`
        <div class="table-responsive">
            <table id="stockitemregisterTable"
                   class="table table-bordered table-sm table-hover">
                <thead><tr>${headerHtml}</tr></thead>
                <tbody></tbody>
            </table>
        </div>
    `);

    //   Destroy old DataTable instance if exists
    if (stockTable !== null) {
        stockTable.destroy();
        stockTable = null;
    }

    //   ONE call only — no manual $.ajax before this
    stockTable = $('#stockitemregisterTable').DataTable({
        processing: true,
        serverSide: true,
        pageLength: 25,
        //order: [[2, 'asc']],
        columns: columns,
        ajax: {
            url: '/StockItemRegister/GetData',
            type: 'POST',
            data: function (d) {
                // DataTables auto-sends draw/start/length — just merge your filter
                return $.extend({}, d, currentFilter);
            },
            error: function (xhr) {
                $('#gridContainer').html(
                    '<div class="alert alert-danger">Error loading data. Please try again.</div>'
                );
                console.error('Error:', xhr.responseText);
            }
        },
        language: {
            processing: '<div class="spinner-border spinner-border-sm text-primary"></div> Loading...',
            emptyTable: 'No records found'
        }
    });
}

function printOnlyTable() {
    if (!$.fn.DataTable.isDataTable('#stockitemregisterTable')) {
        alert('Please load the report before printing.');
        return;
    }

    var dtable = $('#stockitemregisterTable').DataTable();
    var originalPageLen = dtable.page.len();

    // Temporarily show all rows so print captures the full dataset
    dtable.page.len(-1).draw();

    var table = document.getElementById("stockitemregisterTable");
    var columnCount = table.querySelectorAll('thead th').length;
    var colWidth = (100 / columnCount).toFixed(2);

    var colgroupHtml = '<colgroup>' +
        Array(columnCount).fill('<col style="width:' + colWidth + '%">').join('') +
        '</colgroup>';

    var printWindow = window.open('', '_blank');
    printWindow.document.write(`
    <html>
    <head>
        <title>Stock Item Register</title>
        <style>
            @page {
                size: A4 landscape;
                margin: 5mm;
            }
            * {
                box-sizing: border-box;
            }
            html, body {
                width: 100%;
                margin: 0;
                padding: 0;
            }
            table {
                border-collapse: collapse;
                width: 100%;
                table-layout: fixed;
                font-size: 8px;
            }
            th, td {
                border: 1px solid #333;
                padding: 2px 3px;
                text-align: left;
                word-wrap: break-word;
                overflow-wrap: break-word;
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
        <table>
            ${colgroupHtml}
            ${table.querySelector('thead').outerHTML}
            ${table.querySelector('tbody').outerHTML}
        </table>
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
    const element = document.getElementById('stockitemregisterTable');
    const opt = {
        margin: 0.5,
        filename: 'stockitemregister.pdf',
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
    const table = document.querySelector("#stockitemregisterTable");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'stockitemregister.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#stockitemregisterTable");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the stock item register below:\n\n" + html);
    window.location.href = `mailto:?subject=StockItemRegister&body=${body}`;
}

const menuid = 157;

function CloseEntry() {
    location.reload()
}

//$(document).ready(function () {

//    // Go button
//    $('#btnGo').on('click', function () {
//        loadReport();
//    });

    //// Clear button
    //$('#btnClear').on('click', function () {
    //    $('#Date, #Item, #Unit, #Size, #Barcode, #ItemOrigin, #Brand, #ItemOrigin, #Category, #Color,#BatchNo, #Supplier,#Customer').val('');
    //    $('#Item, #Unit, #Size, #Barcode, #ItemOrigin,#Brand, #ItemOrigin, #Category, #Color, #Supplier,#Customer').attr('data-idvalue', "");
    //    $('#Warehouse').val('');
    //    $('#IItemWise').prop('checked', false);
    //    $('#gridContainer').html('<p class="text-muted text-center py-4">Select filters and click Go.</p>');

    //    // Destroy DataTable if exists
    //    if ($.fn.DataTable.isDataTable('#stockitemregisterTable')) {
    //        $('#stockitemregisterTable').DataTable().destroy();
    //    }
    //});

//    function loadReport() {

//        var filter = {
//            Date: $('#Date').val() || null,
//            BatchNo: $('#BatchNo').val() || null,
//            LocationID: parseInt($('#Warehouse').attr('data-idvalue')) || null,
//            ItemID: parseInt($('#Item').attr('data-idvalue')) || null,
//            Barcode: $('#Barcode').attr('data-idvalue') && $('#Barcode').attr('data-idvalue') !== "0" ? $('#Barcode').attr('data-idvalue') : null,
//            OriginID: parseInt($('#ItemOrigin').attr('data-idvalue')) || null,
//            ColorID: parseInt($('#Color').attr('data-idvalue')) || null,
//            BrandID: parseInt($('#Brand').attr('data-idvalue')) || null,
//            SupplierID: parseInt($('#Supplier').attr('data-idvalue')) || null,
//            CustomerID: parseInt($('#Customer').attr('data-idvalue')) || null,
//            IsItemwise: $('#ItemWise').is(':checked'),
//        };

//        // Show loader
//        $('#gridContainer').html(
//            '<div class="text-center py-5">' +
//            '<div class="spinner-border text-primary" role="status"></div>' +
//            '<p class="mt-2 text-muted">Loading...</p></div>'
//        );

//        $.ajax({
//            url: '/StockItemRegister/GetData',
//            type: 'POST',
//            timeout: 300000, // 5 minutes in ms
//            data: filter,
//            //headers: {
//            //    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
//            //},
//            success: function (html) {
//                alert(html)
//                $('#gridContainer').html(html);

//                //  Auto hide no-records alert after 5 seconds
//                setTimeout(function () {
//                    $('#gridContainer').find('.alert-warning').fadeOut('slow');
//                }, 5000);

//                // Init DataTable after HTML is injected
//                if ($('#stockitemregisterTable').length) {
//                    alert("A")
//                    $('#stockitemregisterTable').DataTable({
//                        paging: true,
//                        pageLength: 50,
//                        // ❌ scrollX:true, removed
//                        order: [[2, 'asc']],
//                        columnDefs: [
//                            { targets: [4, 5], className: 'dt-right' }
//                        ],
//                        language: {
//                            emptyTable: 'No records found'
//                        }
//                    });
//                }
//            },
//            error: function (xhr) {
//                $('#gridContainer').html(
//                    '<div class="alert alert-danger">Error loading data. Please try again.</div>'
//                );
//                console.error('Stock Item Register Error:', xhr.responseText);

//                //  Auto hide error alert after 5 seconds
//                setTimeout(function () {
//                    $('#gridContainer').find('.alert-danger').fadeOut('slow');
//                }, 5000);
//            }
//        });
//    }
//}); 
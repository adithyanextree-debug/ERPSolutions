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
        LocationID: parseInt($('#Warehouse').attr('data-idvalue')) || null,
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
        pageLength: 10,
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
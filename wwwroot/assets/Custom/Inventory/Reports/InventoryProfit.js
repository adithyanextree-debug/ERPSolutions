$(document).ready(function () {

    function applyViewByRules() {
        var viewBy = $('#ViewBy').val(); // change selector to match your actual dropdown id/name

        if (viewBy === 'Item') {
            // Disable Party and Detailed, enable Item
            $('#Party').prop('disabled', true).val('');
            $('#Detailed').prop('disabled', true).prop('checked', false);
            //$('#ViewBy').prop('disabled', false);

        } else if (viewBy === 'Voucher') {
            // Disable Item, enable Party and Detailed
            $('#Item').prop('disabled', true).val('');
            $('#Party').prop('disabled', false);
            $('#Detailed').prop('disabled', false);

        } else if (viewBy === 'Party') {
            // Enable all
            $('#Item').prop('disabled', false);
            $('#Party').prop('disabled', false);
            $('#Detailed').prop('disabled', false);
        }
    }

    // Run on page load to set initial state
    applyViewByRules();

    // Run on change
    $('#ViewBy').on('change', applyViewByRules);

    // Go button
    $('#btnGo').on('click', function () {
        loadReport();
    });

    // Clear button
    $('#btnClear').on('click', function () {
        $('#Party, #Item').val('');
        $('#Party, #Item').attr('data-idvalue', "");
        $('#Detailed').prop('checked', false);
        $('#ViewBy').prop('selectedIndex', 0);
        $('#gridContainer').html('<p class="text-muted text-center py-4">Select filters and click Go.</p>');
        // Destroy DataTable if exists
        if ($.fn.DataTable.isDataTable('#inventoryprofitTable')) {
            $('#inventoryprofitTable').DataTable().destroy();
        }
    });

    function loadReport() {
        var viewBy = $("#ViewBy").val(); 

        var filter = {
            FromDate: $('#FromDate').val() || null,
            ToDate: $('#ToDate').val() || null,
            AccountID: parseInt($('#Party').attr('data-idvalue')) || null,
            ItemID: parseInt($('#Item').attr('data-idvalue')) || null,
            IsDetailed: $('#Detailed').is(':checked'),
            Criteria: viewBy,
        };

        // Show loader
        $('#gridContainer').html(
            '<div class="text-center py-5">' +
            '<div class="spinner-border text-primary" role="status"></div>' +
            '<p class="mt-2 text-muted">Loading...</p></div>'
        );

        $.ajax({
            url: '/InventoryProfit/GetData',
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
                if ($('#inventoryprofitTable').length) {
                    $('#inventoryprofitTable').DataTable({
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
                console.error('Inventory Profit Error:', xhr.responseText);

                //  Auto hide error alert after 5 seconds
                setTimeout(function () {
                    $('#gridContainer').find('.alert-danger').fadeOut('slow');
                }, 5000);
            }
        });
    }
}); 

function printOnlyTable() {
    if (!$.fn.DataTable.isDataTable('#inventoryprofitTable')) {
        alert('Please load the report before printing.');
        return;
    }

    var dtable = $('#inventoryprofitTable').DataTable();
    var originalPageLen = dtable.page.len();

    // Temporarily show all rows so print captures the full dataset
    dtable.page.len(-1).draw();

    var table = document.getElementById("inventoryprofitTable");
    var printWindow = window.open('', '_blank');
    printWindow.document.write(`
        <html>
        <head>
            <title>Inventory Profit</title>
            <style>
                @page {
                    size: A4 landscape;
                    margin: 2.5mm;
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
    const element = document.getElementById('inventoryprofitTable');
    const opt = {
        margin: 0.5,
        filename: 'inventoryprofit.pdf',
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
    const table = document.querySelector("#inventoryprofitTable");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'inventoryprofit.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#inventoryprofitTable");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the inventory profit below:\n\n" + html);
    window.location.href = `mailto:?subject=InventoryProfit&body=${body}`;
}

const menuid = 278;

function CloseEntry() {
    location.reload()
}
$(document).ready(function () {
    // Go button
    $('#btnGo').on('click', function () {
        loadReport();
    });

    // Clear button
    $('#btnClear').on('click', function () {
        $('#gridContainer').html('<p class="text-muted text-center py-4">Select filters and click Go.</p>');

        // Destroy DataTable if exists
        if ($.fn.DataTable.isDataTable('#monthlysalessummaryTable')) {
            $('#monthlysalessummaryTable').DataTable().destroy();
        }
    });

    function loadReport() {

        var filter = {
            FromDate: $('#FromDate').val() || null,
            ToDate: $('#ToDate').val() || null,
        };

        // Show loader
        $('#gridContainer').html(
            '<div class="text-center py-5">' +
            '<div class="spinner-border text-primary" role="status"></div>' +
            '<p class="mt-2 text-muted">Loading...</p></div>'
        );

        $.ajax({
            url: '/MonthlySalesSummary/GetData',
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
                if ($('#monthlysalessummaryTable').length) {
                    $('#monthlysalessummaryTable').DataTable({
                        paging: true,
                        pageLength: 25,
                        //columnDefs: [
                        //    { targets: [2], className: 'dt-right' }  // only Amount (index 2) needs right-align
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
                console.error('Monthly Sales Summary Error:', xhr.responseText);

                //  Auto hide error alert after 5 seconds
                setTimeout(function () {
                    $('#gridContainer').find('.alert-danger').fadeOut('slow');
                }, 5000);
            }
        });
    }
}); 

function printOnlyTable() {
    if (!$.fn.DataTable.isDataTable('#monthlysalessummaryTable')) {
        alert('Please load the report before printing.');
        return;
    }

    var dtable = $('#monthlysalessummaryTable').DataTable();
    var originalPageLen = dtable.page.len();

    // Temporarily show all rows so print captures the full dataset
    dtable.page.len(-1).draw();

    var table = document.getElementById("monthlysalessummaryTable");
    var printWindow = window.open('', '_blank');
    printWindow.document.write(`
        <html>
        <head>
            <title>Monthly Sales Summary</title>
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
    const element = document.getElementById('monthlysalessummaryTable');
    const opt = {
        margin: 0.5,
        filename: 'monthlysalessummary.pdf',
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
    const table = document.querySelector("#monthlysalessummaryTable");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'monthlysalessummary.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#monthlysalessummaryTable");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the monthly sales summary below:\n\n" + html);
    window.location.href = `mailto:?subject=PurchaseRegister&body=${body}`;
}

const menuid = 200;

function CloseEntry() {
    location.reload()
}
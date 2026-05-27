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
                        pageLength: 50,
                        // ❌ scrollX:true, removed
                        //order: [[2, 'asc']],
                        columnDefs: [
                            { targets: [4, 5], className: 'dt-right' }
                        ],
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
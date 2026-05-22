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
                        pageLength: 50,
                        // ❌ scrollX:true, removed
                        order: [[2, 'asc']],
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
                console.error('Purchase Register Error:', xhr.responseText);

                //  Auto hide error alert after 5 seconds
                setTimeout(function () {
                    $('#gridContainer').find('.alert-danger').fadeOut('slow');
                }, 5000);
            }
        });
    }
}); 
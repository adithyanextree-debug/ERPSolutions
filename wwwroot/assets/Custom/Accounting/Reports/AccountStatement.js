var webUrl = '/AccountStatement/';
var CommonUrl = '/AccountStatement/';

$("#accountStatementForm").submit(function (event) {
    event.preventDefault(); // Prevent normal form submission

    // Validation
    if ($("#AccountName").val() == null || $("#AccountName").val() == '') {
        $("#AccountName").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Accounts is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return;
    }

    if ($("#StartDate").val() == null || $("#StartDate").val() == '') {
        $("#StartDate").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Start Date is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return;
    }

    if ($("#EndDate").val() == null || $("#EndDate").val() == '') {
        $("#EndDate").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "End Date is mandatory",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return;
    }

    // Prepare data
    var formData = new FormData(this);

    $.ajax({
        url: $(this).attr("action"),
        method: "POST",
        data: formData,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $(".loader-wrapper").fadeIn("fast"); // will work if it's not removed
        },
        success: function (response) {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide(); // keep it for reuse
            });
            // Inject the response (HTML) into the container
            $("#reportContainer").html(response);
        },
        error: function () {
            $(".loader-wrapper").fadeOut("slow", function () {
                $(this).hide();
            });
            Swal.fire({
                icon: 'error',
                title: 'Oops!',
                text: "Something went wrong while loading the report.",
            });
        }
    });
});


function printOnlyTable() {
    var printContent = document.getElementById("accountTable").innerHTML;
    var originalContent = document.body.innerHTML;

    document.body.innerHTML = printContent;
    window.print();
    document.body.innerHTML = originalContent;
    location.reload();
    //loadiframe("AccountStatement/Index", menuid);

}
function exportToPDF() {
    const element = document.getElementById('accountTable');
    const opt = {
        margin: 0.5,
        filename: 'account-statement.pdf',
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
    const table = document.querySelector("#accountTable table");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'account-statement.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#accountTable table");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the account statement below:\n\n" + html);
    window.location.href = `mailto:?subject=Account Statement&body=${body}`;
}

const menuid = 77;

function CloseEntry() {
    location.reload()
}
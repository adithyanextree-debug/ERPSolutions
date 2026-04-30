var webUrl = '/BillwiseStatement/';
var CommonUrl = '/BillwiseStatement/';
//--DropDown [21/09/23]
$(document).ready(function () {
    //added select box
    var op = "<option value='0'>All</option><option value='1'>Receivables</option><option value='2'>Payables</option>";
    $("#BillType").html(op);
    //setting StartDate and EndDate --[21/09/23]
    document.getElementById('EndDate').valueAsDate = new Date();
    var today = new Date();
    var preYear = today.getFullYear() - 1;
    today.setFullYear(preYear);
    document.getElementById('StartDate').valueAsDate = today;
});
//showReport-table[21/09/23]
$("#BillwiseStatementForm").submit(function (event) {
    event.preventDefault(); // Prevent normal form submission

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

    //added  area on salesregister [06/10/2023]
    if ($("#Accounts").attr('data-idvalue') == "0" && $("#Accounts").val() != "" || $("#Accounts").attr('data-idvalue') == "" && $("#Accounts").val() != "") {
        $("#Accounts").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Please provide a valid Account!",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }
    //Added validation ON AccGroup
    if ($("#AccGroup").attr('data-idvalue') == "0" && $("#AccGroup").val() != "" || $("#AccGroup").attr('data-idvalue') == "" && $("#AccGroup").val() != "") {
        $("#AccGroup").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Please provide a valid Account Group!",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }
    //Added validation ON AccCategory
    if ($("#AccCategory").attr('data-idvalue') == "0" && $("#AccCategory").val() != "" || $("#AccCategory").attr('data-idvalue') == "" && $("#AccCategory").val() != "") {
        $("#AccCategory").focus();
        Swal.fire({
            icon: 'error',
            title: 'Oops!',
            text: "Please provide a valid Account Category!",
            confirmButtonText: 'Okay',
            confirmButtonColor: '#d33',
        });
        return false;
    }
    var pending = $('#Pending').is(":checked");
    var detailed = $('#Detailed').is(":checked");

    var pay = false;
    var rec = false;
    if ($("#BillType").val() == 1) {
        rec = true;
    }
    else if ($("#BillType").val() == 2) {
        pay = true;
    }

    // Prepare data
    var formData = new FormData(this);
    formData.set("Receivables", rec.toString());
    formData.set("Payables", pay.toString());
    formData.set("Detailed", detailed.toString());
    formData.set("Pending", pending.toString());

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

//CloseEntry--[21/09/23]
function CloseEntry() {
    location.reload()
}

$(document).on("keyup", "#DueDaysFrom", function () {
    this.value = this.value.replace(/[^0-9\.]/g, '');
});
//upto [21/09/23]
$(document).on("keyup", "#DueDaysUpto", function () {
    this.value = this.value.replace(/[^0-9\.]/g, '');
});
function printOnlyTable() {
    var printContent = document.getElementById("billwisestatementtable").innerHTML;
    var originalContent = document.body.innerHTML;

    document.body.innerHTML = printContent;
    window.print();
    document.body.innerHTML = originalContent;
    location.reload();
}
function exportToPDF() {
    const element = document.getElementById('billwisestatementtable');
    const opt = {
        margin: 0.5,
        filename: 'billwise-statement.pdf',
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
    const table = document.querySelector("#billwisestatementtable table");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'billwise-statement.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#billwisestatementtable table");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the billwise statement below:\n\n" + html);
    window.location.href = `mailto:?subject=Billwise Statement&body=${body}`;
}
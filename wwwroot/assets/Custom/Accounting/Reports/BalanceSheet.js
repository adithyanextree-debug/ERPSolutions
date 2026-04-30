const menuid = 82;

function CloseEntry() {
    location.reload()
}
//For getting dropdown of Viewby on 19-09-2023
$(document).ready(function () {
    var op = "<option value='0'>Default</option><option value='1'>TwoSided</option>";
    $("#Viewby").html(op);
    //setting StartDate and EndDate --[21/09/23]
    document.getElementById('EndDate').valueAsDate = new Date();
    var today = new Date();
    var preYear = today.getFullYear() - 1
    today.setFullYear(preYear);
    document.getElementById('StartDate').valueAsDate = today;

});

//function RowDoubleClick(AcoountID) {
//    var startdate = $("#StartDate").val();
//    var enddate = $("#EndDate").val();
//    location.href = '@Url.Action("ShowReport", "AccountStatement")?accountid=' + AcoountID + '&startdate=' + startdate + '&enddate=' + enddate;
//}

//For form submit
document.getElementById("Form").addEventListener("submit", function (event) {
    // Prevent the form from submitting initially
    event.preventDefault();
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
    var printContent = document.getElementById("balancesheetTable").innerHTML;
    var originalContent = document.body.innerHTML;

    document.body.innerHTML = printContent;
    window.print();
    document.body.innerHTML = originalContent;
    location.reload();
    //loadiframe("BalanceSheet/Index", menuid);

}
function exportToPDF() {
    const element = document.getElementById('balancesheetTable');
    const opt = {
        margin: 0.5,
        filename: 'balancesheet-statement.pdf',
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
    const table = document.querySelector("#balancesheetTable table");
    const tableHTML = table.outerHTML.replace(/ /g, '%20');

    const a = document.createElement('a');
    a.href = 'data:application/vnd.ms-excel,' + tableHTML;
    a.download = 'balancesheet-statement.xls';
    a.click();
}

function emailTableData() {
    const table = document.querySelector("#balancesheetTable table");
    const html = table.outerHTML;

    const body = encodeURIComponent("Please find the balancesheet statement below:\n\n" + html);
    window.location.href = `mailto:?subject=Balancesheet Statement&body=${body}`;
}
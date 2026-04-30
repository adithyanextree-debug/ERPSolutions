document.addEventListener("DOMContentLoaded", function () {

    //  Initialize DataTable ONLY if table exists
    const tableEl = document.querySelector("#project-status");

    if (tableEl) {
        new simpleDatatables.DataTable(tableEl, {
            perPage: 10,
            search: true,
            sort: true,
        });
    }

    //  filter option
    const listItems1 = document.querySelectorAll(".light-box");

    listItems1.forEach(function (item) {
        const envelope_1 = item.querySelector(".filter-icon");
        const envelope_2 = item.querySelector(".filter-close");

        item.addEventListener("click", function () {

            if (envelope_1 && envelope_2) {
                envelope_1.classList.toggle("show");
                envelope_1.classList.toggle("hide");

                envelope_2.classList.toggle("show");
                envelope_2.classList.toggle("hide");
            }

        });
    });

});


//const datatable = new simpleDatatables.DataTable("#project-status", {
//  perpage: 10,
//  // tabIndex: 1,
//  search: true,
//  sort: true,
//  // footer: true,
//});

//// filter option
//const listItems1 = document.querySelectorAll(".light-box");

//listItems1.forEach(function (item) {
//  const envelope_1 = item.querySelector(".filter-icon");
//  const envelope_2 = item.querySelector(".filter-close");

//  item.addEventListener("click", function () {
//    if (envelope_1) {
//      envelope_1.classList.toggle("show");
//      envelope_2.classList.toggle("hide");
//    }
//    if (envelope_2) {
//      envelope_1.classList.toggle("hide");
//      envelope_2.classList.toggle("show");
//    }
//  });
//});

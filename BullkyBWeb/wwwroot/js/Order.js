var datatable

$(document).ready(function () {
   
    datatable = $('#tblDate').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "10%" },
            { "data": "phonrNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                 <div class="d-flex ">
                    <a class="btn btn-primary me-2 col-3 col-6" href="Order/Details?Id=${data}"> <i class="bi bi-pencil-square"></i></a>
                    </div>
                `
                },
                "width": "10%"
            }
        ]
    });
});

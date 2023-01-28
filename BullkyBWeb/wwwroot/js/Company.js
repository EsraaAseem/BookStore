var datatable
$(document).ready(function () {
    datatable = $('#tblDate').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            {"data":"name","width":"20%"},
            { "data": "city", "width": "20%" },
            { "data": "streetAdress", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "state", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                 <div class="d-flex ">
                    <a class="btn btn-primary me-2 col-3 col-5" href="Company/Upsert?id=${data}"> <i class="bi bi-pencil-square"></i></a>
                        <a class="btn btn-danger col-3 col-5" onClick="Delete('Company/DeletePost/${data}')"><i class="bi bi-trash3"></i></a>

                    </div>
                `
                }

                    ,"width":"20%"
            }

        ]
    })
})
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        datatable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                    data.ajax.reload();
                }
            })
        }
    })
}
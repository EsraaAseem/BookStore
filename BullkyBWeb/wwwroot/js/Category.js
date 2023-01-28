var datatable

$(document).ready(function () {
    datatable = $('#tblDate').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "disorder", "width": "15%" },
            {
            "data": "id",
            "render": function (data) {
                return `
                 <div class="d-flex ">
                    <a class="btn btn-primary me-2 col-3 col-6" href="Category/Upsert?id=${data}"> <i class="bi bi-pencil-square"></i></a>
                        <a class="btn btn-danger col-3 col-6" onClick="Delete('Category/DeletePost/${data}')"><i class="bi bi-trash3"></i></a>

                    </div>
                `
            },
            "width": "25%"
            }
        ]
    });
});
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
                        toastr.success(data.message)
                    }
                    else {
                        toastr.error(data.message)
                    }
                    datatable.ajax.reload();
                }
            })
        }
    })
}
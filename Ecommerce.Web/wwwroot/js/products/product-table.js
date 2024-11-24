$(document).ready(function () {
    loadData();

    function loadData() {
        dtble = $("#productTable").DataTable({
            ajax: {
                url: "/Admin/Products/GetProducts",
                type: "GET",
                dataSrc: "data"
            },
            columns: [
                { data: "name", className: "text-center" },
                { data: "category", className: "text-center" },
                { data: "price", className: "text-center" },
                {
                    data: "image",
                    className: "text-center",
                    render: function (data) {
                        const imageUrl = data ? `/Images/Products/${data}` : '/Images/default-product.png';
                        return `<img src="${imageUrl}" alt="Product Image" style="width: 50px; height: 50px; object-fit: cover; border-radius: 5px;" />`;
                    }
                },
                {
                    data: "id",
                    className: "text-center",
                    render: function (data) {
                        return `
                            <div class="d-flex justify-content-center gap-2">
                                <a class="btn btn-sm btn-info" href="/Admin/Products/Details/${data}">
                                    <i class="bi-eye"></i> Details
                                </a>
                                <a class="btn btn-sm btn-primary" href="/Admin/Products/Edit/${data}">
                                    <i class="bi bi-pencil"></i> Edit
                                </a>
                                <button href="javascript:;" class="btn btn-sm btn-danger deleteBtn" data-id="${data}">
                                    <i class="bi bi-trash"></i> Delete
                                </button>
                            </div>`;
                    }
                }
            ],
            createdRow: function (row) {
                $(row).addClass("align-middle"); // Vertically align all data
            }
        });
    }
});

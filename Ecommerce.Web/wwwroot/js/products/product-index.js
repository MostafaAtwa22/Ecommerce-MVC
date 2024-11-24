$(document).ready(function () {
    $('#productTable').on('click', '.deleteBtn', function () {
        const productId = $(this).data('id');
        const row = $(this).closest('tr');

        // SweetAlert Delete Confirmation
        Swal.fire({
            title: 'Are you sure you want to delete this Product?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true,
            customClass: {
                confirmButton: 'btn btn-danger mx-2',
                cancelButton: 'btn btn-secondary'
            },
            buttonsStyling: false // Disable SweetAlert2 default styles
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Admin/Products/Delete/${productId}`,
                    type: 'DELETE',
                    success: function (data) {
                        if (data.success) {
                            dtble.ajax.reload();
                            toastr.error(data.message);
                        }
                    },
                    error: function () {
                        Swal.fire(
                            'Oops...',
                            'Something went wrong. Please try again.',
                            'error'
                        );
                    }
                });
            }
        });
    });
});

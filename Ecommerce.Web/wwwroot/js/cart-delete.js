$(document).ready(function () {
    $('.js-delete').on('click', function (event) {
        var btn = $(this);

        const swal = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-danger mx-2',
                cancelButton: 'btn btn-light'
            },
            buttonsStyling: false
        });

        swal.fire({
            title: 'Are you sure you want to delete this Item?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Customer/Cart/Delete/${btn.data('id')}`,
                    method: 'DELETE',
                    success: function (response) {
                        if (response.success) {
                            swal.fire(
                                'Deleted!',
                                response.message,
                                'success'
                            );
                            btn.closest('.product').fadeOut(); // Assuming '.product' is your container class
                        } else {
                            swal.fire(
                                'Error!',
                                response.message,
                                'error'
                            );
                        }
                    },
                    error: function () {
                        swal.fire(
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

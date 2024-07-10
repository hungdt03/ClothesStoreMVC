$('#modalConfirm').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var orderId = button.data('id');
    var aspController = button.data('controller')
    var aspAction = button.data('action')
    var modalHeader = button.data('title')
    var modalBody = button.data('description')
    $('#modalConfirmLabel').text(modalHeader);
    $('#modalBody').text(modalBody);
    var modal = $(this);

    $('#btn-modal-confirm-accept').attr("href", `/${aspController}/${aspAction}/${orderId}`);
});

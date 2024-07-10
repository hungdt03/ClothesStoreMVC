console.log('BLALALALALALAL')

$('#modalConfirmAdmin').on('show.bs.modal', function (event) {
    console.log("Đây nè trong modal")
    var button = $(event.relatedTarget);
    var orderId = button.data('id');
    var aspController = button.data('controller')
    var aspAction = button.data('action')
    var modalHeader = button.data('title')
    var modalBody = button.data('description')
    $('#exampleModalLabel').text(modalHeader);
    $('#modalBody').text(modalBody);
    var modal = $(this);

    $('#btn-modal-confirm-accept').attr("href", `/Admin/${aspController}/${aspAction}/${orderId}`);
});

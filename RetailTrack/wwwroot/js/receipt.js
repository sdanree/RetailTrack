$(document).ready(function () {
    $('#addMaterialModal').on('show.bs.modal', function () {
        $.get('@Url.Action("CreatePartial", "Receipt")', function (data) {
            $('#materialFormContainer').html(data);
        });
    });

    // Enviar formulario del modal
    $(document).on('submit', '#addMaterialForm', function (e) {
        e.preventDefault();

        $.ajax({
            url: '@Url.Action("AddMaterial", "Receipt")',
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    location.reload(); // Recargar la vista principal
                } else {
                    // Volver a cargar la vista parcial con errores
                    $('#materialFormContainer').html(response);
                }
            }
        });
    });
});
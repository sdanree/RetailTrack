// Función para cargar la vista parcial de alta de materiales en el modal
$(document).ready(function () {
    $('#addMaterialModal').on('show.bs.modal', function () {
        $.ajax({
            url: '/Material/CreatePartial', // URL relativa
            type: 'GET',
            success: function (data) {
                $('#materialFormContainer').html(data);
            },
            error: function (xhr, status, error) {
                console.error("Error al cargar la vista parcial:", error);
                $('#materialFormContainer').html('<p class="text-danger">Error al cargar la vista de Nuevo Material.</p>');
            }
        });
    });
});

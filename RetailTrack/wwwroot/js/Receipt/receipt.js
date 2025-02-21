$(document).ready(function () {
    console.log("receipt.js cargado correctamente");

    // Evento para cargar el modal
    $('#addMaterialModal').on('show.bs.modal', function () {
        var url = appSettings.createPartialUrl; // URL definida en appSettings
        console.log("Cargando URL:", url);

        $.get(url, function (data) {
            $('#materialFormContainer').html(data);
        }).fail(function () {
            alert("Error al cargar el formulario de material.");
        });
    });

    // Manejar el envío del formulario
    $(document).on('submit', '#addMaterialForm', function (e) {
        e.preventDefault();
    
        $.ajax({
            url: $(this).attr('action'), 
            type: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    // Cerrar el modal
                    $('#addMaterialModal').modal('hide');

                    // Agregar el nuevo material a la lista desplegable
                    $('#Materials').append(new Option(response.materialName, response.materialId));
                    
                    // Seleccionar el material recién creado
                    $('#Materials').val(response.materialId);
            
                    // Seleccionar automáticamente el tipo de material
                    $('#MaterialTypeId').val(response.materialTypeId);
                } else {
                    // Mostrar mensaje de error
                    alert(response.message);
                }
            },
            error: function () {
                alert("Ocurrió un error inesperado.");
            }
        });
    });
});
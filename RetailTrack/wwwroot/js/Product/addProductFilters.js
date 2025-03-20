document.addEventListener("DOMContentLoaded", function () {
    console.log("Cargando script de filtros de productos.");

    const materialTypeSelect = document.getElementById("MaterialTypeId");
    const materialSelect = document.getElementById("MaterialId");
    const sizeSelect = document.getElementById("SizeId");

    if (!materialTypeSelect || !materialSelect || !sizeSelect) {
        console.error("Error: No se encontraron elementos de selección.");
        return;
    }

    // Evento para actualizar materiales al cambiar el tipo de material
    materialTypeSelect.addEventListener("change", function () {
        resetFilters();
        const materialTypeId = this.value.trim();
        if (!materialTypeId) return;

        fetch(`/Material/GetMaterialsByType?materialTypeId=${materialTypeId}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(material => {
                    materialSelect.innerHTML += `<option value="${material.Id}">${material.Name}</option>`;
                });
            })
            .catch(error => console.error("Error al obtener materiales:", error));
    });

    // Evento para actualizar tamaños al cambiar el material
    materialSelect.addEventListener("change", function () {
        sizeSelect.innerHTML = "<option value=''>Seleccione un tamaño</option>";
        const materialId = this.value.trim();
        if (!materialId) return;

        fetch(`/Product/GetMaterialSizesByMaterial?materialId=${materialId}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(size => {
                    sizeSelect.innerHTML += `<option value="${size.sizeId}">${size.size_Name}</option>`;
                });
            })
            .catch(error => console.error("Error al obtener tamaños:", error));
    });

    function resetFilters() {
        materialSelect.innerHTML = "<option value=''>Seleccione un material</option>";
        sizeSelect.innerHTML = "<option value=''>Seleccione un tamaño</option>";
    }
});

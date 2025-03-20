document.addEventListener("DOMContentLoaded", function () {
    console.log("Script de selección de materiales y proveedores cargado correctamente.");

    // Elementos del formulario de materiales
    const materialTypeSelect = document.getElementById("MaterialTypeId");
    const materialSelect = document.getElementById("MaterialId");
    const sizeSelect = document.getElementById("SizeId");
    const providerSelect = document.getElementById("providerSelect");
    const unitCostInput = document.getElementById("UnitCost");
    const quantityInput = document.getElementById("Quantity");
    const orderItemsTable = document.getElementById("orderItemsTable");

    if (!providerSelect || !materialTypeSelect || !materialSelect || !sizeSelect) {
        console.error("Error: No se encontraron uno o más elementos del formulario.");
        return;
    }

    let materialSizesData = [];

    // Evento cuando se selecciona un tipo de material
    materialTypeSelect.addEventListener("change", function () {
        resetFilters();
        const materialTypeId = this.value.trim();
        if (!materialTypeId) return;
        
        fetch(`/Material/GetMaterialsByType?materialTypeId=${materialTypeId}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(material => {
                    materialSelect.innerHTML += `<option value="${material.id}">${material.name}</option>`;
                });
            })
            .catch(error => console.error("Error al obtener materiales:", error));
    });

    materialSelect.addEventListener("change", function () {
        resetSizeAndProvider();
        const materialId = this.value.trim();
        if (!materialId) return;

        fetch(`/PurchaseOrder/GetSizesByMaterial?materialId=${materialId}`)
            .then(response => response.json())
            .then(data => {
                materialSizesData = data;
                data.forEach(sizeItem => {
                    sizeSelect.innerHTML += `<option value="${sizeItem.sizeId}">${sizeItem.size_Name}</option>`;
                });
            })
            .catch(error => console.error("Error al obtener tamaños:", error));
    });

    sizeSelect.addEventListener("change", function () {
        const selectedSizeId = parseInt(this.value, 10);
        const selectedMaterialId = materialSelect.value.trim();
        if (!selectedSizeId || !selectedMaterialId) return;

        const selectedSize = materialSizesData.find(size => size.sizeId === selectedSizeId);
        if (selectedSize) {
            unitCostInput.value = selectedSize.cost.toFixed(2);
            fetch(`/PurchaseOrder/GetProvidersByMaterial?materialId=${selectedMaterialId}&sizeId=${selectedSizeId}`)
                .then(response => response.json())
                .then(providers => updateProviderSelect(selectedSize, providers))
                .catch(error => console.error("Error al obtener proveedores:", error));
        }
    });

    function updateProviderSelect(selectedSize, providers) {
        providerSelect.innerHTML = "<option value=''>Seleccione un proveedor</option>";
        if (selectedSize?.lastProviderId) {
            providerSelect.innerHTML += `<option value="${selectedSize.lastProviderId}" selected>${selectedSize.lastProviderName}</option>`;
        }
        if (Array.isArray(providers)) {
            providers.forEach(provider => {
                if (provider.id !== selectedSize?.lastProviderId) {
                    providerSelect.innerHTML += `<option value="${provider.id}">${provider.businessName}</option>`;
                }
            });
        }
    }

    function resetFilters() {
        materialSelect.innerHTML = "<option value=''>Seleccione un material</option>";
        resetSizeAndProvider();
    }

    function resetSizeAndProvider() {
        sizeSelect.innerHTML = "<option value=''>Seleccione un tamaño</option>";
        providerSelect.innerHTML = "<option value=''>Seleccione un proveedor</option>";
        unitCostInput.value = "";
        quantityInput.value = "";
    }

    // Manejo de Modales (Agregar / Buscar Proveedor)
    const searchProviderModalElement = document.getElementById("searchProviderModal");
    const providerSelectModal = document.getElementById("modalProviderSelect");
    const selectProviderButton = document.getElementById("modalSelectProviderButton");
    const orderProviderSelect = document.getElementById("providerSelect");

    providerSelectModal.addEventListener("change", async function () {
        const selectedOption = this.options[this.selectedIndex];
        console.log("selectedOption value = ",selectedOption.value)
    
        try {
            // Realizamos la solicitud para obtener los detalles del proveedor
            const response = await fetch(`/Receipt/GetProviderDetails?providerId=${selectedOption.value}`);
            console.log("response = ",response);
            if (!response.ok) {
                throw new Error(`Error al obtener los detalles del proveedor. Status: ${response.status}`);
            }
    
            // Procesamos la respuesta JSON
            const data = await response.json();
            console.log("data = ",data);
            // Mostramos los datos obtenidos en un alert para depuración
            // alert(`Datos obtenidos:\n${JSON.stringify(data, null, 2)}`);
    
            // Asignamos los valores a los campos correspondientes
            document.getElementById('BusinessName').value = data.businessName || '';
            document.getElementById('Address').value = data.address || '';
            document.getElementById('Phone').value = data.phone || '';
            document.getElementById('RUT').value = data.rut || '';
    
        } catch (error) {
            console.error("Error al cargar los detalles del proveedor:", error);
            alert(`Hubo un error al cargar los detalles del proveedor:\n${error.message}`);
        }
    });

    selectProviderButton.addEventListener("click", function () {
        const selectedProviderId = providerSelectModal.value;
        const selectedProviderName = providerSelectModal.options[providerSelectModal.selectedIndex].text;
        if (!selectedProviderId) {
            alert("Debe seleccionar un proveedor.");
            return;
        }
        let existingOption = Array.from(orderProviderSelect.options).find(option => option.value === selectedProviderId);
        if (!existingOption) {
            const newOption = new Option(selectedProviderName, selectedProviderId, true, true);
            orderProviderSelect.appendChild(newOption);
        } else {
            existingOption.selected = true;
        }
        const searchProviderModal = bootstrap.Modal.getInstance(document.getElementById("searchProviderModal"));
        searchProviderModal.hide();
    });

    searchProviderModalElement.addEventListener("show.bs.modal", function () {
        providerSelectModal.value = "";
        resetProviderFields();
    });

    function resetProviderFields() {
        document.getElementById("BusinessName").value = "";
        document.getElementById("Phone").value = "";
        document.getElementById("RUT").value = "";
        document.getElementById("Address").value = "";
    }
});


function addItem() {
    const materialTypeSelect = document.getElementById("MaterialTypeId");
    const materialSelect = document.getElementById("MaterialId");
    const sizeSelect = document.getElementById("SizeId");
    const providerSelect = document.getElementById("providerSelect");
    const unitCostInput = document.getElementById("UnitCost");
    const quantityInput = document.getElementById("Quantity");
    const orderItemsTable = document.getElementById("orderItemsTable");

    const materialTypeId = materialTypeSelect.value;
    const materialTypeName = materialTypeSelect.selectedOptions[0].text;
    const materialId = materialSelect.value;
    const materialName = materialSelect.selectedOptions[0].text;
    const sizeId = sizeSelect.value;
    const sizeName = sizeSelect.selectedOptions[0].text;
    const quantity = parseInt(quantityInput.value, 10);
    const unitCost = parseFloat(unitCostInput.value);
    const providerId = providerSelect.value;
    const providerName = providerSelect.selectedOptions[0].text;

    if (!materialTypeId || !materialId || !sizeId || isNaN(quantity) || isNaN(unitCost) || !providerId) {
        alert("Todos los campos son obligatorios.");
        return;
    }

    const newItem = {
        MaterialTypeId: materialTypeId,
        MaterialTypeName: materialTypeName,
        MaterialId: materialId,
        MaterialName: materialName,
        SizeId: sizeId,
        SizeName: sizeName,
        Quantity: quantity,
        UnitCost: unitCost,
        ProviderId: providerId,
        ProviderName: providerName
    };

    fetch('/PurchaseOrder/AddItem', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newItem)
    })
    .then(response => response.json())
    .then(result => {
        if (result.success) {
            updateItemList(result.items);
        } else {
            alert("Error al agregar el material: " + result.message);
        }
    })
    .catch(error => {
        console.error("Error al agregar el material:", error);
        alert("Ocurrió un error al intentar agregar el material.");
    });
}

function generatePurchaseOrders() {
    fetch('/PurchaseOrder/GeneratePurchaseOrders', { method: 'POST' })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                alert("Órdenes de compra generadas exitosamente.");
                window.location.href = "/PurchaseOrder/Index";;
            } else {
                alert("Error al generar órdenes de compra.");
            }
        })
        .catch(error => console.error("Error al generar órdenes de compra:", error));
}

function removeItem(materialId, sizeId) {
    fetch(`/PurchaseOrder/RemoveItem?materialId=${materialId}&sizeId=${sizeId}`, { method: 'DELETE' })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                updateItemList(result.items);
            } else {
                alert("Error al eliminar el material.");
            }
        })
        .catch(error => console.error("Error al eliminar material:", error));
}

function updateItemList(items) {
    orderItemsTable.innerHTML = "";

    items.forEach(item => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${item.materialTypeName}</td>
            <td>${item.materialName}</td>
            <td>${item.sizeName}</td>
            <td>${item.quantity}</td>
            <td>${item.providerName}</td>
            <td><button class="btn btn-danger btn-sm" onclick="removeItem('${item.materialId}', ${item.sizeId})">❌</button></td>
        `;
        orderItemsTable.appendChild(row);
    });

    console.log("Lista de ítems actualizada.");
}
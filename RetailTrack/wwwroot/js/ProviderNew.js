document.getElementById("saveProviderButton").addEventListener("click", async () => {
    const providerData = {
        Name: document.getElementById("providerName").value.trim(),
        BusinessName: document.getElementById("providerBusinessName").value.trim(),
        Phone: document.getElementById("providerPhone").value.trim(),
        RUT: document.getElementById("providerRUT").value.trim(),
        Address: document.getElementById("providerAddress").value.trim(),
        Description: document.getElementById("providerDescription").value.trim()
    };

    console.log("Enviando datos al backend:", providerData);

    try {
        console.log("* Enviando datos al backend:", providerData);
    
        const response = await fetch('/Provider/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(providerData)
        });
    
        const result = await response.json();
    
        console.log("Respuesta del backend:", result);
    
        if (!response.ok || !result.success) {
            throw new Error("Error en la respuesta del servidor: " + (result.message || response.statusText));
        }
    
        // Cerrar el modal
        $('#addProviderModal').modal('hide');

        console.log("Cerramos modal:");
        
        // Agregar el nuevo proveedor al select de Receipt
        const providerSelect = document.getElementById("providerSelect");
        const newOption = new Option(result.providerName, result.providerId);
        console.log("antes de agrgar :", newOption, "a providerSelect:", providerSelect);
        providerSelect.add(newOption);
    
        // Seleccionar automáticamente el proveedor recién agregado
        providerSelect.value = result.providerId;
    
        alert("Proveedor agregado exitosamente.");
    } catch (error) {
        console.error("Error al agregar el proveedor:", error);
        alert("Error al intentar agregar el proveedor.");
    }
    
});

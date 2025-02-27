document.getElementById('providerSelect').addEventListener('change', async function () {
    const providerId = this.value;

    if (!providerId) {
        console.error("No se seleccionó un proveedor.");
        return;
    }

    try {
        // Realizamos la solicitud para obtener los detalles del proveedor
        const response = await fetch(`/Receipt/GetProviderDetails?providerId=${providerId}`);
        if (!response.ok) {
            throw new Error(`Error al obtener los detalles del proveedor. Status: ${response.status}`);
        }

        // Procesamos la respuesta JSON
        const data = await response.json();

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
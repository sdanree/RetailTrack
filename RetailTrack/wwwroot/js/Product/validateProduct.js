document.getElementById("registerProductButton").addEventListener("click", async function (event) {
    event.preventDefault(); // Evita la recarga del formulario

    // Obtener valores de los campos
    let productName = document.getElementById("SelectedProductName").value.trim();
    let generalPrice = parseFloat(document.getElementById("SelectedGeneralPrice").value);
    let designId = document.getElementById("designSelect").value;
    let description = document.getElementById("SelectedDescription").value;

    // Validar que los campos requeridos no estén vacíos
    if (!productName) {
        alert("El nombre del producto es obligatorio.");
        return;
    }
    if (!generalPrice || generalPrice <= 0) {
        alert("El precio de venta debe ser mayor a 0.");
        return;
    }
    if (!designId) {
        alert("Debe seleccionar un diseño.");
        return;
    }

    // Construir JSON con solo los datos del producto (las variantes ya están en sesión)
    let productData = {
        Product: {
            Name: productName,
            GeneralPrice: generalPrice,
            Description: description,
            DesignId: designId
        }
    };

    try {
        let response = await fetch('/Product/CreateProduct', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(productData)
        });

        let data = await response.json();

        if (data.success) {
            window.location.href = "/Product";
        } else {
            alert("Error al crear el producto: " + (data.message || "Error desconocido."));
        }
    } catch (error) {
        console.error("Error en la solicitud:", error);
        alert("Hubo un problema al procesar la solicitud.");
    }
});

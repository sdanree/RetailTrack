document.addEventListener("DOMContentLoaded", function () {
    event.preventDefault(); 
    const saveButton = document.getElementById("saveProductButton");

    saveButton.addEventListener("click", async function () {
        const name = document.getElementById("SelectedProductName").value.trim();
        const price = parseFloat(document.getElementById("SelectedGeneralPrice").value);
        const description = document.getElementById("SelectedDescription").value.trim();
        const status = document.getElementById("SelectedState").value;

        
        if (!name || isNaN(price) || price <= 0 || !status) {
            alert("Por favor, complete todos los campos obligatorios correctamente.");
            return;
        }

        // Armar el objeto que vas a enviar (viewModel)
        const viewModel = {
            Product: {
                Id: document.querySelector("input[name='Product.Id']").value,
                Name: name,
                Description: description,
                GeneralPrice: price,
                ProductStatusId: parseInt(status),
                DesignId: document.querySelector("input[name='Product.DesignId']").value,
            }
        };

        try {
            const response = await fetch("/Product/EditProduct", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(viewModel)
            });

            const result = await response.json();
            if (result.success) {
                alert("Producto actualizado correctamente.");
                window.location.href = "/Product/Index";
            } else {
                alert("Error: " + (result.message || "No se pudo actualizar el producto."));
            }
        } catch (error) {
            console.error("Error en la solicitud:", error);
            alert("Error inesperado. Intente nuevamente.");
        }
    });
});

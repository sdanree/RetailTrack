document.addEventListener("DOMContentLoaded", function () {
    console.log("editVariantList.js");

    // Agregar nueva variante
    document.getElementById("add-variant").addEventListener("click", async function () {
        
        const materialId = document.querySelector("[name='MaterialId']").value;
        const sizeId = document.querySelector("[name='SizeId']").value;
        const Stock = document.querySelector("[name='Stock']").value;
        const Price = document.querySelector("[name='Price']").value;
        const ProductName = document.querySelector("[name='SelectedProductName'").value;

        if (!materialId){
            alert("Por favor, seleccione un material válido.");
            return;
        }

        if(!sizeId || isNaN(sizeId)){
            alert("Por favor, seleccione un tamaño válido.");
            return;
        }

        const variantData = {
            MaterialId: materialId,
            SizeId: sizeId,
            Stock: Stock ? parseInt(Stock) : 0,
            CustomPrice: Price ? parseFloat(Price) : 0,
            SelectedProductName: ProductName 
        }

        try {
            const response = await fetch('/Product/AddVariant', {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    'Accept' : 'application/json',
                },
                body: JSON.stringify(variantData)
            });

            if (!response.ok) {
                throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
            }

            const result = await response.json();
            console.log("Respuesta del servidor:", result);

            if (result.success) {
                window.location.reload();

            } else {
                    alert("Error al agregar variante: " + result.message);
            }
            setTimeout(updateTotalAmount, 500);       

        } catch (error) {
            console.error("Error al agregar el material:", error);
            alert("Ocurrió un error al intentar agregar el material.");        
        }
    });    

    // Actualizar stock, precio y costo de la variante
    document.querySelectorAll(".variant-stock, .variant-price").forEach(input => {
        input.addEventListener("change", async function () {
            updateVariant(this);
            setTimeout(updateTotalAmount, 500);
        });
    });

    // Eliminar variante
    document.querySelectorAll(".delete-variant").forEach(button => {
        button.addEventListener("click", async function () {
            // variantId es materialId, ya que aun se supone que no existe el id de la variante
            const materialId = this.getAttribute("data-material-id");

            try {
                const response = await fetch('/Product/DeleteVariant', {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                    },
                    body: JSON.stringify({ MaterialId: materialId }),                        
                    
                });

                if (!response.ok) {
                    throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
                }

                const result = await response.json();
                console.log("Respuesta del servidor:", result);

                if (result.success) {
                    window.location.reload();

                } else {
                    alert("Error al eliminar variante: " + result.message);
                }
                setTimeout(updateTotalAmount, 500);       

            } catch (error) {
                console.error("Error al eliminar la variante:", error);
                alert("Ocurrió un error al intentar eliminar la variante.");        
            }
        });
    });
    
});    


async function updateVariant(element){
    if (!element) {
        console.error("Elemento no válido proporcionado a updateVariant.");
        return;
    }

    const materialId = element.getAttribute("data-material-id");
    if (!materialId) {
        console.error("El elemento no contiene un atributo data-material-id.");
        alert("Error: No se pudo identificar el material de la variante.");
        return;
    }

    const row = element.closest("tr"); // Encuentra la fila para buscar los otros datos
    const StockInput = row.querySelector(".variant-quantity");
    const CustomPriceInput = row.querySelector(".variant-unitcost");

    const updatedVariant = {
        MaterialId: materialId,
        Stock: parseInt(StockInput.value),
        CustomPrice: parseFloat(CustomPriceInput.value)
    };

    // Validaciones
    if (isNaN(updatedVariant.Stock) || updatedVariant.Stock <= 0) {
        alert("Por favor, ingrese una cantidad válida.");
        return;
    }

    if (isNaN(updatedVariant.CustomPrice) || updatedVariant.CustomPrice <= 0) {
        alert("Por favor, ingrese un costo unitario válido.");
        return;
    }

    console.log("Actualizando Variante:", updatedVariant);

    try {
        const response = await fetch('/Product/UpdateVariant', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(updatedVariant)
        });

        if (!response.ok) {
            console.error(`Error en la respuesta del servidor: ${response.statusText}`);
            alert("Ocurrió un error al intentar actualizar el material.");
            return;
        }

        const result = await response.json();
        if (!result.success) 
        {
            alert("Error al actualizar el material: " + result.message);
            return;
        }
         
    } catch (error) {
        console.error("Error al intentar actualizar el material:", error);
        alert("Ocurrió un error inesperado al intentar actualizar el material.");
    }
}
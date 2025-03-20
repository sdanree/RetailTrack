document.addEventListener("DOMContentLoaded", function () {
    let ProductName = document.getElementById("SelectedProductName");
    let Description = document.getElementById("SelectedDescription");
    let GeneralPrice = document.getElementById("SelectedGeneralPrice");
    let DesignId = document.getElementById("designSelect");

    function updateProductHeader() {
            console.log("ejecutando updateProductHeader");
            const updateHeader = {
                SelectedProductName: ProductName.value || "",
                SelectedDescription: Description.value || "",
                SelectedGeneralPrice: parseFloat(GeneralPrice.value) || 0,
                SelectedDesignId: DesignId.value || null
            }
            
            console.log("upadateheader", updateHeader);
            
            fetch('/Product/UpdateProductHeader', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updateHeader) 
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (!data.success) {
                    throw new Error(data.message || "Error desconocido.");
                }
                console.log("Código externo guardado en sesión:", data.externalCode);
            })
            .catch(error => console.error("Error guardando código externo:", error.message));
    };

    ProductName.addEventListener("change", updateProductHeader);
    Description.addEventListener("change", updateProductHeader);
    GeneralPrice.addEventListener("change", updateProductHeader);
    DesignId.addEventListener("change", updateProductHeader);

});
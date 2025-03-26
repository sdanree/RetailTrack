document.addEventListener("DOMContentLoaded", function () {
    const newProductButton = document.getElementById("newProductButton");
    const editButtons = document.querySelectorAll(".editProductButton");

    if (newProductButton) {
        newProductButton.addEventListener("click", async function () {
            try {
                const response = await fetch('/Product/ClearProductSession', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });

                const result = await response.json();

                if (result.success) {
                    window.location.href = "/Product/Create";
                } else {
                    alert("Error al limpiar la sesión. Intente nuevamente.");
                }
            } catch (error) {
                console.error("Error al limpiar la sesión:", error);
                alert("Ocurrió un error. Intente nuevamente.");
            }
        });
    }

    editButtons.forEach(button => {
        button.addEventListener("click", async function () {
            const productId = this.getAttribute("data-product-id");

            if (!productId) {
                console.error("No se encontró el ID del producto.");
                return;
            }

            try {
                const response = await fetch('/Product/ClearProductSession', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });

                const result = await response.json();

                if (result.success) {
                    window.location.href = `/Product/Edit/${productId}`;
                } else {
                    alert("Error al limpiar la sesión. Intente nuevamente.");
                }
            } catch (error) {
                console.error("Error al limpiar la sesión:", error);
                alert("Ocurrió un error. Intente nuevamente.");
            }
        });
    });
});

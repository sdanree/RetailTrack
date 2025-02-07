document.addEventListener("DOMContentLoaded", function () {
    const newReceiptButton = document.getElementById("newReceiptButton");

    if (!newReceiptButton) {
        console.error("Error: No se encontró el botón de nueva orden de compra.");
        return;
    }

    newReceiptButton.addEventListener("click", async function () {
        try {
            const response = await fetch('/Receipt/ClearReceiptSession', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });

            const result = await response.json();

            if (result.success) {
                console.log("Sesión de la orden de compra limpiada correctamente.");
                window.location.href = "/Receipt/Create";
            } else {
                alert("Error al limpiar la sesión. Intente nuevamente.");
            }
        } catch (error) {
            console.error("Error al limpiar la sesión:", error);
            alert("Ocurrió un error. Intente nuevamente.");
        }
    });
});

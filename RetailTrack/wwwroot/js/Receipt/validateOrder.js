document.addEventListener("DOMContentLoaded", function () {
    console.log("Script de validación cargado correctamente.");

    const registerOrderButton = document.getElementById("registerOrderButton");

    if (!registerOrderButton) {
        console.error("Error: No se encontró el botón de registrar orden.");
        return;
    }

    registerOrderButton.addEventListener("click", async function (event) {
        event.preventDefault(); // Evita el envío automático del formulario

        console.log("Validando datos antes de registrar la orden...");

        // Validar proveedor
        const providerCard = document.querySelector(".card[data-provider-id]");
        let providerId = providerCard ? providerCard.getAttribute("data-provider-id") : null;

        if (!providerId || providerId === "00000000-0000-0000-0000-000000000000") {
            alert("Debe seleccionar un proveedor válido.");
            console.error("Proveedor inválido:", providerId);
            return;
        }

        console.log("Proveedor seleccionado:", providerId);

        // Validar materiales
        const items = Array.from(document.querySelectorAll(".material-row"));
        if (items.length === 0) {
            alert("Debe agregar al menos un material a la orden.");
            console.error("Lista de materiales vacía.");
            return;
        }

        // Validar métodos de pago
        const payments = Array.from(document.querySelectorAll(".payment-row"));
        if (payments.length === 0) {
            alert("Debe agregar al menos un método de pago.");
            console.error("Lista de métodos de pago vacía.");
            return;
        }

        // Calcular totales
        let totalMaterials = 0;
        const materialData = items.map(row => {
            const materialId = row.querySelector(".material-quantity").getAttribute("data-material-id");
            const sizeId = parseInt(row.querySelector(".material-quantity").getAttribute("data-size-id"), 10);
            const quantity = parseFloat(row.querySelector(".material-quantity").value) || 0;
            const unitCost = parseFloat(row.querySelector(".material-unitcost").value) || 0;

            totalMaterials += quantity * unitCost;

            return { MaterialId: materialId, SizeId: sizeId, Quantity: quantity, UnitCost: unitCost };
        });

        let totalPayments = 0;
        const paymentData = payments.map(row => {
            const paymentMethodId = parseInt(row.querySelector(".payment-amount").getAttribute("data-payment-method-id"), 10);
            const amount = parseFloat(row.querySelector(".payment-amount").value) || 0;

            totalPayments += amount;

            return { PaymentMethodId: paymentMethodId, Amount: amount };
        });

        if (totalMaterials.toFixed(2) !== totalPayments.toFixed(2)) {
            alert("La suma de los métodos de pago no coincide con el costo total de los materiales.");
            console.error(`Totales no coinciden. Materiales: ${totalMaterials}, Pagos: ${totalPayments}`);
            return;
        }

        console.log("Totales validados correctamente.");

        // Crear el JSON a enviar
        const orderData = {
            ProviderId: providerId,
            Items: materialData,
            Payments: paymentData
        };

        console.log("Datos a enviar al servidor:", orderData);

        try {
            const response = await fetch('/Receipt/CreateReceipt', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(orderData)
            });

            const result = await response.json();

            if (!response.ok || !result.success) {
                console.error("Error del servidor:", result);

                if (result.errors) {
                    console.error("Errores del modelo:", result.errors);
                    alert("Errores en los datos enviados:\n" + result.errors.map(e => `${e.field}: ${e.errors.join(', ')}`).join('\n'));
                } else {
                    alert(result.message);
                }
                return;
            }

            window.location.href = "/Receipt/Index";
        } catch (error) {
            console.error("Error al registrar la orden:", error);
            alert("Ocurrió un error al intentar registrar la orden.");
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    console.log("Script de validación de orden cargado correctamente.");

    const registerOrderButton = document.getElementById("registerOrderButton");
    const orderForm = document.getElementById("orderForm");

    if (!registerOrderButton) {
        console.error("Error: No se encontró el botón de registrar orden.");
        return;
    }

    if (!orderForm) {
        console.error("Error: No se encontró el formulario de orden.");
        return;
    }

    registerOrderButton.addEventListener("click", function (event) {
        event.preventDefault(); // Evita el envío automático del formulario

        console.log("Validando totales antes de registrar la orden...");

        let totalMaterials = 0;
        let totalPayments = 0;

        // Obtener valores de materiales desde los inputs ocultos
        document.querySelectorAll("input[name^='Items'][name$='UnitCost']").forEach((input, index) => {
            const quantityInput = document.querySelector(`input[name='Items[${index}].Quantity']`);
            const unitCost = parseFloat(input.value) || 0;
            const quantity = parseFloat(quantityInput.value) || 0;
            totalMaterials += unitCost * quantity;
        });

        // Obtener valores de métodos de pago desde los inputs ocultos
        document.querySelectorAll("input[name^='Payments'][name$='Amount']").forEach(input => {
            totalPayments += parseFloat(input.value) || 0;
        });

        console.log("Total Materiales:", totalMaterials.toFixed(2));
        console.log("Total Pagos:", totalPayments.toFixed(2));

        // Validación: Si los totales no coinciden, detener el envío
        if (totalMaterials.toFixed(2) !== totalPayments.toFixed(2)) {
            alert("Error: La suma de los pagos no coincide con el costo total de los materiales.");
            return; // Bloquea la ejecución para que el formulario no se envíe
        }

        console.log("Totales validados correctamente. Enviando formulario...");

        // Enviar el formulario si los valores son correctos
        orderForm.submit();
    });
});

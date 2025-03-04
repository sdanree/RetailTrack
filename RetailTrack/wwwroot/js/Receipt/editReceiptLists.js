document.addEventListener("DOMContentLoaded", function () {
    console.log("Script de edición cargado.");

    // Agregar un nuevo material
    document.getElementById("addMaterialButton").addEventListener("click", async function () {
        const materialId    = document.querySelector("[name='NewItem.MaterialId']").value;
        const sizeId        = parseInt(document.querySelector("[name='NewItem.SizeId']").value);
        const quantity      = parseInt(document.querySelector("[name='NewItem.Quantity']").value);
        const unitCost      = parseFloat(document.querySelector("[name='NewItem.UnitCost']").value);

        // Validaciones
        if (!materialId) {
            alert("Por favor, seleccione un material válido.");
            return;
        }

        if (!sizeId || isNaN(sizeId)) {
            alert("Por favor, seleccione un tamaño válido.");
            return;
        }

        if (isNaN(quantity) || quantity <= 0) {
            alert("Por favor, ingrese una cantidad válida.");
            return;
        }

        if (isNaN(unitCost) || unitCost <= 0) {
            alert("Por favor, ingrese un costo unitario válido.");
            return;
        }

        const materialData = {
            MaterialId: materialId,
            SizeId: sizeId,
            Quantity: quantity,
            UnitCost: unitCost
        };

        console.log("Enviando nuevo material:", materialData);

        try {
            const response = await fetch('/Receipt/AddItem', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(materialData)
            });

            if (!response.ok) {
                throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
            }

            const result = await response.json();
            console.log("Respuesta del servidor:", result);

            if (result.success) {
                window.location.reload();

            } else {
                    alert("Error al agregar el material: " + result.message);
            }
            setTimeout(updateTotalAmount, 500);
        } catch (error) {
            console.error("Error al agregar el material:", error);
            alert("Ocurrió un error al intentar agregar el material.");
        }
    });

    // Actualizar materiales al cambiar cantidad o costo unitario
    document.querySelectorAll(".material-quantity, .material-unitcost").forEach(input => {
        input.addEventListener("change", async function () {
            updateMaterial(this);
            setTimeout(updateTotalAmount, 500);
        });
    });

    // Manejar eliminación de materiales
    document.querySelectorAll(".delete-material").forEach(button => {
        button.addEventListener("click", async function () {
            const materialId = button.getAttribute("data-material-id");

            if (!materialId) {
                alert("Error: No se pudo identificar el material a eliminar.");
                console.error("MaterialId no encontrado.");
                return;
            }

            if (!confirm("¿Está seguro de que desea eliminar este material?")) {
                return;
            }

            console.log("Eliminando material con ID:", materialId);

            try {
                const response = await fetch(`/Receipt/DeleteMaterial`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                    },
                    body: JSON.stringify({ MaterialId: materialId }),
                });

                if (!response.ok) {
                    throw new Error(`Error en la solicitud: ${response.statusText}`);
                }

                const result = await response.json();

                if (result.success) {
                    setTimeout(updateTotalAmount, 500);
                    window.location.reload(); 
                } else {
                    alert(`Error al eliminar el material: ${result.message}`);
                    console.error(result);
                }
            } catch (error) {
                console.error("Error al intentar eliminar el material:", error);
                alert("Ocurrió un error al intentar eliminar el material.");
            }
        });
    });

    // Agregar un nuevo método de pago
    document.getElementById("addPaymentButton").addEventListener("click", async function () {
        const paymentMethodId   = document.querySelector("[name='NewPayment.PaymentMethodId']").value;
        const paymentAmount     = document.querySelector("[name='NewPayment.Amount']").value;

        // Validaciones
        if (!paymentMethodId || paymentMethodId === "0") {
            alert("Por favor, seleccione un método de pago válido.");
            return;
        }

        if (!paymentAmount || isNaN(parseFloat(paymentAmount)) || parseFloat(paymentAmount) <= 0) {
            alert("Por favor, ingrese un importe válido.");
            return;
        }

        const paymentData = {
            PaymentMethodId: parseInt(paymentMethodId, 10),
            Amount: parseFloat(paymentAmount),
            Percentage: null
        };

        console.log("Enviando nuevo método de pago:", paymentData);

        try {
            const response = await fetch('/Receipt/AddPayment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(paymentData)
            });

            if (!response.ok) {
                throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
            }

            const result = await response.json();
            console.log("Resultado del backend:", result);

            if (result.success) {
                console.log("Método de pago agregado correctamente:", result.payments);
                window.location.reload();
            } else {
                alert("Error al agregar el método de pago: " + result.message);
            }
        } catch (error) {
            console.error("Error al agregar el método de pago:", error);
            alert("Ocurrió un error al intentar agregar el método de pago.");
        }
    });

    // Actualizar importe de método de pago al cambiar valor
    document.querySelectorAll(".payment-amount").forEach(input => {
        input.addEventListener("change", function () {
            updatePayment(this);
        });
    });

    // Eliminacion de metodo de pago
    document.querySelectorAll(".delete-payment").forEach(button => {
        button.addEventListener("click", async function () {
            const paymentMethodId = button.getAttribute("data-payment-method-id");
    
            if (!paymentMethodId) {
                alert("Error: No se pudo identificar el método de pago a eliminar.");
                console.error("PaymentMethodId no encontrado.");
                return;
            }
    
            if (!confirm("¿Está seguro de que desea eliminar este metodo de pago?")) {
                return;
            }

            console.log("Eliminando método de pago con ID:", paymentMethodId);
    
            try {
                const response = await fetch(`/Receipt/DeletePayment`, {
                    method: 'POST', // Cambiar a POST, ya que el controlador lo usa
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify({ PaymentMethodId: parseInt(paymentMethodId, 10) }),
                });
    
                if (!response.ok) {
                    throw new Error(`Error en la solicitud: ${response.statusText}`);
                }
    
                const result = await response.json();
    
                if (result.success) {
                    window.location.reload();
                } else {
                    alert(`Error al eliminar el método de pago: ${result.message}`);
                    console.error(result);
                }
            } catch (error) {
                console.error("Error al intentar eliminar el método de pago:", error);
                alert("Ocurrió un error al intentar eliminar el método de pago.");
            }
        });
    });
    
});

document.addEventListener("DOMContentLoaded", function () {
    let externalCodeInput = document.getElementById("ReceiptExternalCode");

    if (externalCodeInput) {
        externalCodeInput.addEventListener("change", function () {
            fetch('/Receipt/UpdateExternalCode', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ ExternalCode: this.value }) 
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
        });
    }
});



// Actualizar un método de pago
async function updatePayment(element) {
    if (!element) {
        console.error("Elemento no válido proporcionado a updatePayment.");
        return;
    }

    const paymentMethodId   = element.getAttribute("data-payment-method-id");
    const amountValue       = parseFloat(element.value);

    if (!paymentMethodId) {
        console.error("El elemento no contiene un atributo data-payment-method-id.");
        alert("Error: No se pudo identificar el método de pago.");
        return;
    }

    if (isNaN(amountValue) || amountValue <= 0) {
        alert("Por favor, ingrese un importe válido para el método de pago.");
        return;
    }

    const updatedPayment = {
        PaymentMethodId: parseInt(paymentMethodId),
        Amount: amountValue
    };

    console.log("Actualizando método de pago:", updatedPayment);

    try {
        const response = await fetch('/Receipt/UpdatePayment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(updatedPayment)
        });

        const result = await response.json();
        if (!result.success) {
            alert("Error al actualizar el método de pago: " + result.message);
        }
    } catch (error) {
        console.error("Error al intentar actualizar el método de pago:", error);
        alert("Ocurrió un error inesperado al intentar actualizar el método de pago.");
    }
}

// Actualizar Material
async function updateMaterial(element) {
    if (!element) {
        console.error("Elemento no válido proporcionado a updateMaterial.");
        return;
    }

    const materialId = element.getAttribute("data-material-id");
    if (!materialId) {
        console.error("El elemento no contiene un atributo data-material-id.");
        alert("Error: No se pudo identificar el material.");
        return;
    }

    const row = element.closest("tr"); // Encuentra la fila para buscar los otros datos
    const quantityInput = row.querySelector(".material-quantity");
    const unitCostInput = row.querySelector(".material-unitcost");

    const updatedMaterial = {
        MaterialId: materialId,
        Quantity: parseInt(quantityInput.value),
        UnitCost: parseFloat(unitCostInput.value)
    };

    // Validaciones
    if (isNaN(updatedMaterial.Quantity) || updatedMaterial.Quantity <= 0) {
        alert("Por favor, ingrese una cantidad válida.");
        return;
    }

    if (isNaN(updatedMaterial.UnitCost) || updatedMaterial.UnitCost <= 0) {
        alert("Por favor, ingrese un costo unitario válido.");
        return;
    }

    console.log("Actualizando material:", updatedMaterial);

    try {
        const response = await fetch('/Receipt/UpdateMaterial', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(updatedMaterial)
        });

        if (!response.ok) {
            console.error(`Error en la respuesta del servidor: ${response.statusText}`);
            alert("Ocurrió un error al intentar actualizar el material.");
            return;
        }

        const result = await response.json();
        if (result.success) {
            const totalCostCell = row.querySelector(".material-total-cost");
            totalCostCell.textContent = `$${(updatedMaterial.Quantity * updatedMaterial.UnitCost).toFixed(2)}`;
        } else {
            alert("Error al actualizar el material: " + result.message);
        }
    } catch (error) {
        console.error("Error al intentar actualizar el material:", error);
        alert("Ocurrió un error inesperado al intentar actualizar el material.");
    }
}

//Actualizar importe Total
function updateTotalAmount() {
    let totalAmount = 0;

    document.querySelectorAll(".material-row").forEach(row => {
        const quantity = parseFloat(row.querySelector(".material-quantity").value) || 0;
        const unitCost = parseFloat(row.querySelector(".material-unitcost").value) || 0;
        totalAmount += quantity * unitCost;
    });

    // Actualiza el campo de total en la UI
    document.getElementById("ReceiptTotalAmount").value = totalAmount.toFixed(2);
}

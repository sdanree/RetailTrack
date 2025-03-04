document.addEventListener("DOMContentLoaded", function () {
    const togglePurchaseOrders = document.getElementById("togglePurchaseOrders");
    const purchaseOrdersTableBody = document.getElementById("purchaseOrdersTableBody");
    const processSelectedOrders = document.getElementById("processSelectedOrders");
    const selectedOrdersContainer = document.getElementById("selectedOrdersContainer");
    const providerCard = document.querySelector(".card[data-provider-id]");
    let providerId = providerCard ? providerCard.getAttribute("data-provider-id") : null;
    const selectAllCheckbox = document.getElementById("selectAll");
    let selectedOrders = [];
    let materialList = [];

    function GetPurchaseOrders() 
    {
        fetch(`/Receipt/GetPurchaseOrdersByProviderIdAndStatus?providerId=${providerId}&status=1`)
            .then(response => response.json())
            .then(data => {
                purchaseOrdersTableBody.innerHTML = "";
                
                if (data.length === 0) {
                    purchaseOrdersTableBody.innerHTML = `<tr><td colspan="6" class="text-center">No hay órdenes aprobadas.</td></tr>`;
                    return;
                }

                data.forEach(order => {
                    console.log("Orden de compra:", order);
                    const row = document.createElement("tr");
                    row.innerHTML = `
                        <td><input type="checkbox" class="order-checkbox" value="${order.purchaseOrderId}"'></td>
                        <td>${order.purchaseOrderNumber}</td>
                        <td>${new Date(order.orderDate).toLocaleDateString()}</td>
                        <td>${order.providerName}</td>
                        <td>${order.status}</td>
                        <td>$${order.totalAmount.toFixed(2)}</td>
                        <td>
                            <a href="${window.location.origin}/PurchaseOrder/Details/${order.purchaseOrderId}" class="btn btn-info btn-sm">
                                <i class="bi bi-search"></i>
                            </a>
                        </td>  
                    `;
                    purchaseOrdersTableBody.appendChild(row);
                });
            })
            .catch(error => console.error("Error al cargar órdenes de compra:", error));
    }

    togglePurchaseOrders.addEventListener("click", function () {
        GetPurchaseOrders();
    });

    selectAllCheckbox.addEventListener("change", function () {
        document.querySelectorAll(".order-checkbox").forEach(checkbox => {
            checkbox.checked = selectAllCheckbox.checked;
        });
    });

    document.getElementById("processSelectedOrders").addEventListener("click", async function () {
        const checkboxes = document.querySelectorAll(".order-checkbox:checked");
    
        if (checkboxes.length === 0) {
            alert("Seleccione al menos una orden de compra.");
            return;
        }
    
        const selectedOrders = Array.from(checkboxes).map(checkbox => checkbox.value);

        try {
            const response = await fetch('/Receipt/AddItemFromPurchaseOrders', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(selectedOrders)
            });
    
            if (!response.ok) {
                throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
            }
    
            const result = await response.json();
            console.log("Respuesta del servidor:", result);
    
            if (result.success) {
                window.location.reload();
            } else {
                alert("Error al procesar las órdenes de compra: " + result.message);
            }
        } catch (error) {
            console.error("Error al procesar las órdenes de compra:", error);
            alert("Ocurrió un error al intentar procesar las órdenes de compra.");
        }
    });

    // Eliminar una orden de compra del recibo
    document.querySelectorAll(".delete-purchaseOrder").forEach(button => {
        button.addEventListener("click", async function () {
            const purchaseOrderId = this.getAttribute("data-purchase-order-id");
            if (!confirm("¿Está seguro de que desea eliminar esta orden de compra?")) {
                return;
            }

            console.log("Eliminando orden de compra:", purchaseOrderId);

            try {
                const response = await fetch(`/Receipt/DeletePurchaseOrderFromReceipt`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(purchaseOrderId)
                });

                if (!response.ok) {
                    throw new Error(`Error en la respuesta del servidor: ${response.statusText}`);
                }

                const result = await response.json();
                console.log("Respuesta del servidor tras eliminar orden:", result);

                if (result.success) {
                    window.location.reload();
                } else {
                    alert("Error al eliminar la orden de compra: " + result.message);
                }
            } catch (error) {
                console.error("Error al eliminar la orden de compra:", error);
                alert("Ocurrió un error al intentar eliminar la orden de compra.");
            }
        });
    });

    GetPurchaseOrders();
});

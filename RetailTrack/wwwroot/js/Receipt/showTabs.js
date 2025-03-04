document.getElementById("togglePayments").addEventListener("click", function() {
    document.getElementById("payments-tab").classList.remove("d-none");
    document.getElementById("purchaseorders-tab").classList.remove("d-none");
    document.getElementById("materials-tab").classList.add("d-none");
    document.getElementById("payments-tab").click();
});

document.getElementById("toggleMaterials").addEventListener("click", function() {
    document.getElementById("materials-tab").classList.remove("d-none");
    document.getElementById("purchaseorders-tab").classList.remove("d-none");
    document.getElementById("payments-tab").classList.add("d-none");
    document.getElementById("materials-tab").click();
});

document.getElementById("togglePurchaseOrders").addEventListener("click", function() {
    document.getElementById("materials-tab").classList.remove("d-none");
    document.getElementById("payments-tab").classList.remove("d-none");
    document.getElementById("purchaseorders-tab").classList.add("d-none");
    document.getElementById("purchaseorders-tab").click();
});
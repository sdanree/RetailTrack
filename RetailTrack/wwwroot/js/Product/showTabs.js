document.getElementById("toggleVariants").addEventListener("click", function() {
    console.log("toggleVariants");
    document.getElementById("product-info-tab").classList.remove("d-none");
    document.getElementById("product-variants-tab").classList.add("d-none");
    document.getElementById("product-variants-tab").click();
});
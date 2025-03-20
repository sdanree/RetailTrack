document.addEventListener("DOMContentLoaded", function () {
    const imageContainer = document.getElementById('designImageContainer');
    const imageElement = document.getElementById('designImage');
    const designImageUrl = document.getElementById('designImageUrl').value;

    if (designImageUrl && designImageUrl.trim() !== "") {
        imageElement.src = designImageUrl.startsWith("/") ? designImageUrl : `/${designImageUrl}`;
        imageContainer.style.display = "block";
    } else {
        imageContainer.style.display = "none";
    }
});

document.getElementById('designSelect').addEventListener('change', async function () {
    const desingId = this.value;

    if (!desingId) {
        console.error("No se seleccionó un desing.");
        return;
    }

    try {
        const response = await fetch(`/Design/GetDesignDetails?designId=${desingId}`);
        if (!response.ok) {
            throw new Error(`Error al obtener los detalles del design. Status: ${response.status}`);
        }

        const data = await response.json();

        document.getElementById('designDescription').value = data.description || '';
        document.getElementById('designPrice').value = data.price || '';
        document.getElementById('designComision').value = data.comision || '';

        const imageContainer = document.getElementById('designImageContainer');
        const imageElement = document.getElementById('designImage');

        if (!data.imageUrl || data.imageUrl.trim() === "") {
            imageContainer.style.display = "none";
        } else {
            const imageUrl = `/${data.imageUrl}`;
            document.getElementById('designImageUrl').value = imageUrl;
            imageElement.src = imageUrl;
            imageContainer.style.display = "block"; 
        }

    } catch (error) {
        console.error("Error al cargar los detalles del design:", error);
        alert(`Hubo un error al cargar los detalles del design:\n${error.message}`);
    }
});
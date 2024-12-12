window.getMaterialsByType = async function (materialTypeId) {
    if (!materialTypeId) {
        console.error("No se proporcionó el tipo de material.");
        return;
    }

    try {
        const response = await fetch(`/Product/GetMaterialsByType?materialTypeId=${materialTypeId}`);
        if (!response.ok) {
            throw new Error("Error al cargar los materiales.");
        }

        const result = await response.json();

        const materialDropdown = document.getElementById("Materials");
        if (!materialDropdown) {
            console.error("El elemento con id 'Materials' no se encontró en el DOM.");
            return;
        }

        materialDropdown.innerHTML = ""; // Limpia las opciones actuales

        if (result.success) {
            result.data.forEach(material => {
                const option = document.createElement("option");
                option.value = material.id;
                option.textContent = material.name;
                materialDropdown.appendChild(option);
            });
        } else {
            const option = document.createElement("option");
            option.value = "";
            option.textContent = result.message || "No hay materiales disponibles.";
            materialDropdown.appendChild(option);
        }
    } catch (error) {
        console.error("Error en getMaterialsByType:", error);
        alert("Hubo un error al cargar los materiales. Inténtelo de nuevo.");
    }
};

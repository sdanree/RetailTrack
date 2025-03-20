window.getMaterialsByType = async function (materialTypeId) {
    if (!materialTypeId) {
        console.error("No se proporcionó el tipo de material.");
        return;
    }

    try {
        const response = await fetch(`/Material/GetMaterialsByType?materialTypeId=${materialTypeId}`);
        if (!response.ok) {
            throw new Error("Error al cargar los materiales.");
        }
        
        console.log("response :", response);
        
        const result = await response.json();

        const materialDropdown = document.getElementById("MaterialId");
        if (!materialDropdown) {
            console.error("El elemento con id 'Materials' no se encontró en el DOM.");
            return;
        }

        materialDropdown.innerHTML = ""; // Limpia las opciones actuales
        console.log("Datos recibidos:", result.data);
        console.log("Result GetMaterials:", result.success);

        if (result.success) {
            const option = document.createElement("option");
            option.value = "";
            option.textContent = "Seleccione un material";    
            materialDropdown.appendChild(option);
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

window.getMaterialSizesByMaterial = async function (materialId) {
    if (!materialId) {
        console.error("No se proporcionó material.");
        return;
    }

    try {
        const response = await fetch(`/Material/GetMaterialSizesByMaterial?materialId=${materialId}`);
        if (!response.ok) {
            throw new Error("Error al cargar los materiales.");
        }
        
        console.log("response :", response);
        
        const result = await response.json();

        const SizeDropdown = document.getElementById("SizeId");
        if (!SizeDropdown) {
            console.error("El elemento con id 'Materials' no se encontró en el DOM.");
            return;
        }

        SizeDropdown.innerHTML = ""; // Limpia las opciones actuales
        console.log("Datos recibidos:", result.data);
        console.log("Result GetMaterials:", result.success);

        if (result.success) {
            const option = document.createElement("option");
            option.value = "";
            option.textContent = "Seleccione un tamaño";    
            SizeDropdown.appendChild(option);
            result.data.forEach(size => {
                const option = document.createElement("option");
                option.value = size.sizeId;
                option.textContent = size.sizeName;
                SizeDropdown.appendChild(option);
            });
        } else {
            const option = document.createElement("option");
            option.value = "";
            option.textContent = result.message || "No hay talles disponibles.";
            SizeDropdown.appendChild(option);
        }        
    } catch (error) {
        console.error("Error en getMaterialsByType:", error);
        alert("Hubo un error al cargar los materiales. Inténtelo de nuevo.");        
    }
};    
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models.Products
{
    public class MaterialType
    {
        [Key]
        public Guid Id { get; private set; } // Identificador único para el tipo de material

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Nombre del tipo de material (e.g., Remera, Niño)

        public ICollection<Material> Materials { get; set; } // Lista de materiales asociados

        public MaterialType(string name)
        {
            Id = Guid.NewGuid(); // Generar ID único
            Name = name;
            Materials = new List<Material>();
        }
        public MaterialType(){}
    }
}

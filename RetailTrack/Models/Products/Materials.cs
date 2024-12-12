using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models.Products
{
    public class Material
    {
        [Key]
        public Guid Id { get; private set; } // Identificador único para el material

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Nombre del material (e.g., negro, blanco)

        [Required]
        public decimal Cost { get; set; } // Costo del material

        [Required]
        public int Stock { get; set; } // Stock disponible para el material

        [Required]
        public Guid MaterialTypeId { get; set; } // Foreign Key para MaterialType

        [ForeignKey(nameof(MaterialTypeId))]
        public MaterialType MaterialType { get; set; } // Relación con MaterialType

        public Material(string name, decimal cost, int stock, Guid materialTypeId)
        {
            Id              = Guid.NewGuid(); // Generar ID único
            Name            = name;
            Cost            = cost;
            Stock           = stock;
            MaterialTypeId  = materialTypeId;
        }

        public Material(){}     

    }
}

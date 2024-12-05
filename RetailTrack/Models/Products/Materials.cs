using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RetailTrack.Models.Products
{
    public class Material
    {
        // Propiedades del material
        [Key]
        public Guid Id { get; private set; } // Identificador único del material (GUID)
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Nombre del material

        [Required]
        public decimal Cost { get; set; } // Costo del material

        [Required]
        public int Quantity { get; set; } // Cantidad requerida para el producto

        // Constructor
        public Material(string name, decimal cost, int quantity)
        {
            Id = Guid.NewGuid(); // Generar un GUID único automáticamente
            Name = name;
            Cost = cost;
            Quantity = quantity;
        }

        // Método para mostrar información del material
        public override string ToString()
        {
            return $"ID: {Id} - {Name} - ${Cost:F2} (Cantidad: {Quantity})";
        }
    }
}


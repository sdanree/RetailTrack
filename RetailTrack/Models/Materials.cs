using System;

namespace RetailTrack.Models
{
    public class Material
    {
        // Propiedades del material
        public Guid Id { get; private set; } // Identificador único del material (GUID)
        public string Name { get; set; } // Nombre del material
        public decimal Cost { get; set; } // Costo del material
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


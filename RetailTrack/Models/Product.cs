using System;
using System.Collections.Generic;
using System.Linq;

namespace RetailTrack.Models
{
    public class Product
    {
        // Propiedades básicas del producto
        public Guid Id { get; private set; } // Identificador único del producto
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } // Precio base del producto
        public int QuantityRequested { get; set; } // Cantidad solicitada del producto

        // Lista de materiales (BOM)
        public List<Material> Materials { get; set; }

        // Diseño asociado al producto
        public Design Design { get; set; }

        // Talle del producto
        public ProductSize Size { get; set; }

        // Estado del producto
        public ProductStatus Status { get; private set; }

        // Lista de movimientos asociados al producto
        public List<Movement> Movements { get; private set; }

        // Constructor
        public Product(string name, string description, int quantityRequested, ProductSize size, ProductStatus status, Design design)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            QuantityRequested = quantityRequested;
            Size = size;
            Status = status;
            Design = design;
            Materials = new List<Material>();
            Movements = new List<Movement>();
        }

        // Confirmar alta de producto
        public void ConfirmCreation(decimal finalPrice, string currency)
        {
            var movement = new Movement(MovementType.PorCobrar, finalPrice, currency);
            Movements.Add(movement);
            Console.WriteLine($"Movimiento generado: {movement}");
        }

        // Cambiar estado del producto a "Sold" y registrar movimiento
        public void MarkAsSold(decimal finalPrice, string currency)
        {
            if (Status != ProductStatus.Pending && Status != ProductStatus.ReadyToMake)
            {
                throw new InvalidOperationException("El producto debe estar en un estado vendible antes de ser marcado como vendido.");
            }

            Status = ProductStatus.Sold;
            var movement = new Movement(MovementType.Vendido, finalPrice, currency);
            Movements.Add(movement);
            Console.WriteLine($"Estado actualizado a 'Sold' y movimiento generado: {movement}");
        }

        public override string ToString()
        {
            return $"{Name} - Talle: {Size} - Estado: {Status} - Cantidad Solicitada: {QuantityRequested} - ${Price:F2} - Diseño: {Design?.Name ?? "N/A"}";
        }
    }
}

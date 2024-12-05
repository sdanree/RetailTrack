using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RetailTrack.Models.Products
{ 
    public class Product
    {
        // Propiedades básicas del producto
        [Key]
        public Guid Id { get; private set; } // Identificador único del producto
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } // Precio base del producto
        public int QuantityRequested { get; set; } // Cantidad solicitada del producto

        // Lista de materiales (BOM)
        public List<Material> Materials { get; set; } = new List<Material>();

        // Diseño asociado al producto
        [Required(ErrorMessage = "El diseño es obligatorio")]
        [ForeignKey("DesingId")]
        public Guid DesignId { get; set; } 
        public Design Design { get; set; } = null!;

        // Talle del producto
        [Required]
        public ProductSize Size { get; set; }

        // Estado del producto
        [Required]
        public ProductStatus Status { get; private set; }

        // Lista de movimientos asociados al producto
        public List<Movement> Movements { get; private set; } = new List<Movement>();

        // Constructor
        public Product(string name, string description, int quantityRequested, ProductSize size, ProductStatus status)
        {
            Name = name;
            Description = description;
            QuantityRequested = quantityRequested;
            Size = size;
            Status = status;
            Materials = new List<Material>();
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

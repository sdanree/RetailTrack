using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailTrack.Models;

namespace RetailTrack.Models.Products
{
    public class Product
    {
        [Key]
        public Guid Id { get; private set; } 

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set;}

        public decimal Price { get; set; } 

        public int QuantityRequested { get; set; } 

        [Required]
        public Guid MaterialId  { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }

        [Required]
        public Guid DesignId { get; set; }

        [ForeignKey("DesignId")]
        public Design? Design { get; set; }        

        [Required]
        public int ProductSizeId { get; set; } 
        
        [ForeignKey("ProductSizeId")]
        public ProductSize? Size { get; set; }

        [Required]
        public int ProductStatusId { get; set; } 
        
        [ForeignKey("ProductStatusId")]
        public ProductStatus? Status { get; set; }

        public List<Movement> Movements { get; private set; } = new List<Movement>();

        public Product() 
        { 
            // Id = 1 is "pedido" is the default value.
            ProductStatusId = 1;
        }

        public void ConfirmCreation(decimal finalPrice, string currency, MovementType movementType)
        {
            var movement = new Movement
            {
                MovementTypeId  = movementType.Movement_Id,
                FinalPrice      = finalPrice,
                Currency        = currency,
                Timestamp       = DateTime.UtcNow
            };
            Movements.Add(movement);
            Console.WriteLine($"Movimiento generado: {movement}");
        }

        public void MarkAsSold(decimal finalPrice, string currency, int soldStatusId, MovementType soldMovementType)
        {
            if (Status.Status_Id != 1 && Status.Status_Id != 2) // Reemplaza con los IDs correctos para "Pending" o "ReadyToMake"
            {
                throw new InvalidOperationException("El producto debe estar en un estado vendible antes de ser marcado como vendido.");
            }

            ProductStatusId = soldStatusId;
            var movement = new Movement
            {
                MovementTypeId  = soldMovementType.Movement_Id,
                FinalPrice      = finalPrice,
                Currency        = currency,
                Timestamp       = DateTime.UtcNow
            };
            Movements.Add(movement);
            Console.WriteLine($"Estado actualizado a 'Sold' y movimiento generado: {movement}");
        }

        public override string ToString()
        {
            return $"{Name} - Talle: {Size.Size_Name} - Estado: {Status.Status_Name} - Cantidad Solicitada: {QuantityRequested} - ${Price:F2}";
        }
    }
}

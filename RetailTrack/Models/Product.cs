
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int QuantityRequested { get; set; }

        [Required]
        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; } = null!;

        [Required]
        public Guid DesignId { get; set; }

        [ForeignKey(nameof(DesignId))]
        public Design Design { get; set; } = null!;

        [Required]
        public int ProductStatusId { get; set; }

        [ForeignKey(nameof(ProductStatusId))]
        public ProductStatus Status { get; set; } = null!;

        public ICollection<Movement> Movements { get; set; } = new List<Movement>();

        public void ConfirmCreation(decimal finalPrice, string currency, MovementType movementType)
        {
            var movement = new Movement
            {
                MovementTypeId = movementType.Movement_Id,
                FinalPrice = finalPrice,
                Currency = currency,
                Timestamp = DateTime.UtcNow
            };
            Movements.Add(movement);
        }

        public void MarkAsSold(decimal finalPrice, string currency, int soldStatusId, MovementType soldMovementType)
        {
            ProductStatusId = soldStatusId;
            var movement = new Movement
            {
                MovementTypeId = soldMovementType.Movement_Id,
                FinalPrice = finalPrice,
                Currency = currency,
                Timestamp = DateTime.UtcNow
            };
            Movements.Add(movement);
        }
    }
}

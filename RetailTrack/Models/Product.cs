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

        [Required]
        public Guid DesignId { get; set; }

        [ForeignKey(nameof(DesignId))]
        public Design Design { get; set; } = null!;

        // Referencias a la clave compuesta de MaterialSize
        [Required]
        public Guid MaterialId { get; set; } 

        [Required]
        public int SizeId { get; set; } 

        [ForeignKey("MaterialId, SizeId")]
        public MaterialSize MaterialSize { get; set; } = null!; 

        [Required]
        public int ProductStatusId { get; set; }

        [ForeignKey(nameof(ProductStatusId))]
        public ProductStatus Status { get; set; } = null!;

        public ICollection<Movement> Movements { get; set; } = new List<Movement>();
    }
}

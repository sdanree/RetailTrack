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

        [Required]
        public Guid DesignId { get; set; }

        [ForeignKey(nameof(DesignId))]
        public Design Design { get; set; } = null!;

        public decimal? GeneralPrice { get; set; }
        
        [Required]
        public int ProductStatusId { get; set; }

        [ForeignKey(nameof(ProductStatusId))]
        public ProductStatus Status { get; set; } = null!;

        public ICollection<ProductStock> Variants { get; set; } = new List<ProductStock>();
    }

}

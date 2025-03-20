
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class ProductStock
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        // Cada presentación tiene un MaterialSize único (material + talle)
        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        public int SizeId { get; set; }

        [ForeignKey("MaterialId, SizeId")]
        public MaterialSize MaterialSize { get; set; } = null!;

        public decimal? CustomPrice { get; set; } // Precio de venta específico para esta variante

        [Required]
        public decimal Cost { get; set; } // Costo de fabricación

        [Required]
        public int Stock { get; set; } = 0;
        
        [Required]
        public bool Available {get; set;} = false;
    }

}    
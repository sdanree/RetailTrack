using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class MaterialSize
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; } = null!;

        [Required]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SizeId))]
        public Size Size { get; set; } = null!;

        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public Guid? LastProviderId { get; set; }
        public string? LastProviderName { get; set; }        
    }
}

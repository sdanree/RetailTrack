
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class Material
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public Guid MaterialTypeId { get; set; }

        [ForeignKey(nameof(MaterialTypeId))]
        public MaterialType MaterialType { get; set; } = null!;

        [Required]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SizeId))]
        public Size Size { get; set; } = null!;
    }
}

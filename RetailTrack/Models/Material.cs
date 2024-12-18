using System;
using System.Collections.Generic;
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
        public Guid MaterialTypeId { get; set; }

        [ForeignKey(nameof(MaterialTypeId))]
        public MaterialType MaterialType { get; set; } = null!;

        // Relación con MaterialSize
        public ICollection<MaterialSize> MaterialSizes { get; set; } = new List<MaterialSize>();
    }
}

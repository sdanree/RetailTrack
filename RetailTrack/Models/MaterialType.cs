
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class MaterialType
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}

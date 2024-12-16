
using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class Provider
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RUT { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}

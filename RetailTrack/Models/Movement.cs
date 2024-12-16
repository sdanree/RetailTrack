
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class Movement
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int MovementTypeId { get; set; }

        [ForeignKey(nameof(MovementTypeId))]
        public MovementType Type { get; set; } = null!;

        [Required]
        public decimal FinalPrice { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }
    }
}

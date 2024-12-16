
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class ReceiptDetail
    {
        [Key]
        public Guid DetailId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ReceiptId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public Receipt Receipt { get; set; } = null!;

        [Required]
        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        [NotMapped]
        public decimal TotalCost => Quantity * UnitCost;

        [Required]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SizeId))]
        public Size Size { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailTrack.Models;

namespace RetailTrack.Models
{
    public class PurchaseOrderDetail
    {
        [Key]
        public Guid DetailId { get; set; }

        [Required]
        public Guid PurchaseOrderId { get; set; }

        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrder PurchaseOrder { get; set; }

        [Required]
        public Guid MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; }

        [Required]
        public int SizeId { get; set; }

        [ForeignKey(nameof(SizeId))]
        public Size Size { get; set; }

        [Required]
        public int Quantity { get; set; } // Cantidad solicitada

        [Required]
        public decimal UnitCost { get; set; } // Costo unitario al momento de generar la orden

        public int? ReceivedQuantity { get; set; } // Cantidad recibida hasta el momento
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailTrack.Models;

namespace RetailTrack.Models
{
    public class PurchaseOrder
    {
        [Key]
        public Guid PurchaseOrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } 

        public int? PurchaseOrderNumber { get; set; } 

        [Required]
        public Guid ProviderId { get; set; } 

        [ForeignKey(nameof(ProviderId))]
        public Provider Provider { get; set; }

        public List<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>(); 

        public List<Receipt> Receipts { get; set; } = new List<Receipt>();

        public PurchaseOrderStatus Status { get; set; }
    }
}
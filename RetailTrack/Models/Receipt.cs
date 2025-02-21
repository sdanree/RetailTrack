using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class Receipt
    {
        [Key]
        public Guid ReceiptId { get; set; } = Guid.NewGuid();

        public decimal ReceiptTotalAmount => Details?.Sum(d => d.Quantity * d.UnitCost) ?? 0;

        [Required]
        public DateTime ReceiptDate { get; set; }

        public string? ReceiptExternalCode {get;set;}

        public ICollection<ReceiptPayment> Payments { get; set; } = new List<ReceiptPayment>();

        public ICollection<ReceiptDetail> Details { get; set; } = new List<ReceiptDetail>();

        [Required]
        public Guid ProviderId {get;set;}

        [ForeignKey(nameof(ProviderId))]
        public Provider Provider { get; set; } = null!;
    }
}

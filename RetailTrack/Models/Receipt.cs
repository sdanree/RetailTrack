using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class Receipt
    {
        [Key]
        public Guid ReceiptId { get; set; } = Guid.NewGuid();

        [Required]
        public decimal ReceiptAmount { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        public ICollection<ReceiptPayment> Payments { get; set; } = new List<ReceiptPayment>();

        public ICollection<ReceiptDetail> Details { get; set; } = new List<ReceiptDetail>();

        [Required]
        public Provider Provider { get; set; } = null!;
    }
}

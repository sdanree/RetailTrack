
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace RetailTrack.Models
{
    public class ReceiptPayment
    {
        public Guid ReceiptPaymentId { get; set; }

        [Required]
        public Guid ReceiptId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public Receipt? Receipt { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }

        [ForeignKey(nameof(PaymentMethodId))]
        public PaymentMethod? PaymentMethod { get; set; }

        public decimal Percentage { get; set; }
    }
}
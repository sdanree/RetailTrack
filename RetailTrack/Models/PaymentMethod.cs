
using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; } // Clave primaria

        [Required]
        public string Name { get; set; } = null!; // Nombre del método de pago

        // Relación con ReceiptPayments
        public ICollection<ReceiptPayment> Receipts { get; set; } = new List<ReceiptPayment>();
    }
}
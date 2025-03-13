using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderNumber { get; set; } // Número de orden secuencial

        [Required, MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? CustomerPhone { get; set; }

        [MaxLength(300)]
        public string? CustomerAddress { get; set; }

        [MaxLength(50)]
        public string? CustomerRut { get; set; } // RUT para clientes empresariales

        public string? Comments { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? EstimatedCompletionDate { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        [ForeignKey(nameof(OrderStatusId))]
        public OrderStatus Status { get; set; } = new OrderStatus();

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
    }
}

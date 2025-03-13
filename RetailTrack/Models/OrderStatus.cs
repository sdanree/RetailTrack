using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailTrack.Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public enum OrderStatusEnum
    {
        Pendiente = 1,
        EnProceso = 2,
        ProntoParaEntrega = 3,
        Finalizado = 4
    }    
}    
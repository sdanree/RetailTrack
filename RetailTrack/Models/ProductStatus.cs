
using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class ProductStatus
    {
        [Key]
        public int Status_Id { get; set; }

        public string Status_Name { get; set; } = string.Empty;
    }

    public enum ProductStatusEnum
    {
        EnProduccion = 1,
        ListoParaVenta = 2,
        Vendido = 3,
        Devuelto = 4
    }    
}
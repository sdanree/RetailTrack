using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RetailTrack.Models
{
    public class Movement
    {
        // Propiedades del movimiento
        [Key]
        public Guid Id { get; private set; } // Identificador único del movimiento
        [Required]
        public MovementType Type { get; private set; } // Tipo de movimiento
        [Required]
        public decimal FinalPrice { get; private set; } // Precio final del movimiento
        [Required]
        [MaxLength(10)]
        public string Currency { get; private set; } // Moneda del movimiento
        [Required]
        public DateTime Timestamp { get; private set; } // Fecha y hora del movimiento

        // Constructor
        public Movement(MovementType type, decimal finalPrice, string currency)
        {
            Id = Guid.NewGuid();
            Type = type;
            FinalPrice = finalPrice;
            Currency = currency;
            Timestamp = DateTime.UtcNow;
        }

        // Método para actualizar el tipo de movimiento
        public void UpdateType(MovementType newType)
        {
            Type = newType;
        }

        // Método para mostrar información del movimiento
        public override string ToString()
        {
            return $"ID: {Id} - Tipo: {Type} - Precio Final: {FinalPrice:F2} {Currency} - Fecha: {Timestamp}";
        }
    }
}

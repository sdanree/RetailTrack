using System;

namespace RetailTrack.Models
{
    // Enum para los tipos de movimiento
    public enum MovementType
    {
        PorCobrar, // Movimiento generado al confirmar el alta de un producto
        Vendido    // Movimiento generado cuando un producto se vende
    }

    public class Movement
    {
        // Propiedades del movimiento
        public Guid Id { get; private set; } // Identificador único del movimiento
        public MovementType Type { get; private set; } // Tipo de movimiento
        public decimal FinalPrice { get; private set; } // Precio final del movimiento
        public string Currency { get; private set; } // Moneda del movimiento
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

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
        public int MovementTypeId { get; set; } // Llave foránea
        [ForeignKey("MovementTypeId")]
        public MovementType Type { get; set; } // Relación con la tabla `MovementTypes`

        [Required]
        public decimal FinalPrice { get; set; } // Precio final del movimiento

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = string.Empty; // Moneda del movimiento

        [Required]
        public DateTime Timestamp { get; set; } // Fecha y hora del movimiento

        // Constructor vacío
        public Movement() { }

        // Método para actualizar el tipo de movimiento
        public void UpdateType(MovementType newType)
        {
            Type = newType;
            MovementTypeId = newType.Movement_Id;
        }

        // Método para mostrar información del movimiento
        public override string ToString()
        {
            return $"ID: {Id} - Tipo: {Type.Movement_Name} - Precio Final: {FinalPrice:F2} {Currency} - Fecha: {Timestamp}";
        }
    }
}

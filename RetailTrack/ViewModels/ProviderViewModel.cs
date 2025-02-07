using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.ViewModels
{
    public class ProviderViewModel
    {

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre del proveedor no puede exceder los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [MaxLength(150, ErrorMessage = "La razón social no puede exceder los 150 caracteres.")]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El RUT no puede exceder los 50 caracteres.")]
        public string RUT { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "La dirección no puede exceder los 300 caracteres.")]
        public string Address { get; set; } = string.Empty;

        // Para guardar contactos adicionales en caso necesario.
        [MaxLength(150, ErrorMessage = "El contacto no puede exceder los 150 caracteres.")]
        public string? Contact { get; set; }
    }
}

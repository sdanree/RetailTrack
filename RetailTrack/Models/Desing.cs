using System;

namespace RetailTrack.Models
{
    public class Design
    {
        // Propiedades del diseño
        public Guid Id { get; private set; } // Identificador único del diseño (GUID)
        public string Name { get; set; } // Nombre del diseño
        public string Description { get; set; } // Descripción del diseño
        public string ImageUrl { get; set; } // URL de la imagen del diseño

        // Constructor
        public Design(string name, string description, string imageUrl)
        {
            Id = Guid.NewGuid(); // Generar un GUID único automáticamente
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
        }

        // Método para mostrar información del diseño
        public override string ToString()
        {
            return $"ID: {Id} - {Name}: {Description} (Imagen: {ImageUrl})";
        }
    }
}

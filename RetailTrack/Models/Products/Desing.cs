using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace RetailTrack.Models.Products
{
    public class Design
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        
        public Design(){}
        /*
        public Design(string name, string description, string imageUrl)
        {
         
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
        
        }
        */
        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }
}

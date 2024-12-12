using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class ProductSize
    {
        [Key]
        public int Size_Id { get; set; }
        public string Size_Name { get; set; } = string.Empty;
    }

    public class ProductStatus
    {
        [Key]
        public int Status_Id { get; set; }
        public string Status_Name { get; set; } = string.Empty;
    }

    public class MovementType
    {
        [Key]
        public int Movement_Id { get; set; }
        public string Movement_Name { get; set; } = string.Empty;
    }
}

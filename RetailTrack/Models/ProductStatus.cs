
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
        [Description("Habilitado")]
        Available = 1,

        [Description("Sin stock")]
        OutStock = 2,

        [Description("Descontinuado")]
        Unavailable = 3,
    }    

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }    
}
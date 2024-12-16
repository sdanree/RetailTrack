
using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class Size
    {
        [Key]
        public int Size_Id { get; set; }

        public string Size_Name { get; set; } = string.Empty;
    }
}

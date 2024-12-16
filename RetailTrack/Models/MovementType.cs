
using System;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.Models
{
    public class MovementType
    {
        [Key]
        public int Movement_Id { get; set; }

        public string Movement_Name { get; set; } = string.Empty;
    }
}

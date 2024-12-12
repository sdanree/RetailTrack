using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models.Products;
using System.Collections.Generic;

namespace RetailTrack.ViewModels
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new Product();
        public IEnumerable<SelectListItem> Designs { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Materials { get; set; } = new List<SelectListItem>();
        public string MaterialTypeId { get; set; } = string.Empty; 
    }
}

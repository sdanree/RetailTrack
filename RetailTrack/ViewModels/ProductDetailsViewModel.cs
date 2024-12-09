using RetailTrack.Models.Products;

namespace RetailTrack.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = new Product();
        public Design? Design { get; set; }
    }
}

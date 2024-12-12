namespace RetailTrack.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuantityRequested { get; set; }
        public string? Size { get; set; }
        public string? Design { get; set; }
        public string? MaterialType { get; set; }
        public string? Material { get; set; }
        public string? Status { get; set; }
    }
}

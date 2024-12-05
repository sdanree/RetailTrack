using RetailTrack.Data;
using RetailTrack.Models.Products;

namespace RetailTrack.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los productos
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        // Obtener un producto por ID
        public Product? GetProductById(Guid id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id) ?? throw new Exception("Producto no encontrado");
        }


        // Crear un nuevo producto
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Actualizar un producto existente
        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        // Eliminar un producto
        public void DeleteProduct(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}

using RetailTrack.Data;
using RetailTrack.Models;
using Microsoft.EntityFrameworkCore;


namespace RetailTrack.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly DesignService _designService;

        public ProductService(ApplicationDbContext context, DesignService designService)
        {
            _context        = context;
            _designService  = designService;
        }

         public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Status)
                .Include(p => p.Material)
                    .ThenInclude(m => m.MaterialType)
                .Include(p => p.Design)
                .AsNoTracking()
                .ToList();
        }

        public Product? GetProductById(Guid id)
        {
            return _context.Products
                .Include(p => p.Status)
                .Include(p => p.Material)
                    .ThenInclude(m => m.MaterialType)
                .Include(p => p.Design)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            // Validar y establecer el estado de ProductStatus
            if (product.Status != null)
            {
                var trackedStatus = _context.ChangeTracker.Entries<ProductStatus>()
                    .FirstOrDefault(e => e.Entity.Status_Id == product.Status.Status_Id);

                if (trackedStatus != null)
                {
                    trackedStatus.State = EntityState.Unchanged;
                }
                else
                {
                    _context.Entry(product.Status).State = EntityState.Unchanged;
                }
            }

            // Validar y establecer el estado de Material
            if (product.Material != null)
            {
                var trackedMaterial = _context.ChangeTracker.Entries<Material>()
                    .FirstOrDefault(e => e.Entity.Id == product.Material.Id);

                if (trackedMaterial != null)
                {
                    trackedMaterial.State = EntityState.Unchanged;
                }
                else
                {
                    _context.Entry(product.Material).State = EntityState.Unchanged;
                }
            }

            // Validar y establecer el estado de Design
            if (product.Design != null)
            {
                var trackedDesign = _context.ChangeTracker.Entries<Design>()
                    .FirstOrDefault(e => e.Entity.Id == product.Design.Id);

                if (trackedDesign != null)
                {
                    trackedDesign.State = EntityState.Unchanged;
                }
                else
                {
                    _context.Entry(product.Design).State = EntityState.Unchanged;
                }
            }

            // Agregar el producto y guardar cambios
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }



        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public async Task<ProductStatus?> GetProductStatusByIdAsync(int statusId)
        {
            return await _context.ProductStatuses
                .AsNoTracking() 
                .FirstOrDefaultAsync(s => s.Status_Id == statusId);
        }

        public async Task<MaterialType?> GetMaterialTypeByIdAsync(Guid materialTypeId)
        {
            return await _context.MaterialTypes //.ToListAsync();
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == materialTypeId);
        }

        public async Task<List<Material>> GetMaterialsByTypeAsync(Guid materialTypeId)
        {
            return await _context.Materials
                .Where(m => m.MaterialTypeId == materialTypeId)
                .ToListAsync();
        }

        public async Task<Material?> GetMaterialByIdAsync(Guid materialId)
        {
            return await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == materialId);
        }

    }
}

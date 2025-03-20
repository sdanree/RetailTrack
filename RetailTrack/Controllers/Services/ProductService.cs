using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailTrack.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crear un producto
        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        
        public async Task AddProductStocksAsync(List<ProductStock> productStocks)
        {
            if (productStocks == null || !productStocks.Any())
            {
                throw new ArgumentException("La lista de variantes de productos (ProductStock) está vacía o es nula.");
            }

            // Asegurar que las referencias a MaterialSize sean correctas
            foreach (var stock in productStocks)
            {
                var existingMaterialSize = await _context.MaterialSizes
                    .FirstOrDefaultAsync(ms => ms.MaterialId == stock.MaterialId && ms.SizeId == stock.SizeId);

                if (existingMaterialSize == null)
                {
                    throw new InvalidOperationException($"No se encontró MaterialSize para MaterialId {stock.MaterialId} y SizeId {stock.SizeId}.");
                }

                stock.MaterialSize = existingMaterialSize;
            }

            // Agregar los ProductStock en la BD
            await _context.ProductStocks.AddRangeAsync(productStocks);
            await _context.SaveChangesAsync();
        }

        // Obtener todos los productos con sus variantes
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Design)
                .Include(p => p.Status)
                .Include(p => p.Variants) // Incluir las variantes del producto
                    .ThenInclude(v => v.MaterialSize)
                        .ThenInclude(ms => ms.Material) // Incluir Material dentro de MaterialSize
                .Include(p => p.Variants)
                    .ThenInclude(v => v.MaterialSize)
                        .ThenInclude(ms => ms.Size) // Incluir Size dentro de MaterialSize
                .ToListAsync();
        }

        // Obtener un producto por ID con sus variantes
        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Design)
                .Include(p => p.Status)
                .Include(p => p.Variants) // Incluir las variantes del producto
                    .ThenInclude(v => v.MaterialSize)
                        .ThenInclude(ms => ms.Material) // Incluir Material dentro de MaterialSize
                .Include(p => p.Variants)
                    .ThenInclude(v => v.MaterialSize)
                        .ThenInclude(ms => ms.Size) // Incluir Size dentro de MaterialSize
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Actualizar un producto
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Eliminar un producto
        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProductStatus>> GetAllProductStatusesAsync()
        {
            if (!await _context.ProductStatuses.AnyAsync())
            {
                var defaultStatuses = Enum.GetValues<ProductStatusEnum>()
                    .Select(e => new ProductStatus
                    {
                        Status_Id = (int)e,
                        Status_Name = e.GetDescription() // Usa la descripción en lugar del nombre del enum
                    })
                    .ToList();

                await _context.ProductStatuses.AddRangeAsync(defaultStatuses);
                await _context.SaveChangesAsync();

                return defaultStatuses;
            }

            return await _context.ProductStatuses.ToListAsync();
        }

        // Obtener un estado de producto por ID
        public async Task<ProductStatus?> GetProductStatusByIdAsync(int id)
        {
            return await _context.ProductStatuses.FindAsync(id);
        }

        // Obtener todos los diseños
        public async Task<List<Design>> GetAllDesignsAsync()
        {
            return await _context.Designs.ToListAsync();
        }

        // Obtener un diseño por ID
        public async Task<Design?> GetDesignByIdAsync(Guid id)
        {
            return await _context.Designs.FindAsync(id);
        }

        // Obtener todos los MaterialSizes
        public async Task<List<MaterialSize>> GetAllMaterialSizesAsync()
        {
            return await _context.MaterialSizes
                .Include(ms => ms.Material)
                .Include(ms => ms.Size)
                .ToListAsync();
        }

        // Obtener un MaterialSize por ID
        public async Task<MaterialSize?> GetMaterialSizeByIdAsync(Guid id)
        {
            return await _context.MaterialSizes
                .Include(ms => ms.Material)
                .Include(ms => ms.Size)
                .FirstOrDefaultAsync(ms => ms.Id == id);
        }

        public async Task<List<ProductStock>> GetProductStocksByProductIdAsync(Guid productId)
        {
            return await _context.ProductStocks
                .Where(ps => ps.ProductId == productId)
                .Include(ps => ps.MaterialSize)
                    .ThenInclude(ms => ms.Material)
                        .ThenInclude(m => m.MaterialType)
                .Include(ps => ps.MaterialSize)
                    .ThenInclude(ms => ms.Size)
                .ToListAsync();
        }


    }
}

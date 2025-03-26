using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models;

public class MaterialService
{
    private readonly ApplicationDbContext _context;

    public MaterialService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Crear Material
    public async Task AddMaterialAsync(Material material)
    {
        await _context.Materials.AddAsync(material);
        await _context.SaveChangesAsync();
    }

    // Leer Material por ID
    public async Task<Material?> GetMaterialByIdAsync(Guid id)
    {
        return await _context.Materials
            .Include(m => m.MaterialType) // Incluye el MaterialType asociado
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<MaterialType>> GetAllMaterialTypesAsync()
    {
        return await _context.MaterialTypes.ToListAsync();
    }    

    public async Task<Material?> GetMaterialWithSizesByIdAsync(Guid materialId)
    {
        return await _context.Materials
            .Include(m => m.MaterialType)
            .Include(m => m.MaterialSizes)
                .ThenInclude(ms => ms.Size)
            .FirstOrDefaultAsync(m => m.Id == materialId);
    }
 
    // Leer todos los Materiales
    public async Task<List<Material>> GetAllMaterialsAsync()
    {
        return await _context.Materials
            .Include(m => m.MaterialSizes)
                .ThenInclude(ms => ms.Size) 
            .Include(m => m.MaterialType)
            .ToListAsync();
    }

    public IQueryable<Material> GetMaterialQuery()
    {
        return _context.Materials
            .Include(m => m.MaterialType)
            .Include(m => m.MaterialSizes)
                .ThenInclude(ms => ms.Size);
    }

    // Actualizar Material
    public async Task UpdateMaterialAsync(Material material)
    {
        _context.Materials.Update(material);
        await _context.SaveChangesAsync();
    }

    // Eliminar Material
    public async Task DeleteMaterialAsync(Guid id)
    {
        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (material != null)
        {
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<MaterialSize>> GetAllMaterialSizesAsync()
    {
        return await _context.MaterialSizes
            .Include(ms => ms.Material)
            .Include(ms => ms.Size)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MaterialSize>> GetMaterialSizesByMaterialIdWithLastPurchase(Guid materialId)
    {
        var materialSizes = await _context.MaterialSizes
            .Where(ms => ms.MaterialId == materialId)
            .Include(ms => ms.Size)
            .ToListAsync();

        foreach (var ms in materialSizes)
        {
            // Obtener la última compra de este material y talla en los ReceiptDetails
            var lastPurchase = await _context.ReceiptDetails
                .Where(rd => rd.MaterialId == materialId && rd.SizeId == ms.SizeId)
                .OrderByDescending(rd => rd.Receipt.ReceiptDate)
                .Select(rd => new
                {
                    LastCost = rd.UnitCost,
                    LastProviderId = rd.Receipt.ProviderId,
                    LastProviderName = rd.Receipt.Provider.BusinessName
                })
                .FirstOrDefaultAsync();

            // Si existe una compra previa, actualiza los valores
            if (lastPurchase != null)
            {
                ms.Cost = lastPurchase.LastCost;
                ms.LastProviderId = lastPurchase.LastProviderId;
                ms.LastProviderName = lastPurchase.LastProviderName;
            }
        }

        return materialSizes;
    }

    public async Task<MaterialSize?> GetMaterialSizeAsync(Guid materialId, int sizeId)
    {
        return await _context.MaterialSizes
            .Include(ms => ms.Material)
                .Include(ms => ms.Material.MaterialType)
            .Include(ms => ms.Size)
            .FirstOrDefaultAsync(ms => ms.MaterialId == materialId && ms.SizeId == sizeId);
    }

    public async Task UpdateMaterialSizeAsync(MaterialSize materialSize)
    {
        _context.MaterialSizes.Update(materialSize);
        await _context.SaveChangesAsync();
    }

    public async Task AddMaterialSizeAsync(MaterialSize materialSize)
    {
        _context.MaterialSizes.Add(materialSize);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MaterialType>> GetTypesByProviderAsync(Guid providerId)
    {
        return await _context.Receipts
            .Include(r => r.Details)
            .ThenInclude(d => d.Material)
            .ThenInclude(m => m.MaterialType)
            .Where(r => r.ProviderId == providerId)
            .SelectMany(r => r.Details.Select(d => d.Material.MaterialType))
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Material>> GetMaterialsByTypeAndProviderAsync(Guid materialTypeId, Guid providerId)
    {
        return await _context.Receipts
            .Include(r => r.Details)
            .ThenInclude(d => d.Material)
            .Where(r => r.ProviderId == providerId && r.Details.Any(d => d.Material.MaterialTypeId == materialTypeId))
            .SelectMany(r => r.Details.Select(d => d.Material))
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Material>> GetMaterialsByTypeAsync(Guid materialTypeId)
    {
        return await _context.Materials
            .Where(m => m.MaterialTypeId == materialTypeId)
            .ToListAsync();
    }

    public async Task<List<MaterialSize>> GetMaterialSizesByMaterialAsync(Guid materialId)
    {
        return await _context.MaterialSizes
            .Where(ms => ms.MaterialId == materialId)
            .Include(ms => ms.Size)  // Asegura que Size esté cargado
            .Where(ms => ms.Size != null) // Evita nulos en Size
            .ToListAsync();
    }

    public async Task<MaterialType?> GetMaterialTypeByMaterialIdAsync(Guid materialId)
    {
        return await _context.Materials
                    .Where(m => m.Id == materialId)
                    .Select(m => m.MaterialType)
                    .FirstOrDefaultAsync();
        
    }    

    public async Task UpdateMaterialSizeStockAsync (Guid materialId, int sizeId, int units )
    {
        var materialsize = await GetMaterialSizeAsync(materialId, sizeId);
        if (materialsize == null)
        {
            throw new InvalidOperationException($"No se encontró MaterialSize para MaterialId {materialId} y SizeId {sizeId}");
        }
        materialsize.Stock += units;
        await UpdateMaterialSizeAsync(materialsize); 
    }
}

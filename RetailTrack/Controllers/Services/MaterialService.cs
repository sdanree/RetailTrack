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

        public async Task<MaterialSize?> GetMaterialSizeAsync(Guid materialId, int sizeId)
        {
            return await _context.MaterialSizes
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

}

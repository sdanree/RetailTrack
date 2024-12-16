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

    // Leer todos los Materiales
    public async Task<List<Material>> GetAllMaterialsAsync()
    {
        return await _context.Materials
            .Include(m => m.MaterialType) // Incluye el MaterialType asociado
            .ToListAsync();
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

    public async Task<List<MaterialType>> GetAllMaterialTypesAsync()
    {
        return await _context.MaterialTypes.ToListAsync();
    }


}

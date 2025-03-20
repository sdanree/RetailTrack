using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models;

public class MaterialTypeService
{
    private readonly ApplicationDbContext _context;

    public MaterialTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InitializeMaterialTypesAsync(List<MaterialType> materialTypes)
    {
        _context.MaterialTypes.RemoveRange(_context.MaterialTypes);
        await _context.MaterialTypes.AddRangeAsync(materialTypes);
        await _context.SaveChangesAsync();
    }

    public async Task<MaterialType> GetMaterialTypeByIdAsync(Guid materialTypesId)
    {
        return await _context.MaterialTypes
                    .FirstOrDefaultAsync(mt => mt.Id == materialTypesId);
        
    }    
}

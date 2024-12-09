using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models.Products;

public class MaterialService
{
    private readonly ApplicationDbContext _context;

    public MaterialService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddMaterialsAsync(List<Material> materials)
    {
        await _context.Materials.AddRangeAsync(materials);
        await _context.SaveChangesAsync();
    }
}

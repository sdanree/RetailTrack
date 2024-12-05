using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models.Products;

namespace RetailTrack.Services
{
    public class DesignService
    {
        private readonly ApplicationDbContext _context;

        public DesignService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los diseños
        public async Task<List<Design>> GetAllDesignsAsync()
        {
            return await _context.Designs.ToListAsync() ?? new List<Design>();
        }

        // Obtener un diseño por ID
        public async Task<Design> GetDesignByIdAsync(Guid id)
        {
            return await _context.Designs.FirstOrDefaultAsync(d => d.Id == id);
        }

        // Agregar un nuevo diseño
        public async Task<bool> AddDesignAsync(Design design)
        {
            try
            {
                // Valida duplicados por nombre
                if (await _context.Designs.AnyAsync(d => d.Name == design.Name))
                {
                    throw new InvalidOperationException("Ya existe un diseño con el mismo nombre.");
                }

                await _context.Designs.AddAsync(design);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Loggear el error (esto depende de tu sistema de logging)
                Console.WriteLine($"Error al agregar diseño: {ex.Message}");
                return false;
            }
        }

        // Eliminar un diseño por ID
        public async Task<bool> DeleteDesignAsync(Guid id)
        {
            try
            {
                var design = await _context.Designs.FirstOrDefaultAsync(d => d.Id == id);
                if (design == null) return false;

                _context.Designs.Remove(design);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Loggear el error
                Console.WriteLine($"Error al eliminar diseño: {ex.Message}");
                return false;
            }
        }
    }
}

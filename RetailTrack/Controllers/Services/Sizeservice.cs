using RetailTrack.Data;
using RetailTrack.Models;
using Microsoft.EntityFrameworkCore;


namespace RetailTrack.Services
{
    public class SizeService
    {
        private readonly ApplicationDbContext _context;


        public SizeService(ApplicationDbContext context)
        {
            _context = context;
        }        

        public async Task<List<Size>> GetAllSizesAsync()
        {
            return await _context.Sizes.ToListAsync();
        }

        public async Task<Size?> GetSizeByIdAsync(int sizeId)
        {
            return await _context.Sizes
                .AsNoTracking() 
                .FirstOrDefaultAsync(s => s.Size_Id == sizeId);
        }
    }
}                
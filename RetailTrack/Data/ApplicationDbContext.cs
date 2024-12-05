using Microsoft.EntityFrameworkCore;
using RetailTrack.Models.Products;
using RetailTrack.Models;

namespace RetailTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Design> Designs { get; set; }
    }
}

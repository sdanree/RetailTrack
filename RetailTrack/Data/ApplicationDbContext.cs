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

        public DbSet<Design> Designs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }    
        public DbSet<Material> Materials { get; set; }
        public DbSet<Movement> Movements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre MaterialType y Material
            modelBuilder.Entity<Material>()
                .HasOne(m => m.MaterialType)
                .WithMany(mt => mt.Materials)
                .HasForeignKey(m => m.MaterialTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
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
        public DbSet<MaterialSize> MaterialSizes { get; set; } 
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<MovementType> MovementTypes { get; set; }       
        public DbSet<Receipt> Receipts { get; set; } 
        public DbSet<ReceiptDetail> ReceiptDetails { get; set; } 
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<ReceiptPayment> ReceiptPayments { get; set; }
        public DbSet<Provider> Providers {get;set;}
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrdersDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrdersStatus { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre MaterialType y Material
            modelBuilder.Entity<Material>()
                .HasOne(m => m.MaterialType)
                .WithMany(mt => mt.Materials)
                .HasForeignKey(m => m.MaterialTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación entre MaterialSize y Material
            modelBuilder.Entity<MaterialSize>()
                .HasKey(ms => new { ms.MaterialId, ms.SizeId }); // Clave compuesta

            modelBuilder.Entity<MaterialSize>()
                .HasOne(ms => ms.Material)
                .WithMany(m => m.MaterialSizes)
                .HasForeignKey(ms => ms.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación entre MaterialSize y Size
            modelBuilder.Entity<MaterialSize>()
                .HasOne(ms => ms.Size)
                .WithMany()
                .HasForeignKey(ms => ms.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la clave primaria en Receipt
            modelBuilder.Entity<Receipt>()
                .HasKey(gr => gr.ReceiptId);

            // Configurar la relación entre Receipt y ReceiptDetail
            modelBuilder.Entity<ReceiptDetail>()
                .HasKey(grd => new { grd.ReceiptId, grd.MaterialId }); // Clave compuesta

            modelBuilder.Entity<ReceiptDetail>()
                .HasOne(grd => grd.Receipt)
                .WithMany(gr => gr.Details)
                .HasForeignKey(grd => grd.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación entre ReceiptDetail y Material
            modelBuilder.Entity<ReceiptDetail>()
                .HasOne(grd => grd.Material)
                .WithMany()
                .HasForeignKey(grd => grd.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar la relación entre ReceiptDetail y Size
            modelBuilder.Entity<ReceiptDetail>()
                .HasOne(grd => grd.Size)
                .WithMany()
                .HasForeignKey(grd => grd.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceiptPayment>()
                .HasKey(rp => new { rp.ReceiptId, rp.PaymentMethodId });

            modelBuilder.Entity<ReceiptPayment>()
                .HasOne(rp => rp.Receipt)
                .WithMany(r => r.Payments)
                .HasForeignKey(rp => rp.ReceiptId);

            modelBuilder.Entity<ReceiptPayment>()
                .HasOne(rp => rp.PaymentMethod)
                .WithMany(pm => pm.Receipts)
                .HasForeignKey(rp => rp.PaymentMethodId);
        }
    }
}

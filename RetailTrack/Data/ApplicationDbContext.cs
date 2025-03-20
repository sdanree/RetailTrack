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
        public DbSet<Provider> Providers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Sincronizar los valores del Enum con la base de datos
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = (int)OrderStatusEnum.Pendiente, Name = "Pendiente" },
                new OrderStatus { Id = (int)OrderStatusEnum.EnProceso, Name = "En Proceso" },
                new OrderStatus { Id = (int)OrderStatusEnum.ProntoParaEntrega, Name = "Pronto Para Entrega" },
                new OrderStatus { Id = (int)OrderStatusEnum.Finalizado, Name = "Finalizado" }
            );

            modelBuilder.Entity<ProductStatus>().HasData(
                new ProductStatus { Status_Id = (int)ProductStatusEnum.Available, Status_Name = "Habilitado" },
                new ProductStatus { Status_Id = (int)ProductStatusEnum.OutStock, Status_Name = "Sin stock" },
                new ProductStatus { Status_Id = (int)ProductStatusEnum.Unavailable, Status_Name = "Descontinuado" }
            );


            // Relación entre Order y OrderStatus
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Order y OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre OrderDetail y Product
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Order y OrderPayment
            modelBuilder.Entity<OrderPayment>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderPayments)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre OrderPayment y PaymentMethod
            modelBuilder.Entity<OrderPayment>()
                .HasOne(op => op.PaymentMethod)
                .WithMany()
                .HasForeignKey(op => op.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Product y ProductStatus
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Status)
                .WithMany()
                .HasForeignKey(p => p.ProductStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Product y ProductStock
            modelBuilder.Entity<ProductStock>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.Variants) // Cada producto puede tener muchas variantes
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un producto, se eliminan sus variantes

            // Relación entre ProductStock y MaterialSize
            modelBuilder.Entity<ProductStock>()
                .HasOne(ps => ps.MaterialSize)
                .WithMany()
                .HasForeignKey(ps => new { ps.MaterialId, ps.SizeId })
                .OnDelete(DeleteBehavior.Restrict);
                                
            // Configuración de la relación entre MaterialType y Material
            modelBuilder.Entity<Material>()
                .HasOne(m => m.MaterialType)
                .WithMany(mt => mt.Materials)
                .HasForeignKey(m => m.MaterialTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación entre MaterialSize y Material
            modelBuilder.Entity<MaterialSize>()
                .HasKey(ms => new { ms.MaterialId, ms.SizeId });

            modelBuilder.Entity<MaterialSize>()
                .HasOne(ms => ms.Material)
                .WithMany(m => m.MaterialSizes)
                .HasForeignKey(ms => ms.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaterialSize>()
                .HasOne(ms => ms.Size)
                .WithMany()
                .HasForeignKey(ms => ms.SizeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la clave primaria en Receipt
            modelBuilder.Entity<Receipt>()
                .HasKey(gr => gr.ReceiptId);

            // Relación entre Receipt y ReceiptDetail
            modelBuilder.Entity<ReceiptDetail>()
                .HasKey(grd => new { grd.ReceiptId, grd.MaterialId });

            modelBuilder.Entity<ReceiptDetail>()
                .HasOne(grd => grd.Receipt)
                .WithMany(gr => gr.Details)
                .HasForeignKey(grd => grd.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceiptDetail>()
                .HasOne(grd => grd.Material)
                .WithMany()
                .HasForeignKey(grd => grd.MaterialId)
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

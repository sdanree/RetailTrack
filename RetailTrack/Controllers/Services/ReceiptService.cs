using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RetailTrack.Data;
using RetailTrack.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailTrack.Services
{
    public class ReceiptService
    {
        private readonly ApplicationDbContext _context;
        private readonly MaterialService _materialService;

        public ReceiptService(ApplicationDbContext context, MaterialService materialService)
        {
            _context = context;
            _materialService = materialService;
        }

        public async Task AddReceiptAsync(Receipt receipt, List<ReceiptDetail> details, List<ReceiptPayment> payments)
        {
            _context.Receipts.Add(receipt);

            foreach (var detail in details)
            {
                detail.ReceiptId = receipt.ReceiptId;
                _context.ReceiptDetails.Add(detail);

                var material = await _context.Materials.FindAsync(detail.MaterialId);
                if (material != null)
                {

                    _context.Materials.Update(material);
                }
            }

            foreach (var payment in payments)
            {
                payment.ReceiptId = receipt.ReceiptId;
                _context.ReceiptPayments.Add(payment);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<PaymentMethod>> GetAllPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        public async Task<List<Receipt>> GetAllReceiptsAsync()
        {
            return await _context.Receipts
                .Include(r => r.Provider)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Material)
                        .ThenInclude(m => m.MaterialType)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Size)
                .Include(r => r.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<List<Receipt>> GetReceiptsByMaterialIdAsync(Guid materialId)
        {
            return await _context.Receipts
                .Where(r => r.Details.Any(d => d.MaterialId == materialId))
                .Include(r => r.Provider)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Size)
                .ToListAsync();
        }

        public async Task<List<Provider>> GetProvidersByMaterialAsync(Guid materialId, int sizeId)
        {
            return await _context.Receipts
                .Where(r => r.Details.Any(d => d.MaterialId == materialId && d.SizeId == sizeId))
                .Select(r => r.Provider)
                .Distinct()
                .ToListAsync();
        }


        public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(int paymentMethodId)
        {
            return await _context.PaymentMethods.FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);
        }

        /// <summary>
        /// Inicia una nueva transacción de base de datos.
        /// </summary>
        /// <returns>Una instancia de IDbContextTransaction.</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Maneja la creación de un recibo, sus detalles, y pagos dentro de una transacción.
        /// </summary>
        /// <param name="receipt">Recibo a crear.</param>
        /// <param name="details">Lista de detalles del recibo.</param>
        /// <param name="payments">Lista de pagos asociados al recibo.</param>
        public async Task ProcessReceiptTransactionAsync(Receipt receipt, List<ReceiptDetail> details, List<ReceiptPayment> payments)
        {
            using (var transaction = await BeginTransactionAsync())
            {
                try
                {
                    // Agregar el recibo
                    _context.Receipts.Add(receipt);
                    await _context.SaveChangesAsync();

                    // Agregar los detalles y actualizar MaterialSizes
                    foreach (var detail in details)
                    {
                        detail.ReceiptId = receipt.ReceiptId;
                        _context.ReceiptDetails.Add(detail);

                        var materialSize = await _context.MaterialSizes
                            .FirstOrDefaultAsync(ms => ms.MaterialId == detail.MaterialId && ms.SizeId == detail.SizeId);

                        if (materialSize != null)
                        {
                            materialSize.Stock += detail.Quantity;
                            materialSize.Cost = detail.UnitCost;
                            _context.MaterialSizes.Update(materialSize);
                        }
                        else
                        {
                            // Crear nuevo MaterialSize si no existe
                            var newMaterialSize = new MaterialSize
                            {
                                Id = Guid.NewGuid(),
                                MaterialId = detail.MaterialId,
                                SizeId = detail.SizeId,
                                Stock = detail.Quantity,
                                Cost = detail.UnitCost
                            };
                            _context.MaterialSizes.Add(newMaterialSize);
                        }
                    }

                    // Agregar los pagos
                    foreach (var payment in payments)
                    {
                        payment.ReceiptId = receipt.ReceiptId;
                        _context.ReceiptPayments.Add(payment);
                    }

                    // Guardar cambios y confirmar transacción
                    await _context.SaveChangesAsync();
                    Console.WriteLine("################# Cambios guardados en la base de datos.");
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<MaterialSize?> GetMaterialSizeAsync(Guid materialId, int sizeId)
        {
            return await _context.MaterialSizes.FirstOrDefaultAsync(ms => ms.MaterialId == materialId && ms.SizeId == sizeId);
        }

        public async Task UpdateMaterialSizeAsync(MaterialSize materialSize)
        {
            _context.MaterialSizes.Update(materialSize);
            await _context.SaveChangesAsync();
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid receiptId)
        {
            return await _context.Receipts
                .Include(r => r.Provider)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Material)
                        .ThenInclude(m => m.MaterialType)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Size)
                .Include(r => r.Payments)
                    .ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);
        }

    }
}

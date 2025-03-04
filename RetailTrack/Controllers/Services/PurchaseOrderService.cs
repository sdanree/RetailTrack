using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Models;
using RetailTrack.ViewModels;

namespace RetailTrack.Services
{
    public class PurchaseOrderService
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrderIndexDetailViewModel>> GetPurchaseOrdersAsync(DateTime? startDate, DateTime? endDate, Guid? providerId, string status, int? purchaseOrderNumber)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.Provider)
                .Include(po => po.Details)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(po => po.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(po => po.OrderDate <= endDate.Value);

            if (providerId.HasValue)
                query = query.Where(po => po.ProviderId == providerId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<PurchaseOrderStatus>(status, out var statusEnum))
                query = query.Where(po => po.Status == statusEnum);

            if(purchaseOrderNumber.HasValue)            
                query = query.Where(po => po.PurchaseOrderNumber == purchaseOrderNumber);

            return await query.Select(po => new PurchaseOrderIndexDetailViewModel
            {
                PurchaseOrderId = po.PurchaseOrderId,
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                OrderDate = po.OrderDate,
                ProviderName = po.Provider.BusinessName,
                Status = po.Status.ToString(),
                TotalAmount = po.Details.Sum(d => d.Quantity * d.UnitCost)
            }).ToListAsync();
        }
    
        public async Task<PurchaseOrderDetailViewModel> GetPurchaseOrderByIdAsync(Guid id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Provider)
                .Include(po => po.Details)
                    .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == id);

            if (purchaseOrder == null)
                return null;

            return new PurchaseOrderDetailViewModel
            {
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber ?? 0,
                OrderDate = purchaseOrder.OrderDate,
                ProviderName = purchaseOrder.Provider.BusinessName,
                Status = purchaseOrder.Status.ToString(),
                Items = purchaseOrder.Details.Select(d => new PurchaseOrderItemViewModel
                {
                    MaterialId = d.MaterialId,
                    MaterialName = d.Material.Name,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost
                }).ToList()
            };
        }

        public async Task CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder == null || purchaseOrder.Details == null || !purchaseOrder.Details.Any())
                throw new ArgumentException("La orden de compra no contiene detalles válidos.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Agregar la orden de compra a la base de datos
                _context.PurchaseOrders.Add(purchaseOrder);

                // Guardar los cambios
                await _context.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PurchaseOrderForReceipIndexViewModel>> GetPurchaseOrdersByProviderAndStatusAsync(Guid? providerId, PurchaseOrderStatus status)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.Provider)
                .Include(po => po.Details)
                .AsQueryable();

            if (providerId.HasValue)
                query = query.Where(po => po.ProviderId == providerId.Value);

            query = query.Where(po => po.Status == status);

            return await query.Select(po => new PurchaseOrderForReceipIndexViewModel
            {
                PurchaseOrderId = po.PurchaseOrderId,
                PurchaseOrderNumber = po.PurchaseOrderNumber ?? 0,
                OrderDate = po.OrderDate,
                ProviderName = po.Provider.BusinessName,
                Status = po.Status.ToString(),
                TotalAmount = po.Details.Sum(d => d.Quantity * d.UnitCost)
            }).ToListAsync();
        }

        public async Task<int> GetLastPurchaseOrderNumberAsync()
        {
            return await _context.PurchaseOrders
                .Where(po => po.PurchaseOrderNumber.HasValue)
                .MaxAsync(po => (int?)po.PurchaseOrderNumber) ?? 0;
        }


        public async Task<PurchaseOrderDetailViewModel> GetPurchaseOrderByNumberAsync(int number)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Provider)
                .Include(po => po.Details)
                    .ThenInclude(d => d.Material)
                .FirstOrDefaultAsync(po => po.PurchaseOrderNumber == number);

            if (purchaseOrder == null)
                return null;

            return new PurchaseOrderDetailViewModel
            {
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber ?? 0,
                OrderDate = purchaseOrder.OrderDate,
                ProviderName = purchaseOrder.Provider.BusinessName,
                Status = purchaseOrder.Status.ToString(),
                Items = purchaseOrder.Details.Select(d => new PurchaseOrderItemViewModel
                {
                    MaterialId = d.MaterialId,
                    MaterialName = d.Material.Name,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost
                }).ToList()
            };                
        }

    }
}

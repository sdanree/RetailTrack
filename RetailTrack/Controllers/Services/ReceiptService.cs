using Microsoft.EntityFrameworkCore;
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

        public ReceiptService(ApplicationDbContext context)
        {
            _context = context;
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
//                    material.Stock += detail.Quantity;
//                    material.Cost = detail.UnitCost;
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
                .Include(r => r.Payments)
                    .ThenInclude(rp => rp.PaymentMethod) 
                .Include(r => r.Details)
                    .ThenInclude(d => d.Material)
                .Include(r => r.Details)
                    .ThenInclude(d => d.Size)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(int paymentMethodId)
        {
            return await _context.PaymentMethods.FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);
        }


    }
}

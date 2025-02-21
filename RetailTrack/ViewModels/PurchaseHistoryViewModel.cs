using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailTrack.ViewModels

{
    public class PurchaseHistoryViewModel
    {
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public List<ReceiptIndexDetailViewModel> Receipts { get; set; } = new List<ReceiptIndexDetailViewModel>();
    }


    public class PurchaseHistoryReceiptViewModel
    {
        public Guid ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Provider { get; set; }
        public List<PurchaseHistoryDetailViewModel> Details { get; set; } = new List<PurchaseHistoryDetailViewModel>();
    }

    public class PurchaseHistoryDetailViewModel
    {
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
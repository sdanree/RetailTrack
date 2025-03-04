using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetailTrack.ViewModels
{
    public class PurchaseOrderIndexViewModel
    {
        public Guid PurchaseOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PurchaseOrderDetailViewModel
    {
        public Guid PurchaseOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public List<PurchaseOrderItemViewModel> Items { get; set; } = new List<PurchaseOrderItemViewModel>();
        public List<ReceiptSummaryViewModel> AssociatedReceipts { get; set; } = new List<ReceiptSummaryViewModel>();
    }

    public class PurchaseOrderItemViewModel
    {
        public Guid MaterialId { get; set; }
        public string MaterialName { get; set; }
        public Guid MaterialTypeId {get; set;}
        public string MaterialTypeName {get;set;}
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost => Quantity * UnitCost;
        public int SizeId { get; set; }
        public string SizeName { get; set; }      
        public Guid ProviderId { get; set; } 
        public string ProviderName { get; set; }  
    }

    public class PurchaseOrderCreateViewModel
    {
        public DateTime OrderDate { get; set; }
        public Guid ProviderId { get; set; }
        public List<PurchaseOrderItemViewModel> Items { get; set; } = new List<PurchaseOrderItemViewModel>();
        public List<SelectListItem> MaterialTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Providers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Materials { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
    }

    public class ReceiptSummaryViewModel
    {
        public Guid ReceiptId { get; set; }
        public string ReceiptExternalCode { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PurchaseOrderForReceipIndexViewModel
    {
        public Guid PurchaseOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }    
}
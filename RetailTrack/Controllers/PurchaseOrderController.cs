using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.ViewModels;
using RetailTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    [Authorize(Roles = "UserApproved")]
    public class PurchaseOrderController : Controller
    {
        private readonly PurchaseOrderService _purchaseOrderService;
        private readonly MaterialService _materialService;
        private readonly ReceiptService _receiptService;
        private readonly ProductService _productService;
        private readonly SizeService _sizeService;
        private readonly ProviderService _providerService;        

        public PurchaseOrderController(PurchaseOrderService purchaseOrderService, MaterialService materialService, ReceiptService receiptService, ProductService productService, SizeService sizeService, ProviderService providerService)
        {
            _purchaseOrderService   = purchaseOrderService;
            _materialService        = materialService;
            _receiptService         = receiptService;
            _productService         = productService;
            _sizeService            = sizeService;
            _providerService        = providerService;            
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, Guid? providerId, string status, int? purchaseOrderNumber)
        {
            var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersAsync(startDate, endDate, providerId, status, purchaseOrderNumber);

            // Obtener la lista de proveedores para el dropdown
            var providersList = await _providerService.GetAllProvidersAsync();

            // Construcción del ViewModel
            var viewModel = new PurchaseOrderIndexViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                SelectedProviderId = providerId,
                SelectedStatus = status,
                SelectedPurchaseOrderNumber = purchaseOrderNumber,
                Providers = providersList.Select(pv => new SelectListItem
                {
                    Value = pv.Id.ToString(),
                    Text  = pv.BusinessName
                }),
                PurchaseOrders = purchaseOrders.Select(po => new PurchaseOrderIndexDetailViewModel
                {
                    PurchaseOrderId = po.PurchaseOrderId,
                    PurchaseOrderNumber = po.PurchaseOrderNumber,
                    OrderDate = po.OrderDate,
                    ProviderName = po.ProviderName,
                    Status = po.Status,
                    TotalAmount = po.TotalAmount
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var providers = await _providerService.GetAllProvidersAsync();
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            
            var viewModel = new PurchaseOrderCreateViewModel
            {
                OrderDate = DateTime.Now,
                Providers = providers.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.BusinessName
                }).ToList(),
                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTypesByProvider(Guid providerId)
        {
            var materialTypes = await _materialService.GetTypesByProviderAsync(providerId);
            return Json(materialTypes.Select(mt => new { mt.Id, mt.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialsByType(Guid materialTypeId)
        {
            var materials = await _materialService.GetMaterialsByTypeAsync(materialTypeId);
            return Json(materials.Select(m => new { m.Id, m.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetSizesByMaterial(Guid materialId)
        {
            var materialSizes = await _materialService.GetMaterialSizesByMaterialIdWithLastPurchase(materialId);
            Console.WriteLine(JsonSerializer.Serialize(materialSizes));
            return Json(materialSizes.Select(ms => new
            {
                ms.SizeId,
                ms.Size.Size_Name,
                ms.Cost,
                ms.LastProviderId,
                ms.LastProviderName
            }));
        }

        /// <summary>
        /// Obtiene los proveedores a los que se les ha comprado un material.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProvidersByMaterial(Guid materialId, int sizeId)
        {
            var providers = await _receiptService.GetProvidersByMaterialAsync(materialId, sizeId);
            return Json(providers.Select(p => new { p.Id, p.BusinessName }));
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddItem([FromBody] PurchaseOrderItemViewModel newItem)
        {
            if (newItem == null || newItem.MaterialId == Guid.Empty || newItem.SizeId == 0 || newItem.Quantity <= 0 || newItem.UnitCost <= 0 || newItem.ProviderId == Guid.Empty)
            {
                return Json(new { success = false, message = "Datos inválidos para el material." });
            }

            var sessionItems = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderItemViewModel>>("PurchaseOrderItems") ?? new List<PurchaseOrderItemViewModel>();

            var existingItem = sessionItems.FirstOrDefault(i => i.MaterialId == newItem.MaterialId && i.SizeId == newItem.SizeId && i.ProviderId == newItem.ProviderId);

            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                sessionItems.Add(new PurchaseOrderItemViewModel
                {
                    MaterialId = newItem.MaterialId,
                    MaterialName = newItem.MaterialName,
                    MaterialTypeId = newItem.MaterialTypeId,
                    MaterialTypeName = newItem.MaterialTypeName,
                    SizeId = newItem.SizeId,
                    SizeName = newItem.SizeName,
                    Quantity = newItem.Quantity,
                    UnitCost = newItem.UnitCost,
                    ProviderId = newItem.ProviderId,
                    ProviderName = newItem.ProviderName
                });
            }

            HttpContext.Session.SetObjectAsJson("PurchaseOrderItems", sessionItems);

            return Json(new { success = true, items = sessionItems });
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePurchaseOrders()
        {
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderItemViewModel>>("PurchaseOrderItems");

            if (sessionItems == null || !sessionItems.Any())
            {
                return Json(new { success = false, message = "No hay materiales agregados para generar órdenes de compra." });
            }
            var NewpurchaseOrderNumber = await _purchaseOrderService.GetLastPurchaseOrderNumberAsync() + 1;

            Console.WriteLine($"ultimo nro de oreden = {NewpurchaseOrderNumber}");

            var groupedOrders = sessionItems.GroupBy(item => item.ProviderId)
                .Select(group => new PurchaseOrder
                {
                    PurchaseOrderId = Guid.NewGuid(),
                    OrderDate = DateTime.Now,
                    PurchaseOrderNumber = NewpurchaseOrderNumber ,
                    ProviderId = group.Key,
                    Status = PurchaseOrderStatus.Pending,
                    Details = group.Select(item => new PurchaseOrderDetail
                    {
                        DetailId = Guid.NewGuid(),
                        MaterialId = item.MaterialId,
                        SizeId = item.SizeId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost
                    }).ToList()
                }).ToList();

            foreach (var order in groupedOrders)
            {
                await _purchaseOrderService.CreatePurchaseOrderAsync(order);
            }

            // Limpiar la sesión después de generar las órdenes
            HttpContext.Session.Remove("PurchaseOrderItems");

            return Json(new { success = true, message = "Órdenes de compra generadas exitosamente." });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound(); // Retorna 404 si no encuentra la orden de compra
            }

            return View(purchaseOrder);
        }
    }
}

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
using Newtonsoft.Json;

namespace RetailTrack.Controllers
{
    public class GoodsReceiptController : Controller
    {
        private readonly MaterialService _materialService;
        private readonly ReceiptService _Receiptservice;
        private readonly ProductService _productService;
        private readonly SizeService _sizeService;

        public GoodsReceiptController(MaterialService materialService, ReceiptService Receiptservice, ProductService productService, SizeService sizeService)
        {
            _materialService    = materialService;
            _Receiptservice     = Receiptservice;
            _productService     = productService;
            _sizeService        = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = await BuildCreateViewModel();
            // Recuperar items de la sesión si ya existen
            viewModel.Items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ReceiptViewModel viewModel)
        {
            var material = await _materialService.GetMaterialByIdAsync(viewModel.NewItem.MaterialId);
            if (material == null)
            {
                ModelState.AddModelError("NewItem.MaterialId", "Material no válido.");
            }
            else
            {
                var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();

                var selectedMaterialType    = await _productService.GetMaterialTypeByIdAsync(material.MaterialTypeId);
                var selectedSize            = await _sizeService.GetSizeByIdAsync(viewModel.NewItem.SizeId);

                var newItem = new ReceiptDetailViewModel
                {
                    MaterialId          = material.Id,
                    MaterialName        = material.Name,
                    MaterialTypeName    = selectedMaterialType?.Name ?? "N/A",
                    SizeId              = selectedSize?.Size_Id ?? 0,
                    SizeName            = selectedSize?.Size_Name ?? "N/A",
                    Quantity            = viewModel.NewItem.Quantity,
                    UnitCost            = viewModel.NewItem.UnitCost
                };

                sessionItems.Add(newItem);

                HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);
                viewModel.Items = sessionItems;
            }

            await RecargarListas(viewModel);
            return View("Create", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReceiptViewModel viewModel)
        {
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            if (!sessionItems.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un material.");
                await RecargarListas(viewModel);
                viewModel.Items = sessionItems;
                return View(viewModel);
            }

            if (!sessionPayments.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un método de pago.");
                await RecargarListas(viewModel);
                viewModel.Payments = sessionPayments;
                return View(viewModel);
            }

            var goodsReceipt = new Receipt
            {
                ReceiptDate = DateTime.Now
            };

            var details = sessionItems.Select(item => new ReceiptDetail
            {
                MaterialId = item.MaterialId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                SizeId = item.SizeId
            }).ToList();

            var payments = sessionPayments.Select(p => new ReceiptPayment
            {
                PaymentMethodId = p.PaymentMethodId,
                Percentage = p.Percentage / 100
            }).ToList();

            await _Receiptservice.AddReceiptAsync(goodsReceipt, details, payments);

            HttpContext.Session.Remove("ReceiptItems");
            HttpContext.Session.Remove("ReceiptPayments");

            TempData["Message"] = "Ingreso de materiales registrado con éxito.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? paymentMethodId)
        {
            var Receipts = await _Receiptservice.GetAllReceiptsAsync();

            if (startDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate <= endDate.Value).ToList();

            if (paymentMethodId.HasValue)
                Receipts = Receipts.Where(gr => gr.Payments.Any(p => p.PaymentMethodId == paymentMethodId.Value)).ToList();

            var paymentMethods = await _Receiptservice.GetAllPaymentMethodsAsync();

            var viewModel = new ReceiptIndexViewModel
            {
                Receipts = Receipts.Select(gr => new ReceiptIndexDetailViewModel
                {
                    ReceiptId = gr.ReceiptId,
                    ReceiptDate = gr.ReceiptDate,
                    PaymentMethods = string.Join(", ", gr.Payments.Select(p => p.PaymentMethod.Name)),
                    Details = gr.Details.Select(detail => new ReceiptDetailViewModel
                    {
                        MaterialId = detail.MaterialId,
                        MaterialName = detail.Material.Name,
                        MaterialTypeName = detail.Material.MaterialType.Name,
                        SizeId = detail.SizeId,
                        SizeName = detail.Size.Size_Name,
                        Quantity = detail.Quantity,
                        UnitCost = detail.UnitCost
                    }).ToList()
                }).ToList(),
                PaymentMethods = paymentMethods.Select(pm => new SelectListItem
                {
                    Value = pm.PaymentMethodId.ToString(),
                    Text = pm.Name
                }),
                StartDate = startDate,
                EndDate = endDate,
                SelectedPaymentMethod = paymentMethodId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(ReceiptViewModel viewModel)
        {
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            var selectedPaymentMethod = await _Receiptservice.GetPaymentMethodByIdAsync(viewModel.NewPayment.PaymentMethodId);

            if (selectedPaymentMethod == null)
            {
                ModelState.AddModelError("NewPayment.PaymentMethodId", "Método de pago no válido.");
            }
            else
            {
                // Verificar si el porcentaje total supera el 100%
                var totalPercentage = sessionPayments.Sum(p => p.Percentage) + viewModel.NewPayment.Percentage;
                if (totalPercentage > 100)
                {
                    ModelState.AddModelError("NewPayment.Percentage", "El porcentaje total no puede superar el 100%.");
                }
                else
                {
                    var newPayment = new ReceiptPaymentViewModel
                    {
                        PaymentMethodId = selectedPaymentMethod.PaymentMethodId,
                        PaymentMethodName = selectedPaymentMethod.Name,
                        Percentage = viewModel.NewPayment.Percentage
                    };

                    sessionPayments.Add(newPayment);
                    HttpContext.Session.SetObjectAsJson("ReceiptPayments", sessionPayments);

                    viewModel.Payments = sessionPayments;
                }
            }

            await RecargarListas(viewModel);
            return View("Create", viewModel);
        }


        private async Task RecargarListas(ReceiptViewModel viewModel)
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var paymentMethods  = await _Receiptservice.GetAllPaymentMethodsAsync();
            var sizes           = await _Receiptservice.GetAllSizesAsync();

            viewModel.MaterialTypes     = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            viewModel.PaymentMethods    = paymentMethods.Select(pm => new SelectListItem { Value = pm.PaymentMethodId.ToString(), Text = pm.Name });
            viewModel.Sizes             = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });

            viewModel.Items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
        }

        private async Task<ReceiptViewModel> BuildCreateViewModel()
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var paymentMethods  = await _Receiptservice.GetAllPaymentMethodsAsync();
            var sizes           = await _Receiptservice.GetAllSizesAsync();

            return new ReceiptViewModel
            {
                MaterialTypes   = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name }),
                Materials       = new List<SelectListItem>(),
                PaymentMethods  = paymentMethods.Select(pm => new SelectListItem { Value = pm.PaymentMethodId.ToString(), Text = pm.Name }),
                Sizes           = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name }),
                Items           = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>()
            };
        }
    }
}

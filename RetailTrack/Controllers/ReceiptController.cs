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


namespace RetailTrack.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly MaterialService _materialService;
        private readonly ReceiptService _receiptService;
        private readonly ProductService _productService;
        private readonly SizeService _sizeService;

        public ReceiptController(MaterialService materialService, ReceiptService receiptService, ProductService productService, SizeService sizeService)
        {
            _materialService    = materialService;
            _receiptService     = receiptService;
            _productService     = productService;
            _sizeService        = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? paymentMethodId)
        {
            var Receipts = await _receiptService.GetAllReceiptsAsync();

            if (startDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate <= endDate.Value).ToList();

            if (paymentMethodId.HasValue)
                Receipts = Receipts.Where(gr => gr.Payments.Any(p => p.PaymentMethodId == paymentMethodId.Value)).ToList();

            var paymentMethods = await _receiptService.GetAllPaymentMethodsAsync();

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var materials       = await _materialService.GetAllMaterialsAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();  
            var paymentTypes    = await _receiptService.GetAllPaymentMethodsAsync();
            
            var viewModel = new ReceiptCreateViewModel
            {
                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value   = mt.Id.ToString(),
                    Text    = mt.Name
                }),
                Materials = materials.Select(m => new SelectListItem
                {
                    Value   = m.Id.ToString(),
                    Text    = m.Name
                }),
                Sizes = sizes.Select(s => new SelectListItem
                {
                    Value   = s.Size_Id.ToString(),
                    Text    = s.Size_Name
                }),
                PaymentMethods = paymentTypes.Select(pm => new SelectListItem
                {
                    Value = pm.PaymentMethodId.ToString(),
                    Text = pm.Name
                }),
                Payments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>(),
                Items = (HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>())
                        .Select(item => new ReceiptItemViewModel
                        {
                            MaterialTypeName = item.MaterialTypeName,
                            MaterialName = item.MaterialName,
                            SizeName = item.SizeName,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost
                        }).ToList(),
            };
  
            // Cargar valores seleccionados desde la sesión
            if (HttpContext.Session.GetString("SelectedMaterialType") != null)
            {
                viewModel.SelectedMaterialType  = Guid.Parse(HttpContext.Session.GetString("SelectedMaterialType"));
                viewModel.SelectedMaterial      = Guid.Parse(HttpContext.Session.GetString("SelectedMaterial"));
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ReceiptViewModel viewModel)
        {
            var sessionData = HttpContext.Session.GetString("ReceiptItems");
        
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();

            var material = await _materialService.GetMaterialByIdAsync(viewModel.NewItem.MaterialId);
            if (material == null)
            {
                ModelState.AddModelError("NewItem.MaterialId", "Material no válido.");
            }
            else
            {
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

                // Guardar en la sesión actualizada
                HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);

                // Mostrar nuevamente el JSON actualizado en la consola
                var updatedJson = JsonSerializer.Serialize(sessionItems);
                Console.WriteLine("Contenido actualizado de ReceiptItems en lita: " + updatedJson);
            }

            await RecargarListas(viewModel);

            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            // Convertir ReceiptViewModel a ReceiptCreateViewModel
            var createViewModel = new ReceiptCreateViewModel
            {
                MaterialTypes   = viewModel.MaterialTypes,
                Materials       = viewModel.Materials,
                Sizes           = viewModel.Sizes,
                PaymentMethods  = viewModel.PaymentMethods,                
                Payments        = sessionPayments,                
                Items = viewModel.Items.Select(i => new ReceiptItemViewModel
                {
                    MaterialTypeName = i.MaterialTypeName,
                    MaterialName = i.MaterialName,
                    SizeName = i.SizeName,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost
                }).ToList(),
                SelectedMaterialType    = viewModel.SelectedMaterialType,
                SelectedMaterial        = viewModel.SelectedMaterial,
                SelectedSize            = viewModel.SelectedSize
            };

            return View("Create", createViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddMaterial(MaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error al intentar agregar el material. Verifique los datos.";

                // Guardar valores actuales en sesión
                HttpContext.Session.SetString("SelectedMaterialType", model.MaterialTypeId.ToString());
                HttpContext.Session.SetString("SelectedMaterial", model.Name);

                return RedirectToAction("Create");
            }

            var material = new Material
            {
                Id              = Guid.NewGuid(),
                Name            = model.Name,
                MaterialTypeId  = model.MaterialTypeId
            };

            await _materialService.AddMaterialAsync(material);

            // Guardar en sesión el material y tipo seleccionados
            if (material.Id != Guid.Empty)
            {
                HttpContext.Session.SetString("SelectedMaterialType", model.MaterialTypeId.ToString());
                HttpContext.Session.SetString("SelectedMaterial", material.Id.ToString());
            }
            else{
               Console.WriteLine("El ID del material es inválido.");
                TempData["ErrorMessage"] = "Hubo un error al generar el material. Intente nuevamente.";
                return RedirectToAction("Create");                
            }

            return Json(new { success = true, materialTypeId = model.MaterialTypeId, materialId = material.Id, materialName = material.Name });
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(ReceiptViewModel viewModel)
        {
            // Obtener la lista de pagos desde la sesión
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            // Validar el nuevo método de pago
            var paymentMethod = await _receiptService.GetPaymentMethodByIdAsync(viewModel.NewPayment.PaymentMethodId);
            if (paymentMethod == null)
            {
                ModelState.AddModelError("NewPayment.PaymentMethodId", "Método de pago no válido.");
            }
            else
            {
                var newPayment = new ReceiptPaymentViewModel
                {
                    PaymentMethodId     = paymentMethod.PaymentMethodId,
                    PaymentMethodName   = paymentMethod.Name,
                    Percentage          = viewModel.NewPayment.Percentage,
                    Amount              = viewModel.NewPayment.Amount
                };

                sessionPayments.Add(newPayment);
                HttpContext.Session.SetObjectAsJson("ReceiptPayments", sessionPayments);
            }

            await RecargarListas(viewModel);

            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();

            var createViewModel = new ReceiptCreateViewModel
            {
                MaterialTypes   = viewModel.MaterialTypes,
                Materials       = viewModel.Materials,
                Sizes           = viewModel.Sizes,
                PaymentMethods  = viewModel.PaymentMethods,
                Payments        = sessionPayments,
                Items           = viewModel.Items.Select(i => new ReceiptItemViewModel
                {
                    MaterialTypeName = i.MaterialTypeName,
                    MaterialName = i.MaterialName,
                    SizeName = i.SizeName,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost
                }).ToList(),
                SelectedMaterialType    = viewModel.SelectedMaterialType,
                SelectedMaterial        = viewModel.SelectedMaterial,
                SelectedSize            = viewModel.SelectedSize
            };

            return View("Create", createViewModel);
        }

        private async Task RecargarListas(ReceiptViewModel viewModel)
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var paymentMethods  = await _receiptService.GetAllPaymentMethodsAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();

            viewModel.MaterialTypes     = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            viewModel.PaymentMethods    = paymentMethods.Select(pm => new SelectListItem { Value = pm.PaymentMethodId.ToString(), Text = pm.Name });
            viewModel.Sizes             = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });

            viewModel.Items     = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            viewModel.Payments  = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();
        }

        private async Task RecargarMaterialForm(MaterialViewModel model)
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();

            model.MaterialTypes = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            model.Sizes         = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });
        }

    }
}

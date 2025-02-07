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
        private readonly ProviderService _providerService;

        public ReceiptController(MaterialService materialService, ReceiptService receiptService, ProductService productService, SizeService sizeService, ProviderService providerService)
        {
            _materialService    = materialService;
            _receiptService     = receiptService;
            _productService     = productService;
            _sizeService        = sizeService;
            _providerService    = providerService;
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
        public IActionResult Create()
        {
            var viewModel = new ReceiptCreateViewModel
            {
                SelectedProviderDetails = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails"),
                //Items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>(),
                Items = (HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>())
                        .Select(item => new ReceiptItemViewModel
                        {
                            MaterialId = item.MaterialId,
                            SizeId = item.SizeId,
                            MaterialTypeName = item.MaterialTypeName,
                            MaterialName = item.MaterialName,
                            SizeName = item.SizeName,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost
                        }).ToList(),
                Payments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>(),
                Providers = _providerService.GetAllProvidersAsync().Result.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }),
                MaterialTypes = _materialService.GetAllMaterialTypesAsync().Result.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }),
                Sizes = _sizeService.GetAllSizesAsync().Result.Select(s => new SelectListItem
                {
                    Value = s.Size_Id.ToString(),
                    Text = s.Size_Name
                }),
                PaymentMethods = _receiptService.GetAllPaymentMethodsAsync().Result.Select(pm => new SelectListItem
                {
                    Value = pm.PaymentMethodId.ToString(),
                    Text = pm.Name
                }),
            };

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
            var selectedProvider = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails");

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

            if (selectedProvider != null)
            {
                createViewModel.ProviderId = selectedProvider.Id;
                createViewModel.SelectedProviderDetails = selectedProvider;
            }
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
            var selectedProvider = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails");

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
                SelectedSize            = viewModel.SelectedSize,
                ProviderId              = selectedProvider.Id,
                SelectedProviderDetails = selectedProvider
            };

            return View("Create", createViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProvider(Guid providerId)
        {
            if (providerId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un proveedor válido.";
                return RedirectToAction("Create");
            }

            try
            {
                var provider = await _providerService.GetProviderByIdAsync(providerId);
                if (provider == null)
                {
                    TempData["ErrorMessage"] = "Proveedor no encontrado.";
                    return RedirectToAction("Create");
                }

                // Guardar el proveedor completo en sesión
                HttpContext.Session.SetObjectAsJson("SelectedProviderDetails", provider);

                TempData["SuccessMessage"] = "Proveedor asociado correctamente.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asociar el proveedor: {ex.Message}");
                TempData["ErrorMessage"] = "Ocurrió un error al asociar el proveedor. Intente nuevamente.";
            }

            return RedirectToAction("Create");
        }


        [HttpGet]
        public async Task<IActionResult> GetProviderDetails(Guid providerId)
        {
            System.Console.WriteLine($"providerId {providerId}");

            var provider = await _providerService.GetProviderByIdAsync(providerId);
            
            // Loggear el payload para depuración
            var providerJson = System.Text.Json.JsonSerializer.Serialize(provider, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine("provider encontrado:");
            Console.WriteLine(providerJson);
            return Json(new
            {
                provider.Name,
                provider.Address,
                provider.Phone,
                provider.BusinessName,
                provider.RUT
            });
        }

        private async Task RecargarListas<T>(T viewModel) where T : class
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var paymentMethods  = await _receiptService.GetAllPaymentMethodsAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();

            if (viewModel is ReceiptViewModel receiptViewModel)
            {
                receiptViewModel.MaterialTypes  = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
                receiptViewModel.PaymentMethods = paymentMethods.Select(pm => new SelectListItem { Value = pm.PaymentMethodId.ToString(), Text = pm.Name });
                receiptViewModel.Sizes          = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });
                receiptViewModel.Items          = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
                receiptViewModel.Payments       = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();
            }
            else if (viewModel is ReceiptCreateViewModel createViewModel)
            {
                createViewModel.MaterialTypes   = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
                createViewModel.Materials       = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
                createViewModel.Sizes           = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });
                createViewModel.Items           = HttpContext.Session.GetObjectFromJson<List<ReceiptItemViewModel>>("ReceiptItems") ?? new List<ReceiptItemViewModel>();
                createViewModel.Payments        = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();
                
                var selectedProvider = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails");
                if (selectedProvider != null)
                {
                    createViewModel.ProviderId              = selectedProvider.Id;
                    createViewModel.SelectedProviderDetails = selectedProvider; // Asignar detalles completos del proveedor
                }

            }
        }

        private async Task RecargarMaterialForm(MaterialViewModel model)
        {
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();

            model.MaterialTypes = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            model.Sizes         = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt(ReceiptCreateViewModel viewModel)
        {
            // Validar que el modelo sea válido
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hubo un error en los datos proporcionados. Por favor, revise e intente nuevamente.";
                await RecargarListas(viewModel); // Asegurar que las listas necesarias se recarguen
                return View("Create", viewModel);
            }

            // Recuperar datos necesarios desde la sesión
            var provider = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails");
            var details = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            var payments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            // Validaciones adicionales
            if (provider == null)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un proveedor antes de registrar la orden.";
                await RecargarListas(viewModel);
                return View("Create", viewModel);
            }

            if (!details.Any())
            {
                TempData["ErrorMessage"] = "Debe agregar al menos un material antes de registrar la orden.";
                await RecargarListas(viewModel);
                return View("Create", viewModel);
            }

            if (!payments.Any())
            {
                TempData["ErrorMessage"] = "Debe agregar al menos un método de pago antes de registrar la orden.";
                await RecargarListas(viewModel);
                return View("Create", viewModel);
            }

            try
            {
                // Crear el objeto Receipt
                var receipt = new Receipt
                {
                    ReceiptId = Guid.NewGuid(),
                    ReceiptDate = DateTime.Now,
                    ProviderId = provider.Id
                };

                // Mapear detalles
                var mappedDetails = details.Select(item => new ReceiptDetail
                {
                    ReceiptId = receipt.ReceiptId,
                    MaterialId = item.MaterialId,
                    SizeId = item.SizeId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList();

                // Mapear pagos
                var mappedPayments = payments.Select(payment => new ReceiptPayment
                {
                    ReceiptId = receipt.ReceiptId,
                    PaymentMethodId = payment.PaymentMethodId,
                    Amount = payment.Amount,
                    Percentage = payment.Percentage
                }).ToList();

                // Procesar la transacción
                await _receiptService.ProcessReceiptTransactionAsync(receipt, mappedDetails, mappedPayments);

                // Limpiar datos de sesión
                HttpContext.Session.Remove("SelectedProviderDetails");
                HttpContext.Session.Remove("ReceiptItems");
                HttpContext.Session.Remove("ReceiptPayments");

                TempData["SuccessMessage"] = "Orden de compra registrada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al registrar la orden de compra: {ex.Message}";
                await RecargarListas(viewModel);
                return View("Create", viewModel);
            }
        }

        [HttpPost]
        public IActionResult ClearReceiptSession()
        {
            HttpContext.Session.Remove("ReceiptItems");
            HttpContext.Session.Remove("ReceiptPayments");
            HttpContext.Session.Remove("SelectedProvider");
            HttpContext.Session.Remove("SelectedProviderDetails");

            return Json(new { success = true });
        }





    }
}

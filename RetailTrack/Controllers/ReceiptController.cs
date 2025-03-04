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
        private readonly PurchaseOrderService _purchaseOrderService;

        public ReceiptController(MaterialService materialService, ReceiptService receiptService, ProductService productService, SizeService sizeService, ProviderService providerService, PurchaseOrderService purchaseOrderService)
        {
            _materialService        = materialService;
            _receiptService         = receiptService;
            _productService         = productService;
            _sizeService            = sizeService;
            _providerService        = providerService;
            _purchaseOrderService   = purchaseOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? paymentMethodId, Guid? InproviderId, string? InExternalCode)
        {
            var Receipts = await _receiptService.GetAllReceiptsAsync();

            if (startDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                Receipts = Receipts.Where(gr => gr.ReceiptDate <= endDate.Value).ToList();

            if (paymentMethodId.HasValue)
                Receipts = Receipts.Where(gr => gr.Payments.Any(p => p.PaymentMethodId == paymentMethodId.Value)).ToList();

            if (InproviderId.HasValue)
                Receipts = Receipts.Where(gr => gr.Provider != null && gr.Provider.Id == InproviderId.Value).ToList();
             
            if (!string.IsNullOrEmpty(InExternalCode))
            {
                Receipts = Receipts
                    .Where(gr => !string.IsNullOrWhiteSpace(gr.ReceiptExternalCode) && gr.ReceiptExternalCode.Contains(InExternalCode))
                    .ToList();
            }

            var paymentMethods = await _receiptService.GetAllPaymentMethodsAsync();
            
            var providersList = await _providerService.GetAllProvidersAsync();

            var viewModel = new ReceiptIndexViewModel
            {
                Receipts = Receipts.Select(gr => new ReceiptIndexDetailViewModel
                {
                    ReceiptId       = gr.ReceiptId,
                    ReceiptDate     = gr.ReceiptDate,
                    ProviderName    = gr.Provider?.BusinessName ?? "Sin Proveedor",
                    ExternalCode    = gr.ReceiptExternalCode ?? "",
                    PaymentMethods  = string.Join(", ", gr.Payments.Select(p => p.PaymentMethod.Name)),
                    Details         = gr.Details.Select(detail => new ReceiptDetailViewModel
                    {
                        MaterialId          = detail.MaterialId,
                        MaterialName        = detail.Material.Name,
                        MaterialTypeName    = detail.Material.MaterialType.Name,
                        SizeId              = detail.SizeId,
                        SizeName            = detail.Size.Size_Name,
                        Quantity            = detail.Quantity,
                        UnitCost            = detail.UnitCost
                    }).ToList()
                }).ToList(),
                PaymentMethods = paymentMethods.Select(pm => new SelectListItem
                {
                    Value = pm.PaymentMethodId.ToString(),
                    Text  = pm.Name
                }),
                Providers = providersList.Select(pv => new SelectListItem
                {
                    Value = pv.Id.ToString(),
                    Text  = pv.BusinessName
                }),
                StartDate               = startDate,
                EndDate                 = endDate,
                SelectedPaymentMethod   = paymentMethodId,
                SelectedProvider        = InproviderId,
                SelectedExternalCode    = InExternalCode
            };

            return View(viewModel);
        }        

        [HttpGet]
        public IActionResult Create()
        {
            var items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            var receiptExternalCode = HttpContext.Session.GetString("ReceiptExternalCode") ?? "";   
            Console.WriteLine($" Nro. Factura_{receiptExternalCode}");     

            var viewModel = new ReceiptCreateViewModel
            {
                SelectedReceiptExternalCode = receiptExternalCode,
                SelectedProviderDetails     = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails"),
                Items = items.Select(item => new ReceiptItemViewModel
                        {
                            MaterialId = item.MaterialId,
                            SizeId = item.SizeId,
                            MaterialTypeName = item.MaterialTypeName,
                            MaterialName = item.MaterialName,
                            SizeName = item.SizeName,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost
                        }).ToList(),
                PurchaseOrders = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderIndexDetailViewModel>>("PurchaseOrders") ?? new List<PurchaseOrderIndexDetailViewModel>(),
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
                ReceiptTotalAmount = items.Sum(item => item.Quantity * item.UnitCost)

            };

            return View(viewModel);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddItem([FromBody] ReceiptDetailViewModel newItem)
        {
            // Log para verificar los datos recibidos
            Console.WriteLine($"JSON recibido: {System.Text.Json.JsonSerializer.Serialize(newItem)}");

            if (newItem == null || newItem.MaterialId == Guid.Empty || newItem.SizeId == 0 || newItem.Quantity <= 0 || newItem.UnitCost <= 0)
            {
                return Json(new { success = false, message = "Datos inválidos para el material." });
            }

            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();

            // Validar y obtener el material
            var material = await _materialService.GetMaterialByIdAsync(newItem.MaterialId);
            if (material == null)
            {
                return Json(new { success = false, message = "Material no válido." });
            }

            var selectedMaterialType = await _productService.GetMaterialTypeByIdAsync(material.MaterialTypeId);
            var selectedSize = await _sizeService.GetSizeByIdAsync(newItem.SizeId);

            // Crear el nuevo ítem a partir del material y los datos recibidos
            var itemToAdd = new ReceiptDetailViewModel
            {
                MaterialId = material.Id,
                MaterialName = material.Name,
                MaterialTypeName = selectedMaterialType?.Name ?? "N/A",
                SizeId = selectedSize?.Size_Id ?? 0,
                SizeName = selectedSize?.Size_Name ?? "N/A",
                Quantity = newItem.Quantity,
                UnitCost = newItem.UnitCost
            };

            // Agregar el nuevo ítem a la lista
            sessionItems.Add(itemToAdd);

            // Guardar en la sesión actualizada
            HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);

            var receiptExternalCode = HttpContext.Session.GetString("ReceiptExternalCode") ?? "";
            Console.WriteLine($"Nro. Factura después de agregar material: {receiptExternalCode}");
            if (!string.IsNullOrEmpty(receiptExternalCode))
            {
            HttpContext.Session.SetString("ReceiptExternalCode", receiptExternalCode);
            }

            // Log de la lista actualizada
            Console.WriteLine("Lista actualizada de ReceiptItems:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sessionItems));

            return Json(new { success = true, items = sessionItems });
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
        [Produces("application/json")]
        public async Task<IActionResult> AddPayment([FromBody] ReceiptPaymentViewModel newPayment)
        {
            Console.WriteLine($"JSON recibido: {System.Text.Json.JsonSerializer.Serialize(newPayment)}");

            // Validación inicial de datos
            if (newPayment == null || newPayment.PaymentMethodId <= 0 || newPayment.Amount <= 0)
            {
                return Json(new { success = false, message = "Datos inválidos en el método de pago." });
            }

            // Obtener la lista de pagos desde la sesión
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            Console.WriteLine($"sessionPayments from session: {System.Text.Json.JsonSerializer.Serialize(sessionPayments)}");

            // Validar el nuevo método de pago
            var paymentMethod = await _receiptService.GetPaymentMethodByIdAsync(newPayment.PaymentMethodId);
            if (paymentMethod == null)
            {
                return Json(new { success = false, message = "Método de pago no válido." });
            }

            // Crear el nuevo pago y agregarlo a la lista
            var paymentToAdd = new ReceiptPaymentViewModel
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                PaymentMethodName = paymentMethod.Name,
                Percentage = newPayment.Percentage,
                Amount = newPayment.Amount
            };
            sessionPayments.Add(paymentToAdd);

            Console.WriteLine($"sessionPayments before save session: {System.Text.Json.JsonSerializer.Serialize(sessionPayments)}");
            // Guardar los pagos actualizados en la sesión
            HttpContext.Session.SetObjectAsJson("ReceiptPayments", sessionPayments);

            // Reconstruir el modelo necesario para devolver datos a la vista
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            var selectedProvider = HttpContext.Session.GetObjectFromJson<Provider>("SelectedProviderDetails");

            // Aquí mantengo tu lógica para reconstruir el modelo completo
            var createViewModel = new ReceiptCreateViewModel
            {
                MaterialTypes = new List<SelectListItem>(), 
                Materials = new List<SelectListItem>(), 
                Sizes = new List<SelectListItem>(), 
                PaymentMethods = new List<SelectListItem>(), 
                Payments = sessionPayments,
                Items = sessionItems.Select(i => new ReceiptItemViewModel
                {
                    MaterialTypeName = i.MaterialTypeName,
                    MaterialName = i.MaterialName,
                    SizeName = i.SizeName,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost
                }).ToList(),
                SelectedMaterialType = null,
                SelectedMaterial = null,
                SelectedSize = null,
                ProviderId = selectedProvider?.Id ?? Guid.Empty,
                SelectedProviderDetails = selectedProvider
            };

            var receiptExternalCode = HttpContext.Session.GetString("ReceiptExternalCode") ?? "";
            Console.WriteLine($"Nro. Factura después de agregar pago: {receiptExternalCode}");
            if (!string.IsNullOrEmpty(receiptExternalCode))
            {
            HttpContext.Session.SetString("ReceiptExternalCode", receiptExternalCode);
            }            

            // Retorna un JSON con los pagos actualizados
            return Json(new { success = true, payments = sessionPayments });
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
        public async Task<IActionResult> CreateReceipt([FromBody] ReceiptCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Field = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    })
                    .ToList();

                Console.WriteLine("Errores del modelo:", System.Text.Json.JsonSerializer.Serialize(errors));

                return Json(new { success = false, message = "Hubo un error en los datos proporcionados. Por favor, revise e intente nuevamente.", errors });
            }

            // Validación extra para SizeId
            if (viewModel.Items.Any(item => item.SizeId == 0))
            {
                return Json(new
                {
                    success = false,
                    message = "El campo SizeId no puede ser nulo o inválido.",
                    errors = viewModel.Items.Select((item, index) => new
                    {
                        Field = $"Items[{index}].SizeId",
                        Errors = new[] { "El valor de SizeId no es válido." }
                    })
                });
            }

            try
            {

                var receipt = new Receipt
                {
                    ReceiptId = Guid.NewGuid(),
                    ReceiptDate = DateTime.Now,
                    ProviderId = viewModel.ProviderId,
                    ReceiptExternalCode = viewModel.ReceiptExternalCode
                };

                var mappedDetails = viewModel.Items.Select(item => new ReceiptDetail
                {
                    ReceiptId = receipt.ReceiptId,
                    MaterialId = item.MaterialId,
                    SizeId = item.SizeId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                }).ToList();

                var mappedPayments = viewModel.Payments.Select(payment => new ReceiptPayment
                {
                    ReceiptId = receipt.ReceiptId,
                    PaymentMethodId = payment.PaymentMethodId,
                    Amount = payment.Amount,
                    Percentage = payment.Percentage
                }).ToList();

                await _receiptService.ProcessReceiptTransactionAsync(receipt, mappedDetails, mappedPayments);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
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

        [HttpPost]
        public IActionResult UpdatePayment([FromBody] ReceiptPaymentViewModel updatedPayment)
        {
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();

            // Buscar el método de pago por su PaymentMethodId
            var payment = sessionPayments.FirstOrDefault(p => p.PaymentMethodId == updatedPayment.PaymentMethodId);
            if (payment != null)
            {
                payment.Amount = updatedPayment.Amount;

                HttpContext.Session.SetObjectAsJson("ReceiptPayments", sessionPayments);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult UpdateMaterial([FromBody] ReceiptItemViewModel updatedMaterial)
        {
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptItemViewModel>>("ReceiptItems") ?? new List<ReceiptItemViewModel>();

            // Buscar el material por su MaterialId
            var material = sessionItems.FirstOrDefault(m => m.MaterialId == updatedMaterial.MaterialId);
            if (material != null)
            {
                material.Quantity = updatedMaterial.Quantity;
                material.UnitCost = updatedMaterial.UnitCost;

                HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpDelete]
        public IActionResult DeleteMaterial([FromBody] DeleteMaterialRequest request)
        {
            if (request == null || request.MaterialId == Guid.Empty)
            {
                return Json(new { success = false, message = "MaterialId inválido." });
            }

            try
            {
                // Obtener y actualizar la lista de materiales en sesión
                var items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
                var updatedItems = items.Where(item => item.MaterialId != request.MaterialId).ToList();
                HttpContext.Session.SetObjectAsJson("ReceiptItems", updatedItems);

                return Json(new { success = true, message = "Material eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar el material: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult DeletePayment([FromBody] DeletePaymentRequest request)
        {
            if (request == null || request.PaymentMethodId <= 0)
            {
                return Json(new { success = false, message = "PaymentMethodId inválido." });
            }

            try
            {
                // Obtener y actualizar la lista de métodos de pago en sesión
                var payments = HttpContext.Session.GetObjectFromJson<List<ReceiptPaymentViewModel>>("ReceiptPayments") ?? new List<ReceiptPaymentViewModel>();
                var updatedPayments = payments.Where(payment => payment.PaymentMethodId != request.PaymentMethodId).ToList();
                HttpContext.Session.SetObjectAsJson("ReceiptPayments", updatedPayments);

                return Json(new { success = true, message = "Método de pago eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar el método de pago: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid receiptId)
        {
            var receipt = await _receiptService.GetReceiptByIdAsync(receiptId);
            if (receipt == null)
            {
                TempData["ErrorMessage"] = "El recibo no fue encontrado.";
                return RedirectToAction("Index");
            }

            var viewModel = new ReceiptGeneralDetailViewModel
            {
                ReceiptId = receipt.ReceiptId,
                ReceiptDate = receipt.ReceiptDate,
                Provider = new ProviderViewModel
                {
                    Name = receipt.Provider.Name,
                    BusinessName = receipt.Provider.BusinessName,
                    RUT = receipt.Provider.RUT,
                    Address = receipt.Provider.Address,
                    Phone = receipt.Provider.Phone
                },
                Details = receipt.Details.Select(detail => new ReceiptDetailItemViewModel
                {
                    MaterialId = detail.MaterialId,
                    MaterialName = detail.Material.Name,
                    MaterialTypeName = detail.Material.MaterialType.Name,
                    SizeId = detail.SizeId,
                    SizeName = detail.Size.Size_Name,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost
                }).ToList(),
                Payments = receipt.Payments.Select(payment => new ReceiptPaymentViewModel
                {
                    PaymentMethodId = payment.PaymentMethodId,
                    PaymentMethodName = payment.PaymentMethod.Name,
                    Amount = payment.Amount ?? 0m,
                    Percentage = payment.Percentage ?? 0m
                }).ToList()
            };

            return View("Details", viewModel);
        }

        [HttpPost]
        public IActionResult UpdateExternalCode([FromBody] ExternalCodeViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ExternalCode))
                {
                    HttpContext.Session.SetString("ReceiptExternalCode", model.ExternalCode);
                    Console.WriteLine($"Nro. Factura actualizado en sesión: {model.ExternalCode}");
                }

                return Json(new { success = true, externalCode = model.ExternalCode });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateExternalCode: {ex.Message}");
                return Json(new { success = false, message = "Error al actualizar el código externo." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrdersByProviderIdAndStatus(Guid providerId, string status)
        {
            if (!Enum.TryParse(typeof(PurchaseOrderStatus), status, true, out var parsedStatus))
            {
                Console.WriteLine($"El estado '{status}' no es válido.");
                return BadRequest($"El estado '{status}' no es válido.");
            }          

            // Obtener todas las órdenes de compra del proveedor y estado desde el servicio
            var orders = await _purchaseOrderService.GetPurchaseOrdersByProviderAndStatusAsync(providerId, (PurchaseOrderStatus)parsedStatus);

            // Obtener las órdenes de compra ya asignadas desde la sesión
            var sessionPurchaseOrders = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderDetailViewModel>>("PurchaseOrders") 
                ?? new List<PurchaseOrderDetailViewModel>();

            // Filtrar las órdenes que **NO** están en la sesión
            var filteredOrders = orders.Where(o => !sessionPurchaseOrders.Any(s => s.PurchaseOrderId == o.PurchaseOrderId)).ToList();

            return Json(filteredOrders.Select(o => new
            {
                o.PurchaseOrderId,
                o.PurchaseOrderNumber,
                o.OrderDate,
                o.TotalAmount,
                o.ProviderName,
                o.Status
            }));
        }


        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddItemFromPurchaseOrders([FromBody] List<Guid> purchaseOrderIds)
        {
            if (purchaseOrderIds == null || !purchaseOrderIds.Any())
            {
                return Json(new { success = false, message = "No se recibieron órdenes de compra válidas." });
            }

            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
            var sessionPurchaseOrders = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderDetailViewModel>>("PurchaseOrders") ?? new List<PurchaseOrderDetailViewModel>();

            foreach (var purchaseOrderId in purchaseOrderIds)
            {
                var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(purchaseOrderId);

                if (purchaseOrder == null)
                {
                    return Json(new { success = false, message = $"Orden de compra {purchaseOrderId} no encontrada." });
                }

                // Agregar la orden a la lista de órdenes seleccionadas si aún no está
                if (!sessionPurchaseOrders.Any(po => po.PurchaseOrderId == purchaseOrderId))
                {
                    sessionPurchaseOrders.Add(new PurchaseOrderDetailViewModel
                    {
                        PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                        PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
                        OrderDate = purchaseOrder.OrderDate,
                        ProviderName = purchaseOrder.ProviderName,
                        Status = purchaseOrder.Status.ToString()
                    });
                }

                foreach (var item in purchaseOrder.Items)
                {
                    var material = await _materialService.GetMaterialByIdAsync(item.MaterialId);
                    if (material == null)
                    {
                        return Json(new { success = false, message = $"Material con ID {item.MaterialId} no encontrado." });
                    }

                    var selectedMaterialType = await _productService.GetMaterialTypeByIdAsync(material.MaterialTypeId);
                    var selectedSize = await _sizeService.GetSizeByIdAsync(item.SizeId);

                    var unitCost = item.UnitCost > 0 ? item.UnitCost : 9999999;

                    var itemToAdd = new ReceiptDetailViewModel
                    {
                        MaterialId = material.Id,
                        MaterialName = material.Name,
                        MaterialTypeName = selectedMaterialType?.Name ?? "N/A",
                        SizeId = selectedSize?.Size_Id ?? 0,
                        SizeName = selectedSize?.Size_Name ?? "N/A",
                        Quantity = item.Quantity,
                        UnitCost = unitCost
                    };

                    sessionItems.Add(itemToAdd);
                }
            }

            // Guardar en la sesión
            HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);
            HttpContext.Session.SetObjectAsJson("PurchaseOrders", sessionPurchaseOrders);

            return Json(new
            {
                success = true,
                items = sessionItems ?? new List<ReceiptDetailViewModel>(),
                purchaseOrders = sessionPurchaseOrders ?? new List<PurchaseOrderDetailViewModel>()  
            });

        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> DeletePurchaseOrderFromReceipt([FromBody] Guid purchaseOrderId)
        {
            Console.WriteLine($"DeletePurchaseOrderFromReceipt - START -----> purchaseOrderId: {purchaseOrderId}");
            if (purchaseOrderId == Guid.Empty)
            {
                return Json(new { success = false, message = "ID de orden de compra no válido." });
            }

            var sessionPurchaseOrders = HttpContext.Session.GetObjectFromJson<List<PurchaseOrderDetailViewModel>>("PurchaseOrders") ?? new List<PurchaseOrderDetailViewModel>();
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();

            var purchaseOrderToRemove = sessionPurchaseOrders.FirstOrDefault(po => po.PurchaseOrderId == purchaseOrderId);
            
            if (purchaseOrderToRemove == null)
            {
                return Json(new { success = false, message = "Orden de compra no encontrada en la sesión." });
            }

            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(purchaseOrderId);

            if (purchaseOrder == null)
            {
                return Json(new { success = false, message = "Orden de compra no encontrada en la base de datos." });
            }

            // Obtener los materiales de la orden a eliminar
            foreach (var item in purchaseOrder.Items)
            {
                var existingMaterial = sessionItems.FirstOrDefault(m => m.MaterialId == item.MaterialId && m.SizeId == item.SizeId);

                if (existingMaterial != null)
                {
                    var totalQuantityFromOtherOrders = sessionPurchaseOrders
                        .Where(po => po.PurchaseOrderId != purchaseOrderId)
                        .SelectMany(po => purchaseOrder.Items.Where(d => d.MaterialId == item.MaterialId && d.SizeId == item.SizeId))
                        .Sum(d => d.Quantity);

                    if (totalQuantityFromOtherOrders == 0)
                    {
                        // Caso 1: Material solo existe en esta orden
                        if (existingMaterial.Quantity == item.Quantity)
                        {
                            // 1.a - Eliminar el material si tiene la misma cantidad exacta
                            sessionItems.Remove(existingMaterial);
                        }
                        else if (existingMaterial.Quantity > item.Quantity)
                        {
                            // 1.b - Restar la cantidad de la orden eliminada
                            existingMaterial.Quantity -= item.Quantity;
                        }
                        else
                        {
                            // 1.c - Mostrar confirmación si la cantidad en la lista de materiales es menor
                            return Json(new { 
                                success = false, 
                                confirmationRequired = true,
                                message = $"La cantidad en la lista de materiales ({existingMaterial.Quantity}) es menor que la de la orden a eliminar ({item.Quantity}). ¿Desea eliminar este material?" 
                            });
                        }
                    }
                    else
                    {
                        // Caso 2: Material existe en otras órdenes de compra
                        if (existingMaterial.Quantity >= item.Quantity)
                        {
                            existingMaterial.Quantity -= item.Quantity;
                        }
                        else
                        {
                            return Json(new { 
                                success = false, 
                                confirmationRequired = true,
                                message = $"La cantidad en la lista de materiales ({existingMaterial.Quantity}) es menor que la de la orden a eliminar ({item.Quantity}). ¿Desea eliminar este material?" 
                            });
                        }
                    }
                }
            }

            // Eliminar la orden de compra de la lista de órdenes seleccionadas
            sessionPurchaseOrders.Remove(purchaseOrderToRemove);

            // Guardar en sesión las listas actualizadas
            HttpContext.Session.SetObjectAsJson("PurchaseOrders", sessionPurchaseOrders);
            HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);

            return Json(new { success = true, message = "Orden de compra eliminada correctamente.", items = sessionItems, purchaseOrders = sessionPurchaseOrders });
        }


    }

}

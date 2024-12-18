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
        private readonly ReceiptService _Receiptservice;
        private readonly ProductService _productService;
        private readonly SizeService _sizeService;

        public ReceiptController(MaterialService materialService, ReceiptService Receiptservice, ProductService productService, SizeService sizeService)
        {
            _materialService = materialService;
            _Receiptservice = Receiptservice;
            _productService = productService;
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            var materials = await _materialService.GetAllMaterialsAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            var viewModel = new ReceiptCreateViewModel
            {
                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }),
                Materials = materials.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                }),
                Sizes = sizes.Select(s => new SelectListItem
                {
                    Value = s.Size_Id.ToString(),
                    Text = s.Size_Name
                })
            };

            // Cargar valores seleccionados desde la sesión
            if (HttpContext.Session.GetString("SelectedMaterialType") != null)
            {
                viewModel.SelectedMaterialType = Guid.Parse(HttpContext.Session.GetString("SelectedMaterialType"));
                viewModel.SelectedMaterial = Guid.Parse(HttpContext.Session.GetString("SelectedMaterial"));
                viewModel.SelectedSize = int.Parse(HttpContext.Session.GetString("SelectedSize") ?? "0");
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ReceiptViewModel viewModel)
        {
            var sessionData = HttpContext.Session.GetString("ReceiptItems");
            Console.WriteLine("Contenido crudo en sesión (ReceiptItems): " + sessionData);
            
            // Obtener el JSON desde la sesión
            var jsonFromSession = HttpContext.Session.GetString("ReceiptItems");
            Console.WriteLine("Contenido de ReceiptItems en sesión: " + jsonFromSession);

            // Deserializar el objeto
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") 
                            ?? new List<ReceiptDetailViewModel>();

            var material = await _materialService.GetMaterialByIdAsync(viewModel.NewItem.MaterialId);
            if (material == null)
            {
                ModelState.AddModelError("NewItem.MaterialId", "Material no válido.");
            }
            else
            {
                var selectedMaterialType = await _productService.GetMaterialTypeByIdAsync(material.MaterialTypeId);
                var selectedSize = await _sizeService.GetSizeByIdAsync(viewModel.NewItem.SizeId);

                var newItem = new ReceiptDetailViewModel
                {
                    MaterialId = material.Id,
                    MaterialName = material.Name,
                    MaterialTypeName = selectedMaterialType?.Name ?? "N/A",
                    SizeId = selectedSize?.Size_Id ?? 0,
                    SizeName = selectedSize?.Size_Name ?? "N/A",
                    Quantity = viewModel.NewItem.Quantity,
                    UnitCost = viewModel.NewItem.UnitCost
                };

                sessionItems.Add(newItem);

                // Guardar en la sesión actualizada
                HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionItems);
                viewModel.Items = sessionItems;

                // Mostrar nuevamente el JSON actualizado en la consola
                var updatedJson = JsonSerializer.Serialize(sessionItems);
                Console.WriteLine("Contenido actualizado de ReceiptItems en sesión: " + updatedJson);
            }

            await RecargarListas(viewModel);

            // Convertir ReceiptViewModel a ReceiptCreateViewModel
            var createViewModel = new ReceiptCreateViewModel
            {
                MaterialTypes = viewModel.MaterialTypes,
                Materials = viewModel.Materials,
                Sizes = viewModel.Sizes,
                Items = viewModel.Items.Select(i => new ReceiptItemViewModel
                {
                    MaterialTypeName = i.MaterialTypeName,
                    MaterialName = i.MaterialName,
                    SizeName = i.SizeName,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost
                }).ToList(),
                SelectedMaterialType = viewModel.SelectedMaterialType,
                SelectedMaterial = viewModel.SelectedMaterial,
                SelectedSize = viewModel.SelectedSize
            };

            return View("Create", createViewModel);
        }

    [HttpPost]
    public async Task<IActionResult> AddMaterial(MaterialViewModel model)
    {
        Console.WriteLine("Contenido crudo en sesión (AddMaterial): " + model);

        if (!ModelState.IsValid)
        {
            // Recuperar los elementos actuales de la sesión
            var sessionItems = HttpContext.Session.GetObjectFromJson<List<ReceiptItemViewModel>>("ReceiptItems") 
                            ?? new List<ReceiptItemViewModel>();
            var sessionPayments = HttpContext.Session.GetObjectFromJson<List<PaymentViewModel>>("ReceiptPayments") 
                                ?? new List<PaymentViewModel>();

            // Reconstruir el modelo ReceiptCreateViewModel
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            var materials = await _materialService.GetAllMaterialsAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            var createViewModel = new ReceiptCreateViewModel
            {
                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text = mt.Name
                }),
                Materials = materials.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                }),
                Sizes = sizes.Select(s => new SelectListItem
                {
                    Value = s.Size_Id.ToString(),
                    Text = s.Size_Name
                }),
                Items = sessionItems, // Mantener los ítems agregados
                Payments = sessionPayments, // Mantener los métodos de pago agregados
                SelectedMaterialType = model.MaterialTypeId,
                SelectedMaterial = null, // No se selecciona ningún material si hay error
                SelectedSize = model.SizeId
            };

            // Agregar un mensaje de error
            ModelState.AddModelError("", "Hubo un error al intentar agregar el material. Verifique los datos.");

            // Retornar a la vista Create con el modelo reconstruido
            return View("Create", createViewModel);
        }

        // Crear el nuevo material si el modelo es válido
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            MaterialTypeId = model.MaterialTypeId,
/*            SizeId = model.SizeId,
            Stock = 0,
            Cost = 0
 */
        };

        await _materialService.AddMaterialAsync(material);

        // Guardar en sesión los valores seleccionados
        HttpContext.Session.SetString("SelectedMaterialType", model.MaterialTypeId.ToString());
        HttpContext.Session.SetString("SelectedMaterial", material.Id.ToString());
        HttpContext.Session.SetString("SelectedSize", model.SizeId.ToString());

        // Redirigir al formulario padre (Create)
        return RedirectToAction("Create");
    }



        private async Task RecargarListas(ReceiptViewModel viewModel)
        {
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            var paymentMethods = await _Receiptservice.GetAllPaymentMethodsAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            viewModel.PaymentMethods = paymentMethods.Select(pm => new SelectListItem { Value = pm.PaymentMethodId.ToString(), Text = pm.Name });
            viewModel.Sizes = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });

            viewModel.Items = HttpContext.Session.GetObjectFromJson<List<ReceiptDetailViewModel>>("ReceiptItems") ?? new List<ReceiptDetailViewModel>();
        }

        private async Task RecargarMaterialForm(MaterialViewModel model)
        {
            var materialTypes = await _materialService.GetAllMaterialTypesAsync();
            var sizes = await _sizeService.GetAllSizesAsync();

            model.MaterialTypes = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });
            model.Sizes = sizes.Select(s => new SelectListItem { Value = s.Size_Id.ToString(), Text = s.Size_Name });
        }
/*
        [HttpGet]
        public async Task<IActionResult> GetSizesByMaterialType(Guid materialId)
        {
            var materials = await _materialService.GetMaterialsByTypeAsync(materialId);
            var sizeIds = materials.Select(m => m.SizeId).Distinct().ToList();
            var sizes = await _sizeService.GetSizesByIdsAsync(sizeIds);

            return Json(sizes.Select(s => new
            {
                Id = s.Size_Id,
                Name = s.Size_Name
            }));
        }
*/
    }
}

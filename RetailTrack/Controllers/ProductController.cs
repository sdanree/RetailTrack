using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    [Authorize(Policy = "UserAproved")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly DesignService _designService;
        private readonly SizeService _sizeService;
        private readonly MaterialService _materialService;

        public ProductController(ProductService productService, DesignService designService, SizeService sizeService, MaterialService materialService)
        {
            _productService     = productService;
            _designService      = designService;
            _sizeService        = sizeService;        
            _materialService    = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Obtener los datos necesarios para el formulario
            var designs         = await _designService.GetAllDesignsAsync() ?? new List<Design>();
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync() ?? new List<MaterialType>();

            // Crear el ViewModel con los datos cargados
            var viewModel = new ProductCreateViewModel
            {
                Designs         = designs.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }),
                MaterialTypes   = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            var product = viewModel.Product;

            // Loggear el payload para depuración
            var viewModelJson = System.Text.Json.JsonSerializer.Serialize(viewModel, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine("Payload recibido:");
            Console.WriteLine(viewModelJson);

            // Validar y asignar propiedades relacionadas
            if (product.MaterialId != Guid.Empty)
            {
                product.Material = await _productService.GetMaterialByIdAsync(product.MaterialId);
                if (product.Material == null)
                {
                    ModelState.AddModelError("MaterialId", "El material seleccionado no es válido.");
                }
            }

            if (product.DesignId != Guid.Empty)
            {
                product.Design = await _designService.GetDesignByIdAsync(product.DesignId);
                if (product.Design == null)
                {
                    ModelState.AddModelError("DesignId", "El diseño seleccionado no es válido.");
                }
            }

            if (product.ProductStatusId > 0)
            {
                product.Status = await _productService.GetProductStatusByIdAsync(product.ProductStatusId);
                if (product.Status == null)
                {
                    ModelState.AddModelError("ProductStatusId", "El estado seleccionado no es válido.");
                }
            }

            product.Description ??= string.Empty;

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Fallo el modelo. Recargando listas...");

                // Loggear los errores en ModelState para entender el problema
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error en {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                await RecargarListas(viewModel);
                return View(viewModel);
            }

            await _productService.AddProductAsync(product);

            TempData["Message"] = "Producto creado con éxito.";
            return RedirectToAction("Index");
        }



        private async Task RecargarListas(ProductCreateViewModel viewModel)
        {
            var designs         = await _designService.GetAllDesignsAsync() ?? new List<Design>();
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync() ?? new List<MaterialType>();

            viewModel.Designs       = designs.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
            viewModel.MaterialTypes = materialTypes.Select(mt => new SelectListItem { Value = mt.Id.ToString(), Text = mt.Name });

            if (!string.IsNullOrEmpty(viewModel.MaterialTypeId))
            {
                var materials = await _productService.GetMaterialsByTypeAsync(Guid.Parse(viewModel.MaterialTypeId));
                viewModel.Materials = materials.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetMaterialsByType(Guid materialTypeId)
        {
            if (materialTypeId == Guid.Empty)
            {
                return Json(new { success = false, message = "ID de tipo de material inválido." });
            }

            var materials = await _productService.GetMaterialsByTypeAsync(materialTypeId);
            if (materials == null || !materials.Any())
            {
                return Json(new { success = false, message = "No se encontraron materiales para este tipo." });
            }

            return Json(new { success = true, data = materials });
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();

            var viewModel = products.Select(product => new ProductDetailsViewModel
            {
                Id                  = product.Id,
                Name                = product.Name,
                Description         = product.Description,
                QuantityRequested   = product.QuantityRequested,
                Design              = product.Design?.Name,
                Material            = product.Material?.Name,
                MaterialType        = product.Material?.MaterialType?.Name,
                Status              = product.Status?.Status_Name
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailsViewModel
            {
                Id                  = product.Id,
                Name                = product.Name,
                Description         = product.Description,
                QuantityRequested   = product.QuantityRequested,
                Design              = product.Design?.Name,
                Material            = product.Material?.Name,
                MaterialType        = product.Material?.MaterialType?.Name,
                Status              = product.Status?.Status_Name
            };

            return View(viewModel);
        }
    }
}

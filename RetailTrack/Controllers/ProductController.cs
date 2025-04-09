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
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly DesignService _designService;
        private readonly MaterialService _materialService;
        private readonly MaterialTypeService _materialtypeService;
        private readonly SizeService _sizeService;

        public ProductController(ProductService productService, DesignService designService, MaterialService materialService, MaterialTypeService materialtypeService, SizeService sizeService)
        {
            _productService         = productService;
            _designService          = designService;
            _materialService        = materialService;
            _materialtypeService    = materialtypeService;
            _sizeService            = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
            var designs         = await _designService.GetAllDesignsAsync() ?? new List<Design>();
            var statuses        = await _productService.GetAllProductStatusesAsync() ?? new List<ProductStatus>();
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync() ?? new List<MaterialType>();
           
            Guid selectedDesignId   = Guid.Empty;
            var designIdString      = HttpContext.Session.GetString("SelectedDesignId");

            if (!string.IsNullOrEmpty(designIdString) && Guid.TryParse(designIdString, out var parsedDesignId))
            {
                selectedDesignId = parsedDesignId;
            }

            var sessionDesignDetails = HttpContext.Session.GetObjectFromJson<Design>("SelectedDesignDetails") ?? new Design();
            var viewModel = new ProductCreateViewModel
            {
                SelectedDesignId        = selectedDesignId,   
                SelectedDescription     = HttpContext.Session.GetString("SelectedDescription") ?? "",   
                SelectedGeneralPrice    = decimal.TryParse(HttpContext.Session.GetString("SelectedGeneralPrice"), out var price) ? price : 0,
                SelectedProductName     = HttpContext.Session.GetString("SelectedProductName") ?? "",      
                SelectedDesignDetails   = sessionDesignDetails,
                Designs                 = designs.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name }),

                Statuses = statuses.Select(s => new SelectListItem
                {
                    Value = s.Status_Id.ToString(),
                    Text  = s.Status_Name
                }),

                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text  = mt.Name
                }),

                // Los materiales y tamaños se cargarán dinámicamente en la UI con AJAX
                Materials = new List<SelectListItem>(),
                Sizes = new List<SelectListItem>(),
                Variants = sessionVariants.Select(variant => new ProductStockViewModel
                                {
                                        MaterialId          = variant.MaterialId,
                                        SizeId              = variant.SizeId,
                                        MaterialName        = variant.MaterialName,
                                        MaterialTypeName    = variant.MaterialTypeName,
                                        MaterialSizeName    = variant.MaterialSizeName,
                                        CustomPrice         = variant.CustomPrice,
                                        Cost                = variant.Cost,
                                        Stock               = variant.Stock
                                }).ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateViewModel viewModel)
        {
            var sessionDesignDetails = HttpContext.Session.GetObjectFromJson<Design>("SelectedDesignDetails") ?? new Design();
            if(sessionDesignDetails == null){
                Console.WriteLine("Obejeto Design vacio");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sessionDesignDetails, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                return Json(new { success = false, message = "Obejeto Design vacio. Por favor, revise e intente nuevamente."});
            }

            var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
            if(sessionVariants == null || sessionVariants.Count < 1){
                Console.WriteLine("no hay variantes ingresadas");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sessionVariants, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                return Json(new { success = false, message = "No hay variantes asignadas, minimo debe de haber una variante. Por favor, revise e intente nuevamente."});
            }

            try
            {
                var product = new Product
                {
                    Id              = Guid.NewGuid(),
                    Name            = viewModel.Product.Name,
                    Description     = viewModel.Product.Description ?? string.Empty,
                    GeneralPrice    = viewModel.Product.GeneralPrice,
                    DesignId        = viewModel.Product.DesignId,
                    ProductStatusId = (int)ProductStatusEnum.Available 
                };

                product.Design = await _designService.GetDesignByIdAsync(product.DesignId);
                if (product.Design == null)
                {
                    return Json(new { success = false, message = "El diseño seleccionado no es válido." });
                }

                await _productService.AddProductAsync(product);

                var productStocks = new List<ProductStock>();

                foreach (var variant in sessionVariants)
                {
                    var materialSize = await _materialService.GetMaterialSizeAsync(variant.MaterialId, variant.SizeId);
                    if (materialSize == null)
                    {
                        Console.WriteLine($"Advertencia: El material y tamaño seleccionado para la variante {variant.MaterialId}-{variant.SizeId} no es válido. Se omitirá.");
                        continue;
                    }

                    productStocks.Add(new ProductStock
                    {
                        Id          = Guid.NewGuid(),
                        ProductId   = product.Id,
                        MaterialId  = variant.MaterialId,
                        SizeId      = variant.SizeId,
                        CustomPrice = variant.CustomPrice,
                        Cost        = variant.Cost,
                        Stock       = variant.Stock,
                        Available   = true
                    });
                }

                if (productStocks.Any())
                {
                    await _productService.UpSertProductStocksByProductIdAsync(product.Id ,productStocks);
                }

                return Json(new { success = true, message = "Producto creado con éxito." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la creación del producto: {ex.Message}");
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Index(string? productName, Guid? designId, Guid? materialTypeId, Guid? materialId, int? sizeId, int? statusId)
        {
            var products = await _productService.GetAllProductsAsync();

            if (!string.IsNullOrEmpty(productName))
            {
                products = products.Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (designId.HasValue)
            {
                products = products.Where(p => p.DesignId == designId.Value).ToList();
            }

            if (statusId.HasValue)
            {
                products = products.Where(p => p.ProductStatusId == statusId.Value).ToList();
            }

            if (materialTypeId.HasValue || materialId.HasValue || sizeId.HasValue)
            {
                products = products
                    .Where(p => p.Variants.Any(v =>
                        (!materialTypeId.HasValue || v.MaterialSize.Material.MaterialTypeId == materialTypeId.Value) &&
                        (!materialId.HasValue || v.MaterialId == materialId.Value) &&
                        (!sizeId.HasValue || v.SizeId == sizeId.Value)
                    )).ToList();
            }

            var designs         = await _designService.GetAllDesignsAsync();
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var materials       = await _materialService.GetAllMaterialsAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();
            var statuses        = await _productService.GetAllProductStatusesAsync();

            var viewModel = new ProductFilterViewModel
            {
                Products = products.Select(product => new ProductDetailsViewModel
                {
                    Id              = product.Id,
                    Name            = product.Name,
                    Description     = product.Description,
                    DesignName      = product.Design?.Name,
                    Design          = product.Design,
                    Status          = product.Status?.Status_Name,
                    Variants        = product.Variants.Select(v => new ProductStockViewModel
                    {
                        MaterialTypeName    = v.MaterialSize.Material.MaterialType.Name,
                        MaterialName        = v.MaterialSize.Material.Name,
                        MaterialSizeName    = v.MaterialSize.Size.Size_Name,
                        CustomPrice         = v.CustomPrice,
                        Cost                = v.Cost,
                        Stock               = v.Stock
                    }).ToList()
                }).ToList(),
                Designs = designs.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }),
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
                sizesList = sizes.Select(ms => new SelectListItem
                {
                    Value = ms.Size_Id.ToString(),
                    Text = ms.Size_Name
                }),
                Statuses = statuses.Select(s => new SelectListItem
                {
                    Value = s.Status_Id.ToString(),
                    Text = s.Status_Name
                }),
                ProductName             = productName,
                SelectedDesignId        = designId,
                SelectedMaterialTypeId  = materialTypeId,
                SelectedMaterialId      = materialId,
                SelectedSizeId          = sizeId,
                SelectedStatusId        = statusId
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Obtener variantes (ProductStock) del producto
            var productStocks = await _productService.GetProductStocksByProductIdAsync(product.Id);

            var viewModel = new ProductDetailsViewModel
            {
                Id          = product.Id,
                Name        = product.Name,
                Description = product.Description,
                Price       = product.GeneralPrice ?? 0,
                Status      = product.Status?.Status_Name,
                DesignName  = product.Design?.Name,
                Design      = product.Design,


                // Cargar variantes del producto
                Variants = productStocks.Select(ps => new ProductStockViewModel
                {
                    MaterialTypeName = ps.MaterialSize.Material.MaterialType.Name,
                    MaterialName     = ps.MaterialSize.Material.Name,
                    MaterialSizeName = ps.MaterialSize.Size.Size_Name,
                    Stock            = ps.Stock,
                    CustomPrice      = ps.CustomPrice,
                    Cost             = ps.Cost
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddVariant([FromBody] ProductStockViewModel variant)
        {
            if (variant == null || variant.MaterialId == Guid.Empty || variant.SizeId == 0)
            {
                return Json(new { success = false, message = "Error en datos" });
            }
            
            var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
            var materialSize    = await _materialService.GetMaterialSizeAsync(variant.MaterialId, variant.SizeId);

            if (materialSize == null)
            {
                return Json(new { success = false, message = "Material y/o Tamaño no válido" });
            }

            var newVariant = new ProductStockViewModel
            {
                MaterialId              = variant.MaterialId,
                SizeId                  = variant.SizeId,
                MaterialName            = materialSize.Material?.Name ?? "N/A",
                MaterialTypeName        = materialSize.Material?.MaterialType.Name ?? "N/A",
                MaterialSizeName        = materialSize.Size?.Size_Name ?? "N/A",
                CustomPrice             = variant.CustomPrice,
                Cost                    = materialSize.Cost,
                Stock                   = variant.Stock,
                Available               = true
            };

            sessionVariants.Add(newVariant);
            HttpContext.Session.SetObjectAsJson("Variants", sessionVariants);

            return Json(new { success = true, VariantList = sessionVariants });

        }        

        [HttpDelete]
        public IActionResult DeleteVariant([FromBody] DeleteVariantRequest request)
        {
            if (request == null || request.MaterialId == Guid.Empty)
            {
                return Json(new { success = false, message = "MaterialId inválido." });
            }

            try
            {
                var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
                var updatedVariants = sessionVariants.Where(item => item.MaterialId != request.MaterialId).ToList();
                HttpContext.Session.SetObjectAsJson("Variants", updatedVariants);

                return Json(new { success = true, message = "Variant eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar el variante: {ex.Message}" });
            }
        }

       [HttpPost]
        public IActionResult UpdateVariant([FromBody] ProductStockViewModel updatedVariant)
        {
            var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
            var variant         = sessionVariants.FirstOrDefault(m => m.MaterialId == updatedVariant.MaterialId);
            
            if (variant != null)
            {
                variant.Stock       = updatedVariant.Stock;
                variant.CustomPrice = updatedVariant.CustomPrice;

                HttpContext.Session.SetObjectAsJson("ReceiptItems", sessionVariants);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
   
        [HttpPost]
        public async Task<IActionResult> UpdateProductHeader([FromBody] ProductHeaderViewModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.SelectedProductName))
                {
                    HttpContext.Session.SetString("SelectedProductName", model.SelectedProductName);
                }

                if (model.SelectedGeneralPrice.HasValue)
                {
                    HttpContext.Session.SetString("SelectedGeneralPrice", model.SelectedGeneralPrice.ToString());
                }

                if (!string.IsNullOrEmpty(model.SelectedDescription))
                {
                    HttpContext.Session.SetString("SelectedDescription", model.SelectedDescription);
                }

                if (model.SelectedDesignId != null)
                {
                    HttpContext.Session.SetString("SelectedDesignId", model.SelectedDesignId.ToString());
                    Guid DesignId   = model.SelectedDesignId.Value;
                    var design      = await _designService.GetDesignByIdAsync(DesignId);
                    HttpContext.Session.SetObjectAsJson("SelectedDesignDetails", design);
                }

                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Product Header: {ex.Message}");
                return Json(new { success = false, message = "Error al actualizar el Product header." });
            }
        }  

        [HttpPost]
        public IActionResult ClearProductSession()
        {
            HttpContext.Session.Remove("Variants");
            HttpContext.Session.Remove("SelectedProductName");
            HttpContext.Session.Remove("SelectedGeneralPrice");
            HttpContext.Session.Remove("SelectedGeneralPrice");
            HttpContext.Session.Remove("SelectedDescription");
            HttpContext.Session.Remove("SelectedDesignId");
            HttpContext.Session.Remove("SelectedDesignDetails");

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.GeneralPrice = product.GeneralPrice.HasValue ? Math.Round(product.GeneralPrice.Value, 2) : 0;
            HttpContext.Session.SetString("SelectedGeneralPrice", product.GeneralPrice.ToString());

            var variants        = new List<ProductStockViewModel>();
            var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants");
            if (sessionVariants != null)
            {
                variants = sessionVariants;
            }
            else
            {
                var productStocks = await _productService.GetProductStocksByProductIdAsync(id);
                variants = productStocks.Select(v => new ProductStockViewModel
                {
                    MaterialId           = v.MaterialId,
                    SizeId               = v.SizeId,
                    MaterialName         = v.MaterialSize?.Material?.Name ?? "N/A",
                    MaterialTypeName     = v.MaterialSize?.Material?.MaterialType?.Name ?? "N/A",
                    MaterialSizeName     = v.MaterialSize?.Size?.Size_Name ?? "N/A",
                    CustomPrice          = v.CustomPrice,
                    Cost                 = v.Cost,
                    Stock                = v.Stock,
                    Available            = v.Available
                }).ToList();
            }
            HttpContext.Session.SetObjectAsJson("Variants", variants);

            var designDetails   = await _designService.GetDesignByIdAsync(product.DesignId);
            var materialTypes   = await _materialService.GetAllMaterialTypesAsync();
            var materials       = await _materialService.GetAllMaterialsAsync();
            var sizes           = await _sizeService.GetAllSizesAsync();
            var statuses        = await _productService.GetAllProductStatusesAsync();
            var designs         = await _designService.GetAllDesignsAsync();

            var viewModel = new ProductEditViewModel
            {
                Product = new Product
                {
                    Id              = product.Id,
                    Name            = product.Name,
                    Description     = product.Description,
                    GeneralPrice    = product.GeneralPrice,
                    DesignId        = product.DesignId,
                    ProductStatusId = product.ProductStatusId
                },
                Variants                = variants,
                SelectedDesignDetails   = designDetails,
                
                // Listas de selección
                Designs = designs.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text  = d.Name
                }),
                Statuses = statuses.Select(s => new SelectListItem
                {
                    Value = s.Status_Id.ToString(),
                    Text  = s.Status_Name
                }),
                MaterialTypes = materialTypes.Select(mt => new SelectListItem
                {
                    Value = mt.Id.ToString(),
                    Text  = mt.Name
                }),
                Materials = materials.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text  = m.Name
                }),
                Sizes = sizes.Select(s => new SelectListItem
                {
                    Value = s.Size_Id.ToString(),
                    Text  = s.Size_Name
                }),
                SelectedState           = product.ProductStatusId,
                SelectedDesignId        = product.DesignId,
                SelectedProductName     = product.Name,
                SelectedDescription     = product.Description,
                SelectedGeneralPrice    = decimal.TryParse(HttpContext.Session.GetString("SelectedGeneralPrice"), out var price) ? price : 0,

            };
            return View(viewModel);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> EditProduct([FromBody]ProductEditViewModel viewModel)
        {
            if (viewModel == null)
            {
                Console.WriteLine("Error: viewModel.Product es null");
                return Json(new { success = false, message = "El objeto del producto no fue recibido correctamente." });
            }            
            
            var designDetails   = await _designService.GetDesignByIdAsync(viewModel.Product.DesignId);    
            viewModel.Product.Design = designDetails;
             
            var sessionDesignDetails = HttpContext.Session.GetObjectFromJson<Design>("SelectedDesignDetails") ?? new Design();
            if(sessionDesignDetails == null){
                Console.WriteLine("Obejeto Design vacio");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sessionDesignDetails, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                return Json(new { success = false, message = "Objeto Design vacio. Por favor, revise e intente nuevamente."});
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(viewModel.Product.Id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado" });
                }

                // Actualización de campos del producto
                product.Name             = viewModel.Product.Name;
                product.Description      = viewModel.Product.Description;
                product.GeneralPrice     = viewModel.Product.GeneralPrice;
                product.DesignId         = viewModel.Product.DesignId;
                product.ProductStatusId  = viewModel.Product.ProductStatusId;

                await _productService.UpdateProductAsync(product);

                // Obtener lista actualizada desde sesión
                var sessionVariants = HttpContext.Session.GetObjectFromJson<List<ProductStockViewModel>>("Variants") ?? new List<ProductStockViewModel>();
                var updatedStocks   = sessionVariants.Select(v => new ProductStock
                {
                    ProductId   = product.Id,
                    MaterialId  = v.MaterialId,
                    SizeId      = v.SizeId,
                    CustomPrice = v.CustomPrice,
                    Cost        = v.Cost,
                    Stock       = v.Stock,
                    Available   = v.Available
                }).ToList();

                if (updatedStocks.Any())
                {
                    await _productService.UpSertProductStocksByProductIdAsync(product.Id,updatedStocks);
                }

                return Json(new { success = true, message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto: {ex.Message}");
                return Json(new { success = false, message = $"Error interno: {ex.Message}" });
            }
        }

         
    }
}

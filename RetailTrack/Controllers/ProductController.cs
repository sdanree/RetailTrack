using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models.Products;
using RetailTrack.Services;
using RetailTrack.ViewModels;

namespace RetailTrack.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly DesignService _designService;

        public ProductController(ProductService productService, DesignService designService)
        {
            _productService = productService;
            _designService  = designService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var designs = await _designService.GetAllDesignsAsync() ?? new List<Design>();
            ViewBag.Designs = new SelectList(designs, "Id", "Name");

            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                var designs = await _designService.GetAllDesignsAsync() ?? new List<Design>();
                ViewBag.Designs = new SelectList(designs, "Id", "Name");
                return View(product);
            }

            await _productService.AddProductAsync(product);
            TempData["Message"] = "Producto creado con éxito.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            // Obtener el producto por su ID
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            // Cargar el diseño asociado basado en el DesignId
            var design = product.DesignId != Guid.Empty
                ? await _designService.GetDesignByIdAsync(product.DesignId)
                : null;

            // Crear un ViewModel para combinar Producto y Diseño
            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Design = design
            };

            return View(viewModel);
        }



        public IActionResult Delete(Guid id)
        {
            _productService.DeleteProduct(id);
            TempData["Message"] = "Producto eliminado con éxito.";
            return RedirectToAction("Index");
        }
    }
}

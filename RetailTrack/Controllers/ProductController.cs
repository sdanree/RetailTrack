using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailTrack.Models.Products;
using RetailTrack.Services;

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
            if (ModelState.IsValid)
            {
                _productService.AddProduct(product);
                TempData["Message"] = "Producto creado con éxito.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Asegúrate de esperar la lista de diseños
            var designs = await _designService.GetAllDesignsAsync() ?? new List<Design>();
            ViewBag.Designs = new SelectList(designs, "Id", "Name");

            return View(product);
        }


        public IActionResult Details(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Delete(Guid id)
        {
            _productService.DeleteProduct(id);
            TempData["Message"] = "Producto eliminado con éxito.";
            return RedirectToAction("Index");
        }
    }
}

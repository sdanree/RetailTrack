using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using RetailTrack.Services;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    [Authorize(Policy = "UserAproved")]
    public class DesignController : Controller
    {
        private readonly DesignService _designService;
        private readonly IWebHostEnvironment _environment;

        public DesignController(DesignService designService, IWebHostEnvironment environment)
        {
            _designService  = designService;
            _environment    = environment;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var designs = await _designService.GetAllDesignsAsync();
            return View(designs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description")] Design design, IFormFile? Image)
        {


            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Error: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(design);
            }

            if (Image != null && Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img");

                // Verificar si la carpeta 'img' existe y crearla si es necesario
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                // Guardamos solo la ruta relativa para evitar problemas con rutas absolutas
                design.ImageUrl = $"/img/{fileName}";
            }
            else
            {
                // Si no se subió ninguna imagen, asignamos una imagen predeterminada
                design.ImageUrl = "/img/mono_trabajando.jpeg";
            }


            var success = await _designService.AddDesignAsync(design);
            if (!success)
            {
                ModelState.AddModelError("", "Ocurrió un error al agregar el diseño.");
                return View(design);
            }

            TempData["Message"] = "Diseño creado con éxito.";
            return RedirectToAction("Index");
        }

        [HttpGet]        
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _designService.GetDesignByIdAsync(id);

            if (design == null)
            {
                return NotFound();
            }

            return View(design);
        }

        [HttpGet]
        public async Task<IActionResult> GetDesignDetails(Guid designId)
        {
            System.Console.WriteLine($"designId {designId}");

            var design = await _designService.GetDesignByIdAsync(designId);
            
            // Loggear el payload para depuración
            var designJson = System.Text.Json.JsonSerializer.Serialize(design, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine("design encontrado:");
            Console.WriteLine(designJson);
            return Json(new
            {
                design.Name,
                design.Description,
                design.Price,
                design.Comision,
                design.ImageUrl
            });
        }        
    }
}

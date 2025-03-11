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
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                design.ImageUrl = Path.Combine("img", fileName).Replace("\\", "/");
            }
            else
            {
                // Si no se subió ninguna imagen, asigna un valor predeterminado
                design.ImageUrl = "img/mono_trabajando.jpeg"; 
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
    }
}

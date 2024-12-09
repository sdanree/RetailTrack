using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models.Products;
using RetailTrack.Services;
using RetailTrack.Data;

namespace RetailTrack.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ApplicationDbContext context, ILogger<SettingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Settings/InitializeData")]
        public async Task<IActionResult> InitializeData()
        {
            Console.WriteLine("Algo no anda!!!!");
            _logger.LogInformation("InitializeData _ before");
            try
            {
                _logger.LogInformation("InitializeData _ CALLING");
                await SeedDataAsync();
                TempData["Message"] = "Datos inicializados correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("InitializeData _ CALLING");
                return BadRequest("Error al inicializar datos");
            }
            _logger.LogInformation("InitializeData _ Saliendo");
            return RedirectToAction("Index");
        }

        private async Task SeedDataAsync()
        {
            Console.WriteLine("SeedDataAsync _ Algo no anda!!!!");
            _logger.LogInformation("SeedDataAsync _ before");            
            // Limpia las tablas
            _context.Materials.RemoveRange(_context.Materials);
            _context.MaterialTypes.RemoveRange(_context.MaterialTypes);
            _context.Designs.RemoveRange(_context.Designs);
            await _context.SaveChangesAsync();

            // Init. MaterialTypes & Materials
            var materialTypes = new List<MaterialType>
            {
                new MaterialType
                {
                    Name = "Remera",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 10, Stock = 100},
                        new Material { Name = "Carbon", Cost = 12, Stock = 90 },
                        new Material { Name = "Gris Jaspe", Cost = 8, Stock = 120 },
                        new Material { Name = "Plata", Cost = 15, Stock = 70 },
                        new Material { Name = "Blanco", Cost = 10, Stock = 110 },
                        new Material { Name = "Arena", Cost = 9, Stock = 100 },
                        new Material { Name = "Beige", Cost = 10, Stock = 85 },
                        new Material { Name = "Chocolate", Cost = 14, Stock = 95 },
                        new Material { Name = "Marron", Cost = 12, Stock = 80 },
                        new Material { Name = "Ladrillo", Cost = 11, Stock = 75 },
                        new Material { Name = "Rojo", Cost = 13, Stock = 90 },
                        new Material { Name = "Ocre", Cost = 9, Stock = 100 },
                        new Material { Name = "Naranja", Cost = 10, Stock = 110 },
                        new Material { Name = "Mango", Cost = 10, Stock = 90 },
                        new Material { Name = "Canario", Cost = 11, Stock = 80 },
                        new Material { Name = "Lima", Cost = 10, Stock = 95 },
                        new Material { Name = "Jade", Cost = 14, Stock = 85 },
                        new Material { Name = "Olivo", Cost = 13, Stock = 75 },
                        new Material { Name = "Bosque", Cost = 15, Stock = 70 },
                        new Material { Name = "Marino", Cost = 12, Stock = 90 },
                        new Material { Name = "Delfin", Cost = 11, Stock = 85 },
                        new Material { Name = "Royal", Cost = 13, Stock = 95 },
                        new Material { Name = "Turquesa", Cost = 10, Stock = 100 },
                        new Material { Name = "Celeste", Cost = 8, Stock = 120 },
                        new Material { Name = "Azul Claro", Cost = 12, Stock = 85 },
                        new Material { Name = "Aqua", Cost = 9, Stock = 95 },
                        new Material { Name = "Rosa", Cost = 14, Stock = 70 },
                        new Material { Name = "Coral", Cost = 13, Stock = 90 },
                        new Material { Name = "Fucsia", Cost = 12, Stock = 100 },
                        new Material { Name = "Morado", Cost = 11, Stock = 80 },
                        new Material { Name = "Amarillo N", Cost = 10, Stock = 85 },
                        new Material { Name = "Verde N", Cost = 15, Stock = 75 },
                        new Material { Name = "Naranja N", Cost = 14, Stock = 95 },
                        new Material { Name = "Rosa N", Cost = 12, Stock = 80 },
                        new Material { Name = "Negro J", Cost = 11, Stock = 90 },
                        new Material { Name = "Marrón J", Cost = 10, Stock = 100 },
                        new Material { Name = "Rojo J", Cost = 9, Stock = 85 },
                        new Material { Name = "Jade J", Cost = 13, Stock = 95 },
                        new Material { Name = "Bosque J", Cost = 14, Stock = 75 },
                        new Material { Name = "Royal J", Cost = 12, Stock = 80 },
                        new Material { Name = "Turquesa J", Cost = 11, Stock = 70 },
                        new Material { Name = "Fucsia J", Cost = 13, Stock = 85 },
                        new Material { Name = "Morado J", Cost = 10, Stock = 100 },
                        new Material { Name = "Batik", Cost = 15, Stock = 90 },
                    }
                },
                new MaterialType
                {
                    Name = "Niño",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 10, Stock = 100 },
                        new Material { Name = "Carbon", Cost = 12, Stock = 90 },
                        new Material { Name = "Gris Jaspe", Cost = 8, Stock = 120 },
                        new Material { Name = "Blanco", Cost = 10, Stock = 110 },
                        new Material { Name = "Rojo", Cost = 13, Stock = 95 },
                        new Material { Name = "Naranja", Cost = 10, Stock = 90 },
                        new Material { Name = "Mango", Cost = 11, Stock = 85 },
                        new Material { Name = "Canario", Cost = 12, Stock = 80 },
                        new Material { Name = "Lima", Cost = 10, Stock = 100 },
                        new Material { Name = "Jade", Cost = 14, Stock = 75 },
                        new Material { Name = "Marino", Cost = 12, Stock = 85 },
                        new Material { Name = "Royal", Cost = 13, Stock = 95 },
                        new Material { Name = "Turquesa", Cost = 10, Stock = 100 },
                        new Material { Name = "Celeste", Cost = 8, Stock = 110 },
                        new Material { Name = "Rosa", Cost = 9, Stock = 105 },
                        new Material { Name = "Fucsia", Cost = 11, Stock = 80 },
                        new Material { Name = "Morado", Cost = 12, Stock = 85 },
                    }
                },
                new MaterialType
                {
                    Name = "Bebe",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 10, Stock = 100 },
                        new Material { Name = "Blanco", Cost = 9, Stock = 120 },
                        new Material { Name = "Rojo", Cost = 11, Stock = 90 },
                        new Material { Name = "Naranja", Cost = 10, Stock = 80 },
                        new Material { Name = "Canario", Cost = 12, Stock = 85 },
                        new Material { Name = "Lima", Cost = 8, Stock = 110 },
                        new Material { Name = "Marino", Cost = 13, Stock = 95 },
                        new Material { Name = "Royal", Cost = 14, Stock = 70 },
                        new Material { Name = "Turquesa", Cost = 10, Stock = 100 },
                        new Material { Name = "Celeste", Cost = 9, Stock = 105 },
                        new Material { Name = "Rosa", Cost = 10, Stock = 90 },
                        new Material { Name = "Fucsia", Cost = 11, Stock = 85 },
                    }
                },
                new MaterialType
                {
                    Name = "Buzo",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 12, Stock = 100 },
                        new Material { Name = "Gris Jaspe", Cost = 10, Stock = 90 },
                        new Material { Name = "Blanco", Cost = 11, Stock = 120 },
                        new Material { Name = "Rojo", Cost = 13, Stock = 85 },
                        new Material { Name = "Marino", Cost = 14, Stock = 95 },
                        new Material { Name = "Royal", Cost = 15, Stock = 80 },

                    }
                },
                new MaterialType
                {
                    Name = "Canguro Abierto",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 12, Stock = 100 },
                        new Material { Name = "Gris Jaspe", Cost = 10, Stock = 90 },
                        new Material { Name = "Blanco", Cost = 11, Stock = 120 },
                        new Material { Name = "Rojo", Cost = 13, Stock = 85 },
                        new Material { Name = "Marino", Cost = 14, Stock = 95 },
                        new Material { Name = "Royal", Cost = 15, Stock = 80 },

                    }
                },
                new MaterialType
                {
                    Name = "Canguro Cerrado",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 12, Stock = 100 },
                        new Material { Name = "Gris Jaspe", Cost = 10, Stock = 90 },
                        new Material { Name = "Blanco", Cost = 11, Stock = 120 },
                        new Material { Name = "Marrón", Cost = 13, Stock = 85 },
                        new Material { Name = "Rojo", Cost = 13, Stock = 95 },
                        new Material { Name = "Bosque", Cost = 14, Stock = 80 },
                        new Material { Name = "Marino", Cost = 15, Stock = 90 },
                        new Material { Name = "Royal", Cost = 16, Stock = 70 },

                    }
                },
                new MaterialType
                {
                    Name = "Gorro",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro", Cost = 12, Stock = 100 },
                        new Material { Name = "Beige", Cost = 10, Stock = 80 },
                        new Material { Name = "Canario", Cost = 11, Stock = 90 },
                        new Material { Name = "Jade", Cost = 14, Stock = 85 },
                        new Material { Name = "Azul Claro", Cost = 13, Stock = 95 },
                    }
                },
                new MaterialType
                {
                    Name = "Manga Larga",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Gris Jaspe", Cost = 10, Stock = 120 },
                        new Material { Name = "Blanco", Cost = 12, Stock = 140 },
                        new Material { Name = "Rojo", Cost = 15, Stock = 100 },
                        new Material { Name = "Mango", Cost = 13, Stock = 110 },
                        new Material { Name = "Jade", Cost = 14, Stock = 90 },
                        new Material { Name = "Marino", Cost = 11, Stock = 130 },
                        new Material { Name = "Royal", Cost = 16, Stock = 80 },

                    }
                },
                new MaterialType
                {
                    Name = "Tote",
                    Materials = new List<Material>{}
                },
                new MaterialType
                {
                    Name = "Super Tote",
                    Materials = new List<Material>{}
                }                  
            };

            // Init. Design
            var DesingsList = new List<Design>
            {
                new Design { Name = "Venya 1", Description = "", Comision = 0, Price = 650, ImageUrl = "" },
                new Design { Name = "Venya 2", Description = "", Comision = 0, Price = 750, ImageUrl = "" },
                new Design { Name = "Manochuecca 1", Description = "", Comision = 0, Price = 350, ImageUrl = "" },
                new Design { Name = "Mano hueca 2", Description = "", Comision = 100, Price = 550, ImageUrl = "" },
                new Design { Name = "Fido", Description = "", Comision = 0, Price = 450, ImageUrl = "" },
                new Design { Name = "Bazooka", Description = "", Comision = 0, Price = 450, ImageUrl = "" },
                new Design { Name = "Fake", Description = "", Comision = 0, Price = 550, ImageUrl = "" },
                new Design { Name = "Navidad", Description = "", Comision = 0, Price = 350, ImageUrl = "" },
                new Design { Name = "Nutria Vin", Description = "", Comision = 100, Price = 650, ImageUrl = "" },
                new Design { Name = "Nutria Space", Description = "", Comision = 100, Price = 650, ImageUrl = "" },
                new Design { Name = "Hongos", Description = "", Comision = 300, Price = 650, ImageUrl = "" },
                new Design { Name = "Margarita", Description = "", Comision = 300, Price = 650, ImageUrl = "" },
                new Design { Name = "Mojando", Description = "", Comision = 0, Price = 450, ImageUrl = "" },
                new Design { Name = "Moco", Description = "", Comision = 0, Price = 450, ImageUrl = "" },
                new Design { Name = "Bebe", Description = "", Comision = 0, Price = 550, ImageUrl = "" },
                new Design { Name = "Logo", Description = "", Comision = 0, Price = 350, ImageUrl = "" },
                new Design { Name = "Perro Flauta", Description = "", Comision = 0, Price = 350, ImageUrl = "" },
                new Design { Name = "Amics", Description = "", Comision = 0, Price = 450, ImageUrl = "" },
                new Design { Name = "Eco", Description = "", Comision = 0, Price = 350, ImageUrl = "" },
                new Design { Name = "Golf", Description = "", Comision = 0, Price = 850, ImageUrl = "" },
                new Design { Name = "Golf bordado", Description = "", Comision = 0, Price = 850, ImageUrl = "" },
            };

            // Add new data
            await _context.MaterialTypes.AddRangeAsync(materialTypes);
            await _context.Designs.AddRangeAsync(DesingsList);
            await _context.SaveChangesAsync();
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.Data;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    [Authorize(Policy = "UserAproved")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ApplicationDbContext context, ILogger<SettingsController> logger)
        {
            _context    = context;
            _logger     = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Settings/InitializeData")]
        public async Task<IActionResult> InitializeData()
        {
            _logger.LogInformation("InitializeData _ before");
            try
            {
                _logger.LogInformation("InitializeData _ CALLING");
                await SeedDataAsync();
                TempData["Message"] = "Datos inicializados correctamente.";
            }
            catch (Exception ex)
            {
               // _logger.LogInformation("InitializeData _ CALLING");
                return BadRequest("Error al inicializar datos");
            }
            _logger.LogInformation("InitializeData _ Saliendo");
            return RedirectToAction("Index");
        }

        private async Task SeedDataAsync()
        {
            Console.WriteLine("SeedDataAsync _ before");                   
            _logger.LogInformation("SeedDataAsync _ before");            
            // Limpia las tablas
            _context.Products.RemoveRange(_context.Products);
            _context.ProductStatuses.RemoveRange(_context.ProductStatuses);
            _context.MaterialSizes.RemoveRange(_context.MaterialSizes);
            _context.Materials.RemoveRange(_context.Materials);
            _context.MaterialTypes.RemoveRange(_context.MaterialTypes);
            _context.Sizes.RemoveRange(_context.Sizes);
            _context.Designs.RemoveRange(_context.Designs);
            _context.PaymentMethods.RemoveRange(_context.PaymentMethods);
            await _context.SaveChangesAsync();
            
            var random = new Random();
            
            // Init. ProductSize 
            var Sizes = new List<Size>
            {
                new Size { Size_Id = 1, Size_Name = "S" },
                new Size { Size_Id = 2, Size_Name = "M" },
                new Size { Size_Id = 3, Size_Name = "L" },
                new Size { Size_Id = 4, Size_Name = "XL" },
                new Size { Size_Id = 5, Size_Name = "XXL" },
                new Size { Size_Id = 6, Size_Name = "XXXL" },
                new Size { Size_Id = 7, Size_Name = "1 año" },
                new Size { Size_Id = 8, Size_Name = "2 años" },
                new Size { Size_Id = 9, Size_Name = "3 años" }
            };

            // Init. ProductStatus
            var productStatuses = new List<ProductStatus>
            {
                new ProductStatus { Status_Id = 1, Status_Name = "pedido" },
                new ProductStatus { Status_Id = 2, Status_Name = "taller" },
                new ProductStatus { Status_Id = 3, Status_Name = "estampado" },
                new ProductStatus { Status_Id = 4, Status_Name = "Planchado" },
                new ProductStatus { Status_Id = 5, Status_Name = "empacado" },
                new ProductStatus { Status_Id = 6, Status_Name = "entregado" },
                new ProductStatus { Status_Id = 7, Status_Name = "proovedor" }
            };

            // Init. MaterialTypes & Materials
            var materialTypes = new List<MaterialType>
            {
                new MaterialType
                {
                    Name = "Remera",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro" },
                        new Material { Name = "Carbon" },
                        new Material { Name = "Gris Jaspe" },
                        new Material { Name = "Plata" },
                        new Material { Name = "Blanco" },
                        new Material { Name = "Arena" },
                        new Material { Name = "Beige" },
                        new Material { Name = "Chocolate" },
                        new Material { Name = "Marron" },
                        new Material { Name = "Ladrillo" },
                        new Material { Name = "Rojo" },
                        new Material { Name = "Ocre" },
                        new Material { Name = "Naranja" },
                        new Material { Name = "Mango" },
                        new Material { Name = "Canario" },
                        new Material { Name = "Lima" },
                        new Material { Name = "Jade" },
                        new Material { Name = "Olivo" },
                        new Material { Name = "Bosque" },
                        new Material { Name = "Marino" },
                        new Material { Name = "Delfin" },
                        new Material { Name = "Royal" },
                        new Material { Name = "Turquesa" },
                        new Material { Name = "Celeste" },
                        new Material { Name = "Azul Claro" },
                        new Material { Name = "Aqua" },
                        new Material { Name = "Rosa" },
                        new Material { Name = "Coral" },
                        new Material { Name = "Fucsia" },
                        new Material { Name = "Morado" },
                        new Material { Name = "Amarillo N" },
                        new Material { Name = "Verde N" },
                        new Material { Name = "Naranja N" },
                        new Material { Name = "Rosa N" },
                        new Material { Name = "Negro J" },
                        new Material { Name = "Marrón J" },
                        new Material { Name = "Rojo J" },
                        new Material { Name = "Jade J" },
                        new Material { Name = "Bosque J" },
                        new Material { Name = "Royal J" },
                        new Material { Name = "Turquesa J" },
                        new Material { Name = "Fucsia J" },
                        new Material { Name = "Morado J" },
                        new Material { Name = "Batik" },

                    }
                },
                new MaterialType
                {
                    Name = "Niño",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro" },
                        new Material { Name = "Carbon" },
                        new Material { Name = "Gris Jaspe" },
                        new Material { Name = "Blanco" },
                        new Material { Name = "Rojo" },
                        new Material { Name = "Naranja" },
                        new Material { Name = "Mango" },
                        new Material { Name = "Canario" },
                        new Material { Name = "Lima" },
                        new Material { Name = "Jade" },
                        new Material { Name = "Marino" },
                        new Material { Name = "Royal" },
                        new Material { Name = "Turquesa" },
                        new Material { Name = "Celeste" },
                        new Material { Name = "Rosa" },
                        new Material { Name = "Fucsia" },
                        new Material { Name = "Morado" },

                    }
                },
                new MaterialType
                {
                    Name = "Bebe",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro" },
                        new Material { Name = "Blanco" },
                        new Material { Name = "Rojo" },
                        new Material { Name = "Naranja" },
                        new Material { Name = "Canario" },
                        new Material { Name = "Lima" },
                        new Material { Name = "Marino" },
                        new Material { Name = "Royal" },
                        new Material { Name = "Turquesa" },
                        new Material { Name = "Celeste" },
                        new Material { Name = "Rosa" },
                        new Material { Name = "Fucsia" },
                    }
                },
                new MaterialType
                {
                    Name = "Buzo",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro"},
                        new Material { Name = "Gris Jaspe"},
                        new Material { Name = "Blanco"},
                        new Material { Name = "Rojo"},
                        new Material { Name = "Marino"},
                        new Material { Name = "Royal"},

                    }
                },
                new MaterialType
                {
                    Name = "Canguro Abierto",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro"},
                        new Material { Name = "Gris Jaspe"},
                        new Material { Name = "Blanco"},
                        new Material { Name = "Rojo"},
                        new Material { Name = "Marino"},
                        new Material { Name = "Royal"},                    }
                },
                new MaterialType
                {
                    Name = "Canguro Cerrado",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro"},
                        new Material { Name = "Gris Jaspe"},
                        new Material { Name = "Blanco"},
                        new Material { Name = "Marrón"},
                        new Material { Name = "Rojo"},
                        new Material { Name = "Bosque"},
                        new Material { Name = "Marino"},
                        new Material { Name = "Royal"},

                    }
                },
                new MaterialType
                {
                    Name = "Gorro",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Negro"},
                        new Material { Name = "Beige"},
                        new Material { Name = "Canario"},
                        new Material { Name = "Jade"},
                        new Material { Name = "Azul Claro"},
                    }
                },
                new MaterialType
                {
                    Name = "Manga Larga",
                    Materials = new List<Material>
                    {
                        new Material { Name = "Gris Jaspe"},
                        new Material { Name = "Blanco"},
                        new Material { Name = "Rojo"},
                        new Material { Name = "Mango"},
                        new Material { Name = "Jade"},
                        new Material { Name = "Marino"},
                        new Material { Name = "Royal"},

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

            foreach (var materialType in materialTypes)
            {
                foreach (var material in materialType.Materials)
                {
                    // Asocia entre 1 y 5 talles a cada material
                    int numberOfSizes = random.Next(1, 6);
                    material.MaterialSizes = new List<MaterialSize>();
                    var assignedSizes = new HashSet<int>();

                    for (int i = 0; i < numberOfSizes; i++)
                    {
                        int sizeId;
                        do
                        {
                            sizeId = Sizes[random.Next(Sizes.Count)].Size_Id;
                        } while (!assignedSizes.Add(sizeId));

                        material.MaterialSizes.Add(new MaterialSize
                        {
                            Id = Guid.NewGuid(),
                            MaterialId = material.Id,
                            SizeId = sizeId,
                            Stock = random.Next(10, 101),
                            Cost = random.Next(5, 21)
                        });
                    }

                }
            }            

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

            // Init. PaymentMethod
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { PaymentMethodId = 1, Name = "Trans Itau" },
                new PaymentMethod { PaymentMethodId = 2, Name = "Trans Otro" },
                new PaymentMethod { PaymentMethodId = 3, Name = "MP" },
                new PaymentMethod { PaymentMethodId = 4, Name = "PayPal" },
                new PaymentMethod { PaymentMethodId = 5, Name = "Efectivo" }
            };

            // Add new data
            await _context.Sizes.AddRangeAsync(Sizes);
            await _context.ProductStatuses.AddRangeAsync(productStatuses);
            await _context.MaterialTypes.AddRangeAsync(materialTypes);
            await _context.Designs.AddRangeAsync(DesingsList);
            await _context.PaymentMethods.AddRangeAsync(paymentMethods);
            await _context.SaveChangesAsync();
        }

    }
}

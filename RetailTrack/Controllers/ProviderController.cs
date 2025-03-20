using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.ViewModels;
using RetailTrack.Helpers;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    [Authorize(Policy = "UserAproved")]
    public class ProviderController : Controller
    {
        private readonly ProviderService _providerService;

        public ProviderController(ProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProviderViewModel model)
        {
            Console.WriteLine("Create provider - Start");
            string modelJson = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("📌 Modelo recibido en JSON antes de validación:");
            Console.WriteLine(modelJson); 

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Create provider - Invalid Model");
                
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error de validación: {error.ErrorMessage}");
                }

                return Json(new { success = false, message = "Datos inválidos. Verifique los campos ingresados." });
            }

            try
            {
                var newProvider = new Provider
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    BusinessName = model.BusinessName,
                    Phone = model.Phone,
                    RUT = model.RUT,
                    Address = model.Address,
                    Description = model.Description
                };

                await _providerService.AddProviderAsync(newProvider);

                // Guardar el nuevo proveedor en sesión
                HttpContext.Session.SetObjectAsJson("SelectedProviderDetails", newProvider);

                return Json(new
                {
                    success = true,
                    providerId = newProvider.Id,
                    providerName = newProvider.Name
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar proveedor: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error interno del servidor." });
            }
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

    }
}

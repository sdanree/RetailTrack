using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using System.Collections.Generic;

namespace RetailTrack.Controllers
{
    public class MovementController : Controller
    {
        private static List<Movement> Movements = new List<Movement>();

        public IActionResult Index()
        {
            return View(Movements);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movement movement)
        {
            Movements.Add(movement); // El Id se asigna automáticamente
            TempData["Message"] = "Movimiento creado con éxito.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(Guid id)
        {
            var movement = Movements.Find(m => m.Id == id);
            if (movement == null) return NotFound();
            return View(movement);
        }
    }
}

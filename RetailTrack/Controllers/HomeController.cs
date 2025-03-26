using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.ViewModels;
using RetailTrack.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RetailTrack.Controllers
{
    // [Authorize(Policy = "UserAproved")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public IActionResult Index()
        {
            Console.WriteLine($"🔹 Usuario autenticado: {User.Identity?.IsAuthenticated}");

            var userClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            Console.WriteLine("🔹 Claims del usuario:");
            foreach (var claim in userClaims)
            {
                Console.WriteLine($"   {claim}");
            }
                        
            var userProfile = new
            {
                Name = User.Identity?.Name ?? "Usuario Desconocido",
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "No disponible",
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value ?? "/images/default-profile.png"
            };
            
            var model = new HomeIndexViewModel{
                UserEmail = userProfile.EmailAddress
            };

            return View(model);
        }

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class UserProfile
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string ProfileImage { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RetailTrack.Models;
using RetailTrack.Services;
using RetailTrack.Data;

namespace RetailTrack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

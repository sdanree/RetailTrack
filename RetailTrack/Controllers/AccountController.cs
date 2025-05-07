using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RetailTrack.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return Challenge(authenticationProperties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 1) Notifica a Keycloak que cierre la sesión OIDC
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // 2) Luego elimina la cookie de sesión local
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 3) Redirige al Home
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("/signout-callback-oidc")]
        public IActionResult SignedOutCallback()
        {
            // Keycloak redirige aquí tras cerrar sesión
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "UserApproved")]
        public IActionResult Profile()
        {
            return View(new
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Callback()
        {
            return User.Identity.IsAuthenticated
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

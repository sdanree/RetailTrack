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
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl },
                             OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // 1) Notifica a Keycloak que cierre sesión
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            // 2) Elimina cookie local
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // 3) Redirige al home
            return RedirectToAction("Index", "Home");
        }

        // Recibe la redirección tras logout en Keycloak
        [AllowAnonymous]
        [HttpGet("/signout-callback-oidc")]
        public IActionResult SignedOutCallback()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "UserApproved")]
        public IActionResult Profile()
        {
            return View(new
            {
                Name         = User.Identity.Name,
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
        public IActionResult AccessDenied() => View();
    }
}

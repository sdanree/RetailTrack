using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;

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
    public IActionResult Logout()
    {
        var returnUrl = Url.Action("Login", "Account", null, Request.Scheme);
        var auth0Domain = _configuration["Auth0:Domain"];
        var auth0ClientId = _configuration["Auth0:ClientId"];

        var auth0LogoutUrl = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}&returnTo={Uri.EscapeDataString(returnUrl)}";

        return Redirect(auth0LogoutUrl);
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
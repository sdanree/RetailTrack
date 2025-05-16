using Microsoft.EntityFrameworkCore;
using RetailTrack.Data;
using RetailTrack.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Sólo para producción: deshabilita HTTP/2 y fuerza TLS1.2
if (!builder.Environment.IsDevelopment())
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
}

var environment = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var kc = builder.Configuration.GetSection("Authentication:Keycloak");
        options.Authority = kc["Authority"];
        options.RequireHttpsMetadata = bool.Parse(kc["RequireHttpsMetadata"] ?? "true");
        options.ClientId = kc["ClientId"];
        options.ClientSecret = kc["ClientSecret"];
        options.CallbackPath = kc["CallbackPath"];
        options.ResponseType = kc["ResponseType"];
        options.SaveTokens = bool.Parse(kc["SaveTokens"] ?? "true");

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RoleClaimType = ClaimTypes.Role
        };

        // Mapeo de claims
        options.ClaimActions.DeleteClaim("roles");
        options.ClaimActions.MapJsonKey("roles", "roles");
        options.ClaimActions.MapUniqueJsonKey("roles", "roles");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

        // Logs para debug
        options.Events.OnTokenValidated = ctx =>
        {
            var id = (ClaimsIdentity)ctx.Principal.Identity;
            Console.WriteLine("=== Claims recibidos ===");
            foreach (var c in id.Claims)
                Console.WriteLine($"  {c.Type}: {c.Value}");
            return Task.CompletedTask;
        };
        options.Events.OnRemoteFailure = ctx =>
        {
            Console.WriteLine($"OpenID error: {ctx.Failure?.Message}");
            ctx.Response.Redirect("/Home/Error");
            ctx.HandleResponse();
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("UserApproved", p => p.RequireClaim("roles", "UserAproved"));
});

builder.Services.ConfigureApplicationCookie(o =>
{
    o.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
     .EnableSensitiveDataLogging()
     .LogTo(Console.WriteLine, LogLevel.Information)
);

// <-- Aquí registras tus servicios y MVC, sessions, logging, etc. -->

builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opts.KnownNetworks.Clear();
    opts.KnownProxies.Clear();
});

var app = builder.Build();

// **IMPORTANTE:** UseForwardedHeaders debe ir **antes** de UseAuthentication()
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    var publicOrigin = builder.Configuration["App:PublicOrigin"];
    if (!string.IsNullOrWhiteSpace(publicOrigin))
    {
        var uri = new Uri(publicOrigin);
        app.Use(async (ctx, next) =>
        {
            ctx.Request.Scheme = uri.Scheme;
            ctx.Request.Host = uri.IsDefaultPort
                ? new HostString(uri.Host)
                : new HostString(uri.Host, uri.Port);
            await next();
        });
    }

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

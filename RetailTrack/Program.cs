using Microsoft.EntityFrameworkCore;
using RetailTrack.Data; 
using RetailTrack.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.HttpOverrides;



var builder = WebApplication.CreateBuilder(args);

//Fix para handshake SSL con Keycloak detrás de NGINX (ECDSA + proxy)
if (!builder.Environment.IsDevelopment())
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
}

var environment = builder.Environment.EnvironmentName;

// Configura el DbContext con MySQL
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
    
// Obtener la cadena de conexión según el entorno
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
        var keycloakConfig = builder.Configuration.GetSection("Authentication:Keycloak");

        options.Authority            = keycloakConfig["Authority"];
        options.RequireHttpsMetadata = bool.Parse(keycloakConfig["RequireHttpsMetadata"] ?? "true");
        options.ClientId             = keycloakConfig["ClientId"];
        options.ClientSecret         = keycloakConfig["ClientSecret"];
        options.CallbackPath         = keycloakConfig["CallbackPath"];
        options.ResponseType         = keycloakConfig["ResponseType"];
        options.SaveTokens           = bool.Parse(keycloakConfig["SaveTokens"] ?? "true");

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true
        };

        // Mapeo de claims personalizados
        options.ClaimActions.DeleteClaim("roles");
        options.ClaimActions.MapJsonKey("roles", "roles");
        options.ClaimActions.MapUniqueJsonKey("roles", "roles");

        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

        // Verificar claims en consola
        options.Events.OnTokenValidated = context =>
        {
            var identity = (ClaimsIdentity)context.Principal.Identity;

            Console.WriteLine(" Claims del Usuario Autenticado:");
            foreach (var claim in identity.Claims)
            {
                Console.WriteLine($"   {claim.Type}: {claim.Value}");
            }

            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAproved", policy =>
        policy.RequireClaim("roles", "UserAproved"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; 
});

// Configura el DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine, LogLevel.Information)
);

// Otros servicios
builder.Services.AddScoped<ProductService>(); 
builder.Services.AddScoped<MovementService>();
builder.Services.AddScoped<DesignService>();
builder.Services.AddScoped<MaterialTypeService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<SizeService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<PurchaseOrderService>();

// Configura Razor Runtime Compilation
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Configura la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de vida de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

// 🔄 Reenvío de encabezados (debe ir antes que nada relacionado con routing/auth)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
    ForwardLimit = null,
    RequireHeaderSymmetry = false,
    KnownNetworks = { },
    KnownProxies = { }
});

// Manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 🔒 Redirección HTTPS
app.UseHttpsRedirection();

// Archivos estáticos
app.UseStaticFiles();

// Ruteo
app.UseRouting();

// Sesión
app.UseSession();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Logging de todas las solicitudes (opcional)
/*
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} | Scheme: {context.Request.Scheme}");
    await next();
});
*/

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"EXCEPCIÓN EN REQUEST: {context.Request.Path}");
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"STACKTRACE: {ex.StackTrace}");
        throw;
    }
});

app.Run();
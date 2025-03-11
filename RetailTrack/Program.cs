using Microsoft.EntityFrameworkCore;
using RetailTrack.Data; 
using RetailTrack.Services;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

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
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        var auth0Audience = builder.Configuration["Auth0:Audience"];
        options.ResponseType = "code";

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.CallbackPath = "/callback";
        options.SignedOutCallbackPath = "/logout";
        options.SaveTokens = true;
        options.UseTokenLifetime = true;

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidAudience = builder.Configuration["Auth0:ClientId"],
            ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}/"
        };

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                Console.WriteLine("🔹 Enviando solicitud de autenticación con audience:");
                Console.WriteLine($"   Audience: {auth0Audience}");

                context.ProtocolMessage.SetParameter("audience", auth0Audience);
                return Task.CompletedTask;
            }
        };

        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        options.ClaimActions.MapJsonKey("picture", "picture");        
        options.ClaimActions.MapJsonKey("roles", $"https://{builder.Configuration["Auth0:Domain"]}/claims/roles");
    });

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnTokenValidated = context =>
    {
        var claims = context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
        Console.WriteLine("🔹 Claims del Usuario Autenticado:");
        claims.ForEach(c => Console.WriteLine($"   {c}"));

        return Task.CompletedTask;
    };
});    

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAproved", policy =>
        policy.RequireClaim("https://retailtrack.com/roles", "UserAproved"));
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

// Middleware para registrar todas las solicitudes en consola
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agrega el middleware para habilitar la sesión
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

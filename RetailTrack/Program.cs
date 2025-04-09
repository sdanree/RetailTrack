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
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };

        options.ClaimActions.DeleteClaim("roles");
        options.ClaimActions.MapJsonKey("roles", "roles");
        options.ClaimActions.MapUniqueJsonKey("roles", "roles");

        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

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
    options.AddPolicy("UserApproved", policy =>
        policy.RequireClaim("roles", "UserAproved"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; 
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine, LogLevel.Information)
);

builder.Services.AddScoped<ProductService>(); 
builder.Services.AddScoped<MovementService>();
builder.Services.AddScoped<DesignService>();
builder.Services.AddScoped<MaterialTypeService>();
builder.Services.AddScoped<MaterialService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<SizeService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<PurchaseOrderService>();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
    ForwardLimit = null,
    RequireHeaderSymmetry = false,
    KnownNetworks = { },
    KnownProxies = { }
});

//  Leer publicOrigin para ajustar Scheme y Host según el entorno
if (!app.Environment.IsDevelopment())
{
    var publicOrigin = builder.Configuration.GetSection("App")["PublicOrigin"];
    if (!string.IsNullOrEmpty(publicOrigin))
    {
        var uri = new Uri(publicOrigin);
        app.Use(async (context, next) =>
        {
            context.Request.Scheme = uri.Scheme;
            //context.Request.Host = new HostString(uri.Host, (uri.Port == 80 || uri.Port == 443) ? 0 : uri.Port);
            context.Request.Host = uri.IsDefaultPort ? new HostString(uri.Host) : new HostString(uri.Host, uri.Port);
            await next();
        });
    }
}

if (!app.Environment.IsDevelopment())
{
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

using Microsoft.EntityFrameworkCore;
using RetailTrack.Data; 
using RetailTrack.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configuración de entorno, tiempo y PII
if (!builder.Environment.IsDevelopment())
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
}

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
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
        options.Cookie.SameSite   = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly    = true;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var kc = builder.Configuration.GetSection("Authentication:Keycloak");
        options.Authority            = kc["Authority"];
        options.RequireHttpsMetadata = bool.Parse(kc["RequireHttpsMetadata"] ?? "true");
        options.ClientId             = kc["ClientId"];
        options.ClientSecret         = kc["ClientSecret"];
        options.CallbackPath         = kc["CallbackPath"];
        options.ResponseType         = kc["ResponseType"];
        options.SaveTokens           = bool.Parse(kc["SaveTokens"] ?? "true");

        // <-- Nuevos ajustes para logout y cookies OIDC -->
        options.SignedOutCallbackPath = "/signout-callback-oidc";
        options.CorrelationCookie.SameSite = SameSiteMode.None;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.NonceCookie.SameSite       = SameSiteMode.None;
        options.NonceCookie.SecurePolicy   = CookieSecurePolicy.Always;
        // <-- fin ajustes -->

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer           = true,
            ValidateAudience         = false,
            ValidateLifetime         = true,
            RoleClaimType            = ClaimTypes.Role
        };

        // Logging de tokens y errores
        options.Events.OnTokenValidated = context =>
        {
            var id = (ClaimsIdentity)context.Principal.Identity;
            Console.WriteLine("Claims validados:");
            foreach (var c in id.Claims) Console.WriteLine($" - {c.Type}: {c.Value}");
            return Task.CompletedTask;
        };
        options.Events.OnRemoteFailure = context =>
        {
            Console.WriteLine($"OpenID error: {context.Failure?.Message}");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(opts =>
    opts.AddPolicy("UserApproved", p => p.RequireClaim("roles", "UserApproved")));

builder.Services.ConfigureApplicationCookie(opts =>
    opts.AccessDeniedPath = "/Account/AccessDenied");

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<ProductService>();
// … otros servicios …

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential = true;
});

// Forwarded headers para HTTPS detrás de proxy
builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
    opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    opts.KnownNetworks.Clear();
    opts.KnownProxies.Clear();
});

var app = builder.Build();

// Aplicar forwarded headers antes de auth
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders   = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
    RequireHeaderSymmetry = false,
    ForwardLimit       = null
});

// Ajuste de PublicOrigin en producción
if (!app.Environment.IsDevelopment())
{
    var publicOrigin = builder.Configuration["App:PublicOrigin"];
    if (!string.IsNullOrEmpty(publicOrigin))
    {
        var uri = new Uri(publicOrigin);
        app.Use((ctx, next) =>
        {
            ctx.Request.Scheme = uri.Scheme;
            ctx.Request.Host   = uri.IsDefaultPort
                                 ? new HostString(uri.Host)
                                 : new HostString(uri.Host, uri.Port);
            return next();
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
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

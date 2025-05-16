using Microsoft.AspNetCore.Authentication;              // <-- Importa las extensiones
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetailTrack.Data;
using RetailTrack.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Deshabilita HTTP/2 y fuerza TLS1.2 fuera de Development
if (!builder.Environment.IsDevelopment())
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
    System.Net.ServicePointManager.SecurityProtocol =
        System.Net.SecurityProtocolType.Tls12;
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
}

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                 optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddAuthentication(opts =>
    {
        opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(opts =>
    {
        opts.Cookie.SameSite   = SameSiteMode.None;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opts.Cookie.HttpOnly   = true;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts =>
    {
        var kc = builder.Configuration.GetSection("Authentication:Keycloak");
        opts.Authority            = kc["Authority"];
        opts.RequireHttpsMetadata = bool.Parse(kc["RequireHttpsMetadata"]!);
        opts.ClientId             = kc["ClientId"];
        opts.ClientSecret         = kc["ClientSecret"];
        opts.CallbackPath         = kc["CallbackPath"];
        opts.ResponseType         = kc["ResponseType"];
        opts.SaveTokens           = bool.Parse(kc["SaveTokens"]!);

        opts.Scope.Add("openid");
        opts.Scope.Add("profile");
        opts.Scope.Add("email");
        opts.GetClaimsFromUserInfoEndpoint = true;

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = false,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ClockSkew                = TimeSpan.FromMinutes(5),   // <-- tolerancia extra
            RoleClaimType           = ClaimTypes.Role
        };

        // **Mapeo de claims**: elimina 'roles' duplicados y mapea nuevos
        opts.ClaimActions.DeleteClaim("roles");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Name,  "name");
        opts.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        opts.ClaimActions.MapUniqueJsonKey("roles",    "roles");

        opts.Events.OnRemoteFailure = ctx =>
        {
            Console.WriteLine($"OpenID error: {ctx.Failure?.Message}");
            ctx.HandleResponse();
            ctx.Response.Redirect("/Home/Error");
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(o =>
    o.AddPolicy("UserApproved", p => p.RequireClaim("roles", "UserAproved"))
);

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseMySql(conn, ServerVersion.AutoDetect(conn))
     .EnableSensitiveDataLogging()
     .LogTo(Console.WriteLine)
);

// … resto de servicios (MVC, sesiones, logging, etc.) …

// Configura proxy headers **antes** de UseAuthentication()
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                        | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();
});

var app = builder.Build();
app.UseForwardedHeaders();

// Ajusta Scheme/Host si usas PublicOrigin
if (!app.Environment.IsDevelopment())
{
    var origin = builder.Configuration["App:PublicOrigin"];
    if (!string.IsNullOrWhiteSpace(origin))
    {
        var uri = new Uri(origin);
        app.Use(async (ctx, next) =>
        {
            ctx.Request.Scheme = uri.Scheme;
            ctx.Request.Host   = uri.IsDefaultPort
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

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();

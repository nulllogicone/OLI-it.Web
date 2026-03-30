using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add DbContext with connection string from configuration
builder.Services.AddDbContext<OliItDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OliItDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// API endpoints for login/logout
app.MapPost("/api/login", async (HttpContext context, OliItDbContext db, ILogger<Program> logger) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();
        var stammName = form["StammName"].ToString();
        var unterschrift = form["Unterschrift"].ToString();

        logger.LogInformation("Login attempt for user: {StammName}", stammName);

        if (string.IsNullOrWhiteSpace(stammName) || string.IsNullOrWhiteSpace(unterschrift))
        {
            logger.LogWarning("Login failed: Missing credentials");
            return Results.Json(new { success = false, message = "Please enter both stamm name and password." });
        }

        var stamm = await db.Stamms
            .FirstOrDefaultAsync(s => s.Stamm1 == stammName && s.Unterschrift == unterschrift);

        if (stamm == null)
        {
            logger.LogWarning("Login failed: Invalid credentials for user {StammName}", stammName);
            return Results.Json(new { success = false, message = "Invalid stamm name or password." });
        }

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, stamm.StammGuid.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, stamm.Stamm1),
            new System.Security.Claims.Claim("Datei", stamm.Datei ?? string.Empty),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, stamm.EMail ?? string.Empty)
        };

        var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
            authProperties);

        logger.LogInformation("Login successful for user: {StammName}", stammName);
        return Results.Json(new { success = true });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during login");
        return Results.Json(new { success = false, message = "An error occurred. Please try again." });
    }
}).DisableAntiforgery();

app.MapPost("/api/logout", async (HttpContext context, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("User logout: {UserName}", context.User?.Identity?.Name);
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Json(new { success = true });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during logout");
        return Results.Json(new { success = false, message = "An error occurred during logout." });
    }
}).DisableAntiforgery();

app.Run();

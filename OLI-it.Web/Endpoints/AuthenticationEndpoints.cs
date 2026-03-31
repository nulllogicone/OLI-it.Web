using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using System.Security.Claims;

namespace OLI_it.Web.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", LoginAsync)
            .RequireRateLimiting("login")
            .DisableAntiforgery();

        app.MapPost("/api/logout", LogoutAsync)
            .DisableAntiforgery();
    }

    private static async Task<IResult> LoginAsync(
        HttpContext context,
        OliItDbContext db,
        ILogger<Program> logger)
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, stamm.StammGuid.ToString()),
                new Claim(ClaimTypes.Name, stamm.Stamm1),
                new Claim("Datei", stamm.Datei ?? string.Empty),
                new Claim(ClaimTypes.Email, stamm.EMail ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            logger.LogInformation("Login successful for user: {StammName}", stammName);
            return Results.Json(new { success = true, stammGuid = stamm.StammGuid });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login");
            return Results.Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext context,
        ILogger<Program> logger)
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
    }
}

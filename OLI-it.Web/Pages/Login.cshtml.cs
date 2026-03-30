using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using System.Security.Claims;

namespace OLI_it.Web.Pages;

public class LoginModel : PageModel
{
    private readonly OliItDbContext _context;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(OliItDbContext context, ILogger<LoginModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public string? StammName { get; set; }

    [BindProperty]
    public string? Unterschrift { get; set; }

    public IActionResult OnGet()
    {
        // Redirect to home if already authenticated
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {StammName}", StammName);

            if (string.IsNullOrWhiteSpace(StammName) || string.IsNullOrWhiteSpace(Unterschrift))
            {
                _logger.LogWarning("Login failed: Missing credentials. StammName: '{StammName}', Unterschrift: '{HasUnterschrift}'", 
                    StammName, !string.IsNullOrWhiteSpace(Unterschrift));
                return new JsonResult(new { success = false, message = "Please enter both stamm name and password." });
            }

            var stamm = await _context.Stamms
                .FirstOrDefaultAsync(s => s.Stamm1 == StammName && s.Unterschrift == Unterschrift);

            if (stamm == null)
            {
                _logger.LogWarning("Login failed: Invalid credentials for user {StammName}", StammName);
                return new JsonResult(new { success = false, message = "Invalid stamm name or password." });
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

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Login successful for user: {StammName}", StammName);
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {StammName}", StammName);
            return new JsonResult(new { success = false, message = "An error occurred. Please try again." });
        }
    }
}

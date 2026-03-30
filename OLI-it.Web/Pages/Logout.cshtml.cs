using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OLI_it.Web.Pages;

public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // Redirect to home
        return RedirectToPage("/Index");
    }

    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            _logger.LogInformation("User logout: {UserName}", User?.Identity?.Name);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return new JsonResult(new { success = false, message = "An error occurred during logout." });
        }
    }
}

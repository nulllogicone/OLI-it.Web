using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OLI_it.Web.Pages;

public class RegisterModel : PageModel
{
    private readonly OliItDbContext _context;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(OliItDbContext context, ILogger<RegisterModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public RegistrationInput Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return RedirectAuthenticatedUser();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return RedirectAuthenticatedUser();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedStammName = Input.StammName.Trim();
        var normalizedEmail = Input.EMail.Trim();

        var stammNameExists = await _context.Stamms.AnyAsync(s => s.Stamm1 == normalizedStammName);
        if (stammNameExists)
        {
            ModelState.AddModelError(nameof(Input.StammName), "This stamm name is already taken.");
        }

        var emailExists = await _context.Stamms.AnyAsync(s => s.EMail == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(Input.EMail), "This email is already registered.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var stamm = new OLI_it.Web.Models.Stamm
        {
            Stamm1 = normalizedStammName,
            EMail = normalizedEmail,
            Unterschrift = Input.Unterschrift,
            Datum = DateTime.UtcNow,
            ZuQid = 1
        };

        _context.Stamms.Add(stamm);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Registration failed due to persistence conflict for user: {StammName}", normalizedStammName);
            ModelState.AddModelError(string.Empty, "Registration could not be completed. Please try a different stamm name or email.");
            return Page();
        }

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, stamm.StammGuid.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Name, stamm.Stamm1),
            new System.Security.Claims.Claim("Datei", stamm.Datei ?? string.Empty),
            new System.Security.Claims.Claim(ClaimTypes.Email, stamm.EMail ?? string.Empty)
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

        _logger.LogInformation("Registration successful for user: {StammName}", stamm.Stamm1);
        return RedirectToPage("/Stamm/Index", new { id = stamm.StammGuid });
    }

    private IActionResult RedirectAuthenticatedUser()
    {
        var stammGuidClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(stammGuidClaim, out var stammGuid))
        {
            return RedirectToPage("/Stamm/Index", new { id = stammGuid });
        }

        return RedirectToPage("/Index");
    }

    public sealed class RegistrationInput
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Stamm Name")]
        public string StammName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string EMail { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Unterschrift { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Unterschrift), ErrorMessage = "Password and confirmation do not match.")]
        public string ConfirmUnterschrift { get; set; } = string.Empty;
    }
}

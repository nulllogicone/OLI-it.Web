using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;

namespace OLI_it.Web.Pages.Angler;

[Authorize]
public sealed class CreateModel(OliItDbContext context, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty] public AnglerInput Input { get; set; } = new();
    public Models.Stamm Stamm { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!await LoadOwnerAsync(cancellationToken)) return Forbid();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await LoadOwnerAsync(cancellationToken)) return Forbid();
        if (!ModelState.IsValid) return Page();

        var angler = new Models.Angler
        {
            StammGuid = Stamm.StammGuid,
            Angler1 = Input.EncodedTitle,
            Beschreibung = Input.EncodedDescription,
            Datum = DateTime.UtcNow
        };
        context.Anglers.Add(angler);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Could not create Angler for Stamm {StammId}", Stamm.StammGuid);
            ModelState.AddModelError(string.Empty, "We could not create your Angler. Please try again.");
            return Page();
        }

        return RedirectToPage("/Angler/Index", new { id = angler.AnglerGuid });
    }

    private async Task<bool> LoadOwnerAsync(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)) return false;

        var owner = await context.Stamms.AsNoTracking()
            .SingleOrDefaultAsync(s => s.StammGuid == ownerId, cancellationToken);
        if (owner == null) return false;

        Stamm = owner;
        ViewData["Sidebar"] = "_SidebarUnified";
        return true;
    }
}

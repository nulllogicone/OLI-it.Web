using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;

namespace OLI_it.Web.Pages.Angler;

[Authorize]
public sealed class EditModel(OliItDbContext context, ILogger<EditModel> logger) : PageModel
{
    [BindProperty] public AnglerInput Input { get; set; } = new();
    public Models.Angler Angler { get; private set; } = null!;
    public Models.Stamm Stamm => Angler.Stamm;

    public async Task<IActionResult> OnGetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;

        Input.Title = WebUtility.HtmlDecode(Angler.Angler1);
        Input.Description = WebUtility.HtmlDecode(Angler.Beschreibung);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;
        if (!ModelState.IsValid) return Page();

        Angler.Angler1 = Input.EncodedTitle;
        Angler.Beschreibung = Input.EncodedDescription;

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Could not update Angler {AnglerId}", id);
            ModelState.AddModelError(string.Empty, "We could not save your Angler. Please try again.");
            return Page();
        }

        return RedirectToPage("/Angler/Index", new { id });
    }

    private async Task<IActionResult?> LoadAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var ownerId)) return Forbid();

        var angler = await context.Anglers.Include(a => a.Stamm)
            .SingleOrDefaultAsync(a => a.AnglerGuid == id, cancellationToken);
        if (angler == null) return NotFound();
        if (angler.StammGuid != ownerId) return Forbid();

        Angler = angler;
        ViewData["Sidebar"] = "_SidebarUnified";
        return null;
    }
}

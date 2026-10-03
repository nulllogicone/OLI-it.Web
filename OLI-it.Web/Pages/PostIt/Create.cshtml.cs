using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages.PostIt;

[Authorize]
public sealed class CreateModel(OliItDbContext context, PostItCreationService creation,
    PostItCreationTicket tickets, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty] public CreatePostItInput Input { get; set; } = new();
    [BindProperty] public string Submission { get; set; } = "";
    public Models.Stamm? Stamm { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!await LoadAuthorAsync()) return Forbid();
        Submission = tickets.Issue(Stamm!.StammGuid);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await LoadAuthorAsync()) return Forbid();
        if (!tickets.TryRead(Submission, Stamm!.StammGuid, out var id))
        {
            ModelState.AddModelError(string.Empty, "This form has expired. Review your message and save again.");
            Submission = tickets.Issue(Stamm.StammGuid);
            return Page();
        }
        if (!ModelState.IsValid) return Page();
        try
        {
            await creation.CreateAsync(Stamm.StammGuid, id, Input, cancellationToken);
            TempData["MessageSaved"] = "Message saved.";
            return RedirectToPage("/PostIt/Index", new { id });
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Could not create PostIt {PostItId}", id);
            ModelState.AddModelError(string.Empty, "We could not save your message. Please try again.");
            return Page();
        }
    }

    private async Task<bool> LoadAuthorAsync()
    {
        ViewData["Sidebar"] = "_SidebarUnified";
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return false;
        Stamm = await context.Stamms.AsNoTracking().SingleOrDefaultAsync(s => s.StammGuid == id);
        return Stamm != null;
    }
}

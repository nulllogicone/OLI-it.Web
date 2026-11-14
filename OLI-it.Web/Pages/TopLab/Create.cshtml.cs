using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages.TopLab;

[Authorize]
public sealed class CreateModel(OliItDbContext context, PostItCreationTicket tickets, ILogger<CreateModel> logger)
    : PageModel
{
    [BindProperty] public AnswerInput Input { get; set; } = new();
    [BindProperty] public string Submission { get; set; } = "";
    public Models.PostIt PostIt { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;
        Submission = tickets.Issue(AuthorId, id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;
        if (!tickets.TryRead(Submission, AuthorId, out var answerId, id))
        {
            ModelState.Remove(nameof(Submission));
            Submission = tickets.Issue(AuthorId, id);
            ModelState.AddModelError(string.Empty, "This form has expired. Review your answer and save again.");
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        if (!await AnswerExistsAsync())
        {
            context.TopLabs.Add(new Models.TopLab
            {
                TopLabGuid = answerId,
                PostItGuid = id,
                StammGuid = AuthorId,
                Titel = Input.EncodedTitle,
                TopLab1 = Input.EncodedBody,
                Typ = "txt",
                Lohn = 0
            });

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // A concurrent retry may have saved this same submission.
                if (!await AnswerExistsAsync())
                {
                    logger.LogError(ex, "Could not create TopLab {AnswerId} for PostIt {PostItId}", answerId, id);
                    ModelState.AddModelError(string.Empty, "We could not save your answer. Please try again.");
                    return Page();
                }
            }
        }

        TempData["AnswerSaved"] = "Answer saved.";
        return RedirectToPage("/PostIt/TopLab", new { id });

        Task<bool> AnswerExistsAsync() => context.TopLabs.AnyAsync(
            answer => answer.TopLabGuid == answerId && answer.StammGuid == AuthorId && answer.PostItGuid == id,
            cancellationToken);
    }

    private Guid AuthorId { get; set; }

    private async Task<IActionResult?> LoadAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authorId) ||
            !await context.Stamms.AnyAsync(stamm => stamm.StammGuid == authorId, cancellationToken))
            return Forbid();

        var postIt = await context.PostIts.AsNoTracking()
            .SingleOrDefaultAsync(post => post.PostItGuid == id, cancellationToken);
        if (postIt == null) return NotFound();

        AuthorId = authorId;
        PostIt = postIt;
        ViewData["Sidebar"] = "_SidebarUnified";
        return null;
    }

}

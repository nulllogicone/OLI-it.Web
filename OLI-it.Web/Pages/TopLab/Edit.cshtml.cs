using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages.TopLab;

[Authorize]
public sealed class EditModel(OliItDbContext context, AzureBlobStorageService blobService, ILogger<EditModel> logger) : PageModel
{
    [BindProperty] public AnswerInput Input { get; set; } = new();
    [BindProperty] public IFormFile? DateiUpload { get; set; }
    [BindProperty] public string? SelectedImageUrl { get; set; }
    public Models.TopLab Answer { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;
        Input.Title = PostItText.Title(Answer.Titel);
        Input.Body = PostItText.Body(Answer.TopLab1, Answer.Typ);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (await LoadAsync(id, cancellationToken) is { } error) return error;
        if (!ModelState.IsValid) return Page();

        if (DateiUpload is { Length: > 0 } upload)
        {
            if (Path.GetFileName(upload.FileName).Length > 217)
            {
                ModelState.AddModelError(string.Empty, "Choose an image with a shorter file name.");
                return Page();
            }

            var result = await blobService.UploadImageAsync(Answer.StammGuid, upload);
            if (!result.Success || result.BlobName == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "We could not upload your image.");
                return Page();
            }

            Answer.Datei = $"/{result.BlobName}";
        }
        else if (!string.IsNullOrEmpty(SelectedImageUrl) && SelectedImageUrl != Answer.Datei)
        {
            var prefix = $"/{Answer.StammGuid}/";
            if (SelectedImageUrl.Length > 255 ||
                !SelectedImageUrl.StartsWith(prefix, StringComparison.Ordinal) ||
                Uri.UnescapeDataString(SelectedImageUrl[prefix.Length..]).Split('/')
                    .Any(segment => segment is "" or "." or ".." || segment.Contains('\\')))
            {
                ModelState.AddModelError(string.Empty, "Choose an image from your gallery.");
                return Page();
            }

            Answer.Datei = SelectedImageUrl;
        }

        Answer.Titel = Input.EncodedTitle;
        if (Input.Body != PostItText.Body(Answer.TopLab1, Answer.Typ))
        {
            Answer.TopLab1 = Input.EncodedBody;
            Answer.Typ = "txt";
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Could not update TopLab {AnswerId}", id);
            ModelState.AddModelError(string.Empty, "We could not save your answer. Please try again.");
            return Page();
        }

        TempData["AnswerSaved"] = "Answer updated.";
        return RedirectToPage("/TopLab/Index", new { id });
    }

    private async Task<IActionResult?> LoadAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var authorId)) return Forbid();

        var answer = await context.TopLabs.SingleOrDefaultAsync(t => t.TopLabGuid == id, cancellationToken);
        if (answer == null) return NotFound();
        if (answer.StammGuid != authorId) return Forbid();

        Answer = answer;
        ViewData["Sidebar"] = "_SidebarUnified";
        return null;
    }
}

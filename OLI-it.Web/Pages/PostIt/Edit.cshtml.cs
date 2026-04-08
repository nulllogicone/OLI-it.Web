using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Services;
using System.Security.Claims;

namespace OLI_it.Web.Pages.PostIt
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly OliItDbContext _context;
        private readonly AzureBlobStorageService _blobService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            OliItDbContext context,
            AzureBlobStorageService blobService,
            ILogger<EditModel> logger)
        {
            _context = context;
            _blobService = blobService;
            _logger = logger;
        }

        [BindProperty]
        public Models.PostIt PostIt { get; set; } = default!;

        [BindProperty]
        public IFormFile? DateiUpload { get; set; }

        [BindProperty]
        public string? SelectedImageUrl { get; set; }

        [BindProperty]
        public string? Titel { get; set; }

        [BindProperty]
        public string PostIt1 { get; set; } = null!;

        [BindProperty]
        public string? Url { get; set; }

        [BindProperty]
        public string Typ { get; set; } = null!;

        public Guid? AuthorStammGuid { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postit = await _context.PostIts.FirstOrDefaultAsync(m => m.PostItGuid == id);

            if (postit == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is the author of this PostIt
            var currentUserGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserGuid, out var userGuid))
            {
                _logger.LogWarning("Invalid user GUID: {UserId}", currentUserGuid);
                return Forbid();
            }

            // Find the author of this PostIt (StammZust = 1)
            var authorWurzel = await _context.Wurzelns
                .FirstOrDefaultAsync(w => w.PostItGuid == id.Value && w.StammZust == 1);

            if (authorWurzel == null || authorWurzel.StammGuid != userGuid)
            {
                _logger.LogWarning("Unauthorized edit attempt: User {UserId} tried to edit PostIt {PostItId}", currentUserGuid, id);
                return Forbid();
            }

            PostIt = postit;
            Titel = postit.Titel;
            PostIt1 = postit.PostIt1;
            Url = postit.Url;
            Typ = postit.Typ;
            AuthorStammGuid = authorWurzel.StammGuid;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is the author of this PostIt
            var currentUserGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserGuid, out var userGuid))
            {
                _logger.LogWarning("Invalid user GUID: {UserId}", currentUserGuid);
                return Forbid();
            }

            var postItToUpdate = await _context.PostIts.FirstOrDefaultAsync(m => m.PostItGuid == id);

            if (postItToUpdate == null)
            {
                return NotFound();
            }

            // Find the author of this PostIt (StammZust = 1)
            var authorWurzel = await _context.Wurzelns
                .FirstOrDefaultAsync(w => w.PostItGuid == id.Value && w.StammZust == 1);

            if (authorWurzel == null || authorWurzel.StammGuid != userGuid)
            {
                _logger.LogWarning("Unauthorized edit attempt: User {UserId} tried to edit PostIt {PostItId}", currentUserGuid, id);
                return Forbid();
            }

            // Update the fields
            postItToUpdate.Titel = Titel;
            postItToUpdate.PostIt1 = PostIt1;
            postItToUpdate.Url = Url;
            postItToUpdate.Typ = Typ;
            // Note: KooK is not updated here as it's calculated in the backend

            // Handle image selection or upload
            string? newImagePath = null;

            // Priority 1: New file upload
            if (DateiUpload != null && DateiUpload.Length > 0)
            {
                var uploadResult = await _blobService.UploadImageAsync(authorWurzel.StammGuid, DateiUpload);

                if (!uploadResult.Success)
                {
                    ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage ?? "An error occurred while uploading the file.");
                    PostIt = postItToUpdate;
                    AuthorStammGuid = authorWurzel.StammGuid;
                    return Page();
                }

                if (uploadResult.BlobName != null)
                {
                    newImagePath = $"/{uploadResult.BlobName}";
                    _logger.LogInformation("New image uploaded for PostIt {PostItId}: {BlobName}", id, uploadResult.BlobName);
                }
            }
            // Priority 2: Selected existing image
            else if (!string.IsNullOrEmpty(SelectedImageUrl))
            {
                newImagePath = SelectedImageUrl;
                _logger.LogInformation("Existing image selected for PostIt {PostItId}: {ImageUrl}", id, SelectedImageUrl);
            }

            // Update the Datei field if a new image was set
            if (newImagePath != null)
            {
                postItToUpdate.Datei = newImagePath;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("PostIt {PostItId} updated successfully", id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating PostIt {PostItId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while saving changes.");
                PostIt = postItToUpdate;
                AuthorStammGuid = authorWurzel.StammGuid;
                return Page();
            }

            return RedirectToPage("/PostIt/Index", new { id = id });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;
using OLI_it.Web.Services;
using System.Security.Claims;

namespace OLI_it.Web.Pages.Stamm
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly OliItDbContext _context;
        private readonly AzureBlobStorageService _blobService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            OliItDbContext context,
            AzureBlobStorageService blobService,
            IConfiguration configuration,
            ILogger<EditModel> logger)
        {
            _context = context;
            _blobService = blobService;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public Models.Stamm Stamm { get; set; } = default!;

        [BindProperty]
        public IFormFile? DateiUpload { get; set; }

        [BindProperty]
        public string? SelectedImageUrl { get; set; }

        [BindProperty]
        public string? Beschreibung { get; set; }

        [BindProperty]
        public string? Link { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is authorized to edit this Stamm
            var currentUserGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserGuid != id.ToString())
            {
                _logger.LogWarning("Unauthorized edit attempt: User {UserId} tried to edit Stamm {StammId}", currentUserGuid, id);
                return Forbid();
            }

            var stamm = await _context.Stamms.FirstOrDefaultAsync(m => m.StammGuid == id);

            if (stamm == null)
            {
                return NotFound();
            }

            Stamm = stamm;
            Beschreibung = stamm.Beschreibung;
            Link = stamm.Link;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Check if the logged-in user is authorized to edit this Stamm
            var currentUserGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserGuid != id.ToString())
            {
                _logger.LogWarning("Unauthorized edit attempt: User {UserId} tried to edit Stamm {StammId}", currentUserGuid, id);
                return Forbid();
            }

            var stammToUpdate = await _context.Stamms.FirstOrDefaultAsync(m => m.StammGuid == id);

            if (stammToUpdate == null)
            {
                return NotFound();
            }

            // Update the fields
            stammToUpdate.Beschreibung = Beschreibung;
            stammToUpdate.Link = Link;

            // Handle image selection or upload
            string? newImagePath = null;

            // Priority 1: New file upload
            if (DateiUpload != null && DateiUpload.Length > 0)
            {
                var uploadResult = await _blobService.UploadImageAsync(id.Value, DateiUpload);

                if (!uploadResult.Success)
                {
                    // Show the friendly error message to the user
                    ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage ?? "An error occurred while uploading the file.");
                    Stamm = stammToUpdate;
                    return Page();
                }

                if (uploadResult.BlobName != null)
                {
                    // Store only the relative path with leading slash: /{guid}/name.ext
                    newImagePath = $"/{uploadResult.BlobName}";
                    _logger.LogInformation("New image uploaded for Stamm {StammId}: {BlobName}", id, uploadResult.BlobName);
                }
            }
            // Priority 2: Selected existing image
            else if (!string.IsNullOrEmpty(SelectedImageUrl))
            {
                // Store the relative path (should already be in format /{guid}/name.ext)
                newImagePath = SelectedImageUrl;
                _logger.LogInformation("Existing image selected for Stamm {StammId}: {ImageUrl}", id, SelectedImageUrl);
            }

            // Update the Datei field if a new image was set
            if (newImagePath != null)
            {
                stammToUpdate.Datei = newImagePath;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Stamm {StammId} updated successfully", id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating Stamm {StammId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while saving changes.");
                Stamm = stammToUpdate;
                return Page();
            }

            return RedirectToPage("./Index", new { id = id });
        }
    }
}

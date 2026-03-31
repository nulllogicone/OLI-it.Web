using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.PostIt
{
    public class IndexModel : PageModel
    {
        private readonly OliItDbContext _context;

        public IndexModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public List<Models.Code>? PostItCodes { get; set; }
        public List<Models.TopLab>? PostItTopLabs { get; set; }
        public List<Models.Spiegel>? PostItSpiegel { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, Guid? stamm)
        {
            if (id == null)
            {
                return NotFound();
            }

            PostIt = await _context.PostIts
                .FirstOrDefaultAsync(m => m.PostItGuid == id);

            if (PostIt == null)
            {
                return NotFound();
            }

            // Load parent Stamm (author or follower)
            if (stamm.HasValue)
            {
                // Use provided Stamm (follower)
                Stamm = await _context.Stamms
                    .FirstOrDefaultAsync(s => s.StammGuid == stamm.Value);
            }
            else
            {
                // Find author Stamm (StammZust = 1)
                var authorWurzel = await _context.Wurzelns
                    .Include(w => w.Stamm)
                    .FirstOrDefaultAsync(w => w.PostItGuid == id.Value && w.StammZust == 1);

                Stamm = authorWurzel?.Stamm;
            }

            // Load PostIt's Codes
            PostItCodes = await _context.Codes
                .Where(c => c.PostItGuid == id.Value)
                .OrderByDescending(c => c.Gescannt)
                .Take(20)
                .ToListAsync();

            // Load counts for Codes (Spiegel = receivers)
            foreach (var codeItem in PostItCodes)
            {
                codeItem.Spiegels = await _context.Spiegels
                    .Where(s => s.CodeGuid == codeItem.CodeGuid)
                    .ToListAsync();
            }

            // Load PostIt's TopLabs
            PostItTopLabs = await _context.TopLabs
                .Include(t => t.Stamm)
                .Where(t => t.PostItGuid == id.Value)
                .OrderByDescending(t => t.Datum)
                .Take(20)
                .ToListAsync();

            // Load all Spiegel for this PostIt (via Codes)
            var codeGuids = PostItCodes.Select(c => c.CodeGuid).ToList();
            PostItSpiegel = await _context.Spiegels
                .Include(s => s.Angler)
                .Where(s => codeGuids.Contains(s.CodeGuid))
                .OrderByDescending(s => s.Zeit)
                .Take(50)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarPostIt";

            return Page();
        }
    }
}

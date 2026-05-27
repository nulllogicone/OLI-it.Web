using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;

namespace OLI_it.Web.Pages.PostIt
{
    public class FlowModel : PageModel
    {
        private readonly OliItDbContext _context;

        public FlowModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; private set; }
        public Models.PostIt? PostIt { get; private set; }
        public List<RecipientFlowItem> RecipientFlows { get; private set; } = new();
        public List<AnswerFlowItem> Answers { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PostIt = await _context.PostIts
                .FirstOrDefaultAsync(m => m.PostItGuid == id.Value);

            if (PostIt == null)
            {
                return NotFound();
            }

            var authorWurzel = await _context.Wurzelns
                .Include(w => w.Stamm)
                .FirstOrDefaultAsync(w => w.PostItGuid == id.Value && w.StammZust == 1);

            Stamm = authorWurzel?.Stamm;

            var matchedProfiles = await _context.Spiegels
                .Include(s => s.Angler)
                    .ThenInclude(a => a.Stamm)
                .Include(s => s.Code)
                .Where(s => s.Code.PostItGuid == id.Value)
                .OrderByDescending(s => s.Zeit)
                .ToListAsync();

            RecipientFlows = matchedProfiles
                .GroupBy(s => s.AnglerGuid)
                .Select(group => group
                    .OrderByDescending(s => s.Zeit ?? DateTime.MinValue)
                    .ThenByDescending(s => s.Gelesen ?? DateTime.MinValue)
                    .First())
                .OrderByDescending(s => s.Zeit ?? DateTime.MinValue)
                .Select(s => new RecipientFlowItem
                {
                    RecipientGuid = s.Angler.StammGuid,
                    RecipientName = s.Angler.Stamm?.Stamm1 ?? "Unknown recipient",
                    FilterProfileGuid = s.AnglerGuid,
                    FilterProfileName = s.Angler.Angler1,
                    FilterProfileDescription = s.Angler.Beschreibung,
                    Status = string.IsNullOrWhiteSpace(s.Status) ? "matched" : s.Status!,
                    MatchedAt = s.Zeit,
                    ReadAt = s.Gelesen
                })
                .ToList();

            Answers = await _context.TopLabs
                .Include(t => t.Stamm)
                .Where(t => t.PostItGuid == id.Value)
                .OrderBy(t => t.Datum)
                .Select(t => new AnswerFlowItem
                {
                    TopLabGuid = t.TopLabGuid,
                    Title = t.Titel,
                    Body = t.TopLab1,
                    AuthorName = t.Stamm.Stamm1,
                    Reward = t.Lohn,
                    CreatedAt = t.Datum
                })
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarUnified";

            return Page();
        }

        public sealed class RecipientFlowItem
        {
            public Guid RecipientGuid { get; init; }
            public string RecipientName { get; init; } = string.Empty;
            public Guid FilterProfileGuid { get; init; }
            public string FilterProfileName { get; init; } = string.Empty;
            public string? FilterProfileDescription { get; init; }
            public string Status { get; init; } = string.Empty;
            public DateTime? MatchedAt { get; init; }
            public DateTime? ReadAt { get; init; }
        }

        public sealed class AnswerFlowItem
        {
            public Guid TopLabGuid { get; init; }
            public string? Title { get; init; }
            public string Body { get; init; } = string.Empty;
            public string AuthorName { get; init; } = string.Empty;
            public decimal Reward { get; init; }
            public DateTime CreatedAt { get; init; }
        }
    }
}

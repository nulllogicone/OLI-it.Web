using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages;

public class GalleryModel : PageModel
{
    private readonly OliItDbContext _context;

    public GalleryModel(OliItDbContext context)
    {
        _context = context;
    }

    public List<GalleryItem> GalleryItems { get; set; } = new();

    public async Task OnGetAsync()
    {
        var items = new List<GalleryItem>();

        // Get latest Stamm items with Datei starting with /
        var stammItems = await _context.Stamms
            .Where(s => s.Datei != null && s.Datei.StartsWith("/"))
            .OrderByDescending(s => s.Datum)
            .Take(40)
            .Select(s => new GalleryItem
            {
                Guid = s.StammGuid,
                Type = "Stamm",
                Title = s.Stamm1,
                ImagePath = s.Datei,
                Date = s.Datum,
                Description = s.Beschreibung
            })
            .ToListAsync();

        items.AddRange(stammItems);

        // Get latest PostIt items with Datei starting with /
        var postItItems = await _context.PostIts
            .Where(p => p.Datei != null && p.Datei.StartsWith("/"))
            .OrderByDescending(p => p.Datum)
            .Take(40)
            .Select(p => new GalleryItem
            {
                Guid = p.PostItGuid,
                Type = "PostIt",
                Title = p.Titel ?? "Untitled",
                ImagePath = p.Datei,
                Date = p.Datum,
                Description = p.PostIt1
            })
            .ToListAsync();

        items.AddRange(postItItems);

        // Get latest TopLab items with Datei starting with /
        var topLabItems = await _context.TopLabs
            .Where(t => t.Datei != null && t.Datei.StartsWith("/"))
            .OrderByDescending(t => t.Datum)
            .Take(40)
            .Select(t => new GalleryItem
            {
                Guid = t.TopLabGuid,
                Type = "TopLab",
                Title = t.Titel ?? "Untitled",
                ImagePath = t.Datei,
                Date = t.Datum,
                Description = t.TopLab1
            })
            .ToListAsync();

        items.AddRange(topLabItems);

        // Sort all items by date descending and take top 40 for the gallery
        GalleryItems = items
            .OrderByDescending(i => i.Date)
            .Take(40)
            .ToList();
    }
}

public class GalleryItem
{
    public Guid Guid { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}

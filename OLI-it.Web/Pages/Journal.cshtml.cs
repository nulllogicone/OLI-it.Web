using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OLI_it.Web.Models;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages;

public class JournalModel(JournalService journalService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Filter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Count { get; set; } = JournalService.DefaultCount;

    public List<UnionJournale> Entries { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Entries = await journalService.GetLatestAsync(Filter, Count);
    }

    /// <summary>Maps Zeichen to the URL path segment for that entry type.</summary>
    public static string GetEntryUrl(UnionJournale entry) => entry.Zeichen switch
    {
        "P" => $"/postit/{entry.Guid}",
        "S" => $"/stamm/{entry.Guid}",
        "T" => $"/toplab/{entry.Guid}",
        "A" => $"/angler/{entry.Guid}",
        _ => "/"
    };

    /// <summary>Maps Zeichen to a human-readable label.</summary>
    public static string GetTypeLabel(string zeichen) => zeichen switch
    {
        "P" => "message",
        "S" => "author",
        "T" => "answer",
        "A" => "recipient",
        _ => zeichen
    };

    /// <summary>Maps Zeichen to the CSS modifier class.</summary>
    public static string GetTypeClass(string zeichen) => zeichen switch
    {
        "P" => "timeline-item-postit",
        "S" => "timeline-item-stamm",
        "T" => "timeline-item-toplab",
        "A" => "timeline-item-angler",
        _ => "timeline-item-unknown"
    };

    /// <summary>Formats a nullable date: same year → "26 Mar", older → "Feb 2024".</summary>
    public static string FormatDate(DateTime? datum)
    {
        if (datum is null) return "—";
        return datum.Value.Year == DateTime.Now.Year
            ? datum.Value.ToString("d MMM")
            : datum.Value.ToString("MMM yyyy");
    }
}

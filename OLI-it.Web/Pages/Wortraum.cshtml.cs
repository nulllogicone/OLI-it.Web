using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;
using Microsoft.AspNetCore.Mvc;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages
{
    public class WortraumModel : PageModel
    {
        private readonly OliItDbContext _context;
        private readonly WortraumCacheService _cacheService;

        // Root Netz GUID - special entry point for describing author, content, and recipient characteristics
        private const string ROOT_NETZ_GUID = "76035F19-F4AE-4D58-A388-4BBC72C51CEF";

        public WortraumModel(OliItDbContext context, WortraumCacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public List<NetzViewModel> Netze { get; set; } = new();
        public NetzViewModel? RootNetz { get; set; }

        public async Task OnGetAsync()
        {
            var rootNetzGuid = Guid.Parse(ROOT_NETZ_GUID);

            // Load all Netze and Baums from CACHE (shared across all users!)
            // First user triggers DB load, subsequent users get instant cached data
            var (allNetze, baums) = await _cacheService.GetAllWortraumDataAsync();

            // Build the view model from in-memory cached data (no DB access per user)
            // Limit initial depth to 2 levels to prevent building massive hidden structures
            var visitedNetz = new HashSet<Guid>();
            var visitedBaum = new HashSet<Guid>();
            RootNetz = BuildNetzViewModel(rootNetzGuid, allNetze, baums, visitedNetz, visitedBaum, maxDepth: 2, currentDepth: 0);

            if (RootNetz != null)
            {
                Netze = new List<NetzViewModel> { RootNetz };
            }
        }

        // AJAX handler for on-demand loading of node children
        public async Task<IActionResult> OnGetLoadNodeAsync(string type, Guid guid, int indentLevel)
        {
            try
            {
                if (type == "netz")
                {
                    // Get from cache instead of direct DB query
                    var allNetze = await _cacheService.GetAllNetzeAsync();

                    if (!allNetze.TryGetValue(guid, out var netz))
                        return NotFound();

                    var viewModel = new NetzViewModel
                    {
                        Guid = netz.NetzGuid,
                        Name = netz.Netz1,
                        Beschreibung = netz.Beschreibung,
                        Knoten = netz.Knotens
                            .OrderBy(k => k.Knoten1)
                            .Select(k => new KnotenViewModel
                            {
                                Guid = k.KnotenGuid,
                                Name = k.Knoten1,
                                Beschreibung = k.Beschreibung,
                                WeiterNetzGuid = k.WeiterNetzGuid,
                                WeiterBaumGuid = k.WeiterBaumGuid
                            })
                            .ToList()
                    };

                    return new JsonResult(new { 
                        type = "netz", 
                        data = viewModel,
                        indentLevel = indentLevel
                    }, new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase 
                    });
                }
                else if (type == "baum")
                {
                    // Get from cache instead of direct DB query
                    var allBaums = await _cacheService.GetAllBaumsAsync();

                    if (!allBaums.TryGetValue(guid, out var baum))
                        return NotFound();

                    var viewModel = new BaumViewModel
                    {
                        Guid = baum.BaumGuid,
                        Name = baum.Baum1,
                        Beschreibung = baum.Beschreibung,
                        Zweige = baum.Zweigs
                            .OrderBy(z => z.Zweig1)
                            .Select(z => new ZweigViewModel
                            {
                                Guid = z.ZweigGuid,
                                Name = z.Zweig1,
                                WeiterNetzGuid = z.WeiterNetzGuid,
                                WeiterBaumGuid = z.WeiterBaumGuid
                            })
                            .ToList()
                    };

                    return new JsonResult(new { 
                        type = "baum", 
                        data = viewModel,
                        indentLevel = indentLevel
                    }, new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase 
                    });
                }

                return BadRequest("Invalid node type");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private NetzViewModel? BuildNetzViewModel(
            Guid netzGuid, 
            Dictionary<Guid, Netz> allNetze, 
            Dictionary<Guid, Baum> baums,
            HashSet<Guid> visitedNetz,
            HashSet<Guid> visitedBaum,
            int maxDepth,
            int currentDepth)
        {
            // Stop if we've reached max depth (for initial page load performance)
            if (currentDepth >= maxDepth)
                return null;

            // Prevent infinite recursion for Netz cycles
            if (visitedNetz.Contains(netzGuid))
                return null;

            if (!allNetze.ContainsKey(netzGuid))
                return null;

            visitedNetz.Add(netzGuid);
            var netz = allNetze[netzGuid];

            var netzViewModel = new NetzViewModel
            {
                Guid = netz.NetzGuid,
                Name = netz.Netz1,
                Beschreibung = netz.Beschreibung,
                Knoten = netz.Knotens
                    .OrderBy(k => k.Knoten1)  // Alphabetical ordering
                    .Select(k => BuildKnotenViewModel(k, allNetze, baums, visitedNetz, visitedBaum, maxDepth, currentDepth + 1))
                    .ToList()
            };

            visitedNetz.Remove(netzGuid); // Allow this Netz to appear in other branches
            return netzViewModel;
        }

        private KnotenViewModel BuildKnotenViewModel(
            Knoten knoten,
            Dictionary<Guid, Netz> allNetze,
            Dictionary<Guid, Baum> baums,
            HashSet<Guid> visitedNetz,
            HashSet<Guid> visitedBaum,
            int maxDepth,
            int currentDepth)
        {
            var knotenViewModel = new KnotenViewModel
            {
                Guid = knoten.KnotenGuid,
                Name = knoten.Knoten1,
                Beschreibung = knoten.Beschreibung,
                WeiterNetzGuid = knoten.WeiterNetzGuid,
                WeiterBaumGuid = knoten.WeiterBaumGuid
            };

            // Only build nested structures if we haven't reached max depth
            if (currentDepth < maxDepth)
            {
                // Load referenced Baum if present (mutually exclusive with WeiterNetz)
                if (knoten.WeiterBaumGuid.HasValue && baums.ContainsKey(knoten.WeiterBaumGuid.Value))
                {
                    knotenViewModel.Baum = BuildBaumViewModel(
                        knoten.WeiterBaumGuid.Value,
                        allNetze,
                        baums,
                        visitedNetz,
                        visitedBaum,
                        maxDepth,
                        currentDepth);
                }
                // Load referenced Netz if present
                else if (knoten.WeiterNetzGuid.HasValue)
                {
                    knotenViewModel.WeiterNetz = BuildNetzViewModel(
                        knoten.WeiterNetzGuid.Value, 
                        allNetze, 
                        baums, 
                        visitedNetz,
                        visitedBaum,
                        maxDepth,
                        currentDepth);
                }
            }

            return knotenViewModel;
        }

        private BaumViewModel? BuildBaumViewModel(
            Guid baumGuid,
            Dictionary<Guid, Netz> allNetze,
            Dictionary<Guid, Baum> baums,
            HashSet<Guid> visitedNetz,
            HashSet<Guid> visitedBaum,
            int maxDepth,
            int currentDepth)
        {
            // Stop if we've reached max depth (for initial page load performance)
            if (currentDepth >= maxDepth)
                return null;

            // Prevent infinite recursion for Baum cycles
            if (visitedBaum.Contains(baumGuid))
                return null;

            if (!baums.ContainsKey(baumGuid))
                return null;

            visitedBaum.Add(baumGuid);
            var baum = baums[baumGuid];

            var baumViewModel = new BaumViewModel
            {
                Guid = baum.BaumGuid,
                Name = baum.Baum1,
                Beschreibung = baum.Beschreibung,
                Zweige = baum.Zweigs
                    .OrderBy(z => z.Zweig1)  // Alphabetical ordering
                    .Select(z => BuildZweigViewModel(z, allNetze, baums, visitedNetz, visitedBaum, maxDepth, currentDepth + 1))
                    .ToList()
            };

            visitedBaum.Remove(baumGuid); // Allow this Baum to appear in other branches
            return baumViewModel;
        }

        private ZweigViewModel BuildZweigViewModel(
            Zweig zweig,
            Dictionary<Guid, Netz> allNetze,
            Dictionary<Guid, Baum> baums,
            HashSet<Guid> visitedNetz,
            HashSet<Guid> visitedBaum,
            int maxDepth,
            int currentDepth)
        {
            var zweigViewModel = new ZweigViewModel
            {
                Guid = zweig.ZweigGuid,
                Name = zweig.Zweig1,
                WeiterNetzGuid = zweig.WeiterNetzGuid,
                WeiterBaumGuid = zweig.WeiterBaumGuid
            };

            // Only build nested Baum if we haven't reached max depth
            if (currentDepth < maxDepth)
            {
                // Load referenced Baum if present
                if (zweig.WeiterBaumGuid.HasValue && baums.ContainsKey(zweig.WeiterBaumGuid.Value))
                {
                    zweigViewModel.WeiterBaum = BuildBaumViewModel(
                        zweig.WeiterBaumGuid.Value,
                        allNetze,
                        baums,
                        visitedNetz,
                        visitedBaum,
                        maxDepth,
                        currentDepth);
                }
            }

            return zweigViewModel;
        }
    }

    public class NetzViewModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public List<KnotenViewModel> Knoten { get; set; } = new();
    }

    public class KnotenViewModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public Guid? WeiterNetzGuid { get; set; }
        public Guid? WeiterBaumGuid { get; set; }
        public BaumViewModel? Baum { get; set; }
        public NetzViewModel? WeiterNetz { get; set; }
    }

    public class BaumViewModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public List<ZweigViewModel> Zweige { get; set; } = new();
    }

    public class ZweigViewModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? WeiterNetzGuid { get; set; }
        public Guid? WeiterBaumGuid { get; set; }
        public BaumViewModel? WeiterBaum { get; set; }
    }
}

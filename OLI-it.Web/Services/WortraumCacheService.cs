using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Services;

/// <summary>
/// Provides cached access to Wortraum (NKBZ) data for optimal performance across multiple users.
/// Loads Netz and Baum dictionaries once and caches them in memory with sliding expiration.
/// </summary>
public class WortraumCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WortraumCacheService> _logger;

    private const string NETZ_CACHE_KEY = "Wortraum_AllNetze";
    private const string BAUM_CACHE_KEY = "Wortraum_AllBaums";

    // Cache for 1 hour with sliding expiration (resets timer on each access)
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public WortraumCacheService(
        IMemoryCache cache,
        IServiceScopeFactory scopeFactory,
        ILogger<WortraumCacheService> logger)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Gets all Netz entities with their Knoten from cache or database.
    /// Thread-safe with lock to prevent duplicate loads.
    /// </summary>
    public async Task<Dictionary<Guid, Netz>> GetAllNetzeAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(NETZ_CACHE_KEY, out Dictionary<Guid, Netz>? cachedNetze))
        {
            _logger.LogDebug("Netz data retrieved from cache");
            return cachedNetze!;
        }

        // Not in cache - load from database with lock to prevent duplicate loads
        return await _cache.GetOrCreateAsync(NETZ_CACHE_KEY, async entry =>
        {
            _logger.LogInformation("Loading Netz data from database...");

            entry.SlidingExpiration = CacheDuration;
            entry.Priority = CacheItemPriority.High;

            // Create a scope to resolve DbContext (can't inject scoped service into singleton)
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OliItDbContext>();

            var netze = await context.Netzs
                .Include(n => n.Knotens)
                .ToDictionaryAsync(n => n.NetzGuid);

            _logger.LogInformation("Loaded {Count} Netz entities into cache", netze.Count);
            return netze;
        }) ?? new Dictionary<Guid, Netz>();
    }

    /// <summary>
    /// Gets all Baum entities with their Zweige from cache or database.
    /// Thread-safe with lock to prevent duplicate loads.
    /// </summary>
    public async Task<Dictionary<Guid, Baum>> GetAllBaumsAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(BAUM_CACHE_KEY, out Dictionary<Guid, Baum>? cachedBaums))
        {
            _logger.LogDebug("Baum data retrieved from cache");
            return cachedBaums!;
        }

        // Not in cache - load from database with lock to prevent duplicate loads
        return await _cache.GetOrCreateAsync(BAUM_CACHE_KEY, async entry =>
        {
            _logger.LogInformation("Loading Baum data from database...");

            entry.SlidingExpiration = CacheDuration;
            entry.Priority = CacheItemPriority.High;

            // Create a scope to resolve DbContext (can't inject scoped service into singleton)
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OliItDbContext>();

            var baums = await context.Baums
                .Include(b => b.Zweigs)
                .ToDictionaryAsync(b => b.BaumGuid);

            _logger.LogInformation("Loaded {Count} Baum entities into cache", baums.Count);
            return baums;
        }) ?? new Dictionary<Guid, Baum>();
    }

    /// <summary>
    /// Loads both Netz and Baum data in parallel for optimal performance.
    /// Returns a tuple of dictionaries.
    /// </summary>
    public async Task<(Dictionary<Guid, Netz> Netze, Dictionary<Guid, Baum> Baums)> GetAllWortraumDataAsync()
    {
        // Load both in parallel for best performance
        var netzeTask = GetAllNetzeAsync();
        var baumsTask = GetAllBaumsAsync();

        await Task.WhenAll(netzeTask, baumsTask);

        return (await netzeTask, await baumsTask);
    }

    /// <summary>
    /// Clears the Wortraum cache, forcing a reload from the database on next access.
    /// Call this when vocabulary data has been updated.
    /// </summary>
    public void InvalidateCache()
    {
        _cache.Remove(NETZ_CACHE_KEY);
        _cache.Remove(BAUM_CACHE_KEY);
        _logger.LogInformation("Wortraum cache invalidated");
    }

    /// <summary>
    /// Preloads the cache with Wortraum data (useful at application startup).
    /// </summary>
    public async Task WarmupCacheAsync()
    {
        _logger.LogInformation("Warming up Wortraum cache...");
        await GetAllWortraumDataAsync();
        _logger.LogInformation("Wortraum cache warmed up successfully");
    }
}

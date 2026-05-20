using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Endpoints;
using OLI_it.Web.Middleware;
using OLI_it.Web.Services;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var supportedCultureCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "en", "de", "es" };
var supportedCultures = supportedCultureCodes
    .Select(code => new CultureInfo(code))
    .ToList();

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Add anti-forgery protection
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Login endpoint: 5 attempts per minute per IP
    options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0; // No queueing
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { success = false, message = "Too many login attempts. Please try again later." },
            cancellationToken: cancellationToken);
    };
});

// Add DbContext with connection string from configuration
builder.Services.AddDbContext<OliItDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OliItDb")));

// Add memory cache
builder.Services.AddMemoryCache();

// Add Wortraum Cache Service (singleton for shared caching across all users)
builder.Services.AddSingleton<WortraumCacheService>();

// Add Azure Blob Storage Service
builder.Services.AddSingleton<AzureBlobStorageService>();

// Add Search Service
builder.Services.AddScoped<SearchService>();

// Add Journal Service
builder.Services.AddScoped<JournalService>();

// Add Chart Service
builder.Services.AddScoped<ChartService>();

var app = builder.Build();
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

localizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
{
    if (context.Items.TryGetValue("UiLanguage", out var value)
        && value is string languageCode
        && supportedCultureCodes.Contains(languageCode))
    {
        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(languageCode, languageCode));
    }

    return Task.FromResult<ProviderCultureResult?>(null);
}));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseUiLanguagePathMiddleware(supportedCultureCodes);

app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Map authentication endpoints
app.MapAuthenticationEndpoints();

app.Run();

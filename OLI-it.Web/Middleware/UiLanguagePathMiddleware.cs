namespace OLI_it.Web.Middleware;

public sealed class UiLanguagePathMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _supportedCultureCodes;

    public UiLanguagePathMiddleware(RequestDelegate next, IEnumerable<string> supportedCultureCodes)
    {
        _next = next;
        _supportedCultureCodes = new HashSet<string>(supportedCultureCodes, StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var pathValue = context.Request.Path.Value;
        if (string.IsNullOrWhiteSpace(pathValue)
            || pathValue == "/"
            || Path.HasExtension(pathValue))
        {
            await _next(context);
            return;
        }

        var segments = pathValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
        {
            await _next(context);
            return;
        }

        var candidate = segments[0].ToLowerInvariant();
        if (candidate.Length == 2)
        {
            if (_supportedCultureCodes.Contains(candidate))
            {
                context.Items["UiLanguage"] = candidate;
                var remainingSegments = segments.Skip(1).ToArray();
                context.Request.Path = remainingSegments.Length == 0
                    ? "/"
                    : $"/{string.Join('/', remainingSegments)}";
            }
            else
            {
                var fallbackSegments = segments.Skip(1).ToArray();
                var fallbackPath = fallbackSegments.Length == 0
                    ? "/en"
                    : $"/en/{string.Join('/', fallbackSegments)}";
                var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;

                context.Response.Redirect($"{fallbackPath}{queryString}", permanent: false);
                return;
            }
        }

        await _next(context);
    }
}

public static class UiLanguagePathMiddlewareExtensions
{
    public static IApplicationBuilder UseUiLanguagePathMiddleware(
        this IApplicationBuilder app,
        IEnumerable<string> supportedCultureCodes)
    {
        return app.UseMiddleware<UiLanguagePathMiddleware>(supportedCultureCodes);
    }
}

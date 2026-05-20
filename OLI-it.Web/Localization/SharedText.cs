using System.Globalization;
using System.Resources;

namespace OLI_it.Web.Localization;

public static class SharedText
{
    private static readonly ResourceManager ResourceManager = new(
        "OLI_it.Web.Resources.SharedResource",
        typeof(SharedResource).Assembly);

    public static string Get(string key)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public static string Get(string key, params object[] args)
    {
        return string.Format(CultureInfo.CurrentUICulture, Get(key), args);
    }
}

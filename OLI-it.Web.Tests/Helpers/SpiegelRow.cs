namespace OLI_it.Web.Tests.Helpers;

/// <summary>
/// A single row from <c>oli.Spiegel</c> representing one matchmaking outcome.
/// </summary>
public sealed record SpiegelRow(Guid CodeGuid, Guid AnglerGuid, string? Status);

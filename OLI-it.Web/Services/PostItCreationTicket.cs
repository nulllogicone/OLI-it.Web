using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace OLI_it.Web.Services;

public sealed class PostItCreationTicket(IDataProtectionProvider provider)
{
    private readonly ITimeLimitedDataProtector _protector =
        provider.CreateProtector("OLI-it.PostItCreation.v1").ToTimeLimitedDataProtector();

    public string Issue(Guid authorId, Guid? postItId = null) =>
        GetProtector(postItId).Protect($"{authorId:D}|{Guid.NewGuid():D}", TimeSpan.FromHours(24));

    public bool TryRead(string? ticket, Guid authorId, out Guid messageId, Guid? postItId = null)
    {
        messageId = Guid.Empty;
        if (string.IsNullOrEmpty(ticket)) return false;
        try
        {
            var fields = GetProtector(postItId).Unprotect(ticket, out _).Split('|');
            return fields.Length == 2 && Guid.TryParse(fields[0], out var owner) && owner == authorId &&
                Guid.TryParse(fields[1], out messageId) && messageId != Guid.Empty;
        }
        catch (CryptographicException) { return false; }
    }

    private ITimeLimitedDataProtector GetProtector(Guid? postItId) =>
        postItId is Guid id ? _protector.CreateProtector($"TopLab:{id:D}") : _protector;
}

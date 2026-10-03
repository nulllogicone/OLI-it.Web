using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Services;

public sealed class PostItCreationService(OliItDbContext context)
{
    public async Task<Guid> CreateAsync(Guid authorId, Guid messageId, CreatePostItInput input,
        CancellationToken cancellationToken = default)
    {
        Validator.ValidateObject(input, new ValidationContext(input), validateAllProperties: true);
        if (authorId == Guid.Empty || messageId == Guid.Empty)
            throw new ValidationException("Invalid author or submission.");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        // Serialize retries of the same signed submission across processes/instances.
        var resource = $"PostItCreate:{messageId:D}";
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @result int;
            EXEC @result = sys.sp_getapplock @Resource={resource}, @LockMode='Exclusive',
                @LockOwner='Transaction', @LockTimeout=10000;
            IF @result < 0 THROW 50001, 'Could not lock message submission.', 1;
            """, cancellationToken);

        var existing = await context.Wurzelns.AsNoTracking().AnyAsync(
            w => w.PostItGuid == messageId && w.StammGuid == authorId && w.StammZust == 1,
            cancellationToken);
        if (existing)
        {
            await transaction.CommitAsync(cancellationToken);
            return messageId;
        }
        var author = await context.Stamms.AsNoTracking().SingleOrDefaultAsync(
            s => s.StammGuid == authorId, cancellationToken)
            ?? throw new ValidationException("Your account is unavailable.");
        var now = DateTime.Now; // Existing SQL/legacy timestamps use local server time.
        var deadline = now.AddDays(10);
        var code = new Code
        {
            CodeGuid = Guid.NewGuid(), PostItGuid = messageId, StammGuid = authorId,
            Kommentar = "Markierung von " + author.Stamm1, Gescannt = false
        };
        var shortcuts = await context.ShortCuts.AsNoTracking().Include(s => s.Strings)
            .Where(s => s.StammGuid == authorId && s.Auto).ToListAsync(cancellationToken);
        foreach (var marking in shortcuts.SelectMany(s => s.Strings))
        {
            var isBranch = marking.BaumGuid.HasValue && marking.ZweigGuid.HasValue;
            code.Ringes.Add(new Ringe
            {
                RingGuid = Guid.NewGuid(), CodeGuid = code.CodeGuid,
                NetzGuid = marking.NetzGuid, KnotenGuid = marking.KnotenGuid,
                BaumGuid = isBranch ? marking.BaumGuid : null,
                ZweigGuid = isBranch ? marking.ZweigGuid : null,
                Olis = marking.Verb, Get = marking.Attrib
            });
        }
        context.PostIts.Add(new PostIt
        {
            PostItGuid = messageId, Titel = WebUtility.HtmlEncode(input.Title.Trim()),
            PostIt1 = WebUtility.HtmlEncode(input.Body), Typ = "txt", Datum = now,
            KooK = 0.01m, Hits = 0, Url = string.IsNullOrWhiteSpace(input.Url) ? null : input.Url.Trim(),
            Codes = [code],
            Wurzelns = [new Wurzeln
            {
                StammGuid = authorId, StammZust = 1, Bezahlt = 0, Frist = deadline,
                Gemailt = false, Closed = false
            }]
        });
        context.PostItKontos.Add(new PostItKonto
        {
            PostItGuid = messageId, Datum = now, Betrag = 0.01m, Kommentar = "Startwert"
        });
        await context.SaveChangesAsync(cancellationToken);
        // Existing procedure owns balances, both transfer ledger rows and Wurzeln updates.
        // Its nested transaction participates in our outer transaction.
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            EXEC oli.zahlen @sguid={authorId}, @betrag={1.0m}, @pguid={messageId},
                @frist={deadline}, @kommentar={"Neue Nachricht"}
            """, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return messageId;
    }
}

using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OLI_it.Web.Data;
using OLI_it.Web.Services;
using OLI_it.Web.Tests.Helpers;
using System.Data.Common;

namespace OLI_it.Web.Tests;

public sealed class PostItCreationDatabase : IAsyncLifetime
{
    private readonly string _name = "OliItSendMessage_" + Guid.NewGuid().ToString("N");
    public string ConnectionString => $"Server=(localdb)\\MSSQLLocalDB;Database={_name};Integrated Security=true;TrustServerCertificate=true;";
    public Task InitializeAsync() => DatabaseHelper.RestoreBackupAsync(
        Path.Combine(AppContext.BaseDirectory, "data", "null.bak"), _name);
    public Task DisposeAsync() => DatabaseHelper.DropDatabaseAsync(_name);
    public OliItDbContext Open(params IInterceptor[] interceptors) => new(
        new DbContextOptionsBuilder<OliItDbContext>().UseSqlServer(ConnectionString)
            .AddInterceptors(interceptors).Options);
}

public sealed class PostItCreationTests(PostItCreationDatabase database) : IClassFixture<PostItCreationDatabase>
{
    private static readonly Guid Author = Guid.Parse("44d772c1-7481-4c2f-b6b7-00ed5e06bc7b");
    private static CreatePostItInput Input() => new() { Title = "Creation <test> & title", Body = "A <script> & emoji 😀\nSecond line", Url = "https://example.com/" };

    [Fact]
    public async Task CreatesMessageAuthorCodeAndAccountingTogether()
    {
        await using var db = database.Open();
        var before = await db.Stamms.AsNoTracking().Where(s => s.StammGuid == Author).Select(s => s.KooK).SingleAsync();
        var id = Guid.NewGuid();
        await new PostItCreationService(db).CreateAsync(Author, id, Input());
        await using var verify = database.Open();
        var post = await verify.PostIts.Include(p => p.Codes).Include(p => p.Wurzelns).SingleAsync(p => p.PostItGuid == id);
        Assert.Equal(1.01m, post.KooK);
        Assert.Equal("txt", post.Typ.Trim());
        Assert.Equal(Input().Title, PostItText.Title(post.Titel));
        Assert.Equal(Input().Body, PostItText.Body(post.PostIt1, post.Typ));
        Assert.DoesNotContain("<script>", post.PostIt1);
        Assert.Equal(1, Assert.Single(post.Wurzelns).StammZust);
        Assert.Equal(1m, Assert.Single(post.Wurzelns).Bezahlt);
        Assert.False(Assert.Single(post.Codes).Gescannt);
        Assert.Equal(Author, Assert.Single(post.Codes).StammGuid);
        Assert.InRange((Assert.Single(post.Wurzelns).Frist!.Value - post.Datum).TotalDays, 9.999, 10.001);
        Assert.Equal(before - 1m, await verify.Stamms.Where(s => s.StammGuid == Author).Select(s => s.KooK).SingleAsync());
        var entries = await verify.PostItKontos.Where(k => k.PostItGuid == id).Select(k => k.Betrag).ToListAsync();
        Assert.Equal(2, entries.Count);
        Assert.Contains(0.01m, entries);
        Assert.Contains(1m, entries);
        Assert.Equal(-1m, await verify.StammKontos.Where(k => k.PostItGuid == id).Select(k => k.Betrag).SingleAsync());
    }

    [Fact]
    public async Task ConcurrentRetryCreatesAndChargesOnce()
    {
        var id = Guid.NewGuid();
        await using var first = database.Open();
        await using var second = database.Open();
        await Task.WhenAll(new PostItCreationService(first).CreateAsync(Author, id, Input()),
            new PostItCreationService(second).CreateAsync(Author, id, Input()));
        await using var verify = database.Open();
        Assert.Equal(1, await verify.PostIts.CountAsync(p => p.PostItGuid == id));
        Assert.Equal(1, await verify.Codes.CountAsync(c => c.PostItGuid == id));
        Assert.Equal(1, await verify.StammKontos.CountAsync(k => k.PostItGuid == id));
    }

    [Fact]
    public async Task FailureAtAccountingRollsBackAlreadyInsertedRows()
    {
        var id = Guid.NewGuid();
        await using var db = database.Open(new FailAccounting());
        await Assert.ThrowsAsync<InvalidOperationException>(() => new PostItCreationService(db).CreateAsync(Author, id, Input()));
        await using var verify = database.Open();
        Assert.False(await verify.PostIts.AnyAsync(p => p.PostItGuid == id));
        Assert.False(await verify.Wurzelns.AnyAsync(w => w.PostItGuid == id));
        Assert.False(await verify.Codes.AnyAsync(c => c.PostItGuid == id));
        Assert.False(await verify.PostItKontos.AnyAsync(k => k.PostItGuid == id));
        Assert.False(await verify.StammKontos.AnyAsync(k => k.PostItGuid == id));
    }

    [Fact]
    public async Task InvalidInputAndUnknownAuthorCreateNothing()
    {
        await using var db = database.Open();
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<ValidationException>(() => new PostItCreationService(db).CreateAsync(Author, id,
            new() { Title = "Title", Body = " ", Url = "javascript:alert(1)" }));
        await Assert.ThrowsAsync<ValidationException>(() => new PostItCreationService(db).CreateAsync(Guid.NewGuid(), id, Input()));
        Assert.False(await db.PostIts.AnyAsync(p => p.PostItGuid == id));
    }

    [Fact]
    public async Task CopiesAutomaticShortcutsIntoInitialCode()
    {
        await using var db = database.Open();
        var shortcutId = Guid.NewGuid();
        var source = await db.Strings.AsNoTracking().FirstAsync();
        db.ShortCuts.Add(new OLI_it.Web.Models.ShortCut
        {
            ShortCutsGuid = shortcutId, StammGuid = Author, ShortCut1 = "Creation test", Auto = true,
            Strings = [new OLI_it.Web.Models.String
            {
                StringsGuid = Guid.NewGuid(), NetzGuid = source.NetzGuid, KnotenGuid = source.KnotenGuid,
                BaumGuid = source.BaumGuid, ZweigGuid = source.ZweigGuid, Verb = 2, Attrib = 2
            }]
        });
        await db.SaveChangesAsync();
        try
        {
            var id = Guid.NewGuid();
            await new PostItCreationService(db).CreateAsync(Author, id, Input());
            await using var verify = database.Open();
            Assert.True(await verify.Ringes.AnyAsync(r => r.Code.PostItGuid == id &&
                r.NetzGuid == source.NetzGuid && r.KnotenGuid == source.KnotenGuid && r.Olis == 2 && r.Get == 2));
        }
        finally
        {
            await db.Strings.Where(s => s.ShortCutsGuid == shortcutId).ExecuteDeleteAsync();
            await db.ShortCuts.Where(s => s.ShortCutsGuid == shortcutId).ExecuteDeleteAsync();
        }
    }

    [Fact]
    public void CreationTicketIsBoundToAuthorAndRejectsTampering()
    {
        var tickets = new PostItCreationTicket(new EphemeralDataProtectionProvider());
        var ticket = tickets.Issue(Author);
        Assert.True(tickets.TryRead(ticket, Author, out var id));
        Assert.NotEqual(Guid.Empty, id);
        Assert.False(tickets.TryRead(ticket, Guid.NewGuid(), out _));
        Assert.False(tickets.TryRead(ticket + "bad", Author, out _));
        Assert.False(tickets.TryRead(null, Author, out _));
    }

    [Theory]
    [InlineData("&", 601)]
    [InlineData("😀", 600)]
    public void EncodedStorageLimitIsValidated(string character, int count)
    {
        var input = new CreatePostItInput { Title = "Title", Body = string.Concat(Enumerable.Repeat(character, count)) };
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(input, new(input), true));
    }

    [Fact]
    public async Task ForgedSubmissionCannotCreateMessage()
    {
        await using var db = database.Open();
        var tickets = new PostItCreationTicket(new EphemeralDataProtectionProvider());
        var page = new OLI_it.Web.Pages.PostIt.CreateModel(db, new(db), tickets,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<OLI_it.Web.Pages.PostIt.CreateModel>.Instance);
        page.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
        {
            ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()),
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                    [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Author.ToString())], "test"))
            }
        };
        page.Input = Input();
        page.Submission = tickets.Issue(Guid.NewGuid());
        var count = await db.PostIts.CountAsync();
        Assert.IsType<Microsoft.AspNetCore.Mvc.RazorPages.PageResult>(await page.OnPostAsync(default));
        Assert.False(page.ModelState.IsValid);
        Assert.Equal(count, await db.PostIts.CountAsync());
    }

    [Fact]
    public async Task InvalidIdentityCannotCreateMessage()
    {
        await using var db = database.Open();
        var page = new OLI_it.Web.Pages.PostIt.CreateModel(db, new(db),
            new(new EphemeralDataProtectionProvider()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<OLI_it.Web.Pages.PostIt.CreateModel>.Instance);
        page.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
        {
            ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()),
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
        };
        Assert.IsType<Microsoft.AspNetCore.Mvc.ForbidResult>(await page.OnPostAsync(default));
    }

    [Fact]
    public async Task ContentUpdatePreservesAccountingAndSemanticRecords()
    {
        await using var db = database.Open();
        var id = Guid.NewGuid();
        await new PostItCreationService(db).CreateAsync(Author, id, Input());
        var code = await db.Codes.SingleAsync(c => c.PostItGuid == id);
        code.Gescannt = true;
        await db.SaveChangesAsync();
        var ringIds = await db.Ringes.Where(r => r.CodeGuid == code.CodeGuid).Select(r => r.RingGuid).OrderBy(g => g).ToListAsync();
        var ledgerCount = await db.PostItKontos.CountAsync(k => k.PostItGuid == id);
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:OliItStorageConnectionString"] = "UseDevelopmentStorage=true" }).Build();
        var page = new OLI_it.Web.Pages.PostIt.EditModel(db,
            new AzureBlobStorageService(configuration, Microsoft.Extensions.Logging.Abstractions.NullLogger<AzureBlobStorageService>.Instance),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<OLI_it.Web.Pages.PostIt.EditModel>.Instance)
        {
            PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                    new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                    new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()),
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                        [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Author.ToString())], "test"))
                }
            }
        };
        Assert.IsType<Microsoft.AspNetCore.Mvc.RazorPages.PageResult>(await page.OnGetAsync(id));
        Assert.Equal(Input().Title, page.Titel);
        Assert.Equal(Input().Body, page.PostIt1);
        page.Titel = "Updated <title> & text";
        page.PostIt1 = "Updated <script> & emoji 😀";
        page.Typ = "txt";
        Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToPageResult>(await page.OnPostAsync(id));
        await using var verify = database.Open();
        var post = await verify.PostIts.SingleAsync(p => p.PostItGuid == id);
        Assert.Equal(page.Titel, PostItText.Title(post.Titel));
        Assert.Equal(page.PostIt1, PostItText.Body(post.PostIt1, post.Typ));
        Assert.DoesNotContain("<script>", post.PostIt1);
        Assert.Equal(ledgerCount, await verify.PostItKontos.CountAsync(k => k.PostItGuid == id));
        Assert.True((await verify.Codes.SingleAsync(c => c.CodeGuid == code.CodeGuid)).Gescannt);
        Assert.Equal(ringIds, await verify.Ringes.Where(r => r.CodeGuid == code.CodeGuid).Select(r => r.RingGuid).OrderBy(g => g).ToListAsync());
    }
    private sealed class FailAccounting : DbCommandInterceptor
    {
        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command,
            CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (command.CommandText.Contains("EXEC oli.zahlen")) throw new InvalidOperationException("Injected accounting failure.");
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}

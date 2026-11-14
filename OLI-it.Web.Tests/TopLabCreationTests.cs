using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using OLI_it.Web.Pages.TopLab;
using OLI_it.Web.Services;

namespace OLI_it.Web.Tests;

public sealed class TopLabCreationTests(PostItCreationDatabase database) : IClassFixture<PostItCreationDatabase>
{
    [Fact]
    public async Task SelfAnswerSavesOnceAndSubmissionCannotMoveToAnotherPostIt()
    {
        await using var db = database.Open();
        var authorId = Guid.Parse("44d772c1-7481-4c2f-b6b7-00ed5e06bc7b");
        var postItId = Guid.NewGuid();
        await new PostItCreationService(db).CreateAsync(authorId, postItId, new()
        {
            Title = "Question",
            Body = "A question to answer"
        });

        var tickets = new PostItCreationTicket(new EphemeralDataProtectionProvider());
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, authorId.ToString())], "test"))
        };
        var page = new CreateModel(db, tickets, NullLogger<CreateModel>.Instance)
        {
            PageContext = new PageContext
            {
                HttpContext = httpContext,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            },
            TempData = new TempDataDictionary(httpContext, new NoopTempDataProvider())
        };

        Assert.IsType<PageResult>(await page.OnGetAsync(postItId, default));
        Assert.False(tickets.TryRead(page.Submission, authorId, out _, Guid.NewGuid()));
        Assert.False(tickets.TryRead(page.Submission, authorId, out _));
        const string body = "An answer with <markup> & punctuation";
        page.Input.Body = body;
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var redirect = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync(postItId, default));
            Assert.Equal("/PostIt/TopLab", redirect.PageName);
        }

        var answer = await db.TopLabs.AsNoTracking().SingleAsync(t => t.PostItGuid == postItId);
        Assert.Equal(authorId, answer.StammGuid);
        Assert.Equal(0m, answer.Lohn);
        Assert.Equal(body, PostItText.Body(answer.TopLab1, answer.Typ));
        Assert.Contains("&lt;markup&gt;", answer.TopLab1);

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["ConnectionStrings:OliItStorageConnectionString"] = "UseDevelopmentStorage=true" }).Build();
        var gallery = new AzureBlobStorageService(configuration, NullLogger<AzureBlobStorageService>.Instance);
        var editPage = new EditModel(db, gallery, NullLogger<EditModel>.Instance)
        {
            PageContext = page.PageContext,
            TempData = page.TempData
        };
        var owner = httpContext.User;
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())], "test"));
        Assert.IsType<ForbidResult>(await editPage.OnGetAsync(answer.TopLabGuid, default));
        Assert.IsType<ForbidResult>(await editPage.OnPostAsync(answer.TopLabGuid, default));

        httpContext.User = owner;
        Assert.IsType<PageResult>(await editPage.OnGetAsync(answer.TopLabGuid, default));
        Assert.Equal(body, editPage.Input.Body);
        editPage.Input.Title = "Updated <title>";
        editPage.Input.Body = "Updated & clarified";
        editPage.SelectedImageUrl = $"/{Guid.NewGuid()}/answer.png";
        Assert.IsType<PageResult>(await editPage.OnPostAsync(answer.TopLabGuid, default));
        Assert.False(editPage.ModelState.IsValid);
        Assert.Equal(answer.Datei, await db.TopLabs.AsNoTracking()
            .Where(t => t.TopLabGuid == answer.TopLabGuid).Select(t => t.Datei).SingleAsync());

        editPage.ModelState.Clear();
        editPage.SelectedImageUrl = answer.Datei;
        Assert.IsType<RedirectToPageResult>(await editPage.OnPostAsync(answer.TopLabGuid, default));

        var updated = await db.TopLabs.AsNoTracking().SingleAsync(t => t.TopLabGuid == answer.TopLabGuid);
        Assert.Equal(editPage.Input.Title, PostItText.Title(updated.Titel));
        Assert.Equal(editPage.Input.Body, PostItText.Body(updated.TopLab1, updated.Typ));
        Assert.Equal(answer.Datum, updated.Datum);
        Assert.Equal(answer.Lohn, updated.Lohn);
        Assert.Equal(answer.Datei, updated.Datei);
    }

    private sealed class NoopTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}

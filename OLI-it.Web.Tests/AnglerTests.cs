using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using OLI_it.Web.Models;
using OLI_it.Web.Pages.Angler;

namespace OLI_it.Web.Tests;

public sealed class AnglerTests(PostItCreationDatabase database) : IClassFixture<PostItCreationDatabase>
{
    [Fact]
    public async Task OwnerCanCreateAndEditAnglerWithRecordedCatches()
    {
        await using var db = database.Open();
        var ownerId = Guid.Parse("44d772c1-7481-4c2f-b6b7-00ed5e06bc7b");
        var otherId = await db.Stamms.Where(s => s.StammGuid != ownerId)
            .Select(s => s.StammGuid).FirstAsync();
        var create = new CreateModel(db, NullLogger<CreateModel>.Instance)
        {
            PageContext = Context(Guid.NewGuid()),
            Input = new() { Title = "Fish & chips", Description = "<fresh>" }
        };
        Assert.IsType<ForbidResult>(await create.OnPostAsync(default));
        create.PageContext = Context(ownerId);
        Assert.IsType<PageResult>(await create.OnGetAsync(default));
        var redirect = Assert.IsType<RedirectToPageResult>(await create.OnPostAsync(default));
        Assert.Equal("/Angler/Index", redirect.PageName);
        var id = Assert.IsType<Guid>(redirect.RouteValues!["id"]);
        var created = await db.Anglers.AsNoTracking().SingleAsync(a => a.AnglerGuid == id);
        Assert.Equal(ownerId, created.StammGuid);
        Assert.Equal("Fish &amp; chips", created.Angler1);
        Assert.False(created.Gescannt);

        var code = await db.Codes.AsNoTracking().Include(c => c.PostIt).FirstAsync();
        db.Spiegels.Add(new Spiegel { AnglerGuid = id, CodeGuid = code.CodeGuid, Zeit = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var edit = new EditModel(db, NullLogger<EditModel>.Instance) { PageContext = Context(otherId) };
        Assert.IsType<ForbidResult>(await edit.OnGetAsync(id, default));
        Assert.IsType<ForbidResult>(await edit.OnPostAsync(id, default));
        edit.PageContext = Context(ownerId);
        Assert.IsType<PageResult>(await edit.OnGetAsync(id, default));
        Assert.Equal("Fish & chips", edit.Input.Title);
        Assert.Equal("<fresh>", edit.Input.Description);
        edit.Input.Title = "Fresh catch";
        edit.Input.Description = null;
        Assert.IsType<RedirectToPageResult>(await edit.OnPostAsync(id, default));

        await using var verify = database.Open();
        var detail = new IndexModel(verify) { PageContext = Context(ownerId) };
        Assert.IsType<PageResult>(await detail.OnGetAsync(id));
        Assert.Equal("Fresh catch", detail.Angler!.Angler1);
        Assert.Null(detail.Angler.Beschreibung);
        Assert.Equal(created.Datum, detail.Angler.Datum);
        Assert.Equal(ownerId, detail.Angler.StammGuid);
        Assert.Equal(code.PostIt.PostItGuid, Assert.Single(detail.CatchedPostIts!).Code.PostIt.PostItGuid);
    }

    private static PageContext Context(Guid stammId)
    {
        return new PageContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, stammId.ToString())], "test"))
            },
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        };
    }
}

public sealed class AnglerInputTests
{
    [Fact]
    public void ValidationChecksEncodedColumnLengths()
    {
        var input = new AnglerInput { Title = new string('&', 11), Description = new string('&', 52) };
        var errors = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(input, new ValidationContext(input), errors, true));
        Assert.Equal(new[] { "Title", "Description" }, errors.SelectMany(error => error.MemberNames));
    }
}

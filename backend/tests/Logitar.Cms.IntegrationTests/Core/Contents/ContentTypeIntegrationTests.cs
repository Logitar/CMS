using Logitar.Cms.Core.Contents.Commands;
using Logitar.Cms.Core.Contents.Models;

namespace Logitar.Cms.Core.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class ContentTypeIntegrationTests : IntegrationTests
{
  public ContentTypeIntegrationTests() : base()
  {
  }

  [Theory(DisplayName = "It should create a new invariant content type.")]
  [InlineData(null)]
  [InlineData("326808d7-b9c5-46f9-aea4-0a22ee4a82d5")]
  public async Task Given_Invariant_When_Create_Then_ContentTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = true,
      UniqueName = "Blog",
      DisplayName = " Blog ",
      Description = "  This is a content type for blogs.  "
    };
    CreateOrReplaceContentTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceContentTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    ContentTypeModel? contentType = result.ContentType;
    Assert.NotNull(contentType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, contentType.Id);
    }
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.IsInvariant, contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), contentType.DisplayName);
    Assert.Equal(payload.Description.Trim(), contentType.Description);
  }

  [Theory(DisplayName = "It should create a new localized content type.")]
  [InlineData(null)]
  [InlineData("4f82dedc-dcda-4cd1-a359-9cbc8a5545c0")]
  public async Task Given_Localized_When_Create_Then_ContentTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = false,
      UniqueName = "BlogArticle",
      DisplayName = " Blog Article ",
      Description = "  This is the content type for blog articles.  "
    };
    CreateOrReplaceContentTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceContentTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    ContentTypeModel? contentType = result.ContentType;
    Assert.NotNull(contentType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, contentType.Id);
    }
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.IsInvariant, contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), contentType.DisplayName);
    Assert.Equal(payload.Description.Trim(), contentType.Description);
  }
}

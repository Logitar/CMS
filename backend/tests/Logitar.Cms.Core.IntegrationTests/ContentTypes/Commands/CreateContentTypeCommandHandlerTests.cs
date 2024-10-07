using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateContentTypeCommandHandlerTests : IntegrationTests
{
  public CreateContentTypeCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new content type.")]
  public async Task It_should_create_a_new_content_type()
  {
    CreateContentTypePayload payload = new("BlogArticle")
    {
      IsInvariant = false,
      DisplayName = " Blog Article ",
      Description = "    "
    };
    CreateContentTypeCommand command = new(payload);
    CmsContentType contentType = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, contentType.Id);
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.NotEqual(default, contentType.CreatedOn);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.True(contentType.CreatedOn < contentType.UpdatedOn);

    Assert.Equal(payload.IsInvariant, contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), contentType.DisplayName);
    Assert.Null(contentType.Description);
  }
}

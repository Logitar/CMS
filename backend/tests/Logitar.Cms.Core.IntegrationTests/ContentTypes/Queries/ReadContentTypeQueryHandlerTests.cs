using Logitar.Cms.Contracts.ContentTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadContentTypeQueryHandlerTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _contentType;

  public ReadContentTypeQueryHandlerTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _contentType = new(new IdentifierUnit("BlogArticle"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_contentType);
  }

  [Fact(DisplayName = "It should return the content type found by ID.")]
  public async Task It_should_return_the_content_type_found_by_Id()
  {
    ReadContentTypeQuery query = new(_contentType.Id.ToGuid(), UniqueName: null);
    CmsContentType? contentType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentType);
    Assert.Equal(_contentType.Id.ToGuid(), contentType.Id);
  }

  [Fact(DisplayName = "It should return the content type found by unique name.")]
  public async Task It_should_return_the_content_type_found_by_unique_name()
  {
    ReadContentTypeQuery query = new(Id: null, $" {_contentType.UniqueName.Value.ToLower()} ");
    CmsContentType? contentType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentType);
    Assert.Equal(_contentType.Id.ToGuid(), contentType.Id);
  }
}

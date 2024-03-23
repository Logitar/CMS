using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Shared;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadContentQueryTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _contentType;
  private readonly ContentAggregate _content;

  public ReadContentQueryTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _contentType = new(new IdentifierUnit("BlogArticle"));
    _content = new(_contentType, new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "prolongez-lete-avec-une-acura-nsx-coupe"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_contentType);
    await _contentRepository.SaveAsync(_content);
  }

  [Fact(DisplayName = "It should return null when the content is not found.")]
  public async Task It_should_return_null_when_the_content_is_not_found()
  {
    ReadContentQuery query = new(Id: Guid.NewGuid());
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the content found by ID.")]
  public async Task It_should_return_the_content_found_by_Id()
  {
    ReadContentQuery query = new(_content.Id.ToGuid());
    ContentItem? contentItem = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentItem);
    Assert.Equal(_content.Id.ToGuid(), contentItem.Id);
  }
}

using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Queries;

public class ReadContentQueryTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _blogArticleType;
  private readonly ContentAggregate _blogArticle;

  public ReadContentQueryTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _blogArticleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);

    ContentLocaleUnit invariant = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "introduction"));
    _blogArticle = new(_blogArticleType, invariant, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_blogArticleType);
    await _contentRepository.SaveAsync(_blogArticle);
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
    ReadContentQuery query = new(_blogArticle.Id.ToGuid());
    ContentItem? contentItem = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentItem);
    Assert.Equal(_blogArticle.Id.ToGuid(), contentItem.Id);
  }
}

using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadContentQueryHandlerTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _contentType;
  private readonly ContentAggregate _content;

  public ReadContentQueryHandlerTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _contentType = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _content = new(_contentType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "ryan-hucks")));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_contentType);
    await _contentRepository.SaveAsync(_content);
  }

  [Fact(DisplayName = "It should return the content found by ID.")]
  public async Task It_should_return_the_content_found_by_Id()
  {
    ReadContentQuery query = new(_content.Id.ToGuid());
    ContentItem? content = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(content);
    Assert.Equal(_content.Id.ToGuid(), content.Id);
  }
}


using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly SaveContentTypeCommandHandler _handler;

  public SaveContentTypeCommandHandlerTests()
  {
    _handler = new(_contentTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save the content type.")]
  public async Task It_should_save_the_content_type()
  {
    ContentTypeAggregate contentType = new(new IdentifierUnit("BlogArticle"));
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.UniqueName, _cancellationToken)).ReturnsAsync(contentType);

    SaveContentTypeCommand command = new(contentType);
    await _handler.Handle(command, _cancellationToken);

    _contentTypeRepository.Verify(x => x.SaveAsync(contentType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CmsUniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_CmsUniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentTypeAggregate contentType = new(new IdentifierUnit("BlogArticle"));
    ContentTypeAggregate other = new(contentType.UniqueName);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.UniqueName, _cancellationToken)).ReturnsAsync(other);

    SaveContentTypeCommand command = new(contentType);
    var exception = await Assert.ThrowsAsync<CmsUniqueNameAlreadyUsedException<ContentTypeAggregate>>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
    Assert.Equal(contentType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}

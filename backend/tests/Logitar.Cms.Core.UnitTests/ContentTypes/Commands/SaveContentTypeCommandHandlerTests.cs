using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveContentTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();

  private readonly SaveContentTypeCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("BlogArticle"));

  public SaveContentTypeCommandHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object, _contentTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save the content type.")]
  public async Task It_should_save_the_content_type()
  {
    _contentTypeQuerier.Setup(x => x.FindIdAsync(_contentType.UniqueName, _cancellationToken)).ReturnsAsync(_contentType.Id);

    SaveContentTypeCommand command = new(_contentType);

    await _handler.Handle(command, _cancellationToken);

    _contentTypeQuerier.Verify(x => x.FindIdAsync(_contentType.UniqueName, _cancellationToken), Times.Once);
    _contentTypeRepository.Verify(x => x.SaveAsync(_contentType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentType conflict = new(_contentType.UniqueName, _contentType.IsInvariant);
    _contentTypeQuerier.Setup(x => x.FindIdAsync(_contentType.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    SaveContentTypeCommand command = new(_contentType);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(typeof(ContentType).GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Equal(_contentType.Id.ToGuid(), exception.AggregateId);
    Assert.Equal(conflict.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(_contentType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}

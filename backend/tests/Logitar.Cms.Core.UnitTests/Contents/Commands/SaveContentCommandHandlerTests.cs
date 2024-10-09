using Logitar.Cms.Core.ContentTypes;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveContentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();

  private readonly SaveContentCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("BlogArticle"));
  private readonly Content _content;

  public SaveContentCommandHandlerTests()
  {
    _handler = new(_contentQuerier.Object, _contentRepository.Object);

    ContentLocale invariant = new(new(Content.UniqueNameSettings, "acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week"));
    _content = new(_contentType, invariant);
  }

  [Fact(DisplayName = "It should save the content.")]
  public async Task It_should_save_the_content()
  {
    SaveContentCommand command = new(_content);

    await _handler.Handle(command, _cancellationToken);

    _contentRepository.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once);
  }
}

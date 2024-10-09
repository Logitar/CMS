using Logitar.Cms.Contracts.Contents;
using Moq;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly ReadContentQueryHandler _handler;

  private readonly ContentModel _content = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadContentQueryHandlerTests()
  {
    _handler = new(_contentQuerier.Object);

    _contentQuerier.Setup(x => x.ReadAsync(_content.Id, _cancellationToken)).ReturnsAsync(_content);
  }

  [Fact(DisplayName = "It should return null when no content could be found.")]
  public async Task It_should_return_null_when_no_content_could_be_found()
  {
    ReadContentQuery query = new(Guid.NewGuid());

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the content type found by ID.")]
  public async Task It_should_return_the_content_type_found_by_Id()
  {
    ReadContentQuery query = new(_content.Id);

    ContentModel? content = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(content);
    Assert.Same(_content, content);
  }
}

using Moq;

namespace Logitar.Cms.Core.Contents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly ReadContentQueryHandler _handler;

  public ReadContentQueryHandlerTests()
  {
    _handler = new(_contentQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no content is found.")]
  public async Task It_should_return_null_when_no_content_is_found()
  {
    ReadContentQuery query = new(Guid.Parse("9e07eaad-e117-4300-9748-b8d82bc6f66d"));
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }
}

using Logitar.Cms.Contracts.ContentTypes;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();

  private readonly ReadContentTypeQueryHandler _handler;

  public ReadContentTypeQueryHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object);
  }

  [Theory(DisplayName = "It should return null when no content type is found.")]
  [InlineData(null, null)]
  [InlineData("665e0c58-e0f3-477e-81c5-40c679182ff9", "BlogArticle")]
  public async Task It_should_return_null_when_no_content_type_is_found(string? id, string? uniqueName)
  {
    ReadContentTypeQuery query = new(id == null ? null : Guid.Parse(id), uniqueName);
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many content types are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_content_types_are_found()
  {
    CmsContentType title = new("BlogArticle")
    {
      Id = Guid.NewGuid()
    };
    _contentTypeQuerier.Setup(x => x.ReadAsync(title.Id, _cancellationToken)).ReturnsAsync(title);

    CmsContentType body = new("BlogAuthor")
    {
      Id = Guid.NewGuid()
    };
    _contentTypeQuerier.Setup(x => x.ReadAsync(body.UniqueName, _cancellationToken)).ReturnsAsync(body);

    ReadContentTypeQuery query = new(title.Id, body.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<CmsContentType>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

using Logitar.Cms.Contracts.ContentTypes;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadContentTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();

  private readonly ReadContentTypeQueryHandler _handler;

  private readonly ContentTypeModel _blogArticle = new("BlogArticle")
  {
    Id = Guid.NewGuid()
  };
  private readonly ContentTypeModel _blogAuthor = new("BlogAuthor")
  {
    Id = Guid.NewGuid(),
    IsInvariant = true
  };

  public ReadContentTypeQueryHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object);

    _contentTypeQuerier.Setup(x => x.ReadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);
    _contentTypeQuerier.Setup(x => x.ReadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);

    _contentTypeQuerier.Setup(x => x.ReadAsync(_blogArticle.UniqueName, _cancellationToken)).ReturnsAsync(_blogArticle);
    _contentTypeQuerier.Setup(x => x.ReadAsync(_blogAuthor.UniqueName, _cancellationToken)).ReturnsAsync(_blogAuthor);
  }

  [Fact(DisplayName = "It should return null when no content type could be found.")]
  public async Task It_should_return_null_when_no_content_type_could_be_found()
  {
    ReadContentTypeQuery query = new(Guid.NewGuid(), "BlogCategory");

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the content type found by ID.")]
  public async Task It_should_return_the_content_type_found_by_Id()
  {
    ReadContentTypeQuery query = new(_blogAuthor.Id, UniqueName: null);

    ContentTypeModel? contentType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Same(_blogAuthor, contentType);
  }

  [Fact(DisplayName = "It should return the content type found by locale.")]
  public async Task It_should_return_the_content_type_found_by_locale()
  {
    ReadContentTypeQuery query = new(Id: null, _blogArticle.UniqueName);

    ContentTypeModel? contentType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Same(_blogArticle, contentType);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when too many content types were found.")]
  public async Task It_should_throw_TooManyResultsException_when_too_many_content_types_were_found()
  {
    ReadContentTypeQuery query = new(_blogAuthor.Id, _blogArticle.UniqueName);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<ContentTypeModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

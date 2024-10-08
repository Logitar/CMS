using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Contracts.Search;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchContentTypesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();

  private readonly SearchContentTypesQueryHandler _handler;

  public SearchContentTypesQueryHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchContentTypesPayload payload = new();

    SearchResults<ContentTypeModel> results = new();
    _contentTypeQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(results);

    SearchContentTypesQuery query = new(payload);

    SearchResults<ContentTypeModel> searchResults = await _handler.Handle(query, _cancellationToken);
    Assert.Same(results, searchResults);
  }
}

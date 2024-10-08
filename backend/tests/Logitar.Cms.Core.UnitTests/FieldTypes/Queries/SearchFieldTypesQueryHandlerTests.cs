using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.Search;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchFieldTypesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();

  private readonly SearchFieldTypesQueryHandler _handler;

  public SearchFieldTypesQueryHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchFieldTypesPayload payload = new();

    SearchResults<FieldTypeModel> results = new();
    _fieldTypeQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(results);

    SearchFieldTypesQuery query = new(payload);

    SearchResults<FieldTypeModel> searchResults = await _handler.Handle(query, _cancellationToken);
    Assert.Same(results, searchResults);
  }
}

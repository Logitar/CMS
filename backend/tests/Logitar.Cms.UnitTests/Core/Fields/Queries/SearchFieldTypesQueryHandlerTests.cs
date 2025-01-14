using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Search;
using Moq;

namespace Logitar.Cms.Core.Fields.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchFieldTypesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _languageQuerier = new();

  private readonly SearchFieldTypesQueryHandler _handler;

  public SearchFieldTypesQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_Handle_Then_ResultsReturned()
  {
    SearchFieldTypesPayload payload = new();
    SearchResults<FieldTypeModel> fieldTypes = new();
    _languageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(fieldTypes);

    SearchFieldTypesQuery query = new(payload);
    SearchResults<FieldTypeModel> results = await _handler.Handle(query, _cancellationToken);
    Assert.Same(fieldTypes, results);
  }
}

using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using Moq;

namespace Logitar.Cms.Core.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchLanguagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly SearchLanguagesQueryHandler _handler;

  public SearchLanguagesQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchLanguagesPayload payload = new();

    SearchResults<LanguageModel> results = new();
    _languageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(results);

    SearchLanguagesQuery query = new(payload);

    SearchResults<LanguageModel> searchResults = await _handler.Handle(query, _cancellationToken);
    Assert.Same(results, searchResults);
  }
}

using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Contracts.Search;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchLanguagesQueryTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _americanEnglish;
  private readonly LanguageAggregate _canadianEnglish;
  private readonly LanguageAggregate _french;

  public SearchLanguagesQueryTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _americanEnglish = new(new LocaleUnit("en-US"));
    _canadianEnglish = new(new LocaleUnit("en-CA"));
    _french = new(new LocaleUnit("fr"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_americanEnglish, _canadianEnglish, _french]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchLanguagesPayload payload = new()
    {
      Search = new([new SearchTerm("es%")])
    };

    SearchLanguagesQuery query = new(payload);
    SearchResults<Language> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchLanguagesPayload payload = new()
    {
      Ids = (await _languageRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("en%")]),
      Sort = [new LanguageSortOption(LanguageSort.Code, isDescending: false)],
      Skip = 1,
      Limit = 1
    };

    payload.Ids.Remove(_americanEnglish.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchLanguagesQuery query = new(payload);
    SearchResults<Language> results = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, results.Total);
    Assert.Equal(payload.Limit, results.Items.Count);
    Assert.Equal(_canadianEnglish.Id.ToGuid(), results.Items.Single().Id);
  }
}

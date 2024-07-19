using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Languages.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchLanguagesQueryHandlerTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _americanEnglish;
  private readonly LanguageAggregate _canadianEnglish;
  private readonly LanguageAggregate _french;

  public SearchLanguagesQueryHandlerTests() : base()
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

  [Fact(DisplayName = "It should return empty results when no language matches.")]
  public async Task It_should_return_empty_results_when_no_language_matches()
  {
    SearchLanguagesPayload payload = new()
    {
      IdIn = [Guid.Empty]
    };
    SearchLanguagesQuery query = new(payload);

    SearchResults<Language> results = await Pipeline.ExecuteAsync(query);

    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct matching languages.")]
  public async Task It_should_return_the_correct_matching_languages()
  {
    SearchLanguagesPayload payload = new()
    {
      IdIn = (await _languageRepository.LoadAsync()).Select(language => language.Id.ToGuid()).ToList(),
      Search = new TextSearch([new SearchTerm("en%")]),
      Sort = [new LanguageSortOption(LanguageSort.DisplayName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    payload.IdIn.Add(Guid.Empty);
    payload.IdIn.Remove((await _languageRepository.LoadDefaultAsync()).Id.ToGuid());
    SearchLanguagesQuery query = new(payload);

    SearchResults<Language> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    Language language = Assert.Single(results.Items);
    Assert.Equal(_canadianEnglish.Id.ToGuid(), language.Id);
  }
}

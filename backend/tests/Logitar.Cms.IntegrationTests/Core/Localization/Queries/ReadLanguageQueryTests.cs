using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Core.Shared;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadLanguageQueryTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _language;

  public ReadLanguageQueryTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _language = new(new LocaleUnit("fr-CA"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_language);
  }

  [Fact(DisplayName = "It should return null when the language is not found.")]
  public async Task It_should_return_null_when_the_language_is_not_found()
  {
    ReadLanguageQuery query = new(Id: Guid.NewGuid(), Locale: "en-CA");
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_language.Id.ToGuid(), Locale: null);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_language.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should return the language found by locale.")]
  public async Task It_should_return_the_language_found_by_locale()
  {
    ReadLanguageQuery query = new(Id: null, _language.Locale.Code);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_language.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many languages are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_languages_are_found()
  {
    ReadLanguageQuery query = new(_language.Id.ToGuid(), Faker.Locale);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Language>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

using Logitar.Cms.Contracts.Languages;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Languages.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadLanguageQueryHandlerTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _french;

  public ReadLanguageQueryHandlerTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _french = new(new LocaleUnit("fr"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_french);
  }

  [Fact(DisplayName = "It should return the default language.")]
  public async Task It_should_return_the_default_language()
  {
    ReadLanguageQuery query = new(Id: null, Locale: null, IsDefault: true);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(DefaultLocale, language.Locale.Code);
    Assert.True(language.IsDefault);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_french.Id.ToGuid(), Locale: null, IsDefault: false);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_french.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should return the language found by locale.")]
  public async Task It_should_return_the_language_found_by_locale()
  {
    ReadLanguageQuery query = new(Id: null, DefaultLocale, IsDefault: false);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(DefaultLocale, language.Locale.Code);
  }
}

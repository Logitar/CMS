using Logitar.Cms.Contracts.Localization;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadLanguageQueryTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _french;
  private readonly LanguageAggregate _spanish;

  public ReadLanguageQueryTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _french = new(new LocaleUnit("fr"));
    _spanish = new(new LocaleUnit("es"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_french, _spanish]);
  }

  [Fact(DisplayName = "It should return null when the language is not found.")]
  public async Task It_should_return_null_when_the_language_is_not_found()
  {
    ReadLanguageQuery query = new(Id: Guid.NewGuid(), Code: "de", IsDefault: false);
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the default language.")]
  public async Task It_should_return_the_default_language()
  {
    LanguageAggregate @default = await _languageRepository.LoadDefaultAsync();

    ReadLanguageQuery query = new(Id: null, Code: null, IsDefault: true);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(@default.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_spanish.Id.ToGuid(), Code: null, IsDefault: false);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_spanish.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should return the language found by unique name.")]
  public async Task It_should_return_the_language_found_by_unique_name()
  {
    ReadLanguageQuery query = new(Id: null, _french.Locale.Code, IsDefault: false);
    Language? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_french.Id.ToGuid(), language.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many languages are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_languages_are_found()
  {
    ReadLanguageQuery query = new(_spanish.Id.ToGuid(), _french.Locale.Code, IsDefault: true);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Language>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}

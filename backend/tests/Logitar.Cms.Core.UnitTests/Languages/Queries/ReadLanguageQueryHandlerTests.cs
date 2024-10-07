using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Languages;
using Moq;

namespace Logitar.Cms.Core.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly ReadLanguageQueryHandler _handler;

  private readonly LanguageModel _americanEnglish = new(new LocaleModel("en-US"))
  {
    Id = Guid.NewGuid()
  };
  private readonly LanguageModel _canadianEnglish = new(new LocaleModel("en-CA"))
  {
    Id = Guid.NewGuid()
  };
  private readonly LanguageModel _canadianFrench = new(new LocaleModel("fr-CA"))
  {
    Id = Guid.NewGuid(),
    IsDefault = true
  };

  public ReadLanguageQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);

    _languageQuerier.Setup(x => x.ReadAsync(_americanEnglish.Id, _cancellationToken)).ReturnsAsync(_americanEnglish);
    _languageQuerier.Setup(x => x.ReadAsync(_canadianEnglish.Id, _cancellationToken)).ReturnsAsync(_canadianEnglish);
    _languageQuerier.Setup(x => x.ReadAsync(_canadianFrench.Id, _cancellationToken)).ReturnsAsync(_canadianFrench);

    _languageQuerier.Setup(x => x.ReadAsync(_americanEnglish.Locale.Code, _cancellationToken)).ReturnsAsync(_americanEnglish);
    _languageQuerier.Setup(x => x.ReadAsync(_canadianEnglish.Locale.Code, _cancellationToken)).ReturnsAsync(_canadianEnglish);
    _languageQuerier.Setup(x => x.ReadAsync(_canadianFrench.Locale.Code, _cancellationToken)).ReturnsAsync(_canadianFrench);

    _languageQuerier.Setup(x => x.ReadDefaultAsync(_cancellationToken)).ReturnsAsync(_canadianFrench);
  }

  [Fact(DisplayName = "It should return null when no language could be found.")]
  public async Task It_should_return_null_when_no_language_could_be_found()
  {
    ReadLanguageQuery query = new(Guid.NewGuid(), "fr-FR", IsDefault: false);

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the default language.")]
  public async Task It_should_return_the_default_language()
  {
    ReadLanguageQuery query = new(Id: null, Locale: null, IsDefault: true);

    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_canadianFrench, language);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_americanEnglish.Id, Locale: null, IsDefault: false);

    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_americanEnglish, language);
  }

  [Fact(DisplayName = "It should return the language found by locale.")]
  public async Task It_should_return_the_language_found_by_locale()
  {
    ReadLanguageQuery query = new(Id: null, _canadianEnglish.Locale.Code, IsDefault: false);

    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(language);
    Assert.Same(_canadianEnglish, language);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when too many languages were found.")]
  public async Task It_should_throw_TooManyResultsException_when_too_many_languages_were_found()
  {
    ReadLanguageQuery query = new(_americanEnglish.Id, _canadianEnglish.Locale.Code, IsDefault: true);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<LanguageModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}

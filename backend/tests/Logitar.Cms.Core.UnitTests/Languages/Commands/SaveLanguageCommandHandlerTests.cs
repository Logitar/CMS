using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SaveLanguageCommandHandler _handler;

  public SaveLanguageCommandHandlerTests()
  {
    _handler = new(_languageRepository.Object);
  }

  [Fact(DisplayName = "It should save the language.")]
  public async Task It_should_save_the_language()
  {
    LanguageAggregate language = new(new LocaleUnit("fr-CA"));
    _languageRepository.Setup(x => x.LoadAsync(language.Locale, _cancellationToken)).ReturnsAsync(language);

    SaveLanguageCommand command = new(language);
    await _handler.Handle(command, _cancellationToken);

    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when the locale is already used.")]
  public async Task It_should_throw_LocaleAlreadyUsedException_when_the_locale_is_already_used()
  {
    LanguageAggregate language = new(new LocaleUnit("fr-CA"));
    LanguageAggregate other = new(language.Locale);
    _languageRepository.Setup(x => x.LoadAsync(language.Locale, _cancellationToken)).ReturnsAsync(other);

    SaveLanguageCommand command = new(language);
    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(language.Locale.Code, exception.LocaleCode);
    Assert.Equal("Locale", exception.PropertyName);
  }
}

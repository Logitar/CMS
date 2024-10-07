using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SaveLanguageCommandHandler _handler;

  private readonly Language _language = new(new Locale("fr"), isDefault: true);

  public SaveLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should save the language.")]
  public async Task It_should_save_the_language()
  {
    _languageQuerier.Setup(x => x.FindIdAsync(_language.Locale, _cancellationToken)).ReturnsAsync(_language.Id);

    SaveLanguageCommand command = new(_language);

    await _handler.Handle(command, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(_language.Locale, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(_language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when the locale is already used.")]
  public async Task It_should_throw_LocaleAlreadyUsedException_when_the_locale_is_already_used()
  {
    Language conflict = new(_language.Locale);
    _languageQuerier.Setup(x => x.FindIdAsync(_language.Locale, _cancellationToken)).ReturnsAsync(conflict.Id);

    SaveLanguageCommand command = new(_language);

    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_language.Id.ToGuid(), exception.LanguageId);
    Assert.Equal(conflict.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(_language.Locale.Code, exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }
}

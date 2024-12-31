using Logitar.Identity.Core;
using Moq;

namespace Logitar.Cms.Core.Localization.Commands;

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

  [Fact(DisplayName = "Handle: it should save the language when there is no issue.")]
  public async Task Given_NoIssue_When_Handle_Then_LanguageSaved()
  {
    _languageQuerier.Setup(x => x.FindIdAsync(_language.Locale, _cancellationToken)).ReturnsAsync(_language.Id);

    SaveLanguageCommand command = new(_language);
    await _handler.Handle(command, _cancellationToken);

    _languageRepository.Verify(x => x.SaveAsync(_language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should throw LocaleAlreadyUsedException when there is a locale conflict.")]
  public async Task Given_LocaleConflict_When_Handle_Then_LocaleAlreadyUsedException()
  {
    LanguageId conflictId = LanguageId.NewId();
    _languageQuerier.Setup(x => x.FindIdAsync(_language.Locale, _cancellationToken)).ReturnsAsync(conflictId);

    SaveLanguageCommand command = new(_language);
    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_language.Id.ToGuid(), exception.LanguageId);
    Assert.Equal(conflictId.ToGuid(), exception.ConflictId);
    Assert.Equal(_language.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }
}

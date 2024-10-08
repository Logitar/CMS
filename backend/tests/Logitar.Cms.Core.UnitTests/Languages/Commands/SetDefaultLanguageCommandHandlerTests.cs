using Logitar.Cms.Contracts.Languages;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetDefaultLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SetDefaultLanguageCommandHandler _handler;

  private readonly Language _default = new(new Locale("en"), isDefault: true);
  private readonly Language _language = new(new Locale("fr"));

  public SetDefaultLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object);

    _languageRepository.Setup(x => x.LoadAsync(_default.Id, _cancellationToken)).ReturnsAsync(_default);
    _languageRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync(_language);
    _languageRepository.Setup(x => x.LoadDefaultAsync(_cancellationToken)).ReturnsAsync(_default);
  }

  [Fact(DisplayName = "It should do nothing when the language is already default.")]
  public async Task It_should_do_nothing_when_the_language_is_already_default()
  {
    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_default, _cancellationToken)).ReturnsAsync(model);

    SetDefaultLanguageCommand command = new(_default.Id.ToGuid());

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _languageRepository.Verify(x => x.LoadDefaultAsync(It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Language>>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    SetDefaultLanguageCommand command = new(Guid.NewGuid());

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task It_should_set_the_default_language()
  {
    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_language, _cancellationToken)).ReturnsAsync(model);

    SetDefaultLanguageCommand command = new(_language.Id.ToGuid());
    command.Contextualize();

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.False(_default.IsDefault);
    Assert.True(_language.IsDefault);

    _languageRepository.Verify(x => x.LoadDefaultAsync(_cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Language>>(y => y.SequenceEqual(new Language[] { _default, _language })),
      _cancellationToken), Times.Once);
  }
}

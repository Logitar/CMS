using Logitar.Cms.Contracts.Languages;
using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetDefaultLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SetDefaultLanguageCommandHandler _handler;

  public SetDefaultLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should not change the language when it is already the default.")]
  public async Task It_should_not_change_the_language_when_it_is_already_the_default()
  {
    LanguageAggregate aggregate = new(new LocaleUnit("fr"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(aggregate.Id, _cancellationToken)).ReturnsAsync(aggregate);

    Language language = new();
    _languageQuerier.Setup(x => x.ReadAsync(aggregate, _cancellationToken)).ReturnsAsync(language);

    SetDefaultLanguageCommand command = new(aggregate.Id.ToGuid());
    Language? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(language, result);

    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<LanguageAggregate>>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the language cannot be found.")]
  public async Task It_should_return_null_when_the_language_cannot_be_found()
  {
    SetDefaultLanguageCommand command = new(Guid.Empty);
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }
}

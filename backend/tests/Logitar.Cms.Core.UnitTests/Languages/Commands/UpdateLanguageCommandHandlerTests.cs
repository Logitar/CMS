using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Languages;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateLanguageCommandHandler _handler;

  public UpdateLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    UpdateLanguagePayload payload = new();
    UpdateLanguageCommand command = new(Guid.NewGuid(), payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateLanguagePayload payload = new()
    {
      Locale = "test"
    };
    UpdateLanguageCommand command = new(Guid.NewGuid(), payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("LocaleValidator", error.ErrorCode);
    Assert.Equal("Locale", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    Language language = new(new Locale("fr-CA"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    UpdateLanguagePayload payload = new()
    {
      Locale = "fr"
    };
    UpdateLanguageCommand command = new(language.Id.ToGuid(), payload);
    command.Contextualize();

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(language) && y.Language.Locale.Code == payload.Locale),
      _cancellationToken), Times.Once);
  }
}

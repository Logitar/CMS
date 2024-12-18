using FluentValidation.Results;
using Logitar.Cms.Core.Localization.Models;
using Logitar.EventSourcing;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLanguageCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IMediator> _mediator = new();

  private readonly UpdateLanguageCommandHandler _handler;

  public UpdateLanguageCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object, _mediator.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "Handle: it should return null when the language was not found.")]
  public async Task Given_LanguageNotFound_When_Handle_Then_NullReturned()
  {
    UpdateLanguageCommand command = new(Guid.NewGuid(), new UpdateLanguagePayload());
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateLanguagePayload payload = new()
    {
      Locale = "invalid"
    };
    UpdateLanguageCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("LocaleValidator", failure.ErrorCode);
    Assert.Equal("Locale", failure.PropertyName);
  }

  [Fact(DisplayName = "Handle: it should update the language.")]
  public async Task Given_Changes_When_Handle_Then_LanguageUpdated()
  {
    Language language = new(new Locale("fr"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    UpdateLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };
    UpdateLanguageCommand command = new(language.Id.ToGuid(), payload);
    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Equal(_actorId, language.UpdatedBy);
    Assert.Equal(payload.Locale, language.Locale.Value);

    _mediator.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(language)), _cancellationToken), Times.Once);
  }
}

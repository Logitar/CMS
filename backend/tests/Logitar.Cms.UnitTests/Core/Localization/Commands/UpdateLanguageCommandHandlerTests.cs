using Logitar.Cms.Core.Localization.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Security.Cryptography;
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

  private readonly Language _english = new(new Locale("en"), isDefault: true);

  public UpdateLanguageCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object, _mediator.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
  }

  [Fact(DisplayName = "Handle: it update replace an existing language.")]
  public async Task Given_LanguageExists_When_Handle_Then_LanguageUpdated()
  {
    LanguageModel language = new();
    _languageQuerier.Setup(x => x.ReadAsync(_english, _cancellationToken)).ReturnsAsync(language);

    UpdateLanguagePayload payload = new()
    {
      Locale = "en-US"
    };
    UpdateLanguageCommand command = new(_english.Id.ToGuid(), payload);
    LanguageModel? result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(language, result);

    Assert.Equal(_actorId, _english.UpdatedBy);
    Assert.True(_english.IsDefault);
    Assert.Equal(payload.Locale, _english.Locale.Code);

    _mediator.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(_english)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should return null when updating a language that does not exist.")]
  public async Task Given_UpdatingNotFound_When_Handle_Then_NullReturned()
  {
    UpdateLanguagePayload payload = new();
    UpdateLanguageCommand command = new(Guid.NewGuid(), payload);
    LanguageModel? language = await _handler.Handle(command, _cancellationToken);
    Assert.Null(language);
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateLanguagePayload payload = new()
    {
      Locale = RandomStringGenerator.GetString(50)
    };
    UpdateLanguageCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Locale");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
  }
}

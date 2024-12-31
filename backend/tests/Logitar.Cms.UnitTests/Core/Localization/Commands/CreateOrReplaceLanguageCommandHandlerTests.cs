using Logitar.Cms.Core.Localization.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceLanguageCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IMediator> _mediator = new();

  private readonly CreateOrReplaceLanguageCommandHandler _handler;

  private readonly Language _english = new(new Locale("en"), isDefault: true);

  public CreateOrReplaceLanguageCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object, _mediator.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
  }

  [Theory(DisplayName = "Handle: it should create a new language.")]
  [InlineData(null)]
  [InlineData("d855cace-5c65-4eab-9f48-19095c276c36")]
  public async Task Given_NewLanguage_When_Handle_Then_LanguageCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    LanguageModel language = new();
    _languageQuerier.Setup(x => x.ReadAsync(It.IsAny<Language>(), _cancellationToken)).ReturnsAsync(language);

    CreateOrReplaceLanguagePayload payload = new("en");
    CreateOrReplaceLanguageCommand command = new(id, payload, Version: null);
    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.Language);
    Assert.Same(language, result.Language);
    Assert.True(result.Created);

    _mediator.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => (!id.HasValue || y.Language.Id.ToGuid().Equals(id.Value))
        && y.Language.CreatedBy == _actorId
        && !y.Language.IsDefault
        && y.Language.Locale.Code == payload.Locale),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should replace an existing language.")]
  public async Task Given_ExistingLanguageNoVersion_When_Handle_Then_LanguageReplaced()
  {
    LanguageModel language = new();
    _languageQuerier.Setup(x => x.ReadAsync(_english, _cancellationToken)).ReturnsAsync(language);

    CreateOrReplaceLanguagePayload payload = new("en-US");
    CreateOrReplaceLanguageCommand command = new(_english.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.Language);
    Assert.Same(language, result.Language);
    Assert.False(result.Created);

    Assert.Equal(_actorId, _english.UpdatedBy);
    Assert.True(_english.IsDefault);
    Assert.Equal(payload.Locale, _english.Locale.Code);

    _mediator.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(_english)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should return null when updating a language that does not exist.")]
  public async Task Given_UpdatingNotFound_When_Handle_Then_NullReturned()
  {
    CreateOrReplaceLanguagePayload payload = new("en");
    CreateOrReplaceLanguageCommand command = new(Id: null, payload, Version: 1);
    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Language);
    Assert.False(result.Created);
  }

  [Fact(DisplayName = "Handle: it should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceLanguagePayload payload = new();
    CreateOrReplaceLanguageCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Locale");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
  }

  [Fact(DisplayName = "Handle: it should update an existing language.")]
  public async Task Given_ExistingLanguageWithVersion_When_Handle_Then_LanguageUpdated()
  {
    _english.SetLocale(new Locale("en-US"), _actorId);
    long version = _english.Version;

    Language reference = new(_english.Locale, _english.IsDefault, _actorId, _english.Id);
    _languageRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Locale locale = new("en-CA");
    _english.SetLocale(locale, _actorId);

    LanguageModel language = new();
    _languageQuerier.Setup(x => x.ReadAsync(_english, _cancellationToken)).ReturnsAsync(language);

    CreateOrReplaceLanguagePayload payload = new("en-US");
    CreateOrReplaceLanguageCommand command = new(_english.Id.ToGuid(), payload, version);
    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);

    Assert.NotNull(result.Language);
    Assert.Same(language, result.Language);
    Assert.False(result.Created);

    Assert.Equal(_actorId, _english.UpdatedBy);
    Assert.True(_english.IsDefault);
    Assert.Equal(locale, _english.Locale);

    _mediator.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(_english)), _cancellationToken), Times.Once);
  }
}

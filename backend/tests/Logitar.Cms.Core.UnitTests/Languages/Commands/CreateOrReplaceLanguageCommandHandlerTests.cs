using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Languages;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceLanguageCommandHandler _handler;

  private readonly Language _language = new(new Locale("en-CA"));

  public CreateOrReplaceLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object, _sender.Object);

    _languageRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync(_language);
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(null)]
  [InlineData("8f8a2c24-eb37-461f-ae39-77f70ef6762d")]
  public async Task It_should_create_a_new_language(string? idValue)
  {
    bool idParsed = Guid.TryParse(idValue, out Guid id);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(It.IsAny<Language>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLanguagePayload payload = new("fr");
    CreateOrReplaceLanguageCommand command = new(id, payload, Version: null);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Language);
    Assert.True(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => (!idParsed || y.Language.Id.ToGuid() == id)
        && !y.Language.IsDefault && y.Language.Locale.Code == payload.Locale),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task It_should_replace_an_existing_language()
  {
    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_language, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLanguagePayload payload = new("en");
    CreateOrReplaceLanguageCommand command = new(_language.Id.ToGuid(), payload, Version: null);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Language);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(_language)
        && !y.Language.IsDefault && y.Language.Locale.Code == payload.Locale),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return an empty result when updating a language that does not exist.")]
  public async Task It_should_return_an_empty_result_when_updating_a_language_that_does_not_exist()
  {
    CreateOrReplaceLanguagePayload payload = new("en");
    CreateOrReplaceLanguageCommand command = new(Guid.NewGuid(), payload, Version: 1);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Language);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(It.IsAny<SaveLanguageCommand>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceLanguagePayload payload = new();
    CreateOrReplaceLanguageCommand command = new(Id: null, payload, Version: null);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("LocaleValidator", error.ErrorCode);
    Assert.Equal("Locale", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    long version = _language.Version;

    Language reference = new(_language.Locale, _language.IsDefault, _language.CreatedBy, _language.Id);
    _languageRepository.Setup(x => x.LoadAsync(_language.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Locale locale = new("en");
    _language.Locale = locale;
    _language.Update();

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(_language, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceLanguagePayload payload = new("en-CA");
    CreateOrReplaceLanguageCommand command = new(_language.Id.ToGuid(), payload, version);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result.Language);
    Assert.False(result.Created);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(_language) && y.Language.Locale.Equals(locale)),
      _cancellationToken), Times.Once);
  }
}

using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Languages;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateLanguageCommandHandler _handler;

  public CreateLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateLanguagePayload payload = new(locale: "invalid");
    CreateLanguageCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("LocaleValidator", error.ErrorCode);
    Assert.Equal("Locale", error.PropertyName);
  }
}

using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Moq;

namespace Logitar.Cms.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateApiKeyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();
  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();

  private readonly CreateApiKeyCommandHandler _handler;

  public CreateApiKeyCommandHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object, _apiKeyRepository.Object, _passwordManager.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateApiKeyPayload payload = new("Default API Key");
    payload.CustomAttributes.Add(new CustomAttribute("123_Key", "Value"));
    CreateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("IdentifierValidator", error.ErrorCode);
    Assert.Equal("CustomAttributes[0].Key", error.PropertyName);
  }
}

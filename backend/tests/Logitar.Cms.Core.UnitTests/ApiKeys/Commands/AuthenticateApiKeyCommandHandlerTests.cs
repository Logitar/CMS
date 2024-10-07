using FluentValidation.Results;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Cms.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class AuthenticateApiKeyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();
  private readonly Mock<IApiKeyRepository> _apiKeyRepository = new();

  private readonly AuthenticateApiKeyCommandHandler _handler;

  public AuthenticateApiKeyCommandHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object, _apiKeyRepository.Object);
  }

  [Fact(DisplayName = "It should throw ApiKeyIsExpiredException when the API key is notexpired.")]
  public async Task It_should_throw_ApiKeyIsExpiredException_when_the_Api_key_is_expired()
  {
    const int MillisecondsDelay = 100;

    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Test"), new PasswordMock(secret));
    apiKey.SetExpiration(DateTime.Now.AddMilliseconds(MillisecondsDelay));
    _apiKeyRepository.Setup(x => x.LoadAsync(apiKey.Id, _cancellationToken)).ReturnsAsync(apiKey);

    await Task.Delay(MillisecondsDelay);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, secret));
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<ApiKeyIsExpiredException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(apiKey.Id, exception.ApiKeyId);
  }

  [Fact(DisplayName = "It should throw IncorrectApiKeySecretException when the secret is not correct.")]
  public async Task It_should_throw_IncorrectApiKeySecretException_when_the_secret_is_not_correct()
  {
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Test"), new PasswordMock("Test123!"));
    _apiKeyRepository.Setup(x => x.LoadAsync(apiKey.Id, _cancellationToken)).ReturnsAsync(apiKey);

    string secret = RandomStringGenerator.GetBase64String(XApiKey.SecretLength, out _);
    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, secret));
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<IncorrectApiKeySecretException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(apiKey.Id, exception.ApiKeyId);
    Assert.Equal(secret, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidXApiKeyException when the value is not valid.")]
  public async Task It_should_throw_InvalidXApiKeyException_when_the_value_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new("invalid");
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<InvalidXApiKeyException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.XApiKey, exception.XApiKey);
    Assert.Equal("XApiKey", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateApiKeyPayload payload = new();
    AuthenticateApiKeyCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("XApiKey", error.PropertyName);
  }
}

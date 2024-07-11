using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateApiKeyCommandHandlerTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  public AuthenticateApiKeyCommandHandlerTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
  }

  [Fact(DisplayName = "It should authenticate the API key.")]
  public async Task It_should_authenticate_the_Api_key()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secretString);
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Test"), secret, tenantId: null, ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    AuthenticateApiKeyPayload payload = new(XApiKey.Encode(apiKey.Id, secretString));
    AuthenticateApiKeyCommand command = new(payload);
    ApiKey result = await Pipeline.ExecuteAsync(command);

    Assert.Equal(apiKey.Id.ToGuid(), result.Id);
    Assert.Equal(apiKey.Version + 1, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(apiKey.CreatedOn.AsUniversalTime(), result.CreatedOn);
    Assert.Equal(apiKey.Id.ToGuid(), result.UpdatedBy.Id);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.Null(result.XApiKey);
    Assert.Equal(result.UpdatedOn, result.AuthenticatedOn);
  }
}

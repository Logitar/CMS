using FluentValidation;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core.ApiKeys.Validators;
using Logitar.Identity.Domain.ApiKeys;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Commands;

internal class AuthenticateApiKeyCommandHandler : IRequestHandler<AuthenticateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;

  public AuthenticateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
  }

  public async Task<ApiKey> Handle(AuthenticateApiKeyCommand command, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = command.Payload;
    new AuthenticateApiKeyValidator().ValidateAndThrow(payload);

    XApiKey xApiKey;
    try
    {
      xApiKey = XApiKey.Decode(payload.XApiKey);
    }
    catch (Exception innerException)
    {
      throw new InvalidXApiKeyException(payload.XApiKey, nameof(payload.XApiKey), innerException);
    }

    ApiKeyAggregate apiKey = await _apiKeyRepository.LoadAsync(xApiKey.Id, cancellationToken)
      ?? throw new ApiKeyNotFoundException(xApiKey.Id, nameof(payload.XApiKey));
    apiKey.Authenticate(xApiKey.Secret);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }
}

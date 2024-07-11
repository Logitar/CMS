using FluentValidation;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core.ApiKeys.Validators;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Commands;

internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;

  public CreateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository, IPasswordManager passwordManager)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _passwordManager = passwordManager;
  }

  public async Task<ApiKey> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateApiKeyPayload payload = command.Payload;
    new CreateApiKeyValidator().ValidateAndThrow(payload);

    DisplayNameUnit displayName = new(payload.DisplayName);
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secretString);
    ApiKeyAggregate apiKey = new(displayName, secret, tenantId: null, command.ActorId)
    {
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.SetExpiration(payload.ExpiresOn.Value);
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    apiKey.Update(command.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKey result = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
    result.XApiKey = XApiKey.Encode(apiKey, secretString);
    return result;
  }
}

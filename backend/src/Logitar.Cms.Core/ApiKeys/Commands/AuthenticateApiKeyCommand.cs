using Logitar.Cms.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Commands;

public record AuthenticateApiKeyCommand(AuthenticateApiKeyPayload Payload) : Activity, IRequest<ApiKey>;

using Logitar.Cms.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Commands;

public record CreateApiKeyCommand(CreateApiKeyPayload Payload) : Activity, IRequest<ApiKey>;

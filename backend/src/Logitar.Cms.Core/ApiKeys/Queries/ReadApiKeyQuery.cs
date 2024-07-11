using Logitar.Cms.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Queries;

public record ReadApiKeyQuery(Guid Id) : Activity, IRequest<ApiKey?>;

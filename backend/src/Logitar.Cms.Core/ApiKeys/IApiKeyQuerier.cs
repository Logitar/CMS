using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Identity.Domain.ApiKeys;

namespace Logitar.Cms.Core.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}

using Logitar.Cms.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Cms.Core.ApiKeys.Queries;

internal class ReadApiKeyQueryHandler : IRequestHandler<ReadApiKeyQuery, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public ReadApiKeyQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<ApiKey?> Handle(ReadApiKeyQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

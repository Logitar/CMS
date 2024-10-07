using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core.ApiKeys;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ApiKeyEntity> _apiKeys;

  public ApiKeyQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
  }

  public async Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apiKey.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The API key entity 'AggregateId={apiKey.Id.AggregateId}' could not be found.");
  }
  public async Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return apiKey == null ? null : await MapAsync(apiKey, cancellationToken);
  }

  private async Task<ApiKey> MapAsync(ApiKeyEntity apiKey, CancellationToken cancellationToken)
    => (await MapAsync([apiKey], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<ApiKey>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(mapper.ToApiKey).ToArray();
  }
}

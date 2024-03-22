using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core.Archetypes;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ArchetypeQuerier : IArchetypeQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ArchetypeEntity> _archetypes;

  public ArchetypeQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _archetypes = context.Archetypes;
  }

  public async Task<Archetype> ReadAsync(ArchetypeAggregate archetype, CancellationToken cancellationToken)
  {
    return await ReadAsync(archetype.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The archetype entity 'AggregateId={archetype.Id.AggregateId}' could not be found.");
  }
  public async Task<Archetype?> ReadAsync(ArchetypeId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Archetype?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ArchetypeEntity? archetype = await _archetypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return archetype == null ? null : await MapAsync(archetype, cancellationToken);
  }

  public async Task<Archetype?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = uniqueName.Trim().ToUpper();

    ArchetypeEntity? archetype = await _archetypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return archetype == null ? null : await MapAsync(archetype, cancellationToken);
  }

  private async Task<Archetype> MapAsync(ArchetypeEntity archetype, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = archetype.GetActorIds();
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToArchetype(archetype);
  }
}

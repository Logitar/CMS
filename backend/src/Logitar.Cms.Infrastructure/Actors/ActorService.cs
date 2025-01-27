﻿using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Caching;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> _actors;
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService, IdentityContext context)
  {
    _cacheService = cacheService;
    _actors = context.Actors;
  }

  public async Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, ActorModel> actors = new(capacity);
    HashSet<string> missingIds = new(capacity);

    foreach (ActorId id in ids)
    {
      ActorModel? actor = _cacheService.GetActor(id);
      if (actor == null)
      {
        missingIds.Add(id.Value);
      }
      else
      {
        actors[id] = actor;
      }
    }

    ActorEntity[] entities = await _actors.AsNoTracking()
      .Where(actor => missingIds.Contains(actor.Id))
      .ToArrayAsync(cancellationToken);
    foreach (ActorEntity entity in entities)
    {
      ActorId id = new(entity.Id);
      ActorModel actor = Mapper.ToActor(entity);
      actors[id] = actor;
    }

    foreach (ActorModel actor in actors.Values)
    {
      _cacheService.SetActor(actor);
    }

    return actors.Values;
  }
}

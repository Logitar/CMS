﻿using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

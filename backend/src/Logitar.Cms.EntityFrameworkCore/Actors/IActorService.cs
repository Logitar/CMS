using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Actors;

internal interface IActorService
{
  Task<IEnumerable<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

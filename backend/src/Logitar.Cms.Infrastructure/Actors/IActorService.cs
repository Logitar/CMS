using Logitar.Cms.Core.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

using Logitar.Cms.Core.Models;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

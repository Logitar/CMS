using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
}

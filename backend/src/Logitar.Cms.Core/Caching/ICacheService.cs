using Logitar.Cms.Core.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Caching;

public interface ICacheService
{
  ActorModel? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(ActorModel actor);
}

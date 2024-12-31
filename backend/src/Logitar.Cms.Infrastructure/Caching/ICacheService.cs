using Logitar.Cms.Core.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Infrastructure.Caching;

public interface ICacheService
{
  ActorModel? GetActor(ActorId id);
  void SetActor(ActorModel actor);
}

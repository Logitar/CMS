using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Caching;

public interface ICacheService
{
  ConfigurationModel? Configuration { get; set; }

  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
}

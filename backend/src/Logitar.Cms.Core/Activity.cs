using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public abstract record Activity
{
  public ActorId GetActorId() => new();
}

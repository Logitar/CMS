using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }
}

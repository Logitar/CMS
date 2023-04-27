using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public interface IApplicationContext
{
  AggregateId ActorId { get; }
}

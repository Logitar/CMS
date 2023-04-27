using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public interface IApplicationContext
{
  AggregateId ActorId { get; }

  ConfigurationAggregate Configuration { get; }
}

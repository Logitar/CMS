using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public ConfigurationAggregate(AggregateId id) : base(id) { }
}

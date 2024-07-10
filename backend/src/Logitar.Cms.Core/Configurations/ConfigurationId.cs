using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Configurations;

public record ConfigurationId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ConfigurationId()
  {
    AggregateId = new(Guid.Empty);
  }
}

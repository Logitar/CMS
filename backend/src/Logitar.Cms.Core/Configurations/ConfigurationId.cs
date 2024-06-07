using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Configurations;

public readonly struct ConfigurationId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ConfigurationId()
  {
    AggregateId = new("CONFIGURATION");
  }

  public static bool operator ==(ConfigurationId left, ConfigurationId right) => left.Equals(right);
  public static bool operator !=(ConfigurationId left, ConfigurationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ConfigurationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

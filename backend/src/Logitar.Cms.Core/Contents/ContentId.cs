using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public readonly struct ContentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentId(Guid id)
  {
    AggregateId = new(id);
  }
  public ContentId(string id)
  {
    AggregateId = new(id);
  }
  public ContentId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static ContentId NewId() => new(AggregateId.NewId());
  public static ContentId? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(ContentId left, ContentId right) => left.Equals(right);
  public static bool operator !=(ContentId left, ContentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

using Logitar.EventSourcing;

namespace Logitar.Cms.Core.ContentTypes;

public readonly struct ContentTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentTypeId(Guid id) : this(new AggregateId(id))
  {
  }
  public ContentTypeId(string value) : this(new AggregateId(value))
  {
  }
  public ContentTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static ContentTypeId NewId() => new(Guid.NewGuid());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(ContentTypeId left, ContentTypeId right) => left.Equals(right);
  public static bool operator !=(ContentTypeId left, ContentTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

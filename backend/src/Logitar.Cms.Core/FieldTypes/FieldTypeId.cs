using Logitar.EventSourcing;

namespace Logitar.Cms.Core.FieldTypes;

public readonly struct FieldTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public FieldTypeId(Guid id) : this(new AggregateId(id))
  {
  }
  public FieldTypeId(string value) : this(new AggregateId(value))
  {
  }
  public FieldTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static FieldTypeId NewId() => new(Guid.NewGuid());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

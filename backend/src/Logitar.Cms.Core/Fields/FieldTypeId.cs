using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Fields;

public readonly struct FieldTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public FieldTypeId(Guid id)
  {
    AggregateId = new(id);
  }
  public FieldTypeId(string id)
  {
    AggregateId = new(id);
  }
  public FieldTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static FieldTypeId NewId() => new(AggregateId.NewId());
  public static FieldTypeId? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

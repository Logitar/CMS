using Logitar.EventSourcing;

namespace Logitar.Cms.Core.FieldTypes;

public readonly struct FieldTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public FieldTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }
  public FieldTypeId(Guid value)
  {
    AggregateId = new(value);
  }
  public FieldTypeId(string value)
  {
    AggregateId = new(value);
  }

  public static FieldTypeId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId languageId && languageId.AggregateId.Equals(AggregateId);
  public override int GetHashCode() => AggregateId.GetHashCode();
  public override string ToString() => AggregateId.ToString();
}

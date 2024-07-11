using Logitar.EventSourcing;

namespace Logitar.Cms.Core.FieldTypes;

public record FieldTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public FieldTypeId(Guid value) : this(new AggregateId(value))
  {
  }
  public FieldTypeId(string value) : this(new AggregateId(value))
  {
  }
  public FieldTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static FieldTypeId NewId() => new(AggregateId.NewId());

  public static FieldTypeId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}

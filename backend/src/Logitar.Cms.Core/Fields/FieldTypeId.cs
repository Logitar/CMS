using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Fields;

public record FieldTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public FieldTypeId(Guid id) : this(new AggregateId(id))
  {
  }
  public FieldTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private FieldTypeId(string value)
  {
    value = value.Trim();
    new IdValidator().ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();

  public static FieldTypeId NewId() => new(AggregateId.NewId());

  public static FieldTypeId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}

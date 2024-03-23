using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public record ContentTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentTypeId(Guid id) : this(new AggregateId(id))
  {
  }
  public ContentTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private ContentTypeId(string value)
  {
    value = value.Trim();
    new IdValidator().ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();

  public static ContentTypeId NewId() => new(AggregateId.NewId());

  public static ContentTypeId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}

using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public record ContentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentId(Guid id) : this(new AggregateId(id))
  {
  }
  public ContentId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private ContentId(string value)
  {
    value = value.Trim();
    new IdValidator().ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();

  public static ContentId NewId() => new(AggregateId.NewId());

  public static ContentId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}

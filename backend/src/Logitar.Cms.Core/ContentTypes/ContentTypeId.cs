using Logitar.EventSourcing;

namespace Logitar.Cms.Core.ContentTypes;

public record ContentTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentTypeId(Guid value) : this(new AggregateId(value))
  {
  }
  public ContentTypeId(string value) : this(new AggregateId(value))
  {
  }
  public ContentTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static ContentTypeId NewId() => new(AggregateId.NewId());

  public static ContentTypeId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}

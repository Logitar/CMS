using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public record ContentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentId(Guid value) : this(new AggregateId(value))
  {
  }
  public ContentId(string value) : this(new AggregateId(value))
  {
  }
  public ContentId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static ContentId NewId() => new(AggregateId.NewId());

  public static ContentId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}

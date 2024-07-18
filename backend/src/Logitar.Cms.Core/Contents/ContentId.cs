using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public readonly struct ContentId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }
  public ContentId(Guid value)
  {
    AggregateId = new(value);
  }
  public ContentId(string value)
  {
    AggregateId = new(value);
  }

  public static ContentId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(ContentId left, ContentId right) => left.Equals(right);
  public static bool operator !=(ContentId left, ContentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentId languageId && languageId.AggregateId.Equals(AggregateId);
  public override int GetHashCode() => AggregateId.GetHashCode();
  public override string ToString() => AggregateId.ToString();
}

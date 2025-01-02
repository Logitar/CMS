using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public readonly struct ContentId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ContentId(Guid value)
  {
    StreamId = new(value);
  }
  public ContentId(string value)
  {
    StreamId = new(value);
  }
  public ContentId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static ContentId NewId() => new(StreamId.NewId());

  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(ContentId left, ContentId right) => left.Equals(right);
  public static bool operator !=(ContentId left, ContentId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public readonly struct ContentTypeId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ContentTypeId(Guid value)
  {
    StreamId = new(value);
  }
  public ContentTypeId(string value)
  {
    StreamId = new(value);
  }
  public ContentTypeId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static ContentTypeId NewId() => new(StreamId.NewId());

  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(ContentTypeId left, ContentTypeId right) => left.Equals(right);
  public static bool operator !=(ContentTypeId left, ContentTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

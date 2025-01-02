using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Fields;

public readonly struct FieldTypeId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public FieldTypeId(Guid value)
  {
    StreamId = new(value);
  }
  public FieldTypeId(string value)
  {
    StreamId = new(value);
  }
  public FieldTypeId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static FieldTypeId NewId() => new(StreamId.NewId());

  public Guid ToGuid() => StreamId.ToGuid();

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

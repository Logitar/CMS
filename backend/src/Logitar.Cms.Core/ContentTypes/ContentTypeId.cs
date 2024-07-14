using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Cms.Core.ContentTypes;

public readonly struct ContentTypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ContentTypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }
  public ContentTypeId(Guid value)
  {
    AggregateId = new(value);
  }
  public ContentTypeId(string value)
  {
    AggregateId = new(value);
  }

  public static ContentTypeId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(ContentTypeId left, ContentTypeId right) => left.Equals(right);
  public static bool operator !=(ContentTypeId left, ContentTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentTypeId languageId && languageId.AggregateId.Equals(AggregateId);
  public override int GetHashCode() => AggregateId.GetHashCode();
  public override string ToString() => AggregateId.ToString();
}

using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Cms.Core.Languages;

public readonly struct LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(Guid id) : this(new AggregateId(id))
  {
  }
  public LanguageId(string value) : this(new AggregateId(value))
  {
  }
  public LanguageId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static LanguageId NewId() => new(Guid.NewGuid());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

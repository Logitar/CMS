using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Cms.Core.Languages;

public readonly struct LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }
  public LanguageId(Guid value)
  {
    AggregateId = new(value);
  }
  public LanguageId(string value)
  {
    AggregateId = new(value);
  }

  public static LanguageId NewId() => new(AggregateId.NewId());

  public Guid ToGuid() => AggregateId.ToGuid();

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId languageId && languageId.AggregateId.Equals(AggregateId);
  public override int GetHashCode() => AggregateId.GetHashCode();
  public override string ToString() => AggregateId.ToString();
}

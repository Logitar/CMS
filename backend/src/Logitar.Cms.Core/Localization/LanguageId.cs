using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Localization;

public readonly struct LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static LanguageId NewId() => new(AggregateId.NewId());

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

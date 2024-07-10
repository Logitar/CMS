using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Languages;

public record LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(Guid value) : this(new AggregateId(value))
  {
  }
  public LanguageId(string value) : this(new AggregateId(value))
  {
  }
  public LanguageId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  public static LanguageId NewId() => new(AggregateId.NewId());

  public static LanguageId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }

  public Guid ToGuid() => AggregateId.ToGuid();
}

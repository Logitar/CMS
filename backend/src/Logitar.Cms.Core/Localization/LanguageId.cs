using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Localization;

public record LanguageId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public LanguageId(Guid id) : this(new AggregateId(id))
  {
  }
  public LanguageId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private LanguageId(string value)
  {
    value = value.Trim();
    new IdValidator().ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();

  public static LanguageId NewId() => new(AggregateId.NewId());

  public static LanguageId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}

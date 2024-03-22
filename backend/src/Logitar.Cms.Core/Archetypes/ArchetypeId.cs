using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Archetypes;

public record ArchetypeId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ArchetypeId(Guid id) : this(new AggregateId(id))
  {
  }
  public ArchetypeId(AggregateId aggregateId)
  {
    AggregateId = aggregateId;
  }

  private ArchetypeId(string value)
  {
    value = value.Trim();
    new IdValidator().ValidateAndThrow(value);

    AggregateId = new(value);
  }

  public Guid ToGuid() => AggregateId.ToGuid();

  public static ArchetypeId NewId() => new(AggregateId.NewId());

  public static ArchetypeId? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new(value.Trim());
  }
}

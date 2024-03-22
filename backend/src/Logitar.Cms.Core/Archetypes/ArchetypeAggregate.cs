using Logitar.Cms.Core.Archetypes.Events;
using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Archetypes;

public class ArchetypeAggregate : AggregateRoot
{
  private ArchetypeUpdatedEvent _updatedEvent = new();

  public new ArchetypeId Id => new(base.Id);

  public bool IsInvariant { get; private set; }

  private IdentifierUnit? _uniqueName = null;
  public IdentifierUnit UniqueName => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  public ArchetypeAggregate(AggregateId id) : base(id)
  {
  }

  public ArchetypeAggregate(IdentifierUnit identifier, ActorId actorId = default, ArchetypeId? id = null)
    : base((id ?? ArchetypeId.NewId()).AggregateId)
  {
    bool isInvariant = true;
    Raise(new ArchetypeCreatedEvent(isInvariant, identifier, actorId));
  }
  protected virtual void Apply(ArchetypeCreatedEvent @event)
  {
    IsInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      _updatedEvent.OccurredOn = DateTime.Now;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(ArchetypeUpdatedEvent @event)
  {
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}

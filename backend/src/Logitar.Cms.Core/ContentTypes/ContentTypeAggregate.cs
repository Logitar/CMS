using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public class ContentTypeAggregate : AggregateRoot
{
  private ContentTypeUpdatedEvent _updatedEvent = new();

  public new ContentTypeId Id => new(base.Id);

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

  public ContentTypeAggregate(AggregateId id) : base(id)
  {
  }

  public ContentTypeAggregate(IdentifierUnit identifier, ActorId actorId = default, ContentTypeId? id = null)
    : base((id ?? ContentTypeId.NewId()).AggregateId)
  {
    bool isInvariant = true;
    Raise(new ContentTypeCreatedEvent(isInvariant, identifier, actorId));
  }
  protected virtual void Apply(ContentTypeCreatedEvent @event)
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
  protected virtual void Apply(ContentTypeUpdatedEvent @event)
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

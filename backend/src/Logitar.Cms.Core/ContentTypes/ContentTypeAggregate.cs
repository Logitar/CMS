using Logitar.Cms.Contracts;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes;

public class ContentTypeAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new ReadOnlyUniqueNameSettings();

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
      if (_displayName != value)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Change<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updatedEvent.Description = new Change<DescriptionUnit>(value);
      }
    }
  }

  public ContentTypeAggregate() : base()
  {
  }

  public ContentTypeAggregate(IdentifierUnit uniqueName, bool isInvariant = false, ActorId actorId = default, ContentTypeId? id = null)
    : base((id ?? ContentTypeId.NewId()).AggregateId)
  {
    Raise(new ContentTypeCreatedEvent(isInvariant, uniqueName), actorId);
  }
  protected virtual void Apply(ContentTypeCreatedEvent @event)
  {
    IsInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ContentTypeDeletedEvent(), actorId);
    }
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
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

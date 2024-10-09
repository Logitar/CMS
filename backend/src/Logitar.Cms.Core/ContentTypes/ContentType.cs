using Logitar.Cms.Contracts;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes;

public class ContentType : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

  public new ContentTypeId Id => new(base.Id);

  public bool IsInvariant { get; private set; }

  private Identifier? _uniqueName = null;
  public Identifier UniqueName
  {
    get => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
    set
    {
      if (_uniqueName != value)
      {
        _uniqueName = value;
        _updatedEvent.UniqueName = value;
      }
    }
  }
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updatedEvent.Description = new Change<Description>(value);
      }
    }
  }

  public ContentType() : base()
  {
  }

  public ContentType(Identifier uniqueName, bool isInvariant = false, ActorId actorId = default, ContentTypeId? id = null)
    : base((id ?? ContentTypeId.NewId()).AggregateId)
  {
    Raise(new CreatedEvent(isInvariant, uniqueName), actorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    IsInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(UpdatedEvent @event)
  {
    if (@event.UniqueName != null)
    {
      _uniqueName = @event.UniqueName;
    }
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

  public class CreatedEvent : DomainEvent, INotification
  {
    public bool IsInvariant { get; }

    public Identifier UniqueName { get; }

    public CreatedEvent(bool isInvariant, Identifier uniqueName)
    {
      IsInvariant = isInvariant;

      UniqueName = uniqueName;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Identifier? UniqueName { get; set; }
    public Change<DisplayName>? DisplayName { get; set; }
    public Change<Description>? Description { get; set; }

    public bool HasChanges => UniqueName != null || DisplayName != null || Description != null;
  }
}

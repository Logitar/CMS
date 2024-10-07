using Logitar.Cms.Contracts;
using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.EventSourcing;
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

  private readonly Dictionary<Guid, FieldDefinitionUnit> _fieldDefinitionByIds = [];
  public IReadOnlyDictionary<Guid, FieldDefinitionUnit> FieldDefinitions => _fieldDefinitionByIds.AsReadOnly();
  private readonly Dictionary<string, Guid> _fieldIdByUniqueNames = [];
  private readonly List<Guid> _orderedFieldIds = [];
  public IReadOnlyCollection<FieldDefinitionUnit> OrderedFieldDefinitions
  {
    get
    {
      List<FieldDefinitionUnit> fieldDefinitions = new(capacity: _orderedFieldIds.Count);
      foreach (Guid id in _orderedFieldIds)
      {
        fieldDefinitions.Add(_fieldDefinitionByIds[id]);
      }
      return fieldDefinitions.AsReadOnly();
    }
  }
  public FieldDefinitionUnit GetFieldDefinition(Guid id) => TryGetFieldDefinition(id) ?? throw new InvalidOperationException($"The field definition 'Id={id}' could not be found.");
  public FieldDefinitionUnit GetFieldDefinition(IdentifierUnit uniqueName) => TryGetFieldDefinition(uniqueName) ?? throw new InvalidOperationException($"The field definition 'UniqueName={uniqueName}' could not be found.");
  public FieldDefinitionUnit? TryGetFieldDefinition(Guid id) => _fieldDefinitionByIds.TryGetValue(id, out FieldDefinitionUnit? value) ? value : null;
  public FieldDefinitionUnit? TryGetFieldDefinition(IdentifierUnit uniqueName) => _fieldIdByUniqueNames.TryGetValue(Normalize(uniqueName), out Guid id) ? _fieldDefinitionByIds[id] : null;

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

  public Guid AddFieldDefinition(FieldDefinitionUnit fieldDefinition, ActorId actorId = default)
  {
    Guid id = Guid.NewGuid();
    SetFieldDefinition(id, fieldDefinition, actorId);
    return id;
  }
  public void SetFieldDefinition(Guid id, FieldDefinitionUnit fieldDefinition, ActorId actorId = default)
  {
    if (_fieldIdByUniqueNames.TryGetValue(Normalize(fieldDefinition.UniqueName), out Guid existingId) && existingId != id)
    {
      throw new CmsUniqueNameAlreadyUsedException<FieldDefinitionUnit>(fieldDefinition.UniqueName, nameof(fieldDefinition.UniqueName));
    }

    FieldDefinitionUnit? existingFieldDefinition = TryGetFieldDefinition(id);
    if (existingFieldDefinition == null)
    {
      int order = _orderedFieldIds.Count;
      Raise(new FieldDefinitionChangedEvent(id, fieldDefinition, order), actorId);
    }
    else if (fieldDefinition != existingFieldDefinition)
    {
      Raise(new FieldDefinitionChangedEvent(id, fieldDefinition, order: null), actorId);
    }
  }
  protected virtual void Apply(FieldDefinitionChangedEvent @event)
  {
    if (_fieldDefinitionByIds.TryGetValue(@event.FieldId, out FieldDefinitionUnit? fieldDefinition))
    {
      _fieldIdByUniqueNames.Remove(Normalize(fieldDefinition.UniqueName));
    }

    _fieldDefinitionByIds[@event.FieldId] = @event.FieldDefinition;
    _fieldIdByUniqueNames[Normalize(@event.FieldDefinition.UniqueName)] = @event.FieldId;

    if (@event.Order.HasValue)
    {
      if (@event.Order.Value == _orderedFieldIds.Count)
      {
        _orderedFieldIds.Add(@event.FieldId);
      }
      else
      {
        _orderedFieldIds[@event.Order.Value] = @event.FieldId;
      }
    }
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

  private static string Normalize(IdentifierUnit identifier) => identifier.Value.ToLowerInvariant();
}

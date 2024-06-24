using Logitar.Cms.Core.ContentTypes.Events;
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
  public IdentifierUnit UniqueName
  {
    get => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
    set
    {
      if (value != _uniqueName)
      {
        _uniqueName = value;
        _updatedEvent.UniqueName = value;
      }
    }
  }
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

  private readonly Dictionary<Guid, FieldDefinitionUnit> _fieldDefinitionByIds = [];
  public IReadOnlyDictionary<Guid, FieldDefinitionUnit> FieldDefinitionByIds => _fieldDefinitionByIds.AsReadOnly();
  private readonly Dictionary<string, Guid> _fieldIdByUniqueNames = [];
  private readonly List<Guid> _orderedFieldIds = [];
  public IReadOnlyCollection<FieldDefinitionUnit> FieldDefinitions
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
  public FieldDefinitionUnit? TryGetFieldDefinition(Guid id) => _fieldDefinitionByIds.TryGetValue(id, out FieldDefinitionUnit? value) ? value : null;
  public FieldDefinitionUnit? TryGetFieldDefinition(IdentifierUnit uniqueName)
    => _fieldIdByUniqueNames.TryGetValue(Normalize(uniqueName), out Guid id) ? _fieldDefinitionByIds[id] : null;
  public FieldDefinitionUnit GetFieldDefinition(Guid id) => TryGetFieldDefinition(id) ?? throw new InvalidOperationException($"The field definition 'Id={id}' could not be found.");
  public FieldDefinitionUnit GetFieldDefinition(IdentifierUnit uniqueName) => TryGetFieldDefinition(uniqueName) ?? throw new InvalidOperationException($"The field definition 'UniqueName={uniqueName}' could not be found.");

  public ContentTypeAggregate() : base()
  {
  }

  public ContentTypeAggregate(IdentifierUnit identifier, bool isInvariant = false, ActorId actorId = default, ContentTypeId? id = null)
    : base((id ?? ContentTypeId.NewId()).AggregateId)
  {
    Raise(new ContentTypeCreatedEvent(isInvariant, identifier), actorId);
  }
  protected virtual void Apply(ContentTypeCreatedEvent @event)
  {
    IsInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void AddFieldDefinition(FieldDefinitionUnit fieldDefinition, ActorId actorId = default) => SetFieldDefinition(Guid.NewGuid(), fieldDefinition, actorId);
  public void SetFieldDefinition(Guid id, FieldDefinitionUnit fieldDefinition, ActorId actorId = default)
  {
    if (_fieldIdByUniqueNames.TryGetValue(Normalize(fieldDefinition.UniqueName), out Guid existingId) && existingId != id)
    {
      throw new UniqueNameAlreadyUsedException<FieldDefinitionUnit>(fieldDefinition.UniqueName, nameof(fieldDefinition.UniqueName));
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

  private static string Normalize(IdentifierUnit identifier) => identifier.Value.ToLower();
}

using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes;

public class FieldTypeAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new ReadOnlyUniqueNameSettings();

  private FieldTypeUpdatedEvent _updatedEvent = new();

  public new FieldTypeId Id => new(base.Id);

  private UniqueNameUnit? _uniqueName = null;
  public UniqueNameUnit UniqueName => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
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

  public DataType DataType { get; private set; }
  private readonly FieldTypeProperties? _properties = null;
  public FieldTypeProperties Properties => _properties ?? throw new InvalidOperationException($"The {nameof(Properties)} have not been initialized yet.");

  public FieldTypeAggregate() : base()
  {
  }

  public FieldTypeAggregate(UniqueNameUnit uniqueName, FieldTypeProperties properties, ActorId actorId = default, FieldTypeId? id = null)
    : base((id ?? FieldTypeId.NewId()).AggregateId)
  {
    DataType dataType = properties.DataType;
    Raise(new FieldTypeCreatedEvent(uniqueName, dataType), actorId);

    switch (dataType)
    {
      case DataType.String:
        SetProperties((ReadOnlyStringProperties)properties, actorId);
        break;
      default:
        throw new DataTypeNotSupportedException(dataType);
    }
  }
  protected virtual void Apply(FieldTypeCreatedEvent @event)
  {
    _uniqueName = @event.UniqueName;

    DataType = @event.DataType;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new FieldTypeDeletedEvent(), actorId);
    }
  }

  public void SetProperties(ReadOnlyStringProperties properties, ActorId actorId = default)
  {
    if (DataType != properties.DataType)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (_properties != properties)
    {
      Raise(new StringPropertiesChangedEvent(properties), actorId);
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
  protected virtual void Apply(FieldTypeUpdatedEvent @event)
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

using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Fields;

public class FieldTypeAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings();

  private FieldTypeUpdatedEvent _updatedEvent = new();

  public new FieldTypeId Id => new(base.Id);

  private UniqueNameUnit? _uniqueName = null;
  public UniqueNameUnit UniqueName
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

  public DataType DataType { get; private set; }
  private FieldTypeProperties? _properties = null;
  public FieldTypeProperties Properties => _properties ?? throw new InvalidOperationException($"The {nameof(Properties)} has not been initialized yet.");

  public FieldTypeAggregate(UniqueNameUnit uniqueName, FieldTypeProperties properties, ActorId actorId = default, FieldTypeId? id = null)
    : base((id ?? FieldTypeId.NewId()).AggregateId)
  {
    DataType dataType = properties.DataType;
    Raise(new FieldTypeCreatedEvent(uniqueName, dataType), actorId);

    switch (dataType)
    {
      case DataType.Boolean:
        SetProperties((ReadOnlyBooleanProperties)properties, actorId);
        break;
      case DataType.DateTime:
        SetProperties((ReadOnlyDateTimeProperties)properties, actorId);
        break;
      case DataType.Number:
        SetProperties((ReadOnlyNumberProperties)properties, actorId);
        break;
      case DataType.String:
        SetProperties((ReadOnlyStringProperties)properties, actorId);
        break;
      case DataType.Text:
        SetProperties((ReadOnlyTextProperties)properties, actorId);
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

  public void SetProperties(ReadOnlyBooleanProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.Boolean)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new BooleanFieldTypePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(BooleanFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ReadOnlyDateTimeProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.DateTime)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new DateTimeFieldTypePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(DateTimeFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ReadOnlyNumberProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.Number)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new NumberFieldTypePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(NumberFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ReadOnlyStringProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.String)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new StringFieldTypePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(StringFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ReadOnlyTextProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.Text)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new TextFieldTypePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(TextFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
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
}

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
  public UniqueNameUnit UniqueName => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
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
  public FieldTypeProperties Properties => _properties ?? throw new InvalidOperationException($"The {nameof(Properties)} have not been initialized yet.");

  public FieldTypeAggregate(AggregateId id) : base(id)
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
        SetProperties((ReadOnlyStringProperties)properties);
        break;
      case DataType.Text:
        SetProperties((ReadOnlyTextProperties)properties);
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

  public void SetProperties(ReadOnlyStringProperties properties)
  {
    if (DataType != DataType.String)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new StringFieldTypePropertiesChangedEvent(properties));
    }
  }
  protected virtual void Apply(StringFieldTypePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(ReadOnlyTextProperties properties)
  {
    if (DataType != DataType.Text)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (properties != _properties)
    {
      Raise(new TextFieldTypePropertiesChangedEvent(properties));
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

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}

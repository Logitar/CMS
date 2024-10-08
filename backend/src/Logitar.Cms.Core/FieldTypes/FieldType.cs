using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes;

public class FieldType : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_");

  private UpdatedEvent _updatedEvent = new();

  public new FieldTypeId Id => new(base.Id);

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException($"The {nameof(UniqueName)} has not been initialized yet.");
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

  public DataType DataType { get; private set; }
  private BaseProperties? _properties = null;
  public BaseProperties Properties => _properties ?? throw new InvalidOperationException($"The {nameof(Properties)} has not been initialized yet.");

  public FieldType() : base()
  {
  }

  public FieldType(string uniqueNameValue, BaseProperties properties, ActorId actorId = default, FieldTypeId? id = null)
    : base((id ?? FieldTypeId.NewId()).AggregateId)
  {
    UniqueName uniqueName = new(UniqueNameSettings, uniqueNameValue);
    Raise(new CreatedEvent(uniqueName, properties.DataType), actorId);

    switch (DataType)
    {
      case DataType.String:
        SetProperties((StringProperties)properties);
        break;
      case DataType.Text:
        SetProperties((TextProperties)properties);
        break;
      default:
        throw new DataTypeNotSupportedException(DataType);
    }
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    _uniqueName = @event.UniqueName;

    DataType = @event.DataType;
  }

  public void SetProperties(StringProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.String)
    {
      throw new DataTypeMismatchException(this, DataType.String);
    }

    if (_properties != properties)
    {
      Raise(new StringPropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(StringPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(TextProperties properties, ActorId actorId = default)
  {
    if (DataType != DataType.Text)
    {
      throw new DataTypeMismatchException(this, DataType.Text);
    }

    if (_properties != properties)
    {
      Raise(new TextPropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(TextPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
  }

  public void SetUniqueName(string value)
  {
    UniqueName uniqueName = new(UniqueNameSettings, value);
    if (_uniqueName != uniqueName)
    {
      _uniqueName = uniqueName;
      _updatedEvent.UniqueName = uniqueName;
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
    public UniqueName UniqueName { get; }

    public DataType DataType { get; }

    public CreatedEvent(UniqueName uniqueName, DataType dataType)
    {
      UniqueName = uniqueName;

      DataType = dataType;
    }
  }

  public class StringPropertiesChangedEvent : DomainEvent, INotification
  {
    public StringProperties Properties { get; }

    public StringPropertiesChangedEvent(StringProperties properties)
    {
      Properties = properties;
    }
  }

  public class TextPropertiesChangedEvent : DomainEvent, INotification
  {
    public TextProperties Properties { get; }

    public TextPropertiesChangedEvent(TextProperties properties)
    {
      Properties = properties;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public UniqueName? UniqueName { get; set; }
    public Change<DisplayName>? DisplayName { get; set; }
    public Change<Description>? Description { get; set; }

    public bool HasChanges => UniqueName != null || DisplayName != null || Description != null;
  }
}

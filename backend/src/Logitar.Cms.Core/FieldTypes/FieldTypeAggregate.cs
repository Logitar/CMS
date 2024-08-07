﻿using FluentValidation.Results;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes;

public class FieldTypeAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new ReadOnlyUniqueNameSettings();

  private FieldTypeUpdatedEvent _updatedEvent = new();

  private BooleanFieldValueValidator _booleanValueValidator = new(new ReadOnlyBooleanProperties());
  private DateTimeFieldValueValidator _dateTimeValueValidator = new(new ReadOnlyDateTimeProperties());
  private NumberFieldValueValidator _numberValueValidator = new(new ReadOnlyNumberProperties());
  private StringFieldValueValidator _stringValueValidator = new(new ReadOnlyStringProperties());
  private TextFieldValueValidator _textValueValidator = new(new ReadOnlyTextProperties());

  public new FieldTypeId Id => new(base.Id);

  private UniqueNameUnit? _uniqueName = null;
  public UniqueNameUnit UniqueName
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
  private FieldTypeProperties? _properties = null;
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

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new FieldTypeDeletedEvent(), actorId);
    }
  }

  public void SetProperties(ReadOnlyBooleanProperties properties, ActorId actorId = default)
  {
    if (DataType != properties.DataType)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (_properties != properties)
    {
      Raise(new BooleanPropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(BooleanPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
    _booleanValueValidator = new(@event.Properties);
  }

  public void SetProperties(ReadOnlyDateTimeProperties properties, ActorId actorId = default)
  {
    if (DataType != properties.DataType)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (_properties != properties)
    {
      Raise(new DateTimePropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(DateTimePropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
    _dateTimeValueValidator = new(@event.Properties);
  }

  public void SetProperties(ReadOnlyNumberProperties properties, ActorId actorId = default)
  {
    if (DataType != properties.DataType)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (_properties != properties)
    {
      Raise(new NumberPropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(NumberPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
    _numberValueValidator = new(@event.Properties);
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
  protected virtual void Apply(StringPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
    _stringValueValidator = new(@event.Properties);
  }

  public void SetProperties(ReadOnlyTextProperties properties, ActorId actorId = default)
  {
    if (DataType != properties.DataType)
    {
      throw new DataTypeMismatchException(this, properties.DataType);
    }
    else if (_properties != properties)
    {
      Raise(new TextPropertiesChangedEvent(properties), actorId);
    }
  }
  protected virtual void Apply(TextPropertiesChangedEvent @event)
  {
    _properties = @event.Properties;
    _textValueValidator = new(@event.Properties);
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

  public ValidationResult Validate(string value, string propertyName) => DataType switch
  {
    DataType.Boolean => ValidateBoolean(value, propertyName),
    DataType.DateTime => ValidateDateTime(value, propertyName),
    DataType.Number => ValidateNumber(value, propertyName),
    DataType.String => ValidateString(value, propertyName),
    DataType.Text => ValidateText(value, propertyName),
    _ => throw new DataTypeNotSupportedException(DataType),
  };
  private ValidationResult ValidateBoolean(string value, string propertyName)
  {
    if (bool.TryParse(value, out bool boolean))
    {
      return _booleanValueValidator.Validate(boolean); // ISSUE: https://github.com/Logitar/CMS/issues/3
    }

    ValidationFailure error = new(propertyName, "The value is not a valid boolean.", value)
    {
      ErrorCode = "BooleanParsing"
    };
    return new ValidationResult([error]);
  }
  private ValidationResult ValidateDateTime(string value, string propertyName)
  {
    if (DateTime.TryParse(value, out DateTime dateTime))
    {
      return _dateTimeValueValidator.Validate(dateTime); // ISSUE: https://github.com/Logitar/CMS/issues/3
    }

    ValidationFailure error = new(propertyName, "The value is not a valid DateTime.", value)
    {
      ErrorCode = "DateTimeParsing"
    };
    return new ValidationResult([error]);
  }
  private ValidationResult ValidateNumber(string value, string propertyName)
  {
    if (double.TryParse(value, out double number))
    {
      return _numberValueValidator.Validate(number); // ISSUE: https://github.com/Logitar/CMS/issues/3
    }

    ValidationFailure error = new(propertyName, "The value is not a valid number.", value)
    {
      ErrorCode = "NumberParsing"
    };
    return new ValidationResult([error]);
  }
  private ValidationResult ValidateString(string value, string propertyName) => _stringValueValidator.Validate(value); // ISSUE: https://github.com/Logitar/CMS/issues/3
  private ValidationResult ValidateText(string value, string propertyName) => _textValueValidator.Validate(value); // ISSUE: https://github.com/Logitar/CMS/issues/3

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}

using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;
using Logitar.Cms.Core.Fields.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class FieldTypeEntity : AggregateEntity
{
  public int FieldTypeId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public DataType DataType { get; private set; }
  public Dictionary<string, string> Properties { get; private set; } = [];
  public string? PropertiesSerialized
  {
    get => Properties.Count == 0 ? null : JsonSerializer.Serialize(Properties);
    private set
    {
      if (value == null)
      {
        Properties.Clear();
      }
      else
      {
        Properties = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];

  public FieldTypeEntity(FieldTypeCreatedEvent @event) : base(@event)
  {
    UniqueName = @event.UniqueName.Value;
    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public void SetProperties(BooleanFieldTypePropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();
  }
  public void SetProperties(DateTimeFieldTypePropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    SetProperty(nameof(IDateTimeProperties.MinimumValue), @event.Properties.MinimumValue?.ToString("O"));
    SetProperty(nameof(IDateTimeProperties.MaximumValue), @event.Properties.MaximumValue?.ToString("O"));
  }
  public void SetProperties(NumberFieldTypePropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    SetProperty(nameof(INumberProperties.MinimumValue), @event.Properties.MinimumValue?.ToString());
    SetProperty(nameof(INumberProperties.MaximumValue), @event.Properties.MaximumValue?.ToString());
    SetProperty(nameof(INumberProperties.Step), @event.Properties.Step?.ToString());
  }
  public void SetProperties(StringFieldTypePropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    SetProperty(nameof(IStringProperties.MinimumLength), @event.Properties.MinimumLength?.ToString());
    SetProperty(nameof(IStringProperties.MaximumLength), @event.Properties.MaximumLength?.ToString());
    SetProperty(nameof(IStringProperties.Pattern), @event.Properties.Pattern);
  }
  public void SetProperties(TextFieldTypePropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    SetProperty(nameof(ITextProperties.ContentType), @event.Properties.ContentType);
    SetProperty(nameof(ITextProperties.MinimumLength), @event.Properties.MinimumLength?.ToString());
    SetProperty(nameof(ITextProperties.MaximumLength), @event.Properties.MaximumLength?.ToString());
  }

  private void SetProperty(string key, string? value)
  {
    if (value == null)
    {
      Properties.Remove(key);
    }
    else
    {
      Properties[key] = value;
    }
  }

  public void Update(FieldTypeUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.UniqueName != null)
    {
      UniqueName = @event.UniqueName.Value;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}

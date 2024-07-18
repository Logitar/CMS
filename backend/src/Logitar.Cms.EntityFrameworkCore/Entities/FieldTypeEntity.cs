using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class FieldTypeEntity : AggregateEntity
{
  public int FieldTypeId { get; private set; }
  public Guid UniqueId { get; private set; }

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
      Properties.Clear();

      if (value != null)
      {
        Dictionary<string, string>? properties = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
        if (properties != null)
        {
          foreach (KeyValuePair<string, string> property in properties)
          {
            Properties[property.Key] = property.Value;
          }
        }
      }
    }
  }

  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];

  public FieldTypeEntity(FieldTypeCreatedEvent @event) : base(@event)
  {
    UniqueId = @event.AggregateId.ToGuid();

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public void SetProperties(StringPropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    if (@event.Properties.MinimumLength.HasValue)
    {
      Properties[nameof(IStringProperties.MinimumLength)] = @event.Properties.MinimumLength.Value.ToString();
    }
    if (@event.Properties.MaximumLength.HasValue)
    {
      Properties[nameof(IStringProperties.MaximumLength)] = @event.Properties.MaximumLength.Value.ToString();
    }
    if (@event.Properties.Pattern != null)
    {
      Properties[nameof(IStringProperties.Pattern)] = @event.Properties.Pattern;
    }
  }
  public void SetProperties(TextPropertiesChangedEvent @event)
  {
    Update(@event);

    Properties.Clear();

    Properties[nameof(ITextProperties.ContentType)] = @event.Properties.ContentType;
    if (@event.Properties.MinimumLength.HasValue)
    {
      Properties[nameof(ITextProperties.MinimumLength)] = @event.Properties.MinimumLength.Value.ToString();
    }
    if (@event.Properties.MaximumLength.HasValue)
    {
      Properties[nameof(ITextProperties.MaximumLength)] = @event.Properties.MaximumLength.Value.ToString();
    }
  }

  public void Update(FieldTypeUpdatedEvent @event)
  {
    base.Update(@event);

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

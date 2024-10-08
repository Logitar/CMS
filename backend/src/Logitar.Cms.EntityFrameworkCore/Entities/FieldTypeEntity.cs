using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class FieldTypeEntity : AggregateEntity
{
  public int FieldTypeId { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public DataType DataType { get; private set; }
  public string? Properties { get; private set; }

  public FieldTypeEntity(FieldType.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public StringPropertiesModel GetStringProperties() => (Properties == null ? null : JsonSerializer.Deserialize<StringPropertiesModel>(Properties)) ?? new();
  public TextPropertiesModel GetTextProperties() => (Properties == null ? null : JsonSerializer.Deserialize<TextPropertiesModel>(Properties)) ?? new();

  public void SetProperties(FieldType.StringPropertiesChangedEvent @event)
  {
    Properties = JsonSerializer.Serialize(@event.Properties);
  }
  public void SetProperties(FieldType.TextPropertiesChangedEvent @event)
  {
    Properties = JsonSerializer.Serialize(@event.Properties);
  }

  public void Update(FieldType.UpdatedEvent @event)
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

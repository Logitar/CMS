using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

public class FieldTypeEntity : AggregateEntity
{
  public int FieldTypeId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public DataType DataType { get; private set; }
  public string? Settings { get; private set; }

  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];
  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public FieldTypeEntity(FieldTypeCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public void SetSettings(FieldTypeBooleanSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeDateTimeSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeNumberSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeRichTextSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeSelectSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeStringSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  public void SetSettings(FieldTypeTagsSettingsChanged @event)
  {
    Update(@event);

    SetSettings(@event.Settings);
  }
  private void SetSettings(FieldTypeSettings settings)
  {
    Settings = JsonSerializer.Serialize(settings, settings.GetType());
  }

  public void SetUniqueName(FieldTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(FieldTypeUpdated @event)
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

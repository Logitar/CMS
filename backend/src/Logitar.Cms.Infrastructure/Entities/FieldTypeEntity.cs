using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Events;
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

  public FieldTypeEntity(FieldTypeCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public void SetSettings(FieldTypeBooleanSettingsChanged e)
  {
    Update(e);

    // TODO(fpion): implement
  }
  public void SetSettings(FieldTypeDateTimeSettingsChanged e)
  {
    Update(e);

    // TODO(fpion): implement
  }
  public void SetSettings(FieldTypeNumberSettingsChanged e)
  {
    Update(e);

    // TODO(fpion): implement
  }
  public void SetSettings(FieldTypeRichTextSettingsChanged e)
  {
    Update(e);

    // TODO(fpion): implement
  }
  public void SetSettings(FieldTypeStringSettingsChanged e)
  {
    Update(e);

    // TODO(fpion): implement
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

using Logitar.Cms.Core;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class FieldDefinitionEntity
{
  public int FieldDefinitionId { get; private set; }
  public Guid UniqueId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public int Order { get; private set; }

  public FieldTypeEntity? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }

  public bool IsInvariant { get; private set; }
  public bool IsRequired { get; private set; }
  public bool IsIndexed { get; private set; }
  public bool IsUnique { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }
  public string? Placeholder { get; private set; }

  public string CreatedBy { get; set; } = string.Empty;
  public DateTime CreatedOn { get; set; }
  public string UpdatedBy { get; set; } = string.Empty;
  public DateTime UpdatedOn { get; set; }

  public FieldDefinitionEntity(ContentTypeEntity contentType, FieldTypeEntity fieldType, FieldDefinitionChangedEvent @event)
  {
    UniqueId = @event.FieldId;

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private FieldDefinitionEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = [new(CreatedBy), new(UpdatedBy)];

    if (FieldType != null)
    {
      actorIds.AddRange(FieldType.GetActorIds());
    }

    return actorIds;
  }

  public void Update(FieldDefinitionChangedEvent @event)
  {
    if (@event.Order.HasValue)
    {
      Order = @event.Order.Value;
    }

    SetFieldDefinition(@event.FieldDefinition);

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
  private void SetFieldDefinition(FieldDefinitionUnit fieldDefinition)
  {
    IsInvariant = fieldDefinition.IsInvariant;
    IsRequired = fieldDefinition.IsRequired;
    IsIndexed = fieldDefinition.IsIndexed;
    IsUnique = fieldDefinition.IsUnique;

    UniqueName = fieldDefinition.UniqueName.Value;
    DisplayName = fieldDefinition.DisplayName?.Value;
    Description = fieldDefinition.Description?.Value;
    Placeholder = fieldDefinition.Placeholder?.Value;
  }
}

using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentTypeEntity : AggregateEntity
{
  public int ContentTypeId { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public List<ContentItemEntity> ContentItems { get; private set; } = [];
  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];

  public ContentTypeEntity(ContentTypeCreatedEvent @event) : base(@event)
  {
    IsInvariant = @event.IsInvariant;
    UniqueName = @event.UniqueName.Value;
  }

  private ContentTypeEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = [];
    actorIds.AddRange(base.GetActorIds());

    foreach (FieldDefinitionEntity fieldDefinition in FieldDefinitions)
    {
      actorIds.AddRange(fieldDefinition.GetActorIds());
    }

    return actorIds.AsReadOnly();
  }

  public void SetFieldDefinition(FieldTypeEntity fieldType, FieldDefinitionChangedEvent @event)
  {
    Update(@event);

    FieldDefinitionEntity? fieldDefinition = FieldDefinitions.SingleOrDefault(field => field.Id == @event.FieldId);
    if (fieldDefinition == null)
    {
      fieldDefinition = new(this, fieldType, @event);
      FieldDefinitions.Add(fieldDefinition);
    }
    else
    {
      fieldDefinition.Update(@event);
    }
  }

  public void Update(ContentTypeUpdatedEvent @event)
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

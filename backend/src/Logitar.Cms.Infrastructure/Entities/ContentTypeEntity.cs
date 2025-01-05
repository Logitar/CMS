using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

public class ContentTypeEntity : AggregateEntity
{
  public int ContentTypeId { get; private set; }
  public Guid Id { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int FieldCount { get; private set; }

  public List<ContentLocaleEntity> ContentLocales { get; private set; } = [];
  public List<ContentEntity> Contents { get; private set; } = [];
  public List<FieldDefinitionEntity> Fields { get; private set; } = [];

  public ContentTypeEntity(ContentTypeCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ContentTypeEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = [.. base.GetActorIds()];
    foreach (FieldDefinitionEntity field in Fields)
    {
      if (field.FieldType != null)
      {
        actorIds.AddRange(field.FieldType.GetActorIds());
      }
    }
    return actorIds.AsReadOnly();
  }

  public void SetField(FieldTypeEntity fieldType, ContentTypeFieldDefinitionChanged @event)
  {
    Update(@event);

    FieldDefinitionEntity? fieldDefinition = Fields.SingleOrDefault(x => x.Id == @event.FieldId);
    if (fieldDefinition == null)
    {
      fieldDefinition = new(this, fieldType, order: FieldCount, @event);
      Fields.Add(fieldDefinition);
      FieldCount = Fields.Count;
    }
    else
    {
      fieldDefinition.Update(@event);
    }
  }

  public void SetUniqueName(ContentTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(ContentTypeUpdated @event)
  {
    base.Update(@event);

    if (@event.IsInvariant.HasValue)
    {
      IsInvariant = @event.IsInvariant.Value;
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

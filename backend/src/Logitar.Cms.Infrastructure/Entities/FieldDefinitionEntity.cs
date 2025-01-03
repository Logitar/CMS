using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Fields;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

public class FieldDefinitionEntity
{
  public int FieldDefinitionId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid Id { get; private set; }
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
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }
  public string? Placeholder { get; private set; }

  public FieldDefinitionEntity(ContentTypeEntity contentType, FieldTypeEntity fieldType, ContentTypeFieldDefinitionChanged @event)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    Id = @event.FieldId;
    // TODO(fpion): Order

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;

    Update(@event);
  }

  private FieldDefinitionEntity()
  {
  }

  public void Update(ContentTypeFieldDefinitionChanged @event)
  {
    FieldDefinition field = @event.FieldDefinition;

    IsInvariant = field.IsInvariant;
    IsRequired = field.IsRequired;
    IsIndexed = field.IsIndexed;
    IsUnique = field.IsUnique;

    UniqueName = field.UniqueName.Value;
    DisplayName = field.DisplayName?.Value;
    Description = field.Description?.Value;
    Placeholder = field.Placeholder?.Value;
  }
}

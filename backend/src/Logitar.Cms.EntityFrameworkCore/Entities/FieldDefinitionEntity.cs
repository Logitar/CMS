using Logitar.Cms.Contracts.Fields;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class FieldDefinitionEntity
{
  public int FieldDefinitionId { get; private set; }

  public Guid Id { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;
  public int Order { get; private set; }

  public FieldTypeEntity? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }
  public string FieldTypeName { get; private set; } = string.Empty;
  public DataType FieldDataType { get; private set; }

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

  public FieldDefinitionEntity(ContentTypeEntity contentType, FieldTypeEntity fieldType)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeName = contentType.UniqueName;

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeName = fieldType.UniqueName;
    FieldDataType = fieldType.DataType;
  }

  private FieldDefinitionEntity()
  {
  }
}

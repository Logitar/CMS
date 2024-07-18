namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal abstract class FieldIndexEntity<T>
{
  public int FieldIndexId { get; private set; }

  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;

  public int FieldTypeId { get; private set; }
  public Guid FieldTypeUid { get; private set; }
  public string FieldTypeName { get; private set; } = string.Empty;

  public int FieldDefinitionId { get; private set; }
  public Guid FieldDefinitionUid { get; private set; }
  public string FieldDefinitionName { get; private set; } = string.Empty;

  public int ContentItemId { get; private set; }
  public Guid ContentItemUid { get; private set; }

  public int ContentLocaleId { get; private set; }
  public Guid ContentLocaleUid { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }
  public string? LanguageCode { get; private set; }

  public T Value { get; set; } = default!;

  protected FieldIndexEntity()
  {
  }

  protected FieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, T value)
  {
    if (contentLocale.LanguageId != null && contentLocale.Language == null)
    {
      throw new ArgumentException($"The {nameof(contentLocale.Language)} is required.", nameof(contentLocale));
    }

    ContentItemEntity contentItem = contentLocale.Item ?? throw new ArgumentException($"The {nameof(contentLocale.Item)} is required.", nameof(contentLocale));
    ContentTypeEntity contentType = contentItem.ContentType ?? throw new ArgumentException($"The {nameof(contentItem.ContentType)} is required.", nameof(contentLocale));
    FieldTypeEntity fieldType = fieldDefinition.FieldType ?? throw new ArgumentException($"The {nameof(fieldDefinition.FieldType)} is required.", nameof(fieldDefinition));
    LanguageEntity? language = contentLocale.Language;

    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.UniqueId;
    ContentTypeName = contentType.UniqueName;

    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeUid = fieldType.UniqueId;
    FieldTypeName = fieldType.UniqueName;

    FieldDefinitionId = fieldDefinition.FieldDefinitionId;
    FieldDefinitionUid = fieldDefinition.UniqueId;
    FieldDefinitionName = fieldDefinition.UniqueName;

    ContentItemId = contentItem.ContentItemId;
    ContentItemUid = contentItem.UniqueId;

    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleUid = contentLocale.UniqueId;
    ContentLocaleName = contentLocale.UniqueName;

    if (language != null)
    {
      LanguageId = language.LanguageId;
      LanguageUid = language.UniqueId;
      LanguageCode = language.Code;
    }

    Value = value;
  }
}

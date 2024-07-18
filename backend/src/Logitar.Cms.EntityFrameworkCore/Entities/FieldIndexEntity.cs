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

  public T Value { get; private set; } = default!;

  protected FieldIndexEntity()
  {
  }

  protected FieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, T value)
  {
    if (contentLocale.Item == null)
    {
      throw new ArgumentException($"The {nameof(contentLocale.Item)} is required.", nameof(contentLocale));
    }
    if (contentLocale.LanguageId != null && contentLocale.Language == null)
    {
      throw new ArgumentException($"The {nameof(contentLocale.Language)} is required.", nameof(contentLocale));
    }

    if (fieldDefinition.ContentType == null)
    {
      throw new ArgumentException($"The {nameof(fieldDefinition.ContentType)} is required.", nameof(fieldDefinition));
    }
    if (fieldDefinition.FieldType == null)
    {
      throw new ArgumentException($"The {nameof(fieldDefinition.FieldType)} is required.", nameof(fieldDefinition));
    }

    ContentTypeId = fieldDefinition.ContentType.ContentTypeId;
    ContentTypeUid = fieldDefinition.ContentType.UniqueId;
    ContentTypeName = fieldDefinition.ContentType.UniqueName;

    FieldTypeId = fieldDefinition.FieldType.FieldTypeId;
    FieldTypeUid = fieldDefinition.FieldType.UniqueId;
    FieldTypeName = fieldDefinition.FieldType.UniqueName;

    FieldDefinitionId = fieldDefinition.FieldDefinitionId;
    FieldDefinitionUid = fieldDefinition.UniqueId;
    FieldDefinitionName = fieldDefinition.UniqueName;

    ContentItemId = contentLocale.Item.ContentItemId;
    ContentItemUid = contentLocale.Item.UniqueId;

    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleUid = contentLocale.UniqueId;
    ContentLocaleName = contentLocale.UniqueName;

    LanguageId = contentLocale.Language?.LanguageId;
    LanguageUid = contentLocale.Language?.UniqueId;
    LanguageCode = contentLocale.Language?.Code;

    Value = value;
  }
}

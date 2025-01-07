using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

// TODO(fpion): insert values
// TODO(fpion): remove values
// TODO(fpion): update values
// TODO(fpion): update index on ContentType Name change
// TODO(fpion): update index on Language Code change
// TODO(fpion): update index on FieldType Name change
// TODO(fpion): update index on FieldDefinition Name change
// TODO(fpion): update index on ContentLocale Name change

public class UniqueIndexEntity
{
  public const char KeySeparator = '|';
  public const int MaximumLength = byte.MaxValue;

  public int UniqueIndexId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }
  public string? LanguageCode { get; private set; }

  public FieldTypeEntity? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }
  public Guid FieldTypeUid { get; private set; }
  public string FieldTypeName { get; private set; } = string.Empty;

  public FieldDefinitionEntity? FieldDefinition { get; private set; }
  public int FieldDefinitionId { get; private set; }
  public Guid FieldDefinitionUid { get; private set; }
  public string FieldDefinitionName { get; private set; } = string.Empty;

  public ContentStatus Status { get; private set; }

  public string Value { get; private set; } = string.Empty;
  public string ValueNormalized
  {
    get => Helper.Normalize(Value);
    private set { }
  }

  public string Key
  {
    get => CreateKey(FieldDefinitionUid, ValueNormalized);
    private set { }
  }

  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentLocaleEntity? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public UniqueIndexEntity(
    ContentTypeEntity contentType,
    LanguageEntity? language,
    FieldTypeEntity fieldType,
    FieldDefinitionEntity fieldDefinition,
    ContentEntity content,
    ContentLocaleEntity contentLocale,
    ContentStatus status,
    string value)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;
    ContentTypeName = contentType.UniqueNameNormalized;

    if (language != null)
    {
      Language = language;
      LanguageId = language.LanguageId;
      LanguageUid = language.Id;
      LanguageCode = language.CodeNormalized;
    }

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeUid = fieldType.Id;
    FieldTypeName = fieldType.UniqueNameNormalized;

    FieldDefinition = fieldDefinition;
    FieldDefinitionId = fieldDefinition.FieldDefinitionId;
    FieldDefinitionUid = fieldDefinition.Id;
    FieldDefinitionName = fieldDefinition.UniqueNameNormalized;

    Status = status;

    Value = value.Truncate(MaximumLength);

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleName = contentLocale.UniqueNameNormalized;
  }

  private UniqueIndexEntity()
  {
  }

  public static string CreateKey(KeyValuePair<Guid, string> fieldValue) => CreateKey(fieldValue.Key, fieldValue.Value);
  public static string CreateKey(Guid fieldDefinitionId, string value) => string.Join(KeySeparator,
    Convert.ToBase64String(fieldDefinitionId.ToByteArray()).TrimEnd('='),
    Helper.Normalize(value));
}

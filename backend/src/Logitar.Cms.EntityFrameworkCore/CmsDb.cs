using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class CmsDb
{
  public static class ContentTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentTypes));

    public static readonly ColumnId AggregateId = new(nameof(ContentTypeEntity.AggregateId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentTypeEntity.ContentTypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentTypeEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ContentTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ContentTypeEntity.DisplayName), Table);
    public static readonly ColumnId IsInvariant = new(nameof(ContentTypeEntity.IsInvariant), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentTypeEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentTypeEntity.Version), Table);
  }

  public static class FieldTypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.FieldTypes));

    public static readonly ColumnId AggregateId = new(nameof(FieldTypeEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(FieldTypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(FieldTypeEntity.CreatedOn), Table);
    public static readonly ColumnId DataType = new(nameof(FieldTypeEntity.DataType), Table);
    public static readonly ColumnId Description = new(nameof(FieldTypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(FieldTypeEntity.DisplayName), Table);
    public static readonly ColumnId FieldTypeId = new(nameof(FieldTypeEntity.FieldTypeId), Table);
    public static readonly ColumnId Properties = new(nameof(FieldTypeEntity.Properties), Table);
    public static readonly ColumnId UniqueName = new(nameof(FieldTypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldTypeEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(FieldTypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(FieldTypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(FieldTypeEntity.Version), Table);
  }

  public static class Languages
  {
    public static readonly TableId Table = new(nameof(CmsContext.Languages));

    public static readonly ColumnId AggregateId = new(nameof(LanguageEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
    public static readonly ColumnId IsDefault = new(nameof(LanguageEntity.IsDefault), Table);
    public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
    public static readonly ColumnId Locale = new(nameof(LanguageEntity.Locale), Table);
    public static readonly ColumnId LocaleNormalized = new(nameof(LanguageEntity.LocaleNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);
  }

  public static string Normalize(string value) => value.Trim().ToUpper();
}

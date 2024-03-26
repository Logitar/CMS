using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class CmsDb
{
  public static class ContentItems
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentItems));

    public static readonly ColumnId AggregateId = new(nameof(ContentItemEntity.AggregateId), Table);
    public static readonly ColumnId ContentItemId = new(nameof(ContentItemEntity.ContentItemId), Table);
    public static readonly ColumnId ContentTypeId = new(nameof(ContentItemEntity.ContentTypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentItemEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentItemEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentItemEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentItemEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ContentItemEntity.Version), Table);
  }

  public static class ContentLocales
  {
    public static readonly TableId Table = new(nameof(CmsContext.ContentLocales));

    public static readonly ColumnId ContentItemId = new(nameof(ContentLocaleEntity.ContentItemId), Table);
    public static readonly ColumnId ContentLocaleId = new(nameof(ContentLocaleEntity.ContentLocaleId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ContentLocaleEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ContentLocaleEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ContentLocaleEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ContentLocaleEntity.DisplayName), Table);
    public static readonly ColumnId LanguageId = new(nameof(ContentLocaleEntity.LanguageId), Table);
    public static readonly ColumnId UniqueName = new(nameof(ContentLocaleEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocaleEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ContentLocaleEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ContentLocaleEntity.UpdatedOn), Table);
  }

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
}

using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore.CmsDb;

internal static class ContentLocales
{
  public static readonly TableId Table = new(nameof(CmsContext.ContentLocales));

  public static readonly ColumnId ContentId = new(nameof(ContentLocaleEntity.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(ContentLocaleEntity.ContentLocaleId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentLocaleEntity.ContentTypeId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ContentLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId LanguageId = new(nameof(ContentLocaleEntity.LanguageId), Table);
  public static readonly ColumnId UniqueName = new(nameof(ContentLocaleEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocaleEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentLocaleEntity.UpdatedOn), Table);
}

using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore.CmsDb;

internal static class Languages
{
  public static readonly TableId Table = new(nameof(CmsContext.Languages));

  public static readonly ColumnId AggregateId = new(nameof(LanguageEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);

  public static readonly ColumnId DisplayName = new(nameof(LanguageEntity.DisplayName), Table);
  public static readonly ColumnId EnglishName = new(nameof(LanguageEntity.EnglishName), Table);
  public static readonly ColumnId Id = new(nameof(LanguageEntity.Id), Table);
  public static readonly ColumnId IsDefault = new(nameof(LanguageEntity.IsDefault), Table);
  public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
  public static readonly ColumnId Locale = new(nameof(LanguageEntity.Locale), Table);
  public static readonly ColumnId LocaleNormalized = new(nameof(LanguageEntity.LocaleNormalized), Table);
  public static readonly ColumnId NativeName = new(nameof(LanguageEntity.NativeName), Table);
}

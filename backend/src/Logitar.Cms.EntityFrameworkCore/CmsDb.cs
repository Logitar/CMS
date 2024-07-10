using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

public static class CmsDb
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();

  public static class Languages
  {
    public static readonly TableId Table = new(nameof(CmsContext.Languages));

    public static readonly ColumnId AggregateId = new(nameof(LanguageEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(LanguageEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(LanguageEntity.CreatedOn), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(LanguageEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(LanguageEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(LanguageEntity.Version), Table);

    public static readonly ColumnId Code = new(nameof(LanguageEntity.Code), Table);
    public static readonly ColumnId CodeNormalized = new(nameof(LanguageEntity.CodeNormalized), Table);
    public static readonly ColumnId DisplayName = new(nameof(LanguageEntity.DisplayName), Table);
    public static readonly ColumnId EnglishName = new(nameof(LanguageEntity.EnglishName), Table);
    public static readonly ColumnId IsDefault = new(nameof(LanguageEntity.IsDefault), Table);
    public static readonly ColumnId LanguageId = new(nameof(LanguageEntity.LanguageId), Table);
    public static readonly ColumnId LCID = new(nameof(LanguageEntity.LCID), Table);
    public static readonly ColumnId NativeName = new(nameof(LanguageEntity.NativeName), Table);
    public static readonly ColumnId UniqueId = new(nameof(LanguageEntity.UniqueId), Table);
  }
}

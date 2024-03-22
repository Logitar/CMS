using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class CmsDb
{
  public static class Archetypes
  {
    public static readonly TableId Table = new(nameof(CmsContext.Archetypes));

    public static readonly ColumnId AggregateId = new(nameof(ArchetypeEntity.AggregateId), Table);
    public static readonly ColumnId ArchetypeId = new(nameof(ArchetypeEntity.ArchetypeId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(ArchetypeEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(ArchetypeEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(ArchetypeEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(ArchetypeEntity.DisplayName), Table);
    public static readonly ColumnId IsInvariant = new(nameof(ArchetypeEntity.IsInvariant), Table);
    public static readonly ColumnId UniqueName = new(nameof(ArchetypeEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(ArchetypeEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(ArchetypeEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(ArchetypeEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(ArchetypeEntity.Version), Table);
  }
}

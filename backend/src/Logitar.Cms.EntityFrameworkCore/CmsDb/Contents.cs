using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore.CmsDb;

internal static class Contents
{
  public static readonly TableId Table = new(nameof(CmsContext.Contents));

  public static readonly ColumnId AggregateId = new(nameof(ContentEntity.AggregateId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ContentEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ContentEntity.Version), Table);

  public static readonly ColumnId ContentId = new(nameof(ContentEntity.ContentId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentEntity.ContentTypeId), Table);
  public static readonly ColumnId Id = new(nameof(ContentEntity.Id), Table);
}

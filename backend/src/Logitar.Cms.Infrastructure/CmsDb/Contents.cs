using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class Contents
{
  public static readonly TableId Table = new(CmsContext.Schema, nameof(CmsContext.Contents), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ContentEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ContentEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ContentEntity.Version), Table);

  public static readonly ColumnId ContentId = new(nameof(ContentEntity.ContentId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentEntity.ContentTypeId), Table);
  public static readonly ColumnId Id = new(nameof(ContentEntity.Id), Table);
}

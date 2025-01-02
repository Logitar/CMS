using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class ContentTypes
{
  public static readonly TableId Table = new(nameof(CmsContext.ContentTypes));

  public static readonly ColumnId CreatedBy = new(nameof(ContentTypeEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentTypeEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ContentTypeEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentTypeEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentTypeEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ContentTypeEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(ContentTypeEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ContentTypeEntity.DisplayName), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentTypeEntity.ContentTypeId), Table);
  public static readonly ColumnId Id = new(nameof(ContentTypeEntity.Id), Table);
  public static readonly ColumnId IsInvariant = new(nameof(ContentTypeEntity.IsInvariant), Table);
  public static readonly ColumnId UniqueName = new(nameof(ContentTypeEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentTypeEntity.UniqueNameNormalized), Table);
}

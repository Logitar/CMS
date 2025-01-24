using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class PublishedContents
{
  public static readonly TableId Table = new(nameof(CmsContext.PublishedContents));

  public static readonly ColumnId CreatedBy = new(nameof(PublishedContentEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(PublishedContentEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(PublishedContentEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(PublishedContentEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(PublishedContentEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(PublishedContentEntity.Version), Table);

  public static readonly ColumnId ContentLocaleId = new(nameof(PublishedContentEntity.ContentLocaleId), Table);
  // TODO(fpion): columns
}

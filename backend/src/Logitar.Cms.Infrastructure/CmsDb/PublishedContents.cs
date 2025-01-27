﻿using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure.CmsDb;

public static class PublishedContents
{
  public static readonly TableId Table = new(nameof(CmsContext.PublishedContents));

  public static readonly ColumnId ContentId = new(nameof(PublishedContentEntity.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(PublishedContentEntity.ContentLocaleId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(PublishedContentEntity.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(PublishedContentEntity.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(PublishedContentEntity.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(PublishedContentEntity.ContentUid), Table);
  public static readonly ColumnId Description = new(nameof(PublishedContentEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(PublishedContentEntity.DisplayName), Table);
  public static readonly ColumnId FieldValues = new(nameof(PublishedContentEntity.FieldValues), Table);
  public static readonly ColumnId LanguageCode = new(nameof(PublishedContentEntity.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(PublishedContentEntity.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(PublishedContentEntity.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(PublishedContentEntity.LanguageUid), Table);
  public static readonly ColumnId PublishedBy = new(nameof(PublishedContentEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(PublishedContentEntity.PublishedOn), Table);
  public static readonly ColumnId Revision = new(nameof(PublishedContentEntity.Revision), Table);
  public static readonly ColumnId UniqueName = new(nameof(PublishedContentEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(PublishedContentEntity.UniqueNameNormalized), Table);
}
